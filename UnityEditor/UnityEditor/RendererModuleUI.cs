using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class RendererModuleUI : ModuleUI
	{
		private enum RenderMode
		{
			Billboard,
			Stretch3D,
			BillboardFixedHorizontal,
			BillboardFixedVertical,
			Mesh,
			None
		}

		private class Texts
		{
			public GUIContent renderMode = EditorGUIUtility.TextContent("Render Mode|Defines the render mode of the particle renderer.");

			public GUIContent material = EditorGUIUtility.TextContent("Material|Defines the material used to render particles.");

			public GUIContent trailMaterial = EditorGUIUtility.TextContent("Trail Material|Defines the material used to render particle trails.");

			public GUIContent mesh = EditorGUIUtility.TextContent("Mesh|Defines the mesh that will be rendered as particle.");

			public GUIContent minParticleSize = EditorGUIUtility.TextContent("Min Particle Size|How small is a particle allowed to be on screen at least? 1 is entire viewport. 0.5 is half viewport.");

			public GUIContent maxParticleSize = EditorGUIUtility.TextContent("Max Particle Size|How large is a particle allowed to be on screen at most? 1 is entire viewport. 0.5 is half viewport.");

			public GUIContent cameraSpeedScale = EditorGUIUtility.TextContent("Camera Scale|How much the camera speed is factored in when determining particle stretching.");

			public GUIContent speedScale = EditorGUIUtility.TextContent("Speed Scale|Defines the length of the particle compared to its speed.");

			public GUIContent lengthScale = EditorGUIUtility.TextContent("Length Scale|Defines the length of the particle compared to its width.");

			public GUIContent sortingFudge = EditorGUIUtility.TextContent("Sorting Fudge|Lower the number and most likely these particles will appear in front of other transparent objects, including other particles.");

			public GUIContent sortMode = EditorGUIUtility.TextContent("Sort Mode|The draw order of particles can be sorted by distance, oldest in front, or youngest in front.");

			public GUIContent rotation = EditorGUIUtility.TextContent("Rotation|Set whether the rotation of the particles is defined in Screen or World space.");

			public GUIContent castShadows = EditorGUIUtility.TextContent("Cast Shadows|Only opaque materials cast shadows");

			public GUIContent receiveShadows = EditorGUIUtility.TextContent("Receive Shadows|Only opaque materials receive shadows");

			public GUIContent motionVectors = EditorGUIUtility.TextContent("Motion Vectors|Specifies whether the Particle System renders 'Per Object Motion', 'Camera Motion', or 'No Motion' vectors to the Camera Motion Vector Texture. Note that there is no built-in support for Per-Particle Motion.");

			public GUIContent normalDirection = EditorGUIUtility.TextContent("Normal Direction|Value between 0.0 and 1.0. If 1.0 is used, normals will point towards camera. If 0.0 is used, normals will point out in the corner direction of the particle.");

			public GUIContent sortingLayer = EditorGUIUtility.TextContent("Sorting Layer");

			public GUIContent sortingOrder = EditorGUIUtility.TextContent("Order in Layer");

			public GUIContent space = EditorGUIUtility.TextContent("Billboard Alignment|Specifies if the particles will face the camera, align to world axes, or stay local to the system's transform.");

			public GUIContent pivot = EditorGUIUtility.TextContent("Pivot|Applies an offset to the pivot of particles, as a multiplier of its size.");

			public GUIContent visualizePivot = EditorGUIUtility.TextContent("Visualize Pivot|Render the pivot positions of the particles.");

			public GUIContent useCustomVertexStreams = EditorGUIUtility.TextContent("Custom Vertex Streams|Choose whether to send custom particle data to the shader.");

			public string[] particleTypes = new string[]
			{
				"Billboard",
				"Stretched Billboard",
				"Horizontal Billboard",
				"Vertical Billboard",
				"Mesh",
				"None"
			};

			public string[] sortTypes = new string[]
			{
				"None",
				"By Distance",
				"Oldest in Front",
				"Youngest in Front"
			};

			public string[] spaces = new string[]
			{
				"View",
				"World",
				"Local",
				"Facing"
			};

			public string[] motionVectorOptions = new string[]
			{
				"Camera Motion Only",
				"Per Object Motion",
				"Force No Motion"
			};

			public string[] vertexStreamsMenu = new string[]
			{
				"Position",
				"Normal",
				"Tangent",
				"Color",
				"UV/UV1",
				"UV/UV2",
				"UV/UV3",
				"UV/UV4",
				"UV/AnimBlend",
				"UV/AnimFrame",
				"Center",
				"VertexID",
				"Size/Size.x",
				"Size/Size.xy",
				"Size/Size.xyz",
				"Rotation/Rotation",
				"Rotation/Rotation3D",
				"Rotation/RotationSpeed",
				"Rotation/RotationSpeed3D",
				"Velocity",
				"Speed",
				"Lifetime/AgePercent",
				"Lifetime/InverseStartLifetime",
				"Random/Stable.x",
				"Random/Stable.xy",
				"Random/Stable.xyz",
				"Random/Stable.xyzw",
				"Random/Varying.x",
				"Random/Varying.xy",
				"Random/Varying.xyz",
				"Random/Varying.xyzw",
				"Custom/Custom1.x",
				"Custom/Custom1.xy",
				"Custom/Custom1.xyz",
				"Custom/Custom1.xyzw",
				"Custom/Custom2.x",
				"Custom/Custom2.xy",
				"Custom/Custom2.xyz",
				"Custom/Custom2.xyzw"
			};

			public string[] vertexStreamsPacked = new string[]
			{
				"Position",
				"Normal",
				"Tangent",
				"Color",
				"UV",
				"UV2",
				"UV3",
				"UV4",
				"AnimBlend",
				"AnimFrame",
				"Center",
				"VertexID",
				"Size",
				"Size.xy",
				"Size.xyz",
				"Rotation",
				"Rotation3D",
				"RotationSpeed",
				"RotationSpeed3D",
				"Velocity",
				"Speed",
				"AgePercent",
				"InverseStartLifetime",
				"StableRandom.x",
				"StableRandom.xy",
				"StableRandom.xyz",
				"StableRandom.xyzw",
				"VariableRandom.x",
				"VariableRandom.xy",
				"VariableRandom.xyz",
				"VariableRandom.xyzw",
				"Custom1.x",
				"Custom1.xy",
				"Custom1.xyz",
				"Custom1.xyzw",
				"Custom2.x",
				"Custom2.xy",
				"Custom2.xyz",
				"Custom2.xyzw"
			};

			public string[] vertexStreamPackedTypes = new string[]
			{
				"POSITION.xyz",
				"NORMAL.xyz",
				"TANGENT.xyzw",
				"COLOR.xyzw"
			};

			public int[] vertexStreamTexCoordChannels = new int[]
			{
				0,
				0,
				0,
				0,
				2,
				2,
				2,
				2,
				1,
				1,
				3,
				1,
				1,
				2,
				3,
				1,
				3,
				1,
				3,
				3,
				1,
				1,
				1,
				1,
				2,
				3,
				4,
				1,
				2,
				3,
				4,
				1,
				2,
				3,
				4,
				1,
				2,
				3,
				4
			};

			public string channels = "xyzw|xyz";
		}

		private class StreamCallbackData
		{
			public SerializedProperty streamProp;

			public int stream;

			public StreamCallbackData(SerializedProperty prop, int s)
			{
				this.streamProp = prop;
				this.stream = s;
			}
		}

		private const int k_MaxNumMeshes = 4;

		private SerializedProperty m_CastShadows;

		private SerializedProperty m_ReceiveShadows;

		private SerializedProperty m_MotionVectors;

		private SerializedProperty m_Material;

		private SerializedProperty m_TrailMaterial;

		private SerializedProperty m_SortingOrder;

		private SerializedProperty m_SortingLayerID;

		private SerializedProperty m_RenderMode;

		private SerializedProperty[] m_Meshes = new SerializedProperty[4];

		private SerializedProperty[] m_ShownMeshes;

		private SerializedProperty m_MinParticleSize;

		private SerializedProperty m_MaxParticleSize;

		private SerializedProperty m_CameraVelocityScale;

		private SerializedProperty m_VelocityScale;

		private SerializedProperty m_LengthScale;

		private SerializedProperty m_SortMode;

		private SerializedProperty m_SortingFudge;

		private SerializedProperty m_NormalDirection;

		private RendererEditorBase.Probes m_Probes;

		private SerializedProperty m_RenderAlignment;

		private SerializedProperty m_Pivot;

		private SerializedProperty m_UseCustomVertexStreams;

		private SerializedProperty m_VertexStreams;

		private ReorderableList m_VertexStreamsList;

		private int m_NumTexCoords;

		private int m_TexCoordChannelIndex;

		private bool m_HasTangent;

		private bool m_HasColor;

		private static bool s_VisualizePivot = false;

		private static RendererModuleUI.Texts s_Texts;

		[CompilerGenerated]
		private static GenericMenu.MenuFunction2 <>f__mg$cache0;

		public RendererModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ParticleSystemRenderer", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
		{
			this.m_ToolTip = "Specifies how the particles are rendered.";
		}

		protected override void Init()
		{
			if (this.m_CastShadows == null)
			{
				this.m_CastShadows = base.GetProperty0("m_CastShadows");
				this.m_ReceiveShadows = base.GetProperty0("m_ReceiveShadows");
				this.m_MotionVectors = base.GetProperty0("m_MotionVectors");
				this.m_Material = base.GetProperty0("m_Materials.Array.data[0]");
				this.m_TrailMaterial = base.GetProperty0("m_Materials.Array.data[1]");
				this.m_SortingOrder = base.GetProperty0("m_SortingOrder");
				this.m_SortingLayerID = base.GetProperty0("m_SortingLayerID");
				this.m_RenderMode = base.GetProperty0("m_RenderMode");
				this.m_MinParticleSize = base.GetProperty0("m_MinParticleSize");
				this.m_MaxParticleSize = base.GetProperty0("m_MaxParticleSize");
				this.m_CameraVelocityScale = base.GetProperty0("m_CameraVelocityScale");
				this.m_VelocityScale = base.GetProperty0("m_VelocityScale");
				this.m_LengthScale = base.GetProperty0("m_LengthScale");
				this.m_SortingFudge = base.GetProperty0("m_SortingFudge");
				this.m_SortMode = base.GetProperty0("m_SortMode");
				this.m_NormalDirection = base.GetProperty0("m_NormalDirection");
				this.m_Probes = new RendererEditorBase.Probes();
				this.m_Probes.Initialize(base.serializedObject);
				this.m_RenderAlignment = base.GetProperty0("m_RenderAlignment");
				this.m_Pivot = base.GetProperty0("m_Pivot");
				this.m_Meshes[0] = base.GetProperty0("m_Mesh");
				this.m_Meshes[1] = base.GetProperty0("m_Mesh1");
				this.m_Meshes[2] = base.GetProperty0("m_Mesh2");
				this.m_Meshes[3] = base.GetProperty0("m_Mesh3");
				List<SerializedProperty> list = new List<SerializedProperty>();
				for (int i = 0; i < this.m_Meshes.Length; i++)
				{
					if (i == 0 || this.m_Meshes[i].objectReferenceValue != null)
					{
						list.Add(this.m_Meshes[i]);
					}
				}
				this.m_ShownMeshes = list.ToArray();
				this.m_UseCustomVertexStreams = base.GetProperty0("m_UseCustomVertexStreams");
				this.m_VertexStreams = base.GetProperty0("m_VertexStreams");
				this.m_VertexStreamsList = new ReorderableList(base.serializedObject, this.m_VertexStreams, true, true, true, true);
				this.m_VertexStreamsList.elementHeight = 16f;
				this.m_VertexStreamsList.headerHeight = 0f;
				this.m_VertexStreamsList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.OnVertexStreamListAddDropdownCallback);
				this.m_VertexStreamsList.onCanRemoveCallback = new ReorderableList.CanRemoveCallbackDelegate(this.OnVertexStreamListCanRemoveCallback);
				this.m_VertexStreamsList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawVertexStreamListElementCallback);
				RendererModuleUI.s_VisualizePivot = EditorPrefs.GetBool("VisualizePivot", false);
			}
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (RendererModuleUI.s_Texts == null)
			{
				RendererModuleUI.s_Texts = new RendererModuleUI.Texts();
			}
			EditorGUI.BeginChangeCheck();
			RendererModuleUI.RenderMode renderMode = (RendererModuleUI.RenderMode)ModuleUI.GUIPopup(RendererModuleUI.s_Texts.renderMode, this.m_RenderMode, RendererModuleUI.s_Texts.particleTypes, new GUILayoutOption[0]);
			bool flag = EditorGUI.EndChangeCheck();
			if (!this.m_RenderMode.hasMultipleDifferentValues)
			{
				if (renderMode == RendererModuleUI.RenderMode.Mesh)
				{
					EditorGUI.indentLevel++;
					this.DoListOfMeshesGUI();
					EditorGUI.indentLevel--;
					if (flag && this.m_Meshes[0].objectReferenceInstanceIDValue == 0 && !this.m_Meshes[0].hasMultipleDifferentValues)
					{
						this.m_Meshes[0].objectReferenceValue = Resources.GetBuiltinResource(typeof(Mesh), "Cube.fbx");
					}
				}
				else if (renderMode == RendererModuleUI.RenderMode.Stretch3D)
				{
					EditorGUI.indentLevel++;
					ModuleUI.GUIFloat(RendererModuleUI.s_Texts.cameraSpeedScale, this.m_CameraVelocityScale, new GUILayoutOption[0]);
					ModuleUI.GUIFloat(RendererModuleUI.s_Texts.speedScale, this.m_VelocityScale, new GUILayoutOption[0]);
					ModuleUI.GUIFloat(RendererModuleUI.s_Texts.lengthScale, this.m_LengthScale, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
				if (renderMode != RendererModuleUI.RenderMode.None)
				{
					if (renderMode != RendererModuleUI.RenderMode.Mesh)
					{
						ModuleUI.GUIFloat(RendererModuleUI.s_Texts.normalDirection, this.m_NormalDirection, new GUILayoutOption[0]);
					}
					if (this.m_Material != null)
					{
						ModuleUI.GUIObject(RendererModuleUI.s_Texts.material, this.m_Material, new GUILayoutOption[0]);
					}
				}
			}
			bool flag2 = this.m_ParticleSystemUI.m_ParticleSystems.FirstOrDefault((ParticleSystem o) => o.trails.enabled) != null;
			if (flag2 && this.m_TrailMaterial != null)
			{
				ModuleUI.GUIObject(RendererModuleUI.s_Texts.trailMaterial, this.m_TrailMaterial, new GUILayoutOption[0]);
			}
			if (!this.m_RenderMode.hasMultipleDifferentValues)
			{
				if (renderMode != RendererModuleUI.RenderMode.None)
				{
					ModuleUI.GUIPopup(RendererModuleUI.s_Texts.sortMode, this.m_SortMode, RendererModuleUI.s_Texts.sortTypes, new GUILayoutOption[0]);
					ModuleUI.GUIFloat(RendererModuleUI.s_Texts.sortingFudge, this.m_SortingFudge, new GUILayoutOption[0]);
					if (renderMode != RendererModuleUI.RenderMode.Mesh)
					{
						ModuleUI.GUIFloat(RendererModuleUI.s_Texts.minParticleSize, this.m_MinParticleSize, new GUILayoutOption[0]);
						ModuleUI.GUIFloat(RendererModuleUI.s_Texts.maxParticleSize, this.m_MaxParticleSize, new GUILayoutOption[0]);
					}
					if (renderMode == RendererModuleUI.RenderMode.Billboard)
					{
						bool flag3 = this.m_ParticleSystemUI.m_ParticleSystems.FirstOrDefault((ParticleSystem o) => o.shape.alignToDirection) != null;
						if (flag3)
						{
							using (new EditorGUI.DisabledScope(true))
							{
								ModuleUI.GUIPopup(RendererModuleUI.s_Texts.space, 0, new string[]
								{
									RendererModuleUI.s_Texts.spaces[2]
								}, new GUILayoutOption[0]);
							}
							GUIContent gUIContent = EditorGUIUtility.TextContent("Using Align to Direction in the Shape Module forces the system to be rendered using Local Billboard Alignment.");
							EditorGUILayout.HelpBox(gUIContent.text, MessageType.Info, true);
						}
						else
						{
							ModuleUI.GUIPopup(RendererModuleUI.s_Texts.space, this.m_RenderAlignment, RendererModuleUI.s_Texts.spaces, new GUILayoutOption[0]);
						}
					}
					ModuleUI.GUIVector3Field(RendererModuleUI.s_Texts.pivot, this.m_Pivot, new GUILayoutOption[0]);
					EditorGUI.BeginChangeCheck();
					RendererModuleUI.s_VisualizePivot = ModuleUI.GUIToggle(RendererModuleUI.s_Texts.visualizePivot, RendererModuleUI.s_VisualizePivot, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						EditorPrefs.SetBool("VisualizePivot", RendererModuleUI.s_VisualizePivot);
					}
					if (ModuleUI.GUIToggle(RendererModuleUI.s_Texts.useCustomVertexStreams, this.m_UseCustomVertexStreams, new GUILayoutOption[0]))
					{
						this.DoVertexStreamsGUI(renderMode);
					}
					EditorGUILayout.Space();
					ModuleUI.GUIPopup(RendererModuleUI.s_Texts.castShadows, this.m_CastShadows, this.m_CastShadows.enumDisplayNames, new GUILayoutOption[0]);
					using (new EditorGUI.DisabledScope(SceneView.IsUsingDeferredRenderingPath()))
					{
						ModuleUI.GUIToggle(RendererModuleUI.s_Texts.receiveShadows, this.m_ReceiveShadows, new GUILayoutOption[0]);
					}
					ModuleUI.GUIPopup(RendererModuleUI.s_Texts.motionVectors, this.m_MotionVectors, RendererModuleUI.s_Texts.motionVectorOptions, new GUILayoutOption[0]);
					EditorGUILayout.SortingLayerField(RendererModuleUI.s_Texts.sortingLayer, this.m_SortingLayerID, ParticleSystemStyles.Get().popup, ParticleSystemStyles.Get().label);
					ModuleUI.GUIInt(RendererModuleUI.s_Texts.sortingOrder, this.m_SortingOrder, new GUILayoutOption[0]);
				}
			}
			List<ParticleSystemRenderer> list = new List<ParticleSystemRenderer>();
			ParticleSystem[] particleSystems = this.m_ParticleSystemUI.m_ParticleSystems;
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem particleSystem = particleSystems[i];
				list.Add(particleSystem.GetComponent<ParticleSystemRenderer>());
			}
			this.m_Probes.OnGUI(list.ToArray(), list.FirstOrDefault<ParticleSystemRenderer>(), true);
		}

		private void DoListOfMeshesGUI()
		{
			base.GUIListOfFloatObjectToggleFields(RendererModuleUI.s_Texts.mesh, this.m_ShownMeshes, null, null, false, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(0f, 13f);
			rect.x = rect.xMax - 24f - 5f;
			rect.width = 12f;
			if (this.m_ShownMeshes.Length > 1)
			{
				if (ModuleUI.MinusButton(rect))
				{
					this.m_ShownMeshes[this.m_ShownMeshes.Length - 1].objectReferenceValue = null;
					List<SerializedProperty> list = new List<SerializedProperty>(this.m_ShownMeshes);
					list.RemoveAt(list.Count - 1);
					this.m_ShownMeshes = list.ToArray();
				}
			}
			if (this.m_ShownMeshes.Length < 4)
			{
				rect.x += 17f;
				if (ModuleUI.PlusButton(rect))
				{
					List<SerializedProperty> list2 = new List<SerializedProperty>(this.m_ShownMeshes);
					list2.Add(this.m_Meshes[list2.Count]);
					this.m_ShownMeshes = list2.ToArray();
				}
			}
		}

		private static void SelectVertexStreamCallback(object obj)
		{
			RendererModuleUI.StreamCallbackData streamCallbackData = (RendererModuleUI.StreamCallbackData)obj;
			int arraySize = streamCallbackData.streamProp.arraySize;
			streamCallbackData.streamProp.InsertArrayElementAtIndex(arraySize);
			SerializedProperty arrayElementAtIndex = streamCallbackData.streamProp.GetArrayElementAtIndex(arraySize);
			arrayElementAtIndex.intValue = streamCallbackData.stream;
		}

		private void DoVertexStreamsGUI(RendererModuleUI.RenderMode renderMode)
		{
			this.m_NumTexCoords = 0;
			this.m_TexCoordChannelIndex = 0;
			this.m_HasTangent = false;
			this.m_HasColor = false;
			this.m_VertexStreamsList.DoLayoutList();
			if (!this.m_ParticleSystemUI.multiEdit)
			{
				string text = "";
				if (this.m_Material != null)
				{
					Material material = this.m_Material.objectReferenceValue as Material;
					int texCoordChannelCount = this.m_NumTexCoords * 4 + this.m_TexCoordChannelIndex;
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = this.m_ParticleSystemUI.m_ParticleSystems[0].CheckVertexStreamsMatchShader(this.m_HasTangent, this.m_HasColor, texCoordChannelCount, material, ref flag, ref flag2, ref flag3);
					if (flag4)
					{
						text += "Vertex streams do not match the shader inputs. Particle systems may not render correctly. Ensure your streams match and are used by the shader.";
						if (flag)
						{
							text += "\n- TANGENT stream does not match.";
						}
						if (flag2)
						{
							text += "\n- COLOR stream does not match.";
						}
						if (flag3)
						{
							text += "\n- TEXCOORD streams do not match.";
						}
					}
				}
				int maxTexCoordStreams = this.m_ParticleSystemUI.m_ParticleSystems[0].GetMaxTexCoordStreams();
				if (this.m_NumTexCoords > maxTexCoordStreams || (this.m_NumTexCoords == maxTexCoordStreams && this.m_TexCoordChannelIndex > 0))
				{
					if (text != "")
					{
						text += "\n\n";
					}
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"Only ",
						maxTexCoordStreams,
						" TEXCOORD streams are supported."
					});
				}
				if (renderMode == RendererModuleUI.RenderMode.Mesh)
				{
					ParticleSystemRenderer component = this.m_ParticleSystemUI.m_ParticleSystems[0].GetComponent<ParticleSystemRenderer>();
					Mesh[] array = new Mesh[4];
					int meshes = component.GetMeshes(array);
					for (int i = 0; i < meshes; i++)
					{
						if (array[i].HasChannel(Mesh.InternalShaderChannel.TexCoord2))
						{
							if (text != "")
							{
								text += "\n\n";
							}
							text += "Meshes may only use a maximum of 2 input UV streams.";
						}
					}
				}
				if (text != "")
				{
					GUIContent gUIContent = EditorGUIUtility.TextContent(text);
					EditorGUILayout.HelpBox(gUIContent.text, MessageType.Error, true);
				}
			}
		}

		private void OnVertexStreamListAddDropdownCallback(Rect rect, ReorderableList list)
		{
			List<int> list2 = new List<int>();
			for (int i = 0; i < RendererModuleUI.s_Texts.vertexStreamsPacked.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < this.m_VertexStreams.arraySize; j++)
				{
					if (this.m_VertexStreams.GetArrayElementAtIndex(j).intValue == i)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list2.Add(i);
				}
			}
			GenericMenu genericMenu = new GenericMenu();
			for (int k = 0; k < list2.Count; k++)
			{
				GenericMenu arg_CC_0 = genericMenu;
				GUIContent arg_CC_1 = new GUIContent(RendererModuleUI.s_Texts.vertexStreamsMenu[list2[k]]);
				bool arg_CC_2 = false;
				if (RendererModuleUI.<>f__mg$cache0 == null)
				{
					RendererModuleUI.<>f__mg$cache0 = new GenericMenu.MenuFunction2(RendererModuleUI.SelectVertexStreamCallback);
				}
				arg_CC_0.AddItem(arg_CC_1, arg_CC_2, RendererModuleUI.<>f__mg$cache0, new RendererModuleUI.StreamCallbackData(this.m_VertexStreams, list2[k]));
			}
			genericMenu.ShowAsContext();
			Event.current.Use();
		}

		private bool OnVertexStreamListCanRemoveCallback(ReorderableList list)
		{
			SerializedProperty arrayElementAtIndex = this.m_VertexStreams.GetArrayElementAtIndex(list.index);
			return RendererModuleUI.s_Texts.vertexStreamsPacked[arrayElementAtIndex.intValue] != "Position";
		}

		private void DrawVertexStreamListElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty arrayElementAtIndex = this.m_VertexStreams.GetArrayElementAtIndex(index);
			int intValue = arrayElementAtIndex.intValue;
			string text = (!base.isWindowView) ? "TEXCOORD" : "TEX";
			int num = RendererModuleUI.s_Texts.vertexStreamTexCoordChannels[intValue];
			if (num != 0)
			{
				int length = (this.m_TexCoordChannelIndex + num <= 4) ? num : (num + 1);
				string text2 = RendererModuleUI.s_Texts.channels.Substring(this.m_TexCoordChannelIndex, length);
				GUI.Label(rect, string.Concat(new object[]
				{
					RendererModuleUI.s_Texts.vertexStreamsPacked[intValue],
					" (",
					text,
					this.m_NumTexCoords,
					".",
					text2,
					")"
				}), ParticleSystemStyles.Get().label);
				this.m_TexCoordChannelIndex += num;
				if (this.m_TexCoordChannelIndex >= 4)
				{
					this.m_TexCoordChannelIndex -= 4;
					this.m_NumTexCoords++;
				}
			}
			else
			{
				GUI.Label(rect, RendererModuleUI.s_Texts.vertexStreamsPacked[intValue] + " (" + RendererModuleUI.s_Texts.vertexStreamPackedTypes[intValue] + ")", ParticleSystemStyles.Get().label);
				if (RendererModuleUI.s_Texts.vertexStreamsPacked[intValue] == "Tangent")
				{
					this.m_HasTangent = true;
				}
				if (RendererModuleUI.s_Texts.vertexStreamsPacked[intValue] == "Color")
				{
					this.m_HasColor = true;
				}
			}
		}

		public override void OnSceneViewGUI()
		{
			if (RendererModuleUI.s_VisualizePivot)
			{
				Color color = Handles.color;
				Handles.color = Color.green;
				Matrix4x4 matrix = Handles.matrix;
				Vector3[] array = new Vector3[6];
				ParticleSystem[] particleSystems = this.m_ParticleSystemUI.m_ParticleSystems;
				for (int i = 0; i < particleSystems.Length; i++)
				{
					ParticleSystem particleSystem = particleSystems[i];
					ParticleSystem.Particle[] array2 = new ParticleSystem.Particle[particleSystem.particleCount];
					int particles = particleSystem.GetParticles(array2);
					Matrix4x4 matrix2 = Matrix4x4.identity;
					if (particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local)
					{
						matrix2 = particleSystem.GetLocalToWorldMatrix();
					}
					Handles.matrix = matrix2;
					for (int j = 0; j < particles; j++)
					{
						ParticleSystem.Particle particle = array2[j];
						Vector3 vector = particle.GetCurrentSize3D(particleSystem) * 0.05f;
						array[0] = particle.position - Vector3.right * vector.x;
						array[1] = particle.position + Vector3.right * vector.x;
						array[2] = particle.position - Vector3.up * vector.y;
						array[3] = particle.position + Vector3.up * vector.y;
						array[4] = particle.position - Vector3.forward * vector.z;
						array[5] = particle.position + Vector3.forward * vector.z;
						Handles.DrawLines(array);
					}
				}
				Handles.color = color;
				Handles.matrix = matrix;
			}
		}
	}
}
