using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;
using TelegramFoodBot.Business.Commands;
using TelegramFoodBot.Business.Commands.Handlers;
using TelegramFoodBot.Business.Services;
using TelegramFoodBot.Business.Configuration;
using TelegramFoodBot.Entities.Models;
using TelegramMessage = Telegram.Bot.Types.Message;
using AppMessage = TelegramFoodBot.Entities.Models.Message;
using AppUser = TelegramFoodBot.Entities.Models.User;

namespace TelegramFoodBot.Business.Services
{    /// <summary>
    /// Versión mejorada de TelegramService con diagnósticos avanzados
    /// para resolver problemas de conectividad específicos de PC
    /// </summary>
    public class EnhancedTelegramService : ITelegramService
    {
        private static EnhancedTelegramService? _instance;
        private static readonly object _lock = new object();
        
        private readonly TelegramBotClient _botClient;
        private CancellationTokenSource? _cts;
        private readonly EnhancedCallbackHandlerService _callbackService;

        // Comandos especializados
        private readonly ComandoRegistro _comandoRegistro;
        private readonly ComandoPedido _comandoPedido;
        private readonly ComandoReserva _comandoReserva;

        public event EventHandler<MessageEventArgs>? MessageReceived;
        public event EventHandler<PedidoEventArgs>? PedidoCreado;
        public event EventHandler<ReservaEventArgs>? ReservaCreada;

        /// <summary>
        /// Dispara el evento PedidoCreado
        /// </summary>
        public void OnPedidoCreado(Pedido pedido)
        {
            PedidoCreado?.Invoke(this, new PedidoEventArgs(pedido));
        }

        /// <summary>
        /// Dispara el evento ReservaCreada
        /// </summary>
        public void OnReservaCreada(Reserva reserva)
        {
            ReservaCreada?.Invoke(this, new ReservaEventArgs(reserva));
        }

        // Configuración de timeouts y diagnósticos
        private static readonly TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan CALLBACK_TIMEOUT = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Obtiene la instancia singleton del servicio
        /// </summary>
        public static EnhancedTelegramService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EnhancedTelegramService();
                        }
                    }
                }
                return _instance;
            }
        }        private EnhancedTelegramService()
        {
            // Configurar HttpClient con timeouts personalizados
            var httpClient = CreateConfiguredHttpClient();
            _botClient = new TelegramBotClient(BotConfiguration.BotToken, httpClient);

            _comandoRegistro = new ComandoRegistro(_botClient, OnMessageReceived);
            _comandoPedido = new ComandoPedido(_botClient, OnMessageReceived);
            _comandoReserva = new ComandoReserva(_botClient, OnMessageReceived);

            _callbackService = new EnhancedCallbackHandlerService(_botClient);
            RegisterCallbackHandlers();

            // Log de inicio con diagnósticos
            LogSystemInfo();
        }

        /// <summary>
        /// Crea un HttpClient configurado específicamente para resolver problemas de conectividad
        /// </summary>
        private static HttpClient CreateConfiguredHttpClient()
        {
            var handler = new HttpClientHandler()
            {
                // Configuraciones para resolver problemas de proxy/firewall
                UseProxy = false, // Deshabilitar proxy automático
                UseCookies = false,
                UseDefaultCredentials = false
            };

            var httpClient = new HttpClient(handler)
            {
                Timeout = DEFAULT_TIMEOUT
            };

            // Headers para evitar bloqueos de antivirus/firewall
            httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "TelegramBot/1.0 (compatible; Windows; .NET)");
            
            Console.WriteLine($"[INFO] HttpClient configurado con timeout: {DEFAULT_TIMEOUT.TotalSeconds}s");
            return httpClient;
        }

        /// <summary>
        /// Log información del sistema para diagnóstico
        /// </summary>
        private static void LogSystemInfo()
        {
            Console.WriteLine("=== DIAGNÓSTICO DEL SISTEMA ===");
            Console.WriteLine($"OS: {Environment.OSVersion}");
            Console.WriteLine($"Machine: {Environment.MachineName}");
            Console.WriteLine($"User: {Environment.UserName}");
            Console.WriteLine($"CLR Version: {Environment.Version}");
            Console.WriteLine($"Working Directory: {Environment.CurrentDirectory}");
            Console.WriteLine("===============================");
        }        /// <summary>
        /// Registra todos los handlers de callbacks siguiendo el principio Open/Closed
        /// </summary>
        private void RegisterCallbackHandlers()
        {
            _callbackService.RegisterHandler(new MainCommandCallbackHandler(_comandoRegistro, _comandoPedido, _comandoReserva));
            _callbackService.RegisterHandler(new PedidoCallbackHandler(_comandoPedido));
            _callbackService.RegisterHandler(new ProductoCallbackHandler(_comandoPedido));
            _callbackService.RegisterHandler(new PagoCallbackHandler(_comandoPedido));
            _callbackService.RegisterHandler(new AdminPagoCallbackHandler());
            _callbackService.RegisterHandler(new ReservaCallbackHandler(_comandoReserva));
            _callbackService.RegisterHandler(new NavigationCallbackHandler(_botClient));
            _callbackService.RegisterHandler(new AsesorCallbackHandler()); // Handler para contacto con asesor
        }

        public void StartBot()
        {
            _cts = new CancellationTokenSource();
            try
            {
                var receiverOptions = new Telegram.Bot.Polling.ReceiverOptions()
                {
                    AllowedUpdates = new[]
                    {
                        Telegram.Bot.Types.Enums.UpdateType.Message,
                        Telegram.Bot.Types.Enums.UpdateType.CallbackQuery
                    },
                    ThrowPendingUpdates = true // Limpiar updates pendientes
                };

                _botClient.StartReceiving(
                    HandleUpdateAsync, 
                    HandleErrorAsync, 
                    receiverOptions, 
                    _cts.Token
                );
                
                Console.WriteLine($"[SUCCESS] Bot iniciado correctamente en {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                
                // Test de conectividad
                _ = Task.Run(TestConnectivity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error al iniciar el bot: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Test de conectividad al iniciar el bot
        /// </summary>
        private async Task TestConnectivity()
        {
            try
            {
                Console.WriteLine("[TEST] Probando conectividad con Telegram API...");
                var me = await _botClient.GetMeAsync();
                Console.WriteLine($"[SUCCESS] Conectado como: {me.FirstName} (@{me.Username})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Fallo en test de conectividad: {ex.Message}");
                Console.WriteLine("[SUGGESTION] Verifica firewall, antivirus o configuración de proxy");
            }
        }

        public void StopBot()
        {
            _cts?.Cancel();
            
            // Liberar recursos del servicio de callbacks
            _callbackService.Dispose();
            
            Console.WriteLine($"[INFO] Bot detenido y recursos liberados en {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        public void SendMessage(long clientId, string text)
        {
            _ = SendMessageAsync(clientId, text);
        }

        public async Task SendMessageAsync(long clientId, string text)
        {
            try
            {
                Console.WriteLine($"[SEND] Enviando mensaje a {clientId}: {text.Substring(0, Math.Min(50, text.Length))}...");
                
                using var cts = new CancellationTokenSource(DEFAULT_TIMEOUT);
                await _botClient.SendTextMessageAsync(
                    chatId: clientId, 
                    text: text,
                    cancellationToken: cts.Token
                );
                
                Console.WriteLine($"[SUCCESS] Mensaje enviado a {clientId}");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"[ERROR] Timeout al enviar mensaje a {clientId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error al enviar mensaje a {clientId}: {ex.Message}");
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"[RECEIVE] Update recibido: {update.Type}");

                // 1. Manejar botones inline (CallbackQuery) con timeout específico
                if (update.CallbackQuery != null)
                {
                    Console.WriteLine($"[CALLBACK] Procesando callback: {update.CallbackQuery.Data}");
                    
                    // Usar timeout específico para callbacks
                    using var callbackCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    callbackCts.CancelAfter(CALLBACK_TIMEOUT);
                    
                    await ProcessCallbackWithDiagnostics(update.CallbackQuery, callbackCts.Token);
                    return;
                }
                
                // 2. Procesar mensajes normales de texto
                var telegramMessage = update.Message;
                if (telegramMessage?.From == null) return;

                Console.WriteLine($"[MESSAGE] De: {telegramMessage.From.FirstName} ({telegramMessage.From.Id})");
                Console.WriteLine($"[MESSAGE] Texto: {telegramMessage.Text}");

                var appUser = new AppUser
                {
                    Id = telegramMessage.From.Id,
                    FirstName = telegramMessage.From.FirstName ?? "",
                    LastName = telegramMessage.From.LastName ?? "",
                    Username = telegramMessage.From.Username ?? ""
                };

                // Imagen
                if (telegramMessage.Photo != null && telegramMessage.Photo.Length > 0)
                {
                    Console.WriteLine("[PHOTO] Procesando imagen...");
                    var largestPhoto = telegramMessage.Photo[^1];
                    var file = await _botClient.GetFileAsync(largestPhoto.FileId, cancellationToken);

                    if (file.FilePath != null)
                    {
                        using var ms = new MemoryStream();
                        await _botClient.DownloadFileAsync(file.FilePath, ms, cancellationToken);
                        string base64 = Convert.ToBase64String(ms.ToArray());

                        var photoMessage = new AppMessage
                        {
                            Id = telegramMessage.MessageId,
                            Timestamp = telegramMessage.Date.ToLocalTime(),
                            ClientId = telegramMessage.From.Id,
                            From = appUser,
                            IsFromAdmin = false,
                            PhotoBase64 = base64
                        };

                        OnMessageReceived(photoMessage);
                    }
                    return;
                }

                if (!string.IsNullOrWhiteSpace(telegramMessage.Text))
                {
                    string texto = telegramMessage.Text.Trim();
                    string textoLower = texto.ToLower();

                    // Registrar en historial (entrada)
                    OnMessageReceived(new AppMessage
                    {
                        Id = telegramMessage.MessageId,
                        Text = texto,
                        Timestamp = telegramMessage.Date.ToLocalTime(),
                        IsFromAdmin = false,
                        ClientId = telegramMessage.From.Id,
                        From = appUser
                    });

                    // Comando inicial
                    if (textoLower == "/start")
                    {
                        Console.WriteLine("[COMMAND] Ejecutando /start");
                        
                        // Crear teclado inline con botones interactivos
                        var tecladoInline = CrearTecladoInline(
                            ("🛍️ Hacer Pedido", "pedir"),
                            ("📅 Reservar Mesa", "reservar")
                        );

                        await Responder("🎉 ¡Bienvenido a DomiBot! 🍔📱\n" +
                            "Soy tu asistente para pedir comida y reservar tu mesa de forma rápida y sencilla 😄.\n" +
                            "¿Qué quieres hacer hoy?\n\n" +
                            "👇 *Toca uno de los botones* para comenzar, o escribe uno de los siguientes comandos:\n\n" +
                            "🛍️ /pedir – Explora nuestro menú y haz tu pedido ¡así de fácil!\n" +
                            "📅 /reservar – Reserva tu mesa con seguro incluido 🛡️", 
                            telegramMessage, tecladoInline, cancellationToken);
                        return;
                    }

                    // Delegar comandos
                    if (textoLower == "/registrarse" || _comandoRegistro.EnRegistro(telegramMessage.From.Id))
                    {
                        Console.WriteLine("[COMMAND] Ejecutando registro");
                        await _comandoRegistro.Ejecutar(telegramMessage);
                        return;
                    }

                    if (textoLower == "/pedir" || _comandoPedido.EnPedido(telegramMessage.From.Id))
                    {
                        Console.WriteLine("[COMMAND] Ejecutando pedido");
                        await _comandoPedido.Ejecutar(telegramMessage);
                        return;
                    }

                    if (textoLower == "/reservar" || _comandoReserva.EnReserva(telegramMessage.From.Id))
                    {
                        Console.WriteLine("[COMMAND] Ejecutando reserva");
                        await _comandoReserva.Ejecutar(telegramMessage);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en HandleUpdateAsync: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            }
        }        /// <summary>
        /// Procesa callbacks con diagnósticos detallados y prevención de duplicados
        /// </summary>
        private async Task ProcessCallbackWithDiagnostics(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var callbackData = callbackQuery.Data ?? "null";
            var userId = callbackQuery.From?.Id ?? 0;
            var userName = callbackQuery.From?.FirstName ?? "Unknown";
            
            try
            {
                Console.WriteLine($"[CALLBACK_START] Iniciando callback: {callbackData} para usuario {userName} ({userId}) a las {DateTime.Now:HH:mm:ss.fff}");
                Console.WriteLine($"[CALLBACK_INFO] Chat ID: {callbackQuery.Message?.Chat?.Id}, Message ID: {callbackQuery.Message?.MessageId}");
                
                // Procesar el callback (la validación de duplicados está dentro de ProcessCallback)
                await _callbackService.ProcessCallback(callbackQuery, this);
                
                stopwatch.Stop();
                Console.WriteLine($"[CALLBACK_SUCCESS] Callback {callbackData} completado en {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (TaskCanceledException)
            {
                stopwatch.Stop();
                Console.WriteLine($"[CALLBACK_TIMEOUT] Callback {callbackData} cancelado por timeout después de {stopwatch.ElapsedMilliseconds}ms");
                
                // Intentar responder el callback aunque haya timeout
                try
                {
                    await _botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        "⏱️ El servidor está tardando más de lo normal. Intenta de nuevo.",
                        showAlert: true,
                        cancellationToken: CancellationToken.None
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] No se pudo responder callback después de timeout: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"[CALLBACK_ERROR] Error en callback {callbackData} después de {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
                Console.WriteLine($"[ERROR_DETAIL] {ex.GetType().Name}: {ex.StackTrace?.Substring(0, Math.Min(500, ex.StackTrace?.Length ?? 0))}...");
                
                // Intentar responder con error
                try
                {
                    await _botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        "❌ Error interno del servidor. Intenta de nuevo.",
                        showAlert: true,
                        cancellationToken: CancellationToken.None
                    );
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"[ERROR] No se pudo responder callback con error: {innerEx.Message}");
                }
            }
        }

        private async Task Responder(string texto, TelegramMessage mensaje, 
            Telegram.Bot.Types.ReplyMarkups.IReplyMarkup? replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(DEFAULT_TIMEOUT);
                
                await _botClient.SendTextMessageAsync(
                    chatId: mensaje.Chat.Id, 
                    text: texto,
                    replyMarkup: replyMarkup,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: cts.Token
                );
                
                OnMessageReceived(new AppMessage
                {
                    Text = texto,
                    Timestamp = DateTime.Now,
                    IsFromAdmin = true,
                    ClientId = mensaje.From?.Id ?? 0
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en Responder: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Crea un teclado inline con botones para interactuar con el bot
        /// </summary>
        private Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup CrearTecladoInline(params (string texto, string callback)[] botones)
        {
            var filas = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
            
            foreach (var (texto, callback) in botones)
            {
                filas.Add(new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(texto, callback) });
            }
            
            return new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(filas);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[ERROR] Error del bot: {exception.GetType().Name}: {exception.Message}");
            
            if (exception.InnerException != null)
            {
                Console.WriteLine($"[ERROR] Inner exception: {exception.InnerException.Message}");
            }
            
            // Log específico para errores de red/timeout
            if (exception is HttpRequestException || exception is TaskCanceledException)
            {
                Console.WriteLine("[NETWORK_ERROR] Posibles causas:");
                Console.WriteLine("- Firewall o antivirus bloqueando conexiones");
                Console.WriteLine("- Proxy corporativo");
                Console.WriteLine("- Configuración DNS");
                Console.WriteLine("- Throttling de ISP");
                Console.WriteLine("Intenta ejecutar el bot desde una red diferente para confirmar");
            }
            
            return Task.CompletedTask;
        }

        protected virtual void OnMessageReceived(AppMessage message)
        {
            MessageReceived?.Invoke(this, new MessageEventArgs(message));
        }
        
        /// <summary>
        /// Notifica a los administradores sobre un pago pendiente de confirmación
        /// </summary>
        public async Task NotificarPagoPendiente(string pedidoId, long clienteId, string productos, string direccion, decimal total)
        {
            var adminIds = new long[] { 1066516207 }; // IDs de administradores
            
            string mensaje = $"🔔 *PAGO PENDIENTE DE CONFIRMACIÓN*\n\n" +
                           $"📋 Pedido: #{pedidoId}\n" +
                           $"👤 Cliente: {clienteId}\n" +
                           $"📍 Dirección: {direccion}\n" +
                           $"🍔 Productos: {productos}\n" +
                           $"💰 Total: ${total:F2}\n" +
                           $"💳 Método: Transferencia\n\n" +
                           $"Por favor, confirma o rechaza el pago:";
            
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Confirmar", $"confirmar_pago_{pedidoId}"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ Rechazar", $"rechazar_pago_{pedidoId}")
                }
            });
            
            foreach (var adminId in adminIds)
            {
                try
                {
                    Console.WriteLine($"[ADMIN_NOTIFY] Notificando admin {adminId} sobre pedido {pedidoId}");
                    
                    using var cts = new CancellationTokenSource(DEFAULT_TIMEOUT);
                    await _botClient.SendTextMessageAsync(
                        chatId: adminId,
                        text: mensaje,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: keyboard,
                        cancellationToken: cts.Token
                    );
                    
                    Console.WriteLine($"[SUCCESS] Admin {adminId} notificado");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error al notificar admin {adminId}: {ex.Message}");
                }
            }
        }
    }
}
