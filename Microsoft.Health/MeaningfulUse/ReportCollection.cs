// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Health.MeaningfulUse
{
    /// <summary>
    /// A base implementation of Meaningful Use report collection of item(s).
    /// </summary>
    internal abstract class ReportCollection<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// List of items in the collection
        /// </summary>
        protected Collection<T> Items
        {
            get
            {
                return _items;
            }
        }

        private Collection<T> _items = new Collection<T>();

        /// <summary>
        /// Gets or sets if there is more items to be retrieved from Platform.
        /// </summary>
        protected bool HasMoreItems
        {
            get
            {
                return _hasMoreItems;
            }

            set
            {
                _hasMoreItems = value;
            }
        }

        private bool _hasMoreItems = true;

        /// <summary>
        /// Gets or sets the cursor from Platform if paging has occurred.
        /// </summary>
        protected string Cursor { get; set; }

        #region IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        /// <remarks>
        /// This enumerator might cause network requests to the HealthVault
        /// as it enumerates the results. The HealthVault service limits the
        /// number of results with each request, therefore, it might be 
        /// necessary to make multiple requests to fill in the result data as the 
        /// collection is enumerated.
        /// </remarks>
        /// 
        public IEnumerator<T> GetEnumerator()
        {
            return new CollectionEnumerator<T>(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
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
            return new CollectionEnumerator<T>(this);
        }

        /// <summary>
        /// This class acts as the enumerator for the result group.
        /// </summary>
        /// 
        /// <remarks>
        /// All synchronization is done in the PatientActivityCollection. The
        /// iterator only keeps track of the current index and uses the 
        /// Item collection of the PatientActivityCollection to retrieve the
        /// item.
        /// </remarks>
        /// 
        internal class CollectionEnumerator<Generic> : IEnumerator<Generic>
        {
            internal CollectionEnumerator(
                ReportCollection<Generic> collection)
            {
                _collection = collection;
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
            public Generic Current
            {
                get
                {
                    if (_currentIndex == -1 ||
                        (_currentIndex >= _collection.Items.Count &&
                        !_collection.HasMoreItems))
                    {
                        return default(Generic);
                    }

                    return _collection.GetItem(_currentIndex);
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
            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

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
                if (_collection.Items.Count == 0 && _collection.HasMoreItems)
                {
                    _collection.GetItem(0);
                }

                if (_currentIndex >= _collection.Items.Count - 1 && !_collection.HasMoreItems)
                {
                    return false;
                }

                _currentIndex++;
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
            public void Dispose()
            {
                Reset();
            }

            private ReportCollection<Generic> _collection;
        }
        #endregion

        #region helpers
        private T GetItem(int index)
        {
            if (Items.Count <= index)
            {
                if (_hasMoreItems)
                {
                    GetItemsFromPlatform();
                }
            }

            if (Items.Count <= index)
            {
                return default(T);
            }
            else
            {
                return Items[index];
            }
        }

        /// <summary>
        /// Helper to get items from platform
        /// </summary>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// </exception>
        protected abstract void GetItemsFromPlatform();
        #endregion helpers
    }
}
