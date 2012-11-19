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
using System.ComponentModel;
using System.Collections.ObjectModel;
using SPTDataModel;
using System.Text.RegularExpressions;
using System.Reflection;
using SPTSettings;


namespace PurchaseBilling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PurchaseBillingWindow : Window
    {
        static int billSerialNoCount = 0;

        SettingsData sptSettings;


        public PurchaseBillingWindow()
        {
            InitializeComponent();

            SetBillingInfoFromSPTSettings();
        }

        private void SetBillingInfoFromSPTSettings()
        {
            sptSettings = FetchSPTSettings.FetcheSettingsData();
            customernameTb.Text = sptSettings.Name;
            customerAddressTb.Text += sptSettings.Address;
            customerPhoneTb.Text += "Ph: " + sptSettings.Phone;
            declarationTextBlock.Text = sptSettings.BillDisclaimer;
        }

        ObservableCollection<BillingData> _purchaseBillingCollection = new ObservableCollection<BillingData>();
        ObservableCollection<VendorsData> _vendorCollection = new ObservableCollection<VendorsData>();


        public ObservableCollection<VendorsData> vendorCollection
        {
            get
            {
                return _vendorCollection;
            }
        }


        public ObservableCollection<BillingData> purchaseBillingCollection
        {
            get
            {
                return _purchaseBillingCollection;
            }
        }


        public void SetBillInvoiceNumber(int count)
        {
            invoiceNumberTB.Text = sptSettings.InvoicePrefix + count.ToString("P-0000") + "-" + DateTime.Now.ToOADate().ToString();
            vendorInfoTb.Text += count.ToString("0000");
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    e.Cancel = true;
        //    this.Hide();
        //}




        private void addItemBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewPurchaseItemWindow addNewItem = new AddNewPurchaseItemWindow();

            addNewItem.OnAddNewBillingData += new AddNewPurchaseItemWindow.delegateAddNewBillingData(addNewItem_OnAddNewBillingData);


            billSerialNoCount++;

            addNewItem.ShowDialog();

            addNewItem.OnAddNewBillingData -= new AddNewPurchaseItemWindow.delegateAddNewBillingData(addNewItem_OnAddNewBillingData);
        }

        void addNewItem_OnAddNewBillingData(BillingData returnBillData)
        {
            returnBillData.serialNo = salesBillingItemListView.Items.Count + 1;//billSerialNoCount.ToString();
            double amount = Convert.ToDouble(returnBillData.quantity) * Convert.ToDouble(returnBillData.rate);
            returnBillData.amount = amount;

            double vat = amount * Convert.ToDouble(returnBillData.vat) * (.01);
            returnBillData.calVat = vat;

            _purchaseBillingCollection.Add(returnBillData);
        }

        private void deleteItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (salesBillingItemListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item to delete.");
            else
            {
                int i = salesBillingItemListView.SelectedIndex;

                _purchaseBillingCollection.RemoveAt(i);
            }
        }


        long totalCalulatedAmount = 0;

        double CalculateTotalAmount()
        {
            double total = 0.0;

            foreach (BillingData bill in _purchaseBillingCollection)
            {
                total += Convert.ToDouble(bill.amount);
            }

            return total;
        }

        private void calculateTotalBtn_Click(object sender, RoutedEventArgs e)
        {

            DoFinalCalculation();
        }

        private void DoFinalCalculation()
        {

            totalCalulatedAmount = Convert.ToInt64(CalculateTotalAmount() + CalculateVAT());
            salesTotalAmountLabel.Content = totalCalulatedAmount;
            priceInWordLabel.Content = SPTHelper.NumberToWords(totalCalulatedAmount) + " Only.";

        }

        double totalVat = 0.0;

        double CalculateVAT()
        {
            totalVat = 0.0;

            foreach (BillingData bill in _purchaseBillingCollection)
            {
                totalVat += bill.calVat;
            }

            vatAmount.Content = totalVat.ToString();
            return totalVat;
        }

        private void calculateVATBtn_Click(object sender, RoutedEventArgs e)
        {
            CalculateVAT();
        }


        private void salesCancelBillBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void completeBtn_Click(object sender, RoutedEventArgs e)
        {
            ConnectInsertToPurchaselistTable();
            ConnectInsertToPurchaseBillingTable();
            ConnectInsertToCustomerpaymentTable();

            string venIdKey = vendorInfoTb.SelectedValue.ToString();
            double totalBilledAmount = CalculateTotalAmount() + CalculateVAT();
            double purchasePaymentAmount = double.Parse(PurchaseAmountTB.Text);

            SptStorage.DbInteraction.UpdateVendorTableWithPaymentOnPurchase(totalBilledAmount, purchasePaymentAmount, venIdKey);
            UpdateStockTable();

            this.Close();
        }

        private void cancelBillBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #region Database Interaction

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;



        #region insert into vendor payment table
        string GeneratePaymentId()
        {
            return "payment-" + DateTime.Now.ToOADate().ToString();
        }

        private void ConnectInsertToCustomerpaymentTable()
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
                if (msqlConnection.State != System.Data.ConnectionState.Open)
                    msqlConnection.Open();

                msqlCommand.CommandText = "INSERT INTO vendor_payment(vendor_id,payment_id,payment_date,payment_amount)"
                    + "VALUES (@vendor_id,@payment_id,@payment_date,@payment_amount)";

                msqlCommand.Parameters.AddWithValue("@vendor_id", vendorInfoTb.SelectedValue.ToString());
                msqlCommand.Parameters.AddWithValue("@payment_id", GeneratePaymentId());
                msqlCommand.Parameters.AddWithValue("@payment_date", DateTime.Now);
                msqlCommand.Parameters.AddWithValue("@payment_amount", PurchaseAmountTB.Text);
                msqlCommand.ExecuteNonQuery();
            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + MethodBase.GetCurrentMethod().Name + ":" + er.Message);
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }

        }
        #endregion

        #region insert into purchaselist table
        private void ConnectInsertToPurchaselistTable()
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
                if (msqlConnection.State != System.Data.ConnectionState.Open)
                    msqlConnection.Open();

                msqlCommand.CommandText = "INSERT INTO purchaselist(invoiceNo,vendorId,datePurchase,totalAmount,payment,vendorName)"
                    + "VALUES (@invoiceNo,@vendorId,@datePurchase,@totalAmount,@payment,@vendorName)";

                msqlCommand.Parameters.AddWithValue("@invoiceNo", invoiceNumberTB.Text);
                msqlCommand.Parameters.AddWithValue("@vendorId", vendorInfoTb.SelectedValue.ToString());
                if (SPTHelper.DateEquals((DateTime)purchaseDatePicker.SelectedDate, DateTime.Now))
                    msqlCommand.Parameters.AddWithValue("@datePurchase", DateTime.Now);
                else
                    msqlCommand.Parameters.AddWithValue("@datePurchase", purchaseDatePicker.SelectedDate);
                msqlCommand.Parameters.AddWithValue("@totalAmount", CalculateTotalAmount());
                msqlCommand.Parameters.AddWithValue("@payment", PurchaseAmountTB.Text);
                msqlCommand.Parameters.AddWithValue("@vendorName", vendorInfoTb.Text);

                msqlCommand.ExecuteNonQuery();


            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + MethodBase.GetCurrentMethod().Name + ":" + er.Message);
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }

        }
        #endregion

        #region insert into purchasebilling table
        private void ConnectInsertToPurchaseBillingTable()
        {

            //define the connection reference and initialize it
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");


            try
            {

                foreach (BillingData billData in _purchaseBillingCollection)
                {

                    //define the command reference
                    MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                    //define the connection used by the command object
                    msqlCommand.Connection = msqlConnection;


                    //open the connection
                    if (msqlConnection.State != System.Data.ConnectionState.Open)
                        msqlConnection.Open();

                    FeedBillData(msqlCommand, billData);

                    //always close the connection
                    msqlConnection.Close();
                }

            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + MethodBase.GetCurrentMethod().Name + ":" + er.Message);
            }
            finally
            {
                if (msqlConnection.State == System.Data.ConnectionState.Open)
                    msqlConnection.Close();
            }

        }

        void FeedBillData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, BillingData billData)
        {
            //define the command text
            msqlCommand.CommandText = "INSERT INTO purchaseBilling(product,quantity,vat,rate,amount,invoiceNo,billItemId)"
                + "VALUES (@product, @quantity, @vat, @rate, @amount,@invoiceNo,@billItemId)";

            msqlCommand.Parameters.AddWithValue("@product", billData.productName);
            msqlCommand.Parameters.AddWithValue("@quantity", billData.quantity);
            msqlCommand.Parameters.AddWithValue("@vat", billData.vat);
            msqlCommand.Parameters.AddWithValue("@rate", billData.rate);
            msqlCommand.Parameters.AddWithValue("@billItemId", invoiceNumberTB.Text + "_" + billData.serialNo);
            //   msqlCommand.Parameters.AddWithValue("@serialNo", billData.serialNo);
            msqlCommand.Parameters.AddWithValue("@amount", billData.amount);
            msqlCommand.Parameters.AddWithValue("@invoiceNo", invoiceNumberTB.Text);

            msqlCommand.ExecuteNonQuery();
        }
        #endregion

        #region select vendor name
        private void ShowVendorName()
        {
            MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");

            //define the connection used by the command object
            msqlCommand.Connection = msqlConnection;

            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            CustomersData customersData = new CustomersData();

            msqlCommand.CommandText = "SELECT vendor_name,vendor_id FROM vendors group by vendor_name";

            MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

            _vendorCollection.Clear();

            while (msqlReader.Read())
            {
                VendorsData venData = new VendorsData();
                venData.vendorName = msqlReader.GetString("vendor_name");
                venData.vendorId = msqlReader.GetString("vendor_id");
                _vendorCollection.Add(venData);
            }

            msqlConnection.Close();
        }
        #endregion

        #region stock table update

        private void UpdateStockTable()
        {

            //define the connection reference and initialize it
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");


            try
            {

                foreach (BillingData billData in _purchaseBillingCollection)
                {

                    //define the command reference
                    MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                    //define the connection used by the command object
                    msqlCommand.Connection = msqlConnection;

                    FeedStockData(msqlCommand, billData);
                }

            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + MethodBase.GetCurrentMethod().Name + ":" + er.Message);
            }
            finally
            {
                if (msqlConnection.State == System.Data.ConnectionState.Open)
                    msqlConnection.Close();
            }

        }

        void FeedStockData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, BillingData billData)
        {
            double dbQuantityAvailable = 0;
            double dbQuantityPurchased = 0.0;
            // UPDATE `sptdb`.`stock` SET `quantity_available`='100' WHERE `id`='Prod-40791.0558022338';

            //open the connection
            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            msqlCommand.CommandText = "SELECT quantity_available,quantity_purchased FROM stock WHERE id='" + billData.productId + "';";


            MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();
            while (msqlReader.Read())
            {
                dbQuantityAvailable = int.Parse(msqlReader.GetString("quantity_available"));
                dbQuantityPurchased = double.Parse(msqlReader.GetString("quantity_purchased"));
            }
            if (msqlConnection.State == System.Data.ConnectionState.Open)
                msqlConnection.Close();

            //updating the value
            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            double newQuantityAvailable = dbQuantityAvailable + billData.quantity;
            double newQuantityPurchased = dbQuantityPurchased + billData.quantity;

            msqlCommand.CommandText = "UPDATE stock SET quantity_available = @newQuantity,quantity_purchased = @newQuantityPurchased,rate=@rate, vendor_id=@vendor_id,date_purchased=@date_purchased  WHERE product_name='" + billData.productName + "'; ";
            msqlCommand.Parameters.AddWithValue("@newQuantity", newQuantityAvailable);
            msqlCommand.Parameters.AddWithValue("@newQuantityPurchased", newQuantityPurchased);
            msqlCommand.Parameters.AddWithValue("@rate", billData.rate);
            msqlCommand.Parameters.AddWithValue("@vendor_id", vendorInfoTb.SelectedValue.ToString());
            msqlCommand.Parameters.AddWithValue("@date_purchased", purchaseDatePicker.SelectedDate);
            msqlCommand.ExecuteNonQuery();

            if (msqlConnection.State == System.Data.ConnectionState.Open)
                msqlConnection.Close();
        }


        #endregion


        private void salesVendorSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            vendorInfoTb.IsEnabled = true;
            ShowVendorName();
        }
        #region validation
        
        private void PurchaseAmountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }
        #endregion

        private void printBillBtn_Click(object sender, RoutedEventArgs e)
        {
            DoFinalCalculation();
            Printer.PrintArea(printableBillAreaDockPanel, "Purchase Bill");
        }


    }



}
        #endregion