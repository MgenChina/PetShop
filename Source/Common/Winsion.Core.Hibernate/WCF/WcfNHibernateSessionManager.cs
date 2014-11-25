using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Collections;
using log4net;
using System.IO;

namespace Winsion.Core.Hibernate.WCF
{
    internal sealed class WcfNHibernateSessionManager : NHibernateSessionManagerBase, INHibernateSessionManager
    {
        #region Thread-safe, lazy Singleton

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static INHibernateSessionManager Instance
        {
            get
            {
                try
                {
                    return Nested.NHibernateSessionManager;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() 
            {
            }

            internal static readonly INHibernateSessionManager NHibernateSessionManager =
                new WcfNHibernateSessionManager();
        }

        #endregion

        #region Declarations



        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(WcfNHibernateSessionManager));

        #endregion

        #region Constructors & Finalizers

        /// <summary>
        /// This will load the NHibernate settings from the App.config.
        /// Note: This can/should be expanded to support multiple databases.
        /// </summary>
        private WcfNHibernateSessionManager()
            : base()
        {
            var defaultSessionFactory = DefaultSessionFactory;
        }

        ~WcfNHibernateSessionManager()
        {
            Dispose(true);
        }

        #endregion

        #region IDisposable

        private bool isDisposed = false;

        public override void Dispose()
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

            base.Dispose();

            if (finalizing == false)
            {
            }

            isDisposed = true;
        }

        #endregion

        #region Imp INHibernateSessionManager

        public override NHSessionContextType ContextType
        {
            get
            {
                return NHSessionContextType.WCF;
            }
        }

        #endregion

        #region protected



        protected override IDictionary ContextSessions
        {
            get
            {
                WcfNHibernateContext current = WcfNHibernateContext.Current;
                if (current != null)
                {
                    if (current.Items[contextSessionsKey] == null)
                    {
                        current.Items[contextSessionsKey] = new Hashtable();
                    }

                    return (Hashtable)current.Items[contextSessionsKey];
                }
                else
                {
                    log.Warn("WcfNHibernateContext.Current为null时， get WcfNHibernateSessionManager.ContextSessions。");
                    throw new ArgumentNullException("WcfNHibernateContext.Current", "WcfNHibernateContext 不存在，请查看Wcf服务是否添加了NHInstanceContextAttribute。");
                }
            }
        }


        #endregion

        public void InstanceContextDetach(WcfNHibernateContext context)
        {
            if (context != null)
            {
                try
                {
                    var dic = context.Items[contextSessionsKey] as IDictionary;
                    if (dic != null)
                    {
                        CloseContextSessions(dic);
                    }
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("InstanceContext_Detach ", ex);
                    }
                }
            }
        }        

        private const string contextSessionsKey = "my_contextSessions_Key";
       
    }
}
