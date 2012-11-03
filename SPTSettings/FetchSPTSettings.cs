using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPTDataModel;

namespace SPTSettings
{
    public static class FetchSPTSettings
    {

        public static SettingsData FetcheSettingsData()
        {
            SettingsData settingsDataObject = null;
            MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "Select * from sptinfo;";
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

                msqlReader.Read();

                settingsDataObject = new SettingsData();

                settingsDataObject.Address = msqlReader.GetString("address");
                settingsDataObject.Name = msqlReader.GetString("name");
                settingsDataObject.Phone = msqlReader.GetString("phone");
                settingsDataObject.Password = msqlReader.GetString("password");
                settingsDataObject.BillDisclaimer = msqlReader.GetString("bill_disclaimer");
                settingsDataObject.InvoicePrefix = msqlReader.GetString("invoice_prefix");
                //settingsDataObject.sptinfo = msqlReader.GetString("id_sptinfo");


            }
            catch (Exception er)
            {
                //Assert//.Show(er.Message);
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }

            return settingsDataObject;
        }

        public static void EditSptPassword(string passwordStr)
        {
            MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();
                int idSptinfo = 1;
                
                msqlCommand.CommandText = "UPDATE sptinfo SET password='" + passwordStr + "' WHERE id_sptinfo='" + idSptinfo + "'; ";
                msqlCommand.ExecuteNonQuery();
            }
            catch (Exception er)
            {
                //MessageBox.Show(er.Message);
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }
        }

        public static string FetchePassword()
        {
            string passwordStr = string.Empty;

            MySql.Data.MySqlClient.MySqlConnection msqlConnection = null;

            msqlConnection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;Password=technicise;database=sptdb;persist security info=False");
            try
            {   //define the command reference
                MySql.Data.MySqlClient.MySqlCommand msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
                msqlCommand.Connection = msqlConnection;

                msqlConnection.Open();

                msqlCommand.CommandText = "Select password from sptinfo;";
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

                msqlReader.Read();

                passwordStr = msqlReader.GetString("password");

            }
            catch (Exception er)
            {
                //Assert//.Show(er.Message);
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }

            return passwordStr;
        }
    }
}
