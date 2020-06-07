using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Extansion.Int;

namespace EffectClasses
{
	public class ParallaxHDMA : AnimatedHDMA
	{
		public class ParallaxHDMAEntry : IEquatable<ParallaxHDMAEntry>
		{
			public int Number { get; set; }
			public int Scanline { get; set; }
			public double Multiplier
			{
				get {return _multiplier;}
				set
				{
					if(value == 0)
					{
						_multiplier = 0;
						return;
					}
					double val = Math.Log(value)/Math.Log(2.0);
					if(Math.Abs(val % 1) != 0)
						throw new InvalidOperationException("Multiplier must be 2 to the power of something.");
					_multiplier = value;
				}
			}
			public bool Autoscroll { get; set; }
			public int Autospeed 
			{
				get { return _autospeed; }
				set
				{
					if (Direction == Orientation.Right)
						_autospeed = (0x200 - value) % 0x200;
					else
						_autospeed = value;
				}
			}
			public Orientation Direction
			{
				get { return _direction; }
				set
				{
					if ((int)value % 2 != 1)
						throw new InvalidOperationException
						  ("Direction must be either " +
						  "EffectClasses.Orientation.Up or EffectClasses.Orientation.Down");
					_direction = value;
				}
			}
			private Orientation _direction = Orientation.Left;
			private double _multiplier;
			private int _autospeed;

			public static string LsrAsl(double multiplier)
			{
				if(multiplier == 0.0)
					return "LDA #$0000";
				double val = Math.Log(multiplier)/Math.Log(2.0);
				if(val < 0)
					return "LSR #" + (int)Math.Abs(val);
				else
					return "ASL #" + (int)val;
			}

			public ParallaxHDMAEntry(int number, int scanline, double multiplier,
				bool autoscroll, int autospeed, Orientation direction)
			{
				Number = number;
				Scanline = scanline;
				Multiplier = multiplier;
				Autoscroll = autoscroll;
				Direction = direction;
				Autospeed = autospeed;
			}

			public override int GetHashCode()
			{
				return _autospeed * 256 + (int)(_multiplier * 16);
			}

			public override bool Equals(object obj)
			{
				return GetHashCode() == obj.GetHashCode();
			}

			public bool Equals(ParallaxHDMAEntry other)
			{
				return GetHashCode() == other.GetHashCode();
			}
		}

		public List<ParallaxHDMAEntry> Bars { get; set; }

		/// <summary>
		/// Which register the effect code should work on
		/// </summary>
		/// <summary>
		/// Which register to use for the effect in the code
		/// </summary>
		public LayerRegister Layers
		{
			set
			{
				if (((int)value & 0x01) == 0)
					throw new ArgumentException("This wave effect cannot take a LayerRegister for the Y axis");
				_layers = value;
			}
			get { return _layers; }
		}
			  
		public const int ImageWidth = 256;// * 2;

		public ParallaxHDMA()
		{
			Bars = new List<ParallaxHDMAEntry>();
		}

		private LayerRegister _layers = LayerRegister.Layer2_X;
		private Bitmap[] _frames = new Bitmap[ImageWidth];

		public override Bitmap StaticPic(Bitmap basePic)
		{
			Reset();
			return new Bitmap(Original);
		}
		public override Bitmap NextAnimateFrame(Bitmap basePic)
		{
			Bitmap newBM = new Bitmap(basePic);
			int offset = 0;

			foreach(var bar in Bars)
			{
				Bitmap single;
				int index = (int)(bar.Multiplier * Frame) % _frames.Length;
				if(_frames[index] == null)
				{
					single = BitmapEffects.MoveLine(0, basePic.Height, index, basePic, Orientation.Left);
					_frames[index] = single;
				}
				else
				{
					single = _frames[index];
				}

				using(Graphics g = Graphics.FromImage(newBM))
				{
					int scanlines = 0;
					if (offset + bar.Scanline >= basePic.Height)
						scanlines = basePic.Height - offset;
					else
						scanlines = bar.Scanline;

					Rectangle rect = new Rectangle(0, offset, basePic.Width, scanlines);
					g.SetClip(rect);
					g.Clear(Color.Transparent); //delete the part to be moved before drawing over it.
					g.ResetClip();
					g.DrawImageUnscaled(single.Clone(rect, basePic.PixelFormat), rect);
				}
				offset += bar.Scanline;
				if (offset >= basePic.Height)
					break;
			}

			Frame++;
			return newBM;
		}

		public override void Reset()
		{
			Frame = 0;
			for (int i = 0; i < _frames.Length; i++)
			{
				if (_frames[i] != null)
				{
					_frames[i].Dispose();
					_frames[i] = null;
				}
			}
		}

		public override int CountRAMBytes()
		{
			throw new NotImplementedException();
		}
		public override int CountROMBytes()
		{
			throw new NotImplementedException();
		}
		

		public override string Code(int channel, HDMATable table, bool sa1)
		{
			int Base = 0x4300 + (channel * 0x10);
			int Register = (int)Layers;
			int BaseAddress = RAM.Layer1X[sa1] + ((int)Layers - (int)LayerRegister.Layer1_X) * 2;
			int RegMode = ((Register & 0xFF) << 8) + GetMode(true, DMAMode.PP);

			var grouped = Bars.GroupBy(b => b.Multiplier);
			HDMATable _table = new HDMATable("ParallaxTable_" + DateTime.Now.ToString("HHmmssfff"));
			int tableInRAM = grouped.Count() * 2;

			Dictionary<int, int> multiplierAddressMapping = new Dictionary<int, int>();
			int addresseForOne = 0;	//address that has the scrollrate of 1 to finish the table.


			string tableString = "The Table takes up " + tableInRAM + " bytes of the free RAM\n" +
				"It ranges from $" + FreeRAM.ToString("X6") + " - $" + (FreeRAM + tableInRAM - 1).ToString("X6") + " (both addresses included)";
						
			ASMCodeBuilder CodeBuilder = new ASMCodeBuilder();

			if (Original.Height > Scanlines)
				CodeBuilder.AppendCommentLine("IMPORTANT! Please edit the JMP command below for the level you use this on");
			CodeBuilder.AppendEmptyLine();

			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendLabel(INITLabel, "This section is to be used in the INIT code of levelASM");
			CodeBuilder.AppendCode("REP #$20");
			CodeBuilder.AppendCode("LDA #$" + RegMode.ToASMString(), "Use indeirect and mode " + (int)DMAMode.PP + " on register " + Register.ToASMString());
			CodeBuilder.AppendCode("STA $" + Base.ToASMString(), "43" + Channel + "0 = Mode, 43" + Channel + "1 = Register");

			if(Original.Height <= Scanlines)
			{
				CodeBuilder.AppendCode("LDA #" + _table.Name, "Address of HDMA table, get high and low byte");
				CodeBuilder.AppendCode("STA $" + (Base + 2).ToASMString(), "43" + Channel + "2 = Low-Byte of table, 43" + Channel + "3 = High-Byte of table");
			}

			CodeBuilder.AppendCode("SEP #$20");
			CodeBuilder.AppendCode("LDA.b #" + _table.Name + ">>16", "Address of HDMA table, get bank byte");
			CodeBuilder.AppendCode("STA $" + (Base + 4).ToASMString(), "43" + Channel + "4 = Bank-Byte of table");

			CodeBuilder.AppendCode("LDA #$" + (FreeRAM >> 16).ToASMString(), "Address of indirect table in RAM bank byte");
			CodeBuilder.AppendCode("STA $" + (Base + 7).ToASMString(), "43" + Channel + "4 = Bank-Byte of indirect table");

			CodeBuilder.AppendCode("LDA #$" + (0x01 << Channel).ToASMString());
			CodeBuilder.AppendCode("TSB $" + RAM.HDMAEnable[sa1].ToASMString() + "|!addr", "Enable HDMA channel " + Channel);

			if (Original.Height > Scanlines)
				CodeBuilder.AppendCode("JMP level105_" + MAINLabel.TrimStart('.'), "Jump to main code" + MAINSeperator);
			else
				CodeBuilder.AppendCode("RTL", "Return" + MAINSeperator);
			CodeBuilder.CloseBlock();
			
			CodeBuilder.AppendCommentLine(tableString);
			CodeBuilder.AppendEmptyLine();

			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendLabel(MAINLabel, "This section is to be used in the MAIN code of levelASM");
			CodeBuilder.AppendCode("REP #$20", "16 bit action starts here. (To load the x position of the BG)");
			CodeBuilder.CloseBlock();

			//index for HDMA table entry calculation
			if (Original.Height > Scanlines)
			{
				CodeBuilder.OpenNewBlock();
				CodeBuilder.AppendCode("LDA $" + (BaseAddress + 2).ToASMString(), "Get Y position of BG");
				CodeBuilder.AppendCode("ASL", "times 2");
				CodeBuilder.AppendCode("CLC : ADC $" + (BaseAddress + 2).ToASMString(), "+1 = times 3");
				CodeBuilder.AppendCode("CLC : ADC #" + _table.Name + "+3", "plus Address of HDMA table +3");
				//CodeBuilder.AppendCode("INC : INC : INC", "I have no idea why, but the result is off by 1 entry (3 bytes) so this is to fix it.");
				CodeBuilder.AppendCode("STA $" + (Base + 2).ToASMString(), "43" + Channel + "2 = Low-Byte of table, 43" + Channel + "3 = High-Byte of table");
				CodeBuilder.CloseBlock();
			}

			CodeBuilder.AppendEmptyLine();
			int i = 0;
			//grouped by multiplier
			foreach(var bar in grouped)
			{
				CodeBuilder.OpenNewBlock();
				if(bar.Key != 0.0)
					CodeBuilder.AppendCode("LDA $" + BaseAddress.ToASMString(), "Load BG x Position");
				CodeBuilder.AppendCode(ParallaxHDMAEntry.LsrAsl(bar.Key), "Multiplied by " + bar.Key);
				var windGroup = bar.GroupBy(b => b.Autospeed);

				foreach (var singleBar in windGroup)
				{
					//if there is no wind, don't add or preserve A
					if (singleBar.ElementAt(0).Autoscroll && singleBar.Key != 0)
					{
						CodeBuilder.AppendCode("PHA", "Preserve A (current multiplication result)");
						CodeBuilder.AppendCode("CLC : ADC #$" + singleBar.Key.ToString("X4"), "Add rate.");
					}
					CodeBuilder.AppendCode("STA $" + (FreeRAM + i).ToASMString(), "Store to FreeRAM for indirect HDMA");
					//also don't restore A if there is no wind.
					if (singleBar.ElementAt(0).Autoscroll && singleBar.Key != 0)
						CodeBuilder.AppendCode("PLA", "Restore A (current multiplication result)");

					//keep track of the address for normal scrolling behaviour (multiplier = 1, no auto scroll)
					if (bar.Key == 1.0 && singleBar.ElementAt(0).Autoscroll && singleBar.Key != 0)
						addresseForOne = FreeRAM + i;

					//remember which address has this specs in dictionary with hashcode
					multiplierAddressMapping.Add(singleBar.ElementAt(0).GetHashCode(), FreeRAM + i);
					i += 2;
				}
				CodeBuilder.CloseBlock();
			}

			//if the normal scrolling hasn't been set yet, make one.
			if(addresseForOne == 0)
			{
				CodeBuilder.OpenNewBlock();
				CodeBuilder.AppendCode("LDA $" + BaseAddress.ToASMString(), "Load BG x Position");
				CodeBuilder.AppendCode("STA $" + (FreeRAM + i).ToASMString(), "Store to FreeRAM for indirect HDMA");
				CodeBuilder.CloseBlock();
				addresseForOne = FreeRAM + i;
				i += 2;
			}

			//build the table
			foreach (var bar in Bars)
			{
				//fetch indirect HDMA address
				int adr = multiplierAddressMapping[bar.GetHashCode()] & 0xFFFF;
				//split it
				byte low = (byte)(adr & 0xFF);
				byte high = (byte)((adr >> 8) & 0xFF);

				//make as many 1 scanline high entries as needed.
				var entry = new HDMATableEntry(TableValueType.dw, 1, low, high);
				_table.AddRange(Enumerable.Repeat<HDMATableEntry>(entry, bar.Scanline));
			}

			//if there aren't enough entries for the whole screen yet, fill it up with scrollrate 1.
			if(_table.Count < Original.Height)
			{
				//fetch indirect HDMA address for normal scrolling
				int adr = addresseForOne & 0xFFFF;
				//split it
				byte low = (byte)(adr & 0xFF);
				byte high = (byte)((adr >> 8) & 0xFF);

				var entry = new HDMATableEntry(TableValueType.dw, 1, low, high);
				_table.AddRange(Enumerable.Repeat<HDMATableEntry>(entry, Original.Height - _table.Count));
			}

			//cut down table
			for (int l = _table.Count - 1; l >= Original.Height; l--)
				_table.RemoveAt(l);

			//add end.
			_table.Add(HDMATableEntry.End);
			
			CodeBuilder.AppendCode("SEP #$20", "Back to 8bit");
			CodeBuilder.AppendCode("RTL", "Return");
			CodeBuilder.CloseBlock();

			CodeBuilder.AppendEmptyLine();
			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendTable(_table);
			CodeBuilder.CloseBlock();

			return CodeBuilder.ToString();// +"\n\n" + tableString;
		}
	}
}
