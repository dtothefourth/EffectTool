using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Extansion.Int;
using System.Drawing.Imaging;

namespace EffectClasses
{
	public enum Window { Window1 = 0, Window2 = 2 }

	public class OneWindowEventArgs : EventArgs
	{
		public Window Window { get; set; }
		public bool Cancel { get; set; }

		public OneWindowEventArgs(Window window)
		{
			Window = window;
			Cancel = false;
		}
	}

	public delegate void ChooseWindowEventHandler(object sender, OneWindowEventArgs e);

	public class WindowingHDMA : HDMA
	{
		class ColPos
		{
			public int Position { get; set; }
			public Color Color { get; set; }

			public ColPos(Color C, int P)
			{
				Position = P;
				Color = C;
			}
		}

		public event ChooseWindowEventHandler OneWindowEvent;

		public Bitmap Orignal { get; set; }
		public Bitmap EffectImage { get; set; }

		public WindowingHDMA()
		{
			Orignal = BitmapEffects.FromColor(Color.White, 256, Scanlines);
		}

		public bool CheckAndSplitMask(Bitmap mask, ColorMath math)
		{
			//array of with array for each line.
			//each line has entrys with the color and the start position of that color in that line
			ColPos[][] C_Arr = new ColPos[Scanlines][];

			BitmapData data = mask.LockBits(new Rectangle(new Point(0, 0), mask.Size), ImageLockMode.ReadWrite, mask.PixelFormat);
			byte[] bytes = new byte[data.Height * data.Stride];
			System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

			for (int y = 0; y < mask.Height; y++)
			{
				Color ColorToCheck = BitmapEffects.GetPixel(data, bytes, 0, y); //mask.GetPixel(0, y);
				List<ColPos> CList = new List<ColPos>();
				CList.Add(new ColPos(ColorToCheck, 0));

				for (int x = 1; x < mask.Width; x++)
				{
					Color c = BitmapEffects.GetPixel(data, bytes, x, y);
					if (ColorToCheck == c) //mask.GetPixel(x, y))
						continue;

					ColorToCheck = c; //mask.GetPixel(x, y);
					CList.Add(new ColPos(ColorToCheck, x));
				}
				C_Arr[y] = CList.ToArray();
			}

			mask.UnlockBits(data);

			//if more then two black/white transitions appear in (any) line
			bool black3 = C_Arr.Any((CA => CA.Count(c => c.Color.Name == "ff000000") > 2));
			bool white3 = C_Arr.Any((CA => CA.Count(c => c.Color.Name == "ffffffff") > 2));

			//3 transitions in both won't work eg. black-white-black-white-black-white
			if (black3 && white3)
				return false;

			int[] BlackLines;
			int[] WhiteLines;

			//get all the lines with 3 black transitions.
			//can't be more then 3 because that would result in more then 3 white trans. in the same line.
			IEnumerable<ColPos[]> Puff = C_Arr.Where((CA => CA.Count(c => c.Color.Name == "ff000000") > 2));
			//an array of indeces to those lines.
			BlackLines = new int[Puff.Count()];
			int i = 0;
			foreach (ColPos[] cp in Puff)
			{
				BlackLines[i] = Array.IndexOf(C_Arr, cp);
				i++;
			}

			//same for white.
			Puff = C_Arr.Where((CA => CA.Count(c => c.Color.Name == "ffffffff") > 2));
			WhiteLines = new int[Puff.Count()];
			i = 0;
			foreach (ColPos[] cp in Puff)
			{
				WhiteLines[i] = Array.IndexOf(C_Arr, cp);
				i++;
			}
			
			Bitmap w1 = BitmapEffects.FromColor(Color.White, 256, 224);
			Bitmap w2 = BitmapEffects.FromColor(Color.White, 256, 224);

			BitmapData w1Data = w1.LockBits(new Rectangle(0, 0, w1.Width, w1.Height), ImageLockMode.ReadWrite, w1.PixelFormat);
			BitmapData w2Data = w2.LockBits(new Rectangle(0, 0, w2.Width, w2.Height), ImageLockMode.ReadWrite, w2.PixelFormat);
			byte[] w1Bytes = new byte[w1Data.Height * w1Data.Stride];
			byte[] w2Bytes = new byte[w2Data.Height * w2Data.Stride];
			System.Runtime.InteropServices.Marshal.Copy(w1Data.Scan0, w1Bytes, 0, w1Bytes.Length);
			System.Runtime.InteropServices.Marshal.Copy(w2Data.Scan0, w2Bytes, 0, w2Bytes.Length);


			Table = new HDMATable(".windowTable");
			HDMATableEntry entry = null;
			HDMATableEntry lastEntry = null;

			//if any line in this image requires 3 black and 2 white, one window (bm1) has to be inverted.
			bool WindowInverted = C_Arr.Any(cp => (cp.Count(c => c.Color.Name == "ff000000") == 3 &&
				cp.Count(c => c.Color.Name == "ffffffff") == 2));

			for (int y = 0; y < Scanlines; y++)
			{
				entry = new HDMATableEntry(TableValueType.db, (byte)(1 + y - Table.TotalScanlines),
					0xFF, 0x00, 0xFF, 0x00);

				int countBlack = C_Arr[y].Count(cp => cp.Color.Name == "ff000000");
				int countWhite = C_Arr[y].Count(cp => cp.Color.Name == "ffffffff");

				Func<int, int, int, bool> BlackFromTill = (window, from, till) =>
					{
						if (till == 0x100)
							till = 0xFF;

						if(window == 1)
						{
							for (int x = from; x < till; x++)
								BitmapEffects.SetPixel(w1Data, ref w1Bytes, x, y, Color.Black);
							entry.Values[0] = (byte)from;
							entry.Values[1] = (byte)till;
							return true;
						}
						else if(window == 2)
						{
							for (int x = from; x < till; x++)
								BitmapEffects.SetPixel(w2Data, ref w2Bytes, x, y, Color.Black);
							entry.Values[2] = (byte)from;
							entry.Values[3] = (byte)till;
							return true;
						}
						return false;
					};

				if (countBlack == 1 && countWhite == 0)
				{
					// whole line black.
					// if window get's inverted, line can be left unchanged.
					if (!WindowInverted)
						BlackFromTill(1, 0, w1.Width);	// if not, whole line black
				}
				else if (countBlack == 2 && countWhite == 1)
				{
					//outer black, inner white (black-white-black)

					if (WindowInverted)		//if window 1 is inverted it can be done using one window.
						BlackFromTill(1, C_Arr[y][1].Position, C_Arr[y][2].Position);	//The part white in the mask will be black in w1

					else    //window 1 not inveted:
					{
						BlackFromTill(1, C_Arr[y][0].Position, C_Arr[y][1].Position);	// first black part of mask in window 1,
						BlackFromTill(2, C_Arr[y][2].Position, w2.Width);				// and the second in window 2.
					}
				}
				else if (countBlack == 3 && countWhite == 2)
				{
					//black-white-black-white-black
					//1 window inverted, the other not (not possible otherwise).
					//meaning window 1 will be inverted.

					BlackFromTill(1, C_Arr[y][1].Position, C_Arr[y][4].Position);	// windows 1 will be black from the beginning of the first white
					BlackFromTill(2, C_Arr[y][2].Position, C_Arr[y][3].Position);	// till the end of the second. Windows 2 is the middle black part.
				}

				//else if (countBlack == 3 && countWhite == 3)
					//... IMPOSSIBRU <.<

				else if (countBlack == 2 && countWhite == 3)
				{
					//white-black-white-black-white
					//both windows have to be uninverted. (or playing with XOR setting)

					BlackFromTill(1, C_Arr[y][1].Position, C_Arr[y][2].Position);	// window 1 black for first black part of mask
					BlackFromTill(2, C_Arr[y][3].Position, C_Arr[y][4].Position);	// windows 2 black for second part.
				}
				else if (countBlack == 1 && countWhite == 2)
				{
					//outer white inner black (white-black-white)

					if (WindowInverted) //if windows 1 is inverted, we have to use window 2 and set window 1 black on the whole line
					{
						BlackFromTill(1, 0, w1.Width);									// window 1 completly black
						BlackFromTill(2, C_Arr[y][1].Position, C_Arr[y][2].Position);	// window 2 black where mask is black.
					}
					else
						BlackFromTill(1, C_Arr[y][1].Position, C_Arr[y][2].Position);	// if not inverted, simply use window 1.
				}
				else if (countBlack == 0 && countWhite == 1)
				{
					//whole line visible
					if (WindowInverted)
						BlackFromTill(1, 0, w1.Width);			// if window 1 inverted, whole line black
				}
				else if (countBlack == 2 && countWhite == 2)
				{
					//problem: unknown if black-white-black-white or white-black-white-black
					#region 2/2
					if (C_Arr[y][0].Color.Name == "ff000000") //if fisrt part black then B-W-B-W
					{
						if (WindowInverted) //inverted:
						{
							BlackFromTill(1, C_Arr[y][1].Position, w1.Width);				// the -W-B-W area will be black, so inverted makes the first bar black
							BlackFromTill(2, C_Arr[y][2].Position, C_Arr[y][3].Position);	// window 2 simply makes second bar black	
						}
						else	//not inverted
						{
							BlackFromTill(1, C_Arr[y][0].Position, C_Arr[y][1].Position);	// first bar in window 1
							BlackFromTill(2, C_Arr[y][2].Position, C_Arr[y][3].Position);	// guess what
						}
					}
					else // first bar not black, thus W-B-W-B
					{
						if (WindowInverted) //inverted:
						{
							BlackFromTill(1, C_Arr[y][0].Position, C_Arr[y][3].Position);	// the W-B-W- area will be black, inverting makes the last bar black.
							BlackFromTill(2, C_Arr[y][1].Position, C_Arr[y][2].Position);	// window 2 will be black where the first bar is.                         
						}
						else	//not inverted
						{
							BlackFromTill(1, C_Arr[y][1].Position, C_Arr[y][2].Position);	// first bar in window 2
							BlackFromTill(2, C_Arr[y][3].Position, w2.Width);				// ...
						}
					}
					#endregion
				}
				else if (countBlack == 1 && countWhite == 1)
				{
					//problem: unknown if black-white or white-black
					if (C_Arr[y][0].Color.Name == "ff000000") //first bar is black thus B-W
					{
						if (WindowInverted) //inverted:
							BlackFromTill(1, C_Arr[y][1].Position, w1.Width);		//set second bar (white) to black due to inverting

						else    //not inverted
							BlackFromTill(1, C_Arr[y][0].Position, C_Arr[y][1].Position);	//first bar (black) in window one to black
					}
					else //first bar not black thus W-B
					{
						if (WindowInverted) //inverted:
							BlackFromTill(1, C_Arr[y][0].Position, C_Arr[y][1].Position);   // set white part black;

						else    //not inverted
							BlackFromTill(1, C_Arr[y][1].Position, w1.Width);	//set second bar black.
					}
				}


				if ((lastEntry != null && !entry.Values.SequenceEqual(lastEntry.Values)) || entry.Scanlines == 0x81)
				{
					Table.Add(lastEntry);
					entry.Scanlines = 1;
				}
				lastEntry = (HDMATableEntry)entry.Clone();

			}//end of for-loop

			Table.Add(lastEntry);			//the last last entry
			Table.Add(HDMATableEntry.End);	//the actual end.

			System.Runtime.InteropServices.Marshal.Copy(w1Bytes, 0, w1Data.Scan0, w1Bytes.Length);
			System.Runtime.InteropServices.Marshal.Copy(w2Bytes, 0, w2Data.Scan0, w2Bytes.Length);
			w1.UnlockBits(w1Data);
			w2.UnlockBits(w2Data);

			math.WindowingMask1 = w1;
			math.WindowingMask2 = w2;

			math.Window1Inverted = WindowInverted ? (WindowingLayers)0x3F : 0;

			return true;
		}

		public override bool UsesMain { get { return false; } }

		public override int CountRAMBytes()
		{
			if (!UsesMain)
				return 0;
			throw new NotImplementedException();
		}
		public override int CountROMBytes()
		{
			return Table.TotalBytes;
		}

		public override string Code(int channel, HDMATable table, bool sa1)
		{
			bool bothWindows = (Table.Any(t => (t.ValueType == TableValueType.db && (t.Values[2] != 0xFF || t.Values[3] != 0x00))));
			int mode = GetMode(false, bothWindows ? DMAMode.PP1P2P3 : DMAMode.PP1);

			int register = Registers.WindowMask1Left;

			if(!bothWindows && OneWindowEvent != null)
			{
				OneWindowEventArgs e = new OneWindowEventArgs(Window.Window1);
				OneWindowEvent(this, e);
				if (e.Cancel)
					return "";

				register += (int)e.Window;
			}

			if(!bothWindows)
				foreach (HDMATableEntry entry in table)
				{
					if (entry.ValueType == TableValueType.db)
					{
						byte[] values = entry.Values;
						Array.Resize(ref values, 2);
						entry.Values = values;
					}
				}

				
			int regmode = ((register & 0xFF) << 8) + mode;
			int baseChannel = 0x4300 + (channel * 0x10);

			ASMCodeBuilder code = new ASMCodeBuilder();

			code.OpenNewBlock();
			code.AppendCode("REP #$20", "Get into 16 bit mode");
			code.AppendCode("LDA #$" + regmode.ToASMString(), "Register $" + register.ToASMString() + " using mode " + mode);
			code.AppendCode("STA $" + baseChannel.ToASMString(), baseChannel.ToASMString() + " = transfer mode, " + (baseChannel + 1).ToASMString() + " = register");
			code.AppendCode("LDA #" + table.Name, "High byte and low byte of table addresse.");
			code.AppendCode("STA $" + (baseChannel + 2).ToASMString(), (baseChannel + 2).ToASMString() + " = low byte, " + (baseChannel + 3).ToASMString() + " = high byte");
			code.AppendCode("SEP #$20", "Back to 8 bit mode");
			code.AppendCode("LDA.b #" + table.Name + ">>16", "Bank byte of table addresse.");
			code.AppendCode("STA $" + (baseChannel + 4).ToASMString(), "= bank byte");
			code.CloseBlock();

			code.OpenNewBlock();
			code.AppendCode("LDA #$" + (1 << channel).ToASMString());
			code.AppendCode("TSB $" + RAM.HDMAEnable[sa1].ToASMString() + "|!addr", "enable HDMA channel " + channel);
			code.CloseBlock();

			code.AppendCode("RTL", "Return");
			code.AppendEmptyLine();

			code.AppendTable(table);

			return code.ToString();
		}
	}
}
