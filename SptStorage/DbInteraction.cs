using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPTDataModel;

namespace SptStorage
{
    public static class DbInteraction
    {
        static string passwordCurrent = "technicise";
        static string dbmsCurrent = "sptdb";

        private static MySql.Data.MySqlClient.MySqlConnection OpenDbConnection()
        {
            MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=" + passwordCurrent + ";database=" + dbmsCurrent + ";persist security info=False");

            //open the connection
            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            return msqlConnection;
        }


        #region Customer


        public static List<CustomersData> FetchCustomersList()
        {
            List<CustomersData> customerCollection = new List<CustomersData>();

            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();

            //define the command reference
            MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();


            msqlCommand.Connection = msqlConnection;

            if (msqlConnection.State != System.Data.ConnectionState.Open)
                msqlConnection.Open();

            msqlCommand.CommandText = "SELECT * FROM customers";
            MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

            //_customerCollection.Clear();

            while (msqlReader.Read())
            {
                CustomersData cusData = new CustomersData();
                cusData.customerName = msqlReader.GetString("customer_name");
                cusData.customerId = msqlReader.GetString("id");
                cusData.customerAdress = msqlReader.GetString("address");
                cusData.phoneNumber = msqlReader.GetString("ph_no");

                customerCollection.Add(cusData);

            }

            msqlConnection.Close();

            return customerCollection;
        }
              

        public static void UpdateCustomerTableWithPaymentOnSales(double totalBilledAmount, double paymentAmount, string cusIdKey)
        {

            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();

            try
            {
                //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                //define the connection used by the command object
                msqlCommand.Connection = msqlConnection;

                double dbDue = 0.0;
                double dbTurnOver = 0.0;
                

                string cmdStr = "SELECT turn_over,due FROM customers WHERE id='" + cusIdKey + "';";
                msqlCommand.CommandText = cmdStr;


                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

                while (msqlReader.Read())
                {
                    dbDue = double.Parse(msqlReader.GetString("due"));
                    dbTurnOver = double.Parse(msqlReader.GetString("turn_over"));
                }

                if (msqlConnection.State == System.Data.ConnectionState.Open)
                    msqlConnection.Close();

                //updating the value
                if (msqlConnection.State != System.Data.ConnectionState.Open)
                    msqlConnection.Open();


                double newDueDouble = dbDue + totalBilledAmount - paymentAmount;
                String newDue = newDueDouble.ToString();
                String newTurnOver = (dbTurnOver + totalBilledAmount).ToString();
                msqlCommand.CommandText = "UPDATE customers SET due='" + newDue + "', turn_over='" + newTurnOver + "' WHERE id='" + cusIdKey + "'; ";

                msqlCommand.ExecuteNonQuery();


            }
            catch (Exception er)
            {
                //MessageBox.Show(er.Message);
            }
            finally
            {
                if (msqlConnection.State == System.Data.ConnectionState.Open)
                    msqlConnection.Close();
            }

        }



        #endregion

        #region Vendor

        public static void UpdateVendorTableWithPaymentOnPurchase(double totalBilledAmount, double purchasePaymentAmount, string venIdKey)
        {

            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();

            try
            {
                //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                //define the connection used by the command object
                msqlCommand.Connection = msqlConnection;

                double dbDue = 0.0;
                double dbTurnOver = 0.0;

               
                string cmdStr = "SELECT turn_over,due FROM vendors WHERE vendor_id='" + venIdKey + "';";
                msqlCommand.CommandText = cmdStr;


                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

                while (msqlReader.Read())
                {
                    dbDue = double.Parse(msqlReader.GetString("due"));
                    dbTurnOver = double.Parse(msqlReader.GetString("turn_over"));
                }

                if (msqlConnection.State == System.Data.ConnectionState.Open)
                    msqlConnection.Close();

                //updating the value
                if (msqlConnection.State != System.Data.ConnectionState.Open)
                    msqlConnection.Open();



                double newDueDouble = dbDue + totalBilledAmount - purchasePaymentAmount;
                String newDue = newDueDouble.ToString();
                String newTurnOver = (dbTurnOver + totalBilledAmount).ToString();
                msqlCommand.CommandText = "UPDATE vendors SET due='" + newDue + "', turn_over='" + newTurnOver + "' WHERE vendor_id='" + venIdKey + "'; ";

                msqlCommand.ExecuteNonQuery();


            }
            catch (Exception er)
            {
                //MessageBox.Show("Error: " + MethodBase.GetCurrentMethod().Name + ":" + er.Message);
            }
            finally
            {
                if (msqlConnection.State == System.Data.ConnectionState.Open)
                    msqlConnection.Close();
            }

        }

        #endregion

      

        public static void ConnectInsertTocustomerPaymentTable(CustomerPaymentData customerDataObject)
        {

            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();

            try
            {
                //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();

                //define the connection used by the command object
                msqlCommand.Connection = msqlConnection;


                FeedcustomerData(msqlCommand, customerDataObject);

            }
            catch (Exception er)
            {
            
            }
            finally
            {
                msqlConnection.Close();

            }

        }

        static void  FeedcustomerData(MySql.Data.MySqlClient.MySqlCommand msqlCommand, CustomerPaymentData cusomerDataObject)
        {
            //define the command text
            msqlCommand.CommandText = "INSERT INTO customer_payment(customer_id,payment_amount,payment_date,payment_id)"
                + "VALUES (@customer_id,@payment_amount,@payment_date,@payment_id)";

            msqlCommand.Parameters.AddWithValue("@customer_id", cusomerDataObject.customerId);
            msqlCommand.Parameters.AddWithValue("@payment_id", cusomerDataObject.paymentId);
            msqlCommand.Parameters.AddWithValue("@payment_amount", cusomerDataObject.paymentAmount);
            msqlCommand.Parameters.AddWithValue("@payment_date", cusomerDataObject.paymentDate);

            msqlCommand.ExecuteNonQuery();
        }




        public static void DeleteCustomerPayment(string customerPaymentToDelete)
        {
            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();
            
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlCommand.CommandText = "DELETE FROM customer_payment WHERE payment_id= @vendorIdToDelete";
                msqlCommand.Parameters.AddWithValue("@vendorIdToDelete", customerPaymentToDelete);

                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

            }
            catch (Exception er)
            {
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }

        }



        public static List<CustomerPaymentData> FetchCustomerPaymentData()
        {
            List<CustomerPaymentData> customerPaymentCollection = new List<CustomerPaymentData>();

            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();

            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;


                msqlCommand.CommandText = "Select customers.id, customers.address, customers.ph_no,customers.customer_name, customer_payment.payment_id, customer_payment.payment_amount, customer_payment.payment_date FROM customers,customer_payment WHERE customer_payment.customer_id = customers.id;";

                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();
                while (msqlReader.Read())
                {
                    CustomerPaymentData addCustomerPaymentWindowObject = new CustomerPaymentData();

                    //addCustomerPaymentWindowObject.serialNo = (_customerPaymentCollection.Count + 1).ToString();
                    addCustomerPaymentWindowObject.customerId = msqlReader.GetString("id");
                    addCustomerPaymentWindowObject.paymentId = msqlReader.GetString("payment_id");
                    addCustomerPaymentWindowObject.customerName = msqlReader.GetString("customer_name");
                    addCustomerPaymentWindowObject.customerAddress = msqlReader.GetString("address");
                    addCustomerPaymentWindowObject.customerPhone = msqlReader.GetString("ph_no");
                    addCustomerPaymentWindowObject.paymentAmount = msqlReader.GetDouble("payment_amount");
                    addCustomerPaymentWindowObject.paymentDate = msqlReader.GetDateTime("payment_date");
                    customerPaymentCollection.Add(addCustomerPaymentWindowObject);

                }

            }
            catch (Exception er)
            {
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }
            return customerPaymentCollection;
        }



        public static void EditCustomerPayments(CustomerPaymentData returnEditedCustomerPaymentsData)
        {
            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();
            
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlCommand.CommandText = "UPDATE customer_payment SET  payment_amount='" + returnEditedCustomerPaymentsData.paymentAmount + "' WHERE payment_id='" + returnEditedCustomerPaymentsData.paymentId + "'; ";

                msqlCommand.ExecuteNonQuery();


            }
            catch (Exception er)
            {
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }

        }

      

        public static void DeleteTodo(string todoIdToDelete)
        {
            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                

                msqlCommand.CommandText = "DELETE FROM to_do WHERE id= @todoIdToDelete";
                msqlCommand.Parameters.AddWithValue("@todoIdToDelete", todoIdToDelete);

                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

            }
            catch (Exception er)
            {
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }

        }

        public static void ConnectInsertToDoListTable(ToDoData tdData)
        {
            //define the connection reference and initialize it
            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();

            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;


                msqlCommand.CommandText = "INSERT INTO to_do(date_time,to_do,id)" + "VALUES(@date_time,@to_do,@id)";

                msqlCommand.Parameters.AddWithValue("@date_time", tdData.date_time);
                msqlCommand.Parameters.AddWithValue("@to_do", tdData.to_do);
                msqlCommand.Parameters.AddWithValue("@id", tdData.id);

                msqlCommand.ExecuteNonQuery();


            }
            catch (Exception er)
            {
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }
        }

        public static List<ToDoData> fetcheToDoData()
        {
            List<ToDoData> _toDoCollection = new List<ToDoData>();

            MySql.Data.MySqlClient.MySqlConnection msqlConnection = OpenDbConnection();
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;
                

                msqlCommand.CommandText = "Select * from to_do;";
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

                while (msqlReader.Read())
                {
                    ToDoData ToDoData = new ToDoData();
                    ToDoData.to_do = msqlReader.GetString("to_do");
                    ToDoData.date_time = msqlReader.GetDateTime("date_time");
                    ToDoData.id = msqlReader.GetString("id");

                    _toDoCollection.Add(ToDoData);

                }

            }
            catch (Exception er)
            {
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }
            return _toDoCollection;
        }



    }
}
