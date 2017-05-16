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
        private IHealthVaultSodaConnection connection;
        private IReadOnlyCollection<IThing> collection;
        private IThingCell prototypeCell;

        private Dictionary<Type, UINib> cellMap = new Dictionary<Type, UINib>()
        {
            {typeof(BloodPressure), BloodPressureCell.Nib }
        };

        private Dictionary<Type, Type> viewControllerMap = new Dictionary<Type, Type>()
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

            if (cellMap.TryGetValue(typeof(TThing), out nib))
            {
                // Register the collection view cell
                collectionView.RegisterNibForCell(nib, "Cell");

                NSObject[] views = nib.Instantiate(null, null);
                prototypeCell = (IThingCell)views.First<NSObject>();
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

            if (!viewControllerMap.TryGetValue(typeof(TThing), out type))
            {
                return;
            }

            ConstructorInfo constructor = type.GetConstructor(new[] { typeof(IHealthVaultSodaConnection) });

            UIViewController viewController = (UIViewController)constructor.Invoke(new object[] { connection });

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

            IThingClient thingClient = connection.CreateThingClient();
            PersonInfo personInfo = await connection.GetPersonInfoAsync();
            collection = (IReadOnlyCollection<IThing>)await thingClient.GetThingsAsync<TThing>(personInfo.SelectedRecord.Id);

            activityIndicator.StopAnimating();

            collectionView.ReloadData();

            if (collection.Count < 1)
            {
                messageLabel.Hidden = false;
            }
        }

        [Export("collectionView:cellForItemAtIndexPath:")]
        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionReusableView cell = collectionView.DequeueReusableCell("Cell", indexPath);

            ((IThingCell)cell).SetThing(collection.ElementAt<IThing>(indexPath.Row));

            return (UICollectionViewCell)cell;
        }

        [Export("collectionView:numberOfItemsInSection:")]
        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (collection == null)
            {
                return 0;
            }

            return collection.Count;
        }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return prototypeCell.GetSize(collection.ElementAt<IThing>(indexPath.Row));
        }
    }
}