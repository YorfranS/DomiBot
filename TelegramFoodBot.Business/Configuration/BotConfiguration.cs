using System;
using System.Configuration;

namespace TelegramFoodBot.Business.Configuration
{
    /// <summary>
    /// Configuración centralizada para el bot de Telegram
    /// </summary>
    public static class BotConfiguration
    {
        /// <summary>
        /// Token del bot de Telegram obtenido desde configuración
        /// </summary>
        public static string BotToken
        {
            get
            {
                // Primero intentar obtener desde variables de entorno
                var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
                
                if (!string.IsNullOrEmpty(token))
                    return token;

                // Si no existe en variables de entorno, usar configuración por defecto
                // En producción se debe configurar como variable de entorno
                return "8153338706:AAGbRIiiTkfflyqiCjSGoIN3e-aaIOfvBbI";
            }
        }

        /// <summary>
        /// Timeout por defecto para operaciones HTTP
        /// </summary>
        public static TimeSpan DefaultTimeout => TimeSpan.FromSeconds(30);

        /// <summary>
        /// Timeout para callbacks
        /// </summary>
        public static TimeSpan CallbackTimeout => TimeSpan.FromSeconds(10);

        /// <summary>
        /// Configuración de logging
        /// </summary>
        public static bool EnableLogging => true;

        /// <summary>
        /// Configuración para habilitar diagnósticos avanzados
        /// </summary>
        public static bool EnableDiagnostics => false;

        /// <summary>
        /// Configuración de la base de datos
        /// </summary>
        public static string DatabaseConnectionString
        {
            get
            {
                var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
                return !string.IsNullOrEmpty(connectionString) 
                    ? connectionString 
                    : AppConstants.DEFAULT_CONNECTION_STRING;
            }
        }

        /// <summary>
        /// Configuración del entorno de ejecución
        /// </summary>
        public static bool IsProduction => 
            Environment.GetEnvironmentVariable("ENVIRONMENT")?.ToLower() == "production";

        /// <summary>
        /// Configuración de logs detallados
        /// </summary>
        public static bool EnableDetailedLogging => 
            Environment.GetEnvironmentVariable("DETAILED_LOGGING")?.ToLower() == "true" || !IsProduction;

        /// <summary>
        /// Configuración de reintentos para operaciones de red
        /// </summary>
        public static int MaxRetryAttempts => 
            int.TryParse(Environment.GetEnvironmentVariable("MAX_RETRY_ATTEMPTS"), out int attempts) 
                ? attempts : 3;

        /// <summary>
        /// Configuración de timeout para operaciones de base de datos
        /// </summary>
        public static TimeSpan DatabaseTimeout => 
            TimeSpan.FromSeconds(
                int.TryParse(Environment.GetEnvironmentVariable("DB_TIMEOUT_SECONDS"), out int seconds) 
                    ? seconds : AppConstants.DEFAULT_CONNECTION_TIMEOUT);
    }
}
