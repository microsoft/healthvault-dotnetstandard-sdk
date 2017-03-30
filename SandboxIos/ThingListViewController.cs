using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Thing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using System.Reflection;
using System.Linq;
using CoreGraphics;

namespace SandboxIos
{
    public partial class ThingListViewController<TThing> : UIViewController, IUICollectionViewDataSource, IUICollectionViewDelegateFlowLayout
        where TThing:IThing
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
            this.connection = connection;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UINib nib = null;

            if (this.cellMap.TryGetValue(typeof(TThing), out nib))
            {
                // Register the collection view cell
                this.collectionView.RegisterNibForCell(nib, "Cell");

                NSObject[] views = nib.Instantiate(null, null);
                this.prototypeCell = (IThingCell)views.First<NSObject>();
            }

            // Add a + button to the navigation bar
            this.NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Add,
                (sender, args) =>
                {
                    this.AddButtonPressed();
                }),
                false);
        }

        private void AddButtonPressed()
        {
            Type type = null;

            if (!this.viewControllerMap.TryGetValue(typeof(TThing), out type))
            {
                return;
            }

            ConstructorInfo constructor = type.GetConstructor(new[] { typeof(IHealthVaultSodaConnection)});

            UIViewController viewController = (UIViewController)constructor.Invoke(new object[] { this.connection });

            this.PresentViewController(new UINavigationController(viewController), true, null);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.LoadCollectionAsync();
        }

        private async void LoadCollectionAsync()
        {
            this.activityIndicator.StartAnimating();
            this.messageLabel.Hidden = true;

            IThingClient thingClient = ClientHealthVaultFactory.GetThingClient(this.connection);
            PersonInfo personInfo = await this.connection.GetPersonInfoAsync();
            this.collection = (IReadOnlyCollection<IThing>)await thingClient.GetThingsAsync<TThing>(personInfo.SelectedRecord.Id);

            this.activityIndicator.StopAnimating();

            this.collectionView.ReloadData();

            if (this.collection.Count < 1)
            {
                this.messageLabel.Hidden = false;
            }
        }

        [Export("collectionView:cellForItemAtIndexPath:")]
        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionReusableView cell = collectionView.DequeueReusableCell("Cell", indexPath);

            ((IThingCell)cell).SetThing(this.collection.ElementAt<IThing>(indexPath.Row));

            return (UICollectionViewCell)cell;
        }

        [Export("collectionView:numberOfItemsInSection:")]
        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.collection == null)
            {
                return 0;
            }

            return this.collection.Count;
        }

        [Export ("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return this.prototypeCell.GetSize(this.collection.ElementAt<IThing>(indexPath.Row));
        }
    }
}