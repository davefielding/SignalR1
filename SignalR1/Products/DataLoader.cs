using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR1.Products
{
    public class DataLoader
    {
        private static readonly object dataLock = new object();
        private static List<Product> products;
        private static List<Price> prices;
        private static Dictionary<int, Quality> qualities;

        public IEnumerable<Product> GetProducts()
        {
            this.PopulateData();
            return products;
        }

        public IEnumerable<Price> GetPrices()
        {
            this.PopulateData();
            return prices;
        }

        private void PopulateData()
        {
            lock (dataLock)
            {
                if (qualities == null) { this.PopulateQualities(); }
                if (prices == null) { this.PopulatePrices(); }
                if (products == null) { this.PopulateProducts(); }
            }
        }

        private void PopulateQualities()
        {
            qualities = new Dictionary<int, Quality>();
            foreach (var q in Enum.GetValues(typeof(Quality)))
            {
                qualities.Add((int)q, (Quality)q);
            }
        }

        private void PopulatePrices()
        {
            var id = 0;
            var q = Quality.Good;
            prices = new List<Price>{
                new Price { Id = id++, Name = "DAX Index", Ric = ".GDAXI", ISIN = "GDAXI", Quality = q },
                new Price { Id = id++, Name = "CAC40 Index", Ric = ".CAC", ISIN = "CAC40", Quality = q },
                new Price { Id = id++, Name = "Vodafone", Ric = "VOD.L", ISIN = "VODL", Quality = q },
                new Price { Id = id++, Name = "Royal Bank of Scotland", Ric = "RBS.L", ISIN = "RBSL", Quality = q },
                new Price { Id = id++, Name = "Adidas", Ric = "ASDGn.DE", ISIN = "ADSGNDE", Quality = q },
                new Price { Id = id++, Name = "Apple", Ric = "AAPL.O", ISIN = "AAPLO", Quality = q },
                new Price { Id = id++, Name = "Google", Ric = "GOOG.O", ISIN = "GOOGO", Quality = q },
                new Price { Id = id++, Name = "Microsoft", Ric = "MSFT.O", ISIN = "MSFTO", Quality = q },
                new Price { Id = id++, Name = "Peugeot", Ric = "PEUG.PA", ISIN = "PEUGPA", Quality = q },
                new Price { Id = id++, Name = "Renault", Ric = "RENA.PA", ISIN = "RENAPA", Quality = q },
                new Price { Id = id++, Name = "Nike", Ric="NIKE.O", ISIN = "NIKEO", Quality = q }
            };

    
        }

        private void PopulateProducts()
        {
            var id = 0;
            products = new List<Product>();
            var expiryDates = new List<DateTime>();
            var currentExpiry = DateTime.Today.AddDays(5);
            while (expiryDates.Count < 100)
            {
                expiryDates.Add(currentExpiry);
                currentExpiry = currentExpiry.AddMonths(1);
            }

            var productTypes = new[] { "Warrant", "Certificate" };
            var rand = new Random((int)DateTime.Now.Ticks / 2);
            foreach (var price in prices)
            {
                foreach (var productType in productTypes)
                {
                    foreach (var expiry in expiryDates)
                    {
                        products.Add(
                            new Product
                            {
                                Id = id,
                                Name = price.Name + " " + productType + " " + expiry.ToShortDateString(),
                                ISIN = "GBRBS" + price.ISIN + id,
                                ExpiryDate = expiry,
                                IssueDate = DateTime.Today.AddDays(-rand.Next(500)),
                                PrimaryLink = "L-" + price.Ric.Replace(".", "-"),
                                Quality = Quality.Good,
                                Underlyer = price
                            });
                    }
                }
            }
        }
    }
}