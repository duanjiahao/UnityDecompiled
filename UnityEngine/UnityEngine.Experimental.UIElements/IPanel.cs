using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IPanel
	{
		VisualContainer visualTree
		{
			get;
		}

		IDispatcher dispatcher
		{
			get;
		}

		IScheduler scheduler
		{
			get;
		}

		IDataWatchService dataWatch
		{
			get;
		}

		ContextType contextType
		{
			get;
		}

		BasePanelDebug panelDebug
		{
			get;
			set;
		}

		VisualElement Pick(Vector2 point);
	}
}
