using System;
using System.Collections.Generic;
using System.ServiceModel;

using Microsoft.Practices.Unity;
using System.Configuration;
using Winsion.Core;
using System.Linq;
using System.ServiceModel.Channels;
using Microsoft.Practices.ServiceLocation;
using System.ServiceModel.Description;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections.ObjectModel;

namespace Winsion.Core.WCF
{

    public abstract class HostHelper
    {
        public static void HostService(ServiceDescription serviceDescription, params Uri[] baseAddresses)
        {
            ServiceHost host = null;
            try
            {
                //Uri[] tempList = null;
                //lock (_lockobj)
                //{
                //    tempList = _hostList.SelectMany(p => p.BaseAddresses).Distinct().ToArray();
                //}
                //var addrList = new List<Uri>();
                ////排除当前进程以外已经使用的端口号地址
                //foreach (var li in baseAddresses)
                //{
                //    //同一个进程内允许相同的基地址监听多个服务
                //    if (tempList.Contains(li) == false && li.Port > 0 && IsPortInUsing(li.Port))
                //    {
                //        _log.WarnFormat("HostService uri={0} 该地址的端口号已经被其他程序占用。", li);
                //        continue;
                //    }
                //    addrList.Add(li);
                //}
                //if (addrList.Count > 0)
                //{
                //    host = StartHost(serviceDescription, addrList.ToArray());
                //}
                host = StartHost(serviceDescription, baseAddresses);
            }
            catch (AddressAlreadyInUseException ex)
            {
                _log.Warn("HostService AddressAlreadyInUseException", ex);
                var addr = baseAddresses.Where(p => p.Scheme.ToLower() == "net.pipe").ToArray();
                try
                {
                    host = StartHost(serviceDescription, addr);
                }
                catch (Exception ex2)
                {
                    _log.Error("HostService ", ex2);
                    return;
                }
            }
            catch (Exception ex)
            {
                _log.Error("HostService ", ex);
                return;
            }
            serviceDescription.OnServiceHostCompleted(host);
            _hostList.Add(new ServiceHostFacade(host, serviceDescription));
            if (_log.IsEnabled(1))
            {
                foreach (var li in host.Description.Endpoints)
                {
                    _log.InfoFormat("host 服务成功，服务类型={0}，绑定配置={1},地址={2}",
                        serviceDescription.ServiceImpl.FullName,
                        serviceDescription.BindingConfigName,
                        li.ListenUri);
                }
            }
        }

        public static void HostServices(IList<ServiceDescription> serviceList, params Uri[] baseAddresses)
        {
            foreach (var s in serviceList)
            {
                //匿名委托，此处用到多线程，必须通过Tuple state参数传递值，如果通过闭包引用外部变量会发生问题。
                ThreadPool.QueueUserWorkItem(p =>
                {
                    var arr = (Tuple<ServiceDescription, Uri[]>)p;
                    HostService(arr.Item1, arr.Item2);
                }, new Tuple<ServiceDescription, Uri[]>(s, baseAddresses));
            }
        }

        public static void HostServices(IList<ServiceDescription> serviceList)
        {
            var _baseAddresses = EndpointHelper.GetBaseAddress();
            if (_baseAddresses == null)
            {
                throw new ArgumentNullException("_baseAddresses", "请在配置文件中添加 WcfHostBaseAddress 默认基地址");
            }
            HostServices(serviceList, _baseAddresses);
        }

        public static void CloseHosts()
        {
            var list = _hostList.ToList();
            foreach (var host in list)
            {
                try
                {
                    host.Close();
                    _hostList.Remove(host);
                }
                catch (Exception ex)
                {
                    _log.Error("CloseHosts ", ex);
                    continue;
                }
            }
        }

        public static Uri[] GetBaseAddress(string baseAddressKey)
        {
            return EndpointHelper.GetBaseAddress(baseAddressKey);
        }

        public static bool IsPortInUsing(int port)
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            var isOk = tcpEndPoints.Any(p => p.Port == port);
            return isOk;
        }

        public static int GetFirstAvailablePort(int minPort, int maxPort)
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            List<int> usedPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();
            int unusedPort = 0;
            for (int port = minPort; port < maxPort; port++)
            {
                if (!usedPorts.Contains(port))
                {
                    unusedPort = port;
                    break;
                }
            }
            return unusedPort;
        }

        public static IList<ServiceHostFacade> Hosts { get { return _hostList; } }

        internal static ServiceHost StartHost(ServiceDescription serviceDescription, params Uri[] baseAddresses)
        {
            var host = new ServiceHost(serviceDescription.ServiceImpl, baseAddresses);
            if (serviceDescription.IsEnableMetadata)
            {
                var smb = host.Description.Behaviors.OfType<ServiceMetadataBehavior>().FirstOrDefault();
                if (smb == null)
                {
                    smb = new ServiceMetadataBehavior();
                    host.Description.Behaviors.Add(smb);
                }
            }
            var contractList = GetContractList(serviceDescription.ServiceImpl);
            foreach (var addr in baseAddresses)
            {
                Binding binding = EndpointHelper.GetBinding(addr, serviceDescription.BindingConfigName);
                if (binding == null)
                {
                    continue;
                }
                foreach (var c in contractList)
                {
                    var contractAddr = EndpointHelper.GetRelativeAddress(c.Item2, c.Item1);
                    host.AddServiceEndpoint(c.Item1, binding, contractAddr);
                    if (serviceDescription.IsEnableMetadata)
                    {
                        var mexEndpointAddress = new EndpointAddress(string.Format("{0}/{1}/mex", addr.AbsoluteUri.TrimEnd('/'), contractAddr.OriginalString));
                        var mexServiceEndpoint = new ServiceMetadataEndpoint(binding, mexEndpointAddress);
                        host.AddServiceEndpoint(mexServiceEndpoint);
                    }
                }
            }
            host.Open();
            return host;
        }

        private static IList<Tuple<Type, ServiceContractAttribute>> GetContractList(Type impl)
        {
            IList<Tuple<Type, ServiceContractAttribute>> list = new List<Tuple<Type, ServiceContractAttribute>>();
            var temp = impl.GetInterfaces();
            var scType = typeof(ServiceContractAttribute);
            foreach (var li in temp)
            {
                if (Attribute.IsDefined(li, scType, false))
                {
                    var attr = (ServiceContractAttribute)Attribute.GetCustomAttribute(li, scType, false);
                    list.Add(new Tuple<Type, ServiceContractAttribute>(li, attr));
                }
            }
            return list;
        }

        private static readonly IList<ServiceHostFacade> _hostList = new ObservableCollection<ServiceHostFacade>();
        private static readonly ILog _log = new Logger(typeof(HostHelper));
    }

    public class ServiceDescription
    {
        public ServiceDescription(Type serviceImpl)
            : this(serviceImpl, null)
        {
        }

        public ServiceDescription(Type serviceImpl, string bindingConfigName)
        {
            this.ServiceImpl = serviceImpl;
            this.BindingConfigName = bindingConfigName;
            this.IsEnableMetadata = false;
        }

        public Type ServiceImpl { get; private set; }

        public string BindingConfigName { get; private set; }

        public bool IsEnableMetadata { get; set; }

        public event Action<ServiceDescription, ServiceHost> ServiceHostCompleted;

        internal void OnServiceHostCompleted(ServiceHost host)
        {
            var handler = ServiceHostCompleted;
            if (handler != null)
            {
                try
                {
                    handler(this, host);
                }
                catch { }
            }
        }
    }
}
