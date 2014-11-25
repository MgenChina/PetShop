using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Practices.ServiceLocation;
using System.Collections;
using System.ComponentModel;

namespace Winsion.Domain
{
    public interface IEntityBase
    {
        // Methods
        int GetHashCode();
        bool Equals(object obj);

        // Properties
        object Id { get; }
    }

    /// <summary>
    /// Base for all business objects.
    ///     
    /// </summary>
    /// <typeparam name="TKey">DataType of the primary key.</typeparam>   
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class EntityBase<TKey> : IEntityBase, INotifyPropertyChanged
    {
        public EntityBase()
        {
        }

        #region Declarations

        [IgnoreDataMember]
        private TKey _id = default(TKey);

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            EntityBase<TKey> compareTo = obj as EntityBase<TKey>;

            return compareTo != null
                && HasSameNonDefaultIdAs(compareTo)
                && HasSameBusinessSignatureAs(compareTo);
        }

        /// <summary>  
        /// Transient objects are not associated with an   
        /// item already in storage.  For instance, a   
        /// Customer is transient if its ID is 0.  
        /// </summary>  
        public virtual bool IsTransient()
        {
            return Id == null || Id.Equals(default(TKey));
        }

        /// <summary>  
        /// Must be implemented to compare two objects  
        /// </summary>  
        public abstract override int GetHashCode();


        private bool HasSameBusinessSignatureAs(EntityBase<TKey> compareTo)
        {
            return GetHashCode().Equals(compareTo.GetHashCode());
        }

        /// <summary>  
        /// Returns true if self and the provided domain   
        /// object have the same ID values and the IDs   
        /// are not of the default ID value  
        /// </summary>  
        private bool HasSameNonDefaultIdAs(EntityBase<TKey> compareTo)
        {
            return IsTransient() || compareTo.IsTransient() || Id.Equals(compareTo.Id);
        }

        #endregion

        #region Properties

        [DataMember]
        public virtual TKey Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id.Equals(value) == false)
                {
                    _id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        [IgnoreDataMember]
        object IEntityBase.Id
        {
            get { return this.Id; }
        }

        #endregion

        #region Unproxy        

        protected virtual void OnUnproxy()
        {
        }

        protected void Unproxy<TUnproxy>(ref TUnproxy unproxyObj)
            where TUnproxy : EntityBase<TKey>, new()
        {
            if (unproxyObj == null || UnproxyHandler == null)
            {
                return;
            }

            var obj = UnproxyHandler.UnproxyEntity(unproxyObj);
            if (obj == null)
            {
                var entity = (EntityBase<TKey>)unproxyObj;
                unproxyObj = new TUnproxy();
                ((EntityBase<TKey>)unproxyObj).Id = entity.Id;
            }
            else
            {
                unproxyObj = obj;
            }
        }

        protected void UnproxyCollection<TUnproxy>(ref TUnproxy unproxyObj)
            where TUnproxy : class, IEnumerable
        {
            if (unproxyObj == null || UnproxyHandler == null)
            {
                return;
            }

            unproxyObj = UnproxyHandler.UnproxyCollection(unproxyObj);
        }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            if (IsNeedsUnproxy)
            {
                OnUnproxy();
            }
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            lockPropertyChanged = new object();
        }

        private IUnproxy _unproxyHandler;
        [IgnoreDataMember]
        private IUnproxy UnproxyHandler
        {
            get
            {
                if (_unproxyHandler == null)
                {
                    try
                    {
                        _unproxyHandler = ServiceLocator.Current.GetInstance<IUnproxy>();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return _unproxyHandler;
            }
        }

        [IgnoreDataMember]
        private bool IsNeedsUnproxy
        {
            get
            {
                return OperationContext.Current != null;

            }
        }

        #endregion

        #region INotifyPropertyChanged

        [field: NonSerializedAttribute()]
        private event PropertyChangedEventHandler _propertyChanged;
        
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                lock (lockPropertyChanged)
                {
                    _propertyChanged += value;
                }
            }

            remove
            {
                lock (lockPropertyChanged)
                {
                    _propertyChanged -= value;
                }
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = _propertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private object lockPropertyChanged = new object();
        #endregion
    }
}
