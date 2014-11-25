using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Winsion.ServiceProxy.Utils.Security
{
    [DataContract(IsReference = true, Namespace = "Winsion.Domain.Models.Security", Name = "BstarUser")]
    public class BstarUser
    {
        public BstarUser()
        {
            Id = -1;
            Name = String.Empty;
            Application = String.Empty;
            TokenExpiryInMins = 0;
            UserType = 0;
        }

        public BstarUser(long id, string name)
            : this()
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// 构造当前用户的对象
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="name">用户名</param>
        /// <param name="userType">用户所属组的类型，1为超级管理员，0为普通组</param>
        public BstarUser(long id, string name,int userType)
            : this()
        {
            Id = id;
            Name = name;
            UserType = userType;
        }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Application { get; set; }

        [DataMember]
        public int TokenExpiryInMins { get; set; }

        [DataMember]
        public int UserType { get; set; } // 用户所属组的类型

        public override string ToString()
        {
            return string.Format("BstarUser:[Id={0}, Name={1}, Application={2}, TokenExpiryInMins={3},UserType ={4}]", Id, Name, Application, TokenExpiryInMins, UserType);
        }

    }

    [DataContract(IsReference = true, Namespace = "Winsion.Domain.Models.Security", Name = "BstarClaims")]
    public class BstarClaims
    {
        public BstarClaims()
        {
            Roles = new List<string>();
        }

        public BstarClaims(BstarUser user)
            : this()
        {
            User = user;
        }


        [DataMember]
        public BstarUser User { get; set; }

        [DataMember]
        public List<string> Roles { get; private set; }

        [DataMember]
        public byte[] ClaimsSignedHash { get; set; }

        public BstarClaims Clone()
        {
            BstarClaims claims = null;
            if (User != null)
            {
                claims = new BstarClaims(
                    new BstarUser()
                    {
                        Id = User.Id,
                        Name = User.Name,
                        Application = User.Application,
                        TokenExpiryInMins = User.TokenExpiryInMins,
                        UserType=User.UserType,
                    });

                if (ClaimsSignedHash != null)
                {
                    byte[] hash = new byte[ClaimsSignedHash.Length];
                    ClaimsSignedHash.CopyTo(hash, 0);

                    claims.ClaimsSignedHash = hash;
                }

                foreach (var r in Roles)
                {
                    claims.Roles.Add(r);
                }
            }

            return claims;

        }

        public override string ToString()
        {
            var roles = "";
            if (Roles != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var r in Roles)
                {
                    sb.Append(r);
                    sb.Append(",");
                }
                roles = sb.ToString().Trim(',');
            }
            return string.Format("BstarClaims:[\r\nUser={0},\r\nRoles=[{1}]\r\n]", User, roles);
        }

    }
}
