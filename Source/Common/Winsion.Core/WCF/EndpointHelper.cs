using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Winsion.Core.WCF
{
    internal static class EndpointHelper
    {
        public static Uri[] GetBaseAddress(string baseAddressKey)
        {
            try
            {
                AppSettingsReader r = new AppSettingsReader();
                var addr = r.GetValue(baseAddressKey, typeof(string));
                var temp = (string)addr;
                if (string.IsNullOrEmpty(temp) == false)
                {
                    var arr = temp.Split('|');
                    var len = arr.Length;
                    Uri[] uris = null;
                    if (len > 0)
                    {
                        uris = new Uri[len];
                        for (int i = 0; i < len; i++)
                        {
                            uris[i] = new Uri(arr[i]);
                        }
                    }
                    else
                    {
                        uris = new Uri[] { new Uri(temp) };
                    }
                    return uris;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取AppSettings名为WcfHostBaseAddress的默认基地址，就算没有配置基地址也只获取一次，防止不必要的初始化。
        /// </summary>
        /// <returns></returns>
        public static Uri[] GetBaseAddress()
        {
            if (isInitedDefaultBaseAddr == false)
            {
                _defaultBaseAddr = GetBaseAddress(_defaultBaseAddrKey);
                isInitedDefaultBaseAddr = true;
            }
            return _defaultBaseAddr;
        }

        public static Uri GetAddress(Type contract, Uri baseAddress)
        {
            if (baseAddress == null)
            {
                throw new ArgumentNullException("baseAddress", string.Format("服务基地址不能为null，请在AppSettings配置{0}默认地址，或检查Uri输入参数有效。", _defaultBaseAddrKey));
            }
            var addr = GetRelativeAddress(contract);
            var uri = new Uri(baseAddress.AbsoluteUri + "\\" + addr.OriginalString);
            return uri;
        }

        public static Uri GetRelativeAddress(Type contract)
        {
            var scType = typeof(ServiceContractAttribute);
            var attr = (ServiceContractAttribute)Attribute.GetCustomAttribute(contract, scType, false);
            return GetRelativeAddress(attr, contract);
        }

        public static Uri GetRelativeAddress(ServiceContractAttribute contractAttr, Type contract)
        {
            string contractName = contract.Name;
            if (contractAttr != null && string.IsNullOrWhiteSpace(contractAttr.Name) == false)
            {
                contractName = contractAttr.Name;
            }
            var addr = contractName.TrimStart('I') + ".svc";
            return new Uri(addr, UriKind.Relative);
        }

        public static Binding GetBinding(Uri baseAddress, string bindingConfigurationName)
        {
            if (baseAddress == null)
            {
                throw new ArgumentNullException("baseAddress", string.Format("服务基地址不能为null，请在AppSettings配置{0}默认地址，或检查Uri输入参数有效。", _defaultBaseAddrKey));
            }
            Binding binding = null;
            string scheme = baseAddress.Scheme.ToLower();
            switch (scheme)
            {
                case "net.tcp":
                    {
                        NetTcpBinding tcpBinding;
                        if (string.IsNullOrEmpty(bindingConfigurationName))
                        {
                            tcpBinding = new NetTcpBinding(SecurityMode.None);

                            int max = int.MaxValue - 1;
                            tcpBinding.MaxBufferSize = max;
                            tcpBinding.MaxBufferPoolSize = max;
                            tcpBinding.MaxReceivedMessageSize = max;
                            tcpBinding.ReaderQuotas.MaxNameTableCharCount = max;
                            tcpBinding.ReaderQuotas.MaxStringContentLength = max;
                            tcpBinding.ReaderQuotas.MaxDepth = max;
                            tcpBinding.ReaderQuotas.MaxBytesPerRead = max;
                            tcpBinding.ReaderQuotas.MaxArrayLength = max;

                            tcpBinding.ReceiveTimeout = timeout;
                            tcpBinding.SendTimeout = timeout;
                            tcpBinding.OpenTimeout = timeout;
                            tcpBinding.CloseTimeout = timeout;
                        }
                        else
                        {
                            tcpBinding = new NetTcpBinding(bindingConfigurationName);
                        }

                        binding = tcpBinding;
                        break;
                    }
                case "net.pipe":
                    {
                        NetNamedPipeBinding pipeBinding;
                        if (string.IsNullOrEmpty(bindingConfigurationName))
                        {
                            pipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

                            int max = int.MaxValue - 1;
                            pipeBinding.MaxBufferSize = max;
                            pipeBinding.MaxBufferPoolSize = max;
                            pipeBinding.MaxReceivedMessageSize = max;
                            pipeBinding.ReaderQuotas.MaxNameTableCharCount = max;
                            pipeBinding.ReaderQuotas.MaxStringContentLength = max;
                            pipeBinding.ReaderQuotas.MaxDepth = max;
                            pipeBinding.ReaderQuotas.MaxBytesPerRead = max;
                            pipeBinding.ReaderQuotas.MaxArrayLength = max;

                            pipeBinding.ReceiveTimeout = timeout;
                            pipeBinding.SendTimeout = timeout;
                            pipeBinding.OpenTimeout = timeout;
                            pipeBinding.CloseTimeout = timeout;
                        }
                        else
                        {
                            pipeBinding = new NetNamedPipeBinding(bindingConfigurationName);
                        }

                        binding = pipeBinding;
                        break;
                    }
                default:
                    break;
            }

            return binding;
        }

        private static Uri[] _defaultBaseAddr = null;
        private const string _defaultBaseAddrKey = "WcfHostBaseAddress";
        private static bool isInitedDefaultBaseAddr = false;
        private static readonly TimeSpan timeout = TimeSpan.FromSeconds(15);
    }
}
