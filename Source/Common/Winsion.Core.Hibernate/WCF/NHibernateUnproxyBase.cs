using System;
using NHibernate;
using NHibernate.Metadata;
using NHibernate.Proxy;
using NHibernate.Type;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using NHibernate.Collection;

namespace Winsion.Core.Hibernate.WCF
{
    public static class NHibernateUnproxyExt
    {
        /// <summary>
        /// Force initialization of a proxy or persistent collection.
        /// </summary>
        /// <param name="persistentObject">a persistable object, proxy or null</param>
        /// <typeparam name="T">a persistable type</typeparam>
        /// <exception cref="HibernateException">if we can't initialize the proxy at this time, eg. the Session was closed</exception>
        public static T Unproxy<T>(this T persistentObject)
        {
            var proxy = persistentObject as INHibernateProxy;

            if (proxy == null)
            {
                return persistentObject;
            }

            var li = proxy.HibernateLazyInitializer;
            if (li.IsUninitialized)
            {
                return default(T);
            }
            else
            {
                return (T)li.GetImplementation();
            }
        }

        public static T UnproxyCollection<T>(this T persistenCollection)
        {
            // NHibernate.Proxy.NHibernateProxyHelper
            if (NHibernateUtil.IsInitialized(persistenCollection))
            {
                return persistenCollection;
            }

            return default(T);
        }

        /// <summary>
        /// Gets the underlying class type of a persistent object that may be proxied
        /// </summary>
        public static Type GetUnproxiedType<T>(this T persistentObject)
        {
            if (persistentObject == null)
            {
                return typeof(T);
            }

            var proxy = persistentObject as INHibernateProxy;
            if (proxy != null)
            {
                return proxy.HibernateLazyInitializer.PersistentClass;
            }

            return persistentObject.GetType();
        }
    }

    public abstract class NHibernateUnproxyBase
    {
        /// <summary>
        /// 如果第一层有是DTO对象，不能Unproxy。
        /// Force initialzation of a possibly proxied object tree up to the maxDepth.
        /// Once the maxDepth is reached, entity properties will be replaced with
        /// placeholder objects having only the identifier property populated.
        /// </summary>
        public virtual T UnproxyObjectTree<T>(T persistentObject, ISessionFactory sessionFactory, int maxDepth)
        {
            if (persistentObject == null || persistentObject.GetType().IsArray || IsGenericCollection(persistentObject))
            {
                return persistentObject;
            }

            // Determine persistent type of the object
            Type persistentType = persistentObject.GetUnproxiedType();
            IClassMetadata classMetadata = sessionFactory.GetClassMetadata(persistentType);
            // 如果第一层有是DTO对象,不能Unproxy
            if (classMetadata == null) 
            {
                return persistentObject;
            }

            // If we've already reached the max depth, we will return a placeholder object
            if (maxDepth < 0)
            {
                return CreatePlaceholder(persistentObject, persistentType, classMetadata);
            }

            // Now lets go ahead and make sure everything is unproxied
            var unproxiedObject = persistentObject.Unproxy();
            if (unproxiedObject == null)
            {
                return unproxiedObject;
            }

            // Iterate through each property and unproxy entity types
            for (int i = 0; i < classMetadata.PropertyTypes.Length; i++)
            {
                IType nhType = classMetadata.PropertyTypes[i];
                string propertyName = classMetadata.PropertyNames[i];
                PropertyInfo propertyInfo = persistentType.GetProperty(propertyName);

                UnproxyKind kind = UnproxyKind.None;
                if (nhType.IsEntityType)
                {
                    kind = UnproxyKind.EntityType;
                }
                else if (nhType.IsCollectionType)
                {
                    kind = UnproxyKind.CollectionType;
                }

                Unproxy<T>(kind, propertyInfo, unproxiedObject, sessionFactory, maxDepth - 1);
            }

            return unproxiedObject;
        }

        /// <summary>
        /// 第1层对象开始必须是Entity,否则不能Unproxy
        /// </summary>       
        public virtual T UnproxyObjectTree<T>(T persistentObject, int maxDepth)
        {
            if (persistentObject == null || persistentObject.GetType().IsArray || IsGenericCollection(persistentObject))
            {
                return persistentObject;
            }

            Type persistentType = persistentObject.GetUnproxiedType();

            if (IsEntity(persistentType) == false)
            {
                return persistentObject;
            }

            if (maxDepth < 0)
            {
                return CreatePlaceholder(persistentObject, persistentType);
            }

            var unproxiedObject = persistentObject.Unproxy();
            if (unproxiedObject == null)
            {
                return unproxiedObject;
            }


            var pis = persistentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (pis != null)
            {
                for (int i = 0; i < pis.Length; i++)
                {
                    PropertyInfo propertyInfo = pis[i];

                    UnproxyKind kind = UnproxyKind.None;
                    if (IsEntity(propertyInfo.PropertyType))
                    {
                        kind = UnproxyKind.EntityType;
                    }
                    else if (IsEntityGenericCollection(propertyInfo.PropertyType))
                    {
                        kind = UnproxyKind.CollectionType;
                    }

                    Unproxy<T>(kind, propertyInfo, unproxiedObject, null, maxDepth - 1);
                }
            }

            return unproxiedObject;
        }

        

        private void Unproxy<T>(UnproxyKind unproxyKind, PropertyInfo propertyInfo, T unproxiedObject, ISessionFactory sessionFactory, int maxDepth)
        {
            if (unproxyKind == UnproxyKind.EntityType)
            {
                object propertyValue = propertyInfo.GetValue(unproxiedObject, null);
                if (propertyValue != null)
                {
                    object obj;
                    if (sessionFactory == null)
                    {
                        obj = UnproxyObjectTree(propertyValue, maxDepth);
                    }
                    else
                    {
                        obj = UnproxyObjectTree(propertyValue, sessionFactory, maxDepth);
                    }

                    propertyInfo.SetValue(unproxiedObject, obj, null);
                }
            }
            else if (unproxyKind == UnproxyKind.CollectionType)
            {
                object propertyValue = propertyInfo.GetValue(unproxiedObject, null);
                if (propertyValue != null)
                {
                    var list = propertyValue as IEnumerable;
                    if (list != null)
                    {
                        if (NHibernateUtil.IsInitialized(propertyValue))
                        {
                            foreach (var li in list)
                            {
                                if (sessionFactory == null)
                                {
                                    UnproxyObjectTree(li, maxDepth);
                                }
                                else
                                {
                                    UnproxyObjectTree(li, sessionFactory, maxDepth);
                                }
                            }
                        }
                        else
                        {
                            propertyInfo.SetValue(unproxiedObject, null, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return an empty placeholder object with the Identifier set.  We can safely access the identifier
        /// property without the object being initialized.
        /// </summary>
        protected abstract T CreatePlaceholder<T>(T persistentObject, Type persistentType, IClassMetadata classMetadata);


        protected abstract T CreatePlaceholder<T>(T persistentObject, Type persistentType);


        public abstract bool IsEntity(Type persistentType);


        private static bool IsGenericCollection<T>(T persistentObject)
        {
            var type = persistentObject == null ? typeof(T) : persistentObject.GetType();
            return IsGenericCollection(type);
        }

        private static bool IsGenericCollection(Type persistentType)
        {
            var b2 = typeof(IEnumerable).IsAssignableFrom(persistentType);
            var b3 = (b2) && (persistentType.IsGenericType);
            return b3;
        }

        private bool IsEntityGenericCollection(Type persistentType)
        {
            if (IsGenericCollection(persistentType))
            {
                var types = persistentType.GetGenericArguments();
                if (types != null && types.Length > 0)
                {
                    return IsEntity(types[0]);
                }
            }

            return false;
        }

        private enum UnproxyKind
        {
            None = 0,
            EntityType = 1,
            CollectionType = 2
        }
    }
}
