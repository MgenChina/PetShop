using System;
using System.Collections.Generic;
using System.Text;



namespace Winsion.Core.Hibernate.Repository
{
    public partial interface IRepository<TEntity> : IViewRepository<TEntity>, ICRUD<TEntity>, IDisposable
    {
    }   
}
