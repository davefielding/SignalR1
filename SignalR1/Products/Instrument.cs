using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR1.Products
{
    public class Instrument : IQuality
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISIN { get; set; }
        public Quality Quality { get; set; }
        public Instrument Underlyer { get; set; }
    }

}