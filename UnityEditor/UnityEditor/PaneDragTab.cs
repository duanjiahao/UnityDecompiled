using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	internal class PaneDragTab : GUIView
	{
		private const float kTopThumbnailOffset = 10f;

		[SerializeField]
		private bool m_Shadow;

		private static PaneDragTab s_Get;

		[SerializeField]
		private Vector2 m_ThumbnailSize = new Vector2(80f, 60f);

		private Rect m_StartRect;

		[SerializeField]
		private Rect m_TargetRect;

		[SerializeField]
		private static GUIStyle s_PaneStyle;

		[SerializeField]
		private static GUIStyle s_TabStyle;

		private AnimBool m_PaneVisible = new AnimBool();

		private AnimBool m_TabVisible = new AnimBool();

		private float m_StartAlpha = 1f;

		private float m_TargetAlpha = 1f;

		private bool m_DidResizeOnLastLayout;

		private DropInfo.Type m_Type = (DropInfo.Type)(-1);

		private float m_StartTime;

		public GUIContent content;

		private Texture2D m_Thumbnail;

		[SerializeField]
		internal ContainerWindow m_Window;

		[SerializeField]
		private ContainerWindow m_InFrontOfWindow;

		public static PaneDragTab get
		{
			get
			{
				if (!PaneDragTab.s_Get)
				{
					UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(PaneDragTab));
					if (array.Length != 0)
					{
						PaneDragTab.s_Get = (PaneDragTab)array[0];
					}
					if (PaneDragTab.s_Get)
					{
						return PaneDragTab.s_Get;
					}
					PaneDragTab.s_Get = ScriptableObject.CreateInstance<PaneDragTab>();
				}
				return PaneDragTab.s_Get;
			}
		}

		public void OnEnable()
		{
			this.m_PaneVisible.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_TabVisible.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public void OnDisable()
		{
			this.m_PaneVisible.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_TabVisible.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public void GrabThumbnail()
		{
			if (this.m_Thumbnail != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Thumbnail);
			}
			this.m_Thumbnail = new Texture2D(Screen.width, Screen.height);
			this.m_Thumbnail.ReadPixels(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), 0, 0);
			this.m_Thumbnail.Apply();
			float num = (float)(this.m_Thumbnail.width * this.m_Thumbnail.height);
			this.m_ThumbnailSize = new Vector2((float)this.m_Thumbnail.width, (float)this.m_Thumbnail.height) * Mathf.Sqrt(Mathf.Clamp01(50000f / num));
		}

		public void SetDropInfo(DropInfo di, Vector2 mouseScreenPos, ContainerWindow inFrontOf)
		{
			if (this.m_Type != di.type || (di.type == DropInfo.Type.Pane && di.rect != this.m_TargetRect))
			{
				this.m_Type = di.type;
				this.m_StartRect = this.GetInterpolatedRect(this.CalcFade());
				this.m_StartTime = Time.realtimeSinceStartup;
				switch (di.type)
				{
				case DropInfo.Type.Tab:
				case DropInfo.Type.Pane:
					this.m_TargetAlpha = 1f;
					break;
				case DropInfo.Type.Window:
					this.m_TargetAlpha = 0.6f;
					break;
				}
			}
			switch (di.type)
			{
			case DropInfo.Type.Tab:
			case DropInfo.Type.Pane:
				this.m_TargetRect = di.rect;
				break;
			case DropInfo.Type.Window:
				this.m_TargetRect = new Rect(mouseScreenPos.x - this.m_ThumbnailSize.x / 2f, mouseScreenPos.y - this.m_ThumbnailSize.y / 2f, this.m_ThumbnailSize.x, this.m_ThumbnailSize.y);
				break;
			}
			this.m_PaneVisible.target = (di.type == DropInfo.Type.Pane);
			this.m_TabVisible.target = (di.type == DropInfo.Type.Tab);
			this.m_TargetRect.x = Mathf.Round(this.m_TargetRect.x);
			this.m_TargetRect.y = Mathf.Round(this.m_TargetRect.y);
			this.m_TargetRect.width = Mathf.Round(this.m_TargetRect.width);
			this.m_TargetRect.height = Mathf.Round(this.m_TargetRect.height);
			this.m_InFrontOfWindow = inFrontOf;
			this.m_Window.MoveInFrontOf(this.m_InFrontOfWindow);
			this.SetWindowPos(this.GetInterpolatedRect(this.CalcFade()));
			base.Repaint();
		}

		public void Close()
		{
			if (this.m_Thumbnail != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Thumbnail);
			}
			if (this.m_Window)
			{
				this.m_Window.Close();
			}
			UnityEngine.Object.DestroyImmediate(this, true);
			PaneDragTab.s_Get = null;
		}

		public void Show(Rect pixelPos, Vector2 mouseScreenPosition)
		{
			if (!this.m_Window)
			{
				this.m_Window = ScriptableObject.CreateInstance<ContainerWindow>();
				this.m_Window.m_DontSaveToLayout = true;
				base.SetMinMaxSizes(Vector2.zero, new Vector2(10000f, 10000f));
				this.SetWindowPos(pixelPos);
				this.m_Window.mainView = this;
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

		private float CalcFade()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return 1f;
			}
			return Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(5f * (Time.realtimeSinceStartup - this.m_StartTime)));
		}

		private Rect GetInterpolatedRect(float fade)
		{
			return new Rect(Mathf.Lerp(this.m_StartRect.x, this.m_TargetRect.x, fade), Mathf.Lerp(this.m_StartRect.y, this.m_TargetRect.y, fade), Mathf.Lerp(this.m_StartRect.width, this.m_TargetRect.width, fade), Mathf.Lerp(this.m_StartRect.height, this.m_TargetRect.height, fade));
		}

		private void OnGUI()
		{
			float num = this.CalcFade();
			if (PaneDragTab.s_PaneStyle == null)
			{
				PaneDragTab.s_PaneStyle = "dragtabdropwindow";
				PaneDragTab.s_TabStyle = "dragtab";
			}
			if (Event.current.type == EventType.Layout)
			{
				this.m_DidResizeOnLastLayout = !this.m_DidResizeOnLastLayout;
				if (!this.m_DidResizeOnLastLayout)
				{
					this.SetWindowPos(this.GetInterpolatedRect(num));
					if (Application.platform == RuntimePlatform.OSXEditor)
					{
						this.m_Window.SetAlpha(Mathf.Lerp(this.m_StartAlpha, this.m_TargetAlpha, num));
					}
					return;
				}
			}
			if (Event.current.type == EventType.Repaint)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 1f);
				if (this.m_Thumbnail != null)
				{
					GUI.DrawTexture(new Rect(0f, 0f, base.position.width, base.position.height), this.m_Thumbnail, ScaleMode.StretchToFill, false);
				}
				if (this.m_TabVisible.faded != 0f)
				{
					GUI.color = new Color(1f, 1f, 1f, this.m_TabVisible.faded);
					PaneDragTab.s_TabStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), this.content, false, false, true, true);
				}
				if (this.m_PaneVisible.faded != 0f)
				{
					GUI.color = new Color(1f, 1f, 1f, this.m_PaneVisible.faded);
					PaneDragTab.s_PaneStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), this.content, false, false, true, true);
				}
				GUI.color = color;
			}
			if (Application.platform != RuntimePlatform.WindowsEditor)
			{
				base.Repaint();
			}
		}
	}
}
