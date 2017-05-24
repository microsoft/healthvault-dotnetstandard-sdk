// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Utilities to help with dispatchers.
    /// </summary>
    internal static class DispatcherUtilities
    {
        /// <summary>
        /// Runs the given action if we currently have access to the UI thread, or
        /// on the dispatcher if it needs to be.
        /// </summary>
        /// <param name="action">The action to run.</param>
        /// <param name="runImmediately">When true, it will try to run the action
        /// inline if we're already on the UI thread. otherwise we wait for a new
        /// dispatcher task to execute the code.</param>
        public static Task RunOnUIThreadAsync(Action action, bool runImmediately = true)
        {
            return RunOnUIThreadAsync(
                () =>
                {
                    action();
                    return Task.FromResult<object>(null);
                },
                runImmediately);
        }

        /// <summary>
        /// Runs the given action if we currently have access to the UI thread, or
        /// on the dispatcher if it needs to be.
        /// </summary>
        /// <param name="func">The function to run.</param>
        /// <param name="runImmediately">When true, it will try to run the action
        /// inline if we're already on the UI thread. otherwise we wait for a new
        /// dispatcher task to execute the code.</param>
        public static Task<T> RunOnUIThreadAsync<T>(Func<T> func, bool runImmediately = true)
        {
            return RunOnUIThreadAsync(
                () =>
                {
                    return Task.FromResult(func());
                },
                runImmediately);
        }

        /// <summary>
        /// Runs the given action if we currently have access to the UI thread, or
        /// on the dispatcher if it needs to be.
        /// </summary>
        /// <param name="func">The function to run.</param>
        /// <param name="runImmediately">When true, it will try to run the action
        /// inline if we're already on the UI thread. otherwise we wait for a new
        /// dispatcher task to execute the code.</param>
        public static async Task RunOnUIThreadAsync(Func<Task> func, bool runImmediately = true)
        {
            if (runImmediately && IsOnUIThread)
            {
                await func().ConfigureAwait(false);
                return;
            }

            var tcs = new TaskCompletionSource<object>();

            await Dispatcher.RunAsync(
                new CoreDispatcherPriority(),
                async () =>
                {
                    try
                    {
                        await func().ConfigureAwait(false);
                        tcs.SetResult(null);
                    }
                    catch (Exception exception)
                    {
                        tcs.SetException(exception);
                    }
                }).AsTask().ConfigureAwait(false);

            await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the given action if we currently have access to the UI thread, or
        /// on the dispatcher if it needs to be.
        /// </summary>
        /// <param name="func">The function to run.</param>
        /// <param name="runImmediately">When true, it will try to run the action
        /// inline if we're already on the UI thread. otherwise we wait for a new
        /// dispatcher task to execute the code.</param>
        public static async Task<T> RunOnUIThreadAsync<T>(Func<Task<T>> func, bool runImmediately = true)
        {
            if (runImmediately && IsOnUIThread)
            {
                return await func().ConfigureAwait(false);
            }

            var tcs = new TaskCompletionSource<T>();

            await Dispatcher.RunAsync(
                new CoreDispatcherPriority(),
                async () =>
                {
                    try
                    {
                        tcs.SetResult(await func().ConfigureAwait(false));
                    }
                    catch (Exception exception)
                    {
                        tcs.SetException(exception);
                    }
                }).AsTask().ConfigureAwait(false);

            return await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a value that indicates whether we are on the UI thread.
        /// </summary>
        public static bool IsOnUIThread => Dispatcher.HasThreadAccess;

        /// <summary>
        /// Gets the application's dispatcher.
        /// </summary>
        private static CoreDispatcher Dispatcher
        {
            get { return Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher; }
        }
    }
}
