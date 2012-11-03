using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTDataModel
{
   public class StockData
    {
        //public string serialNo { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string vendorId { get; set; }
        public DateTime purchaseDate { get; set; }
        public double quantityPurchased { get; set; }
        public double rate { get; set; }
        public double vatRate { get; set; }
        public double quantityAvailable { get; set; }
    }
}
