using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace Winsion.Core.WCF
{
    public class ServiceHostFacade
    {
        public ServiceHostFacade(ServiceHost host, ServiceDescription serviceDesc)
        {
            this.ServiceDescription = serviceDesc;
            this.Host = host;
            this.Host.Closed += new EventHandler(Host_Closed);
            this.Host.Faulted += new EventHandler(Host_Faulted);
        }

        public ServiceHostFacade(ServiceDescription serviceDesc, params Uri[] baseAddresses)
            : this(new ServiceHost(serviceDesc.ServiceImpl, baseAddresses), serviceDesc)
        {
        }

        public ServiceDescription ServiceDescription { get; private set; }

        public ServiceHost Host { get; private set; }

        public void Open()
        {
            Host.Open();
        }

        public void Close()
        {            
            Host.Close();
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            _log.FatalFormat("Host_Faulted ServiceType={0}", ServiceDescription.ServiceImpl.FullName);
            try
            {
                Host.Closed -= new EventHandler(Host_Closed);
                Host.Faulted -= new EventHandler(Host_Faulted);
                Host.Abort();
                Host = HostHelper.StartHost(ServiceDescription, Host.BaseAddresses.ToArray());
                if (Host != null)
                {
                    Host.Closed += new EventHandler(Host_Closed);
                    Host.Faulted += new EventHandler(Host_Faulted);
                }
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Host_Faulted HostHelper.StartHost ServiceType={0}", ex, ServiceDescription.ServiceImpl.FullName);
            }
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            _log.FatalFormat("Host_Closed ServiceType={0}", ServiceDescription.ServiceImpl.FullName);
        }

        private static readonly ILog _log = new Logger(typeof(ServiceHostFacade));
    }
}
