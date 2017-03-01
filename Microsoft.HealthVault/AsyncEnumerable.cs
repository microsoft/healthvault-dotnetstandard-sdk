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
