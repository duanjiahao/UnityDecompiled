using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ShapeModuleUI : ModuleUI
	{
		private enum ShapeTypes
		{
			Sphere,
			SphereShell,
			Hemisphere,
			HemisphereShell,
			Cone,
			Box,
			Mesh,
			ConeShell,
			ConeVolume,
			ConeVolumeShell,
			Circle,
			CircleEdge,
			SingleSidedEdge,
			MeshRenderer,
			SkinnedMeshRenderer,
			BoxShell,
			BoxEdge
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

			public GUIContent useMeshColors = EditorGUIUtility.TextContent("Use Mesh Colors|Modulate particle color with mesh vertex colors, or if they don't exist, use the shader color property \"_Color\" or \"_TintColor\" from the material.");

			public GUIContent meshNormalOffset = EditorGUIUtility.TextContent("Normal Offset|Offset particle spawn positions along the mesh normal.");

			public GUIContent meshScale = EditorGUIUtility.TextContent("Mesh Scale|Adjust the size of the source mesh.");

			public GUIContent alignToDirection = EditorGUIUtility.TextContent("Align To Direction|Automatically align particles based on their initial direction of travel.");

			public GUIContent randomDirectionAmount = EditorGUIUtility.TextContent("Randomize Direction|Randomize the emission direction.");

			public GUIContent sphericalDirectionAmount = EditorGUIUtility.TextContent("Spherize Direction|Spherize the emission direction.");

			public GUIContent emitFromShell = EditorGUIUtility.TextContent("Emit from Shell|Emit from shell of the sphere. If disabled particles will be emitted from the volume of the shape.");

			public GUIContent emitFromEdge = EditorGUIUtility.TextContent("Emit from Edge|Emit from edge of the shape. If disabled particles will be emitted from the volume of the shape.");

			public GUIContent emitFrom = EditorGUIUtility.TextContent("Emit from:|Specifies from where particles are emitted.");

			public GUIContent arc = EditorGUIUtility.TextContent("Arc|Circle arc angle.");
		}

		private SerializedProperty m_Type;

		private SerializedProperty m_RandomDirectionAmount;

		private SerializedProperty m_SphericalDirectionAmount;

		private SerializedProperty m_Radius;

		private SerializedProperty m_Angle;

		private SerializedProperty m_Length;

		private SerializedProperty m_BoxX;

		private SerializedProperty m_BoxY;

		private SerializedProperty m_BoxZ;

		private SerializedProperty m_Arc;

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

		private static int s_BoxHash = "BoxColliderEditor".GetHashCode();

		private BoxEditor m_BoxEditor = new BoxEditor(true, ShapeModuleUI.s_BoxHash);

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

		private readonly ShapeModuleUI.ShapeTypes[] m_GuiTypes = new ShapeModuleUI.ShapeTypes[]
		{
			ShapeModuleUI.ShapeTypes.Sphere,
			ShapeModuleUI.ShapeTypes.Hemisphere,
			ShapeModuleUI.ShapeTypes.Cone,
			ShapeModuleUI.ShapeTypes.Box,
			ShapeModuleUI.ShapeTypes.Mesh,
			ShapeModuleUI.ShapeTypes.MeshRenderer,
			ShapeModuleUI.ShapeTypes.SkinnedMeshRenderer,
			ShapeModuleUI.ShapeTypes.Circle,
			ShapeModuleUI.ShapeTypes.SingleSidedEdge
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

		private readonly ShapeModuleUI.ShapeTypes[] boxShapes = new ShapeModuleUI.ShapeTypes[]
		{
			ShapeModuleUI.ShapeTypes.Box,
			ShapeModuleUI.ShapeTypes.BoxShell,
			ShapeModuleUI.ShapeTypes.BoxEdge
		};

		private readonly ShapeModuleUI.ShapeTypes[] coneShapes = new ShapeModuleUI.ShapeTypes[]
		{
			ShapeModuleUI.ShapeTypes.Cone,
			ShapeModuleUI.ShapeTypes.ConeShell,
			ShapeModuleUI.ShapeTypes.ConeVolume,
			ShapeModuleUI.ShapeTypes.ConeVolumeShell
		};

		private readonly ShapeModuleUI.ShapeTypes[] shellShapes = new ShapeModuleUI.ShapeTypes[]
		{
			ShapeModuleUI.ShapeTypes.BoxShell,
			ShapeModuleUI.ShapeTypes.HemisphereShell,
			ShapeModuleUI.ShapeTypes.SphereShell,
			ShapeModuleUI.ShapeTypes.ConeShell,
			ShapeModuleUI.ShapeTypes.ConeVolumeShell,
			ShapeModuleUI.ShapeTypes.CircleEdge
		};

		private static ShapeModuleUI.Texts s_Texts = new ShapeModuleUI.Texts();

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
				this.m_Radius = base.GetProperty("radius");
				this.m_Angle = base.GetProperty("angle");
				this.m_Length = base.GetProperty("length");
				this.m_BoxX = base.GetProperty("boxX");
				this.m_BoxY = base.GetProperty("boxY");
				this.m_BoxZ = base.GetProperty("boxZ");
				this.m_Arc = base.GetProperty("arc");
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
				this.m_BoxEditor.SetAlwaysDisplayHandles(true);
			}
		}

		public override float GetXAxisScalar()
		{
			return this.m_ParticleSystemUI.GetEmitterDuration();
		}

		private ShapeModuleUI.ShapeTypes ConvertConeEmitFromToConeType(int emitFrom)
		{
			return this.coneShapes[emitFrom];
		}

		private int ConvertConeTypeToConeEmitFrom(ShapeModuleUI.ShapeTypes shapeType)
		{
			return Array.IndexOf<ShapeModuleUI.ShapeTypes>(this.coneShapes, shapeType);
		}

		private ShapeModuleUI.ShapeTypes ConvertBoxEmitFromToConeType(int emitFrom)
		{
			return this.boxShapes[emitFrom];
		}

		private int ConvertBoxTypeToConeEmitFrom(ShapeModuleUI.ShapeTypes shapeType)
		{
			return Array.IndexOf<ShapeModuleUI.ShapeTypes>(this.boxShapes, shapeType);
		}

		private bool GetUsesShell(ShapeModuleUI.ShapeTypes shapeType)
		{
			return Array.IndexOf<ShapeModuleUI.ShapeTypes>(this.shellShapes, shapeType) != -1;
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (ShapeModuleUI.s_Texts == null)
			{
				ShapeModuleUI.s_Texts = new ShapeModuleUI.Texts();
			}
			int num = this.m_Type.intValue;
			int num2 = this.m_TypeToGuiTypeIndex[num];
			bool usesShell = this.GetUsesShell((ShapeModuleUI.ShapeTypes)num);
			int num3 = ModuleUI.GUIPopup(ShapeModuleUI.s_Texts.shape, num2, this.m_GuiNames, new GUILayoutOption[0]);
			ShapeModuleUI.ShapeTypes shapeTypes = this.m_GuiTypes[num3];
			if (num3 != num2)
			{
				num = (int)shapeTypes;
			}
			switch (shapeTypes)
			{
			case ShapeModuleUI.ShapeTypes.Sphere:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
				bool flag = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]);
				num = ((!flag) ? 0 : 1);
				break;
			}
			case ShapeModuleUI.ShapeTypes.Hemisphere:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
				bool flag2 = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.emitFromShell, usesShell, new GUILayoutOption[0]);
				num = ((!flag2) ? 2 : 3);
				break;
			}
			case ShapeModuleUI.ShapeTypes.Cone:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.coneAngle, this.m_Angle, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
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
				int num4 = this.ConvertConeTypeToConeEmitFrom((ShapeModuleUI.ShapeTypes)num);
				num4 = ModuleUI.GUIPopup(ShapeModuleUI.s_Texts.emitFrom, num4, options, new GUILayoutOption[0]);
				num = (int)this.ConvertConeEmitFromToConeType(num4);
				break;
			}
			case ShapeModuleUI.ShapeTypes.Box:
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
				int num5 = this.ConvertBoxTypeToConeEmitFrom((ShapeModuleUI.ShapeTypes)num);
				num5 = ModuleUI.GUIPopup(ShapeModuleUI.s_Texts.emitFrom, num5, options2, new GUILayoutOption[0]);
				num = (int)this.ConvertBoxEmitFromToConeType(num5);
				break;
			}
			case ShapeModuleUI.ShapeTypes.Mesh:
			case ShapeModuleUI.ShapeTypes.MeshRenderer:
			case ShapeModuleUI.ShapeTypes.SkinnedMeshRenderer:
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
				if (shapeTypes == ShapeModuleUI.ShapeTypes.Mesh)
				{
					ModuleUI.GUIObject(ShapeModuleUI.s_Texts.mesh, this.m_Mesh, new GUILayoutOption[0]);
				}
				else if (shapeTypes == ShapeModuleUI.ShapeTypes.MeshRenderer)
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
				bool flag3 = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.useMeshColors, this.m_UseMeshColors, new GUILayoutOption[0]);
				if (flag3)
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
			case ShapeModuleUI.ShapeTypes.Circle:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.arc, this.m_Arc, new GUILayoutOption[0]);
				bool flag4 = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.emitFromEdge, usesShell, new GUILayoutOption[0]);
				num = ((!flag4) ? 10 : 11);
				break;
			}
			case ShapeModuleUI.ShapeTypes.SingleSidedEdge:
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius, new GUILayoutOption[0]);
				break;
			}
			this.m_Type.intValue = num;
			ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.alignToDirection, this.m_AlignToDirection, new GUILayoutOption[0]);
			ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.randomDirectionAmount, this.m_RandomDirectionAmount, new GUILayoutOption[0]);
			ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.sphericalDirectionAmount, this.m_SphericalDirectionAmount, new GUILayoutOption[0]);
		}

		public override void OnSceneGUI(ParticleSystem system, InitialModuleUI initial)
		{
			Color color = Handles.color;
			Handles.color = ShapeModuleUI.s_ShapeGizmoColor;
			Matrix4x4 matrix = Handles.matrix;
			EditorGUI.BeginChangeCheck();
			int intValue = this.m_Type.intValue;
			Matrix4x4 matrix4x = default(Matrix4x4);
			float d = (intValue != 6) ? 1f : this.m_MeshScale.floatValue;
			if (system.main.scalingMode == ParticleSystemScalingMode.Hierarchy)
			{
				matrix4x.SetTRS(system.transform.position, system.transform.rotation, system.transform.lossyScale * d);
			}
			else
			{
				matrix4x.SetTRS(system.transform.position, system.transform.rotation, system.transform.localScale * d);
			}
			Handles.matrix = matrix4x;
			if (intValue == 0 || intValue == 1)
			{
				this.m_Radius.floatValue = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue, false);
			}
			if (intValue == 10 || intValue == 11)
			{
				float floatValue = this.m_Radius.floatValue;
				float floatValue2 = this.m_Arc.floatValue;
				Handles.DoSimpleRadiusArcHandleXY(Quaternion.identity, Vector3.zero, ref floatValue, ref floatValue2);
				this.m_Radius.floatValue = floatValue;
				this.m_Arc.floatValue = floatValue2;
			}
			else if (intValue == 2 || intValue == 3)
			{
				this.m_Radius.floatValue = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue, true);
			}
			else if (intValue == 4 || intValue == 7)
			{
				Vector3 radiusAngleRange = new Vector3(this.m_Radius.floatValue, this.m_Angle.floatValue, initial.m_Speed.scalar.floatValue);
				radiusAngleRange = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange);
				this.m_Radius.floatValue = radiusAngleRange.x;
				this.m_Angle.floatValue = radiusAngleRange.y;
				initial.m_Speed.scalar.floatValue = radiusAngleRange.z;
			}
			else if (intValue == 8 || intValue == 9)
			{
				Vector3 radiusAngleRange2 = new Vector3(this.m_Radius.floatValue, this.m_Angle.floatValue, this.m_Length.floatValue);
				radiusAngleRange2 = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange2);
				this.m_Radius.floatValue = radiusAngleRange2.x;
				this.m_Angle.floatValue = radiusAngleRange2.y;
				this.m_Length.floatValue = radiusAngleRange2.z;
			}
			else if (intValue == 5 || intValue == 15 || intValue == 16)
			{
				Vector3 zero = Vector3.zero;
				Vector3 vector = new Vector3(this.m_BoxX.floatValue, this.m_BoxY.floatValue, this.m_BoxZ.floatValue);
				if (this.m_BoxEditor.OnSceneGUI(matrix4x, ShapeModuleUI.s_ShapeGizmoColor, false, ref zero, ref vector))
				{
					this.m_BoxX.floatValue = vector.x;
					this.m_BoxY.floatValue = vector.y;
					this.m_BoxZ.floatValue = vector.z;
				}
			}
			else if (intValue == 12)
			{
				this.m_Radius.floatValue = Handles.DoSimpleEdgeHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue);
			}
			else if (intValue == 6)
			{
				Mesh mesh = (Mesh)this.m_Mesh.objectReferenceValue;
				if (mesh)
				{
					bool wireframe = GL.wireframe;
					GL.wireframe = true;
					this.m_Material.SetPass(0);
					Graphics.DrawMeshNow(mesh, matrix4x);
					GL.wireframe = wireframe;
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
			}
			Handles.color = color;
			Handles.matrix = matrix;
		}
	}
}
