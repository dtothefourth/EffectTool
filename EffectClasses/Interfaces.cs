using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EffectClasses
{
	public interface ICodeProvider
	{
		string Code();
	}

	public class AbordException : Exception
	{
		public AbordException() { }
		public AbordException(string message) : base(message) { }
		public AbordException(string message, Exception innerException) : base(message, innerException) { }
		public AbordException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
