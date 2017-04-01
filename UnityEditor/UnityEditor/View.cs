using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	internal class View : ScriptableObject
	{
		[SerializeField]
		private MonoReloadableIntPtr m_ViewPtr;

		[SerializeField]
		private View[] m_Children = new View[0];

		[NonSerialized]
		private View m_Parent;

		[NonSerialized]
		private ContainerWindow m_Window;

		[SerializeField]
		private Rect m_Position = new Rect(0f, 0f, 100f, 100f);

		[SerializeField]
		internal Vector2 m_MinSize;

		[SerializeField]
		internal Vector2 m_MaxSize;

		public Vector2 minSize
		{
			get
			{
				return this.m_MinSize;
			}
		}

		public Vector2 maxSize
		{
			get
			{
				return this.m_MaxSize;
			}
		}

		public View[] allChildren
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				View[] children = this.m_Children;
				for (int i = 0; i < children.Length; i++)
				{
					View view = children[i];
					arrayList.AddRange(view.allChildren);
				}
				arrayList.Add(this);
				return (View[])arrayList.ToArray(typeof(View));
			}
		}

		public Rect position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.SetPosition(value);
			}
		}

		public Rect windowPosition
		{
			get
			{
				Rect result;
				if (this.m_Parent == null)
				{
					result = this.position;
				}
				else
				{
					Rect windowPosition = this.parent.windowPosition;
					result = new Rect(windowPosition.x + this.position.x, windowPosition.y + this.position.y, this.position.width, this.position.height);
				}
				return result;
			}
		}

		public Rect screenPosition
		{
			get
			{
				Rect windowPosition = this.windowPosition;
				if (this.window != null)
				{
					Vector2 vector = this.window.WindowToScreenPoint(Vector2.zero);
					windowPosition.x += vector.x;
					windowPosition.y += vector.y;
				}
				return windowPosition;
			}
		}

		public ContainerWindow window
		{
			get
			{
				return this.m_Window;
			}
		}

		public View parent
		{
			get
			{
				return this.m_Parent;
			}
		}

		public View[] children
		{
			get
			{
				return this.m_Children;
			}
		}

		internal virtual void Reflow()
		{
			View[] children = this.children;
			for (int i = 0; i < children.Length; i++)
			{
				View view = children[i];
				view.Reflow();
			}
		}

		internal string DebugHierarchy(int level)
		{
			string text = "";
			string text2 = "";
			for (int i = 0; i < level; i++)
			{
				text += "  ";
			}
			text2 = string.Concat(new object[]
			{
				text2,
				text,
				this.ToString(),
				" p:",
				this.position
			});
			if (this.children.Length > 0)
			{
				text2 += " {\n";
				View[] children = this.children;
				for (int j = 0; j < children.Length; j++)
				{
					View view = children[j];
					text2 += view.DebugHierarchy(level + 2);
				}
				text2 = text2 + text + " }\n";
			}
			else
			{
				text2 += "\n";
			}
			return text2;
		}

		internal virtual void Initialize(ContainerWindow win)
		{
			this.SetWindow(win);
			View[] children = this.m_Children;
			for (int i = 0; i < children.Length; i++)
			{
				View view = children[i];
				view.m_Parent = this;
				view.Initialize(win);
			}
		}

		internal void SetMinMaxSizes(Vector2 min, Vector2 max)
		{
			if (!(this.minSize == min) || !(this.maxSize == max))
			{
				this.m_MinSize = min;
				this.m_MaxSize = max;
				if (this.m_Parent)
				{
					this.m_Parent.ChildrenMinMaxChanged();
				}
				if (this.window && this.window.rootView == this)
				{
					this.window.SetMinMaxSizes(min, max);
				}
			}
		}

		protected virtual void ChildrenMinMaxChanged()
		{
		}

		private void __internalAwake()
		{
			base.hideFlags = HideFlags.DontSave;
		}

		protected virtual void SetPosition(Rect newPos)
		{
			this.m_Position = newPos;
		}

		internal void SetPositionOnly(Rect newPos)
		{
			this.m_Position = newPos;
		}

		public int IndexOfChild(View child)
		{
			int num = 0;
			View[] children = this.m_Children;
			int result;
			for (int i = 0; i < children.Length; i++)
			{
				View x = children[i];
				if (x == child)
				{
					result = num;
					return result;
				}
				num++;
			}
			result = -1;
			return result;
		}

		public void OnDestroy()
		{
			View[] children = this.m_Children;
			for (int i = 0; i < children.Length; i++)
			{
				View obj = children[i];
				UnityEngine.Object.DestroyImmediate(obj, true);
			}
		}

		public void AddChild(View child)
		{
			this.AddChild(child, this.m_Children.Length);
		}

		public virtual void AddChild(View child, int idx)
		{
			Array.Resize<View>(ref this.m_Children, this.m_Children.Length + 1);
			if (idx != this.m_Children.Length - 1)
			{
				Array.Copy(this.m_Children, idx, this.m_Children, idx + 1, this.m_Children.Length - idx - 1);
			}
			this.m_Children[idx] = child;
			if (child.m_Parent)
			{
				child.m_Parent.RemoveChild(child);
			}
			child.m_Parent = this;
			child.SetWindowRecurse(this.window);
			this.ChildrenMinMaxChanged();
		}

		public virtual void RemoveChild(View child)
		{
			int num = Array.IndexOf<View>(this.m_Children, child);
			if (num == -1)
			{
				Debug.LogError("Unable to remove child - it's not IN the view");
			}
			else
			{
				this.RemoveChild(num);
			}
		}

		public virtual void RemoveChild(int idx)
		{
			View view = this.m_Children[idx];
			view.m_Parent = null;
			view.SetWindowRecurse(null);
			Array.Copy(this.m_Children, idx + 1, this.m_Children, idx, this.m_Children.Length - idx - 1);
			Array.Resize<View>(ref this.m_Children, this.m_Children.Length - 1);
			this.ChildrenMinMaxChanged();
		}

		protected virtual void SetWindow(ContainerWindow win)
		{
			this.m_Window = win;
		}

		internal void SetWindowRecurse(ContainerWindow win)
		{
			this.SetWindow(win);
			View[] children = this.m_Children;
			for (int i = 0; i < children.Length; i++)
			{
				View view = children[i];
				view.SetWindowRecurse(win);
			}
		}

		protected virtual bool OnFocus()
		{
			return true;
		}
	}
}
