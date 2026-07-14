
using DotNet.Core.Demo.Models;
using DotNet.Standard.NParsing.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DawnZhu.NBubble.Models.SystemModels
{
    [ObModel(Name = "Sys_Users")]
    public class UserInfo : BaseInfo
    {
        [ObProperty(Name = "UserName", Length = 50, Nullable = false)]
        public virtual string UserName { get; set; }

        [ObProperty(Name = "PassWord", Length = 50, Nullable = false)]
        public virtual string PassWord { get; set; }

        [ObProperty(Name = "RoleId", Length = 8, Nullable = false)]
        public long RoleId { get; set; }

        [ObProperty(Name = "ServiceDuedate", Length = 8, Precision = 3, Nullable = false)]
        public DateTime ServiceDuedate { get; set; }

        [ObProperty(Name = "IsMain", Length = 1, Nullable = false)]
        public bool IsMain { get; set; }

        [ObProperty(Name = "SessionID", Length = 50, Nullable = true)]
        public string SessionId { get; set; }

        [ObProperty(Name = "LoginTime", Length = 8, Precision = 3, Nullable = true)]
        public DateTime LoginTime { get; set; }

        [ObProperty(Name = "SessionID", Length = 50, Nullable = true)]
        public string LoginIpAddress { get; set; }

        [ObProperty(Name = "SessionID", Length = 50, Nullable = true)]
        public string LoginArea { get; set; }

        [ObProperty(Name = "SessionID", Length = 50, Nullable = true)]
        public string OldSessionId { get; set; }
    }
}
