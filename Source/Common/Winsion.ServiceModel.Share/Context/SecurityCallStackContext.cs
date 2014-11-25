// ?2011 IDesign Inc. 
// Questions? Comments? go to 
// http://www.idesign.net

using System.Runtime.Serialization;
using Winsion.ServiceModel.Share.Security;
using Winsion.Core.WCF.Context;

namespace Winsion.ServiceModel.Share.Context
{
    [DataContract(Namespace = "Winsion.Core.WCF.Context")]
    public class SecurityCallStackContext
    {
        public static SecurityCallStack Current
        {
            get
            {
                if (GenericContext<SecurityCallStack>.Current == null)
                {
                    return null;
                }
                return GenericContext<SecurityCallStack>.Current.Value;
            }
            set
            {
                GenericContext<SecurityCallStack>.Current = new GenericContext<SecurityCallStack>(value);
            }
        }
    }
}