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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SPTDataModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CustomerPayments
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CustomerPaymentsWindow : Window
    {
        ObservableCollection<CustomerPaymentData> _customerPaymentCollection = new ObservableCollection<CustomerPaymentData>();
        static int customerCount = 0;

        public ObservableCollection<CustomerPaymentData> customerPaymentCollection
        {
            get
            {
                return _customerPaymentCollection;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            this.Hide();
        }

        public CustomerPaymentsWindow()
        {
            InitializeComponent();
        }


        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            addCustomerPaymentWindow addCustomerPaymentWindowObject = new addCustomerPaymentWindow();
            addCustomerPaymentWindowObject.OnAddCustomerPaymentData += new addCustomerPaymentWindow.delegateAddCustomerPaymentData(addCustomerPaymentWindowObject_OnAddCustomerPaymentData);
            addCustomerPaymentWindowObject.ShowDialog();
        }

        void addCustomerPaymentWindowObject_OnAddCustomerPaymentData(CustomerPaymentData addCustomerPaymentWindowObject)
        {
            //addCustomerPaymentWindowObject.serialNo = (_customerPaymentCollection.Count + 1).ToString();
            _customerPaymentCollection.Add(addCustomerPaymentWindowObject);
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            CustomerPaymentData customerPaymentToDelete = GetSelectedItem();
            if (customerPaymentToDelete != null)
            {
                _customerPaymentCollection.Remove(customerPaymentToDelete);
                SptStorage.DbInteraction.DeleteCustomerPayment(customerPaymentToDelete.paymentId);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshCustomerPaymentsCollection();
        }

        private void RefreshCustomerPaymentsCollection()
        {
            List<CustomerPaymentData> cusPayments = SptStorage.DbInteraction.FetchCustomerPaymentData();

            _customerPaymentCollection.Clear();

            foreach (CustomerPaymentData cusPay in cusPayments)
            {
                _customerPaymentCollection.Add(cusPay);
            }
        }

        private void showPaymentsBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshCustomerPaymentsCollection();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(customerPaymentsListView, "Customer Payments List");
        }

        private CustomerPaymentData GetSelectedItem()
        {

            CustomerPaymentData customerPaymentsToEdit = null;

            if (customerPaymentsListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item");
            else
            {
                CustomerPaymentData i = (CustomerPaymentData)customerPaymentsListView.SelectedItem;

                customerPaymentsToEdit = _customerPaymentCollection.Where(item => item.paymentId.Equals(i.paymentId)).First();
            }

            return customerPaymentsToEdit;
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            CustomerPaymentData customerPaymentsToEdit = GetSelectedItem();
            if (customerPaymentsToEdit != null)
            {
                EditCustomerPaymentsWindow editWindow = new EditCustomerPaymentsWindow(customerPaymentsToEdit);
                editWindow.OnEditCustomersPaymentData += new EditCustomerPaymentsWindow.delegateEditCustomrerPaymentData(editWindow_OnEditVendorsData);
                editWindow.ShowDialog();
            }
        }

        void editWindow_OnEditVendorsData(CustomerPaymentData returnEditedCusPaymentData)
        {
            //finding the element
            CustomerPaymentData cData = _customerPaymentCollection.Where(item => item.paymentId.Equals(returnEditedCusPaymentData.paymentId)).First();
            //finding the element position
            int itemIdex = _customerPaymentCollection.IndexOf(cData);
            //remove the element so that the list gets refreshed
            _customerPaymentCollection.RemoveAt(itemIdex);
            //insert the edited element at same position
            _customerPaymentCollection.Insert(itemIdex, returnEditedCusPaymentData);
        }

    }
}
