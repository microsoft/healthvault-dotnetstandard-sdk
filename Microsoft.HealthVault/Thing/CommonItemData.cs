// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents data that is common for all types of things.
    /// </summary>
    ///
    /// <remarks>
    /// The common data for all thing types includes data
    /// such as notes, source, and extensions.
    /// </remarks>
    ///
    public class CommonItemData
    {
        internal void ParseXml(XPathNavigator commonNav)
        {
            XPathNavigator sourceNav = commonNav.SelectSingleNode("source");
            if (sourceNav != null)
            {
                Source = sourceNav.Value;
            }

            XPathNavigator noteNav = commonNav.SelectSingleNode("note");
            if (noteNav != null)
            {
                Note = noteNav.Value;
            }

            // Please leave this code until the data-xml/common/tags gets removed.
            XPathNavigator tagsNav = commonNav.SelectSingleNode("tags");
            if (tagsNav != null)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Tags = tagsNav.Value;
#pragma warning restore CS0618 // Type or member is obsolete
            }

            XPathNodeIterator extensionIterator = commonNav.Select("extension");

            foreach (XPathNavigator extensionNav in extensionIterator)
            {
                ThingExtension extension = DeserializeExtension(extensionNav);

                if (extension != null)
                {
                    Extensions.Add(extension);
                }
            }

            XPathNodeIterator relationshipIterator =
                commonNav.Select("related-thing[./thing-id != '' or ./client-thing-id != '']");

            foreach (XPathNavigator relationshipNav in relationshipIterator)
            {
                ThingRelationship relationship =
                    new ThingRelationship();

                relationship.ParseXml(relationshipNav);

                RelatedItems.Add(relationship);
            }

            XPathNavigator clientIdNav = commonNav.SelectSingleNode("client-thing-id");
            if (clientIdNav != null)
            {
                ClientId = clientIdNav.Value;
            }
        }

        internal void ParseRelatedAttribute(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var relThings = value.Split(';');
                foreach (var relThing in relThings)
                {
                    Guid thingId;
                    if (Guid.TryParse(relThing.Split(',')[0], out thingId))
                    {
                        RelatedItems.Add(new ThingRelationship(thingId));
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            // <common>
            writer.WriteStartElement("common");

            if (!string.IsNullOrEmpty(Source))
            {
                writer.WriteElementString("source", Source);
            }

            if (!string.IsNullOrEmpty(Note))
            {
                writer.WriteElementString("note", Note);
            }

#pragma warning disable CS0618 // Type or member is obsolete

            // Please leave this code until the data-xml/common/tags gets removed.
            if (!string.IsNullOrEmpty(Tags))
            {
                writer.WriteElementString("tags", Tags);
            }

#pragma warning restore CS0618 // Type or member is obsolete

            foreach (ThingExtension extension in Extensions)
            {
                extension.WriteExtensionXml(writer);
            }

            foreach (ThingRelationship relationship in RelatedItems)
            {
                relationship.WriteXml("related-thing", writer);
            }

            if (!string.IsNullOrEmpty(ClientId))
            {
                writer.WriteElementString("client-thing-id", ClientId);
            }

            // </common>
            writer.WriteEndElement();
        }

        internal ThingExtension DeserializeExtension(XPathNavigator extensionNav)
        {
            ThingExtension result;
            string source = extensionNav.GetAttribute("source", string.Empty);

            var thingTypeRegistrar = Ioc.Get<IThingTypeRegistrar>();

            var extensionHandlers = thingTypeRegistrar.RegisteredExtensionHandlers;

            if (extensionHandlers.ContainsKey(source))
            {
                Type handler = extensionHandlers[source];
                result = (ThingExtension)Activator.CreateInstance(handler);
            }
            else
            {
                result = new ThingExtension(source);
            }

            result.ParseXml(extensionNav);
            return result;
        }

        /// <summary>
        /// Gets or sets the source of the thing.
        /// </summary>
        ///
        ///
        /// <value>
        /// A string representing the item source.
        /// </value>
        ///
        /// <remarks>
        /// The source is the description of the device or application
        /// from which the thing came.
        /// </remarks>
        ///
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets a note on the thing.
        /// </summary>
        ///
        /// <value>
        /// A string.
        /// </value>
        ///
        /// <remarks>
        /// Notes are general annotations about the thing.
        /// </remarks>
        ///
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of tags on
        /// the thing.
        /// </summary>
        ///
        /// <value>
        /// A string representing the tag list.
        /// </value>
        ///
        /// <remarks>
        /// Tags enable users to group information freely. Applications
        /// must parse the tag list for individual tags.
        /// </remarks>
        ///
        [Obsolete("This property will be soon removed. Please use ThingBase.Tags instead.")]
        public string Tags { get; set; }

        /// <summary>
        /// Gets the collection representing the extension data of the
        /// thing.
        /// </summary>
        ///
        /// <value>
        /// A collection of <see cref="ThingExtension"/> objects.
        /// </value>
        ///
        /// <remarks>
        /// To add extensions to the thing, add an instance of the
        /// <see cref="ThingExtension"/> or derived class to this
        /// collection.
        /// </remarks>
        ///
        public Collection<ThingExtension> Extensions { get; } = new Collection<ThingExtension>();

        /// <summary>
        /// Gets the collection representing the things related to this one.
        /// </summary>
        ///
        /// <value>
        /// A collection of <see cref="ThingRelationship"/> objects.
        /// </value>
        ///
        /// <remarks>
        /// The relationships between this item and the things defined in this collection
        /// are not maintained by HealthVault. It is solely the responsibility of applications to
        /// ensure that the referenced items exist and are in the same health record.
        /// </remarks>
        ///
        public Collection<ThingRelationship> RelatedItems { get; } = new Collection<ThingRelationship>();

        /// <summary>
        /// Gets and sets a client assigned identifier to be associated with the <see cref="ThingBase" />.
        /// </summary>
        public string ClientId { get; set; }
    }
}
