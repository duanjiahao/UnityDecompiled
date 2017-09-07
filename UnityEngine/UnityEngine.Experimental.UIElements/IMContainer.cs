using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMContainer : VisualContainer, IOnGUIHandler, IRecyclable
	{
		private GUIStyle m_GUIStyle;

		public Vector2 translation
		{
			get
			{
				return new Vector2(base.transform.m03, base.transform.m13);
			}
			set
			{
				this.m_Transform.m03 = value.x;
				this.m_Transform.m13 = value.y;
				base.Dirty(ChangeType.Repaint);
			}
		}

		public int id
		{
			get;
			set;
		}

		public bool isTrashed
		{
			get;
			set;
		}

		internal GUIStyle style
		{
			get
			{
				return this.m_GUIStyle;
			}
			set
			{
				this.m_GUIStyle = value;
			}
		}

		public IMContainer()
		{
			this.style = GUIStyle.none;
			base.clipChildren = true;
		}

		public virtual void OnTrash()
		{
		}

		public virtual void OnReuse()
		{
			this.style = GUIStyle.none;
			this.translation = Vector2.zero;
			base.position = new Rect(0f, 0f, 0f, 0f);
		}

		public virtual bool OnGUI(Event evt)
		{
			return false;
		}

		public virtual void GenerateControlID()
		{
			this.id = 0;
		}
	}
}
