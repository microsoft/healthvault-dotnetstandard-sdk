// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Reads and writes simple objects to encrypted local storage.
    /// </summary>
    internal interface ILocalObjectStore
    {
        /// <summary>
        /// Reads an object from local storage.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key the object is stored under.</param>
        /// <returns>The stored object, or null if it was not found.</returns>
        Task<T> ReadAsync<T>(string key);

        /// <summary>
        /// Writes an object to local storage.
        /// </summary>
        /// <param name="key">The key the object is to be stored under.</param>
        /// <param name="value">The value to store.</param>
        Task WriteAsync(string key, object value);

        /// <summary>
        /// Deletes an object from local storage.
        /// </summary>
        /// <param name="key">The key the object is stored under.</param>
        Task DeleteAsync(string key);
    }
}