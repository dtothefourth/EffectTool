using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Extansion.IO;

namespace HDMA_Generator_Tool
{
	public partial class TestingGround : Form
	{
		Bitmap Original;
		EffectClasses.WaveXHDMA wave;

		public TestingGround()
		{
			InitializeComponent();

			EffectClasses.ColorMath math = new EffectClasses.ColorMath();
			math.WindowingMask1 = new Bitmap("left.png");
			math.WindowingMask2 = new Bitmap("right.png");
			math.Collection = EffectClasses.BitmapCollection.Load(Properties.Resources.Default);

			math.MainScreenWindowMaskDesignation = EffectClasses.ScreenDesignation.BG1;
			math.Window1Enabled = EffectClasses.WindowingLayers.BG1 | EffectClasses.WindowingLayers.BG2;
			math.Window1Inverted = EffectClasses.WindowingLayers.BG1;

			math.SubScreenWindowMaskDesignation = EffectClasses.ScreenDesignation.BG2;
			math.Window2Enabled = EffectClasses.WindowingLayers.BG2;
			math.Bg2MaskLogic = EffectClasses.WindowMaskLogic.And;

			pictureBox1.Image = math.GetScreen();
		}
		
		int pixel = 0;

		private void timer1_Tick(object sender, EventArgs e)
		{
			pictureBox1.Image = EffectClasses.BitmapEffects.MoveLine(50, 100, pixel, Original, EffectClasses.Orientation.Down);
			pixel += 3;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			wave.StartAnimation(im => pictureBox1.Image = im);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			wave.StopAnimation();
		}
	}
}
