using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winsion.ServiceProxy.Utils
{
    public interface IServiceFactory
    {
        IProxy<TService> GetProxy<TService>()
         where TService : class;

        IProxy<TService> GetProxy<TService>(BindingConfig bindingConfig)
            where TService : class;

        IProxy<TService> GetProxy<TService>(string endpointConfigurationName)
          where TService : class;

        IProxy<TService> GetProxy<TService>(Uri baseAddress)
         where TService : class;

        IProxy<TService> GetSecurityProxy<TService>()
          where TService : class;


        IProxy<TService> GetSecurityProxy<TService>(BindingConfig bindingConfig)
           where TService : class;


        IProxy<TService> GetSecurityProxy<TService>(string endpointConfigurationName)
           where TService : class;

        IProxy<TService> GetSecurityProxy<TService>(Uri baseAddress)
          where TService : class;

        IDuplexProxy<TService> GetDuplexProxy<TService>(object callback)
            where TService : class;

        IDuplexProxy<TService> GetDuplexProxy<TService>(object callback, Uri baseAddress)
            where TService : class;

    }

    public class BindingConfig
    {
        public BindingConfig(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("BindingConfig:[name={0}]", Name);
        }
    }
}
