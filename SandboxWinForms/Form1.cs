using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Client.NetFramework;
using Microsoft.HealthVault.Configuration;

namespace SandboxWinForms
{
    public partial class Form1 : Form
    {
        private IHealthVaultSodaConnection _connection;

        public Form1()
        {
            InitializeComponent();

            HealthVaultConnectionFactory.WinFormsInvoke = this;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Connecting...";

            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse("d6318dff-5352-4a10-a140-6c82c6536a3b")
            };
            _connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
            await _connection.AuthenticateAsync();

            label1.Text = "Connected.";
        }
    }
}
