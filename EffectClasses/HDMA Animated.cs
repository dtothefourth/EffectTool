using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EffectClasses
{
	public class AnimationExceptionEventArgs : EventArgs
	{
		public Exception Exception { get; set; }
		public AnimationExceptionEventArgs(Exception ex)
		{
			Exception = ex;
		}
	}
	public delegate void AnimationExceptionOccured(object sender, AnimationExceptionEventArgs e);

	public delegate void AnimatedImage(Image img);

	public abstract class AnimatedHDMA : HDMA
	{
		/// <summary>
		/// The Image used for the effect
		/// </summary>
		public Bitmap Original { get; set; }

		/// <summary>
		/// Which Frame we're currently running.
		/// </summary>
		public int Frame { get; set; }

		/// <summary>
		/// Whether or not the code uses the MAIN routine of uberASM.
		/// </summary>
		public override bool UsesMain { get { return true; } }

		/// <summary>
		/// Event that occures when an exception happens during the animation.
		/// This is recommended to be used to cancel the animation.
		/// </summary>
		public event AnimationExceptionOccured AnimationException;

		public int FrameRate
		{
			get { return 1000 / _t.Interval; }
			set
			{
				if (value <= 0)
					throw new InvalidProgramException("Framerate must be positive and more than 0");
				if(value > 100)
					throw new InvalidProgramException("Framerate is limited to 100 fps");
				_t.Interval = 1000 / value;
			}
		}

		protected AnimatedImage _anImge;

		private System.Windows.Forms.Timer _t;

		public AnimatedHDMA()
		{
			_t = new System.Windows.Forms.Timer();
			_t.Interval = 16;
			_t.Tick += _t_Tick;
		}
		public AnimatedHDMA(Bitmap original)
			: this()
		{
			Original = original;
		}

		protected virtual void _t_Tick(object sender, EventArgs e)
		{
			if (_anImge != null && Original != null)
				_anImge(NextAnimateFrame(Original));
		}

		
		/// <summary>
		/// Creates a base image with the preset setting of Amplitede and Width using the Original member
		/// </summary>
		/// <returns>The according to the settings disordered image</returns>
		public virtual Bitmap StaticPic()
		{
			return StaticPic(Original);
		}

		/// <summary>
		/// Creates a Bitmap depending on the settings and the internal counter, which also get's increased at the end using the Original Member.
		/// </summary>
		/// <returns>Depending on the settings moved image.</returns>
		public virtual Bitmap NextAnimateFrame()
		{
			return NextAnimateFrame(Original);
		}		

		/// <summary>
		/// Creates a base image with the preset setting of Amplitede and Width
		/// </summary>
		/// <param name="basePic">The base for the disordered image</param>
		/// <returns>The according to the settings disordered image</returns>
		public abstract Bitmap StaticPic(Bitmap basePic);

		/// <summary>
		/// Creates a Bitmap depending on the settings and the internal counter, which also get's increased at the end.
		/// </summary>
		/// <param name="basePic">The still (unchanged) image.</param>
		/// <returns>Depending on the settings moved image.</returns>
		public abstract Bitmap NextAnimateFrame(Bitmap basePic);

		public abstract void Reset();

		/// <summary>
		/// Starts up the animation with a desired method as to what to do with the images for the animation.
		/// </summary>
		/// <param name="imgFunc"></param>
		public virtual void StartAnimation(AnimatedImage imgFunc)
		{
			_anImge = imgFunc;
			_t.Start();
		}

		/// <summary>
		/// Halts the animation
		/// </summary>
		public virtual void StopAnimation()
		{
			_t.Stop();
		}

		protected virtual void ThrowExceptionEvent(Exception ex)
		{
			if (AnimationException != null)
				AnimationException(this, new AnimationExceptionEventArgs(ex));
		}

	}
}
