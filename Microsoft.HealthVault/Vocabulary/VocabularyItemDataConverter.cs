// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
    internal static class VocabularyItemDataConverter
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
            Validator.ThrowIfArgumentNull(vocabularyItem, nameof(vocabularyItem), Resources.VocabularyItemNull);

            if (vocabularyItem.InfoXml == null)
            {
                throw new NotSupportedException(Resources.VocabularyItemConversionNotSupported);
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
                throw new ConversionFailureException(Resources.VocabularyItemConversionGeneralException);
            }

            if (nav == null)
            {
                throw new NotSupportedException(Resources.VocabularyItemConversionNotSupported);
            }

            return DoConversion(nav, value);
        }

        private static double DoConversion(XPathNavigator nav, double val)
        {
            // only linear conversions currently supported
            nav = nav.SelectSingleNode("linear-conversion");
            if (nav == null)
            {
                throw new ConversionFailureException(Resources.VocabularyItemUnsupportedConversion);
            }

            return LinearConvert(nav, val);
        }

        private static double LinearConvert(XPathNavigator nav, double val)
        {
            // required multiplier
            XPathNavigator localNav = nav.SelectSingleNode("multiplier");
            if (localNav == null)
            {
                throw new ConversionFailureException(Resources.VocabularyItemUnsupportedConversion);
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
