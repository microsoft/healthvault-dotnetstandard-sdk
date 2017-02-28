// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Blue Button text files are health documents provided by organizations that
    /// support the Blue Button initiative. The XML portion of the Blue Button
    /// text file type represents metadata about the document and its format.
    /// The document itself is uploaded as a BLOB instance of the thing.
    /// </summary>
    ///
    public class BlueButtonTextFile : HealthRecordItem
    {
        /// <summary>
        /// Creates a <see cref="BlueButtonTextFile"/> item instance using the specified file path and content
        /// type.
        /// </summary>
        ///
        /// <param name="record">
        /// The record to stream the data to.
        /// </param>
        ///
        /// <param name="path">
        /// The path of the file to associate with this <see cref="BlueButtonTextFile"/> instance. The instance
        /// name field is set from the file name.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <returns>
        /// A new instance of the <see cref="BlueButtonTextFile"/> health record item with data populated from the
        /// specified file path.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="path"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.
        /// </exception>
        ///
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        ///
        /// <exception cref="IOException">
        /// An I/O error occurred while opening the file.
        /// </exception>
        ///
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="path"/> was not found.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <paramref name="path"/> is in an invalid format.
        /// </exception>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <remarks>
        /// Content Type of the Blob for this <see cref="BlueButtonTextFile"/> instance is set to "text/plain".
        /// </remarks>
        ///
        public static BlueButtonTextFile CreateFromFilePath(
            HealthRecordAccessor record,
            string path,
            CodableValue sourceFormat)
        {
            // Content Type of Blob is set to "text/plain"
            CodableValue contentType = new CodableValue("text/plain");

            return CreateFromFilePath(record, path, sourceFormat, contentType);
        }

        /// <summary>
        /// Creates a <see cref="BlueButtonTextFile"/> item instance using the specified file path and content
        /// type.
        /// </summary>
        ///
        /// <param name="record">
        /// The record to stream the data to.
        /// </param>
        ///
        /// <param name="path">
        /// The path of the file to associate with this <see cref="BlueButtonTextFile"/> instance. The instance
        /// name field is set from the file name.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the Blue Button file associated with the path. This is usually something like "text/plain".
        /// </param>
        ///
        /// <returns>
        /// A new instance of the <see cref="BlueButtonTextFile"/> health record item with data populated from the
        /// specified file path.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="path"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.
        /// </exception>
        ///
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        ///
        /// <exception cref="IOException">
        /// An I/O error occurred while opening the file.
        /// </exception>
        ///
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="path"/> was not found.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <paramref name="path"/> is in an invalid format.
        /// </exception>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        ///
        public static BlueButtonTextFile CreateFromFilePath(
            HealthRecordAccessor record,
            string path,
            CodableValue sourceFormat,
            CodableValue contentType)
        {
            BlueButtonTextFile blueButtonTextFile = new BlueButtonTextFile();
            blueButtonTextFile.SetContent(record, path, sourceFormat, contentType);
            return blueButtonTextFile;
        }

        /// <summary>
        /// Creates a <see cref="BlueButtonTextFile"/> item instance using the specified stream and content
        /// type.
        /// </summary>
        ///
        /// <param name="record">
        /// The record to stream the data to.
        /// </param>
        ///
        /// <param name="stream">
        /// The stream containing the data to associate with this <see cref="BlueButtonTextFile"/> instance.
        /// </param>
        ///
        /// <param name="name">
        /// A name describing the Blue Button instance. For example this could be
        /// the original name of the file, with the extension if it is available.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <returns>
        /// A new instance of the <see cref="BlueButtonTextFile"/> health record item with data populated from the
        /// specified stream.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="record"/> or <paramref name="stream"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If a failure occurs streaming the data to HealthVault.
        /// </exception>
        ///
        /// <remarks>
        /// Content Type of the Blob for this <see cref="BlueButtonTextFile"/> instance is set to "text/plain".
        /// </remarks>
        public static BlueButtonTextFile CreateFromStream(
            HealthRecordAccessor record,
            Stream stream,
            string name,
            CodableValue sourceFormat)
        {
            // Content Type of Blob is set to "text/plain"
            CodableValue contentType = new CodableValue("text/plain");

            return CreateFromStream(record, stream, name, sourceFormat, contentType);
        }

        /// <summary>
        /// Creates a <see cref="BlueButtonTextFile"/> item instance using the specified stream and content
        /// type.
        /// </summary>
        ///
        /// <param name="record">
        /// The record to stream the data to.
        /// </param>
        ///
        /// <param name="stream">
        /// The stream containing the data to associate with this <see cref="BlueButtonTextFile"/> instance.
        /// </param>
        ///
        /// <param name="name">
        /// A name describing the Blue Button instance. For example this could be
        /// the original name of the file, with the extension if it is available.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the record stream. This is usually something like "text/plain".
        /// </param>
        ///
        /// <returns>
        /// A new instance of the <see cref="BlueButtonTextFile"/> health record item with data populated from the
        /// specified stream.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="record"/> or <paramref name="stream"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If a failure occurs streaming the data to HealthVault.
        /// </exception>
        ///
        public static BlueButtonTextFile CreateFromStream(
            HealthRecordAccessor record,
            Stream stream,
            string name,
            CodableValue sourceFormat,
            CodableValue contentType)
        {
            BlueButtonTextFile blueButtonTextFile = new BlueButtonTextFile();
            blueButtonTextFile.SetContent(record, stream, name, sourceFormat, contentType);
            return blueButtonTextFile;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BlueButtonTextFile"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public BlueButtonTextFile()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BlueButtonTextFile"/> class with the
        /// specified name and source format.
        /// </summary>
        ///
        /// <param name="name">
        /// A name describing the Blue Button instance. For example this could be
        /// the original name of the file, with the extension if it is available.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public BlueButtonTextFile(string name, CodableValue sourceFormat)
            : base(TypeId)
        {
            this.Name = name;
            this.SourceFormat = sourceFormat;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BlueButtonTextFile"/> class with the
        /// specified name.
        /// </summary>
        ///
        /// <param name="name">
        /// A name describing the Blue Button instance. For example this could be
        /// the original name of the file, with the extension if it is available.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public BlueButtonTextFile(string name)
            : base(TypeId)
        {
            this.Name = name;
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
            new Guid("2FA3D1C1-DB8C-4C0D-9873-FB01F0659360");

        /// <summary>
        /// Populates this <see cref="BlueButtonTextFile"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the Blue Button text file data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a blue-button-text-file node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("blue-button-text-file");

            Validator.ThrowInvalidIfNull(itemNav, "BlueButtonTextFileUnexpectedNode");

            this.name = itemNav.SelectSingleNode("name").Value;

            this.sourceFormat = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "source-format");
        }

        /// <summary>
        /// Writes the Blue Button text file data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the Blue Button text file data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIf(string.IsNullOrEmpty(this.name), "BlueButtonTextFileNameNotSet");

            // <blue-button-text-file>
            writer.WriteStartElement("blue-button-text-file");

            writer.WriteElementString("name", this.name);

            XmlWriterHelper.WriteOpt(writer, "source-format", this.sourceFormat);

            // </blue-button-text-file>
            writer.WriteEndElement();
        }

        /// <summary>
        /// A name describing the Blue Button instance. For example this could be
        /// the original name of the file, with the extension if it is available.
        /// </summary>
        ///
        /// <value>
        /// A string representing the name of <see cref="BlueButtonTextFile"/> instance.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b> or empty on set.
        /// </exception>
        ///
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                this.name = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets the source format of the Blue Button text file like 'VA' or 'CMS'.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> representing the source format of the Blue Button text file.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the source format is not specified.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </remarks>
        ///
        public CodableValue SourceFormat
        {
            get { return this.sourceFormat; }
            set { this.sourceFormat = value; }
        }

        private CodableValue sourceFormat;

        /// <summary>
        /// Gets the content of the Blue Button text file item.
        /// </summary>
        ///
        public string Content
        {
            get
            {
                string result = null;
                BlobStore store = this.GetBlobStore(default(HealthRecordAccessor));
                if (store.Count > 0 && store[string.Empty] != null)
                {
                    result = store[string.Empty].ReadAsString();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a stream to read the Blue Button text file contents.
        /// </summary>
        ///
        public Stream ContentStream
        {
            get
            {
                Stream result = null;
                BlobStore store = this.GetBlobStore(default(HealthRecordAccessor));
                if (store.Count > 0 && store[string.Empty] != null)
                {
                    result = store[string.Empty].GetReaderStream();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a string representation of the Blue Button text file name.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the Blue Button text file name.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Sets the blob contents of the Blue Button text file instance using the specified file.
        /// </summary>
        ///
        /// <param name="record">
        /// The record to stream the data to. This cannot be null.
        /// </param>
        /// <param name="path">
        /// The path of the file to associate with this <see cref="BlueButtonTextFile"/> instance. The instance
        /// name field is set from the file name. This cannot be null.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="path"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.
        /// </exception>
        ///
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        ///
        /// <exception cref="IOException">
        /// An I/O error occurred while opening the file.
        /// </exception>
        ///
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="path"/> was not found.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <paramref name="path"/> is in an invalid format.
        /// </exception>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <remarks>
        /// Content Type of the Blob is set to "text/plain".
        /// </remarks>
        ///
        public void SetContent(
            HealthRecordAccessor record,
            string path,
            CodableValue sourceFormat)
        {
            // Content Type of the Blob is set to "text/plain"
            CodableValue contentType = new CodableValue("text/plain");

            this.SetContent(record, path, sourceFormat, contentType);
        }

        /// <summary>
        /// Sets the blob contents of the Blue Button text file instance using the specified file.
        /// </summary>
        ///
        /// <param name="record">
        /// The record to stream the data to. This cannot be null.
        /// </param>
        /// <param name="path">
        /// The path of the file to associate with this <see cref="BlueButtonTextFile"/> instance. The instance
        /// name field is set from the file name. This cannot be null.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the Blue Button file associated with the path. This is usually something like "text/plain".
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="path"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.
        /// </exception>
        ///
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        ///
        /// <exception cref="IOException">
        /// An I/O error occurred while opening the file.
        /// </exception>
        ///
        /// <exception cref="UnauthorizedAccessException">
        /// <paramref name="path"/> specified a file that is read-only.
        /// -or-
        /// This operation is not supported on the current platform.
        /// -or-
        /// path specified a directory.
        /// -or-
        /// The caller does not have the required permission.
        /// </exception>
        ///
        /// <exception cref="FileNotFoundException">
        /// The file specified in <paramref name="path"/> was not found.
        /// </exception>
        ///
        /// <exception cref="NotSupportedException">
        /// <paramref name="path"/> is in an invalid format.
        /// </exception>
        ///
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        ///
        public void SetContent(
            HealthRecordAccessor record,
            string path,
            CodableValue sourceFormat,
            CodableValue contentType)
        {
            Validator.ThrowIfArgumentNull(record, "record", "FileRecordMustBeSpecified");
            Validator.ThrowIfStringNullOrEmpty(path, "path");
            Validator.ThrowIfArgumentNull(contentType, "contentType", "BlueButtonFileContentTypeNotSet");

            FileInfo fileInfo = new FileInfo(path);

            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                this.SetContent(record, fileStream, fileInfo.Name, sourceFormat, contentType);
            }
        }

        /// <summary>
        /// Sets the content of the Blue Button text file instance using the specified stream.
        /// </summary>
        ///
        /// <param name="record">
        /// The record to stream the data to. This cannot be null.
        /// </param>
        ///
        /// <param name="stream">
        /// The stream containing the data to associate with this <see cref="BlueButtonTextFile"/> instance.
        /// This cannot be null.
        /// </param>
        ///
        /// <param name="name">
        /// A name describing the Blue Button instance. For example this could be
        /// the original name of the file, with the extension if it is available.
        /// This cannot be null.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="record"/> or <paramref name="stream"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If a failure occurs streaming the data to HealthVault.
        /// </exception>
        ///
        /// <remarks>
        /// Content Type of the Blob is set to "text/plain".
        /// </remarks>
        ///
        public void SetContent(
            HealthRecordAccessor record,
            Stream stream,
            string name,
            CodableValue sourceFormat)
        {
            // Content Type of the Blob is set to "text/plain"
            CodableValue contentType = new CodableValue("text/plain");

            this.SetContent(record, stream, name, sourceFormat, contentType);
        }

        /// <summary>
        /// Sets the content of the Blue Button text file instance using the specified stream.
        /// </summary>
        ///
        /// <param name="record">
        /// The record to stream the data to. This cannot be null.
        /// </param>
        ///
        /// <param name="stream">
        /// The stream containing the data to associate with this <see cref="BlueButtonTextFile"/> instance.
        /// This cannot be null.
        /// </param>
        ///
        /// <param name="name">
        /// A name describing the Blue Button instance. For example this could be
        /// the original name of the file, with the extension if it is available.
        /// This cannot be null.
        /// </param>
        ///
        /// <param name="sourceFormat">
        /// Blue Button text file's source format like 'VA' or 'CMS'. This is an optional parameter and can be null.
        /// The preferred vocabulary is blue-button-source-formats.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the record stream. This is usually something like "text/plain".
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="record"/> or <paramref name="stream"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If a failure occurs streaming the data to HealthVault.
        /// </exception>
        ///
        public void SetContent(
            HealthRecordAccessor record,
            Stream stream,
            string name,
            CodableValue sourceFormat,
            CodableValue contentType)
        {
            Validator.ThrowIfArgumentNull(record, "record", "FileRecordMustBeSpecified");
            Validator.ThrowIfArgumentNull(stream, "stream", "FileStreamMustBeSpecified");
            Validator.ThrowIfStringNullOrEmpty(name, "name");
            Validator.ThrowIfArgumentNull(contentType, "contentType", "BlueButtonStreamContentTypeNotSet");

            this.Name = name;
            this.SourceFormat = sourceFormat;

            BlobStore store = this.GetBlobStore(record);
            Blob blob = store.NewBlob(string.Empty, contentType.Text);
            blob.Write(stream);
        }
    }
}
