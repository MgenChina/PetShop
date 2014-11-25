using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;

namespace Winsion.ServiceProxy.Utils
{

    internal class Proxy<TService> : CodeBlockProxyBase<TService>, IProxy<TService>
        where TService : class
    {
        public Proxy()
        {
            this._channelFactoryKey = GetChannelFactoryKey();
        }

        public Proxy(BindingConfig bindingConfig)
        {
            this._bindingConfig = bindingConfig;
            this._channelFactoryKey = GetChannelFactoryKey();
        }

        public Proxy(string endpointConfigName)
        {
            this._endpointConfigName = endpointConfigName;
            this._channelFactoryKey = GetChannelFactoryKey();
        }

        public Proxy(Uri baseAddress)
        {
            this._baseAddress = baseAddress;
            this._channelFactoryKey = GetChannelFactoryKey();
        }

        ~Proxy()
        {
            Dispose(true);
        }

        #region IDisposable

        private bool _isDisposed = false;

        protected override void Dispose(bool finalizing)
        {
            if (_isDisposed == false)
            {
                _isDisposed = true;
                //if (finalizing == false)
                //{                    
                //}
                Helper.CloseChannelFactory(ref newChannelFactory);
            }
            base.Dispose(finalizing);
        }

        #endregion

        public override ChannelFactory<TService> GetChannelFactory()
        {
            if (Helper.IsInvalidChannelFactory(newChannelFactory))
            {
                if (newChannelFactory != null)
                {
                    Helper.CloseChannelFactory(ref newChannelFactory);
                }
                newChannelFactory = CreateChannelFactory();
            }
            return newChannelFactory;
        }

        protected override ChannelFactory<TService> ChannelFactory
        {
            get
            {
                if (newChannelFactory != null)
                {
                    return newChannelFactory;
                }
                else
                {
                    return GetChannelFactoryFromCache();
                }
            }
        }

        #region CreateChannelFactory
        /// <summary>
        /// 返回 channel object. The channel 尚未打开.
        /// </summary>       
        /// <returns>channel object</returns>
        private ChannelFactory<TService> GetChannelFactoryFromCache()
        {
            var key = _channelFactoryKey;
            ChannelFactory<TService> chFactory = null;
            if (!_channelPool.TryGetValue(key, out chFactory) || Helper.IsInvalidChannelFactory(chFactory))
            {
                lock (_lockObj)
                {
                    if (!_channelPool.TryGetValue(key, out chFactory) || Helper.IsInvalidChannelFactory(chFactory))
                    {
                        if (chFactory != null)
                        {
                            Helper.CloseChannelFactory(ref chFactory);
                        }
                        chFactory = CreateChannelFactory();
                        _channelPool[key] = chFactory;
                    }
                }
            }

            return chFactory;
        }

        private ChannelFactory<TService> CreateChannelFactory()
        {
            ChannelFactory<TService> chFactory;
            Uri uri = null;
            Binding binding = null;
            if (_baseAddress != null)
            {
                uri = GetAddress(typeof(TService), _baseAddress);
                binding = GetBinding(uri, null);
                chFactory = new ChannelFactory<TService>(binding, new EndpointAddress(uri));
                return chFactory;
            }
            if (string.IsNullOrEmpty(_endpointConfigName) == false)
            {
                chFactory = new ChannelFactory<TService>(_endpointConfigName);
                return chFactory;
            }
            if (_bindingConfig != null)
            {
                uri = GetAddress(typeof(TService));
                binding = GetBinding(uri, _bindingConfig);
                chFactory = new ChannelFactory<TService>(binding, new EndpointAddress(uri));
                return chFactory;
            }
            uri = GetAddress(typeof(TService));
            binding = GetBinding(uri, null);
            chFactory = new ChannelFactory<TService>(binding, new EndpointAddress(uri));
            return chFactory;
        }

        private string GetChannelFactoryKey()
        {
            string key = "";
            string _serviceTypeName = typeof(TService).FullName;
            if (_baseAddress != null)
            {
                key = string.Format("BA;{0};{1}", _serviceTypeName, _baseAddress);
                return key;
            }
            if (string.IsNullOrEmpty(_endpointConfigName) == false)
            {
                key = string.Format("ECN;{0};{1}", _serviceTypeName, _endpointConfigName);
                return key;
            }
            if (_bindingConfig != null)
            {
                key = string.Format("BC;{0};{1}", _serviceTypeName, _bindingConfig);
                return key;
            }
            key = string.Format("default;{0}", _serviceTypeName);
            return key;
        }

        #endregion

        #region Field

        private BindingConfig _bindingConfig = null;

        private Uri _baseAddress = null;

        private string _endpointConfigName = "";

        private readonly string _channelFactoryKey = "";

        private static readonly object _lockObj = new object();

        /// <summary>
        /// This is the store of the channel.
        /// </summary>
        private static readonly IDictionary<string, ChannelFactory<TService>> _channelPool
            = new Dictionary<string, ChannelFactory<TService>>();

        private ChannelFactory<TService> newChannelFactory = null;

        #endregion
    }
}
