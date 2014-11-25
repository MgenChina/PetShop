﻿// © 2011 IDesign Inc. 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.ServiceModel.Dispatcher;


using Winsion.Core.WCF.Context;
using Winsion.ServiceModel.Share.Interceptor.Base;
using Winsion.ServiceModel.Share.Security;
using Winsion.ServiceModel.Share.Context;



namespace Winsion.ServiceModel.Share.Interceptor.Security
{
    class SecurityCallStackInvoker : OperationInvoker
   {
      public SecurityCallStackInvoker(IOperationInvoker oldInvoker) : base(oldInvoker)
      {}

      protected override void PreInvoke(object instance,object[] inputs)
      {
         try
         {
            SecurityCallStack callStack = SecurityCallStackContext.Current;
            if(callStack != null)
            {
               LogCallChain(callStack);
               ValidateCallChain(callStack);
               SignCallChain(callStack);
            }
         }
         catch(NullReferenceException)
         {
            throw new InvalidOperationException("No security call stack was found. Are you using the right proxy?");
         }
      }

      void ValidateCallChain(SecurityCallStack callStack)
      {
         //Perform custom validation steps here
      }

      void SignCallChain(SecurityCallStack callStack)
      {
         //Digitally sign call stack here
      }
      void LogCallChain(SecurityCallStack callStack)
      {
         //Log call stack here. For example:
         foreach(SecurityCallFrame call in callStack.Calls)
         {
            Trace.Write("Activity ID = " + call.ActivityId + ",");
            Trace.Write(" Address = " + call.Address + ",");
            Trace.Write(" Authentication = " + call.Authentication + ",");
            Trace.Write(" Time = " + call.CallTime + ",");
            Trace.Write(" Identity = " + call.IdentityName + ",");
            Trace.Write(" Operation = " + call.Operation + ",");
            Trace.WriteLine(" Caller = " + call.CallerType);
         }
      }
   }

    public class SecurityCallStackOperationInterceptorAttribute : OperationInterceptorBehaviorAttribute
   {
       protected override OperationInvoker CreateInvoker(IOperationInvoker oldInvoker)
      {
         return new SecurityCallStackInvoker(oldInvoker);
      }
   }
    public class SecurityCallStackServiceInterceptorAttribute : ServiceInterceptorBehaviorAttribute
   {
      protected override OperationInterceptorBehaviorAttribute CreateOperationInterceptor()
      {
          return new SecurityCallStackOperationInterceptorAttribute();
      }
   }
}