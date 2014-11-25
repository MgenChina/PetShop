using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;

using Winsion.Core.WCF.Context;
using Winsion.ServiceModel.Share.Security;
using Winsion.ServiceModel.Share.Context;


namespace Winsion.ServiceProxy.Utils.ChannelFactory
{
    public class SecurityCallStackChannelFactory<T> : HeaderChannelFactory<T, SecurityCallStack>
        where T : class
    {
        public SecurityCallStackChannelFactory()
            : base()
        {
            InitializeCallStack();
        }

        public SecurityCallStackChannelFactory(string endpointName)
            : base(endpointName)
        {
            InitializeCallStack();
        }

        public SecurityCallStackChannelFactory(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
            InitializeCallStack();
        }



        void InitializeCallStack()
        {
            if (OperationContext.Current != null)
            {
                Header = SecurityCallStackContext.Current;

                if (Header == null)
                {
                    Header = new SecurityCallStack();
                }
            }
            else
            {
                Header = new SecurityCallStack();
            }
        }
        protected override void PreInvoke(ref Message reply)
        {
            Header.AppendCall();
            base.PreInvoke(ref reply);
        }
    }
}
