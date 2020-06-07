using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace HDMA_Generator_Tool
{
	public partial class HDMA_Waves_GUI : Form, IScreenshotUser, IAnimated
	{
		#region ITab

		public TabControl GetTabControl()
		{ 
			return tbc;
		}
		public void SetASMMode(ASMMode Mode)
		{
			switch(Mode)
			{
				case ASMMode.Standard:
					grpHowChnAdv.Visible = false;
					grpVerChnAdv.Visible = false;
					break;
				case ASMMode.Advanced:
				case ASMMode.Expert:
					grpHowChnAdv.Visible = true;
					grpVerChnAdv.Visible = true;
					break;
			}
		}
		public ComboBox[] ScreenSelectors { get; set; }

		#endregion
		#region IScreenshotUser
		public Bitmap[] ScreenshotsImages { get; private set; }
		#endregion
		#region IAnimated
		public void StopAnimation()
		{
			//events stop timers
			chbHorAni.Checked = false;
			chbVerAni.Checked = false;
		}
		#endregion
	
		public class InvBooleanBinding : Binding
		{
			public InvBooleanBinding(string propertyName, object dataSource, string dataMember)
				: base(propertyName, dataSource, dataMember)
			{
				this.Format += InvBooleanBinding_Format;
			}

			void InvBooleanBinding_Format(object sender, ConvertEventArgs e)
			{
				if (e.DesiredType != typeof(bool))
					throw new ArgumentException("Desired type has to be boolean");
				bool data = Convert.ToBoolean(e.Value);
				e.Value = !data;
			}

		}

		public HDMA_Waves_GUI()
		{
			InitializeComponent();

			ScreenshotsImages = new Bitmap[tbc.TabCount];

			ScreenSelectors = new ComboBox[]
			{
				cmbHorScnSel,
				cmbVerScnSel,
			};

			cmbHorChn.SelectedIndex = 3;
			cmbVerChn.SelectedIndex = 3;
			
			_xEffect.EnabledWaveCollection = new EffectClasses.ListLineBased<bool>()
			{
				new EffectClasses.LineBased<bool>(50, true),
				new EffectClasses.LineBased<bool>(50, false),
				new EffectClasses.LineBased<bool>(50, true),
				new EffectClasses.LineBased<bool>(50, false),
			};

			#region ToolTip X
			toolTip.SetToolTip(btnHorCod, "Generates the code for the HDMA, which is to be inserted with levelASM or uberASM");
			toolTip.SetToolTip(chbHorAni, "Toggles the animation for the wave effect on the image above.");
			toolTip.SetToolTip(cmbHorScnSel, "Selects which images to use for the effect.");

			toolTip.SetToolTip(trbHorSpd, "The speed at which the animation should run with. It determines x in: Animation changes every 1/xth frame");
			toolTip.SetToolTip(trbHorAmp, "The Amplitude: Defines how strong the wave effect will be");
			toolTip.SetToolTip(trbHorWid, "The width of one moving bar");
			toolTip.SetToolTip(txtHorWid, "The Amplitude: Defines how strong the wave effect will be");
			toolTip.SetToolTip(txtHorRam, "The freeRAM address the table for the HDMA should get inserted at.\nHow many bytes the table takes up can be found in the generated code");
			toolTip.SetToolTip(lblHorInvRam, "The above address is not a valid RAM. It has to be inbetween 7E0000 and 7FFFFF.");

			toolTip.SetToolTip(rdbHorCh3, "Sets the generated HDMA code to use HDMA channel 3 ($433x)");
			toolTip.SetToolTip(rdbHorCh4, "Sets the generated HDMA code to use HDMA channel 4 ($434x)");
			toolTip.SetToolTip(rdbHorCh5, "Sets the generated HDMA code to use HDMA channel 5 ($435x)");
			toolTip.SetToolTip(cmbHorChn, "Sets the generated HDMA code to use the selected HDMA Channel");

			toolTip.SetToolTip(rdbHorLay1, "Makes the HDMA work on layer 1 (FG)");
			toolTip.SetToolTip(rdbHorLay2, "Makes the HDMA work on layer 2 (BG)");
			toolTip.SetToolTip(rdbHorLay3, "Makes the HDMA work on layer 3 (BG)");

			_xEffect.AnimationException += _xEffect_AnimationException;
			#endregion
			#region ToolTip Y
			toolTip.SetToolTip(btnVerCod, "Generates the code for the HDMA, which is to be inserted with levelASM or uberASM");
			toolTip.SetToolTip(chbVerAni, "Toggles the animation for the wave effect on the image above.");
			toolTip.SetToolTip(cmbVerScnSel, "Selects which images to use for the effect.");

			toolTip.SetToolTip(trbVerSpd, "The speed at which the animation should run with. It determines x in: Animation changes every 1/xth frame");
			toolTip.SetToolTip(trbVerAmp, "The Amplitude: Defines how strong the wave effect will be");
			toolTip.SetToolTip(trbVerWid, "The width of one moving bar");
			toolTip.SetToolTip(txtVerWid, "The Amplitude: Defines how strong the wave effect will be");
			toolTip.SetToolTip(txtVerRam, "The freeRAM address the table for the HDMA should get inserted at.\nHow many bytes the table takes up can be found in the generated code");
			toolTip.SetToolTip(lblVerInvRam, "The above address is not a valid RAM. It has to be inbetween 7E0000 and 7FFFFF.");

			toolTip.SetToolTip(rdbVerCh3, "Sets the generated HDMA code to use HDMA channel 3 ($433x)");
			toolTip.SetToolTip(rdbVerCh4, "Sets the generated HDMA code to use HDMA channel 4 ($434x)");
			toolTip.SetToolTip(rdbVerCh5, "Sets the generated HDMA code to use HDMA channel 5 ($435x)");
			toolTip.SetToolTip(cmbVerChn, "Sets the generated HDMA code to use the selected HDMA Channel");

			toolTip.SetToolTip(rdbVerLay1, "Makes the HDMA work on layer 1 (FG)");
			toolTip.SetToolTip(rdbVerLay2, "Makes the HDMA work on layer 2 (BG)");

			_yEffect.AnimationException += _yEffect_AnimationException;
			#endregion
		}
		public Bitmap GetScreen()
		{
			var tab = tbc.SelectedTab;
			if (tab == tpgHor)
				return (Bitmap)pcbHorMainPic.Image;
			if (tab == tpgVer)
				return (Bitmap)pcbVerMainPic.Image;
			return null;
		}
		private void tbc_SelectedIndexChanged(object sender, EventArgs e)
		{
			StopAnimation();
		}

		#region Wave X

		private EffectClasses.ColorMath _xMathSave = new EffectClasses.ColorMath();
		private EffectClasses.ColorMath _xMathDisordered = new EffectClasses.ColorMath();
		private EffectClasses.WaveXHDMA _xEffect = new EffectClasses.WaveXHDMA();

		void _xEffect_AnimationException(object sender, EffectClasses.AnimationExceptionEventArgs e)
		{
			MessageBox.Show(
				"An unexpected exception occured durring the X wave animation:\n\n" + e.Exception.Message,
				"An Error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
			StopAnimation();
		}

		private void cmbHorScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 0, _xMathSave, sender);	//fetch new inmage or screenshot (if screenshot, math will only have blank images)
			//_xMathDisordered.FixedColor = _xMathSave.FixedColor;	//set fixed color...
			rdbHorLay_CheckedChanged(sender, e);					//update disordered images based on the loaded one.
		}

		private void chbHorAni_CheckedChanged(object sender, EventArgs e)
		{
			if (((CheckBox)sender).Checked)
			{
				_xEffect.StartAnimation(
					im =>
					{
						if (rdbHorLay1.Checked)
							_xMathDisordered.BG1 = (Bitmap)im;
						if (rdbHorLay2.Checked)
							_xMathDisordered.BG2 = (Bitmap)im;
						if (rdbHorLay3.Checked)
							_xMathDisordered.BG3 = (Bitmap)im;
						pcbHorMainPic.Image = _xMathDisordered.GetScreen();
					});
			}
			else
				_xEffect.StopAnimation();
		}

		private void trbHorSpd_Scroll(object sender, EventArgs e)
		{
			lblHorSpd.Text = (1f / Math.Pow(2, ((TrackBar)sender).Value)).ToString();
			_xEffect.Speed = (1f / Math.Pow(2, ((TrackBar)sender).Value));
			//rdbHorLay_CheckedChanged(sender, e);
		}

		private void trbHorAmp_Scroll(object sender, EventArgs e)
		{
			lblHorAmp.Text = ((TrackBar)sender).Value.ToString();
			_xEffect.Amplitude = ((TrackBar)sender).Value;
			rdbHorLay_CheckedChanged(sender, e);
		}

		private void rdbHorLay_CheckedChanged(object sender, EventArgs e)
		{
			foreach (Bitmap b in _xMathDisordered.Collection)
				b.Dispose();
			_xMathDisordered.Collection = (EffectClasses.BitmapCollection)_xMathSave.Collection.Clone();

			var ss = ScreenshotsImages[tbc.SelectedIndex];

			if(rdbHorLay1.Checked)
			{
				_xEffect.Layers = EffectClasses.LayerRegister.Layer1_X;
				_xEffect.Original = (ss == null) ? _xMathSave.BG1 : ss;
				_xMathDisordered.BG1 = _xEffect.StaticPic();
			}
			else if (rdbHorLay2.Checked)
			{
				_xEffect.Layers = EffectClasses.LayerRegister.Layer2_X;
				_xEffect.Original = (ss == null) ? _xMathSave.BG2 : ss;
				_xMathDisordered.BG2 = _xEffect.StaticPic();
			}
			else if (rdbHorLay3.Checked)
			{
				_xEffect.Layers = EffectClasses.LayerRegister.Layer3_X;
				_xEffect.Original = (ss == null) ? _xMathSave.BG3 : ss;
				_xMathDisordered.BG3 = _xEffect.StaticPic();
			}
			
			pcbHorMainPic.Image = _xMathDisordered.GetScreen();
		}

		private void trbHorWid_Scroll(object sender, EventArgs e)
		{
			txtHorWid.Text = ((TrackBar)sender).Value.ToString();
			_xEffect.Width = ((TrackBar)sender).Value;
			txtHorWid.BackColor = SystemColors.Window;
			rdbHorLay_CheckedChanged(sender, e);
		}

		private void txtHorWid_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
				e.Handled = true;
		}

		private void txtHorWid_TextChanged(object sender, EventArgs e)
		{
			int val = 0;
			if (!Int32.TryParse(((TextBox)sender).Text, out val))
			{
				((TextBox)sender).BackColor = Color.DarkRed;
				return;
			}
			if(val < trbHorWid.Minimum || val > trbHorWid.Maximum)
			{
				((TextBox)sender).BackColor = Color.DarkRed;
				return;
			}
			else
				((TextBox)sender).BackColor = SystemColors.Window;

			trbHorWid.Value = val;
			_xEffect.Width = val;
			rdbHorLay_CheckedChanged(sender, e);
		}

		private void btnHorCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_xEffect);
		}

		private void txtHorRam_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b' && !Uri.IsHexDigit(e.KeyChar))
				e.Handled = true;
			e.KeyChar = Char.ToUpper(e.KeyChar);
		}

		private void txtHorRam_TextChanged(object sender, EventArgs e)
		{
			try
			{
				int ram = Convert.ToInt32(((TextBox)sender).Text, 16);
				_xEffect.FreeRAM = ram;
				lblHorInvRam.Visible = false;
			}
			catch
			{
				lblHorInvRam.Visible = true;
			}
		}

		private void cmbHorChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_xEffect.SetChannel((ComboBox)sender);
		}
		private void rdbHorChn_CheckedChanged(object sender, EventArgs e)
		{
			_xEffect.SetChannel(rdbHorCh3, rdbHorCh4, rdbHorCh5);
		}

		#endregion
		#region Wave Y

		private EffectClasses.ColorMath _yMathSave = new EffectClasses.ColorMath();
		private EffectClasses.ColorMath _yMathDisordered = new EffectClasses.ColorMath();
		private EffectClasses.WaveYHDMA _yEffect = new EffectClasses.WaveYHDMA();

		void _yEffect_AnimationException(object sender, EffectClasses.AnimationExceptionEventArgs e)
		{
			MessageBox.Show(
				"An unexpected exception occured durring the Y wave animation:\n\n" + e.Exception.Message, 
				"An Error occured",	MessageBoxButtons.OK, MessageBoxIcon.Error);
			StopAnimation();
		}

		private void cmbVerScnSel_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerManager.AsignLayers(this, 0, _yMathSave, sender);
			_yMathDisordered.FixedColor = _yMathSave.FixedColor;
			rdbVerLay_CheckedChanged(sender, e);
		}
		
		private void chbVerAni_CheckedChanged(object sender, EventArgs e)
		{
			if (((CheckBox)sender).Checked)
			{
				_yEffect.StartAnimation(
					im =>
					{
						if (rdbVerLay1.Checked)
							_yMathDisordered.BG1 = (Bitmap)im;
						if (rdbVerLay2.Checked)
							_yMathDisordered.BG2 = (Bitmap)im;
						pcbVerMainPic.Image = _yMathDisordered.GetScreen();
					});
			}
			else
				_yEffect.StopAnimation();
		}

		private void trbVerSpd_Scroll(object sender, EventArgs e)
		{
			lblVerSpd.Text = (1f / Math.Pow(2, ((TrackBar)sender).Value)).ToString();
			_yEffect.Speed = (1f / Math.Pow(2, ((TrackBar)sender).Value));
			//rdbVerLay_CheckedChanged(sender, e);
		}

		private void trbVerAmp_Scroll(object sender, EventArgs e)
		{
			lblVerAmp.Text = ((TrackBar)sender).Value.ToString();
			_yEffect.Amplitude = ((TrackBar)sender).Value;
			rdbVerLay_CheckedChanged(sender, e);
		}

		private void rdbVerLay_CheckedChanged(object sender, EventArgs e)
		{
			foreach (Bitmap b in _yMathDisordered.Collection)
				b.Dispose();
			_yMathDisordered.Collection = (EffectClasses.BitmapCollection)_yMathSave.Collection.Clone();

			var ss = ScreenshotsImages[tbc.SelectedIndex];

			if (rdbVerLay1.Checked)
			{
				_yEffect.Layers = EffectClasses.LayerRegister.Layer1_Y;
				_yEffect.Original = (ss == null) ? _yMathSave.BG1 : ss;
				_yMathDisordered.BG1 = _yEffect.StaticPic();
			}
			else if (rdbVerLay2.Checked)
			{
				_yEffect.Layers = EffectClasses.LayerRegister.Layer2_Y;
				_yEffect.Original = (ss == null) ? _yMathSave.BG2 : ss;
				_yMathDisordered.BG2 = _yEffect.StaticPic();
			}
			pcbVerMainPic.Image = _yMathDisordered.GetScreen();
		}

		private void trbVerWid_Scroll(object sender, EventArgs e)
		{
			txtVerWid.Text = ((TrackBar)sender).Value.ToString();
			_yEffect.Width = ((TrackBar)sender).Value;
			txtVerWid.BackColor = SystemColors.Window;
			rdbVerLay_CheckedChanged(sender, e);
		}

		private void txtVerWid_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
				e.Handled = true;
		}

		private void txtVerWid_TextChanged(object sender, EventArgs e)
		{
			int val = 0;
			if (!Int32.TryParse(((TextBox)sender).Text, out val))
			{
				((TextBox)sender).BackColor = Color.DarkRed;
				return;
			}
			if (val < trbVerWid.Minimum || val > trbVerWid.Maximum)
			{
				((TextBox)sender).BackColor = Color.DarkRed;
				return;
			}
			else
				((TextBox)sender).BackColor = SystemColors.Window;

			trbVerWid.Value = val;
			_yEffect.Width = val;
			rdbVerLay_CheckedChanged(sender, e);
		}

		private void btnVerCod_Click(object sender, EventArgs e)
		{
			ShowCode.ShowCodeDialog(_yEffect);
		}

		private void txtVerRam_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b' && !Uri.IsHexDigit(e.KeyChar))
				e.Handled = true;
			e.KeyChar = Char.ToUpper(e.KeyChar);
		}

		private void txtVerRam_TextChanged(object sender, EventArgs e)
		{
			try
			{
				int ram = Convert.ToInt32(((TextBox)sender).Text, 16);
				_yEffect.FreeRAM = ram;
				lblVerInvRam.Visible = false;
			}
			catch
			{
				lblVerInvRam.Visible = true;
			}
		}

		private void cmbVerChn_SelectedIndexChanged(object sender, EventArgs e)
		{
			_yEffect.SetChannel((ComboBox)sender);
		}
		private void rdbVerChn_CheckedChanged(object sender, EventArgs e)
		{
			_yEffect.SetChannel(rdbVerCh3, rdbVerCh4, rdbVerCh5);
		}
		#endregion

		private void chbHorRan_CheckedChanged(object sender, EventArgs e)
		{
		}
	}
}
