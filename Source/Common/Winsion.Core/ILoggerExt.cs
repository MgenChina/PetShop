using System;
using System.Collections.Generic;

namespace Winsion.Core
{
    public interface ILoggerExt
    {
        string LogName
        {
            get;
        }

        /// <summary>
        /// 根据级别，记录消息
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>       
        void Log(int loggingLevel, object message);

        /// <summary>
        ///  根据级别，记录消息和异常
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>   
        void Log(int loggingLevel, object message, Exception exception);

        /// <summary>
        ///  根据级别，记录消息、异常、自定义属性
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>   
        void Log(int loggingLevel, object message, Exception exception, IDictionary<string, string> loggingProperties);

        /// <summary>
        ///  根据级别，记录消息、异常、自定义属性
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>   
        /// <param name="message">消息</param>
        /// <param name="exception">异常</param>
        /// <param name="loggingProperties">通过反射把该对象的共有实例属性值映射为键值对</param>
        void Log(int loggingLevel, object message, Exception exception, object loggingProperties);

        /// <summary>
        ///  根据级别，记录消息、自定义属性
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param> 
        void Log(int loggingLevel, object message, IDictionary<string, string> loggingProperties);

        /// <summary>
        ///  根据级别，记录消息、自定义属性
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>   
        /// <param name="message">消息</param>
        /// <param name="loggingProperties">通过反射把该对象的共有实例属性值映射为键值对</param>
        void Log(int loggingLevel, object message, object loggingProperties);

        /// <summary>
        /// 根据级别，记录格式化消息。
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>       
        /// <param name="format">格式化消息</param>
        /// <param name="arg0">消息格式化参数</param>
        void LogFormat(int loggingLevel, string format, object arg0);

        /// <summary>
        /// 根据级别，记录格式化消息。
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>       
        /// <param name="format">格式化消息</param>
        /// <param name="args">消息格式化参数数组</param>
        void LogFormat(int loggingLevel, string format, params object[] args);

        /// <summary>
        /// 根据级别，记录格式化消息。
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>       
        /// <param name="format">格式化消息</param>
        /// <param name="exception">异常</param>
        /// <param name="arg0">消息格式化参数</param>
        void LogFormat(int loggingLevel, string format, Exception exception, object arg0);

        /// <summary>
        /// 根据级别，记录格式化消息。
        /// </summary>
        /// <param name="loggingLevel">enum LoggingLevel: Debug = 0,Info = 1,Warning = 2,Error = 3,Fatal = 4</param>       
        /// <param name="format">格式化消息</param>
        /// <param name="exception">异常</param>
        /// <param name="args">消息格式化参数数组</param>
        void LogFormat(int loggingLevel, string format, Exception exception, params object[] args);
    }

}
