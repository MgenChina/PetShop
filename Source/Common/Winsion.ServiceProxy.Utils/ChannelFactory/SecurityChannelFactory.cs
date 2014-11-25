using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel.Channels;
using System.ServiceModel;

using Winsion.Core.WCF.Context;
using Winsion.ServiceModel.Share.Security;
using Winsion.ServiceModel.Share.Context;
using Winsion.ServiceProxy.Utils.Security;
using Winsion.ServiceProxy.Utils.Context;



namespace Winsion.ServiceProxy.Utils.ChannelFactory
{
    public class SecurityChannelFactory<TService> : HeaderChannelFactory<TService, UserContext>
        where TService : class
    {
        public SecurityChannelFactory()
            : base()
        {
            Initialize();
        }

        public SecurityChannelFactory(string endpointName)
            : base(endpointName)
        {
            Initialize();
        }

        public SecurityChannelFactory(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
            Initialize();
        }



        void Initialize()
        {
            if (OperationContext.Current != null)
            {
                Header = SecurityContext.Current;

                if (Header == null)
                {
                    Header = new UserContext();
                }
            }
            else
            {
                Header = new UserContext();
            }
           
        }
        protected override void PreInvoke(ref Message reply)
        {
            Header.AppendContext();
            base.PreInvoke(ref reply);
        }
    }
}
