// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Entry point for HealthVault SODA based apps
    /// </summary>
    public class HealthVaultConnectionFactory
    {
        private static readonly object s_instanceLock = new object();
        private static IHealthVaultConnectionFactory _current;

        /// <summary>
        /// Gets the current factory instance.
        /// </summary>
        public static IHealthVaultConnectionFactory Current
        {
            get
            {
                if (_current == null)
                {
                    lock (s_instanceLock)
                    {
                        if (_current == null)
                        {
                            ClientIoc.EnsureTypesRegistered();
                            _current = new HealthVaultConnectionFactoryInternal();
                        }
                    }
                }

                return _current;
            }
        }

        /// <summary>
        /// Gets ThingTypeRegistrar to allow apps to register custom thing types and thing extensions
        /// </summary>
        public static IThingTypeRegistrar ThingTypeRegistrar
        {
            get
            {
                ClientIoc.EnsureTypesRegistered();
                return Ioc.Get<IThingTypeRegistrar>();
            }
        }
    }
}