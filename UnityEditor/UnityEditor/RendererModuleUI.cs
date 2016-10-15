using System;
using System.Collections.Generic;
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
			Mesh
		}

		private class Texts
		{
			public GUIContent renderMode = EditorGUIUtility.TextContent("Render Mode|Defines the render mode of the particle renderer.");

			public GUIContent material = EditorGUIUtility.TextContent("Material|Defines the material used to render particles.");

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

			public string[] particleTypes = new string[]
			{
				"Billboard",
				"Stretched Billboard",
				"Horizontal Billboard",
				"Vertical Billboard",
				"Mesh"
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
				"Local"
			};
		}

		private const int k_MaxNumMeshes = 4;

		private SerializedProperty m_CastShadows;

		private SerializedProperty m_ReceiveShadows;

		private SerializedProperty m_Material;

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

		private static RendererModuleUI.Texts s_Texts;

		public RendererModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ParticleSystemRenderer", displayName, ModuleUI.VisibilityState.VisibleAndFolded)
		{
			this.m_ToolTip = "Specifies how the particles are rendered.";
		}

		protected override void Init()
		{
			if (this.m_CastShadows != null)
			{
				return;
			}
			this.m_CastShadows = base.GetProperty0("m_CastShadows");
			this.m_ReceiveShadows = base.GetProperty0("m_ReceiveShadows");
			this.m_Material = base.GetProperty0("m_Materials.Array.data[0]");
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
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (RendererModuleUI.s_Texts == null)
			{
				RendererModuleUI.s_Texts = new RendererModuleUI.Texts();
			}
			RendererModuleUI.RenderMode intValue = (RendererModuleUI.RenderMode)this.m_RenderMode.intValue;
			RendererModuleUI.RenderMode renderMode = (RendererModuleUI.RenderMode)ModuleUI.GUIPopup(RendererModuleUI.s_Texts.renderMode, this.m_RenderMode, RendererModuleUI.s_Texts.particleTypes);
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
				ModuleUI.GUIFloat(RendererModuleUI.s_Texts.cameraSpeedScale, this.m_CameraVelocityScale);
				ModuleUI.GUIFloat(RendererModuleUI.s_Texts.speedScale, this.m_VelocityScale);
				ModuleUI.GUIFloat(RendererModuleUI.s_Texts.lengthScale, this.m_LengthScale);
				EditorGUI.indentLevel--;
			}
			if (renderMode != RendererModuleUI.RenderMode.Mesh)
			{
				ModuleUI.GUIFloat(RendererModuleUI.s_Texts.normalDirection, this.m_NormalDirection);
			}
			if (this.m_Material != null)
			{
				ModuleUI.GUIObject(RendererModuleUI.s_Texts.material, this.m_Material);
			}
			ModuleUI.GUIPopup(RendererModuleUI.s_Texts.sortMode, this.m_SortMode, RendererModuleUI.s_Texts.sortTypes);
			ModuleUI.GUIFloat(RendererModuleUI.s_Texts.sortingFudge, this.m_SortingFudge);
			ModuleUI.GUIPopup(RendererModuleUI.s_Texts.castShadows, this.m_CastShadows, this.m_CastShadows.enumDisplayNames);
			using (new EditorGUI.DisabledScope(SceneView.IsUsingDeferredRenderingPath()))
			{
				ModuleUI.GUIToggle(RendererModuleUI.s_Texts.receiveShadows, this.m_ReceiveShadows);
			}
			if (renderMode != RendererModuleUI.RenderMode.Mesh)
			{
				ModuleUI.GUIFloat(RendererModuleUI.s_Texts.minParticleSize, this.m_MinParticleSize);
				ModuleUI.GUIFloat(RendererModuleUI.s_Texts.maxParticleSize, this.m_MaxParticleSize);
			}
			EditorGUILayout.Space();
			EditorGUILayout.SortingLayerField(RendererModuleUI.s_Texts.sortingLayer, this.m_SortingLayerID, ParticleSystemStyles.Get().popup, ParticleSystemStyles.Get().label);
			ModuleUI.GUIInt(RendererModuleUI.s_Texts.sortingOrder, this.m_SortingOrder);
			if (renderMode == RendererModuleUI.RenderMode.Billboard)
			{
				ModuleUI.GUIPopup(RendererModuleUI.s_Texts.space, this.m_RenderAlignment, RendererModuleUI.s_Texts.spaces);
			}
			ModuleUI.GUIVector3Field(RendererModuleUI.s_Texts.pivot, this.m_Pivot);
			this.m_Probes.OnGUI(null, s.GetComponent<Renderer>(), true);
		}

		private void DoListOfMeshesGUI()
		{
			base.GUIListOfFloatObjectToggleFields(RendererModuleUI.s_Texts.mesh, this.m_ShownMeshes, null, null, false);
			Rect rect = GUILayoutUtility.GetRect(0f, 13f);
			rect.x = rect.xMax - 24f - 5f;
			rect.width = 12f;
			if (this.m_ShownMeshes.Length > 1 && ModuleUI.MinusButton(rect))
			{
				this.m_ShownMeshes[this.m_ShownMeshes.Length - 1].objectReferenceValue = null;
				List<SerializedProperty> list = new List<SerializedProperty>(this.m_ShownMeshes);
				list.RemoveAt(list.Count - 1);
				this.m_ShownMeshes = list.ToArray();
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

		public bool IsMeshEmitter()
		{
			return this.m_RenderMode != null && this.m_RenderMode.intValue == 4;
		}
	}
}
