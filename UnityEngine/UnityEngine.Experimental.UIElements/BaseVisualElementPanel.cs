using System;

namespace UnityEngine.Experimental.UIElements
{
	internal abstract class BaseVisualElementPanel : IPanel
	{
		public virtual VisualElement focusedElement
		{
			get;
			set;
		}

		public abstract EventInterests IMGUIEventInterests
		{
			get;
			set;
		}

		public abstract int instanceID
		{
			get;
			protected set;
		}

		public abstract LoadResourceFunction loadResourceFunc
		{
			get;
			protected set;
		}

		public abstract int IMGUIContainersCount
		{
			get;
			set;
		}

		internal virtual IStylePainter stylePainter
		{
			get;
			set;
		}

		public abstract VisualContainer visualTree
		{
			get;
		}

		public abstract IDispatcher dispatcher
		{
			get;
			protected set;
		}

		public abstract IScheduler scheduler
		{
			get;
		}

		public abstract IDataWatchService dataWatch
		{
			get;
			protected set;
		}

		public abstract ContextType contextType
		{
			get;
			protected set;
		}

		public BasePanelDebug panelDebug
		{
			get;
			set;
		}

		public abstract void Repaint(Event e);

		public abstract void ValidateLayout();

		public abstract VisualElement Pick(Vector2 point);
	}
}
