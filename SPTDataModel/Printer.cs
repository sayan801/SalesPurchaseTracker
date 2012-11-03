using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SPTDataModel
{
   public static class Printer
    {
       public static void PrintArea(System.Windows.Media.Visual areaToPrint, string titleString)
       {
           System.Windows.Controls.PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
           if (printDlg.ShowDialog() == true)
           {
               printDlg.PrintVisual(areaToPrint, titleString);
           }
       }
    }
}
