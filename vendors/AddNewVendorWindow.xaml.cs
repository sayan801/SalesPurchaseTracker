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

namespace vendors
{
    /// <summary>
    /// Interaction logic for AddNewVendorWindow.xaml
    /// </summary>
    public partial class AddNewVendorWindow : Window
    {
        public delegate void delegateAddNewvendorsData(VendorsData returnStockData);

        public event delegateAddNewvendorsData OnAddNewvendorData;
        public AddNewVendorWindow()
        {
            InitializeComponent();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {

            if (NameDataTB.Text != string.Empty && AddrDataTB.Text != string.Empty && phDataTB.Text != string.Empty && vatdataTB.Text != string.Empty)
            {
                VendorsData vendorDataObject = new VendorsData();
                vendorDataObject.vendorName = NameDataTB.Text;
                vendorDataObject.vendorAdress = AddrDataTB.Text;
                vendorDataObject.vendorVatNo = vatdataTB.Text;
                vendorDataObject.phoneNumber = phDataTB.Text;
                if (OnAddNewvendorData != null)
                    OnAddNewvendorData(vendorDataObject);

                ConnectInsertTovendorsTable(vendorDataObject);

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


        #region Database Interaction data insertion

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void ConnectInsertTovendorsTable(VendorsData vendordataobject)
        {

            //define the connection reference and initialize it
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");


            try
            {

                //foreach (vendorsData vendordataobject in _vendorsCollection)
                //{

                    //define the command reference
                    MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                    //define the connection used by the command object
                    msqlCommand.Connection = msqlConnection;


                    //open the connection
                    msqlConnection.Open();

                    FeedvendorData(msqlCommand, vendordataobject);

//                }

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

        void FeedvendorData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, VendorsData vendorDataObject)
        {
            //define the command text
            msqlCommand.CommandText = "INSERT INTO vendors(vendor_id,vendor_name,vendor_address,ph_no,vat_no,turn_over,due)"
                + "VALUES (@vendor_id,@vendor_name,@vendor_address,@ph_no,@vat_no,@turn_over,@due)";

            //msqlCommand.Parameters.AddWithValue("@sl_no", vendorDataObject.serialNo);
            msqlCommand.Parameters.AddWithValue("@vendor_id", vendorDataObject.vendorId);
            msqlCommand.Parameters.AddWithValue("@vendor_name", vendorDataObject.vendorName);
            msqlCommand.Parameters.AddWithValue("@vendor_address", vendorDataObject.vendorAdress);
            msqlCommand.Parameters.AddWithValue("@ph_no", vendorDataObject.phoneNumber);
            msqlCommand.Parameters.AddWithValue("@vat_no", vendorDataObject.vendorVatNo);
            msqlCommand.Parameters.AddWithValue("@turn_over", vendorDataObject.vendorTurnOver);
            msqlCommand.Parameters.AddWithValue("@due", vendorDataObject.vendorDue);

            msqlCommand.ExecuteNonQuery();
        }

        #endregion

        #region validation
        

        private void NameDataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyAlphabet(e.Text);
        }

        private void AddrDataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = !onlyAlphaNumeric(e.Text);
        }

        private void phDataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }

        private void vatdataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ValidationHandler.onlyAlphaNumeric(e.Text);
        }
        #endregion
    }
}
