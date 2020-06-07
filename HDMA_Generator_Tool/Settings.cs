using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HDMA_Generator_Tool
{
	public enum ASMMode { Standard, Advanced, Expert }
	
	static class Settings
	{
		public const string UseScreenshot = "<Screenshot>";
		public const string ClearColorPositions = "Are you sure you want to remove all the entries from the list?";
		public const string NeedSelection = "You need to select and entry first.";

		public static Size DefaultSize = new Size(256,EffectClasses.HDMA.Scanlines);

		public static void MissingSelectionMessage()
		{
			MessageBox.Show(NeedSelection, "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static bool DeleteAllMessage()
		{
			return MessageBox.Show(ClearColorPositions, "Clear List", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
		}

		public const string MultilayerFolder = "Multilayers\\";

		public const string OneChannel = "The code only requires one HDMA Channel.\nPlease select the channel of your coice as the high-priority one.";
		public const string TwoChannels = "It appears, that your HDMA can be generated without using all 3 channels.\n" +
					"You'd have to select a high-priority channel, which will be used if only one channel is needed " +
					"and a low-priorety channel, in case two channels are needed.";

		public const string OneWindow = "The code only requires one window.\nPlease select the window of your choice for the code to be used.";


	}

}
