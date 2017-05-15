// (c) Microsoft. All rights reserved

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.HealthVault.Thing
{
    public sealed class ItemFilterCollection : IItemFilterCollection
    {
        private List<ThingQuery> _items = new List<ThingQuery>();

        #region IItemFilterCollection Members

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public ThingQuery this[int index]
        {
            get { return _items[index]; }

            set
            {
                ValidateItem(value);
                _items[index] = value;
            }
        }

        public void Add(ThingQuery item)
        {
            ValidateItem(item);
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(ThingQuery item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(ThingQuery[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int IndexOf(ThingQuery item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, ThingQuery item)
        {
            ValidateItem(item);
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public bool Remove(ThingQuery item)
        {
            return _items.Remove(item);
        }

        public IEnumerator<ThingQuery> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void AddRange(IEnumerable<ThingQuery> itemFilters)
        {
            _items.AddRange(itemFilters);
        }

        private static void ValidateItem(ThingQuery item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
        }
    }

    public interface IItemFilterCollection : IList<ThingQuery>
    {
    }
}