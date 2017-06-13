using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Client.NetFramework;
using Microsoft.HealthVault.Configuration;

namespace SandboxWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IHealthVaultSodaConnection _connection;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            OutputBlock.Text = "Connecting...";

            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("d6318dff-5352-4a10-a140-6c82c6536a3b")
            };
            _connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
            await _connection.AuthenticateAsync();

            OutputBlock.Text = "Connected.";
        }
    }
}
