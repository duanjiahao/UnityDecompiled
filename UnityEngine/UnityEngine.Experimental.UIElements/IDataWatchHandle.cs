using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IDataWatchHandle : IDisposable
	{
		UnityEngine.Object watched
		{
			get;
		}

		bool disposed
		{
			get;
		}
	}
}
