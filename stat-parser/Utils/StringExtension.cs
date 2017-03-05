using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class StringExtension
    {
        public static decimal? ConvertEfficiency(this string str)
        {
            if (str.Equals("N/A"))
                return null;

            if (str.EndsWith("%"))
                str = str.TrimEnd(new char[] { '%' });

            if (str.StartsWith("%"))
                str = str.TrimStart(new char[] { '%' });

            decimal convertedValue;
            decimal.TryParse(str.Trim(), out convertedValue);
            return convertedValue/100M;
        }

        public static int ConvertToInt(this string str)
        {
            int convertedValue;
            int.TryParse(str.Trim(), out convertedValue);
            return convertedValue;
        }

    }
}
