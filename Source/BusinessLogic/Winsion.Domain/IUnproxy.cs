using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winsion.Domain
{
    public interface IUnproxy
    {
        T UnproxyEntity<T>(T persistentObject);

        T UnproxyCollection<T>(T persistenCollection);
    }
}
