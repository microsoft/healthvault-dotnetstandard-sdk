using System;
using System.Collections.Generic;

namespace Microsoft.HealthVault
{
    //TODO: this is a placeholder class
    public class HttpContext
    {
        public static HttpContext Current = new HttpContext();

        public IDictionary<string, Guid> Items { get; }
    }
}
