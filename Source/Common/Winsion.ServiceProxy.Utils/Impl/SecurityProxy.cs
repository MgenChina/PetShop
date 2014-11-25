using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Configuration;
using System.Diagnostics.Contracts;
using Winsion.ServiceProxy.Utils.ChannelFactory;
using System.ServiceModel.Channels;

namespace Winsion.ServiceProxy.Utils
{
    internal class SecurityProxy<TService> : CodeBlockProxyBase<TService>, IProxy<TService>
        where TService : class
    {
        public SecurityProxy()
        {
        }

        public SecurityProxy(BindingConfig bindingConfig)
        {
            this._bindingConfig = bindingConfig;
        }

        public SecurityProxy(string endpointConfigurationName)
        {
            this._endpointConfigName = endpointConfigurationName;
        }

        public SecurityProxy(Uri baseAddress)
        {
            this._baseAddress = baseAddress;
        }

        ~SecurityProxy()
        {
            Dispose(true);
        }

        public override ChannelFactory<TService> GetChannelFactory()
        {
            return ChannelFactory;
        }

        private bool _isDisposed = false;
        protected override void Dispose(bool finalizing)
        {
            if (_isDisposed == false)
            {
                _isDisposed = true;
                //if (finalizing == false)
                //{                   
                //}
                Helper.CloseChannelFactory(ref _channelFactory);
            }
            base.Dispose(finalizing);
        }

        protected override ChannelFactory<TService> ChannelFactory
        {
            get
            {
                if (_channelFactory == null || Helper.IsInvalidChannelFactory(_channelFactory))
                {
                    if (_channelFactory != null)
                    {
                        Helper.CloseChannelFactory(ref _channelFactory);
                    }
                    _channelFactory = CreateChannelFactory();
                }

                return _channelFactory;
            }
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
                chFactory = new SecurityChannelFactory<TService>(binding, new EndpointAddress(uri));
                return chFactory;
            }
            if (string.IsNullOrEmpty(_endpointConfigName) == false)
            {
                chFactory = new SecurityChannelFactory<TService>(_endpointConfigName);
                return chFactory;
            }
            if (_bindingConfig != null)
            {
                uri = GetAddress(typeof(TService));
                binding = GetBinding(uri, _bindingConfig);
                chFactory = new SecurityChannelFactory<TService>(binding, new EndpointAddress(uri));
                return chFactory;
            }
            uri = GetAddress(typeof(TService));
            binding = GetBinding(uri, null);
            chFactory = new SecurityChannelFactory<TService>(binding, new EndpointAddress(uri));
            return chFactory;
        }

        #region Field

        private ChannelFactory<TService> _channelFactory;

        private BindingConfig _bindingConfig;

        private string _endpointConfigName;

        private Uri _baseAddress = null;

        #endregion
    }
}
