using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HDMA_Generator_Tool
{
	static class Program
	{
		private static string[] _dependecies = new string[]
		{
			"Extansion.dll", "EffectClasses.dll"
		};
		
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main()
		{
			foreach (string dep in _dependecies)
				if (!System.IO.File.Exists(dep))
				{
					MessageBox.Show(dep + " is missing. Effect Tool cannot run without it.\nProgram will terminate.", "Missing dll",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					Application.Exit();
					return;
				}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Main_Form());
		}
	}
}
