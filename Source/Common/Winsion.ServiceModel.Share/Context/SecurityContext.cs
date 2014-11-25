// ?2011 IDesign Inc. 
// Questions? Comments? go to 
// http://www.idesign.net

using System.Runtime.Serialization;

using Winsion.ServiceModel.Share.Security;
using Winsion.Core.WCF.Context;

namespace Winsion.ServiceModel.Share.Context
{
    [DataContract(Namespace = "Winsion.Core.WCF.Context")]
    public class SecurityContext
    {
        public static UserContext Current
        {
            get
            {
                if (GenericContext<UserContext>.Current == null)
                {
                    return null;
                }
                return GenericContext<UserContext>.Current.Value;
            }
            set
            {
                GenericContext<UserContext>.Current = new GenericContext<UserContext>(value);
            }
        }
    }
}