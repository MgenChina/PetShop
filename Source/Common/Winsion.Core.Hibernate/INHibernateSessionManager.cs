using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Winsion.Core.Hibernate
{
    public enum NHSessionContextType
    {
        None = 0,
        Web = 1,
        Call = 2,
        WCF = 3,
    }

    public partial interface INHibernateSessionManager : IDisposable
    {
        // Methods
        ISession CreateISession(INHibernateSessionExt session);

        INHibernateSession GetSession(IDbConnectionObject dbConnectionObj);

        INHibernateSession GetSession(string nHibernateCfgFileName);
        
        INHibernateSession GetSession();

        void RemoveSession(INHibernateSessionExt session);       
     
        void CloseContextSessions();

        NHSessionContextType ContextType { get; }
    }

    public interface ISessionFactoryProvider
    {
        ISessionFactory GetSessionFactory();

        ISessionFactory GetSessionFactory(string cfgFileName);
    }
}
