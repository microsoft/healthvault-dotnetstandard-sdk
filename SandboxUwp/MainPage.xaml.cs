// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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
using NodaTime;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SandboxUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IHealthVaultSodaConnection _connection;
        private IClock _clock;
        private IDateTimeZoneProvider _dateTimeZoneProvider;

        private const string AppId = "d6318dff-5352-4a10-a140-6c82c6536a3b";
        private const string CustomDataTypeSubtypeTag = "custom-data-type";

        public MainPage()
        {
            InitializeComponent();

            _clock = SystemClock.Instance;
            _dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;
        }

        private async void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            OutputBlock.Text = "Connecting...";

            var configuration = new HealthVaultConfiguration
            {
                MasterApplicationId = Guid.Parse(AppId)
            };
            _connection = HealthVaultConnectionFactory.Current.GetOrCreateSodaConnection(configuration);

            HealthVaultConnectionFactory.ThingTypeRegistrar.RegisterApplicationSpecificHandler(
                applicationId: AppId,
                subtypeTag: CustomDataTypeSubtypeTag,
                applicationSpecificHandlerClass: typeof(CustomDataType));

            await _connection.AuthenticateAsync();

            OutputBlock.Text = "Connected.";
        }

        private async void Get_BP_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = _connection.CreateThingClient();

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
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = _connection.CreateThingClient();

            LocalDateTime nowLocal = _clock.GetCurrentInstant().InZone(_dateTimeZoneProvider.GetSystemDefault()).LocalDateTime;

            await thingClient.CreateNewThingsAsync(
                recordInfo.Id, 
                new List<BloodPressure>
                {
                    new BloodPressure(new HealthServiceDateTime(nowLocal), 117, 70)
                });

            OutputBlock.Text = "Created blood pressure.";
        }

        private async void Get_Height_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = _connection.CreateThingClient();
            var heights = await thingClient.GetThingsAsync<Height>(recordInfo.Id);
            Height firstHeight = heights.FirstOrDefault();
            if (firstHeight == null)
            {
                OutputBlock.Text = "No height.";
            }
            else
            {
                OutputBlock.Text = firstHeight.Value.Meters.ToString() + "m";
            }
        }

        private async void Set_Height_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = _connection.CreateThingClient();

            Random rand = new Random();
            double minHeight = 1.53;
            double maxHeight = 1.83;
            double range = maxHeight - minHeight;
            double randHeight = Math.Round((minHeight + rand.NextDouble() * range), 2);

            LocalDateTime nowLocal = _clock.GetCurrentInstant().InZone(_dateTimeZoneProvider.GetSystemDefault()).LocalDateTime;

            await thingClient.CreateNewThingsAsync(
                recordInfo.Id, 
                new List<Height>
                {
                    new Height(new HealthServiceDateTime(nowLocal), new Length(randHeight))
                });
            OutputBlock.Text = "Created height.";
        }

        private async void GetUserImage_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;

            IThingClient thingClient = _connection.CreateThingClient();
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
            var vocabClient = _connection.CreateVocabularyClient();
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

        // Get custom data from HealthVault
        private async void Get_CustomData_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = _connection.CreateThingClient();

            IReadOnlyCollection<ApplicationSpecific> healthData = await thingClient.GetThingsAsync<ApplicationSpecific>(recordInfo.Id);

            if (healthData.Count == 0)
            {
                OutputBlock.Text = "No custom data found.";
            }
            else
            {
                List<CustomDataType> customDataTypes = new List<CustomDataType>();

                foreach (ApplicationSpecific customDataType in healthData)
                {
                    CustomDataType customData = new CustomDataType
                    {
                        Values = new List<double>()
                    };

                    var appSpecificXml = customDataType?.ApplicationSpecificXml;
                    if (appSpecificXml.Count <= 0)
                    {
                        continue;
                    }
                    XDocument doc = XDocument.Parse(appSpecificXml[0].CreateNavigator()?.OuterXml);

                    foreach (XElement element in doc.Descendants("CustomDataType"))
                    {
                        customData.StartTime = long.Parse(element.Attribute("StartTime").Value);
                        customData.EndTime = long.Parse(element.Attribute("EndTime").Value);

                        var array = element.Attribute("Values")?.Value.Split(',');
                        if (array == null)
                        {
                            continue;
                        }

                        foreach (var entry in array)
                        {
                            double pressure = double.Parse(entry);
                            customData.Values.Add(pressure);
                        }
                        customDataTypes.Add(customData);
                    }

                }
                OutputBlock.Text = "Custom data found.";
            }
        }

        // Store custom data in HealthVault
        private async void Set_CustomData_OnClick(object sender, RoutedEventArgs e)
        {
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            HealthRecordInfo recordInfo = personInfo.SelectedRecord;
            IThingClient thingClient = _connection.CreateThingClient();

            await thingClient.CreateNewThingsAsync(
                recordInfo.Id,
                new List<CustomDataType>
                {
                    new CustomDataType
                    {
                        StartTime = DateTime.Now.Ticks,
                        EndTime = DateTime.Now.Ticks + 1200,
                        Values = new List<double> { 1.8, 2.5, 2.6, 3.7 },
                        ApplicationId = AppId,
                        SubtypeTag = CustomDataTypeSubtypeTag,
                        Description = "CustomDataType"
                    }
                });
            OutputBlock.Text = "Created custom data type.";
        }

        private async void DeleteConnectionInfo_OnClick(object sender, RoutedEventArgs e)
        {
            await _connection.DeauthorizeApplicationAsync();
            OutputBlock.Text = "Deleted connection information.";
        }
    }
}
