using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using Microsoft.Practices.Unity;




namespace Winsion.ServiceModel.Share
{
    public class UnityServiceHostFactory : System.ServiceModel.Activation.ServiceHostFactory
    {
        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            //Creates the unity service host
            UnityServiceHost unityServiceHost = new UnityServiceHost(serviceType, baseAddresses);

            return unityServiceHost;
        }

    
    }



    public class UnityServiceHost : System.ServiceModel.ServiceHost
    {     

        public UnityServiceHost()
            : base()
        {
        }

        public UnityServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        protected override void OnOpening()
        {
            //Verifies if the UnityServiceBehavior is already attached
            if (this.Description.Behaviors.Find<UnityServiceBehaviorAttribute>() == null)
            {
                //Attach the UnityServiceBehavior
                this.Description.Behaviors.Add(new UnityServiceBehaviorAttribute());
            }

            base.OnOpening();
        }
    }


}



