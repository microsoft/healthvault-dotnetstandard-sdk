using System;
using System.Globalization;

namespace Microsoft.HealthVault.Extensions
{
    /// <summary>
    /// Extension methods for the string type.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Formats a resource string using the current UI culture.
        /// </summary>
        /// <param name="resourceFormat">The format string.</param>
        /// <param name="arguments">The arguments to insert.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatResource(this string resourceFormat, params object[] arguments)
        {
            return string.Format(
                CultureInfo.CurrentUICulture,
                resourceFormat,
                arguments);
        }
    }
}
