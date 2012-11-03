using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTDataModel
{
 public   class PurchaseData
    {
        //public string serialNo { get; set; }
        public string invoiceNo { get; set; }
        public string vendorId { get; set; }
        public string vendorName { get; set; }
        public double totalAmount { get; set; }
        public double payment { get; set; }
        public DateTime datePurchase { get; set; }
    }
}
