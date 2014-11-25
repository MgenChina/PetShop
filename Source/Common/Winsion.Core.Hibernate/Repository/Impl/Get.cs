
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using NHibernate;

namespace Winsion.Core.Hibernate.Repository.Impl
{
    public class Get<TEntity> : Common<TEntity>, IGet<TEntity>
    {
        #region Declarations


        protected const int defaultMaxResults = 100;


        private Dictionary<string, FetchMode> fetchModeMap = new Dictionary<string, FetchMode>();

        #endregion

        #region Constructors

        public Get()
            : base()
        { 
        }

        public Get(INHibernateSession session)
            : base(session)
        {
        }

        

        #endregion


        #region Get Methods

        public virtual TEntity GetById<TKey>(TKey id)
        {
            return (TEntity)Session.GetISession().Get(typeof(TEntity), id);
        }

        public IList<TEntity> GetAll()
        {
            return GetByCriteria(defaultMaxResults);
        }

        public IList<TEntity> GetAll(int maxResults)
        {
            return GetByCriteria(maxResults);
        }

        public IList<TEntity> GetByCriteria(params ICriterion[] criterionList)
        {
            return GetByCriteria(defaultMaxResults, criterionList);
        }

        public IList<TEntity> GetByCriteria(int maxResults, params ICriterion[] criterionList)
        {
            ICriteria criteria = CreateCriteria().SetMaxResults(maxResults);

            foreach (ICriterion criterion in criterionList)
            {
                criteria.Add(criterion);
            }

            return criteria.List<TEntity>();
        }

        public TEntity GetUniqueByCriteria(params ICriterion[] criterionList)
        {
            ICriteria criteria = CreateCriteria();

            foreach (ICriterion criterion in criterionList)
            {
                criteria.Add(criterion);
            }

            return criteria.UniqueResult<TEntity>();
        }

        public IList<TEntity> GetByExample(TEntity exampleObject, params string[] excludePropertyList)
        {
            ICriteria criteria = CreateCriteria();
            Example example = Example.Create(exampleObject);

            foreach (string excludeProperty in excludePropertyList)
            {
                example.ExcludeProperty(excludeProperty);
            }

            criteria.Add(example);

            return criteria.List<TEntity>();
        }

        public IList<TEntity> GetByQuery(string query)
        {
            return GetByQuery(defaultMaxResults, query);
        }

        public IList<TEntity> GetByQuery(int maxResults, string query)
        {
            IQuery iQuery = Session.GetISession().CreateQuery(query).SetMaxResults(maxResults);
            return iQuery.List<TEntity>();
        }

        public TEntity GetUniqueByQuery(string query)
        {
            IQuery iQuery = Session.GetISession().CreateQuery(query);
            return iQuery.UniqueResult<TEntity>();
        }

        #endregion

        #region Misc Methods

        public void SetFetchMode(string associationPath, FetchMode mode)
        {
            if (!fetchModeMap.ContainsKey(associationPath))
            {
                fetchModeMap.Add(associationPath, mode);
            }
        }

        public ICriteria CreateCriteria()
        {
            ICriteria criteria = Session.GetISession().CreateCriteria(typeof(TEntity));

            foreach (KeyValuePair<string, FetchMode> pair in fetchModeMap)
            {
                criteria = criteria.SetFetchMode(pair.Key, pair.Value);
            }

            return criteria;
        }

        #endregion
    }
}
