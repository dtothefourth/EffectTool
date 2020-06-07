using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Extansion.Drawing;

namespace HDMA_Generator_Tool
{
	public enum ImageType { Color, Transparent, Outline }

	public partial class HDMA_Circle_GUI : Form, ITab
	{
		#region ITab Interface

		public TabControl GetTabControl() { return tbc; }
		public void SetASMMode(ASMMode Mode)
		{
		}
		public ComboBox[] ScreenSelectors { get; set; }
		public Bitmap GetScreen() { return null; }

		#endregion

		private bool StartUpFinished = false;
		private Window_HDMA Circle = new Window_HDMA();
		private DynamicWindow_HDMA Dynamic_Circle = new DynamicWindow_HDMA();

		public HDMA_Circle_GUI()
		{
			InitializeComponent();
			tbc.TabPages.RemoveByKey("tpgStatic");

			foreach (ColorWindow C in Enum.GetValues(typeof(ColorWindow)))
			{
				cmbStatic_Black.Items.Add(C);
				cmbStatic_Math.Items.Add(C);

				cmbDynamic_Black.Items.Add(C);
				cmbDynamic_Math.Items.Add(C);

				cmbImage_Black.Items.Add(C);
				cmbImage_Math.Items.Add(C);
			}
			cmbStatic_Black.SelectedIndex = 0;
			cmbStatic_Math.SelectedIndex = 2;
			cmbDynamic_Black.SelectedIndex = 0;
			cmbDynamic_Math.SelectedIndex = 2;
			cmbImage_Black.SelectedIndex = 0;
			cmbImage_Math.SelectedIndex = 2;

			foreach (ImageType IT in Enum.GetValues(typeof(ImageType)))
				cmbImage_Type.Items.Add(IT);
			cmbImage_Type.SelectedIndex = 2;

			UpdateStaticCircle();
			UpdateDynamicCircle();
			StartUpFinished = true;
		}

		/// <summary>
		/// Setzt den Trackbar auf den Wert der TextBox wenn Enter gedrückt wird
		/// Methode sollte in dem KeyDown Event aufgerufen werden.
		/// </summary>
		/// <param name="txt">TextBox, die übertragen werden soll, meinstens der sender</param>
		/// <param name="trb">TrackBar auf den der Wert übertragen werden soll</param>
		/// <param name="e"></param>
		private void SetTrackBar(TextBox txt, TrackBar trb, KeyEventArgs e)
		{
			if (e.KeyCode.IsLetter())
			{
				e.SuppressKeyPress = false;
				return;
			}
			if (e.KeyCode != Keys.Enter)
				return;

			int Val;
			if (Int32.TryParse(txt.Text, out Val) && Val <= trb.Maximum)
				trb.Value = Val;
			else
				txt.Text = trb.Value.ToString();
		}

		/// <summary>
		/// Updated den Kreis und die Werte in der Klasse
		/// </summary>
		private void UpdateStaticCircle()
		{
			Circle.DrawMask(trbStatic_Radius.Value, trbStatic_Xpos.Value, trbStatic_Ypos.Maximum - trbStatic_Ypos.Value);
			pcbStatic.Image = Circle.Draw();
			pcbStatic.Update();
		}

		private void UpdateDynamicCircle()
		{
			Dynamic_Circle.DrawMask(trbDynamic_Radius.Value, trbDynamic_Xpos.Value, trbDynamic_Ypos.Maximum - trbDynamic_Ypos.Value);
			pcbDynamic.Image = Dynamic_Circle.Draw();
			pcbDynamic.Update();
		}

		//-------------------------------------------------------------------------------------------------------------------------------

		#region Static Circle
		private void trbradius_Scroll(object sender, EventArgs e)
		{
			txtStatic_radius.Text = trbStatic_Radius.Value.ToString();
			UpdateStaticCircle();
		}        

		private void txtradius_KeyDown(object sender, KeyEventArgs e) 
		{
			SetTrackBar((TextBox)sender, trbStatic_Radius, e);
			UpdateStaticCircle();
		}

		private void trbXpos_Scroll(object sender, EventArgs e)
		{
			txtStatic_Xpos.Text = trbStatic_Xpos.Value.ToString();
			UpdateStaticCircle();
		}

		private void txtXpos_KeyDown(object sender, KeyEventArgs e)
		{
			SetTrackBar((TextBox)sender, trbStatic_Xpos, e);
			UpdateStaticCircle();
		}

		private void trbYpos_Scroll(object sender, EventArgs e)
		{
			txtStatic_Ypos.Text = trbStatic_Ypos.Value.ToString();
			UpdateStaticCircle();
		}

		private void txtYpos_KeyDown(object sender, KeyEventArgs e)
		{
			SetTrackBar((TextBox)sender, trbStatic_Ypos, e);
			UpdateStaticCircle();
		}

		private void btnSimple_Load_Click(object sender, EventArgs e)
		{
			Screenshot.Load(im => { pnlCircle.BackgroundImage = im; },
				UpdateStaticCircle);
		}

		private void chbInvert_CheckedChanged(object sender, EventArgs e)
		{
			Circle.Inverted = chbStatic_Invert.Checked;
			UpdateStaticCircle();
		}

		private void StaticWindow_CheckedChanged(object sender, EventArgs e)
		{
			Circle.WindowToUse = rdbStatic_Window1.Checked ? WindowUse.Window1 : WindowUse.Window2;
		}

		private void rdbChanel_static_CheckedChanged(object sender, EventArgs e)
		{
			Circle.SetChannel(rdbStatic_CH3, rdbStatic_CH4, rdbStatic_CH5);
		}

		private void btnStatic_Code_Click(object sender, EventArgs e)
		{
			try { new ShowCode(Circle.Code()).ShowDialog(); }
			catch (FormatException FE) { ShowCode.ShowMessage(FE); }
			catch (Exception Ex) { ShowCode.ShowMessage(Ex); }
		}

		private void chbStatic_Color_CheckedChanged(object sender, EventArgs e)
		{
			grbStatic_BGs.Enabled = !chbStatic_Color.Checked;
			grbStatic_ColSet.Enabled = chbStatic_Color.Checked;
			StaticBG_CheckedChanged(sender, e);
		}

		private void StaticBG_CheckedChanged(object sender, EventArgs e)
		{
			if (!chbStatic_Color.Checked)
			{
				WindowBG BG = 0;
				if (chbStatic_BG1.Checked) BG |= WindowBG.BG1;
				if (chbStatic_BG2.Checked) BG |= WindowBG.BG2;
				if (chbStatic_BG3.Checked) BG |= WindowBG.BG3;
				if (chbStatic_BG4.Checked) BG |= WindowBG.BG4;
				if (chbStatic_Sprites.Checked) BG |= WindowBG.OBJ;
				Circle.BGToHide = BG;
			}
			else
				Circle.BGToHide = WindowBG.Color | WindowBG.BG1 | WindowBG.BG2 | WindowBG.BG3 | WindowBG.OBJ;
			UpdateStaticCircle();
		}

		private void ColorSettingChange(object sender, EventArgs e)
		{
			if (StartUpFinished)
			{
				Circle.BlackMain = (ColorWindow)cmbStatic_Black.SelectedItem;
				Circle.ColorMath = (ColorWindow)cmbStatic_Math.SelectedItem;
				Circle.UseSubscreen = chbStatic_UseSub.Checked;
				Circle.DirectColorMode = chbStatic_256Col.Checked;
			}
		}
		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------

		#region Dynamic Circle
		private void trbDynamic_Radius_Scroll(object sender, EventArgs e)
		{
			txtDynamic_Radius.Text = trbDynamic_Radius.Value.ToString();
			UpdateDynamicCircle();
		}

		private void txtDynamic_Radius_KeyDown(object sender, KeyEventArgs e)
		{
			SetTrackBar((TextBox)sender, trbDynamic_Radius, e);
			UpdateDynamicCircle();
		}

		private void trbDynamic_Xpos_Scroll(object sender, EventArgs e)
		{
			txtDynamic_Xpos.Text = trbDynamic_Xpos.Value.ToString();
			UpdateDynamicCircle();
		}

		private void txtDynamic_Xpos_KeyDown(object sender, KeyEventArgs e)
		{
			SetTrackBar((TextBox)sender, trbDynamic_Xpos, e);
			UpdateDynamicCircle();
		}

		private void trbDynamic_Ypos_Scroll(object sender, EventArgs e)
		{
			txtDynamic_Ypos.Text = trbDynamic_Ypos.Value.ToString();
			UpdateDynamicCircle();
		}

		private void txtDynamic_Ypos_KeyDown(object sender, KeyEventArgs e)
		{
			SetTrackBar((TextBox)sender, trbDynamic_Ypos, e);
			UpdateDynamicCircle();
		}

		private void FollowMario_CheckedChanged(object sender, EventArgs e)
		{
			Dynamic_Circle.DynamicX = chbDynamic_Xpos.Checked;
			Dynamic_Circle.DynamicY = chbDynamic_Ypos.Checked;
			trbDynamic_Xpos.Enabled = !chbDynamic_Xpos.Checked;
			txtDynamic_Xpos.Enabled = !chbDynamic_Xpos.Checked;
			trbDynamic_Ypos.Enabled = !chbDynamic_Ypos.Checked;
			txtDynamic_Ypos.Enabled = !chbDynamic_Ypos.Checked;
			UpdateDynamicCircle();
		}

		private void DynamicWindow_CheckedChanged(object sender, EventArgs e)
		{
			Dynamic_Circle.WindowToUse = rdbDynamic_Window1.Checked ? WindowUse.Window1 : WindowUse.Window2;
		}

		private void rdbChannel_dynamic_CheckedChanged(object sender, EventArgs e)
		{
			Dynamic_Circle.SetChannel(rdbDynamic_CH3, rdbDynamic_CH4, rdbDynamic_CH5);
		}

		private void DynamicBG_CheckedChanged(object sender, EventArgs e)
		{
			if (!chbDynamic_Colors.Checked)
			{
				WindowBG BG = 0;
				if (chbDynamic_BG1.Checked) BG |= WindowBG.BG1;
				if (chbDynamic_BG2.Checked) BG |= WindowBG.BG2;
				if (chbDynamic_BG3.Checked) BG |= WindowBG.BG3;
				if (chbDynamic_BG4.Checked) BG |= WindowBG.BG4;
				if (chbDynamic_Sprites.Checked) BG |= WindowBG.OBJ;
				Dynamic_Circle.BGToHide = BG;
			}
			else
				Dynamic_Circle.BGToHide = WindowBG.Color | WindowBG.BG1 | WindowBG.BG2 | WindowBG.BG3 | WindowBG.OBJ;
			UpdateDynamicCircle();
		}

		private void Dynamic_ColorSettingsChange(object sender, EventArgs e)
		{
			if (StartUpFinished)
			{
				Dynamic_Circle.BlackMain = (ColorWindow)cmbDynamic_Black.SelectedItem;
				Dynamic_Circle.ColorMath = (ColorWindow)cmbDynamic_Math.SelectedItem;
				Dynamic_Circle.UseSubscreen = chbDynamic_UseSub.Checked;
				Dynamic_Circle.DirectColorMode = chbDynamic_256Col.Checked;
			}
		}

		private void chbDynamic_Colors_CheckedChanged(object sender, EventArgs e)
		{
			grbDynamic_BGs.Enabled = !chbDynamic_Colors.Checked;
			grbDynamic_ColorSetting.Enabled = chbDynamic_Colors.Checked;
			DynamicBG_CheckedChanged(sender, e);
		}

		private void btnDynamic_Code_Click(object sender, EventArgs e)
		{
			try { new ShowCode(Dynamic_Circle.Code()).ShowDialog(); }
			catch (FormatException FE) { ShowCode.ShowMessage(FE); }
			catch (Exception Ex) { ShowCode.ShowMessage(Ex); }
		}

		private void txtFreeRAM_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				return;
			try { Dynamic_Circle.FreeRAM = Convert.ToInt32(txtFreeRAM.Text, 16); }
			catch { txtFreeRAM.Text = Dynamic_Circle.FreeRAM.ToString("X6"); }
		}
		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------

		private MultiWindow_HDMA Multi = new MultiWindow_HDMA();

		private void UpdateMulti()
		{
			Multi.Mask = new FastBitmap(new Bitmap(pcbAfter.Image));
			lblNoEffekt.Visible = !Multi.UseAble;
			pcbImage.Image = Multi.Draw();
			pcbImage.Update();
		}

		private void RunPropperImageFilter()
		{
			Bitmap Mask = new Bitmap(pcbOriginal.Image, new Size(256, HDMA.Scanlines));
			float Value = (float)trbBlackWhite.Value / 10f;

			switch((ImageType)cmbImage_Type.SelectedItem)
			{
				case ImageType.Outline:
					
					FastBitmap OutlineMask = new FastBitmap(Mask);
					OutlineStart start = 0;

					if (chbImage_TopLeft.Checked)
						start |= OutlineStart.TopLeft;
					if (chbImage_TopRight.Checked)
						start |= OutlineStart.TopRight;
					if (chbImage_BottomLeft.Checked)
						start |= OutlineStart.BottomLeft;
					if (chbImage_BottomRight.Checked)
						start |= OutlineStart.BottomRight;
					OutlineMask.Outline(start, trbBlackWhite.Value);
					Value = 1f;

					Mask = OutlineMask;
					goto case ImageType.Transparent;

				case ImageType.Transparent:
					FastBitmap FastMask = new FastBitmap(Mask);
					FastBitmap BlackBase = new FastBitmap(EffectClasses.BitmapEffects.FromColor(Color.Black, 256, 224));
					for (int y = 0; y < 224; y++)
						for (int x = 0; x < 256; x++)
							if (FastMask.GetPixel(x, y).A <= (255 - (Value * 255f)))
								BlackBase.SetPixel(x, y, Color.White);
					if (chbImage_Invert.Checked)
						pcbAfter.Image = ((Bitmap)BlackBase).Invert();
					else
						pcbAfter.Image = ((Bitmap)BlackBase);
					break;

				case ImageType.Color:
					if (chbImage_Invert.Checked)
						pcbAfter.Image = Mask.BlackWhite(Value).Invert();
					else
						pcbAfter.Image = Mask.BlackWhite(Value);
					break;
			}



			UpdateMulti();
		}

		private void btnImage_LoadImage_Click(object sender, EventArgs e)
		{
			OpenFileDialog OFD = new OpenFileDialog();
			OFD.Filter = "Image Files|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tiff|All files|*.*";
			OFD.Title = "Load Mask";

			if (OFD.ShowDialog() == DialogResult.Cancel)
				return;

			try { pcbOriginal.Image = Image.FromFile(OFD.FileName); }
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			RunPropperImageFilter();
		}


		private void trbBlackWhite_Scroll(object sender, EventArgs e)
		{
			if (cmbImage_Type.SelectedIndex != 2)
			{
				float Value = (float)trbBlackWhite.Value / 10f;
				lblBlackWhite.Text = Value.ToString("0.0");
			}
			else
				lblBlackWhite.Text = trbBlackWhite.Value + "%";
			RunPropperImageFilter();
		}

		private void cmbImage_Type_SelectedIndexChanged(object sender, EventArgs e)
		{
			grbImage_Corners.Enabled = cmbImage_Type.SelectedIndex == 2;
			//trbBlackWhite.Enabled = !(cmbImage_Type.SelectedIndex == 2);
			/*
			if (cmbImage_Type.SelectedIndex != 2)
			{
				trbBlackWhite.Minimum = 0;
				trbBlackWhite.Maximum = 10;
			}
			else
			{
				trbBlackWhite.Minimum = 0;
				trbBlackWhite.Maximum = 100;
			}*/
			trbBlackWhite_Scroll(sender, e);
		}

		private void rdbChannel_Image_CheckedChanged(object sender, EventArgs e)
		{
			Multi.SetChannel(rdbImage_CH3, rdbImage_CH4, rdbImage_CH5);
		}

		private void ImageBG_CheckedChanged(object sender, EventArgs e)
		{
			if (!chbImage_Color.Checked)
			{
				WindowBG BG = 0;
				if (chbImage_BG1.Checked) BG |= WindowBG.BG1;
				if (chbImage_BG2.Checked) BG |= WindowBG.BG2;
				if (chbImage_BG3.Checked) BG |= WindowBG.BG3;
				if (chbImage_BG4.Checked) BG |= WindowBG.BG4;
				if (chbImage_Sprites.Checked) BG |= WindowBG.OBJ;
				Multi.BGToHide = BG;
			}
			else
				Multi.BGToHide = WindowBG.Color | WindowBG.BG1 | WindowBG.BG2 | WindowBG.BG3 | WindowBG.OBJ;
			UpdateMulti();
		}

		private void chbImage_Color_CheckedChanged(object sender, EventArgs e)
		{
			grbImage_BGs.Enabled = !chbImage_Color.Checked;
			ImageBG_CheckedChanged(sender, e);
		}

		private void btnImage_Code_Click(object sender, EventArgs e)
		{
			try 
			{
				if (!Multi.UseAble)
					throw new FormatException("Effect cannot be created from image!\n" +
						"The scanlines: " + String.Join(", ", Multi.BlackLines) + " " +
						"would need 3 black sections, and the scanlines: " +
						String.Join(", ", Multi.WhiteLines) + " at least 3 white ones.\n" +
						"Both of which cannot be done using only 2 windows without further HDMA (which this tool doesn't implement)");
				else
					new ShowCode(Multi.Code()).ShowDialog();
			}
			catch (FormatException FE) { ShowCode.ShowMessage(FE); }
			catch (Exception Ex) { ShowCode.ShowMessage(Ex); }
		}

		private void Image_ColorSettingChanged(object sender, EventArgs e)
		{
			if (StartUpFinished)
			{
				Multi.BlackMain = (ColorWindow)cmbImage_Black.SelectedItem;
				Multi.ColorMath = (ColorWindow)cmbImage_Math.SelectedItem;
				Multi.UseSubscreen = chbImage_UseSub.Checked;
				Multi.DirectColorMode = chbImage_256Col.Checked;
			}
		}

		private void chbImage_CornerChange(object sender, EventArgs e)
		{
			RunPropperImageFilter();
		}

		private void chbDynamic_Invert_CheckedChanged(object sender, EventArgs e)
		{
			Dynamic_Circle.Inverted = chbDynamic_Invert.Checked;
			UpdateDynamicCircle();
		}
	}
}
