using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

			public GUIContent normalDirection = EditorGUIUtility.TextContent("Normal Direction|Value between 0.0 and 1.0. If 1.0 is used, normals will point towards camera. If 0.0 is used, normals will point out in the corner direction of the particle.");

			public GUIContent sortingLayer = EditorGUIUtility.TextContent("Sorting Layer");

			public GUIContent sortingOrder = EditorGUIUtility.TextContent("Order in Layer");

			public GUIContent space = EditorGUIUtility.TextContent("Billboard Alignment|Specifies if the particles will face the camera, align to world axes, or stay local to the system's transform.");

			public GUIContent pivot = EditorGUIUtility.TextContent("Pivot|Applies an offset to the pivot of particles, as a multiplier of its size.");

			public GUIContent visualizePivot = EditorGUIUtility.TextContent("Visualize Pivot|Render the pivot positions of the particles.");

			public GUIContent useCustomVertexStreams = EditorGUIUtility.TextContent("Use Custom Vertex Streams|Choose wheher to send custom particle data to the shader.");

			public GUIContent streams = EditorGUIUtility.TextContent("Vertex Streams|Configure the list of vertex attributes supplied to the vertex shader.");

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

			public string[] vertexStreams = new string[]
			{
				"Position",
				"Normal",
				"Tangent",
				"Color",
				"UV",
				"UV2BlendAndFrame",
				"CenterAndVertexID",
				"Size",
				"Rotation",
				"Velocity",
				"Lifetime",
				"Custom1",
				"Custom2",
				"Random"
			};

			public bool[] vertexStreamIsTexCoord = new bool[]
			{
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true
			};

			public string[] vertexStreamDataTypes = new string[]
			{
				"float3",
				"float3",
				"float4",
				"fixed4",
				"float2",
				"float4",
				"float4",
				"float3",
				"float3",
				"float3",
				"float2",
				"float4",
				"float4",
				"float4"
			};
		}

		private class StreamCallbackData
		{
			public SerializedProperty streamProp;

			public string text;

			public StreamCallbackData(SerializedProperty prop, string t)
			{
				this.streamProp = prop;
				this.text = t;
			}
		}

		private const int k_MaxNumMeshes = 4;

		private SerializedProperty m_CastShadows;

		private SerializedProperty m_ReceiveShadows;

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

		private SerializedProperty m_VertexStreamMask;

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
				this.m_VertexStreamMask = base.GetProperty0("m_VertexStreamMask");
				RendererModuleUI.s_VisualizePivot = EditorPrefs.GetBool("VisualizePivot", false);
			}
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (RendererModuleUI.s_Texts == null)
			{
				RendererModuleUI.s_Texts = new RendererModuleUI.Texts();
			}
			RendererModuleUI.RenderMode intValue = (RendererModuleUI.RenderMode)this.m_RenderMode.intValue;
			RendererModuleUI.RenderMode renderMode = (RendererModuleUI.RenderMode)ModuleUI.GUIPopup(RendererModuleUI.s_Texts.renderMode, this.m_RenderMode, RendererModuleUI.s_Texts.particleTypes, new GUILayoutOption[0]);
			if (renderMode == RendererModuleUI.RenderMode.Mesh)
			{
				EditorGUI.indentLevel++;
				this.DoListOfMeshesGUI();
				EditorGUI.indentLevel--;
				if (intValue != RendererModuleUI.RenderMode.Mesh && this.m_Meshes[0].objectReferenceInstanceIDValue == 0)
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
			if (this.m_ParticleSystemUI.m_ParticleSystem.trails.enabled && this.m_TrailMaterial != null)
			{
				ModuleUI.GUIObject(RendererModuleUI.s_Texts.trailMaterial, this.m_TrailMaterial, new GUILayoutOption[0]);
			}
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
					if (this.m_ParticleSystemUI.m_ParticleSystem.shape.alignToDirection)
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
				EditorGUILayout.SortingLayerField(RendererModuleUI.s_Texts.sortingLayer, this.m_SortingLayerID, ParticleSystemStyles.Get().popup, ParticleSystemStyles.Get().label);
				ModuleUI.GUIInt(RendererModuleUI.s_Texts.sortingOrder, this.m_SortingOrder, new GUILayoutOption[0]);
			}
			this.m_Probes.OnGUI(null, s.GetComponent<Renderer>(), true);
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
			RendererModuleUI.StreamCallbackData data = (RendererModuleUI.StreamCallbackData)obj;
			data.streamProp.intValue |= 1 << Array.FindIndex<string>(RendererModuleUI.s_Texts.vertexStreams, (string item) => item == data.text);
		}

		private void DoVertexStreamsGUI(RendererModuleUI.RenderMode renderMode)
		{
			Rect controlRect = ModuleUI.GetControlRect(13, new GUILayoutOption[0]);
			GUI.Label(controlRect, RendererModuleUI.s_Texts.streams, ParticleSystemStyles.Get().label);
			int num = 0;
			for (int i = 0; i < RendererModuleUI.s_Texts.vertexStreams.Length; i++)
			{
				if ((this.m_VertexStreamMask.intValue & 1 << i) != 0)
				{
					bool flag = this.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner is ParticleSystemInspector;
					string text = (!flag) ? "TEX" : "TEXCOORD";
					Rect position = new Rect(controlRect.x + EditorGUIUtility.labelWidth, controlRect.y, controlRect.width, controlRect.height);
					if (RendererModuleUI.s_Texts.vertexStreamIsTexCoord[i])
					{
						GUI.Label(position, string.Concat(new object[]
						{
							RendererModuleUI.s_Texts.vertexStreams[i],
							" (",
							text,
							num++,
							", ",
							RendererModuleUI.s_Texts.vertexStreamDataTypes[i],
							")"
						}), ParticleSystemStyles.Get().label);
					}
					else
					{
						GUI.Label(position, RendererModuleUI.s_Texts.vertexStreams[i] + " (" + RendererModuleUI.s_Texts.vertexStreamDataTypes[i] + ")", ParticleSystemStyles.Get().label);
					}
					position.x = controlRect.xMax - 12f;
					controlRect = ModuleUI.GetControlRect(13, new GUILayoutOption[0]);
					if (i == 0)
					{
						if (this.m_VertexStreamMask.intValue != (1 << RendererModuleUI.s_Texts.vertexStreams.Length) - 1)
						{
							position.x -= 2f;
							position.y -= 2f;
							if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, "OL Plus"))
							{
								List<GUIContent> list = new List<GUIContent>();
								for (int j = 0; j < RendererModuleUI.s_Texts.vertexStreams.Length; j++)
								{
									if ((this.m_VertexStreamMask.intValue & 1 << j) == 0)
									{
										list.Add(new GUIContent(RendererModuleUI.s_Texts.vertexStreams[j]));
									}
								}
								GenericMenu genericMenu = new GenericMenu();
								for (int k = 0; k < list.Count; k++)
								{
									GenericMenu arg_292_0 = genericMenu;
									GUIContent arg_292_1 = list[k];
									bool arg_292_2 = false;
									if (RendererModuleUI.<>f__mg$cache0 == null)
									{
										RendererModuleUI.<>f__mg$cache0 = new GenericMenu.MenuFunction2(RendererModuleUI.SelectVertexStreamCallback);
									}
									arg_292_0.AddItem(arg_292_1, arg_292_2, RendererModuleUI.<>f__mg$cache0, new RendererModuleUI.StreamCallbackData(this.m_VertexStreamMask, list[k].text));
								}
								genericMenu.ShowAsContext();
								Event.current.Use();
							}
						}
					}
					else if (ModuleUI.MinusButton(position))
					{
						this.m_VertexStreamMask.intValue &= ~(1 << i);
					}
				}
			}
			string text2 = "";
			if (this.m_Material != null)
			{
				Material material = this.m_Material.objectReferenceValue as Material;
				ParticleSystemVertexStreams particleSystemVertexStreams = this.m_ParticleSystemUI.m_ParticleSystem.CheckVertexStreamsMatchShader((ParticleSystemVertexStreams)this.m_VertexStreamMask.intValue, material);
				if (particleSystemVertexStreams != ParticleSystemVertexStreams.None)
				{
					text2 += "Vertex streams do not match the shader inputs. Particle systems may not render correctly. Ensure your streams match and are used by the shader.";
					if ((particleSystemVertexStreams & ParticleSystemVertexStreams.Tangent) != ParticleSystemVertexStreams.None)
					{
						text2 += "\n- TANGENT stream does not match.";
					}
					if ((particleSystemVertexStreams & ParticleSystemVertexStreams.Color) != ParticleSystemVertexStreams.None)
					{
						text2 += "\n- COLOR stream does not match.";
					}
					if ((particleSystemVertexStreams & ParticleSystemVertexStreams.UV) != ParticleSystemVertexStreams.None)
					{
						text2 += "\n- TEXCOORD streams do not match.";
					}
				}
			}
			int maxTexCoordStreams = this.m_ParticleSystemUI.m_ParticleSystem.GetMaxTexCoordStreams();
			if (num > maxTexCoordStreams)
			{
				if (text2 != "")
				{
					text2 += "\n\n";
				}
				string text3 = text2;
				text2 = string.Concat(new object[]
				{
					text3,
					"Only ",
					maxTexCoordStreams,
					" TEXCOORD streams are supported."
				});
			}
			if (renderMode == RendererModuleUI.RenderMode.Mesh)
			{
				ParticleSystemRenderer component = this.m_ParticleSystemUI.m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
				Mesh[] array = new Mesh[4];
				int meshes = component.GetMeshes(array);
				for (int l = 0; l < meshes; l++)
				{
					if (array[l].HasChannel(Mesh.InternalShaderChannel.TexCoord2))
					{
						if (text2 != "")
						{
							text2 += "\n\n";
						}
						text2 += "Meshes may only use a maximum of 2 input UV streams.";
					}
				}
			}
			if (text2 != "")
			{
				GUIContent gUIContent = EditorGUIUtility.TextContent(text2);
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Error, true);
			}
		}

		public bool IsMeshEmitter()
		{
			return this.m_RenderMode != null && this.m_RenderMode.intValue == 4;
		}

		[DrawGizmo(GizmoType.Active)]
		private static void RenderPivots(ParticleSystem system, GizmoType gizmoType)
		{
			ParticleSystemRenderer component = system.GetComponent<ParticleSystemRenderer>();
			if (!(component == null))
			{
				if (component.enabled)
				{
					if (RendererModuleUI.s_VisualizePivot)
					{
						ParticleSystem.Particle[] array = new ParticleSystem.Particle[system.particleCount];
						int particles = system.GetParticles(array);
						Color color = Gizmos.color;
						Gizmos.color = Color.green;
						Matrix4x4 matrix = Matrix4x4.identity;
						if (system.main.simulationSpace == ParticleSystemSimulationSpace.Local)
						{
							matrix = system.GetLocalToWorldMatrix();
						}
						Matrix4x4 matrix2 = Gizmos.matrix;
						Gizmos.matrix = matrix;
						for (int i = 0; i < particles; i++)
						{
							ParticleSystem.Particle particle = array[i];
							Vector3 currentSize3D = particle.GetCurrentSize3D(system);
							Gizmos.DrawWireSphere(particle.position, Math.Max(currentSize3D.x, Math.Max(currentSize3D.y, currentSize3D.z)) * 0.05f);
						}
						Gizmos.color = color;
						Gizmos.matrix = matrix2;
					}
				}
			}
		}
	}
}
