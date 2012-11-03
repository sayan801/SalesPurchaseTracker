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

namespace SalesReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SalesReportWindow : Window
    {
        ObservableCollection<SalesData> _salesDataCollection = new ObservableCollection<SalesData>();

        public ObservableCollection<SalesData> salesDataCollection
        {
            get
            {
                return _salesDataCollection;
            }
        }


        public SalesReportWindow()
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


        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(salesItemListView, "Sales List");
        }

        private void showItemsBtn_Click(object sender, RoutedEventArgs e)
        {
            ConnectFetchFromSaleslistTable();
        }

        #region Database Interaction

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void ConnectFetchFromSaleslistTable()
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
                
                TimeSpan diff = (TimeSpan)(endDatePicker.SelectedDate - startDatePicker.SelectedDate);
                msqlCommand.CommandText = "SELECT * FROM saleslist where date(saleslist.dateSales) >= DATE_SUB( @enddate, INTERVAL @diff DAY);";
                msqlCommand.Parameters.AddWithValue("@enddate", endDatePicker.SelectedDate);
                msqlCommand.Parameters.AddWithValue("@diff", diff.Days);  

                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

                _salesDataCollection.Clear();

                while (msqlReader.Read())
                {
                    SalesData salesData = new SalesData();
                    salesData.customerId = msqlReader.GetString("customerId");
                    salesData.customerName = msqlReader.GetString("customerName"); //add
                    salesData.dateSales = msqlReader.GetDateTime("dateSales");
                    salesData.invoiceNo = msqlReader.GetString("invoiceNo");
                    salesData.payment = msqlReader.GetDouble("payment");
                    salesData.totalAmount = msqlReader.GetDouble("totalAmount");
                    //salesData.serialNo = (_salesDataCollection.Count + 1).ToString();

                    _salesDataCollection.Add(salesData);
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

      

        private SalesData GetSelectedItem()
        {

            SalesData salesDataToEdit = null;

            if (salesItemListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item");
            else
            {
                SalesData i = (SalesData)salesItemListView.SelectedItem;

                salesDataToEdit = _salesDataCollection.Where(item => item.invoiceNo.Equals(i.invoiceNo)).First();
            }

            return salesDataToEdit;
        }

        private void deleteItemBtn_Click(object sender, RoutedEventArgs e)
        {
            SalesData salesReportToDelete = GetSelectedItem();
            if (salesReportToDelete != null)
            {
                _salesDataCollection.Remove(salesReportToDelete);
                DeleteSalesReport(salesReportToDelete.invoiceNo);
            }
        }

        #region database interaction delete from customers

        private void DeleteSalesReport(string salesReportToDelete)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "DELETE FROM saleslist WHERE invoiceNo= @vendorIdToDelete";
                msqlCommand.Parameters.AddWithValue("@vendorIdToDelete", salesReportToDelete);

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

        private void salesItemListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (salesItemListView.SelectedIndex != -1)
            {
                SalesData salesData = _salesDataCollection.ElementAt(salesItemListView.SelectedIndex);
                BillDetailsWindow billDeatils = new BillDetailsWindow(salesData);
                billDeatils.ShowDialog();
            }
        }
    }
}
