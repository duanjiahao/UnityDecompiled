using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Audio Mixer", icon = "Audio Mixer")]
	internal class AudioMixerWindow : EditorWindow, IHasCustomMenu
	{
		private enum SectionType
		{
			MixerTree,
			GroupTree,
			ViewList,
			SnapshotList
		}

		public enum LayoutMode
		{
			Horizontal,
			Vertical
		}

		[Serializable]
		private class Layout
		{
			[SerializeField]
			public SplitterState m_VerticalSplitter;

			[SerializeField]
			public SplitterState m_HorizontalSplitter;
		}

		private class GUIContents
		{
			public GUIContent rms;

			public GUIContent editSnapShots;

			public GUIContent infoText;

			public GUIContent selectAudioMixer;

			public GUIContent output;

			public GUIStyle toolbarObjectField = new GUIStyle("ShurikenObjectField");

			public GUIStyle toolbarLabel = new GUIStyle(EditorStyles.miniLabel);

			public GUIStyle mixerHeader = new GUIStyle(EditorStyles.largeLabel);

			public GUIContents()
			{
				this.rms = new GUIContent("RMS", "Switches between RMS (Root Mean Square) metering and peak metering. RMS is closer to the energy level and perceived loudness of the sound (hence lower than the peak meter), while peak-metering is useful for monitoring spikes in the signal that can cause clipping.");
				this.editSnapShots = new GUIContent("Edit in Play Mode", EditorGUIUtility.IconContent("Animation.Record", "|Are scene and inspector changes recorded into the animation curves?").image, "Edit in playmode and your changes are automatically saved. Note when editting is disabled then live values are shown.");
				this.infoText = new GUIContent("Create an AudioMixer asset from the Project Browser to get started");
				this.selectAudioMixer = new GUIContent("", "Select an Audio Mixer");
				this.output = new GUIContent("Output", "Select an Audio Mixer Group from another Audio Mixer to output to. If 'None' is selected then output is routed directly to the Audio Listener.");
				this.toolbarLabel.alignment = TextAnchor.MiddleLeft;
				this.toolbarObjectField.normal.textColor = this.toolbarLabel.normal.textColor;
				this.mixerHeader.fontStyle = FontStyle.Bold;
				this.mixerHeader.fontSize = 17;
				this.mixerHeader.margin = new RectOffset();
				this.mixerHeader.padding = new RectOffset();
				this.mixerHeader.alignment = TextAnchor.MiddleLeft;
				if (!EditorGUIUtility.isProSkin)
				{
					this.mixerHeader.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1f);
				}
				else
				{
					this.mixerHeader.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
				}
			}
		}

		private class AudioMixerPostprocessor : AssetPostprocessor
		{
			private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
			{
				if (AudioMixerWindow.s_Instance != null)
				{
					bool flag = importedAssets.Any((string val) => val.EndsWith(".mixer"));
					flag |= deletedAssets.Any((string val) => val.EndsWith(".mixer"));
					flag |= movedAssets.Any((string val) => val.EndsWith(".mixer"));
					flag |= movedFromPath.Any((string val) => val.EndsWith(".mixer"));
					if (flag)
					{
						AudioMixerWindow.s_Instance.UpdateAfterAssetChange();
					}
				}
			}
		}

		private static AudioMixerWindow s_Instance;

		private static string kAudioMixerUseRMSMetering = "AudioMixerUseRMSMetering";

		[NonSerialized]
		private bool m_Initialized;

		private AudioMixerController m_Controller;

		private List<AudioMixerController> m_AllControllers;

		private AudioMixerChannelStripView.State m_ChannelStripViewState;

		private AudioMixerChannelStripView m_ChannelStripView;

		private TreeViewState m_AudioGroupTreeState;

		private AudioMixerGroupTreeView m_GroupTree;

		[SerializeField]
		private TreeViewState m_MixersTreeState;

		private AudioMixersTreeView m_MixersTree;

		private ReorderableListWithRenameAndScrollView.State m_ViewsState;

		private AudioMixerGroupViewList m_GroupViews;

		private ReorderableListWithRenameAndScrollView.State m_SnapshotState;

		private AudioMixerSnapshotListView m_SnapshotListView;

		[SerializeField]
		private AudioMixerWindow.Layout m_LayoutStripsOnTop;

		[SerializeField]
		private AudioMixerWindow.Layout m_LayoutStripsOnRight;

		[SerializeField]
		private AudioMixerWindow.SectionType[] m_SectionOrder = new AudioMixerWindow.SectionType[]
		{
			AudioMixerWindow.SectionType.MixerTree,
			AudioMixerWindow.SectionType.SnapshotList,
			AudioMixerWindow.SectionType.GroupTree,
			AudioMixerWindow.SectionType.ViewList
		};

		[SerializeField]
		private AudioMixerWindow.LayoutMode m_LayoutMode = AudioMixerWindow.LayoutMode.Vertical;

		[SerializeField]
		private bool m_SortGroupsAlphabetically = false;

		[SerializeField]
		private bool m_ShowReferencedBuses = true;

		[SerializeField]
		private bool m_ShowBusConnections = false;

		[SerializeField]
		private bool m_ShowBusConnectionsOfSelection = false;

		private Vector2 m_SectionsScrollPosition = Vector2.zero;

		private int m_RepaintCounter = 2;

		private Vector2 m_LastSize;

		private bool m_GroupsRenderedAboveSections = true;

		[NonSerialized]
		private bool m_ShowDeveloperOverlays = false;

		private readonly TickTimerHelper m_Ticker = new TickTimerHelper(0.05);

		private static AudioMixerWindow.GUIContents s_GuiContents;

		public AudioMixerController controller
		{
			get
			{
				return this.m_Controller;
			}
		}

		private AudioMixerWindow.LayoutMode layoutMode
		{
			get
			{
				return this.m_LayoutMode;
			}
			set
			{
				this.m_LayoutMode = value;
				this.m_RepaintCounter = 2;
			}
		}

		private void UpdateAfterAssetChange()
		{
			if (!(this.m_Controller == null))
			{
				this.m_Controller.SanitizeGroupViews();
				this.m_Controller.OnUnitySelectionChanged();
				if (this.m_GroupTree != null)
				{
					this.m_GroupTree.ReloadTreeData();
				}
				if (this.m_GroupViews != null)
				{
					this.m_GroupViews.RecreateListControl();
				}
				if (this.m_SnapshotListView != null)
				{
					this.m_SnapshotListView.LoadFromBackend();
				}
				if (this.m_MixersTree != null)
				{
					this.m_MixersTree.ReloadTree();
				}
				AudioMixerUtility.RepaintAudioMixerAndInspectors();
			}
		}

		public static void Create()
		{
			AudioMixerWindow window = EditorWindow.GetWindow<AudioMixerWindow>(new Type[]
			{
				typeof(ProjectBrowser)
			});
			if (window.m_Pos.width < 400f)
			{
				window.m_Pos = new Rect(window.m_Pos.x, window.m_Pos.y, 800f, 450f);
			}
		}

		public static void RepaintAudioMixerWindow()
		{
			if (AudioMixerWindow.s_Instance != null)
			{
				AudioMixerWindow.s_Instance.Repaint();
			}
		}

		private void Init()
		{
			if (!this.m_Initialized)
			{
				if (this.m_LayoutStripsOnTop == null)
				{
					this.m_LayoutStripsOnTop = new AudioMixerWindow.Layout();
				}
				if (this.m_LayoutStripsOnTop.m_VerticalSplitter == null || this.m_LayoutStripsOnTop.m_VerticalSplitter.realSizes.Length != 2)
				{
					this.m_LayoutStripsOnTop.m_VerticalSplitter = new SplitterState(new int[]
					{
						65,
						35
					}, new int[]
					{
						85,
						105
					}, null);
				}
				if (this.m_LayoutStripsOnTop.m_HorizontalSplitter == null || this.m_LayoutStripsOnTop.m_HorizontalSplitter.realSizes.Length != 4)
				{
					this.m_LayoutStripsOnTop.m_HorizontalSplitter = new SplitterState(new int[]
					{
						60,
						60,
						60,
						60
					}, new int[]
					{
						85,
						85,
						85,
						85
					}, null);
				}
				if (this.m_LayoutStripsOnRight == null)
				{
					this.m_LayoutStripsOnRight = new AudioMixerWindow.Layout();
				}
				if (this.m_LayoutStripsOnRight.m_HorizontalSplitter == null || this.m_LayoutStripsOnRight.m_HorizontalSplitter.realSizes.Length != 2)
				{
					this.m_LayoutStripsOnRight.m_HorizontalSplitter = new SplitterState(new int[]
					{
						30,
						70
					}, new int[]
					{
						160,
						160
					}, null);
				}
				if (this.m_LayoutStripsOnRight.m_VerticalSplitter == null || this.m_LayoutStripsOnRight.m_VerticalSplitter.realSizes.Length != 4)
				{
					this.m_LayoutStripsOnRight.m_VerticalSplitter = new SplitterState(new int[]
					{
						60,
						60,
						60,
						60
					}, new int[]
					{
						100,
						85,
						85,
						85
					}, null);
				}
				if (this.m_AudioGroupTreeState == null)
				{
					this.m_AudioGroupTreeState = new TreeViewState();
				}
				this.m_GroupTree = new AudioMixerGroupTreeView(this, this.m_AudioGroupTreeState);
				if (this.m_MixersTreeState == null)
				{
					this.m_MixersTreeState = new TreeViewState();
				}
				this.m_MixersTree = new AudioMixersTreeView(this, this.m_MixersTreeState, new Func<List<AudioMixerController>>(this.GetAllControllers));
				if (this.m_ViewsState == null)
				{
					this.m_ViewsState = new ReorderableListWithRenameAndScrollView.State();
				}
				this.m_GroupViews = new AudioMixerGroupViewList(this.m_ViewsState);
				if (this.m_SnapshotState == null)
				{
					this.m_SnapshotState = new ReorderableListWithRenameAndScrollView.State();
				}
				this.m_SnapshotListView = new AudioMixerSnapshotListView(this.m_SnapshotState);
				if (this.m_ChannelStripViewState == null)
				{
					this.m_ChannelStripViewState = new AudioMixerChannelStripView.State();
				}
				this.m_ChannelStripView = new AudioMixerChannelStripView(this.m_ChannelStripViewState);
				this.OnMixerControllerChanged();
				this.m_Initialized = true;
			}
		}

		private List<AudioMixerController> GetAllControllers()
		{
			return this.m_AllControllers;
		}

		private static List<AudioMixerController> FindAllAudioMixerControllers()
		{
			List<AudioMixerController> list = new List<AudioMixerController>();
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			hierarchyProperty.SetSearchFilter(new SearchFilter
			{
				classNames = new string[]
				{
					"AudioMixerController"
				}
			});
			while (hierarchyProperty.Next(null))
			{
				AudioMixerController audioMixerController = hierarchyProperty.pptrValue as AudioMixerController;
				if (audioMixerController)
				{
					list.Add(audioMixerController);
				}
			}
			return list;
		}

		public void Awake()
		{
			this.m_AllControllers = AudioMixerWindow.FindAllAudioMixerControllers();
			if (this.m_MixersTreeState != null)
			{
				this.m_MixersTreeState.OnAwake();
				this.m_MixersTreeState.selectedIDs = new List<int>();
			}
		}

		public void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			AudioMixerWindow.s_Instance = this;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.PlaymodeChanged));
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectChanged));
		}

		public void OnDisable()
		{
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.PlaymodeChanged));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectChanged));
		}

		private void PlaymodeChanged()
		{
			this.m_Ticker.Reset();
			if (this.m_Controller != null)
			{
				base.Repaint();
			}
			this.EndRenaming();
		}

		private void OnLostFocus()
		{
			this.EndRenaming();
		}

		private void EndRenaming()
		{
			if (this.m_GroupTree != null)
			{
				this.m_GroupTree.EndRenaming();
			}
			if (this.m_MixersTree != null)
			{
				this.m_MixersTree.EndRenaming();
			}
		}

		public void UndoRedoPerformed()
		{
			if (!(this.m_Controller == null))
			{
				this.m_Controller.SanitizeGroupViews();
				this.m_Controller.OnUnitySelectionChanged();
				this.m_Controller.OnSubAssetChanged();
				if (this.m_GroupTree != null)
				{
					this.m_GroupTree.OnUndoRedoPerformed();
				}
				if (this.m_GroupViews != null)
				{
					this.m_GroupViews.OnUndoRedoPerformed();
				}
				if (this.m_SnapshotListView != null)
				{
					this.m_SnapshotListView.OnUndoRedoPerformed();
				}
				if (this.m_MixersTree != null)
				{
					this.m_MixersTree.OnUndoRedoPerformed();
				}
				AudioMixerUtility.RepaintAudioMixerAndInspectors();
			}
		}

		private void OnMixerControllerChanged()
		{
			if (this.m_Controller)
			{
				this.m_Controller.ClearEventHandlers();
			}
			this.m_MixersTree.OnMixerControllerChanged(this.m_Controller);
			this.m_GroupTree.OnMixerControllerChanged(this.m_Controller);
			this.m_GroupViews.OnMixerControllerChanged(this.m_Controller);
			this.m_ChannelStripView.OnMixerControllerChanged(this.m_Controller);
			this.m_SnapshotListView.OnMixerControllerChanged(this.m_Controller);
			if (this.m_Controller)
			{
				this.m_Controller.ForceSetView(this.m_Controller.currentViewIndex);
			}
		}

		private void OnProjectChanged()
		{
			if (this.m_MixersTree == null)
			{
				this.Init();
			}
			this.m_AllControllers = AudioMixerWindow.FindAllAudioMixerControllers();
			this.m_MixersTree.ReloadTree();
		}

		public void Update()
		{
			if (this.m_Ticker.DoTick())
			{
				if (EditorApplication.isPlaying || (this.m_ChannelStripView != null && this.m_ChannelStripView.requiresRepaint))
				{
					base.Repaint();
				}
			}
		}

		private void DetectControllerChange()
		{
			AudioMixerController controller = this.m_Controller;
			if (Selection.activeObject is AudioMixerController)
			{
				this.m_Controller = (Selection.activeObject as AudioMixerController);
			}
			if (this.m_Controller != controller)
			{
				this.OnMixerControllerChanged();
			}
		}

		private void OnSelectionChange()
		{
			if (this.m_Controller != null)
			{
				this.m_Controller.OnUnitySelectionChanged();
			}
			if (this.m_GroupTree != null)
			{
				this.m_GroupTree.InitSelection(true);
			}
			base.Repaint();
		}

		private Dictionary<AudioMixerEffectController, AudioMixerGroupController> GetEffectMap(List<AudioMixerGroupController> allGroups)
		{
			Dictionary<AudioMixerEffectController, AudioMixerGroupController> dictionary = new Dictionary<AudioMixerEffectController, AudioMixerGroupController>();
			foreach (AudioMixerGroupController current in allGroups)
			{
				AudioMixerEffectController[] effects = current.effects;
				for (int i = 0; i < effects.Length; i++)
				{
					AudioMixerEffectController key = effects[i];
					dictionary[key] = current;
				}
			}
			return dictionary;
		}

		private void DoToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[]
			{
				GUILayout.Height(17f)
			});
			GUILayout.FlexibleSpace();
			if (this.m_Controller != null)
			{
				if (Application.isPlaying)
				{
					Color backgroundColor = GUI.backgroundColor;
					if (AudioSettings.editingInPlaymode)
					{
						GUI.backgroundColor = AnimationMode.animatedPropertyColor;
					}
					EditorGUI.BeginChangeCheck();
					AudioSettings.editingInPlaymode = GUILayout.Toggle(AudioSettings.editingInPlaymode, AudioMixerWindow.s_GuiContents.editSnapShots, EditorStyles.toolbarButton, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						InspectorWindow.RepaintAllInspectors();
					}
					GUI.backgroundColor = backgroundColor;
				}
				GUILayout.FlexibleSpace();
				AudioMixerExposedParametersPopup.Popup(this.m_Controller, EditorStyles.toolbarPopup, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndHorizontal();
		}

		private void RepaintIfNeeded()
		{
			if (this.m_RepaintCounter > 0)
			{
				if (Event.current.type == EventType.Repaint)
				{
					this.m_RepaintCounter--;
				}
				base.Repaint();
			}
		}

		public void OnGUI()
		{
			this.Init();
			if (AudioMixerWindow.s_GuiContents == null)
			{
				AudioMixerWindow.s_GuiContents = new AudioMixerWindow.GUIContents();
			}
			AudioMixerDrawUtils.InitStyles();
			this.DetectControllerChange();
			this.m_GroupViews.OnEvent();
			this.m_SnapshotListView.OnEvent();
			this.DoToolbar();
			List<AudioMixerGroupController> allGroups;
			if (this.m_Controller != null)
			{
				allGroups = this.m_Controller.GetAllAudioGroupsSlow();
			}
			else
			{
				allGroups = new List<AudioMixerGroupController>();
			}
			Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap = this.GetEffectMap(allGroups);
			this.m_GroupTree.UseScrollView(this.m_LayoutMode == AudioMixerWindow.LayoutMode.Horizontal);
			if (this.m_LayoutMode == AudioMixerWindow.LayoutMode.Horizontal)
			{
				this.LayoutWithStripsOnTop(allGroups, effectMap);
			}
			else
			{
				this.LayoutWithStripsOnRightSideOneScrollBar(allGroups, effectMap);
			}
			if (this.m_LastSize.x != base.position.width || this.m_LastSize.y != base.position.height)
			{
				this.m_RepaintCounter = 2;
				this.m_LastSize = new Vector2(base.position.width, base.position.height);
			}
			this.RepaintIfNeeded();
		}

		private void LayoutWithStripsOnRightSideOneScrollBar(List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
		{
			SplitterState horizontalSplitter = this.m_LayoutStripsOnRight.m_HorizontalSplitter;
			SplitterGUILayout.BeginHorizontalSplit(horizontalSplitter, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			SplitterGUILayout.EndHorizontalSplit();
			float num = (float)horizontalSplitter.realSizes[0];
			float width = base.position.width - num;
			Rect rect = new Rect(0f, 17f, num, base.position.height - 17f);
			Rect rect2 = new Rect(num, 17f, width, rect.height);
			if (EditorGUIUtility.isProSkin)
			{
				EditorGUI.DrawRect(rect, (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 0f) : new Color(0.19f, 0.19f, 0.19f));
			}
			float num2 = 10f;
			Rect[] array = new Rect[this.m_SectionOrder.Length];
			float num3 = 0f;
			for (int i = 0; i < this.m_SectionOrder.Length; i++)
			{
				num3 += num2;
				if (i > 0)
				{
					num3 += array[i - 1].height;
				}
				array[i] = new Rect(0f, num3, rect.width, this.GetHeightOfSection(this.m_SectionOrder[i]));
				Rect[] expr_157_cp_0 = array;
				int expr_157_cp_1 = i;
				expr_157_cp_0[expr_157_cp_1].x = expr_157_cp_0[expr_157_cp_1].x + 4f;
				Rect[] expr_171_cp_0 = array;
				int expr_171_cp_1 = i;
				expr_171_cp_0[expr_171_cp_1].width = expr_171_cp_0[expr_171_cp_1].width - 8f;
			}
			Rect viewRect = new Rect(0f, 0f, 1f, array.Last<Rect>().yMax);
			if (viewRect.height > rect.height)
			{
				for (int j = 0; j < array.Length; j++)
				{
					Rect[] expr_1E3_cp_0 = array;
					int expr_1E3_cp_1 = j;
					expr_1E3_cp_0[expr_1E3_cp_1].width = expr_1E3_cp_0[expr_1E3_cp_1].width - 14f;
				}
			}
			this.m_SectionsScrollPosition = GUI.BeginScrollView(rect, this.m_SectionsScrollPosition, viewRect);
			this.DoSections(rect, array, this.m_SectionOrder);
			GUI.EndScrollView();
			this.m_ChannelStripView.OnGUI(rect2, this.m_ShowReferencedBuses, this.m_ShowBusConnections, this.m_ShowBusConnectionsOfSelection, allGroups, effectMap, this.m_SortGroupsAlphabetically, this.m_ShowDeveloperOverlays, this.m_GroupTree.ScrollToItem);
			EditorGUI.DrawRect(new Rect(rect.xMax - 1f, 17f, 1f, base.position.height - 17f), (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.15f, 0.15f, 0.15f));
		}

		private void LayoutWithStripsOnTop(List<AudioMixerGroupController> allGroups, Dictionary<AudioMixerEffectController, AudioMixerGroupController> effectMap)
		{
			SplitterState horizontalSplitter = this.m_LayoutStripsOnTop.m_HorizontalSplitter;
			SplitterState verticalSplitter = this.m_LayoutStripsOnTop.m_VerticalSplitter;
			SplitterGUILayout.BeginVerticalSplit(verticalSplitter, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			if (this.m_GroupsRenderedAboveSections)
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.EndVertical();
			}
			SplitterGUILayout.BeginHorizontalSplit(horizontalSplitter, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			if (!this.m_GroupsRenderedAboveSections)
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.EndVertical();
			}
			SplitterGUILayout.EndHorizontalSplit();
			SplitterGUILayout.EndVerticalSplit();
			float y = (float)((!this.m_GroupsRenderedAboveSections) ? (17 + verticalSplitter.realSizes[0]) : 17);
			float height = (float)((!this.m_GroupsRenderedAboveSections) ? verticalSplitter.realSizes[1] : verticalSplitter.realSizes[0]);
			float y2 = (float)(this.m_GroupsRenderedAboveSections ? (17 + verticalSplitter.realSizes[0]) : 17);
			float num = (float)(this.m_GroupsRenderedAboveSections ? verticalSplitter.realSizes[1] : verticalSplitter.realSizes[0]);
			Rect rect = new Rect(0f, y, base.position.width, height);
			Rect totalRectOfSections = new Rect(0f, rect.yMax, base.position.width, base.position.height - rect.height);
			Rect[] array = new Rect[this.m_SectionOrder.Length];
			for (int i = 0; i < array.Length; i++)
			{
				float x = (i <= 0) ? 0f : array[i - 1].xMax;
				array[i] = new Rect(x, y2, (float)horizontalSplitter.realSizes[i], num - 12f);
			}
			Rect[] expr_1F2_cp_0 = array;
			int expr_1F2_cp_1 = 0;
			expr_1F2_cp_0[expr_1F2_cp_1].x = expr_1F2_cp_0[expr_1F2_cp_1].x + 8f;
			Rect[] expr_20B_cp_0 = array;
			int expr_20B_cp_1 = 0;
			expr_20B_cp_0[expr_20B_cp_1].width = expr_20B_cp_0[expr_20B_cp_1].width - 12f;
			Rect[] expr_229_cp_0 = array;
			int expr_229_cp_1 = array.Length - 1;
			expr_229_cp_0[expr_229_cp_1].x = expr_229_cp_0[expr_229_cp_1].x + 4f;
			Rect[] expr_247_cp_0 = array;
			int expr_247_cp_1 = array.Length - 1;
			expr_247_cp_0[expr_247_cp_1].width = expr_247_cp_0[expr_247_cp_1].width - 12f;
			for (int j = 1; j < array.Length - 1; j++)
			{
				Rect[] expr_26A_cp_0 = array;
				int expr_26A_cp_1 = j;
				expr_26A_cp_0[expr_26A_cp_1].x = expr_26A_cp_0[expr_26A_cp_1].x + 4f;
				Rect[] expr_284_cp_0 = array;
				int expr_284_cp_1 = j;
				expr_284_cp_0[expr_284_cp_1].width = expr_284_cp_0[expr_284_cp_1].width - 8f;
			}
			this.DoSections(totalRectOfSections, array, this.m_SectionOrder);
			this.m_ChannelStripView.OnGUI(rect, this.m_ShowReferencedBuses, this.m_ShowBusConnections, this.m_ShowBusConnectionsOfSelection, allGroups, effectMap, this.m_SortGroupsAlphabetically, this.m_ShowDeveloperOverlays, this.m_GroupTree.ScrollToItem);
			EditorGUI.DrawRect(new Rect(0f, (float)(17 + verticalSplitter.realSizes[0] - 1), base.position.width, 1f), new Color(0f, 0f, 0f, 0.4f));
		}

		private float GetHeightOfSection(AudioMixerWindow.SectionType sectionType)
		{
			float result;
			switch (sectionType)
			{
			case AudioMixerWindow.SectionType.MixerTree:
				result = this.m_MixersTree.GetTotalHeight();
				break;
			case AudioMixerWindow.SectionType.GroupTree:
				result = this.m_GroupTree.GetTotalHeight();
				break;
			case AudioMixerWindow.SectionType.ViewList:
				result = this.m_GroupViews.GetTotalHeight();
				break;
			case AudioMixerWindow.SectionType.SnapshotList:
				result = this.m_SnapshotListView.GetTotalHeight();
				break;
			default:
				Debug.LogError("Unhandled enum value");
				result = 0f;
				break;
			}
			return result;
		}

		private void DoSections(Rect totalRectOfSections, Rect[] sectionRects, AudioMixerWindow.SectionType[] sectionOrder)
		{
			Event current = Event.current;
			bool flag = this.m_Controller == null || AudioMixerController.EditingTargetSnapshot();
			for (int i = 0; i < sectionOrder.Length; i++)
			{
				Rect rect = sectionRects[i];
				if (rect.height > 0f)
				{
					switch (sectionOrder[i])
					{
					case AudioMixerWindow.SectionType.MixerTree:
						this.m_MixersTree.OnGUI(rect);
						break;
					case AudioMixerWindow.SectionType.GroupTree:
						this.m_GroupTree.OnGUI(rect);
						break;
					case AudioMixerWindow.SectionType.ViewList:
						this.m_GroupViews.OnGUI(rect);
						break;
					case AudioMixerWindow.SectionType.SnapshotList:
						using (new EditorGUI.DisabledScope(!flag))
						{
							this.m_SnapshotListView.OnGUI(rect);
						}
						break;
					default:
						Debug.LogError("Unhandled enum value");
						break;
					}
					if (current.type == EventType.ContextClick)
					{
						Rect rect2 = new Rect(rect.x, rect.y, rect.width - 15f, 22f);
						if (rect2.Contains(current.mousePosition))
						{
							this.ReorderContextMenu(rect2, i);
							current.Use();
						}
					}
				}
			}
		}

		private void ReorderContextMenu(Rect rect, int sectionIndex)
		{
			Event current = Event.current;
			if (Event.current.type == EventType.ContextClick && rect.Contains(current.mousePosition))
			{
				GUIContent content = new GUIContent((this.m_LayoutMode != AudioMixerWindow.LayoutMode.Horizontal) ? "Move Up" : "Move Left");
				GUIContent content2 = new GUIContent((this.m_LayoutMode != AudioMixerWindow.LayoutMode.Horizontal) ? "Move Down" : "Move Right");
				GenericMenu genericMenu = new GenericMenu();
				if (sectionIndex > 1)
				{
					genericMenu.AddItem(content, false, new GenericMenu.MenuFunction2(this.ChangeSectionOrder), new Vector2((float)sectionIndex, -1f));
				}
				else
				{
					genericMenu.AddDisabledItem(content);
				}
				if (sectionIndex > 0 && sectionIndex < this.m_SectionOrder.Length - 1)
				{
					genericMenu.AddItem(content2, false, new GenericMenu.MenuFunction2(this.ChangeSectionOrder), new Vector2((float)sectionIndex, 1f));
				}
				else
				{
					genericMenu.AddDisabledItem(content2);
				}
				genericMenu.ShowAsContext();
			}
		}

		private void ChangeSectionOrder(object userData)
		{
			Vector2 vector = (Vector2)userData;
			int num = (int)vector.x;
			int num2 = (int)vector.y;
			int num3 = Mathf.Clamp(num + num2, 0, this.m_SectionOrder.Length - 1);
			if (num3 != num)
			{
				AudioMixerWindow.SectionType sectionType = this.m_SectionOrder[num];
				this.m_SectionOrder[num] = this.m_SectionOrder[num3];
				this.m_SectionOrder[num3] = sectionType;
			}
		}

		public MixerParameterDefinition ParamDef(string name, string desc, string units, float displayScale, float minRange, float maxRange, float defaultValue)
		{
			return new MixerParameterDefinition
			{
				name = name,
				description = desc,
				units = units,
				displayScale = displayScale,
				minRange = minRange,
				maxRange = maxRange,
				defaultValue = defaultValue
			};
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Sort groups alphabetically"), this.m_SortGroupsAlphabetically, delegate
			{
				this.m_SortGroupsAlphabetically = !this.m_SortGroupsAlphabetically;
			});
			menu.AddItem(new GUIContent("Show referenced groups"), this.m_ShowReferencedBuses, delegate
			{
				this.m_ShowReferencedBuses = !this.m_ShowReferencedBuses;
			});
			menu.AddItem(new GUIContent("Show group connections"), this.m_ShowBusConnections, delegate
			{
				this.m_ShowBusConnections = !this.m_ShowBusConnections;
			});
			if (this.m_ShowBusConnections)
			{
				menu.AddItem(new GUIContent("Only highlight selected group connections"), this.m_ShowBusConnectionsOfSelection, delegate
				{
					this.m_ShowBusConnectionsOfSelection = !this.m_ShowBusConnectionsOfSelection;
				});
			}
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Vertical layout"), this.layoutMode == AudioMixerWindow.LayoutMode.Vertical, delegate
			{
				this.layoutMode = AudioMixerWindow.LayoutMode.Vertical;
			});
			menu.AddItem(new GUIContent("Horizontal layout"), this.layoutMode == AudioMixerWindow.LayoutMode.Horizontal, delegate
			{
				this.layoutMode = AudioMixerWindow.LayoutMode.Horizontal;
			});
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Use RMS metering for display"), EditorPrefs.GetBool(AudioMixerWindow.kAudioMixerUseRMSMetering, true), delegate
			{
				EditorPrefs.SetBool(AudioMixerWindow.kAudioMixerUseRMSMetering, true);
			});
			menu.AddItem(new GUIContent("Use peak metering for display"), !EditorPrefs.GetBool(AudioMixerWindow.kAudioMixerUseRMSMetering, true), delegate
			{
				EditorPrefs.SetBool(AudioMixerWindow.kAudioMixerUseRMSMetering, false);
			});
			if (Unsupported.IsDeveloperBuild())
			{
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("DEVELOPER/Groups Rendered Above"), this.m_GroupsRenderedAboveSections, delegate
				{
					this.m_GroupsRenderedAboveSections = !this.m_GroupsRenderedAboveSections;
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build 10 groups"), false, delegate
				{
					this.m_Controller.BuildTestSetup(0, 7, 10);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build 20 groups"), false, delegate
				{
					this.m_Controller.BuildTestSetup(0, 7, 20);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build 40 groups"), false, delegate
				{
					this.m_Controller.BuildTestSetup(0, 7, 40);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build 80 groups"), false, delegate
				{
					this.m_Controller.BuildTestSetup(0, 7, 80);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build 160 groups"), false, delegate
				{
					this.m_Controller.BuildTestSetup(0, 7, 160);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build chain of 10 groups"), false, delegate
				{
					this.m_Controller.BuildTestSetup(1, 1, 10);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build chain of 20 groups "), false, delegate
				{
					this.m_Controller.BuildTestSetup(1, 1, 20);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build chain of 40 groups"), false, delegate
				{
					this.m_Controller.BuildTestSetup(1, 1, 40);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Build chain of 80 groups"), false, delegate
				{
					this.m_Controller.BuildTestSetup(1, 1, 80);
				});
				menu.AddItem(new GUIContent("DEVELOPER/Show overlays"), this.m_ShowDeveloperOverlays, delegate
				{
					this.m_ShowDeveloperOverlays = !this.m_ShowDeveloperOverlays;
				});
			}
		}
	}
}
