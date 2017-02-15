// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Security.Cryptography;
using System.Xml;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    ///
    /// </summary>
    internal sealed class CryptoHmac : CryptoHash
    {
        #region properties

        internal byte[] KeyMaterial
        {
            get { return _keyMaterial; }
            set
            {
                _keyMaterial = value;
                HMAC.Key = _keyMaterial;
            }
        }
        private byte[] _keyMaterial;

        private HMAC HMAC
        {
            get { return (this.HashAlgorithm as HMAC); }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Generates a default HMAC algorithm and generate random key material.
        /// </summary>
        ///
        internal CryptoHmac()
        {
            AlgorithmName = HealthApplicationConfiguration.Current.CryptoConfiguration.HmacAlgorithmName;

            this.HashAlgorithm = ServiceLocator.Current.CryptoService.CreateHashAlgorithm(AlgorithmName);

            KeyMaterial = new byte[HMAC.Key.Length];
            CryptoUtil.GetRandomBytes(KeyMaterial);

            HMAC.Key = KeyMaterial;
        }

        internal CryptoHmac(string algName, byte[] keyMaterial)
            : base(algName)
        {
            this.HashAlgorithm =
                ServiceLocator.Current.CryptoService.CreateHmac(
                    AlgorithmName,
                    keyMaterial);
            KeyMaterial = keyMaterial;
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
        protected override string DigestAlgorithmName
        {
            get { return "hmac"; }
        }

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
            if (IsFinalized)
            {
                throw Validator.InvalidOperationException("CryptoHmacAlreadyFinalized");
            }

            IsFinalized = true;

            return new CryptoHmacFinalized(
                AlgorithmName,
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
                throw new ArgumentNullException("writer");
            }

            writer.WriteStartElement(StartElementName);
            writer.WriteAttributeString("algName", AlgorithmName);
            writer.WriteString(Convert.ToBase64String(KeyMaterial));
            writer.WriteEndElement();
        }
    }
}
