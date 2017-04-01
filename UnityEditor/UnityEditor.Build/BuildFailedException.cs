using System;
using UnityEngine.Scripting;

namespace UnityEditor.Build
{
	[RequiredByNativeCode]
	public class BuildFailedException : Exception
	{
		public BuildFailedException(string message) : base(message)
		{
		}

		public BuildFailedException(Exception innerException) : base(null, innerException)
		{
		}

		[RequiredByNativeCode]
		private Exception BuildFailedException_GetInnerException()
		{
			return base.InnerException;
		}
	}
}
