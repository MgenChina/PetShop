

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System;

namespace Winsion.ServiceProxy.Utils
{
    public class DuplexChannelFactory<TServiceContract, TCallbackContract> : DuplexChannelFactory<TServiceContract> where TServiceContract : class
    {
        static DuplexChannelFactory()
        {
            VerifyCallback();
        }
        public DuplexChannelFactory(TCallbackContract callback)
            : base(callback)
        { }
        public DuplexChannelFactory(InstanceContext<TCallbackContract> context, Binding binding)
            : base(context.Context, binding)
        { }
        public DuplexChannelFactory(InstanceContext<TCallbackContract> context, ServiceEndpoint endpoint)
            : base(context.Context, endpoint)
        { }

        public DuplexChannelFactory(InstanceContext<TCallbackContract> context, string endpointName)
            : base(context.Context, endpointName)
        { }
        public DuplexChannelFactory(TCallbackContract callback, Binding binding)
            : base(callback, binding)
        { }
        public DuplexChannelFactory(TCallbackContract callback, ServiceEndpoint endpoint)
            : base(callback, endpoint)
        { }
        public DuplexChannelFactory(TCallbackContract callback, string endpointName)
            : base(callback, endpointName)
        { }
        public DuplexChannelFactory(InstanceContext<TCallbackContract> context, Binding binding, EndpointAddress endpointAddress)
            : base(context.Context, binding, endpointAddress)
        { }
        public DuplexChannelFactory(InstanceContext<TCallbackContract> context, string endpointName, EndpointAddress endpointAddress)
            : base(context.Context, endpointName, endpointAddress)
        { }
        public DuplexChannelFactory(TCallbackContract callback, Binding binding, EndpointAddress endpointAddress)
            : base(callback, binding, endpointAddress)
        { }
        public DuplexChannelFactory(TCallbackContract callback, string endpointName, EndpointAddress endpointAddress)
            : base(callback, endpointName, endpointAddress)
        { }
        public static TServiceContract CreateChannel(TCallbackContract callback, string endpointName)
        {
            return DuplexChannelFactory<TServiceContract>.CreateChannel(callback, endpointName);
        }
        public static TServiceContract CreateChannel(InstanceContext<TCallbackContract> context, string endpointName)
        {
            return DuplexChannelFactory<TServiceContract>.CreateChannel(context.Context, endpointName);
        }
        public static TServiceContract CreateChannel(TCallbackContract callback, Binding binding, EndpointAddress endpointAddress)
        {
            return DuplexChannelFactory<TServiceContract>.CreateChannel(callback, binding, endpointAddress);
        }
        public static TServiceContract CreateChannel(InstanceContext<TCallbackContract> context, Binding binding, EndpointAddress endpointAddress)
        {
            return DuplexChannelFactory<TServiceContract>.CreateChannel(context.Context, binding, endpointAddress);
        }

        internal static void VerifyCallback()
        {
            Type contractType = typeof(TServiceContract);
            Type callbackType = typeof(TCallbackContract);
            object[] attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), false);
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException("Type of " + contractType + " is not a service contract");
            }
            ServiceContractAttribute serviceContractAttribute = attributes[0] as ServiceContractAttribute;
            if (callbackType != serviceContractAttribute.CallbackContract)
            {
                throw new InvalidOperationException("Type of " + callbackType + " is not configured as callback contract for " + contractType);
            }
        }
    }
}
