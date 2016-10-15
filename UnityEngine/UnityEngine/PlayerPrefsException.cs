using System;

namespace UnityEngine
{
	public sealed class PlayerPrefsException : Exception
	{
		public PlayerPrefsException(string error) : base(error)
		{
		}
	}
}
