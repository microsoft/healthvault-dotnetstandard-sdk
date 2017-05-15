// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// A collection of the BLOBs associated with a thing.
    /// </summary>
    ///
    /// <remarks>
    /// The <see cref="BlobStore"/> is a dictionary that is read-only but allows removal of
    /// <see cref="Blob"/> instances.
    /// To add <see cref="Blob"/> instances to the dictionary use the
    /// <see cref="NewBlob(string, string)"/> method.
    /// </remarks>
    ///
    internal class BlobStore : IDictionary<string, Blob>
    {
        internal BlobStore(ThingBase item, HealthRecordAccessor record)
        {
            _item = item;
            Record = record;
        }

        internal BlobStore(
            ThingBase item)
        {
            _item = item;
        }

        internal HealthRecordAccessor Record { get; set; }

        private ThingBase _item;

        #region IDictionary implementation

        /// <summary>
        /// Gets the count of BLOBs associated with the thing.
        /// </summary>
        ///
        public int Count => blobs.Count;

        /// <summary>
        /// Not supported.
        /// </summary>
        ///
        /// <remarks>
        /// Use the <see cref="NewBlob(string, string)"/> method instead.
        /// </remarks>
        ///
        public void Add(Blob blob)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        ///
        /// <remarks>
        /// Use the <see cref="NewBlob(string, string)"/> method instead.
        /// </remarks>
        ///
        public void Add(string key, Blob blob)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Clears all Blob instances from the BlobStore.
        /// </summary>
        ///
        public void Clear()
        {
            blobs.Clear();
        }

        /// <summary>
        /// Determines whether the BlobStore contains the specified Blob instance.
        /// </summary>
        ///
        /// <param name="blob">
        /// The Blob instance to locate in the BlobStore.
        /// </param>
        ///
        /// <returns>
        /// True if the Blob instance is in the store or false otherwise.
        /// </returns>
        ///
        /// <remarks>
        /// This implementation is a reference comparison and does not equate Blob instances
        /// with the same name.
        /// </remarks>
        ///
        public bool Contains(Blob blob)
        {
            return blobs.ContainsValue(blob);
        }

        /// <summary>
        /// Determines whether the BlobStore contains a Blob with the specified key.
        /// </summary>
        ///
        /// <param name="key">
        /// The key to locate in the BlobStore.
        /// </param>
        ///
        /// <returns>
        /// True if the BlobStore contains a Blob with the key; otherwise, false.
        /// </returns>
        ///
        /// <remarks>
        /// This is a case-sensitive comparison.
        /// </remarks>
        ///
        public bool ContainsKey(string key)
        {
            key = MapNullKey(key);
            return blobs.ContainsKey(key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        ///
        IEnumerator IEnumerable.GetEnumerator()
        {
            return blobs.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> object that can be used to iterate through the collection.
        /// </returns>
        ///
        IEnumerator<KeyValuePair<string, Blob>> IEnumerable<KeyValuePair<string, Blob>>.GetEnumerator()
        {
            return blobs.GetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether the BlobStore is read-only.
        /// </summary>
        ///
        /// <remarks>
        /// The BlobStore cannot be added to through the Add methods. Instead the
        /// <see cref="NewBlob(string, string)"/> method must be used. However, the <see cref="Clear"/> and
        /// <see cref="Remove"/> methods are available.
        /// </remarks>
        bool ICollection<KeyValuePair<string, Blob>>.IsReadOnly => false;

        /// <summary>
        /// Removes the first occurrence of a specific Blob from the BlobStore.
        /// </summary>
        ///
        /// <param name="item">
        /// The Blob instance to remove.
        /// </param>
        ///
        bool ICollection<KeyValuePair<string, Blob>>.Remove(KeyValuePair<string, Blob> item)
        {
            bool result = blobs.Remove(item.Key);
            if (result)
            {
                RemovedBlobs[item.Key] = item.Value;
            }

            return result;
        }

        /// <summary>
        /// Copies the elements of the BlobStore to an <see cref="Array"/>, starting at a
        /// particular <see cref="Array"/> index.
        /// </summary>
        ///
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the Blobs copied from
        /// the BlobStore. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        ///
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        ///
        void ICollection<KeyValuePair<string, Blob>>.CopyTo(
            KeyValuePair<string, Blob>[] array,
            int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, Blob>>)blobs).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Determines whether the BlobStore contains the specified item.
        /// </summary>
        ///
        /// <param name="item">
        /// The Blob to locate in the BlobStore.
        /// </param>
        ///
        /// <returns>
        /// True if <paramref name="item"/> is found in the BlobStore; otherwise, false.
        /// </returns>
        ///
        bool ICollection<KeyValuePair<string, Blob>>.Contains(KeyValuePair<string, Blob> item)
        {
            return blobs.ContainsKey(item.Key);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The item to consider adding before throwing the NotSupportedException</param>
        void ICollection<KeyValuePair<string, Blob>>.Add(KeyValuePair<string, Blob> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets an ICollection&lt;Blob&gt; containing the values in the BlobStore.
        /// </summary>
        ///
        public ICollection<Blob> Values => blobs.Values;

        /// <summary>
        /// Gets an ICollection&lt;string&gt; containing the Blob names in the BlobStore.
        /// </summary>
        ///
        public ICollection<string> Keys => blobs.Keys;

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        ///
        /// <param name="key">
        /// The key whose value to get.
        /// </param>
        ///
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter. This parameter is passed
        /// uninitialized.
        /// </param>
        ///
        /// <returns>
        /// True if a Blob with the specified name exists in the BlobStore; otherwise, false.
        /// </returns>
        ///
        public bool TryGetValue(string key, out Blob value)
        {
            key = MapNullKey(key);
            return blobs.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes the Blob with the specified name from the BlobStore.
        /// </summary>
        ///
        /// <param name="key">
        /// The name of the Blob to remove.
        /// </param>
        ///
        /// <returns>
        /// True if the Blob is successfully removed; otherwise, false.
        /// </returns>
        ///
        public bool Remove(string key)
        {
            key = MapNullKey(key);

            if (blobs.ContainsKey(key))
            {
                RemovedBlobs[key] = blobs[key];
            }

            return blobs.Remove(key);
        }

        #endregion IDictionary implementation

        /// <summary>
        /// Gets the BLOB with the specified name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the BLOB to retrieve for the item. A null key indicates the "default" BLOB.
        /// </param>
        ///
        /// <returns>
        /// The BLOB with the specified name or <b>null</b> if no BLOB with the specified name
        /// exists.
        /// </returns>
        ///
        public Blob this[string name]
        {
            get
            {
                name = MapNullKey(name);

                Blob result = null;
                if (blobs.ContainsKey(name))
                {
                    result = blobs[name];
                }

                return result;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        private Dictionary<string, Blob> blobs = new Dictionary<string, Blob>();

        internal Dictionary<string, Blob> RemovedBlobs { get; } = new Dictionary<string, Blob>();

        /// <summary>
        /// Writes the specified bytes to the blob.
        /// </summary>
        ///
        /// <param name="blobName">
        /// The name of the BLOB. It can be <see cref="string.Empty"/> but cannot be <b>null</b>.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the BLOB.
        /// </param>
        ///
        /// <param name="bytes">
        /// The bytes to write to the blob.
        /// </param>
        ///
        /// <remarks>
        /// This is limited to about 3.5MB of data. Use <see cref="Blob.GetWriterStream"/> to write
        /// more data.
        /// </remarks>
        ///
        /// <exception name="ArgumentNullException">
        /// If <paramref name="bytes"/> or <paramref name="blobName"/>is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="contentType"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public void WriteInline(string blobName, string contentType, byte[] bytes)
        {
            Blob blob = NewBlob(blobName, contentType);
            blob.WriteInline(bytes);
        }

        /// <summary>
        /// Writes the bytes from the specified stream.
        /// </summary>
        ///
        /// <param name="blobName">
        /// The name of the BLOB. It can be <see cref="string.Empty"/> but cannot be <b>null</b>.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the BLOB.
        /// </param>
        ///
        /// <param name="stream">
        /// The stream to get the bytes from to write to the blob.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="contentType"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception name="ArgumentNullException">
        /// If <paramref name="stream"/> or <paramref name="blobName"/>
        /// is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is an error writing the data to HealthVault.
        /// </exception>
        ///
        public void Write(
            string blobName,
            string contentType,
            Stream stream)
        {
            Blob blob = NewBlob(blobName, contentType);
            blob.Write(stream);
        }

        /// <summary>
        /// Creates a BLOB in the store with the specified name, content type, and encoding.
        /// </summary>
        ///
        /// <param name="blobName">
        /// The name of the BLOB. It can be <see cref="string.Empty"/> but cannot be <b>null</b>.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the BLOB.
        /// </param>
        ///
        /// <returns>
        /// The <see cref="Blob"/> instance that was created in the store.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="blobName"/> or <paramref name="contentType"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If a Blob with the same name already exists in the store. To update an existing Blob,
        /// remove the existing Blob and create a new one.
        /// </exception>
        ///
        public Blob NewBlob(string blobName, string contentType)
        {
            Blob blob = new Blob(blobName, contentType, null, null, Record);
            blobs.Add(blobName, blob);
            _item.Sections |= ThingSections.BlobPayload;
            return blob;
        }

        /// <summary>
        /// Recreates a BLOB in the store to allow for restarting multiple BLOB uploads on a
        /// <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <param name="blobName">
        /// The name of the BLOB. It can be <see cref="string.Empty"/> but cannot be <b>null</b>.
        /// </param>
        ///
        /// <param name="contentType">
        /// The content type of the BLOB.
        /// </param>
        ///
        /// <param name="hashInfo">
        /// The hash information for the BLOB.
        /// </param>
        ///
        /// <param name="blobUrl">
        /// The HealthVault URL of the BLOB.
        /// </param>
        ///
        /// <returns>
        /// The <see cref="Blob"/> instance that was recreated in the store.
        /// </returns>
        ///
        /// <remarks>
        /// This overload is intended to allow the caller to recover from issues that may arise
        /// while uploading large BLOBs or many BLOBs for a <see cref="ThingBase"/>.
        /// If you have a large amount of data to upload, you can periodically save the state of
        /// the Blob instance (and BlobStream if necessary) and then use this method to recreate
        /// that same Blob instance in the store.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="blobName"/> or <paramref name="contentType"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// If a Blob with the same name already exists in the store. To update an existing Blob,
        /// remove the existing Blob and create a new one.
        /// </exception>
        ///
        public Blob NewBlob(
            string blobName,
            string contentType,
            BlobHashInfo hashInfo,
            Uri blobUrl)
        {
            Blob blob = new Blob(blobName, contentType, null, null, hashInfo, Record) { Url = blobUrl };

            blobs.Add(blobName, blob);
            _item.Sections |= ThingSections.BlobPayload;
            return blob;
        }

        internal void ParseXml(XPathNavigator nav)
        {
            XPathNodeIterator blobsIterator = nav.Select("blob");
            foreach (XPathNavigator blobNav in blobsIterator)
            {
                string name = blobNav.SelectSingleNode("blob-info/name").Value;
                string contentType = blobNav.SelectSingleNode("blob-info/content-type").Value;

                XPathNavigator hashNav = blobNav.SelectSingleNode("blob-info/hash-info");
                BlobHashInfo hashInfo = null;
                if (hashNav != null)
                {
                    hashInfo = new BlobHashInfo();
                    hashInfo.Parse(hashNav);
                }

                string legacyContentEncoding = null;

                XPathNavigator encodingNav = blobNav.SelectSingleNode("legacy-content-encoding");
                if (encodingNav != null)
                {
                    legacyContentEncoding = encodingNav.Value;
                }

                string currentContentEncoding = null;

                XPathNavigator currentEncodingNav =
                    blobNav.SelectSingleNode("current-content-encoding");
                if (currentEncodingNav != null)
                {
                    currentContentEncoding = currentEncodingNav.Value;
                }

                Blob blob = new Blob(
                    name,
                    contentType,
                    currentContentEncoding,
                    legacyContentEncoding,
                    hashInfo,
                    Record);

                XPathNavigator lengthNav = blobNav.SelectSingleNode("content-length");
                if (lengthNav != null)
                {
                    blob.ContentLength = lengthNav.ValueAsLong;
                }

                XPathNavigator base64Nav = blobNav.SelectSingleNode("base64data");
                if (base64Nav != null)
                {
                    blob.InlineData = Convert.FromBase64String(base64Nav.Value);
                }

                XPathNavigator urlNav = blobNav.SelectSingleNode("blob-ref-url");
                if (urlNav != null)
                {
                    blob.Url = new Uri(urlNav.Value);
                }

                blobs[blob.Name] = blob;
            }
        }

        internal void WriteXml(string nodeName, XmlWriter writer)
        {
            bool containingNodeWritten = false;

            foreach (Blob blob in blobs.Values)
            {
                if (blob.IsDirty)
                {
                    if (!containingNodeWritten)
                    {
                        writer.WriteStartElement(nodeName);
                        containingNodeWritten = true;
                    }

                    writer.WriteStartElement("blob");

                    writer.WriteStartElement("blob-info");
                    writer.WriteElementString("name", blob.Name);
                    writer.WriteElementString("content-type", blob.ContentType);

                    if (blob.HashInfo != null)
                    {
                        blob.HashInfo.Write(writer);
                    }

                    writer.WriteEndElement();

                    if (blob.ContentLength != null && blob.ContentLength.Value != 0)
                    {
                        writer.WriteElementString(
                            "content-length",
                            blob.ContentLength.Value.ToString(CultureInfo.InvariantCulture));
                    }

                    if (blob.Url != null)
                    {
                        writer.WriteElementString("blob-ref-url", blob.Url.OriginalString);
                    }
                    else if (blob.InlineData != null)
                    {
                        writer.WriteElementString("base64data", Convert.ToBase64String(blob.InlineData));
                    }

                    if (blob.ContentEncoding != null)
                    {
                        writer.WriteElementString("legacy-content-encoding", blob.ContentEncoding);
                    }

                    writer.WriteEndElement();
                }
            }

            foreach (Blob blob in RemovedBlobs.Values)
            {
                if (!blobs.ContainsKey(blob.Name))
                {
                    if (!containingNodeWritten)
                    {
                        writer.WriteStartElement(nodeName);
                        containingNodeWritten = true;
                    }

                    writer.WriteStartElement("blob");

                    writer.WriteStartElement("blob-info");
                    writer.WriteElementString("name", blob.Name);
                    writer.WriteElementString("content-type", blob.ContentType);
                    writer.WriteEndElement();

                    writer.WriteElementString("content-length", "0");

                    writer.WriteEndElement();
                }
            }

            if (containingNodeWritten)
            {
                writer.WriteEndElement();
            }
        }

        private string MapNullKey(string key)
        {
            return key ?? string.Empty;
        }
    }
}
