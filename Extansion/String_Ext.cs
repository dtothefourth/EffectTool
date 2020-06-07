using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Extansion
{
    namespace String
    {
        public static class String_Ext
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static int ToHexInt(this string str)
            {
                str = str.Trim();
				str = str.TrimStart('d', 'b', ' ');
                if (str.StartsWith("$"))
                {
                    return Convert.ToInt32(str.Substring(1), 16);
                }
                else if (str.StartsWith("0x"))
                {
                    return Convert.ToInt32(str.Substring(2), 16);
                }
                else
                {
                    return Convert.ToInt32(str);
                }
            }

            public static bool IsDigit(this string input)
            {
                return Regex.IsMatch(input, @"^\d+$");
            }
        }
    }
}
