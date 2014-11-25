using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity;
using System.Collections;
using Winsion.Core;

namespace Winsion.Core.AOP
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class LogAttribute : HandlerAttribute
    {
        public LogAttribute(string callingMsg)
            : this(callingMsg, null)
        {

        }

        public LogAttribute(string callingMsg, string calledMsg)
        {
            this.callingMsg = callingMsg;
            this.calledMsg = calledMsg;
        }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LogHandler(Order, callingMsg, calledMsg);
        }

        string callingMsg = string.Empty;
        string calledMsg = string.Empty;
    }

    public class LogHandler : ICallHandler
    {
        public LogHandler(int order, string callingMsg)
            : this(order, callingMsg, null)
        {

        }

        public LogHandler(int order, string callingMsg, string calledMsg)
        {
            Order = order;
            this.callingMsg = callingMsg;
            this.calledMsg = calledMsg;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            if (string.IsNullOrEmpty(callingMsg) == false)
                LogCallingMessage(input, callingMsg);

            var methodReturn = getNext().Invoke(input, getNext);

            if (string.IsNullOrEmpty(calledMsg) == false)
                LogCalledMessage(input, calledMsg);

            // log exception if there is one... 
            if (methodReturn.Exception != null)
            {
                ExceptionHandler.LogException(input, methodReturn);
            }

            return methodReturn;
        }


        public int Order { get; set; }

        static void LogCallingMessage(IMethodInvocation input, string callingMsg)
        {
            Type type;
            string preMethodMessage;
            GetMethodMessage(input, out type, out preMethodMessage);

            ILogger log = new Logger(type);

            log.ErrorFormat("准备调用 {0} 方法。\r\n 消息：{1}", preMethodMessage, callingMsg);

        }

        static void LogCalledMessage(IMethodInvocation input, string calledMsg)
        {
            Type type;
            string preMethodMessage;
            GetMethodMessage(input, out type, out preMethodMessage);

            ILogger log = new Logger(type);

            log.ErrorFormat("调用 {0} 方法后。\r\n 消息：{1}", preMethodMessage, calledMsg);

        }


        private static void GetMethodMessage(IMethodInvocation input, out Type type, out string preMethodMessage)
        {
            type = input.MethodBase.DeclaringType;

            string className = type.Name;
            string methodName = input.MethodBase.Name;
            string generic = type.IsGenericType ? string.Format("<{0}>", type.GetGenericArguments().ToStringList()) : string.Empty;
            string arguments = input.Arguments.ToStringList();

            preMethodMessage = string.Format("{0}{1}.{2}({3})", className, generic, methodName, arguments);
        }

        string callingMsg = string.Empty;
        string calledMsg = string.Empty;
    }




}
