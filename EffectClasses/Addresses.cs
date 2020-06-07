using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EffectClasses
{
	/// <summary>
	/// Contains various registers the SNES can use for DMA/HDMA
	/// </summary>
	public static class Registers
	{
		/// <summary>
		/// SNES Brigtness register.
		/// <para>Format: ----bbbb</para>
		/// <para>0 = Total darkness, F = full brightness</para>
		/// </summary>
		public static int Brightness { get { return 0x2100; } }

		/// <summary>
		/// Screen Pixelation
		/// <para>xxxxDCBA</para>
		/// <para>A/B/C/D = Affect BG1/BG2/BG3/BG4</para>
		/// <para>xxxx = pixel size, 0=1x1, F=16x16</para>
		/// </summary>
		public static int Pixelation { get { return 0x2106; } }

		/// <summary>
		/// ------xx xxxxxxxx
		/// x = The BG offset, 10 bits. Only durring mode 0-6
		/// </summary>
		public static int BG1Horizontal { get { return 0x210D; } }
		/// <summary>
		/// ------xx xxxxxxxx
		/// x = The BG offset, 10 bits. Only durring mode 0-6
		/// </summary>
		public static int BG1Vertical { get { return 0x210E; } }
		/// <summary>
		/// ------xx xxxxxxxx
		/// x = The BG offset, 10 bits. Only durring mode 0-6
		/// </summary>
		public static int BG2Horizontal { get { return 0x210F; } }
		/// <summary>
		/// ------xx xxxxxxxx
		/// x = The BG offset, 10 bits. Only durring mode 0-6
		/// </summary>
		public static int BG2Vertical { get { return 0x2110; } }
		/// <summary>
		/// ------xx xxxxxxxx
		/// x = The BG offset, 10 bits. Only durring mode 0-6
		/// </summary>
		public static int BG3Horizontal { get { return 0x2111; } }
		/// <summary>
		/// ------xx xxxxxxxx
		/// x = The BG offset, 10 bits. Only durring mode 0-6
		/// </summary>
		public static int BG3Vertical { get { return 0x2112; } }
		/// <summary>
		/// ------xx xxxxxxxx
		/// x = The BG offset, 10 bits. Only durring mode 0-6
		/// </summary>
		public static int BG4Horizontal { get { return 0x2113; } }
		/// <summary>
		/// ------xx xxxxxxxx
		/// x = The BG offset, 10 bits. Only durring mode 0-6
		/// </summary>
		public static int BG4Vertical { get { return 0x2114; } }
		

		/// <summary>
		/// Handles inverting and enabling for BG1 and BG2 on windows 1 and 2
		/// </summary>
		public static int WindowMaskSettingBG1BG2 { get { return 0x2123; } }
		/// <summary>
		/// Handles inverting and enabling for BG3 and BG4 on windows 1 and 2
		/// </summary>
		public static int WindowMaskSettingBG3BG4 { get { return 0x2124; } }
		/// <summary>
		/// Handles inverting and enabling for OBJ and Color on windows 1 and 2
		/// </summary>
		public static int WindowMaskSettingOBJColor { get { return 0x2125; } }


		/// <summary>
		/// The left position of the 1st window to mask the screen
		/// </summary>
		public static int WindowMask1Left { get { return 0x2126; } }
		/// <summary>
		/// The right position of the 1st window to mask the screen
		/// </summary>
		public static int WindowMask1Right { get { return 0x2127; } }
		/// <summary>
		/// The left position of the 2nd window to mask the screen
		/// </summary>
		public static int WindowMask2Left { get { return 0x2128; } }
		/// <summary>
		/// The right position of the 2nd window to mask the screen
		/// </summary>
		public static int WindowMask2Right { get { return 0x2129; } }

		/// <summary>
		/// 44332211
		/// Logic for overlapping windows on BGs
		/// </summary>
		public static int WindowingLogicBgs { get { return 0x212A; } }
		/// <summary>
		/// ----ccoo
		/// Logic for overlapping windows on color and objects
		/// </summary>
		public static int WindowingLogicObjColor { get { return 0x212B; } }

		/// <summary>
		/// ---o4321
		/// Enable BGx or OBJ on the mainscreen
		/// </summary>
		public static int MainScreenDesignation { get { return 0x212C; } }
		/// <summary>
		/// ---o4321
		/// Enable BGx or OBJ on the subscreen
		/// </summary>
		public static int SubScreenDesignation { get { return 0x212D; } }
		/// <summary>
		/// ---o4321
		/// Enable window on mainscreen for BGx/OBJ
		/// </summary>
		public static int MainScreenWindow { get { return 0x212E; } }
		/// <summary>
		/// ---o4321
		/// Enable window on subscreen for BGx/OBJ
		/// </summary>
		public static int SubScreenWindow { get { return 0x212F; } }
		/// <summary>
		/// bgrccccc
		/// b/g/r = Which color plane(s) to set the intensity for.
		/// ccccc = Color intensity.
		/// </summary>
		public static int FixedColor { get { return 0x2132; } }

		/// <summary>
		/// DMA Control for Channel 0. Add 16 per channel
		/// <para>da-ifttt</para>
		/// <para>d = Transfer Direction. Clear: <![CDATA[CPU > PPU. Set: CPU < PPU]]></para>
		/// <para>a = HDMA Addressing Mode. Set: Indirect HDMA</para>
		/// <para>i = DMA Address Increment. Clear: increment, Set: decrement</para>
		/// <para>f = DMA Fixed Transfer. Set: Address not adjusted</para>
		/// <para>ttt = Transfer Mode. p | p,p+1 | p,p | p,p,p+1,p+1 | p,p+1,p+2,p+3 | p,p+1,p,p+1</para>
		/// </summary>
		public static int DMAMode { get { return 0x4300; } }
		/// <summary>
		/// DMA Destination Register Channel 0. Add 16 per channel.
		/// pppppppp 
		/// </summary>
		public static int DMATarget { get { return 0x4301; } }
		/// <summary>
		/// DMA Source Address for Channel 0 low byte. Add 16 per channel.
		/// </summary>
		public static int DMALowByte { get { return 0x4302; } }
		/// <summary>
		/// DMA Source Address for Channel 0 high byte. Add 16 per channel
		/// </summary>
		public static int DMAHighByte { get { return 0x4303; } }
		/// <summary>
		/// DMA Source Address for Channel 0 bank byte. Add 16 per channel
		/// </summary>
		public static int DMABankByte { get { return 0x4304; } }
		/// <summary>
		/// DMA Size/HDMA Indirect Address low byte channel 0. Add 16 per channel.
		/// </summary>
		public static int DMASizeLow { get { return 0x4305; } }
		/// <summary>
		/// DMA Size/HDMA Indirect Address high byte channel 0. Add 16 per channel.
		/// </summary>
		public static int DMASizeHigh { get { return 0x4306; } }
		/// <summary>
		/// DMA Size/HDMA Indirect Address low byte channel 0. Add 16 per channel.
		/// </summary>
		public static int HDMAIndirectBank { get { return 0x4305; } }
	}

	/// <summary>
	/// Contains various Dictionaries with a boolean key.
	/// <c>True</c> makes it return the SA-1 address, while <c>false</c> returns the normal one.
	/// </summary>
	public static class RAM
	{
		/// <summary>
		/// Use SA-1
		/// </summary>
		public static bool SA1;

		/// <summary>
		/// SMW Frame Counter
		/// </summary>
		public static Dictionary<bool, int> FrameCounter
		{ get { return new Dictionary<bool, int>() { { false, 0x13 }, { true, 0x13 } }; } }

		/// <summary>
		/// CGADSUB settings. 
		/// <para>Format: shbo4321</para>
		/// <para>s = 0 for adding , 1 for subtracting color layer</para>
		/// <para>h = half-color enable</para>
		/// <para>b = backdrop enable</para>
		/// <para>o = object (aka sprite) enable</para>
		/// <para>4321 = enable Layer 4, 3, 2, 1 (Layer 3 is only affected below the status bar)</para>
		/// <para>Mirror of SNES register $2131</para>
		/// </summary>
		public static Dictionary<bool, int> ColorMathSetting
		{ get { return new Dictionary<bool, int>() { { false, 0x40 }, { true, 0x40 } }; } }
		
		/// <summary>
		/// W12SEL
		/// <para>Format: ABCDabcd</para>
		/// <para>a = enable window 2 for BG1</para>
		/// <para>b = invert window 2 for BG1</para>
		/// <para>c = enable window 1 for BG1</para>
		/// <para>d = invert window 1 for BG1</para>
		/// <para>capital letters for BG2</para>
		/// <para>Mirror of SNES register $2123</para>
		/// </summary>
		public static Dictionary<bool, int> WindowMaskSettingBG1BG2
		{ get { return new Dictionary<bool, int>() { { false, 0x41 }, { true, 0x41 } }; } }

		/// <summary>
		/// W34SEL
		/// <para>Format: ABCDabcd</para>
		/// <para>a = enable window 2 for BG3</para>
		/// <para>b = invert window 2 for BG3</para>
		/// <para>c = enable window 1 for BG3</para>
		/// <para>d = invert window 1 for BG3</para>
		/// <para>capital letters for BG4</para>
		/// <para>Mirror of SNES register $2124</para>
		/// </summary>
		public static Dictionary<bool, int> WindowMaskSettingBG3BG4
		{ get { return new Dictionary<bool, int>() { { false, 0x42 }, { true, 0x42 } }; } }

		/// <summary>
		/// WOBJSEL
		/// <para>Format: ABCDabcd</para>
		/// <para>a = enable window 2 for OBJ</para>
		/// <para>b = invert window 2 for OBJ</para>
		/// <para>c = enable window 1 for OBJ</para>
		/// <para>d = invert window 1 for OBJ</para>
		/// <para>capital letters for color</para>
		/// <para>Mirror of SNES register $2125</para>
		/// </summary>
		public static Dictionary<bool, int> WindowMaskSettingOBJColor
		{ get { return new Dictionary<bool, int>() { { false, 0x43 }, { true, 0x43 } }; } }

		/// <summary>
		/// $44: CGWSEL settings. 
		/// <para>Format: ccmm--sd</para>
		/// <para>cc = Clip to black before math. 00-Never, 01-Outside, 10-Inside, 11-Always</para>
		/// <para>mm = Prevent color math. 00-Never, 01-Outside, 10-Inside, 11-Always</para>
		/// <para>s = Add subscreen instead of fixed color</para>
		/// <para>d = Direct color mode for 256-color BGs</para>
		/// <para>Mirror of SNES register $2130</para>
		/// </summary>
		public static Dictionary<bool, int> ColorAdditionSelect
		{ get { return new Dictionary<bool, int>() { { false, 0x44 }, { true, 0x44 } }; } }
		
		/// <summary>
		/// TM/TMW: 
		/// <para>format: ---o2134</para>
		/// <para>o = Object layer</para>
		/// <para>1/2/3/4 = Layer 1/2/3/4</para>
		/// <para>Mirror of SNES register $212C and $212E</para>
		/// </summary>
		public static Dictionary<bool, int> MainScreenAndWindowDesignation
		{ get { return new Dictionary<bool, int>() { { false, 0x0D9D }, { true, 0x6D9D } }; } }

		/// <summary>
		/// TS/TSW: 
		/// <para>format: ---o2134</para>
		/// <para>o = Object layer</para>
		/// <para>1/2/3/4 = Layer 1/2/3/4</para>
		/// <para>Mirror of SNES register $212D and $212F</para>
		/// </summary>
		public static Dictionary<bool, int> SubScreenAndWindowDesignation
		{ get { return new Dictionary<bool, int>() { { false, 0x0D9E }, { true, 0x6D9E } }; } }

		/// <summary>
		/// HDMA Channel Enable: 
		/// <para>abcdefgh</para>
		/// <para>a = Channel 7 .. h = Channel 0</para>
		/// <para>1 = Enable 0 = Disable.</para>
		/// <para>Mirror of SNES register $420C.</para>
		/// </summary>
		public static Dictionary<bool, int> HDMAEnable
		{ get { return new Dictionary<bool, int>() { { false, 0x0D9F }, { true, 0x6D9F } }; } }

		/// <summary>
		/// Layer 1 X position. 2 Bytes
		/// <para>Mirror of $210D</para>
		/// <para>Used for temporary storage to determine how much the screen has moved horizontally in the current frame.</para>
		/// </summary>
		public static Dictionary<bool, int> Layer1X
		{ get { return new Dictionary<bool, int>() { { false, 0x1A }, { true, 0x1A } }; } }
		
		/// <summary>
		/// Layer 1 Y position. 2 Bytes
		/// <para>Mirror of $210E</para>
		/// <para>Used for temporary storage to determine how much the screen has moved vertically in the current frame.</para>
		/// </summary>
		public static Dictionary<bool, int> Layer1Y
		{ get { return new Dictionary<bool, int>() { { false, 0x1C }, { true, 0x1C } }; } }

		/// <summary>
		/// Layer 2 X position. 2 Bytes
		/// <para>Mirror of $210F</para>
		/// <para>Used for temporary storage to determine how much the screen has moved horizontally in the current frame.</para>
		/// </summary>
		public static Dictionary<bool, int> Layer2X
		{ get { return new Dictionary<bool, int>() { { false, 0x1E }, { true, 0x1E } }; } }

		/// <summary>
		/// Layer 2 Y position. 2 Bytes
		/// <para>Mirror of $2110</para>
		/// <para>Used for temporary storage to determine how much the screen has moved vertically in the current frame.</para>
		/// </summary>
		public static Dictionary<bool, int> Layer2Y
		{ get { return new Dictionary<bool, int>() { { false, 0x20 }, { true, 0x20 } }; } }

		/// <summary>
		/// Layer 3 X position. 2 Bytes
		/// <para>Mirror of $2111</para>
		/// <para>Used for temporary storage to determine how much the screen has moved horizontally in the current frame.</para>
		/// </summary>
		public static Dictionary<bool, int> Layer3X
		{ get { return new Dictionary<bool, int>() { { false, 0x22 }, { true, 0x22 } }; } }

		/// <summary>
		/// Layer 3 Y position. 2 Bytes
		/// <para>Mirror of $2112</para>
		/// <para>Used for temporary storage to determine how much the screen has moved horizontally in the current frame.</para>
		/// </summary>
		public static Dictionary<bool, int> Layer3Y
		{ get { return new Dictionary<bool, int>() { { false, 0x24 }, { true, 0x24 } }; } }
	}
}
