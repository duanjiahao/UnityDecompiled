using System;
using UnityEngine;

namespace UnityEditor
{
	public abstract class PopupWindowContent
	{
		public EditorWindow editorWindow
		{
			get;
			internal set;
		}

		public abstract void OnGUI(Rect rect);

		public virtual Vector2 GetWindowSize()
		{
			return new Vector2(200f, 200f);
		}

		public virtual void OnOpen()
		{
		}

		public virtual void OnClose()
		{
		}
	}
}
