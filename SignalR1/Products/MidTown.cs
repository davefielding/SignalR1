using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using System.Web;

namespace SignalR1.Products
{
    public class MidTown
    {
        private readonly static Lazy<MidTown> instance = new Lazy<MidTown>(
            () => new MidTown(GlobalHost.ConnectionManager.GetHubContext<MidTownHub>().Clients));

        private readonly QualityBasedDictionary<int, Price> prices = new QualityBasedDictionary<int, Price>();
        private readonly QualityBasedDictionary<int, Product> products = new QualityBasedDictionary<int, Product>();
        private readonly Dictionary<int, Quality> qualities = new Dictionary<int, Quality>();
        private readonly TimeSpan updateInterval = TimeSpan.FromSeconds(3);
        private readonly System.Threading.Timer timer;
        private readonly DataLoader data = new DataLoader();
        private readonly Subject<DataUpdates> priceUpdates = new Subject<DataUpdates>();

        public MidTown(IHubConnectionContext<dynamic> clients)
        {
            var random = new Random((int)DateTime.Now.Ticks / (int)(DateTime.Today.DayOfWeek) + 2);
            this.Clients = clients;
            this.LoadData();
            this.timer = new System.Threading.Timer(_ => 
            {
                // randomly add/remove items from the list
                // send updates to the clients
                var priceSize = prices.Count;
                var numChanges = Math.Max(5, random.Next((int)priceSize / 10));
                var changes = new List<DataUpdate>();
                for (int changeId = 0; changeId < numChanges; changeId++)
                {
                //    var direction = (Direction)(random.Next(100) % 2);
                    var id = random.Next(priceSize);
                    var quality = (Quality)random.Next(Enum.GetValues(typeof(Quality)).Length);
                    this.prices.Transition(id, this.prices[id], quality);
                //    var change = new DataUpdate { Direction = direction, Id = id };
                //    change.Values.Add("Quality", (direction == Direction.Out ? Quality.Good : Quality.Error).ToString());
                //    changes.Add(change);
                }

                this.priceUpdates.OnNext(new DataUpdates(changes));
            },
            null, this.updateInterval, this.updateInterval);
        }

        public IObservable<DataUpdates> PriceUpdates
        {
            get
            {
                return this.priceUpdates;
            }
        }

        private void LoadData()
        {
            var tempProducts = this.data.GetProducts().ForEach(p => products.Add(p.Id, p));
            var tempPrices = this.data.GetPrices().ForEach(p => prices.Add(p.Id, p));
        }

        public static MidTown Instance { get { return instance.Value; } }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public IEnumerable<Product> GetAllProducts() { return this.products.Values; }
        public IEnumerable<Price> GetAllPrices() { return this.prices.Values; }
        public IEnumerable<KeyValuePair<int, Quality>> GetAllQualities() { return this.qualities; }

        
    }
}