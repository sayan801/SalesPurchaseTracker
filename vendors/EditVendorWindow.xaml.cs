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
    /// Interaction logic for EditVendorWindow.xaml
    /// </summary>
    public partial class EditVendorWindow : Window
    {
        public VendorsData _vendorToEdit;

        public delegate void delegateEditVendorsData(VendorsData returnEditedVendorData);

        public event delegateEditVendorsData OnEditVendorsData;

        public EditVendorWindow(VendorsData vendorToEdit)
        {
            InitializeComponent();
            _vendorToEdit = vendorToEdit;

            NameDataTB.Text = _vendorToEdit.vendorName;
            AddrDataTB.Text = _vendorToEdit.vendorAdress;
            phDataTB.Text = _vendorToEdit.phoneNumber;
            vatdataTB.Text = _vendorToEdit.vendorVatNo;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            _vendorToEdit.vendorName = NameDataTB.Text;
            _vendorToEdit.vendorAdress = AddrDataTB.Text ;
            _vendorToEdit.phoneNumber = phDataTB.Text;
            _vendorToEdit.vendorVatNo = vatdataTB.Text;

            if (OnEditVendorsData != null)
                OnEditVendorsData(_vendorToEdit);

            EditVendor(_vendorToEdit);
            this.Close();
        }


        #region Database Interaction data insertion

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void EditVendor(VendorsData returnEditedVendorData)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();
                msqlCommand.CommandText = "UPDATE vendors SET vendor_name='" + returnEditedVendorData.vendorName + "', vendor_address='" + returnEditedVendorData.vendorAdress + "', ph_no='" + returnEditedVendorData.phoneNumber + "', vat_no='" + returnEditedVendorData.vendorVatNo + "' WHERE vendor_id='" + returnEditedVendorData.vendorId + "'; ";

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
