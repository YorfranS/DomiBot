using System;
using TelegramFoodBot.Business.Services;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Business.Interfaces
{
    public interface ITelegramService
    {
        event EventHandler<MessageEventArgs> MessageReceived;
        void StartBot();
        void StopBot();
        void SendMessage(long clientId, string text);

    }
}
