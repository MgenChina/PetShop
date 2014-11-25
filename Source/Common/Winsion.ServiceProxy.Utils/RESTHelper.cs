using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.Web.Routing;

namespace Winsion.ServiceProxy.Utils
{
    public static class RESTHelper
    {              
        public static void RegisterRoutes(params KeyValuePair<string, Type>[] serviceTypes)
        {           
            foreach (var kv in serviceTypes)
            {
                RegisterRoutes(kv.Key, kv.Value);
            }

        }

        public static void RegisterRoutes(string routePrefix, Type serviceType)
        {
            RouteTable.Routes.Add(new ServiceRoute(routePrefix, Factory, serviceType));
        }




        public static WebServiceHostFactory Factory
        {
            get
            {
                return Nested.WebServiceHostFactory;
            }
        }    

        private class Nested
        {
            static Nested() { }
            internal static readonly WebServiceHostFactory WebServiceHostFactory =
                new WebServiceHostFactory();
        }
    }
}
