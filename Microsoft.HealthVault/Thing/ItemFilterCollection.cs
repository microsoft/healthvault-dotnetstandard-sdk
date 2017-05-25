// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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