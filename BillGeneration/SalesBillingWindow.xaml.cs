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
using SPTSettings;


namespace SalesBillGeneration
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SalesBillingWindow : Window
    {
        static int billSerialNoCount = 0;

        SettingsData sptSettings;

                   

        public SalesBillingWindow()
        {
            InitializeComponent();

            SetSalesBillingInfoFromSPTSettings();
        }

        private void SetSalesBillingInfoFromSPTSettings()
        {
            sptSettings = FetchSPTSettings.FetcheSettingsData();
            sellernameTb.Text = sptSettings.Name;
            sellerAddressTb.Text += sptSettings.Address;
            sellerPhoneTb.Text += "Ph: " + sptSettings.Phone;
            declarationTextBlock.Text = sptSettings.BillDisclaimer;
        }


        ObservableCollection<BillingData> _billingCollection = new ObservableCollection<BillingData>();
        ObservableCollection<CustomersData> _customerCollection = new ObservableCollection<CustomersData>();


        public ObservableCollection<CustomersData> customerCollection
        {
            get
            {
                return _customerCollection;
            }
        }


        public ObservableCollection<BillingData> billingCollection
        {
            get
            {
                return _billingCollection;
            }
        }


        public void SetBillInvoiceNumber(int count)
        {
            invoiceNumberTB.Text = sptSettings.InvoicePrefix + count.ToString("0000") + "-" + DateTime.Now.ToOADate().ToString();
            customerInfoTb.Text += count.ToString("0000");
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    e.Cancel = true;
        //    this.Hide();
        //}

        private void addItemBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewSalesItemWindow addNewItem = new AddNewSalesItemWindow();

            addNewItem.OnAddNewBillingData += new AddNewSalesItemWindow.delegateAddNewBillingData(addNewItem_OnAddNewBillingData);


            billSerialNoCount++;

            addNewItem.ShowDialog();

            addNewItem.OnAddNewBillingData -= new AddNewSalesItemWindow.delegateAddNewBillingData(addNewItem_OnAddNewBillingData);
        }

        void addNewItem_OnAddNewBillingData(BillingData returnBillData)
        {
            double amount = Convert.ToDouble(returnBillData.quantity) * Convert.ToDouble(returnBillData.rate);
            returnBillData.amount = amount;

            double vat = amount * Convert.ToDouble(returnBillData.vat) * (.01);
            returnBillData.calVat = vat;

            returnBillData.serialNo = billingItemListView.Items.Count + 1;

            _billingCollection.Add(returnBillData);
        }

        private void deleteItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (billingItemListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item to delete.");
            else
            {
                int i = billingItemListView.SelectedIndex;

                _billingCollection.RemoveAt(i);
            }
        }

        long totalCalulatedAmount = 0;

        double CalculateTotalAmount()
        {
            double total = 0.0;

            foreach (BillingData bill in _billingCollection)
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
            totalAmountLabel.Content = totalCalulatedAmount;
            priceInWordLabel.Content = SPTHelper.NumberToWords(totalCalulatedAmount) + " Only.";
        }

        double totalVat = 0.0;

        double CalculateVAT()
        {
            totalVat = 0.0;

            foreach (BillingData bill in _billingCollection)
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

        private void customerSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            customerInfoTb.IsEnabled = true;
            ShowCustomerName();
        }

        private void completeBtn_Click(object sender, RoutedEventArgs e)
        {
            //update saleslist
            ConnectInsertToSaleslistTable();

            //update salesbilling
            ConnectInsertToSalesBillingTable();

            //update stock
            UpdateStockTable();

            //update customer payment
            ConnectInsertToCustomerpaymentTable();

            //update customer
            double totalBilledAmount = CalculateTotalAmount() + CalculateVAT();
            double paymentAmount = double.Parse(paymentAmountTB.Text);
            string cusIdKey = customerInfoTb.SelectedValue.ToString();
            SptStorage.DbInteraction.UpdateCustomerTableWithPaymentOnSales(totalBilledAmount, paymentAmount,cusIdKey);

            this.Close();
        }

        private void cancelBillBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #region Database Interaction

        #region customer payement table update

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

                msqlCommand.CommandText = "INSERT INTO customer_payment(customer_id,payment_id,payment_date,payment_amount)"
                    + "VALUES (@customer_id,@payment_id,@payment_date,@payment_amount)";

                msqlCommand.Parameters.AddWithValue("@customer_id", customerInfoTb.SelectedValue.ToString());
                msqlCommand.Parameters.AddWithValue("@payment_id", GeneratePaymentId());
                msqlCommand.Parameters.AddWithValue("@payment_date", DateTime.Now);
                msqlCommand.Parameters.AddWithValue("@payment_amount", paymentAmountTB.Text);
                msqlCommand.ExecuteNonQuery();


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

        #region stock table update

        private void UpdateStockTable()
        {

            //define the connection reference and initialize it
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");


            try
            {

                foreach (BillingData billData in _billingCollection)
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
                MessageBox.Show(er.Message);
            }
            finally
            {
                if (msqlConnection.State == System.Data.ConnectionState.Open)
                    msqlConnection.Close();
            }

        }

        void FeedStockData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, BillingData billData)
        {
            int dbQuantity = 0;
        
            //open the connection
            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            msqlCommand.CommandText = "SELECT quantity_available FROM stock WHERE id='" + billData.productId + "';";


            MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();
            while (msqlReader.Read())
            {
                dbQuantity = int.Parse(msqlReader.GetString("quantity_available"));
            }
            if (msqlConnection.State == System.Data.ConnectionState.Open)
                msqlConnection.Close();

            //updating the value
            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            String newQuantity = (dbQuantity - billData.quantity).ToString();
            msqlCommand.CommandText = "UPDATE stock SET quantity_available='" + newQuantity + "' WHERE id='" + billData.productId + "'; ";

            msqlCommand.ExecuteNonQuery();

            if (msqlConnection.State == System.Data.ConnectionState.Open)
                msqlConnection.Close();
        }

        #endregion
                
        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void ConnectInsertToSaleslistTable()
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

                msqlCommand.CommandText = "INSERT INTO saleslist(invoiceNo,customerId,customerName,dateSales,totalAmount,payment)"
                    + "VALUES (@invoiceNo,@customerId,@customerName,@dateSales,@totalAmount,@payment)";

                msqlCommand.Parameters.AddWithValue("@invoiceNo", invoiceNumberTB.Text);
                msqlCommand.Parameters.AddWithValue("@customerId", customerInfoTb.SelectedValue.ToString());
                msqlCommand.Parameters.AddWithValue("@customerName", customerInfoTb.Text);

                if ( SPTHelper.DateEquals( (DateTime)datePicker.SelectedDate, DateTime.Now))
                    msqlCommand.Parameters.AddWithValue("@dateSales", DateTime.Now);
                else
                    msqlCommand.Parameters.AddWithValue("@dateSales", datePicker.SelectedDate);

                msqlCommand.Parameters.AddWithValue("@totalAmount", CalculateTotalAmount());
                msqlCommand.Parameters.AddWithValue("@payment", paymentAmountTB.Text);

                msqlCommand.ExecuteNonQuery();


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

        private void ConnectInsertToSalesBillingTable()
        {

            //define the connection reference and initialize it
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");


            try
            {

                foreach (BillingData billData in _billingCollection)
                {

                    //define the command reference
                    MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                    //define the connection used by the command object
                    msqlCommand.Connection = msqlConnection;


                    //open the connection
                    if (msqlConnection.State != System.Data.ConnectionState.Open)
                        msqlConnection.Open();

                    FeedBillData(msqlCommand, billData);



                }

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

        void FeedBillData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, BillingData billData)
        {
            //define the command text
            msqlCommand.CommandText = "INSERT INTO salesBilling(description,quantity,vat,rate,amount,invoiceNo,billItemId)"
                + "VALUES (@description, @quantity, @vat, @rate, @amount,@invoiceNo,@billItemId)";

            msqlCommand.Parameters.AddWithValue("@description", billData.productName);
            msqlCommand.Parameters.AddWithValue("@quantity", billData.quantity);
            msqlCommand.Parameters.AddWithValue("@vat", billData.vat);
            msqlCommand.Parameters.AddWithValue("@rate", billData.rate);
            msqlCommand.Parameters.AddWithValue("@billItemId", invoiceNumberTB.Text + "_" + billData.serialNo);
            //  msqlCommand.Parameters.AddWithValue("@serialNo", billData.serialNo);
            msqlCommand.Parameters.AddWithValue("@amount", billData.amount);
            msqlCommand.Parameters.AddWithValue("@invoiceNo", invoiceNumberTB.Text);

            msqlCommand.ExecuteNonQuery();
        }

        private void ShowCustomerName()
        {
            MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            //define the connection used by the command object
            msqlCommand.Connection = msqlConnection;

            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            msqlCommand.CommandText = "SELECT customer_name,id FROM customers";

            MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

            _customerCollection.Clear();

            while (msqlReader.Read())
            {
                CustomersData cusData = new CustomersData();
                cusData.customerName = msqlReader.GetString("customer_name");
                cusData.customerId = msqlReader.GetString("id");
                _customerCollection.Add(cusData);
            }

            msqlConnection.Close();
        }



        #endregion



        #region validation       

        private void paymentAmountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }

        #endregion

        private void printBillBtn_Click(object sender, RoutedEventArgs e)
        {
            DoFinalCalculation();
            Printer.PrintArea(printableBillAreaDockPanel, "Sales Bill");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }




}



