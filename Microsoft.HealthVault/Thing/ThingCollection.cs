// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;

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
    public class ThingCollection :
        IList<IThing>,
        IList
    {
        #region ctor

        /// <summary>
        /// Create an instance of the <see cref="ThingCollection"/> class with a specific set of items.
        /// </summary>
        ///
        /// <remarks>
        /// This constructor is intended for testing purposes.
        /// </remarks>
        ///
        /// <param name="items">The items to put into the collection.</param>
        public ThingCollection(IEnumerable<ThingBase> items)
        {
            foreach (ThingBase item in items)
            {
                _abstractResults.Add(item);
            }
        }

        internal ThingCollection(
            string name,
            HealthRecordAccessor record,
            ThingQuery query,
            IHealthVaultConnection healthVaultConnection)
        {
            Name = name;
            Record = record;
            Query = query;
            Connection = healthVaultConnection;
        }

        #endregion ctor

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
        /// <b>true</b> if the result set of things was filtered due
        /// to the callers permissions; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool WasFiltered { get; internal set; }

        /// <summary>
        /// Gets the value indicating the culture that order by values were sorted in.
        /// </summary>
        ///
        /// <remarks>
        /// The culture may not be the same culture as requested in request header.  It is the closest match HealthVault supports.
        /// </remarks>
        ///
        public string OrderByCulture { get; internal set; }

        /// <summary>
        /// Gets the health record that the items were retrieved from.
        /// </summary>
        ///
        internal HealthRecordAccessor Record { get; }

        internal ThingQuery Query { get; }

        internal IHealthVaultConnection Connection { get; }

        // This collection contains a combination of the full IThing
        // results as well as any partial thing IDs that were returned
        private Collection<object> _abstractResults = new Collection<object>();

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
        public int Count => _abstractResults.Count;

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
        public object SyncRoot => _abstractResults;

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
            throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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
            throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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
            throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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

            return Contains(item.Key);
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the object
        /// having the specified value.
        /// </summary>
        ///
        /// <param name="value">
        /// The thing to locate in the collection.
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

            result = thing != null ? Contains(thing) : Contains(value as ThingKey);

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains a
        /// <see cref="IThing"/> with the specified
        /// <see cref="ThingKey"/>.
        /// </summary>
        ///
        /// <param name="itemKey">
        /// The unique <see cref="ThingKey"/> used to locate the
        /// <see cref="ThingBase"/>item in the collection. The key
        /// contains a unique identifier for the <see cref="ThingBase"/>
        /// and a unique version stamp identifying the version of
        /// the <see cref="IThing"/>.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if a matching object is found; otherwise, <b>false</b>.
        /// </returns>
        ///
        public bool Contains(ThingKey itemKey)
        {
            lock (_abstractResults)
            {
                bool result = false;
                for (int index = 0; index < _abstractResults.Count; ++index)
                {
                    ThingKey abstractThingKey;
                    IThing thing
                        = _abstractResults[index] as IThing;
                    if (thing != null)
                    {
                        abstractThingKey = thing.Key;
                    }
                    else
                    {
                        abstractThingKey
                            = (ThingKey)_abstractResults[index];
                    }

                    if (abstractThingKey.Equals(itemKey))
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
            throw new NotSupportedException(Resources.ResultGroupNotCopyable);
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
            throw new NotSupportedException(Resources.ResultGroupNotCopyable);
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
            throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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
                if (index < 0 || index >= _abstractResults.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), Resources.ResultGroupIndexOutOfRange);
                }

                lock (_abstractResults)
                {
                    IThing result = _abstractResults[index] as IThing;
                    if (result == null)
                    {
                        GetPartialThingsAsync(index).Wait();
                        result = _abstractResults[index] as IThing;
                    }

                    return result;
                }
            }

            set
            {
                throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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
                throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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

            return IndexOf(item.Key);
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
                result = IndexOf(thing);
            }
            else
            {
                result = IndexOf(value as ThingKey);
            }

            return result;
        }

        /// <summary>
        /// Determines the index of the specific item in the list using the
        /// unique thing identifier.
        /// </summary>
        ///
        /// <param name="key">
        /// The unique thing key used to locate the
        /// item in the list.
        /// </param>
        ///
        /// <returns>
        /// The index of item, if found in the list; otherwise, -1.
        /// </returns>
        ///
        public int IndexOf(ThingKey key)
        {
            lock (_abstractResults)
            {
                int result = -1;
                for (int index = 0; index < _abstractResults.Count; ++index)
                {
                    ThingKey abstractThingKey;
                    IThing thing = _abstractResults[index] as IThing;
                    if (thing != null)
                    {
                        abstractThingKey = thing.Key;
                    }
                    else
                    {
                        abstractThingKey = (ThingKey)_abstractResults[index];
                    }

                    if (abstractThingKey.Equals(key))
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
            throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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
            throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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
            throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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
            throw new NotSupportedException(Resources.ResultGroupIsReadOnly);
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
        /// All synchronization is done in the ThingCollection. The
        /// iterator only keeps track of the current index and uses the
        /// Item collection of the ThingCollection to retrieve the
        /// item.
        /// </remarks>
        ///
        internal class ThingEnumerator : IEnumerator<IThing>
        {
            internal ThingEnumerator(
                ThingCollection resultGroup)
            {
                _resultGroup = resultGroup;
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
                    if (_currentIndex == -1 ||
                        _currentIndex >= _resultGroup.Count)
                    {
                        return null;
                    }

                    return _resultGroup[_currentIndex];
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
            object IEnumerator.Current => Current;

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
                if (_currentIndex >= _resultGroup.Count - 1)
                {
                    return false;
                }

                ++_currentIndex;
                return true;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before
            /// the first element in the collection.
            /// </summary>
            ///
            public void Reset()
            {
                _currentIndex = -1;
            }

            private int _currentIndex = -1;

            /// <summary>
            /// Resets the enumerator.
            /// </summary>
            ///
            public void Dispose()
            {
                // I am not implementing the entire IDisposable pattern here
                // because we don't really need it. However, IEnumerator<T>
                // requires IDisposable so we have to have the method.

                Reset();
            }

            private ThingCollection _resultGroup;
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
        /// as "partial" (only identifying information is returned). ThingCollection
        /// automatically retrieves the full information for the partial results as the
        /// collection gets enumerated. In order to sort the results, ThingCollection
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
            List<IThing> sortableList = GetSortableList();
            sortableList.Sort(comparison);

            ReplaceAbstractResults(sortableList);
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
        /// as "partial" (only identifying information is returned). ThingCollection
        /// automatically pages down the full information for the partial results as the
        /// collection gets enumerated. In order to sort the results, ThingCollection
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
            List<IThing> sortableList = GetSortableList();
            sortableList.Sort(comparer);

            ReplaceAbstractResults(sortableList);
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
            _abstractResults.Clear();
            foreach (IThing item in sortableList)
            {
                _abstractResults.Add(item);
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
            if (minIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minIndex), Resources.ResultGroupRangeIndexesOutOfRange);
            }

            if (maxIndex < 0 || maxIndex > Count - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxIndex), Resources.ResultGroupRangeIndexesOutOfRange);
            }

            if (minIndex > maxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(minIndex), Resources.ResultGroupRangeMinGreaterThanMax);
            }

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

            List<ThingKey> partialThings = new List<ThingKey>();
            for (
                int partialThingIndex = index;
                (partialThings.Count < MaxResultsPerRequest ||
                 MaxResultsPerRequest == int.MinValue) &&
                partialThingIndex < _abstractResults.Count;
                ++partialThingIndex)
            {
                var key = _abstractResults[partialThingIndex] as ThingKey;
                if (key != null)
                {
                    partialThings.Add(
                        key);
                }
                else
                {
                    // We break if we hit something that is
                    // not a Guid (like ThingBase). This means
                    // that we will retrieve fewer things than
                    // MaxResultsPerRequest but it will be simpler
                    // to replace the partial things with the real
                    // things.
                    break;
                }
            }

            ThingCollection things = await GetPartialThingsAsync(partialThings).ConfigureAwait(false);

            bool atEndOfPartialThings = false;
            int newThingIndex = index;
            foreach (IThing thing in things)
            {
                // We need to start at the current index but look until
                // we find a matching thing ID.  It is possible that the
                // thing may have been removed while holding on to the
                // partial thing ID from the original query.

                for (; newThingIndex < _abstractResults.Count; ++newThingIndex)
                {
                    ThingKey abstractResultKey =
                        _abstractResults[newThingIndex] as ThingKey;

                    if (abstractResultKey != null)
                    {
                        if (abstractResultKey.Equals(thing.Key))
                        {
                            // replace the partial thing with the real
                            // thing
                            _abstractResults[newThingIndex++] = thing;
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

        private async Task<ThingCollection> GetPartialThingsAsync(
            IList<ThingKey> partialThings)
        {
            IThingClient thingClient = Connection.CreateThingClient();

            ThingQuery query = new ThingQuery();

            foreach (ThingKey key in partialThings)
            {
                query.ItemKeys.Add(key);
            }

            // Need to copy the view from the original filter
            query.View = Query.View;
            query.States = Query.States;
            query.CurrentVersionOnly = Query.CurrentVersionOnly;
            if (Query.OrderByClauses.Count > 0)
            {
                foreach (var orderByClause in Query.OrderByClauses)
                {
                    query.OrderByClauses.Add(orderByClause);
                }
            }

            ThingCollection results = await thingClient.GetThingsAsync(Record.Id, query).ConfigureAwait(false);

            return results;
        }

        internal void AddResult(ThingBase result)
        {
            _abstractResults.Add(result);
        }

        internal void AddResult(ThingKey thingKey)
        {
            _abstractResults.Add(thingKey);
        }

        #endregion helpers

        #region debughelper

        /// <summary>
        /// A class that helps the <see cref="ThingCollection"/> display better in the debugger.
        /// </summary>
        ///
        private class HealthRecordItemCollectionDebugView
        {
            private ThingCollection _baseObject;

            /// <summary>
            /// Constructs a <see cref="HealthRecordItemCollectionDebugView"/> with the specified
            /// <see cref="ThingCollection"/>.
            /// </summary>
            ///
            /// <param name="baseObject">
            /// The object this class presents a view of for the debugger.
            /// </param>
            ///
            public HealthRecordItemCollectionDebugView(ThingCollection baseObject)
            {
                _baseObject = baseObject;
            }

            /// <summary>
            /// Gets the items in the <see cref="ThingCollection"/> that were fetched.
            /// </summary>
            ///
            public int FetchedItems
            {
                get
                {
                    int fetched = 0;
                    foreach (object o in _baseObject._abstractResults)
                    {
                        if (o is ThingBase)
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
            public Collection<object> Items => _baseObject._abstractResults;
        }

        #endregion debughelper
    }
}
