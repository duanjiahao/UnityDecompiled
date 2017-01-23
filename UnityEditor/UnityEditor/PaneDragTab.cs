using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PaneDragTab : GUIView
	{
		private const float kMaxArea = 50000f;

		[SerializeField]
		private bool m_Shadow;

		private static PaneDragTab s_Get;

		private const float kTopThumbnailOffset = 10f;

		[SerializeField]
		private Vector2 m_FullWindowSize = new Vector2(80f, 60f);

		private Rect m_StartRect;

		[SerializeField]
		private Rect m_TargetRect;

		[SerializeField]
		private static GUIStyle s_PaneStyle;

		[SerializeField]
		private static GUIStyle s_TabStyle;

		private bool m_TabVisible;

		private float m_TargetAlpha = 1f;

		private bool m_DidResizeOnLastLayout;

		private DropInfo.Type m_Type = (DropInfo.Type)(-1);

		private GUIContent m_Content;

		[SerializeField]
		internal ContainerWindow m_Window;

		[SerializeField]
		private ContainerWindow m_InFrontOfWindow = null;

		public static PaneDragTab get
		{
			get
			{
				PaneDragTab result;
				if (!PaneDragTab.s_Get)
				{
					UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(PaneDragTab));
					if (array.Length != 0)
					{
						PaneDragTab.s_Get = (PaneDragTab)array[0];
					}
					if (PaneDragTab.s_Get)
					{
						result = PaneDragTab.s_Get;
						return result;
					}
					PaneDragTab.s_Get = ScriptableObject.CreateInstance<PaneDragTab>();
				}
				result = PaneDragTab.s_Get;
				return result;
			}
		}

		public void SetDropInfo(DropInfo di, Vector2 mouseScreenPos, ContainerWindow inFrontOf)
		{
			if (this.m_Type != di.type || (di.type == DropInfo.Type.Pane && di.rect != this.m_TargetRect))
			{
				this.m_Type = di.type;
				DropInfo.Type type = di.type;
				if (type != DropInfo.Type.Window)
				{
					if (type == DropInfo.Type.Pane || type == DropInfo.Type.Tab)
					{
						this.m_TargetAlpha = 1f;
					}
				}
				else
				{
					this.m_TargetAlpha = 0.6f;
				}
			}
			DropInfo.Type type2 = di.type;
			if (type2 != DropInfo.Type.Window)
			{
				if (type2 == DropInfo.Type.Pane || type2 == DropInfo.Type.Tab)
				{
					this.m_TargetRect = di.rect;
				}
			}
			else
			{
				this.m_TargetRect = new Rect(mouseScreenPos.x - this.m_FullWindowSize.x / 2f, mouseScreenPos.y - this.m_FullWindowSize.y / 2f, this.m_FullWindowSize.x, this.m_FullWindowSize.y);
			}
			this.m_TabVisible = (di.type == DropInfo.Type.Tab);
			this.m_TargetRect.x = Mathf.Round(this.m_TargetRect.x);
			this.m_TargetRect.y = Mathf.Round(this.m_TargetRect.y);
			this.m_TargetRect.width = Mathf.Round(this.m_TargetRect.width);
			this.m_TargetRect.height = Mathf.Round(this.m_TargetRect.height);
			this.m_InFrontOfWindow = inFrontOf;
			this.m_Window.MoveInFrontOf(this.m_InFrontOfWindow);
			this.SetWindowPos(this.m_TargetRect);
			base.Repaint();
		}

		public void Close()
		{
			if (this.m_Window)
			{
				this.m_Window.Close();
			}
			UnityEngine.Object.DestroyImmediate(this, true);
			PaneDragTab.s_Get = null;
		}

		public void Show(Rect pixelPos, GUIContent content, Vector2 viewSize, Vector2 mouseScreenPosition)
		{
			this.m_Content = content;
			float num = viewSize.x * viewSize.y;
			this.m_FullWindowSize = viewSize * Mathf.Sqrt(Mathf.Clamp01(50000f / num));
			if (!this.m_Window)
			{
				this.m_Window = ScriptableObject.CreateInstance<ContainerWindow>();
				this.m_Window.m_DontSaveToLayout = true;
				base.SetMinMaxSizes(Vector2.zero, new Vector2(10000f, 10000f));
				this.SetWindowPos(pixelPos);
				this.m_Window.rootView = this;
			}
			else
			{
				this.SetWindowPos(pixelPos);
			}
			this.m_Window.Show(ShowMode.NoShadow, true, false);
			this.m_TargetRect = pixelPos;
		}

		private void SetWindowPos(Rect screenPosition)
		{
			this.m_Window.position = screenPosition;
		}

		private void OnGUI()
		{
			if (PaneDragTab.s_PaneStyle == null)
			{
				PaneDragTab.s_PaneStyle = "dragtabdropwindow";
				PaneDragTab.s_TabStyle = "dragtab";
			}
			if (Event.current.type == EventType.Repaint)
			{
				Color color = GUI.color;
				GUI.color = Color.white;
				PaneDragTab.s_PaneStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), this.m_Content, false, false, true, true);
				if (this.m_TabVisible)
				{
					PaneDragTab.s_TabStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), this.m_Content, false, false, true, true);
				}
				GUI.color = color;
				this.m_Window.SetAlpha(this.m_TargetAlpha);
			}
		}
	}
}
