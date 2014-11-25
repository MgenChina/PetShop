// ?2011 IDesign Inc. 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Winsion.ServiceModel.Share.Interceptor.Base
{
   [AttributeUsage(AttributeTargets.Method)]
   public abstract class OperationInterceptorBehaviorAttribute : Attribute,IOperationBehavior
   {
       protected abstract OperationInvoker CreateInvoker(IOperationInvoker oldInvoker);

      public void AddBindingParameters(OperationDescription operationDescription,BindingParameterCollection bindingParameters)
      {}

      public void ApplyClientBehavior(OperationDescription operationDescription,ClientOperation clientOperation)
      {}

      public void ApplyDispatchBehavior(OperationDescription operationDescription,DispatchOperation dispatchOperation)
      {
         IOperationInvoker oldInvoker = dispatchOperation.Invoker;
         dispatchOperation.Invoker = CreateInvoker(oldInvoker);
      }

      public void Validate(OperationDescription operationDescription)
      {}
   }
}