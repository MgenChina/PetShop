using System;
using System.ServiceModel.Dispatcher;

using System.ServiceModel.Description;
using Microsoft.Practices.Unity;

using log4net;

namespace Winsion.ServiceModel.Share
{
    public class UnityServiceBehaviorAttribute : Attribute, IServiceBehavior
    {
        public UnityServiceBehaviorAttribute()
        {           
            this.InstanceProvider = new UnityInstanceProvider();          
        }

               
        #region IServiceBehavior Members

        public void AddBindingParameters(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<System.ServiceModel.Description.ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher cd = cdb as ChannelDispatcher;

                if (cd != null)
                {
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        //Indicates to instance provider the Service Type
                        this.InstanceProvider.ServiceType = serviceDescription.ServiceType;

                        //Sets the UnityInstanceProvider Foreach EndpointDispatcher
                        ed.DispatchRuntime.InstanceProvider = this.InstanceProvider;
                    }
                }
            }
        }

        public void Validate(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {

        }

        #endregion


        /// <summary>
        /// The Instance Provider Reference
        /// </summary>
        private UnityInstanceProvider InstanceProvider { get; set; }

   
        private static readonly ILog log = LogManager.GetLogger(typeof(UnityServiceBehaviorAttribute));
    }
}


