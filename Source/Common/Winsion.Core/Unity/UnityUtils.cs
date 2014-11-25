using System;
using Microsoft.Practices.Unity;
using System.Web;

using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using Microsoft.Practices.ServiceLocation;

namespace Winsion.Core.Unity
{
    [Obsolete("已过时", false)]
    public static class UnityUtils
    {
    
        /// <summary>
        /// get WcfUnityContainer or CurrentHttpContainer
        /// </summary>
        /// <returns></returns>
        public static IUnityContainer GetContainer()
        {
            IUnityContainer _container=null;
            try
            {
                _container = ServiceLocator.Current.GetInstance<IUnityContainer>();
                return _container;
            }
            catch (Exception ex)
            {
                log.Error("GetContainer，ServiceLocator.Current.GetInstance<IUnityContainer>()", ex);
                return null;
            }           
        }

        public static UnityConfigurationSection GetUnityCfgSection()
        {
            try
            {
                UnityConfigurationSection unitySection;
                unitySection = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                if (unitySection == null)
                {
                    string unityConfigFilename = Helper.GetCurrentDomainFilePathFor("unity.config");
                    var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = unityConfigFilename };
                    var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    unitySection = (UnityConfigurationSection)configuration.GetSection("unity");
                }

                return unitySection;
            }
            catch (Exception ex)
            {

                log.Error("UnityUtils.GetUnityCfgSection 获取容器（IUnityContainer）配置节发生错误，请核实 bin 目录是否存在 unity.config 文件，或 web.config 是否存在 unity 节。", ex);
                throw ex;
            }

        }

        private static readonly ILog log = new Logger(typeof(UnityUtils));

    }
}
