using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extansion
{
	namespace Enumerable
	{
		static class Enumerable_Ext
		{
			public static T Random<T>(this IEnumerable<T> list)
			{
				return list.ElementAt(new Random().Next(list.Count()));
			}
			public static T Random<T>(this IEnumerable<T> list, Func<T, bool> predicate)
			{
				IEnumerable<T> sublist = list.Where(predicate);
				return sublist.ElementAt(new Random().Next(sublist.Count()));
			}
			public static T Random<T>(this IEnumerable<T> list, Func<T, int, bool> predicate)
			{
				IEnumerable<T> sublist = list.Where(predicate);
				return sublist.ElementAt(new Random().Next(sublist.Count()));
			}

			public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
			{
				List<T> Storage = new List<T>(list);
				List<T> Ret = new List<T>(list);
				int cnt = 0;

				while (Storage.Count() != 0)
				{
					int i = new Random().Next(Storage.Count());
					Ret[cnt] = Storage[i];
					Storage.RemoveAt(i);
					cnt++;
				}
				return Ret;
			}
		}
	}
}
