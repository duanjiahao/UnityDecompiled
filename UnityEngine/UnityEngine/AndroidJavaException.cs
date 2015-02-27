using System;
namespace UnityEngine
{
	public sealed class AndroidJavaException : Exception
	{
		internal AndroidJavaException(string message) : base(message)
		{
		}
	}
}
