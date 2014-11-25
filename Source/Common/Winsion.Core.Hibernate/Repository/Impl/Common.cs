
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winsion.Core.Hibernate.Repository.Impl
{
    public abstract class Common<TEntity> : ICommon<TEntity>
    {
        #region Declarations

        protected INHibernateSession session;

        private bool disposed = false;

        #endregion

        #region Constructors

        public Common()
            : this(NHFactory.Instance.GetSessionManager().GetSession())
        {         
        }

        public Common(INHibernateSession session)
        {
            this.session = session;
            this.session.IncrementRefCount();
        }

        ~Common()
        {
            Dispose(true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The NHibernate Session object is exposed only to the Manager class.
        /// It is recommended that you...
        /// ...use the the NHibernateSession methods to control Transactions (unless you specifically want nested transactions).
        /// ...do not directly expose the Flush method (to prevent open transactions from locking your DB).
        /// </summary>
        public System.Type Type
        {
            get { return typeof(TEntity); }
        }

        public INHibernateSession Session
        {
            get { return session; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool finalizing)
        {
            if (disposed)
            {
                return;
            }

            if (finalizing == false)
            {
                if (session != null)
                {
                    session.DecrementRefCount();
                }
            }

            disposed = true;
        }

        #endregion
    }
}
