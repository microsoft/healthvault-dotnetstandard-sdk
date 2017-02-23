// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Represents finalized hash states that are sent via XML requests.
    /// </summary>
    ///
    /// <remarks>
    /// HealthVault verifies the hash against this object's digest.
    /// </remarks>
    ///
    public class CryptoHashFinalized
    {
        #region properties

        internal string Digest { get; private set; }

        internal string AlgorithmName { get; private set; }

        internal string DigestAlgorithmName { get; set; } = "hash";

        internal string StartElementName => this.DigestAlgorithmName + "-data";

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoHashFinalized"/>
        /// class.
        /// </summary>
        ///
        /// <remarks>
        /// This default constructor is included because of mandatory design
        /// considerations. To actually use this class, you must create an
        /// instance of it using the parameterized constructor.
        /// </remarks>
        ///
        internal CryptoHashFinalized()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoHashFinalized"/>
        /// class with the known algorithm name and finalized hash digest.
        /// </summary>
        ///
        /// <param name="algorithmName">
        /// A string representing the algorithm name.
        /// </param>
        ///
        /// <param name="digest">
        /// An array of bytes representing the finalized hash digest.
        /// </param>
        ///
        public CryptoHashFinalized(
            string algorithmName,
            byte[] digest)
        {
            this.Digest = Convert.ToBase64String(digest);
            this.AlgorithmName = algorithmName;
        }

        #endregion

        /// <summary>
        /// Generates the corresponding XML for the finalized state.
        /// </summary>
        ///
        internal virtual string GetXml()
        {
            StringBuilder builder = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                this.WriteInfoXml(writer);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Writes the XML that is used when authenticating with the
        /// HealthVault Service.
        /// </summary>
        ///
        /// <remarks>
        /// This method is only called internally and is subject to change.
        /// </remarks>
        ///
        /// <param name="writer">
        /// The XML writer that will be written to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        internal virtual void WriteInfoXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.WriteStartElement(this.StartElementName);
            writer.WriteAttributeString("algName", this.AlgorithmName);
            writer.WriteString(this.Digest);
            writer.WriteEndElement();
        }
    }
}
