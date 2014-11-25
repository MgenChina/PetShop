using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Winsion.Core.Hibernate.WCF
{
	public class NHInstanceContextAttribute : Attribute, IContractBehavior
	{
		#region IContractBehavior Members

		public void AddBindingParameters(
			ContractDescription contractDescription, 
			ServiceEndpoint endpoint, 
			BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(
			ContractDescription contractDescription, 
			ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		public void ApplyDispatchBehavior(
			ContractDescription contractDescription, 
			ServiceEndpoint endpoint, 
			DispatchRuntime dispatchRuntime)
		{
			dispatchRuntime.InstanceContextInitializers.Add(
                new NHInstanceContextInitializer());
		}

		public void Validate(
			ContractDescription contractDescription, 
			ServiceEndpoint endpoint)
		{
		}

		#endregion
	}
}
