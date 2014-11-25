using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Configuration;
using System.Diagnostics.Contracts;
using Winsion.ServiceProxy.Utils.ChannelFactory;
using Winsion.Core.WCF;
using System.ServiceModel.Channels;


namespace Winsion.ServiceProxy.Utils
{
    public interface IDuplexProxy<TService> : IContract<TService>, IDisposable
        where TService : class
    {
        ChannelFactory<TService> GetChannelFactory();
    }

    internal class DuplexProxy<TService> : ProxyBase<TService>, IDuplexProxy<TService>
        where TService : class
    {
        public DuplexProxy(object callback)
            : this(callback, null)
        {
        }

        public DuplexProxy(object callback, Uri baseAddress)
        {
            this.baseAddress = baseAddress;
            this.callback = callback;
        }

        ~DuplexProxy()
        {
            Dispose(true);
        }

        public override ChannelFactory<TService> GetChannelFactory()
        {
            return ChannelFactory;
        }

        protected override ChannelFactory<TService> ChannelFactory
        {
            get
            {
                if (channelFactory == null || Helper.IsInvalidChannelFactory(channelFactory))
                {
                    lock (lockFactory)
                    {
                        if (channelFactory == null || Helper.IsInvalidChannelFactory(channelFactory))
                        {
                            if (channelFactory != null)
                            {
                                Helper.CloseChannelFactory(ref channelFactory);
                            }
                            channelFactory = CreateChannelFactory();
                        }
                    }
                }

                return channelFactory;
            }
        }

        private DuplexChannelFactory<TService> CreateChannelFactory()
        {
            Uri uri;
            if (baseAddress != null)
            {
                uri = GetAddress(typeof(TService), baseAddress);
            }
            else
            {
                uri = GetAddress(typeof(TService));
            }
            Binding binding = GetBinding(uri, null);
            var chFactory = new DuplexChannelFactory<TService>(callback, binding, new EndpointAddress(uri));
            return chFactory;
        }

        private bool isDisposed = false;
        protected override void Dispose(bool finalizing)
        {
            if (isDisposed == false)
            {
                isDisposed = true;
                //if (finalizing == false)
                //{                    
                //}
                Helper.CloseChannelFactory(ref channelFactory);
            }
            base.Dispose(finalizing);
        }

        #region Field

        private ChannelFactory<TService> channelFactory = null;

        private Uri baseAddress = null;
        private object callback = null;

        private readonly object lockFactory = new object();

        #endregion
    }

}
