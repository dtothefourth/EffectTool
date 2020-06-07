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
	public partial class ChooseChannel : Form
	{
		private object[] _channelsStd = new object[] { 3, 4, 5 };
		private object[] _channelsAdv = new object[] { 0, 1, 2, 3, 4, 5, 6, 7 };

		/// <summary>
		/// If the mode is Standard, only channels 3-5 will be chooseable. Othersie 0-7
		/// </summary>
		public static ASMMode Mode { get; set; }

		/// <summary>
		/// The selected channel with higher priority
		/// </summary>
		public int HighChannel { get; set; }
		/// <summary>
		/// The selected channel with lower priority
		/// </summary>
		public int LowChannel { get; set; }

		/// <summary>
		/// The selected channel with the least priority.
		/// </summary>
		public int LeastChannel { get; set; }

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ChooseChannel()
		{
			InitializeComponent();
			if (Mode == ASMMode.Standard)
			{
				cmbLeast.Items.Clear();
				cmbLeast.Items.AddRange(_channelsStd);
				cmbLeast.SelectedIndex = 0;

				cmbLow.Items.Clear();
				cmbLow.Items.AddRange(_channelsStd);
				cmbLow.SelectedIndex = 0;

				cmbHigh.Items.Clear();
				cmbHigh.Items.AddRange(_channelsStd);
				cmbHigh.SelectedIndex = 0;
				cmbLeast.Enabled = false;
			}
			else
			{
				cmbLeast.Items.Clear();
				cmbLeast.Items.AddRange(_channelsAdv);
				cmbLeast.SelectedIndex = 5;

				cmbLow.Items.Clear();
				cmbLow.Items.AddRange(_channelsAdv);
				cmbLow.SelectedIndex = 4;

				cmbHigh.Items.Clear();
				cmbHigh.Items.AddRange(_channelsAdv);
				cmbHigh.SelectedIndex = 3;
				cmbLeast.Enabled = true;
			}
			DialogResult = System.Windows.Forms.DialogResult.Abort;
		}


		private void cmbHigh_SelectedIndexChanged(object sender, EventArgs e)
		{
			object Save = cmbLow.SelectedItem;
			cmbLow.Items.Clear();

			if (Mode == ASMMode.Standard)
				cmbLow.Items.AddRange(_channelsStd);
			else
				cmbLow.Items.AddRange(_channelsAdv);

			cmbLow.Items.RemoveAt(cmbHigh.SelectedIndex);

			int Index = cmbLow.Items.IndexOf(Save);
			cmbLow.SelectedIndex = (Index < 0) ? 0 : Index;
		}


		private void cmbLow_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmbLow.SelectedIndex < 0 || cmbHigh.SelectedIndex < 0 || cmbLeast.SelectedItem == null)
				return;

			object Save = cmbLeast.SelectedItem;
			cmbLeast.Items.Clear();

			if (Mode == ASMMode.Standard)
				cmbLeast.Items.AddRange(_channelsStd);
			else
				cmbLeast.Items.AddRange(_channelsAdv);

			cmbLeast.Items.RemoveAt(cmbHigh.SelectedIndex);
			cmbLeast.Items.RemoveAt(cmbLow.SelectedIndex);

			int Index = cmbLeast.Items.IndexOf(Save);
			cmbLeast.SelectedIndex = (Index < 0) ? 0 : Index;
		}


		private void btnDone_Click(object sender, EventArgs e)
		{
			HighChannel = Convert.ToInt32(cmbHigh.Text);
			LowChannel = Convert.ToInt32(cmbLow.Text);
			if(Mode != ASMMode.Standard)
				LeastChannel = Convert.ToInt32(cmbLeast.Text);
			DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		/// <summary>
		/// Shows a message and opens the dialog for the user to choose a new channel.
		/// </summary>
		/// <returns>The selected channel with the high priority</returns>
		public static int GetOneChannel()
		{
			MessageBox.Show(Settings.OneChannel, "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
			ChooseChannel cc = new ChooseChannel();
			cc.ShowDialog();
			return cc.HighChannel ;
		}
		/// <summary>
		/// Shows a message and opens the dialog for the user to choose two new channels.
		/// </summary>
		/// <returns>An array containing the two selected channels</returns>
		public static int[] GetTwoChannel()
		{
			MessageBox.Show(Settings.TwoChannels, "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
			ChooseChannel cc = new ChooseChannel();
			cc.ShowDialog();
			return new int[] { cc.HighChannel, cc.LowChannel };
		}

		/// <summary>
		/// Opens the dialog to select 3 channels.
		/// </summary>
		/// <returns>An array containing the three selected channels</returns>
		public static int[] GetThreeChannels()
		{
			if (Mode == ASMMode.Standard)
				return new int[] { 3, 4, 5 };

			ChooseChannel cc = new ChooseChannel();
			cc.ShowDialog();
			return new int[] { cc.HighChannel, cc.LowChannel , cc.LeastChannel };
		}

		private void ChooseChannel_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

	}
}
