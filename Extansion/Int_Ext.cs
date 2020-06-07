using System;
using System.Collections.Generic;
using System.Text;

namespace Extansion
{
    namespace Int
    {
        public static class Int_Ext
        {
            public static int RotateLeft(this int value, int count)
            {
                return (int)((value << count) | (value >> (32 - count)));
            }

            public static int RotateRight(this byte value, int count)
            {
                return (int)((value >> count) | (value << (32 - count)));
            }

            public static int SetBit(this int value, int NewValue, int Digit)
            {
                if (Digit < 0 || Digit > 32)
                    throw new ArgumentOutOfRangeException("Digit must be between 0 and 32");

                if (NewValue == 0)
                    return (int)(value & (Convert.ToInt32(0xFFFFFFFE)).RotateLeft(Digit));
                else
                    return (int)(value | ((int)0x00000001).RotateLeft(Digit));
            }

            public static int SetBit(this byte value, bool NewValue, int Digit)
            {
                if (Digit < 0 || Digit > 32)
                    throw new ArgumentOutOfRangeException("Digit must be between 0 and 32");

                if (!NewValue)
                    return (int)(value & (Convert.ToInt32(0xFFFFFFFE)).RotateLeft(Digit));
                else
                    return (int)(value | 0x00000001.RotateLeft(Digit));
            }

            /// <summary>
            /// Überprüft ob der Wert über einem bestimmten Mindestwert liegt. Wenn nicht, wird der Mindestwert zurückgegeben
            /// </summary>
            /// <param name="value">Der Wert, der überprüft wird</param>
            /// <param name="MinValue">Der Wert, der nicht unterschritten werden darf</param>
            /// <returns>Der Wert, der entweder dem Mindestwert oder darüber entspricht</returns>
            public static int Min(this int value, int MinValue)
            {
                return value < MinValue ? MinValue : value;
            }

            /// <summary>
            /// Überprüft ob der Wert unter einem bestimmten Maximalwert liegt. Wenn nicht, wird der Maximalwert zurückgegeben
            /// </summary>
            /// <param name="value">Der Wert, der überprüft wird</param>
            /// <param name="MinValue">Der Wert, der nicht überschritten werden darf</param>
            /// <returns>Der Wert, der entweder dem Maximalwert oder darunter entspricht</returns>
            public static int Max(this int value, int MaxValue)
            {
                return value > MaxValue ? MaxValue : value;
            }

            /// <summary>
            /// Stellt sicher, dass der Wert in einem bestimmten Beriech liegt.
            /// Ist er größer, wird der Max Wert zurückgegeben, ist er kleiner, der Min Wert
            /// </summary>
            /// <param name="Value">Der Wert der überprüft wird</param>
            /// <param name="Min">Der Mindestwert. Die untere Gränze des gültigen Bereiches</param>
            /// <param name="Max">Der Maximalwert. Die obere Gränze des gültigen Bereiches</param>
            /// <returns></returns>
            public static int Range(this int value, int MinValue, int MaxValue)
            {
                return value > MaxValue ? MaxValue : (value < MinValue ? MinValue : value);
            }

            /// <summary>
            /// Zählt wie viele stellen die Zahl hat
            /// </summary>
            /// <param name="value">Die Zahl deren Stellen gezählt werden sollen</param>
            /// <returns></returns>
            public static int Digits(this int value)
            {
                int count = 0;
                do
                {
                    count++;
                    value /= 10;
                } while (value != 0);
                return count;
            }

            public static string ToASMString(this int value)
            {
                if(value < 256)
                    return value.ToString("X2");
                if (value < 65536)
                    return value.ToString("X4");
                return value.ToString("X6");
            }
        }
    }
}
