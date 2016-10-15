using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SketchUpImportDlg : EditorWindow
	{
		internal class Styles
		{
			public readonly float buttonWidth;

			public readonly GUIStyle headerStyle;

			public readonly GUIStyle toggleStyle;

			public readonly GUIStyle boxBackground = "OL Box";

			public readonly GUIContent okButton = EditorGUIUtility.TextContent("OK");

			public readonly GUIContent cancelButton = EditorGUIUtility.TextContent("Cancel");

			public readonly GUIContent nodesLabel = EditorGUIUtility.TextContent("Select the SketchUp nodes to import|Nodes in the file hierarchy");

			public readonly GUIContent windowTitle = EditorGUIUtility.TextContent("SketchUp Node Selection Dialog|SketchUp Node Selection Dialog");

			private static SketchUpImportDlg.Styles s_Styles;

			public static SketchUpImportDlg.Styles styles
			{
				get
				{
					SketchUpImportDlg.Styles arg_17_0;
					if ((arg_17_0 = SketchUpImportDlg.Styles.s_Styles) == null)
					{
						arg_17_0 = (SketchUpImportDlg.Styles.s_Styles = new SketchUpImportDlg.Styles());
					}
					return arg_17_0;
				}
			}

			public Styles()
			{
				this.buttonWidth = 32f;
				this.headerStyle = new GUIStyle(EditorStyles.toolbarButton);
				this.headerStyle.padding.left = 4;
				this.headerStyle.alignment = TextAnchor.MiddleLeft;
				this.toggleStyle = new GUIStyle(EditorStyles.toggle);
				this.toggleStyle.padding.left = 8;
				this.toggleStyle.alignment = TextAnchor.MiddleCenter;
			}
		}

		private const float kHeaderHeight = 25f;

		private const float kBottomHeight = 30f;

		private TreeView m_TreeView;

		private SketchUpTreeViewGUI m_ImportGUI;

		private SketchUpDataSource m_DataSource;

		private int[] m_Selection;

		private WeakReference m_ModelEditor;

		private readonly Vector2 m_WindowMinSize = new Vector2(350f, 350f);

		private TreeViewState m_TreeViewState;

		private bool isModal
		{
			get;
			set;
		}

		public void Init(SketchUpNodeInfo[] nodes, SketchUpImporterModelEditor suModelEditor)
		{
			base.titleContent = SketchUpImportDlg.Styles.styles.windowTitle;
			base.minSize = this.m_WindowMinSize;
			base.position = new Rect(base.position.x, base.position.y, base.minSize.x, base.minSize.y);
			this.m_TreeViewState = new TreeViewState();
			this.m_TreeView = new TreeView(this, this.m_TreeViewState);
			this.m_ImportGUI = new SketchUpTreeViewGUI(this.m_TreeView);
			this.m_DataSource = new SketchUpDataSource(this.m_TreeView, nodes);
			this.m_TreeView.Init(base.position, this.m_DataSource, this.m_ImportGUI, null);
			TreeView expr_C3 = this.m_TreeView;
			expr_C3.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_C3.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
			this.m_ModelEditor = new WeakReference(suModelEditor);
			this.isModal = false;
		}

		internal static void Launch(SketchUpNodeInfo[] nodes, SketchUpImporterModelEditor suModelEditor)
		{
			SketchUpImportDlg windowDontShow = EditorWindow.GetWindowDontShow<SketchUpImportDlg>();
			windowDontShow.Init(nodes, suModelEditor);
			windowDontShow.ShowAuxWindow();
		}

		internal static int[] LaunchAsModal(SketchUpNodeInfo[] nodes)
		{
			SketchUpImportDlg windowDontShow = EditorWindow.GetWindowDontShow<SketchUpImportDlg>();
			windowDontShow.Init(nodes, null);
			windowDontShow.isModal = true;
			windowDontShow.ShowModal();
			return windowDontShow.m_DataSource.FetchEnableNodes();
		}

		private void HandleKeyboardEvents()
		{
			Event current = Event.current;
			if (current.type == EventType.KeyDown && (current.keyCode == KeyCode.Space || current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter) && this.m_Selection != null && this.m_Selection.Length > 0)
			{
				SketchUpNode sketchUpNode = this.m_TreeView.FindItem(this.m_Selection[0]) as SketchUpNode;
				if (sketchUpNode != null && sketchUpNode != this.m_DataSource.root)
				{
					sketchUpNode.Enabled = !sketchUpNode.Enabled;
					current.Use();
					base.Repaint();
				}
			}
		}

		public void OnTreeSelectionChanged(int[] selection)
		{
			this.m_Selection = selection;
		}

		private void OnGUI()
		{
			Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
			Rect position = new Rect(0f, 0f, base.position.width, 25f);
			GUI.Label(position, string.Empty, SketchUpImportDlg.Styles.styles.headerStyle);
			GUI.Label(new Rect(10f, 2f, base.position.width, 25f), SketchUpImportDlg.Styles.styles.nodesLabel);
			Rect screenRect = new Rect(rect.x, rect.yMax - 30f, rect.width, 30f);
			GUILayout.BeginArea(screenRect);
			GUILayout.Box(string.Empty, SketchUpImportDlg.Styles.styles.boxBackground, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(1f)
			});
			GUILayout.Space(2f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			bool flag = false;
			if (this.isModal)
			{
				if (GUILayout.Button(SketchUpImportDlg.Styles.styles.okButton, new GUILayoutOption[0]))
				{
					flag = true;
				}
			}
			else if (GUILayout.Button(SketchUpImportDlg.Styles.styles.cancelButton, new GUILayoutOption[0]))
			{
				flag = true;
			}
			else if (GUILayout.Button(SketchUpImportDlg.Styles.styles.okButton, new GUILayoutOption[0]))
			{
				flag = true;
				if (this.m_ModelEditor.IsAlive)
				{
					SketchUpImporterModelEditor sketchUpImporterModelEditor = this.m_ModelEditor.Target as SketchUpImporterModelEditor;
					sketchUpImporterModelEditor.SetSelectedNodes(this.m_DataSource.FetchEnableNodes());
				}
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			rect.y = 18f;
			rect.height -= position.height + screenRect.height - 7f;
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(rect, controlID);
			this.HandleKeyboardEvents();
			if (flag)
			{
				base.Close();
			}
		}

		private void OnLostFocus()
		{
			if (!this.isModal)
			{
				base.Close();
			}
		}
	}
}
