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
	public partial class ChooseWindow : Form
	{
		public EffectClasses.Window Window
		{
			get { return _window; }
			set
			{
				_window = value;
				if (value == EffectClasses.Window.Window2)
					rdbWinTwo.Checked = true;
				else
					rdbWinOne.Checked = true;
			}
		}

		private EffectClasses.Window _window;

		public ChooseWindow()
		{
			InitializeComponent();
			this.Window = EffectClasses.Window.Window1;
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void btnDone_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		private void rdbWinOne_CheckedChanged(object sender, EventArgs e)
		{
			_window = EffectClasses.Window.Window1;
		}

		private void rdbWinTwo_CheckedChanged(object sender, EventArgs e)
		{
			_window = EffectClasses.Window.Window2;
		}

		public static void GetWindow(object sender, EffectClasses.OneWindowEventArgs e)
		{
			if (MessageBox.Show(Settings.OneWindow, "One Window", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
			{
				e.Cancel = true;
				return;
			}

			ChooseWindow cw = new ChooseWindow();
			if (cw.ShowDialog() == DialogResult.Cancel)
				e.Cancel = true;
			e.Window = cw.Window;
		}
	}
}
