using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	internal class ShapeModuleUI : ModuleUI
	{
		private struct MultiModeParameter
		{
			public enum ValueMode
			{
				Random,
				Loop,
				PingPong,
				BurstSpread
			}

			public SerializedProperty m_Value;

			public SerializedProperty m_Mode;

			public SerializedProperty m_Spread;

			public SerializedMinMaxCurve m_Speed;

			public static ShapeModuleUI.MultiModeParameter GetProperty(ModuleUI ui, string name, GUIContent speed)
			{
				return new ShapeModuleUI.MultiModeParameter
				{
					m_Value = ui.GetProperty(name + ".value"),
					m_Mode = ui.GetProperty(name + ".mode"),
					m_Spread = ui.GetProperty(name + ".spread"),
					m_Speed = new SerializedMinMaxCurve(ui, speed, name + ".speed", ModuleUI.kUseSignedRange),
					m_Speed = 
					{
						m_AllowRandom = false
					}
				};
			}

			public void OnInspectorGUI(ShapeModuleUI.MultiModeTexts text)
			{
				ModuleUI.GUIFloat(text.value, this.m_Value, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				ModuleUI.GUIPopup(text.mode, this.m_Mode, new string[]
				{
					"Random",
					"Loop",
					"Ping-Pong",
					"Burst Spread"
				}, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(text.spread, this.m_Spread, new GUILayoutOption[0]);
				if (!this.m_Mode.hasMultipleDifferentValues)
				{
					ShapeModuleUI.MultiModeParameter.ValueMode intValue = (ShapeModuleUI.MultiModeParameter.ValueMode)this.m_Mode.intValue;
					if (intValue == ShapeModuleUI.MultiModeParameter.ValueMode.Loop || intValue == ShapeModuleUI.MultiModeParameter.ValueMode.PingPong)
					{
						ModuleUI.GUIMinMaxCurve(text.speed, this.m_Speed, new GUILayoutOption[0]);
					}
				}
				EditorGUI.indentLevel--;
			}
		}

		private class Texts
		{
			public GUIContent shape = EditorGUIUtility.TextContent("Shape|Defines the shape of the volume from which particles can be emitted, and the direction of the start velocity.");

			public GUIContent radius = EditorGUIUtility.TextContent("Radius|Radius of the shape.");

			public GUIContent coneAngle = EditorGUIUtility.TextContent("Angle|Angle of the cone.");

			public GUIContent coneLength = EditorGUIUtility.TextContent("Length|Length of the cone.");

			public GUIContent boxX = EditorGUIUtility.TextContent("Box X|Scale of the box in X Axis.");

			public GUIContent boxY = EditorGUIUtility.TextContent("Box Y|Scale of the box in Y Axis.");

			public GUIContent boxZ = EditorGUIUtility.TextContent("Box Z|Scale of the box in Z Axis.");

			public GUIContent mesh = EditorGUIUtility.TextContent("Mesh|Mesh that the particle system will emit from.");

			public GUIContent meshRenderer = EditorGUIUtility.TextContent("Mesh|MeshRenderer that the particle system will emit from.");

			public GUIContent skinnedMeshRenderer = EditorGUIUtility.TextContent("Mesh|SkinnedMeshRenderer that the particle system will emit from.");

			public GUIContent meshMaterialIndex = EditorGUIUtility.TextContent("Single Material|Only emit from a specific material of the mesh.");

			public GUIContent useMeshColors = EditorGUIUtility.TextContent("Use Mesh Colors|Modulate particle color with mesh vertex colors, or if they don't exist, use the shader color property \"_Color\" or \"_TintColor\" from the material. Does not read texture colors.");

			public GUIContent meshNormalOffset = EditorGUIUtility.TextContent("Normal Offset|Offset particle spawn positions along the mesh normal.");

			public GUIContent meshScale = EditorGUIUtility.TextContent("Mesh Scale|Adjust the size of the source mesh.");

			public GUIContent alignToDirection = EditorGUIUtility.TextContent("Align To Direction|Automatically align particles based on their initial direction of travel.");

			public GUIContent randomDirectionAmount = EditorGUIUtility.TextContent("Randomize Direction|Randomize the emission direction.");

			public GUIContent sphericalDirectionAmount = EditorGUIUtility.TextContent("Spherize Direction|Spherize the emission direction.");

			public GUIContent emitFromShell = EditorGUIUtility.TextContent("Emit from Shell|Emit from shell of the sphere. If disabled particles will be emitted from the volume of the shape.");

			public GUIContent emitFromEdge = EditorGUIUtility.TextContent("Emit from Edge|Emit from edge of the shape. If disabled particles will be emitted from the volume of the shape.");

			public GUIContent emitFrom = EditorGUIUtility.TextContent("Emit from:|Specifies from where particles are emitted.");
		}

		private class MultiModeTexts
		{
			public GUIContent value;

			public GUIContent mode;

			public GUIContent spread;

			public GUIContent speed;

			public MultiModeTexts(string _value, string _mode, string _spread, string _speed)
			{
				this.value = EditorGUIUtility.TextContent(_value);
				this.mode = EditorGUIUtility.TextContent(_mode);
				this.spread = EditorGUIUtility.TextContent(_spread);
				this.speed = EditorGUIUtility.TextContent(_speed);
			}
		}

		private SerializedProperty m_Type;

		private SerializedProperty m_RandomDirectionAmount;

		private SerializedProperty m_SphericalDirectionAmount;

		private ShapeModuleUI.MultiModeParameter m_Radius;

		private SerializedProperty m_Angle;

		private SerializedProperty m_Length;

		private SerializedProperty m_BoxX;

		private SerializedProperty m_BoxY;

		private SerializedProperty m_BoxZ;

		private ShapeModuleUI.MultiModeParameter m_Arc;

		private SerializedProperty m_PlacementMode;

		private SerializedProperty m_Mesh;

		private SerializedProperty m_MeshRenderer;

		private SerializedProperty m_SkinnedMeshRenderer;

		private SerializedProperty m_MeshMaterialIndex;

		private SerializedProperty m_UseMeshMaterialIndex;

		private SerializedProperty m_UseMeshColors;

		private SerializedProperty m_MeshNormalOffset;

		private SerializedProperty m_MeshScale;

		private SerializedProperty m_AlignToDirection;

		private Material m_Material;

		private static int s_BoxHandleControlIDHint = typeof(ShapeModuleUI).Name.GetHashCode();

		private BoxBoundsHandle m_BoxBoundsHandle = new BoxBoundsHandle(ShapeModuleUI.s_BoxHandleControlIDHint);

		private static Color s_ShapeGizmoColor = new Color(0.5803922f, 0.8980392f, 1f, 0.9f);

		private readonly string[] m_GuiNames = new string[]
		{
			"Sphere",
			"Hemisphere",
			"Cone",
			"Box",
			"Mesh",
			"Mesh Renderer",
			"Skinned Mesh Renderer",
			"Circle",
			"Edge"
		};

		private readonly ParticleSystemShapeType[] m_GuiTypes = new ParticleSystemShapeType[]
		{
			ParticleSystemShapeType.Sphere,
			ParticleSystemShapeType.Hemisphere,
			ParticleSystemShapeType.Cone,
			ParticleSystemShapeType.Box,
			ParticleSystemShapeType.Mesh,
			ParticleSystemShapeType.MeshRenderer,
			ParticleSystemShapeType.SkinnedMeshRenderer,
			ParticleSystemShapeType.Circle,
			ParticleSystemShapeType.SingleSidedEdge
		};

		private readonly int[] m_TypeToGuiTypeIndex = new int[]
		{
			0,
			0,
			1,
			1,
			2,
			3,
			4,
			2,
			2,
			2,
			7,
			7,
			8,
			5,
			6,
			3,
			3
		};

		private readonly ParticleSystemShapeType[] boxShapes = new ParticleSystemShapeType[]
		{
			ParticleSystemShapeType.Box,
			ParticleSystemShapeType.BoxShell,
			ParticleSystemShapeType.BoxEdge
		};

		private readonly ParticleSystemShapeType[] coneShapes = new ParticleSystemShapeType[]
		{
			ParticleSystemShapeType.Cone,
			ParticleSystemShapeType.ConeShell,
			ParticleSystemShapeType.ConeVolume,
			ParticleSystemShapeType.ConeVolumeShell
		};

		private readonly ParticleSystemShapeType[] shellShapes = new ParticleSystemShapeType[]
		{
			ParticleSystemShapeType.BoxShell,
			ParticleSystemShapeType.HemisphereShell,
			ParticleSystemShapeType.SphereShell,
			ParticleSystemShapeType.ConeShell,
			ParticleSystemShapeType.ConeVolumeShell,
			ParticleSystemShapeType.CircleEdge
		};

		private static ShapeModuleUI.Texts s_Texts = new ShapeModuleUI.Texts();

		private static ShapeModuleUI.MultiModeTexts s_RadiusTexts = new ShapeModuleUI.MultiModeTexts("Radius|New particles are spawned along the radius.", "Mode|Control how particles are spawned along the radius.", "Spread|Spawn particles only at specific positions along the radius (0 to disable).", "Speed|Control the speed that the emission position moves along the radius.");

		private static ShapeModuleUI.MultiModeTexts s_ArcTexts = new ShapeModuleUI.MultiModeTexts("Arc|New particles are spawned around the arc.", "Mode|Control how particles are spawned around the arc.", "Spread|Spawn particles only at specific angles around the arc (0 to disable).", "Speed|Control the speed that the emission position moves around the arc.");

		public ShapeModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ShapeModule", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
		{
			this.m_ToolTip = "Shape of the emitter volume, which controls where particles are emitted and their initial direction.";
		}

		protected override void Init()
		{
			if (this.m_Type == null)
			{
				if (ShapeModuleUI.s_Texts == null)
				{
					ShapeModuleUI.s_Texts = new ShapeModuleUI.Texts();
				}
				this.m_Type = base.GetProperty("type");
				this.m_Radius = ShapeModuleUI.MultiModeParameter.GetProperty(this, "radius", ShapeModuleUI.s_RadiusTexts.speed);
				this.m_Angle = base.GetProperty("angle");
				this.m_Length = base.GetProperty("length");
				this.m_BoxX = base.GetProperty("boxX");
				this.m_BoxY = base.GetProperty("boxY");
				this.m_BoxZ = base.GetProperty("boxZ");
				this.m_Arc = ShapeModuleUI.MultiModeParameter.GetProperty(this, "arc", ShapeModuleUI.s_ArcTexts.speed);
				this.m_PlacementMode = base.GetProperty("placementMode");
				this.m_Mesh = base.GetProperty("m_Mesh");
				this.m_MeshRenderer = base.GetProperty("m_MeshRenderer");
				this.m_SkinnedMeshRenderer = base.GetProperty("m_SkinnedMeshRenderer");
				this.m_MeshMaterialIndex = base.GetProperty("m_MeshMaterialIndex");
				this.m_UseMeshMaterialIndex = base.GetProperty("m_UseMeshMaterialIndex");
				this.m_UseMeshColors = base.GetProperty("m_UseMeshColors");
				this.m_MeshNormalOffset = base.GetProperty("m_MeshNormalOffset");
				this.m_MeshScale = base.GetProperty("m_MeshScale");
				this.m_RandomDirectionAmount = base.GetProperty("randomDirectionAmount");
				this.m_SphericalDirectionAmount = base.GetProperty("sphericalDirectionAmount");
				this.m_AlignToDirection = base.GetProperty("alignToDirection");
				this.m_Material = (EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Material.mat") as Material);
			}
		}

		public override float GetXAxisScalar()
		{
			return this.m_ParticleSystemUI.GetEmitterDuration();
		}

		private ParticleSystemShapeType ConvertConeEmitFromToConeType(int emitFrom)
		{
			return this.coneShapes[emitFrom];
		}

		private int ConvertConeTypeToConeEmitFrom(ParticleSystemShapeType shapeType)
		{
			return Array.IndexOf<ParticleSystemShapeType>(this.coneShapes, shapeType);
		}

		private ParticleSystemShapeType ConvertBoxEmitFromToConeType(int emitFrom)
		{
			return this.boxShapes[emitFrom];
		}

		private int ConvertBoxTypeToConeEmitFrom(ParticleSystemShapeType shapeType)
		{
			return Array.IndexOf<ParticleSystemShapeType>(this.boxShapes, shapeType);
		}

		private bool GetUsesShell(ParticleSystemShapeType shapeType)
		{
			return Array.IndexOf<ParticleSystemShapeType>(this.shellShapes, shapeType) != -1;
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (ShapeModuleUI.s_Texts == null)
			{
				ShapeModuleUI.s_Texts = new ShapeModuleUI.Texts();
			}
			int num = this.m_Type.intValue;
			int num2 = this.m_TypeToGuiTypeIndex[num];
			bool usesShell = this.GetUsesShell((ParticleSystemShapeType)num);
			EditorGUI.BeginChangeCheck();
			int num3 = ModuleUI.GUIPopup(ShapeModuleUI.s_Texts.shape, num2, this.m_GuiNames, new GUILayoutOption[0]);
			bool flag = EditorGUI.EndChangeCheck();
			ParticleSystemShapeType particleSystemShapeType = this.m_GuiTypes[num3];
			if (num3 != num2)
			{
				num = (int)particleSystemShapeType;
			}
			switch (particleSystemShapeType)
			{
			case ParticleSystemShapeType.Sphere:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius.m_Value, new GUILayoutOption[0]);
				bool flag2 = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]);
				num = ((!flag2) ? 0 : 1);
				break;
			}
			case ParticleSystemShapeType.Hemisphere:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius.m_Value, new GUILayoutOption[0]);
				bool flag3 = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]);
				num = ((!flag3) ? 2 : 3);
				break;
			}
			case ParticleSystemShapeType.Cone:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.coneAngle, this.m_Angle, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius.m_Value, new GUILayoutOption[0]);
				this.m_Arc.OnInspectorGUI(ShapeModuleUI.s_ArcTexts);
				bool disabled = num != 8 && num != 9;
				using (new EditorGUI.DisabledScope(disabled))
				{
					ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.coneLength, this.m_Length, new GUILayoutOption[0]);
				}
				string[] options = new string[]
				{
					"Base",
					"Base Shell",
					"Volume",
					"Volume Shell"
				};
				int num4 = this.ConvertConeTypeToConeEmitFrom((ParticleSystemShapeType)num);
				num4 = ModuleUI.GUIPopup(ShapeModuleUI.s_Texts.emitFrom, num4, options, new GUILayoutOption[0]);
				num = (int)this.ConvertConeEmitFromToConeType(num4);
				break;
			}
			case ParticleSystemShapeType.Box:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.boxX, this.m_BoxX, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.boxY, this.m_BoxY, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.boxZ, this.m_BoxZ, new GUILayoutOption[0]);
				string[] options2 = new string[]
				{
					"Volume",
					"Shell",
					"Edge"
				};
				int num5 = this.ConvertBoxTypeToConeEmitFrom((ParticleSystemShapeType)num);
				num5 = ModuleUI.GUIPopup(ShapeModuleUI.s_Texts.emitFrom, num5, options2, new GUILayoutOption[0]);
				num = (int)this.ConvertBoxEmitFromToConeType(num5);
				break;
			}
			case ParticleSystemShapeType.Mesh:
			case ParticleSystemShapeType.MeshRenderer:
			case ParticleSystemShapeType.SkinnedMeshRenderer:
			{
				string[] options3 = new string[]
				{
					"Vertex",
					"Edge",
					"Triangle"
				};
				ModuleUI.GUIPopup("", this.m_PlacementMode, options3, new GUILayoutOption[0]);
				Material material = null;
				Mesh mesh = null;
				if (particleSystemShapeType == ParticleSystemShapeType.Mesh)
				{
					ModuleUI.GUIObject(ShapeModuleUI.s_Texts.mesh, this.m_Mesh, new GUILayoutOption[0]);
				}
				else if (particleSystemShapeType == ParticleSystemShapeType.MeshRenderer)
				{
					ModuleUI.GUIObject(ShapeModuleUI.s_Texts.meshRenderer, this.m_MeshRenderer, new GUILayoutOption[0]);
					MeshRenderer meshRenderer = (MeshRenderer)this.m_MeshRenderer.objectReferenceValue;
					if (meshRenderer)
					{
						material = meshRenderer.sharedMaterial;
						if (meshRenderer.GetComponent<MeshFilter>())
						{
							mesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
						}
					}
				}
				else
				{
					ModuleUI.GUIObject(ShapeModuleUI.s_Texts.skinnedMeshRenderer, this.m_SkinnedMeshRenderer, new GUILayoutOption[0]);
					SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)this.m_SkinnedMeshRenderer.objectReferenceValue;
					if (skinnedMeshRenderer)
					{
						material = skinnedMeshRenderer.sharedMaterial;
						mesh = skinnedMeshRenderer.sharedMesh;
					}
				}
				ModuleUI.GUIToggleWithIntField(ShapeModuleUI.s_Texts.meshMaterialIndex, this.m_UseMeshMaterialIndex, this.m_MeshMaterialIndex, false, new GUILayoutOption[0]);
				bool flag4 = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.useMeshColors, this.m_UseMeshColors, new GUILayoutOption[0]);
				if (flag4)
				{
					if (material != null && mesh != null)
					{
						int nameID = Shader.PropertyToID("_Color");
						int nameID2 = Shader.PropertyToID("_TintColor");
						if (!material.HasProperty(nameID) && !material.HasProperty(nameID2) && !mesh.HasChannel(Mesh.InternalShaderChannel.Color))
						{
							GUIContent gUIContent = EditorGUIUtility.TextContent("To use mesh colors, your source mesh must either provide vertex colors, or its shader must contain a color property named \"_Color\" or \"_TintColor\".");
							EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, true);
						}
					}
				}
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.meshNormalOffset, this.m_MeshNormalOffset, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.meshScale, this.m_MeshScale, new GUILayoutOption[0]);
				break;
			}
			case ParticleSystemShapeType.Circle:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius.m_Value, new GUILayoutOption[0]);
				this.m_Arc.OnInspectorGUI(ShapeModuleUI.s_ArcTexts);
				bool flag5 = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.emitFromEdge, usesShell, new GUILayoutOption[0]);
				num = ((!flag5) ? 10 : 11);
				break;
			}
			case ParticleSystemShapeType.SingleSidedEdge:
				this.m_Radius.OnInspectorGUI(ShapeModuleUI.s_RadiusTexts);
				break;
			}
			if (flag || !this.m_Type.hasMultipleDifferentValues)
			{
				this.m_Type.intValue = num;
			}
			ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.alignToDirection, this.m_AlignToDirection, new GUILayoutOption[0]);
			ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.randomDirectionAmount, this.m_RandomDirectionAmount, new GUILayoutOption[0]);
			ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.sphericalDirectionAmount, this.m_SphericalDirectionAmount, new GUILayoutOption[0]);
		}

		public override void OnSceneViewGUI()
		{
			Color color = Handles.color;
			Handles.color = ShapeModuleUI.s_ShapeGizmoColor;
			Matrix4x4 matrix = Handles.matrix;
			EditorGUI.BeginChangeCheck();
			ParticleSystem[] particleSystems = this.m_ParticleSystemUI.m_ParticleSystems;
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem particleSystem = particleSystems[i];
				ParticleSystem.ShapeModule shape = particleSystem.shape;
				ParticleSystem.MainModule main = particleSystem.main;
				ParticleSystemShapeType shapeType = shape.shapeType;
				Matrix4x4 matrix2 = default(Matrix4x4);
				float num = (shapeType != ParticleSystemShapeType.Mesh) ? 1f : shape.meshScale;
				if (main.scalingMode == ParticleSystemScalingMode.Local)
				{
					matrix2.SetTRS(particleSystem.transform.position, particleSystem.transform.rotation, particleSystem.transform.localScale * num);
				}
				else if (main.scalingMode == ParticleSystemScalingMode.Hierarchy)
				{
					matrix2 = particleSystem.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(num, num, num));
				}
				else
				{
					matrix2.SetTRS(particleSystem.transform.position, particleSystem.transform.rotation, particleSystem.transform.lossyScale * num);
				}
				Handles.matrix = matrix2;
				if (shapeType == ParticleSystemShapeType.Sphere || shapeType == ParticleSystemShapeType.SphereShell)
				{
					EditorGUI.BeginChangeCheck();
					float radius = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, shape.radius, false);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(particleSystem, "Sphere Handle Change");
						shape.radius = radius;
					}
				}
				else if (shapeType == ParticleSystemShapeType.Circle || shapeType == ParticleSystemShapeType.CircleEdge)
				{
					EditorGUI.BeginChangeCheck();
					float radius2 = shape.radius;
					float arc = shape.arc;
					Handles.DoSimpleRadiusArcHandleXY(Quaternion.identity, Vector3.zero, ref radius2, ref arc);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(particleSystem, "Circle Handle Change");
						shape.radius = radius2;
						shape.arc = arc;
					}
				}
				else if (shapeType == ParticleSystemShapeType.Hemisphere || shapeType == ParticleSystemShapeType.HemisphereShell)
				{
					EditorGUI.BeginChangeCheck();
					float radius3 = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, shape.radius, true);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(particleSystem, "Hemisphere Handle Change");
						shape.radius = radius3;
					}
				}
				else if (shapeType == ParticleSystemShapeType.Cone || shapeType == ParticleSystemShapeType.ConeShell)
				{
					EditorGUI.BeginChangeCheck();
					Vector3 radiusAngleRange = new Vector3(shape.radius, shape.angle, main.startSpeedMultiplier);
					radiusAngleRange = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(particleSystem, "Cone Handle Change");
						shape.radius = radiusAngleRange.x;
						shape.angle = radiusAngleRange.y;
						main.startSpeedMultiplier = radiusAngleRange.z;
					}
				}
				else if (shapeType == ParticleSystemShapeType.ConeVolume || shapeType == ParticleSystemShapeType.ConeVolumeShell)
				{
					EditorGUI.BeginChangeCheck();
					Vector3 radiusAngleRange2 = new Vector3(shape.radius, shape.angle, shape.length);
					radiusAngleRange2 = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange2);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(particleSystem, "Cone Volume Handle Change");
						shape.radius = radiusAngleRange2.x;
						shape.angle = radiusAngleRange2.y;
						shape.length = radiusAngleRange2.z;
					}
				}
				else if (shapeType == ParticleSystemShapeType.Box || shapeType == ParticleSystemShapeType.BoxShell || shapeType == ParticleSystemShapeType.BoxEdge)
				{
					EditorGUI.BeginChangeCheck();
					this.m_BoxBoundsHandle.center = Vector3.zero;
					this.m_BoxBoundsHandle.size = shape.box;
					this.m_BoxBoundsHandle.DrawHandle();
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(particleSystem, "Box Handle Change");
						shape.box = this.m_BoxBoundsHandle.size;
					}
				}
				else if (shapeType == ParticleSystemShapeType.SingleSidedEdge)
				{
					EditorGUI.BeginChangeCheck();
					float radius4 = Handles.DoSimpleEdgeHandle(Quaternion.identity, Vector3.zero, shape.radius);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(particleSystem, "Edge Handle Change");
						shape.radius = radius4;
					}
				}
				else if (shapeType == ParticleSystemShapeType.Mesh)
				{
					Mesh mesh = shape.mesh;
					if (mesh)
					{
						bool wireframe = GL.wireframe;
						GL.wireframe = true;
						this.m_Material.SetPass(0);
						Graphics.DrawMeshNow(mesh, matrix2);
						GL.wireframe = wireframe;
					}
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
			}
			Handles.color = color;
			Handles.matrix = matrix;
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			this.Init();
			if (this.m_Arc.m_Mode.intValue != 0 || this.m_Radius.m_Mode.intValue != 0)
			{
				text += "\n\tAnimated shape emission is enabled.";
			}
		}
	}
}
