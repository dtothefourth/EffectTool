using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace HDMA_Generator_Tool
{
	public partial class HDMA_Mosaic_GUI : Form, ITab
	{
		#region ITab Interface

		public ComboBox[] ScreenSelectors { get; set; }

		public TabControl GetTabControl()
		{
			return tbc;
		}

		public void SetASMMode(ASMMode Mode)
		{
			switch (Mode)
			{
				default:
				case ASMMode.Standard:
					grpLinChnStd.Visible = true;
					grpLinChnAdv.Visible = false;
					break;
				case ASMMode.Advanced:
				case ASMMode.Expert:
					grpLinChnStd.Visible = false;
					grpLinChnAdv.Visible = true;
					break;
			}
		}
		public Bitmap GetScreen()
		{
			var tab = tbc.SelectedTab;
			if (tab == tabPage1)
				return (Bitmap)pcbWhlMainPic.Image;
			if (tab == tabPage2)
				return (Bitmap)pcbLinMainPic.Image;
			return null;
		}

		#endregion
		#region IScreenshotUser
		public Bitmap[] ScreenshotsImages { get; private set; }
		#endregion

		private EffectClasses.PixelationHDMA _whole = new EffectClasses.PixelationHDMA();
		private EffectClasses.ColorMath _wholeMath = new EffectClasses.ColorMath();
		private EffectClasses.PixelationHDMA _line = new EffectClasses.PixelationHDMA();
		private EffectClasses.ColorMath _lineMath = new EffectClasses.ColorMath();

		/// <summary>
		/// default constructor
		/// </summary>
		public HDMA_Mosaic_GUI()
		{
			InitializeComponent();

			ScreenshotsImages = new Bitmap[tbc.TabCount];

			//ITab interface
			ScreenSelectors = new ComboBox[]
			{
				cmbWhlScnSel,
				cmbLinScnSel,
			};

			cmbLinChn.SelectedIndex = 3;
			dgvLinVal.Columns[0].Width = 60;
			dgvLinVal.Columns[1].Width = 40;

			grpLinChnAdv.Location = grpLinChnStd.Location;
		}

		#region Whole Tab

		/// <summary>
		/// Update methode for the "Whole" Tab
		/// </summary>
		private void UpdateWhole()
		{
			EffectClasses.PixelationBGs bgs = 0;
			if (chbWhlBg1.Checked)
				bgs |= EffectClasses.PixelationBGs.BG1;
			if (chbWhlBg2.Checked)
				bgs |= EffectClasses.PixelationBGs.BG2;
			if (chbWhlBg3.Checked)
				bgs |= EffectClasses.PixelationBGs.BG3;
			if (chbWhlBg4.Checked)
				bgs |= EffectClasses.PixelationBGs.BG4;

			_whole.Values.Clear();
			_whole.Values.Add(new EffectClasses.PixelScanline(EffectClasses.HDMA.Scanlines, (byte)trbWhlPix.Value, bgs));

			pcbWhlMainPic.Image = _whole.EffectImage;
		}

		private void trbWhlPix_Scroll(object sender, EventArgs e)
		{
			int pixel = trbWhlPix.Value;
			lblWhlPix.Text = pixel + " x " + pixel;
			UpdateWhole();
		}

		private void cmbWhlScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(_wholeMath, sender);
			foreach (Bitmap b in _whole.OriginalImages)
				if(b != null)
					b.Dispose();
			_whole.OriginalImages = (EffectClasses.BitmapCollection)_wholeMath.Collection.Clone();
			_whole.ColorMath.FixedColor = _wholeMath.FixedColor;
			UpdateWhole();
		}

		private void chbWhlBGs_CheckedChanged(object sender, EventArgs e)
		{
			UpdateWhole();
		}
		
		private void btnWhlCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_whole.Code());
		}

		#endregion

		#region Lines Tab

		private void UpdateLine()
		{
			_line.Values.Clear();

			foreach (DataGridViewRow row in dgvLinVal.Rows)
			{
				byte scan = Convert.ToByte(row.Cells["colScanline"].Value);

				Match pixelstr = Regex.Match(row.Cells["colPixel"].Value.ToString(), @"(?<VAL>[\d]*)x[\d]*");
				byte pixel = Convert.ToByte(pixelstr.Groups["VAL"].Value);

				EffectClasses.PixelationBGs bgs = 0;
				string bgsstring = row.Cells["colBG"].Value.ToString();
				if (bgsstring.Contains(EffectClasses.PixelationBGs.BG1.ToString()))
					bgs |= EffectClasses.PixelationBGs.BG1;
				if (bgsstring.Contains(EffectClasses.PixelationBGs.BG2.ToString()))
					bgs |= EffectClasses.PixelationBGs.BG2;
				if (bgsstring.Contains(EffectClasses.PixelationBGs.BG3.ToString()))
					bgs |= EffectClasses.PixelationBGs.BG3;
				if (bgsstring.Contains(EffectClasses.PixelationBGs.BG4.ToString()))
					bgs |= EffectClasses.PixelationBGs.BG4;

				_line.Values.Add(new EffectClasses.PixelScanline(scan, pixel, bgs));
			}

			pcbLinMainPic.Image = _line.EffectImage;
		}

		private void btnLinNew_Click(object sender, EventArgs e)
		{
			dgvLinVal.Rows.Add("10", "3x3", (EffectClasses.PixelationBGs.BG1 | EffectClasses.PixelationBGs.BG2).ToString());
			UpdateLine();
		}
		private void btnLinDel_Click(object sender, EventArgs e)
		{
			if (dgvLinVal.SelectedRows.Count == 0)
				return;
			dgvLinVal.Rows.Remove(dgvLinVal.SelectedRows[0]);
			UpdateLine();
		}

		private void cmbLinScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(_lineMath, sender);
			foreach (Bitmap b in _line.OriginalImages)
				if(b != null)
					b.Dispose();
			_line.OriginalImages = (EffectClasses.BitmapCollection)_lineMath.Collection.Clone();
			_line.ColorMath.FixedColor = _lineMath.FixedColor;
			UpdateLine();
		}

		private void dgvLinVal_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			grpLinCur.Enabled = true;
		}

		private void dgvLinVal_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			grpLinCur.Enabled = dgvLinVal.Rows.Count != 0;
		}

		private void dgvLinVal_SelectionChanged(object sender, EventArgs e)
		{
			if (dgvLinVal.SelectedRows.Count == 0)
				return;
			var row = dgvLinVal.SelectedRows[0];

			nudLinScnLin.Value = Convert.ToByte(row.Cells["colScanline"].Value);

			Match pixelstr = Regex.Match(row.Cells["colPixel"].Value.ToString(), @"(?<VAL>[\d]*)x[\d]*");
			trbLinPix.Value = Convert.ToByte(pixelstr.Groups["VAL"].Value);

			string bgsstring = row.Cells["colBG"].Value.ToString();
			chbLinBg1.Checked = bgsstring.Contains(EffectClasses.PixelationBGs.BG1.ToString());
			chbLinBg1.Checked = bgsstring.Contains(EffectClasses.PixelationBGs.BG1.ToString());
			chbLinBg1.Checked = bgsstring.Contains(EffectClasses.PixelationBGs.BG1.ToString());
			chbLinBg1.Checked = bgsstring.Contains(EffectClasses.PixelationBGs.BG1.ToString());
		}

		private void trbLinPix_Scroll(object sender, EventArgs e)
		{
			if (dgvLinVal.SelectedRows.Count == 0)
				return;
			int val = ((TrackBar)sender).Value;
			dgvLinVal.SelectedRows[0].Cells["colPixel"].Value = val + "x" + val;
			UpdateLine();
		}

		private void nudLinScnLin_ValueChanged(object sender, EventArgs e)
		{
			dgvLinVal.SelectedRows[0].Cells["colScanline"].Value = ((NumericUpDown)sender).Value;
			UpdateLine();
		}
		
		private void chbLinBGs_CheckedChanged(object sender, EventArgs e)
		{
			EffectClasses.PixelationBGs bgs = 0;
			if (chbLinBg1.Checked)
				bgs |= EffectClasses.PixelationBGs.BG1;
			if (chbLinBg2.Checked)
				bgs |= EffectClasses.PixelationBGs.BG2;
			if (chbLinBg3.Checked)
				bgs |= EffectClasses.PixelationBGs.BG3;
			if (chbLinBg4.Checked)
				bgs |= EffectClasses.PixelationBGs.BG4;

			dgvLinVal.SelectedRows[0].Cells["colBG"].Value = bgs.ToString();
			UpdateLine();
		}

		private void btnLinUp_Click(object sender, EventArgs e)
		{
			if (dgvLinVal.RowCount > 0)
			{
				if (dgvLinVal.SelectedRows.Count > 0)
				{
					int rowCount = dgvLinVal.Rows.Count;
					int index = dgvLinVal.SelectedCells[0].OwningRow.Index;

					if (index == 0)
					{
						return;
					}
					DataGridViewRowCollection rows = dgvLinVal.Rows;

					// remove the previous row and add it behind the selected row.
					DataGridViewRow prevRow = rows[index - 1];
					rows.Remove(prevRow);
					prevRow.Frozen = false;
					rows.Insert(index, prevRow);
					dgvLinVal.ClearSelection();
					dgvLinVal.Rows[index - 1].Selected = true;
					UpdateLine();
				}
			}
		}

		private void btnLinDwn_Click(object sender, EventArgs e)
		{
			if (dgvLinVal.RowCount > 0)
			{
				if (dgvLinVal.SelectedRows.Count > 0)
				{
					int rowCount = dgvLinVal.Rows.Count;
					int index = dgvLinVal.SelectedCells[0].OwningRow.Index;

					if (index == (rowCount - 2)) // include the header row
					{
						return;
					}
					DataGridViewRowCollection rows = dgvLinVal.Rows;

					// remove the next row and add it in front of the selected row.
					DataGridViewRow nextRow = rows[index + 1];
					rows.Remove(nextRow);
					nextRow.Frozen = false;
					rows.Insert(index, nextRow);
					dgvLinVal.ClearSelection();
					dgvLinVal.Rows[index + 1].Selected = true;
					UpdateLine();
				}
			}
		}

		private void chbLinChn_CheckedChanged(object sender, EventArgs e)
		{
			_line.SetChannel(rdbLinCh3, rdbLinCh4, rdbLinCh5);
		}

		private void cmbLinChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_line.SetChannel((ComboBox)sender);
			//_line.Channel = ((ComboBox)sender).SelectedIndex;
		}

		private void btnLinCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_line.Code());
		}

		#endregion
	}
}
