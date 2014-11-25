using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;

namespace Winsion.ServiceProxy.Utils
{
    /// <summary>
    /// Helper class for creating proxies at the client end for the exposed services.
    /// Usage:  IProxy<IService>.Use(serviceProxy =>
    ///        {
    ///            returnObject = serviceProxy.SomeMethod(parameters);
    ///        });
    /// </summary>
    /// <typeparam name="TService">This is the type of the interface.</typeparam>
    public interface IProxy<TService> : IContract<TService>, IDisposable
        where TService : class
    {
        ChannelFactory<TService> GetChannelFactory();

        void Use(Action<TService> codeBlock);

        //void Use(Action<TService> codeBlock, TimeSpan timeout);

        void UseAsync(Action<TService> codeBlock);

        void UseAsync(Action<TService> codeBlock, Action<AsyncResponse> callBack);

        void UseAsync<TAsyncState>(Action<TService> codeBlock, Action<AsyncResponse<TAsyncState>> callBack, TAsyncState obj);

        void UseAsync<TReturn, TAsyncState>(Func<TService, TReturn> codeBlock, Action<AsyncResponse<TReturn, TAsyncState>> callBack, TAsyncState obj);

    }

    public enum ResponseStatus
    {
        None = 0,

        Success = 1,

        Error = 2,
    }

    public class AsyncResponse
    {
        public AsyncResponse(Exception error)
        {
            this.Error = error;
            if (error != null)
            {
                this.Status = ResponseStatus.Error;
            }
            else
            {
                this.Status = ResponseStatus.Success;
            }
        }

        public ResponseStatus Status { get; private set; }

        public Exception Error { get; private set; }
    }

    public class AsyncResponse<TAsyncState> : AsyncResponse
    {
        public AsyncResponse()
            : this(default(TAsyncState))
        {

        }

        public AsyncResponse(TAsyncState asyncState)
            : this(asyncState, null)
        {

        }

        public AsyncResponse(TAsyncState asyncState, Exception error)
            : base(error)
        {

            this.UserState = asyncState;

        }

        public TAsyncState UserState { get; private set; }

    }

    public class AsyncResponse<TReturn, TAsyncState> : AsyncResponse
    {
        public AsyncResponse()
            : this(default(TReturn), default(TAsyncState))
        {

        }

        public AsyncResponse(TReturn returnValue, TAsyncState asyncState)
            : this(returnValue, asyncState, null)
        {

        }

        public AsyncResponse(TReturn returnValue, TAsyncState asyncState, Exception error)
            : base(error)
        {
            this.Result = returnValue;
            this.UserState = asyncState;
        }

        public TReturn Result { get; private set; }

        public TAsyncState UserState { get; private set; }
    }
}
