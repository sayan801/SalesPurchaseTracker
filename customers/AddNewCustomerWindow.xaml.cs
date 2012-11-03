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

namespace customers
{
    /// <summary>
    /// Interaction logic for AddNewCustomerWindow.xaml
    /// </summary>
    public partial class AddNewCustomerWindow : Window
    {
        public delegate void delegateAddNewcustomersData(CustomersData returnStockData);

        public event delegateAddNewcustomersData OnAddNewcustomerData;
        public AddNewCustomerWindow()
        {
            InitializeComponent();
        }

        string AssignSpaceIfEmptyField(TextBox textBox)
        {
            if (textBox.Text == string.Empty)
                return " ";
            else
                return textBox.Text;
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDataAvailability() == true)
            {
                CustomersData customerDataObject = new CustomersData();
                customerDataObject.customerName = NameDataTB.Text;
                customerDataObject.customerAdress = AssignSpaceIfEmptyField(AddrDataTB);
                customerDataObject.customerVatNo = AssignSpaceIfEmptyField(vatdataTB);
                customerDataObject.phoneNumber = AssignSpaceIfEmptyField(phDataTB);

                if (OnAddNewcustomerData != null)
                    OnAddNewcustomerData(customerDataObject);

                ConnectInsertTocustomersTable(customerDataObject);

                this.Close();
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Database Interaction data insertion

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void ConnectInsertTocustomersTable(CustomersData customerDataObject)
        {

            //define the connection reference and initialize it
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");


            try
            {

                //foreach (customersData cusomerDataObject in _customersCollection)
                //{

                //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                //define the connection used by the command object
                msqlCommand.Connection = msqlConnection;


                //open the connection
                msqlConnection.Open();

                FeedcustomerData(msqlCommand, customerDataObject);

                //}

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

        void FeedcustomerData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, CustomersData cusomerDataObject)
        {
            //define the command text
            msqlCommand.CommandText = "INSERT INTO customers(id,customer_name,address,ph_no,vat_no,turn_over,due)"
                + "VALUES (@id,@customer_name,@address,@ph_no,@vat_no,@turn_over,@due)";

            //msqlCommand.Parameters.AddWithValue("@sl_no", cusomerDataObject.serialNo);
            msqlCommand.Parameters.AddWithValue("@id", cusomerDataObject.customerId);
            msqlCommand.Parameters.AddWithValue("@customer_name", cusomerDataObject.customerName);
            msqlCommand.Parameters.AddWithValue("@address", cusomerDataObject.customerAdress);
            msqlCommand.Parameters.AddWithValue("@ph_no", cusomerDataObject.phoneNumber);
            msqlCommand.Parameters.AddWithValue("@vat_no", cusomerDataObject.customerVatNo);
            msqlCommand.Parameters.AddWithValue("@turn_over", cusomerDataObject.customerTurnOver);
            msqlCommand.Parameters.AddWithValue("@due", cusomerDataObject.customerDue);

            msqlCommand.ExecuteNonQuery();
        }

        #endregion

        #region validation

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
