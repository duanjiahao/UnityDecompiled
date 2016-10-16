using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class FrameDebuggerWindow : EditorWindow
	{
		internal class Styles
		{
			public GUIStyle header = "OL title";

			public GUIStyle entryEven = "OL EntryBackEven";

			public GUIStyle entryOdd = "OL EntryBackOdd";

			public GUIStyle rowText = "OL Label";

			public GUIStyle rowTextRight = new GUIStyle("OL Label");

			public GUIContent recordButton = new GUIContent(EditorGUIUtility.TextContent("Record|Record profiling information"));

			public GUIContent prevFrame = new GUIContent(EditorGUIUtility.IconContent("Profiler.PrevFrame", "|Go back one frame"));

			public GUIContent nextFrame = new GUIContent(EditorGUIUtility.IconContent("Profiler.NextFrame", "|Go one frame forwards"));

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
				EditorGUIUtility.TextContent("RT 0|Show render target #0"),
				EditorGUIUtility.TextContent("RT 1|Show render target #1"),
				EditorGUIUtility.TextContent("RT 2|Show render target #2"),
				EditorGUIUtility.TextContent("RT 3|Show render target #3"),
				EditorGUIUtility.TextContent("RT 4|Show render target #4"),
				EditorGUIUtility.TextContent("RT 5|Show render target #5"),
				EditorGUIUtility.TextContent("RT 6|Show render target #6"),
				EditorGUIUtility.TextContent("RT 7|Show render target #7")
			};

			public static readonly GUIContent depthLabel = EditorGUIUtility.TextContent("Depth|Show depth buffer");

			public static readonly GUIContent[] channelLabels = new GUIContent[]
			{
				EditorGUIUtility.TextContent("All|Show all (RGB) color channels"),
				EditorGUIUtility.TextContent("R|Show red channel only"),
				EditorGUIUtility.TextContent("G|Show green channel only"),
				EditorGUIUtility.TextContent("B|Show blue channel only"),
				EditorGUIUtility.TextContent("A|Show alpha channel only")
			};

			public static readonly GUIContent channelHeader = EditorGUIUtility.TextContent("Channels|Which render target color channels to show");

			public static readonly GUIContent levelsHeader = EditorGUIUtility.TextContent("Levels|Render target display black/white intensity levels");

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

		private const string kFloatFormat = "g2";

		private const string kFloatDetailedFormat = "g7";

		private const float kPropertyFieldHeight = 16f;

		private const float kPropertyFieldIndent = 15f;

		private const float kPropertyNameWidth = 0.4f;

		private const float kPropertyFlagsWidth = 0.1f;

		private const float kPropertyValueWidth = 0.5f;

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
			"Plugin Event",
			"Draw Mesh (instanced)"
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

		private Vector2 m_ScrollViewShaderProps = Vector2.zero;

		private ShowAdditionalInfo m_AdditionalInfo;

		private GUIContent[] m_AdditionalInfoGuiContents = (from m in Enum.GetNames(typeof(ShowAdditionalInfo))
		select new GUIContent(m)).ToArray<GUIContent>();

		private static List<FrameDebuggerWindow> s_FrameDebuggers = new List<FrameDebuggerWindow>();

		private AttachProfilerUI m_AttachProfilerUI = new AttachProfilerUI();

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
				frameDebuggerWindow.titleContent = EditorGUIUtility.TextContent("Frame Debug");
			}
			return frameDebuggerWindow;
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
				GameObject frameEventGameObject = FrameDebuggerUtility.GetFrameEventGameObject(newLimit - 1);
				if (frameEventGameObject != null)
				{
					EditorGUIUtility.PingObject(frameEventGameObject);
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
			if (FrameDebuggerUtility.IsLocalEnabled())
			{
				EditorApplication.SetSceneRepaintDirty();
			}
			FrameDebuggerUtility.SetEnabled(false, FrameDebuggerUtility.GetRemotePlayerGUID());
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
			if (FrameDebuggerUtility.IsLocalEnabled() || FrameDebuggerUtility.IsRemoteEnabled())
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
			bool flag = FrameDebuggerUtility.IsLocalEnabled() || FrameDebuggerUtility.IsRemoteEnabled();
			bool flag2 = !flag && this.m_AttachProfilerUI.IsEditor();
			if (flag2 && !FrameDebuggerUtility.locallySupported)
			{
				return;
			}
			if (flag2 && EditorApplication.isPlaying && !EditorApplication.isPaused)
			{
				EditorApplication.isPaused = true;
			}
			if (!flag)
			{
				FrameDebuggerUtility.SetEnabled(true, ProfilerDriver.connectedProfiler);
			}
			else
			{
				FrameDebuggerUtility.SetEnabled(false, FrameDebuggerUtility.GetRemotePlayerGUID());
			}
			if (FrameDebuggerUtility.IsLocalEnabled())
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
			bool flag = !this.m_AttachProfilerUI.IsEditor() || FrameDebuggerUtility.locallySupported;
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			using (new EditorGUI.DisabledScope(!flag))
			{
				GUILayout.Toggle(FrameDebuggerUtility.IsLocalEnabled() || FrameDebuggerUtility.IsRemoteEnabled(), FrameDebuggerWindow.styles.recordButton, EditorStyles.toolbarButton, new GUILayoutOption[]
				{
					GUILayout.MinWidth(80f)
				});
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.ClickEnableFrameDebugger();
				result = true;
			}
			this.m_AttachProfilerUI.OnGUILayout(this);
			bool flag2 = FrameDebuggerUtility.IsLocalEnabled() || FrameDebuggerUtility.IsRemoteEnabled();
			if (flag2 && ProfilerDriver.connectedProfiler != FrameDebuggerUtility.GetRemotePlayerGUID())
			{
				FrameDebuggerUtility.SetEnabled(false, FrameDebuggerUtility.GetRemotePlayerGUID());
				FrameDebuggerUtility.SetEnabled(true, ProfilerDriver.connectedProfiler);
			}
			GUI.enabled = flag2;
			EditorGUI.BeginChangeCheck();
			int num;
			using (new EditorGUI.DisabledScope(FrameDebuggerUtility.count <= 1))
			{
				num = EditorGUILayout.IntSlider(FrameDebuggerUtility.limit, 1, FrameDebuggerUtility.count, new GUILayoutOption[0]);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.ChangeFrameEventLimit(num);
			}
			GUILayout.Label(" of " + FrameDebuggerUtility.count, EditorStyles.miniLabel, new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(num <= 1))
			{
				if (GUILayout.Button(FrameDebuggerWindow.styles.prevFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					this.ChangeFrameEventLimit(num - 1);
				}
			}
			using (new EditorGUI.DisabledScope(num >= FrameDebuggerUtility.count))
			{
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
			}
			GUILayout.EndHorizontal();
			return result;
		}

		private void DrawMeshPreview(FrameDebuggerEventData curEventData, Rect previewRect, Rect meshInfoRect, Mesh mesh, int meshSubset)
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
				this.m_WireMaterial = ModelInspector.CreateWireframeMaterial();
			}
			this.m_PreviewUtility.BeginPreview(previewRect, "preBackground");
			ModelInspector.RenderMeshPreview(mesh, this.m_PreviewUtility, this.m_Material, this.m_WireMaterial, this.m_PreviewDir, meshSubset);
			this.m_PreviewUtility.EndAndDrawPreview(previewRect);
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
				curEventData.vertexCount,
				" verts, ",
				curEventData.indexCount,
				" indices"
			});
			if (curEventData.instanceCount > 1)
			{
				string text3 = text2;
				text2 = string.Concat(new object[]
				{
					text3,
					", ",
					curEventData.instanceCount,
					" instances"
				});
			}
			EditorGUI.DropShadowLabel(meshInfoRect, text2);
		}

		private bool DrawEventMesh(FrameDebuggerEventData curEventData)
		{
			Mesh mesh = curEventData.mesh;
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
			GameObject frameEventGameObject = FrameDebuggerUtility.GetFrameEventGameObject(curEventData.frameEventIndex);
			Rect rect2 = rect;
			rect2.yMin = rect2.yMax - EditorGUIUtility.singleLineHeight * 2f;
			Rect position = rect2;
			rect2.xMin = rect2.center.x;
			position.xMax = position.center.x;
			if (Event.current.type == EventType.MouseDown)
			{
				if (rect2.Contains(Event.current.mousePosition))
				{
					EditorGUIUtility.PingObject(mesh);
					Event.current.Use();
				}
				if (frameEventGameObject != null && position.Contains(Event.current.mousePosition))
				{
					EditorGUIUtility.PingObject(frameEventGameObject.GetInstanceID());
					Event.current.Use();
				}
			}
			this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, rect);
			if (Event.current.type == EventType.Repaint)
			{
				int meshSubset = curEventData.meshSubset;
				this.DrawMeshPreview(curEventData, rect, rect2, mesh, meshSubset);
				if (frameEventGameObject != null)
				{
					EditorGUI.DropShadowLabel(position, frameEventGameObject.name);
				}
			}
			return true;
		}

		private void DrawRenderTargetControls(FrameDebuggerEventData cur)
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
			bool flag2;
			using (new EditorGUI.DisabledScope(num <= 1))
			{
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
				flag2 = (num2 != this.m_RTIndex);
				this.m_RTIndex = num2;
				this.m_RTIndex = EditorGUILayout.Popup(this.m_RTIndex, array, EditorStyles.toolbarPopup, new GUILayoutOption[]
				{
					GUILayout.Width(70f)
				});
			}
			GUILayout.Space(10f);
			using (new EditorGUI.DisabledScope(disabled))
			{
				GUILayout.Label(FrameDebuggerWindow.Styles.channelHeader, EditorStyles.miniLabel, new GUILayoutOption[0]);
				this.m_RTChannel = GUILayout.Toolbar(this.m_RTChannel, FrameDebuggerWindow.Styles.channelLabels, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			}
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
				else if (this.m_RTChannel == 2)
				{
					channels.y = 1f;
				}
				else if (this.m_RTChannel == 3)
				{
					channels.z = 1f;
				}
				else if (this.m_RTChannel == 4)
				{
					channels.w = 1f;
				}
				else
				{
					channels = Vector4.one;
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
			FrameDebuggerEventData frameDebuggerEventData;
			bool frameEventData = FrameDebuggerUtility.GetFrameEventData(num, out frameDebuggerEventData);
			if (frameEventData)
			{
				this.DrawRenderTargetControls(frameDebuggerEventData);
			}
			GUILayout.Label(string.Format("Event #{0}: {1}", num + 1, FrameDebuggerWindow.s_FrameEventTypeNames[(int)frameDebuggerEvent.type]), EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (FrameDebuggerUtility.IsRemoteEnabled() && FrameDebuggerUtility.receivingRemoteFrameEventData)
			{
				GUILayout.Label("Receiving frame event data...", new GUILayoutOption[0]);
			}
			else if (frameEventData && (frameDebuggerEventData.vertexCount > 0 || frameDebuggerEventData.indexCount > 0))
			{
				Shader shader = frameDebuggerEventData.shader;
				int shaderPassIndex = frameDebuggerEventData.shaderPassIndex;
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (GUILayout.Button(string.Concat(new object[]
				{
					"Shader: ",
					frameDebuggerEventData.shaderName,
					" pass #",
					shaderPassIndex
				}), GUI.skin.label, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					EditorGUIUtility.PingObject(shader);
					Event.current.Use();
				}
				GUILayout.Label(frameDebuggerEventData.shaderKeywords, EditorStyles.miniLabel, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				this.DrawStates(frameDebuggerEventData);
				GUILayout.Space(15f);
				this.m_AdditionalInfo = (ShowAdditionalInfo)GUILayout.Toolbar((int)this.m_AdditionalInfo, this.m_AdditionalInfoGuiContents, new GUILayoutOption[0]);
				ShowAdditionalInfo additionalInfo = this.m_AdditionalInfo;
				if (additionalInfo != ShowAdditionalInfo.Preview)
				{
					if (additionalInfo == ShowAdditionalInfo.ShaderProperties)
					{
						if (frameEventData)
						{
							this.DrawShaderProperties(frameDebuggerEventData.shaderProperties);
						}
					}
				}
				else if (frameEventData)
				{
					if (!this.DrawEventMesh(frameDebuggerEventData))
					{
						GUILayout.Label("Vertices: " + frameDebuggerEventData.vertexCount, new GUILayoutOption[0]);
						GUILayout.Label("Indices: " + frameDebuggerEventData.indexCount, new GUILayoutOption[0]);
					}
				}
			}
			GUILayout.EndArea();
		}

		private void DrawShaderPropertyFlags(Rect flagsRect, int flags)
		{
			string text = string.Empty;
			if ((flags & 2) != 0)
			{
				text += 'v';
			}
			if ((flags & 4) != 0)
			{
				text += 'f';
			}
			if ((flags & 8) != 0)
			{
				text += 'g';
			}
			if ((flags & 16) != 0)
			{
				text += 'h';
			}
			if ((flags & 32) != 0)
			{
				text += 'd';
			}
			GUI.Label(flagsRect, text, EditorStyles.miniLabel);
		}

		private void ShaderPropertyCopyValueMenu(Rect valueRect, object value)
		{
			Event current = Event.current;
			if (current.type == EventType.ContextClick && valueRect.Contains(current.mousePosition))
			{
				current.Use();
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Copy value"), false, delegate
				{
					string systemCopyBuffer = string.Empty;
					if (value is Vector4)
					{
						systemCopyBuffer = ((Vector4)value).ToString("g7");
					}
					else if (value is Matrix4x4)
					{
						systemCopyBuffer = ((Matrix4x4)value).ToString("g7");
					}
					else if (value is float)
					{
						systemCopyBuffer = ((float)value).ToString("g7");
					}
					else
					{
						systemCopyBuffer = value.ToString();
					}
					EditorGUIUtility.systemCopyBuffer = systemCopyBuffer;
				});
				genericMenu.ShowAsContext();
			}
		}

		private void OnGUIShaderPropFloat(Rect nameRect, Rect flagsRect, Rect valueRect, ShaderFloatInfo t)
		{
			GUI.Label(nameRect, t.name, EditorStyles.miniLabel);
			this.DrawShaderPropertyFlags(flagsRect, t.flags);
			GUI.Label(valueRect, t.value.ToString("g2"), EditorStyles.miniLabel);
			this.ShaderPropertyCopyValueMenu(valueRect, t.value);
		}

		private void OnGUIShaderPropVector4(Rect nameRect, Rect flagsRect, Rect valueRect, ShaderVectorInfo t)
		{
			GUI.Label(nameRect, t.name, EditorStyles.miniLabel);
			this.DrawShaderPropertyFlags(flagsRect, t.flags);
			GUI.Label(valueRect, t.value.ToString("g2"), EditorStyles.miniLabel);
			this.ShaderPropertyCopyValueMenu(valueRect, t.value);
		}

		private void OnGUIShaderPropMatrix(Rect nameRect, Rect flagsRect, Rect valueRect, ShaderMatrixInfo t)
		{
			GUI.Label(nameRect, t.name, EditorStyles.miniLabel);
			this.DrawShaderPropertyFlags(flagsRect, t.flags);
			string text = t.value.ToString("g2");
			GUI.Label(valueRect, text, EditorStyles.miniLabel);
			this.ShaderPropertyCopyValueMenu(valueRect, t.value);
		}

		private void OnGUIShaderPropTexture(Rect nameRect, Rect flagsRect, Rect valueRect, ShaderTextureInfo t)
		{
			GUI.Label(nameRect, t.name, EditorStyles.miniLabel);
			this.DrawShaderPropertyFlags(flagsRect, t.flags);
			if (Event.current.type == EventType.Repaint)
			{
				Rect position = valueRect;
				position.width = position.height;
				Rect position2 = valueRect;
				position2.xMin += position.width;
				if (t.value != null)
				{
					EditorGUI.DrawPreviewTexture(position, t.value);
				}
				GUI.Label(position2, (!(t.value != null)) ? t.textureName : t.value.name);
			}
			else if (Event.current.type == EventType.MouseDown && valueRect.Contains(Event.current.mousePosition))
			{
				EditorGUIUtility.PingObject(t.value);
				Event.current.Use();
			}
		}

		private void GetPropertyFieldRects(int count, float height, out Rect nameRect, out Rect flagsRect, out Rect valueRect)
		{
			Rect rect = GUILayoutUtility.GetRect(1f, height * (float)count);
			rect.height /= (float)count;
			rect.xMin += 15f;
			nameRect = rect;
			nameRect.width *= 0.4f;
			flagsRect = rect;
			flagsRect.width *= 0.1f;
			flagsRect.x += nameRect.width;
			valueRect = rect;
			valueRect.width *= 0.5f;
			valueRect.x += nameRect.width + flagsRect.width;
		}

		private void DrawShaderProperties(ShaderProperties props)
		{
			this.m_ScrollViewShaderProps = GUILayout.BeginScrollView(this.m_ScrollViewShaderProps, new GUILayoutOption[0]);
			if (props.textures.Count<ShaderTextureInfo>() > 0)
			{
				GUILayout.Label("Textures", EditorStyles.boldLabel, new GUILayoutOption[0]);
				Rect nameRect;
				Rect flagsRect;
				Rect valueRect;
				this.GetPropertyFieldRects(props.textures.Count<ShaderTextureInfo>(), 16f, out nameRect, out flagsRect, out valueRect);
				ShaderTextureInfo[] textures = props.textures;
				for (int i = 0; i < textures.Length; i++)
				{
					ShaderTextureInfo t = textures[i];
					this.OnGUIShaderPropTexture(nameRect, flagsRect, valueRect, t);
					nameRect.y += nameRect.height;
					flagsRect.y += flagsRect.height;
					valueRect.y += valueRect.height;
				}
			}
			if (props.floats.Count<ShaderFloatInfo>() > 0)
			{
				GUILayout.Label("Floats", EditorStyles.boldLabel, new GUILayoutOption[0]);
				Rect nameRect;
				Rect flagsRect;
				Rect valueRect;
				this.GetPropertyFieldRects(props.floats.Count<ShaderFloatInfo>(), 16f, out nameRect, out flagsRect, out valueRect);
				ShaderFloatInfo[] floats = props.floats;
				for (int j = 0; j < floats.Length; j++)
				{
					ShaderFloatInfo t2 = floats[j];
					this.OnGUIShaderPropFloat(nameRect, flagsRect, valueRect, t2);
					nameRect.y += nameRect.height;
					flagsRect.y += flagsRect.height;
					valueRect.y += valueRect.height;
				}
			}
			if (props.vectors.Count<ShaderVectorInfo>() > 0)
			{
				GUILayout.Label("Vectors", EditorStyles.boldLabel, new GUILayoutOption[0]);
				Rect nameRect;
				Rect flagsRect;
				Rect valueRect;
				this.GetPropertyFieldRects(props.vectors.Count<ShaderVectorInfo>(), 16f, out nameRect, out flagsRect, out valueRect);
				ShaderVectorInfo[] vectors = props.vectors;
				for (int k = 0; k < vectors.Length; k++)
				{
					ShaderVectorInfo t3 = vectors[k];
					this.OnGUIShaderPropVector4(nameRect, flagsRect, valueRect, t3);
					nameRect.y += nameRect.height;
					flagsRect.y += flagsRect.height;
					valueRect.y += valueRect.height;
				}
			}
			if (props.matrices.Count<ShaderMatrixInfo>() > 0)
			{
				GUILayout.Label("Matrices", EditorStyles.boldLabel, new GUILayoutOption[0]);
				Rect nameRect;
				Rect flagsRect;
				Rect valueRect;
				this.GetPropertyFieldRects(props.matrices.Count<ShaderMatrixInfo>(), 48f, out nameRect, out flagsRect, out valueRect);
				ShaderMatrixInfo[] matrices = props.matrices;
				for (int l = 0; l < matrices.Length; l++)
				{
					ShaderMatrixInfo t4 = matrices[l];
					this.OnGUIShaderPropMatrix(nameRect, flagsRect, valueRect, t4);
					nameRect.y += nameRect.height;
					flagsRect.y += flagsRect.height;
					valueRect.y += valueRect.height;
				}
			}
			GUILayout.EndScrollView();
		}

		private void DrawStates(FrameDebuggerEventData curEventData)
		{
			FrameDebuggerBlendState blendState = curEventData.blendState;
			FrameDebuggerRasterState rasterState = curEventData.rasterState;
			FrameDebuggerDepthState depthState = curEventData.depthState;
			string text = string.Empty;
			if (blendState.renderTargetWriteMask == 0u)
			{
				text = "0";
			}
			else
			{
				if ((blendState.renderTargetWriteMask & 2u) != 0u)
				{
					text += "R";
				}
				if ((blendState.renderTargetWriteMask & 4u) != 0u)
				{
					text += "G";
				}
				if ((blendState.renderTargetWriteMask & 8u) != 0u)
				{
					text += "B";
				}
				if ((blendState.renderTargetWriteMask & 1u) != 0u)
				{
					text += "A";
				}
			}
			GUILayout.Label(string.Format("Blend {0} {1}, {2} {3} ColorMask {4}", new object[]
			{
				blendState.srcBlend,
				blendState.dstBlend,
				blendState.srcBlendAlpha,
				blendState.dstBlendAlpha,
				text
			}), EditorStyles.miniLabel, new GUILayoutOption[0]);
			GUILayout.Label(string.Format("ZTest {0} ZWrite {1} Cull {2} Offset {3}, {4}", new object[]
			{
				depthState.depthFunc,
				(depthState.depthWrite != 0) ? "On" : "Off",
				rasterState.cullMode,
				rasterState.slopeScaledDepthBias,
				rasterState.depthBias
			}), EditorStyles.miniLabel, new GUILayoutOption[0]);
		}

		internal void OnGUI()
		{
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
			if (!FrameDebuggerUtility.IsLocalEnabled() && !FrameDebuggerUtility.IsRemoteEnabled() && this.m_AttachProfilerUI.IsEditor())
			{
				GUI.enabled = true;
				if (!FrameDebuggerUtility.locallySupported)
				{
					EditorGUILayout.HelpBox("Frame Debugger requires multi-threaded renderer. Usually Unity uses that; if it does not, try starting with -force-gfx-mt command line argument.", MessageType.Warning, true);
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
