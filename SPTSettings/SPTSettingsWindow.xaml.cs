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

namespace SPTSettings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SPTSettingsWindow : Window
    {
        public SPTSettingsWindow()
        {
            InitializeComponent();
        }

        

        #region database interction data fetching

        

        private void EditSptInfo(SettingsData returnEditedsettingsData)
        {
            MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();
                int idSptinfo = 1;
                msqlCommand.CommandText = "UPDATE sptinfo SET name='" + returnEditedsettingsData.Name + "', address='" + returnEditedsettingsData.Address + "', phone='" +
                    returnEditedsettingsData.Phone + "', bill_disclaimer='" + returnEditedsettingsData.BillDisclaimer + "', invoice_prefix='" + returnEditedsettingsData.InvoicePrefix +
                    "' WHERE id_sptinfo='" + idSptinfo + "'; ";

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

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            SetGUIValuesFromDB(FetchSPTSettings.FetcheSettingsData());
        }

        private void SetGUIValuesFromDB(SettingsData settingsData)
        {
            nameTextbox.Text = settingsData.Name;
            addressTextbox.Text = settingsData.Address;
            phoneTextbox.Text = settingsData.Phone.ToString();
            billDisclaimerTextbox.Text = settingsData.BillDisclaimer;
            invoicePrefixTextbox.Text = settingsData.InvoicePrefix;           

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
           SettingsData currentData = GetGUIValues();
           EditSptInfo(currentData);
        }

        private SettingsData GetGUIValues()
        {
            SettingsData currentData = new SettingsData();
            currentData.Address = addressTextbox.Text;
            currentData.BillDisclaimer = billDisclaimerTextbox.Text;
            currentData.InvoicePrefix = invoicePrefixTextbox.Text;
            currentData.Name = nameTextbox.Text;
            currentData.Phone = phoneTextbox.Text;
            return currentData;
        }

        private void passwordChangeButton_Click(object sender, RoutedEventArgs e)
        {
            ChandgePasswordWindow passWindow = new ChandgePasswordWindow();
            passWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetGUIValuesFromDB(FetchSPTSettings.FetcheSettingsData());
        }

        private void phoneTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }
    }
}
