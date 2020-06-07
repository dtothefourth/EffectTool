using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Extansion.Drawing;
using Extansion.Enum;

namespace HDMA_Generator_Tool
{
	[Flags]
	public enum WindowBG : byte { BG1 = 1, BG2 = 2, BG3 = 4, BG4 = 8, OBJ = 16, Color = 32 }
	public enum WindowUse { Window1 = 0, Window2 = 2 }
	public enum ColorWindow { Never = 0, Outside = 1, Inside = 2, Always = 3 }

	abstract class Window_Basic_HDMA : HDMA
	{
		public const int RegisterWindowBG1BG2 = 0x41;
		public const int RegisterWindowBG3BG4 = 0x42;
		public const int RegisterWindowOBJColor = 0x43;

		public const int RegisterWindow1Left = 0x2126;
		public const int RegisterWindow1Right = 0x2127;
		public const int RegisterWindow2Left = 0x2128;
		public const int RegisterWindow2Right = 0x2129;

		protected FastBitmap _mask;
		protected bool _inverted = true;
		protected WindowBG _BGToHide = WindowBG.Color | WindowBG.BG1 | WindowBG.BG2 | WindowBG.BG3 | WindowBG.OBJ;
		protected WindowUse _WindowToUse = WindowUse.Window1;
		protected int Mode;
		
		public Window_Basic_HDMA()
		{
			L1 = Properties.Resources.BG_Layer1;
			L2 = Properties.Resources.BG_Layer2;
			L3 = Properties.Resources.BG_Layer3;
			LS = Properties.Resources.BG_Sprites;

			BlackMain = ColorWindow.Never;
			ColorMath = ColorWindow.Inside;
			UseSubscreen = true;
			DirectColorMode = false;
		}

		#region Public Members
		/// <summary>
		/// The mask to be used on the window. The black will be cut out, the white will be set transparent
		/// </summary>
		public FastBitmap Mask
		{
			get { return _mask; }
			set
			{
				_mask = value;
				this._tables = GetTableFromMask(_mask);
			}
		}

		/// <summary>
		/// Which window should be used for the effect
		/// </summary>
		public WindowUse WindowToUse
		{
			get { return _WindowToUse; }
			set { _WindowToUse = value; }
		}
		/// <summary>
		/// The BG the window should work on
		/// </summary>
		public WindowBG BGToHide
		{
			get { return _BGToHide; }
			set { _BGToHide = value; }
		}
		/// <summary>
		/// Indicates if the BG should be ereased inside the window or outside.
		/// True if outside.
		/// </summary>
		public bool Inverted
		{
			get { return _inverted; }
			set { _inverted = value; }
		}

		/// <summary>
		/// Which image to use for layer 1
		/// </summary>
		public Bitmap L1 { get; set; }
		/// <summary>
		/// Which image to use for layer 2
		/// </summary>
		public Bitmap L2 { get; set; }
		/// <summary>
		/// Which image to use for layer 3
		/// </summary>
		public Bitmap L3 { get; set; }
		/// <summary>
		/// Which image to use for the sprites layer
		/// </summary>
		public Bitmap LS { get; set; }

		public ColorWindow BlackMain { get; set; }
		public ColorWindow ColorMath { get; set; }
		public bool UseSubscreen { get; set; }
		public bool DirectColorMode { get; set; }
		#endregion

		/// <summary>
		/// Calculates the table need for the ASM from a mask
		/// </summary>
		/// <param name="mask">The mask needed</param>
		public abstract List<string[]> GetTableFromMask(FastBitmap mask);

		/// <summary>
		/// Draws the internal mask ontop of the multilayer screens, which screens to be effected by this is set be BGToHide member
		/// </summary>
		/// <returns></returns>
		public virtual FastBitmap Draw() { return Draw(this._mask); }
		/// <summary>
		/// Draws a mask ontop of the multilayer screen, which screens to be effected by this is set be BGToHide member
		/// </summary>
		/// <param name="mask">The mask that should be used to hide part of the layer. The black pixels of the mask decide which part should be hidden in a not inverted window
		/// The mask has to be setup using only black and white</param>
		/// <returns></returns>
		public abstract FastBitmap Draw(FastBitmap mask);

		public virtual String Code() { return Code(this._tables, "WindowTable", this._WindowToUse); }
		public virtual String Code(WindowUse window) { return Code(this._tables, "WindowTable", window); }
		public virtual String Code(List<string[]> tables, string TableName) { return Code(tables, TableName, this._WindowToUse); }
		public abstract String Code(List<String[]> tables, String TableName, WindowUse window);
	}

	class Window_HDMA : Window_Basic_HDMA
	{
		public Window_HDMA()
		{
			Mode = 0x01;
		}
		
		public override List<string[]> GetTableFromMask(FastBitmap mask)
		{
			List<List<int>> CollectionStartEnd = new List<List<int>>();
			List<bool> StartInside = new List<bool>();

			for (int y = 0; y < Scanlines; y++)
			{
				Color ColorToCheck = _mask.GetPixel(0, y);
				StartInside.Add(ColorToCheck.Name == "ffffffff");
				List<int> StartEnd = new List<int>();

				//Started bei 1, weil 0 sowieso nicht passen kann.
				for (int x = 1; x < 256; x++)
				{
					Color CurrentColor = _mask.GetPixel(x, y);
					if (CurrentColor != ColorToCheck)
					{
						StartEnd.Add(x);
						ColorToCheck = CurrentColor;
					}
				}

				CollectionStartEnd.Add(StartEnd);
			}

			List<int> Compare = CollectionStartEnd[0];
			int ScanlineCounter = 1;
			List<string[]> tables = new List<string[]>();

			for (int i = 1; i < CollectionStartEnd.Count; i++)
			{
				//if (!Dynamic)
				//{
					if (Compare.SequenceEqual(CollectionStartEnd[i]) && i != CollectionStartEnd.Count - 1 && ScanlineCounter != 0x80)
						ScanlineCounter++;
					else
					{
						String[] TableEntry = new String[3];
						TableEntry[0] = "$" + ScanlineCounter.ToString("X2");
						if (Compare.Count == 0)
						{
							TableEntry[1] = "$FF";
							TableEntry[2] = "$FE";
						}
						else if (Compare.Count == 1)
						{
							if (!StartInside[i])
							{
								TableEntry[1] = "$" + Compare[0].ToString("X2");
								TableEntry[2] = "$FF";
							}
							else
							{
								TableEntry[1] = "$00";
								TableEntry[2] = "$" + Compare[0].ToString("X2");
							}
						}
						else
						{
							TableEntry[1] = "$" + Compare[0].ToString("X2");
							TableEntry[2] = "$" + Compare[Compare.Count - 1].ToString("X2");
						}
						tables.Add(TableEntry);
						ScanlineCounter = 1;
						Compare = CollectionStartEnd[i];
					}
			}
			return tables;
		}
		
		/// <summary>
		/// Draws the mask needed for the windowing effect. This one draws a circle
		/// </summary>
		/// <param name="radius">Radius of the circle to be drawn</param>
		/// <param name="xpos">The x position of the center of the circle</param>
		/// <param name="ypos">The y position of the center of the circle</param>
		public virtual void DrawMask(int radius, int xpos, int ypos)
		{
			using (Bitmap b = EffectClasses.BitmapEffects.FromColor(Inverted ? Color.Black : Color.White, 256, 224))
			using (Graphics g = Graphics.FromImage(b))
			{
				g.FillEllipse(Inverted ? Brushes.White : Brushes.Black,
					new Rectangle(xpos - radius, ypos - radius, (radius * 2), (radius * 2)));
				Mask = new FastBitmap(b);
			}
		}

		/// <summary>
		/// Draws a mask ontop of the multilayer screen, which screens to be effected by this is set be BGToHide member
		/// </summary>
		/// <param name="mask">The mask that should be used to hide part of the layer. The black pixels of the mask decide which part should be hidden in a not inverted window
		/// The mask has to be setup using only black and white</param>
		/// <returns></returns>
		public override FastBitmap Draw(FastBitmap mask)
		{
			FastBitmap L1 = new FastBitmap(new Bitmap(this.L1));
			FastBitmap L2 = new FastBitmap(new Bitmap(this.L2));
			FastBitmap L3 = new FastBitmap(new Bitmap(this.L3));
			FastBitmap LS = new FastBitmap(new Bitmap(this.LS));

			if (!_BGToHide.HasFlag(WindowBG.Color))
			{
				if (_BGToHide.HasFlag(WindowBG.BG1)) L1.Mask(mask);
				if (_BGToHide.HasFlag(WindowBG.BG2)) L2.Mask(mask);
				if (_BGToHide.HasFlag(WindowBG.BG3)) L3.Mask(mask);
				if (_BGToHide.HasFlag(WindowBG.OBJ)) LS.Mask(mask);
			}
			FastBitmap Final = FastBitmap.Merge(LS, L3, L1, L2);

			if (!_BGToHide.HasFlag(WindowBG.Color))
				return Final;

			FastBitmap Final2 = new FastBitmap(256, 224);

			for (int y = 0; y < 224; y++)
				for (int x = 0; x < 256; x++)
				{
					//Color P = Image.GetPixel(x, y);
					//Console.WriteLine("{3},{4}\tR: {0}, G: {1}, B: {2}", P.R, P.G, P.B, x, y);
					if (mask.GetPixel(x, y).Name == "ffffffff")
						Final2.SetPixel(x, y, Final.GetPixel(x, y));
					else
						Final2.SetPixel(x, y, Color.Black);
				}

			return Final2;
		}

		public override String Code(List<String[]> tables, String TableName, WindowUse window)
		{
			if (_BGToHide == 0x00)
				throw new FormatException("You didn't select any BGs for the window to work on");

			int Base = 0x4330 + ((_channel - 3) * 0x10);
			int Register = RegisterWindow1Left + (int)window;
			String RegMode = (((Register % 0x100) << 8) + Mode).ToString("X4");

			String CodeToReturn = INIT_Label;

			int[] ValueForReg = new int[3];

			foreach (WindowBG BG in _BGToHide.GetFlags())
			{
				int i_BGToHide = (int)(Math.Log((int)BG) / Math.Log(2));

				int RegisterToUse = i_BGToHide / 2;
				int Shift = (i_BGToHide % 2) * 4;
				int Val = _inverted ? 0x03 : 0x02;
				ValueForReg[RegisterToUse] += (Val << Shift) << (int)window;
			}

			CodeToReturn += ";" + new String('-', 100) + "\n" +
				"; Window is set to work on " + _BGToHide.ToString() + "\n" +
				";" + new String('-', 100) + "\n\n";

			String TableToUse = "." + TableName.TrimStart('.');
			foreach (String[] SA in tables)
				TableToUse += "\n\tdb " + String.Join(", ", SA);


			if (ValueForReg[0] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[0].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.BG1) ? WindowBG.BG1.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.BG2) ? WindowBG.BG2.ToString() : "") + " for " + window.ToString() +
					"\nSTA $" + RegisterWindowBG1BG2.ToString("X2") + "\t\t\t; /\n";
			if (ValueForReg[1] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[1].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.BG3) ? WindowBG.BG3.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.BG4) ? WindowBG.BG4.ToString() : "") + " for " + window.ToString() +
					"\nSTA $" + RegisterWindowBG3BG4.ToString("X2") + "\t\t\t; /\n";
			if (ValueForReg[2] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[2].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.OBJ) ? WindowBG.OBJ.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.Color) ? WindowBG.Color.ToString() : "") + " for " + window.ToString() +
					"\nSTA $" + RegisterWindowOBJColor.ToString("X2") + "\t\t\t; /\n";

			if (_BGToHide.HasFlag(WindowBG.Color))
			{
				int CC = (int)BlackMain << 6;
				int MM = (int)ColorMath << 4;
				int s = UseSubscreen ? 2 : 0;
				int d = DirectColorMode ? 1 : 0;

				CodeToReturn += "LDA #$" + (CC + MM + d + s).ToString("X2") + "\t\t; \\  Set main screen black: " + BlackMain + "\n" +
					"STA $44\t\t\t;  | Disable color math on window: " + ColorMath + "\n" +
					"\t\t\t; / " + (UseSubscreen ? "" : "Don't") + " Use subscreen, " + (DirectColorMode ? "" : "Don't") + " Use direct color mode";
			}

			CodeToReturn += "\n" +
				"REP #$20\t\t;\\\n" +
				"LDA #$" + RegMode + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
				"STA $" + Base.ToString("X") + "\t\t; | 43" + _channel + "0 = Mode, 43" + _channel + "1 = Register\n" +
				"LDA #." + TableName.TrimStart('.') + "\t; | Address of HDMA table\n" +
				"STA $" + (Base + 2).ToString("X4") + "\t\t; | 43" + _channel + "2 = Low-Byte of table, 43" + _channel + "3 = High-Byte of table\n" +
				"LDY.b #." + TableName.TrimStart('.') + ">>16\t; | Address of HDMA table, get bank byte\n" +
				"STY $" + (Base + 4).ToString("X4") + "\t\t; | 43" + _channel + "4 = Bank-Byte of table\n" +
				"SEP #$20\t\t;/\n" +
				"LDA #$" + (0x08 << (Channel - 3)).ToString("X2") + "\t\t;\\\n" +
				"TSB $0D9F\t\t;/ Enable HDMA channel " + Channel + "\n" +
				"RTS\n\n" + TableToUse;

			return CodeToReturn;
		}
	}

	class DynamicWindow_HDMA : Window_HDMA
	{
		protected int _Radius;
		protected int _FreeRAM = 0x7F9F00;
		public int FreeRAM
		{
			get { return _FreeRAM; }
			set
			{
				if (value < 0x7E0000 || value > 0x7FFFFF)
					throw new InvalidOperationException("The given address is not RAM");
				else
					_FreeRAM = value;
			}
		}


		public bool DynamicX { get; set; }
		public bool DynamicY { get; set; }
		
		public override List<string[]> GetTableFromMask(FastBitmap mask)
		{

			List<List<int>> CollectionStartEnd = new List<List<int>>();
			List<bool> StartInside = new List<bool>();

			for (int y = 0; y < Scanlines; y++)
			{
				Color ColorToCheck = _mask.GetPixel(0, y);
				StartInside.Add(ColorToCheck.Name == "ffffffff");
				List<int> StartEnd = new List<int>();

				//Started bei 1, weil 0 sowieso nicht passen kann.
				for (int x = 1; x < 256; x++)
				{
					Color CurrentColor = _mask.GetPixel(x, y);
					if (CurrentColor != ColorToCheck)
					{
						StartEnd.Add(x);
						ColorToCheck = CurrentColor;
					}
				}

				CollectionStartEnd.Add(StartEnd);
			}

			List<int> Compare = CollectionStartEnd[0];
			int ScanlineCounter = 1;
			List<string[]> tables = new List<string[]>();

			int Startup = DynamicY ? 0 : 1;

			for (int i = Startup; i < CollectionStartEnd.Count; i++)
			{
				if (Compare.SequenceEqual(CollectionStartEnd[i]) && i != CollectionStartEnd.Count - 1 && ScanlineCounter != 0x80 && !DynamicY)
					ScanlineCounter++;
				else
				{
					String[] TableEntry = new String[2 + Startup];
					if (!DynamicY)
						TableEntry[0] = "$" + ScanlineCounter.ToString("X2");

					if (Compare.Count == 0)
					{
						if (!DynamicY)
						{
							TableEntry[Startup + 0] = "$FF";
							TableEntry[Startup + 1] = "$FE";
						}
					}
					else if (Compare.Count == 1)
					{
						if (!StartInside[i])
						{
							if (DynamicX)
								TableEntry[Startup + 0] = "$" + ((byte)((byte)Compare[0] - (byte)0x80)).ToString("X2");
							else
								TableEntry[Startup + 0] = "$" + Compare[0].ToString("X2");
							TableEntry[Startup + 1] = "$FF";
						}
						else
						{
							TableEntry[Startup + 0] = "$00";
							if (DynamicX)
								TableEntry[Startup + 1] = "$" + ((byte)((byte)Compare[0] - (byte)0x80)).ToString("X2");
							else
								TableEntry[Startup + 1] = "$" + Compare[0].ToString("X2");
						}
					}
					else
					{
						if (DynamicX)
						{
							TableEntry[Startup + 0] = "$" + ((byte)((byte)Compare[0] - (byte)0x80)).ToString("X2");
							TableEntry[Startup + 1] = "$" + ((byte)((byte)Compare[Compare.Count - 1] - (byte)0x80)).ToString("X2");
						}
						else
						{
							TableEntry[Startup + 0] = "$" + Compare[0].ToString("X2");
							TableEntry[Startup + 1] = "$" + Compare[Compare.Count - 1].ToString("X2");
						}
					}

					if (TableEntry[0] != null)
						tables.Add(TableEntry);
					ScanlineCounter = 1;
					Compare = CollectionStartEnd[i];
				}
			}
			return tables;
		}
		
		/// <summary>
		/// Draws the mask needed for the windowing effect. This one draws a circle
		/// </summary>
		/// <param name="radius">Radius of the circle to be drawn</param>
		/// <param name="xpos">The x position of the center of the circle</param>
		/// <param name="ypos">The y position of the center of the circle</param>
		public override void DrawMask(int radius, int xpos, int ypos)
		{
			_Radius = radius;
			using (Bitmap b = EffectClasses.BitmapEffects.FromColor(Inverted ? Color.Black : Color.White, 256, 224))
			using (Graphics g = Graphics.FromImage(b))
			{
				g.FillEllipse(Inverted ? Brushes.White : Brushes.Black,
					new Rectangle((DynamicX ? 128 : xpos) - radius,(DynamicY ? 112 : ypos) - radius, (radius * 2), (radius * 2)));
				Mask = new FastBitmap(b);
			}
		}

		public const int minGrenze = 0xFFF0;  //Alles schwarz ab hier und weniger (höher)
		public const int maxGrenze = 0x00D2;  //Alles schwarz ab hier und mehr (tiefer)

		public override string Code(List<string[]> tables, string TableName, WindowUse window)
		{
			if(!DynamicX && !DynamicY)
				return base.Code(tables, TableName, window);
					   
			if (_BGToHide == 0x00)
				throw new FormatException("You didn't select any BGs for the window to work on");

			int Base = 0x4330 + ((_channel - 3) * 0x10);
			int Register = RegisterWindow1Left + (int)window;
			String RegMode = (((Register % 0x100) << 8) + Mode).ToString("X4");

			String CodeToReturn = INIT_Label;

			int[] ValueForReg = new int[3];

			foreach (WindowBG BG in _BGToHide.GetFlags())
			{
				int i_BGToHide = (int)(Math.Log((int)BG) / Math.Log(2));

				int RegisterToUse = i_BGToHide / 2;
				int Shift = (i_BGToHide % 2) * 4;
				int Val = _inverted ? 0x03 : 0x02;
				ValueForReg[RegisterToUse] += (Val << Shift) << (int)window;
			}

			CodeToReturn += ";" + new String('-', 100) + "\n" +
				"; Window is set to work on " + _BGToHide.ToString() + "\n" +
				";" + new String('-', 100) + "\n\n";

			String TableToUse = "." + TableName.TrimStart('.');
			foreach (String[] SA in tables)
				TableToUse += "\n\tdb " + String.Join(", ", SA);

			if (ValueForReg[0] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[0].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.BG1) ? WindowBG.BG1.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.BG2) ? WindowBG.BG2.ToString() : "") + " for " + window.ToString() +
					"\nSTA $" + RegisterWindowBG1BG2.ToString("X2") + "\t\t\t; /\n";
			if (ValueForReg[1] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[1].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.BG3) ? WindowBG.BG3.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.BG4) ? WindowBG.BG4.ToString() : "") + " for " + window.ToString() +
					"\nSTA $" + RegisterWindowBG3BG4.ToString("X2") + "\t\t\t; /\n";
			if (ValueForReg[2] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[2].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.OBJ) ? WindowBG.OBJ.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.Color) ? WindowBG.Color.ToString() : "") + " for " + window.ToString() +
					"\nSTA $" + RegisterWindowOBJColor.ToString("X2") + "\t\t\t; /\n";

			if (_BGToHide.HasFlag(WindowBG.Color))
			{
				int CC = (int)BlackMain << 6;
				int MM = (int)ColorMath << 4;
				int s = UseSubscreen ? 2 : 0;
				int d = DirectColorMode ? 1 : 0;

				CodeToReturn += "LDA #$" + (CC + MM + d + s).ToString("X2") + "\t\t; \\  Set main screen black: " + BlackMain + "\n" +
					"STA $44\t\t\t;  | Disable color math on window: " + ColorMath + "\n" +
					"\t\t\t; / " + (UseSubscreen ? "" : "Don't") + " Use subscreen, " + (DirectColorMode ? "" : "Don't") + " Use direct color mode";
			}

			CodeToReturn += "\n" +
				"REP #$20\t\t;\\\n" +
				"LDA #$" + RegMode + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
				"STA $" + Base.ToString("X") + "\t\t; | 43" + _channel + "0 = Mode, 43" + _channel + "1 = Register\n" +
				"LDA #$" + (_FreeRAM & 0xFFFF).ToString("X4") + "\t\t; | Address of HDMA table\n" +
				"STA $" + (Base + 2).ToString("X4") + "\t\t; | 43" + _channel + "2 = Low-Byte of table, 43" + _channel + "3 = High-Byte of table\n" +
				"LDY.b #$" + (_FreeRAM>>16).ToString("X2") + "\t\t; | Address of HDMA table, get bank byte\n" +
				"STY $" + (Base + 4).ToString("X4") + "\t\t; | 43" + _channel + "4 = Bank-Byte of table\n" +
				"SEP #$20\t\t;/\n" +
				"LDA #$" + (0x08 << (Channel - 3)).ToString("X2") + "\t\t;\\\n" +
				"TSB $0D9F\t\t;/ Enable HDMA channel " + Channel + "\n" +
				"RTS\n\n" + MAIN_Label;

			if (DynamicX && !DynamicY)
			{
				#region Code X
				bool Bit8 = ((_tables.Count * 3) - 1) < 256;
				CodeToReturn +=
					"LDA ." + TableName.TrimStart('.') + "\t;\\  Load scanlines from table\n" +
					"STA $" + _FreeRAM.ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"LDA ." + TableName.TrimStart('.') + "+1\t; |  Load left position (needs to be bigger than right)\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"LDA ." + TableName.TrimStart('.') + "+2\t; |  Load right position (needs to be smaller than left)\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + "\t\t;/ Store to freeRAM table\n";

				if (_tables[1][1] == "$FF")
				{
					CodeToReturn +=
						"LDA ." + TableName.TrimStart('.') + "+3\t;\\  Load scanlines from table\n" +
						"STA $" + (_FreeRAM + 3).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
						"LDA ." + TableName.TrimStart('.') + "+4\t; |  Load left position (needs to be bigger than right)\n" +
						"STA $" + (_FreeRAM + 4).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
						"LDA ." + TableName.TrimStart('.') + "+5\t; |  Load right position (needs to be smaller than left)\n" +
						"STA $" + (_FreeRAM + 5).ToString("X6") + "\t\t;/ Store to freeRAM table\n";
				}

				CodeToReturn += "\n" +
					(Bit8 ? "" : "REP #$10\t\t;\\  Setup X/Y to be 16bit so that the table can be read to the end\n") +
					"LDX #$" + (Bit8 ? "" : "00") + "03\t\t;/  Preset X for proper indexing\n" +
					".Loop\n" +
					"LDA ." + TableName.TrimStart('.') + ",x\t;\\ Load scanlines from table\n" +
					"STA $" + _FreeRAM.ToString("X6") + ",x\t\t;/ Store to freeRAM table\n\n" +

					"LDA ." + TableName.TrimStart('.') + "+1,x\t;\\  Load left position from table\n" +
					"CLC : ADC $7E\t\t; | Add Mario's X-position onscreen\n" +
					"BCS .skip\t\t\t; | The addidtion should cause an overflow. If it does, skip next command\n" +
					"LDA #$00\t\t; | If no overflow happens, than Mario's to far to the left, the result would be bigger than the right position\n" +
					".skip\t\t\t; | thus, the result is set back to start at #$00\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + ",x\t\t;/  Store left position to freeRAM table\n\n" +

					"LDA ." + TableName.TrimStart('.') + "+2,x\t;\\  Load right position from table\n" +
					"CLC : ADC $7E\t\t; | Add Mario's X-position onscreen\n" +
					"BCC .skip2\t\t; | The addidtion shouldn't cause an overflow. If it doesn't, skip next command\n" +
					"LDA #$FF\t\t; | If an overflow happens, than Mario's to far to the right, the result would be smaller than the left position\n" +
					".skip2\t\t\t; | thus, the result is set back to start at #$FF\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + ",x\t\t;/  Store right position to freeRAM table\n\n" +

					"INX : INX : INX\t\t;\\  Increase X three times for proper indexing\n" +
					"CPX #$" + ((_tables.Count * 3) - 3).ToString(Bit8 ? "X2" : "X4") + "\t\t; | Compare to see if the end of table has been reached\n" +
					"BCC .Loop\t\t;/  Loopback if not at the end\n\n" +

					"LDA ." + TableName.TrimStart('.') + ",x\t;\\  Load scanlines from table\n" +
					"STA $" + _FreeRAM.ToString("X6") + ",x\t\t; | Store to freeRAM table\n" +
					"LDA ." + TableName.TrimStart('.') + "+1,x\t; |  Load left position (needs to be bigger than right)\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + ",x\t\t; | Store to freeRAM table\n" +
					"LDA ." + TableName.TrimStart('.') + "+2,x\t; |  Load right position (needs to be smaller than left)\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + ",x\t\t; | Store to freeRAM table\n" +
					"LDA #$00\t\t; | Load zero\n" +
					"STA $" + (_FreeRAM + 3).ToString("X6") + ",x\t\t;/  Store end of HDMA table to freeRAM table\n" +
					(Bit8 ? "" : "SEP #$10\t\t; Set X/Y back to 8bit or bad things will happen\n") +
					"RTS\n\n";

				#endregion
			}
			else if (!DynamicX && DynamicY)
			{
				#region Code Y
				CodeToReturn +=
					"LDX #$00\t\t;\\  Preloading X with #$00 \n" +
					"STZ $00\t\t\t; | $00 will be used later for indexing\n" +
					"REP #$20\t\t; | 16bit math\n" +
					"LDA $80\t\t\t; | get Mario's Y position on screen\n" +
					"SEC\t\t\t; | set carry for SBC\n" +
					"SBC #$" + (maxGrenze + _Radius).ToString("X4") + "\t\t; |\n" +
					"CMP #$" + ((minGrenze - _Radius) - (maxGrenze + _Radius) + 1).ToString("X4") + "\t\t; | check if Mario's too high above the screen or too far below\n" +
					"BCC .black\t\t;/  if so, make the whole screen black\n\n" +

					"LDA $80\t\t\t;\\  load Y position again\n" +
					"SBC #$" + (minGrenze - _Radius + 1).ToString("X4") + "\t\t; | carry still set\n" +
					"CMP #$" + (_Radius * 2 - 1).ToString("X4") + "\t\t; | check if Mario is high enough for the circle to not be fully drawn\n" +
					"BCS .dontset\t\t; | if not, don't increase $00\n" +
					"INC $00\t\t\t;/  $00 will serve as indicator if we need to pre-index the table\n" +
					".dontset\n\n" +
					"LDA #$0000\t\t;\\  clear A high and low\n" +
					"SEP #$20\t\t; | no more 16bit\n" +
					"LDA $80\t\t\t; | get low byte of Y position\n" +
					"SEC : SBC #$" + (_Radius - 17).ToString("X2") + "\t\t; | get how many scanlines need to be black above cirle\n" +
					"LDY $00\t\t\t; | load $00 in Y to not lose A\n" +
					"BNE .useindex\t\t; |\n" +
					"CMP #$81\t\t; | check if it's more than 80\n" +
					"BCC .notmore80\t\t;/  if not, no need for substracting and preseting\n\n" +

					"SBC #$80\t\t;\\  substract 80 to get the needed remaining scanlines\n" +
					"STA $" + (_FreeRAM + 3).ToString("X6") + "\t\t; | set remaining scanline in the next table entry\n" +
					"LDA #$FF\t\t; |  Left position of the window need to be bigger than right to be blank\n" +
					"STA $" + (_FreeRAM + 4).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"LDA #$00\t\t; | Right position of the window need to be smaller than left to be blank\n" +
					"STA $" + (_FreeRAM + 5).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"LDX #$03\t\t; | later index now 3 higher, to not overwrite what we just did\n" +
					"LDA #$80\t\t;/  Load A with 80 as we still need to store the first table entry\n\n" +

					".notmore80\t\t;\\  code jumps here if we don't need to handle more than 80 black scanlines\n" +
					"STA $" + _FreeRAM.ToString("X6") + "\t\t ; | number of blank scanlines before the cirle\n" +
					"LDA #$FF\t\t;\\  Left position of the window need to be bigger than right to be blank\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"LDA #$00\t\t; | Right position of the window need to be smaller than left to be blank\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"INX : INX : INX\t\t;/ Increase X by 3 for indexing (now 6 if we handles 80+ scanlines, otherwise 3)\n\n" +

					".useindex\n" +
					"EOR #$FF\t\t;\\  calculate A * -1\n" +
					"INC A\t\t\t; | so we start indexing at the table where circle needs to start in case it' too high\n" +
					"REP #$30\t\t; | indexing may need higher values\n" +
					"ASL\t\t\t;/  times two, since the table has two bytes per scanline\n" +
					"TAY\t\t\t; Transfer A to Y for indexing\n" +
					"SEP #$20\t\t; A go back to 8bit\n\n" +

					".Loop\t\t\t; Label to repeat the loop\n\n" +

					"LDA #$01\t\t;\\  Set scanline count to 1\n" +
					"STA $" + _FreeRAM.ToString("X6") + ",x\t\t; | Store to freeRAM table\n" +
					"LDA ." + TableName.TrimStart('.') + ",y\t; |  Load left position from table\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + ",x\t\t; | Store to freeRAM table\n" +
					"LDA ." + TableName.TrimStart('.') + "+1,y\t; |  Load right from table\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + ",x\t\t;/ Store to freeRAM table\n\n" +

					"INY : INY\t\t;\\  Increase Y by two for next table indexing\n" +
					"INX : INX : INX\t\t;/  Increase X by three for next freeRAM indexing\n" +
					"CPY #$" + ((_tables.Count * 2) - 1).ToString("X4") + "\t\t;\\  Compare Y with length of table\n" +
					"BCC .Loop\t\t;/  If less, we gotta continue\n\n" +

					".black\t\t\t;\\  Jump here if the whole screen is black\n" +
					"SEP #$20\t\t;/  A to 8bit in case we jumped here from .black\n" +
					"LDA #$01\t\t;\\  Scanline counter (lenght doesn't really matter here)\n" +
					"STA $" + _FreeRAM.ToString("X6") + ",x\t\t; | Store to freeRAM table\n" +
					"LDA #$FF\t\t; | After circle is put into freeRAM table\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + ",x\t\t; | Another blank scanline is added\n" +
					"LDA #$00\t\t; | After the HDMA table reaches its end, it will draw the last line all the way to the bottom\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + ",x\t\t; | Thus drawing a blank below the circle all the way to the bottom\n" +
					"STA $" + (_FreeRAM + 3).ToString("X6") + ",x\t\t;/ Store $00 as next scanline count to freeRAM table to indicate end of HDMA\n\n" +

					"SEP #$10\t\t; X/Y back to 8bit or giant cookie monster will pop uo and eat your face :>\n" +
					"RTS\n\n";
				#endregion
			}
			else
			{
				#region Code XY
				CodeToReturn +=

					"LDX #$00\t\t;\\  Preloading X with #$00 \n" +
					"STZ $00\t\t\t; | $00 will be used later for indexing\n" + 
					"REP #$20\t\t; | 16bit math\n" +
					"LDA $80\t\t\t; | get Mario's Y position on screen\n" +
					"SEC\t\t\t; | set carry for SBC\n" + 
					"SBC #$" + (maxGrenze + _Radius).ToString("X4") + "\t\t; |\n" + 
					"CMP #$" + ((minGrenze - _Radius) - (maxGrenze + _Radius) + 1).ToString("X4") + "\t\t; | check if Mario's too high above the screen or too far below\n" + 
					"BCC .black\t\t;/  if so, make the whole screen black\n\n" +

					"LDA $80\t\t\t;\\  load Y position again\n" +
					"SBC #$" + (minGrenze - _Radius + 1).ToString("X4") + "\t\t; | carry still set\n" +
					"CMP #$" + (_Radius * 2 - 1).ToString("X4")+ "\t\t; | check if Mario is high enough for the circle to not be fully drawn\n" +
					"BCS .dontset\t\t; | if not, don't increase $00\n" +
					"INC $00\t\t\t;/  $00 will serve as indicator if we need to pre-index the table\n" +
					".dontset\n\n" + 
					"LDA #$0000\t\t;\\  clear A high and low\n" +
					"SEP #$20\t\t; | no more 16bit\n" +
					"LDA $80\t\t\t; | get low byte of Y position\n" +
					"SEC : SBC #$" + (_Radius - 17).ToString("X2") + "\t\t; | get how many scanlines need to be black above cirle\n" +
					"LDY $00\t\t\t; | load $00 in Y to not lose A\n" + 
					"BNE .useindex\t\t; |\n" +
					"CMP #$81\t\t; | check if it's more than 80\n" + 
					"BCC .notmore80\t\t;/  if not, no need for substracting and preseting\n\n" +

					"SBC #$80\t\t;\\  substract 80 to get the needed remaining scanlines\n"+
					"STA $" + (_FreeRAM + 3).ToString("X6") + "\t\t; | set remaining scanline in the next table entry\n" +
					"LDA #$FF\t\t; |  Left position of the window need to be bigger than right to be blank\n" +
					"STA $" + (_FreeRAM + 4).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"LDA #$00\t\t; | Right position of the window need to be smaller than left to be blank\n" +
					"STA $" + (_FreeRAM + 5).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"LDX #$03\t\t; | later index now 3 higher, to not overwrite what we just did\n" +
					"LDA #$80\t\t;/  Load A with 80 as we still need to store the first table entry\n\n" +
					
					".notmore80\t\t;\\  code jumps here if we don't need to handle more than 80 black scanlines\n" +
					"STA $" + _FreeRAM.ToString("X6") + "\t\t ; | number of blank scanlines before the cirle\n" +
					"LDA #$FF\t\t;\\  Left position of the window need to be bigger than right to be blank\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"LDA #$00\t\t; | Right position of the window need to be smaller than left to be blank\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + "\t\t; | Store to freeRAM table\n" +
					"INX : INX : INX\t\t;/ Increase X by 3 for indexing (now 6 if we handles 80+ scanlines, otherwise 3)\n\n" +
					
					".useindex\n" +
					"EOR #$FF\t\t;\\  calculate A * -1\n" +
					"INC A\t\t\t; | so we start indexing at the table where circle needs to start in case it' too high\n" +
					"REP #$30\t\t; | indexing may need higher values\n" +
					"ASL\t\t\t;/  times two, since the table has two bytes per scanline\n" +
					"TAY\t\t\t; Transfer A to Y for indexing\n" +
					"SEP #$20\t\t; A go back to 8bit\n\n" +

					".Loop\t\t\t; Label to repeat the loop\n\n" +

					"LDA #$01\t\t;\\  Set scanline count to 1\n" +
					"STA $" + _FreeRAM.ToString("X6") + ",x\t\t; | Store to freeRAM table\n\n" +

					"LDA ." + TableName.TrimStart('.') + ",y\t; |  Load left position from table\n" +
					"CLC : ADC $7E\t\t; | Add Mario's X-position onscreen\n" +
					"BCS .skipleft\t\t; | The addidtion should cause an overflow. If it does, skip next command\n" +
					"LDA #$00\t\t; | If no overflow happens, than Mario's to far to the left, the result would be bigger than the right position\n" +
					".skipleft\t\t\t; | thus, the result is set back to start at #$00\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + ",x\t\t;/  Store left position to freeRAM table\n\n" +
					
					"LDA ." + TableName.TrimStart('.') + "+1,y\t;\\  Load right position from table\n" +
					"CLC : ADC $7E\t\t; | Add Mario's X-position onscreen\n" +
					"BCC .skipright\t\t; | The addidtion shouldn't cause an overflow. If it doesn't, skip next command\n" +
					"LDA #$FF\t\t; | If an overflow happens, than Mario's to far to the right, the result would be smaller than the left position\n" +
					".skipright\t\t\t; | thus, the result is set back to start at #$FF\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + ",x\t\t;/  Store right position to freeRAM table\n\n" +

					"INY : INY\t\t\t;\\  Increase Y by two for next table indexing\n" +
					"INX : INX : INX\t\t;/  Increase X by three for next freeRAM indexing\n" +
					"CPY #$" + ((_tables.Count * 2) - 1).ToString("X4") + "\t\t;\\  Compare Y with length of table\n" +
					"BCC .Loop\t\t;/  If less, we gotta continue\n\n" +

					".black\t\t\t;\\  Jump here if the whole screen is black\n" +
					"SEP #$20\t\t;/  A to 8bit in case we jumped here from .black\n" +
					"LDA #$01\t\t;\\  Scanline counter (lenght doesn't really matter here)\n" +
					"STA $" + _FreeRAM.ToString("X6") + ",x\t\t; | Store to freeRAM table\n" +
					"LDA #$FF\t\t; | After circle is put into freeRAM table\n" +
					"STA $" + (_FreeRAM + 1).ToString("X6") + ",x\t\t; | Another blank scanline is added\n" +
					"LDA #$00\t\t; | After the HDMA table reaches its end, it will draw the last line all the way to the bottom\n" +
					"STA $" + (_FreeRAM + 2).ToString("X6") + ",x\t\t; | Thus drawing a blank below the circle all the way to the bottom\n" +
					"STA $" + (_FreeRAM + 3).ToString("X6") + ",x\t\t;/ Store $00 as next scanline count to freeRAM table to indicate end of HDMA\n\n" +

					"SEP #$10\t\t; X/Y back to 8bit or giant cookie monster will pop uo and eat your face :>\n" +
					"RTS\n\n";

				#endregion
			}

			CodeToReturn += TableToUse;
			return CodeToReturn;
		}
	}
	
	class MultiWindow_HDMA : Window_Basic_HDMA
	{                
		class MultiWindowTable
		{
			public string Left1;
			public string Left2;
			public string Right1;
			public string Right2;

			public MultiWindowTable(string left1, string right1, string left2, string right2)
			{
				Left1 = left1;
				Left2 = left2;
				Right1 = right1;
				Right2 = right2;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				MultiWindowTable cmp = (MultiWindowTable)obj;
				return (cmp.Left1 == this.Left1 &&
					cmp.Left2 == this.Left2 &&
					cmp.Right1 == this.Right1 &&
					cmp.Right2 == this.Right2);
			}
		}

		public Window_HDMA Window1 { get; set; }
		public Window_HDMA Window2 { get; set; }
		public bool UseAble { get; set; }

		public MultiWindow_HDMA()
		{
			/*
			L1 = Properties.Resources.BG_Layer1;
			L2 = Properties.Resources.BG_Layer2;
			L3 = Properties.Resources.BG_Layer3;
			LS = Properties.Resources.BG_Sprites;
			*/

			Window1 = new Window_HDMA();
			Window2 = new Window_HDMA();
			Window1.WindowToUse = WindowUse.Window1;
			Window2.WindowToUse = WindowUse.Window2;
			Window1.Inverted = false;
			Window2.Inverted = false;
			Mode = 4;
		}

		public override List<string[]> GetTableFromMask(FastBitmap mask)
		{            
			UseAble = CheckAndSplitMask(mask);
			if (!UseAble)
				return null;
			MultiWindowTable[] TableSave = new MultiWindowTable[Scanlines];

			int Total = 0;
			foreach (string[] SA in Window1.Tables)
			{
				int Scan = Convert.ToInt32(SA[0].Trim('$'), 16);
				for (int i = 0; i < Scan; i++)
					TableSave[i + Total] = new MultiWindowTable(SA[1], SA[2], "$FF", "$00");
				Total += Scan;
			}
			Total = 0;
			foreach (string[] SA in Window2.Tables)
			{
				int Scan = Convert.ToInt32(SA[0].Trim('$'), 16);
				for (int i = 0; i < Scan; i++)
				{
					TableSave[i + Total].Left2 = SA[1];
					TableSave[i + Total].Right2 = SA[2];
				}
				Total += Scan;
			}

			List<string[]> tables = new List<string[]>();
			MultiWindowTable mwt = TableSave[0];
			for (int i = 1, k = 1; i < Scanlines; i++, k++)
			{
				if(mwt.Equals(TableSave[i]) && k <= 0x80)
					continue;
				tables.Add(new String[] { "$" + k.ToString("X2"), mwt.Left1, mwt.Right1, mwt.Left2, mwt.Right2 });
				mwt = TableSave[i];
				k = 0;
			}

			return tables;
		}

		public int[] BlackLines { get; set; }
		public int[] WhiteLines { get; set; }

		private bool CheckAndSplitMask(FastBitmap mask)
		{      
			ColPos[][] C_Arr = new ColPos[Scanlines][];

			for (int y = 0; y < Scanlines; y++)
			{
				Color ColorToCheck = mask.GetPixel(0, y);
				List<ColPos> CList = new List<ColPos>();
				CList.Add(new ColPos(ColorToCheck, 0));

				for (int x = 1; x < 256; x++)
				{
					if (ColorToCheck == mask.GetPixel(x, y))
						continue;

					ColorToCheck = mask.GetPixel(x, y);
					CList.Add(new ColPos(ColorToCheck, x));
				}
				C_Arr[y] = CList.ToArray();
			}

			bool black3 = C_Arr.Any((CA => CA.Count(c => c.Color.Name == "ff000000") > 2));
			bool white3 = C_Arr.Any((CA => CA.Count(c => c.Color.Name == "ffffffff") > 2));

			IEnumerable<ColPos[]> Puff = C_Arr.Where((CA => CA.Count(c => c.Color.Name == "ff000000") > 2));
			BlackLines = new int[Puff.Count()];
			int i = 0;
			foreach (ColPos[] cp in Puff)
			{
				BlackLines[i] = Array.IndexOf(C_Arr, cp);
				i++;
			}

			Puff = C_Arr.Where((CA => CA.Count(c => c.Color.Name == "ffffffff") > 2));
			WhiteLines = new int[Puff.Count()];
			i = 0;
			foreach(ColPos[] cp in Puff)
			{
				WhiteLines[i] = Array.IndexOf(C_Arr, cp);
				i++;
			}

			if (black3 && white3)
				return false;

			FastBitmap FBM1 = new FastBitmap(EffectClasses.BitmapEffects.FromColor(Color.White, 256, 224));
			FastBitmap FBM2 = new FastBitmap(EffectClasses.BitmapEffects.FromColor(Color.White, 256, 224));
			
			//Wenn irgendeine Zeile im Bild diese Bedingung benötigt (3S, 2W), muss ein Fenster invertiert werden.
			bool WindowInverted = C_Arr.Any(cp => (cp.Count(c => c.Color.Name == "ff000000") == 3 &&
				cp.Count(c => c.Color.Name == "ffffffff") == 2));

			for (int y = 0; y < Scanlines; y++)
			{
				int CountBlack = C_Arr[y].Count(cp => cp.Color.Name == "ff000000");
				int CountWhite = C_Arr[y].Count(cp => cp.Color.Name == "ffffffff");

				if (CountBlack == 1 && CountWhite == 0)
				{
					// Ganze Zeile schwarz.
					// Wenn fenster invertiert ist, kann die Zeile weiss bleiben.
					if (!WindowInverted)
						for (int x = 0; x < 256; x++)           // Wenn nicht, 
							FBM1.SetPixel(x, y, Color.Black);   // ganze Zeile schwarz ausmalen :P
				}
				else if (CountBlack == 2 && CountWhite == 1)
				{
					//Außen Schwarz innen Weiss.
					if (WindowInverted)     //Wenn Fenster 1 sowieso schon invertiert ist, dann kann der Effekt auch
											//nur mit einem Fenster gestalltet werden
						for (int x = C_Arr[y][1].Position; x < C_Arr[y][2].Position; x++)   //Der Teil, der in der Maske weiss ist
							FBM1.SetPixel(x, y, Color.Black);                               //Wird in für Fenster1 schwarz gefärbt

					else    //Wenn nicht
					{   
						for (int x = C_Arr[y][0].Position; x < C_Arr[y][1].Position; x++)   // Wird der erste schwarze Teil der maske
							FBM1.SetPixel(x, y, Color.Black);                               // auf Fenster1 übertragen,
						for (int x = C_Arr[y][2].Position; x < 256; x++)                    // und der 2. auf Fenster2
							FBM2.SetPixel(x, y, Color.Black);
					}
				}
				else if (CountBlack == 3 && CountWhite == 2)
				{
					//Schwarz - Weiss - Schwarz - Weiss - Schwarz
					//1 Fenster invertiert, 1 Fenster nicht invertiert

					//Die Frage ob ein invertiertes Schwarzes Fenster benutzt wird erübrigt sich hier,
					//weil die Bedingung dieser Abfrage die gleiche ist wie für die Invertierung :>

					for (int x = C_Arr[y][1].Position; x < C_Arr[y][4].Position; x++)   // Fenster1 wird schwarz gesetzt von Anfange des ersten weissen
						FBM1.SetPixel(x, y, Color.Black);                               // bis zum Ende des 2.
					for (int x = C_Arr[y][2].Position; x < C_Arr[y][3].Position; x++)   // und das Fenster2 wird schwarz wo der mittlere schwarze sich befindet
						FBM2.SetPixel(x, y, Color.Black);
				}
				else if (CountBlack == 3 && CountWhite == 3)
				{
					//... IMPOSSIBRU <.<
				}
				else if (CountBlack == 2 && CountWhite == 3)
				{
					//Weiss - Schwarz - Weiss - Schwarz - Weiss
					//2 Fenster nicht invertiert... oder mit überlagerungsbits spielen...

					//Dieser Teil kann nicht aufgerufen werden wenn Fenster1 invertiert ist, da dieser 3 Weisse benötigt.
					//und die Invertierung 3 Schwarze... ist beides vorhanden, wird abgebrochen.

					for (int x = C_Arr[y][1].Position; x < C_Arr[y][2].Position; x++)   // Fenster1 wird schwarz gesetzt für
						FBM1.SetPixel(x, y, Color.Black);                               // ersten schwarzen Teil der Maske
					for (int x = C_Arr[y][3].Position; x < C_Arr[y][4].Position; x++)   // und das Fenster2 für den 2.
						FBM2.SetPixel(x, y, Color.Black);
				}
				else if (CountBlack == 1 && CountWhite == 2)
				{
					//Außen weiss innen schwarz
					//1 Fenster nicht invertiert
					if (WindowInverted) //Wenn Fenster1 invertiert ist, muss Fenster2 benutzt werden und Fenster1 komplett Schwarz
					{
						for (int x = C_Arr[y][1].Position; x < C_Arr[y][2].Position; x++)   // Schwarz übertragen
							FBM2.SetPixel(x, y, Color.Black);
						for (int x = 0; x < 256; x++)
							FBM1.SetPixel(x, y, Color.Black);
					}
					else
						for (int x = C_Arr[y][1].Position; x < C_Arr[y][2].Position; x++)   // ...
							FBM1.SetPixel(x, y, Color.Black);
				}
				else if (CountBlack == 0 && CountWhite == 1)
				{
					//ganze zeile sichtbar...
					//entweder oder mit einem Fenster
					if (WindowInverted)
						for (int x = 0; x < 256; x++)           // Wenn nicht, 
							FBM1.SetPixel(x, y, Color.Black);   // ganze Zeile schwarz ausmalen :P
				}
				else if (CountBlack == 2 && CountWhite == 2)
				{
					//2 Fenster nicht invertiert
					//problem hier, man weiß nicht ob S-W-S-W oder W-S-W-S
					#region 2/2
					if (C_Arr[y][0].Color.Name == "ff000000") //wenn der erste Schwarz ist, dann ist S-W-S-W
					{
						if (WindowInverted) //Wenn das Fenster1 invertiert ist...
						{
							for (int x = C_Arr[y][1].Position; x < 256; x++)    // Wird der W-S-W Bereich schwarz gesetzt,
								FBM1.SetPixel(x, y, Color.Black);               // damit, invertiert, der erste Balken Schwarz wird
							for (int x = C_Arr[y][2].Position; x < C_Arr[y][3].Position; x++)
								FBM2.SetPixel(x, y, Color.Black);               // Fenster2 setzt den zweiten Schwarzen balken
						}
						else    //wenn nicht invertiert
						{
							for (int x = C_Arr[y][0].Position; x < C_Arr[y][1].Position; x++)   //Setzt den ersten Balken im F1
								FBM1.SetPixel(x, y, Color.Black);
							for (int x = C_Arr[y][2].Position; x < C_Arr[y][3].Position; x++)   //Setzt den ersten Balken im F1
								FBM2.SetPixel(x, y, Color.Black);
						}
					}
					else // wenn der erste nicht Schwarz ist, dann W-S-W-S
					{
						if (WindowInverted) //Wenn das Fenster1 invertiert ist...
						{
							for (int x = C_Arr[y][0].Position; x < C_Arr[y][3].Position; x++)   // Wird der W-S-W Bereich schwarz gesetzt,
								FBM1.SetPixel(x, y, Color.Black);                               // damit, invertiert, der erste Balken Schwarz wird
							for (int x = C_Arr[y][1].Position; x < C_Arr[y][2].Position; x++)   // Fenster2 setzt den zweiten Schwarzen balken
								FBM2.SetPixel(x, y, Color.Black);                               //
						}
						else    //wenn nicht invertiert
						{
							for (int x = C_Arr[y][1].Position; x < C_Arr[y][2].Position; x++)   //Setzt den ersten Balken im F1
								FBM1.SetPixel(x, y, Color.Black);
							for (int x = C_Arr[y][3].Position; x < 256; x++)                    //Setzt den ersten Balken im F1
								FBM2.SetPixel(x, y, Color.Black);
						}
					}
					#endregion
				}
				else if (CountBlack == 1 && CountWhite == 1)
				{
					//2 Fenster nicht invertiert
					//problem hier, man weiß nicht ob S-W oder W-S
					if (C_Arr[y][0].Color.Name == "ff000000") //wenn der erste Schwarz ist, dann ist S-W
					{
						if (WindowInverted) //Wenn das Fenster1 invertiert ist...
							for (int x = C_Arr[y][1].Position; x < 256; x++)    // Wird der weisse Bereich schwarz gesetzt,
								FBM1.SetPixel(x, y, Color.Black);               // damit, invertiert, der erste Balken Schwarz wird

						else    //wenn nicht invertiert
							for (int x = C_Arr[y][0].Position; x < C_Arr[y][1].Position; x++)   //Setzt den ersten Balken im F1
								FBM1.SetPixel(x, y, Color.Black);
					}
					else // wenn der erste nicht Schwarz ist, dann W-S
					{
						if (WindowInverted) //Wenn das Fenster1 invertiert ist...
							for (int x = C_Arr[y][0].Position; x < C_Arr[y][1].Position; x++)   // Wird der weisse Bereich schwarz gesetzt,
								FBM1.SetPixel(x, y, Color.Black);                               // damit, invertiert, der erste Balken Schwarz wird

						else    //wenn nicht invertiert
							for (int x = C_Arr[y][1].Position; x < 256; x++)                    //Setzt den ersten Balken im F1
								FBM2.SetPixel(x, y, Color.Black);
					}
				}
			}

			//((Bitmap)FBM1).Save("Mask1.png");
			//((Bitmap)FBM2).Save("Mask2.png");

			Window1.Inverted = WindowInverted;
			if(WindowInverted)
				Window1.Mask = new Bitmap(FBM1).Invert();
			else
				Window1.Mask = FBM1;
			Window2.Mask = FBM2;

			return true;
		}

		public override void SetChannel(System.Windows.Forms.RadioButton CH3Button, System.Windows.Forms.RadioButton CH4Button, System.Windows.Forms.RadioButton CH5Button)
		{
			base.SetChannel(CH3Button, CH4Button, CH5Button);
			Window1.SetChannel(CH3Button, CH4Button, CH5Button);
			Window2.SetChannel(CH3Button, CH4Button, CH5Button);
		}

		/// <summary>
		/// Draws a mask ontop of the multilayer screen, which screens to be effected by this is set be BGToHide member
		/// </summary>
		/// <param name="mask">The mask that should be used to hide part of the layer. The black pixels of the mask decide which part should be hidden in a not inverted window
		/// The mask has to be setup using only black and white</param>
		/// <returns></returns>
		public override FastBitmap Draw(FastBitmap mask)
		{
			FastBitmap L1 = new FastBitmap(new Bitmap(this.L1));
			FastBitmap L2 = new FastBitmap(new Bitmap(this.L2));
			FastBitmap L3 = new FastBitmap(new Bitmap(this.L3));
			FastBitmap LS = new FastBitmap(new Bitmap(this.LS));
			
			if (!_BGToHide.HasFlag(WindowBG.Color))
			{
				if (_BGToHide.HasFlag(WindowBG.BG1)) { L1.Mask(mask); }
				if (_BGToHide.HasFlag(WindowBG.BG2)) { L2.Mask(mask); }
				if (_BGToHide.HasFlag(WindowBG.BG3)) { L3.Mask(mask); }
				if (_BGToHide.HasFlag(WindowBG.OBJ)) { LS.Mask(mask); }
			}
			FastBitmap Final = FastBitmap.Merge(LS, L3, L1, L2);

			if (!_BGToHide.HasFlag(WindowBG.Color))
				return Final;

			FastBitmap Final2 = new FastBitmap(256, 224);

			for (int y = 0; y < 224; y++)
				for (int x = 0; x < 256; x++)
				{
					//Color P = Image.GetPixel(x, y);
					//Console.WriteLine("{3},{4}\tR: {0}, G: {1}, B: {2}", P.R, P.G, P.B, x, y);
					if (mask.GetPixel(x, y).Name == "ffffffff")
						Final2.SetPixel(x, y, Final.GetPixel(x, y));
					else
						Final2.SetPixel(x, y, Color.Black);
				}

			L1.LockBits();
			return Final2;
		}

		public override string Code(List<string[]> tables, string TableName, WindowUse window)
		{
			if(Window2.Tables.Count == 2 &&
				Window2.Tables[0].SequenceEqual(new string[]{"$80","$FF","$FE"}) &&
				Window2.Tables[1].SequenceEqual(new string[]{"$5F","$FF","$FE"}))
			{
				MessageBox.Show("The effect can be created using only one window.\n" +
					"Please select the winodow of your choice.", "Only one Window", MessageBoxButtons.OK, MessageBoxIcon.Information);
				ChooseWindow CW = new ChooseWindow();
				CW.ShowDialog();
				return Window1.Code((WindowUse)(int)CW.Window);
			}

			if (_BGToHide == 0x00)
				throw new FormatException("You didn't select any BGs for the window to work on");

			int Base = 0x4330 + ((_channel - 3) * 0x10);
			int Register = RegisterWindow1Left;
			String RegMode = (((Register % 0x100) << 8) + Mode).ToString("X4");

			String CodeToReturn = INIT_Label;

			int[] ValueForReg = new int[3];

			foreach (WindowBG BG in _BGToHide.GetFlags())
			{
				int i_BGToHide = (int)(Math.Log((int)BG) / Math.Log(2));

				int RegisterToUse = i_BGToHide / 2;
				int Shift = (i_BGToHide % 2) * 4;
				int Val = 0x08 + (Window1.Inverted ? 0x03 : 0x02);
				ValueForReg[RegisterToUse] += (Val << Shift);
			}

			CodeToReturn += ";" + new String('-', 100) + "\n" +
				"; Window is set to work on " + _BGToHide.ToString() + "\n" +
				";" + new String('-', 100) + "\n\n";

			String TableToUse = "." + TableName.TrimStart('.');
			foreach (String[] SA in tables)
				TableToUse += "\n\tdb " + String.Join(", ", SA);


			if (ValueForReg[0] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[0].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.BG1) ? WindowBG.BG1.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.BG2) ? WindowBG.BG2.ToString() : "") + " for both windows" +
					"\nSTA $" + RegisterWindowBG1BG2.ToString("X2") + "\t\t\t; /\n";
			if (ValueForReg[1] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[1].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.BG3) ? WindowBG.BG3.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.BG4) ? WindowBG.BG4.ToString() : "") + " for both windows" +
					"\nSTA $" + RegisterWindowBG3BG4.ToString("X2") + "\t\t\t; /\n";
			if (ValueForReg[2] != 0)
				CodeToReturn += "LDA #$" + ValueForReg[2].ToString("X2") + "\t\t; \\ Enable " + (_BGToHide.HasFlag(WindowBG.OBJ) ? WindowBG.OBJ.ToString() + ", " : "") +
					(_BGToHide.HasFlag(WindowBG.Color) ? WindowBG.Color.ToString() : "") + " for both windows" +
					"\nSTA $" + RegisterWindowOBJColor.ToString("X2") + "\t\t\t; /\n";

			if (_BGToHide.HasFlag(WindowBG.Color))
			{
				int CC = (int)BlackMain << 6;
				int MM = (int)ColorMath << 4;
				int s = UseSubscreen ? 2 : 0;
				int d = DirectColorMode ? 1 : 0;

				CodeToReturn += "LDA #$" + (CC + MM + d + s).ToString("X2") + "\t\t; \\  Set main screen black: " + BlackMain + "\n" +
					"STA $44\t\t\t;  | Disable color math on window: " + ColorMath + "\n" +
					"\t\t\t; / " + (UseSubscreen ? "" : "Don't") + " Use subscreen, " + (DirectColorMode ? "" : "Don't") + " Use direct color mode";
			}

			CodeToReturn += "\n" +
				"REP #$20\t\t;\\\n" +
				"LDA #$" + RegMode + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
				"STA $" + Base.ToString("X") + "\t\t; | 43" + _channel + "0 = Mode, 43" + _channel + "1 = Register\n" +
				"LDA #." + TableName.TrimStart('.') + "\t; | Address of HDMA table\n" +
				"STA $" + (Base + 2).ToString("X4") + "\t\t; | 43" + _channel + "2 = Low-Byte of table, 43" + _channel + "3 = High-Byte of table\n" +
				"LDY.b #." + TableName.TrimStart('.') + ">>16\t; | Address of HDMA table, get bank byte\n" +
				"STY $" + (Base + 4).ToString("X4") + "\t\t; | 43" + _channel + "4 = Bank-Byte of table\n" +
				"SEP #$20\t\t;/\n" +
				"LDA #$" + (0x08 << (Channel - 3)).ToString("X2") + "\t\t;\\\n" +
				"TSB $0D9F\t\t;/ Enable HDMA channel " + Channel + "\n" +
				"RTS\n\n" + TableToUse;

			return CodeToReturn;

		}
	}

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
}
