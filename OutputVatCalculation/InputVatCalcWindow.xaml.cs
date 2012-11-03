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
using SPTDataModel;
using System.Collections.ObjectModel;

namespace InputVatCalculation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class InputVatCalcWindow : Window
    {
        ObservableCollection<IpVatData> _inputVatCollection = new ObservableCollection<IpVatData>();

        public ObservableCollection<IpVatData> inputVatCollection
        {
            get
            {
                return _inputVatCollection;
            }
        }
        
        public InputVatCalcWindow()
        {
            InitializeComponent();

            SetStartEndDate();
        }

        public void SetStartEndDate()
        {
            DateTime startDate = DateTime.Now.Subtract(TimeSpan.FromDays(90));
            startDatePicker.SelectedDate = startDate;
            startDatePicker.DisplayDate = startDate;

            endDatePicker.SelectedDate = DateTime.Now;
            endDatePicker.SelectedDate = DateTime.Now;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            this.Hide();
        }

        #region Database Interaction

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;


        private void ConnectFetchFromPurchaselistTable()
        {

            //define the connection reference and initialize it
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            //msqlConnectionPB = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");

            try
            {
                //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommandPL = new MySql.Data.MySqlClient.MySqlCommand();

                //define the connection used by the command object
                msqlCommandPL.Connection = msqlConnection;
                
                //open the connection
                msqlConnection.Open();

               
                TimeSpan diff = (TimeSpan)(endDatePicker.SelectedDate - startDatePicker.SelectedDate);
                msqlCommandPL.CommandText = "SELECT purchaselist.datePurchase, purchaselist.invoiceNo,purchaselist.vendorId,purchasebilling.quantity,purchasebilling.rate, purchasebilling.amount, purchasebilling.vat,vendors.vendor_name,vendors.vat_no  FROM purchaselist,purchasebilling,vendors WHERE purchaselist.invoiceNo=purchasebilling.invoiceNo  AND date(purchaselist.datePurchase) >= DATE_SUB( @enddate, INTERVAL @diff DAY) AND purchaselist.vendorId = vendors.vendor_id ;";
                msqlCommandPL.Parameters.AddWithValue("@enddate", endDatePicker.SelectedDate);
                msqlCommandPL.Parameters.AddWithValue("@diff", diff.Days);
               
                MySql.Data.MySqlClient.MySqlDataReader msqlReaderPL = msqlCommandPL.ExecuteReader();

                _inputVatCollection.Clear();
                
                while (msqlReaderPL.Read())
                {
                    IpVatData ipVatData = new IpVatData();
                    ipVatData.date = msqlReaderPL.GetDateTime("datePurchase");
                    ipVatData.invoiceNo = msqlReaderPL.GetString("invoiceNo");
                    ipVatData.vendorName = msqlReaderPL.GetString("vendor_name");
                    ipVatData.vendorVatNo = msqlReaderPL.GetString("vat_no");
                    ipVatData.vendorId = msqlReaderPL.GetString("vendorId");
                    ipVatData.quantity = msqlReaderPL.GetDouble("quantity");
                    ipVatData.pricePerUnit = msqlReaderPL.GetDouble("rate");
                    ipVatData.totalPrice = msqlReaderPL.GetDouble("amount");

                    double totPrice = Convert.ToDouble(ipVatData.totalPrice);

                    ipVatData.vatRate = msqlReaderPL.GetDouble("vat");
                    double vatRat = Convert.ToDouble(ipVatData.vatRate);

                    double vatTot = (totPrice / 100) * vatRat;
                    ipVatData.vatTotal = vatTot;
                    ipVatData.totalAmount = totPrice + vatTot;
                    _inputVatCollection.Add(ipVatData);
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

        private void IpVatShowBtn_Click(object sender, RoutedEventArgs e)
        {
            ConnectFetchFromPurchaselistTable();
        }

        private void calculateTotalVATBtn_Click(object sender, RoutedEventArgs e)
        {
            double totalVat = 0.0;
            foreach (IpVatData vat in inputVatCollection)
            {
                totalVat += Convert.ToDouble(vat.vatTotal);
            }
            vatTotalAmount.Content = totalVat.ToString();
        }

        private void calculateTotalAmountBtn_Click(object sender, RoutedEventArgs e)
        {
            double totalPrice = 0.0;
            foreach (IpVatData vat in inputVatCollection)
            {
                totalPrice += Convert.ToDouble(vat.totalAmount);
            }
            priceTotalAmount.Content = totalPrice.ToString();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(InputvatListView, "Input VAT List");
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveXlsBtn_Click(object sender, RoutedEventArgs e)
        {
            Export2xlsx.SaveToExcel(inputVatCollection);
        }
       
    }
}
