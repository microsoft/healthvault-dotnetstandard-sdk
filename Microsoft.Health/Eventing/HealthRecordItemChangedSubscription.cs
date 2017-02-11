// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.Events
{
    /// <summary>
    /// A subscription that sends notifications when an item in a record changes.
    /// </summary>
    /// <remarks>
    /// This subscription enables application notification when an item of a specific data type in a record
    /// changes. 
    /// 
    /// A notification will be sent when any of the filters in the <see cref="Filters"/> collection match a 
    /// record item change on the HealthVault platform. 
    /// </remarks>
    public class HealthRecordItemChangedSubscription : Subscription
    {
        /// <summary>
        /// Create an instance of the HealthHealthRecordItemChangedSubscription class, specifying the delivery mechanism and the 
        /// set of type ids on which to send event notifications.
        /// </summary>
        /// <param name="commonSubscriptionData">The common subscription data to associate with this subscription.</param>
        /// <param name="typeIds">The type ids on which to send event notifications</param>
        public HealthRecordItemChangedSubscription(CommonSubscriptionData commonSubscriptionData, params Guid[] typeIds) :
            base(commonSubscriptionData)
        {
            HealthRecordItemChangedFilter filter = new HealthRecordItemChangedFilter(typeIds);
            if (filter.TypeIds.Count != 0)
            {
                _filters.Add(filter);
            }
        }

        /// <summary>
        /// Create an instance of the HealthRecordItemChangedSubscription class, specifying the delivery mechanism and the 
        /// set of type ids on which to send event notifications.
        /// </summary>
        /// <param name="commonSubscriptionData">The common subscription data to associate with this subscription.</param>
        /// <param name="typeIds">The type ids on which to send event notifications</param>
        public HealthRecordItemChangedSubscription(CommonSubscriptionData commonSubscriptionData, IList<Guid> typeIds) :
            base(commonSubscriptionData)
        {
            HealthRecordItemChangedFilter filter = new HealthRecordItemChangedFilter(typeIds);
            if (filter.TypeIds.Count != 0)
            {
                _filters.Add(filter);
            }
        }

        internal override void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("record-item-changed-event");
            {
                xmlWriter.WriteStartElement("filters");
                {
                    foreach (HealthRecordItemChangedFilter filter in _filters)
                    {
                        filter.WriteXml(xmlWriter);
                    }
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        internal override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator recordItemChangedNav = navigator.SelectSingleNode("record-item-changed-event");
            Validator.ThrowArgumentExceptionIf(recordItemChangedNav == null, "navigator", "MissingRecordItemChangedNode");

            XPathNavigator filtersNav = recordItemChangedNav.SelectSingleNode("filters");
            Validator.ThrowArgumentExceptionIf(filtersNav == null, "navigator", "MissingFiltersNode");

            XPathNodeIterator filters = filtersNav.Select("filter");
            _filters.Clear();

            foreach (XPathNavigator filterNav in filters)
            {
                HealthRecordItemChangedFilter filter = new HealthRecordItemChangedFilter();
                filter.ParseXml(filterNav);

                _filters.Add(filter);
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="HealthRecordItemChangedFilter"/> instances that 
        /// define when a notification is sent for this subscription.
        /// </summary>
        public Collection<HealthRecordItemChangedFilter> Filters
        {
            get { return _filters; }
        }
        Collection<HealthRecordItemChangedFilter> _filters = new Collection<HealthRecordItemChangedFilter>();

        /// <summary>
        ///  Return a string representation of the subscription.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            List<string> filterStrings = new List<string>();

            foreach (HealthRecordItemChangedFilter filter in _filters)
            {
                filterStrings.Add(filter.ToString());
            }

            string result = String.Join(
                                ResourceRetriever.GetResourceString("GroupSeparator"),
                                filterStrings.ToArray());

            return result;
        }
    }
}
