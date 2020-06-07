using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Extansion.Enum
{
    public static class EnumExt
    {
        public static IEnumerable<System.Enum> GetFlags(this System.Enum input)
        {
            foreach (System.Enum value in System.Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }
    }
}
