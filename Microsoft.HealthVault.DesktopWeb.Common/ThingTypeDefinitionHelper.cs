using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.DesktopWeb.Common
{
    /// <summary>
    //  Supports XSL transform for HealthRecordItemTypeDefintion
    /// </summary>
    public class ThingTypeDefinitionHelper
    {
        /// <summary>
        /// Gets or sets the HealthVault transform names supported by the type.
        /// </summary>
        /// 
        /// <value>
        /// A read-only collection containing the transforms.
        /// </value>
        /// 
        public ReadOnlyCollection<string> SupportedTransformNames
        {
            get { return _supportedTransformNames; }
            protected set { _supportedTransformNames = value; }
        }
        private ReadOnlyCollection<string> _supportedTransformNames =
            new ReadOnlyCollection<string>(new string[0]);

        /// <summary>
        /// Gets or sets the HealthVault transforms supported by the type.
        /// </summary>
        /// 
        /// <value>
        /// A dictionary containing each of the transforms supported by the type. The key is the
        /// transform name and the value is the source of the transform.
        /// </value>
        /// 
        /// <remarks>
        /// The transform can be run by calling one of the <see cref="TransformItem"/> overloads.
        /// </remarks>
        /// 
        public Dictionary<string, string> TransformSource
        {
            get { return _transformSource; }
            protected set { _transformSource = value; }
        }
        private Dictionary<string, string> _transformSource =
            new Dictionary<string, string>();

        private Dictionary<string, XslCompiledTransform> _compiledTransform =
            new Dictionary<string, XslCompiledTransform>();

        /// <summary>
        /// Gets or sets the column definitions when dealing with the type as a
        /// single type table.
        /// </summary>
        ///
        /// <value>
        /// A read-only collection containing the defintions.
        /// </value>
        ///
        public ReadOnlyCollection<ItemTypeDataColumn> ColumnDefinitions
        {
            get { return _columns; }
            protected set { _columns = value; }
        }
        private ReadOnlyCollection<ItemTypeDataColumn> _columns =
            new ReadOnlyCollection<ItemTypeDataColumn>(new ItemTypeDataColumn[0]);

        public ThingTypeDefinition ItemTypeDefinition { get; protected set; }

        public static ThingTypeDefinitionHelper Create(ThingTypeDefinition itemTypeDefinition)
        {
            ThingTypeDefinitionHelper helper = new ThingTypeDefinitionHelper(itemTypeDefinition);

            if (itemTypeDefinition.TypeNavigator == null)
            {
                throw new ArgumentException("TypeNavigator should be set on itemTypeDefinition");
            }

            itemTypeDefinition.TypeNavigator.MoveToRoot();
            helper.ParseXml(itemTypeDefinition.TypeNavigator);

            return helper;
        }

        protected ThingTypeDefinitionHelper(ThingTypeDefinition itemTypeDefinition)
        {
            if (itemTypeDefinition == null)
            {
                throw new ArgumentNullException(nameof(itemTypeDefinition));
            }

            ItemTypeDefinition = itemTypeDefinition;
        }

        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="typeNavigator">The type navigator.</param>
        public void ParseXml(XPathNavigator typeNavigator)
        {
            _columns = GetThingTypeColumns(typeNavigator);

            var transforms = new List<string>();
            XPathNodeIterator transformsIterator = typeNavigator.Select("transforms/tag");

            foreach (XPathNavigator transformsNav in transformsIterator)
            {
                transforms.Add(transformsNav.Value);
            }

            _supportedTransformNames = new ReadOnlyCollection<string>(transforms);

            XPathNodeIterator transformSourceIterator = typeNavigator.Select("transform-source");

            foreach (XPathNavigator transformSourceNav in transformSourceIterator)
            {
                string transformTag = transformSourceNav.GetAttribute("tag", String.Empty);
                _transformSource.Add(transformTag, transformSourceNav.Value);
            }
        }

        private static ReadOnlyCollection<ItemTypeDataColumn> GetThingTypeColumns(XPathNavigator typeNavigator)
        {
            XPathNodeIterator columnIterator = typeNavigator.Select("columns/column");

            var columns = (from XPathNavigator columnNav in columnIterator select ItemTypeDataColumn.CreateFromXml(columnNav)).ToList();

            return new ReadOnlyCollection<ItemTypeDataColumn>(columns);
        }

        #region TransformItem

        /// <summary>
        /// Transforms the XML of the specified thing using the specified transform.
        /// </summary>
        /// 
        /// <param name="transformName">
        /// The name of the transform to use. Supported transforms for the type can be found in the
        /// <see cref="SupportedTransformNames"/> collection.
        /// </param>
        /// 
        /// <param name="item">
        /// The thing to be transformed.
        /// </param>
        /// 
        /// <returns>
        /// A string containing the results of the transform.
        /// </returns>
        /// 
        /// <remarks>
        /// If the transform has been used before a cached instance of the compiled transform will
        /// be used. Compiled transforms are not thread safe. It is up to the caller to ensure
        /// that multiple threads do not attempt to use the same transform at the same time.
        /// </remarks>
        /// 
        /// <exception cref="KeyNotFoundException">
        /// If <paramref name="transformName"/> could not be found in the <see cref="TransformSource"/>
        /// collection.
        /// </exception>
        /// 
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "StringReader can be disposed multiple times. Usings block makes the code more readable")]
        public string TransformItem(string transformName, ThingBase item)
        {
            XslCompiledTransform transform = GetTransform(transformName);

            string thingXml = item.GetItemXml();

            StringBuilder result = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            XsltArgumentList args = new XsltArgumentList();

            args.AddParam("culture", String.Empty, System.Globalization.CultureInfo.CurrentUICulture.Name);
            args.AddParam("typename", String.Empty, this.ItemTypeDefinition.Name);
            args.AddParam("typeid", String.Empty, this.ItemTypeDefinition.ToString());

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                using (StringReader stringReader = new StringReader(thingXml))
                {
                    using (XmlReader reader = XmlReader.Create(stringReader, SDKHelper.XmlReaderSettings))
                    {
                        XPathDocument thingXmlDoc = new XPathDocument(reader);
                        transform.Transform(thingXmlDoc.CreateNavigator(), args, writer, null);
                    }
                }

                writer.Flush();
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets a compiled version of the specified transform.
        /// </summary>
        /// 
        /// <param name="transformName">
        /// The name of the transform to get.
        /// </param>
        /// 
        /// <returns>
        /// A compiled version of the specified transform.
        /// </returns>
        /// 
        /// <exception cref="KeyNotFoundException">
        /// If <paramref name="transformName"/> is not found as a transform for the item type.
        /// </exception>
        /// 
        /// <exception cref="XmlException">
        /// There is a load or parse error in the specified transform.
        /// </exception>
        /// 
        /// <exception cref="XsltException">
        /// The specified style sheet contains an error.
        /// </exception>
        /// 
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "StringReader can be disposed multiple times. Usings block makes the code more readable")]
        public XslCompiledTransform GetTransform(string transformName)
        {
            if (!_transformSource.ContainsKey(transformName))
            {
                throw new KeyNotFoundException(
                    ResourceRetriever.FormatResourceString("TransformNameNotFound", transformName));
            }

            XslCompiledTransform transform = null;

            lock (_compiledTransform)
            {
                if (_compiledTransform.ContainsKey(transformName))
                {
                    transform = _compiledTransform[transformName];
                }
                else
                {
                    using (StringReader stringReader = new StringReader(_transformSource[transformName]))
                    {
                        using (XmlReader reader = XmlReader.Create(stringReader, SDKHelper.XmlReaderSettings))
                        {
                            transform = new XslCompiledTransform();
                            XsltSettings settings = new XsltSettings(false, true);
                            XPathDocument transformDoc = new XPathDocument(reader);
                            transform.Load(transformDoc.CreateNavigator(), settings, null);
                            _compiledTransform.Add(transformName, transform);
                        }
                    }
                }
            }

            return transform;
        }
        #endregion TransformItem
    }
}
