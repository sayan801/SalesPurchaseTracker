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

namespace OutputVatCalculation
{
    public partial class OutputVatCalcWindow : Window
    {
        ObservableCollection<OpVatData> _outputVatCollection = new ObservableCollection<OpVatData>();

        public ObservableCollection<OpVatData> outputVatCollection
        {
            get
            {
                return _outputVatCollection;
            }
        }
       

        public OutputVatCalcWindow()
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


        private void ConnectFetchFromSaleslistTable()
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {
                
                MySql.Data.MySqlClient.MySqlCommand msqlCommandPL = new MySql.Data.MySqlClient.MySqlCommand();

                
                msqlCommandPL.Connection = msqlConnection;


                
                if (msqlConnection.State != System.Data.ConnectionState.Open)
                    msqlConnection.Open();

                TimeSpan diff = (TimeSpan)(endDatePicker.SelectedDate - startDatePicker.SelectedDate);
                msqlCommandPL.CommandText = "SELECT saleslist.dateSales, saleslist.invoiceNo,saleslist.customerId,salesbilling.quantity,salesbilling.rate, salesbilling.amount, salesbilling.vat,customers.customer_name,customers.vat_no  FROM saleslist,salesbilling,customers WHERE saleslist.invoiceNo=salesbilling.invoiceNo  AND date(saleslist.dateSales) >= DATE_SUB( @enddate, INTERVAL @diff DAY) AND saleslist.customerId = customers.id ;";
                msqlCommandPL.Parameters.AddWithValue("@enddate", endDatePicker.SelectedDate);
                msqlCommandPL.Parameters.AddWithValue("@diff", diff.Days);  

                MySql.Data.MySqlClient.MySqlDataReader msqlReaderPL = msqlCommandPL.ExecuteReader();

                _outputVatCollection.Clear();


                while (msqlReaderPL.Read())
                {
                    OpVatData OPVatData = new OpVatData();
                    OPVatData.date = msqlReaderPL.GetDateTime("dateSales");
                    OPVatData.invoiceNo = msqlReaderPL.GetString("invoiceNo");
                    OPVatData.customerId = msqlReaderPL.GetString("customerId");
                    OPVatData.customerName = msqlReaderPL.GetString("customer_name");
                    OPVatData.customerVatNo = msqlReaderPL.GetString("vat_no");

                    OPVatData.quantity = msqlReaderPL.GetDouble("quantity");
                    OPVatData.pricePerUnit = msqlReaderPL.GetDouble("rate");
                    OPVatData.totalPrice = msqlReaderPL.GetDouble("amount");
                    double totPrice = Convert.ToDouble(OPVatData.totalPrice);

                    OPVatData.vatRate = msqlReaderPL.GetDouble("vat");
                    double vatRat = Convert.ToDouble(OPVatData.vatRate);

                    double vatTot = (totPrice / 100) * vatRat;
                    OPVatData.vatTotal = vatTot;//msqlReaderPL.GetString("vat");
                    OPVatData.totalAmount = totPrice + vatTot;
                    _outputVatCollection.Add(OPVatData);
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

        private void OpVatShowBtn_Click(object sender, RoutedEventArgs e)
        {
            ConnectFetchFromSaleslistTable();
        }

        private void calculateTotalVATBtn_Click(object sender, RoutedEventArgs e)
        {
            double totalVat = 0.0;
            foreach (OpVatData vat in outputVatCollection)
            {
                totalVat += Convert.ToDouble(vat.vatTotal);
            }
            vatTotalAmount.Content = totalVat.ToString();
        }

        private void calculateTotalAmountBtn_Click(object sender, RoutedEventArgs e)
        {
            double totalPrice = 0.0;
            foreach (OpVatData vat in outputVatCollection)
            {
                totalPrice += Convert.ToDouble(vat.totalAmount);
            }
            priceTotalAmount.Content = totalPrice.ToString();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(OpVatListView, "Output VAT List");
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveXlsBtn_Click(object sender, RoutedEventArgs e)
        {
            Export2xlsx.SaveToExcel(outputVatCollection);
        }

    }
}
