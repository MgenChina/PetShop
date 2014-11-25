using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Collections;
using log4net;

namespace Winsion.Core.Hibernate
{
    public abstract class NHibernateSessionManagerBase : INHibernateSessionManager, ISessionFactoryProvider
    {
        #region Imp INHibernateSessionManager


        public virtual ISession CreateISession(INHibernateSessionExt session)
        {
            ISession iSession;
            switch (session.SessionType)
            {
                case NHibernateSessionType.NewConfigFile:
                    iSession = CreateISession(session.CfgFileName);
                    break;
                case NHibernateSessionType.NewDbConnection:
                    iSession = CreateISession(session.DbConnection);
                    break;
                case NHibernateSessionType.DefaultConfig:
                    iSession = CreateISession();
                    break;
                default:
                    iSession = CreateISession();
                    break;
            }

            return iSession;
        }

        public virtual INHibernateSession GetSession(IDbConnectionObject dbConnectionObj)
        {
            var key = dbConnectionObj.ToString();
            INHibernateSession session = GetContextSession(key);
            if (session == null)
            {
                session = new NHibernateSession(dbConnectionObj);
                SetContextSession(key, session);
            }

            return session;
        }

        public virtual INHibernateSession GetSession(string nHibernateCfgFileName)
        {
            var key = nHibernateCfgFileName;
            INHibernateSession session = GetContextSession(key);
            if (session == null)
            {
                session = new NHibernateSession(nHibernateCfgFileName);
                SetContextSession(key, session);
            }

            return session;
        }

        public virtual INHibernateSession GetSession()
        {
            INHibernateSession session = GetContextSession(defaultContextSessionKey);
            if (session == null)
            {
                session = new NHibernateSession();
                SetContextSession(defaultContextSessionKey, session);
            }

            return session;
        }

        public void RemoveSession(INHibernateSessionExt session)
        {
            var key = "";
            switch (session.SessionType)
            {
                case NHibernateSessionType.NewConfigFile:
                    key = session.CfgFileName;
                    break;
                case NHibernateSessionType.NewDbConnection:
                    key = session.DbConnectionObj.ToString();
                    break;
                case NHibernateSessionType.DefaultConfig:
                    key = defaultContextSessionKey;
                    break;
                default:
                    key = defaultContextSessionKey;
                    break;
            }

            if (ContextSessions.Contains(key))
            {
                ContextSessions.Remove(key);
            }
        }

        public virtual void CloseContextSessions()
        {
            CloseContextSessions(ContextSessions);
        }

        public abstract NHSessionContextType ContextType { get; }

        #region IDisposable

        private bool isDisposed = false;

        public virtual void Dispose()
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
                // Close SessionFactory
                Close();
            }

            // Flag as disposed.
            isDisposed = true;
        }

        #endregion


        #endregion

        #region protected

        private ISessionFactory defaultSessionFactory = null;
        protected ISessionFactory DefaultSessionFactory
        {
            get
            {
                if (defaultSessionFactory == null)
                {
                    string filename = Helper.GetCurrentDomainFilePathFor(defaultSessionFactoryCfgFileName);
                    defaultSessionFactory = GetSessionFactoryByCfgFileName(filename);
                }
                return defaultSessionFactory;
            }
        }

        protected static ISessionFactory GetSessionFactoryByCfgFileName(string nHibernateCfgFileName)
        {
            string fileName = nHibernateCfgFileName;
            if (string.IsNullOrEmpty(fileName))
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetSessionFactoryByCfgFileName method,nHibernateCfgFileName may not be null nor empty");
                }

                throw new ArgumentException("nHibernateCfgFileName may not be null nor empty");
            }

            ISessionFactory sFactory = (ISessionFactory)sessionFactoryStore[fileName];
            if (sFactory == null)
            {
                // lazy lock 
                lock (lockObj)
                {
                    sFactory = (ISessionFactory)sessionFactoryStore[fileName];
                    if (sFactory == null)
                    {
                        try
                        {
                            sFactory = new NHibernate.Cfg.Configuration().Configure(fileName).BuildSessionFactory();
                        }
                        catch (Exception ex)
                        {
                            if (log.IsErrorEnabled)
                            {
                                log.Error(string.Format("配置会话工厂(ISessionFactory)发生错误,配置文件名={0}", nHibernateCfgFileName), ex);
                            }

                            throw ex;
                        }

                        if (sFactory == null)
                        {
                            throw new InvalidOperationException("cfg.BuildSessionFactory() returned null.");
                        }

                        sessionFactoryStore[fileName] = sFactory;
                    }
                }
            }

            return sFactory;
        }

        /// <summary>
        /// 如果wcf InstanceContextMode = InstanceContextMode.PerCall，是线程安全的
        /// </summary>
        protected abstract IDictionary ContextSessions { get; }


        protected static void CloseContextSessions(IDictionary contextSessions)
        {
            try
            {
                if (contextSessions == null)
                {
                    return;
                }

                foreach (object li in contextSessions.Values)
                {
                    var contextSession = li as INHibernateSession;
                    CloseSession(ref contextSession);
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CloseContextSessions方法发生Exception", ex);
                }
            }
            finally
            {
                if (contextSessions != null && contextSessions.Count > 0)
                {
                    contextSessions.Clear();
                }
            }
        }

        #endregion

        #region CreateISession

        private ISession CreateISession()
        {
            ISession iSession = DefaultSessionFactory.OpenSession();
            return iSession;
        }

        private ISession CreateISession(System.Data.IDbConnection dbConnection)
        {
            ISession iSession;
            try
            {
                if (dbConnection.State == System.Data.ConnectionState.Closed)
                {
                    dbConnection.Open();
                }

                iSession = DefaultSessionFactory.OpenSession(dbConnection);
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("打开数据库会话发生错误,链接字符串={0}", dbConnection != null ? dbConnection.ConnectionString : ""), ex);
                }

                throw ex;
            }

            return iSession;
        }

        private ISession CreateISession(string nHibernateCfgFileName)
        {
            ISession iSession;
            ISessionFactory sFactory = GetSessionFactoryByCfgFileName(nHibernateCfgFileName);
            try
            {
                iSession = sFactory.OpenSession();
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("打开数据库会话发生错误,配置文件名={0}", nHibernateCfgFileName), ex);
                }

                throw ex;
            }

            return iSession;
        }

        #endregion

        ISessionFactory ISessionFactoryProvider.GetSessionFactory()
        {
            return DefaultSessionFactory;
        }

        ISessionFactory ISessionFactoryProvider.GetSessionFactory(string cfgFileName)
        {
            string filename = Helper.GetCurrentDomainFilePathFor(cfgFileName);
            return GetSessionFactoryByCfgFileName(filename);
        }

        private void Close()
        {
            try
            {
                if (sessionFactoryStore != null)
                {
                    foreach (var sf in sessionFactoryStore.Values)
                    {
                        var sf1 = ((ISessionFactory)sf);
                        if (sf1 != null)
                        {
                            sf1.Close();
                            sf1.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Close 方法", ex);
                }

                throw ex;
            }
            finally
            {
                if (sessionFactoryStore != null)
                {
                    sessionFactoryStore.Clear();
                }
            }
        }

        private static void CloseSession(ref INHibernateSession ises)
        {
            if (ises == null)
            {
                return;
            }

            try
            {
                if (ises.IsOpen)
                {
                    if (log.IsInfoEnabled)
                    {
                        log.Info("CloseSession方法，释放本次请求的ISession链接");
                    }
                }

                ises.Close();
                ises = null;
            }
            catch (NHibernate.TransactionException ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CloseSession方法发生NHibernate.TransactionException", ex);
                }
            }
            catch (System.Transactions.TransactionException ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CloseSession方法发生System.Transactions.TransactionException", ex);
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("CloseSession方法，INHibernateSession.Close() error", ex);
                }
            }
        }

        private INHibernateSession GetContextSession(string sessionContextKey)
        {
            return (INHibernateSession)ContextSessions[sessionContextKey];
        }

        private void SetContextSession(string sessionContextKey, INHibernateSession contextSession)
        {
            ContextSessions[sessionContextKey] = contextSession;
        }


        private const string defaultSessionFactoryCfgFileName = "hibernate.cfg.xml";
        private const string defaultContextSessionKey = "nhibernateContextSession_Key";


        private static readonly IDictionary sessionFactoryStore = new Hashtable();
        private static readonly object lockObj = new object();

        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(NHibernateSessionManagerBase));
    }
}
