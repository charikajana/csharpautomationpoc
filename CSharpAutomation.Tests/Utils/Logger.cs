using System.Runtime.CompilerServices;

namespace CSharpAutomation.Tests.Utils
{
    /// <summary>
    /// Logger utility for consistent logging across the framework.
    /// Logs are written to console and can be extended to file/external systems.
    /// </summary>
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static LogLevel _minLogLevel = LogLevel.Info;

        /// <summary>
        /// Set minimum log level to display
        /// </summary>
        public static void SetLogLevel(LogLevel level)
        {
            _minLogLevel = level;
        }

        /// <summary>
        /// Log an informational message
        /// </summary>
        public static void Info(string message, [CallerMemberName] string caller = "")
        {
            Log(LogLevel.Info, message, caller);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        public static void Debug(string message, [CallerMemberName] string caller = "")
        {
            Log(LogLevel.Debug, message, caller);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        public static void Warn(string message, [CallerMemberName] string caller = "")
        {
            Log(LogLevel.Warn, message, caller);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        public static void Error(string message, [CallerMemberName] string caller = "")
        {
            Log(LogLevel.Error, message, caller);
        }

        /// <summary>
        /// Log an error with exception details
        /// </summary>
        public static void Error(string message, Exception ex, [CallerMemberName] string caller = "")
        {
            Log(LogLevel.Error, $"{message} | Exception: {ex.Message}", caller);
            if (ex.StackTrace != null)
            {
                Log(LogLevel.Debug, $"StackTrace: {ex.StackTrace}", caller);
            }
        }

        /// <summary>
        /// Log a success message (shown in green)
        /// </summary>
        public static void Success(string message, [CallerMemberName] string caller = "")
        {
            Log(LogLevel.Success, message, caller);
        }

        /// <summary>
        /// Log a step/action message
        /// </summary>
        public static void Step(string message)
        {
            Log(LogLevel.Step, message, "");
        }

        /// <summary>
        /// Log a separator line
        /// </summary>
        public static void Separator()
        {
            lock (_lock)
            {
                Console.WriteLine(new string('─', 80));
            }
        }

        /// <summary>
        /// Log a header/title
        /// </summary>
        public static void Header(string title)
        {
            lock (_lock)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"╔{new string('═', 78)}╗");
                Console.WriteLine($"║ {title.PadRight(76)} ║");
                Console.WriteLine($"╚{new string('═', 78)}╝");
                Console.ResetColor();
            }
        }

        private static void Log(LogLevel level, string message, string caller)
        {
            if (level < _minLogLevel && level != LogLevel.Success && level != LogLevel.Step)
                return;

            lock (_lock)
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                var levelStr = GetLevelString(level);
                var color = GetLevelColor(level);

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"[{timestamp}] ");

                Console.ForegroundColor = color;
                Console.Write($"[{levelStr}]");

                Console.ForegroundColor = ConsoleColor.White;
                
                if (!string.IsNullOrEmpty(caller) && level != LogLevel.Step)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($" [{caller}]");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine($" {message}");
                Console.ResetColor();
            }
        }

        private static string GetLevelString(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => "DEBUG",
                LogLevel.Info => "INFO ",
                LogLevel.Warn => "WARN ",
                LogLevel.Error => "ERROR",
                LogLevel.Success => " ✓  ",
                LogLevel.Step => "STEP ",
                _ => "INFO "
            };
        }

        private static ConsoleColor GetLevelColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => ConsoleColor.DarkGray,
                LogLevel.Info => ConsoleColor.Blue,
                LogLevel.Warn => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Success => ConsoleColor.Green,
                LogLevel.Step => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
        }
    }

    /// <summary>
    /// Log levels for filtering log output
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Step = 2,
        Success = 3,
        Warn = 4,
        Error = 5
    }
}
