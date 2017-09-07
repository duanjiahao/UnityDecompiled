using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IDataWatchService
	{
		IDataWatchHandle AddWatch(VisualElement watcher, UnityEngine.Object watched, Action OnDataChanged);
	}
}
