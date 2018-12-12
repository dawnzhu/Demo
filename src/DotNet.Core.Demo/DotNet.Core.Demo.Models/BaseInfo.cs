using System;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NSmart;

namespace DotNet.Core.Demo.Models
{
    public class BaseInfo : DoModelBase
    {
    }

    public class BaseTerm : DoTermBase
    {
        protected BaseTerm(Type modelType) : base(modelType)
        { }

        protected BaseTerm(Type modelType, string rename) : base(modelType, rename)
        { }

        protected BaseTerm(Type modelType, ObTermBase parent, MethodBase currentMethod) : base(modelType, parent, currentMethod)
        { }
    }
}
