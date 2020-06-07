using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing.Imaging;

namespace HDMA_Generator_Tool
{
	public partial class Main_Form : Form
	{
		public string ActiveTab = "";
		
		public Dictionary<string, ITab> Tabs;
		public List<EffectClasses.BitmapCollection> Screens;
		
		//Distance that the splitter should remains at when changing tabs.
		private const int _splitterDistance = 140;
		private const int _originalHeight = 360;
		private const int _widthAdd = 20;
		private const int _heightAdder = 65;


		public Main_Form()
		{
			InitializeComponent();  //
			
			//Christmas Message
			DateTime today = DateTime.Today;
			if (today.Month == 12 && today.Day == 24)
				lblNoSelect.Text += "\n\nMERRY CHRISTMAS <3";
			//Halloween Message
			else if (today.Month == 10 && today.Day == 31)
				lblNoSelect.Text += "\n\nHAPPY HALLOWEEN >:D";
			
			using (Graphics g = this.CreateGraphics())
			{
				// All tabs have the same height. So set height based on that and DPI setting.
				this.Height = (int)Math.Ceiling(_originalHeight * (g.DpiY / 96));
			}

			//expand all nodes from the beginning.
			foreach (TreeNode node in tvwEffects.Nodes)
				if (node.Nodes.Count != 0)
					node.ExpandAll();

			//contains all the tabs linking them to their names with the nodes
			Tabs = new Dictionary<string, ITab>()
			{
				{"BG Gradient", new HDMA_Gradiant_GUI(false) },	//bg
				{"FG Gradient", new HDMA_Gradiant_GUI(true) },	//fg
				{"Brightness", new HDMA_Brightness_GUI()},
				{"Waves", new HDMA_Waves_GUI()},
				{"Parallax", new HDMA_Parallax_GUI()},
				{"Windows", new HDMA_Windowing_GUI()},
				{"Mosaic", new HDMA_Mosaic_GUI()},
				{"Color Math", new Color_Math_GUI()},
			};
			
			foreach (ITab IT in Tabs.Values)
			{
				TabControl t = IT.GetTabControl();      // \  
				t.Location = new Point(0, 0);           //  | Runs through all the GUIs and gets the TabControls
				t.Visible = false;                      //  | and adds them to the main control.
				splitContainer1.Panel2.Controls.Add(t); // /  
			}

			//Setup the ComboBoxes containing the mulilayers
			SetupMulitlayerComboBoxes();
		}
		
		/// <summary>
		/// Event that is triggered when a nod in the TreeView get's selected
		/// If the nod is a different than the currently selected one (and not a folder)
		/// all running animations will be halted and all TabControls will be made invisible.
		/// Afterwards the needed TabControl will again be made visible and the forms size will be adjusted to it.
		/// </summary>
		/// <param name="sender">The object to trigger the event</param>
		/// <param name="e">the event args containing the node that was selected</param>
		private void tvwEffects_AfterSelect(object sender, TreeViewEventArgs e)
		{
			//saves the name of the selected node
			string hdmaEffect = tvwEffects.SelectedNode.Name;

			if (!Tabs.ContainsKey(hdmaEffect))  // \  if the name is not in the dictionary
				return;                         //  | for example one of the folders
			if (ActiveTab == hdmaEffect)        //  | or the same tab is selected twice
				return;                         // /  return

			//the label at the beginning telling you to select a node.
			lblNoSelect.Visible = false;

			//makes al the TabControls invisible and stops the animation
			foreach (ITab GUI in Tabs.Values)
			{
				GUI.GetTabControl().Visible = false;
				IAnimated AnimatedGUI = GUI as IAnimated;
				if (AnimatedGUI != null)
					AnimatedGUI.StopAnimation();
			}

			//save the active node's name and make the tabcontrol visible
			TabControl T = Tabs[hdmaEffect].GetTabControl();
			T.Visible = true;
			ActiveTab = hdmaEffect;

			//adjust size
			using (Graphics g = this.CreateGraphics())
			{
				this.Width = (int)Math.Ceiling((_splitterDistance + _widthAdd) * (g.DpiX / 96)) + T.Width;
				this.Height = (int)Math.Ceiling((T.Height + _heightAdder) * (g.DpiY / 96));    // setzt die Weite der Main-control auf die der TabControl
				splitContainer1.SplitterDistance = (int)Math.Ceiling(_splitterDistance * (g.DpiX / 96)); // der Splitter würde dabei prozentuel verschoben werden, deswegen wird er wieder rückgesetzt.
			}
		}

		/// <summary>
		/// Event das Aktiviert wird, wenn eine Nod geöffnet oder geschloßen wird
		/// Wird verwendet um für die Überordner das Bild anzupassen
		/// </summary>
		/// <param name="sender">Object, dass das Event ausgelößt hat</param>
		/// <param name="e">Die EventAtgs</param>
		private void tvwEffects_AfterExpandColaps(object sender, TreeViewEventArgs e)
		{
			TreeNode TN = e.Node; //Zwischenspeichern

			//Wenn die Nod geschlossen wurde...
			if (e.Action == TreeViewAction.Expand)
			{
				TN.SelectedImageIndex = 3;  // Soll das Bild auf das, des geschlossenen Ordners geändert werden
				TN.ImageIndex = 3;          //
			}
			//Wenn die Nod geöffnet wurde
			else if (e.Action == TreeViewAction.Collapse)
			{
				TN.SelectedImageIndex = 0;  // Soll das Bild zu einem geöffnet Ordner werden
				TN.ImageIndex = 0;          //
			}
		}
		

		private void addNewMultilayerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MultiLayerCreator mlc = new MultiLayerCreator();
			if (mlc.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				return;

			//if the multilayer is new and not an edit.
			if (!mlc.Edit)
			{
				Screens.Add(mlc.GeneratedCollection);
				foreach (ITab IT in Tabs.Values)
				{
					if (IT.ScreenSelectors != null)
						foreach (ComboBox cb in IT.ScreenSelectors)
						{
							cb.Items.Add(mlc.GeneratedCollection);
						}
				}
			}
			//if the multilayer is an edit
			else
			{
				SetupMulitlayerComboBoxes();
			}
		}
		
		private void asmModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				foreach(ToolStripMenuItem item in aSMOptionsToolStripMenuItem.DropDownItems)
					item.Checked = false;
				ToolStripMenuItem clicked = (ToolStripMenuItem)sender;
				clicked.Checked = true;

				ASMMode selectedMode = 0;
				if (clicked.Text == "Standard")
					selectedMode = ASMMode.Standard;
				else if (clicked.Text == "All")
					selectedMode = ASMMode.Advanced;
				else
					selectedMode = ASMMode.Expert;

				foreach(ITab tab in Tabs.Values)
					tab.SetASMMode(selectedMode);

				ChooseChannel.Mode = selectedMode;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Something went wrong.", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

        /*
		private void sA1CodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EffectClasses.RAM.SA1 = sA1CodeToolStripMenuItem.Checked;
			if (sA1CodeToolStripMenuItem.Checked)
				MessageBox.Show("This feature is untested.\nUse at own rist.", "☢ Caution ☢ Caution ☢ Caution ☢", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
        */

		/// <summary>
		/// Removes the annoying "ding" sound.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape || keyData == Keys.Enter)
			{
				//return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}


		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
#if(DEBUG)
			new TestingGround().Show();
#else

#endif
		}

		/// <summary>
		/// (Re)Loads the multilayer files inside the ComboBoxes.
		/// </summary>
		private void SetupMulitlayerComboBoxes()
		{
			//empty Screens
			if (Screens == null)
				Screens = new List<EffectClasses.BitmapCollection>();
			else
				Screens.Clear();


			if (!Directory.Exists(Settings.MultilayerFolder))
			{
				MessageBox.Show(Settings.MultilayerFolder + " folder doesn't exist.\nNo multilayers could be loaded", "Folder Missing",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				//get all the multilayer files in the .\Multilayer folder
				FileInfo[] files = new DirectoryInfo(Settings.MultilayerFolder).GetFiles("*.ml");

				foreach (FileInfo fi in files)
				{
					try
					{
						//load Layers and use filename as name to appear.
						EffectClasses.BitmapCollection collection = EffectClasses.BitmapCollection.Load(fi.FullName);
						collection.Name = Path.GetFileNameWithoutExtension(fi.Name);
						Screens.Add(collection);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Couldn't open/load multilayer file: " + fi.FullName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
			}

			//set screens on overy tab
			foreach (ITab IT in Tabs.Values)
			{
				if (IT.ScreenSelectors != null)
				{
					foreach (ComboBox cb in IT.ScreenSelectors)
					{
						//if an index has been selected, preserve it, otherwise start with 1
						int index = (cb.SelectedIndex == ListBox.NoMatches ? 1 : cb.SelectedIndex);

						//clear ComboBoxes and add <Load Screenshot> text, default multilayer and the loaded ones.
						cb.Items.Clear();
						cb.Items.Add(Settings.UseScreenshot);
						cb.Items.Add(EffectClasses.BitmapCollection.Load(Properties.Resources.Default));
						foreach (EffectClasses.BitmapCollection col in Screens)
							cb.Items.Add(col);

						//(re)set selected index
						cb.SelectedIndex = index;
					}
				}
			}
		}

		/// <summary>
		/// Makes a screenshot of the current image displayed in the... display
		/// </summary>
		/// <param name="sender">Caller of the event</param>
		/// <param name="e">args. Unused.</param>
		private void screenshotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ActiveTab == "")
			{
				MessageBox.Show("You can't take screenshots before selecting a tab.", "No Tab Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			Bitmap screenshot = Tabs[ActiveTab].GetScreen();
			if (screenshot == null)
			{
				MessageBox.Show("The current tab doesn't support this.", "No Support", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			int i = 0;
			string file = ActiveTab.Replace(' ','_');
			while(File.Exists(file + "_" + i.ToString("00000") + ".png"))
				i++;
			screenshot.Save(file + "_" + i.ToString("00000") + ".png");
		}
	}


	public delegate void TryUseScreenshot (Image IM);
	public delegate void FinallyUpdateeScreenshot ();
	public static class Screenshot
	{
		/// <summary>
		/// Die Höhe, die das zu ladente Bild haben muss, um verwendet zu werden
		/// </summary>
		public static int ControlHeight = EffectClasses.HDMA.Scanlines;
		/// <summary>
		/// Die Breite, die das zu ladente Bild haben muss, um verwendet zu werden
		/// </summary>
		public static int ControlWidth = 256;
		/// <summary>
		/// Die Größe, die das zu ladente Bild haben muss, um verwendet zu werden
		/// </summary>
		public static Size ControlSize
		{
			get
			{
				return new Size(ControlWidth, ControlHeight);
			}
			set
			{
				ControlHeight = value.Height;
				ControlWidth = value.Width;
			}
		}

		public static String Filter = "Screenshot (*.png)|*.png";
		public static NotImplementedException Exception = new NotImplementedException("The selected tab doesn't have an option to load a screenshot.\nPlease select a tab with the \"Load (Own) Screenshot\" button on it and try again");

		public static void ShowMessageBox(Exception ex)
		{ MessageBox.Show(ex.Message, "Screenshot Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

		/// <summary>
		/// Ruft den OpenFileDialog auf für das auswählen eines ZSNES (oder andere) Screenshots
		/// Wenn das ausgewählte Bild nicht genau den Abmessungen eines Screenshots (256x224) entspricht, wird eine Exception ausgelößt
		/// </summary>
		/// <returns>Das Bild, das ausgewählt wurde</returns>
		public static Image Load()
		{
			OpenFileDialog OFD = new OpenFileDialog();  // \
			OFD.Filter = Filter;                        //  | OpenFileDialog aufsetzen
			OFD.Title = "Load Screenshot";              // /
			if (DialogResult.OK != OFD.ShowDialog())    // \ Wenn abgebrochen wird, wird eine Exception ausgelößt
				throw new AccessViolationException();   // /
			if (!File.Exists(OFD.FileName))
				throw new FileNotFoundException();

			//Ladet das Bild und überprüft dessen Abmessung
			Image IM = Image.FromFile(OFD.FileName, false);
			if (IM.Height != ControlHeight || IM.Width != ControlWidth)
				throw new FormatException("Please chose an image which is " + ControlHeight + " pixels high and " + ControlWidth + " pixels wide (ZSNES screenshot)");

			return IM; //Wenn alles passt, wird das Bild zurückgegeben
		}

		public static void Load(TryUseScreenshot delTry, FinallyUpdateeScreenshot delFinally)
		{
			try 
			{ 
				delTry(Load()); 
			}
			catch (FormatException ex)
			{
				Screenshot.ShowMessageBox(ex);
			}
			catch (FileNotFoundException ex)
			{
				Screenshot.ShowMessageBox(ex);
			}
			catch { }
			finally 
			{ 
				delFinally(); 
			}
		}
	}

	public delegate void ColorControlUpdate();

	public class ColorControlManager
	{
		[DebuggerDisplay("{_textbox.Name}, {_picturebox.Name}")]
		private class TxtPcbPair
		{
			private TextBox _textbox;
			private PictureBox _picturebox;
			private ColorControlUpdate _updater;
			
			public TxtPcbPair(TextBox textbox, PictureBox picturebox, ColorControlUpdate updater)
			{
				this._picturebox = picturebox;
				this._textbox = textbox;
				this._updater = updater;

				_picturebox.Click += _picturebox_Click;
				_textbox.KeyPress += _textbox_KeyPress;
				_textbox.KeyUp += _textbox_KeyUp;
			}

			void _textbox_KeyUp(object sender, KeyEventArgs e)
			{
				try
				{                    
					if (e.KeyCode != Keys.Return)
						return;

					string s = _textbox.Text.Trim('#').PadLeft(6, '0');
					_textbox.Text = "#" + s.ToUpper();
					int r = Convert.ToInt32(s.Substring(0, 2), 16);
					int g = Convert.ToInt32(s.Substring(2, 2), 16);
					int b = Convert.ToInt32(s.Substring(4, 2), 16);
					_picturebox.BackColor = Color.FromArgb(r, g, b);
				}
				catch
				{
					_textbox.Text = "#000000";
					_picturebox.BackColor = Color.Black;
				}

				if(_updater != null)
					_updater();
			}

			void _textbox_KeyPress(object sender, KeyPressEventArgs e)
			{
				if (e.KeyChar == '#' && ((System.Windows.Forms.TextBox)sender).TextLength != 0)
					e.Handled = true;
				else if (e.KeyChar != '\b' && e.KeyChar != '#')
					e.Handled = !System.Uri.IsHexDigit(e.KeyChar);
			}

			void _picturebox_Click(object sender, EventArgs e)
		{
			ColorDialog d = new ColorDialog();
			d.Color = _picturebox.BackColor;
			d.FullOpen = true;
			if (d.ShowDialog() == DialogResult.Cancel)
				return;

			_picturebox.BackColor = d.Color;
			_textbox.Text = "#" + d.Color.R.ToString("X2") + d.Color.G.ToString("X2") + d.Color.B.ToString("X2");
			if(_updater != null)
				_updater();
		}
		}

		private List<TxtPcbPair> pairs = new List<TxtPcbPair>();

		public void Add(TextBox textbox, PictureBox picturebox)
		{
			pairs.Add(new TxtPcbPair(textbox, picturebox, null));
		}

		public void Add(TextBox textbox, PictureBox picturebox, ColorControlUpdate update)
		{
			pairs.Add(new TxtPcbPair(textbox, picturebox, update));
		}
	}


	public class ColorControl
	{

		public static void AssignTextBoxEvents(params TextBox[] textboxes)
		{
			foreach(TextBox txt in textboxes)
			{
				txt.KeyPress += HexTextBox;
			}
		}

		private static void HexTextBox(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == '#' && ((System.Windows.Forms.TextBox)sender).TextLength != 0)
				e.Handled = true;
			else if (e.KeyChar != '\b' && e.KeyChar != '#')
				e.Handled = !System.Uri.IsHexDigit(e.KeyChar);
		}

		public static void TextBoxToPictureBox(TextBox textbox, PictureBox picturebox)
		{
			try
			{                
				string s = textbox.Text.Trim('#').PadLeft(6,'0');
				textbox.Text = "#" + s.ToUpper();
				int r = Convert.ToInt32(s.Substring(0, 2), 16);
				int g = Convert.ToInt32(s.Substring(2, 2), 16);
				int b = Convert.ToInt32(s.Substring(4, 2), 16);
				picturebox.BackColor = Color.FromArgb(r,g,b);
			}
			catch
			{
				textbox.Text = "#000000";
				picturebox.BackColor = Color.Black;
			}
		}

		public static void PictureBoxToTextBox(PictureBox picturebox, TextBox textbox)
		{
			textbox.Text = "#" + ColorTranslator.ToHtml(picturebox.BackColor);
		}

		public static void PicureBoxDialog(PictureBox picturebox)
		{
			ColorDialog d = new ColorDialog();
			d.Color = picturebox.BackColor;
			d.FullOpen = true;
			if (d.ShowDialog() == DialogResult.Cancel)
				return;
		}


		public PictureBox PictureBoxColor { get; set; }

		public TrackBar TrackBarGreen { get; set; }
		public TrackBar TrackBarRed { get; set; }
		public TrackBar TrackBarBlue { get; set; }

		public TextBox TextBoxRed { get; set; }
		public TextBox TextBoxGreen { get; set; }
		public TextBox TextBoxBlue { get; set; }

		public ColorControlUpdate AfterUpdateFunction { get; set; }

		public ColorControl(PictureBox pcb, TrackBar trbred, TrackBar trbgreen, TrackBar trbblue, TextBox txtred, TextBox txtgreen, TextBox txtblue)
		{
			this.PictureBoxColor = pcb;

			this.TrackBarBlue = trbblue;
			this.TrackBarGreen = trbgreen;
			this.TrackBarRed = trbred;

			this.TextBoxBlue = txtblue;
			this.TextBoxGreen = txtgreen;
			this.TextBoxRed = txtred;

			pcb.Click += new EventHandler(pcb_Click);

			trbred.Scroll += new EventHandler(trb_Scroll);
			trbgreen.Scroll += new EventHandler(trb_Scroll);
			trbblue.Scroll += new EventHandler(trb_Scroll);

			txtblue.KeyDown += new KeyEventHandler(txtblue_KeyDown);
			txtgreen.KeyDown += new KeyEventHandler(txtgreen_KeyDown);
			txtred.KeyDown += new KeyEventHandler(txtred_KeyDown);
		}

		private void txtred_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;
			Color c = PictureBoxColor.BackColor;
			byte R;
			if (!byte.TryParse(((TextBox)sender).Text, out R))
			{
				((TextBox)sender).Text = c.R.ToString();
				TrackBarRed.Value = c.R;
				return;
			}

			TrackBarRed.Value = R;
			PictureBoxColor.BackColor = Color.FromArgb(R, c.G, c.B);
			if (AfterUpdateFunction != null)
				AfterUpdateFunction();
		}
		private void txtgreen_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;
			Color c = PictureBoxColor.BackColor;
			byte G;
			if (!byte.TryParse(((TextBox)sender).Text, out G))
			{
				((TextBox)sender).Text = c.G.ToString();
				TrackBarGreen.Value = c.G;
				return;
			}

			TrackBarGreen.Value = G;
			PictureBoxColor.BackColor = Color.FromArgb(c.R, G, c.B);
			if(AfterUpdateFunction != null)
				AfterUpdateFunction();
		}
		private void txtblue_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter)
				return;
			Color c = PictureBoxColor.BackColor;
			byte B;
			if (!byte.TryParse(((TextBox)sender).Text, out B))
			{
				((TextBox)sender).Text = c.B.ToString();
				TrackBarBlue.Value = c.B;
				return;
			}

			TrackBarBlue.Value = B;
			PictureBoxColor.BackColor = Color.FromArgb(c.R, c.G, B);
			if(AfterUpdateFunction != null)
				AfterUpdateFunction();
		}

		private void trb_Scroll(object sender, EventArgs e)
		{
			int B = TrackBarBlue.Value;
			int G = TrackBarGreen.Value;
			int R = TrackBarRed.Value;

			TrackBarRed.Value = R;
			TextBoxRed.Text = R.ToString();

			TrackBarGreen.Value = G;
			TextBoxGreen.Text = G.ToString();

			TrackBarBlue.Value = B;
			TextBoxBlue.Text = B.ToString();

			PictureBoxColor.BackColor = Color.FromArgb(R, G, B);
			if(AfterUpdateFunction != null)
				AfterUpdateFunction();
		}
		private void pcb_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			cd.Color = ((PictureBox)sender).BackColor;
			cd.FullOpen = true;

			if (cd.ShowDialog() == DialogResult.Cancel)
				return;
			
			Color c = cd.Color;
			int R = c.R;
			int G = c.G;
			int B = c.B;

			TrackBarBlue.Value = B;
			TrackBarGreen.Value = G;
			TrackBarRed.Value = R;

			TextBoxBlue.Text = B.ToString();
			TextBoxGreen.Text = G.ToString();
			TextBoxRed.Text = R.ToString();

			PictureBoxColor.BackColor = c;
			if(AfterUpdateFunction != null)
				AfterUpdateFunction();
		}
	}

	public static class Extansion
	{	

		public static bool MatchesColor(this Color main, Color compare, double percentage)
		{ 
			double com = 2.55 * percentage;
			return (//compare.A >= (main.A - com) && compare.A <= (main.A + com) &&
				compare.R >= (main.R - com) && compare.R <= (main.R + com) &&
				compare.G >= (main.G - com) && compare.G <= (main.G + com) &&
				compare.B >= (main.B - com) && compare.B <= (main.B + com));
		}
		
		public static bool IsLetter(this Keys Key)
		{
			return (Key == Keys.A ||
				Key == Keys.B ||
				Key == Keys.C ||
				Key == Keys.D ||
				Key == Keys.E ||
				Key == Keys.F ||
				Key == Keys.G ||
				Key == Keys.H ||
				Key == Keys.I ||
				Key == Keys.J ||
				Key == Keys.K ||
				Key == Keys.L ||
				Key == Keys.M ||
				Key == Keys.N ||
				Key == Keys.O ||
				Key == Keys.P ||
				Key == Keys.Q ||
				Key == Keys.R ||
				Key == Keys.S ||
				Key == Keys.T ||
				Key == Keys.U ||
				Key == Keys.V ||
				Key == Keys.W ||
				Key == Keys.X ||
				Key == Keys.Y ||
				Key == Keys.Z);
		}
	}
}
