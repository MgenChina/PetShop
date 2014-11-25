using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity;
using Winsion.Core;
using System.Collections;

namespace Winsion.Core.AOP
{
    public static class EnumerableExtensions
    {
        public static string ToStringList(this IEnumerable list)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in list)
            {
                sb.AppendFormat("{0}, ", item);
            }

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ExceptionAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new ExceptionHandler(Order);
        }
    }

    public class ExceptionHandler : ICallHandler
    {
        public ExceptionHandler(int order)
        {
            Order = order;

        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var methodReturn = getNext().Invoke(input, getNext);

            // log exception if there is one... 
            if (methodReturn.Exception != null)
            {
                LogException(input, methodReturn);
            }

            return methodReturn;
        }

        public int Order { get; set; }


        internal static void LogException(IMethodInvocation input, IMethodReturn methodReturn)
        {
            Exception ex = methodReturn.Exception;

            var type = input.MethodBase.DeclaringType;

            string className = type.Name;
            string methodName = input.MethodBase.Name;
            string generic = type.IsGenericType ? string.Format("<{0}>", type.GetGenericArguments().ToStringList()) : string.Empty;
            string arguments = input.Arguments.ToStringList();

            string preMethodMessage = string.Format("{0}{1}.{2}({3})", className, generic, methodName, arguments);

            ILogger log = new Logger(type);
            log.ErrorFormat("{0} 发生错误。\r\n 消息：{1}", preMethodMessage, ex.Message);

        }

        

    }

}
