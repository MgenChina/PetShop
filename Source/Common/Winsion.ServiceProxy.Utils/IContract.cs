using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winsion.ServiceProxy.Utils
{
    /// <summary>
    /// 提供一种灵活的方式来给调用者控制通信对象，要把Proxy转换为IDisposable手动释放。 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IContract<TService>
    {
        /// <summary>
        /// Proxy 可以转换成IClientChannel，调用底层方法和订阅通道事件；
        /// 该Proxy对象，调用者要通过转换为IDisposable手动释放，调用完方法后要尽快释放，否则会造成资源泄露。
        /// </summary>
        TService CreateProxy();

        //TService CreateProxy(TimeSpan timeout);
    }
}
