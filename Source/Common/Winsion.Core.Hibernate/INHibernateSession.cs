using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Winsion.Core.Hibernate
{
    public interface IDbConnectionObject
    {
        string ConnectionString { get; }

        string ProviderName { get; }
    }

    public enum NHibernateSessionType
    {
        DefaultConfig = 0,

        NewConfigFile = 1,

        NewDbConnection = 2,
    }


    public partial interface INHibernateSessionExt
    {
        NHibernateSessionType SessionType { get; }

        string CfgFileName { get; }

        IDbConnectionObject DbConnectionObj { get; }

        System.Data.IDbConnection DbConnection { get; }
    }

    public partial interface INHibernateSession : IDisposable
    {
        // Methods
        void CommitChanges();

        void Close();

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        void IncrementRefCount();

        void DecrementRefCount();

        ISession GetISession();

        // Properties
        bool HasOpenTransaction { get; }

        bool IsOpen { get; }

        bool AutoCloseSession { get; set; }

        bool HasDistributedTransaction { get; set; }
    }
}
