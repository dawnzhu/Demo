﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DotNet.Demo.WebAPI.Filters
{
    public class CustomerAuthorizationFilterAttribute : AuthorizationFilterAttribute
    {

    }
}
