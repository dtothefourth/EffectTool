using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Extansion.Int;

namespace Extansion.Drawing
{
	public static class Drawing
	{
		/// <summary>
		/// Gibt den invertierten Color Wert einer Farbe zurück. Die Transparenz bleibt unberührt
		/// </summary>
		/// <param name="main">Die Farbe, die invertiert wird.</param>
		/// <returns></returns>
		public static Color Invert(this Color main)
		{
			return Color.FromArgb(main.A, 255 - main.R, 255 - main.G, 255 - main.B);
		}

		public static Color Substract(this Color a, Color b)
		{
			return Color.FromArgb((a.R - b.R).Min(0), (a.G - b.G).Min(0), (a.B - b.B).Min(0));
		}

		public static Bitmap Invert(this Image source)
		{
			//create a blank bitmap the same size as original
			Bitmap newBitmap = new Bitmap(source.Width, source.Height);
			//get a graphics object from the new image
			Graphics g = Graphics.FromImage(newBitmap);

			// create the negative color matrix
			System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(new float[][]
			{
				new float[] {-1, 0, 0, 0, 0},
				new float[] {0, -1, 0, 0, 0},
				new float[] {0, 0, -1, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {1, 1, 1, 0, 1}
			});

			// create some image attributes
			System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();

			attributes.SetColorMatrix(colorMatrix);

			g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
						0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);

			//dispose the Graphics object
			g.Dispose();

			return newBitmap;
		}
		public static Image BlackWhite(this Image Image, float threshold)
		{
			Bitmap SourceImage = new Bitmap(Image);
			using (Graphics gr = Graphics.FromImage(SourceImage)) // SourceImage is a Bitmap object
			{
				var gray_matrix = new float[][] { 
				new float[] { 0.299f, 0.299f, 0.299f, 0, 0 }, 
				new float[] { 0.587f, 0.587f, 0.587f, 0, 0 }, 
				new float[] { 0.114f, 0.114f, 0.114f, 0, 0 }, 
				new float[] { 0,      0,      0,      1, 0 }, 
				new float[] { 0,      0,      0,      0, 1 }};

				var ia = new System.Drawing.Imaging.ImageAttributes();
				ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(gray_matrix));
				ia.SetThreshold(threshold); // Change this threshold as needed
				var rc = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);
				gr.DrawImage(SourceImage, rc, 0, 0, SourceImage.Width, SourceImage.Height, GraphicsUnit.Pixel, ia);
			}
			return SourceImage;
		}
	}
}
