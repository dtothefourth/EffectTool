using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace HDMA_Generator_Tool
{
	/// <summary>
	/// Interface for all the GUIs that are to be used in the main form
	/// </summary>
	public interface ITab
	{
		/// <summary>
		/// An array containing all the ComboBoxes that can be used to change the layers/screens/whatever you call them.
		/// </summary>
		ComboBox[] ScreenSelectors { get; set; }
		/// <summary>
		/// Fetches the TabControl from each GUI Class for implementation on the main form.
		/// </summary>
		/// <returns></returns>
		TabControl GetTabControl();
		/// <summary>
		/// Updated the GUI to add or remove further options.
		/// </summary>
		/// <param name="Mode">The mode the GUI should be set to</param>
		void SetASMMode(ASMMode Mode);

		/// <summary>
		/// Gets the screen of the current open tab
		/// </summary>
		/// <returns>The screen currently displayed by the TabControl</returns>
		System.Drawing.Bitmap GetScreen();
	}

	public interface IScreenshotUser : ITab
	{
		Bitmap[] ScreenshotsImages { get; }
	}
	
	/// <summary>
	/// Interface for all the GUIs that run animations
	/// </summary>
	public interface IAnimated
	{
		/// <summary>
		/// Stops all running animations... duh.
		/// </summary>
		void StopAnimation();
	}
}
