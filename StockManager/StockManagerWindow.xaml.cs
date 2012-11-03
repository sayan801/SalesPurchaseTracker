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

namespace StockManager
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StockManagerWindow : Window
    {

        ObservableCollection<StockData> _stockCollection = new ObservableCollection<StockData>();

        public ObservableCollection<StockData> stockCollection
        {
            get
            {
                return _stockCollection;
            }
        }

        static int serialNo = 0;

        //todo productCount should be fetched from db
        static int productCount = 0;
        string GetProductId()
        {
            return "Prod-" + productCount.ToString("0000");
        }

        public StockManagerWindow()
        {
            InitializeComponent();
        }

        private void addItemBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewStockWindow newStock = new AddNewStockWindow();
            newStock.OnAddNewStockData += new AddNewStockWindow.delegateAddNewStockData(newStock_OnAddNewStockData);
            serialNo++;
            newStock.ShowDialog();
        }

        void newStock_OnAddNewStockData(StockData returnStockData)
        {
            //returnStockData.serialNo = GetCurrentSerialNumber();
            _stockCollection.Add(returnStockData);
        }

        private void deleteItemBtn_Click(object sender, RoutedEventArgs e)
        {
            StockData stockToDelete;
            stockToDelete = GetSelectedItem();
            if (stockToDelete != null)
            {
                _stockCollection.Remove(stockToDelete);
                DeleteStock(stockToDelete.productId);
            }
        }

        #region database interaction delete from customers

        private void DeleteStock(string stockToDelete)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "DELETE FROM stock WHERE id= @vendorIdToDelete";
                msqlCommand.Parameters.AddWithValue("@vendorIdToDelete", stockToDelete);

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

        private void editItemBtn_Click(object sender, RoutedEventArgs e)
        {
            StockData StockItemToEdit = GetSelectedItem();
            if (StockItemToEdit != null)
            {
                EditStockItemWindow editWindow = new EditStockItemWindow(StockItemToEdit);
                editWindow.OnEditStockItemData += new EditStockItemWindow.delegateEditStockItemData(editWindow_OnEditStockItemData);
                editWindow.ShowDialog();
            }
        }

        private StockData GetSelectedItem()
        {

            StockData StockItemToEdit = null;

            if (stockItemListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item");
            else
            {
                StockData i = (StockData)stockItemListView.SelectedItem;

                StockItemToEdit = _stockCollection.Where(item => item.productId.Equals(i.productId)).First();
            }
            return StockItemToEdit;
        }

        void editWindow_OnEditStockItemData(StockData returnEditedStockItemData)
        {
            //finding the element
            StockData vData = _stockCollection.Where(item => item.productId.Equals(returnEditedStockItemData.productId)).First();
            //finding the element position
            int itemIdex = _stockCollection.IndexOf(vData);
            //remove the element so that the list gets refreshed
            _stockCollection.RemoveAt(itemIdex);
            //insert the edited element at same position
            _stockCollection.Insert(itemIdex, returnEditedStockItemData);

        }


        #region database interction data fetching

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void fetchevendorData()
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                if (msqlConnection.State != System.Data.ConnectionState.Open)
                    msqlConnection.Open();

                msqlCommand.CommandText = "Select * from stock;";
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();
                _stockCollection.Clear();

                while (msqlReader.Read())
                {
                    StockData stockDataObject = new StockData();

                    //stockDataObject.serialNo = GetCurrentSerialNumber(); 
                    stockDataObject.productId = msqlReader.GetString("id");
                    stockDataObject.purchaseDate = msqlReader.GetDateTime("date_purchased");
                    stockDataObject.productName = msqlReader.GetString("product_name");
                    stockDataObject.quantityPurchased = msqlReader.GetDouble("quantity_purchased");
                    stockDataObject.vendorId = msqlReader.GetString("vendor_id");
                    stockDataObject.rate = msqlReader.GetDouble("rate");
                    stockDataObject.vatRate = msqlReader.GetDouble("vat_rate");
                    stockDataObject.quantityAvailable = msqlReader.GetDouble("quantity_available");
                    _stockCollection.Add(stockDataObject);

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
            fetchevendorData();
        }

        private string GetCurrentSerialNumber()
        {
            return (_stockCollection.Count + 1).ToString();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(stockItemListView, "Stock List");
        }

        private void stockItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
