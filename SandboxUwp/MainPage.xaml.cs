using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Vocabulary;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SandboxUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IHealthVaultSodaConnection connection;

        public MainPage()
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
            connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);
            await connection.AuthenticateAsync();

            OutputBlock.Text = "Connected.";
        }

        private async void Get_BP_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = connection.CreateThingClient();

            var bloodPressures = await thingClient.GetThingsAsync<BloodPressure>(recordInfo.Id);
            BloodPressure firstBloodPressure = bloodPressures.FirstOrDefault();
            if (firstBloodPressure == null)
            {
                OutputBlock.Text = "No blood pressures.";
            }
            else
            {
                OutputBlock.Text = firstBloodPressure.Systolic + "/" + firstBloodPressure.Diastolic;
            }
        }

        private async void SetBP_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = connection.CreateThingClient();

            await thingClient.CreateNewThingsAsync(recordInfo.Id, new List<BloodPressure> { new BloodPressure(new HealthServiceDateTime(DateTime.Now), 117, 70) });

            OutputBlock.Text = "Created blood pressure.";
        }

        private async void GetUserImage_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;

            IThingClient thingClient = connection.CreateThingClient();
            ThingQuery query = new ThingQuery();
            query.View.Sections = ThingSections.Default | ThingSections.BlobPayload;
            var theThings = await thingClient.GetThingsAsync<PersonalImage>(recordInfo.Id, query);

            Stream imageStream = theThings.First().ReadImage();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memoryStream);
                OutputBlock.Text = $"Image has {memoryStream.Length} bytes";
            }
        }

        private async void GetVocab_OnClick(object sender, RoutedEventArgs e)
        {
            var vocabClient = connection.CreateVocabularyClient();
            IList<VocabularyKey> keys = (await vocabClient.GetVocabularyKeysAsync()).ToList();

            VocabularyKey key = keys[4];

            Vocabulary vocab = await vocabClient.GetVocabularyAsync(key);

            if (vocab.IsTruncated)
            {
                VocabularyKey key2 = new VocabularyKey(key.Name, key.Family, key.Version, vocab.Values.Last().Value);
                Vocabulary vocab2 = await vocabClient.GetVocabularyAsync(key2);

                OutputBlock.Text = $"There are {vocab2.Count} items on the second call";
            }
            else
            {
                OutputBlock.Text = $"There are {vocab.Count} items";
            }
        }

        private async void DeleteConnectionInfo_OnClick(object sender, RoutedEventArgs e)
        {
            await connection.DeauthorizeApplicationAsync();
            OutputBlock.Text = "Deleted connection information.";
        }
    }
}
