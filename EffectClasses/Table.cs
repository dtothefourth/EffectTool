using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EffectClasses
{
	/// <summary>
	/// Describes what format the values of an ASMTableEntry have or in what format they should be written
	/// </summary>
	public enum TableValueType
	{
		/// <summary>
		/// Value is one byte long or will be written in seperate bytes
		/// </summary>
		db = 1,
		/// <summary>
		/// Value is one word (2 bytes) long or will be written in two byte patterns
		/// </summary>
		dw = 2,
		/// <summary>
		/// Values is long (3 bytes) or will be written in three byte patterns
		/// </summary>
		dl = 3,
		/// <summary>
		/// Values is double (4 bytes) or will be written in four byte patterns
		/// </summary>
		dd = 4,
		/// <summary>
		/// Each value entry will be used for one scanline, the lenght can vary.
		/// <para>This cannot be used to write tables.</para>
		/// </summary>
		Single = 5,
		/// <summary>
		/// Last entry in a table, there are no values nor scanlines 
		/// </summary>
		End = 0
	};

	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum TableValueStringArgument
	{
		/// <summary>
		/// Writes a dollar symbol infront of all the values and the scanline
		/// </summary>
		Dollar = 1,
		/// <summary>
		/// Writes the db, dw or dl and adds a colon when the type changes
		/// </summary>
		Direct = 2,
		/// <summary>
		/// Writes a comma + space inbetween the single values and scanline
		/// </summary>
		Comma = 4,
		/// <summary>
		/// A combination of Dollar, Direct and Comma
		/// </summary>
		Standart = 7,
	}
	
	/// <summary>
	/// One entry of an ASMTable.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{ToString()}")]
	public class ASMTableEntry : ICloneable, IEquatable<ASMTableEntry>, IEnumerable<byte>
	{
		public virtual byte[] Values { get; set; }

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ASMTableEntry()
		{
			Values = new byte[0];
		}
		/// <summary>
		/// Creates a new instance of an ASMTableEntry 
		/// </summary>
		/// <param name="values"></param>
		public ASMTableEntry(params byte[] values)
		{
			Values = values;
		}

		/// <summary>
		/// Creates a copy of the ASMTableEntry
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			return new ASMTableEntry(this.Values);
		}
		
		public virtual IEnumerator<byte> GetEnumerator()
		{
			foreach (byte b in Values)
				yield return b;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// checks if two ASMTableEntries are equal to each other.
		/// </summary>
		/// <param name="obther">The other entry to be compared</param>
		/// <returns><c>True</c> if they are equal</returns>
		public virtual bool Equals(ASMTableEntry other)
		{
			return this.Values.SequenceEqual(other.Values);
		}

		/// <summary>
		/// checks if an object is equal to this ASMTableEntry
		/// </summary>
		/// <param name="obj">The object to be compared</param>
		/// <returns><c>True</c> if they are equal</returns>
		public override bool Equals(object obj)
		{
			ASMTableEntry e = obj as ASMTableEntry;
			if(e == null)
				return false;
			return Equals(e);
		}

		/// <summary>
		/// Get the hash code based on the values
		/// </summary>
		/// <returns>A hash code to identify the object.</returns>
		public override int GetHashCode()
		{
			int sum = 0;
			foreach (byte b in Values)
				sum += b;
			return sum;
		}

		/// <summary>
		/// Creates an ASM conform string for an table entry from the stored values.
		/// </summary>
		/// <returns>The combined string of thr values</returns>
		public override string ToString()
		{
			return ToString(TableValueType.db);
		}

		/// <summary>
		/// Creates an ASM conform string for an table entry from the stored values.
		/// </summary>
		/// <param name="type">Which format to use for the table.</param>
		/// <returns>The combined string of thr values</returns>
		public virtual string ToString(TableValueType type)
		{
			if (type == TableValueType.End || type == TableValueType.Single)
				throw new ASMException(type + " can only be used on HDMATableEntry's");

			if (Values.Length % (int)type != 0)
				throw new ASMException("Unmatching value count for the use of " + type);

			string whole = type.ToString() + " ";
			for (int i = 0; i < Values.Length; i += (int)type)
			{
				string one = "";
				for (int j = i; (j - i) < (int)type && j < Values.Length; j++)
				{
					one = Values[j].ToString("X2") + one;
				}
				whole += "$" + one + ", ";
			}

			return whole.Substring(0, whole.Length - 2);
		}
	}


	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	public class ASMTable : ICloneable, IList<ASMTableEntry>, ICodeProvider
	{
		protected List<ASMTableEntry> _entries = new List<ASMTableEntry>();
		public string Name { get; set; }

		public ASMTable() { }
		public ASMTable(string name)
		{
			Name = name;
		}
		public ASMTable(IEnumerable<ASMTableEntry> entries)
		{
			_entries = new List<ASMTableEntry>(entries);
		}
		public ASMTable(string name, IEnumerable<ASMTableEntry> entries)
		{
			Name = name;
			_entries = new List<ASMTableEntry>(entries);
		}


		public virtual object Clone()
		{
			return new ASMTable(this.Name, this._entries);
		}

		#region IList

		public int IndexOf(ASMTableEntry item)
		{
			return _entries.IndexOf(item);
		}

		public void Insert(int index, ASMTableEntry item)
		{
			_entries.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_entries.RemoveAt(index);
		}

		public ASMTableEntry this[int index]
		{
			get
			{
				return _entries[index];
			}
			set
			{
				_entries[index] = value;
			}
		}

		public void Add(ASMTableEntry item)
		{
			_entries.Add(item);
		}

		public void Clear()
		{
			_entries.Clear();
		}

		public bool Contains(ASMTableEntry item)
		{
			return _entries.Contains(item);
		}

		public void CopyTo(ASMTableEntry[] array, int arrayIndex)
		{
			_entries.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _entries.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(ASMTableEntry item)
		{
			return _entries.Remove(item);
		}

		public IEnumerator<ASMTableEntry> GetEnumerator()
		{
			foreach (var item in _entries)
				yield return item;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public string Code()
		{
			ASMCodeBuilder code = new ASMCodeBuilder();
			code.AppendLabel(Name);
			foreach (var entry in _entries)
				code.AppendCode(entry.ToString());
			return code.ToString();

		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{ToString()}")]
	public class HDMATableEntry : ICloneable, IEquatable<HDMATableEntry>
	{
		/// <summary>
		/// The last table entry in an HDMA table
		/// </summary>
		public static HDMATableEntry End { get { return new HDMATableEntry(TableValueType.End); } }

		//fixme exception
		private Exception EndValueException = new Exception("TableEntries of type End can neither have values nor scanlines");
		private Exception SingleMismatchException = new Exception("TableEntries of type Single have to have as many values as scanlines - 128");
		private Exception SingleScanlinesExcpetion = new Exception("TableEntries of type Single need to have at least 129 scanlines");

		private byte[] _values;
		private byte _scanlines;

		/// <summary>
		/// Defines what type of value this entry stores
		/// </summary>
		public TableValueType ValueType { get; private set; }
		/// <summary>
		/// The number of scanlines that should be effected
		/// </summary>
		public byte Scanlines
		{
			get { return _scanlines; }
			set
			{
				if (ValueType == TableValueType.End && value != 0)
					throw EndValueException;
				else if (ValueType == TableValueType.Single && value - 0x80 != Values.Length)
					throw SingleMismatchException;
				_scanlines = value;
			}
		}
		/// <summary>
		/// The Values that are used for the effect. How many values can be set depends on ValueType.
		/// </summary>
		public byte[] Values
		{
			get { return _values; }
			set
			{
				if (ValueType == TableValueType.Single && (Scanlines - 0x80 != value.Length))
					throw SingleMismatchException;
				else if (ValueType == TableValueType.End && value.Length != 0)
					throw EndValueException;
				_values = value;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		public HDMATableEntry(TableValueType type)
		{
			ValueType = type;
			switch (type)
			{
				case TableValueType.End:
					Scanlines = 0;
					Values = new byte[0];
					break;
				case TableValueType.db:
					Scanlines = 1;
					Values = new byte[] { 0 };
					break;
				case TableValueType.dw:
					Scanlines = 1;
					Values = new byte[] { 0, 0 };
					break;
				case TableValueType.dl:
					Scanlines = 1;
					Values = new byte[] { 0, 0, 0 };
					break;
				case TableValueType.Single:
					Scanlines = 0x81;
					Values = new byte[] { 0 };
					break;
				default:
					throw new NotImplementedException("Enum type: " + type.ToString() + " not recognized");
			}

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="scanlines"></param>
		/// <param name="values"></param>
		public HDMATableEntry(TableValueType type, byte scanlines, params byte[] values)
		{
			if (type == TableValueType.End && (scanlines != 0 || values.Length != 0))
				throw EndValueException;
			if (type == TableValueType.Single && scanlines <= 0x80)
				throw SingleScanlinesExcpetion;
			else if (type == TableValueType.Single && values.Length != (scanlines - 0x80))
				throw SingleMismatchException;

			/*if (scanlines == 0)
			{
				ValueType = TableValueType.End;
				_scanlines = 0;
				_values = new byte[0];
				return;
			}*/

			_scanlines = scanlines;
			ValueType = type;
			Values = values;
		}

		#region Object Methods

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(ValueType);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string ToString(TableValueType type)
		{
			if (Values.Length == 0)
				return "db $" + Scanlines.ToString("X2");

			string str = "db $" + Scanlines.ToString("X2") + " : ";
			ASMTableEntry vals = new ASMTableEntry(this.Values);
			return str + vals.ToString(type);
		}

		/// <summary>
		/// Compares and object to the current object and checks if they have the same content
		/// </summary>
		/// <param name="obj">The object to compare with</param>
		/// <returns>True if the members of both HDMATableEntries are the same</returns>
		public override bool Equals(object obj)
		{
			HDMATableEntry t = obj as HDMATableEntry;
			if (!(obj is HDMATableEntry))
				return false;
			else if (ValueType != t.ValueType)
				return false;
			else if (Scanlines != t.Scanlines)
				return false;
			else if (!Values.SequenceEqual(t.Values))
				return false;
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion

		/// <summary>
		/// Compares two HDMATableEntry values and checks if they are equal down to the last entry.
		/// </summary>
		/// <param name="t1">The first object to be compared</param>
		/// <param name="t2">The second object to be compared</param>
		/// <returns><c>True</c> if they are equal</returns>
		public static bool operator ==(HDMATableEntry t1, HDMATableEntry t2)
		{
			if (object.ReferenceEquals(t1, t2))
				return true;
			if (object.ReferenceEquals(t1, null))
				return false;
			if (object.ReferenceEquals(t2, null))
				return false;

			return t1.Equals(t2);
		}
		/// <summary>
		/// Compares two HDMATableEntry values and checks if they are NOT equal even in only on entry.
		/// </summary>
		/// <param name="t1">The first object to be compared</param>
		/// <param name="t2">The second object to be compared</param>
		/// <returns><c>True</c> if the objects are not the same.</returns>
		public static bool operator !=(HDMATableEntry t1, HDMATableEntry t2)
		{
			return !(t1 == t2);
		}

		public IEnumerator<byte> GetEnumerator()
		{
			yield return Scanlines;
			foreach (byte b in Values)
				yield return b;
		}
		
		#region ICloneable Member

		/// <summary>
		/// Creates a dublicate of the current objects and all it's members
		/// </summary>
		/// <returns>An object that needs to be casted containing the same members as this</returns>
		public object Clone()
		{
			HDMATableEntry entry = (HDMATableEntry)this.MemberwiseClone();
			byte[] newvals = new byte[Values.Length];
			Array.Copy(Values, newvals, Values.Length);
			entry.Values = newvals;
			return entry;
		}

		#endregion

		#region IEquatable<HDMATableEntry> Member

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(HDMATableEntry other)
		{
			return this.Equals((object)other);
		}

		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	public class HDMATable : IList<HDMATableEntry>, ICloneable, ICodeProvider
	{
		private List<HDMATableEntry> _entries = new List<HDMATableEntry>();
		/// <summary>
		/// The total number of scanlines the table takes up.
		/// </summary>
		public int TotalScanlines
		{
			get
			{
				int suma = _entries.Where(t => t.ValueType != TableValueType.Single).Sum(t => t.Scanlines);
				int sumb = _entries.Where(t => t.ValueType == TableValueType.Single).Sum(t => t.Scanlines - 0x80);
				return suma + sumb;
			}
		}
		/// <summary>
		/// The total number of bytes the table will take up when inserted into the ROM
		/// </summary>
		public int TotalBytes { get { return _entries.Sum(e => e.Values.Length) + _entries.Count; } }

		public string Name { get; set; }

		public HDMATable()
		{
			Name = "";
			_entries = new List<HDMATableEntry>();
		}
		public HDMATable(string name)
		{
			Name = name;
		}
		public HDMATable(IEnumerable<HDMATableEntry> entries)
		{
			Name = "";
			_entries = new List<HDMATableEntry>(entries);
		}
		public HDMATable(string name, IEnumerable<HDMATableEntry> entries)
		{
			Name = name;
			_entries = new List<HDMATableEntry>(entries);
		}


		public static bool Merge(HDMATable Table1, HDMATable Table2, out HDMATable Merged)
		{
			Merged = null;
			if (Table1.Count != Table2.Count)
				return false;

			if (Table1.SequenceEqual(Table2, new HDMATableEntryComparer()))
			{
				Merged = new HDMATable();
				for (int i = 0; i < Table1.Count; i++)
				{
					List<byte> values = new List<byte>();
					values.AddRange(Table1[i].Values);
					values.AddRange(Table2[i].Values);
					Merged.Add(new HDMATableEntry(TableValueType.db, Table1[i].Scanlines, values.ToArray()));
				}
				if (!Merged.HasEnded())
					Merged.Add(HDMATableEntry.End);
				return true;
			}
			return false;
		}

		private class HDMATableEntryComparer : IEqualityComparer<HDMATableEntry>
		{
			public bool Equals(HDMATableEntry x, HDMATableEntry y)
			{
				return x.Scanlines == y.Scanlines;
			}

			public int GetHashCode(HDMATableEntry obj)
			{
				return obj.GetHashCode();
			}
		}


		/// <summary>
		/// Indicates if the table already has an End entry at it's end
		/// </summary>
		/// <returns></returns>
		public bool HasEnded()
		{
			if (_entries.Count == 0)
				return false;
			return _entries[_entries.Count - 1].ValueType == TableValueType.End;
		}

		public string Code()
		{
			return ToString();
		}

		public override string ToString()
		{
			ASMCodeBuilder code = new ASMCodeBuilder();
			if (Name == "")
				code.AppendLabel("Table" + GetHashCode().ToString("X"));
			else
				code.AppendLabel(Name);
			foreach (var item in _entries)
				code.AppendCode(item.ToString());
			return code.ToString();
		}

		public string ToString(string tableName)
		{
			ASMCodeBuilder code = new ASMCodeBuilder();
			code.AppendLabel(tableName);
			foreach (var item in _entries)
				code.AppendCode(item.ToString());
			return code.ToString();
		}

		public void AddRange(IEnumerable<HDMATableEntry> collection)
		{
			_entries.AddRange(collection);
		}

		#region IList<HDMATableEntry> Member
		/// <summary>
		/// Gets the first index of a certain element within the table
		/// </summary>
		/// <param name="item">The HDMATableEntry to be found in the table</param>
		/// <returns>The indes of the first match </returns>
		public int IndexOf(HDMATableEntry item) { return _entries.IndexOf(item); }

		/// <summary>
		/// Insert an entry somewhere in the table.
		/// <para>Trying to insert an End entry anywhere other than at the end will result in an excpetion</para>
		/// </summary>
		/// <param name="index">The position the element should be added in</param>
		/// <param name="item">The entry to be added to the table</param>
		public void Insert(int index, HDMATableEntry item)
		{
			if (item.ValueType == TableValueType.End || index != _entries.Count)
				//fixme excpetion
				throw new Exception();
			_entries.Insert(index, item);
		}

		/// <summary>
		/// Remove an element at a certain position within the table
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index) { _entries.RemoveAt(index); }

		/// <summary>
		/// Gets or sets the entry at a certain position
		/// </summary>
		/// <param name="index">The position at which an entry should be edited</param>
		/// <returns>The to be edited entry</returns>
		public HDMATableEntry this[int index]
		{
			get { return _entries[index]; }
			set { _entries[index] = value; }
		}

		#endregion

		#region ICollection<HDMATableEntry> Member

		/// <summary>
		/// Add new tableentry at the end of the table
		/// </summary>
		/// <param name="item">entry to be added</param>
		/// <exception cref="System.Exception">Thrown when you try to add entries after an End-entry</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if item is null</exception>
		public void Add(HDMATableEntry item)
		{
			if (item == null)
				throw new ArgumentNullException("Can't add null to HDMATable");
			if (_entries.Count != 0)
				if (_entries[_entries.Count - 1].ValueType == TableValueType.End)
					//fixme excpetion
					throw new Exception();
			_entries.Add(item);
		}

		/// <summary>
		/// Remove all entries from the table
		/// </summary>
		public void Clear() { _entries.Clear(); }

		/// <summary>
		/// Checks if the table contains a certain entry.
		/// </summary>
		/// <param name="item">The entry that should be checked</param>
		/// <returns>True if the table does contain said entry</returns>
		public bool Contains(HDMATableEntry item)
		{
			if (item == null)
				return false;
			return _entries.Contains(item);
		}

		/// <summary>
		/// Copies the table into an array of HDMATableEntries and starts at a specific index
		/// </summary>
		/// <param name="array">The array to be filled with the entries</param>
		/// <param name="arrayIndex">The index from which the data will be copied into the array</param>
		public void CopyTo(HDMATableEntry[] array, int arrayIndex) { _entries.CopyTo(array, arrayIndex); }

		/// <summary>
		/// How many tableentries are there
		/// </summary>
		public int Count { get { return _entries.Count; } }

		/// <summary>
		/// Whether or not data can only be read
		/// </summary>
		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// Removes the first instance of an item from the table
		/// </summary>
		/// <param name="item">item to be removed</param>
		/// <returns>True if an item was successfully removed</returns>
		public bool Remove(HDMATableEntry item) { return _entries.Remove(item); }

		#endregion

		#region IEnumerable<HDMATableEntry> Member

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<HDMATableEntry> GetEnumerator()
		{
			foreach (HDMATableEntry ent in _entries)
				yield return ent;
		}

		#endregion

		#region IEnumerable Member

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

		#endregion

		#region ICloneable Member
		/// <summary>
		/// Creates a HDMATable instance that has the exact same values as this one
		/// </summary>
		/// <returns>The object that can be casted to an HDMATable</returns>
		public object Clone()
		{
			HDMATable table = new HDMATable(this.Name, this._entries);
			return table;
		}

		#endregion
	}
}
