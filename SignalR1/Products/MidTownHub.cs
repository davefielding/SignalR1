using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR1.Products
{
    [HubName("midTown")]
    public class MidTownHub : Hub
    {
        private readonly MidTown midTown;
        public MidTownHub(MidTown midTown)
        {
            this.midTown = midTown;
            this.midTown.PriceUpdates.Subscribe(u => Clients.All.UpdatePrices(u));
        }
        public MidTownHub() : this(MidTown.Instance) { }
        public IEnumerable<Price> GetAllPrices() { return this.midTown.GetAllPrices(); }
        public IEnumerable<Product> GetAllProducts() { return this.midTown.GetAllProducts(); }
        public IEnumerable<KeyValuePair<int, Quality>> GetAllQualities() { return this.midTown.GetAllQualities(); }


    }
}