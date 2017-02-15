using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Provides Crypto Configuration
    /// </summary>
    public interface ICryptoConfiguration
    {
        /// <summary>
        /// Gets the preferred application-wide Hash Message Authentication Code 
        /// (HMAC) algorithm name.
        /// </summary>
        /// 
        /// <remarks>
        /// The application-wide algorithm name may be specified in the 
        /// configuration, but if it is not, then a default value is used.  
        /// This algorithm name can be used to construct an HMAC primitive 
        /// using <see cref="CreateHmac(string)"/>.
        /// </remarks>
        /// 
        /// <returns>
        /// The HMAC algorithm name.
        /// </returns>
        /// 
        string HmacAlgorithmName { get; set; }
    }
}
