using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTDataModel
{
    public class CustomerPaymentData
    {

        //public string serialNo { get; set; }
        public string customerId { get; set; } 
        public string customerName { get; set; }
        public string customerAddress { get; set; }
        public string customerPhone { get; set; }
        public string paymentId { get; set; } //table
        public DateTime paymentDate { get; set; } //table
        public double paymentAmount { get; set; } //table
    }
}
