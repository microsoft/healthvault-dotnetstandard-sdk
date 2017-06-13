// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.HealthVault.Client.Core;
using Microsoft.HealthVault.Client.Exceptions;
using Microsoft.HealthVault.Client.NetFramework;

namespace Microsoft.HealthVault.Client
{
    internal class NetFrameworkBrowserAuthBroker : IBrowserAuthBroker
    {
        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri stopUrlPrefix)
        {
            Task<Uri> resultTask = null;
            try
            {
                var wpfApp = System.Windows.Application.Current;
                if (wpfApp != null)
                {
                    // It's a WPF app, use the WPF dispatcher
                    resultTask = await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        var window = new BrowserAuthWindow(startUrl, stopUrlPrefix);
                        window.ShowDialog();

                        return await window.TaskResult;
                    });
                }
                else
                {
                    // It's not a WPF app; see if the user has supplied us an invoke hook for WinForms.
                    ISynchronizeInvoke winFormsInvoke = HealthVaultConnectionFactory.WinFormsInvoke;
                    if (winFormsInvoke != null)
                    {
                        InvokeIfRequired(winFormsInvoke, () =>
                        {
                            var window = new BrowserAuthWindow(startUrl, stopUrlPrefix);
                            window.ShowDialog();

                            resultTask = window.TaskResult;
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                throw new BrowserAuthException(ClientResources.BrowserAuthException, exception, null);
            }

            if (resultTask != null)
            {
                return await resultTask;
            }
            else
            {
                // The project type is not UI or the user has not specified the WinFormsInvoke object for WinForms.
                throw new BrowserAuthException(ClientResources.NoDispatcherFound, null);
            }
        }

        private static void InvokeIfRequired(ISynchronizeInvoke obj, Action action)
        {
            if (obj.InvokeRequired)
            {
                var args = new object[0];
                obj.Invoke(action, args);
            }
            else
            {
                action();
            }
        }
    }
}
