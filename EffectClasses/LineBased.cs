using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EffectClasses
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class LineBased<T>
	{
		/// <summary>
		/// How many lines should be effected by T
		/// </summary>
		public int LineCount { get; set; }
		/// <summary>
		/// What should be
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lineCount"></param>
		/// <param name="value"></param>
		public LineBased(int lineCount, T value)
		{
			LineCount = lineCount;
			Value = value;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ListLineBased<T> : IList<LineBased<T>>
	{
		List<LineBased<T>> _list = new List<LineBased<T>>();


		public int TotalLineCount { get { return _list.Sum(l => l.LineCount); } }



		public int IndexOf(LineBased<T> item) { return _list.IndexOf(item); }
		public void Insert(int index, LineBased<T> item) { _list.Insert(index, item); }
		public void RemoveAt(int index) { _list.RemoveAt(index); }

		public LineBased<T> this[int index]
		{
			get { return _list[index]; }
			set { _list[index] = value; }
		}

		public void Add(LineBased<T> item) { _list.Add(item); }
		public void Clear() { _list.Clear(); }
		public bool Contains(LineBased<T> item) { return _list.Contains(item); }
		public void CopyTo(LineBased<T>[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }
		public int Count { get { return _list.Count; } }
		public bool IsReadOnly { get { return false; } }
		public bool Remove(LineBased<T> item) { return _list.Remove(item); }

		public IEnumerator<LineBased<T>> GetEnumerator() { return _list.GetEnumerator(); }

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _list.GetEnumerator(); }
	}
}
