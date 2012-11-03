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
using SPTDataModel;
using System.Text.RegularExpressions;


namespace StockManager
{


    /// <summary>
    /// Interaction logic for AddNewItemWindow.xaml
    /// </summary>
    public partial class AddNewStockWindow : Window
    {
        public delegate void delegateAddNewStockData(StockData returnStockData);

        public event delegateAddNewStockData OnAddNewStockData;

        public AddNewStockWindow()
        {
            InitializeComponent();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDataAvailability() == true)
            {
                StockData newStockData = new StockData();

                string addSpace = " ";
                double addZero = 0.0;

                newStockData.productName = productNameDataCB.Text;

                newStockData.vatRate = double.Parse(vatDataCB.Text);


                newStockData.quantityAvailable = addZero;
                newStockData.purchaseDate = DateTime.Now;
                newStockData.quantityPurchased = addZero;
                newStockData.rate = addZero;
                newStockData.vendorId = addSpace;
                newStockData.productId = "Prod-" + DateTime.Now.ToOADate().ToString();

                if (OnAddNewStockData != null)
                    OnAddNewStockData(newStockData);

                ConnectInsertTostockTable(newStockData);

                this.Close();
            }

        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region database interaction insertion

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void ConnectInsertTostockTable(StockData stockDataObject)
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

                FeedstockData(msqlCommand, stockDataObject);
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

        void FeedstockData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, StockData stockDataObject)
        {
            //define the command text
            msqlCommand.CommandText = "INSERT INTO stock(id,vat_rate,product_name,vendor_id,date_purchased,quantity_purchased,rate,quantity_available)"
                + "VALUES (@id,@vatRate,@product_name,@vendor_id,@date_purchased,@quantity_purchased,@rate,@quantity_available)";

            msqlCommand.Parameters.AddWithValue("@id", stockDataObject.productId);
            msqlCommand.Parameters.AddWithValue("@vatRate", stockDataObject.vatRate);
            msqlCommand.Parameters.AddWithValue("@product_name", stockDataObject.productName);
            msqlCommand.Parameters.AddWithValue("@vendor_id", stockDataObject.vendorId);
            msqlCommand.Parameters.AddWithValue("@date_purchased", stockDataObject.purchaseDate);
            msqlCommand.Parameters.AddWithValue("@quantity_purchased", stockDataObject.quantityPurchased);
            msqlCommand.Parameters.AddWithValue("@rate", stockDataObject.rate);
            msqlCommand.Parameters.AddWithValue("@quantity_available", stockDataObject.quantityAvailable);
            msqlCommand.ExecuteNonQuery();
        }

        #endregion

        #region validation

        private bool CheckDataAvailability()
        {
            bool returnVal = true;

            if (string.IsNullOrEmpty(productNameDataCB.Text) || string.IsNullOrEmpty(vatDataCB.Text))
            {
                returnVal = false;
                MessageBox.Show("Enter Product Name & Select VAT rate");
            }
            return returnVal;
        }

        #endregion


    }
}
