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
using System.Collections.ObjectModel;
using SPTDataModel;
using SPTSettings;

namespace PurchaseReport
{
    /// <summary>
    /// Interaction logic for BillDetailsWindow.xaml
    /// </summary>
    public partial class BillDetailsWindow : Window
    {
        SettingsData sptSettings;

        ObservableCollection<BillingData> _billingCollection = new ObservableCollection<BillingData>();

        public ObservableCollection<BillingData> billingCollection
        {
            get
            {
                return _billingCollection;
            }
        }


        public BillDetailsWindow(PurchaseData salesData)
        {
            InitializeComponent();

            datePicker.SelectedDate = salesData.datePurchase;
            invoiceNumberTB.Text = salesData.invoiceNo;
            customerInfoTb.Text = salesData.vendorName;
            long totAmount = Convert.ToInt64(salesData.totalAmount);
            totalAmountLabel.Content = totAmount;
            priceInWordLabel.Content = SPTHelper.NumberToWords(totAmount) + " Only.";

            paymentAmountTB.Text = salesData.payment.ToString();
            ConnectFetchFromSaleslistTable(salesData.invoiceNo);


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


        #region Database Interaction

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void ConnectFetchFromSaleslistTable(string invoiceNumber)
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

                msqlCommand.CommandText = "SELECT * FROM purchaseBilling WHERE invoiceNo = @invoiceNo;";
                msqlCommand.Parameters.AddWithValue("@invoiceNo", invoiceNumber);
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

                double totalVat = 0.0;

                while (msqlReader.Read())
                {
                    BillingData billedData = new BillingData();
                    billedData.amount = msqlReader.GetDouble("amount");
                    billedData.productName = msqlReader.GetString("product");
                    billedData.quantity = msqlReader.GetDouble("quantity");
                    billedData.vat = msqlReader.GetDouble("vat");
                    billedData.calVat = Convert.ToDouble(billedData.amount) * Convert.ToDouble(billedData.vat) * (.01); ;
                    totalVat += billedData.calVat;
                    billedData.rate = msqlReader.GetDouble("rate");
                    billedData.serialNo = billingItemListView.Items.Count + 1; //msqlReader.GetString("serialNo");

                    _billingCollection.Add(billedData);
                    vatAmount.Content = totalVat;
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

    }
}
