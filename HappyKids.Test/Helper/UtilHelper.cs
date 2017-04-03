using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HappyKids.Test.Helper
{
    public class UtilHelper
    {
        public static DateTime PareDateTime(string dateTime)
        {
            return DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
