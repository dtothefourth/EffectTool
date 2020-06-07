using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HDMA_Generator_Tool
{
	public partial class HDMA_Windowing_GUI : Form, ITab
	{
		#region ITab
		public ComboBox[] ScreenSelectors { get; set; }
		public TabControl GetTabControl()
		{
			return tbc;
		}
		public void SetASMMode(ASMMode Mode)
		{
			switch(Mode)
			{
				case ASMMode.Standard:
					cmbImgAdvChn.Visible = false;
					_imgEffect.SetChannel(cmbImgSmpChn);
					break;
				case ASMMode.Advanced:
				case ASMMode.Expert:
					cmbImgAdvChn.Visible = true;
					cmbImgAdvChn.SelectedIndex = cmbImgSmpChn.SelectedIndex + 3;
					break;
			}
		}
		public Bitmap GetScreen()
		{
			string tab = tbc.SelectedTab.Text;
			if (tab == tbgSimple.Text)
				return (Bitmap)pcbImgMainPic.Image;
			return null;
		}
		#endregion

		#region IScreenshotUser
		public Bitmap[] ScreenshotsImages { get; private set; }
		#endregion

		public enum RenderType { Color, Transparent, Points }

		public HDMA_Windowing_GUI()
		{
			InitializeComponent();

			ScreenshotsImages = new Bitmap[tbc.TabCount];
			ScreenSelectors = new ComboBox[]
			{
				cmbImgScnSel,
			};

			lsbImgPoi.Items.Add(new Point(0, 0));

			foreach (Enum e in Enum.GetValues(typeof(EffectClasses.ColorAdditionalSelectOptions)))
			{
				cmbImgClpToBlk.Items.Add(e);
				cmbImgPrvMat.Items.Add(e);
			}
			foreach (Enum e in Enum.GetValues(typeof(RenderType)))
				cmbImgRnd.Items.Add(e);

			cmbImgRnd.SelectedIndex = 2;
			cmbImgPrvMat.SelectedIndex = 2;
			cmbImgClpToBlk.SelectedIndex = 2;
			cmbImgSmpChn.SelectedIndex = 0;
			_imgEffect.OneWindowEvent += ChooseWindow.GetWindow;
		}

		private EffectClasses.ColorMath _imgMath = new EffectClasses.ColorMath();
		private EffectClasses.WindowingHDMA _imgEffect = new EffectClasses.WindowingHDMA();
		private bool _imgLockLsb;

		private void ApplyCorrectRender()
		{
			if (cmbImgRnd.SelectedItem == null)
				return;

			float percent = trbImgBlkWhtSet.Value * 10;
			Bitmap blackwhite = EffectClasses.BitmapEffects.FromColor(Color.White, 256, 224);
			ImageAttributes att = new ImageAttributes();
			att.SetColorMatrix(new ColorMatrix(new float[][]
				{
					new float[] {0, 0, 0, 0, 0},
					new float[] {0, 0, 0, 0, 0},
					new float[] {0, 0, 0, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1},
				}));

			switch ((RenderType)cmbImgRnd.SelectedItem)
			{				
				case RenderType.Points:
					pnlImgPoi.Enabled = true;
					using (Graphics g = Graphics.FromImage(blackwhite))
					{
						g.DrawImage(EffectClasses.BitmapEffects.Render(
							_imgEffect.Orignal, percent, lsbImgPoi.Items.Cast<Point>().ToArray()),
							new Rectangle(new Point(0,0), blackwhite.Size), 0,0,blackwhite.Width, blackwhite.Height,
							GraphicsUnit.Pixel, att);
					}
					break;
				case RenderType.Color:
					blackwhite = EffectClasses.BitmapEffects.BlackWhite(_imgEffect.Orignal, percent / 100);
					break;
				case RenderType.Transparent:
					using (Graphics g = Graphics.FromImage(blackwhite))
					{
						g.DrawImage(_imgEffect.Orignal,
							new Rectangle(new Point(0, 0), blackwhite.Size), 0, 0, blackwhite.Width, blackwhite.Height,
							GraphicsUnit.Pixel, att);
					}
					break;
			}

			if (chbImgInv.Checked)
				blackwhite = EffectClasses.BitmapEffects.Invert(blackwhite);

			pcbImgWin.Image = blackwhite;
			_imgEffect.EffectImage = blackwhite;
			lblImgNoEff.Visible = !_imgEffect.CheckAndSplitMask(blackwhite, _imgMath);
		

			pcbImgMainPic.Image = _imgMath.GetScreen();
		}

		private void btnImgLod_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Windowing Image";

			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;

			Bitmap bm = null;

			try
			{
				bm = new Bitmap(Image.FromFile(ofd.FileName), new Size(256, 224));
				_imgEffect.Orignal = bm;
				pcbImgPic.Image = bm;

				lsbImgPoi.Items.Clear();
				lsbImgPoi.Items.Add(new Point(0, 0));

				ApplyCorrectRender();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Couldn't load image", "Something went wrong while trying to load image\n\n" + ex.Message,
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void pcbImgPic_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				if ((RenderType)cmbImgRnd.SelectedItem != RenderType.Points)
					return;
				lsbImgPoi.Items.Add(new Point(e.X * 2, e.Y * 2));
				ApplyCorrectRender();
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Left)
				btnImgLod_Click(sender, e);
		}

		private void cmbImgScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 0, _imgMath, sender);
			pcbImgMainPic.Image = _imgMath.GetScreen();
		}

		private void lsbImgPoi_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lsbImgPoi.SelectedItem == null)
				return;
			_imgLockLsb = true;
			nudImgX.Value = ((Point)lsbImgPoi.SelectedItem).X;
			nudImgY.Value = ((Point)lsbImgPoi.SelectedItem).Y;
			_imgLockLsb = false;
		}

		private void imgMathControl_CheckedChanged(object sender, EventArgs e) 
		{
			if (cmbImgClpToBlk.SelectedItem != null)
				_imgMath.ClipToBlack = (EffectClasses.ColorAdditionalSelectOptions)cmbImgClpToBlk.SelectedItem;
			if (cmbImgPrvMat.SelectedItem != null)
				_imgMath.PreventColorMath = (EffectClasses.ColorAdditionalSelectOptions)cmbImgPrvMat.SelectedItem;

			_imgMath.MainScreenDesignation = _imgMath.MainScreenWindowMaskDesignation = (EffectClasses.ScreenDesignation)nudImgTMW.Value;
			_imgMath.SubScreenDesignation = _imgMath.SubScreenWindowMaskDesignation = (EffectClasses.ScreenDesignation)nudImgTSW.Value;

			_imgMath.Window1Enabled = _imgMath.Window2Enabled =
				(chbImgEneBg1.Checked ? EffectClasses.WindowingLayers.BG1 : 0) |
				(chbImgEneBg2.Checked ? EffectClasses.WindowingLayers.BG2 : 0) |
				(chbImgEneBg3.Checked ? EffectClasses.WindowingLayers.BG3 : 0) |
				(chbImgEneBg4.Checked ? EffectClasses.WindowingLayers.BG4 : 0) |
				(chbImgEneObj.Checked ? EffectClasses.WindowingLayers.OBJ : 0) |
				(chbImgEneCol.Checked ? EffectClasses.WindowingLayers.Color : 0);

			_imgMath.AddColor = !chbImgAddSub.Checked;
			pcbImgMainPic.Image = _imgMath.GetScreen();
		}

		private void chbImgInv_CheckedChanged(object sender, EventArgs e)
		{
			pnlImgPoi.Enabled = (RenderType?)cmbImgRnd.SelectedValue == RenderType.Points;

			lblImgBlkWhtSet.Text = trbImgBlkWhtSet.Value * 10 + "%";
			ApplyCorrectRender();
		}

		private void nudImgXY_ValueChanged(object sender, EventArgs e)
		{
			if (lsbImgPoi.SelectedItem == null || _imgLockLsb)
				return;
			Point p = (Point)lsbImgPoi.SelectedItem;
			p.Y = (int)nudImgY.Value;
			p.X = (int)nudImgX.Value;
			lsbImgPoi.Items[lsbImgPoi.SelectedIndex] = p;
			ApplyCorrectRender();
		}

		private void lsbImgPoi_KeyDown(object sender, KeyEventArgs e)
		{
			if (lsbImgPoi.SelectedItem != null && e.KeyCode == Keys.Delete)
			{
				lsbImgPoi.Items.RemoveAt(lsbImgPoi.SelectedIndex);
				ApplyCorrectRender();
			}
		}

		private void btnImgCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_imgMath, _imgEffect);
		}

		private void cmbImgChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			cmbImgAdvChn.SelectedIndex = cmbImgSmpChn.SelectedIndex + 3;
			_imgEffect.SetChannel((ComboBox)sender);
		}

	}
}
