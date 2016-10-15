using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Inspector", useTypeNameAsIconName = true)]
	internal class InspectorWindow : EditorWindow, IHasCustomMenu
	{
		internal class Styles
		{
			public readonly GUIStyle preToolbar = "preToolbar";

			public readonly GUIStyle preToolbar2 = "preToolbar2";

			public readonly GUIStyle preDropDown = "preDropDown";

			public readonly GUIStyle dragHandle = "RL DragHandle";

			public readonly GUIStyle lockButton = "IN LockButton";

			public readonly GUIContent preTitle = EditorGUIUtility.TextContent("Preview");

			public readonly GUIContent labelTitle = EditorGUIUtility.TextContent("Asset Labels");

			public readonly GUIContent addComponentLabel = EditorGUIUtility.TextContent("Add Component");

			public GUIStyle preBackground = "preBackground";

			public GUIStyle addComponentArea = EditorStyles.inspectorTitlebar;

			public GUIStyle addComponentButtonStyle = "LargeButton";

			public GUIStyle previewMiniLabel = new GUIStyle(EditorStyles.whiteMiniLabel);

			public GUIStyle typeSelection = new GUIStyle("PR Label");

			public GUIStyle lockedHeaderButton = "preButton";

			public GUIStyle stickyNote = new GUIStyle("VCS_StickyNote");

			public GUIStyle stickyNoteArrow = new GUIStyle("VCS_StickyNoteArrow");

			public GUIStyle stickyNotePerforce = new GUIStyle("VCS_StickyNoteP4");

			public GUIStyle stickyNoteLabel = new GUIStyle("VCS_StickyNoteLabel");

			public Styles()
			{
				this.typeSelection.padding.left = 12;
			}
		}

		private const float kBottomToolbarHeight = 17f;

		internal const int kInspectorPaddingLeft = 14;

		internal const int kInspectorPaddingRight = 4;

		private const long delayRepaintWhilePlayingAnimation = 150L;

		public Vector2 m_ScrollPosition;

		public InspectorMode m_InspectorMode;

		private static readonly List<InspectorWindow> m_AllInspectors = new List<InspectorWindow>();

		private static bool s_AllOptimizedGUIBlocksNeedsRebuild;

		private long s_LastUpdateWhilePlayingAnimation;

		private bool m_ResetKeyboardControl;

		protected ActiveEditorTracker m_Tracker;

		private Editor m_LastInteractedEditor;

		private bool m_IsOpenForEdit;

		private static InspectorWindow.Styles s_Styles;

		[SerializeField]
		private PreviewResizer m_PreviewResizer = new PreviewResizer();

		[SerializeField]
		private PreviewWindow m_PreviewWindow;

		private LabelGUI m_LabelGUI = new LabelGUI();

		private AssetBundleNameGUI m_AssetBundleNameGUI = new AssetBundleNameGUI();

		private TypeSelectionList m_TypeSelectionList;

		private double m_lastRenderedTime;

		private bool m_InvalidateGUIBlockCache = true;

		private List<IPreviewable> m_Previews;

		private IPreviewable m_SelectedPreview;

		public static InspectorWindow s_CurrentInspectorWindow;

		internal static InspectorWindow.Styles styles
		{
			get
			{
				InspectorWindow.Styles arg_17_0;
				if ((arg_17_0 = InspectorWindow.s_Styles) == null)
				{
					arg_17_0 = (InspectorWindow.s_Styles = new InspectorWindow.Styles());
				}
				return arg_17_0;
			}
		}

		public bool isLocked
		{
			get
			{
				this.CreateTracker();
				return this.m_Tracker.isLocked;
			}
			set
			{
				this.CreateTracker();
				this.m_Tracker.isLocked = value;
			}
		}

		private void Awake()
		{
			if (!InspectorWindow.m_AllInspectors.Contains(this))
			{
				InspectorWindow.m_AllInspectors.Add(this);
			}
		}

		private void OnDestroy()
		{
			if (this.m_PreviewWindow != null)
			{
				this.m_PreviewWindow.Close();
			}
			if (this.m_Tracker != null && !this.m_Tracker.Equals(ActiveEditorTracker.sharedTracker))
			{
				this.m_Tracker.Destroy();
			}
		}

		protected virtual void OnEnable()
		{
			this.RefreshTitle();
			base.minSize = new Vector2(275f, 50f);
			if (!InspectorWindow.m_AllInspectors.Contains(this))
			{
				InspectorWindow.m_AllInspectors.Add(this);
			}
			this.m_PreviewResizer.Init("InspectorPreview");
			this.m_LabelGUI.OnEnable();
		}

		protected virtual void OnDisable()
		{
			InspectorWindow.m_AllInspectors.Remove(this);
		}

		private void OnLostFocus()
		{
			EditorGUI.EndEditingActiveTextField();
			this.m_LabelGUI.OnLostFocus();
		}

		internal static void RepaintAllInspectors()
		{
			foreach (InspectorWindow current in InspectorWindow.m_AllInspectors)
			{
				current.Repaint();
			}
		}

		internal static List<InspectorWindow> GetInspectors()
		{
			return InspectorWindow.m_AllInspectors;
		}

		private void OnSelectionChange()
		{
			this.m_Previews = null;
			this.m_SelectedPreview = null;
			this.m_TypeSelectionList = null;
			this.m_Parent.ClearKeyboardControl();
			ScriptAttributeUtility.ClearGlobalCache();
			base.Repaint();
		}

		public static InspectorWindow[] GetAllInspectorWindows()
		{
			return InspectorWindow.m_AllInspectors.ToArray();
		}

		private void OnInspectorUpdate()
		{
			if (this.m_Tracker != null)
			{
				this.m_Tracker.VerifyModifiedMonoBehaviours();
				if (!this.m_Tracker.isDirty || !this.ReadyToRepaint())
				{
					return;
				}
			}
			base.Repaint();
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Normal"), this.m_InspectorMode == InspectorMode.Normal, new GenericMenu.MenuFunction(this.SetNormal));
			menu.AddItem(new GUIContent("Debug"), this.m_InspectorMode == InspectorMode.Debug, new GenericMenu.MenuFunction(this.SetDebug));
			if (Unsupported.IsDeveloperBuild())
			{
				menu.AddItem(new GUIContent("Debug-Internal"), this.m_InspectorMode == InspectorMode.DebugInternal, new GenericMenu.MenuFunction(this.SetDebugInternal));
			}
			menu.AddSeparator(string.Empty);
			menu.AddItem(new GUIContent("Lock"), this.m_Tracker != null && this.isLocked, new GenericMenu.MenuFunction(this.FlipLocked));
		}

		private void RefreshTitle()
		{
			string icon = "UnityEditor.InspectorWindow";
			if (this.m_InspectorMode == InspectorMode.Normal)
			{
				base.titleContent = EditorGUIUtility.TextContentWithIcon("Inspector", icon);
			}
			else
			{
				base.titleContent = EditorGUIUtility.TextContentWithIcon("Debug", icon);
			}
		}

		private void SetMode(InspectorMode mode)
		{
			this.m_InspectorMode = mode;
			this.RefreshTitle();
			this.CreateTracker();
			this.m_Tracker.inspectorMode = mode;
			this.m_ResetKeyboardControl = true;
		}

		private void SetDebug()
		{
			this.SetMode(InspectorMode.Debug);
		}

		private void SetNormal()
		{
			this.SetMode(InspectorMode.Normal);
		}

		private void SetDebugInternal()
		{
			this.SetMode(InspectorMode.DebugInternal);
		}

		private void FlipLocked()
		{
			this.isLocked = !this.isLocked;
		}

		private static void DoInspectorDragAndDrop(Rect rect, UnityEngine.Object[] targets)
		{
			if (!InspectorWindow.Dragging(rect))
			{
				return;
			}
			DragAndDrop.visualMode = InternalEditorUtility.InspectorWindowDrag(targets, Event.current.type == EventType.DragPerform);
			if (Event.current.type == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();
			}
		}

		private static bool Dragging(Rect rect)
		{
			return (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) && rect.Contains(Event.current.mousePosition);
		}

		public ActiveEditorTracker GetTracker()
		{
			this.CreateTracker();
			return this.m_Tracker;
		}

		protected virtual void CreateTracker()
		{
			if (this.m_Tracker != null)
			{
				this.m_Tracker.inspectorMode = this.m_InspectorMode;
				return;
			}
			ActiveEditorTracker sharedTracker = ActiveEditorTracker.sharedTracker;
			bool flag = InspectorWindow.m_AllInspectors.Any((InspectorWindow i) => i.m_Tracker != null && i.m_Tracker.Equals(sharedTracker));
			this.m_Tracker = ((!flag) ? ActiveEditorTracker.sharedTracker : new ActiveEditorTracker());
			this.m_Tracker.inspectorMode = this.m_InspectorMode;
			this.m_Tracker.RebuildIfNecessary();
		}

		protected virtual void CreatePreviewables()
		{
			if (this.m_Previews != null)
			{
				return;
			}
			this.m_Previews = new List<IPreviewable>();
			if (this.m_Tracker.activeEditors.Length == 0)
			{
				return;
			}
			Editor[] activeEditors = this.m_Tracker.activeEditors;
			for (int i = 0; i < activeEditors.Length; i++)
			{
				Editor editor = activeEditors[i];
				IEnumerable<IPreviewable> previewsForType = this.GetPreviewsForType(editor);
				foreach (IPreviewable current in previewsForType)
				{
					this.m_Previews.Add(current);
				}
			}
		}

		private IEnumerable<IPreviewable> GetPreviewsForType(Editor editor)
		{
			List<IPreviewable> list = new List<IPreviewable>();
			Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
			for (int i = 0; i < loadedAssemblies.Length; i++)
			{
				Assembly assembly = loadedAssemblies[i];
				Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
				Type[] array = typesFromAssembly;
				for (int j = 0; j < array.Length; j++)
				{
					Type type = array[j];
					if (typeof(IPreviewable).IsAssignableFrom(type))
					{
						if (!typeof(Editor).IsAssignableFrom(type))
						{
							object[] customAttributes = type.GetCustomAttributes(typeof(CustomPreviewAttribute), false);
							object[] array2 = customAttributes;
							for (int k = 0; k < array2.Length; k++)
							{
								CustomPreviewAttribute customPreviewAttribute = (CustomPreviewAttribute)array2[k];
								if (customPreviewAttribute.m_Type == editor.target.GetType())
								{
									IPreviewable previewable = Activator.CreateInstance(type) as IPreviewable;
									previewable.Initialize(editor.targets);
									list.Add(previewable);
								}
							}
						}
					}
				}
			}
			return list;
		}

		protected virtual void ShowButton(Rect r)
		{
			bool flag = GUI.Toggle(r, this.isLocked, GUIContent.none, InspectorWindow.styles.lockButton);
			if (flag != this.isLocked)
			{
				this.isLocked = flag;
				this.m_Tracker.RebuildIfNecessary();
			}
		}

		protected virtual void OnGUI()
		{
			Profiler.BeginSample("InspectorWindow.OnGUI");
			this.CreateTracker();
			this.CreatePreviewables();
			InspectorWindow.FlushAllOptimizedGUIBlocksIfNeeded();
			this.ResetKeyboardControl();
			this.m_ScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
			if (Event.current.type == EventType.Repaint)
			{
				this.m_Tracker.ClearDirty();
			}
			InspectorWindow.s_CurrentInspectorWindow = this;
			Editor[] activeEditors = this.m_Tracker.activeEditors;
			this.AssignAssetEditor(activeEditors);
			Profiler.BeginSample("InspectorWindow.DrawEditors()");
			this.DrawEditors(activeEditors);
			Profiler.EndSample();
			if (this.m_Tracker.hasComponentsWhichCannotBeMultiEdited)
			{
				if (activeEditors.Length == 0 && !this.m_Tracker.isLocked && Selection.objects.Length > 0)
				{
					this.DrawSelectionPickerList();
				}
				else
				{
					Rect rect = GUILayoutUtility.GetRect(10f, 4f, EditorStyles.inspectorTitlebar);
					if (Event.current.type == EventType.Repaint)
					{
						this.DrawSplitLine(rect.y);
					}
					GUILayout.Label("Components that are only on some of the selected objects cannot be multi-edited.", EditorStyles.helpBox, new GUILayoutOption[0]);
					GUILayout.Space(4f);
				}
			}
			InspectorWindow.s_CurrentInspectorWindow = null;
			EditorGUI.indentLevel = 0;
			this.AddComponentButton(this.m_Tracker.activeEditors);
			GUI.enabled = true;
			this.CheckDragAndDrop(this.m_Tracker.activeEditors);
			this.MoveFocusOnKeyPress();
			GUILayout.EndScrollView();
			Profiler.BeginSample("InspectorWindow.DrawPreviewAndLabels");
			this.DrawPreviewAndLabels();
			Profiler.EndSample();
			if (this.m_Tracker.activeEditors.Length > 0)
			{
				this.DrawVCSShortInfo();
			}
			Profiler.EndSample();
		}

		public virtual Editor GetLastInteractedEditor()
		{
			return this.m_LastInteractedEditor;
		}

		public IPreviewable GetEditorThatControlsPreview(IPreviewable[] editors)
		{
			if (editors.Length == 0)
			{
				return null;
			}
			if (this.m_SelectedPreview != null)
			{
				return this.m_SelectedPreview;
			}
			IPreviewable lastInteractedEditor = this.GetLastInteractedEditor();
			Type type = (lastInteractedEditor == null) ? null : lastInteractedEditor.GetType();
			IPreviewable previewable = null;
			IPreviewable previewable2 = null;
			for (int i = 0; i < editors.Length; i++)
			{
				IPreviewable previewable3 = editors[i];
				if (previewable3 != null && !(previewable3.target == null))
				{
					if (!EditorUtility.IsPersistent(previewable3.target) || !(AssetDatabase.GetAssetPath(previewable3.target) != AssetDatabase.GetAssetPath(editors[0].target)))
					{
						if (!(editors[0] is AssetImporterInspector) || previewable3 is AssetImporterInspector)
						{
							if (previewable3.HasPreviewGUI())
							{
								if (previewable3 == lastInteractedEditor)
								{
									return previewable3;
								}
								if (previewable2 == null && previewable3.GetType() == type)
								{
									previewable2 = previewable3;
								}
								if (previewable == null)
								{
									previewable = previewable3;
								}
							}
						}
					}
				}
			}
			if (previewable2 != null)
			{
				return previewable2;
			}
			if (previewable != null)
			{
				return previewable;
			}
			return null;
		}

		public IPreviewable[] GetEditorsWithPreviews(Editor[] editors)
		{
			IList<IPreviewable> list = new List<IPreviewable>();
			int num = -1;
			for (int i = 0; i < editors.Length; i++)
			{
				Editor editor = editors[i];
				num++;
				if (!(editor.target == null))
				{
					if (!EditorUtility.IsPersistent(editor.target) || !(AssetDatabase.GetAssetPath(editor.target) != AssetDatabase.GetAssetPath(editors[0].target)))
					{
						if (EditorUtility.IsPersistent(editors[0].target) || !EditorUtility.IsPersistent(editor.target))
						{
							if (!this.ShouldCullEditor(editors, num))
							{
								if (!(editors[0] is AssetImporterInspector) || editor is AssetImporterInspector)
								{
									if (editor.HasPreviewGUI())
									{
										list.Add(editor);
									}
								}
							}
						}
					}
				}
			}
			foreach (IPreviewable current in this.m_Previews)
			{
				if (current.HasPreviewGUI())
				{
					list.Add(current);
				}
			}
			return list.ToArray<IPreviewable>();
		}

		public UnityEngine.Object GetInspectedObject()
		{
			if (this.m_Tracker == null)
			{
				return null;
			}
			Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(this.m_Tracker.activeEditors);
			if (firstNonImportInspectorEditor == null)
			{
				return null;
			}
			return firstNonImportInspectorEditor.target;
		}

		private Editor GetFirstNonImportInspectorEditor(Editor[] editors)
		{
			for (int i = 0; i < editors.Length; i++)
			{
				Editor editor = editors[i];
				if (!(editor.target is AssetImporter))
				{
					return editor;
				}
			}
			return null;
		}

		private void MoveFocusOnKeyPress()
		{
			KeyCode keyCode = Event.current.keyCode;
			if (Event.current.type != EventType.KeyDown || (keyCode != KeyCode.DownArrow && keyCode != KeyCode.UpArrow && keyCode != KeyCode.Tab))
			{
				return;
			}
			if (keyCode != KeyCode.Tab)
			{
				EditorGUIUtility.MoveFocusAndScroll(keyCode == KeyCode.DownArrow);
			}
			else
			{
				EditorGUIUtility.ScrollForTabbing(!Event.current.shift);
			}
			Event.current.Use();
		}

		private void ResetKeyboardControl()
		{
			if (this.m_ResetKeyboardControl)
			{
				GUIUtility.keyboardControl = 0;
				this.m_ResetKeyboardControl = false;
			}
		}

		private void CheckDragAndDrop(Editor[] editors)
		{
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true)
			});
			if (rect.Contains(Event.current.mousePosition))
			{
				Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(editors);
				if (firstNonImportInspectorEditor != null)
				{
					InspectorWindow.DoInspectorDragAndDrop(rect, firstNonImportInspectorEditor.targets);
				}
				if (Event.current.type == EventType.MouseDown)
				{
					GUIUtility.keyboardControl = 0;
					Event.current.Use();
				}
			}
		}

		private UnityEngine.Object[] GetInspectedAssets()
		{
			if (this.m_Tracker != null)
			{
				Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(this.m_Tracker.activeEditors);
				if (firstNonImportInspectorEditor != null && firstNonImportInspectorEditor != null && firstNonImportInspectorEditor.targets.Length == 1)
				{
					string assetPath = AssetDatabase.GetAssetPath(firstNonImportInspectorEditor.target);
					bool flag = assetPath.ToLower().StartsWith("assets") && !Directory.Exists(assetPath);
					if (flag)
					{
						return firstNonImportInspectorEditor.targets;
					}
				}
			}
			return Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
		}

		private void DrawPreviewAndLabels()
		{
			if (this.m_PreviewWindow && Event.current.type == EventType.Repaint)
			{
				this.m_PreviewWindow.Repaint();
			}
			IPreviewable[] editorsWithPreviews = this.GetEditorsWithPreviews(this.m_Tracker.activeEditors);
			IPreviewable editorThatControlsPreview = this.GetEditorThatControlsPreview(editorsWithPreviews);
			bool flag = editorThatControlsPreview != null && editorThatControlsPreview.HasPreviewGUI() && this.m_PreviewWindow == null;
			UnityEngine.Object[] inspectedAssets = this.GetInspectedAssets();
			bool flag2 = inspectedAssets.Length > 0;
			bool flag3 = inspectedAssets.Any((UnityEngine.Object a) => !(a is MonoScript) && AssetDatabase.IsMainAsset(a));
			if (!flag && !flag2)
			{
				return;
			}
			Event current = Event.current;
			Rect position = EditorGUILayout.BeginHorizontal(GUIContent.none, InspectorWindow.styles.preToolbar, new GUILayoutOption[]
			{
				GUILayout.Height(17f)
			});
			Rect position2 = default(Rect);
			GUILayout.FlexibleSpace();
			Rect lastRect = GUILayoutUtility.GetLastRect();
			GUIContent content;
			if (flag)
			{
				GUIContent previewTitle = editorThatControlsPreview.GetPreviewTitle();
				content = (previewTitle ?? InspectorWindow.styles.preTitle);
			}
			else
			{
				content = InspectorWindow.styles.labelTitle;
			}
			position2.x = lastRect.x + 3f;
			position2.y = lastRect.y + (17f - InspectorWindow.s_Styles.dragHandle.fixedHeight) / 2f + 1f;
			position2.width = lastRect.width - 6f;
			position2.height = InspectorWindow.s_Styles.dragHandle.fixedHeight;
			if (editorsWithPreviews.Length > 1)
			{
				Vector2 vector = InspectorWindow.styles.preDropDown.CalcSize(content);
				Rect position3 = new Rect(lastRect.x, lastRect.y, vector.x, vector.y);
				lastRect.xMin += vector.x;
				position2.xMin += vector.x;
				GUIContent[] array = new GUIContent[editorsWithPreviews.Length];
				int selected = -1;
				for (int i = 0; i < editorsWithPreviews.Length; i++)
				{
					IPreviewable previewable = editorsWithPreviews[i];
					GUIContent gUIContent = previewable.GetPreviewTitle() ?? InspectorWindow.styles.preTitle;
					string text;
					if (gUIContent == InspectorWindow.styles.preTitle)
					{
						string str = ObjectNames.GetTypeName(previewable.target);
						if (previewable.target is MonoBehaviour)
						{
							str = MonoScript.FromMonoBehaviour(previewable.target as MonoBehaviour).GetClass().Name;
						}
						text = gUIContent.text + " - " + str;
					}
					else
					{
						text = gUIContent.text;
					}
					array[i] = new GUIContent(text);
					if (editorsWithPreviews[i] == editorThatControlsPreview)
					{
						selected = i;
					}
				}
				if (GUI.Button(position3, content, InspectorWindow.styles.preDropDown))
				{
					EditorUtility.DisplayCustomMenu(position3, array, selected, new EditorUtility.SelectMenuItemFunction(this.OnPreviewSelected), editorsWithPreviews);
				}
			}
			else
			{
				float a2 = position2.xMax - lastRect.xMin - 3f - 20f;
				float width = Mathf.Min(a2, InspectorWindow.styles.preToolbar2.CalcSize(content).x);
				Rect position4 = new Rect(lastRect.x, lastRect.y, width, lastRect.height);
				position2.xMin = position4.xMax + 3f;
				GUI.Label(position4, content, InspectorWindow.styles.preToolbar2);
			}
			if (flag && Event.current.type == EventType.Repaint)
			{
				InspectorWindow.s_Styles.dragHandle.Draw(position2, GUIContent.none, false, false, false, false);
			}
			if (flag && this.m_PreviewResizer.GetExpandedBeforeDragging())
			{
				editorThatControlsPreview.OnPreviewSettings();
			}
			EditorGUILayout.EndHorizontal();
			if (current.type == EventType.MouseUp && current.button == 1 && position.Contains(current.mousePosition) && this.m_PreviewWindow == null)
			{
				this.DetachPreview();
			}
			float height;
			if (flag)
			{
				Rect position5 = base.position;
				if (EditorSettings.externalVersionControl != ExternalVersionControl.Disabled && EditorSettings.externalVersionControl != ExternalVersionControl.AutoDetect && EditorSettings.externalVersionControl != ExternalVersionControl.Generic)
				{
					position5.height -= 17f;
				}
				height = this.m_PreviewResizer.ResizeHandle(position5, 100f, 100f, 17f, lastRect);
			}
			else
			{
				if (GUI.Button(position, GUIContent.none, GUIStyle.none))
				{
					this.m_PreviewResizer.ToggleExpanded();
				}
				height = 0f;
			}
			if (!this.m_PreviewResizer.GetExpanded())
			{
				return;
			}
			GUILayout.BeginVertical(InspectorWindow.styles.preBackground, new GUILayoutOption[]
			{
				GUILayout.Height(height)
			});
			if (flag)
			{
				editorThatControlsPreview.DrawPreview(GUILayoutUtility.GetRect(0f, 10240f, 64f, 10240f));
			}
			if (flag2)
			{
				using (new EditorGUI.DisabledScope(inspectedAssets.Any((UnityEngine.Object a) => EditorUtility.IsPersistent(a) && !Editor.IsAppropriateFileOpenForEdit(a))))
				{
					this.m_LabelGUI.OnLabelGUI(inspectedAssets);
				}
			}
			if (flag3)
			{
				this.m_AssetBundleNameGUI.OnAssetBundleNameGUI(inspectedAssets);
			}
			GUILayout.EndVertical();
		}

		protected UnityEngine.Object[] GetTargetsForPreview(IPreviewable previewEditor)
		{
			Editor editor = null;
			Editor[] activeEditors = this.m_Tracker.activeEditors;
			for (int i = 0; i < activeEditors.Length; i++)
			{
				Editor editor2 = activeEditors[i];
				if (editor2.target.GetType() == previewEditor.target.GetType())
				{
					editor = editor2;
					break;
				}
			}
			return editor.targets;
		}

		private void OnPreviewSelected(object userData, string[] options, int selected)
		{
			IPreviewable[] array = userData as IPreviewable[];
			this.m_SelectedPreview = array[selected];
		}

		private void DetachPreview()
		{
			Event.current.Use();
			this.m_PreviewWindow = (ScriptableObject.CreateInstance(typeof(PreviewWindow)) as PreviewWindow);
			this.m_PreviewWindow.SetParentInspector(this);
			this.m_PreviewWindow.Show();
			base.Repaint();
			GUIUtility.ExitGUI();
		}

		protected virtual void DrawVCSSticky(float offset)
		{
			string empty = string.Empty;
			Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(this.m_Tracker.activeEditors);
			if (!EditorPrefs.GetBool("vcssticky") && !Editor.IsAppropriateFileOpenForEdit(firstNonImportInspectorEditor.target, out empty))
			{
				Rect position = new Rect(10f, base.position.height - 94f, base.position.width - 20f, 80f);
				position.y -= offset;
				if (Event.current.type == EventType.Repaint)
				{
					InspectorWindow.styles.stickyNote.Draw(position, false, false, false, false);
					Rect position2 = new Rect(position.x, position.y + position.height / 2f - 32f, 64f, 64f);
					if (EditorSettings.externalVersionControl == "Perforce")
					{
						InspectorWindow.styles.stickyNotePerforce.Draw(position2, false, false, false, false);
					}
					Rect position3 = new Rect(position.x + position2.width, position.y, position.width - position2.width, position.height);
					GUI.Label(position3, new GUIContent("<b>Under Version Control</b>\nCheck out this asset in order to make changes."), InspectorWindow.styles.stickyNoteLabel);
					Rect position4 = new Rect(position.x + position.width / 2f, position.y + 80f, 19f, 14f);
					InspectorWindow.styles.stickyNoteArrow.Draw(position4, false, false, false, false);
				}
			}
		}

		private void DrawVCSShortInfo()
		{
			if (EditorSettings.externalVersionControl == ExternalVersionControl.AssetServer)
			{
				EditorGUILayout.BeginHorizontal(GUIContent.none, InspectorWindow.styles.preToolbar, new GUILayoutOption[]
				{
					GUILayout.Height(17f)
				});
				Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(this.m_Tracker.activeEditors);
				UnityEngine.Object target = (!(firstNonImportInspectorEditor == null)) ? firstNonImportInspectorEditor.target : null;
				int controlID = GUIUtility.GetControlID(FocusType.Passive);
				GUILayout.FlexibleSpace();
				Rect lastRect = GUILayoutUtility.GetLastRect();
				EditorGUILayout.EndHorizontal();
				AssetInspector.Get().OnAssetStatusGUI(lastRect, controlID, target, InspectorWindow.styles.preToolbar2);
			}
			if (Provider.isActive && EditorSettings.externalVersionControl != ExternalVersionControl.Disabled && EditorSettings.externalVersionControl != ExternalVersionControl.AutoDetect && EditorSettings.externalVersionControl != ExternalVersionControl.Generic)
			{
				Editor firstNonImportInspectorEditor2 = this.GetFirstNonImportInspectorEditor(this.m_Tracker.activeEditors);
				string assetPath = AssetDatabase.GetAssetPath(firstNonImportInspectorEditor2.target);
				Asset assetByPath = Provider.GetAssetByPath(assetPath);
				if (assetByPath == null || (!assetByPath.path.StartsWith("Assets") && !assetByPath.path.StartsWith("ProjectSettings")))
				{
					return;
				}
				Asset assetByPath2 = Provider.GetAssetByPath(assetPath.Trim(new char[]
				{
					'/'
				}) + ".meta");
				string text = assetByPath.StateToString();
				string text2 = (assetByPath2 != null) ? assetByPath2.StateToString() : string.Empty;
				bool flag = assetByPath2 != null && (assetByPath2.state & ~Asset.States.MetaFile) != assetByPath.state;
				bool flag2 = text != string.Empty;
				float height = (!flag || !flag2) ? 17f : 34f;
				GUILayout.Label(GUIContent.none, InspectorWindow.styles.preToolbar, new GUILayoutOption[]
				{
					GUILayout.Height(height)
				});
				Rect lastRect2 = GUILayoutUtility.GetLastRect();
				bool flag3 = Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint;
				if (flag2 && flag3)
				{
					Texture2D icon = AssetDatabase.GetCachedIcon(assetPath) as Texture2D;
					if (flag)
					{
						Rect rect = lastRect2;
						rect.height = 17f;
						this.DrawVCSShortInfoAsset(assetByPath, this.BuildTooltip(assetByPath, null), rect, icon, text);
						Texture2D iconForFile = InternalEditorUtility.GetIconForFile(assetByPath2.path);
						rect.y += 17f;
						this.DrawVCSShortInfoAsset(assetByPath2, this.BuildTooltip(null, assetByPath2), rect, iconForFile, text2);
					}
					else
					{
						this.DrawVCSShortInfoAsset(assetByPath, this.BuildTooltip(assetByPath, assetByPath2), lastRect2, icon, text);
					}
				}
				else if (text2 != string.Empty && flag3)
				{
					Texture2D iconForFile2 = InternalEditorUtility.GetIconForFile(assetByPath2.path);
					this.DrawVCSShortInfoAsset(assetByPath2, this.BuildTooltip(assetByPath, assetByPath2), lastRect2, iconForFile2, text2);
				}
				string empty = string.Empty;
				if (!Editor.IsAppropriateFileOpenForEdit(firstNonImportInspectorEditor2.target, out empty))
				{
					float num = 66f;
					Rect position = new Rect(lastRect2.x + lastRect2.width - num, lastRect2.y, num, lastRect2.height);
					if (GUI.Button(position, "Check out", InspectorWindow.styles.lockedHeaderButton))
					{
						EditorPrefs.SetBool("vcssticky", true);
						Task task = Provider.Checkout(firstNonImportInspectorEditor2.targets, CheckoutMode.Both);
						task.Wait();
						base.Repaint();
					}
					this.DrawVCSSticky(lastRect2.height / 2f);
				}
			}
		}

		protected string BuildTooltip(Asset asset, Asset metaAsset)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (asset != null)
			{
				stringBuilder.AppendLine("Asset:");
				stringBuilder.AppendLine(asset.AllStateToString());
			}
			if (metaAsset != null)
			{
				stringBuilder.AppendLine("Meta file:");
				stringBuilder.AppendLine(metaAsset.AllStateToString());
			}
			return stringBuilder.ToString();
		}

		protected void DrawVCSShortInfoAsset(Asset asset, string tooltip, Rect rect, Texture2D icon, string currentState)
		{
			Rect rect2 = new Rect(rect.x, rect.y, 28f, 16f);
			Rect position = rect2;
			position.x += 6f;
			position.width = 16f;
			if (icon != null)
			{
				GUI.DrawTexture(position, icon);
			}
			Overlay.DrawOverlay(asset, rect2);
			Rect position2 = new Rect(rect.x + 26f, rect.y, rect.width - 31f, rect.height);
			GUIContent gUIContent = GUIContent.Temp(currentState);
			gUIContent.tooltip = tooltip;
			EditorGUI.LabelField(position2, gUIContent, InspectorWindow.styles.preToolbar2);
		}

		protected void AssignAssetEditor(Editor[] editors)
		{
			if (editors.Length > 1 && editors[0] is AssetImporterInspector)
			{
				(editors[0] as AssetImporterInspector).assetEditor = editors[1];
			}
		}

		private void DrawEditors(Editor[] editors)
		{
			if (editors.Length == 0)
			{
				return;
			}
			UnityEngine.Object inspectedObject = this.GetInspectedObject();
			string empty = string.Empty;
			DockArea dockArea = this.m_Parent as DockArea;
			if (dockArea != null)
			{
				dockArea.tabStyle = "dragtabbright";
			}
			GUILayout.Space(0f);
			if (inspectedObject is Material)
			{
				int num = 0;
				while (num <= 1 && num < editors.Length)
				{
					MaterialEditor materialEditor = editors[num] as MaterialEditor;
					if (materialEditor != null)
					{
						materialEditor.forceVisible = true;
						break;
					}
					num++;
				}
			}
			bool rebuildOptimizedGUIBlock = false;
			if (Event.current.type == EventType.Repaint)
			{
				if (inspectedObject != null && this.m_IsOpenForEdit != Editor.IsAppropriateFileOpenForEdit(inspectedObject, out empty))
				{
					this.m_IsOpenForEdit = !this.m_IsOpenForEdit;
					rebuildOptimizedGUIBlock = true;
				}
				if (this.m_InvalidateGUIBlockCache)
				{
					rebuildOptimizedGUIBlock = true;
					this.m_InvalidateGUIBlockCache = false;
				}
			}
			else if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "EyeDropperUpdate")
			{
				rebuildOptimizedGUIBlock = true;
			}
			Editor.m_AllowMultiObjectAccess = true;
			bool flag = false;
			Rect position = default(Rect);
			for (int i = 0; i < editors.Length; i++)
			{
				if (this.ShouldCullEditor(editors, i))
				{
					if (Event.current.type == EventType.Repaint)
					{
						editors[i].isInspectorDirty = false;
					}
				}
				else
				{
					bool textFieldInput = GUIUtility.textFieldInput;
					this.DrawEditor(editors[i], i, rebuildOptimizedGUIBlock, ref flag, ref position);
					if (Event.current.type == EventType.Repaint && !textFieldInput && GUIUtility.textFieldInput)
					{
						InspectorWindow.FlushOptimizedGUIBlock(editors[i]);
					}
				}
			}
			EditorGUIUtility.ResetGUIState();
			if (position.height > 0f)
			{
				GUI.BeginGroup(position);
				GUI.Label(new Rect(0f, 0f, position.width, position.height), "Imported Object", "OL Title");
				GUI.EndGroup();
			}
		}

		internal override void OnResized()
		{
			this.m_InvalidateGUIBlockCache = true;
		}

		private void DrawEditor(Editor editor, int editorIndex, bool rebuildOptimizedGUIBlock, ref bool showImportedObjectBarNext, ref Rect importedObjectBarRect)
		{
			if (editor == null)
			{
				return;
			}
			UnityEngine.Object target = editor.target;
			GUIUtility.GetControlID(target.GetInstanceID(), FocusType.Passive);
			EditorGUIUtility.ResetGUIState();
			GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			int visible = this.m_Tracker.GetVisible(editorIndex);
			bool flag;
			if (visible == -1)
			{
				flag = InternalEditorUtility.GetIsInspectorExpanded(target);
				this.m_Tracker.SetVisible(editorIndex, (!flag) ? 0 : 1);
			}
			else
			{
				flag = (visible == 1);
			}
			rebuildOptimizedGUIBlock |= editor.isInspectorDirty;
			if (Event.current.type == EventType.Repaint)
			{
				editor.isInspectorDirty = false;
			}
			ScriptAttributeUtility.propertyHandlerCache = editor.propertyHandlerCache;
			bool flag2 = AssetDatabase.IsMainAsset(target) || AssetDatabase.IsSubAsset(target) || editorIndex == 0 || target is Material;
			if (flag2)
			{
				string empty = string.Empty;
				bool flag3 = editor.IsOpenForEdit(out empty);
				if (showImportedObjectBarNext)
				{
					showImportedObjectBarNext = false;
					GUILayout.Space(15f);
					importedObjectBarRect = GUILayoutUtility.GetRect(16f, 16f);
					importedObjectBarRect.height = 17f;
				}
				flag = true;
				using (new EditorGUI.DisabledScope(!flag3))
				{
					editor.DrawHeader();
				}
			}
			if (editor.target is AssetImporter)
			{
				showImportedObjectBarNext = true;
			}
			bool flag4 = false;
			if (editor is GenericInspector && CustomEditorAttributes.FindCustomEditorType(target, false) != null)
			{
				if (this.m_InspectorMode != InspectorMode.DebugInternal)
				{
					if (this.m_InspectorMode == InspectorMode.Normal)
					{
						flag4 = true;
					}
					else if (target is AssetImporter)
					{
						flag4 = true;
					}
				}
			}
			if (!flag2)
			{
				using (new EditorGUI.DisabledScope(!editor.IsEnabled()))
				{
					bool flag5 = EditorGUILayout.InspectorTitlebar(flag, editor.targets, editor.CanBeExpandedViaAFoldout());
					if (flag != flag5)
					{
						this.m_Tracker.SetVisible(editorIndex, (!flag5) ? 0 : 1);
						InternalEditorUtility.SetIsInspectorExpanded(target, flag5);
						if (flag5)
						{
							this.m_LastInteractedEditor = editor;
						}
						else if (this.m_LastInteractedEditor == editor)
						{
							this.m_LastInteractedEditor = null;
						}
					}
				}
			}
			if (flag4 && flag)
			{
				GUILayout.Label("Multi-object editing not supported.", EditorStyles.helpBox, new GUILayoutOption[0]);
				return;
			}
			this.DisplayDeprecationMessageIfNecessary(editor);
			EditorGUIUtility.ResetGUIState();
			Rect rect = default(Rect);
			using (new EditorGUI.DisabledScope(!editor.IsEnabled()))
			{
				GenericInspector genericInspector = editor as GenericInspector;
				if (genericInspector)
				{
					genericInspector.m_InspectorMode = this.m_InspectorMode;
				}
				EditorGUIUtility.hierarchyMode = true;
				EditorGUIUtility.wideMode = (base.position.width > 330f);
				ScriptAttributeUtility.propertyHandlerCache = editor.propertyHandlerCache;
				OptimizedGUIBlock optimizedGUIBlock;
				float num;
				if (editor.GetOptimizedGUIBlock(rebuildOptimizedGUIBlock, flag, out optimizedGUIBlock, out num))
				{
					rect = GUILayoutUtility.GetRect(0f, (!flag) ? 0f : num);
					this.HandleLastInteractedEditor(rect, editor);
					if (Event.current.type == EventType.Layout)
					{
						return;
					}
					if (optimizedGUIBlock.Begin(rebuildOptimizedGUIBlock, rect) && flag)
					{
						GUI.changed = false;
						editor.OnOptimizedInspectorGUI(rect);
					}
					optimizedGUIBlock.End();
				}
				else
				{
					if (flag)
					{
						GUIStyle style = (!editor.UseDefaultMargins()) ? GUIStyle.none : EditorStyles.inspectorDefaultMargins;
						rect = EditorGUILayout.BeginVertical(style, new GUILayoutOption[0]);
						this.HandleLastInteractedEditor(rect, editor);
						GUI.changed = false;
						try
						{
							editor.OnInspectorGUI();
						}
						catch (Exception exception)
						{
							if (GUIUtility.ShouldRethrowException(exception))
							{
								throw;
							}
							Debug.LogException(exception);
						}
						EditorGUILayout.EndVertical();
					}
					if (Event.current.type == EventType.Used)
					{
						return;
					}
				}
			}
			if (GUILayoutUtility.current.topLevel != topLevel)
			{
				if (!GUILayoutUtility.current.layoutGroups.Contains(topLevel))
				{
					Debug.LogError("Expected top level layout group missing! Too many GUILayout.EndScrollView/EndVertical/EndHorizontal?");
					GUIUtility.ExitGUI();
				}
				else
				{
					Debug.LogWarning("Unexpected top level layout group! Missing GUILayout.EndScrollView/EndVertical/EndHorizontal?");
					while (GUILayoutUtility.current.topLevel != topLevel)
					{
						GUILayoutUtility.EndLayoutGroup();
					}
				}
			}
			this.HandleComponentScreenshot(rect, editor);
		}

		private void DisplayDeprecationMessageIfNecessary(Editor editor)
		{
			if (!editor || !editor.target)
			{
				return;
			}
			ObsoleteAttribute obsoleteAttribute = (ObsoleteAttribute)Attribute.GetCustomAttribute(editor.target.GetType(), typeof(ObsoleteAttribute));
			if (obsoleteAttribute == null)
			{
				return;
			}
			string message = (!string.IsNullOrEmpty(obsoleteAttribute.Message)) ? obsoleteAttribute.Message : "This component has been marked as obsolete.";
			EditorGUILayout.HelpBox(message, (!obsoleteAttribute.IsError) ? MessageType.Warning : MessageType.Error);
		}

		private void HandleComponentScreenshot(Rect contentRect, Editor editor)
		{
			if (ScreenShots.s_TakeComponentScreenshot)
			{
				contentRect.yMin -= 16f;
				if (contentRect.Contains(Event.current.mousePosition))
				{
					Rect contentRect2 = GUIClip.Unclip(contentRect);
					contentRect2.position += this.m_Parent.screenPosition.position;
					ScreenShots.ScreenShotComponent(contentRect2, editor.target);
				}
			}
		}

		private bool ShouldCullEditor(Editor[] editors, int editorIndex)
		{
			if (editors[editorIndex].hideInspector)
			{
				return true;
			}
			UnityEngine.Object target = editors[editorIndex].target;
			if (target is SubstanceImporter || target is ParticleSystemRenderer)
			{
				return true;
			}
			if (target.GetType() == typeof(AssetImporter))
			{
				return true;
			}
			if (this.m_InspectorMode == InspectorMode.Normal && editorIndex != 0)
			{
				AssetImporterInspector assetImporterInspector = editors[0] as AssetImporterInspector;
				if (assetImporterInspector != null && !assetImporterInspector.showImportedObject)
				{
					return true;
				}
			}
			return false;
		}

		private void DrawSelectionPickerList()
		{
			if (this.m_TypeSelectionList == null)
			{
				this.m_TypeSelectionList = new TypeSelectionList(Selection.objects);
			}
			DockArea dockArea = this.m_Parent as DockArea;
			if (dockArea != null)
			{
				dockArea.tabStyle = "dragtabbright";
			}
			GUILayout.Space(0f);
			Editor.DrawHeaderGUI(null, Selection.objects.Length + " Objects");
			GUILayout.Label("Narrow the Selection:", EditorStyles.label, new GUILayoutOption[0]);
			GUILayout.Space(4f);
			Vector2 iconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
			foreach (TypeSelection current in this.m_TypeSelectionList.typeSelections)
			{
				Rect rect = GUILayoutUtility.GetRect(16f, 16f, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				if (GUI.Button(rect, current.label, InspectorWindow.styles.typeSelection))
				{
					Selection.objects = current.objects;
					Event.current.Use();
				}
				if (GUIUtility.hotControl == 0)
				{
					EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
				}
				GUILayout.Space(4f);
			}
			EditorGUIUtility.SetIconSize(iconSize);
		}

		private void HandleLastInteractedEditor(Rect componentRect, Editor editor)
		{
			if (editor != this.m_LastInteractedEditor && Event.current.type == EventType.MouseDown && componentRect.Contains(Event.current.mousePosition))
			{
				this.m_LastInteractedEditor = editor;
				base.Repaint();
			}
		}

		private void AddComponentButton(Editor[] editors)
		{
			Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(editors);
			if (firstNonImportInspectorEditor != null && firstNonImportInspectorEditor.target != null && firstNonImportInspectorEditor.target is GameObject && firstNonImportInspectorEditor.IsEnabled())
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUIContent addComponentLabel = InspectorWindow.s_Styles.addComponentLabel;
				Rect rect = GUILayoutUtility.GetRect(addComponentLabel, InspectorWindow.styles.addComponentButtonStyle, null);
				rect.y += 10f;
				rect.x += (rect.width - 230f) / 2f;
				rect.width = 230f;
				if (Event.current.type == EventType.Repaint)
				{
					this.DrawSplitLine(rect.y - 11f);
				}
				Event current = Event.current;
				bool flag = false;
				EventType type = current.type;
				if (type == EventType.ExecuteCommand)
				{
					string commandName = current.commandName;
					if (commandName == "OpenAddComponentDropdown")
					{
						flag = true;
						current.Use();
					}
				}
				if (EditorGUI.ButtonMouseDown(rect, addComponentLabel, FocusType.Passive, InspectorWindow.styles.addComponentButtonStyle) || flag)
				{
					if (AddComponentWindow.Show(rect, (from o in firstNonImportInspectorEditor.targets
					select (GameObject)o).ToArray<GameObject>()))
					{
						GUIUtility.ExitGUI();
					}
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		private bool ReadyToRepaint()
		{
			if (AnimationMode.InAnimationPlaybackMode())
			{
				long num = DateTime.Now.Ticks / 10000L;
				if (num - this.s_LastUpdateWhilePlayingAnimation < 150L)
				{
					return false;
				}
				this.s_LastUpdateWhilePlayingAnimation = num;
			}
			return true;
		}

		private void DrawSplitLine(float y)
		{
			Rect position = new Rect(0f, y, this.m_Pos.width + 1f, 1f);
			Rect texCoords = new Rect(0f, 1f, 1f, 1f - 1f / (float)EditorStyles.inspectorTitlebar.normal.background.height);
			GUI.DrawTextureWithTexCoords(position, EditorStyles.inspectorTitlebar.normal.background, texCoords);
		}

		internal static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(InspectorWindow));
		}

		private static void FlushOptimizedGUI()
		{
			InspectorWindow.s_AllOptimizedGUIBlocksNeedsRebuild = true;
		}

		private static void FlushAllOptimizedGUIBlocksIfNeeded()
		{
			if (!InspectorWindow.s_AllOptimizedGUIBlocksNeedsRebuild)
			{
				return;
			}
			InspectorWindow.s_AllOptimizedGUIBlocksNeedsRebuild = false;
			foreach (InspectorWindow current in InspectorWindow.m_AllInspectors)
			{
				if (current.m_Tracker != null)
				{
					Editor[] activeEditors = current.m_Tracker.activeEditors;
					for (int i = 0; i < activeEditors.Length; i++)
					{
						Editor editor = activeEditors[i];
						InspectorWindow.FlushOptimizedGUIBlock(editor);
					}
				}
			}
		}

		private static void FlushOptimizedGUIBlock(Editor editor)
		{
			if (editor == null)
			{
				return;
			}
			OptimizedGUIBlock optimizedGUIBlock;
			float num;
			if (editor.GetOptimizedGUIBlock(false, false, out optimizedGUIBlock, out num))
			{
				optimizedGUIBlock.valid = false;
			}
		}

		private void Update()
		{
			if (this.m_Tracker == null)
			{
				return;
			}
			Editor[] activeEditors = this.m_Tracker.activeEditors;
			if (activeEditors == null)
			{
				return;
			}
			bool flag = false;
			Editor[] array = activeEditors;
			for (int i = 0; i < array.Length; i++)
			{
				Editor editor = array[i];
				if (editor.RequiresConstantRepaint() && !editor.hideInspector)
				{
					flag = true;
				}
			}
			if (flag && this.m_lastRenderedTime + 0.032999999821186066 < EditorApplication.timeSinceStartup)
			{
				this.m_lastRenderedTime = EditorApplication.timeSinceStartup;
				base.Repaint();
			}
		}
	}
}
