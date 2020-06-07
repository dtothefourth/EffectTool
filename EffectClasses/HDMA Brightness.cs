using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Extansion.Int;
using Extansion.String;

namespace EffectClasses
{
	public class BrightnessHDMA : HDMA
	{
		private ColorPositionCollection _darknessPosition;

		/// <summary>
		/// A collection of all the positions and which darkness level they ought to have at that position.
		/// </summary>
		public ColorPositionCollection DarknessPositions
		{
			get
			{
				return _darknessPosition;
			}
			set
			{
				_darknessPosition = value;
				UseCollection();
			}
		}

		/// <summary>
		/// The drawn gradient effect
		/// </summary>
		public Bitmap EffectImage { get; set; }
		
		/// <summary>
		/// Whether the code requires the MAIN code in uberASM.
		/// <para>Will always return false.</para>
		/// </summary>
		public override bool UsesMain { get { return false; } }

		public BrightnessHDMA()
		{
			EffectImage = BitmapEffects.FromColor(Color.Transparent, 256, Scanlines);
		}


		/// <summary>
		/// 
		/// </summary>
		internal void UseCollection()
		{
			//http://www.codeproject.com/Articles/20018/Gradients-made-easy
			List<ColorPosition> _list = new List<ColorPosition>(DarknessPositions);
			
			_list.Sort();

			if (_list[0].Position != 0)
				_list.Insert(0, new ColorPosition(0, 0xF));
			if (_list[_list.Count - 1].Position < Scanlines)
				_list.Add(new ColorPosition(Scanlines, 0xF));

			List<float> positions = new List<float>();
			//positions.Add(0.0f);

			int maxpos = _list.Max(cp => cp.Position);
			foreach (ColorPosition cp in _list)
				positions.Add((float)cp.Position / (float)maxpos);
			//positions.Add(1.0f);

			List<float> factors = new List<float>();
			//factors.Add(0.0f);
			foreach (ColorPosition cp in _list)
				factors.Add((float)cp.Orignal8Bit / 15.0f);
			//factors.Add(1.0f);

			Bitmap bm = new Bitmap(1, maxpos);

			using (Graphics g = Graphics.FromImage(bm))
			{
				Rectangle r = new Rectangle(0, 0, bm.Width, maxpos);
				using (LinearGradientBrush lgb = new LinearGradientBrush(r, Color.FromArgb(_list.Min(cp => cp.Orignal8Bit) * 17, 0, 0),
					Color.FromArgb(_list.Max(cp => cp.Orignal8Bit) * 17, 0, 0), 90.0f))
				{
					Blend blend = new Blend();
					blend.Factors = factors.ToArray();
					blend.Positions = positions.ToArray();
					lgb.Blend = blend;
					g.FillRectangle(lgb, r);
				}
			}

			byte Compare = (byte)(bm.GetPixel(0, 0).R / 17);
			Table = new HDMATable();

			for (int y = 0, scan = 1; y < maxpos; y++, scan++)
			{
				byte darkness = (byte)(bm.GetPixel(0, y).R / 17);
				if (Compare != darkness || scan >= 0x80 || y == maxpos - 1)
				{
					Table.Add(new HDMATableEntry(TableValueType.db, (byte)scan, Compare));
					Compare = darkness;
					scan = 0;
				}

				for (int x = 0; x < bm.Width; x++)
					bm.SetPixel(x, y, Color.FromArgb(255 - darkness * 17, 0, 0, 0));
			}

			Table.Add(HDMATableEntry.End);

			Bitmap retImg = new Bitmap(256, maxpos);
			using (TextureBrush brush = new TextureBrush(bm, WrapMode.Tile))
			using (Graphics gr = Graphics.FromImage(retImg))
				gr.FillRectangle(brush, 0, 0, retImg.Width, retImg.Height);

			bm.Dispose();
			EffectImage = retImg;
		}

		public void FromTable(HDMATable table)
		{
			if (!table.HasEnded())
				table.Add(HDMATableEntry.End);
			Table = table;
			UpdateImage();
		}

		public string[] FromString(string[] lines)
		{
			Table = new HDMATable();
			List<string> errorLines = new List<string>();

			foreach (string s in lines)
			{
				try
				{
					
					string[] values = s.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (values.Length < 2)
						throw new ArgumentException("Too few arguments.");
					byte scanlines = (byte)values[0].ToHexInt();

					if(scanlines > 0x80 && scanlines - 0x80 != values.Length - 1)
						throw new ArgumentException("Invalid use of scanline counter $80+");

					byte[] bytes = new byte[values.Length - 1];
					for (int i = 1; i < values.Length; i++)
					{
						bytes[i - 1] = (byte)(values[i].ToHexInt().Range(0,15));
					}

					Table.Add(new HDMATableEntry(TableValueType.db, scanlines, bytes));

					if (Table.TotalScanlines > Scanlines)
						break;
				}
				catch(Exception ex)
				{
#if(DEBUG)
					Console.WriteLine("BrightnessHDMA.FromString ex: " + ex.Message);
#endif
					errorLines.Add(s);
				}
			}

			if (Table.TotalScanlines < Scanlines)
				Table.Add(new HDMATableEntry(TableValueType.db, 1, 0xF));

			Table.Add(HDMATableEntry.End);
			UpdateImage();
			return errorLines.ToArray();
		}

		/// <summary>
		/// Generates the EffectImage based on the setting of the Table.
		/// </summary>
		[Obsolete("test me")]
		internal void UpdateImage()
		{
			if (Table == null)
				return;

			Bitmap bm = BitmapEffects.FromColor(Color.Transparent, 1, Scanlines);
			int counter = 0;
			foreach (HDMATableEntry entry in Table)
			{
				if (entry.ValueType == TableValueType.End)
					break;
				if (entry.ValueType == TableValueType.Single)
				{
					for (int y = counter; y < counter + entry.Scanlines - 0x80 && y < Scanlines; y++)
						for (int x = 0; x < bm.Width; x++)
							bm.SetPixel(x, y, Color.FromArgb(255 - (entry.Values[y] & 0x0F) * 17, 0, 0, 0));
					counter += entry.Scanlines - 0x80;
				}
				else if (entry.ValueType == TableValueType.db)
				{
					for (int y = counter; y < counter + entry.Scanlines && y < Scanlines; y++)
						for (int x = 0; x < bm.Width; x++)
							bm.SetPixel(x, y, Color.FromArgb(255 - (entry.Values[0] & 0x0F) * 17, 0, 0, 0));
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
		/// How many bytes of RAM the code will require during it's runtime.
		/// </summary>
		/// <returns></returns>
		public override int CountRAMBytes()
		{
			if (!UsesMain)
				return 0;
			throw new NotImplementedException("This was not suppose to be ever executed");
		}

		/// <summary>
		/// How many bytes of ROM the code takes up after being inserted
		/// <para>Only counts the bytes of the table so far!!!</para>
		/// </summary>
		/// <returns></returns>
		public override int CountROMBytes()
		{
			return Table.TotalBytes;
		}

		/// <summary>
		/// Calculates the code for the Brightness HDMA with the desired channel, Table and possibly SA-1 compatible
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="table"></param>
		/// <param name="sa1"></param>
		/// <returns></returns>
		public override string Code(int channel, HDMATable table, bool sa1)
		{
			DMAMode mode = (table[0].Values.Length == 1 ? DMAMode.P : DMAMode.PP);

			ASMCodeBuilder code = new ASMCodeBuilder();
			int channelAdd = 16 * channel;
			table.Name = ".BrightTable";

			code.AppendLabel(INITLabel, "Code to be inserted INIT");
			code.OpenNewBlock();
			code.AppendCode("\tREP #$20", "16 bit mode");
			code.AppendCode("\tLDA #$" + (((Registers.Brightness & 0xFF) << 8) + (int)mode).ToString("X4"));
			code.AppendCode("\tSTA $" + (Registers.DMAMode + channelAdd).ToString("X4"));
			code.AppendCode("\tLDA #" + table.Name, "load high and low byte of table address");
			code.AppendCode("\tSTA $" + (Registers.DMALowByte + channelAdd).ToString("X4"));
			code.AppendCode("\tSEP #$20", "back to 8 bit mode");
			code.AppendCode("\tLDA.b #" + table.Name + ">>16", "load bank byte of table address");
			code.AppendCode("\tSTA $" + (Registers.DMABankByte + channelAdd).ToString("X4"));
			code.AppendCode("\tLDA #$" + (1 << channel).ToString("X2"));
			code.AppendCode("\tTSB $" + RAM.HDMAEnable[sa1].ToString("X4") + "|!addr", "enable HDMA channel " + channel);
			code.AppendCode("\tRTL");
			code.CloseBlock();
			code.AppendEmptyLine();

			code.AppendTable(table);

			return code.ToString();
		}

		/// <summary>
		/// Calculates the code for overlapping multiple brightness gradients.
		/// <para>The final code will use the channel of the first BrightnessHDMA object passed.</para>
		/// </summary>
		/// <param name="hdma">An array of BrightnessHDMA object that will be overlapped for the final code.</param>
		/// <returns>The final code.</returns>
		public static string MultiCode(params BrightnessHDMA[] hdma)
		{
			if (hdma == null || hdma.Length == 0)
				return null;

			int channel = hdma[0].Channel;
			EffectClasses.HDMATable table = new HDMATable(".BrightTable");

			using (Bitmap b = BitmapEffects.OverlapImages(hdma.Select(h => h.EffectImage).ToArray()))
			{
				byte scanlines = 1;
				int now = 0;
				int compare = 0x0F - (b.GetPixel(0, 0).A / 17);
				for (int y = 1; y < b.Height; y++, scanlines++)
				{
					now = 0x0F - (b.GetPixel(0,y).A / 17);
					if(compare != now || scanlines >= 0x80)
					{
						table.Add(new HDMATableEntry(TableValueType.db, scanlines, (byte)compare));
						compare = now;
						scanlines = 0;
					}
				}

				if(table.TotalScanlines != Scanlines)
					table.Add(new HDMATableEntry(TableValueType.db, scanlines, (byte)now));

				table.Add(HDMATableEntry.End);

				DMAMode mode = (table[0].Values.Length == 1 ? DMAMode.P : DMAMode.PP);
			ASMCodeBuilder code = new ASMCodeBuilder();
			int channelAdd = 16 * channel;
			table.Name = ".BrightTable";

			code.AppendLabel(INITLabel, "Code to be inserted INIT");
			code.OpenNewBlock();
			code.AppendCode("\tREP #$20", "16 bit mode");
			code.AppendCode("\tLDA #$" + (((Registers.Brightness & 0xFF) << 8) + (int)mode).ToString("X4"));
			code.AppendCode("\tSTA $" + (Registers.DMAMode + channelAdd).ToString("X4"));
			code.AppendCode("\tLDA #" + table.Name, "load high and low byte of table address");
			code.AppendCode("\tSTA $" + (Registers.DMALowByte + channelAdd).ToString("X4"));
			code.AppendCode("\tSEP #$20", "back to 8 bit mode");
			code.AppendCode("\tLDA.b #" + table.Name + ">>16", "load bank byte of table address");
			code.AppendCode("\tSTA $" + (Registers.DMABankByte + channelAdd).ToString("X4"));
			code.AppendCode("\tLDA #$" + (1 << channel).ToString("X2"));
			code.AppendCode("\tTSB $" + RAM.HDMAEnable[RAM.SA1].ToString("X4") + "|!addr", "enable HDMA channel " + channel);
			code.AppendCode("\tRTL");
			code.CloseBlock();
			code.AppendEmptyLine();

			code.AppendTable(table);

			return code.ToString();
			}
		}
	}

}
