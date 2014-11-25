using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Winsion.Core.Hibernate.WCF
{
    public class NHInstanceContextInitializer : IInstanceContextInitializer
	{
	

		#region IInstanceContextInitializer Members
		public void Initialize(
			InstanceContext instanceContext, 
			Message message)
		{						
            WcfNHibernateContext.Add(instanceContext);
		}
		#endregion
	}
}
