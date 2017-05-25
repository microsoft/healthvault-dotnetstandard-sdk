// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Factory for creating client connections to HealthVault.
    /// </summary>
    internal class HealthVaultConnectionFactoryInternal : IHealthVaultConnectionFactory
    {
        private readonly object _connectionLock = new object();

        private HealthVaultSodaConnection _cachedConnection;

        /// <summary>
        /// Gets an <see cref="IHealthVaultSodaConnection"/> used to connect to HealthVault.
        /// </summary>
        /// <param name="configuration">Configuration required for authenticating the connection</param>
        /// <returns>Connection object to be used by the Client classes</returns>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="GetOrCreateSodaConnection"/> has been called already with a different MasterApplicationId.
        /// </exception>
        public IHealthVaultSodaConnection GetOrCreateSodaConnection(HealthVaultConfiguration configuration)
        {
            lock (_connectionLock)
            {
                if (_cachedConnection != null)
                {
                    ValidateConfiguration(_cachedConnection.Configuration, configuration);
                    return _cachedConnection;
                }

                var missingProperties = new List<string>();

                Guid masterApplicationId = configuration.MasterApplicationId;
                if (masterApplicationId == Guid.Empty)
                {
                    missingProperties.Add(nameof(configuration.MasterApplicationId));
                }

                if (missingProperties.Count > 0)
                {
                    string requiredPropertiesString = string.Join(", ", missingProperties);
                    throw new InvalidOperationException(Resources.MissingRequiredProperties.FormatResource(requiredPropertiesString));
                }

                Ioc.Container.Configure(c => c.ExportInstance(configuration).As<HealthVaultConfiguration>());

                HealthVaultSodaConnection newConnection = Ioc.Get<HealthVaultSodaConnection>();

                _cachedConnection = newConnection;
                return newConnection;
            }
        }

        private static void ValidateConfiguration(HealthVaultConfiguration currentConfiguration, HealthVaultConfiguration configuration)
        {
            if (currentConfiguration.MasterApplicationId != configuration.MasterApplicationId)
            {
                throw new InvalidOperationException(Resources.CannotAuthWithDifferentMasterApplicationId.FormatResource(
                    nameof(GetOrCreateSodaConnection),
                    currentConfiguration.MasterApplicationId,
                    configuration.MasterApplicationId));
            }
        }
    }
}
