using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using log4net;

namespace Winsion.Core.Hibernate
{
    internal partial class NHibernateSession : INHibernateSession, INHibernateSessionExt
    {
        #region Declarations

        protected ITransaction transaction = null;
        protected ISession iSession = null;

        private System.Data.IDbConnection dbConnection = null;

        private bool autoCloseSession = true;
        private bool isDisposed = false;
        private int refCount = 0;


        private bool? hasDistributedTransaction = null;

        private string nHibernateCfgFileName = string.Empty;
        private NHibernateSessionType nHibernateSessionType = NHibernateSessionType.DefaultConfig;
        private IDbConnectionObject dbConnectionObj = null;

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool finalizing)
        {
            if (isDisposed)
            {
                return;
            }

            if (finalizing == false)
            {
                Close();
            }

            isDisposed = true;
        }

        #endregion

        #region Methods

        public void CommitChanges()
        {
            if (HasOpenTransaction)
            {
                CommitTransaction();
            }
            else
            {
                iSession.Flush();
            }
        }

        public void Close()
        {
            if (iSession == null)
            {
                return;
            }

            if (HasOpenTransaction)
            {
                RollbackTransaction();
            }

            //如果有分布式事务，ISession对象只能dispose，不能关闭。
            if (HasDistributedTransaction == false)
            {
                if (iSession.IsOpen)
                {
                    iSession.Close();
                }
            }

            iSession.Dispose();
            iSession = null;

            if (HasDbConnection)
            {
                dbConnection.Close();
                dbConnection.Dispose();
                dbConnection = null;
            }

            var manager = NHFactory.Instance.GetSessionManager();
            manager.RemoveSession(this);
        }

        public void BeginTransaction()
        {
            transaction = GetISession().BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (transaction == null)
            {
                return;
            }

            try
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
            catch (HibernateException)
            {
                RollbackTransaction();
                throw;
            }
        }

        public void RollbackTransaction()
        {
            if (transaction == null)
            {
                return;
            }

            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
        }

        public void IncrementRefCount()
        {
            refCount++;
        }

        public void DecrementRefCount()
        {
            refCount--;
            if (refCount == 0 && AutoCloseSession)
            {
                Close();
            }
        }

        public ISession GetISession()
        {
            if (iSession == null)
            {
                var manager = NHFactory.Instance.GetSessionManager();
                if (nHibernateSessionType == NHibernateSessionType.NewDbConnection)
                {
                    dbConnection = CreateConnection(dbConnectionObj.ConnectionString, dbConnectionObj.ProviderName);
                }

                iSession = manager.CreateISession(this);
            }

            return iSession;
        }

        #endregion

        #region Properties

        public bool HasOpenTransaction
        {
            get { return (transaction != null); }
        }

        public bool IsOpen
        {
            get { return (iSession != null && iSession.IsOpen); }
        }

        public bool AutoCloseSession
        {
            get
            {
                return autoCloseSession;
            }

            set
            {
                autoCloseSession = value;
                if (refCount == 0 && autoCloseSession)
                {
                    Close();
                }
            }
        }

        public bool HasDistributedTransaction
        {
            get
            {
                try
                {
                    var has = hasDistributedTransaction ?? System.Transactions.Transaction.Current != null;
                    return has;
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(string.Format("HasDistributedTransaction 方法，调用 System.Transactions.Transaction.Current 异常。"), ex);
                    }

                    return false;
                }
            }

            set
            {
                hasDistributedTransaction = value;
            }
        }

        private bool HasDbConnection
        {
            get { return dbConnection != null; }
        }

        #endregion

        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(NHibernateSession));
    }
}
