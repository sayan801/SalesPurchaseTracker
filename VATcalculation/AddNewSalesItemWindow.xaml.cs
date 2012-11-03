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
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace SalesBillGeneration
{
    /// <summary>
    /// Interaction logic for AddNewItemWindow.xaml
    /// </summary>
    public partial class AddNewSalesItemWindow : Window
    {

        ObservableCollection<string> _salesProductCollection = new ObservableCollection<string>();

        public ObservableCollection<string> salesProductCollection
        {
            get
            {
                return _salesProductCollection;
            }
        }

        public delegate void delegateAddNewBillingData(BillingData returnBillData);

        public event delegateAddNewBillingData OnAddNewBillingData;

        public AddNewSalesItemWindow()
        {
            InitializeComponent();

        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (quantitydataTB.Text != string.Empty && rateDataTB.Text != string.Empty && descriptionDataTB.Text != string.Empty)
            {
                BillingData newBillingData = new BillingData();

                newBillingData.productName = descriptionDataTB.Text;
                newBillingData.quantity =  double.Parse(quantitydataTB.Text);
                newBillingData.rate =  double.Parse(rateDataTB.Text);
                newBillingData.vat =  double.Parse(vatDataCB.Text);

                if (OnAddNewBillingData != null)
                    OnAddNewBillingData(newBillingData);

                this.Close();
            }
            else
            {
                MessageBox.Show("Field has not filled properly");
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Database Interaction
        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void ShowProductDescriptions()
        {
            MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            //define the connection used by the command object
            msqlCommand.Connection = msqlConnection;
           
            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            msqlCommand.CommandText = "SELECT product_name FROM stock";
            MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

            _salesProductCollection.Clear();

            while (msqlReader.Read())
            {

                _salesProductCollection.Add(msqlReader.GetString("product_name"));

            }

            msqlConnection.Close();
        }


        #endregion

        private void descriptionFindBtn_Click(object sender, RoutedEventArgs e)
        {
            descriptionDataTB.IsEnabled = true;
            ShowProductDescriptions();
        }

        #region validation
        private void quantitydataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }
      

        private void rateDataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }
        #endregion
    }
}
