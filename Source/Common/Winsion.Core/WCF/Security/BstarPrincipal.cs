using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Runtime.Serialization;

namespace Winsion.Core.WCF.Security
{

    public class BstarPrincipal : IPrincipal, IIdentity
    {
        public BstarPrincipal(string token, BstarClaims claims)
        {
            AuthToken = token;
            Claims = claims;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return this; }
        }

        public bool IsInRole(string role)
        {
            role = role.ToLower();
            return Claims.Roles.Any(r => r.ToLower() == role);
        }

        #endregion

        #region IIdentity Members

        public string AuthenticationType
        {
            get { return _authenticationSignature; }
        }

        public bool IsAuthenticated
        {
            get { return Claims != null && Claims.User != null && Claims.User.Id != -1; }
        }

        public string Name
        {
            get { return Claims != null && Claims.User != null ? Claims.User.Name : ""; }
        }

        #endregion


        public string AuthToken { get; set; }


        public BstarClaims Claims { get; set; }

        public static bool IsBstarAuthenticated(IIdentity user)
        {
            return (user.AuthenticationType == _authenticationSignature);
        }

        public override string ToString()
        {
            return string.Format("BstarPrincipal:[\r\nClaims={0},\r\nAuthToken={1}\r\n]", Claims, AuthToken);
        }

        private const string _authenticationSignature = "BstarAuth";
    }
}
