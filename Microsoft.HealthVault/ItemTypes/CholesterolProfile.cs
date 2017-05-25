// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a person's
    /// cholesterol profile (Lipid profile) at a single point in time.
    /// </summary>
    ///
    public class CholesterolProfile : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CholesterolProfile"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public CholesterolProfile()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CholesterolProfile"/> class
        /// with the specified date.
        /// </summary>
        ///
        /// <param name="when">
        /// The date when the cholesterol profile was take.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CholesterolProfile(HealthServiceDate when)
            : base(TypeId)
        {
            When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("796C186F-B874-471c-8468-3EEFF73BF66E");

        /// <summary>
        /// Populates this <see cref="CholesterolProfile"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the cholesterol profile data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a cholesterol-profile node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "cholesterol-profile");

            Validator.ThrowInvalidIfNull(itemNav, Resources.CholesterolProfileUnexpectedNode);

            _when = new HealthServiceDate();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _ldl = XPathHelper.GetOptNavValueAsInt(itemNav, "ldl");

            _hdl = XPathHelper.GetOptNavValueAsInt(itemNav, "hdl");

            _totalCholesterol = XPathHelper.GetOptNavValueAsInt(itemNav, "total-cholesterol");

            _triglyceride = XPathHelper.GetOptNavValueAsInt(itemNav, "triglyceride");
        }

        /// <summary>
        /// Writes the cholesterol profile data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the cholesterol profile data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <cholesterol-profile>
            writer.WriteStartElement("cholesterol-profile");

            // <when>
            _when.WriteXml("when", writer);

            XmlWriterHelper.WriteOptInt(
                writer,
                "ldl",
                _ldl);

            XmlWriterHelper.WriteOptInt(
                writer,
                "hdl",
                _hdl);

            XmlWriterHelper.WriteOptInt(
                writer,
                "total-cholesterol",
                _totalCholesterol);

            XmlWriterHelper.WriteOptInt(
                writer,
                "triglyceride",
                _triglyceride);

            // </cholesterol-profile>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date when the cholesterol profile was taken.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDate When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDate _when = new HealthServiceDate();

        /// <summary>
        /// Gets or sets the person's low density lipoprotein in mg/dL.
        /// </summary>
        ///
        /// <value>
        /// The value of the person's LDL measurement or <b>null</b> if unknown.
        /// </value>
        ///
        public int? LDL
        {
            get { return _ldl; }
            set { _ldl = value; }
        }

        private int? _ldl;

        /// <summary>
        /// Gets or sets the person's high density lipoprotein in mg/dL.
        /// </summary>
        ///
        /// <value>
        /// The value of the person's HDL measurement or <b>null</b> if unknown.
        /// </value>
        ///
        public int? HDL
        {
            get { return _hdl; }
            set { _hdl = value; }
        }

        private int? _hdl;

        /// <summary>
        /// Gets or sets the person's total cholesterol in mg/dL.
        /// </summary>
        ///
        /// <value>
        /// The value of the person's total cholesterol measurement or <b>null</b> if unknown.
        /// </value>
        ///
        public int? TotalCholesterol
        {
            get { return _totalCholesterol; }
            set { _totalCholesterol = value; }
        }

        private int? _totalCholesterol;

        /// <summary>
        /// Gets or sets the person's triglyceride in mg/dL.
        /// </summary>
        ///
        /// <value>
        /// The value of the person's triglyceride measurement or <b>null</b> if unknown.
        /// </value>
        ///
        public int? Triglyceride
        {
            get { return _triglyceride; }
            set { _triglyceride = value; }
        }

        private int? _triglyceride;

        /// <summary>
        /// Gets a string representation of the cholesterol profile.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the cholesterol profile.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;
            if (TotalCholesterol != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatTotal,
                        TotalCholesterol.Value);
            }
            else if (LDL != null && HDL != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatLDLAndHDL,
                        LDL.Value,
                        HDL.Value);
            }
            else if (LDL != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatLDL,
                        LDL.Value);
            }
            else if (HDL != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatHDL,
                        HDL.Value);
            }
            else if (Triglyceride != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatTriglyceride,
                        Triglyceride.Value);
            }

            return result;
        }
    }
}
