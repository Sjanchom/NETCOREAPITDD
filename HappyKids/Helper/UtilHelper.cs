using System;
using System.Globalization;

namespace HappyKids.Helper
{
    public class UtilHelper
    {
        public static DateTime PareDateTime(string dateTime)
        {
            return DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
