using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost
{
    /// <summary>
    /// Logging API for debugging
    /// </summary>
    public class Logging
    {

        /// <summary>
        /// Send a log message
        /// </summary>
        /// <param name="space">The space where this log came from</param>
        /// <param name="message">The message to log</param>
        /// <param name="logType">The type of log</param>
        public static void Log(string space, string message, LogType logType)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string console = $"{timestamp} @ {logType}: [{space.ToUpper()}] {message}";

            ConsoleColor color = logType switch
            {
                LogType.IMPORTANT => ConsoleColor.Yellow,
                LogType.INFO => ConsoleColor.Gray,    
                LogType.ERROR => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };

            Console.ForegroundColor = color;
            Console.WriteLine(console);
            Console.ResetColor();
        }
    }
    
    /// <summary>
    /// Logging types
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Important information that needs to stand out
        /// Use sparly
        /// </summary>
        IMPORTANT,

        /// <summary>
        /// General info
        /// </summary>
        INFO,

        /// <summary>
        /// Something when wrong!
        /// </summary>
        ERROR
    }
}
