using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winsion.Core
{
    public delegate void ServiceCallback<TServiceArg>(TServiceArg serviceArg);
    public interface IShareServiceBase
    {
        void Subscribe<TService>(ServiceCallback<object> callback);

        void Publish<TService>(object serviceArg);

    }

    public interface IShareService : IShareServiceBase
    {
        void Subscribe<TService, TServiceArg>(ServiceCallback<TServiceArg> callback);

        void Publish<TService, TServiceArg>(TServiceArg serviceArg);

    }

    public interface IShareMultiService : IShareService
    {
        void Subscribe<TServiceArg>(string serviceEnum, ServiceCallback<TServiceArg> callback);

        void Publish<TServiceArg>(string serviceEnum, TServiceArg serviceArg);
    }
}
