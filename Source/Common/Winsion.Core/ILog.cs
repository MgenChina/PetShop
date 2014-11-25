using System;
using System.Collections.Generic;

namespace Winsion.Core
{
    /// <summary>
    /// 兼容log4net 代码
    /// </summary>   
    public interface ILog
    {
        void Debug(object message);

        void Debug(object message, Exception exception);

        void DebugFormat(string format, object arg0);

        void DebugFormat(string format, params object[] args);


        void Info(object message);

        void Info(object message, Exception exception);

        void InfoFormat(string format, object arg0);

        void InfoFormat(string format, params object[] args);


        void Warn(object message);

        void Warn(object message, Exception exception);

        void WarnFormat(string format, object arg0);

        void WarnFormat(string format, params object[] args);


        void WarnFormat(string format, Exception exception, object arg0);

        void WarnFormat(string format, Exception exception, params object[] args); 
        
      
        void Error(object message);

        void Error(object message, Exception exception);

        void ErrorFormat(string format, object arg0);

        void ErrorFormat(string format, params object[] args);


        void ErrorFormat(string format, Exception exception, object arg0);

        void ErrorFormat(string format, Exception exception, params object[] args);


        void Fatal(object message);

        void Fatal(object message, Exception exception);

        void FatalFormat(string format, object arg0);

        void FatalFormat(string format, params object[] args);


        void FatalFormat(string format, Exception exception, object arg0);

        void FatalFormat(string format, Exception exception, params object[] args);
       
        /// <summary>
        /// 检查Log配置是否打开相应loggingLevel级别的Log功能。
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>               
        bool IsEnabled(int loggingLevel);

    }  

}
