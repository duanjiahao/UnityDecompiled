using System;
using System.Runtime.Serialization;

namespace UnityEngine
{
	[Serializable]
	public class MissingComponentException : SystemException
	{
		private const int Result = -2147467261;

		private string unityStackTrace;

		public MissingComponentException() : base("A Unity Runtime error occurred!")
		{
			base.HResult = -2147467261;
		}

		public MissingComponentException(string message) : base(message)
		{
			base.HResult = -2147467261;
		}

		public MissingComponentException(string message, Exception innerException) : base(message, innerException)
		{
			base.HResult = -2147467261;
		}

		protected MissingComponentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
