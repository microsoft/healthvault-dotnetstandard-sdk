// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Defines a filter for use with <see cref="HealthRecordSearcher"/>
    /// searches.
    /// </summary>
    ///
    /// <remarks>
    /// This class generates the XML for a single filter group for querying
    /// things with the "GetThings" method.
    /// </remarks>
    ///
    [DebuggerDisplay("ThingQuery")]
    public class ThingQuery
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ThingQuery"/>
        /// class using default values.
        /// </summary>
        /// <param name="name">
        /// The name of the filter.
        /// </param>
        ///
        /// <param name="maxItemsReturned">
        /// The maximum number of items to return that match the filter.
        /// </param>
        /// <remarks>
        /// The name is used to distinguish results matching this filter as
        /// opposed to results matching other filters when multiple filters
        /// are applied to the same search.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="name"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="maxItemsReturned"/> parameter is negative.
        /// </exception>
        public ThingQuery(string name = null, int maxItemsReturned = 0)
        {
            _configuration = Ioc.Get<HealthVaultConfiguration>();
            _typeIds = new TypeList(View, _configuration.UseLegacyTypeVersionSupport);
            Name = name;
            MaxItemsReturned = maxItemsReturned;
        }

        /// <summary>
        /// This internal constructor is used to allow custom configurations to be set for testing purposes
        /// </summary>
        /// <param name="configuration">a custom configuration</param>
        internal ThingQuery(HealthVaultConfiguration configuration)
        {
            _configuration = configuration;
            _typeIds = new TypeList(View, configuration.UseLegacyTypeVersionSupport);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ThingQuery"/>
        /// class with the specified unique item type identifiers as filters.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// The unique item type identifiers limiting the scope of the search to
        /// only the specified item types.
        /// </param>
        ///
        public ThingQuery(params Guid[] typeIds)
            : this()
        {
            foreach (Guid typeId in typeIds)
            {
                _typeIds.Add(typeId);
            }
        }

        /// <summary>
        /// Checks whether at least one property is set.
        /// </summary>
        ///
        /// <exception cref="HealthServiceException">
        /// One of the following is true:
        /// (1) The filter has no properties set;
        /// (2) Both ItemIds and ItemKeys are specified; or
        /// (3) There are more than the allowable number of order by clauses.
        /// </exception>
        ///
        internal void ThrowIfNotValid()
        {
            bool isValid = AreFiltersPresent();

            // if no filters are present,
            // at least one of ItemKeys or ItemIds should be non-empty
            isValid |=
                ItemKeys.Count != 0 ||
                ItemIds.Count != 0 ||
                ClientItemIds.Count != 0;

            if (!isValid)
            {
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = Resources.HealthRecordSearcherInvalidFilter
                };

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter,
                        error);
                throw e;
            }

            int idTypesSpecified =
                ItemKeys.Count > 0 ? 1 : 0 +
                ItemIds.Count > 0 ? 1 : 0 +
                ClientItemIds.Count > 0 ? 1 : 0;

            // only one of ItemKeys or ItemIds can be non-empty
            // throw a specific error in this particular case
            if (idTypesSpecified > 1)
            {
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = Resources.HealthRecordSearcherInvalidFilterIdsAndKeysSpecified
                };

                HealthServiceException e = HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter,
                        error);
                throw e;
            }

            if (OrderByClauses.Count > 1)
            {
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = Resources.HealthRecordSearcherInvalidOrderSpecified
                };

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter,
                        error);
                throw e;
            }
        }

        /// <summary>
        /// Gets or sets the name of the filter.
        /// </summary>
        ///
        /// <remarks>
        /// The name is used to distinguish results matching this filter as
        /// opposed to results matching other filters when multiple filters
        /// are applied to the same search.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of things to return.
        /// </summary>
        ///
        /// <remarks>
        /// The default value returns all items that match the filter.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> is set to less than zero.
        /// </exception>
        ///
        public int MaxItemsReturned
        {
            get { return _maxItemsReturned; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxItemsReturned), Resources.ThingQueryMaxReturnsNegative);
                }

                _maxItemsReturned = value;
            }
        }

        private int _maxItemsReturned = int.MinValue;

        /// <summary>
        /// Gets or sets the maximum number of full things returned per request to
        /// HealthVault.
        /// </summary>
        ///
        /// <remarks>
        /// By default HealthVault will only return a certain number of "full" things
        /// for any query. It then returns the "keys" for the remaining items that matched the
        /// query which can then be queried for by ID. <see cref="ThingCollection"/>
        /// automatically manages this paging for you. However, if you want further control over
        /// the count of full items retrieved on each request,
        /// <see cref="MaxFullItemsReturnedPerRequest"/> can be set to optimize for smaller sets
        /// of data. For example, let's say the data being retrieved is being displayed in a
        /// GridView and the results are shown 10 items per page.  Rather than get the default
        /// number of full things, you can request 10 full items per request.  Then only if the
        /// user clicks to the second page would you need to get the next 10 items.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> is set to less than zero.
        /// </exception>
        ///
        public int MaxFullItemsReturnedPerRequest
        {
            get { return _maxFullItemsReturnedPerRequest; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxFullItemsReturnedPerRequest), Resources.ThingQueryMaxFullItemsReturnedNegative);
                }

                _maxFullItemsReturnedPerRequest = value;
            }
        }

        private int _maxFullItemsReturnedPerRequest = int.MinValue;

        /// <summary>
        /// Gets or sets the view for the filter group.
        /// </summary>
        ///
        /// <remarks>
        /// The default view retrieves the "Core" and data
        /// sections ("Xml").
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public HealthRecordView View
        {
            get { return _view ?? (_view = new HealthRecordView(_configuration)); }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(View), Resources.ThingQueryViewNull);
                _view = value;
            }
        }

        private HealthRecordView _view;

        /// <summary>
        /// Gets or sets the ids identifying things for
        /// the search filter.
        /// </summary>
        ///
        /// <remarks>
        /// Each specified ID is AND'd with any other filter parameter. The
        /// filter limits the search to the specified things.
        /// It is illegal to specify both ItemIds and ItemKeys in a single
        /// filter.
        /// </remarks>
        ///
        public IList<Guid> ItemIds => _thingIds;

        private readonly List<Guid> _thingIds = new List<Guid>();

        /// <summary>
        /// Gets or sets the keys uniquely identifying things for
        /// the search filter.
        /// </summary>
        ///
        /// <remarks>
        /// Each specified ItemKey is AND'd with any other filter parameter. The
        /// filter limits the search to the specified things.
        /// It is illegal to specify more than one of ItemIds, ClientItemIds or ItemKeys in a
        /// single filter.
        /// </remarks>
        ///
        public IList<ThingKey> ItemKeys => _thingKeys;

        private readonly List<ThingKey> _thingKeys = new List<ThingKey>();

        /// <summary>
        /// Gets or sets the client assigned IDs identifying things for
        /// the search filter.
        /// </summary>
        ///
        /// <remarks>
        /// Each specified ID is AND'd with any other filter parameter. The
        /// filter limits the search to the specified things.
        /// It is illegal to specify more than one of ItemIds, ClientItemIds or ItemKeys in a
        /// single filter.
        /// </remarks>
        ///
        public IList<string> ClientItemIds => _clientItemIds;

        private readonly List<string> _clientItemIds = new List<string>();

        /// <summary>
        /// Gets a collection of the order by clauses which orders the data returned from GetThings request.
        /// </summary>
        ///
        /// <remarks>
        /// Currently only one order by clause is supported per request.
        /// Multiple clauses may be supported in the future.
        ///
        /// If an order by clause is present, then only things of the type
        /// specified in the order by clause will be returned, even if additional
        /// type IDs are listed in the filter spec.
        /// </remarks>
        ///
        public IList<ThingOrderByClause> OrderByClauses => _orderByClauses;

        private readonly List<ThingOrderByClause> _orderByClauses = new List<ThingOrderByClause>();

        /// <summary>
        /// Gets a collection of the unique item type identifiers to search
        /// for.
        /// </summary>
        ///
        /// <remarks>
        /// Each thing is associated with a type through a
        /// type identifier. If set, these values are combined in an OR
        /// operation. The group is then combined in an AND operation with
        /// any other filter value in the search for matching health record
        /// items. This AND operation excludes ItemId as a filter value,
        /// because ItemId results are obtained by an OR operation.
        /// To add a type ID, use the Add method of the returned collection.
        /// </remarks>
        ///
        public IList<Guid> TypeIds => _typeIds;

        private readonly TypeList _typeIds;

        /// <summary>
        /// Gets or sets a set of flags representing the thing
        /// states to search for.
        /// </summary>
        ///
        /// <value>
        /// The set of flags. If not specified, things with state
        /// <see cref="ThingState.Active"/>
        /// are returned.
        /// </value>
        ///
        public ThingStates States { get; set; } = ThingStates.Default;

        /// <summary>
        /// Gets or sets a value indicating whether to return only the flag
        /// specifying the current versions of the things that
        /// satisfy the filter restrictions.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> to return only current versions of things;
        /// <b>false</b> to return all versions of the things that
        /// satisfy the filter restrictions.
        /// </value>
        ///
        public bool CurrentVersionOnly
        {
            get
            {
                if (_currentVersionOnly.HasValue)
                {
                    return _currentVersionOnly.Value;
                }

                return true;
            }

            set { _currentVersionOnly = value; }
        }

        private bool? _currentVersionOnly;

        /// <summary>
        /// Gets or sets the minimum date of an updated item to return.
        /// </summary>
        ///
        /// <value>
        /// The DateTime in UTC of the minimum date of an updated item to
        /// return.
        /// </value>
        ///
        /// <remarks>
        /// If this property is not set, DateTime.MaxValue is returned.
        /// <br/><br/>
        /// The application is responsible for converting from local time to
        /// UTC, if applicable.
        /// </remarks>
        ///
        public DateTime UpdatedDateMin
        {
            get
            {
                if (_updatedDateMin == null)
                {
                    return DateTime.MaxValue;
                }

                return (DateTime)_updatedDateMin;
            }

            set { _updatedDateMin = value; }
        }

        private DateTime? _updatedDateMin;

        /// <summary>
        /// Gets or sets the maximum date of a returned updated item to return.
        /// </summary>
        ///
        /// <value>
        /// The DateTime in UTC of the maximum date of an updated item
        /// to return.
        /// </value>
        ///
        /// <remarks>
        /// If this property is not set, DateTime.MinValue is returned.
        /// <br/><br/>
        /// The application is responsible for converting from local time to
        /// UTC, if applicable.
        /// </remarks>
        ///
        public DateTime UpdatedDateMax
        {
            get
            {
                if (_updatedDateMax == null)
                {
                    return DateTime.MinValue;
                }

                return (DateTime)_updatedDateMax;
            }

            set { _updatedDateMax = value; }
        }

        private DateTime? _updatedDateMax;

        /// <summary>
        /// Gets or sets the search filter to filter on the person who
        /// last updated the thing.
        /// </summary>
        ///
        /// <remarks>
        /// If the property is not set, Guid.Empty is returned.
        /// </remarks>
        ///
        public Guid UpdatedPerson
        {
            get
            {
                if (_updatedPerson == null)
                {
                    return Guid.Empty;
                }

                return (Guid)_updatedPerson;
            }

            set { _updatedPerson = value; }
        }

        private Guid? _updatedPerson;

        /// <summary>
        /// Gets or sets the search filter to filter on the application that
        /// last updated the thing.
        /// </summary>
        ///
        /// <remarks>
        /// If this property is not set, Guid.Empty is returned.
        /// </remarks>
        ///
        public Guid UpdatedApplication
        {
            get
            {
                if (_updatedApplication == null)
                {
                    return Guid.Empty;
                }

                return (Guid)_updatedApplication;
            }

            set { _updatedApplication = value; }
        }

        private Guid? _updatedApplication;

        /// <summary>
        /// Gets or sets the minimum date the item was created.
        /// </summary>
        ///
        /// <value>
        /// The DateTime in UTC of the minimum date a item was created.
        /// </value>
        ///
        /// <remarks>
        /// If this property has not been set, DateTime.MaxValue is returned.
        /// <br/><br/>
        /// The application is responsible for converting from local time to
        /// UTC, if applicable.
        /// </remarks>
        ///
        public DateTime CreatedDateMin
        {
            get
            {
                if (_createdDateMin == null)
                {
                    return DateTime.MaxValue;
                }

                return (DateTime)_createdDateMin;
            }

            set { _createdDateMin = value; }
        }

        private DateTime? _createdDateMin;

        /// <summary>
        /// Gets or sets the maximum date the item was created.
        /// </summary>
        ///
        /// <value>
        /// The DateTime in UTC of the maximum date a item was created.
        /// </value>
        ///
        /// <remarks>
        /// If this property has not been set, DateTime.MinValue is returned.
        /// <br/><br/>
        /// The application is responsible for converting from local time to
        /// UTC  if applicable.
        /// </remarks>
        ///
        public DateTime CreatedDateMax
        {
            get
            {
                if (_createdDateMax == null)
                {
                    return DateTime.MinValue;
                }

                return (DateTime)_createdDateMax;
            }

            set { _createdDateMax = value; }
        }

        private DateTime? _createdDateMax;

        /// <summary>
        /// Gets or sets the search filter to filter on the person who
        /// created the thing.
        /// </summary>
        ///
        /// <remarks>
        /// If this property has not been set, Guid.Empty is returned.
        /// </remarks>
        ///
        public Guid CreatedPerson
        {
            get
            {
                if (_createdPerson == null)
                {
                    return Guid.Empty;
                }

                return (Guid)_createdPerson;
            }

            set { _createdPerson = value; }
        }

        private Guid? _createdPerson;

        /// <summary>
        /// Gets or sets the search filter to filter on the application that
        /// created the thing.
        /// </summary>
        ///
        /// <remarks>
        /// If this property has not been set, Guid.Empty is returned.
        /// </remarks>
        ///
        public Guid CreatedApplication
        {
            get
            {
                if (_createdApplication == null)
                {
                    return Guid.Empty;
                }

                return (Guid)_createdApplication;
            }

            set { _createdApplication = value; }
        }

        private Guid? _createdApplication;

        /// <summary>
        /// Gets or sets the minimum date the item pertains to.
        /// </summary>
        ///
        /// <value>
        /// The DateTime in UTC of the minimum effective date of a item.
        /// </value>
        ///
        /// <remarks>
        /// If this property has not been set, DateTime.MaxValue is returned.
        /// <br/><br/>
        /// The application is responsible for converting from local time to
        /// UTC, if applicable.
        /// </remarks>
        ///
        public DateTime EffectiveDateMin
        {
            get
            {
                if (_effectiveDateMin == null)
                {
                    return DateTime.MaxValue;
                }

                return (DateTime)_effectiveDateMin;
            }

            set { _effectiveDateMin = value; }
        }

        private DateTime? _effectiveDateMin;

        /// <summary>
        /// Gets or sets the maximum date the item was pertains to.
        /// </summary>
        ///
        /// <value>
        /// The DateTime in UTC of the maximum effective date of a item.
        /// </value>
        ///
        /// <remarks>
        /// If this property has not been set, DateTime.MinValue is returned.
        /// <br/><br/>
        /// The application is responsible for converting from local time to
        /// UTC, if applicable.
        /// </remarks>
        ///
        public DateTime EffectiveDateMax
        {
            get
            {
                if (_effectiveDateMax == null)
                {
                    return DateTime.MinValue;
                }

                return (DateTime)_effectiveDateMax;
            }

            set { _effectiveDateMax = value; }
        }

        private DateTime? _effectiveDateMax;

        /// <summary>
        /// Gets or sets the search filter to filter on the existence
        /// of data in an XML (structured data) item.
        /// </summary>
        ///
        /// <value>
        /// A string representing the filter.
        /// </value>
        /// <remarks>
        /// The HealthVault XPath Explorer can be used to create these XPath queries.
        ///
        /// If this property has not been set, <b>null</b> is returned.
        /// <br/><br/>
        /// This can only be an existence check. It cannot be used for
        /// calculations or to return only specific values.
        /// </remarks>
        ///
        public string XPath { get; set; }

        /// <summary>
        /// Gets or sets the minimum updated end date of the item.
        /// </summary>
        ///
        /// <value>
        /// The DateTime of the minimum updated end date of a item.
        /// </value>
        ///
        /// <remarks>
        /// If this property has not been set, DateTime.MaxValue is returned.
        /// <br/><br/>
        /// The application is responsible for converting from local time to
        /// UTC, if applicable.
        /// </remarks>
        ///
        public DateTime UpdatedEndDateMin
        {
            get
            {
                if (_updatedEndDateMin == null)
                {
                    return DateTime.MaxValue;
                }

                return (DateTime)_updatedEndDateMin;
            }

            set { _updatedEndDateMin = value; }
        }

        private DateTime? _updatedEndDateMin;

        /// <summary>
        /// Gets or sets the maximum updated end date of the item.
        /// </summary>
        ///
        /// <value>
        /// The DateTime of the maximum updated end date of a item.
        /// </value>
        ///
        /// <remarks>
        /// If this property has not been set, DateTime.MinValue is returned.
        /// <br/><br/>
        /// The application is responsible for converting from local time to
        /// UTC, if applicable.
        /// </remarks>
        ///
        public DateTime UpdatedEndDateMax
        {
            get
            {
                if (_updatedEndDateMax == null)
                {
                    return DateTime.MinValue;
                }

                return (DateTime)_updatedEndDateMax;
            }

            set { _updatedEndDateMax = value; }
        }

        private DateTime? _updatedEndDateMax;
        private HealthVaultConfiguration _configuration;

        /// <summary>
        /// The usage intentions for items that will be retrieved in
        /// this filter group.
        /// </summary>
        /// <remarks>
        /// If not set, the default value will be
        /// <see cref="ItemRetrievalIntentions.Unspecified"/>.
        /// This property is reserved for future use.
        /// </remarks>
        public ItemRetrievalIntentions Intentions { get; } = ItemRetrievalIntentions.Unspecified;

        /// <summary>
        /// Gets a string representation of the instance.
        /// </summary>
        ///
        /// <returns>
        /// The XML that is used as the group portion of the
        /// XML request for a "GetThings" method call.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                AddFilterXml(writer);
                writer.Flush();
            }

            return result.ToString();
        }

        #region internal helpers

        internal static ThingQuery CreateFromXml(XPathNavigator nav, HealthVaultConfiguration configuration)
        {
            ThingQuery filter = new ThingQuery();

            XPathNavigator groupNav = nav.SelectSingleNode("group");
            string name = groupNav.GetAttribute("name", string.Empty);

            if (!string.IsNullOrEmpty(name))
            {
                filter.Name = name;
            }

            string maxString = groupNav.GetAttribute("max", string.Empty);
            filter.MaxItemsReturned = TryGetXmlInt32Value(maxString);

            string maxFullString = groupNav.GetAttribute("max-full", string.Empty);
            filter.MaxFullItemsReturnedPerRequest = TryGetXmlInt32Value(maxFullString);

            XPathNodeIterator thingIdIterator = groupNav.Select("id");
            foreach (XPathNavigator thingIdNav in thingIdIterator)
            {
                string versionStamp
                    = thingIdNav.GetAttribute("version-stamp", string.Empty);
                filter.ItemKeys.Add(
                    new ThingKey(
                        new Guid(thingIdNav.Value),
                            new Guid(versionStamp)));
            }

            XPathNodeIterator clientIdIterator = groupNav.Select("client-thing-id");
            foreach (XPathNavigator clientIdNav in clientIdIterator)
            {
                filter.ClientItemIds.Add(clientIdNav.Value);
            }

            ParseOrderByClauses(groupNav, filter);

            XPathNavigator filterNav = groupNav.SelectSingleNode("filter");
            ParseFilterNavValue(filterNav, filter);

            XPathNavigator currentVersionNav = groupNav.SelectSingleNode("current-version-only");
            if (currentVersionNav != null)
            {
                filter.CurrentVersionOnly = currentVersionNav.ValueAsBoolean;
            }

            XPathNavigator viewNav = groupNav.SelectSingleNode("format");
            filter.View = HealthRecordView.CreateFromXml(viewNav, configuration);
            return filter;
        }

        private static void ParseFilterNavValue(XPathNavigator filterNav, ThingQuery filter)
        {
            if (filterNav != null)
            {
                XPathNodeIterator typeIdIterator = filterNav.Select("type-id");
                foreach (XPathNavigator typeIdNav in typeIdIterator)
                {
                    filter.TypeIds.Add(new Guid(typeIdNav.Value));
                }

                XPathNavigator effDateMinNav =
                    filterNav.SelectSingleNode("eff-date-min");
                if (effDateMinNav != null)
                {
                    filter.EffectiveDateMin = effDateMinNav.ValueAsDateTime;
                }

                XPathNavigator effDateMaxNav =
                    filterNav.SelectSingleNode("eff-date-max");
                if (effDateMaxNav != null)
                {
                    filter.EffectiveDateMax = effDateMaxNav.ValueAsDateTime;
                }

                XPathNavigator createdAppIdNav =
                    filterNav.SelectSingleNode("created-app-id");
                if (createdAppIdNav != null)
                {
                    filter.CreatedApplication = new Guid(createdAppIdNav.Value);
                }

                XPathNavigator createdPersonIdNav =
                    filterNav.SelectSingleNode("created-person-id");
                if (createdPersonIdNav != null)
                {
                    filter.CreatedPerson = new Guid(createdPersonIdNav.Value);
                }

                XPathNavigator updatedAppIdNav =
                    filterNav.SelectSingleNode("updated-app-id");
                if (updatedAppIdNav != null)
                {
                    filter.UpdatedApplication = new Guid(updatedAppIdNav.Value);
                }

                XPathNavigator updatedPersonIdNav =
                    filterNav.SelectSingleNode("updated-person-id");
                if (updatedPersonIdNav != null)
                {
                    filter.UpdatedPerson = new Guid(updatedPersonIdNav.Value);
                }

                XPathNavigator createdDateMinNav =
                    filterNav.SelectSingleNode("created-date-min");
                if (createdDateMinNav != null)
                {
                    filter.CreatedDateMin = createdDateMinNav.ValueAsDateTime;
                }

                XPathNavigator createdDateMaxNav =
                    filterNav.SelectSingleNode("created-date-min");
                if (createdDateMaxNav != null)
                {
                    filter.CreatedDateMax = createdDateMaxNav.ValueAsDateTime;
                }

                XPathNavigator updatedDateMinNav =
                    filterNav.SelectSingleNode("updated-date-min");
                if (updatedDateMinNav != null)
                {
                    filter.UpdatedDateMin = updatedDateMinNav.ValueAsDateTime;
                }

                XPathNavigator updatedDateMaxNav =
                    filterNav.SelectSingleNode("updated-date-min");
                if (updatedDateMaxNav != null)
                {
                    filter.UpdatedDateMax = updatedDateMaxNav.ValueAsDateTime;
                }

                XPathNavigator updatedEndDateMinNav =
                    filterNav.SelectSingleNode("updated-end-date-min");
                if (updatedEndDateMinNav != null)
                {
                    filter.UpdatedEndDateMin = updatedEndDateMinNav.ValueAsDateTime;
                }

                XPathNavigator updatedEndDateMaxNav =
                    filterNav.SelectSingleNode("updated-end-date-max");
                if (updatedEndDateMaxNav != null)
                {
                    filter.UpdatedEndDateMax = updatedEndDateMaxNav.ValueAsDateTime;
                }

                XPathNavigator xpathNav =
                    filterNav.SelectSingleNode("xpath");
                if (xpathNav != null)
                {
                    filter.XPath = xpathNav.Value;
                }
            }
        }

        private static int TryGetXmlInt32Value(string maxFullString)
        {
            if (!string.IsNullOrEmpty(maxFullString))
            {
                try
                {
                    return XmlConvert.ToInt32(maxFullString);
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }

            // Value was not present, so return default
            return 0;
        }

        private static void ParseOrderByClauses(XPathNavigator groupNav, ThingQuery filter)
        {
            XPathNavigator orderByNav = groupNav.SelectSingleNode("order-by");
            if (orderByNav != null)
            {
                XPathNodeIterator orderByIterator = orderByNav.Select("order-by-property");
                foreach (XPathNavigator orderByPropertyNav in orderByIterator)
                {
                    var orderByClause = new ThingOrderByClause
                    {
                        ThingTypeId = new Guid(orderByPropertyNav.GetAttribute("type-id", string.Empty)),
                        Name = orderByPropertyNav.GetAttribute("property-name", string.Empty)
                    };

                    string direction = orderByPropertyNav.GetAttribute("direction", string.Empty);
                    if (!string.IsNullOrEmpty(direction))
                    {
                        orderByClause.Direction = (OrderByDirection)Enum.Parse(typeof(OrderByDirection), direction);
                    }
                    else
                    {
                        orderByClause.Direction = OrderByDirection.Asc;
                    }

                    filter.OrderByClauses.Add(orderByClause);
                }
            }
        }

        internal string GetXml()
        {
            StringBuilder filterXml = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(filterXml, settings))
            {
                AddFilterXml(writer);
                writer.Flush();
            }

            return filterXml.ToString();
        }

        /// <summary>
        /// Constructs the XML for the filter group which is used in the
        /// "GetThings" request.
        /// </summary>
        ///
        /// <param name="writer">
        /// The Xml writer to write the filter group XML to.
        /// </param>
        ///
        internal void AddFilterXml(XmlWriter writer)
        {
            // Open with a group tag
            writer.WriteStartElement("group");

            if (!string.IsNullOrEmpty(Name))
            {
                // Add the name attribute to the group tag
                writer.WriteAttributeString("name", Name);
            }

            if (MaxItemsReturned > 0)
            {
                // Add the max attribute to the group tag
                writer.WriteAttributeString(
                    "max",
                    MaxItemsReturned.ToString(CultureInfo.InvariantCulture));
            }

            if (MaxFullItemsReturnedPerRequest >= 0)
            {
                // Add the max-full attribute to the group tag
                writer.WriteAttributeString(
                    "max-full",
                    MaxFullItemsReturnedPerRequest.ToString(CultureInfo.InvariantCulture));
            }

            foreach (Guid id in ItemIds)
            {
                // Add the <id> tag to the filter group
                writer.WriteElementString("id", id.ToString());
            }

            foreach (ThingKey key in ItemKeys)
            {
                // Add the <key> tag to the filter group
                writer.WriteStartElement("key");
                writer.WriteAttributeString(
                    "version-stamp", key.VersionStamp.ToString());
                writer.WriteValue(key.Id.ToString());
                writer.WriteEndElement();
            }

            foreach (string thingId in ClientItemIds)
            {
                // Add the <id> tag to the filter group
                writer.WriteElementString("client-thing-id", thingId);
            }

            AddFilterSection(writer);

            View.AddViewXml(writer);

            if (_currentVersionOnly.HasValue)
            {
                writer.WriteElementString(
                    "current-version-only",
                    SDKHelper.XmlFromBool(_currentVersionOnly.Value));
            }

            if (Intentions != ItemRetrievalIntentions.Unspecified)
            {
                writer.WriteStartElement("intents");

                // view
                if (Intentions.HasFlag(ItemRetrievalIntentions.View))
                {
                    writer.WriteElementString("intent", "view");
                }

                // download
                if (Intentions.HasFlag(ItemRetrievalIntentions.Download))
                {
                    writer.WriteElementString("intent", "download");
                }

                // transmit
                if (Intentions.HasFlag(ItemRetrievalIntentions.Transmit))
                {
                    writer.WriteElementString("intent", "transmit");
                }

                writer.WriteEndElement();
            }

            if (OrderByClauses.Count > 0)
            {
                writer.WriteStartElement("order-by");
                foreach (var orderByClause in OrderByClauses)
                {
                    writer.WriteStartElement("order-by-property");
                    writer.WriteAttributeString("type-id", orderByClause.ThingTypeId.ToString());
                    writer.WriteAttributeString("property-name", orderByClause.Name);
                    writer.WriteAttributeString("direction", orderByClause.Direction.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            // Close the group tag
            writer.WriteEndElement();
        }

        private void AddFilterSection(XmlWriter writer)
        {
            if (AreFiltersPresent())
            {
                // <filter>
                writer.WriteStartElement("filter");

                if (_typeIds.Count > 0)
                {
                    // <type-id>
                    foreach (Guid typeId in _typeIds)
                    {
                        writer.WriteElementString(
                            "type-id", typeId.ToString());
                    }
                }

                if ((States & ThingStates.Active) != 0)
                {
                    // <thing-state>
                    writer.WriteElementString(
                        "thing-state",
                        ThingState.Active.ToString());
                }

                if ((States & ThingStates.Deleted) != 0)
                {
                    // <thing-state>
                    writer.WriteElementString(
                        "thing-state",
                        ThingState.Deleted.ToString());
                }

                if (_effectiveDateMin != null)
                {
                    // <eff-date-min>
                    writer.WriteStartElement("eff-date-min");
                    writer.WriteValue(SDKHelper.XmlFromDateTime(EffectiveDateMin));
                    writer.WriteEndElement();
                }

                if (_effectiveDateMax != null)
                {
                    // <eff-date-max>
                    writer.WriteStartElement("eff-date-max");
                    writer.WriteValue(SDKHelper.XmlFromDateTime(EffectiveDateMax));
                    writer.WriteEndElement();
                }

                if (_createdApplication != null)
                {
                    // <created-application>
                    writer.WriteStartElement("created-app-id");
                    writer.WriteValue(CreatedApplication.ToString());
                    writer.WriteEndElement();
                }

                if (_createdPerson != null)
                {
                    // <created-person>
                    writer.WriteStartElement("created-person-id");
                    writer.WriteValue(CreatedPerson.ToString());
                    writer.WriteEndElement();
                }

                if (_updatedApplication != null)
                {
                    // <updated-application>
                    writer.WriteStartElement("updated-app-id");
                    writer.WriteValue(UpdatedApplication.ToString());
                    writer.WriteEndElement();
                }

                if (_updatedPerson != null)
                {
                    // <updated-person>
                    writer.WriteStartElement("updated-person-id");
                    writer.WriteValue(UpdatedPerson.ToString());
                    writer.WriteEndElement();
                }

                if (_createdDateMin != null)
                {
                    // <created-date-min>
                    writer.WriteStartElement("created-date-min");
                    writer.WriteValue(CreatedDateMin);
                    writer.WriteEndElement();
                }

                if (_createdDateMax != null)
                {
                    // <created-date-max>
                    writer.WriteStartElement("created-date-max");
                    writer.WriteValue(CreatedDateMax);
                    writer.WriteEndElement();
                }

                if (_updatedDateMin != null)
                {
                    // <updated-date-min>
                    writer.WriteStartElement("updated-date-min");
                    writer.WriteValue(UpdatedDateMin);
                    writer.WriteEndElement();
                }

                if (_updatedDateMax != null)
                {
                    // <updated-date-max>
                    writer.WriteStartElement("updated-date-max");
                    writer.WriteValue(UpdatedDateMax);
                    writer.WriteEndElement();
                }

                if (!string.IsNullOrEmpty(XPath))
                {
                    // <xpath>
                    writer.WriteStartElement("xpath");
                    writer.WriteValue(XPath);
                    writer.WriteEndElement();
                }

                if (_updatedEndDateMax != null)
                {
                    // <updated-end-date-max>
                    writer.WriteStartElement("updated-end-date-max");
                    writer.WriteValue(UpdatedEndDateMax);
                    writer.WriteEndElement();
                }

                if (_updatedEndDateMin != null)
                {
                    // <updated-end-date-min>
                    writer.WriteStartElement("updated-end-date-min");
                    writer.WriteValue(UpdatedEndDateMin);
                    writer.WriteEndElement();
                }

                // </filter>
                writer.WriteEndElement();
            }
        }

        private bool AreFiltersPresent()
        {
            return (_typeIds.Count > 0)
                   || (States & ThingStates.Default) != States
                   || (_effectiveDateMin != null)
                   || (_effectiveDateMax != null)
                   || (_updatedDateMin != null)
                   || (_updatedDateMax != null)
                   || (_updatedPerson != null)
                   || (_updatedApplication != null)
                   || (_createdDateMin != null)
                   || (_createdDateMax != null)
                   || (_createdPerson != null)
                   || (_createdApplication != null)
                   || (_updatedEndDateMin != null)
                   || (_updatedEndDateMax != null)
                   || !string.IsNullOrEmpty(XPath);
        }

        #endregion internal helpers

        private class TypeList : IList<Guid>
        {
            private readonly bool _useLegacyTypeVersionSupport;
            private readonly HealthRecordView _view;
            private readonly Collection<Guid> _list = new Collection<Guid>();

            public TypeList(HealthRecordView view, bool? configuration = null)
            {
                _useLegacyTypeVersionSupport = configuration ?? Ioc.Get<HealthVaultConfiguration>().UseLegacyTypeVersionSupport;
                _view = view;
            }

            public void Add(Guid item)
            {
                _list.Add(item);

                if (!_useLegacyTypeVersionSupport)
                {
                    _view.TypeVersionFormat.Add(item);
                }
            }

            public void Clear()
            {
                _list.Clear();
                if (!_useLegacyTypeVersionSupport)
                {
                    _view.TypeVersionFormat.Clear();
                }
            }

            public bool Contains(Guid item)
            {
                return _list.Contains(item);
            }

            public void CopyTo(Guid[] array, int arrayIndex)
            {
                _list.CopyTo(array, arrayIndex);
            }

            public IEnumerator<Guid> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_list).GetEnumerator();
            }

            public int IndexOf(Guid item)
            {
                return _list.IndexOf(item);
            }

            public void Insert(int index, Guid item)
            {
                _list.Insert(index, item);
                if (!_useLegacyTypeVersionSupport)
                {
                    _view.TypeVersionFormat.Add(item);
                }
            }

            public bool Remove(Guid item)
            {
                bool result = _list.Remove(item);
                if (result && !_useLegacyTypeVersionSupport)
                {
                    _view.TypeVersionFormat.Remove(item);
                }

                return result;
            }

            public void RemoveAt(int index)
            {
                Guid item = _list[index];
                _list.RemoveAt(index);
                if (!_useLegacyTypeVersionSupport)
                {
                    _view.TypeVersionFormat.Remove(item);
                }
            }

            public int Count => _list.Count;

            public bool IsReadOnly => false;

            public Guid this[int index]
            {
                get { return _list[index]; }
                set { _list[index] = value; }
            }
        }
    }
}
