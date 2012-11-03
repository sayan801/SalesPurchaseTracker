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
using System.Collections.ObjectModel;
using SPTDataModel;

namespace vendors
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class vendorDetailsWindow : Window
    {
        ObservableCollection<VendorsData> _vendorsCollection = new ObservableCollection<VendorsData>();

        static int vendorCount = 0;

        public ObservableCollection<VendorsData> vendorsCollection
        {
            get
            {
                return _vendorsCollection;
            }
        }
        public vendorDetailsWindow()
        {
            InitializeComponent();
        }

        string GetVendorId()
        {
            return "Vendor-" + vendorCount++ + "-" + DateTime.Now.ToOADate().ToString();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewVendorWindow AddNewVendorWindowObject = new AddNewVendorWindow();
            AddNewVendorWindowObject.OnAddNewvendorData += new AddNewVendorWindow.delegateAddNewvendorsData(AddNewVendorWindowObject_OnAddNewvendorData);
            AddNewVendorWindowObject.ShowDialog();
        }
        void AddNewVendorWindowObject_OnAddNewvendorData(VendorsData vendordataobject)
        {
            int turnover = 0;
            vendordataobject.vendorTurnOver = turnover;
            vendordataobject.vendorDue = turnover;
            vendordataobject.vendorId = GetVendorId();
            //vendordataobject.serialNo = vendorCount.ToString();
            _vendorsCollection.Add(vendordataobject);
        }


        #region database interction data fetching

        MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;



        private void DeleteVendor(string vendorToDelete)
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "DELETE FROM vendors WHERE vendor_id= @vendorIdToDelete";
                msqlCommand.Parameters.AddWithValue("@vendorIdToDelete", vendorToDelete);

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

        private void fetchevendorData()
        {
            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "Select * from vendors;";
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();
                _vendorsCollection.Clear();
                while (msqlReader.Read())
                {
                    VendorsData vendorsDataObject = new VendorsData();

                    //vendorsDataObject.serialNo = msqlReader.GetString("sl_no");
                    vendorsDataObject.vendorAdress = msqlReader.GetString("vendor_address");
                    vendorsDataObject.phoneNumber = msqlReader.GetString("ph_no");
                    vendorsDataObject.vendorName = msqlReader.GetString("vendor_name");
                    vendorsDataObject.vendorVatNo = msqlReader.GetString("vat_no");
                    vendorsDataObject.vendorId = msqlReader.GetString("vendor_id");
                    vendorsDataObject.vendorTurnOver = msqlReader.GetDouble("turn_over");
                    vendorsDataObject.vendorDue = msqlReader.GetDouble("due");
                    _vendorsCollection.Add(vendorsDataObject);

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


        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            fetchevendorData();
        }

        private VendorsData GetSelectedItem()
        {

            VendorsData vendorToEdit = null;

            if (businessPersonListView.SelectedIndex == -1)
                MessageBox.Show("Please Select an Item");
            else
            {
                VendorsData i = (VendorsData)businessPersonListView.SelectedItem;

                vendorToEdit = _vendorsCollection.Where(item => item.vendorId.Equals(i.vendorId)).First();
            }

            return vendorToEdit;
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            VendorsData vendorToEdit = GetSelectedItem();

            if (vendorToEdit != null)
            {
                EditVendorWindow editWindow = new EditVendorWindow(vendorToEdit);
                editWindow.OnEditVendorsData += new EditVendorWindow.delegateEditVendorsData(editWindow_OnEditVendorsData);
                editWindow.ShowDialog();
            }
        }

        void editWindow_OnEditVendorsData(VendorsData returnEditedVendorData)
        {
            //finding the element
            VendorsData vData = _vendorsCollection.Where(item => item.vendorId.Equals(returnEditedVendorData.vendorId)).First();
            //finding the element position
            int itemIdex = _vendorsCollection.IndexOf(vData);
            //remove the element so that the list gets refreshed
            _vendorsCollection.RemoveAt(itemIdex);
            //insert the edited element at same position
            _vendorsCollection.Insert(itemIdex, returnEditedVendorData);
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            VendorsData vendorToDelete = GetSelectedItem();
            if (vendorToDelete != null)
            {
                _vendorsCollection.Remove(vendorToDelete);
                DeleteVendor(vendorToDelete.vendorId);
            }
        }

        private void showVendorsBtn_Click(object sender, RoutedEventArgs e)
        {
            fetchevendorData();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Printer.PrintArea(businessPersonListView, "List of Vendors");
        }



    }
}
