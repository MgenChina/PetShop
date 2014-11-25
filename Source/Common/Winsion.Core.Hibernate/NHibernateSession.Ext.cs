/** $Id$ $DateTime$
 *  @file NHibernateSession.Ext.cs
 *  @brief 
 *  @version 0.0.1
 *  @since 0.0.1
 *  @author zhanglin<zhanglin@bstar.com.cn> 
 *  @date 2012-7-26    Created it
 */
/******************************************************************************
*@note
    Copyright 2007, BeiJing Bluestar Corporation, Limited ALL RIGHTS RESERVED
Permission is hereby granted to licensees of BeiJing Bluestar, Inc. products
to use or abstract this computer program for the sole purpose of implementing
a product based on BeiJing Bluestar, Inc. products. No other rights to reproduce, 
use, or disseminate this computer program,whether in part or in whole, are 
granted. BeiJing Bluestar, Inc. makes no representation or warranties with 
respect to the performance of this computer program, and specifically disclaims
any responsibility for any damages, special or consequential, connected with the
use of this program.For details, see http://www.bstar.com.cn/ 
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Winsion.Core.Hibernate
{
    public class DbConnectionObject : IDbConnectionObject
    {
        /// <summary>
        /// default ProviderName is "System.Data.SqlClient"
        /// </summary>        
        public DbConnectionObject(string connectionString)
            : this(connectionString, DbProviders.MSSql)
        {
        }

        public DbConnectionObject(string connectionString, string providerName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString is null or empty");
            }

            var temp = providerName;
            temp = string.IsNullOrEmpty(temp) ? DbProviders.MSSql : temp;

            this.connectionString = connectionString;
            this.providerName = temp;
        }

        public override string ToString()
        {
            return string.Format("connectionString={0} providerName={1}", ConnectionString, ProviderName);
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }

            private set
            {
                connectionString = value;
            }
        }

        public string ProviderName
        {
            get
            {
                return providerName;
            }

            private set
            {
                providerName = value;
            }
        }

        private string connectionString;
        private string providerName;
    }

    public struct DbProviders
    {
        public const string MSSql = "System.Data.SqlClient";
        public const string MSOracleSql = "System.Data.OracleClient";
    }


    internal partial class NHibernateSession
    {
        #region Constructor & Destructor

        public NHibernateSession()
        {
            this.nHibernateSessionType = NHibernateSessionType.DefaultConfig;
        }

        /// <summary>
        /// dbProviderName ,default value is "System.Data.SqlClient"
        /// </summary>
        /// <param name="dbConnectionObj"></param>
        public NHibernateSession(IDbConnectionObject dbConnectionObj)
        {
            this.nHibernateSessionType = NHibernateSessionType.NewDbConnection;
            this.dbConnectionObj = dbConnectionObj;
        }

        public NHibernateSession(string nHibernateCfgFileName)
        {
            if (string.IsNullOrEmpty(nHibernateCfgFileName))
            {
                throw new ArgumentNullException("nHibernateCfgFileName is null or empty");
            }

            string filename = Helper.GetCurrentDomainFilePathFor(nHibernateCfgFileName);
            if (filename == null || filename == "")
            {
                throw new System.IO.FileNotFoundException("none nHibernateCfgFile, path = " + nHibernateCfgFileName);
            }


            this.nHibernateCfgFileName = filename;
            this.nHibernateSessionType = NHibernateSessionType.NewConfigFile;
        }

        ~NHibernateSession()
        {
            Dispose(true);
        }

        #endregion

        #region INHibernateSessionExt
        NHibernateSessionType INHibernateSessionExt.SessionType
        {
            get { return nHibernateSessionType; }
        }

        string INHibernateSessionExt.CfgFileName
        {
            get
            {
                return nHibernateCfgFileName;
            }
        }

        IDbConnectionObject INHibernateSessionExt.DbConnectionObj
        {
            get { return dbConnectionObj; }
        }

        System.Data.IDbConnection INHibernateSessionExt.DbConnection
        {
            get { return dbConnection; }
        }
        #endregion

        private static System.Data.IDbConnection CreateConnection(string dbConnectionString, string dbProviderName)
        {
            try
            {
                System.Data.Common.DbProviderFactory dbfactory = System.Data.Common.DbProviderFactories.GetFactory(dbProviderName);
                System.Data.Common.DbConnection dbconn = dbfactory.CreateConnection();
                dbconn.ConnectionString = dbConnectionString;
                return dbconn;
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("CreateConnection方法，dbConnectionString={0}，dbProviderName={1}", dbConnectionString, dbProviderName), ex);
                }

                throw ex;
            }
        }
    }
}
