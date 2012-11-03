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

namespace VendorPayments
{
    /// <summary>
    /// Interaction logic for EditVendorPaymentsWindow.xaml
    /// </summary>
    public partial class EditVendorPaymentsWindow : Window
    {
        public VendorPaymentData _vendorPaymentToEdit;

        public delegate void delegateEditVendorPaymentData(VendorPaymentData returnEditedVendorPaymentData);

        public event delegateEditVendorPaymentData OnEditVendorPaymentData;

        public EditVendorPaymentsWindow(VendorPaymentData vendorPaymentToEdit)
        {
            InitializeComponent();

            _vendorPaymentToEdit = vendorPaymentToEdit;
            nameDataTB.Text = _vendorPaymentToEdit.vendorName;
            addressDataTB.Text = _vendorPaymentToEdit.vendorAddress;
            custIdTB.Text = _vendorPaymentToEdit.vendorId;
            paymentAmountTB.Text = _vendorPaymentToEdit.paymentAmount.ToString();
        }     

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            _vendorPaymentToEdit.paymentAmount =  double.Parse(paymentAmountTB.Text);


            if (OnEditVendorPaymentData != null)
                OnEditVendorPaymentData(_vendorPaymentToEdit);

            EditVendorPayments(_vendorPaymentToEdit);
            this.Close();
        }


        #region Database Interaction data insertion

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void EditVendorPayments(VendorPaymentData _vendorPaymentToEdit)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();
                msqlCommand.CommandText = "UPDATE vendor_payment SET payment_amount='" + _vendorPaymentToEdit.paymentAmount + "' WHERE payment_id='" + _vendorPaymentToEdit.paymentId + "'; ";

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
    }
}
