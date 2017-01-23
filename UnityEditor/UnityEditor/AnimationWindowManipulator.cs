using System;
using UnityEngine;

namespace UnityEditor
{
	internal class AnimationWindowManipulator
	{
		public delegate bool OnStartDragDelegate(AnimationWindowManipulator manipulator, Event evt);

		public delegate bool OnDragDelegate(AnimationWindowManipulator manipulator, Event evt);

		public delegate bool OnEndDragDelegate(AnimationWindowManipulator manipulator, Event evt);

		public AnimationWindowManipulator.OnStartDragDelegate onStartDrag;

		public AnimationWindowManipulator.OnDragDelegate onDrag;

		public AnimationWindowManipulator.OnEndDragDelegate onEndDrag;

		public Rect rect;

		public int controlID;

		public AnimationWindowManipulator()
		{
			this.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(this.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate((AnimationWindowManipulator manipulator, Event evt) => false));
			this.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(this.onDrag, new AnimationWindowManipulator.OnDragDelegate((AnimationWindowManipulator manipulator, Event evt) => false));
			this.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(this.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate((AnimationWindowManipulator manipulator, Event evt) => false));
		}

		public virtual void HandleEvents()
		{
			this.controlID = GUIUtility.GetControlID(FocusType.Passive);
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(this.controlID);
			bool flag = false;
			if (typeForControl != EventType.MouseDown)
			{
				if (typeForControl != EventType.MouseDrag)
				{
					if (typeForControl == EventType.MouseUp)
					{
						if (GUIUtility.hotControl == this.controlID)
						{
							flag = this.onEndDrag(this, current);
							GUIUtility.hotControl = 0;
						}
					}
				}
				else if (GUIUtility.hotControl == this.controlID)
				{
					flag = this.onDrag(this, current);
				}
			}
			else if (current.button == 0)
			{
				flag = this.onStartDrag(this, current);
				if (flag)
				{
					GUIUtility.hotControl = this.controlID;
				}
			}
			if (flag)
			{
				current.Use();
			}
		}

		public virtual void IgnoreEvents()
		{
			GUIUtility.GetControlID(FocusType.Passive);
		}
	}
}
