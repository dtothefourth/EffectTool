using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Extansion.Int;

namespace HDMA_Generator_Tool
{
	public partial class HDMA_Parallax_GUI : Form, IScreenshotUser, IAnimated
	{
		#region ITab

		public TabControl GetTabControl() { return tbc; }
		public void SetASMMode(ASMMode Mode)
		{
			switch (Mode)
			{
				case ASMMode.Standard:
					grpAbsChnAdv.Visible = false;
					rdbAbsChn_CheckedChanged(null, null);	//set channel to what is set in simple radio button
					grpRelChnAdv.Visible = false;
					rdbRelChn_CheckedChanged(null, null);	//set channel to what is set in simple radio button
					break;
				case ASMMode.Advanced:
				case ASMMode.Expert:
					grpAbsChnAdv.Visible = true;
					cmbAbsChn_SelectedIndexChanged(cmbAbsChn, null);	//set channel to what is set in advanced box
					grpRelChnAdv.Visible = true;
					cmbRelChn_SelectedIndexChanged(cmbRelChn, null);	//set channel to what is set in advanced box
					break;
			}
		}
		public Bitmap GetScreen()
		{
			var tab = tbc.SelectedTab;
			if (tab == tbpRel)
				return (Bitmap)pcbRelMainPic.Image;
			return null;
		}
		public ComboBox[] ScreenSelectors { get; set; }

		#endregion
		#region IScreenshotUser
		public Bitmap[] ScreenshotsImages { get; private set; }
		#endregion
		#region IAnimated
		public void StopAnimation()
		{
			chbAbsAni.Checked = false;
			chbRelAni.Checked = false;
		}
		#endregion
		
		Dictionary<Control, int> _cellIndex;

		public HDMA_Parallax_GUI()
		{
			InitializeComponent();

			ScreenshotsImages = new Bitmap[tbc.TabCount];

			ScreenSelectors = new ComboBox[]
			{
				cmbRelScnSel,
			};
			_cellIndex = new Dictionary<Control, int>()
			{
				{nudAbsScnLin, 1},
				{cmbAbsMul, 2},
				{chbAbsAut, 3},
				{nudAbsSpd, 4},
				{cmbAbsDir, 5},

				{nudRelScnLin, 1},
				{cmbRelMul, 2},
				{chbRelAut, 3},
				{nudRelSpd, 4},
				{cmbRelDir, 5},
			};

			//relative initialization
			cmbRelLay.SelectedIndex = 1;
			cmbRelChn.SelectedIndex = 3;
			cmbRelDir.Items.Add(EffectClasses.Orientation.Left);
			cmbRelDir.Items.Add(EffectClasses.Orientation.Right);

			//absolute initialization
			cmbAbsDir.Items.Add(EffectClasses.Orientation.Left);
			cmbAbsDir.Items.Add(EffectClasses.Orientation.Right);
			cmbAbsLay.SelectedIndex = 1;
			cmbAbsChn.SelectedIndex = 3;
			SetImageForEffect(432, Properties.Resources.Level105BG, _absEffect);
			_absEffect.AnimationException += _absEffect_AnimationException;
			
		}

		void _absEffect_AnimationException(object sender, EffectClasses.AnimationExceptionEventArgs e)
		{
			MessageBox.Show(
				"An unexpected exception occured durring the absoulte animation:\n\n" + e.Exception.Message,
				"An Error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
			StopAnimation();
		}

		#region Absolute

		EffectClasses.ParallaxHDMA _absEffect = new EffectClasses.ParallaxHDMA();
		DataGridViewRow _activeAbsRow;

		private void btnAbsLoaScn_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Lunar Magic Screenshot (*.png)|*.png";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;
			try
			{
				Bitmap img = new Bitmap(ofd.FileName);
				SetImageForEffect(432, img, _absEffect);
			}
			catch(Exception ex)
			{
				MessageBox.Show("Couldn't open image:\n\n" + ex.Message, "Error loading Image",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void SetImageForEffect(int height, Bitmap image, EffectClasses.ParallaxHDMA effect)
		{
			try
			{
				//The image may not be larger than 256 x height;
				int width = image.Width.Max(EffectClasses.ParallaxHDMA.ImageWidth), actHeight = image.Height.Max(height);

				//Get a new image from the min requirements and fit it into a 256 x height box.
				Bitmap img = new Bitmap(
					image.Clone(new Rectangle(0, 0, width, actHeight), image.PixelFormat),
					new Size(EffectClasses.ParallaxHDMA.ImageWidth, height));

				_absEffect.Original = img;
				pcbAbsMainPic.Image = _absEffect.StaticPic();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Couldn't pass image:\n\n" + ex.Message, "Error passing image",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		
		#region DataGridView Editing Events

		private void dgvAbsEnt_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			MessageBox.Show(dgvAbsEnt.Columns[e.ColumnIndex].HeaderText);
		}

		private void btnAbsNew_Click(object sender, EventArgs e)
		{
			dgvAbsEnt.Rows.Add(_absEffect.Bars.Count + 1,
				10, "1", true, 0, EffectClasses.Orientation.Right);
			dgvAbsEnt.Rows[dgvAbsEnt.Rows.Count - 1].Selected = true;
			dgvAbsEnt.FirstDisplayedScrollingRowIndex = dgvAbsEnt.Rows.Count - 1;

			UpdateAbsoulteBars();
		}

		private void dgvAbsEnt_SelectionChanged(object sender, EventArgs e)
		{
			if (dgvAbsEnt.SelectedRows.Count == 0)
				return;
			_activeAbsRow = dgvAbsEnt.SelectedRows[0];
			if (_activeAbsRow == null)
				return;

			nudAbsScnLin.Value = Convert.ToInt32(_activeAbsRow.Cells[1].Value);
			cmbAbsMul.SelectedIndex = cmbAbsMul.Items.IndexOf(_activeAbsRow.Cells[2].Value);
			chbAbsAut.Checked = (bool)_activeAbsRow.Cells[3].Value;
			
			nudAbsSpd.Value = Convert.ToInt32(_activeAbsRow.Cells[4].Value);
			cmbAbsDir.SelectedIndex = cmbAbsDir.Items.IndexOf(_activeAbsRow.Cells[5].Value);
			EnableAllAbsoulteControls(true);
		}

		/// <summary>
		/// Event that is activated when any of the controls with which the values in the DataGridViews are controlled is used
		/// </summary>
		/// <param name="sender">The control that was used to edit the DataGridViewRow</param>
		/// <param name="e">The (in this case useless) event args</param>
		private void absControl_Changed(object sender, EventArgs e)
		{
			if (_activeAbsRow == null)
				return;
			if(sender is NumericUpDown)
				_activeAbsRow.Cells[_cellIndex[(Control)sender]].Value = ((NumericUpDown)sender).Value;
			else if(sender is ComboBox)
				_activeAbsRow.Cells[_cellIndex[(Control)sender]].Value = ((ComboBox)sender).SelectedItem;
			else if (sender is CheckBox)
				_activeAbsRow.Cells[_cellIndex[(Control)sender]].Value = ((CheckBox)sender).Checked;

			cmbAbsDir.Enabled = chbAbsAut.Checked;
			nudAbsSpd.Enabled = chbAbsAut.Checked;

			UpdateAbsoulteBars();
		}

		private void btnAbsDel_Click(object sender, EventArgs e)
		{
			if (_activeAbsRow == null)
				return;
			dgvAbsEnt.Rows.Remove(_activeAbsRow);
			if (dgvAbsEnt.SelectedRows.Count == 0)
			{
				EnableAllAbsoulteControls(false);
				return;
			}
			_activeAbsRow = dgvAbsEnt.SelectedRows[0];
			UpdateAbsoulteBars();
		}

		private void btnAbsClr_Click(object sender, EventArgs e)
		{
			if(Settings.DeleteAllMessage())
			{
				dgvAbsEnt.Rows.Clear();
				EnableAllAbsoulteControls(false);
				UpdateAbsoulteBars();
			}
		}

		private void btnAbsMovUp_Click(object sender, EventArgs e)
		{
			if (dgvAbsEnt.RowCount > 0)
			{
				if (dgvAbsEnt.SelectedRows.Count > 0)
				{
					int rowCount = dgvAbsEnt.Rows.Count;
					int index = dgvAbsEnt.SelectedCells[0].OwningRow.Index;

					if (index == 0)
					{
						return;
					}
					DataGridViewRowCollection rows = dgvAbsEnt.Rows;

					// remove the previous row and add it behind the selected row.
					DataGridViewRow prevRow = rows[index - 1];
					rows.Remove(prevRow);
					prevRow.Frozen = false;
					rows.Insert(index, prevRow);
					dgvAbsEnt.ClearSelection();
					dgvAbsEnt.Rows[index - 1].Selected = true;
					UpdateAbsoulteBars();
				}
			}
		}

		private void btnAbsMovDwn_Click(object sender, EventArgs e)
		{
			if (dgvAbsEnt.RowCount > 0)
			{
				if (dgvAbsEnt.SelectedRows.Count > 0)
				{
					int rowCount = dgvAbsEnt.Rows.Count;
					int index = dgvAbsEnt.SelectedCells[0].OwningRow.Index;

					if (index == (rowCount - 1))
					{
						return;
					}
					DataGridViewRowCollection rows = dgvAbsEnt.Rows;

					// remove the next row and add it in front of the selected row.
					DataGridViewRow nextRow = rows[index + 1];
					rows.Remove(nextRow);
					nextRow.Frozen = false;
					rows.Insert(index, nextRow);
					dgvAbsEnt.ClearSelection();
					dgvAbsEnt.Rows[index + 1].Selected = true;
					UpdateAbsoulteBars();
				}
			}
		}

		#endregion

		/// <summary>
		/// Sets the Enable state of all Controls for the DataGrid contents.
		/// The Controls depending on the Autoscroll checkbox will only be enabled if it is checked
		/// </summary>
		/// <param name="enable">Whether to enable or disable all controls</param>
		/// <param name="enforce">Enforces the enabling even for the Autoscroll depending controls</param>
		private void EnableAllAbsoulteControls(bool enable, bool enforce = false)
		{
			nudAbsScnLin.Enabled =
			cmbAbsMul.Enabled =
			chbAbsAut.Enabled =
			btnAbsClr.Enabled =
			btnAbsMovDwn.Enabled =
			btnAbsMovUp.Enabled =
			btnAbsDel.Enabled = enable;

			nudAbsSpd.Enabled =
			cmbAbsDir.Enabled = (chbAbsAut.Checked && enable) | enforce;
		}

		/// <summary>
		/// Updates the bars in the effect class with what is written in the DataGridView
		/// </summary>
		public void UpdateAbsoulteBars()
		{
			List<EffectClasses.ParallaxHDMA.ParallaxHDMAEntry> bars
				= new List<EffectClasses.ParallaxHDMA.ParallaxHDMAEntry>();
			for(int i = 0; i < dgvAbsEnt.Rows.Count; i++)
			{
				dgvAbsEnt.Rows[i].Cells[0].Value = i + 1;

				int scan = Convert.ToInt32(dgvAbsEnt.Rows[i].Cells[1].Value);
				string multi = (string)dgvAbsEnt.Rows[i].Cells[2].Value;
				bool auto = (bool)dgvAbsEnt.Rows[i].Cells[3].Value;
				int wind = Convert.ToInt32(dgvAbsEnt.Rows[i].Cells[4].Value);
				var dir = (EffectClasses.Orientation)dgvAbsEnt.Rows[i].Cells[5].Value;

				double dmulti = 0.0;
				switch (multi)
				{
					case "1/16": dmulti = (1.0 / 16.0); break;
					case "1/8": dmulti = (1.0 / 8.0); break;
					case "1/4": dmulti = (1.0 / 4.0); break;
					case "1/2": dmulti = (1.0 / 2.0); break;
					case "1": dmulti = (1.0); break;
					case "2": dmulti = (2.0); break;
					case "4": dmulti = (4.0); break;
					case "8": dmulti = (8.0); break;
					case "16": dmulti = (16.0); break;
				}
				bars.Add(new EffectClasses.ParallaxHDMA.ParallaxHDMAEntry(i + 1, scan, dmulti, auto, wind, dir));
			}
			_absEffect.Bars = bars;
		}

		private void btnAbsCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_absEffect);
		}

		private void chbAbsAni_CheckedChanged(object sender, EventArgs e)
		{
			if (((CheckBox)sender).Checked)
			{
				_absEffect.StartAnimation(
					im =>
					{
						/*
						if (cmbAbsLay.SelectedIndex == 0)
							_absMathDisordered.BG1 = (Bitmap)im;
						if (cmbAbsLay.SelectedIndex == 1)
							_absMathDisordered.BG2 = (Bitmap)im;
						if (cmbAbsLay.SelectedIndex == 2)
							_absMathDisordered.BG3 = (Bitmap)im;*/
						pcbAbsMainPic.Image = im;// _absMathDisordered.GetScreen();
					});
			}
			else
				_absEffect.StopAnimation();
		}

		private void cmbAbsLay_SelectedIndexChanged(object sender, EventArgs e)
		{
			/*
			foreach (Bitmap b in _absMathDisordered.Collection)
				b.Dispose();
			_absMathDisordered.Collection = (EffectClasses.BitmapCollection)_absMathSave.Collection.Clone();

			if (cmbAbsLay.SelectedIndex == 0)
			{
				_absEffect.Layers = EffectClasses.LayerRegister.Layer1_X;
				_absEffect.Original = _absMathSave.BG1;
				_absMathDisordered.BG1 = _absEffect.StaticPic();
			}
			else if (cmbAbsLay.SelectedIndex == 1)
			{
				_absEffect.Layers = EffectClasses.LayerRegister.Layer2_X;
				_absEffect.Original = _absMathSave.BG2;
				_absMathDisordered.BG2 = _absEffect.StaticPic();
			}
			else if (cmbAbsLay.SelectedIndex == 2)
			{
				_absEffect.Layers = EffectClasses.LayerRegister.Layer3_X;
				_absEffect.Original = _absMathSave.BG3;
				_absMathDisordered.BG3 = _absEffect.StaticPic();
			}
			*/
			
			if (cmbAbsLay.SelectedIndex == 0)
			{
				_absEffect.Layers = EffectClasses.LayerRegister.Layer1_X;
			}
			else if (cmbAbsLay.SelectedIndex == 1)
			{
				_absEffect.Layers = EffectClasses.LayerRegister.Layer2_X;
			}
			else if (cmbAbsLay.SelectedIndex == 2)
			{
				_absEffect.Layers = EffectClasses.LayerRegister.Layer3_X;
			}

		}

		private void cmbAbsChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_absEffect.SetChannel((ComboBox)sender);
		}

		private void rdbAbsChn_CheckedChanged(object sender, EventArgs e)
		{
			_absEffect.SetChannel(rdbAbsCh3, rdbAbsCh4, rdbAbsCh5);
		}

		private void txtAbsRam_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b' && !Uri.IsHexDigit(e.KeyChar))
				e.Handled = true;
			e.KeyChar = Char.ToUpper(e.KeyChar);
		}

		private void txtAbsRam_TextChanged(object sender, EventArgs e)
		{
			try
			{
				int ram = Convert.ToInt32(((TextBox)sender).Text, 16);
				_absEffect.FreeRAM = ram;
				((TextBox)sender).ForeColor = SystemColors.WindowText;
				((TextBox)sender).BackColor = SystemColors.Window;
			}
			catch
			{
				((TextBox)sender).ForeColor = Color.White;
				((TextBox)sender).BackColor = Color.DarkRed;
			}
		}
		#endregion

		
		private EffectClasses.ColorMath _relMathSave = new EffectClasses.ColorMath();
		private EffectClasses.ColorMath _relMathDisordered = new EffectClasses.ColorMath();
		private EffectClasses.ParallaxHDMA _relEffect = new EffectClasses.ParallaxHDMA();
		private DataGridViewRow _activeRelRow = null;

		/// <summary>
		/// Sets the Enable state of all Controls for the DataGrid contents.
		/// The Controls depending on the Autoscroll checkbox will only be enabled if it is checked
		/// </summary>
		/// <param name="enable">Whether to enable or disable all controls</param>
		/// <param name="enforce">Enforces the enabling even for the Autoscroll depending controls</param>
		private void EnableAllRelativeControls(bool enable, bool enforce = false)
		{
			nudRelScnLin.Enabled =
			cmbRelMul.Enabled =
			chbRelAut.Enabled =
			btnRelClr.Enabled =
			btnRelMovDwn.Enabled =
			btnRelMovUp.Enabled =
			btnRelDel.Enabled = enable;

			nudRelSpd.Enabled =
			cmbRelDir.Enabled = (chbRelAut.Checked && enable) | enforce;
		}

		private void cmbRelScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 0, _relMathSave, sender);
			_relMathDisordered.FixedColor = _relMathSave.FixedColor;
			UpdateRelative();
		}
		
		private void chbRelAni_CheckedChanged(object sender, EventArgs e)
		{
			if (((CheckBox)sender).Checked)
			{
				_relEffect.StartAnimation(
					im =>
					{
						if (cmbRelLay.SelectedIndex == 0)
							_relMathDisordered.BG1 = (Bitmap)im;
						if (cmbRelLay.SelectedIndex == 1)
							_relMathDisordered.BG2 = (Bitmap)im;
						if (cmbRelLay.SelectedIndex == 2)
							_relMathDisordered.BG3 = (Bitmap)im;
						pcbRelMainPic.Image = _relMathDisordered.GetScreen();
					});
			}
			else
				_relEffect.StopAnimation();
		}

		void UpdateRelative()
		{
			cmbRelLay_SelectedIndexChanged(null, null);
		}

		private void cmbRelLay_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (Bitmap b in _relMathDisordered.Collection)
				b.Dispose();
			_relMathDisordered.Collection = (EffectClasses.BitmapCollection)_relMathSave.Collection.Clone();
			var ss = ScreenshotsImages[tbc.SelectedIndex];

			if (cmbRelLay.SelectedIndex == 0)
			{
				_relEffect.Layers = EffectClasses.LayerRegister.Layer1_X;
				_relEffect.Original = (ss == null) ? _relMathSave.BG1 : ss;
				_relMathDisordered.BG1 = _relEffect.StaticPic();
			}
			else if (cmbRelLay.SelectedIndex == 1)
			{
				_relEffect.Layers = EffectClasses.LayerRegister.Layer2_X;
				_relEffect.Original = (ss == null) ? _relMathSave.BG2 : ss;
				_relMathDisordered.BG2 = _relEffect.StaticPic();
			}
			else if (cmbRelLay.SelectedIndex == 2)
			{
				_relEffect.Layers = EffectClasses.LayerRegister.Layer3_X;
				_relEffect.Original = (ss == null) ? _relMathSave.BG3 : ss;
				_relMathDisordered.BG3 = _relEffect.StaticPic();
			}
			pcbRelMainPic.Image = _relMathDisordered.GetScreen();
		}

		private void btnRelNew_Click(object sender, EventArgs e)
		{
			dgvRelEnt.Rows.Add(_relEffect.Bars.Count + 1,
				10, "1", true, 0, EffectClasses.Orientation.Right);
			dgvRelEnt.Rows[dgvRelEnt.Rows.Count - 1].Selected = true;
			dgvRelEnt.FirstDisplayedScrollingRowIndex = dgvRelEnt.Rows.Count - 1;

			UpdateRelativeBars();
		}

		private void dgvRelEnt_SelectionChanged(object sender, EventArgs e)
		{
			if (dgvRelEnt.SelectedRows.Count == 0)
				return;
			_activeRelRow = dgvRelEnt.SelectedRows[0];
			if (_activeRelRow == null)
				return;

			nudRelScnLin.Value = Convert.ToInt32(_activeRelRow.Cells[1].Value);
			cmbRelMul.SelectedIndex = cmbRelMul.Items.IndexOf(_activeRelRow.Cells[2].Value);
			chbRelAut.Checked = (bool)_activeRelRow.Cells[3].Value;

			nudRelSpd.Value = Convert.ToInt32(_activeRelRow.Cells[4].Value);
			cmbRelDir.SelectedIndex = cmbRelDir.Items.IndexOf(_activeRelRow.Cells[5].Value);
			EnableAllRelativeControls(true);
		}

		private void btnRelMovUp_Click(object sender, EventArgs e)
		{
			if (dgvRelEnt.RowCount > 0)
			{
				if (dgvRelEnt.SelectedRows.Count > 0)
				{
					int rowCount = dgvRelEnt.Rows.Count;
					int index = dgvRelEnt.SelectedCells[0].OwningRow.Index;

					if (index == 0)
					{
						return;
					}
					DataGridViewRowCollection rows = dgvRelEnt.Rows;

					// remove the previous row and add it behind the selected row.
					DataGridViewRow prevRow = rows[index - 1];
					rows.Remove(prevRow);
					prevRow.Frozen = false;
					rows.Insert(index, prevRow);
					dgvRelEnt.ClearSelection();
					dgvRelEnt.Rows[index - 1].Selected = true;
					UpdateRelativeBars();
				}
			}
		}

		private void btnRelMovDwn_Click(object sender, EventArgs e)
		{
			if (dgvRelEnt.RowCount > 0)
			{
				if (dgvRelEnt.SelectedRows.Count > 0)
				{
					int rowCount = dgvRelEnt.Rows.Count;
					int index = dgvRelEnt.SelectedCells[0].OwningRow.Index;

					if (index == (rowCount - 1))
					{
						return;
					}
					DataGridViewRowCollection rows = dgvRelEnt.Rows;

					// remove the next row and add it in front of the selected row.
					DataGridViewRow nextRow = rows[index + 1];
					rows.Remove(nextRow);
					nextRow.Frozen = false;
					rows.Insert(index, nextRow);
					dgvRelEnt.ClearSelection();
					dgvRelEnt.Rows[index + 1].Selected = true;
					UpdateRelativeBars();
				}
			}
		}

		private void btnRelDel_Click(object sender, EventArgs e)
		{
			if (_activeRelRow == null)
				return;
			dgvRelEnt.Rows.Remove(_activeRelRow);
			if (dgvRelEnt.SelectedRows.Count == 0)
			{
				EnableAllRelativeControls(false);
				return;
			}
			_activeRelRow = dgvRelEnt.SelectedRows[0];
			UpdateRelativeBars();
		}

		private void btnRelClr_Click(object sender, EventArgs e)
		{
			if (Settings.DeleteAllMessage())
			{
				dgvRelEnt.Rows.Clear();
				EnableAllRelativeControls(false);
				UpdateRelativeBars();
			}
		}

		/// <summary>
		/// Updates the bars in the effect class with what is written in the DataGridView
		/// </summary>
		public void UpdateRelativeBars()
		{
			List<EffectClasses.ParallaxHDMA.ParallaxHDMAEntry> bars
				= new List<EffectClasses.ParallaxHDMA.ParallaxHDMAEntry>();
			for (int i = 0; i < dgvRelEnt.Rows.Count; i++)
			{
				dgvRelEnt.Rows[i].Cells[0].Value = i + 1;

				int scan = Convert.ToInt32(dgvRelEnt.Rows[i].Cells[1].Value);
				string multi = (string)dgvRelEnt.Rows[i].Cells[2].Value;
				bool auto = (bool)dgvRelEnt.Rows[i].Cells[3].Value;
				int wind = Convert.ToInt32(dgvRelEnt.Rows[i].Cells[4].Value);
				var dir = (EffectClasses.Orientation)dgvRelEnt.Rows[i].Cells[5].Value;

				double dmulti = 0.0;
				switch (multi)
				{
					case "1/16": dmulti = (1.0 / 16.0); break;
					case "1/8": dmulti = (1.0 / 8.0); break;
					case "1/4": dmulti = (1.0 / 4.0); break;
					case "1/2": dmulti = (1.0 / 2.0); break;
					case "1": dmulti = (1.0); break;
					case "2": dmulti = (2.0); break;
					case "4": dmulti = (4.0); break;
					case "8": dmulti = (8.0); break;
					case "16": dmulti = (16.0); break;
				}
				bars.Add(new EffectClasses.ParallaxHDMA.ParallaxHDMAEntry(i + 1, scan, dmulti, auto, wind, dir));
			}
			_relEffect.Bars = bars;
		}

		private void relControl_Changed(object sender, EventArgs e)
		{
			if (_activeRelRow == null)
				return;
			if (sender is NumericUpDown)
				_activeRelRow.Cells[_cellIndex[(Control)sender]].Value = ((NumericUpDown)sender).Value;
			else if (sender is ComboBox)
				_activeRelRow.Cells[_cellIndex[(Control)sender]].Value = ((ComboBox)sender).SelectedItem;
			else if (sender is CheckBox)
				_activeRelRow.Cells[_cellIndex[(Control)sender]].Value = ((CheckBox)sender).Checked;

			cmbRelDir.Enabled = chbRelAut.Checked;
			nudRelSpd.Enabled = chbRelAut.Checked;

			UpdateRelativeBars();
		}

		private void cmbRelChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_relEffect.SetChannel((ComboBox)sender);
		}

		private void rdbRelChn_CheckedChanged(object sender, EventArgs e)
		{
			_relEffect.SetChannel(rdbRelCh3, rdbRelCh4, rdbRelCh5);
		}

		private void cmbRelLay_ValueMemberChanged(object sender, EventArgs e)
		{
		}

		private void txtRelFreRam_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b' && !Uri.IsHexDigit(e.KeyChar))
				e.Handled = true;
			e.KeyChar = Char.ToUpper(e.KeyChar);
		}

		private void txtRelFreRam_TextChanged(object sender, EventArgs e)
		{
			try
			{
				int ram = Convert.ToInt32(((TextBox)sender).Text, 16);
				_relEffect.FreeRAM = ram;
				((TextBox)sender).ForeColor = SystemColors.WindowText;
				((TextBox)sender).BackColor = SystemColors.Window;
			}
			catch
			{
				((TextBox)sender).ForeColor = Color.White;
				((TextBox)sender).BackColor = Color.DarkRed;
			}
		}

		private void btnRelCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_relEffect);
		}
	}
}
