using System;
using TelegramFoodBot.Business.Configuration;

namespace TelegramFoodBot.Business.Utils
{
    /// <summary>
    /// Sistema de logging centralizado y profesional
    /// </summary>
    public static class Logger
    {
        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Debug
        }

        /// <summary>
        /// Registra un mensaje de información
        /// </summary>
        public static void LogInfo(string message, string context = "")
        {
            Log(LogLevel.Info, message, context);
        }

        /// <summary>
        /// Registra una advertencia
        /// </summary>
        public static void LogWarning(string message, string context = "")
        {
            Log(LogLevel.Warning, message, context);
        }

        /// <summary>
        /// Registra un error
        /// </summary>
        public static void LogError(string message, Exception exception = null, string context = "")
        {
            var fullMessage = exception != null 
                ? $"{message} | Exception: {exception.Message}" 
                : message;
            Log(LogLevel.Error, fullMessage, context);
        }

        /// <summary>
        /// Registra información de depuración
        /// </summary>
        public static void LogDebug(string message, string context = "")
        {
            if (BotConfiguration.EnableDetailedLogging)
            {
                Log(LogLevel.Debug, message, context);
            }
        }

        /// <summary>
        /// Método principal de logging
        /// </summary>
        private static void Log(LogLevel level, string message, string context)
        {
            if (!BotConfiguration.EnableLogging) return;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var levelString = GetLevelIcon(level);
            var contextString = !string.IsNullOrEmpty(context) ? $"[{context}] " : "";
            
            var logMessage = $"{timestamp} {levelString} {contextString}{message}";
            
            Console.WriteLine(logMessage);

            // En una implementación más avanzada, aquí se escribiría a archivo
            // WriteToFile(logMessage);
        }

        /// <summary>
        /// Obtiene el icono/emoji para cada nivel de log
        /// </summary>
        private static string GetLevelIcon(LogLevel level)
        {
            return level switch
            {
                LogLevel.Info => "ℹ️ [INFO]",
                LogLevel.Warning => "⚠️ [WARN]",
                LogLevel.Error => "❌ [ERROR]",
                LogLevel.Debug => "🔍 [DEBUG]",
                _ => "[LOG]"
            };
        }
    }
}
