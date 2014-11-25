using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winsion.Core.Hibernate.Repository;

namespace Winsion.Core.Hibernate
{
    public interface INHFactory
    {
        /// <summary>
        /// new 一个INHibernateSession，使用默认hibernate.cfg.xml 文件作为连接数据库的配置
        /// </summary>
        /// <returns>返回一个新的INHibernateSession对象</returns>
        INHibernateSession GetNewSession();

        /// <summary>
        ///  new 一个INHibernateSession，使用默认hibernate.cfg.xml 文件作为连接数据库的配置,使用dbConnectionString作为连接字符串。
        /// </summary>
        /// <param name="dbConnectionString">连接字符串</param>
        /// <returns>返回一个使用hibernate.cfg.xml配置，dbConnectionString作为连接字符串的INHibernateSession对象</returns>
        INHibernateSession GetNewSession(string dbConnectionString);

        /// <summary>
        /// new 一个INHibernateSession，使用默认hibernate.cfg.xml 文件作为连接数据库的配置,使用dbConnectionObj作为连接字符串配置。
        /// </summary>
        /// <param name="dbConnectionObj">连接字符串配置，包括ProviderName，默认是DbProviders.MSSql</param>
        /// <returns>返回一个使用hibernate.cfg.xml配置，dbConnectionObj作为连接字符串配置的INHibernateSession对象</returns>
        INHibernateSession GetNewSession(IDbConnectionObject dbConnectionObj);

        /// <summary>
        /// new 一个INHibernateSession，使用hibernateCfgFileName 文件作为连接数据库的配置。
        /// </summary>
        /// <param name="hibernateCfgFileName">连接数据库的配置文件</param>
        /// <returns>返回一个使用hibernateCfgFileName配置的INHibernateSession对象</returns>
        INHibernateSession GetNewSessionForCfgFile(string hibernateCfgFileName);

        /// <summary>
        /// 在当前请求上下文中获取INHibernateSession对象，如果已经存在，则返回同一个对象，否则返回一个新对象，
        /// 该对象使用默认hibernate.cfg.xml 文件作为连接数据库配置。
        /// </summary>
        /// <returns>返回INHibernateSession对象</returns>
        INHibernateSession GetSession();

        /// <summary>
        ///  在当前请求上下文中获取INHibernateSession对象，如果已经存在，则返回同一个对象，否则返回一个新对象，
        ///  该对象使用默认hibernate.cfg.xml 文件作为连接数据库的配置,使用dbConnectionString作为连接字符串。
        /// </summary>
        /// <param name="dbConnectionString">连接字符串</param>
        /// <returns>返回INHibernateSession对象</returns>
        INHibernateSession GetSession(string dbConnectionString);

        /// <summary>
        /// 在当前请求上下文中获取INHibernateSession对象，如果已经存在，则返回同一个对象，否则返回一个新对象，
        /// 该对象使用默认hibernate.cfg.xml 文件作为连接数据库的配置,使用dbConnectionObj作为连接字符串配置。
        /// </summary>
        /// <param name="dbConnectionObj">连接字符串配置，包括ProviderName，默认是DbProviders.MSSql</param>
        /// <returns>返回INHibernateSession对象</returns>
        INHibernateSession GetSession(IDbConnectionObject dbConnectionObj);

        /// <summary>
        /// 在当前请求上下文中获取INHibernateSession对象，如果已经存在，则返回同一个对象，否则返回一个新对象，
        /// 该对象使用hibernateCfgFileName 文件作为连接数据库的配置。
        /// </summary>
        /// <param name="hibernateCfgFileName">连接数据库的配置文件</param>
        /// <returns>返回INHibernateSession对象</returns>
        INHibernateSession GetSessionForCfgFile(string hibernateCfgFileName);

        INHibernateSessionManager GetSessionManager();

        INHibernateSessionManager GetSessionManager(NHSessionContextType contextType);

        IRepository<TEntity> GetRepositoryFor<TEntity>()
            where TEntity : class, new();

        IRepository<TEntity> GetRepositoryFor<TEntity>(INHibernateSession session)
            where TEntity : class, new();


        IViewRepository<TEntity> ViewRepositoryFor<TEntity>()
            where TEntity : class, new();

        IViewRepository<TEntity> ViewRepositoryFor<TEntity>(INHibernateSession session)
            where TEntity : class, new();
    }
}
