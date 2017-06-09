using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.HealthVault.Client.Core;
using Microsoft.HealthVault.Client.Exceptions;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Interaction logic for BrowserAuthWindow.xaml
    /// </summary>
    internal partial class BrowserAuthWindow : Window
    {
        private readonly TaskCompletionSource<Uri> _tcs = new TaskCompletionSource<Uri>();

        private bool _taskCompleted = false;

        public BrowserAuthWindow(Uri startUrl, Uri stopUrlPrefix)
        {
            InitializeComponent();

            Title = ClientResources.SignInPageTitle;

            Browser.LoadCompleted += (sender, args) =>
            {
                if (args.Uri.ToString().StartsWith(stopUrlPrefix.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    _tcs.SetResult(args.Uri);
                    _taskCompleted = true;
                    Close();
                }

                // args.WebResponse is always null; we can't tell if the page errored out
                // so we let the user see the content and close the page on their own.
            };
            Browser.Navigate(startUrl);
        }

        public Task<Uri> TaskResult => _tcs.Task;

        private void BrowserAuthWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!_taskCompleted)
            {
                _tcs.SetException(new OperationCanceledException());
            }
        }
    }
}
