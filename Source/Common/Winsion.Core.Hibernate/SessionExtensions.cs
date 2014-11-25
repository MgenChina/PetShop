using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Winsion.Core.Hibernate
{
    public static class SessionExtensions
    {
        public static void Delete<TEntity>(this ISession session, object id)
        {
            var queryString = string.Format("delete {0} where Id = :id",
                                            typeof(TEntity));
            session.CreateQuery(queryString)
                   .SetParameter("id", id)
                   .ExecuteUpdate();
        }

        public static void Delete<TEntity>(this ISession session, IList<int> idList)
        {
            var queryString = string.Format("delete {0} where Id in (:idList)",
                                            typeof(TEntity));
            session.CreateQuery(queryString)
                  .SetParameterList("idList", idList)
                   .ExecuteUpdate();
        }

        public static void DeleteByFK<TEntity>(this ISession session, string fkPropertyName, object fk)
        {
            var queryString = string.Format("delete {0} x where x.{1}.Id = :fk", typeof(TEntity), fkPropertyName);
            session.CreateQuery(queryString)
                   .SetParameter("fk", fk)
                   .ExecuteUpdate();
        }
    }
}
