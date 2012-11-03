using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SPTDataModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace CustomerPayments
{
    /// <summary>
    /// Interaction logic for addCustomerPaymentWindow.xaml
    /// </summary>
    public partial class addCustomerPaymentWindow : Window
    {
        CustomerPaymentData customerPaymentData;

        ObservableCollection<CustomersData> _customerCollection = new ObservableCollection<CustomersData>();

        public ObservableCollection<CustomersData> customerCollection
        {
            get
            {
                return _customerCollection;
            }
        }

        public delegate void delegateAddCustomerPaymentData(CustomerPaymentData returnStockData);

        public event delegateAddCustomerPaymentData OnAddCustomerPaymentData;

        public addCustomerPaymentWindow()
        {

            InitializeComponent();

            customerPaymentData = new CustomerPaymentData();

        }

        string GeneratePaymentId()
        {
            return "payment-" + DateTime.Now.ToOADate().ToString();
        }

      

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDataAvailability() == true)
            {
                if (custIdTB.Content != null)
                    customerPaymentData.customerId = custIdTB.Content.ToString();

                #region checking for proper value of payment amount
              
                string errorMsg;
                double paymentAmount;

                if (ValidationHandler.CheckDoubleToStringFormatError(out paymentAmount, paymentAmountTB.Text, " Payment Amount", out errorMsg))
                    customerPaymentData.paymentAmount = paymentAmount;
                else
                {
                    paymentAmountTB.Text = string.Empty;
                    MessageBox.Show(errorMsg);
                    return;
                }

                #endregion

                customerPaymentData.paymentDate = DateTime.Now;
                customerPaymentData.paymentId = GeneratePaymentId();
                customerPaymentData.customerAddress = addressDataTB.Content.ToString();
                customerPaymentData.customerName = nameDataTB.Text;
                customerPaymentData.customerPhone = _customerCollection.Where(item => item.customerId.Equals(nameDataTB.SelectedValue.ToString())).First().phoneNumber;

                if (OnAddCustomerPaymentData != null)
                    OnAddCustomerPaymentData(customerPaymentData);

                SptStorage.DbInteraction.ConnectInsertTocustomerPaymentTable(customerPaymentData);
                SptStorage.DbInteraction.UpdateCustomerTableWithPaymentOnSales(0.0, customerPaymentData.paymentAmount, customerPaymentData.customerId);
                this.Close();
            }
        }

       

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

      

        private void customerNameBtn_Click(object sender, RoutedEventArgs e)
        {
            nameDataTB.IsEnabled = true;
            _customerCollection.Clear();

            List<CustomersData> customers = SptStorage.DbInteraction.FetchCustomersList();

            foreach (CustomersData cus in customers)
            {
                _customerCollection.Add(cus);
            }
        }

        private void NameDataTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (nameDataTB.SelectedIndex != -1)
            {
                custIdTB.Content = nameDataTB.SelectedValue;
                CustomersData tempCust = _customerCollection.Where(item => item.customerId.Equals(nameDataTB.SelectedValue.ToString())).First();
                addressDataTB.Content = tempCust.customerAdress;
            }
        }


        #region Validation

        private bool CheckDataAvailability()
        {
            bool returnVal = true;

            if (string.IsNullOrEmpty(nameDataTB.Text) || string.IsNullOrEmpty(custIdTB.Content.ToString()) || string.IsNullOrEmpty(paymentAmountTB.Text))
            {
                returnVal = false;
                MessageBox.Show("Enter Proper data");

            }
            return returnVal;
        }

        private void paymentAmountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }

        #endregion

    }
}
