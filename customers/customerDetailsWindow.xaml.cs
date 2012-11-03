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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SPTDataModel;
using System.Collections.ObjectModel;

namespace customers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class customerDetailsWindow : Window
    {
        ObservableCollection<CustomersData> _customersCollection = new ObservableCollection<CustomersData>();
        static int customerCount = 0;

        public ObservableCollection<CustomersData> customersCollection
        {
            get
            {
                return _customersCollection;
            }
        }
        public customerDetailsWindow()
        {
            InitializeComponent();
        }
        int getcustomerid
        {
            get
            {
                return customerCount++;
            }
        } // SAGNIK(MY THOUGHT)-- u should use a get keyword and return it

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewCustomerWindow AddNewCustomerWindowObject = new AddNewCustomerWindow();
            AddNewCustomerWindowObject.OnAddNewcustomerData += new AddNewCustomerWindow.delegateAddNewcustomersData(AddNewCustomerWindowObject_OnAddNewcustomerData);
            AddNewCustomerWindowObject.ShowDialog(); // SAGNIK(MY THOUGHT)-- you should call the ShowDialog() method. then the window will be visible.
        }
        void AddNewCustomerWindowObject_OnAddNewcustomerData(CustomersData customerdataobject)
        {
            int turnover = 0;
            customerdataobject.customerTurnOver = turnover;
            customerdataobject.customerDue = turnover;
            customerdataobject.customerId = DateTime.Now.ToOADate().ToString() + getcustomerid.ToString(); //...SAGNIK(MY THOUGHT)ihave added date string as an ex u can any other thing to make it unique......////need some helppppppppppppppppppppppppppppppppppppppppppp
            //customerdataobject.serialNo = customerCount.ToString();
            _customersCollection.Add(customerdataobject);
        }


        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            CustomersData customerToDelete = GetSelectedItem();
            if (customerToDelete != null)
            {
                _customersCollection.Remove(customerToDelete);
                DeleteCustomer(customerToDelete.customerId);
            }
        }

        #region database interaction delete from customers

        private void DeleteCustomer(string customerToDelete)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "DELETE FROM customers WHERE id= @vendorIdToDelete";
                msqlCommand.Parameters.AddWithValue("@vendorIdToDelete", customerToDelete);

                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

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

        #region database interction data fetching

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

        private void fetchecustomerData()
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "Select * from customers;";
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();
                _customersCollection.Clear();

                while (msqlReader.Read())
                {
                    CustomersData customersDataObject = new CustomersData();

                    //customersDataObject.serialNo = msqlReader.GetString("sl_no");
                    customersDataObject.customerAdress = msqlReader.GetString("address");
                    customersDataObject.phoneNumber = msqlReader.GetString("ph_no");
                    customersDataObject.customerName = msqlReader.GetString("customer_name");
                    customersDataObject.customerVatNo = msqlReader.GetString("vat_no");
                    customersDataObject.customerId = msqlReader.GetString("id");
                    customersDataObject.customerTurnOver = msqlReader.GetDouble("turn_over");
                    customersDataObject.customerDue = msqlReader.GetDouble("due");
                    _customersCollection.Add(customersDataObject);

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fetchecustomerData();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(businessPersonListView, "List of Customers");
        }

        private void showPaymentsBtn_Click(object sender, RoutedEventArgs e)
        {
            fetchecustomerData();
        }

        private CustomersData GetSelectedItem()
        {

            CustomersData customerToEdit = null;

            if (businessPersonListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item");
            else
            {
                CustomersData i = (CustomersData)businessPersonListView.SelectedItem;

                customerToEdit = _customersCollection.Where(item => item.customerId.Equals(i.customerId)).First();
            }

            return customerToEdit;
        }

        private void editBtn_Click_1(object sender, RoutedEventArgs e)
        {
            CustomersData customerToEdit = GetSelectedItem();
            if (customerToEdit != null)
            {
                EditCustomerWindow editWindow = new EditCustomerWindow(customerToEdit);
                editWindow.OnEditCustomersData += new EditCustomerWindow.delegateEditCustomersData(editWindow_OnEditCustomersData);
                editWindow.ShowDialog();
            }
        }

        void editWindow_OnEditCustomersData(CustomersData returnEditedCustomerData)
        {
            //finding the element
            CustomersData vData = _customersCollection.Where(item => item.customerId.Equals(returnEditedCustomerData.customerId)).First();
            //finding the element position
            int itemIdex = _customersCollection.IndexOf(vData);
            //remove the element so that the list gets refreshed
            _customersCollection.RemoveAt(itemIdex);
            //insert the edited element at same position
            _customersCollection.Insert(itemIdex, returnEditedCustomerData);
        }

    }
}