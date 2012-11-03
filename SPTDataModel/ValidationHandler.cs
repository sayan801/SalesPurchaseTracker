using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SPTDataModel
{
    public static class ValidationHandler
    {
        public static bool onlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        public static bool onlyAlphabet(string text)
        {
            Regex regex = new Regex("[^A-Z|^a-z|^ |^\t]"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        public static bool onlyAlphaNumeric(string text)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]*$"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        public static bool CheckDoubleToStringFormatError(out double destinationDouble, string sourceDoubleString, string fieldName, out string msg)
        {
            msg = string.Empty;
            destinationDouble = 0.0;

            try
            {
                destinationDouble = Convert.ToDouble(sourceDoubleString);
            }
            catch (FormatException ex)
            {
                msg = "Error in data format of " + fieldName + " : " + ex.Message;
                return false;
            }

            return true;
        }

    }
}
