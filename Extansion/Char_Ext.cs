using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extansion
{
    namespace Char
    {
        public static class Char_Ext
        {
            public static bool IsSpace(this char c)
            {
                return (c == ' ') ||
                    (c == '\t') ||
                    (c == '\n');
            }

            public static bool IsDigit(this char c)
            {
                return (c == '0') ||
                    (c == '1') ||
                    (c == '2') ||
                    (c == '3') ||
                    (c == '4') ||
                    (c == '5') ||
                    (c == '6') ||
                    (c == '7') ||
                    (c == '8') ||
                    (c == '9');
            }
        }
    }
}
