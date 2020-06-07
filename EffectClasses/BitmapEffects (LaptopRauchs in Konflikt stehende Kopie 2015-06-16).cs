using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Extansion.Int;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace EffectClasses
{
	public enum Orientation { Left = 0x01, Right = 0x03, Up = 0x02, Down = 0x00 };
	
	public static class BitmapEffects
	{
		/// <summary>
		/// Puts images ontop of each other and the later images will only be visible through transparent parts of the former images
		/// </summary>
		/// <param name="Images">The array containing all the images that will be overlapped. The former indizes will be ontop.</param>
		/// <returns>The merged image</returns>
		public static Bitmap OverlapImages(params Bitmap[] images)
		{            
			Bitmap final = new Bitmap(images.Last());

			using (Graphics g = Graphics.FromImage(final))
			{
				g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
				for (int i = images.Length - 2; i >= 0; i--)
					if(images[i] != null)
						g.DrawImageUnscaled(images[i], 0, 0);
			}

			return final;

		}
		/// <summary>
		/// Puts two images ontop of each other. Only where top is transparent bottom can be seen
		/// </summary>
		/// <param name="top">The Bitmap to put ontop of the other.</param>
		/// <param name="bottom">The Bitmap to put below the other.</param>
		/// <returns>A combined Bitmap of the given paramteres</returns>
		public static Bitmap SingleOverlapImages(Bitmap top, Bitmap bottom)
		{
			if (top == null)
				return bottom;
			if (bottom == null)
				return top;

			Bitmap final = new Bitmap(bottom);
			using(Graphics g = Graphics.FromImage(final))
			{
				g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
				g.DrawImageUnscaled(top, 0, 0);
			}

			return final;
		}

		/// <summary>
		/// Adds every pixel of the images with each corresponding pixel.
		/// </summary>
		/// <param name="images">The array of images that will be added together.</param>
		/// <returns></returns>
		public static Bitmap MergeImages_(params Bitmap[] images)
		{
			if (images == null || images.Length == 0)
				return null;
			if (images.Length == 1)
				return images[0];

			int nullPointer = 0;
			for (int i = 0; i < images.Length; i++)
			{
				if (images[i] == null)
					nullPointer++;
				else
					break;
			}

			Size checkSize = images[nullPointer].Size;
			PixelFormat checkFormat = images[nullPointer].PixelFormat;

			if (images.Skip(nullPointer + 1).Any(im => im != null && (im.PixelFormat != checkFormat || im.Size != checkSize)))
				throw new ArgumentException("All images need to have the same size and pixel format");

			//make a new bitmap to return later (copy of top)
			Bitmap save = new Bitmap(images[nullPointer]);
			BitmapData saveData = save.LockBits(new Rectangle(0, 0, save.Width, save.Height), ImageLockMode.ReadWrite, save.PixelFormat);
			byte[] saveArray = new byte[saveData.Stride * save.Height];
			Marshal.Copy(saveData.Scan0, saveArray, 0, saveArray.Length);

			BitmapData data = new BitmapData();
			byte[] array = new byte[0];

			for (int i = nullPointer + 1; i < images.Length; i++)
			{
				if (images[i] == null)
					continue;

				data = images[i].LockBits(new Rectangle(0, 0, images[i].Width, images[i].Height),
					ImageLockMode.ReadWrite, images[i].PixelFormat);
				array = new byte[data.Stride * images[i].Height];
				Marshal.Copy(data.Scan0, array, 0, array.Length);

				for (int p = 0; p < array.Length; p++)
				{
					saveArray[p] = (byte)(saveArray[p] + array[p]).Max(255);
				}
				images[i].UnlockBits(data);
			}

			Marshal.Copy(saveArray, 0, saveData.Scan0, saveArray.Length);
			save.UnlockBits(saveData);
			return save;                                          //return the new Bitmap
		}



		/// <summary>
		/// Adds every pixel of the images with each corresponding pixel.
		/// </summary>
		/// <param name="Images">The array of images that will be added together.</param>
		/// <returns></returns>
		public static Bitmap MergeImages(params Bitmap[] Images)
		{
			Bitmap b = Images[0];
			for (int i = 1; i < Images.Length; i++)
				b = SingleMergeImages(b, Images[i]);
			return b;
		}
		private static Bitmap SingleMergeImages(Bitmap Merg1, Bitmap Merg2)
		{
			if (Merg1 == null)
				return Merg2;
			if (Merg2 == null)
				return Merg1;

			//make a new bitmap to return later (copy of top)
			Bitmap save = new Bitmap(Merg1);

			if (Merg1.PixelFormat != Merg2.PixelFormat)
				throw new ArgumentException("Images need same format.");

			//next few lines is locking bitmaps and getting their data.
			BitmapData Data1 = save.LockBits(new Rectangle(0, 0, save.Width, save.Height), ImageLockMode.ReadWrite, save.PixelFormat);
			byte[] Array1 = new byte[Data1.Stride * save.Height];
			Marshal.Copy(Data1.Scan0, Array1, 0, Array1.Length);

			BitmapData Data2 = Merg2.LockBits(new Rectangle(0, 0, Merg2.Width, Merg2.Height), ImageLockMode.ReadWrite, Merg2.PixelFormat);
			byte[] Array2 = new byte[Data2.Stride * Merg2.Height];
			Marshal.Copy(Data2.Scan0, Array2, 0, Array2.Length);

			//run through all the bytes returned of the image. (format bgrA) start with 3 and increase by 4 to only check A
			for (int p = 0; p < Array1.Length; p++)
				Array1[p] = (byte)((Array1[p] + Array2[p]).Max(255));

			Marshal.Copy(Array1, 0, Data1.Scan0, Array1.Length);  //copy changed data back
			save.UnlockBits(Data1);                               //unlock bits and store data back
			Merg2.UnlockBits(Data2);
			return save;                                          //return the new Bitmap
		}

		/// <summary>
		/// Invertes the colors of a bitmap
		/// </summary>
		/// <param name="source">The Bitmap to be inverted</param>
		/// <returns>The inverted Bitmap</returns>
		public static Bitmap Invert(Bitmap source)
		{
			//create a blank bitmap the same size as original
			Bitmap newBitmap = new Bitmap(source.Width, source.Height);

			//get a graphics object from the new image
			Graphics g = Graphics.FromImage(newBitmap);

			// create the negative color matrix
			ColorMatrix colorMatrix = new ColorMatrix(new float[][]
			{
				new float[] {-1, 0, 0, 0, 0},
				new float[] {0, -1, 0, 0, 0},
				new float[] {0, 0, -1, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {1, 1, 1, 0, 1}
			});

			// create some image attributes
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(colorMatrix);

			g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
						0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);

			//dispose the Graphics object
			g.Dispose();

			return newBitmap;
		}

		public static Bitmap BlackWhite(Bitmap source, float threshold)
		{
			Bitmap sourceImage = new Bitmap(source);
			using (Graphics gr = Graphics.FromImage(sourceImage)) // SourceImage is a Bitmap object
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
				var rc = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
				gr.DrawImage(sourceImage, rc, 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, ia);
			}
			return sourceImage;
		}


		public static Bitmap ApplyMask(Bitmap source, Bitmap mask, float threshold)
		{
			if (source == null)
				throw new ArgumentNullException("source Bitmap mustn't be null.");
			if (mask == null)
				throw new ArgumentNullException("mask Bitmap mustn't be null.");

			Bitmap blackWhite = BlackWhite(mask, threshold);
			
			//make a new bitmap to return later (copy of top)
			Bitmap save = new Bitmap(source);

			if (source.PixelFormat != blackWhite.PixelFormat)
				throw new ArgumentException("Images need same format.");

			//next few lines is locking bitmaps and getting their data.
			BitmapData dataSave = save.LockBits(new Rectangle(0, 0, save.Width, save.Height),
				ImageLockMode.ReadWrite, save.PixelFormat);
			byte[] arraySave = new byte[dataSave.Stride * save.Height];
			Marshal.Copy(dataSave.Scan0, arraySave, 0, arraySave.Length);

			BitmapData Data2 = blackWhite.LockBits(new Rectangle(0, 0, blackWhite.Width, blackWhite.Height),
				ImageLockMode.ReadWrite, blackWhite.PixelFormat);
			byte[] arrayBlackWhite = new byte[Data2.Stride * blackWhite.Height];
			Marshal.Copy(Data2.Scan0, arrayBlackWhite, 0, arrayBlackWhite.Length);

			//run through all the bytes returned of the image. (format bgrA) start with 3 and increase by 4 to only check A
			for (int p = 3; p < arrayBlackWhite.Length; p += 4)
				if (arrayBlackWhite[p - 1] == 0)
					arraySave[p] = 0;

			Marshal.Copy(arraySave, 0, dataSave.Scan0, arraySave.Length);  //copy changed data back
			save.UnlockBits(dataSave);   //unlock bits and store data back
			return save;                 //return the new Bitmap
		}


		public static Bitmap PixelLine(int height, int pixelealtion, int offset, Bitmap original)
		{
			if (height < 0 || offset < 0)           // if height or offset is negative.
				return original;                    // Return
			if (pixelealtion == 1)
				return original;
			if (height + offset > original.Height)
				height = original.Height - offset;

			if (original.PixelFormat != PixelFormat.Format32bppArgb)
				throw new ArgumentException("Bitmap requires PixelFormat Format32bppArgb");

			Bitmap map = new Bitmap(original);

			//Get BitmapData of the whole image by locking the bits.
			BitmapData data = original.LockBits(new Rectangle(new Point(0, 0), original.Size), ImageLockMode.ReadWrite, original.PixelFormat);
			BitmapData newData = map.LockBits(new Rectangle(new Point(0, 0), original.Size), ImageLockMode.ReadWrite, original.PixelFormat);

			byte[] bytes = new byte[original.Height * data.Stride]; //and a byte array having room for all the bytes
			byte[] newArray = new byte[original.Height * data.Stride];

			Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);    //Copy all the bytes from the image in the array
			Marshal.Copy(data.Scan0, newArray, 0, bytes.Length);    //Copy all the bytes from the image in the array
			
			//(format bgrA)

			//loop for current line of fat pixels
			int heightSave = height;
			for (int topLeftLine = offset * data.Stride; topLeftLine < (offset * data.Stride) + (heightSave * data.Stride); topLeftLine += pixelealtion * data.Stride, height -= pixelealtion)
			{
				int breakMark = topLeftLine + data.Stride;

				//loop for current fat pixel
				for (int topLeftPixel = topLeftLine; topLeftPixel < breakMark; topLeftPixel += pixelealtion * 4)
				{
					int breakMarkCur = breakMark;
					int topLeftPixelCur = topLeftPixel;

					//values each pixel gets set to
					byte b = bytes[topLeftPixelCur];
					byte g = bytes[topLeftPixelCur + 1];
					byte r = bytes[topLeftPixelCur + 2];
					byte A = bytes[topLeftPixelCur + 3];

					//loop for all the lines of the current pixel
					for (int Cnt = 0; Cnt < height.Max(pixelealtion); topLeftPixelCur += data.Stride, breakMarkCur += data.Stride, Cnt++)
					{
						//loop for the current line of the current pixel
						for (int curPixel = topLeftPixelCur; curPixel < breakMarkCur && curPixel < pixelealtion * 4 + topLeftPixelCur; curPixel += 4)
						{
							newArray[curPixel] = b;
							newArray[curPixel + 1] = g;
							newArray[curPixel + 2] = r;
							newArray[curPixel + 3] = A;
						}
					}
				}
			}
			
			Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
			Marshal.Copy(newArray, 0, newData.Scan0, newArray.Length);
			original.UnlockBits(data);
			map.UnlockBits(newData);

			return map;
		}


		public Bitmap Render(Bitmap image, double percent, params Point[] points)
		{
			Bitmap bm = new Bitmap(image);
			
			//Get BitmapData of the whole image by locking the bits.
			BitmapData data = image.LockBits(new Rectangle(new Point(0, 0), image.Size),
				ImageLockMode.ReadWrite, image.PixelFormat);

			//and a byte array having room for all the bytes
			byte[] bytes = new byte[image.Height * data.Stride]; 

			Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);    //Copy all the bytes from the image in the array
			

			foreach (Point p in points)
			{
				int x = p.X, y = p.Y;
				
				int offset = (y * bm.Width + x) * 4;
				byte b = bytes[offset];
				byte g = bytes[offset + 1];
				byte r = bytes[offset + 2];
				byte a = bytes[offset + 3];

				Color basecolor = Color.FromArgb(a, r, g, b);
				recPixelRender(data, ref bytes, new List<Point>() { new Point(x, y) }, new List<Point>(), basecolor, percent);
			}

			Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
			bm.UnlockBits(data);
			return bm;
		}
		
		private void recPixelRender(BitmapData data, ref byte[] bytes, 
			List<Point> Locations, List<Point> LastLocations, Color BaseColor, double percent)
		{
			List<Point> NextLocations = new List<Point>();
			foreach (Point Loc in Locations)
			{
				int x = Loc.X, y = Loc.Y;
				
				if (!MatchesColor(GetPixel(data, bytes, x, y), BaseColor, percent))
					continue;

				SetPixel(data, ref bytes, x, y, Color.FromArgb(0, BaseColor.G, BaseColor.B, BaseColor.R));

				if (x + 1 < data.Width && !LastLocations.Any(p => (p.X == x + 1 && p.Y == y)))
					NextLocations.Add(new Point(x + 1, y));
				if (y + 1 < data.Height && !LastLocations.Any(p => (p.X == x && p.Y == y + 1)))
					NextLocations.Add(new Point(x, y + 1));
				if (x - 1 >= 0 && !LastLocations.Any(p => (p.X == x - 1 && p.Y == y)))
					NextLocations.Add(new Point(x - 1, y));
				if (y - 1 >= 0 && !LastLocations.Any(p => (p.X == x && p.Y == y - 1)))
					NextLocations.Add(new Point(x, y - 1));
			}
			if (NextLocations.Count != 0)
				recPixelRender(data, ref bytes,
					NextLocations.Distinct().ToList(), Locations, BaseColor, percent);
		}

		private static bool MatchesColor(Color main, Color compare, double percentage)
		{ 
			double com = 2.55 * percentage;
			return (//compare.A >= (main.A - com) && compare.A <= (main.A + com) &&
				compare.R >= (main.R - com) && compare.R <= (main.R + com) &&
				compare.G >= (main.G - com) && compare.G <= (main.G + com) &&
				compare.B >= (main.B - com) && compare.B <= (main.B + com));
		}

		/// <summary>
		/// Gets the color of a pixel for a locked bitmap.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="bytes"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Color GetPixel(BitmapData data, byte[] bytes, int x, int y)
		{
			if (x >= data.Width || x < 0 || y >= data.Height || y < 0)
				throw new IndexOutOfRangeException("x/y not inside the available field.");

			//bytes per pixel
			int bpp = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8;
			//the start pointer
			int pt = (y * data.Width + x) * bpp;


			if (bpp == 4) // For 32 bpp get Red, Green, Blue and Alpha
			{
				byte b = bytes[pt];
				byte g = bytes[pt + 1];
				byte r = bytes[pt + 2];
				byte a = bytes[pt + 3]; // a
				return Color.FromArgb(a, r, g, b);
			}
			if (bpp == 3) // For 24 bpp get Red, Green and Blue
			{
				byte b = bytes[pt];
				byte g = bytes[pt + 1];
				byte r = bytes[pt + 2];
				return Color.FromArgb(r, g, b);
			}
			if (bpp == 1)
			// For 8 bpp get color value (Red, Green and Blue values are the same)
			{
				byte c = bytes[pt];
				return Color.FromArgb(c, c, c);
			}
			return Color.Transparent;
		}

		public static void SetPixel(BitmapData data, ref byte[] bytes, int x, int y, Color color)
		{
			//bytes per pixel
			int bpp = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8;
			//the start pointer
			int pt = (y * data.Width + x) * bpp;
			
			if (bpp == 4) // For 32 bpp set Red, Green, Blue and Alpha
			{
				bytes[pt] = color.B;
				bytes[pt + 1] = color.G;
				bytes[pt + 2] = color.R;
				bytes[pt + 3] = color.A;
			}
			if (bpp == 3) // For 24 bpp set Red, Green and Blue
			{
				bytes[pt] = color.B;
				bytes[pt + 1] = color.G;
				bytes[pt + 2] = color.R;
			}
			if (bpp == 1)
			// For 8 bpp set color value (Red, Green and Blue values are the same)
			{
				bytes[pt] = color.B;
			}
		}

		/// <summary>
		/// Creates a new Bitmap with defined size filled with a certain color.
		/// </summary>
		/// <param name="color">The color the Bitmap should be filled with.</param>
		/// <param name="size">The size of the Bitmap</param>
		/// <returns>A Bitmap filled with the desired color</returns>
		public static Bitmap FromColor(Color color, Size size)
		{
			Bitmap b = new Bitmap(size.Width, size.Height);

			using (Graphics g = Graphics.FromImage(b))
				g.FillRectangle(new SolidBrush(color), new Rectangle(0, 0, size.Width, size.Height));

			return b;
		}

		/// <summary>
		/// Creates a new Bitmap with defined size filled with a certain color.
		/// </summary>
		/// <param name="color">The color the Bitmap should be filled with.</param>
		/// <param name="width">The width of the Bitmap</param>
		/// <param name="height">The height of the Bitmap</param>
		/// <returns>A Bitmap filled with the desired color</returns>
		public static Bitmap FromColor(Color color, int width, int height)
		{
			return FromColor(color, new Size(width, height));
		}

	
		/// <summary>
		/// Rotiert einen bestimmten Bereich einer Bitmap nach links und 
		/// </summary>
		/// <param name="initHeight">Die Y Höhe ab der die Zeilen verschoben werden</param>
		/// <param name="bandwidth">Wie viele Zeilen verschoben werden sollen</param>
		/// <param name="moving">Um wie viele Pixel die Bitmap nach links verschoben werden soll</param>
		/// <param name="original">Die Bitmap die verschoben werden soll</param>
		/// <param name="direction">Die Richtung in die sich der ausgewählte Teil bewegen soll</param>
		/// <returns>Die neue Bitmap mit der entsprechend verschobenen Linien</returns>
		public static Bitmap MoveLine(int initHeight, int bandwidth, int moving, Bitmap original, Orientation direction)
		{
			if (moving == 0 || bandwidth == 0)                      // \ Wenn der zu verschiebende Streifen keine Breite hat
				return new Bitmap(original);                        // / oder um keine Pixel verschoben werden soll, kann das Originalbild zurückgegeben werden.


			if (initHeight < 0 || bandwidth < 0)    // if height or offset is negative.
				return new Bitmap(original);        // Return

			// Left & Right
			if ((int)direction % 2 == 1)
			{
				if (direction == Orientation.Right)
					moving *= -1;
				return MoveLineLeft(initHeight, bandwidth, moving, original);
			}
			// Up & Down
			else
			{
				moving = moving % original.Height;
				if (moving < 0) 
				{                       
					moving *= -1;                         
					direction = (Orientation)((int)direction + 2 % 4); 
				}
				if (direction == Orientation.Up)
					moving *= -1;

				//down
				Bitmap bm = new Bitmap(original);
				using (Bitmap doubleImg = new Bitmap(original.Width, original.Height * 2, original.PixelFormat))
				{
					using (Graphics g = Graphics.FromImage(doubleImg))
					{
						Rectangle rec = new Rectangle(new Point(0, 0), bm.Size);
						g.DrawImageUnscaled(original, rec);
						rec.Y = bm.Height;
						g.DrawImageUnscaled(original, rec);
					}
					using (Graphics g = Graphics.FromImage(bm))
					{
						Rectangle copy = new Rectangle(0, (initHeight + moving).Min(0), bm.Width, bandwidth);
						Rectangle paste = new Rectangle(0, initHeight, bm.Width, bandwidth);
						g.SetClip(paste);
						g.Clear(Color.Transparent);
						g.ResetClip();
						using (Bitmap temp = doubleImg.Clone(copy, bm.PixelFormat))
							g.DrawImageUnscaledAndClipped(temp, paste);
					}
				}
				return bm;
			}
		}

		/// <summary>
		/// Moves a line to the right in an image and loops it back at the other side.
		/// </summary>
		/// <param name="offset">The line to start the moving in. Start counting from the top</param>
		/// <param name="height">The height of the line to be moved in pixel</param>
		/// <param name="pixel">The number of pixel to move the line to the right</param>
		/// <param name="original">The image that will have a line moved</param>
		private static Bitmap MoveLineLeft(int offset, int height, int pixel, Bitmap original)
		{            
			pixel = pixel % original.Width;         //modole division, so that you cannot shift more pixels than the image is wide
			if (pixel == 0)                         //if no pixels are to be shifted, just end there.
				return new Bitmap(original);
			if (pixel < 0)                          // pixel = negative is the same as moving in the other direction
				pixel = original.Width + pixel;     // thus, we just overflow the moving in the right direction (+ because pixel is negative !!!)

			if (height + offset > original.Height)  //the offset for the moving line + the height cannot be bigger than the image itself
				height = original.Height - offset;  //thus the height is changed to just move the remaining image.


			Bitmap retBitmap = new Bitmap(original);

			//Get BitmapData of the whole image by locking the bits.
			BitmapData data = original.LockBits(new Rectangle(new Point(0, 0), original.Size), ImageLockMode.ReadWrite, original.PixelFormat);
			BitmapData retData = retBitmap.LockBits(new Rectangle(new Point(0, 0), original.Size), ImageLockMode.ReadWrite, original.PixelFormat);

			IntPtr pt = data.Scan0;                                 //get pointer for the beginning of the data
			byte[] bytes = new byte[original.Height * data.Stride]; //and a byte array having room for all the bytes

			int bytepp = data.Stride / original.Width;                                  //Byte Per Pixel (stride is the byte per row, width is the pixel per row)
			System.Runtime.InteropServices.Marshal.Copy(pt, bytes, 0, bytes.Length);    //Copy all the bytes from the image in the array

			byte[] retBytes = new byte[bytes.Length];           // New byte array which will hold the new image
			Array.Copy(bytes, retBytes, retBytes.Length);       // With the same data as the old array.

			//loop through every row.
			for (int row = 0; row < height; row++)
			{
				int y = offset + row;                                   // Y position of the first pixel to get (in a 
				int x = 0;                                              // X position of the first pixel to get 
				int indexSource = ((y * original.Width) + x) * bytepp;  // calc index from X and Y

				x = original.Width - pixel;                             // X position of where the first pixel should be put (Y still the same)
				int indexDest = ((y * original.Width) + x) * bytepp;    // calc new index from new X and Y

				Array.Copy(bytes, indexSource, retBytes, indexDest, bytepp * pixel);    //copy from array to new array the number of pixels times bytes per pixel.
			}
			for (int row = 0; row < height; row++)
			{
				int y = offset + row;
				int x = pixel;
				int indexSource = ((y * original.Width) + x) * bytepp;

				x = 0;
				int indexDest = ((y * original.Width) + x) * bytepp;

				Array.Copy(bytes, indexSource, retBytes, indexDest, bytepp * (original.Width - pixel));
			}

			System.Runtime.InteropServices.Marshal.Copy(retBytes, 0, retData.Scan0, retBytes.Length);
			original.UnlockBits(data);
			retBitmap.UnlockBits(retData);
			return retBitmap;
		}
	}

	[Serializable]
	[DebuggerDisplay("{Name}")]
	public class BitmapCollection : ICloneable, IEnumerable<Bitmap>
	{		
		/// <summary>
		/// The first object layer. BG2/BG3/BG4 will appear behind it. Used in any BG Mode
		/// </summary>
		public Bitmap BG1 { get; set; }
		/// <summary>
		/// The second object layer BG3/BG4 will appear behind it. Used in BG Mode 0 - 5
		/// </summary>
		public Bitmap BG2 { get; set; }
		/// <summary>
		/// The third object layer. BG4 will appear behind it. Used in BG Mode 0 and 1
		/// </summary>
		public Bitmap BG3 { get; set; }
		/// <summary>
		/// The fourth and last object layer. Only used in BG Mode 0
		/// </summary>
		public Bitmap BG4 { get; set; }
		/// <summary>
		/// The sprite layer. All other BGs will appear behind it
		/// </summary>
		public Bitmap OBJ { get; set; }
		/// <summary>
		/// The name of the collection
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The solid color at the very last layer.
		/// </summary>
		public Color FixedColor { get; set; }
		
		public override string ToString()
		{
			return Name;
		}

		public object Clone()
		{
			BitmapCollection col = new BitmapCollection();
			col.BG1 = new Bitmap(this.BG1);
			col.BG2 = new Bitmap(this.BG2);
			col.BG3 = new Bitmap(this.BG3);
			col.BG4 = new Bitmap(this.BG4);
			col.OBJ = new Bitmap(this.OBJ);
			col.Name = this.Name;
			col.FixedColor = this.FixedColor;

			return col;
		}

		private static BinaryFormatter formatter = new BinaryFormatter();

		/// <summary>
		/// Saves a BitmapCollection to file.
		/// </summary>
		/// <param name="Collection">The collection to save</param>
		/// <param name="FilePath">The path to save to</param>
		public static void Save(BitmapCollection Collection, string FilePath)
		{
			using(FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
				formatter.Serialize(stream, Collection);
			
		}

		/// <summary>
		/// Loads a BitmapCollection from a file.
		/// </summary>
		/// <param name="FilePath">The path to the file to open</param>
		/// <returns>The opened BitmapCollection</returns>
		public static BitmapCollection Load(string FilePath)
		{
			using (FileStream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				return (BitmapCollection)formatter.Deserialize(stream);
		}

		/// <summary>
		/// Loads a BitmapCollection from a file.
		/// </summary>
		/// <param name="FilePath">The path to the file to open</param>
		/// <returns>The opened BitmapCollection</returns>
		public static BitmapCollection Load(byte[] FileContent)
		{
			using (MemoryStream stream = new MemoryStream(FileContent))
				return (BitmapCollection)formatter.Deserialize(stream);
		}


		public IEnumerator<Bitmap> GetEnumerator()
		{
			yield return BG1;
			yield return BG2;
			yield return BG3;
			yield return BG4;
			yield return OBJ;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
