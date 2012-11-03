using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.Xml;
using OfficeOpenXml.Style;
using System.Collections.ObjectModel;
using SPTDataModel;

namespace InputVatCalculation
{
    public static class Export2xlsx
    {

        public static void SaveToExcel(ObservableCollection<IpVatData> inputVatCollection)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "InputVatCalculation-" + DateTime.Now.ToOADate().ToString(); // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                FileInfo newFile = new FileInfo(filename);
                if (newFile.Exists)
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(filename);
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("SPTInputVat");
                    //Add the headers
                    int i = 1;
                    int j = 1;

                    ws.Cells[i, j++].Value = "Serial No";
                    ws.Cells[i, j++].Value = "Date";
                    ws.Cells[i, j++].Value = "invoiceNo";
                    ws.Cells[i, j++].Value = "vendorName";
                    ws.Cells[i, j++].Value = "vendorVatNo";
                    ws.Cells[i, j++].Value = "quantity";
                    ws.Cells[i, j++].Value = "pricePerUnit";
                    ws.Cells[i, j++].Value = "vatRate";
                    ws.Cells[i, j++].Value = "totalPrice";
                    ws.Cells[i, j++].Value = "vatTotal";
                    ws.Cells[i, j++].Value = "totalAmount";

                    i++;
                    double totalVat = 0.0;
                    double totalAmount = 0.0;
                    foreach (IpVatData ipv in inputVatCollection)
                    {
                        j = 1;
                        ws.Cells[i, j++].Value = (i - 1).ToString();
                        ws.Cells[i, j++].Value = ipv.date.ToString();
                        ws.Cells[i, j++].Value = ipv.invoiceNo;
                        ws.Cells[i, j++].Value = ipv.vendorName;
                        ws.Cells[i, j++].Value = ipv.vendorVatNo;
                        ws.Cells[i, j++].Value = ipv.quantity;
                        ws.Cells[i, j++].Value = ipv.pricePerUnit;
                        ws.Cells[i, j++].Value = ipv.vatRate;
                        ws.Cells[i, j++].Value = ipv.totalPrice;
                        ws.Cells[i, j++].Value = ipv.vatTotal;
                        ws.Cells[i, j].Value = ipv.totalAmount;
                        i++;
                        totalAmount += Convert.ToDouble(ipv.totalAmount);
                        totalVat += Convert.ToDouble(ipv.vatTotal);
                    }
                    ws.Cells[i, j - 1].Value = "totalVAT";
                    ws.Cells[i++, j].Value = totalVat;

                    ws.Cells[i, j - 1].Value = "totalAmount";
                    ws.Cells[i, j].Value = totalAmount;

                    package.Save();
                }
            }
        }

    }
}
