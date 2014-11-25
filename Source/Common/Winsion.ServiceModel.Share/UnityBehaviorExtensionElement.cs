using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;

using log4net;

namespace Winsion.ServiceModel.Share
{
    public class UnityBehaviorExtensionElement : System.ServiceModel.Configuration.BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get
            {
                return typeof(UnityServiceBehaviorAttribute);
            }
        }

        protected override object CreateBehavior()
        {
            var usb = new UnityServiceBehaviorAttribute();
            return usb;
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(UnityBehaviorExtensionElement));
  
    }
}
