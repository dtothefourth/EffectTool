using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HDMA_Generator_Tool
{
	public partial class Color_Math_GUI : Form, ITab
	{
		#region ITab
		public ComboBox[] ScreenSelectors { get; set; }

		public TabControl GetTabControl()
		{
			return tbc;
		}

		public void SetASMMode(ASMMode Mode)
		{
		}

		public Bitmap GetScreen()
		{
			string tab = tbc.SelectedTab.Text;
			if (tab == tbgSimple.Text)
				return (Bitmap)pcbWinMainPic.Image;
			return null;
		}
		#endregion

		public Color_Math_GUI()
		{
			InitializeComponent();

			ScreenSelectors = new ComboBox[]
			{
				cmbWinScnSel,
			};

			ComboBox[] logicMasks = new ComboBox[]
			{				
				cmbWinMskLogBg1,
				cmbWinMskLogBg2,
				cmbWinMskLogBg3,
				cmbWinMskLogBg4,
				cmbWinMskLogObj,
				cmbWinMskLogCol,
			};

			//
			foreach (EffectClasses.WindowMaskLogic em in Enum.GetValues(typeof(EffectClasses.WindowMaskLogic)))
				foreach(var cmb in logicMasks)
				{
					cmb.Items.Add(em);
					cmb.SelectedIndex = 0;
				}

			//
			foreach (EffectClasses.ColorAdditionalSelectOptions em in 
				Enum.GetValues(typeof(EffectClasses.ColorAdditionalSelectOptions)))
			{
				cmbWinClpToBlk.Items.Add(em);
				cmbWinPrvMat.Items.Add(em);
				cmbWinClpToBlk.SelectedIndex = 0;
				cmbWinPrvMat.SelectedIndex = 0;
			}

			//add events
			foreach(Control c in tbgSimple.Controls)
			{
				GroupBox b = c as GroupBox;
				if(b != null)
					foreach(Control cbox in b.Controls)
					{
						if (cbox is CheckBox)
							((CheckBox)cbox).CheckedChanged += control_Changed;
						if (cbox is ComboBox)
							((ComboBox)cbox).SelectedValueChanged += control_Changed;
						if(cbox is GroupBox)
							foreach(Control cboxbox in cbox.Controls)
								((CheckBox)cboxbox).CheckedChanged += control_Changed;

					}
			}
		}

		EffectClasses.ColorMath _windowingMath = new EffectClasses.ColorMath();
		private void cmbWinScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(_windowingMath, sender);
			pcbWinMainPic.Image = _windowingMath.GetScreen();
		}

		private void control_Changed(object sender, EventArgs e)
		{
			//$212C TM
			_windowingMath.MainScreenDesignation =
				(chbWinMaiScnBg1.Checked ? EffectClasses.ScreenDesignation.BG1 : 0) |
				(chbWinMaiScnBg2.Checked ? EffectClasses.ScreenDesignation.BG2 : 0) |
				(chbWinMaiScnBg3.Checked ? EffectClasses.ScreenDesignation.BG3 : 0) |
				(chbWinMaiScnBg4.Checked ? EffectClasses.ScreenDesignation.BG4 : 0) |
				(chbWinMaiScnObj.Checked ? EffectClasses.ScreenDesignation.OBJ : 0);

			//212D TS
			_windowingMath.SubScreenDesignation =
				(chbWinSubScnBg1.Checked ? EffectClasses.ScreenDesignation.BG1 : 0) |
				(chbWinSubScnBg2.Checked ? EffectClasses.ScreenDesignation.BG2 : 0) |
				(chbWinSubScnBg3.Checked ? EffectClasses.ScreenDesignation.BG3 : 0) |
				(chbWinSubScnBg4.Checked ? EffectClasses.ScreenDesignation.BG4 : 0) |
				(chbWinSubScnObj.Checked ? EffectClasses.ScreenDesignation.OBJ : 0);

			//$2131 CGADSUB
			_windowingMath.ColorMathDesignation =
				(chbWinColMatBg1.Checked ? EffectClasses.ColorMathMode.BG1 : 0) |
				(chbWinColMatBg2.Checked ? EffectClasses.ColorMathMode.BG2 : 0) |
				(chbWinColMatBg3.Checked ? EffectClasses.ColorMathMode.BG3 : 0) |
				(chbWinColMatBg4.Checked ? EffectClasses.ColorMathMode.BG4 : 0) |
				(chbWinColMatObj.Checked ? EffectClasses.ColorMathMode.OBJ : 0) |
				(!chbWinColMatAdd.Checked ? EffectClasses.ColorMathMode.Substract : 0) |
				(chbWinColMatHlf.Checked ? EffectClasses.ColorMathMode.Half : 0) |
				(chbWinColMatBak.Checked ? EffectClasses.ColorMathMode.Backdrop : 0);

			//$212A WBGLOG
			_windowingMath.Bg1MaskLogic = (EffectClasses.WindowMaskLogic)cmbWinMskLogBg1.SelectedItem;
			_windowingMath.Bg2MaskLogic = (EffectClasses.WindowMaskLogic)cmbWinMskLogBg2.SelectedItem;
			_windowingMath.Bg3MaskLogic = (EffectClasses.WindowMaskLogic)cmbWinMskLogBg3.SelectedItem;
			_windowingMath.Bg4MaskLogic = (EffectClasses.WindowMaskLogic)cmbWinMskLogBg4.SelectedItem;
			//$212B WOBJLOG
			_windowingMath.ObjMaskLogic = (EffectClasses.WindowMaskLogic)cmbWinMskLogObj.SelectedItem;
			_windowingMath.ColorMaskLogic = (EffectClasses.WindowMaskLogic)cmbWinMskLogCol.SelectedItem;

			//$2130 CGWSEL
			_windowingMath.AddColor = !chbWinAddSub.Checked;
			_windowingMath.ClipToBlack = (EffectClasses.ColorAdditionalSelectOptions)cmbWinClpToBlk.SelectedItem;
			_windowingMath.PreventColorMath = (EffectClasses.ColorAdditionalSelectOptions)cmbWinPrvMat.SelectedItem;

			//$212E TMW
			_windowingMath.MainScreenWindowMaskDesignation =
				(chbWinMaiWinBg1.Checked ? EffectClasses.ScreenDesignation.BG1 : 0) |
				(chbWinMaiWinBg2.Checked ? EffectClasses.ScreenDesignation.BG2 : 0) |
				(chbWinMaiWinBg3.Checked ? EffectClasses.ScreenDesignation.BG3 : 0) |
				(chbWinMaiWinBg4.Checked ? EffectClasses.ScreenDesignation.BG4 : 0) |
				(chbWinMaiWinObj.Checked ? EffectClasses.ScreenDesignation.OBJ : 0);

			//$212F TSW
			_windowingMath.SubScreenWindowMaskDesignation =
				(chbWinSubWinBg1.Checked ? EffectClasses.ScreenDesignation.BG1 : 0) |
				(chbWinSubWinBg2.Checked ? EffectClasses.ScreenDesignation.BG2 : 0) |
				(chbWinSubWinBg3.Checked ? EffectClasses.ScreenDesignation.BG3 : 0) |
				(chbWinSubWinBg4.Checked ? EffectClasses.ScreenDesignation.BG4 : 0) |
				(chbWinSubWinObj.Checked ? EffectClasses.ScreenDesignation.OBJ : 0);

			//$2123 W12SEL
			//$2124 W34SEL
			//$2125 WOBJSEL
			_windowingMath.Window1Enabled =
				(chbWinOneEneBg1.Checked ? EffectClasses.WindowingLayers.BG1 : 0) |
				(chbWinOneEneBg2.Checked ? EffectClasses.WindowingLayers.BG2 : 0) |
				(chbWinOneEneBg3.Checked ? EffectClasses.WindowingLayers.BG3 : 0) |
				(chbWinOneEneBg4.Checked ? EffectClasses.WindowingLayers.BG4 : 0) |
				(chbWinOneEneObj.Checked ? EffectClasses.WindowingLayers.OBJ : 0) |
				(chbWinOneEneCol.Checked ? EffectClasses.WindowingLayers.Color : 0);

			_windowingMath.Window1Inverted =
				(chbWinOneInvBg1.Checked ? EffectClasses.WindowingLayers.BG1 : 0) |
				(chbWinOneInvBg2.Checked ? EffectClasses.WindowingLayers.BG2 : 0) |
				(chbWinOneInvBg3.Checked ? EffectClasses.WindowingLayers.BG3 : 0) |
				(chbWinOneInvBg4.Checked ? EffectClasses.WindowingLayers.BG4 : 0) |
				(chbWinOneInvObj.Checked ? EffectClasses.WindowingLayers.OBJ : 0) |
				(chbWinOneInvCol.Checked ? EffectClasses.WindowingLayers.Color : 0);


			_windowingMath.Window2Enabled =
				(chbWinTwoEneBg1.Checked ? EffectClasses.WindowingLayers.BG1 : 0) |
				(chbWinTwoEneBg2.Checked ? EffectClasses.WindowingLayers.BG2 : 0) |
				(chbWinTwoEneBg3.Checked ? EffectClasses.WindowingLayers.BG3 : 0) |
				(chbWinTwoEneBg4.Checked ? EffectClasses.WindowingLayers.BG4 : 0) |
				(chbWinTwoEneObj.Checked ? EffectClasses.WindowingLayers.OBJ : 0) |
				(chbWinTwoEneCol.Checked ? EffectClasses.WindowingLayers.Color : 0);

			_windowingMath.Window2Inverted =
				(chbWinTwoInvBg1.Checked ? EffectClasses.WindowingLayers.BG1 : 0) |
				(chbWinTwoInvBg2.Checked ? EffectClasses.WindowingLayers.BG2 : 0) |
				(chbWinTwoInvBg3.Checked ? EffectClasses.WindowingLayers.BG3 : 0) |
				(chbWinTwoInvBg4.Checked ? EffectClasses.WindowingLayers.BG4 : 0) |
				(chbWinTwoInvObj.Checked ? EffectClasses.WindowingLayers.OBJ : 0) |
				(chbWinTwoInvCol.Checked ? EffectClasses.WindowingLayers.Color : 0);

			pcbWinMainPic.Image = _windowingMath.GetScreen();
		}

		private void pcbWinWinOne_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Windowing Image";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;

			try
			{
				Bitmap img = EffectClasses.BitmapEffects.BlackWhite(
					new Bitmap(Image.FromFile(ofd.FileName),
					new Size(256, EffectClasses.HDMA.Scanlines)), 0.5f);
				pcbWinWinOne.Image = img;
				_windowingMath.WindowingMask1 = img;
			}
			catch
			{
				MessageBox.Show("Couldn't open image " + ofd.FileName, "Opening Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			control_Changed(sender, e);
		}

		private void pcbWinWinTwo_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Windowing Image";
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;

			try
			{
				Bitmap img = EffectClasses.BitmapEffects.BlackWhite(
					new Bitmap(Image.FromFile(ofd.FileName),
					new Size(256, EffectClasses.HDMA.Scanlines)), 0.5f);
				pcbWinWinTwo.Image = img;
				_windowingMath.WindowingMask2 = img;
			}
			catch
			{
				MessageBox.Show("Couldn't open image " + ofd.FileName, "Opening Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			control_Changed(sender, e);
		}

		private void pcbWinBakDrp_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			cd.FullOpen = true;
			cd.Color = pcbWinBakDrp.BackColor;
			if (cd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;
			pcbWinBakDrp.BackColor = cd.Color;
			_windowingMath.SetBackdrop(cd.Color);
			control_Changed(sender, e);
		}

		private void btnWinCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_windowingMath);
		}
		
	}
}
