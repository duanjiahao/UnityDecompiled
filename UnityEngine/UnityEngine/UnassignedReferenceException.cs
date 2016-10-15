using System;
using System.Runtime.Serialization;

namespace UnityEngine
{
	[Serializable]
	public class UnassignedReferenceException : SystemException
	{
		private const int Result = -2147467261;

		private string unityStackTrace;

		public UnassignedReferenceException() : base("A Unity Runtime error occurred!")
		{
			base.HResult = -2147467261;
		}

		public UnassignedReferenceException(string message) : base(message)
		{
			base.HResult = -2147467261;
		}

		public UnassignedReferenceException(string message, Exception innerException) : base(message, innerException)
		{
			base.HResult = -2147467261;
		}

		protected UnassignedReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
