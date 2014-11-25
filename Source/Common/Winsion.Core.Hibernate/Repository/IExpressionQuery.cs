using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;



namespace Winsion.Core.Hibernate.Repository
{
    public interface IExpressionQuery<TEntity> : ICommon<TEntity>, IDisposable
    {
        IQueryOver<TEntity, TEntity> QueryOver();

        IQueryOver<TEntity, TEntity> QueryOver(System.Linq.Expressions.Expression<Func<TEntity>> alias);
    }
}
