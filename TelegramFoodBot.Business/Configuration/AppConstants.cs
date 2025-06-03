using System;

namespace TelegramFoodBot.Business.Configuration
{
    /// <summary>
    /// Constantes centralizadas de la aplicación
    /// </summary>
    public static class AppConstants
    {
        #region Database Constants
        public const string DEFAULT_CONNECTION_STRING = "Server=MIGUEL;Database=TelegramPedidosDB;Integrated Security=true;";
        public const int DEFAULT_CONNECTION_TIMEOUT = 30;
        #endregion

        #region Bot Messages
        public const string WELCOME_MESSAGE = "¡Bienvenido a DomiBot! 🍕\n\nSoy tu asistente virtual para pedidos de comida.";
        public const string ERROR_MESSAGE = "❌ Ha ocurrido un error. Por favor, intenta nuevamente.";
        public const string ORDER_CONFIRMED_MESSAGE = "✅ Tu pedido ha sido confirmado";
        public const string PAYMENT_PENDING_MESSAGE = "⏳ Tu pago está siendo procesado...";
        #endregion

        #region Bot Commands
        public const string START_COMMAND = "/start";
        public const string HELP_COMMAND = "/help";
        public const string MENU_COMMAND = "/menu";
        public const string PEDIDO_COMMAND = "/pedido";
        public const string RESERVA_COMMAND = "/reserva";
        #endregion

        #region Callback Data
        public const string CONFIRM_ORDER_CALLBACK = "confirm_order";
        public const string CANCEL_ORDER_CALLBACK = "cancel_order";
        public const string SELECT_PAYMENT_CALLBACK = "select_payment";
        public const string CONTACT_ADVISOR_CALLBACK = "contact_advisor";
        #endregion

        #region UI Constants
        public const int MAX_PRODUCTS_PER_PAGE = 10;
        public const int BUTTON_HEIGHT = 35;
        public const int BUTTON_WIDTH = 120;
        public const int BUTTON_MARGIN = 10;
        #endregion

        #region Admin Credentials
        public const string DEFAULT_ADMIN_USER = "admin";
        public const string DEFAULT_ADMIN_PASSWORD = "1234";
        #endregion

        #region Validation
        public const int MIN_PHONE_LENGTH = 10;
        public const int MAX_PHONE_LENGTH = 15;
        public const int MIN_ADDRESS_LENGTH = 10;
        public const int MAX_ORDER_QUANTITY = 50;
        #endregion

        #region Business Hours
        public static readonly TimeSpan OPENING_TIME = new TimeSpan(9, 0, 0);  // 9:00 AM
        public static readonly TimeSpan CLOSING_TIME = new TimeSpan(22, 0, 0); // 10:00 PM
        #endregion
    }
}
