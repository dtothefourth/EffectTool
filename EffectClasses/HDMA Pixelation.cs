using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EffectClasses
{
	[Flags]
	public enum PixelationBGs : byte { BG1 = 1, BG2 = 2, BG3 = 4, BG4 = 8 }

	
	public class PixelScanline
	{
		public byte Scanline { get; set; }
		public PixelationBGs BGs { get; set; }
		public byte Pixelation { get; set; }

		public PixelScanline(byte Scanline, byte Pixelation, PixelationBGs BGs)
		{
			this.Scanline = Scanline;
			this.Pixelation = Pixelation;
			this.BGs = BGs;
		}
	}

	public class PixelationHDMA : HDMA
	{
		public ColorMath ColorMath { get; set; }

		/// <summary>
		/// <c>True</c> if the code requires MAIN code from uberASM
		/// </summary>
		public override bool UsesMain
		{
			get { return false; }
		}        
		/// <summary>
		/// The combined image of all the layers and the pixelated
		/// </summary>
		public Bitmap EffectImage 
		{
			get
			{
				Setup();
				return ColorMath.GetScreen();
			}
		}
		/// <summary>
		/// The collection of the original images for the layers
		/// </summary>
		public BitmapCollection OriginalImages { get; set; }
		/// <summary>
		/// The values for the pixelation for the layers
		/// </summary>
		public List<PixelScanline> Values { get; set; }

		public PixelationHDMA()
		{
			Values = new List<PixelScanline>();
			Table = new HDMATable(".PixelTable");
			ColorMath = new ColorMath();
			OriginalImages = new BitmapCollection();
		}

		/// <summary>
		/// Sets up the color math and calculates the values for the HDMA table based on the entries of Values
		/// </summary>
		private void Setup()
		{
			Table.Clear();
			byte totalScanlines = 0;

			Bitmap bg1 = OriginalImages.BG1;
			Bitmap bg2 = OriginalImages.BG2;
			Bitmap bg3 = OriginalImages.BG3;
			Bitmap bg4 = OriginalImages.BG4;
			
			foreach (PixelScanline pix in Values)
			{
				if (pix.Scanline > 0x80)
				{
					Table.Add(new HDMATableEntry(TableValueType.db, 0x80, (byte)((pix.Pixelation << 4) + (byte)pix.BGs)));
					Table.Add(new HDMATableEntry(TableValueType.db, (byte)(pix.Scanline - 0x80), (byte)((pix.Pixelation << 4) + (byte)pix.BGs)));
				}
				else
				{
					Table.Add(new HDMATableEntry(TableValueType.db, pix.Scanline, (byte)((pix.Pixelation << 4) + (byte)pix.BGs)));
				}

				if (pix.BGs.HasFlag(PixelationBGs.BG1))
					bg1 = BitmapEffects.PixelLine(pix.Scanline, pix.Pixelation, totalScanlines, bg1);
				if (pix.BGs.HasFlag(PixelationBGs.BG2))
					bg2 = BitmapEffects.PixelLine(pix.Scanline, pix.Pixelation, totalScanlines, bg2);
				if (pix.BGs.HasFlag(PixelationBGs.BG3))
					bg3 = BitmapEffects.PixelLine(pix.Scanline, pix.Pixelation, totalScanlines, bg3);
				if (pix.BGs.HasFlag(PixelationBGs.BG4))
					bg4 = BitmapEffects.PixelLine(pix.Scanline, pix.Pixelation, totalScanlines, bg4);

				totalScanlines += pix.Scanline;
				if (totalScanlines >= Scanlines)
					break;
			}

			if (totalScanlines < Scanlines)
				Table.Add(new HDMATableEntry(TableValueType.db, 1, 0));

			ColorMath.BG1 = bg1;
			ColorMath.BG2 = bg2;
			ColorMath.BG3 = bg3;
			ColorMath.BG4 = bg4;
			ColorMath.OBJ = OriginalImages.OBJ;
			Table.Add(HDMATableEntry.End);
		}

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

		public override string Code(int channel, HDMATable table, bool SA1)
		{
			Setup();
			ASMCodeBuilder CodeBuilder = new ASMCodeBuilder();
			if (Values.Count == 1 && Values[0].Scanline == Scanlines)
			{
				CodeBuilder.AppendLabel(MAINLabel, "Caution, not INIT");
				CodeBuilder.OpenNewBlock();
				CodeBuilder.AppendCode("LDA #$" + table[0].Values[0].ToString("X2"), "pixelise BGs");
				CodeBuilder.AppendCode("STA $" + Registers.Pixelation.ToString("X4"));
				CodeBuilder.AppendCode("RTL", "Return");
				CodeBuilder.CloseBlock();
				return CodeBuilder.ToString();
			}

			int channelAdd = 16 * channel;
			CodeBuilder.AppendLabel(INITLabel, "Code to be put in INIT ASM");

			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendCode("\tREP #$20");
			CodeBuilder.AppendCode("\tLDA #$" + ((Registers.Pixelation & 0xFF) << 8).ToString("X4"));
			CodeBuilder.AppendCode("\tSTA $" + (Registers.DMAMode + channelAdd).ToString("X4"));
			CodeBuilder.AppendCode("\tLDA #" + table.Name);
			CodeBuilder.AppendCode("\tSTA $" + (Registers.DMALowByte + channelAdd).ToString("X4"));
			CodeBuilder.AppendCode("\tLDY.b #" + table.Name + ">>16");
			CodeBuilder.AppendCode("\tSTY $" + (Registers.DMABankByte + channelAdd).ToString("X4"));
			CodeBuilder.AppendCode("\tSEP #$20");
			CodeBuilder.AppendCode("\tLDA #$" + (1 << channel).ToString("X2"));
			CodeBuilder.AppendCode("\tTSB $" + RAM.HDMAEnable[SA1].ToString("X4") + "|!addr");
			CodeBuilder.AppendCode("\tRTL");
			CodeBuilder.CloseBlock();
			CodeBuilder.AppendEmptyLine();

			CodeBuilder.AppendTable(table);

			return CodeBuilder.ToString();
			//return PixelationHDMA.Code(channel, table, SA1);
		}
		/*
		public static string Code(int channel, HDMATable table, bool SA1)
		{
			ASMCodeBuilder sbCode = new ASMCodeBuilder();
			int channelAdd = 16 * channel;

			sbCode.AppendLabel(INITLabel, "Code to be put in INIT ASM");

			sbCode.OpenNewBlock();
			sbCode.AppendCode("\tREP #$20");
			sbCode.AppendCode("\tLDA #$" + ((Registers.Pixelation & 0xFF) << 8).ToString("X4"));
			sbCode.AppendCode("\tSTA $" + (Registers.DMAMode + channelAdd).ToString("X4"));
			sbCode.AppendCode("\tLDA #" + table.Name);
			sbCode.AppendCode("\tSTA $" + (Registers.DMALowByte + channelAdd).ToString("X4"));
			sbCode.AppendCode("\tLDY.b #" + table.Name + ">>16");
			sbCode.AppendCode("\tSTY $" + (Registers.DMABankByte + channelAdd).ToString("X4"));
			sbCode.AppendCode("\tSEP #$20");
			sbCode.AppendCode("\tLDA #$" + (1 << channel).ToString("X2"));
			sbCode.AppendCode("\tTSB $" + RAM.HDMAEnable[SA1].ToString("X4"));
			sbCode.AppendCode("\tRTL");
			sbCode.CloseBlock();
			sbCode.AppendEmptyLine();

			sbCode.AppendTable(table);

			return sbCode.ToString();
		}*/
	}
}
