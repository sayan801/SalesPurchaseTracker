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

namespace StockManager
{
    /// <summary>
    /// Interaction logic for EditStockItemWindow.xaml
    /// </summary>
    public partial class EditStockItemWindow : Window
    {
        public StockData _StockItemToEdit;

        public delegate void delegateEditStockItemData(StockData returnEditedStockItemData);

        public event delegateEditStockItemData OnEditStockItemData;


        public EditStockItemWindow(StockData StockItemToEdit)
        {
            InitializeComponent();

            _StockItemToEdit = StockItemToEdit;
            productNameDataCB.Text = _StockItemToEdit.productName;
            vatDataCB.Text = _StockItemToEdit.vatRate.ToString();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #region Database Interaction data insertion

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void EditStockItem(StockData _StockItemToEdit)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();
                msqlCommand.CommandText = "UPDATE stock SET product_name='" + _StockItemToEdit.productName + "',vat_rate='" + _StockItemToEdit.vatRate + "' WHERE id='" + _StockItemToEdit.productId + "'; ";
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

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDataAvailability() == true)
            {
                _StockItemToEdit.productName = productNameDataCB.Text;
                _StockItemToEdit.vatRate = double.Parse(vatDataCB.Text);


                if (OnEditStockItemData != null)
                    OnEditStockItemData(_StockItemToEdit);

                EditStockItem(_StockItemToEdit);
                this.Close();
            }
        }

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
