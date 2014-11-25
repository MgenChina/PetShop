using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections;
using log4net;

namespace Winsion.Core.Hibernate.WCF
{
    public class WcfNHibernateContext : IExtension<InstanceContext>
    {
        public WcfNHibernateContext()
        {
            Items = new Hashtable();
        }

        #region Add Remove
        /// <summary>
        /// 强烈推荐在IInstanceProvider.GetInstance 的第一条语句调用，添加WcfNHibernateContext。
        /// </summary>
        public static void Add()
        {
            Add(CurrentInstanceContext);
        }

        /// <summary>
        /// 强烈推荐在IInstanceProvider.GetInstance 的第一条语句调用，添加WcfNHibernateContext。
        /// </summary>
        public static void Add(InstanceContext instanceContext)
        {
            var contextExtension = instanceContext.Extensions.Find<WcfNHibernateContext>();
            if (contextExtension == null)
            {
                instanceContext.Extensions.Add(new WcfNHibernateContext());
            }
        }

        /// <summary>
        /// 强烈推荐在IInstanceProvider.ReleaseInstance 的最后一条语句调用，移除WcfNHibernateContext。
        /// </summary>
        public static void Remove()
        {
            Remove(CurrentInstanceContext);
        }

        /// <summary>
        /// 强烈推荐在IInstanceProvider.ReleaseInstance 的最后一条语句调用，移除WcfNHibernateContext。
        /// </summary>
        public static void Remove(InstanceContext instanceContext)
        {
            var contextExtension = instanceContext.Extensions.Find<WcfNHibernateContext>();
            if (contextExtension != null)
            {
                instanceContext.Extensions.Remove(contextExtension);
            }
        } 
        #endregion

        public static WcfNHibernateContext Current
        {
            get { return GetExtension(CurrentInstanceContext); }
        }

        public static WcfNHibernateContext GetExtension(InstanceContext instanceContext)
        {
            var temp = instanceContext.Extensions.Find<WcfNHibernateContext>();
            if (temp == null)
            {
                if (log.IsWarnEnabled)
                {
                    log.WarnFormat("Current属性， OperationContext.Current.InstanceContext.Extensions 没有找到WcfNHibernateContext实例。");
                }
            }

            return temp;
        }

        public IDictionary Items { get; private set; }

        #region IExtension<OperationContext> Members


        public void Attach(InstanceContext owner)
        {
            owner.Closing += new EventHandler(
                delegate(object sender, EventArgs args)
                {
                    this.Detach((InstanceContext)sender);
                });
        }

        public void Detach(InstanceContext owner)
        {
            OnContextDetach(owner);
        }

        #endregion

        private static void OnContextDetach(InstanceContext owner)
        {
            var context = GetExtension(owner);
            if (context != null && context.Items != null && context.Items.Count > 0)
            {
                var manager = (WcfNHibernateSessionManager)NHFactory.Instance.GetSessionManager(NHSessionContextType.WCF);
                manager.InstanceContextDetach(context);
            }
        }

        private static InstanceContext CurrentInstanceContext
        {
            get
            {
                return OperationContext.Current.InstanceContext;
            }
        }

        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(WcfNHibernateContext));
    }
}
