using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Extansion.Enum;

namespace HDMA_Generator_Tool
{
	[Flags]
	public enum OutlineStart { TopLeft = 1, Top = 2, TopRight = 4, Right = 8, BottomRight = 16, Bottom = 32, BottomLeft = 64, Left = 128 }
	public class FastBitmap
	{
		Bitmap source = null;
		IntPtr Iptr = IntPtr.Zero;
		BitmapData bitmapData = null;

		bool AlreadyLocked = false;

		public byte[] Pixels { get; set; }
		public int Depth { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public FastBitmap(Bitmap source)
		{
			this.source = source;
			LockBits();
		}
		public FastBitmap(int Width, int Height) : this(new Bitmap(Width, Height)) { }
		public FastBitmap(string filename) : this(new Bitmap(filename)) { }
		public FastBitmap(FastBitmap baseImage) : this(new Bitmap(baseImage)) { baseImage.LockBits(); }

		public void Outline(OutlineStart start, double percent)
		{
			foreach (OutlineStart os in start.GetFlags())
			{
				int x = 0, y = 0;
				switch (os)
				{
					case OutlineStart.TopRight: x = Width - 1; break;
					case OutlineStart.BottomLeft: y = Height - 1; break;
					case OutlineStart.BottomRight: y = Height - 1; x = Width - 1; break;
				}
				StepCounterBreak = 0;
				Color basecolor = this.GetPixel(x, y);
				Rec_Pixel(new List<Point>() { new Point(x, y) }, new List<Point>(), basecolor, percent);
			}
		}

		public void Render(double percent, params Point[] points)
		{
			foreach (Point p in points)
			{
				int x = p.X, y = p.Y;
				Color basecolor = this.GetPixel(x, y);
				Rec_Pixel(new List<Point>() { new Point(x, y) }, new List<Point>(), basecolor, percent);
			}
		}

		private int StepCounterBreak = 0;
		public int RecursivePixelLimit = 3000;

		private void Rec_Pixel(List<Point> Locations, List<Point> LastLocations, Color BaseColor, double percent)
		{
			if (StepCounterBreak++ >= RecursivePixelLimit)
				throw new StackOverflowException("The method has called itself more than " + RecursivePixelLimit + 
					" times. Emergency Exit");// return;

			List<Point> NextLocations = new List<Point>();
			foreach (Point Loc in Locations)
			{
				int x = Loc.X, y = Loc.Y;
				if (!GetPixel(x, y).MatchesColor(BaseColor, percent))
					continue;

				SetPixel(x, y, Color.FromArgb(0, BaseColor.G, BaseColor.B, BaseColor.R));
				
				if (x + 1 < Width && !LastLocations.Any(p => (p.X == x + 1 && p.Y == y)))
					NextLocations.Add(new Point(x + 1, y));
				if (y + 1 < Height && !LastLocations.Any(p => (p.X == x && p.Y == y + 1)))
					NextLocations.Add(new Point(x, y + 1));
				if (x - 1 >= 0 && !LastLocations.Any(p => (p.X == x - 1 && p.Y == y)))
					NextLocations.Add(new Point(x - 1, y));
				if (y - 1 >= 0 && !LastLocations.Any(p => (p.X == x && p.Y == y - 1)))
					NextLocations.Add(new Point(x, y - 1));
			}
			if (NextLocations.Count != 0)
				Rec_Pixel(NextLocations.Distinct().ToList(), Locations, BaseColor, percent);
		}
		
		public void Mask(FastBitmap MaskImage)
		{
			for (int y = 0; y < MaskImage.Height; y++)
				for (int x = 0; x < MaskImage.Width; x++)
				{
					if (MaskImage.GetPixel(x, y).Name != "ffffffff")
						SetPixel(x, y, Color.Transparent);
				}
		}

		public static FastBitmap Merge(params FastBitmap[] Screens)
		{
			for (int i = Screens.Length - 1; i > 0; i--)
				Screens[i - 1] = Merge(Screens[i - 1], Screens[i]);
			return Screens[0];
		}
		
		public static FastBitmap Merge(FastBitmap MainScreen, FastBitmap SubScreen)
		{
			Bitmap gbkn = new Bitmap(MainScreen.Width, MainScreen.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage((Image)gbkn);
			g.DrawImage(SubScreen, new Point(0, 0));
			g.DrawImage(MainScreen, new Point(0, 0));
			return gbkn;
		}

		/// <summary>
		/// Lock bitmap data
		/// </summary>
		public void LockBits()
		{
			if (AlreadyLocked)
				return;

			try
			{
				// Get width and height of bitmap
				Width = source.Width;
				Height = source.Height;

				// get total locked pixels count
				int PixelCount = Width * Height;

				// Create rectangle to lock
				Rectangle rect = new Rectangle(0, 0, Width, Height);

				// get source bitmap pixel format size
				Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

				// Check if bpp (Bits Per Pixel) is 8, 24, or 32
				if (Depth != 8 && Depth != 24 && Depth != 32)
				{
					throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
				}

				// Lock bitmap and return bitmap data
				bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
											 source.PixelFormat);

				// create byte array to copy pixel values
				int step = Depth / 8;
				Pixels = new byte[PixelCount * step];
				Iptr = bitmapData.Scan0;

				// Copy data from pointer to array
				Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
				AlreadyLocked = true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static implicit operator FastBitmap(Bitmap b)
		{
			FastBitmap fb = new FastBitmap(b);
			fb.LockBits();
			return fb;
		}

		public static implicit operator Bitmap(FastBitmap b)
		{
			b.UnlockBits();
			return b.source;
		}

		/// <summary>
		/// Unlock bitmap data
		/// </summary>
		public void UnlockBits()
		{
			try
			{
				if(AlreadyLocked)
				{
					// Copy data from byte array to pointer
					Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

					// Unlock bitmap data
					source.UnlockBits(bitmapData);
					AlreadyLocked = false;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}



		/// <summary>
		/// Get the color of the specified pixel
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Color GetPixel(int x, int y)
		{
			Color clr = Color.Empty;

			// Get color components count
			int cCount = Depth / 8;

			// Get start index of the specified pixel
			int i = ((y * Width) + x) * cCount;

			if (i > Pixels.Length - cCount)
				throw new IndexOutOfRangeException();

			if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
			{
				byte b = Pixels[i];
				byte g = Pixels[i + 1];
				byte r = Pixels[i + 2];
				byte a = Pixels[i + 3]; // a
				clr = Color.FromArgb(a, r, g, b);
			}
			if (Depth == 24) // For 24 bpp get Red, Green and Blue
			{
				byte b = Pixels[i];
				byte g = Pixels[i + 1];
				byte r = Pixels[i + 2];
				clr = Color.FromArgb(r, g, b);
			}
			if (Depth == 8)
			// For 8 bpp get color value (Red, Green and Blue values are the same)
			{
				byte c = Pixels[i];
				clr = Color.FromArgb(c, c, c);
			}
			return clr;
		}

		/// <summary>
		/// Set the color of the specified pixel
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(int x, int y, Color color)
		{
			// Get color components count
			int cCount = Depth / 8;

			// Get start index of the specified pixel
			int i = ((y * Width) + x) * cCount;

			if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
			{
				Pixels[i] = color.B;
				Pixels[i + 1] = color.G;
				Pixels[i + 2] = color.R;
				Pixels[i + 3] = color.A;
			}
			if (Depth == 24) // For 24 bpp set Red, Green and Blue
			{
				Pixels[i] = color.B;
				Pixels[i + 1] = color.G;
				Pixels[i + 2] = color.R;
			}
			if (Depth == 8)
			// For 8 bpp set color value (Red, Green and Blue values are the same)
			{
				Pixels[i] = color.B;
			}
		}
	}
}
