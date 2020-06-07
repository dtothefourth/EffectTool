using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Extansion.Int;

namespace EffectClasses
{

	/// <summary>
	/// The layer (BG) position registers within the SNES
	/// </summary>
	public enum LayerRegister
	{
		Layer1_X = 0x210D,
		Layer1_Y = 0x210E,
		Layer2_X = 0x210F,
		Layer2_Y = 0x2110,
		Layer3_X = 0x2111,
	};

	/// <summary>
	/// The base class that all Wave effects inherit from.
	/// </summary>
	public abstract class WaveHDMA : AnimatedHDMA
	{
		/// <summary>
		/// Which direction the waves go in.
		/// </summary>
		public abstract Orientation Direction { get; }

		/// <summary>
		/// Sets how strong the movement is.
		/// </summary>
		public int Amplitude
		{
			set
			{
				for (int i = 0; i < _IArr.Length; i++)
					_IArr[i] = value * _baseArray[i];
				_amplitude = value;
			}
			get { return _amplitude; }
		}
		/// <summary>
		/// Sets how thick one of the moving sections is (in a sine thiw would be omage)
		/// </summary>
		public int Width
		{
			get { return _width; }
			set
			{
				if (value < 1 || value > 112)
					throw new ArgumentException("Value cannot be smaller than 1 or bigger than 112");
				_width = value;
			}
		}
		/// <summary>
		/// Regulates the speed at which the ampilitude runs.
		/// That is to say, every 1/x runs the image changes and for the code how many frames to wait.
		/// (in a sine, this would be the frequanzy)
		/// </summary>
		public double Speed
		{
			get { return _speed; }
			set
			{
				_rounds = 0;
				_dRounds = 0.0;
				if (value > 1)
					throw new ArgumentOutOfRangeException("Speed cannot be set to a value larger than 1");
				int reversed = (int)(1.0 / value); // 1/x

				if ((reversed & (reversed - 1)) != 0) //checks if the value is 2 to the power of something.
					throw new ArgumentException("Speed can only be a value equal to 1/2^n. For example: 1, 0.5, 0.25...");

				_speed = value;
			}
		}
		/// <summary>
		/// Which register the effect code should work on
		/// </summary>
		public abstract LayerRegister Layers { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ListLineBased<bool> EnabledWaveCollection { get; set; }
		/// <summary>
		/// Enables the content of EnableWaveCollection to be used for the animation.
		/// </summary>
		public bool EnableTable { get; set; }


		protected const double SPEED_FACTOR = 4.0;
		protected int[] _IArr = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1, 0 };
		protected int _rounds = 0;
		protected double _dRounds = 0.0;
		protected LayerRegister _layers = LayerRegister.Layer2_X;

		private double _speed = 0.25;
		private int[] _baseArray = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1, 0 };
		private int _amplitude = 1;
		private int _width = 6;
		private Bitmap[] _animationFrames = new Bitmap[16];

		public WaveHDMA() : base() { }
		public WaveHDMA(Bitmap original) : base(original) { }
		
		/// <summary>
		/// Resets internal variables and animation frames and starts calculating anew.
		/// </summary>
		public override void Reset()
		{
			Frame = 0;
			_rounds = 0;
			_dRounds = 0.0;

			//dispose of all frames and clear array
			for (int i = 0; i < _animationFrames.Length; i++)
				if (_animationFrames[i] != null)
				{
					_animationFrames[i].Dispose();
					_animationFrames[i] = null;
				}
		}
			

		/// <summary>
		/// Creates a base image with the preset setting of Amplitede and Width
		/// </summary>
		/// <param name="basePic">The base for the disordered image</param>
		/// <returns>The according to the settings disordered image</returns>
		public override Bitmap StaticPic(Bitmap basePic)
		{
			Reset();
			Bitmap NewOne = new Bitmap(basePic);

			//NewOne gets overwritten with the disordered image. 
			//_Breite * i sets the start of the to move setction.
			//_Breite sets how thick the to move section is.
			//_IArr[i % 16] says how many pixel to move the image to the right.
			for (int i = 0; i * _width < NewOne.Height; i++)
				NewOne = BitmapEffects.MoveLine((_width * i), _width, _IArr[i % 16] / 2, NewOne, Direction);

			if(!EnableTable)
				return NewOne;


			using (Graphics g = Graphics.FromImage(NewOne))
			{
				int totalCount = 0;
				foreach (var entry in EnabledWaveCollection)
				{
					int height = entry.LineCount;
					if (!entry.Value)
					{
						if (NewOne.Height <= totalCount + height)
							height = NewOne.Height - totalCount;

						var rec = Original.Clone(new Rectangle(0, totalCount, NewOne.Width, height), NewOne.PixelFormat);
						g.DrawImage(rec, 0, totalCount);
					}
					totalCount += height;

					if (totalCount >= NewOne.Height)
						break;
				}

				/*
				if (totalCount < bm.Height)
					g.DrawImage(
						_yEffect.Original.Clone(new Rectangle(0, totalCount, bm.Width, (bm.Height - totalCount)),
						bm.PixelFormat), 0, totalCount);*/
			}
			return NewOne;
		}

		/// <summary>
		/// Creates a Bitmap depending on the settings and the internal counter, which also get's increased at the end.
		/// </summary>
		/// <param name="basePic">The still (unchanged) image.</param>
		/// <returns>Depending on the settings moved image.</returns>
		public override Bitmap NextAnimateFrame(Bitmap basePic)
		{
			Bitmap NewOne;
			try
			{
				int index = (int)(Frame * _speed) % _animationFrames.Length;
				//only calculate new frame if we don't have one yet.
				if (_animationFrames[index] == null)
				{
					NewOne = new Bitmap(basePic);
					for (int i = 0; i * _width < NewOne.Height; i++)
						//NewOne gets overwritten with the disordered image. 
						//_Breite * i sets the start of the to move setction.
						//_Breite sets how thick the to move section is.
						//_IArr[(i + _rounds) % 16] says how many pixel to move the image to the right.
						NewOne = BitmapEffects.MoveLine((_width * i), _width, _IArr[(i + index) % 16] / 2, NewOne, Direction);

					_animationFrames[index] = NewOne;
				}
				else
					//load frame from memory
					NewOne = _animationFrames[index];

				//SPEED_FACTOR multiplication because _speed < 1. So it would get reeeaaally slow.
				//_dRounds = (_dRounds + (_speed * SPEED_FACTOR)) % 16;
				//actual _rounds only increases ever so often due to cast.
				//_rounds = (int)_dRounds;
				Frame++;

				if (!EnableTable)
					return NewOne;

				Bitmap img = new Bitmap(NewOne.Width, NewOne.Height);
				using (Graphics g = Graphics.FromImage(img))
				{
					int totalCount = 0;
					foreach (var entry in EnabledWaveCollection)
					{
						int height = entry.LineCount;
						Bitmap toUse = entry.Value ? NewOne : basePic;

						if (img.Height <= totalCount + height)
							height = img.Height - totalCount;

						var rec = toUse.Clone(new Rectangle(0, totalCount, img.Width, height), toUse.PixelFormat);
						g.DrawImage(rec, 0, totalCount);

						totalCount += height;

						if (totalCount >= img.Height)
							break;
					}

					
					if (totalCount < img.Height)
						g.DrawImage(
							basePic.Clone(new Rectangle(0, totalCount, img.Width, (img.Height - totalCount)),
							img.PixelFormat), 0, totalCount);
				}
				return img;
			}
			catch(Exception ex)
			{
				ThrowExceptionEvent(ex);
				return StaticPic(basePic);
			}	
		}

		
		/// <summary>
		/// Generates the code for the desired effect
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="table"></param>
		/// <param name="sa1"></param>
		/// <returns></returns>
		public override string Code(int channel, HDMATable table, bool sa1)
		{
			int Base = 0x4300 + (channel * 0x10);
			int Register = (int)Layers;
			int BaseAddress = RAM.Layer1X[sa1] + ((int)Layers - (int)LayerRegister.Layer1_X) * 2;
			int RegMode = ((Register & 0xFF) << 8) + 0x02;
			int LSRs = (int)((1.0 / _speed) / 2.0);

			int Tablesize = (Scanlines / _width) >= 15 ? 15 : (Scanlines / _width);
			int TableInRAM = CountRAMBytes();

			string tableString = "The Table takes up " + TableInRAM + " bytes of the free RAM\n" +
				"It ranges from $" + FreeRAM.ToString("X6") + " - $" + (FreeRAM + TableInRAM - 1).ToString("X6") + " (both addresses included)";

			ASMTable codeTable = new ASMTable(".WaveTable");
			for (int i = 0; i <= Tablesize; i++)
				codeTable.Add(new ASMTableEntry((byte)_IArr[i]));
			
			char xy = (((int)Layers & 0x01) == 0 ? 'Y' : 'X');

			ASMCodeBuilder CodeBuilder = new ASMCodeBuilder();

			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendLabel(INITLabel, "This section is to be used in the INIT code of levelASM");
			CodeBuilder.AppendCode("REP #$20");
			CodeBuilder.AppendCode("LDA #$" + RegMode.ToASMString(), "Use Mode 02 on register " + Register.ToASMString());
			CodeBuilder.AppendCode("STA $" + Base.ToASMString(), "43" + Channel + "0 = Mode, 43" + Channel + "1 = Register");
			CodeBuilder.AppendCode("LDA #$" + (FreeRAM & 0xFFFF).ToASMString(), "Address of HDMA table");
			CodeBuilder.AppendCode("STA $" + (Base + 2).ToASMString(), "43" + Channel + "2 = Low-Byte of table, 43" + Channel + "3 = High-Byte of table");
			CodeBuilder.AppendCode("SEP #$20");
			CodeBuilder.AppendCode("LDA.b #$" + (FreeRAM >> 16).ToASMString(), "Address of HDMA table, get bank byte");
			CodeBuilder.AppendCode("STA $" + (Base + 4).ToASMString(), "43" + Channel + "4 = Bank-Byte of table");
			CodeBuilder.AppendCode("LDA #$" + (0x01 << Channel).ToASMString());
			CodeBuilder.AppendCode("TSB $" + RAM.HDMAEnable[sa1].ToASMString() + "|!addr", "Enable HDMA channel " + Channel);
			CodeBuilder.AppendCode("RTL", "End HDMA setup" + MAINSeperator);
			CodeBuilder.CloseBlock();


			CodeBuilder.AppendCommentLine(tableString);
			CodeBuilder.AppendEmptyLine();

			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendLabel(MAINLabel, "This section is to be used in the MAIN code of levelASM");
			CodeBuilder.AppendCode("LDY #$00", "Y will be the loop counter.");
			CodeBuilder.AppendCode("LDX #$00", "X the index for writing the table to the RAM");
			CodeBuilder.AppendCode("LDA $" + RAM.FrameCounter[sa1].ToASMString(), "Speed of waves");
			CodeBuilder.AppendCode("LSR #" + LSRs, "Slowing down A");
			CodeBuilder.AppendCode("STA $00", "Save for later use.");
			CodeBuilder.CloseBlock();

			CodeBuilder.AppendEmptyLine();
			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendCode("PHB : PHK : PLB", "Preservev bank");
			CodeBuilder.AppendLabel(".Loop", "Jump back if not finished writing table");
			CodeBuilder.AppendCode("LDA #$" + _width.ToASMString(), "Set scanline height");
			CodeBuilder.AppendCode("STA $" + FreeRAM.ToASMString() + ",x", "for each wave");
			CodeBuilder.AppendCode("TYA", "Transfer Y to A, to calculate next index");
			CodeBuilder.AppendCode("ADC $00", "Add frame counter");
			CodeBuilder.AppendCode("AND #$" + Tablesize.ToASMString());
			CodeBuilder.AppendCode("PHY", "Preserve loop counter");
			CodeBuilder.AppendCode("TAY", "Get the index in Y");
			CodeBuilder.CloseBlock();

			CodeBuilder.AppendEmptyLine();
			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendCode("LDA.w " + codeTable.Name + ",y", "Load in wave value");
			CodeBuilder.AppendCode("LSR", "Half only");
			CodeBuilder.AppendCode("CLC", "Clear Carry for addition");
			CodeBuilder.AppendCode("ADC $" + (BaseAddress).ToASMString(), "Add value to layer " + xy + " position (low byte)");
			CodeBuilder.AppendCode("STA $" + (FreeRAM + 1).ToASMString() + ",x", "store to HDMA table (low byte)");
			CodeBuilder.AppendCode("LDA $" + (BaseAddress + 1).ToASMString(), "Get high byte of X position");
			CodeBuilder.AppendCode("ADC #$00", "Add value to layer X position (low byte)");
			CodeBuilder.AppendCode("STA $" + (FreeRAM + 2).ToASMString() + ",x", "store to HDMA table (high byte)");
			CodeBuilder.CloseBlock();

			CodeBuilder.AppendEmptyLine();
			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendCode("PLY", "Pull original loop counter");
			CodeBuilder.AppendCode("CPY #$" + (Scanlines / _width).ToASMString(), "Compare if we have written enough HDMA entries.");
			CodeBuilder.AppendCode("BPL .End", "If bigger, end HDMA");
			CodeBuilder.AppendCode("INX", "Increase X, so that in the next loop, it writes the new table data...");
			CodeBuilder.AppendCode("INX", "... at the end of the old one instead of overwritting it.");
			CodeBuilder.AppendCode("INX");
			CodeBuilder.AppendCode("INY", "Increase loop counter");
			CodeBuilder.AppendCode("BRA .Loop", "Repeat loop");
			CodeBuilder.CloseBlock();

			CodeBuilder.AppendEmptyLine();
			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendLabel(".End", "Jump here when at the end of HDMA");
			CodeBuilder.AppendCode("PLB", "Pull back data bank.");
			CodeBuilder.AppendCode("LDA #$00", "End HDMA by writting 00...");
			CodeBuilder.AppendCode("STA $" + (FreeRAM + 3).ToASMString() + ",x", "...at the end of the table.");
			CodeBuilder.AppendCode("RTL");
			CodeBuilder.CloseBlock();

			CodeBuilder.AppendEmptyLine();
			CodeBuilder.OpenNewBlock();
			CodeBuilder.AppendTable(codeTable);
			CodeBuilder.CloseBlock();

			return CodeBuilder.ToString();// +"\n\n" + tableString;
		}
		
		public override int CountRAMBytes()
		{
			return (((Scanlines / Width) + 1) * 3) + 1;
		}

		public override int CountROMBytes()
		{
			throw new NotImplementedException();
		}
	}
	

	public class WaveXHDMA : WaveHDMA
	{
		/// <summary>
		/// Which register to use for the effect in the code
		/// </summary>
		public override LayerRegister Layers
		{
			set
			{
				if (((int)value & 0x01) == 0)
					throw new ArgumentException("This wave effect cannot take a LayerRegister for the Y axis");
				_layers = value;
			}
			get { return _layers; }
		}

		/// <summary>
		/// Which direction the waves go in.
		/// </summary>
		public override Orientation Direction
		{
			get { return Orientation.Right; }
		}

		/// <summary>
		/// Creates a new instance for WaveHDMA with the layer preset on Layer 2.
		/// </summary>
		public WaveXHDMA()
		{
			Layers = LayerRegister.Layer2_X;
		}
		/// <summary>
		/// Creates a new instance for WaveHDMA with an image to be used for the animation and the layer preset on Layer 2.
		/// </summary>
		/// <param name="original">The image to use for the animation</param>
		public WaveXHDMA(Bitmap original)
			: base(original)
		{
			Layers = LayerRegister.Layer2_X;
		}
	}

	public class WaveYHDMA : WaveHDMA
	{
		/// <summary>
		/// Which register to use for the effect in the code
		/// </summary>
		public override LayerRegister Layers
		{
			set
			{
				if (((int)value & 0x01) != 0)
					throw new ArgumentException("This wave effect cannot take a LayerRegister for the X axis");
				_layers = value;
			}
			get { return _layers; }
		}

		/// <summary>
		/// Which direction the waves go in.
		/// </summary>
		public override Orientation Direction
		{
			get { return Orientation.Down; }
		}

		/// <summary>
		/// Creates a new instance for WaveHDMA with the layer preset on Layer 2.
		/// </summary>
		public WaveYHDMA()
		{
			Layers = LayerRegister.Layer2_Y;
		}
		/// <summary>
		/// Creates a new instance for WaveHDMA with an image to be used for the animation and the layer preset on Layer 2.
		/// </summary>
		/// <param name="original">The image to use for the animation</param>
		public WaveYHDMA(Bitmap original)
			: base(original)
		{
			Layers = LayerRegister.Layer2_Y;
		}

	}
}
