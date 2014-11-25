using System;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System.Windows;
using Microsoft.Practices.Prism.Events;


namespace Winsion.Core.Prism
{
  
    public class ServiceStartBootstrapper : UnityBootstrapper
    {
        public ServiceStartBootstrapper()
            : this(new UnityContainer())
        {

        }

        private IUnityContainer unityContainer;
        public ServiceStartBootstrapper(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer ?? new UnityContainer();
        }

        private readonly ILoggerFacade logger = new BootstrapperLogger();

        protected override DependencyObject CreateShell()
        {
            this.logger.Log("+++CreateShell", Category.Info, Priority.Medium);
            return null;
        }



        protected override ILoggerFacade CreateLogger()
        {
            this.logger.Log("+++CreateLogger", Category.Info, Priority.Medium);
            return logger;
        }


        protected override IUnityContainer CreateContainer()
        {
            this.logger.Log("+++CreateContainer", Category.Info, Priority.Medium);
            return unityContainer;
        }

        protected override void ConfigureContainer()
        {
            this.logger.Log("+++ConfigureContainer", Category.Info, Priority.Medium);

            this.Container.AddNewExtension<UnityBootstrapperExtension>();
            this.Container.RegisterInstance<ILoggerFacade>(Logger);
            this.Container.RegisterInstance(this.ModuleCatalog);
            RegisterTypeIfMissing(typeof(IServiceLocator), typeof(UnityServiceLocatorAdapter), true);
            RegisterTypeIfMissing(typeof(IModuleInitializer), typeof(ModuleInitializer), true);
            RegisterTypeIfMissing(typeof(IModuleManager), typeof(ModuleManager), true);
            RegisterTypeIfMissing(typeof(IEventAggregator), typeof(EventAggregator), true);
        }


        protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            this.logger.Log("+++ConfigureDefaultRegionBehaviors", Category.Info, Priority.Medium);

            return null;
        }

        protected override Microsoft.Practices.Prism.Regions.RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            this.logger.Log("+++ConfigureRegionAdapterMappings", Category.Info, Priority.Medium);

            return null;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            this.logger.Log("+++CreateModuleCatalog", Category.Info, Priority.Medium);

            return new AggregateModuleCatalog();
        }


        protected override void ConfigureModuleCatalog()
        {
            this.logger.Log("+++ConfigureModuleCatalog", Category.Info, Priority.Medium);

            var path = string.Format(@"{0}\plugins", AppDomain.CurrentDomain.BaseDirectory);
            if (System.IO.Directory.Exists(path))
            {
                this.logger.Log(string.Format("ConfigureModuleCatalog load plugins, path={0}", path), Category.Info, Priority.Medium);
            }
            else
            {
                this.logger.Log(string.Format("None plugins directory, path={0}", path), Category.Info, Priority.Medium);
            }
            DirectoryModuleCatalog directoryCatalog = new DirectoryModuleCatalog() { ModulePath = path };
            ((AggregateModuleCatalog)ModuleCatalog).AddCatalog(directoryCatalog);

        }
    }
}
