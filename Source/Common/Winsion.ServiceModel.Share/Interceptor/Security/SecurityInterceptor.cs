// © 2011 IDesign Inc. 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.ServiceModel.Dispatcher;


using System.Threading;
using System.Globalization;

using Winsion.Core.WCF.Context;
using Winsion.ServiceModel.Share.Interceptor.Base;
using Winsion.ServiceModel.Share.Security;
using Winsion.ServiceModel.Share.Context;
using Winsion.Core.WCF.Security;


namespace Winsion.ServiceModel.Share.Interceptor.Security
{
    class SecurityInvoker : OperationInvoker
    {
        public SecurityInvoker(IOperationInvoker oldInvoker)
            : base(oldInvoker)
        { }

        protected override void PreInvoke(object instance, object[] inputs)
        {
            try
            {
                UserContext userContext = SecurityContext.Current;
                if (userContext != null)
                {
                    ValidateUserContext(userContext);
                    RevivalUserContext(userContext);
                }
            }
            catch (NullReferenceException)
            {
                throw new InvalidOperationException("No userContext was found. Are you using the right proxy?");
            }
        }

        void ValidateUserContext(UserContext userContext)
        {

        }

        void RevivalUserContext(UserContext userContext)
        {
            if (userContext.CultureInfo != null && userContext.CultureInfo != "")
            {
                var cul = new CultureInfo(userContext.CultureInfo);
                Thread.CurrentThread.CurrentUICulture = cul;
                Thread.CurrentThread.CurrentCulture = cul;
            }

            if (userContext.Claims != null)
                Thread.CurrentPrincipal = new BstarPrincipal(userContext.Token, userContext.Claims);

        }

    }

    public class SecurityOperationInterceptorAttribute : OperationInterceptorBehaviorAttribute
    {
        protected override OperationInvoker CreateInvoker(IOperationInvoker oldInvoker)
        {
            return new SecurityInvoker(oldInvoker);
        }
    }
    public class SecurityServiceInterceptorAttribute : ServiceInterceptorBehaviorAttribute
    {
        protected override OperationInterceptorBehaviorAttribute CreateOperationInterceptor()
        {
            return new SecurityOperationInterceptorAttribute();
        }
    }
}