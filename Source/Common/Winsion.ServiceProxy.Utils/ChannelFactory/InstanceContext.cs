

using System.ServiceModel;

namespace Winsion.ServiceProxy.Utils
{
    public class InstanceContext<TCallbackContract> 
   {
      public InstanceContext Context
      {get;private set;}

      public InstanceContext(TCallbackContract callbackInstance)
      {
         Context = new InstanceContext(callbackInstance);
      }
      public void ReleaseServiceInstance()
      {
         Context.ReleaseServiceInstance();
      }
      public TCallbackContract ServiceInstance
      {
         get
         {
             return (TCallbackContract)Context.GetServiceInstance();
         }
      }
   }
}
