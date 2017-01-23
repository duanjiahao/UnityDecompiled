using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class LayerVisibilityWindow : EditorWindow
	{
		private class Styles
		{
			public readonly GUIStyle background = "grey_border";

			public readonly GUIStyle menuItem = "MenuItem";

			public readonly GUIStyle listEvenBg = "ObjectPickerResultsOdd";

			public readonly GUIStyle listOddBg = "ObjectPickerResultsEven";

			public readonly GUIStyle separator = "sv_iconselector_sep";

			public readonly GUIStyle lockButton = "IN LockButton";

			public readonly GUIStyle listTextStyle;

			public readonly GUIStyle listHeaderStyle;

			public readonly Texture2D visibleOn = EditorGUIUtility.LoadIcon("animationvisibilitytoggleon");

			public readonly Texture2D visibleOff = EditorGUIUtility.LoadIcon("animationvisibilitytoggleoff");

			public Styles()
			{
				this.listTextStyle = new GUIStyle(EditorStyles.label);
				this.listTextStyle.alignment = TextAnchor.MiddleLeft;
				this.listTextStyle.padding.left = 10;
				this.listHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
				this.listHeaderStyle.padding.left = 5;
			}
		}

		private const float kScrollBarWidth = 14f;

		private const float kFrameWidth = 1f;

		private const float kToggleSize = 17f;

		private const float kSeparatorHeight = 6f;

		private const string kLayerVisible = "Show/Hide Layer";

		private const string kLayerLocked = "Lock Layer for Picking";

		private static LayerVisibilityWindow s_LayerVisibilityWindow;

		private static long s_LastClosedTime;

		private static LayerVisibilityWindow.Styles s_Styles;

		private List<string> s_LayerNames;

		private List<int> s_LayerMasks;

		private int m_AllLayersMask;

		private float m_ContentHeight;

		private Vector2 m_ScrollPosition;

		private void CalcValidLayers()
		{
			this.s_LayerNames = new List<string>();
			this.s_LayerMasks = new List<int>();
			this.m_AllLayersMask = 0;
			for (int i = 0; i < 32; i++)
			{
				string layerName = InternalEditorUtility.GetLayerName(i);
				if (!(layerName == string.Empty))
				{
					this.s_LayerNames.Add(layerName);
					this.s_LayerMasks.Add(i);
					this.m_AllLayersMask |= 1 << i;
				}
			}
		}

		internal void OnEnable()
		{
			base.hideFlags = HideFlags.DontSave;
			base.wantsMouseMove = true;
		}

		internal void OnDisable()
		{
			LayerVisibilityWindow.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			LayerVisibilityWindow.s_LayerVisibilityWindow = null;
		}

		internal static bool ShowAtPosition(Rect buttonRect)
		{
			long num = DateTime.Now.Ticks / 10000L;
			bool result;
			if (num >= LayerVisibilityWindow.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (LayerVisibilityWindow.s_LayerVisibilityWindow == null)
				{
					LayerVisibilityWindow.s_LayerVisibilityWindow = ScriptableObject.CreateInstance<LayerVisibilityWindow>();
				}
				LayerVisibilityWindow.s_LayerVisibilityWindow.Init(buttonRect);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void Init(Rect buttonRect)
		{
			buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
			this.CalcValidLayers();
			int num = this.s_LayerNames.Count + 2 + 1 + 1;
			float num2 = (float)num * 16f + 6f;
			int sortingLayerCount = InternalEditorUtility.GetSortingLayerCount();
			if (sortingLayerCount > 1)
			{
				num2 += 22f;
				num2 += (float)sortingLayerCount * 16f;
			}
			this.m_ContentHeight = num2;
			num2 += 2f;
			num2 = Mathf.Min(num2, 600f);
			Vector2 windowSize = new Vector2(180f, num2);
			base.ShowAsDropDown(buttonRect, windowSize);
		}

		internal void OnGUI()
		{
			if (Event.current.type != EventType.Layout)
			{
				if (LayerVisibilityWindow.s_Styles == null)
				{
					LayerVisibilityWindow.s_Styles = new LayerVisibilityWindow.Styles();
				}
				Rect position = new Rect(1f, 1f, base.position.width - 2f, base.position.height - 2f);
				Rect viewRect = new Rect(0f, 0f, 1f, this.m_ContentHeight);
				bool flag = this.m_ContentHeight > position.height;
				float num = position.width;
				if (flag)
				{
					num -= 14f;
				}
				this.m_ScrollPosition = GUI.BeginScrollView(position, this.m_ScrollPosition, viewRect);
				this.Draw(num);
				GUI.EndScrollView();
				GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, LayerVisibilityWindow.s_Styles.background);
				if (Event.current.type == EventType.MouseMove)
				{
					Event.current.Use();
				}
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
				{
					base.Close();
					GUIUtility.ExitGUI();
				}
			}
		}

		private void DrawListBackground(Rect rect, bool even)
		{
			GUIStyle style = (!even) ? LayerVisibilityWindow.s_Styles.listOddBg : LayerVisibilityWindow.s_Styles.listEvenBg;
			GUI.Label(rect, GUIContent.none, style);
		}

		private void DrawHeader(ref Rect rect, string text, ref bool even)
		{
			this.DrawListBackground(rect, even);
			GUI.Label(rect, GUIContent.Temp(text), LayerVisibilityWindow.s_Styles.listHeaderStyle);
			rect.y += 16f;
			even = !even;
		}

		private void DrawSeparator(ref Rect rect, bool even)
		{
			this.DrawListBackground(new Rect(rect.x + 1f, rect.y, rect.width - 2f, 6f), even);
			GUI.Label(new Rect(rect.x + 5f, rect.y + 3f, rect.width - 10f, 3f), GUIContent.none, LayerVisibilityWindow.s_Styles.separator);
			rect.y += 6f;
		}

		private void Draw(float listElementWidth)
		{
			Rect rect = new Rect(0f, 0f, listElementWidth, 16f);
			bool even = false;
			this.DrawHeader(ref rect, "Layers", ref even);
			this.DoSpecialLayer(rect, true, ref even);
			rect.y += 16f;
			this.DoSpecialLayer(rect, false, ref even);
			rect.y += 16f;
			for (int i = 0; i < this.s_LayerNames.Count; i++)
			{
				this.DoOneLayer(rect, i, ref even);
				rect.y += 16f;
			}
			int sortingLayerCount = InternalEditorUtility.GetSortingLayerCount();
			if (sortingLayerCount > 1)
			{
				this.DrawSeparator(ref rect, even);
				this.DrawHeader(ref rect, "Sorting Layers", ref even);
				for (int j = 0; j < sortingLayerCount; j++)
				{
					this.DoOneSortingLayer(rect, j, ref even);
					rect.y += 16f;
				}
			}
			this.DrawSeparator(ref rect, even);
			this.DrawListBackground(rect, even);
			if (GUI.Button(rect, EditorGUIUtility.TempContent("Edit Layers..."), LayerVisibilityWindow.s_Styles.menuItem))
			{
				base.Close();
				Selection.activeObject = EditorApplication.tagManager;
				GUIUtility.ExitGUI();
			}
		}

		private void DoSpecialLayer(Rect rect, bool all, ref bool even)
		{
			int visibleLayers = Tools.visibleLayers;
			int num = (!all) ? 0 : this.m_AllLayersMask;
			bool visible = (visibleLayers & this.m_AllLayersMask) == num;
			bool flag;
			bool flag2;
			this.DoLayerEntry(rect, (!all) ? "Nothing" : "Everything", even, true, false, visible, false, out flag, out flag2);
			if (flag)
			{
				Tools.visibleLayers = ((!all) ? 0 : -1);
				LayerVisibilityWindow.RepaintAllSceneViews();
			}
			even = !even;
		}

		private void DoOneLayer(Rect rect, int index, ref bool even)
		{
			int visibleLayers = Tools.visibleLayers;
			int lockedLayers = Tools.lockedLayers;
			int num = 1 << this.s_LayerMasks[index];
			bool visible = (visibleLayers & num) != 0;
			bool locked = (lockedLayers & num) != 0;
			bool flag;
			bool flag2;
			this.DoLayerEntry(rect, this.s_LayerNames[index], even, true, true, visible, locked, out flag, out flag2);
			if (flag)
			{
				Tools.visibleLayers ^= num;
				LayerVisibilityWindow.RepaintAllSceneViews();
			}
			if (flag2)
			{
				Tools.lockedLayers ^= num;
			}
			even = !even;
		}

		private void DoOneSortingLayer(Rect rect, int index, ref bool even)
		{
			bool sortingLayerLocked = InternalEditorUtility.GetSortingLayerLocked(index);
			bool flag;
			bool flag2;
			this.DoLayerEntry(rect, InternalEditorUtility.GetSortingLayerName(index), even, false, true, true, sortingLayerLocked, out flag, out flag2);
			if (flag2)
			{
				InternalEditorUtility.SetSortingLayerLocked(index, !sortingLayerLocked);
			}
			even = !even;
		}

		private void DoLayerEntry(Rect rect, string layerName, bool even, bool showVisible, bool showLock, bool visible, bool locked, out bool visibleChanged, out bool lockedChanged)
		{
			this.DrawListBackground(rect, even);
			EditorGUI.BeginChangeCheck();
			Rect position = rect;
			position.width -= 34f;
			visible = GUI.Toggle(position, visible, EditorGUIUtility.TempContent(layerName), LayerVisibilityWindow.s_Styles.listTextStyle);
			Rect rect2 = new Rect(rect.width - 34f, rect.y + (rect.height - 17f) * 0.5f, 17f, 17f);
			visibleChanged = false;
			if (showVisible)
			{
				Color color = GUI.color;
				Color color2 = color;
				color2.a = ((!visible) ? 0.4f : 0.6f);
				GUI.color = color2;
				Rect position2 = rect2;
				position2.y += 3f;
				GUIContent content = new GUIContent(string.Empty, (!visible) ? LayerVisibilityWindow.s_Styles.visibleOff : LayerVisibilityWindow.s_Styles.visibleOn, "Show/Hide Layer");
				GUI.Toggle(position2, visible, content, GUIStyle.none);
				GUI.color = color;
				visibleChanged = EditorGUI.EndChangeCheck();
			}
			lockedChanged = false;
			if (showLock)
			{
				rect2.x += 17f;
				EditorGUI.BeginChangeCheck();
				Color backgroundColor = GUI.backgroundColor;
				Color backgroundColor2 = backgroundColor;
				if (!locked)
				{
					backgroundColor2.a *= 0.4f;
				}
				GUI.backgroundColor = backgroundColor2;
				GUI.Toggle(rect2, locked, new GUIContent(string.Empty, "Lock Layer for Picking"), LayerVisibilityWindow.s_Styles.lockButton);
				GUI.backgroundColor = backgroundColor;
				lockedChanged = EditorGUI.EndChangeCheck();
			}
		}

		private static void RepaintAllSceneViews()
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(SceneView));
			for (int i = 0; i < array.Length; i++)
			{
				SceneView sceneView = (SceneView)array[i];
				sceneView.Repaint();
			}
		}
	}
}
