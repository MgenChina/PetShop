using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winsion.Core.Hibernate.Repository
{
    public interface ICommon<TEntity> : IDisposable
    {
        System.Type Type { get; }

        INHibernateSession Session { get; }
    }
}
