using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Winsion.Core
{
    public abstract class TransactionUtility
    {
        public static TransactionScope CreateTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
#if DEBUG
                    Timeout = new TimeSpan(0),
#endif
                });
        }

        public static TransactionScope CreateTransactionScope(TimeSpan timeSpan)
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
#if DEBUG
                    Timeout = new TimeSpan(0),
#else
                    Timeout = timeSpan
#endif
                });
        }

        public static TransactionScope CreateTransactionScope(TransactionScopeOption transactionScopeOption)
        {
            return new TransactionScope(transactionScopeOption, new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
#if DEBUG
                    Timeout = new TimeSpan(0),
#endif
                });
        }
    }
}
