// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Application
{
    /// <summary>
    /// Represents configuration data for an application which can be read from a file or stream
    /// and has an associated content type.
    /// </summary>
    ///
    /// <remarks>
    /// HealthVault applications can be configured with logos, privacy statements, and terms of
    /// use statements that can be read from a file or stream. This class wraps the content and
    /// content type for that configuration.
    /// </remarks>
    ///
    public class ApplicationBinaryConfiguration
    {
        /// <summary>
        /// Constructs a <see cref="ApplicationBinaryConfiguration"/> with default values.
        /// </summary>
        ///
        public ApplicationBinaryConfiguration()
        {
        }

        /// <summary>
        /// Constructs a <see cref="ApplicationBinaryConfiguration"/> with the specified file and
        /// content type.
        /// </summary>
        ///
        /// <param name="binaryConfigurationFilePath">
        /// A local path to a file to use as the content.
        /// </param>
        ///
        /// <param name="contentType">
        /// The MIME type of the content in the file specified by
        /// <paramref name="binaryConfigurationFilePath"/>.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="binaryConfigurationFilePath"/> or <paramref name="contentType"/> is
        /// <b>null</b> or empty,<br/>
        /// or <paramref name="binaryConfigurationFilePath"/> contains one or more invalid characters.
        /// </exception>
        ///
        /// <exception cref="PathTooLongException">
        /// If <paramref name="binaryConfigurationFilePath"/> is too long.
        /// </exception>
        ///
        /// <exception cref="DirectoryNotFoundException">
        /// If <paramref name="binaryConfigurationFilePath"/> is invalid (for example, it is on an
        /// unmapped drive).
        /// </exception>
        ///
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="binaryConfigurationFilePath"/> is a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// <paramref name="binaryConfigurationFilePath"/> specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="binaryConfigurationFilePath"/> was not found.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <paramref name="binaryConfigurationFilePath"/> is in an invalid format.
        /// </exception>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        ///
        public ApplicationBinaryConfiguration(string binaryConfigurationFilePath, string contentType)
        {
            Validator.ThrowIfStringNullOrEmpty(binaryConfigurationFilePath, "binaryConfigurationFilePath");
            Validator.ThrowIfStringNullOrEmpty(contentType, "contentType");

            this.BinaryConfigurationContent = File.ReadAllBytes(binaryConfigurationFilePath);
            this.ContentType = contentType;
        }

        /// <summary>
        /// Constructs a <see cref="ApplicationBinaryConfiguration"/> with the specified stream and
        /// content type.
        /// </summary>
        ///
        /// <param name="binaryConfigurationContent">
        /// A stream containing the content.
        /// </param>
        ///
        /// <param name="contentType">
        /// The MIME type of the content in the stream specified by
        /// <paramref name="binaryConfigurationContent"/>.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="binaryConfigurationContent"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="contentType"/> is
        /// <b>null</b> or empty.
        /// </exception>
        ///
        public ApplicationBinaryConfiguration(Stream binaryConfigurationContent, string contentType)
        {
            Validator.ThrowIfArgumentNull(
                binaryConfigurationContent,
                "binaryConfigurationContent",
                "ApplicationBinaryConfigurationContentStreamRequired");

            Validator.ThrowIfStringNullOrEmpty(contentType, "contentType");

            this.BinaryConfigurationContent = new byte[binaryConfigurationContent.Length];
            int numberOfBytesToRead = (int)binaryConfigurationContent.Length;
            int bytesRead = 0;

            while (numberOfBytesToRead > 0)
            {
                int bytesReceived =
                    binaryConfigurationContent.Read(
                        this.BinaryConfigurationContent,
                        bytesRead,
                        numberOfBytesToRead);
                bytesRead += bytesReceived;
                numberOfBytesToRead -= bytesReceived;
            }

            this.ContentType = contentType;
        }

        /// <summary>
        /// Creates an instance from XML.
        /// </summary>
        ///
        /// <param name="containerNav">
        /// The container nav.
        /// </param>
        ///
        /// <param name="outerElement">
        /// The outer element.
        /// </param>
        ///
        /// <param name="dataElement">
        /// The data element.
        /// </param>
        ///
        /// <param name="contentTypeElement">
        /// The content type element.
        /// </param>
        ///
        /// <returns>
        /// Configuration data for an application which can be read from a file or stream
        /// and has an associated content type.
        /// </returns>
        ///
        internal static ApplicationBinaryConfiguration CreateFromXml(
            XPathNavigator containerNav,
            string outerElement,
            string dataElement,
            string contentTypeElement)
        {
            ApplicationBinaryConfiguration binaryConfig = null;
            XPathNavigator outerNav = containerNav.SelectSingleNode(outerElement);
            if (outerNav != null)
            {
                binaryConfig = new ApplicationBinaryConfiguration();
                binaryConfig.CultureSpecificBinaryConfigurationContents.PopulateFromXml(
                    outerNav,
                    dataElement);
                binaryConfig.ContentType = outerNav.SelectSingleNode(contentTypeElement).Value;
            }

            return binaryConfig;
        }

        internal void AppendRequestParameters(
            XmlWriter writer,
            string outerElementName,
            string dataElementName)
        {
            writer.WriteStartElement(outerElementName);

            this.CultureSpecificBinaryConfigurationContents.AppendLocalizedElements(
                writer,
                dataElementName);

            writer.WriteElementString("content-type", this.ContentType);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the binary content.
        /// </summary>
        ///
        public byte[] BinaryConfigurationContent
        {
            get { return this.CultureSpecificBinaryConfigurationContents.BestValue; }
            set { this.CultureSpecificBinaryConfigurationContents.DefaultValue = value; }
        }

        /// <summary>
        ///     Gets a dictionary of language specifiers and localized content.
        /// </summary>
        public CultureSpecificByteArrayDictionary CultureSpecificBinaryConfigurationContents { get; } = new CultureSpecificByteArrayDictionary();

        /// <summary>
        /// Gets or sets the MIME type of the content.
        /// </summary>
        ///
        public string ContentType { get; set; }
    }
}
