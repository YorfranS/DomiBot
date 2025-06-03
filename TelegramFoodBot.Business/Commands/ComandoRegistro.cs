using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;
using AppMessage = TelegramFoodBot.Entities.Models.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace TelegramFoodBot.Business.Commands
{
    public class ComandoRegistro
    {
        private readonly TelegramBotClient _bot;
        private readonly Action<AppMessage> _onMessage;
        private readonly Dictionary<long, Client> _clientesEnRegistro = new();

        public ComandoRegistro(TelegramBotClient bot, Action<AppMessage> onMessage)
        {
            _bot = bot;
            _onMessage = onMessage;
        }

        public bool EnRegistro(long clientId) => _clientesEnRegistro.ContainsKey(clientId);

        public async Task Ejecutar(TelegramMessage message)
        {
            long clientId = message.From.Id;
            string texto = message.Text?.Trim() ?? "";

            if (!_clientesEnRegistro.ContainsKey(clientId))
            {
                _clientesEnRegistro[clientId] = new Client
                {
                    Id = clientId,
                    Name = message.From.FirstName,
                    Username = message.From.Username
                };

                await Responder("📱 Para completar tu registro, solo tienes que enviarnos tu número de teléfono. ¡Así de fácil y rápido! 🚀", message);
                return;
            }

            if (texto.Length < 7 || !texto.All(char.IsDigit))
            {
                await Responder("⚠️ **Oops!** El número que ingresaste no es válido.\nPor favor, verifica y vuelve a intentarlo. ¡Estoy aquí para ayudarte! 😊", message);
                return;
            }

            var cliente = _clientesEnRegistro[clientId];
            cliente.Phone = texto;            new ClienteRepository().AgregarCliente(cliente);
            _clientesEnRegistro.Remove(clientId);

            // Crear teclado con opciones tras completar el registro
            var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[] 
                { 
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🛍️ Hacer Pedido", "pedir"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Reservar Mesa", "reservar") 
                }
            });

            await Responder("🎉 ¡Registro completado con éxito! Ahora puedes comenzar a usar nuestros servicios.\n\n*¿Qué te gustaría hacer?*", message, teclado);
        }        private async Task Responder(string texto, TelegramMessage mensaje, Telegram.Bot.Types.ReplyMarkups.IReplyMarkup replyMarkup = null)
        {
            await _bot.SendTextMessageAsync(
                chatId: mensaje.Chat.Id, 
                text: texto, 
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: replyMarkup
            );
            
            _onMessage?.Invoke(new AppMessage
            {
                Text = texto,
                Timestamp = DateTime.Now,
                IsFromAdmin = true,
                ClientId = mensaje.From.Id
            });
        }
    }
}
