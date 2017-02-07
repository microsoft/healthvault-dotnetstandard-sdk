using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Health
{
    //TODO: this is a placeholder class
    public class HttpContext
    {
        public static HttpContext Current = new HttpContext();

        public IDictionary<string, Guid> Items { get; }
    }
}
