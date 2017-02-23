// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Vocabulary
{
    /// <summary>
    /// Represents the converter class that converts values
    /// between different vocabulary items.
    /// </summary>
    ///
    public static class VocabularyItemDataConverter
    {
        /// <summary>
        /// Converts a value from the source vocabulary item units to
        /// the base units of the vocabulary.
        /// </summary>
        ///
        /// <remarks>
        /// This method does not check the sign of the value. The caller
        /// is responsible for ensuring the input data value make sense for
        /// the input vocabulary item.
        /// </remarks>
        ///
        /// <param name="vocabularyItem">The vocabulary item of the data
        /// value to convert to base units.</param>
        ///
        /// <param name="value">The data value to convert to base units.
        /// </param>
        ///
        /// <returns>The converted data value in the base units for the
        /// vocabulary.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="vocabularyItem"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// The <paramref name="vocabularyItem"/> parameter does not
        /// support conversions to base units.
        /// </exception>
        ///
        /// <exception cref="ConversionFailureException">
        /// An unknown or unsupported conversion is encountered or
        /// an error occurs during the conversion to base units.
        /// </exception>
        ///
        public static double ConvertToBaseUnits(
            VocabularyItem vocabularyItem,
            double value)
        {
            Validator.ThrowIfArgumentNull(vocabularyItem, "vocabularyItem", "VocabularyItemNull");

            if (vocabularyItem.InfoXml == null)
            {
                throw Validator.NotSupportedException("VocabularyItemConversionNotSupported");
            }

            XPathNavigator nav = vocabularyItem.InfoXml.CreateNavigator();
            try
            {
                nav = nav.SelectSingleNode("unit-conversions/base-unit-conversion");
            }
            catch (Exception)
            {
                // this can occur if conversion xml is corrupted. We don't
                // want to expose xml parse issues here to the outside world
                // as recommended by the .NET documentation for
                // XPathNavigator
                throw new ConversionFailureException(
                    ResourceRetriever.GetResourceString(
                        "VocabularyItemConversionGeneralException"));
            }

            if (nav == null)
            {
                throw Validator.NotSupportedException("VocabularyItemConversionNotSupported");
            }

            return DoConversion(nav, value);
        }

        private static double DoConversion(XPathNavigator nav, double val)
        {
            // only linear conversions currently supported
            nav = nav.SelectSingleNode("linear-conversion");
            if (nav == null)
            {
                throw new ConversionFailureException(
                        ResourceRetriever.GetResourceString(
                            "VocabularyItemUnsupportedConversion"));
            }

            return LinearConvert(nav, val);
        }

        private static double LinearConvert(XPathNavigator nav, double val)
        {
            // required multiplier
            XPathNavigator localNav = nav.SelectSingleNode("multiplier");
            if (localNav == null)
            {
                throw new ConversionFailureException(
                       ResourceRetriever.GetResourceString(
                           "VocabularyItemUnsupportedConversion"));
            }

            double multiplier = localNav.ValueAsDouble;

            // optional offset
            localNav = nav.SelectSingleNode("offset");
            double offset = 0.0;
            if (localNav != null)
            {
                offset = localNav.ValueAsDouble;
            }

            return val * multiplier + offset;
        }
    }
}
