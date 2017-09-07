using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class BasePanelDebug
	{
		internal bool enabled
		{
			get;
			set;
		}

		internal Func<Event, bool> interceptEvents
		{
			get;
			set;
		}

		internal virtual bool RecordRepaint(VisualElement visualElement)
		{
			return false;
		}

		internal virtual bool EndRepaint()
		{
			return false;
		}
	}
}
