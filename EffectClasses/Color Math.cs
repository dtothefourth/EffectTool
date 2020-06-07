using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Extansion.Int;
using Extansion.Enum;

namespace EffectClasses
{
	/// <summary>
	/// Sets the circumstances for color math. Whether it will be added/substracted. Halfed and which layers are to participate
	/// </summary>
	[Flags]
	public enum ColorMathMode : byte {
		/// <summary>
		/// Subscreen or fixed color will be substracted from the mainscreen
		/// </summary>
		Substract = 0x80,
		/// <summary>
		/// Equation will be halfed before clipping
		/// </summary>
		Half = 0x40,
		/// <summary>
		/// Backdrop will be added. It represents color 0 of CGRAM
		/// </summary>
		Backdrop = 0x20,
		/// <summary>
		/// Sprites (objects) on the main screen will participate in color math
		/// </summary>
		OBJ = 0x10,
		/// <summary>
		/// Layer 4 on the main screen will participate in color math
		/// </summary>
		BG4 = 0x08,
		/// <summary>
		/// Layer 3 on the main screen will participate in color math
		/// </summary>
		BG3 = 0x04,
		/// <summary>
		/// Layer 2 on the main screen will participate in color math
		/// </summary>
		BG2 = 0x02,
		/// <summary>
		/// Layer 1 on the main screen will participate in color math
		/// </summary>
		BG1 = 0x01 
	}

	/// <summary>
	/// Which layers will show up on screen
	/// </summary>
	[Flags]
	public enum ScreenDesignation : byte
	{
		/// <summary>
		/// Sprites (objects) will be on the layer
		/// </summary>
		OBJ = 0x10,
		/// <summary>
		/// Layer 4 will be on the layer
		/// </summary>
		BG4 = 0x08,
		/// <summary>
		/// Layer 3 will be on the layer
		/// </summary>
		BG3 = 0x04,
		/// <summary>
		/// Layer 2 will be on the layer
		/// </summary>
		BG2 = 0x02,
		/// <summary>
		/// Layer 1 will be on the layer
		/// </summary>
		BG1 = 0x01 
	}

	/// <summary>
	/// What to do when the two windows overlap
	/// </summary>
	public enum WindowMaskLogic
	{
		/// <summary>
		/// The pixel will be masked if one or more widows reach it
		/// </summary>
		Or = 0,
		/// <summary>
		/// The pixel will be masked if both windows reach it
		/// </summary>
		And = 1,
		/// <summary>
		/// The pixel will be masked if one of the other (not both) windows reach it
		/// </summary>
		Xor = 2,
		/// <summary>
		/// The pixel will be masked if none or both windows reach it.
		/// </summary>
		XNor = 3,
	}

	/// <summary>
	/// Layers that can participate in color math
	/// </summary>
	[Flags]
	public enum WindowingLayers
	{
		BG1 = 0x01,
		BG2 = 0x02,
		BG3 = 0x04,
		BG4 = 0x08,
		/// <summary>
		/// 
		/// </summary>
		OBJ = 0x10,
		Color = 0x20,
	}


	public enum ColorAdditionalSelectOptions
	{
		Never = 0,
		Outside = 1,
		Inside = 2,
		Always = 3,
	}

	/// <summary>
	/// 
	/// </summary>
	public class ColorMath : ICodeProvider
	{
		/// <summary>
		/// The first object layer. BG2/BG3/BG3 will appear behind it. Used in any BG Mode
		/// </summary>
		public Bitmap BG1 { get; set; }
		/// <summary>
		/// The second object layer BG3/BG4 will appear behind it. Used in BG Mode 0 - 5
		/// </summary>
		public Bitmap BG2 { get; set; }
		/// <summary>
		/// The third object layer. BG4 will appear behind it. Used in BG Mode 0 and 1
		/// </summary>
		public Bitmap BG3 { get; set; }
		/// <summary>
		/// The fourth and last object layer. Only used in BG Mode 0
		/// </summary>
		public Bitmap BG4 { get; set; }
		/// <summary>
		/// The sprite layer. All other BGs will appear behind it
		/// </summary>
		public Bitmap OBJ { get; set; }
		/// <summary>
		/// $2132 Requires a Bitmap to also work with HDMA set colors
		/// </summary>
		public Bitmap FixedColor { get; set; }
		/// <summary>
		/// The Backdrop (aka color 0). It's the backcolor of the main screen. Uses a Bitmap for HDMA
		/// </summary>
		public Bitmap Backdrop { get; set; }

		/// <summary>
		/// A black/white mask for the windowing used in window 1.
		/// This will only influence the display from GetScreen but not the code
		/// </summary>
		public Bitmap WindowingMask1 { get; set; }
		/// <summary>
		/// A black/white mask for the windowing used in window 1.
		/// This will only influence the display from GetScreen but not the code
		/// </summary>
		public Bitmap WindowingMask2 { get; set; }

		/// <summary>
		/// The collection of all the BGs and the object layer.
		/// </summary>
		public BitmapCollection Collection
		{
			get
			{
				BitmapCollection col = new BitmapCollection();
				col.BG1 = BG1;
				col.BG2 = BG2;
				col.BG3 = BG3;
				col.BG4 = BG4;
				col.OBJ = OBJ;
				return col;
			}
			set
			{
				BG1 = value.BG1;
				BG2 = value.BG2;
				BG3 = value.BG3;
				BG4 = value.BG4;
				OBJ = value.OBJ;
			}
		}

		/// <summary>
		/// This value will later be used for $212C.
		/// ---o4321
		/// It sets which BGs are on the mainscreen.
		/// </summary>
		public ScreenDesignation MainScreenDesignation { get; set; }
		/// <summary>
		/// This value will later be used for $212D.
		/// ---o4321
		/// It sets which BGs are on the subscreen.
		/// </summary>
		public ScreenDesignation SubScreenDesignation { get; set; }

		/// <summary>
		/// This value will later be used for $212E.
		/// ---o4321
		/// It sets which BGs on the mainscreen the windowing will be used on.
		/// </summary>
		public ScreenDesignation MainScreenWindowMaskDesignation { get; set; }
		/// <summary>
		/// This value will later be used for $212F.
		/// ---o4321
		/// It sets which BGs on the subscreen the windowing will be used on.
		/// </summary>
		public ScreenDesignation SubScreenWindowMaskDesignation { get; set; }


		public WindowMaskLogic Bg1MaskLogic
		{
			get { return (WindowMaskLogic)(_maskLogicBgs & 0x03); }
			set 
			{
				_maskLogicBgs &= ~0x03;
				_maskLogicBgs |= (int)value;
			}
		}
		public WindowMaskLogic Bg2MaskLogic
		{
			get { return (WindowMaskLogic)((_maskLogicBgs & (0x03 << 2)) >> 2); }
			set
			{
				_maskLogicBgs &= ~(0x03 << 2);
				_maskLogicBgs |= (int)value << 2;
			}
		}
		public WindowMaskLogic Bg3MaskLogic
		{
			get { return (WindowMaskLogic)((_maskLogicBgs & (0x03 << 4)) >> 4); }
			set
			{
				_maskLogicBgs &= ~(0x03 << 4);
				_maskLogicBgs |= (int)value << 4;
			}
		}
		public WindowMaskLogic Bg4MaskLogic
		{
			get { return (WindowMaskLogic)((_maskLogicBgs & (0x03 << 6)) >> 6); }
			set
			{
				_maskLogicBgs &= ~(0x03 << 6);
				_maskLogicBgs |= (int)value << 6;
			}
		}
		public WindowMaskLogic ObjMaskLogic
		{
			get { return (WindowMaskLogic)(_maskLogicObjCol & (0x03 << 0)); }
			set
			{
				_maskLogicObjCol &= ~(0x03 << 0);
				_maskLogicObjCol |= (int)value << 0;
			}
		}
		public WindowMaskLogic ColorMaskLogic
		{
			get { return (WindowMaskLogic)((_maskLogicObjCol & (0x03 << 2)) >> 2); }
			set
			{
				_maskLogicObjCol &= ~(0x03 << 2);
				_maskLogicObjCol |= (int)value << 2;
			}
		}

		/// <summary>
		/// The window mask logic that will be applied for the BGs.
		/// <para>Value used on register $212A</para>
		/// <para>Format: 44332211</para>
		/// <para>00 = Or, 01 = And, 10 = Xor, 11 = XNor</para>
		/// </summary>
		public byte MaskLogicBgs
		{
			get { return (byte)_maskLogicBgs; }
			set { _maskLogicBgs = value; }
		}
		/// <summary>
		/// The window mask logic that will be applied for the color screen and objects.
		/// <para>Value used on register $212B</para>
		/// <para>Format: ----ccoo</para>
		/// <para>00 = Or, 01 = And, 10 = Xor, 11 = XNor</para>
		/// </summary>
		public byte MaskLogicObjCol
		{
			get { return (byte)_maskLogicObjCol; }
			set { _maskLogicObjCol = value; }
		}

		/// <summary>
		/// When flag for a layer is set the WindowingMask1 will be inverted before applying.
		/// <para>Controls various bits in $2123 - $2125</para>
		/// </summary>
		public WindowingLayers Window1Inverted { get; set; }
		/// <summary>
		/// When flag for a layer is set the WindowingMask2 will be inverted before applying.
		/// <para>Controls various bits in $2123 - $2125</para>
		/// </summary>
		public WindowingLayers Window2Inverted { get; set; }
		/// <summary>
		/// When flag for a layer is set the WindowingMask1 will be used on that layer.
		/// <para>Controls various bits in $2123 - $2125</para>
		/// </summary>
		public WindowingLayers Window1Enabled { get; set; }
		/// <summary>
		/// When flag for a layer is set the WindowingMask2 will be used on that layer.
		/// <para>Controls various bits in $2123 - $2125</para>
		/// </summary>
		public WindowingLayers Window2Enabled { get; set; }
		

		/// <summary>
		/// Which BGs should participate in color math and how.
		/// <para>Value used on register $2131, $40</para>
		/// <para>Format: shbo4321</para>
		/// <para>s = add 0 / substract 1</para>
		/// <para>h = half after calculation</para>
		/// <para>bo4321 = enable BG1/BG2/BG3/BG4/OBJ/Backdrop</para>
		/// </summary>
		public ColorMathMode ColorMathDesignation { get; set; }

		/// <summary>
		/// Sets pixels on the mainscreen to black before doing color math
		/// Controlled by bit 7-6 of $2130
		/// </summary>
		public ColorAdditionalSelectOptions ClipToBlack { get; set; }
		/// <summary>
		/// Doesn't run color math if set
		/// Controlled by bit 5-4 of $2130
		/// </summary>
		public ColorAdditionalSelectOptions PreventColorMath { get; set; }

		/// <summary>
		/// Whether the final screen will add the subscreen (false) or the fixed color (true).
		/// Controlled by bit 1 of $2130
		/// </summary>
		public bool AddColor { get; set; }
		
		/// <summary>
		/// Any flag in this will not be added in the GetScreen() neither for sub nor for main screen
		/// </summary>
		public static ScreenDesignation Hide { get; set; }

		/// <summary>
		/// 0x00493 - $00:8293
		/// The height of the status bar that will appear before any other layer and is not affected by color math.
		/// </summary>
		public static int StatusBarHeight = 0x24 + 1;

		public Size LayerSizes { get; set; }

		private int _maskLogicBgs = 0;
		private int _maskLogicObjCol = 0;

		/*
		 * =============================================================================================================
		 *                  Constructors 
		 * =============================================================================================================
		*/
		public ColorMath()
		{
			MainScreenDesignation = ScreenDesignation.BG1 | ScreenDesignation.OBJ;
			SubScreenDesignation = ScreenDesignation.BG2 | ScreenDesignation.BG4 | ScreenDesignation.BG3;
			ColorMathDesignation = ColorMathMode.Backdrop;

			LayerSizes = new Size(256, HDMA.Scanlines);

			BG1 = new Bitmap(LayerSizes.Width, LayerSizes.Height);
			BG2 = new Bitmap(LayerSizes.Width, LayerSizes.Height);
			BG3 = new Bitmap(LayerSizes.Width, LayerSizes.Height);
			BG4 =new Bitmap(LayerSizes.Width, LayerSizes.Height);
			OBJ = new Bitmap(LayerSizes.Width, LayerSizes.Height);

			FixedColor =  new Bitmap(LayerSizes.Width, LayerSizes.Height);

			Backdrop = BitmapEffects.FromColor(Color.Black, LayerSizes);

			//default: White = hide nothing
			WindowingMask1 = BitmapEffects.FromColor(Color.White, LayerSizes);
			WindowingMask2 = BitmapEffects.FromColor(Color.White, LayerSizes);
		}

		public ColorMath(Bitmap bg1, Bitmap bg2, Bitmap bg3, Bitmap bg4, Bitmap obj) : this()
		{
			this.BG1 = bg1;
			this.BG2 = bg2;
			this.BG3 = bg3;
			this.BG4 = bg4;
			this.OBJ = obj;
		}

		public ColorMath(BitmapCollection Collection) : this()
		{
			BG1 = Collection.BG1;
			BG2 = Collection.BG2;
			BG3 = Collection.BG3;
			BG4 = Collection.BG4;
			OBJ = Collection.OBJ;

			FixedColor = BitmapEffects.FromColor(Collection.FixedColor, LayerSizes);
		}

		/*
		 * =============================================================================================================
		 *                  Methods 
		 * =============================================================================================================
		*/

		/// <summary>
		/// Applies the color Math to a certain Bitmap.
		/// Depending on the mode the screens will be added or substracted and the result may be halfed
		/// </summary>
		/// <param name="Layer">The layer that will be color mathed (Is that a word?)</param>
		/// <param name="SubScreen">The subscreen that will be mathed to the layer</param>
		/// <param name="Mode">f</param>
		/// <returns>The final image</returns>
		public static Bitmap ApplyColorMath(Bitmap Layer, Bitmap SubScreen, ColorMathMode Mode, 
			Bitmap colorWindow, ColorAdditionalSelectOptions clipToBlack, ColorAdditionalSelectOptions noMath)
		{
			//make a new bitmap to return later (copy of top)
			Bitmap save = new Bitmap(Layer);

			//if top doesn't have the proper format, just return top (copy)
			if (save.PixelFormat != PixelFormat.Format32bppArgb)
				return save;

			//next few lines is locking bitmaps and getting their data.
			BitmapData layerData = save.LockBits(new Rectangle(0, 0, save.Width, save.Height), ImageLockMode.ReadWrite, save.PixelFormat);
			byte[] layerArray = new byte[layerData.Stride * save.Height];
			Marshal.Copy(layerData.Scan0, layerArray, 0, layerArray.Length);

			BitmapData subData = SubScreen.LockBits(new Rectangle(0, 0, SubScreen.Width, SubScreen.Height), ImageLockMode.ReadWrite, SubScreen.PixelFormat);
			byte[] subArray = new byte[subData.Stride * SubScreen.Height];
			Marshal.Copy(subData.Scan0, subArray, 0, subArray.Length);

			BitmapData windowData = colorWindow.LockBits(new Rectangle(0, 0, colorWindow.Width, colorWindow.Height), ImageLockMode.ReadWrite, colorWindow.PixelFormat);
			byte[] windowArray = new byte[windowData.Stride * colorWindow.Height];
			Marshal.Copy(windowData.Scan0, windowArray, 0, windowArray.Length);


			int subBPP = Bitmap.GetPixelFormatSize(SubScreen.PixelFormat) / 8;
			int windowBPP = Bitmap.GetPixelFormatSize(colorWindow.PixelFormat) / 8;

			if (subBPP != 4)
				throw new ArgumentException("Expected SubScreen image to use PixelFormat.Format32bppArgb");
			if (windowBPP != 4)
				throw new ArgumentException("Expected colorWindow image to use PixelFormat.Format32bppArgb");

			//run through all the bytes returned of the image. (format bgrA) start with 3 and increase by 4 to only check A
			for (int p = 3; p < layerArray.Length; p += 4)
			{
				//if the current pixel of the top screen has transparence, continue
				if (layerArray[p] < 128)
				{
					layerArray[p] = 0;  // if A is half or less, clip to min
					continue;           // and continue
				}

				//point to current pixel Blue in subscreen and color window
				int subIndex = p / 4 * subBPP;
				int windowIndex = p / 4 * windowBPP;

				switch(clipToBlack)
				{
					case ColorAdditionalSelectOptions.Never:
						break;
					case ColorAdditionalSelectOptions.Inside:
						if(windowArray[windowIndex] == 0)	//check blue = 0 => black
							goto case ColorAdditionalSelectOptions.Always;
						break;
					case ColorAdditionalSelectOptions.Outside:
						if(windowArray[windowIndex] == 255) //check blue = 255 => white
							goto case ColorAdditionalSelectOptions.Always;
						break;						
					case ColorAdditionalSelectOptions.Always:
						layerArray[p - 3] = 0;	//blue
						layerArray[p - 2] = 0;	//green
						layerArray[p - 1] = 0;	//red
						break;
				}

				//continue => skip color math and check next pixel
				switch(noMath)
				{
					case ColorAdditionalSelectOptions.Never:
						break;
					case ColorAdditionalSelectOptions.Inside:
						if(windowArray[windowIndex] == 0)	//check blue = 0 => black
							continue;
						break;
					case ColorAdditionalSelectOptions.Outside:
						if(windowArray[windowIndex] == 255) //check blue = 255 => white
							continue;
						break;						
					case ColorAdditionalSelectOptions.Always:
						continue;
				}


				if (subArray[subIndex + 3] < 128)	//check Alpha
					continue;

				int divider = (Mode.HasFlag(ColorMathMode.Half)) ? 2 : 1;
				int multiplier = (Mode.HasFlag(ColorMathMode.Substract)) ? -1 : 1;
				
				//p = index + 3. due to deviding multiplying, the bottom index is exact
				//thus substracting p and adding to bottomindex

				layerArray[p - 3] = (byte)((layerArray[p - 3] + (subArray[subIndex + 0] * multiplier)) / divider).Range(0, 255);
				layerArray[p - 2] = (byte)((layerArray[p - 2] + (subArray[subIndex + 1] * multiplier)) / divider).Range(0, 255);
				layerArray[p - 1] = (byte)((layerArray[p - 1] + (subArray[subIndex + 2] * multiplier)) / divider).Range(0, 255);
				layerArray[p - 0] = 255; 
			}
			
			Marshal.Copy(layerArray, 0, layerData.Scan0, layerArray.Length);    //copy changed data back
			save.UnlockBits(layerData);                                         //unlock bits and store data back
			SubScreen.UnlockBits(subData);
			colorWindow.UnlockBits(windowData);
			return save;                                                        //return the new Bitmap
		}


		private static int SumEnumFlag(Enum en)
		{
			return en.GetFlags().Sum(e => Convert.ToInt32(e));
		}

		public string Code()
		{
			ASMCodeBuilder sb = new ASMCodeBuilder();

			sb.AppendLabel(HDMA.INITLabel);

			if (MainScreenDesignation != MainScreenWindowMaskDesignation || true)
			{
				sb.OpenNewBlock();
				sb.AppendCode("LDA #$" + SumEnumFlag(MainScreenDesignation).ToASMString(), MainScreenDesignation.ToString() + " on main screen (TM)");
				sb.AppendCode("STA $" + Registers.MainScreenDesignation.ToASMString());
				sb.AppendCode("LDA #$" + SumEnumFlag(MainScreenWindowMaskDesignation).ToASMString(), MainScreenWindowMaskDesignation.ToString() + " on main screen should use windowing. (TMW)");
				sb.AppendCode("STA $" + Registers.MainScreenWindow.ToASMString());
				sb.CloseBlock();
			}
			/*else
			{
				sb.OpenNewBlock();
				sb.AppendCode("LDA #$" + SumEnumFlag(MainScreenDesignation).ToASMString(), MainScreenDesignation.ToString() + " on main screen (TM) and window (TMW)");
				sb.AppendCode("STA $" + RAM.MainScreenAndWindowDesignation[RAM.SA1].ToASMString(), "mirror of $212C and $212E");
				sb.CloseBlock();
			}*/
			if (SubScreenDesignation != SubScreenWindowMaskDesignation || true)
			{
				sb.OpenNewBlock();
				sb.AppendCode("LDA #$" + SumEnumFlag(SubScreenDesignation).ToASMString(), SubScreenDesignation.ToString() + " on sub screen (TS)");
				sb.AppendCode("STA $" + Registers.SubScreenDesignation.ToASMString());
				sb.AppendCode("LDA #$" + SumEnumFlag(SubScreenWindowMaskDesignation).ToASMString(), SubScreenWindowMaskDesignation.ToString() + " on sub screen should use windowing. (TSW)");
				sb.AppendCode("STA $" + Registers.SubScreenWindow.ToASMString());
				sb.CloseBlock();
			}
			/*else
			{
				sb.OpenNewBlock();
				sb.AppendCode("LDA #$" + SumEnumFlag(SubScreenDesignation).ToASMString(), SubScreenDesignation.ToString() + " on sub screen (TS) and window (TSW)");
				sb.AppendCode("STA $" + RAM.SubScreenAndWindowDesignation[RAM.SA1].ToASMString(), "mirror of $212D and $212F");
				sb.CloseBlock();
			}*/

			sb.AppendCode("LDA #$" + SumEnumFlag(ColorMathDesignation).ToASMString(), ColorMathDesignation.ToString() + " for color math");
			sb.AppendCode("STA $" + RAM.ColorMathSetting[RAM.SA1].ToASMString(), "mirror of $2131");
			sb.CloseBlock();

			if (!AddColor || ClipToBlack != 0 || PreventColorMath != 0)
			{
				sb.OpenNewBlock();
				int CGWSEL = (((int)ClipToBlack) << 6) +
					(((int)PreventColorMath) << 4) +
					(AddColor ? 0 : 2);

				sb.AppendCode("LDA #$" + CGWSEL.ToASMString(), "Clip to black: " + ClipToBlack + ", Prevent colot math: " + PreventColorMath);
				sb.AppendCode("STA $" + RAM.ColorAdditionSelect[RAM.SA1].ToASMString(), "Add subscreen instead of fixed color: " + !AddColor);
				sb.CloseBlock();
			}

			//mask logic
			if(MaskLogicBgs != 0)
			{
				sb.OpenNewBlock();
				sb.AppendCode("LDA #$" + MaskLogicBgs.ToString("X2"), "Mask logic for overlapping windowing on BGs");
				sb.AppendCode("STA $" + Registers.WindowingLogicBgs.ToASMString());
				sb.CloseBlock();
			}
			//mask logic
			if (MaskLogicObjCol != 0)
			{
				sb.OpenNewBlock();
				sb.AppendCode("LDA #$" + MaskLogicObjCol.ToString("X2"), "Mask logic for overlapping windowing on BGs");
				sb.AppendCode("STA $" + Registers.WindowingLogicObjColor.ToASMString());
				sb.CloseBlock();
			}

			int regW12SEL =
				(Window1Inverted.HasFlag(WindowingLayers.BG1) ? 0x01 : 0x00) +
				(Window1Enabled.HasFlag(WindowingLayers.BG1) ? 0x02 : 0x00) +
				(Window2Inverted.HasFlag(WindowingLayers.BG1) ? 0x04 : 0x00) +
				(Window2Enabled.HasFlag(WindowingLayers.BG1) ? 0x08 : 0x00) +

				(Window1Inverted.HasFlag(WindowingLayers.BG2) ? 0x10 : 0x00) +
				(Window1Enabled.HasFlag(WindowingLayers.BG2) ? 0x20 : 0x00) +
				(Window2Inverted.HasFlag(WindowingLayers.BG2) ? 0x40 : 0x00) +
				(Window2Enabled.HasFlag(WindowingLayers.BG2) ? 0x80 : 0x00);

			int regW34SEL =
				(Window1Inverted.HasFlag(WindowingLayers.BG3) ? 0x01 : 0x00) +
				(Window1Enabled.HasFlag(WindowingLayers.BG3) ? 0x02 : 0x00) +
				(Window2Inverted.HasFlag(WindowingLayers.BG3) ? 0x04 : 0x00) +
				(Window2Enabled.HasFlag(WindowingLayers.BG3) ? 0x08 : 0x00) +

				(Window1Inverted.HasFlag(WindowingLayers.BG4) ? 0x10 : 0x00) +
				(Window1Enabled.HasFlag(WindowingLayers.BG4) ? 0x20 : 0x00) +
				(Window2Inverted.HasFlag(WindowingLayers.BG4) ? 0x40 : 0x00) +
				(Window2Enabled.HasFlag(WindowingLayers.BG4) ? 0x80 : 0x00);

			int regWOBJSEL =
				(Window1Inverted.HasFlag(WindowingLayers.OBJ) ? 0x01 : 0x00) +
				(Window1Enabled.HasFlag(WindowingLayers.OBJ) ? 0x02 : 0x00) +
				(Window2Inverted.HasFlag(WindowingLayers.OBJ) ? 0x04 : 0x00) +
				(Window2Enabled.HasFlag(WindowingLayers.OBJ) ? 0x08 : 0x00) +

				(Window1Inverted.HasFlag(WindowingLayers.Color) ? 0x10 : 0x00) +
				(Window1Enabled.HasFlag(WindowingLayers.Color) ? 0x20 : 0x00) +
				(Window2Inverted.HasFlag(WindowingLayers.Color) ? 0x40 : 0x00) +
				(Window2Enabled.HasFlag(WindowingLayers.Color) ? 0x80 : 0x00);



			sb.OpenNewBlock();
			if (regW12SEL != 0)
			{
				sb.AppendCode("LDA #$" + regW12SEL.ToASMString(), "values for enabling/inverting BG1/BG2 on window 1/2");
				sb.AppendCode("STA $" + RAM.WindowMaskSettingBG1BG2[RAM.SA1].ToASMString(), "mirror of $2123");
			}
			if (regW34SEL != 0)
			{
				if (regW34SEL != regW12SEL) //prevent LDA if A still has valid value from before
					sb.AppendCode("LDA #$" + regW34SEL.ToASMString(), "values for enabling/inverting BG3/BG4 on window 1/2");
				sb.AppendCode("STA $" + RAM.WindowMaskSettingBG3BG4[RAM.SA1].ToASMString(), "mirror of $2124");
			}
			if (regWOBJSEL != 0)
			{
				if(regWOBJSEL != regW34SEL)	//prevent LDA if A still has valid value from before
					sb.AppendCode("LDA #$" + regWOBJSEL.ToASMString(), "values for enabling/inverting OBJ/Color on window 1/2");
				sb.AppendCode("STA $" + RAM.WindowMaskSettingOBJColor[RAM.SA1].ToASMString(), "mirror of $2125");
			}
			if (regW12SEL != 0 || regW34SEL != 0 || regWOBJSEL != 0)
			{
				sb.AppendComment("Window 1 enabled on " + Window1Enabled.ToString());
				sb.AppendComment("Window 2 enabled on " + Window2Enabled.ToString());
				sb.AppendComment("Window 1 inverted on " + Window1Inverted.ToString());
				sb.AppendComment("Window 2 inverted on " + Window2Inverted.ToString());
			}
			sb.CloseBlock();


			return sb.ToString();
		}

		/// <summary>
		/// Sets the FixedColor property based on a desired color.
		/// </summary>
		/// <param name="color">The color for the Bitmap</param>
		public void SetFixedColor(Color color)
		{
			FixedColor = BitmapEffects.FromColor(color, LayerSizes);
		}

		/// <summary>
		/// Sets the FixedColor property based on a desired color.
		/// </summary>
		/// <param name="color">The color for the Bitmap</param>
		public void SetBackdrop(Color color)
		{
			Backdrop = BitmapEffects.FromColor(color, LayerSizes);
		}
		
		/// <summary>
		/// Gets the screen as it would be displayed by the SNES.
		/// </summary>
		/// <returns></returns>
		public Bitmap GetScreen()
		{
			if (BG1 == null || BG2 == null || BG3 == null || BG4 == null)
				throw new ArgumentNullException("BGs need to be initialized");
			if (FixedColor == null)
				throw new ArgumentNullException("FixedColor need to be initialized");
			if (Backdrop == null)
				throw new ArgumentNullException("Backdrop need to be initialized");


			Bitmap main, sub;
			Bitmap statusBar = new Bitmap(LayerSizes.Width, LayerSizes.Height);
			Bitmap bg3_remain = new Bitmap(LayerSizes.Width, LayerSizes.Height);

			//cut layer 3 into status bar end rest.
			using (Graphics g = Graphics.FromImage(statusBar))
				g.DrawImageUnscaledAndClipped(BG3, new Rectangle(0, 0, statusBar.Width, StatusBarHeight));
			using (Graphics g = Graphics.FromImage(bg3_remain))
			{
				Rectangle rec = new Rectangle(0, StatusBarHeight, bg3_remain.Width, bg3_remain.Height - StatusBarHeight);
				g.DrawImageUnscaledAndClipped(BG3.Clone(rec, BG3.PixelFormat), rec);
			}


			Func<bool, bool, bool, bool, WindowMaskLogic, Bitmap, Bitmap> doWindowing = (enable1, enable2, invert1, invert2, logic, bg) =>
			{
				Bitmap mask1 = invert1 ? BitmapEffects.Invert(WindowingMask1) : WindowingMask1;
				Bitmap mask2 = invert2 ? BitmapEffects.Invert(WindowingMask2) : WindowingMask2;

				if (enable1 && enable2)
					return BitmapEffects.ApplyMask(bg, MergeMasks(mask1, mask2, logic), 0.5f);
				else if (enable1)
					return BitmapEffects.ApplyMask(bg, mask1, 0.5f);
				else if (enable2)
					return BitmapEffects.ApplyMask(bg, mask2, 0.5f);
				else
					return bg;
			};

			Bitmap bg1Windowed = doWindowing(Window1Enabled.HasFlag(WindowingLayers.BG1), Window2Enabled.HasFlag(WindowingLayers.BG1),
				Window1Inverted.HasFlag(WindowingLayers.BG1), Window2Inverted.HasFlag(WindowingLayers.BG1), Bg1MaskLogic, BG1);

			Bitmap bg2Windowed = doWindowing(Window1Enabled.HasFlag(WindowingLayers.BG2), Window2Enabled.HasFlag(WindowingLayers.BG2),
				Window1Inverted.HasFlag(WindowingLayers.BG2), Window2Inverted.HasFlag(WindowingLayers.BG2), Bg2MaskLogic, BG2);

			Bitmap statusBarWindowed = doWindowing(Window1Enabled.HasFlag(WindowingLayers.BG3), Window2Enabled.HasFlag(WindowingLayers.BG3),
				Window1Inverted.HasFlag(WindowingLayers.BG3), Window2Inverted.HasFlag(WindowingLayers.BG3), Bg3MaskLogic, statusBar);

			Bitmap bg3RemainWindowed = doWindowing(Window1Enabled.HasFlag(WindowingLayers.BG3), Window2Enabled.HasFlag(WindowingLayers.BG3),
				Window1Inverted.HasFlag(WindowingLayers.BG3), Window2Inverted.HasFlag(WindowingLayers.BG3), Bg3MaskLogic, bg3_remain);

			Bitmap bg4Windowed = doWindowing(Window1Enabled.HasFlag(WindowingLayers.BG4), Window2Enabled.HasFlag(WindowingLayers.BG4),
				Window1Inverted.HasFlag(WindowingLayers.BG4), Window2Inverted.HasFlag(WindowingLayers.BG4), Bg4MaskLogic, BG4);

			Bitmap objWindowed = doWindowing(Window1Enabled.HasFlag(WindowingLayers.OBJ), Window2Enabled.HasFlag(WindowingLayers.OBJ),
				Window1Inverted.HasFlag(WindowingLayers.OBJ), Window2Inverted.HasFlag(WindowingLayers.OBJ), ObjMaskLogic, OBJ);

			Bitmap colorWindowed = BitmapEffects.FromColor(Color.Black, LayerSizes);
			Bitmap colorWindowedHoles = doWindowing(Window1Enabled.HasFlag(WindowingLayers.Color), Window2Enabled.HasFlag(WindowingLayers.Color),
				Window1Inverted.HasFlag(WindowingLayers.Color), Window2Inverted.HasFlag(WindowingLayers.Color), ColorMaskLogic,
				BitmapEffects.FromColor(Color.White, LayerSizes));

			//colorWindowsedHoles is white with holes where the black should be
			using (Graphics g = Graphics.FromImage(colorWindowed))
				g.DrawImage(colorWindowedHoles, 0, 0);

			#region Sub Screen
			List<Bitmap> BGs = new List<Bitmap>();
			if (SubScreenDesignation.HasFlag(ScreenDesignation.OBJ) && !Hide.HasFlag(ScreenDesignation.OBJ))
				BGs.Add(SubScreenWindowMaskDesignation.HasFlag(ScreenDesignation.OBJ) ? objWindowed : OBJ);

			if (SubScreenDesignation.HasFlag(ScreenDesignation.BG1) && !Hide.HasFlag(ScreenDesignation.BG1))
				BGs.Add(SubScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG1) ? bg1Windowed : BG1);

			if (SubScreenDesignation.HasFlag(ScreenDesignation.BG2) && !Hide.HasFlag(ScreenDesignation.BG2))
				BGs.Add(SubScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG2) ? bg2Windowed : BG2);

			//layer 3 special (status bar split) onyl lower part
			if (SubScreenDesignation.HasFlag(ScreenDesignation.BG3) && !Hide.HasFlag(ScreenDesignation.BG3))
				BGs.Add(SubScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG3) ? bg3RemainWindowed : bg3_remain);

			if (SubScreenDesignation.HasFlag(ScreenDesignation.BG4) && !Hide.HasFlag(ScreenDesignation.BG4))
				BGs.Add(SubScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG4) ? bg4Windowed : BG4);

			BGs.Add(FixedColor);
			sub = BitmapEffects.OverlapImages(BGs.ToArray());
			#endregion

			#region Main Screen

			BGs.Clear();
			if (MainScreenDesignation.HasFlag(ScreenDesignation.OBJ) && !Hide.HasFlag(ScreenDesignation.OBJ))
			{
				Bitmap objUse = MainScreenWindowMaskDesignation.HasFlag(ScreenDesignation.OBJ) ? objWindowed : OBJ;
				if (ColorMathDesignation.HasFlag(ColorMathMode.OBJ))
					BGs.Add(ApplyColorMath(objUse, AddColor ? FixedColor : sub, ColorMathDesignation, colorWindowed, ClipToBlack, PreventColorMath));
				else
					BGs.Add(objUse);
			}
			if (MainScreenDesignation.HasFlag(ScreenDesignation.BG1) && !Hide.HasFlag(ScreenDesignation.BG1))
			{
				Bitmap bg1Use = MainScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG1) ? bg1Windowed : BG1;
				if (ColorMathDesignation.HasFlag(ColorMathMode.BG1))
					BGs.Add(ApplyColorMath(bg1Use, AddColor ? FixedColor : sub, ColorMathDesignation, colorWindowed, ClipToBlack, PreventColorMath));
				else
					BGs.Add(bg1Use);
			}
			if (MainScreenDesignation.HasFlag(ScreenDesignation.BG2) && !Hide.HasFlag(ScreenDesignation.BG2))
			{
				Bitmap bg2Use = MainScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG2) ? bg2Windowed : BG2;
				if (ColorMathDesignation.HasFlag(ColorMathMode.BG2))
					BGs.Add(ApplyColorMath(bg2Use, AddColor ? FixedColor : sub, ColorMathDesignation, colorWindowed, ClipToBlack, PreventColorMath));
				else
					BGs.Add(bg2Use);
			}
			if (MainScreenDesignation.HasFlag(ScreenDesignation.BG3) && !Hide.HasFlag(ScreenDesignation.BG3))
			{
				//layer 3 special (status bar split) onyl lower half
				Bitmap bg3Use = MainScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG3) ? bg3RemainWindowed : bg3_remain;
				if (ColorMathDesignation.HasFlag(ColorMathMode.BG3))
					BGs.Add(ApplyColorMath(bg3Use, AddColor ? FixedColor : sub, ColorMathDesignation, colorWindowed, ClipToBlack, PreventColorMath));
				else
					BGs.Add(bg3Use);
			}
			if (MainScreenDesignation.HasFlag(ScreenDesignation.BG4) && !Hide.HasFlag(ScreenDesignation.BG4))
			{
				Bitmap bg4Use = MainScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG3) ? bg4Windowed : BG4;
				if (ColorMathDesignation.HasFlag(ColorMathMode.BG4))
					BGs.Add(ApplyColorMath(bg4Use, AddColor ? FixedColor : sub, ColorMathDesignation, colorWindowed, ClipToBlack, PreventColorMath));
				else
					BGs.Add(bg4Use);
			}

			//handle the backdrop too
			if (ColorMathDesignation.HasFlag(ColorMathMode.Backdrop))
				BGs.Add(ApplyColorMath(Backdrop, AddColor ? FixedColor : sub, ColorMathDesignation, colorWindowed, ClipToBlack, PreventColorMath));
			else
				BGs.Add(Backdrop);

			main = BitmapEffects.OverlapImages(BGs.ToArray());
			#endregion

			BGs.Clear();

			//check windowing for status bar
			if (MainScreenDesignation.HasFlag(ScreenDesignation.BG3) && !Hide.HasFlag(ScreenDesignation.BG3))
				BGs.Add(MainScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG3) ? statusBarWindowed : statusBar);
			//if not main, check subscreen. (still put it before main for display purpose)
			else if (SubScreenDesignation.HasFlag(ScreenDesignation.BG3) && !Hide.HasFlag(ScreenDesignation.BG3))
				BGs.Add(SubScreenWindowMaskDesignation.HasFlag(ScreenDesignation.BG3) ? statusBarWindowed : statusBar);

			BGs.Add(main);
			BGs.Add(sub);

			return BitmapEffects.OverlapImages(BGs.ToArray());
		}

		/// <summary>
		/// Combines two black/white masks in a new bitmap
		/// </summary>
		/// <param name="mask1">The first mask for the merge</param>
		/// <param name="mask2">The second mask for the merge</param>
		/// <param name="logic">How to merge the masks</param>
		/// <returns>The merged masks</returns>
		private Bitmap MergeMasks(Bitmap mask1, Bitmap mask2, WindowMaskLogic logic)
		{
			Bitmap[] windowLogicMasks = new Bitmap[4];
			Rectangle maskRec = new Rectangle(new Point(0, 0), mask2.Size);
			//needs to run if there is any OR, XOR, XNOR => baiscally if there is any that is not And
			if (logic != WindowMaskLogic.And)
			{
				Bitmap mask = new Bitmap(mask1);
				ImageAttributes attr = new ImageAttributes();
				attr.SetColorKey(Color.White, Color.White);

				using (Graphics g = Graphics.FromImage(mask))
				{
					g.DrawImage(mask2, maskRec, maskRec.X, maskRec.Y,
						maskRec.Width, maskRec.Height, GraphicsUnit.Pixel, attr);
				}
				windowLogicMasks[(int)WindowMaskLogic.Or] = mask;
			}
			//needs to run if there is any AND, XOR, XNOR => baiscally if there is any that is not Or
			if (logic != WindowMaskLogic.Or)
			{
				Bitmap mask = new Bitmap(mask1);
				ImageAttributes attr = new ImageAttributes();
				attr.SetColorKey(Color.Black, Color.Black);

				using (Graphics g = Graphics.FromImage(mask))
				{
					g.DrawImage(mask2, maskRec, maskRec.X, maskRec.Y,
						maskRec.Width, maskRec.Height, GraphicsUnit.Pixel, attr);
				}
				windowLogicMasks[(int)WindowMaskLogic.And] = mask;
			}
			if (logic == WindowMaskLogic.Xor || logic == WindowMaskLogic.XNor)
			{
				Bitmap mask = new Bitmap(windowLogicMasks[(int)WindowMaskLogic.Or]);
				ImageAttributes attr = new ImageAttributes();
				attr.SetColorKey(Color.Black, Color.Black);
				
				using (Graphics g = Graphics.FromImage(mask))
				{
					g.DrawImage(BitmapEffects.Invert(windowLogicMasks[(int)WindowMaskLogic.And]),
						maskRec, maskRec.X, maskRec.Y, maskRec.Width, maskRec.Height, GraphicsUnit.Pixel, attr);
				}

				mask.Save("XOR.png");
				windowLogicMasks[(int)WindowMaskLogic.Xor] = mask;
				windowLogicMasks[(int)WindowMaskLogic.XNor] = BitmapEffects.Invert(mask);
			}

			return windowLogicMasks[(int)logic];
		}
	}
}
