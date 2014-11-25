using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Winsion.Core.Unity;

using Winsion.Core;
using Microsoft.Practices.ServiceLocation;
using log4net;



namespace Winsion.ServiceModel.Share
{
    public class UnityInstanceProvider :  IInstanceProvider
    {
        public IUnityContainer UnityContainer { set;private get; }
     
        public Type ServiceType { set; get; }

        public UnityInstanceProvider()
            : this(null)
        {           
        }

        public UnityInstanceProvider(Type serviceType)
        {
            ServiceType = serviceType;
            UnityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
        }
        
        // Get Service instace via unity container       
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            try
            {
                //WcfNHibernateContext.Add(instanceContext);

                if (this.UnityContainer != null)
                {
                    return this.UnityContainer.Resolve(this.ServiceType);
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("解析服务实例发生错误，服务名：{0}\r\n", ex, this.ServiceType.FullName);

                throw ex;
            }

            return instanceContext;

        }

        public object GetInstance(System.ServiceModel.InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            try
            {
                //WcfNHibernateContext.Remove(instanceContext);
                this.UnityContainer.Teardown(instance);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("释放服务实例发生错误，服务名：{0}\r\n", ex, this.ServiceType.FullName);
            }

        }

        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(UnityInstanceProvider));

    }
}
