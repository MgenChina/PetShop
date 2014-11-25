
using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Winsion.Core.Hibernate.Repository.Impl
{
    public partial class Repository<TEntity> : ViewRepository<TEntity>, IRepository<TEntity>
    where TEntity : class
    {       
        #region Constructors

        public Repository()
            : base() 
        {
        }

        public Repository(INHibernateSession session)
            : base(session)
        {           
        }

       

        #endregion
   
        #region CRUD Methods

        public object Save(TEntity entity)
        {
            return Session.GetISession().Save(entity);
        }

        public void SaveOrUpdate(TEntity entity)
        {
            Session.GetISession().SaveOrUpdate(entity);
        }

        public void Delete(TEntity entity)
        {
            Session.GetISession().Delete(entity);
        }

        public void Update(TEntity entity)
        {
            Session.GetISession().Update(entity);
        }

        public void Refresh(TEntity entity)
        {
            Session.GetISession().Refresh(entity);
        }

        public void Evict(TEntity entity)
        {
            Session.GetISession().Evict(entity);
        }

        #endregion   
    }
}
