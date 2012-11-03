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

namespace CustomerPayments
{
    /// <summary>
    /// Interaction logic for EditCustomerPaymentsWindow.xaml
    /// </summary>
    public partial class EditCustomerPaymentsWindow : Window
    {
        public EditCustomerPaymentsWindow()
        {
            InitializeComponent();
        }
        public CustomerPaymentData _customerPaymentsToEdit;

        public delegate void delegateEditCustomrerPaymentData(CustomerPaymentData returnEditedVendorData);

        public event delegateEditCustomrerPaymentData OnEditCustomersPaymentData;

        public EditCustomerPaymentsWindow(CustomerPaymentData customerPaymentsToEdit)
        {
            InitializeComponent();

           _customerPaymentsToEdit = customerPaymentsToEdit;
           nameDataTB.Text = _customerPaymentsToEdit.customerName;
           addressDataTB.Text = _customerPaymentsToEdit.customerAddress;
           custIdTB.Text = _customerPaymentsToEdit.customerId;
           paymentAmountDataTB.Text = _customerPaymentsToEdit.paymentAmount.ToString();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {


            #region checking for proper value of payment amount to exclude special characters..

            string errorMsg;
            double paymentAmount;

            if (ValidationHandler.CheckDoubleToStringFormatError(out paymentAmount, paymentAmountDataTB.Text, " Payment Amount", out errorMsg))
                _customerPaymentsToEdit.paymentAmount = paymentAmount;
            else
            {
                paymentAmountDataTB.Text = string.Empty;
                MessageBox.Show(errorMsg);
                return;
            }

            #endregion

            //_customerPaymentsToEdit.paymentAmount = Convert.ToDouble( paymentAmountDataTB.Text);

            if (OnEditCustomersPaymentData != null)
                OnEditCustomersPaymentData(_customerPaymentsToEdit);

           SptStorage.DbInteraction.EditCustomerPayments(_customerPaymentsToEdit);
            this.Close();
        }

      

        #region validation
        private void paymentAmountDataTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationHandler.onlyNumeric(e.Text);
        }
        #endregion


    }


}
