using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class GUIViewDebuggerWindow : EditorWindow
	{
		private class Styles
		{
			public readonly GUIStyle listItem = new GUIStyle("PR Label");

			public readonly GUIStyle listItemBackground = new GUIStyle("CN EntryBackOdd");

			public readonly GUIStyle listBackgroundStyle = new GUIStyle("CN Box");

			public readonly GUIStyle boxStyle = new GUIStyle("CN Box");

			public readonly GUIStyle stackframeStyle = new GUIStyle(EditorStyles.label);

			public readonly GUIStyle stacktraceBackground = new GUIStyle("CN Box");

			public readonly GUIStyle centeredText = new GUIStyle("PR Label");

			public Styles()
			{
				this.stackframeStyle.margin = new RectOffset(0, 0, 0, 0);
				this.stackframeStyle.padding = new RectOffset(0, 0, 0, 0);
				this.stacktraceBackground.padding = new RectOffset(5, 5, 5, 5);
				this.centeredText.alignment = TextAnchor.MiddleCenter;
				this.centeredText.stretchHeight = true;
				this.centeredText.stretchWidth = true;
			}
		}

		private class GUIInstruction
		{
			public Rect rect;

			public GUIStyle usedGUIStyle = GUIStyle.none;

			public GUIContent usedGUIContent = GUIContent.none;

			public StackFrame[] stackframes;

			public void Reset()
			{
				this.rect = default(Rect);
				this.usedGUIStyle = GUIStyle.none;
				this.usedGUIContent = GUIContent.none;
			}
		}

		[Serializable]
		private class CachedInstructionInfo
		{
			public SerializedObject styleContainerSerializedObject;

			public SerializedProperty styleSerializedProperty;

			public readonly GUIStyleHolder styleContainer;

			public CachedInstructionInfo()
			{
				this.styleContainer = ScriptableObject.CreateInstance<GUIStyleHolder>();
			}
		}

		private GUIView m_Inspected;

		private bool m_ShowOverlay = true;

		[NonSerialized]
		private readonly ListViewState m_ListViewState = new ListViewState();

		[NonSerialized]
		private GUIViewDebuggerWindow.GUIInstruction m_Instruction;

		[NonSerialized]
		private int m_LastSelectedRow;

		[NonSerialized]
		private Vector2 m_PointToInspect;

		[NonSerialized]
		private bool m_QueuedPointInspection;

		[NonSerialized]
		private GUIViewDebuggerWindow.CachedInstructionInfo m_CachedinstructionInfo;

		private static GUIViewDebuggerWindow.Styles s_Styles;

		private Vector2 m_InstructionDetailsScrollPos = default(Vector2);

		private Vector2 m_StacktraceScrollPos = default(Vector2);

		private readonly SplitterState m_InstructionListDetailSplitter = new SplitterState(new float[]
		{
			30f,
			70f
		}, new int[]
		{
			32,
			32
		}, null);

		private readonly SplitterState m_InstructionDetailStacktraceSplitter = new SplitterState(new float[]
		{
			80f,
			20f
		}, new int[]
		{
			100,
			100
		}, null);

		private InstructionOverlayWindow m_InstructionOverlayWindow;

		private static GUIViewDebuggerWindow s_ActiveInspector;

		private static void Init()
		{
			if (GUIViewDebuggerWindow.s_ActiveInspector == null)
			{
				GUIViewDebuggerWindow gUIViewDebuggerWindow = (GUIViewDebuggerWindow)EditorWindow.GetWindow(typeof(GUIViewDebuggerWindow));
				GUIViewDebuggerWindow.s_ActiveInspector = gUIViewDebuggerWindow;
			}
			GUIViewDebuggerWindow.s_ActiveInspector.Show();
		}

		private static void InspectPoint(Vector2 point)
		{
			Debug.Log("Inspecting " + point);
			GUIViewDebuggerWindow.s_ActiveInspector.InspectPointAt(point);
		}

		private void OnEnable()
		{
			base.titleContent = new GUIContent("GUI Inspector");
		}

		private void OnGUI()
		{
			this.InitializeStylesIfNeeded();
			this.DoToolbar();
			this.ShowDrawInstructions();
		}

		private void InitializeStylesIfNeeded()
		{
			if (GUIViewDebuggerWindow.s_Styles == null)
			{
				GUIViewDebuggerWindow.s_Styles = new GUIViewDebuggerWindow.Styles();
			}
		}

		private static void OnInspectedViewChanged()
		{
			if (GUIViewDebuggerWindow.s_ActiveInspector == null)
			{
				return;
			}
			GUIViewDebuggerWindow.s_ActiveInspector.Repaint();
		}

		private void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			this.DoWindowPopup();
			this.DoInstructionOverlayToggle();
			GUILayout.EndHorizontal();
		}

		private void DoWindowPopup()
		{
			string t = "<Please Select>";
			if (this.m_Inspected != null)
			{
				t = GUIViewDebuggerWindow.GetViewName(this.m_Inspected);
			}
			GUILayout.Label("Inspected Window: ", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			Rect rect = GUILayoutUtility.GetRect(GUIContent.Temp(t), EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			if (GUI.Button(rect, GUIContent.Temp(t), EditorStyles.toolbarDropDown))
			{
				List<GUIView> list = new List<GUIView>();
				GUIViewDebuggerHelper.GetViews(list);
				List<GUIContent> list2 = new List<GUIContent>(list.Count + 1);
				list2.Add(new GUIContent("None"));
				int selected = 0;
				List<GUIView> list3 = new List<GUIView>(list.Count + 1);
				for (int i = 0; i < list.Count; i++)
				{
					GUIView gUIView = list[i];
					if (this.CanInspectView(gUIView))
					{
						string text = list2.Count + ". " + GUIViewDebuggerWindow.GetViewName(gUIView);
						GUIContent item = new GUIContent(text);
						list2.Add(item);
						list3.Add(gUIView);
						if (gUIView == this.m_Inspected)
						{
							selected = list3.Count;
						}
					}
				}
				EditorUtility.DisplayCustomMenu(rect, list2.ToArray(), selected, new EditorUtility.SelectMenuItemFunction(this.OnWindowSelected), list3);
			}
		}

		private void DoInstructionOverlayToggle()
		{
			EditorGUI.BeginChangeCheck();
			this.m_ShowOverlay = GUILayout.Toggle(this.m_ShowOverlay, GUIContent.Temp("Show overlay"), EditorStyles.toolbarButton, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.OnShowOverlayChanged();
			}
		}

		private void OnShowOverlayChanged()
		{
			if (!this.m_ShowOverlay)
			{
				if (this.m_InstructionOverlayWindow != null)
				{
					this.m_InstructionOverlayWindow.Close();
				}
			}
			else if (this.m_Inspected != null && this.m_Instruction != null)
			{
				this.HighlightInstruction(this.m_Inspected, this.m_Instruction.rect, this.m_Instruction.usedGUIStyle);
			}
		}

		private bool CanInspectView(GUIView view)
		{
			EditorWindow editorWindow = GUIViewDebuggerWindow.GetEditorWindow(view);
			return editorWindow == null || (!(editorWindow == this) && !(editorWindow == this.m_InstructionOverlayWindow));
		}

		private void OnWindowSelected(object userdata, string[] options, int selected)
		{
			selected--;
			GUIView gUIView;
			if (selected >= 0)
			{
				List<GUIView> list = (List<GUIView>)userdata;
				gUIView = list[selected];
			}
			else
			{
				gUIView = null;
			}
			if (this.m_Inspected != gUIView)
			{
				if (this.m_InstructionOverlayWindow != null)
				{
					this.m_InstructionOverlayWindow.Close();
				}
				this.m_Inspected = gUIView;
				if (this.m_Inspected != null)
				{
					GUIViewDebuggerHelper.DebugWindow(this.m_Inspected);
					this.m_Inspected.Repaint();
				}
				this.m_ListViewState.row = -1;
				this.m_ListViewState.selectionChanged = true;
				this.m_Instruction = null;
			}
			base.Repaint();
		}

		private static EditorWindow GetEditorWindow(GUIView view)
		{
			HostView hostView = view as HostView;
			if (hostView != null)
			{
				return hostView.actualView;
			}
			return null;
		}

		private static string GetViewName(GUIView view)
		{
			EditorWindow editorWindow = GUIViewDebuggerWindow.GetEditorWindow(view);
			if (editorWindow != null)
			{
				return editorWindow.titleContent.text;
			}
			return view.GetType().Name;
		}

		private void ShowDrawInstructions()
		{
			if (this.m_Inspected == null)
			{
				return;
			}
			this.m_ListViewState.totalRows = GUIViewDebuggerHelper.GetInstructionCount();
			if (this.m_QueuedPointInspection)
			{
				this.m_ListViewState.row = this.FindInstructionUnderPoint(this.m_PointToInspect);
				this.m_ListViewState.selectionChanged = true;
				this.m_QueuedPointInspection = false;
				this.m_Instruction.Reset();
			}
			SplitterGUILayout.BeginHorizontalSplit(this.m_InstructionListDetailSplitter, new GUILayoutOption[0]);
			this.DrawInstructionList();
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			if (this.m_ListViewState.selectionChanged)
			{
				this.OnSelectedInstructionChanged();
			}
			this.DrawSelectedInstructionDetails();
			EditorGUILayout.EndVertical();
			SplitterGUILayout.EndHorizontalSplit();
		}

		private void OnSelectedInstructionChanged()
		{
			if (this.m_ListViewState.row >= 0)
			{
				if (this.m_Instruction == null)
				{
					this.m_Instruction = new GUIViewDebuggerWindow.GUIInstruction();
				}
				if (this.m_CachedinstructionInfo == null)
				{
					this.m_CachedinstructionInfo = new GUIViewDebuggerWindow.CachedInstructionInfo();
				}
				this.m_Instruction.rect = GUIViewDebuggerHelper.GetRectFromInstruction(this.m_ListViewState.row);
				this.m_Instruction.usedGUIStyle = GUIViewDebuggerHelper.GetStyleFromInstruction(this.m_ListViewState.row);
				this.m_Instruction.usedGUIContent = GUIViewDebuggerHelper.GetContentFromInstruction(this.m_ListViewState.row);
				this.m_Instruction.stackframes = GUIViewDebuggerHelper.GetManagedStackTrace(this.m_ListViewState.row);
				this.m_CachedinstructionInfo.styleContainer.inspectedStyle = this.m_Instruction.usedGUIStyle;
				this.m_CachedinstructionInfo.styleContainerSerializedObject = null;
				this.m_CachedinstructionInfo.styleSerializedProperty = null;
				this.GetSelectedStyleProperty(out this.m_CachedinstructionInfo.styleContainerSerializedObject, out this.m_CachedinstructionInfo.styleSerializedProperty);
				this.HighlightInstruction(this.m_Inspected, this.m_Instruction.rect, this.m_Instruction.usedGUIStyle);
			}
			else
			{
				this.m_Instruction = null;
				this.m_CachedinstructionInfo = null;
				if (this.m_InstructionOverlayWindow != null)
				{
					this.m_InstructionOverlayWindow.Close();
				}
			}
		}

		private void DrawInstructionList()
		{
			Event current = Event.current;
			EditorGUILayout.BeginVertical(GUIViewDebuggerWindow.s_Styles.listBackgroundStyle, new GUILayoutOption[0]);
			GUILayout.Label("Instructions", new GUILayoutOption[0]);
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			foreach (ListViewElement el in ListViewGUI.ListView(this.m_ListViewState, GUIViewDebuggerWindow.s_Styles.listBackgroundStyle, new GUILayoutOption[0]))
			{
				if (current.type == EventType.MouseDown && current.button == 0 && el.position.Contains(current.mousePosition) && current.clickCount == 2)
				{
					this.ShowInstructionInExternalEditor(el.row);
				}
				if (current.type == EventType.Repaint)
				{
					string instructionName = this.GetInstructionName(el);
					GUIContent content = GUIContent.Temp(instructionName);
					GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(el.position, false, false, this.m_ListViewState.row == el.row, false);
					GUIViewDebuggerWindow.s_Styles.listItem.Draw(el.position, content, controlID, this.m_ListViewState.row == el.row);
				}
			}
			EditorGUILayout.EndVertical();
		}

		private void ShowInstructionInExternalEditor(int row)
		{
			StackFrame[] managedStackTrace = GUIViewDebuggerHelper.GetManagedStackTrace(row);
			int interestingFrameIndex = this.GetInterestingFrameIndex(managedStackTrace);
			StackFrame stackFrame = managedStackTrace[interestingFrameIndex];
			InternalEditorUtility.OpenFileAtLineExternal(stackFrame.sourceFile, (int)stackFrame.lineNumber);
		}

		private string GetInstructionName(ListViewElement el)
		{
			int row = el.row;
			StackFrame[] managedStackTrace = GUIViewDebuggerHelper.GetManagedStackTrace(row);
			string instructionListName = this.GetInstructionListName(managedStackTrace);
			return string.Format("{0}. {1}", row, instructionListName);
		}

		private string GetInstructionListName(StackFrame[] stacktrace)
		{
			int num = this.GetInterestingFrameIndex(stacktrace);
			if (num > 0)
			{
				num--;
			}
			StackFrame stackFrame = stacktrace[num];
			return stackFrame.methodName;
		}

		private int GetInterestingFrameIndex(StackFrame[] stacktrace)
		{
			string dataPath = Application.dataPath;
			int num = -1;
			for (int i = 0; i < stacktrace.Length; i++)
			{
				StackFrame stackFrame = stacktrace[i];
				if (!string.IsNullOrEmpty(stackFrame.sourceFile))
				{
					if (!stackFrame.signature.StartsWith("UnityEngine.GUI"))
					{
						if (!stackFrame.signature.StartsWith("UnityEditor.EditorGUI"))
						{
							if (num == -1)
							{
								num = i;
							}
							if (stackFrame.sourceFile.StartsWith(dataPath))
							{
								return i;
							}
						}
					}
				}
			}
			if (num != -1)
			{
				return num;
			}
			return stacktrace.Length - 1;
		}

		private void DrawSelectedInstructionDetails()
		{
			if (this.m_Instruction == null)
			{
				EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Label("Select a Instruction on the left to see details", GUIViewDebuggerWindow.s_Styles.centeredText, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
				return;
			}
			SplitterGUILayout.BeginVerticalSplit(this.m_InstructionDetailStacktraceSplitter, new GUILayoutOption[0]);
			this.m_InstructionDetailsScrollPos = EditorGUILayout.BeginScrollView(this.m_InstructionDetailsScrollPos, GUIViewDebuggerWindow.s_Styles.boxStyle, new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(true))
			{
				this.DrawInspectedRect();
			}
			this.DrawInspectedStyle();
			using (new EditorGUI.DisabledScope(true))
			{
				this.DrawInspectedGUIContent();
			}
			EditorGUILayout.EndScrollView();
			this.DrawInspectedStacktrace();
			SplitterGUILayout.EndVerticalSplit();
		}

		private void DrawInspectedRect()
		{
			EditorGUILayout.RectField(GUIContent.Temp("Rect"), this.m_Instruction.rect, new GUILayoutOption[0]);
		}

		private void DrawInspectedGUIContent()
		{
			GUILayout.Label(GUIContent.Temp("GUIContent"), new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			EditorGUILayout.TextField(this.m_Instruction.usedGUIContent.text, new GUILayoutOption[0]);
			EditorGUILayout.ObjectField(this.m_Instruction.usedGUIContent.image, typeof(Texture2D), false, new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
		}

		private void DrawInspectedStyle()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_CachedinstructionInfo.styleSerializedProperty, GUIContent.Temp("Style"), true, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_CachedinstructionInfo.styleContainerSerializedObject.ApplyModifiedPropertiesWithoutUndo();
				this.m_Inspected.Repaint();
			}
		}

		private void GetSelectedStyleProperty(out SerializedObject serializedObject, out SerializedProperty styleProperty)
		{
			GUISkin gUISkin = null;
			GUISkin current = GUISkin.current;
			GUIStyle gUIStyle = current.FindStyle(this.m_Instruction.usedGUIStyle.name);
			if (gUIStyle != null && gUIStyle == this.m_Instruction.usedGUIStyle)
			{
				gUISkin = current;
			}
			styleProperty = null;
			if (gUISkin != null)
			{
				serializedObject = new SerializedObject(gUISkin);
				SerializedProperty iterator = serializedObject.GetIterator();
				bool enterChildren = true;
				while (iterator.NextVisible(enterChildren))
				{
					if (iterator.type == "GUIStyle")
					{
						enterChildren = false;
						SerializedProperty serializedProperty = iterator.FindPropertyRelative("m_Name");
						if (serializedProperty.stringValue == this.m_Instruction.usedGUIStyle.name)
						{
							styleProperty = iterator;
							return;
						}
					}
					else
					{
						enterChildren = true;
					}
				}
				Debug.Log(string.Format("Showing editable Style from GUISkin: {0}, IsPersistant: {1}", gUISkin.name, EditorUtility.IsPersistent(gUISkin)));
			}
			serializedObject = new SerializedObject(this.m_CachedinstructionInfo.styleContainer);
			styleProperty = serializedObject.FindProperty("inspectedStyle");
		}

		private void DrawInspectedStacktrace()
		{
			this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(false)
			});
			if (this.m_Instruction.stackframes != null)
			{
				StackFrame[] stackframes = this.m_Instruction.stackframes;
				for (int i = 0; i < stackframes.Length; i++)
				{
					StackFrame stackFrame = stackframes[i];
					if (!string.IsNullOrEmpty(stackFrame.sourceFile))
					{
						GUILayout.Label(string.Format("{0} [{1}:{2}]", stackFrame.signature, stackFrame.sourceFile, stackFrame.lineNumber), GUIViewDebuggerWindow.s_Styles.stackframeStyle, new GUILayoutOption[0]);
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}

		private void HighlightInstruction(GUIView view, Rect instructionRect, GUIStyle style)
		{
			if (this.m_ListViewState.row < 0)
			{
				return;
			}
			if (!this.m_ShowOverlay)
			{
				return;
			}
			if (this.m_InstructionOverlayWindow == null)
			{
				this.m_InstructionOverlayWindow = ScriptableObject.CreateInstance<InstructionOverlayWindow>();
			}
			this.m_InstructionOverlayWindow.Show(view, instructionRect, style);
			base.Focus();
		}

		private void InspectPointAt(Vector2 point)
		{
			this.m_PointToInspect = point;
			this.m_QueuedPointInspection = true;
			this.m_Inspected.Repaint();
			base.Repaint();
		}

		private int FindInstructionUnderPoint(Vector2 point)
		{
			int instructionCount = GUIViewDebuggerHelper.GetInstructionCount();
			for (int i = 0; i < instructionCount; i++)
			{
				if (GUIViewDebuggerHelper.GetRectFromInstruction(i).Contains(point))
				{
					return i;
				}
			}
			return -1;
		}

		private void OnDisable()
		{
			if (this.m_Inspected != null)
			{
				GUIViewDebuggerHelper.DebugWindow(this.m_Inspected);
			}
			if (this.m_InstructionOverlayWindow != null)
			{
				this.m_InstructionOverlayWindow.Close();
			}
		}
	}
}
