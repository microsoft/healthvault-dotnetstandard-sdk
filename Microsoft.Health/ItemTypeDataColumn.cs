// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Data;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health
{
    /// <summary>
    /// Describes the schema of a column of health record item data when 
    /// transformed by the single-type item transform or multi-type item 
    /// transform.
    /// </summary>
    /// 
    public class ItemTypeDataColumn : DataColumn
    {
        internal static ItemTypeDataColumn CreateFromXml(
            XPathNavigator columnNavigator)
        {
            string tag = columnNavigator.GetAttribute("tag", String.Empty);
            string label = columnNavigator.GetAttribute("label", String.Empty);
            string type = columnNavigator.GetAttribute("type", String.Empty);
            string widthString = columnNavigator.GetAttribute("width", String.Empty);
            string orderBy = columnNavigator.GetAttribute("order-by", String.Empty);

            int width = 0;
            try
            {
                width = XmlConvert.ToInt32(widthString);
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }

            string visibleString = columnNavigator.GetAttribute("visible", String.Empty);

            bool visible = true;

            try
            {
                visible = XmlConvert.ToBoolean(visibleString);
            }
            catch (FormatException)
            {
            }

            return new ItemTypeDataColumn(tag, label, type, width, visible, orderBy);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ItemTypeDataColumn"/> 
        /// class with the specified tag, label, data type, and column width.
        /// </summary>
        /// 
        /// <param name="tag">
        /// The name of the column. The name should match the name of the
        /// data, such as summary, wc-thingid.
        /// </param>
        /// 
        /// <param name="label">
        /// The caption for the column, such as Date Created.
        /// </param>
        /// 
        /// <param name="type">
        /// The type of the data, such as DateTime, string. If <b>null</b>, 
        /// System.Object is used.
        /// </param>
        /// 
        /// <param name="width">
        /// The width of the column.
        /// </param>
        /// 
        /// <param name="visible">
        /// True if the column should be shown by default or false otherwise.
        /// </param>
        /// 
        /// <param name="orderBy">
        /// The property on which the column can be ordered by.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="type"/> parameter is <b>null</b>.
        /// </exception>
        public ItemTypeDataColumn(
            string tag,
            string label,
            Type type,
            int width,
            bool visible,
            string orderBy)
            : base(tag, type)
        {
            if (type == null)
            {
                type = typeof(Object);
            }

            base.Caption = label;
            _typeName = type.ToString();
            _width = width;
            _visible = visible;
            _orderBy = orderBy;
        }

        private ItemTypeDataColumn(
            string tag,
            string label,
            string type,
            int width,
            bool visible,
            string orderBy)
            : base(tag, GetDataType(type))
        {
            base.Caption = label;

            _typeName = type;
            _width = width;
            _visible = visible;
            _orderBy = orderBy;
        }

        private static Type GetDataType(string type)
        {
            Type dataType = null;

            switch (type)
            {
                case "Boolean":
                    dataType = typeof(bool);
                    break;

                case "DateTime":
                    dataType = typeof(DateTime);
                    break;

                case "Double":
                    dataType = typeof(Double);
                    break;

                case "Int32":
                    dataType = typeof(int);
                    break;

                case "Int64":
                    dataType = typeof(Int64);
                    break;

                case "TimeSpan":
                    dataType = typeof(TimeSpan);
                    break;

                default:
                    dataType = typeof(string);
                    break;
            }
            return dataType;
        }

        internal static Object GetNotPresentValue(Type columnType)
        {
            Object result = null;
            switch (columnType.Name)
            {
                case "Boolean":
                    result = false;
                    break;

                case "String":
                    result = String.Empty;
                    break;

                default:
                    result = DBNull.Value;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Clones the column instance so that it can be added to another
        /// DataTable.
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the column with the same definition.
        /// </returns>
        /// 
        public ItemTypeDataColumn Clone()
        {
            return
                new ItemTypeDataColumn(
                    ColumnName,
                    Caption,
                    _typeName,
                    _width,
                    _visible,
                    _orderBy);
        }

        /// <summary>
        /// Gets the column type name.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the column type name.
        /// </value>
        /// 
        /// <remarks>
        /// The current types supported are:<br/><br/>
        /// Boolean<br/>
        /// String<br/>
        /// DateTime<br/>
        /// Double<br/>
        /// Int64<br/>
        /// TimeSpan<br/>
        /// </remarks>
        /// 
        public string ColumnTypeName
        {
            get { return _typeName; }
        }
        private readonly string _typeName;

        /// <summary>
        /// Gets the column display width.
        /// </summary>
        /// 
        /// <value>
        /// An integer representing the column display width.
        /// </value>
        /// 
        public int ColumnWidth
        {
            get { return _width; }
        }
        private readonly int _width;

        /// <summary>
        /// Gets whether or not the columns should be shown by default.
        /// </summary>
        /// 
        /// <value>
        /// True indicates the column should be shown by default.
        /// </value>
        /// 
        public bool VisibleByDefault
        {
            get { return _visible; }
        }
        private readonly bool _visible;

        /// <summary>
        /// The property on which the column can be ordered by.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the column property.
        /// </value>
        /// 
        public string OrderBy
        {
            get
            {
                return !String.IsNullOrEmpty(_orderBy) ? _orderBy : null;
            }
        }
        private readonly string _orderBy;

        /// <summary>
        /// Gets a string representation of the column definition.
        /// </summary>
        /// 
        /// <returns>
        /// A colon separated list of the fields that make up the column definition.
        /// </returns>
        /// 
        public override string ToString()
        {
            return
                String.Join(
                    ":",
                    new string[] 
                    { 
                        ColumnName, 
                        Caption, 
                        ColumnTypeName, 
                        ColumnWidth.ToString(), 
                        VisibleByDefault.ToString(),
                        OrderBy
                    });
        }

    }
}