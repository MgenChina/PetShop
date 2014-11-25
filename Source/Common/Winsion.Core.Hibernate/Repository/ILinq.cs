using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Winsion.Core.Hibernate.Repository
{
    public interface ILinq<TEntity> : ICommon<TEntity>, IQueryable<TEntity>, IDisposable
    {
        IQueryable<TEntity> Include(Expression<Func<TEntity, object>> subSelector);
    }
}
