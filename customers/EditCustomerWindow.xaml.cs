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

namespace customers
{
    /// <summary>
    /// Interaction logic for EditCustomerWindow.xaml
    /// </summary>
    public partial class EditCustomerWindow : Window
    {
        public CustomersData _customerToEdit;

        public delegate void delegateEditCustomersData(CustomersData returnEditedCustomerData);

        public event delegateEditCustomersData OnEditCustomersData;

        public EditCustomerWindow(CustomersData customerToEdit)
        {
            InitializeComponent();
            _customerToEdit = customerToEdit;

            NameDataTB.Text = _customerToEdit.customerName;
            AddrDataTB.Text = _customerToEdit.customerAdress;
            phDataTB.Text = _customerToEdit.phoneNumber;
            vatdataTB.Text = _customerToEdit.customerVatNo;
        }
        public EditCustomerWindow()
        {
            InitializeComponent();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDataAvailability() == true)
            {
                _customerToEdit.customerName = NameDataTB.Text;
                _customerToEdit.customerAdress = AddrDataTB.Text;
                _customerToEdit.phoneNumber = phDataTB.Text;
                _customerToEdit.customerVatNo = vatdataTB.Text;

                if (OnEditCustomersData != null)
                    OnEditCustomersData(_customerToEdit);

                EditCustomer(_customerToEdit);
                this.Close();
            }
        }

        #region Database Interaction data insertion

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void EditCustomer(CustomersData returnEditedCustomerData)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();
                msqlCommand.CommandText = "UPDATE customers SET customer_name='" + returnEditedCustomerData.customerName + "', address='" + returnEditedCustomerData.customerAdress + "', ph_no='" + returnEditedCustomerData.phoneNumber + "', vat_no='" + returnEditedCustomerData.customerVatNo + "' WHERE id='" + returnEditedCustomerData.customerId + "'; ";

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

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        #region validation
        /// <summary>
        /// This function checks whether all required fields are provided by the user..
        /// </summary>
        /// <returns></returns>
        private bool CheckDataAvailability()
        {
            bool returnVal = true;

            if (string.IsNullOrEmpty(NameDataTB.Text))
            {
                returnVal = false;
                MessageBox.Show("Enter customer Name");

            }
            return returnVal;
        }

        /// <summary>
        /// Allows phone number to be numeric
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void phDataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }

        /// <summary>
        /// allows vat number to only alpha-numeric
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vatdataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ValidationHandler.onlyAlphaNumeric(e.Text);
        }
        #endregion
    }
}
