using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class GUIViewDebuggerWindow : EditorWindow
	{
		internal class Styles
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

		private enum InstructionType
		{
			Draw,
			Clip,
			Layout,
			Unified
		}

		public GUIView m_Inspected;

		private GUIViewDebuggerWindow.InstructionType m_InstructionType = GUIViewDebuggerWindow.InstructionType.Draw;

		private bool m_ShowOverlay = true;

		[NonSerialized]
		private int m_LastSelectedRow;

		[NonSerialized]
		private bool m_QueuedPointInspection = false;

		[NonSerialized]
		private Vector2 m_PointToInspect;

		public static GUIViewDebuggerWindow.Styles s_Styles;

		private readonly SplitterState m_InstructionListDetailSplitter = new SplitterState(new float[]
		{
			30f,
			70f
		}, new int[]
		{
			32,
			32
		}, null);

		private InstructionOverlayWindow m_InstructionOverlayWindow;

		private static GUIViewDebuggerWindow s_ActiveInspector;

		private IBaseInspectView m_InstructionModeView;

		public IBaseInspectView instructionModeView
		{
			get
			{
				return this.m_InstructionModeView;
			}
		}

		public InstructionOverlayWindow InstructionOverlayWindow
		{
			get
			{
				return this.m_InstructionOverlayWindow;
			}
			set
			{
				this.m_InstructionOverlayWindow = value;
			}
		}

		public GUIViewDebuggerWindow()
		{
			this.m_InstructionModeView = new StyleDrawInspectView(this);
		}

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
			if (!(GUIViewDebuggerWindow.s_ActiveInspector == null))
			{
				GUIViewDebuggerWindow.s_ActiveInspector.RefreshData();
				GUIViewDebuggerWindow.s_ActiveInspector.Repaint();
			}
		}

		private void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			this.DoWindowPopup();
			this.DoInspectTypePopup();
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

		private void DoInspectTypePopup()
		{
			EditorGUI.BeginChangeCheck();
			GUIViewDebuggerWindow.InstructionType instructionType = (GUIViewDebuggerWindow.InstructionType)EditorGUILayout.EnumPopup(this.m_InstructionType, EditorStyles.toolbarDropDown, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_InstructionType = instructionType;
				switch (this.m_InstructionType)
				{
				case GUIViewDebuggerWindow.InstructionType.Draw:
					this.m_InstructionModeView = new StyleDrawInspectView(this);
					break;
				case GUIViewDebuggerWindow.InstructionType.Clip:
					this.m_InstructionModeView = new GUIClipInspectView(this);
					break;
				case GUIViewDebuggerWindow.InstructionType.Layout:
					this.m_InstructionModeView = new GUILayoutInspectView(this);
					break;
				case GUIViewDebuggerWindow.InstructionType.Unified:
					this.m_InstructionModeView = new UnifiedInspectView(this);
					break;
				}
				this.m_InstructionModeView.UpdateInstructions();
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
			else if (this.m_Inspected != null)
			{
				this.instructionModeView.ShowOverlay();
			}
		}

		private bool CanInspectView(GUIView view)
		{
			bool result;
			if (view == null)
			{
				result = false;
			}
			else
			{
				EditorWindow editorWindow = GUIViewDebuggerWindow.GetEditorWindow(view);
				result = (editorWindow == null || (!(editorWindow == this) && !(editorWindow == this.m_InstructionOverlayWindow)));
			}
			return result;
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
				else
				{
					GUIViewDebuggerHelper.StopDebugging();
				}
				this.instructionModeView.Unselect();
			}
			base.Repaint();
		}

		private static EditorWindow GetEditorWindow(GUIView view)
		{
			HostView hostView = view as HostView;
			EditorWindow result;
			if (hostView != null)
			{
				result = hostView.actualView;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private static string GetViewName(GUIView view)
		{
			EditorWindow editorWindow = GUIViewDebuggerWindow.GetEditorWindow(view);
			string result;
			if (editorWindow != null)
			{
				result = editorWindow.titleContent.text;
			}
			else
			{
				result = view.GetType().Name;
			}
			return result;
		}

		private void RefreshData()
		{
			this.instructionModeView.UpdateInstructions();
		}

		private void ShowDrawInstructions()
		{
			if (!(this.m_Inspected == null))
			{
				if (this.m_QueuedPointInspection)
				{
					this.instructionModeView.Unselect();
					this.instructionModeView.SelectRow(this.FindInstructionUnderPoint(this.m_PointToInspect));
					this.m_QueuedPointInspection = false;
				}
				SplitterGUILayout.BeginHorizontalSplit(this.m_InstructionListDetailSplitter, new GUILayoutOption[0]);
				this.instructionModeView.DrawInstructionList();
				EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				this.instructionModeView.DrawSelectedInstructionDetails();
				EditorGUILayout.EndVertical();
				SplitterGUILayout.EndHorizontalSplit();
			}
		}

		public void HighlightInstruction(GUIView view, Rect instructionRect, GUIStyle style)
		{
			if (this.m_ShowOverlay)
			{
				if (this.m_InstructionOverlayWindow == null)
				{
					this.m_InstructionOverlayWindow = ScriptableObject.CreateInstance<InstructionOverlayWindow>();
				}
				this.m_InstructionOverlayWindow.Show(view, instructionRect, style);
				base.Focus();
			}
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
			int result;
			for (int i = 0; i < instructionCount; i++)
			{
				if (GUIViewDebuggerHelper.GetRectFromInstruction(i).Contains(point))
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		private void OnDisable()
		{
			GUIViewDebuggerHelper.StopDebugging();
			if (this.m_InstructionOverlayWindow != null)
			{
				this.m_InstructionOverlayWindow.Close();
			}
		}
	}
}
