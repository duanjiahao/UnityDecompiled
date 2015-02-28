using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class FrameDebuggerWindow : EditorWindow
	{
		internal class Styles
		{
			public GUIStyle background = "OL Box";
			public GUIStyle header = "OL title";
			public GUIStyle entryEven = "OL EntryBackEven";
			public GUIStyle entryOdd = "OL EntryBackOdd";
			public GUIStyle rowText = "OL Label";
			public GUIStyle rowTextRight = new GUIStyle("OL Label");
			public GUIContent recordButton = new GUIContent(EditorGUIUtility.TextContent("Profiler.Record"));
			public GUIContent prevFrame = new GUIContent(EditorGUIUtility.IconContent("Profiler.PrevFrame"));
			public GUIContent nextFrame = new GUIContent(EditorGUIUtility.IconContent("Profiler.NextFrame"));
			public GUIContent[] headerContent;
			public static readonly string[] s_ColumnNames = new string[]
			{
				"#",
				"Type",
				"Vertices",
				"Indices"
			};
			public static readonly GUIContent[] mrtLabels = new GUIContent[]
			{
				EditorGUIUtility.TextContent("FrameDebugger.RT0"),
				EditorGUIUtility.TextContent("FrameDebugger.RT1"),
				EditorGUIUtility.TextContent("FrameDebugger.RT2"),
				EditorGUIUtility.TextContent("FrameDebugger.RT3"),
				EditorGUIUtility.TextContent("FrameDebugger.RT4"),
				EditorGUIUtility.TextContent("FrameDebugger.RT5"),
				EditorGUIUtility.TextContent("FrameDebugger.RT6"),
				EditorGUIUtility.TextContent("FrameDebugger.RT7")
			};
			public static readonly GUIContent depthLabel = EditorGUIUtility.TextContent("FrameDebugger.RTDepth");
			public static readonly GUIContent[] channelLabels = new GUIContent[]
			{
				EditorGUIUtility.TextContent("FrameDebugger.ChannelsAll"),
				EditorGUIUtility.TextContent("FrameDebugger.ChannelsR"),
				EditorGUIUtility.TextContent("FrameDebugger.ChannelsG"),
				EditorGUIUtility.TextContent("FrameDebugger.ChannelsB"),
				EditorGUIUtility.TextContent("FrameDebugger.ChannelsA")
			};
			public static readonly GUIContent channelHeader = EditorGUIUtility.TextContent("FrameDebugger.Channels");
			public static readonly GUIContent levelsHeader = EditorGUIUtility.TextContent("FrameDebugger.Levels");
			public Styles()
			{
				this.rowTextRight.alignment = TextAnchor.MiddleRight;
				this.recordButton.text = "Enable";
				this.recordButton.tooltip = "Enable Frame Debugging";
				this.prevFrame.tooltip = "Previous event";
				this.nextFrame.tooltip = "Next event";
				this.headerContent = new GUIContent[FrameDebuggerWindow.Styles.s_ColumnNames.Length];
				for (int i = 0; i < this.headerContent.Length; i++)
				{
					this.headerContent[i] = EditorGUIUtility.TextContent(FrameDebuggerWindow.Styles.s_ColumnNames[i]);
				}
			}
		}
		private const float kScrollbarWidth = 16f;
		private const float kResizerWidth = 5f;
		private const float kMinListWidth = 200f;
		private const float kMinDetailsWidth = 200f;
		private const float kMinWindowWidth = 240f;
		private const float kDetailsMargin = 4f;
		private const float kMinPreviewSize = 64f;
		private const int kNeedToRepaintFrames = 4;
		public static readonly string[] s_FrameEventTypeNames = new string[]
		{
			"Clear (nothing)",
			"Clear (color)",
			"Clear (Z)",
			"Clear (color+Z)",
			"Clear (stencil)",
			"Clear (color+stencil)",
			"Clear (Z+stencil)",
			"Clear (color+Z+stencil)",
			"SetRenderTarget",
			"Resolve Color",
			"Resolve Depth",
			"Grab RenderTexture",
			"Static Batch",
			"Dynamic Batch",
			"Draw Mesh",
			"Draw Dynamic",
			"Draw GL",
			"GPU Skinning",
			"Draw Procedural",
			"Compute Shader",
			"Plugin Event"
		};
		[SerializeField]
		private float m_ListWidth = 300f;
		private int m_RepaintFrames = 4;
		private PreviewRenderUtility m_PreviewUtility;
		public Vector2 m_PreviewDir = new Vector2(120f, -20f);
		private Material m_Material;
		private Material m_WireMaterial;
		[SerializeField]
		private TreeViewState m_TreeViewState;
		[NonSerialized]
		private FrameDebuggerTreeView m_Tree;
		[NonSerialized]
		private int m_FrameEventsHash;
		[NonSerialized]
		private int m_RTIndex;
		[NonSerialized]
		private int m_RTChannel;
		[NonSerialized]
		private float m_RTBlackLevel;
		[NonSerialized]
		private float m_RTWhiteLevel = 1f;
		private int m_PrevEventsLimit;
		private int m_PrevEventsCount;
		private static List<FrameDebuggerWindow> s_FrameDebuggers = new List<FrameDebuggerWindow>();
		private static FrameDebuggerWindow.Styles ms_Styles;
		public static FrameDebuggerWindow.Styles styles
		{
			get
			{
				FrameDebuggerWindow.Styles arg_17_0;
				if ((arg_17_0 = FrameDebuggerWindow.ms_Styles) == null)
				{
					arg_17_0 = (FrameDebuggerWindow.ms_Styles = new FrameDebuggerWindow.Styles());
				}
				return arg_17_0;
			}
		}
		public FrameDebuggerWindow()
		{
			base.position = new Rect(50f, 50f, 600f, 350f);
			base.minSize = new Vector2(400f, 200f);
		}
		[MenuItem("Window/Frame Debugger", false, 2100)]
		public static FrameDebuggerWindow ShowFrameDebuggerWindow()
		{
			FrameDebuggerWindow frameDebuggerWindow = EditorWindow.GetWindow(typeof(FrameDebuggerWindow)) as FrameDebuggerWindow;
			if (frameDebuggerWindow != null)
			{
				frameDebuggerWindow.title = "Frame Debug";
				frameDebuggerWindow.EnableIfNeeded();
			}
			return frameDebuggerWindow;
		}
		[MenuItem("Window/Frame Debugger", true, 2100)]
		public static bool ShowFrameDebuggerWindowValidate()
		{
			return InternalEditorUtility.HasProFeaturesEnabled();
		}
		internal static void RepaintAll()
		{
			foreach (FrameDebuggerWindow current in FrameDebuggerWindow.s_FrameDebuggers)
			{
				current.Repaint();
			}
		}
		internal void ChangeFrameEventLimit(int newLimit)
		{
			if (newLimit <= 0 || newLimit > FrameDebuggerUtility.count)
			{
				return;
			}
			if (newLimit != FrameDebuggerUtility.limit && newLimit > 0)
			{
				GameObject gameObjectForEvent = FrameDebuggerWindow.GetGameObjectForEvent(newLimit - 1);
				if (gameObjectForEvent != null)
				{
					EditorGUIUtility.PingObject(gameObjectForEvent);
				}
			}
			FrameDebuggerUtility.limit = newLimit;
			if (this.m_Tree != null)
			{
				this.m_Tree.SelectFrameEventIndex(newLimit);
			}
		}
		private static void DisableFrameDebugger()
		{
			if (FrameDebuggerUtility.enabled)
			{
				EditorApplication.SetSceneRepaintDirty();
			}
			FrameDebuggerUtility.enabled = false;
		}
		internal void OnDidOpenScene()
		{
			FrameDebuggerWindow.DisableFrameDebugger();
		}
		private void OnPlayModeStateChanged()
		{
			this.RepaintOnLimitChange();
		}
		internal void OnEnable()
		{
			base.autoRepaintOnSceneChange = true;
			FrameDebuggerWindow.s_FrameDebuggers.Add(this);
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
			this.m_RepaintFrames = 4;
		}
		internal void OnDisable()
		{
			if (this.m_WireMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_WireMaterial.shader, true);
				UnityEngine.Object.DestroyImmediate(this.m_WireMaterial, true);
			}
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
			FrameDebuggerWindow.s_FrameDebuggers.Remove(this);
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
			FrameDebuggerWindow.DisableFrameDebugger();
		}
		public void EnableIfNeeded()
		{
			if (FrameDebuggerUtility.enabled)
			{
				return;
			}
			this.m_RTChannel = 0;
			this.m_RTIndex = 0;
			this.m_RTBlackLevel = 0f;
			this.m_RTWhiteLevel = 1f;
			this.ClickEnableFrameDebugger();
			this.RepaintOnLimitChange();
		}
		private void ClickEnableFrameDebugger()
		{
			if (!FrameDebuggerWindow.GraphicsSupportsFrameDebugger())
			{
				return;
			}
			if (!FrameDebuggerUtility.enabled && EditorApplication.isPlaying && !EditorApplication.isPaused)
			{
				EditorApplication.isPaused = true;
			}
			FrameDebuggerUtility.enabled = !FrameDebuggerUtility.enabled;
			if (FrameDebuggerUtility.enabled)
			{
				GameView gameView = (GameView)WindowLayout.FindEditorWindowOfType(typeof(GameView));
				if (gameView)
				{
					gameView.ShowTab();
				}
			}
			this.m_PrevEventsLimit = FrameDebuggerUtility.limit;
			this.m_PrevEventsCount = FrameDebuggerUtility.count;
		}
		private bool DrawToolbar(FrameDebuggerEvent[] descs)
		{
			bool result = false;
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginDisabledGroup(!FrameDebuggerWindow.GraphicsSupportsFrameDebugger());
			GUILayout.Toggle(FrameDebuggerUtility.enabled, FrameDebuggerWindow.styles.recordButton, EditorStyles.toolbarButton, new GUILayoutOption[]
			{
				GUILayout.MinWidth(80f)
			});
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck())
			{
				this.ClickEnableFrameDebugger();
				result = true;
			}
			GUI.enabled = FrameDebuggerUtility.enabled;
			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginDisabledGroup(FrameDebuggerUtility.count <= 1);
			int num = EditorGUILayout.IntSlider(FrameDebuggerUtility.limit, 1, FrameDebuggerUtility.count, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck())
			{
				this.ChangeFrameEventLimit(num);
			}
			GUILayout.Label(" of " + FrameDebuggerUtility.count, EditorStyles.miniLabel, new GUILayoutOption[0]);
			EditorGUI.BeginDisabledGroup(num <= 1);
			if (GUILayout.Button(FrameDebuggerWindow.styles.prevFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.ChangeFrameEventLimit(num - 1);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(num >= FrameDebuggerUtility.count);
			if (GUILayout.Button(FrameDebuggerWindow.styles.nextFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.ChangeFrameEventLimit(num + 1);
			}
			if (this.m_PrevEventsLimit == this.m_PrevEventsCount && FrameDebuggerUtility.count != this.m_PrevEventsCount && FrameDebuggerUtility.limit == this.m_PrevEventsLimit)
			{
				this.ChangeFrameEventLimit(FrameDebuggerUtility.count);
			}
			this.m_PrevEventsLimit = FrameDebuggerUtility.limit;
			this.m_PrevEventsCount = FrameDebuggerUtility.count;
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
			return result;
		}
		private void DrawMeshPreview(FrameDebuggerEvent curEvent, Rect previewRect, Rect meshInfoRect, Mesh mesh, int meshSubset)
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
				this.m_PreviewUtility.m_CameraFieldOfView = 30f;
			}
			if (this.m_Material == null)
			{
				this.m_Material = (EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material);
			}
			if (this.m_WireMaterial == null)
			{
				this.m_WireMaterial = new Material(ModelInspector.WireframeShaderSource);
				this.m_WireMaterial.hideFlags = HideFlags.HideAndDontSave;
				this.m_WireMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
			}
			this.m_PreviewUtility.BeginPreview(previewRect, "preBackground");
			ModelInspector.RenderMeshPreview(mesh, this.m_PreviewUtility, this.m_Material, this.m_WireMaterial, this.m_PreviewDir, meshSubset);
			Texture image = this.m_PreviewUtility.EndPreview();
			GUI.DrawTexture(previewRect, image, ScaleMode.StretchToFill, false);
			string text = mesh.name;
			if (string.IsNullOrEmpty(text))
			{
				text = "<no name>";
			}
			string text2 = string.Concat(new object[]
			{
				text,
				" subset ",
				meshSubset,
				"\n",
				curEvent.vertexCount,
				" verts, ",
				curEvent.indexCount,
				" indices"
			});
			EditorGUI.DropShadowLabel(meshInfoRect, text2);
		}
		internal static GameObject GetGameObjectForEvent(int eventIndex)
		{
			GameObject result = null;
			int frameEventRendererID = FrameDebuggerUtility.GetFrameEventRendererID(eventIndex);
			Component component = EditorUtility.InstanceIDToObject(frameEventRendererID) as Component;
			if (component != null)
			{
				result = component.gameObject;
			}
			return result;
		}
		private bool DrawEventMesh(int curEventIndex, FrameDebuggerEvent curEvent)
		{
			int frameEventMeshID = FrameDebuggerUtility.GetFrameEventMeshID(curEventIndex);
			if (frameEventMeshID == 0)
			{
				return false;
			}
			UnityEngine.Object @object = EditorUtility.InstanceIDToObject(frameEventMeshID);
			Mesh mesh = @object as Mesh;
			if (mesh == null)
			{
				return false;
			}
			Rect rect = GUILayoutUtility.GetRect(10f, 10f, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true)
			});
			if (rect.width < 64f || rect.height < 64f)
			{
				return true;
			}
			GameObject gameObjectForEvent = FrameDebuggerWindow.GetGameObjectForEvent(curEventIndex);
			Rect rect2 = rect;
			rect2.yMin = rect2.yMax - EditorGUIUtility.singleLineHeight * 2f;
			Rect position = rect2;
			rect2.xMin = rect2.center.x;
			position.xMax = position.center.x;
			if (Event.current.type == EventType.MouseDown)
			{
				if (rect2.Contains(Event.current.mousePosition))
				{
					EditorGUIUtility.PingObject(frameEventMeshID);
					Event.current.Use();
				}
				if (gameObjectForEvent != null && position.Contains(Event.current.mousePosition))
				{
					EditorGUIUtility.PingObject(gameObjectForEvent.GetInstanceID());
					Event.current.Use();
				}
			}
			this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, rect);
			if (Event.current.type == EventType.Repaint)
			{
				int frameEventMeshSubset = FrameDebuggerUtility.GetFrameEventMeshSubset(curEventIndex);
				this.DrawMeshPreview(curEvent, rect, rect2, mesh, frameEventMeshSubset);
				if (gameObjectForEvent != null)
				{
					EditorGUI.DropShadowLabel(position, gameObjectForEvent.name);
				}
			}
			return true;
		}
		private void DrawRenderTargetControls(FrameDebuggerEvent cur)
		{
			if (cur.rtWidth <= 0 || cur.rtHeight <= 0)
			{
				return;
			}
			bool disabled = cur.rtFormat == 1 || cur.rtFormat == 3;
			bool flag = cur.rtHasDepthTexture != 0;
			short num = cur.rtCount;
			if (flag)
			{
				num += 1;
			}
			GUILayout.Label("RenderTarget: " + cur.rtName, EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginDisabledGroup(num <= 1);
			GUIContent[] array = new GUIContent[(int)num];
			for (int i = 0; i < (int)cur.rtCount; i++)
			{
				array[i] = FrameDebuggerWindow.Styles.mrtLabels[i];
			}
			if (flag)
			{
				array[(int)cur.rtCount] = FrameDebuggerWindow.Styles.depthLabel;
			}
			int num2 = Mathf.Clamp(this.m_RTIndex, 0, (int)(num - 1));
			bool flag2 = num2 != this.m_RTIndex;
			this.m_RTIndex = num2;
			this.m_RTIndex = EditorGUILayout.Popup(this.m_RTIndex, array, EditorStyles.toolbarPopup, new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			});
			EditorGUI.EndDisabledGroup();
			GUILayout.Space(10f);
			EditorGUI.BeginDisabledGroup(disabled);
			GUILayout.Label(FrameDebuggerWindow.Styles.channelHeader, EditorStyles.miniLabel, new GUILayoutOption[0]);
			this.m_RTChannel = GUILayout.Toolbar(this.m_RTChannel, FrameDebuggerWindow.Styles.channelLabels, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			GUILayout.Space(10f);
			GUILayout.Label(FrameDebuggerWindow.Styles.levelsHeader, EditorStyles.miniLabel, new GUILayoutOption[0]);
			EditorGUILayout.MinMaxSlider(ref this.m_RTBlackLevel, ref this.m_RTWhiteLevel, 0f, 1f, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(200f)
			});
			if (EditorGUI.EndChangeCheck() || flag2)
			{
				Vector4 channels = Vector4.zero;
				if (this.m_RTChannel == 1)
				{
					channels.x = 1f;
				}
				else
				{
					if (this.m_RTChannel == 2)
					{
						channels.y = 1f;
					}
					else
					{
						if (this.m_RTChannel == 3)
						{
							channels.z = 1f;
						}
						else
						{
							if (this.m_RTChannel == 4)
							{
								channels.w = 1f;
							}
							else
							{
								channels = Vector4.one;
							}
						}
					}
				}
				int num3 = this.m_RTIndex;
				if (num3 >= (int)cur.rtCount)
				{
					num3 = -1;
				}
				FrameDebuggerUtility.SetRenderTargetDisplayOptions(num3, channels, this.m_RTBlackLevel, this.m_RTWhiteLevel);
				this.RepaintAllNeededThings();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Label(string.Format("{0}x{1} {2}", cur.rtWidth, cur.rtHeight, (RenderTextureFormat)cur.rtFormat), new GUILayoutOption[0]);
			if (cur.rtDim == 4)
			{
				GUILayout.Label("Rendering into cubemap", new GUILayoutOption[0]);
			}
			if (cur.rtFormat == 3 && SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9"))
			{
				EditorGUILayout.HelpBox("Rendering into shadowmap on DX9, can't visualize it in the game view properly", MessageType.Info, true);
			}
		}
		private void DrawCurrentEvent(Rect rect, FrameDebuggerEvent[] descs)
		{
			int num = FrameDebuggerUtility.limit - 1;
			if (num < 0 || num >= descs.Length)
			{
				return;
			}
			GUILayout.BeginArea(rect);
			FrameDebuggerEvent frameDebuggerEvent = descs[num];
			this.DrawRenderTargetControls(frameDebuggerEvent);
			GUILayout.Label(string.Format("Event #{0}: {1}", num + 1, FrameDebuggerWindow.s_FrameEventTypeNames[(int)frameDebuggerEvent.type]), EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (frameDebuggerEvent.vertexCount > 0 || frameDebuggerEvent.indexCount > 0)
			{
				int frameEventShaderID = FrameDebuggerUtility.GetFrameEventShaderID(num);
				if (frameEventShaderID != 0)
				{
					Shader shader = EditorUtility.InstanceIDToObject(frameEventShaderID) as Shader;
					if (shader != null)
					{
						int frameEventShaderPassIndex = FrameDebuggerUtility.GetFrameEventShaderPassIndex(num);
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						if (GUILayout.Button(string.Concat(new object[]
						{
							"Shader: ",
							shader.name,
							" pass #",
							frameEventShaderPassIndex
						}), GUI.skin.label, new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(false)
						}))
						{
							EditorGUIUtility.PingObject(shader);
							Event.current.Use();
						}
						GUILayout.Label(FrameDebuggerUtility.GetFrameEventShaderKeywords(num), EditorStyles.miniLabel, new GUILayoutOption[0]);
						GUILayout.EndHorizontal();
					}
				}
				if (!this.DrawEventMesh(num, frameDebuggerEvent))
				{
					GUILayout.Label("Vertices: " + frameDebuggerEvent.vertexCount, new GUILayoutOption[0]);
					GUILayout.Label("Indices: " + frameDebuggerEvent.indexCount, new GUILayoutOption[0]);
				}
			}
			GUILayout.EndArea();
		}
		private static bool GraphicsSupportsFrameDebugger()
		{
			return SystemInfo.graphicsMultiThreaded;
		}
		internal void OnGUI()
		{
			if (!FrameDebuggerWindow.ShowFrameDebuggerWindowValidate())
			{
				EditorGUILayout.HelpBox("Frame Debugger requires a Pro license", MessageType.Warning, true);
				return;
			}
			FrameDebuggerEvent[] frameEvents = FrameDebuggerUtility.GetFrameEvents();
			if (this.m_TreeViewState == null)
			{
				this.m_TreeViewState = new TreeViewState();
			}
			if (this.m_Tree == null)
			{
				this.m_Tree = new FrameDebuggerTreeView(frameEvents, this.m_TreeViewState, this, default(Rect));
				this.m_FrameEventsHash = FrameDebuggerUtility.eventsHash;
				this.m_Tree.m_DataSource.SetExpandedWithChildren(this.m_Tree.m_DataSource.root, true);
			}
			if (FrameDebuggerUtility.eventsHash != this.m_FrameEventsHash)
			{
				this.m_Tree.m_DataSource.SetEvents(frameEvents);
				this.m_FrameEventsHash = FrameDebuggerUtility.eventsHash;
			}
			int limit = FrameDebuggerUtility.limit;
			bool flag = this.DrawToolbar(frameEvents);
			if (!FrameDebuggerUtility.enabled)
			{
				GUI.enabled = true;
				if (!FrameDebuggerWindow.GraphicsSupportsFrameDebugger())
				{
					EditorGUILayout.HelpBox("Frame Debugger requires multi-threaded renderer. Usually Unity uses that; if it does not try starting with -force-gfx-mt command line argument.", MessageType.Warning, true);
				}
				EditorGUILayout.HelpBox("Frame Debugger lets you step through draw calls and see how exactly frame is rendered. Click Enable!", MessageType.Info, true);
			}
			else
			{
				float fixedHeight = EditorStyles.toolbar.fixedHeight;
				Rect dragRect = new Rect(this.m_ListWidth, fixedHeight, 5f, base.position.height - fixedHeight);
				dragRect = EditorGUIUtility.HandleHorizontalSplitter(dragRect, base.position.width, 200f, 200f);
				this.m_ListWidth = dragRect.x;
				Rect rect = new Rect(0f, fixedHeight, this.m_ListWidth, base.position.height - fixedHeight);
				Rect rect2 = new Rect(this.m_ListWidth + 4f, fixedHeight + 4f, base.position.width - this.m_ListWidth - 8f, base.position.height - fixedHeight - 8f);
				this.DrawEventsTree(rect);
				EditorGUIUtility.DrawHorizontalSplitter(dragRect);
				this.DrawCurrentEvent(rect2, frameEvents);
			}
			if (flag || limit != FrameDebuggerUtility.limit)
			{
				this.RepaintOnLimitChange();
			}
			if (this.m_RepaintFrames > 0)
			{
				this.m_Tree.SelectFrameEventIndex(FrameDebuggerUtility.limit);
				this.RepaintAllNeededThings();
				this.m_RepaintFrames--;
			}
		}
		private void RepaintOnLimitChange()
		{
			this.m_RepaintFrames = 4;
			this.RepaintAllNeededThings();
		}
		private void RepaintAllNeededThings()
		{
			EditorApplication.SetSceneRepaintDirty();
			base.Repaint();
		}
		private void DrawEventsTree(Rect rect)
		{
			this.m_Tree.OnGUI(rect);
		}
	}
}
