using System;
using System.Collections.Generic;

namespace Winsion.Core
{  
    public interface ILogger : ILog
    {
        string LogName { get; }

        void Log(LoggingLevel loggingLevel, object message);
        void Log(LoggingLevel loggingLevel, object message, Exception exception);
        void Log(LoggingLevel loggingLevel, object message, Exception exception, IDictionary<string, string> loggingProperties);
        void Log(LoggingLevel loggingLevel, object message, Exception exception, object loggingProperties);

        void Log(LoggingLevel loggingLevel, object message, IDictionary<string, string> loggingProperties);
        void Log(LoggingLevel loggingLevel, object message, object loggingProperties);


        void LogFormat(LoggingLevel loggingLevel, string format, object arg0);
        void LogFormat(LoggingLevel loggingLevel, string format, params object[] args);

        void LogFormat(LoggingLevel loggingLevel, string format, Exception exception, object arg0);
        void LogFormat(LoggingLevel loggingLevel, string format, Exception exception, params object[] args);
    }

    public interface ILogger<TForLog> : ILogger
        where TForLog : class
    {
        Type TypeForLog { get; }
    }

    /// <summary>
    /// Represents different logging levels.
    /// </summary>
    public enum LoggingLevel : byte
    {
        /// <summary>
        /// Used for debugging.
        /// </summary>
        Debug = 0,
        /// <summary>
        /// Used for informational purposes.
        /// </summary>
        Info = 1,
        /// <summary>
        /// Flag as a warning.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Flag as an error.
        /// </summary>
        Error = 3,
        /// <summary>
        /// Flag as fatal.
        /// </summary>
        Fatal = 4
    }

}
