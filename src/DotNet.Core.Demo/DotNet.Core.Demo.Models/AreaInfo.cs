using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Standard.NParsing.ComponentModel;

namespace DotNet.Core.Demo.Models
{
    [ObModel(Name = "Areas5")]
    public class AreaInfo : BaseInfo
    {
        [ObIdentity(ObIdentity.Database)]
        [ObProperty(Name = "Id", Length = 4, Nullable = true)]
        public override long Id { get; set; }

        [ObProperty(Name = "AreaId", Length = 4, Nullable = false)]
        public virtual string AreaId { get; set; }

        [ObProperty(Name = "Name", Length = 50, Nullable = false)]
        public virtual string Name { get; set; }

        [ObProperty(Name = "FullName", Length = 500, Nullable = false)]
        public virtual string FullName { get; set; }

        [ObProperty(Name = "ParentId", Length = 4, Nullable = false)]
        public virtual string ParentId { get; set; }

        [ObProperty(Name = "Depth", Length = 4, Nullable = false)]
        public virtual int Depth { get; set; }
    }
}
