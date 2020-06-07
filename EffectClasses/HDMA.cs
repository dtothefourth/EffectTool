using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace EffectClasses
{

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{Message}")]
	public class ASMException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public ASMException() : base() { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ASMException(string message) : base(message) { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ASMException(string message, Exception innerException)
			: base(message, innerException) { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public ASMException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{Message}")]
	public class RAMException : ASMException
	{
		/// <summary>
		/// 
		/// </summary>
		public RAMException() : base() { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public RAMException(string message) : base(message) { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public RAMException(string message, Exception innerException)
			: base(message, innerException) { }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public RAMException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class HDMAException : ASMException
	{
	}



	/// <summary>
	/// DMA/HDMA uses a 1 byte value to dertermine how to transfer the data.
	/// <para>The last 3 bits of which indicate the transfere mode. How often and where to write the data</para>
	/// <para>The values 6 and 7 are a repeate of 2 and 3 respectively</para>
	/// </summary>
	public enum DMAMode
	{
		/// <summary>
		/// 1 register write once
		/// </summary>
		P = 0,
		/// <summary>
		/// 2 registers write once
		/// <para>p, p</para>
		/// </summary>
		PP1 = 1,
		/// <summary>
		/// 1 register write twice
		/// <para>p, p+1</para>
		/// </summary>
		PP = 2,
		/// <summary>
		/// 2 registers write twice
		/// <para>p, p, p+1, p+1</para>
		/// </summary>
		PPP1P1 = 3,
		/// <summary>
		/// 4 register write once
		/// <para>p, p+1, p+2, p+3</para>
		/// </summary>
		PP1P2P3 = 4,
		/// <summary>
		/// 2 registers write twice alternate
		/// <para>p, p+1, p, p+1</para>
		/// </summary>
		PP1PP1 = 5,
	}
	/// <summary>
	/// 
	/// </summary>
	public enum DMADirection
	{
		/// <summary>
		/// 
		/// </summary>
		CPUtoPPU = 0,
		/// <summary>
		/// 
		/// </summary>
		PPUtoCPU = 0x80
	}
	
	/// <summary>
	/// The basic class all HDMA classes inherit from. It describes basic methods and members
	/// </summary>
	[Serializable]
	public abstract class HDMA : ICodeProvider
	{
		/// <summary>
		/// The normal amount of scanlines a SNES screen has
		/// </summary>
		public const int Scanlines = 224;

		public const string INITLabel = "init:";
		public const string MAINSeperator = "\t  \t\n";
		public const string MAINLabel = "main:";

		private int _channel = 3;
		private int _freeRAM = 0x7F9E00;

		/// <summary>
		/// Which Channel the HDMA will run in, assuming it has only one
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if a value out of the range of 0-7 is entered</exception>
		public int Channel
		{
			get { return _channel; }
			set
			{
				if (value < 0 || value > 7)
					throw new ArgumentOutOfRangeException("Channel cannot be smaller than 0 or bigger than 7");
				_channel = value;
			}
		}

		/// <summary>
		/// Whether the HDMA has a MAIN routine or just INIT
		/// </summary>
		public abstract bool UsesMain { get; }
		/// <summary>
		/// The free RAM address the HDMA stores date to in case it needs it.
		/// </summary>
		public int FreeRAM
		{
			get { return _freeRAM; }
			set
			{
				if (value < 0x7E0000 || value > 0x7FFFFF)
					throw new RAMException("Value not in RAM");
				_freeRAM = value;
			}
		}
		/// <summary>
		/// The table the HDMA effect is based on
		/// </summary>
		public HDMATable Table { get; set; }

		/// <summary>
		/// 
		/// </summary>
		protected HDMA()
		{
			Table = new HDMATable();
		}


		/// <summary>
		/// Sets the Channel according to a one out of three choice from RadioButtons
		/// </summary>
		/// <param name="Ch3">RadioButton that when checked sets channel 3</param>
		/// <param name="Ch4">RadioButton that when checked sets channel 4</param>
		/// <param name="Ch5">RadioButton that when checked sets channel 5</param>
		public virtual void SetChannel(System.Windows.Forms.RadioButton Ch3, System.Windows.Forms.RadioButton Ch4, System.Windows.Forms.RadioButton Ch5)
		{
			if (Ch3.Checked) Channel = 3;
			else if (Ch4.Checked) Channel = 4;
			else if (Ch5.Checked) Channel = 5;
			else
			{
				Ch3.Checked = true;
				Channel = 3;
			}
		}
		/// <summary>
		/// Sets the channel according to the slected value.
		/// </summary>
		/// <param name="Cmbox">The combobox that has the channel selected.</param>
		public virtual void SetChannel(System.Windows.Forms.ComboBox Cmbox)
		{
			this.Channel = Convert.ToInt32(Cmbox.Text);
		}
		
		/// <summary>
		/// Counts how many bytes of ROM the table takes up
		/// </summary>
		/// <returns></returns>
		public abstract int CountROMBytes();
		/// <summary>
		/// Counts how many bytes of RAM the table takes up
		/// </summary>
		/// <returns></returns>
		public abstract int CountRAMBytes();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual string Code() 
		{
			return Code(this.Channel, this.Table, RAM.SA1); 
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <returns></returns>
		public virtual string Code(int channel) 
		{
			return Code(channel, this.Table, RAM.SA1); 
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public virtual string Code(HDMATable table) 
		{ 
			return Code(this.Channel, table, RAM.SA1); 
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public abstract string Code(int channel, HDMATable table, bool sa1);



		/// <summary>
		/// Builds the mode value used for HDMA transfer. The value's stored to $43x0
		/// </summary>
		/// <param name="indirect">Used only in HDMA. Whether the table shown by the pointers has the values or addresses to the values
		/// <para><c>True</c> if the table has addresses</para></param>
		/// <param name="mode">How often and where bytes should be written to the register</param>
		/// <returns>The compiled byte</returns>
		public static int GetMode(bool indirect, DMAMode mode)
		{ return (int)mode + (indirect ? 0x40 : 0); }

		/// <summary>
		/// Builds the mode value used for DMA/HDMA transfer. The value's stored to $43x0
		/// </summary>
		/// <param name="direction">Which direction the transfer should go to/come from</param>
		/// <param name="indirect">Used only in HDMA. Whether the table shown by the pointers has the values or addresses to the values
		/// <para><c>True</c> if the table has addresses</para></param>
		/// <param name="increment">Used only in DMA. Whether the address will be incremented or decremented
		/// <c>True</c> if the address should be decremented</param>
		/// <param name="fixedtrans">When set, the address will not be adjusted</param>
		/// <param name="mode">How often and where bytes should be written to the register</param>
		/// <returns>The compiled byte</returns>
		public static int GetMode(DMADirection direction, bool indirect, bool increment, bool fixedtrans, DMAMode mode)
		{
			return (int)direction + (indirect ? 0x40 : 0) + (increment ? 0x20 : 0) + (fixedtrans ? 0x10 : 0) + (int)mode;
		}
	}
	
	public class ASMCodeBuilder
	{
		List<string> code = new List<string>();
		List<string> comment = new List<string>();
		List<string> label = new List<string>();

		List<int> open = new List<int>();
		List<int> close = new List<int>();
		int lineCounter = 0;

		const string Empty = "\x000";
		const string Tab = "   ";

		public void OpenNewBlock()
		{
			CloseBlock();
			open.Add(lineCounter);
		}

		public void CloseBlock()
		{
			//If none has been opened...
			if (open.Count == 0)
				return;

			//if close is called on the same line as open,
			//just remove the opening command.
			if (open.Last() == lineCounter)
				open.Remove(open.Count - 1);

			//only add closeblock if we are in a block.
			if(close.Count < open.Count)
				close.Add(lineCounter - 1);
		}

		public void AppendCode(string code)
		{
			this.code.Add(code);
			this.comment.Add("");
			this.label.Add(Empty);
			lineCounter++;
		}

		public void AppendCode(string code, string comment)
		{
			this.code.Add(code);
			this.comment.Add(comment);
			this.label.Add(Empty);
			lineCounter++;
		}

		public void AppendCommentLine(string comment)
		{
			if (close.Count != open.Count)
				throw new InvalidOperationException("Not possible while inside an open block.");

			string[] arr = comment.Split('\n');

			foreach (string s in arr)
			{
				this.code.Add(Empty);
				this.comment.Add(s);
				this.label.Add(Empty);
				lineCounter++;
			}
		}

		public void AppendComment(string comment)
		{
			this.code.Add("");
			this.comment.Add(comment);
			this.label.Add(Empty);
			lineCounter++;
		}

		public void AppendEmptyLine()
		{
			this.code.Add(Empty);
			this.comment.Add(Empty);
			this.label.Add(Empty);
			lineCounter++;
		}

		public void AppendLabel(string label)
		{
			this.code.Add(Empty);
			this.comment.Add("");
			//this.label.Add("." + label.Trim('.', ':'));
			this.label.Add(label.Trim(':') + ":");
			lineCounter++;
		}

		public void AppendLabel(string label, string comment)
		{
			this.code.Add(Empty);
			this.comment.Add(comment);
			//this.label.Add("." + label.Trim('.', ':'));
			this.label.Add(label.Trim(':') + ":");
			lineCounter++;
		}

		public void AppendTable(HDMATable table)
		{
			AppendLabel(table.Name);
			foreach (var entry in table)
				AppendCode(entry.ToString());
		}

		public void AppendTable(ASMTable table)
		{
			AppendLabel(table.Name);
			foreach (var entry in table)
				AppendCode(entry.ToString());
		}

		private string GetLine(string afterSemiColon, int index, int codePadding)
		{
			if (label[index] == Empty && code[index] == Empty && comment[index] == Empty)   //empty line
				return "";
			if (label[index] == Empty && code[index] == Empty)  //comment line
				return ";" + comment[index];
			if (label[index] == Empty)   //normal line
			   return Tab + code[index].PadRight(codePadding) + ";" + afterSemiColon + comment[index];

			//labels.
			return label[index].PadRight(codePadding + Tab.Length) + ";" + afterSemiColon + comment[index];
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			int openCount = 0;
			int closeCount = 0; 


			int longestCode = code.Max(c => c.Length) + Tab.Length;

			for(int i = 0; i < lineCounter; i++)
			{
				if (openCount != open.Count && open[openCount] == i)
				{
					sb.AppendLine(GetLine("\\  ", i, longestCode));
					openCount++;
				}
				else if (closeCount != close.Count && close[closeCount] == i)
				{
					sb.AppendLine(GetLine("/  ", i, longestCode));
					closeCount++;
				}
				else if(closeCount != openCount)
					sb.AppendLine(GetLine(" | ", i, longestCode));
				else
					sb.AppendLine(GetLine(" ", i, longestCode));
			}

			return sb.ToString();
		}
	}
}
