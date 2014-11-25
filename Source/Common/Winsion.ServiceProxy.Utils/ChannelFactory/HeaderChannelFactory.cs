﻿// © 2011 IDesign Inc. 
//Questions? Comments? go to 
//http://www.idesign.net

using System.ServiceModel;
using System.ServiceModel.Channels;
using Winsion.Core.WCF.Context;




namespace Winsion.ServiceProxy.Utils.ChannelFactory
{
    public class HeaderChannelFactory<T, H> : InterceptorChannelFactory<T> where T : class
    {
        public H Header
        { get; protected set; }

        public HeaderChannelFactory()
            : this(default(H))
        { }

        public HeaderChannelFactory(string endpointName)
            : this(default(H), endpointName)
        { }

        public HeaderChannelFactory(Binding binding, EndpointAddress remoteAddress)
            : this(default(H), binding, remoteAddress)
        { }

        public HeaderChannelFactory(H header)
        {
            Header = header;
        }

        public HeaderChannelFactory(H header, string endpointName)
            : base(endpointName)
        {
            Header = header;
        }

        public HeaderChannelFactory(H header, Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
            Header = header;
        }
        protected override void PreInvoke(ref Message request)
        {
            var context = new GenericContext<H>(Header);
            var genericHeader = new MessageHeader<GenericContext<H>>(context);
            request.Headers.Add(genericHeader.GetUntypedHeader(GenericContext<H>.TypeName, GenericContext<H>.TypeNamespace));

           
        }
    }
}