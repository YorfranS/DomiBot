using System;
using System.Collections.Generic;

namespace TelegramFoodBot.Entities.Models
{
    public class Client
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public DateTime FirstContact { get; set; } = DateTime.Now;
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
