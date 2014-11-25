
using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Expressions;


namespace Winsion.Core.Hibernate.Repository.Impl
{
    public partial class ViewRepository<TEntity> : Get<TEntity>, IViewRepository<TEntity>
    where TEntity : class
    {
        #region Constructors

        public ViewRepository()
            : base()
        { 
        }

        public ViewRepository(INHibernateSession session)
            : base(session)
        {
        }



        #endregion

        #region IExpressionQueryRepositoryBase
        public IQueryOver<TEntity, TEntity> QueryOver()
        {
            return this.Session.GetISession().QueryOver<TEntity>();
        }

        public IQueryOver<TEntity, TEntity> QueryOver(System.Linq.Expressions.Expression<Func<TEntity>> alias)
        {
            return this.Session.GetISession().QueryOver<TEntity>(alias);
        }
        #endregion

        #region ILinqRepositoryBase
        #region Implementation of IQueryable

        public Expression Expression
        {
            get { return this.Session.GetISession().Query<TEntity>().Expression; }
        }

        public Type ElementType
        {
            get { return this.Session.GetISession().Query<TEntity>().ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return this.Session.GetISession().Query<TEntity>().Provider; }
        }

        #endregion

        #region Implementation of IEnumerator

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Session.GetISession().Query<TEntity>().AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        #endregion


        public IQueryable<TEntity> Include(Expression<Func<TEntity, object>> subSelector)
        {
            return this.Session.GetISession().Query<TEntity>().Fetch(subSelector);
        }
        #endregion
    }
}
