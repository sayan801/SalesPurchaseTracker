using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTDataModel
{
    public class BillingData
    {
        public string productName { get; set; }
        public string productId { get; set; }
        public double quantity { get; set; }
        public double vat { get; set; }
        public double rate { get; set; }
        public int serialNo { get; set; }
        public double amount { get; set; }
        public double calVat { get; set; }
    }
}
