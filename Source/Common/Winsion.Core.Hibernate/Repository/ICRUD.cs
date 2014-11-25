using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Winsion.Core.Hibernate.Repository
{
    public interface ICRUD<TEntity> : ICommon<TEntity>, IDisposable
    {
        // CRUD Methods
        object Save(TEntity entity);

        void SaveOrUpdate(TEntity entity);

        void Delete(TEntity entity);

        void Update(TEntity entity);

        void Refresh(TEntity entity);

        void Evict(TEntity entity);
    }
}
