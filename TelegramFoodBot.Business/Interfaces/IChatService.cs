using System.Collections.Generic;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Business.Interfaces
{
    public interface IChatService
    {
        List<Client> GetAllClients();
        List<Message> GetClientMessages(long clientId);
        void AddClient(Client client);
        void SaveMessage(Message message);
    }
}
