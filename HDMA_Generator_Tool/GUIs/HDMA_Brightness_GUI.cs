using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Extansion.String;
using Extansion.Control;
using Extansion.Int;

namespace HDMA_Generator_Tool
{
	public partial class HDMA_Brightness_GUI : Form, IScreenshotUser
	{
		#region ITab

		public TabControl GetTabControl() { return tbc; }
		public void SetASMMode(ASMMode Mode)
		{
			switch (Mode)
			{
				case ASMMode.Standard:
					grpSngChnStd.Visible = true;
					grpIndChnStd.Visible = true;
					grpSngChnAdv.Visible = false;
					grpIndChnAdv.Visible = false;
					break;
				case ASMMode.Advanced:
				case ASMMode.Expert:
					grpSngChnStd.Visible = false;
					grpIndChnStd.Visible = false;
					grpSngChnAdv.Visible = true;
					grpIndChnAdv.Visible = true;
					break;
			}
		}
		public ComboBox[] ScreenSelectors { get; set; }
		public Bitmap GetScreen()
		{
			var tab = tbc.SelectedTab;
			if (tab == tbgSimple)
				return (Bitmap)pcbSmpMainPic.Image;
			if (tab == tbgIndividual)
				return (Bitmap)pcbIndMainPic.Image;
			if (tab == tbgTable)
				return (Bitmap)pcbTblMainPic.Image;
			return null;
		}

		#endregion
		#region IScreenshotUser

		public Bitmap[] ScreenshotsImages { get; private set; }

		#endregion
		
		public HDMA_Brightness_GUI()
		{
			InitializeComponent();

			ScreenshotsImages = new Bitmap[tbc.TabCount];

			ScreenSelectors = new ComboBox[]{
				cmbSmpScnSel,
				cmbIndScnSel,
				cmbTblScnSel,
			};

			cmbSmpChn.SelectedIndex = 3;
			cmbIndChn.SelectedIndex = 3;
			cmbTblChn.SelectedIndex = 3;

			#region ToolTips

			/*
			//toolTip.SetToolTip(pnl_BrightScreen__Simple_Grad, "Shows the Brighness Gradiant");
			//toolTip.SetToolTip(btn_Simple_Code, "Generates the code for the HDMA, which is to be inserted with levelASM or uberASM");
			toolTip.SetToolTip(btn_Individual_Code, "Generates the code for the HDMA, which is to be inserted with levelASM or uberASM");
			toolTip.SetToolTip(trb_Simple_Top, "Sets how far the gariand reaches down.\nTo even show a gradiant, it needs to have at least a value of 0x10 - StartWith");
			toolTip.SetToolTip(nUD_SimpleTop, "Sets how far the gariand reaches down.\nTo even show a gradiant, it needs to have at least a value of 0x10 - StartWith");
			toolTip.SetToolTip(trb_Simple_Bottom, "Sets how far the gariand reaches up.\nTo even show a gradiant, it needs to have at least a value of 0x10 - StartWith");
			toolTip.SetToolTip(nUD_SimpleBottom, "Sets how far the gariand reaches up.\nTo even show a gradiant, it needs to have at least a value of 0x10 - StartWith");
			toolTip.SetToolTip(txtBrightTop, "Sets the brightness the gradiant starts with. 0 is total darkness, whereas F is normal brightness");
			toolTip.SetToolTip(txtBrightTop, "Sets the brightness the gradiant starts with. 0 is total darkness, whereas F is normal brightness");
			toolTip.SetToolTip(chbUseTop, "Enables the gradiant for the top of the screen");
			toolTip.SetToolTip(chbUseBottom, "Enables the gradiant for the bottom of the screen");
			//toolTip.SetToolTip(btnSimple_Load, "Loads a ZSNES screenshot which needs to be 224 pixels high and 256 pixels wide");
			toolTip.SetToolTip(rdbCH3, "Sets the generated HDMA code to use HDMA channel 3 ($433x)");
			toolTip.SetToolTip(rdbCH4, "Sets the generated HDMA code to use HDMA channel 4 ($434x)");
			toolTip.SetToolTip(rdbCH5, "Sets the generated HDMA code to use HDMA channel 5 ($435x)");

			toolTip.SetToolTip(rdb_Indi_CH3, "Sets the generated HDMA code to use HDMA channel 3 ($433x)");
			toolTip.SetToolTip(rdb_Indi_CH4, "Sets the generated HDMA code to use HDMA channel 4 ($434x)");
			toolTip.SetToolTip(rdb_Indi_CH5, "Sets the generated HDMA code to use HDMA channel 5 ($435x)");
			*/
			#endregion
			
			
		}
		
		#region Simple

		private EffectClasses.ColorMath _smpMath = new EffectClasses.ColorMath();
		private EffectClasses.BrightnessHDMA _smpEffectTop = new EffectClasses.BrightnessHDMA();
		private EffectClasses.BrightnessHDMA _smpEffectBtm = new EffectClasses.BrightnessHDMA();
		private EffectClasses.ColorPositionCollection _collectionTop = new EffectClasses.ColorPositionCollection();
		private EffectClasses.ColorPositionCollection _collectionBtm = new EffectClasses.ColorPositionCollection();

		public void UpdateSingle()
		{
			int top = 0, btm = 0;

			if (!Int32.TryParse(txtSmpTopStr.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out top))
				top = 0;
			if (!Int32.TryParse(txtSmpBtmStr.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out btm))
				btm = 0;
			
			_collectionTop.Clear();
			if ((int)trbSmpTopRea.Value != EffectClasses.HDMA.Scanlines)
				_collectionTop.Add(new EffectClasses.ColorPosition(0, (byte)top));
			_collectionTop.Add(new EffectClasses.ColorPosition(EffectClasses.HDMA.Scanlines - trbSmpTopRea.Value, 0xF));
			_smpEffectTop.DarknessPositions = _collectionTop;

			_collectionBtm.Clear();
			if ((int)trbSmpBtmRea.Value != 0)
				_collectionBtm.Add(new EffectClasses.ColorPosition(EffectClasses.HDMA.Scanlines, (byte)btm));
			_collectionBtm.Add(new EffectClasses.ColorPosition(EffectClasses.HDMA.Scanlines - trbSmpBtmRea.Value, 0xF));
			_smpEffectBtm.DarknessPositions = _collectionBtm;

			UpdateSimpleScreen();
		}

		public void UpdateSimpleScreen()
		{
			pcbSmpMainPic.Image = EffectClasses.BitmapEffects.OverlapImages(
				_smpEffectTop.EffectImage, _smpEffectBtm.EffectImage, _smpMath.GetScreen());
		}

		private void cmbSmpScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _smpMath, sender);
			UpdateSimpleScreen();
		}

		private void trbSmpReaTop_Scroll(object sender, EventArgs e)
		{
			nudSmpTopRea.Value = EffectClasses.HDMA.Scanlines - trbSmpTopRea.Value;
		}

		private void nudSmpTopRea_ValueChanged(object sender, EventArgs e)
		{
			trbSmpTopRea.Value = EffectClasses.HDMA.Scanlines - (int)nudSmpTopRea.Value;
			UpdateSingle();
		}

		private void trbSmpBtmRea_Scroll(object sender, EventArgs e)
		{
			nudSmpBtmRea.Value = trbSmpBtmRea.Value;
		}

		private void nudSmpBtmRea_ValueChanged(object sender, EventArgs e)
		{
			trbSmpBtmRea.Value = (int)nudSmpBtmRea.Value;
			UpdateSingle();
		}

		private void txtSmp_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b' && e.KeyChar != '\n')
				e.Handled = !System.Uri.IsHexDigit(e.KeyChar);
		}

		private void btnSmpCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(EffectClasses.BrightnessHDMA.MultiCode(_smpEffectTop, _smpEffectBtm));
		}

		private void rdbSmpCh_CheckedChanged(object sender, EventArgs e)
		{
			_smpEffectTop.SetChannel(rdbSmpCh3, rdbSmpCh4, rdbSmpCh5);
			_smpEffectBtm.SetChannel(rdbSmpCh3, rdbSmpCh4, rdbSmpCh5);
		}

		private void cmbSmpChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_smpEffectTop.SetChannel((ComboBox)sender);
			_smpEffectBtm.SetChannel((ComboBox)sender);
		}

		private void txtSmp_TextChanged(object sender, EventArgs e)
		{
			UpdateSingle();
		}

		#endregion
		#region Individual

		private EffectClasses.ColorMath _indMath = new EffectClasses.ColorMath();
		private EffectClasses.BrightnessHDMA _indEffect = new EffectClasses.BrightnessHDMA();

		public void UpdateIndividual()
		{
			EffectClasses.HDMATable table = new EffectClasses.HDMATable();
			for(int i = 0; i < lsbIndEnt.Items.Count; i++)
				table.Add((EffectClasses.HDMATableEntry)lsbIndEnt.Items[i]);

			if (chbIndEnd.Checked && table.TotalScanlines < EffectClasses.HDMA.Scanlines)
				table.Add(new EffectClasses.HDMATableEntry(EffectClasses.TableValueType.db, 1, 0xF));

			_indEffect.FromTable(table);

			pcbIndMainPic.Image = EffectClasses.BitmapEffects.OverlapImages(_indEffect.EffectImage, _indMath.GetScreen());
		}

		private void txtIndBri_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.KeyChar = Char.ToUpper(e.KeyChar);
			if(e.KeyChar == '%')
				e.Handled = !txtIndBri.Text.IsDigit();

			if (e.KeyChar != '\b' && e.KeyChar != '\n' && e.KeyChar != '%')
				e.Handled = !System.Uri.IsHexDigit(e.KeyChar);
		}

		private void btnIndAdd_Click(object sender, EventArgs e)
		{
			try
			{
				int bright = 0;
				string text = txtIndBri.Text;
				if (text.EndsWith("%"))
				{
					if (!Int32.TryParse(text.Substring(0, text.IndexOf('%')), out bright))
						bright = 0;
					bright = (int)Math.Round(bright / 6.6666);
				}
				else
				{
					if (!Int32.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out bright))
						bright = 0;
				}

				EffectClasses.HDMATableEntry entry = new EffectClasses.HDMATableEntry(
					EffectClasses.TableValueType.db, (byte)nudIndLin.Value, (byte)bright.Range(0, 15));

				lsbIndEnt.Items.Add(entry);
				UpdateIndividual();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Something went Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnIndRmv_Click(object sender, EventArgs e)
		{
			if (lsbIndEnt.SelectedIndex == -1)
			{
				Settings.MissingSelectionMessage();
				return;
			}
			lsbIndEnt.Items.RemoveAt(lsbIndEnt.SelectedIndex);
			UpdateIndividual();
		}

		private void btnIndClr_Click(object sender, EventArgs e)
		{
			if (Settings.DeleteAllMessage())
				lsbIndEnt.Items.Clear();
			UpdateIndividual();
		}

		private void btnIndUp_Click(object sender, EventArgs e)
		{
			if (lsbIndEnt.SelectedIndex == -1)
			{
				Settings.MissingSelectionMessage();
				return;
			}
			lsbIndEnt.MoveItemUp();
			UpdateIndividual();
		}

		private void btnIndDwn_Click(object sender, EventArgs e)
		{
			if (lsbIndEnt.SelectedIndex == -1)
			{
				Settings.MissingSelectionMessage();
				return;
			}
			lsbIndEnt.MoveItemDown();
			UpdateIndividual();
		}

		private void cmbIndScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _indMath, sender);
			UpdateIndividual();
		}

		private void rdbIndChn_CheckedChanged(object sender, EventArgs e)
		{
			_indEffect.SetChannel(rdbIndCh3, rdbIndCh4, rdbIndCh5);
		}
		private void cmbIndChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_indEffect.SetChannel((ComboBox)sender);
		}

		private void btnIndCod_Click(object sender, EventArgs e)
		{
			UpdateIndividual();
			ShowCode.ShowCodeDialog(_indEffect);
		}

		#endregion
		#region Table

		private EffectClasses.ColorMath _tblMath = new EffectClasses.ColorMath();
		private EffectClasses.BrightnessHDMA _tblEffect = new EffectClasses.BrightnessHDMA();

		private void cmbTblScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 1, _tblMath, sender);
			pcbTblMainPic.Image = EffectClasses.BitmapEffects.OverlapImages(_tblEffect.EffectImage, _tblMath.GetScreen());
		}

		private void rtbTblEnt_TextChanged(object sender, EventArgs e)
		{
			lsbTblWrn.Items.Clear();
			lsbTblWrn.Items.AddRange(_tblEffect.FromString(rtbTblEnt.Lines));
			pcbTblMainPic.Image = EffectClasses.BitmapEffects.OverlapImages(_tblEffect.EffectImage, _tblMath.GetScreen());			
		}

		private void rdbTbl_CheckedChanged(object sender, EventArgs e)
		{
			_tblEffect.SetChannel(rdbTblCh3, rdbTblCh4, rdbTblCh5);
		}

		private void cmbTblChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_tblEffect.SetChannel((ComboBox)sender);
		}
		private void btnTblCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_tblEffect);
		}
		#endregion
	}
}
