using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
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
{
    /// <summary>
    /// Servicio principal de Telegram para manejar mensajes y comandos
    /// </summary>
    public class TelegramService : ITelegramService
    {
        private readonly TelegramBotClient _botClient;
        private CancellationTokenSource? _cts;
        private readonly CallbackHandlerService _callbackService;

        // Comandos especializados
        private readonly ComandoRegistro _comandoRegistro;
        private readonly ComandoPedido _comandoPedido;
        private readonly ComandoReserva _comandoReserva;

        public event EventHandler<MessageEventArgs>? MessageReceived;

        public TelegramService()
        {
            _botClient = new TelegramBotClient(BotConfiguration.BotToken);

            _comandoRegistro = new ComandoRegistro(_botClient, OnMessageReceived);
            _comandoPedido = new ComandoPedido(_botClient, OnMessageReceived);
            _comandoReserva = new ComandoReserva(_botClient, OnMessageReceived);

            _callbackService = new CallbackHandlerService(_botClient);
            RegisterCallbackHandlers();
        }

        /// <summary>
        /// Registra todos los handlers de callbacks
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
            _callbackService.RegisterHandler(new AsesorCallbackHandler());
        }

        public void StartBot()
        {
            _cts = new CancellationTokenSource();
            try
            {
                _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, new Telegram.Bot.Polling.ReceiverOptions(), _cts.Token);
                Console.WriteLine("Bot iniciado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al iniciar el bot: {ex.Message}");
            }
        }

        public void StopBot() => _cts?.Cancel();

        public void SendMessage(long clientId, string text)
        {
            try
            {
                _botClient.SendTextMessageAsync(chatId: clientId, text: text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar mensaje: {ex.Message}");
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Manejar callbacks
            if (update.CallbackQuery != null)
            {
                await _callbackService.ProcessCallback(update.CallbackQuery, this);
                return;
            }
            
            // Procesar mensajes de texto
            var telegramMessage = update.Message;
            if (telegramMessage?.From == null) return;

            var appUser = new AppUser
            {
                Id = telegramMessage.From.Id,
                FirstName = telegramMessage.From.FirstName ?? "",
                LastName = telegramMessage.From.LastName ?? "",
                Username = telegramMessage.From.Username ?? ""
            };

            // Procesar imágenes
            if (telegramMessage.Photo != null && telegramMessage.Photo.Length > 0)
            {
                await ProcessPhotoMessage(telegramMessage, appUser);
                return;
            }

            // Procesar mensajes de texto
            if (!string.IsNullOrWhiteSpace(telegramMessage.Text))
            {
                await ProcessTextMessage(telegramMessage, appUser);
            }
        }

        private async Task ProcessPhotoMessage(TelegramMessage telegramMessage, AppUser appUser)
        {
            var largestPhoto = telegramMessage.Photo![^1];
            var file = await _botClient.GetFileAsync(largestPhoto.FileId);

            if (file.FilePath != null)
            {
                using var ms = new MemoryStream();
                await _botClient.DownloadFileAsync(file.FilePath, ms);
                string base64 = Convert.ToBase64String(ms.ToArray());

                var photoMessage = new AppMessage
                {
                    Id = telegramMessage.MessageId,
                    Timestamp = telegramMessage.Date.ToLocalTime(),
                    ClientId = telegramMessage.From!.Id,
                    From = appUser,
                    IsFromAdmin = false,
                    PhotoBase64 = base64
                };

                OnMessageReceived(photoMessage);
            }
        }

        private async Task ProcessTextMessage(TelegramMessage telegramMessage, AppUser appUser)
        {
            string texto = telegramMessage.Text!.Trim();
            string textoLower = texto.ToLower();

            // Registrar mensaje en historial
            OnMessageReceived(new AppMessage
            {
                Id = telegramMessage.MessageId,
                Text = texto,
                Timestamp = telegramMessage.Date.ToLocalTime(),
                IsFromAdmin = false,
                ClientId = telegramMessage.From!.Id,
                From = appUser
            });

            // Procesar comandos
            await ProcessCommand(telegramMessage, textoLower);
        }

        private async Task ProcessCommand(TelegramMessage telegramMessage, string textoLower)
        {
            var userId = telegramMessage.From!.Id;

            // Comando inicial
            if (textoLower == "/start")
            {
                await MostrarProductosEnPromocion(telegramMessage);
                return;
            }

            // Procesar comandos especializados
            if (textoLower == "/registrarse" || _comandoRegistro.EnRegistro(userId))
            {
                await _comandoRegistro.Ejecutar(telegramMessage);
                return;
            }

            if (textoLower == "/pedir" || _comandoPedido.EnPedido(userId))
            {
                await _comandoPedido.Ejecutar(telegramMessage);
                return;
            }

            if (textoLower == "/reservar" || _comandoReserva.EnReserva(userId))
            {
                await _comandoReserva.Ejecutar(telegramMessage);
                return;
            }
        }

        private async Task MostrarProductosEnPromocion(TelegramMessage message)
        {
            try
            {
                var productoService = new ProductoService();
                var productosPromocion = productoService.ObtenerProductosEnPromocion();

                if (productosPromocion.Count == 0)
                {
                    await MostrarMensajeBienvenidaNormal(message);
                    return;
                }

                await MostrarProductosPromocion(message, productosPromocion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mostrar promociones: {ex.Message}");
                await MostrarMensajeBienvenidaNormal(message);
            }
        }

        private async Task MostrarMensajeBienvenidaNormal(TelegramMessage message)
        {
            var teclado = CrearTecladoInline(
                ("🛍️ Hacer Pedido", "pedir"),
                ("📅 Reservar Mesa", "reservar")
            );

            string mensajeBienvenida = "🎉 ¡Bienvenido a DomiBot! 🍔📱\n" +
                                     "Soy tu asistente para pedir comida y reservar tu mesa de forma rápida y sencilla 😄.\n" +
                                     "¿Qué quieres hacer hoy?\n\n" +
                                     "👇 *Toca uno de los botones* para comenzar:\n\n" +
                                     "🛍️ /pedir – Explora nuestro menú y haz tu pedido ¡así de fácil!\n" +
                                     "📅 /reservar – Reserva tu mesa con seguro incluido 🛡️";

            await Responder(mensajeBienvenida, message, teclado);
        }

        private async Task MostrarProductosPromocion(TelegramMessage message, List<Producto> productosPromocion)
        {
            var botones = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();

            string mensajePromocion = "🎉 ¡Bienvenido a DomiBot! 🍔📱\n\n" +
                                    "🔥 *¡PRODUCTOS EN PROMOCIÓN!* 🔥\n" +
                                    "¡No te pierdas estas ofertas especiales!\n\n";

            foreach (var producto in productosPromocion)
            {
                string textoBoton = $"🔥 {producto.Nombre} - ${producto.Precio:N0}";
                botones.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                        textoBoton, 
                        $"promocion_producto_{producto.Id}")
                });
                
                mensajePromocion += $"• {producto.Nombre}: ${producto.Precio:N0}\n";
                if (!string.IsNullOrWhiteSpace(producto.Descripcion))
                {
                    mensajePromocion += $"  {producto.Descripcion}\n";
                }
                mensajePromocion += "\n";
            }

            // Botones de navegación
            botones.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🛍️ Ver Menú Completo", "pedir"),
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Reservar Mesa", "reservar")
            });

            var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botones);
            mensajePromocion += "👇 *Selecciona un producto en promoción o explora más opciones:*";

            await Responder(mensajePromocion, message, teclado);
        }

        private async Task Responder(string texto, TelegramMessage mensaje, Telegram.Bot.Types.ReplyMarkups.IReplyMarkup? replyMarkup = null)
        {
            await _botClient.SendTextMessageAsync(
                chatId: mensaje.Chat.Id, 
                text: texto,
                replyMarkup: replyMarkup,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
            
            OnMessageReceived(new AppMessage
            {
                Text = texto,
                Timestamp = DateTime.Now,
                IsFromAdmin = true,
                ClientId = mensaje.From?.Id ?? 0
            });
        }
        
        /// <summary>
        /// Crea un teclado inline con botones para interactuar con el bot
        /// </summary>
        private Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup CrearTecladoInline(params (string texto, string callback)[] botones)
        {
            var botonesInline = botones.Select(b => 
                new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(b.texto, b.callback) }
            ).ToArray();
            
            return new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botonesInline);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error en el bot de Telegram: {exception.Message}");
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
                    await _botClient.SendTextMessageAsync(
                        chatId: adminId,
                        text: mensaje,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: keyboard
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al notificar admin {adminId}: {ex.Message}");
                }
            }
        }
    }
}
