using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR1.Products
{
    public class Product : Instrument
    {
        public string PrimaryLink { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}