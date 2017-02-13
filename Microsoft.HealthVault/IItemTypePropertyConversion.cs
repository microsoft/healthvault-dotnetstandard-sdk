// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Converts units for a numeric value of a <see cref="HealthRecordItemTypeProperty"/>.
    /// </summary>
    /// <remarks>
    /// When a thing type has multiple versions that store the same data with different units,
    /// a conversion between units may be required to ensure that values are ordered correctly
    /// across versions. The <see cref="HealthRecordItemTypeProperty"/> can define a conversion
    /// formula to perform this conversion.
    /// </remarks>
    public interface IItemTypePropertyConversion
    {
        /// <summary>
        /// Converts a value to different units. The conversion formula is determined by the
        /// implementation of the <see cref="IItemTypePropertyConversion"/> object and the
        /// parameters with which it was constructed.
        /// </summary>
        /// 
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// 
        /// <returns>
        /// The converted value.
        /// </returns>
        double Convert(double value);
    }
}
