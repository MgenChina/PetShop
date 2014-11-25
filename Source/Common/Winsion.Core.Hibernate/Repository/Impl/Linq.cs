
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace Winsion.Core.Hibernate.Repository.Impl
{
    public class Linq<TEntity> : Common<TEntity>, ILinq<TEntity>
    {
        #region Constructors

        public Linq()
            : base()
        {
        }

        public Linq(INHibernateSession session)
            : base(session)
        {
        }

        

        #endregion

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
    }
}
