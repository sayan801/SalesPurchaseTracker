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
using CustomerPayments;
using VendorPayments;
using InputVatCalculation;
using OutputVatCalculation;

namespace SalesPurchaseTracker
{



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ToDoList.ToDoWindow todoWindow;

        OutputVatCalcWindow outputVatCalc;
        InputVatCalcWindow inputVatCalc;
        CustomerPaymentsWindow customerPaymentWindow;
        VendorPaymentsWindow vendorPaymentWindow;

        #region Billing

        static int billInvoiceCount = 0;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            todoWindow = new ToDoList.ToDoWindow();
            inputVatCalc = new InputVatCalcWindow();
            outputVatCalc = new OutputVatCalcWindow();
            customerPaymentWindow = new CustomerPaymentsWindow();
            vendorPaymentWindow = new VendorPaymentsWindow();
        }

        private void calculatorBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("calc");
        }

        private void todoBtn_Click(object sender, RoutedEventArgs e)
        {
            todoWindow.Show();
        }

        private void billingBtn_Click(object sender, RoutedEventArgs e)
        {
            SalesBillGeneration.SalesBillingWindow billingWindow;

            billingWindow = new SalesBillGeneration.SalesBillingWindow();

            billInvoiceCount++;
            billingWindow.Show();
            billingWindow.SetBillInvoiceNumber(billInvoiceCount);
        }



        private void stockBtn_Click(object sender, RoutedEventArgs e)
        {
            StockManager.StockManagerWindow stockWindow = new StockManager.StockManagerWindow();
            stockWindow.ShowDialog();
        }

        private void vendorBtn_Click(object sender, RoutedEventArgs e)
        {
            vendors.vendorDetailsWindow vendorInfo = new vendors.vendorDetailsWindow();
            vendorInfo.ShowDialog();
            //BusinessPersons.BusinessPersonWindow vendorInfo = new BusinessPersons.BusinessPersonWindow("Vendor");
            //vendorInfo.ShowDialog();
        }

        private void customerReportBtn_Click(object sender, RoutedEventArgs e)
        {
            customers.customerDetailsWindow customerinfo = new customers.customerDetailsWindow();
            customerinfo.ShowDialog();
            // BusinessPersons.BusinessPersonWindow customerInfo = new BusinessPersons.BusinessPersonWindow("Customer");
            // customerInfo.ShowDialog();
        }

        private void purchaseBillingBtn_Click(object sender, RoutedEventArgs e)
        {
            PurchaseBilling.PurchaseBillingWindow billingWindow;

            billingWindow = new PurchaseBilling.PurchaseBillingWindow();

            billInvoiceCount++;
            billingWindow.Show();
            billingWindow.SetBillInvoiceNumber(billInvoiceCount);
        }

        private void salesBtn_Click(object sender, RoutedEventArgs e)
        {
            SalesReport.SalesReportWindow salesReport = new SalesReport.SalesReportWindow();
            salesReport.ShowDialog();
        }

        private void purchaseBtn_Click(object sender, RoutedEventArgs e)
        {
            PurchaseReport.PurchaseReportWindow purchaseReport = new PurchaseReport.PurchaseReportWindow();
            purchaseReport.ShowDialog();
        }

        private void ipVatCalcBtn_Click(object sender, RoutedEventArgs e)
        {
            inputVatCalc.Show();
        }

        private void opVatCalcBtn_Click(object sender, RoutedEventArgs e)
        {
            outputVatCalc.Show();
        }

        private void customerPayementsBtn_Click(object sender, RoutedEventArgs e)
        {
            customerPaymentWindow.Show();
        }

        private void vendorPaymentsBtn_Click(object sender, RoutedEventArgs e)
        {
            vendorPaymentWindow.Show();
        }

        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SPTSettings.SPTSettingsWindow settingWindow = new SPTSettings.SPTSettingsWindow();
            settingWindow.ShowDialog();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Equals(SPTSettings.FetchSPTSettings.FetchePassword()))
            {
                mainUniformGrid.IsEnabled = true;
                passwordBox.Password = string.Empty;
                loginExpander.IsExpanded = false;
                loginExpander.IsEnabled = false;
                loginExpander.Header = "Logged In";
            }
            else
            {
                MessageBox.Show("Please Enter correct Password");
                passwordBox.Password = string.Empty;
            }
        }

    }
}
