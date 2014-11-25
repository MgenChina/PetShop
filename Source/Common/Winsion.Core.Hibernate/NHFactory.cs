using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winsion.Core.Hibernate.WCF;
using Winsion.Core.Hibernate.Repository;
using Winsion.Core.Hibernate.Repository.Impl;

namespace Winsion.Core.Hibernate
{
    public sealed class NHFactory : INHFactory
    {
        private NHFactory()
        {
        }

        public static INHFactory Instance = new NHFactory();

        public INHibernateSessionManager GetSessionManager()
        {
            if (System.ServiceModel.OperationContext.Current != null)
            {
                return WcfNHibernateSessionManager.Instance;
            }
            else
            {
                return NHibernateSessionManager.Instance;
            }
        }

        public INHibernateSessionManager GetSessionManager(NHSessionContextType contextType)
        {
            INHibernateSessionManager manager;
            switch (contextType)
            {
                case NHSessionContextType.WCF:
                    manager = WcfNHibernateSessionManager.Instance;
                    break;
                default:
                    manager = NHibernateSessionManager.Instance;
                    break;
            }

            return manager;
        }

        public INHibernateSession GetNewSession()
        {
            return new NHibernateSession();
        }

        public INHibernateSession GetNewSession(string dbConnectionString)
        {
            return GetNewSession(new DbConnectionObject(dbConnectionString));
        }

        public INHibernateSession GetNewSession(IDbConnectionObject dbConnectionObj)
        {
            return new NHibernateSession(dbConnectionObj);
        }

        public INHibernateSession GetNewSessionForCfgFile(string hibernateCfgFileName)
        {
            return new NHibernateSession(hibernateCfgFileName);
        }

        public INHibernateSession GetSession()
        {
            return GetSessionManager().GetSession();
        }

        public INHibernateSession GetSession(string dbConnectionString)
        {
            return GetSession(new DbConnectionObject(dbConnectionString));
        }

        public INHibernateSession GetSession(IDbConnectionObject dbConnectionObj)
        {
            return GetSessionManager().GetSession(dbConnectionObj);
        }

        public INHibernateSession GetSessionForCfgFile(string hibernateCfgFileName)
        {
            return GetSessionManager().GetSession(hibernateCfgFileName);
        }




        public IRepository<TEntity> GetRepositoryFor<TEntity>()
            where TEntity : class, new()
        {
            return new Repository<TEntity>();
        }

        public IRepository<TEntity> GetRepositoryFor<TEntity>(INHibernateSession session)
            where TEntity : class, new()
        {
            return new Repository<TEntity>(session);
        }

        public IViewRepository<TEntity> ViewRepositoryFor<TEntity>()
            where TEntity : class, new()
        {
            return new ViewRepository<TEntity>();
        }

        public IViewRepository<TEntity> ViewRepositoryFor<TEntity>(INHibernateSession session)
            where TEntity : class, new()
        {
            return new ViewRepository<TEntity>(session);
        }
    }
}
