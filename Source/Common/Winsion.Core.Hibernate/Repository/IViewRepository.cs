using System;
using System.Collections.Generic;


namespace Winsion.Core.Hibernate.Repository
{
    public partial interface IViewRepository<TEntity> : IGet<TEntity>, IExpressionQuery<TEntity>, ILinq<TEntity>, IDisposable
    {
    }    
}
