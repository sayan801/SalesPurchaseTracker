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

namespace SPTSettings
{
    /// <summary>
    /// Interaction logic for ChandgePasswordWindow.xaml
    /// </summary>
    public partial class ChandgePasswordWindow : Window
    {
        public ChandgePasswordWindow()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (oldPasswordBox.Password.Equals(FetchSPTSettings.FetchePassword()))
            {
                if (!newPasswordBox.Password.Equals(string.Empty) && newPasswordBox.Password.Equals(reNewPasswordBox.Password))
                {
                    FetchSPTSettings.EditSptPassword(newPasswordBox.Password);
                    this.Close();
                }
                else
                    MessageBox.Show("Please Enter same Password both the password field for new one");
            }
            else
            {
                MessageBox.Show("Please Enter correct Password");
            }
        }
    }
}
