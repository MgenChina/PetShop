
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace Winsion.Core.WCF.Context
{
    [DataContract(Namespace = "Winsion.Core.WCF.Context")]
    public class GenericContext<T>
    {
        static string _typeName;
        internal static string TypeName
        {
            get { return _typeName; }
            private set { _typeName = value; }
        }

        static string _typeNamespace;
        internal static string TypeNamespace
        {
            get { return _typeNamespace; }
            private set { _typeNamespace = value; }
        }

        static GenericContext()
        {
            // Verify [DataContract] or [Serializable] on T
            var type = typeof(T);
            var attr = GetDataContract(type);
            Debug.Assert(attr != null || type.IsSerializable);

            //TypeNamespace = "net.clr:" + typeof(T).FullName;
            TypeNamespace = string.Format("net.clr:{0}.{1}", attr.Namespace, attr.Name);
            TypeName = "GenericContext";
        }
        static DataContractAttribute GetDataContract(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(DataContractAttribute), false);
            if (attributes.Length == 1)
            {
                return attributes[0] as DataContractAttribute;
            }

            return null;
        }

        [DataMember]
        public readonly T Value;

        public GenericContext(T value)
        {
            Value = value;
        }
        public GenericContext()
            : this(default(T))
        { }
        public static GenericContext<T> Current
        {
            get
            {
                OperationContext context = OperationContext.Current;
                if (context == null)
                {
                    return null;
                }
                try
                {
                    var header = context.IncomingMessageHeaders.GetHeader<GenericContext<T>>(TypeName, TypeNamespace);
                    return header;
                }
                catch (Exception exception)
                {
                    //Debug.Assert(exception is MessageHeaderException && exception.Message == "There is not a header with name " + TypeName + " and namespace " + TypeNamespace + " in the message.");
                    return null;
                }
            }
            set
            {
                OperationContext context = OperationContext.Current;
                Debug.Assert(context != null);

                // Having multiple GenericContext<T> headers is an error
                bool headerExists = false;
                try
                {
                    context.OutgoingMessageHeaders.GetHeader<GenericContext<T>>(TypeName, TypeNamespace);
                    headerExists = true;
                }
                catch (MessageHeaderException exception)
                {
                    //Debug.Assert(exception.Message == "There is not a header with name " + TypeName + " and namespace " + TypeNamespace + " in the message.");
                }
                if (headerExists)
                {
                    throw new InvalidOperationException("A header with name " + TypeName + " and namespace " + TypeNamespace + " already exists in the message.");
                }
                MessageHeader<GenericContext<T>> genericHeader = new MessageHeader<GenericContext<T>>(value);
                context.OutgoingMessageHeaders.Add(genericHeader.GetUntypedHeader(TypeName, TypeNamespace));
            }
        }
    }
}