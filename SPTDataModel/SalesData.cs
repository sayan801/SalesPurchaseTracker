using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesReport
{
    public class SalesData
    {
       // public string serialNo { get; set; }
        public string invoiceNo { get; set; }
        public string customerId { get; set; }
        public double totalAmount { get; set; }
        public double payment { get; set; }
        public DateTime dateSales { get; set; }
        public string customerName { get; set; }
    }
}
