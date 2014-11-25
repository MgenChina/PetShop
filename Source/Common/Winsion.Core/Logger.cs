using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.Reflection;
using System.Security.Principal;
using System.Diagnostics.CodeAnalysis;


namespace Winsion.Core
{
    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is OK here.")]
    public class Logger : ILogger
    {
        public Logger()
            : this(typeof(Logger))
        {

        }

        public Logger(string logName)
        {
            _logName = logName;
        }

        public Logger(Type typeForLog)
        {
            _typeForLog = typeForLog;
            _logName = typeForLog.FullName;
        }      
     
        #region ILog Implemention

        [SuppressMessage("Microsoft.Design", "CA1033", Justification = "This is OK here.")]
        public void Debug(object message)
        {
            Log(LoggingLevel.Debug, message);
        }

        public void Debug(object message, Exception exception)
        {
            Log(LoggingLevel.Debug, message, exception);
        }

        public void DebugFormat(string format, object arg0)
        {
            LogFormat(LoggingLevel.Debug, format, arg0);
        }

        public void DebugFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Debug, format, args);
        }


        public void Error(object message)
        {
            Log(LoggingLevel.Error, message);
        }

        public void Error(object message, Exception exception)
        {
            Log(LoggingLevel.Error, message, exception);
        }

        public void ErrorFormat(string format, object arg0)
        {
            LogFormat(LoggingLevel.Error, format, arg0);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Error, format, args);
        }


        public void ErrorFormat(string format, Exception exception, object arg0)
        {
            LogFormat(LoggingLevel.Error, format, exception, arg0);
        }

        public void ErrorFormat(string format, Exception exception, params object[] args)
        {
            LogFormat(LoggingLevel.Error, format, exception, args);
        }


        public void Info(object message)
        {
            Log(LoggingLevel.Info, message);
        }

        public void Info(object message, Exception exception)
        {
            Log(LoggingLevel.Info, message, exception);
        }

        public void InfoFormat(string format, object arg0)
        {
            LogFormat(LoggingLevel.Info, format, arg0);
        }

        public void InfoFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Info, format, args);
        }


        public void Warn(object message)
        {
            Log(LoggingLevel.Warning, message);
        }

        public void Warn(object message, Exception exception)
        {
            Log(LoggingLevel.Warning, message, exception);
        }

        public void WarnFormat(string format, object arg0)
        {
            LogFormat(LoggingLevel.Warning, format, arg0);
        }

        public void WarnFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Warning, format, args);
        }

        public void WarnFormat(string format, Exception exception, object arg0)
        {
            LogFormat(LoggingLevel.Warning, format, exception, arg0);
        }

        public void WarnFormat(string format, Exception exception, params object[] args)
        {
            LogFormat(LoggingLevel.Warning, format, exception, args);
        }


        public void Fatal(object message)
        {
            Log(LoggingLevel.Fatal, message);
        }

        public void Fatal(object message, Exception exception)
        {
            Log(LoggingLevel.Fatal, message, exception);
        }

        public void FatalFormat(string format, object arg0)
        {
            LogFormat(LoggingLevel.Fatal, format, arg0);
        }

        public void FatalFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Fatal, format, args);
        }

        public void FatalFormat(string format, Exception exception, object arg0)
        {
            LogFormat(LoggingLevel.Fatal, format, exception, arg0);
        }

        public void FatalFormat(string format, Exception exception, params object[] args)
        {
            LogFormat(LoggingLevel.Fatal, format, exception, args);
        }


        public bool IsEnabled(int loggingLevel)
        {
            return ShouldLog((LoggingLevel)loggingLevel);
        }

        #endregion

        #region ILogger Implemention Log By Type

        public string LogName
        {
            get
            {
                return _logName;
            }
            private set
            {
                _logName = value;
            }
        }

        public void Log(LoggingLevel loggingLevel, object message)
        {
            Log(loggingLevel, message, (Exception)null);
        }

        public void Log(LoggingLevel loggingLevel, object message, Exception exception)
        {
            Log(loggingLevel, message, exception, (IDictionary<string, string>)null);
        }

        public void Log(LoggingLevel loggingLevel, object message, Exception exception, IDictionary<string, string> loggingProperties)
        {
            ToLog(loggingLevel, message, exception, loggingProperties);
        }

        public void Log(LoggingLevel loggingLevel, object message, Exception exception, object loggingProperties)
        {
            ToLog(loggingLevel, message, exception, loggingProperties);
        }

        public void Log(LoggingLevel loggingLevel, object message, IDictionary<string, string> loggingProperties)
        {
            Log(loggingLevel, message, null, loggingProperties);
        }

        public void Log(LoggingLevel loggingLevel, object message, object loggingProperties)
        {
            Log(loggingLevel, message, null, loggingProperties);
        }

        public void LogFormat(LoggingLevel loggingLevel, string formatMsg, object arg0)
        {
            LogFormat(loggingLevel, formatMsg, null, arg0);
        }

        public void LogFormat(LoggingLevel loggingLevel, string formatMsg, params object[] args)
        {
            LogFormat(loggingLevel, formatMsg, null, args);
        }

        public void LogFormat(LoggingLevel loggingLevel, string formatMsg, Exception exception, object arg0)
        {
            ToLogFormat(loggingLevel, formatMsg, exception, new object[] { arg0 });
        }

        public void LogFormat(LoggingLevel loggingLevel, string formatMsg, Exception exception, params object[] args)
        {
            ToLogFormat(loggingLevel, formatMsg, exception, args);
        }

        #endregion

        #region ILoggerExt Implemention Log By Type

        public void Log(int loggingLevel, object message)
        {
            Log(loggingLevel, message, (Exception)null);
        }

        public void Log(int loggingLevel, object message, Exception exception)
        {
            Log(loggingLevel, message, exception, (IDictionary<string, string>)null);
        }

        public void Log(int loggingLevel, object message, Exception exception, IDictionary<string, string> loggingProperties)
        {
            ToLog((LoggingLevel)loggingLevel, message, exception, loggingProperties);
        }

        public void Log(int loggingLevel, object message, Exception exception, object loggingProperties)
        {
            ToLog((LoggingLevel)loggingLevel, message, exception, loggingProperties);
        }

        public void Log(int loggingLevel, object message, IDictionary<string, string> loggingProperties)
        {
            Log(loggingLevel, message, null, loggingProperties);
        }

        public void Log(int loggingLevel, object message, object loggingProperties)
        {
            Log(loggingLevel, message, null, loggingProperties);
        }

        public void LogFormat(int loggingLevel, string formatMsg, object arg0)
        {
            LogFormat(loggingLevel, formatMsg, null, arg0);
        }

        public void LogFormat(int loggingLevel, string formatMsg, params object[] args)
        {
            LogFormat(loggingLevel, formatMsg, null, args);
        }

        public void LogFormat(int loggingLevel, string formatMsg, Exception exception, object arg0)
        {
            ToLogFormat((LoggingLevel)loggingLevel, formatMsg, exception, new object[] { arg0 });
        }

        public void LogFormat(int loggingLevel, string formatMsg, Exception exception, params object[] args)
        {
            ToLogFormat((LoggingLevel)loggingLevel, formatMsg, exception, args);
        }

        #endregion

        #region protected method

        protected string CreateAppendInfo()
        {
            try
            {
                string identityTitle = "user identity:{0} ";
                IPrincipal currPrincipal = System.Threading.Thread.CurrentPrincipal;
                if (currPrincipal == null || currPrincipal.Identity == null || currPrincipal.Identity.IsAuthenticated == false)
                {
                    return string.Format(identityTitle, "<unknown>");
                }
                string identity = currPrincipal.Identity.Name;
                string principal = currPrincipal.ToString();

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(identityTitle, identity);
                sb.AppendFormat("principal:{0} ", principal);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                if (innerlog.IsErrorEnabled)
                {
                    innerlog.Error(string.Format("Logger.CreateAppendInfo method is error, msg:{0}", ex.Message), ex);
                }
                throw ex;
            }
        }

        protected virtual object AppendInfo(object originalMessage)
        {
            try
            {
                var info = CreateAppendInfo();
                if (info == null || info == "")
                {
                    return originalMessage;
                }
                var msg = string.Format("{0} info -> {1}", originalMessage, info);
                return msg;
            }
            catch (Exception ex)
            {
                if (innerlog.IsErrorEnabled)
                {
                    innerlog.Error("Logger.AppendInfo method .", ex);
                }
                return originalMessage;
            }
        }

        protected virtual void ToLog(LoggingLevel loggingLevel, object message, Exception exception, IDictionary<string, string> loggingProperties)
        {
            if (ShouldLog(loggingLevel))
            {
                var msg = AppendInfo(message);

                if (loggingProperties != null)
                {
                    PushLoggingProperties(loggingProperties);
                }

                if (exception != null)
                {
                    switch (loggingLevel)
                    {
                        case LoggingLevel.Debug:
                            mylog.Debug(msg, exception);
                            break;
                        case LoggingLevel.Info:
                            mylog.Info(msg, exception);
                            break;
                        case LoggingLevel.Warning:
                            mylog.Warn(msg, exception);
                            break;
                        case LoggingLevel.Error:
                            mylog.Error(msg, exception);
                            break;
                        case LoggingLevel.Fatal:
                            mylog.Fatal(msg, exception);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (loggingLevel)
                    {
                        case LoggingLevel.Debug:
                            mylog.Debug(msg);
                            break;
                        case LoggingLevel.Info:
                            mylog.Info(msg);
                            break;
                        case LoggingLevel.Warning:
                            mylog.Warn(msg);
                            break;
                        case LoggingLevel.Error:
                            mylog.Error(msg);
                            break;
                        case LoggingLevel.Fatal:
                            mylog.Fatal(msg);
                            break;
                        default:
                            break;
                    }
                }

                if (loggingProperties != null)
                {
                    PopLoggingProperties(loggingProperties);
                }
            }
        }

        protected virtual void ToLog(LoggingLevel loggingLevel, object message, Exception exception, object loggingProperties)
        {
            if (ShouldLog(loggingLevel))
            {
                var dic = GetLoggingPropertiesFor(loggingProperties);
                ToLog(loggingLevel, message, exception, dic);
            }
        }

        protected virtual void ToLogFormat(LoggingLevel loggingLevel, string message, Exception exception, object[] args)
        {
            if (args == null || args.Length == 0)
            {
                ToLog(loggingLevel, message, exception, (IDictionary<string, string>)null);
                return;
            }

            if (ShouldLog(loggingLevel))
            {
                if (exception != null)
                {
                    var msg = FormatMessage(message, args);
                    ToLog(loggingLevel, msg, exception, (IDictionary<string, string>)null);
                }
                else
                {
                    var temp = AppendInfo(message);
                    var msg = temp.ToString();
                    switch (loggingLevel)
                    {
                        case LoggingLevel.Debug:
                            mylog.DebugFormat(msg, args);
                            break;
                        case LoggingLevel.Info:
                            mylog.InfoFormat(msg, args);
                            break;
                        case LoggingLevel.Warning:
                            mylog.WarnFormat(msg, args);
                            break;
                        case LoggingLevel.Error:
                            mylog.ErrorFormat(msg, args);
                            break;
                        case LoggingLevel.Fatal:
                            mylog.FatalFormat(msg, args);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion


        #region private method


        private static void PushLoggingProperties(IDictionary<string, string> loggingProperties)
        {
            foreach (var kv in loggingProperties)
            {
                ThreadContext.Stacks[kv.Key].Push(kv.Value);
            }
        }

        private static void PopLoggingProperties(IDictionary<string, string> loggingProperties)
        {
            foreach (var kv in loggingProperties)
            {
                ThreadContext.Stacks[kv.Key].Pop();
            }

        }

        private static IDictionary<string, string> GetLoggingPropertiesFor(object loggingProperties)
        {
            IDictionary<string, string> dic = null;

            if (loggingProperties != null)
            {
                dic = new Dictionary<string, string>();

                Type type = loggingProperties.GetType();
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < properties.Length; i++)
                {
                    object value = properties[i].GetValue(loggingProperties, null);
                    if (value != null)
                        dic[properties[i].Name] = value.ToString();
                    else
                        dic[properties[i].Name] = "null";
                }
            }

            return dic;
        }

        private static string FormatMessage(string format, object[] args)
        {
            try
            {
                string message;
                switch (args.Length)
                {
                    case 1:
                        message = string.Format(format, args[0]);
                        break;
                    case 2:
                        message = string.Format(format, args[0], args[1]);
                        break;
                    case 3:
                        message = string.Format(format, args[0], args[1], args[2]);
                        break;
                    default:
                        message = string.Format(format, args);
                        break;
                }

                return message;
            }
            catch (Exception ex)
            {
                if (innerlog.IsWarnEnabled)
                {
                    innerlog.Warn(string.Format("Logger.FormatMessage method is error, msg:{0}", ex.Message), ex);
                }
                return format;
            }
        }

        private static IEnumerable<log4net.ILog> GetLeafLoggers()
        {
            var leafLogs = new List<log4net.ILog>();

            var allLogs = LogManager.GetCurrentLoggers();
            for (int i = 0; i < allLogs.Length; i++)
            {
                bool isParent = false;
                for (int j = 0; j < allLogs.Length; j++)
                {
                    if (i != j && allLogs[j].Logger.Name.StartsWith(string.Format("{0}.", allLogs[i].Logger.Name)))
                    {
                        isParent = true;
                        break;
                    }
                }
                if (!isParent)
                {
                    leafLogs.Add(allLogs[i]);
                }
            }
            return leafLogs;
        }


        private bool ShouldLog(LoggingLevel loggingLevel)
        {
            switch (loggingLevel)
            {
                case LoggingLevel.Debug:
                    return mylog.IsDebugEnabled;
                case LoggingLevel.Info:
                    return mylog.IsInfoEnabled;
                case LoggingLevel.Warning:
                    return mylog.IsWarnEnabled;
                case LoggingLevel.Error:
                    return mylog.IsErrorEnabled;
                case LoggingLevel.Fatal:
                    return mylog.IsFatalEnabled;
                default:
                    return false;
            }
        }

        private log4net.ILog CreateLogger()
        {
            return _typeForLog != null ? LogManager.GetLogger(_typeForLog) : LogManager.GetLogger(_logName);
        }

        #endregion

        private readonly object obj = new object();
        private log4net.ILog _log;
        private log4net.ILog mylog
        {
            get
            {
                if (_log == null)
                {
                    lock (obj)
                    {
                        if (_log == null)
                        {
                            _log = CreateLogger();
                        }
                    }
                }
                return _log;
            }
        }

        private Type _typeForLog;
        private string _logName;

        private static readonly log4net.ILog innerlog = LogManager.GetLogger(typeof(Logger));

    }


    public class Logger<TForLog> : Logger, ILogger<TForLog>
        where TForLog : class
    {
        private Type _typeForLog;


        public Logger()
            : base(typeof(TForLog))
        {
            _typeForLog = typeof(TForLog);
        }


        public Type TypeForLog
        {
            get
            {
                return _typeForLog;
            }
        }

    }





}


