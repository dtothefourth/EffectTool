using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Reflection;
using Extansion.Int;
using Extansion.Drawing;
using System.Diagnostics;


namespace HDMA_Generator_Tool
{
	public partial class HDMA_Gradiant_GUI : Form, IScreenshotUser
	{
		#region ITab Interface

		public TabControl GetTabControl() 
		{ 
			return tbcMainControl; 
		}
		public void SetASMMode(ASMMode Mode)
		{
			switch(Mode)
			{
				case ASMMode.Standard:
					grpSngChnStd.Visible = true;
					grpSngChnAdv.Visible = false;
					break;
				case ASMMode.Advanced:
				case ASMMode.Expert:
					grpSngChnStd.Visible = false;
					grpSngChnAdv.Visible = true;
					break;
			}
		}
		public Bitmap GetScreen()
		{
			var tab = tbcMainControl.SelectedTab;
			if (tab == tabSingle)
				return (Bitmap)pcbSngMainPic.Image;
			if (tab == tabMulti)
				return (Bitmap)pcbMulMainPic.Image;
			if (tab == tabPosition)
				return (Bitmap)pcbPosMainPic.Image;
			if (tab == tabDialog2)
				return (Bitmap)pcbDiaMainPic.Image;
			if (tab == tabImage2)
				return (Bitmap)pcbImgMainPic.Image;
			if (tab == tabTable)
				return (Bitmap)pcbTblMainPic.Image;
			return null;
		}
		public ComboBox[] ScreenSelectors { get; set; }
		
		#endregion
		#region IScreenshotUser

		public Bitmap[] ScreenshotsImages { get; private set; }

		#endregion

		/// <summary>
		/// The width that will be set if the GUI is displayed in BG mode (to cut out the ColorMath options)
		/// </summary>
		private const int _BG_WIDTH = 590;

		private ColorControlManager _colorControlManager = new ColorControlManager();
		private EffectClasses.ColorMath[] _mathCollection;
		private bool _fg;

		/// <summary>
		/// Creates a new instance of a ColorGradient GUI.
		/// </summary>
		public HDMA_Gradiant_GUI()
		{
			InitializeComponent();

			ScreenshotsImages = new Bitmap[tbcMainControl.TabPages.Count];

			ScreenSelectors = new ComboBox[]
			{
				cmbPosScnSel,
				cmbSngScnSel,
				cmbMulScnSel,
				cmbDiaScnSel,
				cmbImgScnSel,
				cmbTblScnSel,
			};
			_mathCollection = new EffectClasses.ColorMath[]
			{
				_sngMath,
				_mulMath,
				_posMath,
				_diaMath,
				_imgMath,
				_tblMath,
			};

			cmbSngChn.SelectedIndex = 3;
						
			_colorControlManager.Add(txtDiaCol, pcbDiaCol);
			_colorControlManager.Add(txtPosColTop, pcbPosColTop, UpdateColorPositions);
			_colorControlManager.Add(txtPosColBtm, pcbPosColBtm, UpdateColorPositions);
			_colorControlManager.Add(txtPosCurCol, pcbPosCurCol, SetCurrentColorPositions);
			_colorControlManager.Add(txtImgColCor, pcbImgColCor, UpdateImageCorrection);
						
			#region ToolTipSingle
			toolTip.SetToolTip(btnSngCode, "Generates the code for the displayed HDMA effect");
			toolTip.SetToolTip(cmbPosScnSel, "The layers to display with the gradient.");

			toolTip.SetToolTip(trbSngTop, "Sets the color value the HDMA should start with from the top");
			toolTip.SetToolTip(trbSngBtm, "Sets the color value the HDMA should end with at the bottom");

			toolTip.SetToolTip(chbSngBlu, "Decides whether or not the HDMA adds blue for the desired values");
			toolTip.SetToolTip(chbSngGrn, "Decides whether or not the HDMA adds green for the desired values");
			toolTip.SetToolTip(chbSngRed, "Decides whether or not the HDMA adds red for the desired values");

			toolTip.SetToolTip(chbSngCen, "Enabling this will make the HDMA effect flip over the middle.\nThe top value will now determine what value the HDMA starts and ends with and the bottom value which value it has at the middle");

			toolTip.SetToolTip(rdbSngCh3, "Sets the generated HDMA code to use HDMA channel 3 ($433x)");
			toolTip.SetToolTip(rdbSngCh4, "Sets the generated HDMA code to use HDMA channel 4 ($434x)");
			toolTip.SetToolTip(rdbSngCh5, "Sets the generated HDMA code to use HDMA channel 5 ($435x)");
			toolTip.SetToolTip(cmbSngChn, "Sets the generated HDMA code to use the selected HDMA channel");
			#endregion
			#region ToolTipMulti
			toolTip.SetToolTip(btnMulCod, "Generates the code for the displayed HDMA effect");
			toolTip.SetToolTip(cmbPosScnSel, "The layers to display with the gradient.");

			toolTip.SetToolTip(pcbMulRed, "Shows what the red part of the HDMA gradient alone looks like");
			toolTip.SetToolTip(pcbMulBlu, "Shows what the blue part of the HDMA gradient alone looks like");
			toolTip.SetToolTip(pcbMulGrn, "Shows what the green part of the HDMA gradient alone looks like");

			toolTip.SetToolTip(trbMulRedTop, "Sets the color value the red part of the HDMA should start with from the top");
			toolTip.SetToolTip(trbMulBluTop, "Sets the color value the blue part of the HDMA should start with from the top");
			toolTip.SetToolTip(trbMulGrnTop, "Sets the color value the green part of the HDMA should start with from the top");

			toolTip.SetToolTip(trbMulRedBot, "Sets the color value the red part of the HDMA should end with from the bottom");
			toolTip.SetToolTip(trbMulBluBot, "Sets the color value the blue part of the HDMA should end with from the bottom");
			toolTip.SetToolTip(trbMulGrnBot, "Sets the color value the green part of the HDMA should end with from the bottom");

			toolTip.SetToolTip(chbMulBluCen, "Sets whether or not the blue part of the HDMA will be flipped.\nIf checked the top trackbar will be used for the top and bottom value and the bottom one for the center value");
			toolTip.SetToolTip(chbMulGrnCen, "Sets whether or not the green part of the HDMA will be flipped.\nIf checked the top trackbar will be used for the top and bottom value and the bottom one for the center value");
			toolTip.SetToolTip(chbMulRedCen, "Sets whether or not the red part of the HDMA will be flipped.\nIf checked the top trackbar will be used for the top and bottom value and the bottom one for the center value");

			#endregion
			#region ToolTipDialog
			toolTip.SetToolTip(btnDiaCod, "Generates the code for the displayed HDMA effect.");
			toolTip.SetToolTip(cmbPosScnSel, "The layers to display with the gradient.");

			toolTip.SetToolTip(btnDiaAdd, "Adds the currently set color and scanline count to the list.");
			toolTip.SetToolTip(btnDiaRmv, "Removes the currently selected entry from the list.");
			toolTip.SetToolTip(btnDiaMovUp, "Moves the currently selected entry up by one.");
			toolTip.SetToolTip(btnDiaMovDwn, "Moves the currently selected entry down by one.");
			toolTip.SetToolTip(btnDiaClr, "Removes all the entries from the list.");

			toolTip.SetToolTip(chbDiaEnd, "Makes the HDMA stop ingame after the last line.\nSee readme for further information.");
			toolTip.SetToolTip(pcbDiaCol, "The color of the next entry to be added.");
			toolTip.SetToolTip(txtDiaCol, "The color of the next entry to be added in hex.");
			toolTip.SetToolTip(nudDiaScn, "The number of scanlines for the next entry to be added.");
			#endregion
			#region ToolTipPosition
			toolTip.SetToolTip(btnPosCod, "Generates the code for the displayed HDMA effect.");
			toolTip.SetToolTip(cmbPosScnSel, "The layers to display with the gradient.");

			toolTip.SetToolTip(btnPosNew, "Adds a new entry to the table.");
			toolTip.SetToolTip(btnPosDel, "Deletes the selected entry from the table.");
			toolTip.SetToolTip(btnPosClr, "Removes all entries from the table.");
			toolTip.SetToolTip(btnPosCpy, "Copies the currently selected entry from the table and inserts it again.");

			toolTip.SetToolTip(pcbPosColTop, "The color the gradient will have at the top.");
			toolTip.SetToolTip(txtPosColTop, "The color the gradient will have at the top in hex.");

			toolTip.SetToolTip(pcbPosColBtm, "The color the gradient will have at the bottom.");
			toolTip.SetToolTip(txtPosColBtm, "The color the gradient will have at the bottom in hex.");

			toolTip.SetToolTip(pcbPosCurCol, "The color of the currently selected entry from the table.");
			toolTip.SetToolTip(txtPosCurCol, "The color of the currently selected entry from the table in hex.");

			toolTip.SetToolTip(nudPosCurPos, "The position of the color within the HDMA.");
			#endregion
			#region ToolTipImage
			toolTip.SetToolTip(btnImgCod, "Generates the code for the displayed HDMA effect.");
			toolTip.SetToolTip(cmbImgScnSel, "The layers to display with the gradient.");

			toolTip.SetToolTip(btnImgLoad, "Loads an image to rip the gradient from.");
			toolTip.SetToolTip(trbImgColRip, "Select the column to rip.");

			toolTip.SetToolTip(chbImgColCor, "Enable color correction.");
			toolTip.SetToolTip(pcbImgColCor, "The color to be substracted for the color correction.");
			toolTip.SetToolTip(pcbImgImgRip, "The ripped column from the image.");
			toolTip.SetToolTip(pcbImgRipColCor, "The ripped and color corrected column from the image");

			#endregion
			#region ToolTipTable
			toolTip.SetToolTip(btnTblCod, "Generates the code for the displayed HDMA effect.");
			toolTip.SetToolTip(cmbTblScnSel, "The layers to display with the gradient.");
			
			toolTip.SetToolTip(chbTblFivBit, "If set, the values entered in the list will be calculated as 5 bit (0-31), otherwiese as 8 (0-255)");

			toolTip.SetToolTip(lsbTblGrn, "A list of all the entries in the list above which will be ignored by the program");
			toolTip.SetToolTip(lsbTblRed, "A list of all the entries in the list above which will be ignored by the program");
			toolTip.SetToolTip(lsbTblBlu, "A list of all the entries in the list above which will be ignored by the program");

			toolTip.SetToolTip(rtbTblRed, "Textbox to enter the table for the red scanlines and values at.\nThe image will only update when ENTER is pressed");
			toolTip.SetToolTip(rtbTblBlu, "Textbox to enter the table for the blue scanlines and values at.\nThe image will only update when ENTER is pressed");
			toolTip.SetToolTip(rtbTblGrn, "Textbox to enter the table for the green scanlines and values at.\nThe image will only update when ENTER is pressed");
			#endregion						
		}

		/// <summary>
		/// Creates a new instance of a ColorGradient GUI.
		/// </summary>
		/// <param name="fg"><c>True:</c> Enables ColorMath on the GUI and the generated code will include it too.
		/// <para>Basically, the FG mode for color gradients.</para></param>
		public HDMA_Gradiant_GUI(bool fg) : this()
		{
			_fg = fg;
			if (fg)
			{
				foreach(var m in _mathCollection)
				{
					m.SubScreenDesignation = 0;
					m.MainScreenDesignation = EffectClasses.ScreenDesignation.BG1 |
						EffectClasses.ScreenDesignation.BG2 | EffectClasses.ScreenDesignation.BG3 |
						EffectClasses.ScreenDesignation.OBJ;
					m.ColorMathDesignation = EffectClasses.ColorMathMode.BG1 | EffectClasses.ColorMathMode.BG2 |
						EffectClasses.ColorMathMode.BG3 | EffectClasses.ColorMathMode.OBJ | EffectClasses.ColorMathMode.Backdrop;
					m.AddColor = true;
				}
				return;
			}
			else
			{
				this.tbcMainControl.Width = _BG_WIDTH;
				this.groupBox1.Visible = false;
				this.groupBox19.Location = new Point(458, 6);
				this.groupBox19.Size = new Size(121, 253);
			}
		}
		
		/// <summary>
		/// Updates the screens of all the tabs in the control.
		/// </summary>
		public void UpdateScreens()
		{
			pcbSngMainPic.Image = _sngMath.GetScreen();
			pcbMulMainPic.Image = _mulMath.GetScreen();
			pcbDiaMainPic.Image = _diaMath.GetScreen();
			pcbImgMainPic.Image = _imgMath.GetScreen();
			pcbTblMainPic.Image = _tblMath.GetScreen();
			pcbPosMainPic.Image = _posMath.GetScreen();
		}

		/// <summary>
		/// Sets the proper flags for the color math and updateds a PictureBox with the new screen.
		/// </summary>
		/// <param name="math">The ColorMath object which's flags should be set.</param>
		/// <param name="substract">Whether the subtraction flag should be set.</param>
		/// <param name="half">Whether the half flag should be set.</param>
		/// <param name="image">The picture box that will get the updated screen. If this is null, nothing will happen.</param>
		private void SetMath(EffectClasses.ColorMath math, bool substract, bool half, PictureBox image)
		{
			if (substract)
				math.ColorMathDesignation |= EffectClasses.ColorMathMode.Substract;
			else
				math.ColorMathDesignation &= ~EffectClasses.ColorMathMode.Substract;

			if (half)
				math.ColorMathDesignation |= EffectClasses.ColorMathMode.Half;
			else
				math.ColorMathDesignation &= ~EffectClasses.ColorMathMode.Half;

			if(image != null)
				image.Image = math.GetScreen();
		}

		#region Single

		private EffectClasses.ColorMath _sngMath = new EffectClasses.ColorMath();
		private EffectClasses.ColorHDMA _sngEffect = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Blue);
		private EffectClasses.ColorPositionCollection _sngColl = new EffectClasses.ColorPositionCollection();

		private void UpdateSingle()
		{
			EffectClasses.ColorHDMAValues col = 0;
			if (chbSngBlu.Checked)
				col |= EffectClasses.ColorHDMAValues.Blue;
			if (chbSngRed.Checked)
				col |= EffectClasses.ColorHDMAValues.Red;
			if (chbSngGrn.Checked)
				col |= EffectClasses.ColorHDMAValues.Green;
			_sngEffect.ColorEffect = col;
			_sngColl.Clear();

			int valTop = trbSngTop.Value;
			pcbSngTop.BackColor = Color.FromArgb(
				(chbSngRed.Checked ? valTop * 8 : 0),
				(chbSngGrn.Checked ? valTop * 8 : 0),
				(chbSngBlu.Checked ? valTop * 8 : 0));

			int valBtm = trbSngBtm.Value;
			pcbSngBtm.BackColor = Color.FromArgb(
				(chbSngRed.Checked ? valBtm * 8 : 0),
				(chbSngGrn.Checked ? valBtm * 8 : 0),
				(chbSngBlu.Checked ? valBtm * 8 : 0));

			if (chbSngCen.Checked)
			{
				_sngColl.Add(new EffectClasses.ColorPosition(0, (byte)(valTop * 8)));
				_sngColl.Add(new EffectClasses.ColorPosition(112, (byte)(valBtm * 8)));
				_sngColl.Add(new EffectClasses.ColorPosition(224, (byte)(valTop * 8)));
			}
			else
			{
				_sngColl.Add(new EffectClasses.ColorPosition(0, (byte)(valTop * 8)));
				_sngColl.Add(new EffectClasses.ColorPosition(224, (byte)(valBtm * 8)));
			}
			
			_sngEffect.ColorsPositions = _sngColl;
			_sngMath.FixedColor = _sngEffect.EffectImage;
			pcbSngMainPic.Image = _sngMath.GetScreen();
		}

		private void UpdateSingleScreen()
		{
			pcbSngMainPic.Image = _sngMath.GetScreen();
		}
		
		private void sngMath_Changed(object sender, EventArgs e)
		{
			SetMath(_sngMath, rdbSngSub.Checked, chbSngHlf.Checked, pcbSngMainPic);
		}

		private void cntSng_Update(object sender, EventArgs e)
		{
			UpdateSingle();
		}

		private void cmbSngScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _sngMath, sender, false);
			UpdateSingleScreen();
		}

		private void btnSngCode_Click(object sender, EventArgs e)
		{
            string code = "";
            if (_fg)
                code += _sngMath.Code() + "\n";
            else code += "init: \n";
            code += _sngEffect.Code();
			ShowCode.ShowCodeDialog(code);
		}

		private void rdbSngChn_CheckedChanged(object sender, EventArgs e)
		{
			_sngEffect.SetChannel(rdbSngCh3, rdbSngCh4, rdbSngCh5);
		}

		private void cmbSngChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_sngEffect.SetChannel((ComboBox)sender);
		}
		#endregion
		#region Multi

		private EffectClasses.ColorMath _mulMath = new EffectClasses.ColorMath();
		private EffectClasses.ColorHDMA _mulRed = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Red);
		private EffectClasses.ColorHDMA _mulGreen = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Green);
		private EffectClasses.ColorHDMA _mulBlue = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Blue);

		private EffectClasses.ColorPositionCollection _MulColl = new EffectClasses.ColorPositionCollection();

		private void UpdateMultiScreen()
		{
			_mulMath.FixedColor = EffectClasses.BitmapEffects.MergeImages(
				_mulRed.EffectImage, _mulGreen.EffectImage, _mulBlue.EffectImage);
			pcbMulMainPic.Image = _mulMath.GetScreen();
		}

		private void UpdateMultiRed(object sender, EventArgs e)
		{
			Stopwatch w = new Stopwatch();
			w.Start();

			_UpdateMultiIndividual(trbMulRedTop, trbMulRedBot, pcbMulRedTop,
				pcbMulRedBot, chbMulRedCen, EffectClasses.ColorHDMAValues.Red);
			_mulRed.ColorsPositions = _MulColl;
			pcbMulRed.Image = _mulRed.EffectImage;

			UpdateMultiScreen();
			w.Stop();
#if(DEBUG)
			Console.WriteLine("Multi-Red Gradient: " + w.ElapsedMilliseconds + "ms");
#endif
		}

		private void UpdateMultiGreen(object sender, EventArgs e)
		{
			Stopwatch w = new Stopwatch();
			w.Start();

			_UpdateMultiIndividual(trbMulGrnTop, trbMulGrnBot, pcbMulGrnTop,
				pcbMulGrnBot, chbMulGrnCen, EffectClasses.ColorHDMAValues.Green);
			_mulGreen.ColorsPositions = _MulColl;
			pcbMulGrn.Image = _mulGreen.EffectImage;

			UpdateMultiScreen();
			w.Stop();
#if(DEBUG)
			Console.WriteLine("Multi-Green Gradient: " + w.ElapsedMilliseconds + "ms");
#endif
		}
		private void UpdateMultiBlue(object sender, EventArgs e)
		{
			Stopwatch w = new Stopwatch();
			w.Start();

			_UpdateMultiIndividual(trbMulBluTop, trbMulBluBot, pcbMulBluTop,
				pcbMulBluBot, chbMulBluCen, EffectClasses.ColorHDMAValues.Blue);
			_mulBlue.ColorsPositions = _MulColl;
			pcbMulBlu.Image = _mulBlue.EffectImage;

			UpdateMultiScreen();
			w.Stop();
#if(DEBUG)
			Console.WriteLine("Multi-Blue Gradient: " + w.ElapsedMilliseconds + "ms");
#endif
		}


		private void _UpdateMultiIndividual(TrackBar trbTop, TrackBar trbBottom,
			PictureBox pcbTop, PictureBox pcbBottom, CheckBox center, EffectClasses.ColorHDMAValues color)
		{
			_MulColl.Clear();

			int valTop = trbTop.Value;
			pcbTop.BackColor = Color.FromArgb(
				(color.HasFlag(EffectClasses.ColorHDMAValues.Red) ? valTop * 8 : 0),
				(color.HasFlag(EffectClasses.ColorHDMAValues.Green) ? valTop * 8 : 0),
				(color.HasFlag(EffectClasses.ColorHDMAValues.Blue) ? valTop * 8 : 0));

			int valBtm = trbBottom.Value;
			pcbBottom.BackColor = Color.FromArgb(
				(color.HasFlag(EffectClasses.ColorHDMAValues.Red) ? valBtm * 8 : 0),
				(color.HasFlag(EffectClasses.ColorHDMAValues.Green) ? valBtm * 8 : 0),
				(color.HasFlag(EffectClasses.ColorHDMAValues.Blue) ? valBtm * 8 : 0));

			if (center.Checked)
			{
				_MulColl.Add(new EffectClasses.ColorPosition(0, (byte)(valTop * 8)));
				_MulColl.Add(new EffectClasses.ColorPosition(112, (byte)(valBtm * 8)));
				_MulColl.Add(new EffectClasses.ColorPosition(224, (byte)(valTop * 8)));
			}
			else
			{
				_MulColl.Add(new EffectClasses.ColorPosition(0, (byte)(valTop * 8)));
				_MulColl.Add(new EffectClasses.ColorPosition(224, (byte)(valBtm * 8)));
			}
		}


		private void cmbMulScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _mulMath, sender, false);
			pcbMulMainPic.Image = _mulMath.GetScreen();
		}

		private void btnMulCod_Click(object sender, EventArgs e)
		{
			try
			{
				string code = "";
                if (_fg)
                    code += _mulMath.Code() + "\n";
                else code += "init: \n";
                code += EffectClasses.FullColorHDMA.Code(_mulRed, _mulGreen, _mulBlue,
					ChooseChannel.GetOneChannel, ChooseChannel.GetTwoChannel, ChooseChannel.GetThreeChannels);
				ShowCode.ShowCodeDialog(code);
			}
			catch (EffectClasses.AbordException) 
			{ }
			catch(Exception ex)
			{
				ShowCode.ShowMessage(ex);
			}
		}

		private void mulMath_Changed(object sender, EventArgs e)
		{
			SetMath(_mulMath, rdbMulSub.Checked, chbMulHlf.Checked, pcbMulMainPic);
		}
		#endregion
		#region Dialog

		private EffectClasses.ColorMath _diaMath = new EffectClasses.ColorMath();
		private EffectClasses.ColorHDMA _diaColRed = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Red);
		private EffectClasses.ColorHDMA _diaColGrn = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Green);
		private EffectClasses.ColorHDMA _diaColBlu = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Blue);

		private void cmbDiaScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _diaMath, sender, false);
			pcbDiaMainPic.Image = _diaMath.GetScreen();
		}

		private void UpdateDialog()
		{
#if(DEBUG)
			//Stopwatch if in DEBUG mode
			Stopwatch w = new Stopwatch();
			Stopwatch w_all = new Stopwatch();
			w.Start();
			w_all.Start();
#endif
			//Red gradient part
			EffectClasses.HDMATable red = new EffectClasses.HDMATable();
			red.AddRange(lsbDiaRed.Items.Cast<EffectClasses.HDMATableEntry>());
			if (chbDiaEnd.Checked)
				red.Add(new EffectClasses.HDMATableEntry(EffectClasses.TableValueType.db, 1, (byte)EffectClasses.ColorHDMAValues.Red));
			_diaColRed.FromTable(red);
#if(DEBUG)
			Console.WriteLine("Dialog Update-Red: " + w.ElapsedMilliseconds + "ms");
			w.Restart();
#endif

			//Green gradient part
			EffectClasses.HDMATable green = new EffectClasses.HDMATable();
			green.AddRange(lsbDiaGrn.Items.Cast<EffectClasses.HDMATableEntry>());
			if (chbDiaEnd.Checked)
				green.Add(new EffectClasses.HDMATableEntry(EffectClasses.TableValueType.db, 1, (byte)EffectClasses.ColorHDMAValues.Green));
			_diaColGrn.FromTable(green);
#if(DEBUG)
			Console.WriteLine("Dialog Update-Green: " + w.ElapsedMilliseconds + "ms");
			w.Restart();
#endif

			//Blue gradient part
			EffectClasses.HDMATable blue = new EffectClasses.HDMATable();
			blue.AddRange(lsbDiaBlu.Items.Cast<EffectClasses.HDMATableEntry>());
			if (chbDiaEnd.Checked)
				blue.Add(new EffectClasses.HDMATableEntry(EffectClasses.TableValueType.db, 1, (byte)EffectClasses.ColorHDMAValues.Blue));
			_diaColBlu.FromTable(blue);
#if(DEBUG)
			Console.WriteLine("Dialog Update-Blue: " + w.ElapsedMilliseconds + "ms");
			w.Restart();
#endif
			//get screens
			_diaMath.FixedColor = EffectClasses.BitmapEffects.MergeImages_(
				_diaColRed.EffectImage, _diaColGrn.EffectImage, _diaColBlu.EffectImage);
			pcbDiaMainPic.Image = _diaMath.GetScreen();

#if(DEBUG)
			//Write Stopwatch results if in DEBUG mode.
			Console.WriteLine("Dialog Update-Image: " + w.ElapsedMilliseconds + "ms");
			Console.WriteLine("Dialog Update-Total: " + w_all.ElapsedMilliseconds + "ms");
			w.Stop();
			w_all.Stop();
#endif
		}

		private void btnDiaAdd_Click(object sender, EventArgs e)
		{
			byte scanlines = (byte)nudDiaScn.Value;
			byte r = (byte)((pcbDiaCol.BackColor.R / 8) | (byte)EffectClasses.ColorHDMAValues.Red);
			byte g = (byte)((pcbDiaCol.BackColor.G / 8) | (byte)EffectClasses.ColorHDMAValues.Green);
			byte b = (byte)((pcbDiaCol.BackColor.B / 8) | (byte)EffectClasses.ColorHDMAValues.Blue);

			EffectClasses.HDMATableEntry _tempR = 
				new EffectClasses.HDMATableEntry(EffectClasses.TableValueType.db, scanlines, r);
			EffectClasses.HDMATableEntry _tempG =
				new EffectClasses.HDMATableEntry(EffectClasses.TableValueType.db, scanlines, g);
			EffectClasses.HDMATableEntry _tempB =
				new EffectClasses.HDMATableEntry(EffectClasses.TableValueType.db, scanlines, b);

			lsbDiaRed.Items.Add(_tempR);
			lsbDiaGrn.Items.Add(_tempG);
			lsbDiaBlu.Items.Add(_tempB);

			btnDiaClr.Enabled = 
				btnDiaMovDwn.Enabled = 
				btnDiaMovUp.Enabled =
				btnDiaRmv.Enabled = true;

			if(lsbDiaBlu.Items.Cast<EffectClasses.HDMATableEntry>().Sum(p => p.Scanlines)
				>= EffectClasses.HDMA.Scanlines)
			{
				lblDiaWarn.Visible = true;
				btnDiaAdd.Enabled = false;
			}

			UpdateDialog();
		}
		
		private void btnDiaRmv_Click(object sender, EventArgs e)
		{
			if(lsbDiaRed.SelectedIndex == ListBox.NoMatches)
			{
				Settings.MissingSelectionMessage();
				return;
			}

			lsbDiaGrn.Items.RemoveAt(lsbDiaRed.SelectedIndex);
			lsbDiaBlu.Items.RemoveAt(lsbDiaRed.SelectedIndex);
			lsbDiaRed.Items.RemoveAt(lsbDiaRed.SelectedIndex);

			if (lsbDiaRed.Items.Count == 0)
			{
				btnDiaClr.Enabled =
					btnDiaMovDwn.Enabled =
					btnDiaMovUp.Enabled =
					btnDiaRmv.Enabled = false;
			}

			if (lsbDiaBlu.Items.Cast<EffectClasses.HDMATableEntry>().Sum(p => p.Scanlines)
				< EffectClasses.HDMA.Scanlines)
			{
				lblDiaWarn.Visible = false;
				btnDiaAdd.Enabled = true;
			}

			UpdateDialog();
		}

		private void btnDiaClr_Click(object sender, EventArgs e)
		{
			if (!Settings.DeleteAllMessage())
				return;

			lsbDiaGrn.Items.Clear();
			lsbDiaBlu.Items.Clear();
			lsbDiaRed.Items.Clear();
			
			btnDiaClr.Enabled =
				btnDiaMovDwn.Enabled =
				btnDiaMovUp.Enabled =
				btnDiaRmv.Enabled = false;

			lblDiaWarn.Visible = false;
			btnDiaAdd.Enabled = true;
			UpdateDialog();
		}
		
		private void btnDiaMovUp_Click(object sender, EventArgs e)
		{
			if (lsbDiaRed.SelectedIndex == ListBox.NoMatches)
			{
				Settings.MissingSelectionMessage();
				return;
			}

			MoveListBoxItem(lsbDiaRed, -1);
			MoveListBoxItem(lsbDiaGrn, -1);
			MoveListBoxItem(lsbDiaBlu, -1);
			UpdateDialog();
		}

		private void btnDiaMovDwn_Click(object sender, EventArgs e)
		{
			if (lsbDiaRed.SelectedIndex == ListBox.NoMatches)
			{
				Settings.MissingSelectionMessage();
				return;
			}

			MoveListBoxItem(lsbDiaRed, 1);
			MoveListBoxItem(lsbDiaGrn, 1);
			MoveListBoxItem(lsbDiaBlu, 1);
			UpdateDialog();
		}

		/// <summary>
		/// Moves the selected item of a ListBox down or up a selectable number of entries.
		/// </summary>
		/// <param name="direction">Direction to move the item to. Positive values move it down.</param>
		/// <param name="listBox">The ListBox to be edited</param>
		public void MoveListBoxItem(ListBox listBox, int direction)
		{
			// Checking selected item
			if (listBox.SelectedItem == null || listBox.SelectedIndex < 0)
				return; // No selected item - nothing to do

			// Calculate new index using move direction
			int newIndex = listBox.SelectedIndex + direction;

			// Checking bounds of the range
			if (newIndex < 0 || newIndex >= listBox.Items.Count)
				return; // Index out of range - nothing to do

			object selected = listBox.SelectedItem;

			// Removing removable element
			listBox.Items.RemoveAt(listBox.SelectedIndex);
			// Insert it in new position
			listBox.Items.Insert(newIndex, selected);
			// Restore selection
			listBox.SetSelected(newIndex, true);
		}



		private void btnDiaCod_Click(object sender, EventArgs e)
		{
            string code = "";
            if (_fg)
				code += _diaMath.Code() + "\n";
            else code += "init: \n";
            code += EffectClasses.FullColorHDMA.Code(_diaColRed, _diaColGrn, _diaColBlu,
				ChooseChannel.GetOneChannel, ChooseChannel.GetTwoChannel, ChooseChannel.GetThreeChannels);

			ShowCode.ShowCodeDialog(code);
		}

		private void lsbDia_MouseDown(object sender, MouseEventArgs e)
		{
			lsbDiaRed.SelectedIndex = lsbDiaRed.IndexFromPoint(e.X, e.Y);
			lsbDiaGrn.SelectedIndex = lsbDiaGrn.IndexFromPoint(e.X, e.Y);
			lsbDiaBlu.SelectedIndex = lsbDiaBlu.IndexFromPoint(e.X, e.Y);
		}

		private void diaMath_Changed(object sender, EventArgs e)
		{
			SetMath(_diaMath, rdbDiaSub.Checked, chbDiaHlf.Checked, pcbDiaMainPic);
		}
		#endregion
		#region Position

		private EffectClasses.FullColorHDMA _posEffect = new EffectClasses.FullColorHDMA();
		private EffectClasses.ColorMath _posMath = new EffectClasses.ColorMath();

		private void UpdateColorPositions()
		{
			try
			{
				_posEffect.Clear();
				_posEffect.AddColorPosition(0, pcbPosColTop.BackColor);
				List<int> takenNumbers = new List<int>();

				foreach (DataGridViewRow row in dgvPosColors.Rows)
				{
					int position = Convert.ToInt32(row.Cells["colPosition"].Value);
					if (takenNumbers.Contains(position))
						continue;
					_posEffect.AddColorPosition(position, ColorTranslator.FromHtml((string)row.Cells["colColorString"].Value));
					takenNumbers.Add(position);
				}
				_posEffect.AddColorPosition(EffectClasses.HDMA.Scanlines, pcbPosColBtm.BackColor);
				_posEffect.End();
				_posMath.FixedColor = _posEffect.EffectImage;
				pcbPosMainPic.Image = _posMath.GetScreen();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ups :<", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#region Buttons
		private void btnPosNew_Click(object sender, EventArgs e)
		{
			//dgvPosColors.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 0, 0, 255);
			dgvPosColors.Rows.Add("", "#000000", "1");
			dgvPosColors.Rows[dgvPosColors.Rows.Count - 1].Cells["colColor"].Style.BackColor = Color.Black;
		}

		private void btnPosCpy_Click(object sender, EventArgs e)
		{
			if (dgvPosColors.SelectedRows.Count == 0)
				return;
			dgvPosColors.Rows.Add(dgvPosColors.SelectedRows[0].Clone());
		}

		private void btnPosDel_Click(object sender, EventArgs e)
		{
			if (dgvPosColors.SelectedRows.Count == 0)
				return;
			dgvPosColors.Rows.Remove(dgvPosColors.SelectedRows[0]);
		}

		private void btnPosClr_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(Settings.ClearColorPositions, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
				return;
			dgvPosColors.Rows.Clear();
		}
		#endregion

		#region DataGridView
		private void dgvPosColors_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			UpdateColorPositions();
			grpPosCurVal.Enabled = true;
			btnPosCpy.Enabled = true;
			btnPosDel.Enabled = true;
			btnPosClr.Enabled = true;
		}

		private void dgvPosColors_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			UpdateColorPositions();
			bool enable = dgvPosColors.SelectedRows.Count != 0;

			grpPosCurVal.Enabled = enable;
			btnPosCpy.Enabled = enable;
			btnPosDel.Enabled = enable;
			btnPosClr.Enabled = enable;
		}

		private void dgvPosColors_SelectionChanged(object sender, EventArgs e)
		{
			if (dgvPosColors.SelectedRows.Count == 0)
				return;

			txtPosCurCol.Text = (string)dgvPosColors.SelectedRows[0].Cells["colColorString"].Value;
			pcbPosCurCol.BackColor = ColorTranslator.FromHtml((string)dgvPosColors.SelectedRows[0].Cells["colColorString"].Value);
			nudPosCurPos.Value = Convert.ToDecimal(dgvPosColors.SelectedRows[0].Cells["colPosition"].Value);
		}
		#endregion

		private void nudPosCurPos_ValueChanged(object sender, EventArgs e)
		{
			dgvPosColors.SelectedRows[0].Cells["colPosition"].Value = ((NumericUpDown)sender).Value.ToString();
			UpdateColorPositions();
		}

		private void SetCurrentColorPositions()
		{
			dgvPosColors.SelectedRows[0].Cells["colColorString"].Value = txtPosCurCol.Text;
			dgvPosColors.SelectedRows[0].Cells["colColor"].Style.BackColor = pcbPosCurCol.BackColor;
			UpdateColorPositions();
		}

		private void cmbPosScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _posMath, sender, false);
			pcbPosMainPic.Image = _posMath.GetScreen();
		}

		private void btnPosCod_Click(object sender, EventArgs e)
		{
            string code = "";
            if (_fg)
                code += _posMath.Code() + "\n";
            else code += "init: \n";
			code += _posEffect.Code(ChooseChannel.GetOneChannel, ChooseChannel.GetTwoChannel, ChooseChannel.GetThreeChannels);

			ShowCode.ShowCodeDialog(code);
		}

		private void posMath_Changed(object sender, EventArgs e)
		{
			SetMath(_posMath, rdbPosSub.Checked, chbPosHlf.Checked, pcbPosMainPic);
		}
		#endregion
		#region Image

		private EffectClasses.ColorMath _imgMath = new EffectClasses.ColorMath();
		private EffectClasses.FullColorHDMA _imgEffect = new EffectClasses.FullColorHDMA();

		private void cmbImgSrnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _imgMath, sender, false);
			pcbImgMainPic.Image = _imgMath.GetScreen();
		}

		private void UpdateImage()
		{
			if (pcbImgImgRip.Image == null)
				return;

#if(DEBUG)
			Stopwatch sw = new Stopwatch();
			sw.Start();
#endif

			Bitmap bn = new Bitmap(pcbImgRipCol.Width, pcbImgRipCol.Height);
			Bitmap bn_cor = new Bitmap(pcbImgRipColCor.Width, pcbImgRipColCor.Height);

			using (Bitmap b = new Bitmap(1, EffectClasses.HDMA.Scanlines))
			using (Bitmap a = new Bitmap(1, EffectClasses.HDMA.Scanlines))
			{
				for (int y = 0; y < b.Height; y++)
				{
					Color c = ((Bitmap)pcbImgImgRip.Image).GetPixel(trbImgColRip.Value, y);
					b.SetPixel(0, y, c);
					a.SetPixel(0, y, c.Substract(pcbImgColCor.BackColor));
				}

				using (Graphics gr_a = Graphics.FromImage(bn))
				using (Graphics gr_b = Graphics.FromImage(bn_cor))
				{
					gr_a.FillRectangle(new TextureBrush(b, System.Drawing.Drawing2D.WrapMode.Tile), 0, 0, bn.Width, bn.Height);
					pcbImgRipCol.Image = bn;

					if (chbImgColCor.Checked)
					{
						gr_b.FillRectangle(new TextureBrush(a, System.Drawing.Drawing2D.WrapMode.Tile), 0, 0, bn_cor.Width, bn_cor.Height);
						pcbImgRipColCor.Image = bn_cor;

						_imgEffect.FromImage(bn_cor);
						_imgMath.FixedColor = _imgEffect.EffectImage;
					}
					else
					{
						_imgEffect.FromImage(bn);
						_imgMath.FixedColor = _imgEffect.EffectImage;
					}
				}
			}

			pcbImgMainPic.Image = _imgMath.GetScreen();
#if(DEBUG)
			Console.WriteLine("Image-Update: " + sw.ElapsedMilliseconds + "ms");
			sw.Stop();
#endif
		}

		public void UpdateImageCorrection()
		{
			trbImgCorRed.Value = pcbImgColCor.BackColor.R;
			trbImgCorGrn.Value = pcbImgColCor.BackColor.G;
			trbImgCorBlu.Value = pcbImgColCor.BackColor.B;

			txtImgCorRed.Text = trbImgCorRed.Value.ToString();
			txtImgCorGrn.Text = trbImgCorGrn.Value.ToString();
			txtImgCorBlu.Text = trbImgCorBlu.Value.ToString();

			if (pcbImgRipCol.Image == null || !chbImgColCor.Checked)
			{
				return;
			}

#if(DEBUG)
			Stopwatch sw = new Stopwatch();
			sw.Start();
#endif

			Bitmap bn_cor = new Bitmap(pcbImgRipColCor.Width, pcbImgRipColCor.Height);
			using (Bitmap b = new Bitmap(1, EffectClasses.HDMA.Scanlines))
			{
				for (int y = 0; y < b.Height; y++)
				{
					Color c = ((Bitmap)pcbImgRipCol.Image).GetPixel(0, y);
					b.SetPixel(0, y, c.Substract(pcbImgColCor.BackColor));
				}

				using (Graphics gr = Graphics.FromImage(bn_cor))
				{
					gr.FillRectangle(new TextureBrush(b, System.Drawing.Drawing2D.WrapMode.Tile), 0, 0, bn_cor.Width, bn_cor.Height);
					pcbImgRipColCor.Image = bn_cor;
				}
			}

			_imgEffect.FromImage(bn_cor);
			_imgMath.FixedColor = _imgEffect.EffectImage;
			pcbImgMainPic.Image = _imgMath.GetScreen();
#if(DEBUG)
			Console.WriteLine("Image-Correction-Update: " + sw.ElapsedMilliseconds + "ms");
			sw.Stop();
#endif
		}

		private void btnImgLoad_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Image files | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;

			pcbImgImgRip.SizeMode = PictureBoxSizeMode.StretchImage;
			pcbImgImgRip.Image = new Bitmap(Image.FromFile(ofd.FileName),
				new Size(256, EffectClasses.HDMA.Scanlines));
			UpdateImage();
		}

		private void trbImgColRip_Scroll(object sender, EventArgs e)
		{
			lblImgColRip.Text = trbImgColRip.Value.ToString("D3");
			if (pcbImgImgRip.Image == null)
				return;
			UpdateImage();
		}

		private void chbImgColCor_CheckedChanged(object sender, EventArgs e)
		{
			grpImgCor.Enabled = chbImgColCor.Checked;
			UpdateImage();
		}
		
		private void btnImgCod_Click(object sender, EventArgs e)
		{
            string code = "";
            if (_fg)
				code += _imgMath.Code() + "\n";
            else code += "init: \n";
            code += _imgEffect.Code(ChooseChannel.GetOneChannel, ChooseChannel.GetTwoChannel, ChooseChannel.GetThreeChannels);

			ShowCode.ShowCodeDialog(code);
		}

		private void trbImgCor_Scroll(object sender, EventArgs e)
		{
			pcbImgColCor.BackColor = Color.FromArgb(trbImgCorRed.Value, trbImgCorGrn.Value, trbImgCorBlu.Value);
			txtImgColCor.Text = "#" + trbImgCorRed.Value.ToString("X2") + trbImgCorGrn.Value.ToString("X2") + trbImgCorBlu.Value.ToString("X2");
			UpdateImageCorrection();
		}

		private void imgMath_Changed(object sender, EventArgs e)
		{
			SetMath(_imgMath, rdbImgSub.Checked, chbImgHlf.Checked, pcbImgMainPic);
		}
		#endregion
		#region Table

		private EffectClasses.ColorMath _tblMath = new EffectClasses.ColorMath();
		private EffectClasses.ColorHDMA _tblRed = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Red);
		private EffectClasses.ColorHDMA _tblGreen = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Green);
		private EffectClasses.ColorHDMA _tblBlue = new EffectClasses.ColorHDMA(EffectClasses.ColorHDMAValues.Blue);

		private void cmbTblScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _tblMath, sender, false);
			pcbTblMainPic.Image = _tblMath.GetScreen();
		}

		private void rtbTblRed_TextChanged(object sender, EventArgs e)
		{
			lsbTblRed.Items.Clear();
			lsbTblRed.Items.AddRange(_tblRed.FromString(rtbTblRed.Lines, !chbTblFivBit.Checked));
			_tblMath.FixedColor = EffectClasses.BitmapEffects.MergeImages_(
				_tblRed.EffectImage, _tblGreen.EffectImage, _tblBlue.EffectImage);
			pcbTblRed.Image = _tblRed.EffectImage;
			pcbTblMainPic.Image = _tblMath.GetScreen();
		}

		private void rtbTblGrn_TextChanged(object sender, EventArgs e)
		{
			lsbTblGrn.Items.Clear();
			lsbTblGrn.Items.AddRange(_tblGreen.FromString(rtbTblGrn.Lines, !chbTblFivBit.Checked));
			_tblMath.FixedColor = EffectClasses.BitmapEffects.MergeImages_(
				_tblRed.EffectImage, _tblGreen.EffectImage, _tblBlue.EffectImage);
			pcbTblGrn.Image = _tblGreen.EffectImage;
			pcbTblMainPic.Image = _tblMath.GetScreen();
		}

		private void rtbTblBlu_TextChanged(object sender, EventArgs e)
		{
			lsbTblBlu.Items.Clear();
			lsbTblBlu.Items.AddRange(_tblBlue.FromString(rtbTblBlu.Lines, !chbTblFivBit.Checked));
			_tblMath.FixedColor = EffectClasses.BitmapEffects.MergeImages_(
				_tblRed.EffectImage, _tblGreen.EffectImage, _tblBlue.EffectImage);
			pcbTblBlu.Image = _tblBlue.EffectImage;
			pcbTblMainPic.Image = _tblMath.GetScreen();
		}

		private void btnTblCod_Click(object sender, EventArgs e)
		{
            string code = "";
            if (_fg)
				code += _tblMath.Code() + "\n";
            else code += "init: \n";
            code += EffectClasses.FullColorHDMA.Code(_tblRed, _tblGreen, _tblBlue,
				ChooseChannel.GetOneChannel, ChooseChannel.GetTwoChannel,
				ChooseChannel.GetThreeChannels);

			ShowCode.ShowCodeDialog(code);
		}

		private void tblMath_Changed(object sender, EventArgs e)
		{
			SetMath(_tblMath, rdbTblSub.Checked, chbTblHlf.Checked, pcbTblMainPic);
		}
		#endregion
	}
}