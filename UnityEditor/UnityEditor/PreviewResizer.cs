using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class PreviewResizer
	{
		private static float s_DraggedPreviewSize = 0f;

		private static float s_CachedPreviewSizeWhileDragging = 0f;

		private static float s_MouseDownLocation;

		private static float s_MouseDownValue;

		private static bool s_MouseDragged;

		[SerializeField]
		private float m_CachedPref;

		[SerializeField]
		private int m_ControlHash;

		[SerializeField]
		private string m_PrefName;

		private int m_Id = 0;

		private int id
		{
			get
			{
				if (this.m_Id == 0)
				{
					this.m_Id = GUIUtility.GetControlID(this.m_ControlHash, FocusType.Passive, default(Rect));
				}
				return this.m_Id;
			}
		}

		public void Init(string prefName)
		{
			if (this.m_ControlHash == 0 || string.IsNullOrEmpty(this.m_PrefName))
			{
				this.m_ControlHash = prefName.GetHashCode();
				this.m_PrefName = "Preview_" + prefName;
				this.m_CachedPref = EditorPrefs.GetFloat(this.m_PrefName, 1f);
			}
		}

		public float ResizeHandle(Rect windowPosition, float minSize, float minRemainingSize, float resizerHeight)
		{
			return this.ResizeHandle(windowPosition, minSize, minRemainingSize, resizerHeight, default(Rect));
		}

		public float ResizeHandle(Rect windowPosition, float minSize, float minRemainingSize, float resizerHeight, Rect dragRect)
		{
			if (Mathf.Abs(this.m_CachedPref) < minSize)
			{
				this.m_CachedPref = minSize * Mathf.Sign(this.m_CachedPref);
			}
			float num = windowPosition.height - minRemainingSize;
			bool flag = GUIUtility.hotControl == this.id;
			float num2 = (!flag) ? Mathf.Max(0f, this.m_CachedPref) : PreviewResizer.s_DraggedPreviewSize;
			bool flag2 = this.m_CachedPref > 0f;
			float num3 = Mathf.Abs(this.m_CachedPref);
			Rect position = new Rect(0f, windowPosition.height - num2 - resizerHeight, windowPosition.width, resizerHeight);
			if (dragRect.width != 0f)
			{
				position.x = dragRect.x;
				position.width = dragRect.width;
			}
			bool flag3 = flag2;
			num2 = -PreviewResizer.PixelPreciseCollapsibleSlider(this.id, position, -num2, -num, 0f, ref flag2);
			num2 = Mathf.Min(num2, num);
			flag = (GUIUtility.hotControl == this.id);
			if (flag)
			{
				PreviewResizer.s_DraggedPreviewSize = num2;
			}
			if (num2 < minSize)
			{
				num2 = ((num2 >= minSize * 0.5f) ? minSize : 0f);
			}
			if (flag2 != flag3)
			{
				num2 = ((!flag2) ? 0f : num3);
			}
			flag2 = (num2 >= minSize / 2f);
			if (GUIUtility.hotControl == 0)
			{
				if (num2 > 0f)
				{
					num3 = num2;
				}
				float num4 = num3 * (float)((!flag2) ? -1 : 1);
				if (num4 != this.m_CachedPref)
				{
					this.m_CachedPref = num4;
					EditorPrefs.SetFloat(this.m_PrefName, this.m_CachedPref);
				}
			}
			PreviewResizer.s_CachedPreviewSizeWhileDragging = num2;
			return num2;
		}

		public bool GetExpanded()
		{
			bool result;
			if (GUIUtility.hotControl == this.id)
			{
				result = (PreviewResizer.s_CachedPreviewSizeWhileDragging > 0f);
			}
			else
			{
				result = (this.m_CachedPref > 0f);
			}
			return result;
		}

		public float GetPreviewSize()
		{
			float result;
			if (GUIUtility.hotControl == this.id)
			{
				result = Mathf.Max(0f, PreviewResizer.s_CachedPreviewSizeWhileDragging);
			}
			else
			{
				result = Mathf.Max(0f, this.m_CachedPref);
			}
			return result;
		}

		public bool GetExpandedBeforeDragging()
		{
			return this.m_CachedPref > 0f;
		}

		public void SetExpanded(bool expanded)
		{
			this.m_CachedPref = Mathf.Abs(this.m_CachedPref) * (float)((!expanded) ? -1 : 1);
			EditorPrefs.SetFloat(this.m_PrefName, this.m_CachedPref);
		}

		public void ToggleExpanded()
		{
			this.m_CachedPref = -this.m_CachedPref;
			EditorPrefs.SetFloat(this.m_PrefName, this.m_CachedPref);
		}

		public static float PixelPreciseCollapsibleSlider(int id, Rect position, float value, float min, float max, ref bool expanded)
		{
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (GUIUtility.hotControl == 0 && current.button == 0 && position.Contains(current.mousePosition))
				{
					GUIUtility.hotControl = id;
					PreviewResizer.s_MouseDownLocation = current.mousePosition.y;
					PreviewResizer.s_MouseDownValue = value;
					PreviewResizer.s_MouseDragged = false;
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id)
				{
					GUIUtility.hotControl = 0;
					if (!PreviewResizer.s_MouseDragged)
					{
						expanded = !expanded;
					}
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					value = Mathf.Clamp(current.mousePosition.y - PreviewResizer.s_MouseDownLocation + PreviewResizer.s_MouseDownValue, min, max - 1f);
					GUI.changed = true;
					PreviewResizer.s_MouseDragged = true;
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (GUIUtility.hotControl == 0)
				{
					EditorGUIUtility.AddCursorRect(position, MouseCursor.SplitResizeUpDown);
				}
				if (GUIUtility.hotControl == id)
				{
					EditorGUIUtility.AddCursorRect(new Rect(position.x, position.y - 100f, position.width, position.height + 200f), MouseCursor.SplitResizeUpDown);
				}
				break;
			}
			return value;
		}
	}
}
