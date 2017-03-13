using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Web.Extensions
{
    internal static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Retrieves the specified setting for GUIDs.
        /// </summary>
        /// <param name="key">A string specifying the name of the setting.</param>
        /// <param name="collection">The NameValueCollection</param>
        /// <returns>
        /// The GUID of the setting.
        /// </returns>
        public static Guid GetGuid(this NameValueCollection collection, string key)
        {
            Guid result = Guid.Empty;
            string resultString = collection[key];
            if (!String.IsNullOrEmpty(resultString))
            {
                // Let the FormatException propagate out.
                result = new Guid(resultString);
            }

            return result;
        }

        /// <summary>
        /// Gets the URL value matching the configuration key.
        /// </summary>
        /// <param name="collection">The NameValueCollection</param>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="appendSlash">If set to true, append a '/' character at the end of URL.</param>
        /// <returns>URL value from configuration if exists, null if not found.</returns>
        public static Uri GetUrl(this NameValueCollection collection, string key, bool appendSlash)
        {
            string resultString = collection[key];
            if (String.IsNullOrEmpty(resultString))
            {
                return null;
            }
            else
            {
                if (appendSlash)
                {
                    return
                        new Uri(resultString.EndsWith("/", StringComparison.Ordinal)
                            ? resultString
                            : (resultString + "/"));
                }
                else
                {
                    return new Uri(resultString);
                }
            }
        }
    }
}
