using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winsion.ServiceProxy.Utils
{
    public class ServiceFactory : IServiceFactory
    {
        public IProxy<TService> GetProxy<TService>()
            where TService : class
        {
            return new Proxy<TService>();
        }

        public IProxy<TService> GetProxy<TService>(BindingConfig bindingConfig)
            where TService : class
        {
            return new Proxy<TService>(bindingConfig);
        }

        public IProxy<TService> GetProxy<TService>(string endpointConfigurationName)
            where TService : class
        {
            return new Proxy<TService>(endpointConfigurationName);
        }



        public IProxy<TService> GetSecurityProxy<TService>()
           where TService : class
        {
            return new SecurityProxy<TService>();
        }

        public IProxy<TService> GetSecurityProxy<TService>(BindingConfig bindingConfig)
            where TService : class
        {
            return new SecurityProxy<TService>(bindingConfig);
        }

        public IProxy<TService> GetSecurityProxy<TService>(string endpointConfigurationName)
            where TService : class
        {
            return new SecurityProxy<TService>(endpointConfigurationName);
        }


        public IProxy<TService> GetProxy<TService>(Uri baseAddress) where TService : class
        {
            return new Proxy<TService>(baseAddress);
        }

        public IProxy<TService> GetSecurityProxy<TService>(Uri baseAddress) where TService : class
        {
            return new SecurityProxy<TService>(baseAddress);
        }


        public IDuplexProxy<TService> GetDuplexProxy<TService>(object callback) where TService : class
        {
            var temp = new DuplexProxy<TService>(callback);
            return temp;
        }

        public IDuplexProxy<TService> GetDuplexProxy<TService>(object callback, Uri baseAddress) where TService : class
        {
            var temp = new DuplexProxy<TService>(callback, baseAddress);
            return temp;
        }
    }
}
