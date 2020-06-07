using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

using Extansion.Int;
using Extansion.String;

namespace EffectClasses
{
	/// <summary>
	/// Describes a 5bit color and where it is located vertically on the screen.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Position = {Position}, 5bit = {Value5Bit}, 8bit = {Value8Bit}")]
	public class ColorPosition : IComparable<ColorPosition>, IEquatable<ColorPosition>, ICloneable
	{
		private byte _val5bit = 0;
		private byte _val8bit = 0;
		/// <summary>
		/// Contains the originally set 8 bit value
		/// </summary>
		public byte Orignal8Bit { get; private set; }

		/// <summary>
		/// The 5bit value of the color used for the ASM
		/// </summary>
		public byte Value5Bit
		{
			get { return _val5bit; }
			set
			{
				if (value < 0 || value > 31)
					throw new ArgumentOutOfRangeException("5bit value cannot be smaller than 0 or bigger than 31");
				_val5bit = value;
				_val8bit = (byte)(value * 8);
			}
		}
		/// <summary>
		/// The 8bit value of the color used for the actual color
		/// </summary>
		public byte Value8Bit
		{
			get { return _val8bit; }
			set
			{
				if (value < 0 || value > 255)
					throw new ArgumentOutOfRangeException("8bit value cannot be smaller than 0 or bigger than 255");
				_val5bit = (byte)(value / 8);
				_val8bit = (byte)(_val5bit * 8);
				Orignal8Bit = value;
			}
		}
		/// <summary>
		/// The position the color has on the screen
		/// </summary>
		public int Position { get; set; }

		/// <summary>
		/// Creates a new instance of a ColorPosition with it's Position and Value set to 0 (zero)
		/// </summary>
		public ColorPosition() : this(0, 0) { }
		/// <summary>
		/// Creates a new instance of a ColorPosition with it's position preset and value set to 0 (zero)
		/// </summary>
		/// <param name="position">The position the color has on the screen</param>
		public ColorPosition(int position) : this(position, 0) { }
		/// <summary>
		/// Creates a new instance of a ColorPosition with it's Position and Value set to defined values
		/// </summary>
		/// <param name="position">The vertical position of the color on the screen</param>
		/// <param name="value8Bit">The 8bit value of the color</param>
		public ColorPosition(int position, byte value8Bit)
		{
			Orignal8Bit = value8Bit;
			Position = position;
			Value8Bit = value8Bit;
		}

		/// <summary>
		/// Compares two ColorPositions with each other and orders them by their position on the screen.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ColorPosition other)
		{
			if (other == null)
				return -1;
			else if (this.Position > other.Position)
				return 1;
			else if (this.Position == other.Position)
				return 0;
			else
				return -1;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ColorPosition other)
		{
			if (Position != other.Position)
				return false;
			if (Value5Bit != other.Value5Bit)
				return false;
			return true;
		}

		#region ICloneable Member

		/// <summary>
		/// Creates a ColorPosition element with the exact same values as this one.
		/// </summary>
		/// <returns>The newly created element</returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}

	/// <summary>
	/// A sorted collection of ColorPosition elements. Any newly added element will be sorted imediatelly as well
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	public sealed class ColorPositionCollection : IList<ColorPosition>
	{
		private List<ColorPosition> _colors;

		#region Constructors

		/// <summary>
		/// Creates a new instance of a ColorPositionCollection with the standard capacity
		/// </summary>
		public ColorPositionCollection()
		{
			_colors = new List<ColorPosition>();
		}
		/// <summary>
		/// Creates a new instance of a ColorPositionCollection with a preset capacity.
		/// </summary>
		/// <param name="capacity">The default capacity of the collection</param>
		public ColorPositionCollection(int capacity)
		{
			_colors = new List<ColorPosition>(capacity);
		}
		/// <summary>
		/// Creates a new instance of a ColorPositionCollection based of another ColorPositionCollection
		/// </summary>
		/// <param name="collection">The collection this instanze will be preinitialized with</param>
		public ColorPositionCollection(IEnumerable<ColorPosition> collection)
		{
			_colors = new List<ColorPosition>(collection);
		}

		#endregion

		#region IList<ColorPosition> Member


		/// <summary>
		/// Will search through the collection to find the index of an item with a certain position.
		/// </summary>
		/// <param name="position">The position the item should have</param>
		/// <returns>The index of the found item or -1 if non was found.</returns>
		public int IndexOf(int position)
		{ return IndexOf(position, 0, _colors.Count); }

		/// <summary>
		/// Will search through the collection to find the index of an item with a certain position. Search starts at <paramref name="index"/>
		/// </summary>
		/// <param name="position">The position the item should have</param>
		/// <param name="index">The index from which the search should start</param>
		/// <exception cref="System.IndexOutOfRangeException">Thrown if <paramref name="index"/> grows too large or small and exeeds the range of the collection</exception>
		/// <returns>The index of the found item or -1 if non was found.</returns>
		public int IndexOf(int position, int index)
		{ return IndexOf(position, index, _colors.Count - index); }

		/// <summary>
		/// Will search through the collection to find the index of an item with a certain position. Search starts at <paramref name="index"/> and looks through <paramref name="count"/> elements
		/// </summary>
		/// <param name="position">The position the item should have</param>
		/// <param name="index">The index from which the search should start</param>
		/// <param name="count">How many items should be checked</param>
		/// <exception cref="System.IndexOutOfRangeException">Thrown if <paramref name="index"/> grows too large or small and exeeds the range of the collection</exception>
		/// <returns>The index of the found item or -1 if non was found.</returns>
		public int IndexOf(int position, int index, int count)
		{
			for (int i = index; i < index + count; i++)
			{
				if (i >= _colors.Count || i < 0)
					throw new IndexOutOfRangeException("The index must be within the range of the object");
				if (_colors[i].Position == position)
					return i;
				if (_colors[i].Position > position)
					return -1;
			}
			return -1;
		}

		/// <summary>
		/// Searches through the collection to find the index of a certain ColorPosition
		/// </summary>
		/// <param name="item">The ColorPosition element that should be searched for</param>
		/// <returns>The index of the found item or -1 if non was found.</returns>
		public int IndexOf(ColorPosition item)
		{
			return _colors.IndexOf(item);
		}

		/// <summary>
		/// Inserts a new ColorPosition object into the collection at a given index
		/// </summary>
		/// <param name="index">The index the ColorPosition should be inserted at</param>
		/// <param name="item">The ColorPosition to be added to the collection</param>
		/// <exception cref="System.Exception">Thrown if the collection already contains a ColorPosition with the same Position as <paramref name="item"/></exception>
		/// <exception cref="System.IndexOutOfRangeException">Thrown if <paramref name="index"/> exeeds the range of the collection</exception>
		[Obsolete("Collection will be sorted after insertion, using Add(ColorPosition) makes more sense")]
		public void Insert(int index, ColorPosition item)
		{
			if (_colors.Any(itm => itm.Position == item.Position))
				//fixme exception
				throw new Exception();
			if (index < 0 || index >= _colors.Count)
				throw new IndexOutOfRangeException("index exeeds the range of the collection");

			_colors.Insert(index, item);
			_colors.Sort();
		}

		/// <summary>
		/// Removes a ColorPosition from the collection at a given index
		/// </summary>
		/// <param name="index">The index at which an object should be removed</param>
		/// <exception cref="System.IndexOutOfRangeException">Thrown if <paramref name="index"/> exeeds the range of the collection</exception>
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _colors.Count)
				throw new IndexOutOfRangeException("index exeeds the range of the collection");
			_colors.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the ColorPosition on a certain index.
		/// </summary>
		/// <param name="index">The index of the ColorPosition that should be changed</param>
		/// <returns>The ColorPosition at the given index</returns>
		/// <exception cref="System.IndexOutOfRangeException">Thrown if <paramref name="index"/> exeeds the range of the collection</exception>
		public ColorPosition this[int index]
		{
			get { return _colors[index]; }
			set
			{
				_colors[index] = value;
				_colors.Sort();
			}
		}

		#endregion

		#region ICollection<ColorPosition> Member

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void Add(ColorPosition item)
		{
			if (_colors.Any(itm => itm.Position == item.Position))
				//fixme exception
				throw new Exception();
			_colors.Add(item);
			_colors.Sort();
		}
		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			_colors.Clear();
		}

		/// <summary>
		/// Checks if the collection contains an item with the given position
		/// </summary>
		/// <param name="position">The position to be checked in the collection</param>
		/// <returns><c>True</c> if an item has been found</returns>
		public bool Contains(int position)
		{
			return _colors.Any(itm => itm.Position == position);
		}
		/// <summary>
		/// Checks if the collection contains a specific item
		/// </summary>
		/// <param name="item">The item to be checked in the collection</param>
		/// <returns><c>True</c> if an item has been found</returns>
		public bool Contains(ColorPosition item)
		{
			return _colors.Contains(item);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(ColorPosition[] array, int arrayIndex)
		{
			_colors.CopyTo(array, arrayIndex);
		}
		/// <summary>
		/// 
		/// </summary>
		public int Count
		{
			get { return _colors.Count; }
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(ColorPosition item)
		{
			return _colors.Remove(item);
		}

		#endregion

		#region IEnumerable<ColorPosition> Member
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ColorPosition> GetEnumerator()
		{
			foreach (ColorPosition cp in _colors)
				yield return cp;
		}

		#endregion

		#region IEnumerable Member
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

	}
	
	/// <summary>
	/// The colors that can be used for various effects regarding the register $2132
	/// </summary>
	[Flags]
	public enum ColorHDMAValues : byte
	{
		/// <summary>
		/// The HDMA uses red (adds $20 to the the 5bit values)
		/// </summary>
		Red = 0x20,
		/// <summary>
		/// The HDMA uses green (adds $40 to the the 5bit values)
		/// </summary>
		Green = 0x40,
		/// <summary>
		/// The HDMA uses blue (adds $80 to the the 5bit values)
		/// </summary>
		Blue = 0x80,
	}
	   
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ColorHDMA : HDMA
	{
		private ColorPositionCollection _colorposition;

		/// <summary>
		/// 
		/// </summary>
		public ColorPositionCollection ColorsPositions 
		{
			get
			{
				return _colorposition;
			}
			set
			{
				_colorposition = value;
				UseCollection();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override bool UsesMain { get { return false; } }
		/// <summary>
		/// 
		/// </summary>
		public ColorHDMAValues ColorEffect { get; set; }
		/// <summary>
		/// The drawn gradient effect
		/// </summary>
		public Bitmap EffectImage { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		public ColorHDMA(ColorHDMAValues color)
		{
			ColorEffect = color;
			ColorsPositions = new ColorPositionCollection();
			EffectImage = new Bitmap(256, HDMA.Scanlines);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="collection"></param>
		public ColorHDMA(ColorHDMAValues color, ColorPositionCollection collection)
		{
			ColorEffect = color;
			ColorsPositions = collection;
		}

		/// <summary>
		/// 
		/// </summary>
		internal void UseCollection()
		{
			//http://www.codeproject.com/Articles/20018/Gradients-made-easy
			List<ColorPosition> _list = new List<ColorPosition>(ColorsPositions);

			if (_list.Count < 2)
				return;

			_list.Sort();

			if (_list[0].Position != 0)
				_list.Insert(0, new ColorPosition(0, 0));
			if (_list[_list.Count - 1].Position < Scanlines)
				_list.Add(new ColorPosition(Scanlines, 0));

			List<float> positions = new List<float>();
			//positions.Add(0.0f);

			int maxpos = _list.Max(cp => cp.Position);
			foreach (ColorPosition cp in _list)
				positions.Add((float)cp.Position / (float)maxpos);
			//positions.Add(1.0f);

			List<float> factors = new List<float>();
			//factors.Add(0.0f);
			foreach (ColorPosition cp in _list)
				factors.Add((float)cp.Value8Bit / 248.0f);
			//factors.Add(1.0f);

			Bitmap bm = new Bitmap(1, maxpos);

			using (Graphics g = Graphics.FromImage(bm))
			{
				Rectangle r = new Rectangle(0, 0, bm.Width, maxpos);
				using (LinearGradientBrush lgb = new LinearGradientBrush(r, Color.FromArgb(_list.Min(cp => cp.Orignal8Bit), 0, 0), Color.FromArgb(_list.Max(cp => cp.Orignal8Bit), 0, 0), 90.0f))
				{
					Blend blend = new Blend();
					blend.Factors = factors.ToArray();
					blend.Positions = positions.ToArray();
					lgb.Blend = blend;
					g.FillRectangle(lgb, r);
				}
			}
			
			byte Compare = (byte)(bm.GetPixel(0, 0).R / 8);
			Table = new HDMATable();

			for (int y = 0, scan = 1; y < maxpos; y++, scan++)
			{
				byte fivebit = (byte)(bm.GetPixel(0, y).R / 8);
				if (Compare != fivebit || scan >= 0x80 || y == maxpos - 1)
				{
					Table.Add(new HDMATableEntry(TableValueType.db, (byte)scan, (byte)((byte)ColorEffect | Compare)));
					Compare = fivebit;
					scan = 0;
				}
				
				//fixme
				for (int x = 0; x < bm.Width; x++)
					bm.SetPixel(x, y, Color.FromArgb(
						ColorEffect.HasFlag(ColorHDMAValues.Red) ? fivebit * 8 : 0, 
						ColorEffect.HasFlag(ColorHDMAValues.Green) ? fivebit * 8 : 0, 
						ColorEffect.HasFlag(ColorHDMAValues.Blue) ? fivebit * 8 : 0));
			}
			
			Table.Add(HDMATableEntry.End);

			Bitmap retImg = new Bitmap(256, maxpos);
			using (TextureBrush brush = new TextureBrush(bm, WrapMode.Tile))
				using (Graphics gr = Graphics.FromImage(retImg))
					gr.FillRectangle(brush, 0, 0, retImg.Width, retImg.Height);

			bm.Dispose();
			EffectImage = retImg;
		}                

		/// <summary>
		/// Generates the EffectImage based on the setting of the Table.
		/// </summary>
		[Obsolete("test me")]
		internal void UpdateImage()
		{
			if(Table == null)
				return;

			Bitmap bm = new Bitmap(1, Scanlines);
			int counter = 0;
			foreach (HDMATableEntry entry in Table)
			{
				if (entry.ValueType == TableValueType.End)
					break;
				if (entry.ValueType == TableValueType.Single)
				{
					for (int y = counter; y < counter + entry.Scanlines - 0x80 && y < Scanlines; y++)
						for (int x = 0; x < bm.Width; x++)
							bm.SetPixel(x, y, Color.FromArgb(
								ColorEffect.HasFlag(ColorHDMAValues.Red) ? (entry.Values[y] & 0x1F) * 8 : 0,
								ColorEffect.HasFlag(ColorHDMAValues.Green) ? (entry.Values[y] & 0x1F) * 8 : 0,
								ColorEffect.HasFlag(ColorHDMAValues.Blue) ? (entry.Values[y] & 0x1F) * 8 : 0));
					counter += entry.Scanlines - 0x80;
				}
				else if (entry.ValueType == TableValueType.db)
				{
					for (int y = counter; y < counter + entry.Scanlines && y < Scanlines; y++)
						for (int x = 0; x < bm.Width; x++)
							bm.SetPixel(x, y, Color.FromArgb(
								ColorEffect.HasFlag(ColorHDMAValues.Red) ? (entry.Values[0] & 0x1F) * 8 : 0,
								ColorEffect.HasFlag(ColorHDMAValues.Green) ? (entry.Values[0] & 0x1F) * 8 : 0,
								ColorEffect.HasFlag(ColorHDMAValues.Blue) ? (entry.Values[0] & 0x1F) * 8 : 0));
					counter += entry.Scanlines;
				}
				else
					//fixme exception
					throw new HDMAException();
			}
			
			Bitmap retImg = new Bitmap(256, Scanlines);
			using (TextureBrush brush = new TextureBrush(bm, WrapMode.Tile))
				using (Graphics gr = Graphics.FromImage(retImg))
					gr.FillRectangle(brush, 0, 0, retImg.Width, retImg.Height);
			EffectImage = retImg;
		}

		/// <summary>
		/// Draws the image and sets the internal table based on a string array.
		/// Returns every line that couldn't be translated.
		/// </summary>
		/// <param name="Lines">The lines that will be turned into an image</param>
		/// <param name="eightBit">Whether the values should be interpreted as 8 or 5 bit.</param>
		/// <returns>An array of error containing lines.</returns>
		public string[] FromString(string[] Lines, bool eightBit)
		{
			Table = new HDMATable();
			List<string> errorLines = new List<string>();

			foreach (string s in Lines)
			{
				try
				{
					string[] values = s.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (values.Length < 2)
						throw new ArgumentException("Too few arguments.");

					byte[] bytes = new byte[values.Length - 1];
					for (int i = 1; i < values.Length; i++)
					{
						if(!eightBit)
							bytes[i - 1] = (byte)((byte)ColorEffect | (0x1F & values[i].ToHexInt()));
						else
							bytes[i - 1] = (byte)((byte)ColorEffect | values[i].ToHexInt() / 8);
					}
					if (bytes.Any(b => b < (byte)ColorEffect))
						throw new Exception("Wrong Convertion");

					Table.Add(new HDMATableEntry(TableValueType.db, (byte)values[0].ToHexInt(), bytes));

					if (Table.TotalScanlines > Scanlines)
						break;
				}
				catch(Exception ex)
				{
#if(DEBUG)
					Console.WriteLine("ColorHDMA.FromString ex: " + ex.Message);
#endif
					errorLines.Add(s);
				}
			}

			if (Table.TotalScanlines < Scanlines)
				Table.Add(new HDMATableEntry(TableValueType.db, 1, (byte)ColorEffect));

			Table.Add(HDMATableEntry.End);
			UpdateImage();
			return errorLines.ToArray();
		}


		public void FromImage(Bitmap image)
		{
			if (Table == null)
				Table = new HDMATable();
			else
				Table.Clear();

			if ((Math.Log((double)ColorEffect) / Math.Log(2)) % 1 != 0)
				throw new ArgumentException("Can't call method with ColorEffect having multiple colors.\nIt's currently set to: " + ColorEffect);

			string color = "";
			if(ColorEffect == ColorHDMAValues.Red)
				color = "R";
			else if(ColorEffect == ColorHDMAValues.Green)
				color = "G";
			else if(ColorEffect == ColorHDMAValues.Blue)
				color = "B";
			else
				throw new ArgumentException("No color selected");
			
			byte check_color = (byte)(Convert.ToInt32(typeof(Color).GetProperty(color).GetValue(image.GetPixel(0, 0), null)) / 8);

			for(int y = 1, lines = 1; y < image.Height; y++, lines++)
			{
				byte cur_color = (byte)(Convert.ToInt32(typeof(Color).GetProperty(color).GetValue(image.GetPixel(0, y), null)) / 8);
				if (cur_color != check_color || lines >= 0x80)
				{
					Table.Add(new HDMATableEntry(TableValueType.db, (byte)lines, (byte)((byte)ColorEffect | check_color)));
					lines = 0;
					check_color = cur_color;
				}                
			}

			if (Table.TotalScanlines != Scanlines)
				Table.Add(new HDMATableEntry(TableValueType.db, (byte)(Scanlines - Table.TotalScanlines), (byte)((byte)ColorEffect | check_color)));

			Table.Add(HDMATableEntry.End);
			UpdateImage();
		}

		public void FromTable(HDMATable table)
		{
			if (!table.HasEnded())
				table.Add(HDMATableEntry.End);
			Table = table;
			UpdateImage();
		}

		/// <summary>
		/// Checks if the color HDMA table contains date that verifies it's need in the final code
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			if (Table == null)
			{
				Table = new HDMATable();
				return true;
			}

			foreach (HDMATableEntry entry in Table)
				foreach (byte b in entry.Values)
					if ((b & 0x1F) != 0)
						return false;
			return true;
		}


		/// <summary>
		/// Calculates the total number of byte the code will later take up in the ROM
		/// </summary>
		/// <returns>The total amount of bytes that will be written to the RAM</returns>
		public override int CountRAMBytes()
		{
			if (!UsesMain)
				return 0;
			throw new NotImplementedException();
		}
		/// <summary>
		/// Calculates the total number of bytes the code will later take up in the ROM
		/// </summary>
		/// <returns>The total amount of bytes that will be written to the ROM</returns>
		public override int CountROMBytes()
		{
			return Table.TotalBytes;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public override string Code(int Channel, HDMATable Table, bool SA1)
		{
			string tablename = "";
			if(ColorEffect.HasFlag(ColorHDMAValues.Red))
				tablename += "Red";
			if (ColorEffect.HasFlag(ColorHDMAValues.Green))
				tablename += "Green";
			if (ColorEffect.HasFlag(ColorHDMAValues.Blue))
				tablename += "Blue";

			return ColorHDMA.Code(Channel, Table, SA1, "." + tablename + "Table");
		}

		public static string Code(int Channel, HDMATable Table, string TableName)
		{
			return ColorHDMA.Code(Channel, Table, RAM.SA1, TableName);
		}

		public static string Code(int Channel, HDMATable Table, bool SA1, string TableName)
		{
			DMAMode mode = (Table[0].Values.Length == 1 ? DMAMode.P : DMAMode.PP);
			StringBuilder sbCode = new StringBuilder();
			int channelAdd = 16 * Channel;
			string tableName = "." + TableName.TrimStart('.');

           // sbCode.AppendLine("init:");
            sbCode.AppendLine("\tREP #$20");
			sbCode.AppendLine("\tLDA #$" + (((Registers.FixedColor & 0xFF) << 8) + (int)mode).ToString("X4"));
			sbCode.AppendLine("\tSTA $" + (Registers.DMAMode + channelAdd).ToString("X4"));
			sbCode.AppendLine("\tLDA #" + tableName);
			sbCode.AppendLine("\tSTA $" + (Registers.DMALowByte + channelAdd).ToString("X4"));
			sbCode.AppendLine("\tLDY.b #" + tableName + ">>16");
			sbCode.AppendLine("\tSTY $" + (Registers.DMABankByte + channelAdd).ToString("X4"));
			sbCode.AppendLine("\tSEP #$20");
			sbCode.AppendLine("\tLDA #$" + (1 << Channel).ToString("X2"));
			sbCode.AppendLine("\tTSB $" + RAM.HDMAEnable[SA1].ToString("X4") + "|!addr");
			sbCode.AppendLine("\tRTL");
			sbCode.AppendLine();
			sbCode.Append(Table.ToString(tableName));

			return sbCode.ToString();
		}
	}

	public delegate int PickSingleChannel();
	public delegate int[] PickMultiChannel();

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class FullColorHDMA
	{
		private ColorHDMA _red;
		private ColorHDMA _green;
		private ColorHDMA _blue;
		
		public Bitmap EffectImage
		{
			get
			{
				return BitmapEffects.MergeImages(_red.EffectImage, _green.EffectImage, _blue.EffectImage);
			}
		}

		public FullColorHDMA()
		{
			_red = new ColorHDMA(ColorHDMAValues.Red);
			_green = new ColorHDMA(ColorHDMAValues.Green);
			_blue = new ColorHDMA(ColorHDMAValues.Blue);

		}

		public void AddColorPosition(int Position, Color Color)
		{
			_red.ColorsPositions.Add(new ColorPosition(Position, Color.R));
			_blue.ColorsPositions.Add(new ColorPosition(Position, Color.B));
			_green.ColorsPositions.Add(new ColorPosition(Position, Color.G));
		}

		public void Clear()
		{
			_red.ColorsPositions.Clear();
			_red.Table.Clear();

			_green.ColorsPositions.Clear();
			_green.Table.Clear();

			_blue.ColorsPositions.Clear();
			_blue.Table.Clear();
		}

		public void End()
		{
			//checks if the table covers 224 scanlines and if not, adds a new entry to reset the color (if there isn't one already)
			if (_red.Table.Count != 0 && _red.Table.TotalScanlines < HDMA.Scanlines && (_red.Table.Last().Values[0] & 0x1F) != 0 && !_red.Table.HasEnded())
				_red.Table.Add(new HDMATableEntry(TableValueType.db, 1, (byte)_red.ColorEffect));
			if (_green.Table.Count != 0 && _green.Table.TotalScanlines < HDMA.Scanlines && (_green.Table.Last().Values[0] & 0x1F) != 0 && !_green.Table.HasEnded())
				_green.Table.Add(new HDMATableEntry(TableValueType.db, 1, (byte)_green.ColorEffect));
			if (_blue.Table.Count != 0 && _blue.Table.TotalScanlines < HDMA.Scanlines && (_blue.Table.Last().Values[0] & 0x1F) != 0 && !_blue.Table.HasEnded())
				_blue.Table.Add(new HDMATableEntry(TableValueType.db, 1, (byte)_blue.ColorEffect));

			if (_red.Table.Count != 0 && !_red.Table.HasEnded())
				_red.Table.Add(HDMATableEntry.End);
			if (_green.Table.Count != 0 && !_green.Table.HasEnded())
				_green.Table.Add(HDMATableEntry.End);
			if (_blue.Table.Count != 0 && !_blue.Table.HasEnded())
				_blue.Table.Add(HDMATableEntry.End);

			_red.UseCollection();
			_green.UseCollection();
			_blue.UseCollection();
		}

		public string Code(PickSingleChannel single, PickMultiChannel multi, PickMultiChannel tripple)
		{
			return Code(this._red, this._green, this._blue, single, multi, tripple);
		}

		public static string Code(ColorHDMA red, ColorHDMA green, ColorHDMA blue, 
			PickSingleChannel single, PickMultiChannel multi, PickMultiChannel tripple)
		{
			bool redEmpty = red.IsEmpty();
			bool greenEmpty = green.IsEmpty();
			bool blueEmpty = blue.IsEmpty();
			
			if (redEmpty && blueEmpty && greenEmpty)
				//fixme Exception
				throw new ArgumentException("Cannot generate code from the provided arguments");

			if (redEmpty && blueEmpty)
				return green.Code(single());
			if (greenEmpty && blueEmpty)
				return red.Code(single());
			if (redEmpty && greenEmpty)
				return blue.Code(single());

			int[] newChannels = new int[] { 3, 4, 5 };
			Dictionary<string, HDMATable> dicTable = new Dictionary<string, HDMATable>();
			HDMATable merged = null;

			if (greenEmpty)
			{
				if (HDMATable.Merge(red.Table, blue.Table, out merged))
					return ColorHDMA.Code(single(), merged, ".RedBlueTable");
				else
				{
					newChannels = multi();
					dicTable.Add(".RedTable", red.Table);
					dicTable.Add(".BlueTable", blue.Table);
				}
			}
			else if (redEmpty)
			{
				if (HDMATable.Merge(green.Table, blue.Table, out merged))
					return ColorHDMA.Code(single(), merged, ".GreenBlueTable");
				else
				{
					newChannels = multi();
					dicTable.Add(".GreenTable", green.Table);
					dicTable.Add(".BlueTable", blue.Table);
				}
			}
			else if (blueEmpty)
			{
				if (HDMATable.Merge(green.Table, red.Table, out merged))
					return ColorHDMA.Code(single(), merged, ".RedGreenTable");
				else
				{
					newChannels = multi();
					dicTable.Add(".GreenTable", green.Table);
					dicTable.Add(".RedTable", red.Table);
				}
			}
			else
			{
				if (HDMATable.Merge(green.Table, red.Table, out merged))
				{
					newChannels = multi();
					dicTable.Add(".RedGreenTable", merged);
					dicTable.Add(".BlueTable", blue.Table);
				}
				else if (HDMATable.Merge(green.Table, blue.Table, out merged))
				{
					newChannels = multi();
					dicTable.Add(".GreenBlueTable", merged);
					dicTable.Add(".RedTable", red.Table);
				}
				else if (HDMATable.Merge(red.Table, blue.Table, out merged))
				{
					newChannels = multi();
					dicTable.Add(".RedBlueTable", merged);
					dicTable.Add(".GreenTable", red.Table);
				}
				else
				{
					newChannels = tripple();
					dicTable.Add(".RedTable", red.Table);
					dicTable.Add(".GreenTable", green.Table);
					dicTable.Add(".BlueTable", blue.Table);
				}
			}

			if (dicTable.Count != newChannels.Length)
				throw new ArgumentException("Number of channels doesn't match number of tables.");

			Func<KeyValuePair<string, HDMATable>, string> getTablename = old => "." + old.Key.TrimStart('.');

			int channelTrigger = 0;
			StringBuilder sbCode = new StringBuilder();
            //sbCode.AppendLine("init:");
            sbCode.AppendLine("\tREP #$20");

			for (int i = 0; i < dicTable.Count; i++)
			{
				KeyValuePair<string, HDMATable> tablePair = dicTable.ElementAt(i);
				DMAMode mode = (tablePair.Value[0].Values.Length == 1 ? DMAMode.P : DMAMode.PP);
				int channelAdd = 16 * newChannels[i];
				string tableName = getTablename(tablePair);

				sbCode.AppendLine("\tLDA #$" + (((Registers.FixedColor & 0xFF) << 8) + (int)mode).ToString("X4"));
				sbCode.AppendLine("\tSTA $" + (Registers.DMAMode + channelAdd).ToString("X4"));
				sbCode.AppendLine("\tLDA #" + tableName);
				sbCode.AppendLine("\tSTA $" + (Registers.DMALowByte + channelAdd).ToString("X4"));
				sbCode.AppendLine("\tLDY.b #" + tableName + ">>16");
				sbCode.AppendLine("\tSTY $" + (Registers.DMABankByte + channelAdd).ToString("X4"));

				channelTrigger += (1 << newChannels[i]);
			}

			sbCode.AppendLine("\tSEP #$20");
			sbCode.AppendLine("\tLDA #$" + channelTrigger.ToString("X2"));
			sbCode.AppendLine("\tTSB $" + RAM.HDMAEnable[RAM.SA1].ToString("X4") + "|!addr");
			sbCode.AppendLine("\tRTL");

			for (int i = 0; i < dicTable.Count; i++)
			{
				KeyValuePair<string, HDMATable> tablePair = dicTable.ElementAt(i);
				string tableName = getTablename(tablePair);
				sbCode.AppendLine();
				sbCode.Append(tablePair.Value.ToString(tableName));
			}

			return sbCode.ToString();
		}
		
		public void FromImage(Bitmap image)
		{
			_red.FromImage(image);
			_blue.FromImage(image);
			_green.FromImage(image);
		}
	}
}
