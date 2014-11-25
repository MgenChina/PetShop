using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;
using Winsion.Core.WCF;

namespace Winsion.ServiceProxy.Utils
{
    internal abstract class ProxyBase<TService> : IContract<TService>, IDisposable
        where TService : class
    {
        #region IDisposable

        private bool _isDisposed = false;

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool finalizing)
        {
            if (_isDisposed == false)
            {
                _isDisposed = true;
                //if (finalizing == false)
                //{                   
                //}
            }
        }

        #endregion

        public abstract ChannelFactory<TService> GetChannelFactory();

        TService IContract<TService>.CreateProxy()
        {
            return this.CreateProxy(TimeSpan.Zero);
        }

        private TService CreateProxy(TimeSpan timeout)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Proxy");
            }
            var cf = ChannelFactory;
            if (timeout != TimeSpan.Zero)
            {
                SetTimeout(cf.Endpoint.Binding, timeout);
            }
            var channel = cf.CreateChannel();
            return channel;
        }

        protected abstract ChannelFactory<TService> ChannelFactory { get; }

        protected static Binding GetBinding(Uri baseAddress, BindingConfig bindingConfig)
        {
            return EndpointHelper.GetBinding(baseAddress, bindingConfig != null ? bindingConfig.Name : null);
        }

        protected static Uri GetAddress(Type contract)
        {
            var uri = EndpointHelper.GetAddress(contract, GetDefaultBaseAddr());
            return uri;
        }

        protected static Uri GetAddress(Type contract, Uri baseAddress)
        {
            var uri = EndpointHelper.GetAddress(contract, baseAddress);
            return uri;
        }

        protected static void SetTimeout(Binding binding, TimeSpan timeout)
        {
            if (timeout != TimeSpan.Zero && binding != null)
            {
                binding.OpenTimeout = timeout;
                binding.CloseTimeout = timeout;
                binding.ReceiveTimeout = timeout;
                binding.SendTimeout = timeout;
            }
        }

        private static Uri GetDefaultBaseAddr()
        {
            if (_isInitedDefaultBaseAddr == false)
            {
                var arr = EndpointHelper.GetBaseAddress();
                _defaultBaseAddr = arr != null && arr.Length > 0 ? arr[0] : null;
                _isInitedDefaultBaseAddr = true;
            }
            return _defaultBaseAddr;
        }

        private static Uri _defaultBaseAddr = null;
        private static bool _isInitedDefaultBaseAddr = false;
    }

    internal abstract class CodeBlockProxyBase<TService> : ProxyBase<TService>, IProxy<TService>
        where TService : class
    {
        #region IProxy<TService>        

        public void Use(Action<TService> codeBlock)
        {
            this.Use(codeBlock, TimeSpan.Zero);
        }

        public void Use(Action<TService> codeBlock, TimeSpan timeout)
        {
            IClientChannel proxy = null;
            try
            {
                var cf = ChannelFactory;
                if (timeout != TimeSpan.Zero)
                {
                    SetTimeout(cf.Endpoint.Binding, timeout);
                }
                var channel = cf.CreateChannel();
                proxy = (IClientChannel)channel;
                proxy.Open();
                codeBlock(channel);
                proxy.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Helper.CloseChannel(ref proxy);
            }
        }

        public void UseAsync(Action<TService> codeBlock)
        {
            this.UseAsync(codeBlock, null);
        }

        public void UseAsync(Action<TService> codeBlock, Action<AsyncResponse> callBack)
        {
            this.UseAsync<object>(codeBlock, callBack, null);
        }

        public void UseAsync<TAsyncState>(Action<TService> codeBlock, Action<AsyncResponse<TAsyncState>> callBack, TAsyncState obj)
        {
            var uiCul = Thread.CurrentThread.CurrentUICulture;
            var cul = Thread.CurrentThread.CurrentCulture;
            var pr = Thread.CurrentPrincipal;
            SynchronizationContext synContext = SynchronizationContext.Current ?? new SynchronizationContext();

            ThreadPool.QueueUserWorkItem(p =>
            {
                Thread.CurrentThread.CurrentUICulture = uiCul;
                Thread.CurrentThread.CurrentCulture = cul;
                Thread.CurrentPrincipal = pr;

                AsyncResponse<TAsyncState> response = null;
                try
                {
                    this.Use(codeBlock);
                    if (callBack == null)
                    {
                        return;
                    }
                    response = new AsyncResponse<TAsyncState>(obj);
                }
                catch (Exception ex)
                {
                    if (callBack == null)
                    {
                        return;
                    }
                    response = new AsyncResponse<TAsyncState>(obj, ex);
                }

                synContext.Send(state =>
                {
                    callBack(state as AsyncResponse<TAsyncState>);
                }, response);
            });
        }

        public void UseAsync<TReturn, TAsyncState>(Func<TService, TReturn> codeBlock, Action<AsyncResponse<TReturn, TAsyncState>> callBack, TAsyncState obj)
        {
            var uiCul = Thread.CurrentThread.CurrentUICulture;
            var cul = Thread.CurrentThread.CurrentCulture;
            var pr = Thread.CurrentPrincipal;
            SynchronizationContext synContext = SynchronizationContext.Current ?? new SynchronizationContext();

            ThreadPool.QueueUserWorkItem(p =>
            {
                Thread.CurrentThread.CurrentUICulture = uiCul;
                Thread.CurrentThread.CurrentCulture = cul;
                Thread.CurrentPrincipal = pr;

                AsyncResponse<TReturn, TAsyncState> response = null;
                try
                {
                    TReturn result = this.Use<TReturn>(codeBlock);
                    if (callBack == null)
                    {
                        return;
                    }
                    response = new AsyncResponse<TReturn, TAsyncState>(result, obj);
                }
                catch (Exception ex)
                {
                    if (callBack == null)
                    {
                        return;
                    }
                    response = new AsyncResponse<TReturn, TAsyncState>(default(TReturn), obj, ex);
                }

                synContext.Send(state =>
                {
                    callBack(state as AsyncResponse<TReturn, TAsyncState>);
                }, response);
            });
        }

        #endregion

        private TReturn Use<TReturn>(Func<TService, TReturn> codeBlock)
        {
            IClientChannel proxy = null;
            try
            {
                var channel = ChannelFactory.CreateChannel();
                proxy = (IClientChannel)channel;
                proxy.Open();
                TReturn result = codeBlock(channel);
                proxy.Close();
                return result;
            }
            catch (CommunicationException communicationException)
            {
                throw communicationException;
            }
            catch (TimeoutException timeoutException)
            {
                throw timeoutException;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Helper.CloseChannel(ref proxy);
            }
        }
    }

}
