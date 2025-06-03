using System;

namespace TelegramFoodBot.Entities.Models
{
    public class Message
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsFromAdmin { get; set; }
        public long ClientId { get; set; }

        public string PhotoBase64 { get; set; }

        public User From { get; set; }
    }
}


