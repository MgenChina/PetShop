using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibernate.Cfg;
using log4net;
using System.IO;
using System.Collections;

namespace Winsion.Core.Hibernate
{
    /// <summary>
    /// A Singleton that creates and persits a single SessionFactory for the to program to access globally.
    /// This uses the .Net CallContext to store a session for each thread.
    /// 
    /// This is heavely based on 'NHibernate Best Practices with ASP.NET'     
    /// </summary>
    internal sealed partial class NHibernateSessionManager : NHibernateSessionManagerBase, INHibernateSessionManager
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

            internal static readonly INHibernateSessionManager NHibernateSessionManager = new NHibernateSessionManager();
        }

        #endregion

        #region Declarations

        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(NHibernateSessionManager));

        #endregion

        #region Constructors & Finalizers

        /// <summary>
        /// This will load the NHibernate settings from the App.config.
        /// Note: This can/should be expanded to support multiple databases.
        /// </summary>
        private NHibernateSessionManager()
            : base()
        {
            var defaultSessionFactory = DefaultSessionFactory;
        }

        ~NHibernateSessionManager()
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

        #region Properties

        private bool IsWebContext
        {
            get { return (System.Web.HttpContext.Current != null); }
        }

        #endregion

        #region Imp INHibernateSessionManager

        public override NHSessionContextType ContextType
        {
            get
            {
                NHSessionContextType cType;
                if (IsWebContext)
                {
                    cType = NHSessionContextType.Web;
                }
                else
                {
                    cType = NHSessionContextType.Call;
                }

                return cType;
            }
        }



        #endregion

        #region protected

        /// <summary>
        /// Since multiple databases may be in use, there may be one session per database 
        /// persisted at any one time.  The easiest way to store them is via a hashtable
        /// with the key being tied to session factory.  If within a web context, this uses
        /// <see cref="HttpContext" /> instead of the WinForms specific <see cref="CallContext" />.  
        /// Discussion concerning this found at http://forum.springframework.net/showthread.php?t=572
        /// </summary>
        protected override IDictionary ContextSessions
        {
            get
            {
                if (IsWebContext)
                {
                    if (System.Web.HttpContext.Current.Items[contextSessionsKey] == null)
                    {
                        System.Web.HttpContext.Current.Items[contextSessionsKey] = new Hashtable();
                    }

                    return (Hashtable)System.Web.HttpContext.Current.Items[contextSessionsKey];
                }
                else
                {
                    if (System.Runtime.Remoting.Messaging.CallContext.GetData(contextSessionsKey) == null)
                    {
                        System.Runtime.Remoting.Messaging.CallContext.SetData(contextSessionsKey, new Hashtable());
                    }

                    return (Hashtable)System.Runtime.Remoting.Messaging.CallContext.GetData(contextSessionsKey);
                }
            }
        }

        #endregion

        private const string contextSessionsKey = "my_contextSessions_Key";
    }
}
