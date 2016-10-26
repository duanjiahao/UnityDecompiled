using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class EyeDropper : GUIView
	{
		private class Styles
		{
			public GUIStyle eyeDropperHorizontalLine = "EyeDropperHorizontalLine";

			public GUIStyle eyeDropperVerticalLine = "EyeDropperVerticalLine";

			public GUIStyle eyeDropperPickedPixel = "EyeDropperPickedPixel";
		}

		private const int kPixelSize = 10;

		private const int kDummyWindowSize = 10000;

		internal static Color s_LastPickedColor;

		private GUIView m_DelegateView;

		private Texture2D m_Preview;

		private static EyeDropper s_Instance;

		private static Vector2 s_PickCoordinates = Vector2.zero;

		private bool m_Focused;

		private static EyeDropper.Styles styles;

		private static EyeDropper get
		{
			get
			{
				if (!EyeDropper.s_Instance)
				{
					ScriptableObject.CreateInstance<EyeDropper>();
				}
				return EyeDropper.s_Instance;
			}
		}

		private EyeDropper()
		{
			EyeDropper.s_Instance = this;
		}

		public static void Start(GUIView viewToUpdate)
		{
			EyeDropper.get.Show(viewToUpdate);
		}

		private void Show(GUIView sourceView)
		{
			this.m_DelegateView = sourceView;
			ContainerWindow containerWindow = ScriptableObject.CreateInstance<ContainerWindow>();
			containerWindow.m_DontSaveToLayout = true;
			containerWindow.title = "EyeDropper";
			containerWindow.hideFlags = HideFlags.DontSave;
			containerWindow.mainView = this;
			containerWindow.Show(ShowMode.PopupMenu, true, false);
			base.AddToAuxWindowList();
			containerWindow.SetInvisible();
			base.SetMinMaxSizes(new Vector2(0f, 0f), new Vector2(10000f, 10000f));
			containerWindow.position = new Rect(-5000f, -5000f, 10000f, 10000f);
			base.wantsMouseMove = true;
			base.StealMouseCapture();
		}

		public static Color GetPickedColor()
		{
			return InternalEditorUtility.ReadScreenPixel(EyeDropper.s_PickCoordinates, 1, 1)[0];
		}

		public static Color GetLastPickedColor()
		{
			return EyeDropper.s_LastPickedColor;
		}

		public static void DrawPreview(Rect position)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			if (EyeDropper.styles == null)
			{
				EyeDropper.styles = new EyeDropper.Styles();
			}
			GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			Texture2D texture2D = EyeDropper.get.m_Preview;
			int num = (int)Mathf.Ceil(position.width / 10f);
			int num2 = (int)Mathf.Ceil(position.height / 10f);
			if (texture2D == null)
			{
				texture2D = (EyeDropper.get.m_Preview = ColorPicker.MakeTexture(num, num2));
				texture2D.filterMode = FilterMode.Point;
			}
			if (texture2D.width != num || texture2D.height != num2)
			{
				texture2D.Resize(num, num2);
			}
			Vector2 a = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
			Vector2 pixelPos = a - new Vector2((float)(num / 2), (float)(num2 / 2));
			texture2D.SetPixels(InternalEditorUtility.ReadScreenPixel(pixelPos, num, num2), 0);
			texture2D.Apply(true);
			Graphics.DrawTexture(position, texture2D);
			float num3 = position.width / (float)num;
			GUIStyle gUIStyle = EyeDropper.styles.eyeDropperVerticalLine;
			for (float num4 = position.x; num4 < position.xMax; num4 += num3)
			{
				Rect position2 = new Rect(Mathf.Round(num4), position.y, num3, position.height);
				gUIStyle.Draw(position2, false, false, false, false);
			}
			float num5 = position.height / (float)num2;
			gUIStyle = EyeDropper.styles.eyeDropperHorizontalLine;
			for (float num6 = position.y; num6 < position.yMax; num6 += num5)
			{
				Rect position3 = new Rect(position.x, Mathf.Floor(num6), position.width, num5);
				gUIStyle.Draw(position3, false, false, false, false);
			}
			Rect position4 = new Rect((a.x - pixelPos.x) * num3 + position.x, (a.y - pixelPos.y) * num5 + position.y, num3, num5);
			EyeDropper.styles.eyeDropperPickedPixel.Draw(position4, false, false, false, false);
			GL.sRGBWrite = false;
		}

		private void OnGUI()
		{
			switch (Event.current.type)
			{
			case EventType.MouseDown:
				if (Event.current.button == 0)
				{
					EyeDropper.s_PickCoordinates = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
					base.window.Close();
					EyeDropper.s_LastPickedColor = EyeDropper.GetPickedColor();
					this.SendEvent("EyeDropperClicked", true);
				}
				break;
			case EventType.MouseMove:
				EyeDropper.s_PickCoordinates = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				base.StealMouseCapture();
				this.SendEvent("EyeDropperUpdate", true);
				break;
			case EventType.KeyDown:
				if (Event.current.keyCode == KeyCode.Escape)
				{
					base.window.Close();
					this.SendEvent("EyeDropperCancelled", true);
				}
				break;
			}
		}

		private void SendEvent(string eventName, bool exitGUI)
		{
			if (this.m_DelegateView)
			{
				Event e = EditorGUIUtility.CommandEvent(eventName);
				this.m_DelegateView.SendEvent(e);
				if (exitGUI)
				{
					GUIUtility.ExitGUI();
				}
			}
		}

		public new void OnDestroy()
		{
			if (this.m_Preview)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Preview);
			}
			if (!this.m_Focused)
			{
				this.SendEvent("EyeDropperCancelled", false);
			}
		}

		protected override bool OnFocus()
		{
			this.m_Focused = true;
			return base.OnFocus();
		}
	}
}
