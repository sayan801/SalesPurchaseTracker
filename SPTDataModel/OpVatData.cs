using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTDataModel
{
    public class OpVatData
    {
    
        public DateTime date { get; set; }
        public string invoiceNo { get; set; }
        public string customerId { get; set; }
        public string customerName { get; set; }
        public string customerVatNo { get; set; }
        public double quantity { get; set; }
        public double pricePerUnit { get; set; }
        public double totalPrice { get; set; }
        public double vatRate { get; set; }
        public double vatTotal { get; set; }
        public double totalAmount { get; set; }
    }

}
