using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTDataModel
{
    public class VendorPaymentData
    {

        public string serialNo { get; set; }
        public string vendorId { get; set; } 
        public string vendorName { get; set; }
        public string vendorAddress { get; set; }
        public string vendorPhone { get; set; }
        public string paymentId { get; set; } //table
        public DateTime paymentDate { get; set; } //table
        public double paymentAmount { get; set; } //table

    }
}
