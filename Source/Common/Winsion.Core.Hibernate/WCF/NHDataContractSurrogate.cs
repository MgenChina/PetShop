using System;
using System.CodeDom;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using log4net;
using NHibernate.Proxy;
using System.Collections;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace Winsion.Core.Hibernate.WCF
{
    public class NHDataContractSurrogate : IDataContractSurrogate
    {
        public NHDataContractSurrogate()
        {
            unproxy = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NHibernateUnproxyBase>();
        }

        #region IDataContractSurrogate Members

        public Type GetDataContractType(Type type)
        {
            return type;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (unproxy.IsEntity(targetType) == false)
            {
                return obj;
            }

            try
            {
                var v = unproxy.UnproxyObjectTree(obj, 1);
                return v;
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("GetObjectToSerialize方法,targetType={0}", targetType.FullName), ex);
                }

                throw ex;
            }
        }


        public object GetDeserializedObject(object obj, Type targetType)
        {
            return obj;
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            return null;
        }


        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {
            return null;
        }



        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
        }


        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }


        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            return typeDeclaration;
        }


        #endregion


        private NHibernateUnproxyBase unproxy;
        private static readonly log4net.ILog log = LogManager.GetLogger(typeof(NHDataContractSurrogate));
    }
}