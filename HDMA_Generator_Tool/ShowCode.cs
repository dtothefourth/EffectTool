using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace HDMA_Generator_Tool
{
	public partial class ShowCode : Form
	{
		public string Code { get; set; }
		public bool UsesMain { get; set; }

		public ShowCode() : this("") { }
		public ShowCode(string code)
		{
			InitializeComponent();
			this.Code = code;

			if(Code.Contains(EffectClasses.HDMA.MAINSeperator))
			{
				string[] split = code.Split(new[] { EffectClasses.HDMA.MAINSeperator }, StringSplitOptions.None);
				rtbInit.Text = split[0];
				rtbMain.Text = split[1].TrimStart('\n','\r');
			}
			else
			{
				rtbInit.Text = code;
				spcCode.Panel2Collapsed = true;
				MinimumSize = new Size(256, this.MinimumSize.Height);
				btnMainToClip.Visible = false;
				btnInitToClip.Text = "Copy to Clipboard";
			}
		}

		public static void ShowCodeDialog(string code)
		{
			try
			{
				if (code == null || code == String.Empty)
					return;
				new ShowCode(code).ShowDialog();
			}
			catch(Exception ex)
			{
				ShowMessage(ex);
			}
		}

		public static void ShowCodeDialog(params EffectClasses.ICodeProvider[] providers)
		{
			try
			{
				string code = "";
				foreach (var provider in providers)
				{
					string single = provider.Code();
					if (single == null || single == String.Empty)
						return;
					code += single + "\n";
				}
				new ShowCode(code).ShowDialog();
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}


		public static void ShowMessage(Exception ex)
		{
			MessageBox.Show(ex.Message, "Can't Generate Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void btnInitToClip_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(rtbInit.Text.Replace("\n", "\r\n"));
		}
		private void btnMainToClip_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(rtbMain.Text.Replace("\n", "\r\n"));
		}
		
		private void btn_AsASM_Click(object sender, EventArgs e)
		{
			SaveFileDialog SFD = new SaveFileDialog();
			SFD.Filter = "UberASMTool LevelASM (*.asm)|*.asm";
			SFD.DefaultExt = ".asm";
			if (SFD.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;

			string path = Path.GetDirectoryName(SFD.FileName) + "\\" + 
				Path.GetFileName(SFD.FileName).Replace(' ', '_');

			System.Windows.Forms.DialogResult res = System.Windows.Forms.DialogResult.OK;

			do
			{
				try
				{
					

					string CodeToPrint = "; To be upplied to a level using UberASMTool.\n\n" +
					     Code;

					CodeToPrint = CodeToPrint.Replace("RTS", "RTL");
					File.WriteAllText(path, CodeToPrint);
					
				}
				catch (Exception ex)
				{
					res = MessageBox.Show("Saving has failed due to reasons such as those listed below:\n\n"
						+ ex.Message, "No Save", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
				}
			} while (res == System.Windows.Forms.DialogResult.Retry);
		}
	}
}
