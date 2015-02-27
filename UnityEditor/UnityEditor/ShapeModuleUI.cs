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
			HemiSphere,
			HemiSphereShell,
			Cone,
			Box,
			Mesh,
			ConeShell,
			ConeVolume,
			ConeVolumeShell
		}
		private class Texts
		{
			public GUIContent shape = new GUIContent("Shape", "Defines the shape of the volume from which particles can be emitted, and the direction of the start velocity.");
			public GUIContent radius = new GUIContent("Radius", "Radius of the shape.");
			public GUIContent coneAngle = new GUIContent("Angle", "Angle of the cone.");
			public GUIContent coneLength = new GUIContent("Length", "Length of the cone.");
			public GUIContent boxX = new GUIContent("Box X", "Scale of the box in X Axis.");
			public GUIContent boxY = new GUIContent("Box Y", "Scale of the box in Y Axis.");
			public GUIContent boxZ = new GUIContent("Box Z", "Scale of the box in Z Axis.");
			public GUIContent mesh = new GUIContent("Mesh", "Mesh that the particle system will emit from.");
			public GUIContent randomDirection = new GUIContent("Random Direction", "Randomizes the starting direction of particles.");
			public GUIContent emitFromShell = new GUIContent("Emit from Shell", "Emit from shell of the sphere. If disabled particles will be emitted from the volume of the shape.");
			public GUIContent emitFrom = new GUIContent("Emit from:", "Specifies from where particles are emitted.");
		}
		private SerializedProperty m_Type;
		private SerializedProperty m_RandomDirection;
		private SerializedProperty m_Radius;
		private SerializedProperty m_Angle;
		private SerializedProperty m_Length;
		private SerializedProperty m_BoxX;
		private SerializedProperty m_BoxY;
		private SerializedProperty m_BoxZ;
		private SerializedProperty m_PlacementMode;
		private SerializedProperty m_Mesh;
		private Material m_Material;
		private static int s_BoxHash = "BoxColliderEditor".GetHashCode();
		private BoxEditor m_BoxEditor = new BoxEditor(true, ShapeModuleUI.s_BoxHash);
		private static Color s_ShapeGizmoColor = new Color(0.5803922f, 0.8980392f, 1f, 0.9f);
		private string[] m_GuiNames = new string[]
		{
			"Sphere",
			"HemiSphere",
			"Cone",
			"Box",
			"Mesh"
		};
		private ShapeModuleUI.ShapeTypes[] m_GuiTypes = new ShapeModuleUI.ShapeTypes[]
		{
			ShapeModuleUI.ShapeTypes.Sphere,
			ShapeModuleUI.ShapeTypes.HemiSphere,
			ShapeModuleUI.ShapeTypes.Cone,
			ShapeModuleUI.ShapeTypes.Box,
			ShapeModuleUI.ShapeTypes.Mesh
		};
		private int[] m_TypeToGuiTypeIndex = new int[]
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
			2
		};
		private static ShapeModuleUI.Texts s_Texts = new ShapeModuleUI.Texts();
		public ShapeModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ShapeModule", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
		{
			this.m_ToolTip = "Shape of the emitter volume, which controls where particles are emitted and their initial direction.";
		}
		protected override void Init()
		{
			if (this.m_Type != null)
			{
				return;
			}
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
			this.m_PlacementMode = base.GetProperty("placementMode");
			this.m_Mesh = base.GetProperty("m_Mesh");
			this.m_RandomDirection = base.GetProperty("randomDirection");
			this.m_Material = (EditorGUIUtility.GetBuiltinExtraResource(typeof(Material), "Default-Diffuse.mat") as Material);
			this.m_BoxEditor.SetAlwaysDisplayHandles(true);
		}
		public override float GetXAxisScalar()
		{
			return this.m_ParticleSystemUI.GetEmitterDuration();
		}
		private ShapeModuleUI.ShapeTypes ConvertConeEmitFromToConeType(int emitFrom)
		{
			if (emitFrom == 0)
			{
				return ShapeModuleUI.ShapeTypes.Cone;
			}
			if (emitFrom == 1)
			{
				return ShapeModuleUI.ShapeTypes.ConeShell;
			}
			if (emitFrom == 2)
			{
				return ShapeModuleUI.ShapeTypes.ConeVolume;
			}
			return ShapeModuleUI.ShapeTypes.ConeVolumeShell;
		}
		private int ConvertConeTypeToConeEmitFrom(ShapeModuleUI.ShapeTypes shapeType)
		{
			if (shapeType == ShapeModuleUI.ShapeTypes.Cone)
			{
				return 0;
			}
			if (shapeType == ShapeModuleUI.ShapeTypes.ConeShell)
			{
				return 1;
			}
			if (shapeType == ShapeModuleUI.ShapeTypes.ConeVolume)
			{
				return 2;
			}
			if (shapeType == ShapeModuleUI.ShapeTypes.ConeVolumeShell)
			{
				return 3;
			}
			return 0;
		}
		private bool GetUsesShell(ShapeModuleUI.ShapeTypes shapeType)
		{
			return shapeType == ShapeModuleUI.ShapeTypes.HemiSphereShell || shapeType == ShapeModuleUI.ShapeTypes.SphereShell || shapeType == ShapeModuleUI.ShapeTypes.ConeShell || shapeType == ShapeModuleUI.ShapeTypes.ConeVolumeShell;
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
			int num3 = ModuleUI.GUIPopup(ShapeModuleUI.s_Texts.shape, num2, this.m_GuiNames);
			ShapeModuleUI.ShapeTypes shapeTypes = this.m_GuiTypes[num3];
			if (num3 != num2)
			{
				num = (int)shapeTypes;
			}
			switch (shapeTypes)
			{
			case ShapeModuleUI.ShapeTypes.Sphere:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius);
				bool flag = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.emitFromShell, usesShell);
				num = ((!flag) ? 0 : 1);
				break;
			}
			case ShapeModuleUI.ShapeTypes.HemiSphere:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius);
				bool flag2 = ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.emitFromShell, usesShell);
				num = ((!flag2) ? 2 : 3);
				break;
			}
			case ShapeModuleUI.ShapeTypes.Cone:
			{
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.coneAngle, this.m_Angle);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.radius, this.m_Radius);
				bool disabled = num != 8 && num != 9;
				EditorGUI.BeginDisabledGroup(disabled);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.coneLength, this.m_Length);
				EditorGUI.EndDisabledGroup();
				string[] options = new string[]
				{
					"Base",
					"Base Shell",
					"Volume",
					"Volume Shell"
				};
				int num4 = this.ConvertConeTypeToConeEmitFrom((ShapeModuleUI.ShapeTypes)num);
				num4 = ModuleUI.GUIPopup(ShapeModuleUI.s_Texts.emitFrom, num4, options);
				num = (int)this.ConvertConeEmitFromToConeType(num4);
				break;
			}
			case ShapeModuleUI.ShapeTypes.Box:
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.boxX, this.m_BoxX);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.boxY, this.m_BoxY);
				ModuleUI.GUIFloat(ShapeModuleUI.s_Texts.boxZ, this.m_BoxZ);
				break;
			case ShapeModuleUI.ShapeTypes.Mesh:
			{
				string[] options2 = new string[]
				{
					"Vertex",
					"Edge",
					"Triangle"
				};
				ModuleUI.GUIPopup(string.Empty, this.m_PlacementMode, options2);
				ModuleUI.GUIObject(ShapeModuleUI.s_Texts.mesh, this.m_Mesh);
				break;
			}
			}
			this.m_Type.intValue = num;
			ModuleUI.GUIToggle(ShapeModuleUI.s_Texts.randomDirection, this.m_RandomDirection);
		}
		public override void OnSceneGUI(ParticleSystem s, InitialModuleUI initial)
		{
			Color color = Handles.color;
			Handles.color = ShapeModuleUI.s_ShapeGizmoColor;
			Matrix4x4 matrix = Handles.matrix;
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x.SetTRS(s.transform.position, s.transform.rotation, s.transform.lossyScale);
			Handles.matrix = matrix4x;
			EditorGUI.BeginChangeCheck();
			int intValue = this.m_Type.intValue;
			if (intValue == 0 || intValue == 1)
			{
				this.m_Radius.floatValue = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue, false);
			}
			else
			{
				if (intValue == 2 || intValue == 3)
				{
					this.m_Radius.floatValue = Handles.DoSimpleRadiusHandle(Quaternion.identity, Vector3.zero, this.m_Radius.floatValue, true);
				}
				else
				{
					if (intValue == 4 || intValue == 7)
					{
						Vector3 radiusAngleRange = new Vector3(this.m_Radius.floatValue, this.m_Angle.floatValue, initial.m_Speed.scalar.floatValue);
						radiusAngleRange = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange);
						this.m_Radius.floatValue = radiusAngleRange.x;
						this.m_Angle.floatValue = radiusAngleRange.y;
						initial.m_Speed.scalar.floatValue = radiusAngleRange.z;
					}
					else
					{
						if (intValue == 8 || intValue == 9)
						{
							Vector3 radiusAngleRange2 = new Vector3(this.m_Radius.floatValue, this.m_Angle.floatValue, this.m_Length.floatValue);
							radiusAngleRange2 = Handles.ConeFrustrumHandle(Quaternion.identity, Vector3.zero, radiusAngleRange2);
							this.m_Radius.floatValue = radiusAngleRange2.x;
							this.m_Angle.floatValue = radiusAngleRange2.y;
							this.m_Length.floatValue = radiusAngleRange2.z;
						}
						else
						{
							if (intValue == 5)
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
							else
							{
								if (intValue == 6)
								{
									Mesh mesh = (Mesh)this.m_Mesh.objectReferenceValue;
									if (mesh)
									{
										bool wireframeMode = ShaderUtil.wireframeMode;
										ShaderUtil.wireframeMode = true;
										this.m_Material.SetPass(0);
										Graphics.DrawMeshNow(mesh, s.transform.localToWorldMatrix);
										ShaderUtil.wireframeMode = wireframeMode;
									}
								}
							}
						}
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
	}
}
