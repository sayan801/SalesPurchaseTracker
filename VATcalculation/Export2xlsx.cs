using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.Xml;
using OfficeOpenXml.Style;
using System.Collections.ObjectModel;
using SPTDataModel;

namespace OutputVatCalculation
{
    public static class Export2xlsx
    {

        public static void SaveToExcel(ObservableCollection<OpVatData> outputVatCollection)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "OutputVatCalculation-" + DateTime.Now.ToOADate().ToString(); // Default file name
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
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("SPTOutputVat");
                    //Add the headers
                    int i = 1;
                    int j = 1;

                    ws.Cells[i, j++].Value = "Serial No";
                    ws.Cells[i, j++].Value = "Date";
                    ws.Cells[i, j++].Value = "invoiceNo";
                    ws.Cells[i, j++].Value = "CustomerName";
                    ws.Cells[i, j++].Value = "CustomerVatNo";
                    ws.Cells[i, j++].Value = "quantity";
                    ws.Cells[i, j++].Value = "pricePerUnit";
                    ws.Cells[i, j++].Value = "vatRate";
                    ws.Cells[i, j++].Value = "totalPrice";
                    ws.Cells[i, j++].Value = "vatTotal";
                    ws.Cells[i, j++].Value = "totalAmount";                   

                    i++;

                    double totalVat = 0.0;
                    double totalAmount = 0.0;

                    foreach (OpVatData opv in outputVatCollection)
                    {
                        j = 1;
                        ws.Cells[i, j++].Value = (i-1).ToString();
                        ws.Cells[i, j++].Value = opv.date.ToString();
                        ws.Cells[i, j++].Value = opv.invoiceNo;
                        ws.Cells[i, j++].Value = opv.customerName;
                        ws.Cells[i, j++].Value = opv.customerVatNo;
                        ws.Cells[i, j++].Value = opv.quantity;
                        ws.Cells[i, j++].Value = opv.pricePerUnit;
                        ws.Cells[i, j++].Value = opv.vatRate;
                        ws.Cells[i, j++].Value = opv.totalPrice;
                        ws.Cells[i, j++].Value = opv.vatTotal;
                        ws.Cells[i, j].Value = opv.totalAmount;
                        i++;
                        totalAmount += Convert.ToDouble(opv.totalAmount);
                        totalVat += Convert.ToDouble(opv.vatTotal);
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
