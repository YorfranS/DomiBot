using System;
using System.Collections.Generic;
using System.Linq;
using TelegramFoodBot.Business.Interfaces;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Business.Services
{
    public class ChatService
    {
        private readonly ClienteRepository _clienteRepo = new ClienteRepository();
        private readonly MessageRepository _messageRepo = new MessageRepository();

        // Obtener todos los clientes registrados desde base
        public List<Client> GetAllClients()
        {
            return _clienteRepo.ObtenerClientes();
        }

        // Obtener clientes ordenados por el último mensaje recibido (más reciente primero)
        public List<Client> GetClientsOrderedByLastMessage()
        {
            var clientes = GetAllClients();
            var ultimos = _messageRepo.ObtenerUltimosMensajesPorCliente();
            var dictUltimos = ultimos.ToDictionary(x => x.ClientId, x => x.LastMessage);
            var clientesOrdenados = clientes
                .OrderByDescending(c => dictUltimos.ContainsKey(c.Id) ? dictUltimos[c.Id] : DateTime.MinValue)
                .ToList();
            return clientesOrdenados;
        }

        // Obtener clientes ordenados con prioridad a los que tienen mensajes nuevos
        public List<Client> GetClientsOrderedByUnreadMessages()
        {
            var clientes = GetAllClients();
            var clientesConMensajesNoLeidos = _messageRepo.ObtenerClientesConMensajesNoLeidos();

            var clientesOrdenados = clientes
                .OrderByDescending(c => clientesConMensajesNoLeidos.Contains(c.Id) ? 1 : 0)
                .ThenBy(c => c.Name)
                .ToList();

            return clientesOrdenados;
        }

        // Agregar cliente a base si no existe
        public void AddClient(Client client)
        {
            if (!_clienteRepo.ExisteCliente(client.Id))
            {
                _clienteRepo.AgregarCliente(client);
            }
        }

        // Guardar mensaje en base de datos y marcar mensajes nuevos si es de usuario
        public void SaveMessage(Message msg)
        {
            _messageRepo.GuardarMensaje(msg);
        }

        // Obtener mensajes de un cliente desde la base de datos
        public List<Message> GetClientMessages(long clientId)
        {
            return _messageRepo.ObtenerMensajesCliente(clientId);
        }

        // Marcar mensajes como leídos en la base de datos
        public void MarkMessagesAsRead(long clientId)
        {
            _messageRepo.MarcarMensajesComoLeidos(clientId);
        }

        // Verificar si tiene mensajes no leídos desde la base de datos
        public bool HasUnreadMessages(long clientId)
        {
            return _messageRepo.TieneMensajesNoLeidos(clientId);
        }
    }
}
