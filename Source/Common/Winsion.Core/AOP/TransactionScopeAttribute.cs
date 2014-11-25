using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity;
using System.Transactions;

namespace Winsion.Core.AOP
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class TransactionScopeAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new TransactionScopeHandler(Order);
        }
    }

    public class TransactionScopeHandler : ICallHandler
    {
        public TransactionScopeHandler(int order)
        {
            Order = order;

        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn methodReturn;
            try
            {
                using (TransactionScope transScope = TransactionUtility.CreateTransactionScope())
                {
                    methodReturn = getNext().Invoke(input, getNext);
                    if (methodReturn.Exception != null)
                        ExceptionHandler.LogException(input, methodReturn);
                    transScope.Complete();
                }
                return methodReturn;
            }
            catch (Exception ex)
            {
                log.Error("分布式事务错误(TransactionScope)\r\n", ex);

                throw ex;
            }
        }

        public int Order { get; set; }


        private static readonly ILogger log = new Logger(typeof(TransactionScopeHandler));

    }
}
