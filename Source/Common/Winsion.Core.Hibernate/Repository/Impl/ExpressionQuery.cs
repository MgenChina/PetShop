
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;


namespace Winsion.Core.Hibernate.Repository.Impl
{
    public class ExpressionQuery<TEntity> : Common<TEntity>, IExpressionQuery<TEntity>
     where TEntity : class
    {
        #region Constructors

        public ExpressionQuery()
            : base()
        { 
        }

        public ExpressionQuery(INHibernateSession session)
            : base(session)
        {
        }



        #endregion


        public IQueryOver<TEntity, TEntity> QueryOver()
        {
            return Session.GetISession().QueryOver<TEntity>();
        }

        public IQueryOver<TEntity, TEntity> QueryOver(System.Linq.Expressions.Expression<Func<TEntity>> alias)
        {
            return Session.GetISession().QueryOver<TEntity>(alias);
        }
    }
}
