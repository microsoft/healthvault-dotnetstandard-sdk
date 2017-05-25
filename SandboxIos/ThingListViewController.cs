// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreGraphics;
using Foundation;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Thing;
using UIKit;

namespace SandboxIos
{
    public partial class ThingListViewController<TThing> : UIViewController, IUICollectionViewDataSource, IUICollectionViewDelegateFlowLayout
        where TThing : IThing
    {
        private IHealthVaultSodaConnection _connection;
        private IReadOnlyCollection<IThing> _collection;
        private IThingCell _prototypeCell;

        private Dictionary<Type, UINib> _cellMap = new Dictionary<Type, UINib>()
        {
            {typeof(BloodPressure), BloodPressureCell.Nib }
        };

        private Dictionary<Type, Type> _viewControllerMap = new Dictionary<Type, Type>()
        {
            {typeof(BloodPressure), typeof(BloodPressureEntryViewController)}
        };

        public ThingListViewController(IHealthVaultSodaConnection connection)

            : base("ThingListViewController", null)
        {
            connection = connection;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UINib nib = null;

            if (_cellMap.TryGetValue(typeof(TThing), out nib))
            {
                // Register the collection view cell
                collectionView.RegisterNibForCell(nib, "Cell");

                NSObject[] views = nib.Instantiate(null, null);
                _prototypeCell = (IThingCell)views.First<NSObject>();
            }

            // Add a + button to the navigation bar
            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Add,
                (sender, args) =>
                {
                    AddButtonPressed();
                }),
                false);
        }

        private void AddButtonPressed()
        {
            Type type = null;

            if (!_viewControllerMap.TryGetValue(typeof(TThing), out type))
            {
                return;
            }

            ConstructorInfo constructor = type.GetConstructor(new[] { typeof(IHealthVaultSodaConnection) });

            UIViewController viewController = (UIViewController)constructor.Invoke(new object[] { _connection });

            PresentViewController(new UINavigationController(viewController), true, null);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            LoadCollectionAsync();
        }

        private async void LoadCollectionAsync()
        {
            activityIndicator.StartAnimating();
            messageLabel.Hidden = true;

            IThingClient thingClient = _connection.CreateThingClient();
            PersonInfo personInfo = await _connection.GetPersonInfoAsync();
            _collection = (IReadOnlyCollection<IThing>)await thingClient.GetThingsAsync<TThing>(personInfo.SelectedRecord.Id);

            activityIndicator.StopAnimating();

            collectionView.ReloadData();

            if (_collection.Count < 1)
            {
                messageLabel.Hidden = false;
            }
        }

        [Export("collectionView:cellForItemAtIndexPath:")]
        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionReusableView cell = collectionView.DequeueReusableCell("Cell", indexPath);

            ((IThingCell)cell).SetThing(_collection.ElementAt<IThing>(indexPath.Row));

            return (UICollectionViewCell)cell;
        }

        [Export("collectionView:numberOfItemsInSection:")]
        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (_collection == null)
            {
                return 0;
            }

            return _collection.Count;
        }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return _prototypeCell.GetSize(_collection.ElementAt<IThing>(indexPath.Row));
        }
    }
}