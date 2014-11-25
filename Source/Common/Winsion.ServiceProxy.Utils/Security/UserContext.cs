using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Threading;


namespace Winsion.ServiceProxy.Utils.Security
{
    [DataContract(IsReference = true, Namespace = "Winsion.Domain.Models.Security", Name = "UserContext")]
    public class UserContext
    {
        [DataMember]
        public String Token { get; set; }

        [DataMember]
        public BstarClaims Claims { get; set; }

        [DataMember]
        public string CultureInfo { get; set; }


        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void AppendContext()
        {
            var cul = Thread.CurrentThread.CurrentUICulture;
            if (cul != null)
                CultureInfo = cul.Name;

            BstarPrincipal bp = Thread.CurrentPrincipal as BstarPrincipal;
            if (bp != null)
            {
                Token = bp.AuthToken;

                if (bp.Claims != null)
                {
                    Claims = bp.Claims.Clone();
                }
            }
        }
    }
}
