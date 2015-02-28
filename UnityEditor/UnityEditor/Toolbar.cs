using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class Toolbar : GUIView
	{
		private static GUIContent[] s_ToolIcons;
		private static GUIContent[] s_ViewToolIcons;
		private static GUIContent[] s_PivotIcons;
		private static GUIContent[] s_PivotRotation;
		private static GUIContent s_LayerContent;
		private static GUIContent[] s_PlayIcons;
		private bool t1;
		private bool t2;
		private bool t3;
		private static GUIContent[] s_ShownToolIcons = new GUIContent[5];
		public static Toolbar get = null;
		[SerializeField]
		private string m_LastLoadedLayoutName;
		internal static string lastLoadedLayoutName
		{
			get
			{
				return (!string.IsNullOrEmpty(Toolbar.get.m_LastLoadedLayoutName)) ? Toolbar.get.m_LastLoadedLayoutName : "Layout";
			}
			set
			{
				Toolbar.get.m_LastLoadedLayoutName = value;
				Toolbar.get.Repaint();
			}
		}
		private static void InitializeToolIcons()
		{
			if (Toolbar.s_ToolIcons != null)
			{
				return;
			}
			Toolbar.s_ToolIcons = new GUIContent[]
			{
				EditorGUIUtility.IconContent("MoveTool"),
				EditorGUIUtility.IconContent("RotateTool"),
				EditorGUIUtility.IconContent("ScaleTool"),
				EditorGUIUtility.IconContent("RectTool"),
				EditorGUIUtility.IconContent("MoveTool On"),
				EditorGUIUtility.IconContent("RotateTool On"),
				EditorGUIUtility.IconContent("ScaleTool On"),
				EditorGUIUtility.IconContent("RectTool On")
			};
			Toolbar.s_ViewToolIcons = new GUIContent[]
			{
				EditorGUIUtility.IconContent("ViewToolOrbit"),
				EditorGUIUtility.IconContent("ViewToolMove"),
				EditorGUIUtility.IconContent("ViewToolZoom"),
				EditorGUIUtility.IconContent("ViewToolOrbit"),
				EditorGUIUtility.IconContent("ViewToolOrbit On"),
				EditorGUIUtility.IconContent("ViewToolMove On"),
				EditorGUIUtility.IconContent("ViewToolZoom On"),
				EditorGUIUtility.IconContent("ViewToolOrbit On")
			};
			Toolbar.s_PivotIcons = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ToolHandleCenter"),
				EditorGUIUtility.TextContent("ToolHandlePivot")
			};
			Toolbar.s_PivotRotation = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ToolHandleLocal"),
				EditorGUIUtility.TextContent("ToolHandleGlobal")
			};
			Toolbar.s_LayerContent = EditorGUIUtility.TextContent("ToolbarLayers");
			Toolbar.s_PlayIcons = new GUIContent[]
			{
				EditorGUIUtility.IconContent("PlayButton"),
				EditorGUIUtility.IconContent("PauseButton"),
				EditorGUIUtility.IconContent("StepButton"),
				EditorGUIUtility.IconContent("PlayButtonProfile"),
				EditorGUIUtility.IconContent("PlayButton On"),
				EditorGUIUtility.IconContent("PauseButton On"),
				EditorGUIUtility.IconContent("StepButton On"),
				EditorGUIUtility.IconContent("PlayButtonProfile On"),
				EditorGUIUtility.IconContent("PlayButton Anim"),
				EditorGUIUtility.IconContent("PauseButton Anim"),
				EditorGUIUtility.IconContent("StepButton Anim"),
				EditorGUIUtility.IconContent("PlayButtonProfile Anim")
			};
		}
		public void OnEnable()
		{
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(base.Repaint));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnSelectionChange));
			Toolbar.get = this;
		}
		public void OnDisable()
		{
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(base.Repaint));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnSelectionChange));
		}
		protected override bool OnFocus()
		{
			return false;
		}
		private void OnSelectionChange()
		{
			Tools.OnSelectionChange();
			EditMode.OnSelectionChange();
			base.Repaint();
		}
		private void OnGUI()
		{
			Toolbar.InitializeToolIcons();
			bool isPlayingOrWillChangePlaymode = EditorApplication.isPlayingOrWillChangePlaymode;
			if (isPlayingOrWillChangePlaymode)
			{
				GUI.color = HostView.kPlayModeDarken;
			}
			GUIStyle gUIStyle = "AppToolbar";
			if (Event.current.type == EventType.Repaint)
			{
				gUIStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), false, false, false, false);
			}
			this.DoToolButtons();
			float num = 100f;
			float num2 = (base.position.width - num) / 2f;
			num2 = Mathf.Max(num2, 373f);
			GUILayout.BeginArea(new Rect(num2, 5f, 120f, 24f));
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.DoPlayButtons(isPlayingOrWillChangePlaymode);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			float num3 = 220f;
			num2 = base.position.width - num3;
			num2 = Mathf.Max(num2, 440f);
			GUILayout.BeginArea(new Rect(num2, 7f, base.position.width - num2 - 10f, 24f));
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.DoLayersDropDown();
			GUILayout.Space(6f);
			this.DoLayoutDropDown();
			GUILayout.EndArea();
			EditorGUI.ShowRepaints();
			Highlighter.ControlHighlightGUI(this);
		}
		private void DoToolButtons()
		{
			GUI.changed = false;
			int num = (int)((!Tools.viewToolActive) ? Tools.current : Tool.View);
			for (int i = 1; i < 5; i++)
			{
				Toolbar.s_ShownToolIcons[i] = Toolbar.s_ToolIcons[i - 1 + ((i != num) ? 0 : 4)];
				Toolbar.s_ShownToolIcons[i].tooltip = Toolbar.s_ToolIcons[i - 1].tooltip;
			}
			Toolbar.s_ShownToolIcons[0] = Toolbar.s_ViewToolIcons[(int)(Tools.viewTool + ((num != 0) ? 0 : 4))];
			num = GUI.Toolbar(new Rect(10f, 5f, 160f, 24f), num, Toolbar.s_ShownToolIcons, "Command");
			if (GUI.changed)
			{
				Tools.current = (Tool)num;
			}
			Tools.pivotMode = (PivotMode)EditorGUI.CycleButton(new Rect(190f, 8f, 64f, 18f), (int)Tools.pivotMode, Toolbar.s_PivotIcons, "ButtonLeft");
			if (Tools.current == Tool.Scale && Selection.transforms.Length < 2)
			{
				GUI.enabled = false;
			}
			PivotRotation pivotRotation = (PivotRotation)EditorGUI.CycleButton(new Rect(254f, 8f, 64f, 18f), (int)Tools.pivotRotation, Toolbar.s_PivotRotation, "ButtonRight");
			if (Tools.pivotRotation != pivotRotation)
			{
				Tools.pivotRotation = pivotRotation;
				if (pivotRotation == PivotRotation.Global)
				{
					Tools.ResetGlobalHandleRotation();
				}
			}
			if (Tools.current == Tool.Scale)
			{
				GUI.enabled = true;
			}
			if (GUI.changed)
			{
				Tools.RepaintAllToolViews();
			}
		}
		private void DoPlayButtons(bool isOrWillEnterPlaymode)
		{
			bool isPlaying = EditorApplication.isPlaying;
			GUI.changed = false;
			int num = (!isPlaying) ? 0 : 4;
			if (AnimationMode.InAnimationMode())
			{
				num = 8;
			}
			Color color = GUI.color + new Color(0.01f, 0.01f, 0.01f, 0.01f);
			GUI.contentColor = new Color(1f / color.r, 1f / color.g, 1f / color.g, 1f / color.a);
			GUILayout.Toggle(isOrWillEnterPlaymode, Toolbar.s_PlayIcons[num], "CommandLeft", new GUILayoutOption[0]);
			GUI.backgroundColor = Color.white;
			if (GUI.changed)
			{
				Toolbar.TogglePlaying();
				GUIUtility.ExitGUI();
			}
			GUI.changed = false;
			bool isPaused = GUILayout.Toggle(EditorApplication.isPaused, Toolbar.s_PlayIcons[num + 1], "CommandMid", new GUILayoutOption[0]);
			if (GUI.changed)
			{
				EditorApplication.isPaused = isPaused;
				GUIUtility.ExitGUI();
			}
			if (GUILayout.Button(Toolbar.s_PlayIcons[num + 2], "CommandRight", new GUILayoutOption[0]))
			{
				EditorApplication.Step();
				GUIUtility.ExitGUI();
			}
		}
		private void DoLayersDropDown()
		{
			GUIStyle style = "DropDown";
			Rect rect = GUILayoutUtility.GetRect(Toolbar.s_LayerContent, style);
			if (EditorGUI.ButtonMouseDown(rect, Toolbar.s_LayerContent, FocusType.Passive, style))
			{
				Rect last = GUILayoutUtility.topLevel.GetLast();
				if (LayerVisibilityWindow.ShowAtPosition(last))
				{
					GUIUtility.ExitGUI();
				}
			}
		}
		private void DoLayoutDropDown()
		{
			Rect rect = GUILayoutUtility.GetRect(Toolbar.s_LayerContent, "DropDown");
			if (EditorGUI.ButtonMouseDown(rect, GUIContent.Temp(Toolbar.lastLoadedLayoutName), FocusType.Passive, "DropDown"))
			{
				Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
				rect.x = vector.x;
				rect.y = vector.y;
				EditorUtility.Internal_DisplayPopupMenu(rect, "Window/Layouts", this, 0);
			}
			GUILayout.EndHorizontal();
		}
		private static void InternalWillTogglePlaymode()
		{
			InternalEditorUtility.RepaintAllViews();
		}
		private static void TogglePlaying()
		{
			bool isPlaying = !EditorApplication.isPlaying;
			EditorApplication.isPlaying = isPlaying;
			Toolbar.InternalWillTogglePlaymode();
		}
		internal static void RepaintToolbar()
		{
			if (Toolbar.get != null)
			{
				Toolbar.get.Repaint();
			}
		}
		public float CalcHeight()
		{
			return 30f;
		}
	}
}
