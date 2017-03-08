// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Encapsulates a cryptographic hash primitive and provides additional
    /// functionality to communicate the hash state via XML.
    /// </summary>
    ///
    /// <remarks>
    /// CryptoHash is for internal use only and is subject to change.
    /// </remarks>
    ///
    public class CryptoHash
    {
        private static Lazy<IConfiguration> configuration = Ioc.Get<Lazy<IConfiguration>>();

        #region properties

        /// <summary>
        /// Gets or sets the hash algorithm that instantiates the hash primitive.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// <see cref="AlgorithmName"/> is set to <b>null</b> or empty.
        /// </exception>
        ///
        protected internal string AlgorithmName
        {
            get { return this.algName; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("value");
                }

                this.algName = value;
            }
        }

        private string algName;

        /// <summary>
        /// Gets or sets an instance of the specified hash algorithm.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// <see cref="HashAlgorithm"/> is set to <b>null</b>.
        /// </exception>
        ///
        protected HashAlgorithm HashAlgorithm
        {
            get { return this.hashAlg; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("value");
                }

                this.hashAlg = value;
            }
        }

        private HashAlgorithm hashAlg;

        /// <summary>
        /// Gets or sets a flag indicating whether the hash is already finalized.
        /// </summary>
        ///
        /// <returns>
        /// <b>true</b> if the hash is already finalized; otherwise, <b>false</b>.
        /// </returns>
        ///
        /// <remarks>
        /// To reuse the <see cref="CryptoHash"/> instance, call Reset().
        /// </remarks>
        ///
        protected bool IsFinalized { get; set; }

        /// <summary>
        /// Gets or sets the computed hash.
        /// </summary>
        protected internal byte[] Hash { get; set; }

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoHash"/> class with
        /// default values.
        /// </summary>
        ///
        /// <remarks>
        /// The default hash algorithm specified by <see cref="BaseCryptoConfiguration"/>
        /// determines which hash primitive to use.
        /// </remarks>
        ///
        public CryptoHash()
            : this(configuration.Value.CryptoConfiguration.HashAlgorithmName)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoHash"/> class with
        /// the specified hash algorithm name.
        /// </summary>
        ///
        /// <remarks>
        /// The caller must use <see cref="WriteInfoXml"/> to generate XML
        /// representing the final digest and <see cref="AlgorithmName"/>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="algorithmName"/> parameter is not supported.
        /// </exception>
        ///
        /// <param name="algorithmName">
        /// The well-known algorithm name for the hash primitive.
        /// </param>
        ///
        public CryptoHash(string algorithmName)
        {
            this.AlgorithmName = algorithmName;
            this.HashAlgorithm = ServiceLocator.Current.CryptoService.CreateHashAlgorithm(this.AlgorithmName);
        }

        #endregion

        /// <summary>
        /// Resets the state of the <see cref="CryptoHash"/> instance and the
        /// underlying hash primitive.
        /// </summary>
        ///
        /// <remarks>
        /// If the <see cref="CryptoHash"/> instance is already finalized,
        /// this will reset it so that the instance can be reused. Calling this
        /// method resets all data, so you must call <see cref="Finalize"/>
        /// beforehand to retrieve the finalized hash data.
        /// </remarks>
        ///
        internal virtual void Reset()
        {
            this.IsFinalized = false;

            this.HashAlgorithm.Initialize();
        }

        /// <summary>
        /// Applies the current hash algorithm to the specified data, beginning
        /// at the specified index.
        /// </summary>
        ///
        /// <param name="buffer">
        /// An array of bytes representing the UTF8 encoded data.
        /// </param>
        ///
        /// <param name="index">
        /// An integer representing the starting location in the byte array.
        /// </param>
        ///
        /// <param name="count">
        /// An integer representing the count of bytes.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsFinalized"/> is <b>true</b>.
        /// </exception>
        ///
        internal virtual void ComputeHash(byte[] buffer, int index, int count)
        {
            if (this.IsFinalized)
            {
                throw Validator.InvalidOperationException("CryptoHashAlreadyFinalized");
            }

            this.Hash = this.HashAlgorithm.ComputeHash(buffer, index, count);
        }

        /// <summary>
        /// Applies the current hash algorithm to the specified data.
        /// </summary>
        ///
        /// <param name="buffer">
        /// An array of bytes representing the data to be hashed.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsFinalized"/> is <b>true</b>.
        /// </exception>
        ///
        internal virtual void ComputeHash(byte[] buffer)
        {
           this.ComputeHash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Applies the current hash algorithm to the provided string data.
        /// </summary>
        ///
        /// <param name="data">
        /// The string data to hash. The string data is UTF8 encoded and
        /// then hashed.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="data"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        internal void ComputeHash(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException("data");
            }

            this.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Constructs the representation of the finalized hash state.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="CryptoHashFinalized"/> object representing the
        /// finalized state of the hash object is returned.
        /// </returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsFinalized"/> is <b>true</b>.
        /// </exception>
        ///
        public virtual CryptoHashFinalized Finalize()
        {
            if (this.IsFinalized)
            {
                throw Validator.InvalidOperationException("CryptoHashAlreadyFinalized");
            }

            this.IsFinalized = true;

            return new CryptoHashFinalized(this.AlgorithmName, this.Hash);
        }

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
        protected virtual string DigestAlgorithmName => "hash";

        /// <summary>
        /// Gets the name of the start element for the serialized info XML.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the XML element name of the
        /// section that contains the content written with
        /// <see cref="WriteInfoXml"/>.
        /// </returns>
        ///
        protected string StartElementName => this.DigestAlgorithmName + "-alg";

        /// <summary>
        /// Writes the XML to use when authenticating with the HealthVault
        /// service.
        /// </summary>
        ///
        /// <remarks>
        /// This method is only called internally and is subject to change.
        /// </remarks>
        ///
        /// <param name="writer">
        /// The XML writer receiving the data.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public virtual void WriteInfoXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStartElement(this.StartElementName);
            writer.WriteAttributeString("algName", this.AlgorithmName);
            writer.WriteEndElement();
        }

        internal string GetInfoXml()
        {
            StringBuilder infoXml = new StringBuilder(256);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer =
                       XmlWriter.Create(infoXml, settings))
            {
                this.WriteInfoXml(writer);
                writer.Flush();
            }

            return infoXml.ToString();
        }

        /// <summary>
        /// Generates the info hash section for HealthVault service
        /// web requests given the specified string data.
        /// </summary>
        ///
        /// <param name="text">
        /// A string representing the data.
        /// </param>
        ///
        /// <returns>
        /// A string representing the info hash.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="text"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        internal static string CreateInfoHash(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text");
            }

            return CreateInfoHash(new UTF8Encoding().GetBytes(text));
        }

        /// <summary>
        /// Generates the info hash section for HealthVault service
        /// web requests given the specified data.
        /// </summary>
        ///
        /// <param name="buffer">
        /// An array of bytes representing the UTF8
        /// encoded data.
        /// </param>
        ///
        /// <returns>
        /// A string representing the info hash.
        /// </returns>
        ///
        internal static string CreateInfoHash(byte[] buffer)
        {
            return CreateInfoHash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Generates the info hash section for HealthVault service
        /// web requests given the specified data beginning at the specified
        /// index.
        /// </summary>
        ///
        /// <param name="buffer">
        /// An array of bytes representing the UTF8 encoded data.
        /// </param>
        ///
        /// <param name="index">
        /// An integer representing the starting location in the byte array.
        /// </param>
        ///
        /// <param name="count">
        /// An integer representing the count of bytes.
        /// </param>
        ///
        /// <returns>
        /// A string representing the info hash.
        /// </returns>
        ///
        internal static string CreateInfoHash(byte[] buffer, int index, int count)
        {
            CryptoHash hash = new CryptoHash();
            hash.ComputeHash(buffer, index, count);
            return hash.Finalize().GetXml();
        }
    }
}
