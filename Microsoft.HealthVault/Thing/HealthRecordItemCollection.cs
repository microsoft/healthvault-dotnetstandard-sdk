// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using System.Linq;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Defines a group of results that gets returned from the
    /// <see cref="HealthRecordSearcher" />.
    /// </summary>
    ///
    /// <remarks>
    /// This collection is read-only.
    /// </remarks>
    ///
    [DebuggerTypeProxy(typeof(HealthRecordItemCollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    public class HealthRecordItemCollection :
        IList<IThing>,
        ICollection<IThing>, 
        IEnumerable<IThing>,
        IList, 
        ICollection, 
        IEnumerable
    {
        #region Factory methods & ctor

        /// <summary>
        /// Creates a result group from the response XML.
        /// </summary>
        ///
        /// <param name="record">
        /// The health record to which all result items are associated.
        /// </param>
        ///
        /// <param name="groupReader">
        /// An XML reader targeted at the group element of the
        /// "GetThings" response.
        /// </param>
        ///
        /// <param name="filters">
        /// The possible filters that were used to get the group.
        /// </param>
        ///
        /// <returns>
        /// An instance of a result group containing the health record items
        /// in the response.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="record"/> parameter is <b>null</b>.
        /// </exception>
        ///
        internal static HealthRecordItemCollection CreateResultGroupFromResponse(
            HealthRecordAccessor record,
            XmlReader groupReader,
            IList<ThingQuery> filters)
        {
            Validator.ThrowIfArgumentNull(record, "record", "ResponseRecordNull");

            // Name is optional
            ThingQuery matchingQuery = null;
            string name = string.Empty;
            if (groupReader.MoveToAttribute("name"))
            {
                name = groupReader.Value;
                groupReader.MoveToElement();
            }

            foreach (ThingQuery filter in filters)
            {
                if (string.IsNullOrEmpty(filter.Name) &&
                    string.IsNullOrEmpty(name))
                {
                    matchingQuery = filter;
                    break;
                }

                if (string.Equals(
                        filter.Name,
                        name,
                        StringComparison.Ordinal))
                {
                    matchingQuery = filter;
                    break;
                }
            }

            return GetResultGroupFromResponse(name, record, matchingQuery, groupReader);
        }

        private static HealthRecordItemCollection GetResultGroupFromResponse(string name, HealthRecordAccessor record, ThingQuery matchingQuery, XmlReader groupReader)
        {
            HealthRecordItemCollection result =
                new HealthRecordItemCollection(name, record, matchingQuery);

            int maxResultsPerRequest = 0;

            bool boolEndLoop = false;
            groupReader.Read();
            if (groupReader.NodeType != XmlNodeType.None)
            {
                while (!boolEndLoop)
                {
                    switch (groupReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (groupReader.Name == "thing")
                            {
                                XmlReader thingReader = groupReader.ReadSubtree();
                                thingReader.MoveToContent();

                                HealthRecordItem resultHealthRecordItem =
                                    ItemTypeManager.DeserializeItem(thingReader);
                                thingReader.Dispose();

                                // groupReader will normally be at the end element of
                                // the group at this point, and needs a read to get to
                                // the next element. If the group was empty, groupReader
                                // will be at the beginning of the group, and a
                                // single read will still move to the next element.
                                groupReader.Read();

                                if (resultHealthRecordItem != null)
                                {
                                    result.AddResult(resultHealthRecordItem);
                                    maxResultsPerRequest++;
                                }
                            }
                            else if (groupReader.Name == "unprocessed-thing-key-info")
                            {
                                using (XmlReader unprocessedThingReader = groupReader.ReadSubtree())
                                {
                                    unprocessedThingReader.ReadToDescendant("thing-id");

                                    string versionStamp = string.Empty;
                                    if (unprocessedThingReader.MoveToAttribute("version-stamp"))
                                    {
                                        versionStamp = unprocessedThingReader.Value;
                                        unprocessedThingReader.MoveToElement();
                                    }

                                    Guid thingId =
                                        new Guid(unprocessedThingReader.ReadElementContentAsString());

                                    HealthRecordItemKey key =
                                        new HealthRecordItemKey(thingId, new Guid(versionStamp));

                                    result.AddResult(key);

                                    groupReader.Read();
                                }
                            }
                            else if (groupReader.Name == "filtered")
                            {
                                result.WasFiltered = groupReader.ReadElementContentAsBoolean();
                            }
                            else if (groupReader.Name == "order-by-culture")
                            {
                                result.OrderByCulture = groupReader.ReadElementContentAsString();
                            }
                            else
                            {
                                // Unrecognized element. There are no other elements allowed by
                                // the xsd file, so should never get here. Just skip the element.
                                using (XmlReader unknownElementReader = groupReader.ReadSubtree())
                                {
                                    unknownElementReader.Read();
                                }

                                groupReader.Read();
                            }

                            break;
                        case XmlNodeType.EndElement:
                            // Must be the end of the group element
                            boolEndLoop = true;
                            break;
                        default:
                            // Skip over white space.
                            groupReader.Read();
                            break;
                    } // Switch
                } // While
            } // If

            if (maxResultsPerRequest > 0)
            {
                result.MaxResultsPerRequest = maxResultsPerRequest;
            }

            return result;
        }

        /// <summary>
        /// Create an instance of the <see cref="HealthRecordItemCollection"/> class with a specific set of items.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is intended for testing purposes.
        /// </remarks>
        ///
        /// <param name="items">The items to put into the collection.</param>
        public HealthRecordItemCollection(IEnumerable<HealthRecordItem> items)
        {
            foreach (HealthRecordItem item in items)
            {
                this.abstractResults.Add(item);
            }
        }

        private HealthRecordItemCollection(
            string name,
            HealthRecordAccessor record,
            ThingQuery query)
        {
            this.Name = name;
            this.Record = record;
            this.Query = query;
        }

        #endregion Factory methods & ctor

        #region Public properties

        /// <summary>
        /// Gets the name of the filter group.
        /// </summary>
        ///
        /// <value>
        /// A string containing the name of the filter to which the results
        /// in this group apply.
        /// </value>
        ///
        /// <remarks>
        /// This name is used to distinguish the responses for the filter
        /// group that was specified in the search.
        /// </remarks>
        ///
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the result set of health record
        /// items was filtered.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the result set of health record items was filtered due
        /// to the callers permissions; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool WasFiltered { get; private set; }

        /// <summary>
        /// Gets the value indicating the culture that order by values were sorted in.
        /// </summary>
        ///
        /// <remarks>
        /// The culture may not be the same culture as requested in request header.  It is the closest match HealthVault supports.
        /// </remarks>
        ///
        public string OrderByCulture { get; private set; }

        /// <summary>
        /// Gets the health record that the items were retrieved from.
        /// </summary>
        ///
        internal HealthRecordAccessor Record { get; }

        internal ThingQuery Query { get; }

        // This collection contains a combination of the full IThing
        // results as well as any partial thing IDs that were returned
        private Collection<object> abstractResults = new Collection<object>();

        /// <summary>
        /// Gets or sets the maximum number of full items returned in a
        /// GetThings request.
        /// </summary>
        ///
        internal int MaxResultsPerRequest { get; set; } = int.MinValue;

        #endregion public properties

        #region ICollection<IThing>

        /// <summary>
        /// Gets the number of items in the result group.
        /// </summary>
        ///
        /// <remarks>
        /// This number can include partial results returned from the server
        /// if the maximum number of items returned is reached. If accessed,
        /// the partial items are retrieved automatically from the server.
        /// </remarks>
        ///
        public int Count => this.abstractResults.Count;

        /// <summary>
        /// Gets a value indicating that this collection is read-only.
        /// </summary>
        ///
        /// <value>
        /// This property always returns <b>true</b>.
        /// </value>
        ///
        public bool IsReadOnly => true;

        /// <summary>
        /// Gets a value indicating that this collection is thread-safe
        /// when the <see cref="SyncRoot"/> is used.
        /// </summary>
        ///
        /// <value>
        /// This property always returns <b>true</b>.
        /// </value>
        ///
        /// <remarks>
        /// <see cref="SyncRoot"/> returns an object that can be used to
        /// synchronize access to the ICollection.
        /// </remarks>
        ///
        public bool IsSynchronized => true;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// ICollection.
        /// </summary>
        ///
        /// <value>
        /// An object that can be used to synchronize access to the
        /// ICollection.
        /// </value>
        ///
        /// <remarks>
        /// For collections whose underlying store is not publicly available,
        /// the expected implementation is to return the current instance.
        /// Note that the pointer to the current instance might be
        /// insufficient for collections that wrap other collections; those
        /// should return the underlying collection's SyncRoot property.
        /// </remarks>
        ///
        public object SyncRoot => this.abstractResults;

        /// <summary>
        /// This method is not used, because the collection is read-only and
        /// you cannot add items to it.
        /// </summary>
        ///
        /// <param name="item">
        /// This parameter is ignored, because items cannot be added.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// This exception is always thrown.
        /// </exception>
        ///
        public void Add(IThing item)
        {
            throw Validator.NotSupportedException("ResultGroupIsReadOnly");
        }

        /// <summary>
        /// This method is not used, because the collection is read-only
        /// you cannot add items to it.
        /// </summary>
        ///
        /// <param name="value">
        /// This parameter is ignored, because items cannot be added.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// This exception is always thrown.
        /// </exception>
        ///
        int IList.Add(object value)
        {
            throw Validator.NotSupportedException("ResultGroupIsReadOnly");
        }

        /// <summary>
        /// This method is not used, because the result group is read-only,
        /// and you cannot clear items from it.
        /// </summary>
        ///
        /// <exception cref="NotSupportedException">
        /// This exception is always thrown.
        /// </exception>
        ///
        public void Clear()
        {
            throw Validator.NotSupportedException("ResultGroupIsReadOnly");
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the
        /// specified <see cref="IThing"/>.
        /// </summary>
        ///
        /// <param name="item">
        /// The <see cref="IThing"/> to locate in the collection.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if a matching <see cref="IThing"/> is found;
        /// otherwise, <b>false</b>.
        /// </returns>
        ///
        public bool Contains(IThing item)
        {
            if (item == null)
            {
                return false;
            }

            return this.Contains(item.Key);
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the object
        /// having the specified value.
        /// </summary>
        ///
        /// <param name="value">
        /// The health record item to locate in the collection.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if a matching object is found; otherwise, <b>false</b>.
        /// </returns>
        ///
        bool IList.Contains(object value)
        {
            bool result;

            IThing thing = value as IThing;

            result = thing != null ? this.Contains(thing) : this.Contains(value as HealthRecordItemKey);

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains a
        /// <see cref="IThing"/> with the specified
        /// <see cref="HealthRecordItemKey"/>.
        /// </summary>
        ///
        /// <param name="itemKey">
        /// The unique <see cref="HealthRecordItemKey"/> used to locate the
        /// <see cref="HealthRecordItem"/>item in the collection. The key
        /// contains a unique identifier for the <see cref="HealthRecordItem"/>
        /// and a unique version stamp identifying the version of
        /// the <see cref="IThing"/>.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if a matching object is found; otherwise, <b>false</b>.
        /// </returns>
        ///
        public bool Contains(HealthRecordItemKey itemKey)
        {
            lock (this.abstractResults)
            {
                bool result = false;
                for (int index = 0; index < this.abstractResults.Count; ++index)
                {
                    HealthRecordItemKey abstractHealthRecordItemKey;
                    IThing thing
                        = this.abstractResults[index] as IThing;
                    if (thing != null)
                    {
                        abstractHealthRecordItemKey = thing.Key;
                    }
                    else
                    {
                        abstractHealthRecordItemKey
                            = (HealthRecordItemKey)this.abstractResults[index];
                    }

                    if (abstractHealthRecordItemKey.Equals(itemKey))
                    {
                        result = true;
                        break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// This method is not used, because copying the result group might
        /// cause unexpected network access.
        /// </summary>
        ///
        /// <param name="array">
        /// This parameter is ignored, since the result group cannot be copied.
        /// </param>
        ///
        /// <param name="arrayIndex">
        /// This parameter is also ignored, since the result group cannot be copied.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// This exception is always thrown.
        /// </exception>
        ///
        public void CopyTo(IThing[] array, int arrayIndex)
        {
            throw Validator.NotSupportedException("ResultGroupNotCopyable");
        }

        /// <summary>
        /// This method is not used, because copying the result group might
        /// cause unexpected network access.
        /// </summary>
        ///
        /// <param name="array">
        /// This parameter is ignored, since the result group cannot be copied.
        /// </param>
        ///
        /// <param name="index">
        /// This parameter is also ignored, since the result group cannot be copied.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// This exception is always thrown.
        /// </exception>
        ///
        void ICollection.CopyTo(Array array, int index)
        {
            throw Validator.NotSupportedException("ResultGroupNotCopyable");
        }

        /// <summary>
        /// This method is not used, because items cannot be removed from a
        /// read-only result group.
        /// </summary>
        ///
        /// <param name="item">
        /// This parameter is ignored, since items cannot be removed.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// This exception is always thrown.
        /// </exception>
        ///
        public bool Remove(IThing item)
        {
            throw Validator.NotSupportedException("ResultGroupIsReadOnly");
        }

        #endregion ICollection<IThing>

        #region IList<IThing>

        /// <summary>
        /// Retrieves the <see cref="IThing"/> at the specified index.
        /// </summary>
        ///
        /// <param name="index">
        /// The zero-based index of the item.
        /// </param>
        ///
        /// <returns>
        /// The item at the specified index.
        /// </returns>
        ///
        /// <exception cref="NotSupportedException">
        /// Set is called on a read-only collection.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index"/> parameter is less than zero or
        /// greater than the value of <see cref="Count"/>.
        /// </exception>
        ///
        public IThing this[int index]
        {
            get
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    index < 0 || index >= this.abstractResults.Count,
                    "index",
                    "ResultGroupIndexOutOfRange");

                lock (this.abstractResults)
                {
                    IThing result = this.abstractResults[index] as IThing;
                    if (result == null)
                    {
                        this.GetPartialThingsAsync(index).Wait();
                        result = this.abstractResults[index] as IThing;
                    }

                    return result;
                }
            }

            set
            {
                throw Validator.NotSupportedException("ResultGroupIsReadOnly");
            }
        }

        /// <summary>
        /// Retrieves the object at the specified index.
        /// </summary>
        ///
        /// <param name="index">
        /// The zero-based index at which to get the object.
        /// </param>
        ///
        /// <returns>
        /// The object at the specified index.
        /// </returns>
        ///
        /// <exception cref="NotSupportedException">
        /// Set is called on a read-only collection.
        /// </exception>
        ///
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw Validator.NotSupportedException("ResultGroupIsReadOnly");
            }
        }

        /// <summary>
        /// Determines the index of the specific <see cref="IThing"/>
        /// in the list.
        /// </summary>
        ///
        /// <param name="item">
        /// The <see cref="IThing"/> to locate in the list.
        /// </param>
        ///
        /// <returns>
        /// The index of the <see cref="IThing"/>, if found;
        /// otherwise, -1.
        /// </returns>
        ///
        public int IndexOf(IThing item)
        {
            if (item == null)
            {
                return -1;
            }

            return this.IndexOf(item.Key);
        }

        /// <summary>
        /// Determines the index of the specific item in the list.
        /// </summary>
        ///
        /// <param name="value">
        /// The object to locate in the list.
        /// </param>
        ///
        /// <returns>
        /// The index of the item, if found in the list; otherwise, -1.
        /// </returns>
        ///
        int IList.IndexOf(object value)
        {
            int result = -1;

            IThing thing = value as IThing;
            if (thing != null)
            {
                result = this.IndexOf(thing);
            }
            else
            {
                result = this.IndexOf(value as HealthRecordItemKey);
            }

            return result;
        }

        /// <summary>
        /// Determines the index of the specific item in the list using the
        /// unique health record item identifier.
        /// </summary>
        ///
        /// <param name="key">
        /// The unique health record item key used to locate the
        /// item in the list.
        /// </param>
        ///
        /// <returns>
        /// The index of item, if found in the list; otherwise, -1.
        /// </returns>
        ///
        public int IndexOf(HealthRecordItemKey key)
        {
            lock (this.abstractResults)
            {
                int result = -1;
                for (int index = 0; index < this.abstractResults.Count; ++index)
                {
                    HealthRecordItemKey abstractHealthRecordItemKey;
                    IThing thing = this.abstractResults[index] as IThing;
                    if (thing != null)
                    {
                        abstractHealthRecordItemKey = thing.Key;
                    }
                    else
                    {
                        abstractHealthRecordItemKey = (HealthRecordItemKey)this.abstractResults[index];
                    }

                    if (abstractHealthRecordItemKey.Equals(key))
                    {
                        result = index;
                        break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// This method is not used, because items cannot be inserted into a
        /// read-only collection.
        /// </summary>
        ///
        /// <param name="index">
        /// This parameter is ignored because the collection is read-only.
        /// </param>
        ///
        /// <param name="item">
        /// This parameter is also ignored because the collection is read-only.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// The collection is read-only and does not support insertion.
        /// </exception>
        ///
        public void Insert(int index, IThing item)
        {
            throw Validator.NotSupportedException("ResultGroupIsReadOnly");
        }

        /// <summary>
        /// This method is not used, because items cannot be inserted into a
        /// read-only collection.
        /// </summary>
        ///
        /// <param name="index">
        /// This parameter is ignored because the collection is read-only.
        /// </param>
        ///
        /// <param name="value">
        /// This parameter is also ignored because the collection is read-only.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// The collection is read-only and does not support insertion.
        /// </exception>
        ///
        void IList.Insert(int index, object value)
        {
            throw Validator.NotSupportedException("ResultGroupIsReadOnly");
        }

        /// <summary>
        /// This method is not used, because items cannot be removed from a
        /// read-only collection.
        /// </summary>
        ///
        /// <param name="index">
        /// This parameter is ignored because the collection is read-only.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// The collection is read-only and does not support the removal of
        /// items.
        /// </exception>
        ///
        public void RemoveAt(int index)
        {
            throw Validator.NotSupportedException("ResultGroupIsReadOnly");
        }

        /// <summary>
        /// This method is not used, because items cannot be removed from a
        /// read-only collection.
        /// </summary>
        ///
        /// <param name="value">
        /// This parameter is ignored because the collection is read-only.
        /// </param>
        ///
        /// <exception cref="NotSupportedException">
        /// The collection is read-only and does not support the removal of
        /// items.
        /// </exception>
        ///
        void IList.Remove(object value)
        {
            throw Validator.NotSupportedException("ResultGroupIsReadOnly");
        }

        /// <summary>
        /// Gets a value indicating that the IList has a fixed size.
        /// </summary>
        ///
        /// <value>
        /// This property always returns <b>true</b>.
        /// </value>
        ///
        /// <remarks>
        /// A collection with a fixed size does not allow the addition or
        /// removal of elements after the collection is created, but it
        /// might allow the modification of existing elements.
        /// </remarks>
        ///
        public bool IsFixedSize => true;

        #endregion IList<IThing>

        #region IEnumerable<IThing>

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        ///
        /// <returns>
        /// A IEnumerator that iterates through the collection.
        /// </returns>
        ///
        /// <remarks>
        /// This enumerator might cause network requests to the HealthVault service
        /// as it enumerates the results. The HealthVault service limits the
        /// number of results with each request, therefore, it might be
        /// necessary to make multiple requests to fill in the result data as the
        /// collection is enumerated.
        /// </remarks>
        ///
        public IEnumerator<IThing> GetEnumerator()
        {
            return new ThingEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        ///
        /// <returns>
        /// A IEnumerator that iterates through the collection.
        /// </returns>
        ///
        /// <remarks>
        /// This enumerator may cause network requests to the HealthVault service
        /// as it enumerates the results. The HealthVault service limits the
        /// number of results with each request so it may be necessary to
        /// make multiple requests to fill in the result data as the
        /// collection is enumerated.
        /// </remarks>
        ///
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ThingEnumerator(this);
        }

        /// <summary>
        /// This class acts as the enumerator for the result group.
        /// </summary>
        ///
        /// <remarks>
        /// All synchronization is done in the HealthRecordItemCollection. The
        /// iterator only keeps track of the current index and uses the
        /// Item collection of the HealthRecordItemCollection to retrieve the
        /// item.
        /// </remarks>
        ///
        internal class ThingEnumerator : IEnumerator<IThing>
        {
            internal ThingEnumerator(
                HealthRecordItemCollection resultGroup)
            {
                this.resultGroup = resultGroup;
            }

            /// <summary>
            /// Gets the element in the collection at the current position of
            /// the enumerator.
            /// </summary>
            ///
            /// <value>
            /// The element in the collection at the current position of the
            /// enumerator.
            /// </value>
            ///
            /// <remarks>
            /// The <see cref="Current"/> property is undefined if:
            ///   - The enumerator is positioned before the first element
            ///     in the collection, immediately after the enumerator is
            ///     created. You must call <see cref="MoveNext"/> to advance the
            ///     enumerator to the first element of the collection before
            ///     reading the value of <see cref="Current"/>.
            ///
            ///   - The last call to <see cref="MoveNext"/> returned <b>false</b>,
            ///     which indicates the end of the collection.
            ///
            ///   - <see cref="Current"/> returns the same object until
            ///     <see cref="MoveNext"/> is called.
            ///     <see cref="MoveNext"/> sets <see cref="Current"/> to the next element.
            ///
            /// </remarks>
            ///
            public IThing Current
            {
                get
                {
                    if (this.currentIndex == -1 ||
                        this.currentIndex >= this.resultGroup.Count)
                    {
                        return null;
                    }

                    return this.resultGroup[this.currentIndex];
                }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of
            /// the enumerator.
            /// </summary>
            ///
            /// <value>
            /// The element in the collection at the current position of the
            /// enumerator.
            /// </value>
            ///
            /// <remarks>
            /// The <see cref="Current"/> property is undefined if:
            ///   - The enumerator is positioned before the first element
            ///     in the collection, immediately after the enumerator is
            ///     created. You must call <see cref="MoveNext"/> to advance the
            ///     enumerator to the first element of the collection before
            ///     reading the value of <see cref="Current"/>.
            ///
            ///   - The last call to <see cref="MoveNext"/> returned <b>false</b>,
            ///     which indicates the end of the collection.
            ///
            ///   - <see cref="Current"/> returns the same object until
            ///     <see cref="MoveNext"/> is called.
            ///     <see cref="MoveNext"/> sets <see cref="Current"/> to the next element.
            ///
            /// </remarks>
            ///
            object IEnumerator.Current => this.Current;

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            ///
            /// <returns>
            /// <b>true</b> if the enumerator was successfully advanced to the
            /// next element; <b>false</b> if the enumerator has passed the end of
            /// the collection.
            /// </returns>
            ///
            /// <remarks>
            /// After an enumerator is created or after the <see cref="Reset"/> method is
            /// called, an enumerator is positioned before the first element
            /// of the collection, and the first call to the <see cref="MoveNext"/> method
            /// moves the enumerator over the first element of the collection.
            /// <br/><br/>
            /// If <see cref="MoveNext"/> passes the end of the collection, the enumerator
            /// is positioned after the last element in the collection and
            /// <see cref="MoveNext"/> returns <b>false</b>. When the enumerator is at this
            /// position, subsequent calls to <see cref="MoveNext"/> also return <b>false</b>
            /// until <see cref="Reset"/> is called.
            /// <br/><br/>
            /// An enumerator remains valid as long as the collection
            /// remains unchanged. If changes are made to the collection,
            /// such as adding, modifying, or deleting elements, the
            /// enumerator is irrecoverably invalidated and the next call
            /// to <see cref="MoveNext"/> or <see cref="Reset"/> throws an
            /// <see cref="InvalidOperationException"/>.
            /// </remarks>
            ///
            public bool MoveNext()
            {
                if (this.currentIndex >= this.resultGroup.Count - 1)
                {
                    return false;
                }

                ++this.currentIndex;
                return true;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before
            /// the first element in the collection.
            /// </summary>
            ///
            public void Reset()
            {
                this.currentIndex = -1;
            }

            private int currentIndex = -1;

            /// <summary>
            /// Resets the enumerator.
            /// </summary>
            ///
            public void Dispose()
            {
                // I am not implementing the entire IDisposable pattern here
                // because we don't really need it. However, IEnumerator<T>
                // requires IDisposable so we have to have the method.

                this.Reset();
            }

            private HealthRecordItemCollection resultGroup;
        }

        #endregion IEnumerable<IThing>

        #region Sort

        /// <summary>
        /// Sorts the IThings in the collection using the specified comparison
        /// method.
        /// </summary>
        ///
        /// <param name="comparison">
        /// The comparison method to use when sorting the collection.
        /// </param>
        ///
        /// <remarks>
        /// This method will cause all results to be retrieved from HealthVault.<br/>
        /// When a query results in many matches, HealthVault will return a fixed number
        /// of "full" results (all data requested is returned) and the remaining matches
        /// as "partial" (only identifying information is returned). HealthRecordItemCollection
        /// automatically retrieves the full information for the partial results as the
        /// collection gets enumerated. In order to sort the results, HealthRecordItemCollection
        /// must retrieve the full set of data for all the results. This may cause several
        /// requests to HealthVault to retrieve all the data.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="comparison"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The implementation of <paramref name="comparison"/> caused an error during the sort.
        /// For example, <paramref name="comparison"/> might not return 0 when comparing an item
        /// with itself.
        /// </exception>
        ///
        public void Sort(Comparison<IThing> comparison)
        {
            List<IThing> sortableList = this.GetSortableList();
            sortableList.Sort(comparison);

            this.ReplaceAbstractResults(sortableList);
        }

        /// <summary>
        /// Sorts the IThings in the collection using the specified comparer.
        /// </summary>
        ///
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing elements,
        /// or <b>null</b> to use the defalt comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        ///
        /// <remarks>
        /// This method will cause all result to be retrieved from HealthVault.<br/>
        /// When a query results in many matches, HealthVault will return a fixed number
        /// of "full" results (all data requested is returned) and the remaining matches
        /// as "partial" (only identifying information is returned). HealthRecordItemCollection
        /// automatically pages down the full information for the partial results as the
        /// collection gets enumerated. In order to sort the results, HealthRecordItemCollection
        /// must retrieve the full set of data for all the results. This may cause several
        /// requests to HealthVault to retrieve all the data.
        /// </remarks>
        ///
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is <b>null</b>, and the default comparer
        /// <see cref="Comparer{IThing}.Default"/> cannot find implementation of the
        /// <see cref="IComparable{IThing}"/> generic interface.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The implementation of <paramref name="comparer"/> caused an error during the sort.
        /// For example, <paramref name="comparer"/> might not return 0 when
        /// comparing an item with itself.
        /// </exception>
        ///
        public void Sort(IComparer<IThing> comparer)
        {
            List<IThing> sortableList = this.GetSortableList();
            sortableList.Sort(comparer);

            this.ReplaceAbstractResults(sortableList);
        }

        private List<IThing> GetSortableList()
        {
            List<IThing> sortableList = new List<IThing>();
            foreach (IThing item in this)
            {
                sortableList.Add(item);
            }

            return sortableList;
        }

        private void ReplaceAbstractResults(List<IThing> sortableList)
        {
            this.abstractResults.Clear();
            foreach (IThing item in sortableList)
            {
                this.abstractResults.Add(item);
            }
        }

        #endregion Sort

        /// <summary>
        /// Retrieves the <see cref="IThing"/> from the result group in the
        /// specified range of indexes, including the specified indexes.
        /// </summary>
        ///
        /// <param name="minIndex">
        /// The starting point for retrieving items.
        /// </param>
        ///
        /// <param name="maxIndex">
        /// The stopping point for retrieving items.
        /// </param>
        ///
        /// <returns>
        /// A collection of items in the specified index range.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="minIndex"/> parameter is greater than the
        /// <paramref name="maxIndex"/> parameter.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either the <paramref name="minIndex"/> parameter or the
        /// <paramref name="maxIndex"/> parameter is less than zero, or the
        /// <paramref name="maxIndex"/> parameter is greater than
        /// <see cref="Count"/> -1.
        /// </exception>
        ///
        public Collection<IThing> GetRange(
            int minIndex,
            int maxIndex)
        {
            Validator.ThrowArgumentOutOfRangeIf(
                minIndex < 0,
                "minIndex",
                "ResultGroupRangeIndexesOutOfRange");

            Validator.ThrowArgumentOutOfRangeIf(
                maxIndex < 0 || maxIndex > this.Count - 1,
                "maxIndex",
                "ResultGroupRangeIndexesOutOfRange");

            Validator.ThrowArgumentOutOfRangeIf(
                minIndex > maxIndex,
                "minIndex",
                "ResultGroupRangeMinGreaterThanMax");

            Collection<IThing> result
                = new Collection<IThing>();

            for (int index = minIndex; index <= maxIndex; ++index)
            {
                result.Add(this[index]);
            }

            return result;
        }

        #region helpers

        private async Task GetPartialThingsAsync(int index)
        {
            // We must have a partial thing at this index. Get
            // the next MaxResultsPerRequest number of partial
            // items.

            List<HealthRecordItemKey> partialThings = new List<HealthRecordItemKey>();
            for (
                int partialThingIndex = index;
                (partialThings.Count < this.MaxResultsPerRequest ||
                 this.MaxResultsPerRequest == int.MinValue) &&
                partialThingIndex < this.abstractResults.Count;
                ++partialThingIndex)
            {
                var key = this.abstractResults[partialThingIndex] as HealthRecordItemKey;
                if (key != null)
                {
                    partialThings.Add(
                        key);
                }
                else
                {
                    // We break if we hit something that is
                    // not a Guid (like HealthRecordItem). This means
                    // that we will retrieve fewer things than
                    // MaxResultsPerRequest but it will be simpler
                    // to replace the partial things with the real
                    // things.
                    break;
                }
            }

            Collection<HealthRecordItem> things = await this.GetPartialThingsAsync(partialThings).ConfigureAwait(false);

            bool atEndOfPartialThings = false;
            int newThingIndex = index;
            foreach (HealthRecordItem thing in things)
            {
                // We need to start at the current index but look until
                // we find a matching thing ID.  It is possible that the
                // thing may have been removed while holding on to the
                // partial thing ID from the original query.

                for (; newThingIndex < this.abstractResults.Count; ++newThingIndex)
                {
                    HealthRecordItemKey abstractResultKey =
                        this.abstractResults[newThingIndex] as HealthRecordItemKey;

                    if (abstractResultKey != null)
                    {
                        if (abstractResultKey.Equals(thing.Key))
                        {
                            // replace the partial thing with the real
                            // thing
                            this.abstractResults[newThingIndex++] = thing;
                            break;
                        }
                    }
                    else
                    {
                        atEndOfPartialThings = true;
                        break;
                    }
                }

                if (atEndOfPartialThings)
                {
                    break;
                }
            }
        }

        private async Task<Collection<HealthRecordItem>> GetPartialThingsAsync(
            IList<HealthRecordItemKey> partialThings)
        {
            Collection<HealthRecordItem> results = new Collection<HealthRecordItem>();

            // Create the searcher
            HealthRecordSearcher searcher = this.Record.CreateSearcher();
            ThingQuery query = new ThingQuery();

            foreach (HealthRecordItemKey key in partialThings)
            {
                query.ItemKeys.Add(key);
            }

            // Need to copy the view from the original filter
            query.View = this.Query.View;
            query.States = this.Query.States;
            query.CurrentVersionOnly = this.Query.CurrentVersionOnly;
            if (this.Query.OrderByClauses.Count > 0)
            {
                foreach (var orderByClause in this.Query.OrderByClauses)
                {
                    query.OrderByClauses.Add(orderByClause);
                }
            }

            searcher.Filters.Add(query);

            // Get the partial things
            XmlReader infoReader = await searcher.GetMatchingItemsReader().ConfigureAwait(false);

            if (infoReader != null)
            {
                infoReader.ReadToDescendant("thing");
                while (infoReader.Name == "thing")
                {
                    using (XmlReader thingReader = infoReader.ReadSubtree())
                    {
                        thingReader.MoveToContent();
                        HealthRecordItem resultHealthRecordItem = ItemTypeManager.DeserializeItem(thingReader);
                        infoReader.Read();

                        if (resultHealthRecordItem != null)
                        {
                            results.Add(resultHealthRecordItem);
                        }
                    }
                }
            }

            return results;
        }

        private static XPathExpression infoGroupPath =
            XPathExpression.Compile("/wc:info/group");

        private static XPathExpression GetGroupXPathExpression(
            XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response.GetThings3");

            XPathExpression infoGroupPathClone = null;
            lock (infoGroupPath)
            {
                infoGroupPathClone = infoGroupPath.Clone();
            }

            infoGroupPathClone.SetContext(infoXmlNamespaceManager);

            return infoGroupPathClone;
        }

        private void AddResult(HealthRecordItem result)
        {
            this.abstractResults.Add(result);
        }

        private void AddResult(HealthRecordItemKey healthRecordItemKey)
        {
            this.abstractResults.Add(healthRecordItemKey);
        }

        #endregion helpers

        #region debughelper

        /// <summary>
        /// A class that helps the <see cref="HealthRecordItemCollection"/> display better in the debugger.
        /// </summary>
        ///
        private class HealthRecordItemCollectionDebugView
        {
            private HealthRecordItemCollection baseObject;

            /// <summary>
            /// Constructs a <see cref="HealthRecordItemCollectionDebugView"/> with the specified
            /// <see cref="HealthRecordItemCollection"/>.
            /// </summary>
            ///
            /// <param name="baseObject">
            /// The object this class presents a view of for the debugger.
            /// </param>
            ///
            public HealthRecordItemCollectionDebugView(HealthRecordItemCollection baseObject)
            {
                this.baseObject = baseObject;
            }

            /// <summary>
            /// Gets the items in the <see cref="HealthRecordItemCollection"/> that were fetched.
            /// </summary>
            ///
            public int FetchedItems
            {
                get
                {
                    int fetched = 0;
                    foreach (object o in this.baseObject.abstractResults)
                    {
                        if (o is HealthRecordItem)
                        {
                            fetched++;
                        }
                    }

                    return fetched;
                }
            }

            /// <summary>
            /// Gets a collection of all the items.
            /// </summary>
            ///
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Collection<object> Items => this.baseObject.abstractResults;
        }

        #endregion debughelper
    }
}
