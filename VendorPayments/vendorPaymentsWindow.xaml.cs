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
using System.Collections.ObjectModel;
using SPTDataModel;
using System.ComponentModel;

namespace VendorPayments
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class VendorPaymentsWindow : Window
    {
        ObservableCollection<VendorPaymentData> _vendorPaymentCollection = new ObservableCollection<VendorPaymentData>();
        //static int vendorCount = 0;

        public ObservableCollection<VendorPaymentData> vendorPaymentCollection
        {
            get
            {
                return _vendorPaymentCollection;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            this.Hide();
        }

        public VendorPaymentsWindow()
        {
            InitializeComponent();
        }


        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewVendorPaymentWindow addVendorPaymentWindowObject = new AddNewVendorPaymentWindow();
            addVendorPaymentWindowObject.OnAddVendorPaymentData += new AddNewVendorPaymentWindow.delegateAddVendorPaymentData(addVendorPaymentWindowObject_OnAddVendorPaymentData);
            addVendorPaymentWindowObject.ShowDialog();
        }

        void addVendorPaymentWindowObject_OnAddVendorPaymentData(VendorPaymentData addVendorPaymentWindowObject)
        {
            addVendorPaymentWindowObject.serialNo = (_vendorPaymentCollection.Count + 1).ToString();
            _vendorPaymentCollection.Add(addVendorPaymentWindowObject);
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            VendorPaymentData vendorPaymentToDelete = GetSelectedItem();
            if (vendorPaymentToDelete != null)
            {
                _vendorPaymentCollection.Remove(vendorPaymentToDelete);
                DeleteStock(vendorPaymentToDelete.paymentId);
            }
        }

        #region database interaction delete from vendorpayments

        private void DeleteStock(string vendorPaymentToDelete)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "DELETE FROM vendor_payment WHERE payment_id= @vendorIdToDelete";
                msqlCommand.Parameters.AddWithValue("@vendorIdToDelete", vendorPaymentToDelete);

                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }

        }

        #endregion

        #region database interaction data fetching
        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void FetchVendorPaymentData()
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "Select vendors.vendor_id, vendors.vendor_address, vendors.ph_no,vendors.vendor_name, vendor_payment.payment_id, vendor_payment.payment_amount, vendor_payment.payment_date FROM vendors,vendor_payment WHERE vendor_payment.vendor_id = vendors.vendor_id;";

                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();
                _vendorPaymentCollection.Clear();
                while (msqlReader.Read())
                {
                    VendorPaymentData addVendorPaymentWindowObject = new VendorPaymentData();

                    addVendorPaymentWindowObject.serialNo = (_vendorPaymentCollection.Count + 1).ToString();
                    addVendorPaymentWindowObject.vendorId = msqlReader.GetString("vendor_id");
                    addVendorPaymentWindowObject.paymentId = msqlReader.GetString("payment_id");
                    addVendorPaymentWindowObject.vendorName = msqlReader.GetString("vendor_name");
                    addVendorPaymentWindowObject.vendorAddress = msqlReader.GetString("vendor_address");
                    addVendorPaymentWindowObject.vendorPhone = msqlReader.GetString("ph_no");
                    addVendorPaymentWindowObject.paymentAmount = msqlReader.GetDouble("payment_amount");
                    addVendorPaymentWindowObject.paymentDate = msqlReader.GetDateTime("payment_date");
                    _vendorPaymentCollection.Add(addVendorPaymentWindowObject);
                }

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FetchVendorPaymentData();
        }

        private VendorPaymentData GetSelectedItem()
        {

            VendorPaymentData vendorPaymentsToEdit = null;

            if (vendorPaymentsListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item");
            else
            {
                VendorPaymentData i = (VendorPaymentData)vendorPaymentsListView.SelectedItem;

                vendorPaymentsToEdit = _vendorPaymentCollection.Where(item => item.paymentId.Equals(i.paymentId)).First();
            }

            return vendorPaymentsToEdit;
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            VendorPaymentData vendorPaymentsToEdit = GetSelectedItem();
            if (vendorPaymentsToEdit != null)
            {
                EditVendorPaymentsWindow editWindow = new EditVendorPaymentsWindow(vendorPaymentsToEdit);
                editWindow.OnEditVendorPaymentData += new EditVendorPaymentsWindow.delegateEditVendorPaymentData(editWindow_OnEditVendorPaymentData);
                editWindow.ShowDialog();
            }
        }

        void editWindow_OnEditVendorPaymentData(VendorPaymentData returnEditedVendorPaymentData)
        {
            //finding the element
            VendorPaymentData vData = _vendorPaymentCollection.Where(item => item.paymentId.Equals(returnEditedVendorPaymentData.paymentId)).First();
            //finding the element position
            int itemIdex = _vendorPaymentCollection.IndexOf(vData);
            //remove the element so that the list gets refreshed
            _vendorPaymentCollection.RemoveAt(itemIdex);
            //insert the edited element at same position
            _vendorPaymentCollection.Insert(itemIdex, returnEditedVendorPaymentData);

        }

        private void showPaymentsBtn_Click(object sender, RoutedEventArgs e)
        {
            FetchVendorPaymentData();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(vendorPaymentsListView, "Vendor Payment List");
        }
    }
}
