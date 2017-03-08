// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Security.Cryptography;
using System.Xml;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Class to generate a HMAC cryptographic algorithm and random key material.
    /// </summary>
    internal sealed class CryptoHmac : CryptoHash
    {
        private Lazy<IConfiguration> configuration = Ioc.Get<Lazy<IConfiguration>>();

        #region properties

        internal byte[] KeyMaterial
        {
            get { return this.keyMaterial; }

            set
            {
                this.keyMaterial = value;
                this.HMAC.Key = this.keyMaterial;
            }
        }

        private byte[] keyMaterial;

        private HMAC HMAC => this.HashAlgorithm as HMAC;

        #endregion

        #region ctor

        /// <summary>
        /// Generates a default HMAC algorithm and generate random key material.
        /// </summary>
        ///
        internal CryptoHmac()
        {
            this.AlgorithmName = this.configuration.Value.CryptoConfiguration.HmacAlgorithmName;

            this.HashAlgorithm = ServiceLocator.Current.CryptoService.CreateHashAlgorithm(this.AlgorithmName);

            this.KeyMaterial = new byte[this.HMAC.Key.Length];
            CryptoUtil.GetRandomBytes(this.KeyMaterial);

            this.HMAC.Key = this.KeyMaterial;
        }

        internal CryptoHmac(string algName, byte[] keyMaterial)
            : base(algName)
        {
            this.HashAlgorithm =
                ServiceLocator.Current.CryptoService.CreateHmac(
                    this.AlgorithmName,
                    keyMaterial);
            this.KeyMaterial = keyMaterial;
        }

        #endregion

        /// <summary>
        /// Gets the digest algorithm name.
        /// </summary>
        ///
        /// <remarks>
        /// Child classes must specify the name of the digest
        /// algorithm they implement.
        /// This method is only called internally and is subject to change.
        /// </remarks>
        ///
        protected override string DigestAlgorithmName => "hmac";

        /// <summary>
        /// Constructs the representation of the finalized HMAC state.
        /// </summary>
        ///
        /// <remarks>
        /// This method is only called internally and is subject to change.
        /// </remarks>
        ///
        /// <returns>
        /// A <see cref="CryptoHashFinalized"/> object representing the finalized state
        /// of the hash object is returned.
        /// </returns>
        ///
        public override CryptoHashFinalized Finalize()
        {
            if (this.IsFinalized)
            {
                throw Validator.InvalidOperationException("CryptoHmacAlreadyFinalized");
            }

            this.IsFinalized = true;

            return new CryptoHmacFinalized(
                this.AlgorithmName,
                this.Hash);
        }

        /// <summary>
        /// Writes the xml that will be used when authenticating with the
        /// Microsoft Health Service.
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
        /// If <paramref name="writer"/> is null.
        /// </exception>
        ///
        public override void WriteInfoXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStartElement(this.StartElementName);
            writer.WriteAttributeString("algName", this.AlgorithmName);
            writer.WriteString(Convert.ToBase64String(this.KeyMaterial));
            writer.WriteEndElement();
        }
    }
}
