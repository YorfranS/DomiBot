using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;

namespace TelegramFoodBot.Business.Services
{
    /// <summary>
    /// Versión mejorada del CallbackHandlerService con diagnósticos y manejo robusto de timeouts
    /// Resuelve problemas de botones que se quedan en loading en PCs específicos
    /// </summary>
    public class EnhancedCallbackHandlerService : IDisposable
    {
        private readonly List<ICallbackHandler> _handlers;
        private readonly TelegramBotClient _botClient;
        // Almacena los IDs de callbacks ya procesadas para evitar duplicados
        private readonly HashSet<string> _processedCallbacks = new HashSet<string>();
        // Almacena el timestamp de cada callback para limpiar las antiguas
        private readonly Dictionary<string, DateTime> _callbackTimestamps = new Dictionary<string, DateTime>();
        // Tiempo para mantener los IDs en memoria (30 minutos)
        private static readonly TimeSpan CALLBACK_EXPIRATION = TimeSpan.FromMinutes(30);
        // Timer para limpiar IDs antiguos y evitar crecimiento indefinido de la memoria
        private readonly Timer _cleanupTimer;
        
        // Configuración de timeouts
        private static readonly TimeSpan ANSWER_CALLBACK_TIMEOUT = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan HANDLER_TIMEOUT = TimeSpan.FromSeconds(15);

        public EnhancedCallbackHandlerService(TelegramBotClient botClient)
        {
            _botClient = botClient;
            _handlers = new List<ICallbackHandler>();
            // Inicializa el timer para limpiar callbacks antiguos cada 10 minutos
            _cleanupTimer = new Timer(CleanupProcessedCallbacks, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
            Console.WriteLine("[INIT] EnhancedCallbackHandlerService inicializado con protección anti-duplicados");
        }

        /// <summary>
        /// Registra un nuevo handler (Open/Closed Principle)
        /// </summary>
        public void RegisterHandler(ICallbackHandler handler)
        {
            _handlers.Add(handler);
            Console.WriteLine($"[REGISTER] Handler registrado: {handler.GetType().Name}");
        }        /// <summary>
        /// Procesa un callback con manejo robusto de errores y timeouts
        /// Incluye sistema para prevenir procesamiento duplicado de callbacks
        /// </summary>
        public async Task ProcessCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            var callbackId = callbackQuery.Id;
            var callbackData = callbackQuery.Data;
            var userId = callbackQuery.From.Id;
            var userName = callbackQuery.From.FirstName ?? "Unknown";
            
            Console.WriteLine($"[CALLBACK_RECEIVED] User: {userName} ({userId}) | Data: {callbackData} | ID: {callbackId}");

            // Generar un ID único para detectar duplicados (combina ID de usuario, datos de callback y chat)
            string uniqueCallbackId = $"{userId}_{callbackData}_{callbackQuery.Message?.Chat?.Id}_{callbackQuery.Message?.MessageId}";
            
            // Verificar si es un duplicado - usar lock para thread safety
            lock (_processedCallbacks)
            {
                if (_processedCallbacks.Contains(uniqueCallbackId))
                {
                    Console.WriteLine($"[DUPLICATE] Detectado callback duplicado: {uniqueCallbackId}");
                    // Solo responder al callback para quitar el spinner, pero no procesar la acción nuevamente
                    _ = AnswerCallbackSafe(callbackId, "✓ Solicitud ya procesada");
                    return;
                }

                // Marcar como procesado antes de continuar
                _processedCallbacks.Add(uniqueCallbackId);
                _callbackTimestamps[uniqueCallbackId] = DateTime.Now;
                Console.WriteLine($"[TRACKING] Callback registrado: {uniqueCallbackId}");
            }

            var processStopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {                // Validar datos básicos
                if (string.IsNullOrEmpty(callbackData))
                {
                    Console.WriteLine("[ERROR] Callback recibido sin data");
                    await AnswerCallbackSafe(callbackId, "❌ Datos de callback inválidos");
                    return;
                }

                // Buscar handler apropiado
                var handler = _handlers.FirstOrDefault(h => h.CanHandle(callbackData));
                
                if (handler == null)
                {
                    Console.WriteLine($"[WARNING] No se encontró handler para: {callbackData}");
                    await AnswerCallbackSafe(callbackId, "⚠️ Comando no reconocido");
                    return;
                }

                Console.WriteLine($"[HANDLER_FOUND] Usando: {handler.GetType().Name}");

                // Ejecutar handler con timeout
                var handlerTask = ExecuteHandlerWithTimeout(handler, callbackQuery, telegramService);
                var answerTask = AnswerCallbackSafe(callbackId, null); // Responder inmediatamente para quitar loading

                // Esperar ambas tareas
                await Task.WhenAll(handlerTask, answerTask);

                processStopwatch.Stop();
                Console.WriteLine($"[CALLBACK_SUCCESS] Completado en {processStopwatch.ElapsedMilliseconds}ms");
            }
            catch (TimeoutException)
            {
                processStopwatch.Stop();
                Console.WriteLine($"[CALLBACK_TIMEOUT] Timeout después de {processStopwatch.ElapsedMilliseconds}ms");
                
                await AnswerCallbackSafe(callbackId, 
                    "⏱️ El servidor está tardando más de lo normal. Por favor, intenta de nuevo.", 
                    showAlert: true);
            }
            catch (Exception ex)
            {
                processStopwatch.Stop();
                Console.WriteLine($"[CALLBACK_ERROR] Error después de {processStopwatch.ElapsedMilliseconds}ms: {ex.Message}");
                Console.WriteLine($"[ERROR_DETAIL] {ex.GetType().Name}: {ex.StackTrace}");
                
                await AnswerCallbackSafe(callbackId, 
                    "❌ Hubo un error procesando tu solicitud. Intenta de nuevo.", 
                    showAlert: true);
            }
        }

        /// <summary>
        /// Ejecuta un handler con timeout configurable
        /// </summary>
        private async Task ExecuteHandlerWithTimeout(ICallbackHandler handler, CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            using var cts = new CancellationTokenSource(HANDLER_TIMEOUT);
            
            var handlerStopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                var handlerTask = handler.HandleCallback(callbackQuery, telegramService);
                
                if (await Task.WhenAny(handlerTask, Task.Delay(HANDLER_TIMEOUT, cts.Token)) == handlerTask)
                {
                    await handlerTask; // Re-throw any exceptions
                    handlerStopwatch.Stop();
                    Console.WriteLine($"[HANDLER_SUCCESS] {handler.GetType().Name} completado en {handlerStopwatch.ElapsedMilliseconds}ms");
                }
                else
                {
                    handlerStopwatch.Stop();
                    Console.WriteLine($"[HANDLER_TIMEOUT] {handler.GetType().Name} cancelado después de {handlerStopwatch.ElapsedMilliseconds}ms");
                    throw new TimeoutException($"Handler {handler.GetType().Name} excedió el timeout de {HANDLER_TIMEOUT.TotalSeconds}s");
                }
            }
            catch (Exception ex) when (!(ex is TimeoutException))
            {
                handlerStopwatch.Stop();
                Console.WriteLine($"[HANDLER_ERROR] {handler.GetType().Name} falló después de {handlerStopwatch.ElapsedMilliseconds}ms: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Responde al callback de forma segura con múltiples intentos y timeouts
        /// </summary>
        private async Task AnswerCallbackSafe(string callbackId, string? text = null, bool showAlert = false)
        {
            const int MAX_RETRIES = 3;
            var retryDelay = TimeSpan.FromMilliseconds(500);

            for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
            {
                try
                {
                    using var cts = new CancellationTokenSource(ANSWER_CALLBACK_TIMEOUT);
                    
                    var answerStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    
                    await _botClient.AnswerCallbackQueryAsync(
                        callbackQueryId: callbackId,
                        text: text,
                        showAlert: showAlert,
                        cancellationToken: cts.Token
                    );
                    
                    answerStopwatch.Stop();
                    Console.WriteLine($"[ANSWER_SUCCESS] Callback respondido en {answerStopwatch.ElapsedMilliseconds}ms (intento {attempt})");
                    return; // Éxito, salir del loop
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"[ANSWER_TIMEOUT] Timeout en intento {attempt}/{MAX_RETRIES}");
                    
                    if (attempt == MAX_RETRIES)
                    {
                        Console.WriteLine("[ANSWER_FAILED] Todos los intentos de respuesta fallaron por timeout");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ANSWER_ERROR] Error en intento {attempt}/{MAX_RETRIES}: {ex.Message}");
                    
                    if (attempt == MAX_RETRIES)
                    {
                        Console.WriteLine("[ANSWER_FAILED] Todos los intentos de respuesta fallaron");
                        return;
                    }
                }

                // Esperar antes del siguiente intento
                if (attempt < MAX_RETRIES)
                {
                    Console.WriteLine($"[RETRY] Esperando {retryDelay.TotalMilliseconds}ms antes del siguiente intento...");
                    await Task.Delay(retryDelay);
                    retryDelay = TimeSpan.FromMilliseconds(retryDelay.TotalMilliseconds * 1.5); // Backoff exponencial
                }
            }
        }

        /// <summary>
        /// Limpia los callbacks antiguos para evitar crecimiento indefinido de memoria
        /// </summary>
        private void CleanupProcessedCallbacks(object? state)
        {
            try
            {
                int countBefore = _processedCallbacks.Count;
                DateTime now = DateTime.Now;
                List<string> toRemove = new List<string>();
                
                // Identificar callbacks antiguos para remover
                lock (_processedCallbacks)
                {
                    foreach (var kvp in _callbackTimestamps)
                    {
                        if (now - kvp.Value > CALLBACK_EXPIRATION)
                        {
                            toRemove.Add(kvp.Key);
                        }
                    }
                    
                    // Remover de ambas colecciones
                    foreach (var key in toRemove)
                    {
                        _processedCallbacks.Remove(key);
                        _callbackTimestamps.Remove(key);
                    }
                }
                
                Console.WriteLine($"[CLEANUP] {toRemove.Count} callbacks antiguos eliminados. Total antes: {countBefore}, después: {_processedCallbacks.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error al limpiar callbacks antiguos: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene estadísticas de handlers registrados
        /// </summary>
        public void LogStatistics()
        {
            Console.WriteLine("=== ESTADÍSTICAS DE CALLBACK HANDLERS ===");
            Console.WriteLine($"Handlers registrados: {_handlers.Count}");
            
            foreach (var handler in _handlers)
            {
                Console.WriteLine($"- {handler.GetType().Name}");
            }
            
            Console.WriteLine($"Timeout de handlers: {HANDLER_TIMEOUT.TotalSeconds}s");
            Console.WriteLine($"Timeout de respuesta: {ANSWER_CALLBACK_TIMEOUT.TotalSeconds}s");
            Console.WriteLine($"Callbacks en memoria: {_processedCallbacks.Count}");
            Console.WriteLine("==========================================");
        }

        /// <summary>
        /// Libera los recursos utilizados por el servicio
        /// </summary>
        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }
}
