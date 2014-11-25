using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Winsion.Core
{

    public sealed class ShareService : IShareMultiService, IShareService, IShareServiceBase
    {
        private ShareService()
        {
            _callbackList = new Dictionary<string, Tuple<object, IList<object>>>();
        }

        public static IShareService Instance
        {
            get
            {
                try
                {
                    return Nested.Service;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private class Nested
        {
            static Nested() { }
            internal static readonly IShareService Service =
                new ShareService();
        }


        #region impl
        void IShareServiceBase.Subscribe<TService>(ServiceCallback<object> callback)
        {
            ((IShareService)this).Subscribe<TService, object>(callback);

        }

        void IShareServiceBase.Publish<TService>(object serviceArg)
        {
            ((IShareService)this).Publish<TService, object>(serviceArg);

        }

        void IShareService.Subscribe<TService, TServiceArg>(ServiceCallback<TServiceArg> callback)
        {
            string subscriber = typeof(TService).AssemblyQualifiedName;
            this.Subscribe(subscriber, callback);
        }

        void IShareService.Publish<TService, TServiceArg>(TServiceArg serviceArg)
        {
            string subscriber = typeof(TService).AssemblyQualifiedName;
            this.Publish(subscriber, serviceArg);
        }


        void IShareMultiService.Subscribe<TServiceArg>(string serviceEnum, ServiceCallback<TServiceArg> callback)
        {
            string key = string.Format("{0}_{1}", serviceEnum, typeof(TServiceArg).AssemblyQualifiedName);
            this.Subscribe(key, callback);
        }

        void IShareMultiService.Publish<TServiceArg>(string serviceEnum, TServiceArg serviceArg)
        {
            string key = string.Format("{0}_{1}", serviceEnum, typeof(TServiceArg).AssemblyQualifiedName);
            this.Publish(key, serviceArg);

        }
        #endregion


        private void Subscribe(string key, object callback)
        {
            var tup = GetCallback(key);
            lock (tup.Item1)
            {
                tup.Item2.Add(callback);
            }
        }

        private void Publish<TServiceArg>(string key, TServiceArg serviceArg)
        {
            Tuple<object, IList<object>> tup;
            if (_callbackList.TryGetValue(key, out tup))
                if (tup != null)
                {
                    lock (tup.Item1)
                    {
                        foreach (var li in tup.Item2)
                        {
                            var _myCallback = li as ServiceCallback<TServiceArg>;
                            if (_myCallback != null)
                            {
                                _myCallback(serviceArg);
                            }
                        }
                    }
                }
        }

        private Tuple<object, IList<object>> GetCallback(string key)
        {
            if (_callbackList.ContainsKey(key) == false)
            {
                lock (lockobj)
                {
                    if (_callbackList.ContainsKey(key) == false)
                    {
                        _callbackList[key] = new Tuple<object, IList<object>>(new object(), new List<object>());
                    }
                }
            }

            return _callbackList[key];
        }

        private IDictionary<string, Tuple<object, IList<object>>> _callbackList;

        private object lockobj = new object();



    }
}
