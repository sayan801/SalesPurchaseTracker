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

namespace PurchaseReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PurchaseReportWindow : Window
    {
        ObservableCollection<PurchaseData> _purchaseDataCollection = new ObservableCollection<PurchaseData>();

        public ObservableCollection<PurchaseData> purchaseDataCollection
        {
            get
            {
                return _purchaseDataCollection;
            }
        }



        public PurchaseReportWindow()
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

              

        private void showItemsBtn_Click(object sender, RoutedEventArgs e)
        {
            ConnectFetchFromSaleslistTable();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(purchaseItemListView, "Purchase Item List");
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
                msqlConnection.Open();
                TimeSpan diff = (TimeSpan)(endDatePicker.SelectedDate - startDatePicker.SelectedDate);
                msqlCommand.CommandText = "SELECT * FROM purchaselist where date(purchaselist.datePurchase) >= DATE_SUB( @enddate, INTERVAL @diff DAY);";
                msqlCommand.Parameters.AddWithValue("@enddate", endDatePicker.SelectedDate);
                msqlCommand.Parameters.AddWithValue("@diff", diff.Days);
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();
                _purchaseDataCollection.Clear();

                while (msqlReader.Read())
                {
                    PurchaseData purchaseData = new PurchaseData();
                    purchaseData.vendorId = msqlReader.GetString("vendorId");
                    purchaseData.vendorName = msqlReader.GetString("vendorName");
                    purchaseData.datePurchase = msqlReader.GetDateTime("datePurchase");
                    purchaseData.invoiceNo = msqlReader.GetString("invoiceNo");
                    purchaseData.payment = msqlReader.GetDouble("payment");
                    purchaseData.totalAmount = msqlReader.GetDouble("totalAmount");
                    //purchaseData.serialNo = (_purchaseDataCollection.Count + 1).ToString();

                    _purchaseDataCollection.Add(purchaseData);
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

        private PurchaseData GetSelectedItem()
        {

            PurchaseData purchaseDataToEdit = null;

            if (purchaseItemListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item");
            else
            {
                PurchaseData i = (PurchaseData)purchaseItemListView.SelectedItem;

                purchaseDataToEdit = _purchaseDataCollection.Where(item => item.invoiceNo.Equals(i.invoiceNo)).First();
            }

            return purchaseDataToEdit;
        }

        private void deleteItemBtn_Click(object sender, RoutedEventArgs e)
        {
            PurchaseData purchaseReportToDelete = GetSelectedItem();
            if (purchaseReportToDelete != null)
            {
                _purchaseDataCollection.Remove(purchaseReportToDelete);
                DeletePurchaseReport(purchaseReportToDelete.invoiceNo);
            }
        }

        #region database interaction delete from purchaselist

        private void DeletePurchaseReport(string purchaseReportToDelete)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "DELETE FROM purchaselist WHERE invoiceNo= @vendorIdToDelete";
                msqlCommand.Parameters.AddWithValue("@vendorIdToDelete", purchaseReportToDelete);

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

        private void purchaseItemListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (purchaseItemListView.SelectedIndex != -1)
            {
                PurchaseData purchaseData = _purchaseDataCollection.ElementAt(purchaseItemListView.SelectedIndex);
                BillDetailsWindow billDeatils = new BillDetailsWindow(purchaseData);
                billDeatils.ShowDialog();
            }
        }
    }
}
