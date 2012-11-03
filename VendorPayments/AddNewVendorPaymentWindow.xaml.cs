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
using System.Collections.ObjectModel;
using SPTDataModel;


namespace VendorPayments
{
    /// <summary>
    /// Interaction logic for AddNewVendorPaymentWindow.xaml
    /// </summary>
    
    public partial class AddNewVendorPaymentWindow : Window
    {
        VendorPaymentData vendorPaymentData;

        ObservableCollection<VendorsData> _vendorCollection = new ObservableCollection<VendorsData>();

        public ObservableCollection<VendorsData> vendorCollection
        {
            get
            {
                return _vendorCollection;
            }
        }

        public delegate void delegateAddVendorPaymentData(VendorPaymentData returnStockData);

        public event delegateAddVendorPaymentData OnAddVendorPaymentData;

        public AddNewVendorPaymentWindow()
        {

            InitializeComponent();

            vendorPaymentData = new VendorPaymentData();

        }

        string GeneratePaymentId()
        {
            return "payment-" + DateTime.Now.ToOADate().ToString();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (custIdTB.Content != null)
                vendorPaymentData.vendorId = custIdTB.Content.ToString();
            vendorPaymentData.paymentAmount =  double.Parse(paymentAmountTB.Text);
            vendorPaymentData.paymentDate = DateTime.Now;
            vendorPaymentData.paymentId = GeneratePaymentId();
            vendorPaymentData.vendorAddress = addressDataTB.Content.ToString();
            vendorPaymentData.vendorName = nameDataTB.Text;
            vendorPaymentData.vendorPhone = _vendorCollection.Where(item => item.vendorId.Equals(nameDataTB.SelectedValue.ToString())).First().phoneNumber;

            if (OnAddVendorPaymentData != null)
                OnAddVendorPaymentData(vendorPaymentData);

            ConnectInsertTovendorPaymentTable(vendorPaymentData);
            SptStorage.DbInteraction.UpdateVendorTableWithPaymentOnPurchase(0, vendorPaymentData.paymentAmount, vendorPaymentData.vendorId);

            this.Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region database  insertion

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void ConnectInsertTovendorPaymentTable(VendorPaymentData vendorDataObject)
        {

            //define the connection reference and initialize it
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");


            try
            {
                //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                //define the connection used by the command object
                msqlCommand.Connection = msqlConnection;

                //open the connection
                msqlConnection.Open();

                FeedvendorData(msqlCommand, vendorDataObject);

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            finally
            {
                msqlConnection.Close();

            }

        }

        void FeedvendorData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, VendorPaymentData cusomerDataObject)
        {
            //define the command text
            msqlCommand.CommandText = "INSERT INTO vendor_payment(vendor_id,payment_amount,payment_date,payment_id)"
                + "VALUES (@vendor_id,@payment_amount,@payment_date,@payment_id)";

            msqlCommand.Parameters.AddWithValue("@vendor_id", cusomerDataObject.vendorId);
            msqlCommand.Parameters.AddWithValue("@payment_id", cusomerDataObject.paymentId);
            msqlCommand.Parameters.AddWithValue("@payment_amount", cusomerDataObject.paymentAmount);
            msqlCommand.Parameters.AddWithValue("@payment_date", cusomerDataObject.paymentDate);

            msqlCommand.ExecuteNonQuery();
        }
        #endregion

        #region Database Interaction combobox data fetchning


        private void ShowVendorsList()
        {
            MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            //define the connection used by the command object
            msqlCommand.Connection = msqlConnection;

            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            msqlCommand.CommandText = "SELECT * FROM vendors";
            MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

            _vendorCollection.Clear();

            while (msqlReader.Read())
            {
                VendorsData cusData = new VendorsData();
                cusData.vendorName = msqlReader.GetString("vendor_name");
                cusData.vendorId = msqlReader.GetString("vendor_id");
                cusData.vendorAdress = msqlReader.GetString("vendor_address");
                cusData.phoneNumber = msqlReader.GetString("ph_no");

                _vendorCollection.Add(cusData);

            }

            msqlConnection.Close();
        }


        #endregion

        private void vendorNameBtn_Click(object sender, RoutedEventArgs e)
        {
            nameDataTB.IsEnabled = true;
            ShowVendorsList();
        }

        private void NameDataTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (nameDataTB.SelectedIndex != -1)
            {
                custIdTB.Content = nameDataTB.SelectedValue;
                VendorsData tempCust = _vendorCollection.Where(item => item.vendorId.Equals(nameDataTB.SelectedValue.ToString())).First();
                addressDataTB.Content = tempCust.vendorAdress;
            }
        }

    }
}
