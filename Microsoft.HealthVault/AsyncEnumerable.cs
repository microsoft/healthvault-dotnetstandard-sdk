// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// A collection of asynchronous items and a method to operate on each of them.
    /// </summary>
    /// <typeparam name="T">The item type in the collection.</typeparam>
    internal class AsyncEnumerable<T>
    {
        private readonly IEnumerable<Task<T>> itemTasks;

        public AsyncEnumerable(IEnumerable<Task<T>> itemTasks)
        {
            this.itemTasks = itemTasks;
        }

        /// <summary>
        /// Iterates through a collection of <see cref="Task{T}"/> and performs an action on the item returned by each one.
        /// </summary>
        /// <param name="func">The asynchronous action to be performed on each item.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        public async Task ForeachAsync(Func<T, CancellationToken, Task> func, CancellationToken cancellationToken)
        {
            var itemTaskIterator = this.itemTasks.GetEnumerator();
            while (!cancellationToken.IsCancellationRequested && itemTaskIterator.MoveNext())
            {
                var item = await itemTaskIterator.Current.ConfigureAwait(false);
                if (!cancellationToken.IsCancellationRequested)
                {
                    await func(item, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Iterates through a collection of <see cref="Task{T}"/> and performs an action on the item returned by each one.
        /// </summary>
        /// <param name="action">The action to be performed on each item.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        public async Task ForeachAsync(Action<T, CancellationToken> action, CancellationToken cancellationToken)
        {
            var itemTaskIterator = this.itemTasks.GetEnumerator();
            while (!cancellationToken.IsCancellationRequested && itemTaskIterator.MoveNext())
            {
                var item = await itemTaskIterator.Current.ConfigureAwait(false);
                if (!cancellationToken.IsCancellationRequested)
                {
                    action(item, cancellationToken);
                }
            }
        }
    }
}
