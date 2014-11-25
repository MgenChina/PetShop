using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using NHibernate;

namespace Winsion.Core.Hibernate.Repository
{
    public interface IGet<TEntity> : ICommon<TEntity>, IDisposable
    {
        // Get Methods
        TEntity GetById<TKey>(TKey Id);

        IList<TEntity> GetAll();

        IList<TEntity> GetAll(int maxResults);

        IList<TEntity> GetByCriteria(params ICriterion[] criterionList);

        IList<TEntity> GetByCriteria(int maxResults, params ICriterion[] criterionList);

        TEntity GetUniqueByCriteria(params ICriterion[] criterionList);

        IList<TEntity> GetByExample(TEntity exampleObject, params string[] excludePropertyList);

        IList<TEntity> GetByQuery(string query);

        IList<TEntity> GetByQuery(int maxResults, string query);

        TEntity GetUniqueByQuery(string query);

        // Misc Methods
        void SetFetchMode(string associationPath, FetchMode mode);

        ICriteria CreateCriteria();
    }
}
