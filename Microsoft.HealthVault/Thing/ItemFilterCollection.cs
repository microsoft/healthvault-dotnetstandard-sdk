// (c) Microsoft. All rights reserved

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.HealthVault.Thing
{
    public sealed class ItemFilterCollection : IItemFilterCollection
    {
        private List<ThingQuery> items = new List<ThingQuery>();

        #region IItemFilterCollection Members

        public int Count => this.items.Count;

        public bool IsReadOnly => false;

        public ThingQuery this[int index]
        {
            get { return this.items[index]; }

            set
            {
                ValidateItem(value);
                this.items[index] = value;
            }
        }

        public void Add(ThingQuery item)
        {
            ValidateItem(item);
            this.items.Add(item);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(ThingQuery item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(ThingQuery[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public int IndexOf(ThingQuery item)
        {
            return this.items.IndexOf(item);
        }

        public void Insert(int index, ThingQuery item)
        {
            ValidateItem(item);
            this.items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        public bool Remove(ThingQuery item)
        {
            return this.items.Remove(item);
        }

        public IEnumerator<ThingQuery> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public void AddRange(IEnumerable<ThingQuery> itemFilters)
        {
            this.items.AddRange(itemFilters);
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