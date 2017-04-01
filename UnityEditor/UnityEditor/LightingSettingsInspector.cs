using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace UnityEditor
{
	internal class LightingSettingsInspector
	{
		private class Styles
		{
			public GUIContent OptimizeRealtimeUVs = EditorGUIUtility.TextContent("Optimize Realtime UVs|Specifies whether the authored mesh UVs get optimized for Realtime Global Illumination or not. When enabled, the authored UVs can get merged, scaled, and packed for optimisation purposes. When disabled, the authored UVs will get scaled and packed, but not merged.");

			public GUIContent IgnoreNormalsForChartDetection = EditorGUIUtility.TextContent("Ignore Normals|When enabled, prevents the UV charts from being split during the precompute process for Realtime Global Illumination lighting.");

			public int[] MinimumChartSizeValues = new int[]
			{
				2,
				4
			};

			public GUIContent[] MinimumChartSizeStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("2 (Minimum)"),
				EditorGUIUtility.TextContent("4 (Stitchable)")
			};

			public GUIContent Lighting = new GUIContent(EditorGUIUtility.TextContent("Lighting").text);

			public GUIContent MinimumChartSize = EditorGUIUtility.TextContent("Min Chart Size|Specifies the minimum texel size used for a UV chart. If stitching is required, a value of 4 will create a chart of 4x4 texels to store lighting and directionality. If stitching is not required, a value of 2 will reduce the texel density and provide better lighting build times and run time performance.");

			public GUIContent ImportantGI = EditorGUIUtility.TextContent("Prioritize Illumination|When enabled, the object will be marked as a priority object and always included in lighting calculations. Useful for objects that will be strongly emissive to make sure that other objects will be illuminated by this object.");

			public GUIContent AutoUVMaxDistance = EditorGUIUtility.TextContent("Max Distance|Specifies the maximum worldspace distance to be used for UV chart simplification. If charts are within this distance they will be simplified for optimization purposes.");

			public GUIContent AutoUVMaxAngle = EditorGUIUtility.TextContent("Max Angle|Specifies the maximum angle in degrees between faces sharing a UV edge. If the angle between the faces is below this value, the UV charts will be simplified.");

			public GUIContent LightmapParameters = EditorGUIUtility.TextContent("Lightmap Parameters|Allows the adjustment of advanced parameters that affect the process of generating a lightmap for an object using global illumination.");

			public GUIContent AtlasTilingX = EditorGUIUtility.TextContent("Tiling X");

			public GUIContent AtlasTilingY = EditorGUIUtility.TextContent("Tiling Y");

			public GUIContent AtlasOffsetX = EditorGUIUtility.TextContent("Offset X");

			public GUIContent AtlasOffsetY = EditorGUIUtility.TextContent("Offset Y");

			public GUIContent ClampedSize = EditorGUIUtility.TextContent("Object's size in lightmap has reached the max atlas size.|If you need higher resolution for this object, divide it into smaller meshes or set higher max atlas size via the LightmapEditorSettings class.");

			public GUIContent ClampedPackingResolution = EditorGUIUtility.TextContent("Object's size in the realtime lightmap has reached the maximum size. If you need higher resolution for this object, divide it into smaller meshes.");

			public GUIContent ZeroAreaPackingMesh = EditorGUIUtility.TextContent("Mesh used by the renderer has zero UV or surface area. Non zero area is required for lightmapping.");

			public GUIContent NoNormalsNoLightmapping = EditorGUIUtility.TextContent("Mesh used by the renderer doesn't have normals. Normals are needed for lightmapping.");

			public GUIContent Atlas = EditorGUIUtility.TextContent("Baked Lightmap");

			public GUIContent RealtimeLM = EditorGUIUtility.TextContent("Realtime Lightmap");

			public GUIContent ScaleInLightmap = EditorGUIUtility.TextContent("Scale In Lightmap|Specifies the relative size of object's UVs within a lightmap. A value of 0 will result in the object not being lightmapped, but still contribute lighting to other objects in the Scene.");

			public GUIContent AtlasIndex = EditorGUIUtility.TextContent("Lightmap Index");

			public GUIContent PVRInstanceHash = EditorGUIUtility.TextContent("Instance Hash|The hash of the baked GI instance.");

			public GUIContent PVRAtlasHash = EditorGUIUtility.TextContent("Atlas Hash|The hash of the atlas this baked GI instance is a part of.");

			public GUIContent PVRAtlasInstanceOffset = EditorGUIUtility.TextContent("Atlas Instance Offset|The offset into the transform array instances of this atlas start at.");

			public GUIContent RealtimeLMResolution = EditorGUIUtility.TextContent("System Resolution|The resolution in texels of the realtime lightmap that this renderer belongs to.");

			public GUIContent RealtimeLMInstanceResolution = EditorGUIUtility.TextContent("Instance Resolution|The resolution in texels of the realtime lightmap packed instance.");

			public GUIContent RealtimeLMInputSystemHash = EditorGUIUtility.TextContent("System Hash|The hash of the realtime system that the renderer belongs to.");

			public GUIContent RealtimeLMInstanceHash = EditorGUIUtility.TextContent("Instance Hash|The hash of the realtime GI instance.");

			public GUIContent RealtimeLMGeometryHash = EditorGUIUtility.TextContent("Geometry Hash|The hash of the realtime GI geometry that the renderer is using.");

			public GUIContent UVCharting = EditorGUIUtility.TextContent("UV Charting Control");

			public GUIContent LightmapSettings = EditorGUIUtility.TextContent("Lightmap Settings");

			public GUIContent LightmapStatic = EditorGUIUtility.TextContent("Lightmap Static|Controls whether the geometry will be marked as Static for lightmapping purposes. When enabled, this mesh will be present in lightmap calculations.");

			public GUIContent CastShadows = EditorGUIUtility.TextContent("Cast Shadows|Specifies whether a geometry creates shadows or not when a shadow-casting Light shines on it.");

			public GUIContent ReceiveShadows = EditorGUIUtility.TextContent("Receive Shadows|When enabled, any shadows cast from other objects are drawn on the geometry.");

			public GUIContent MotionVectors = EditorGUIUtility.TextContent("Motion Vectors|Specifies whether the Mesh renders 'Per Object Motion', 'Camera Motion', or 'No Motion' vectors to the Camera Motion Vector Texture.");

			public GUIContent LightmapInfoBox = EditorGUIUtility.TextContent("To enable generation of lightmaps for this Mesh Renderer, please enable the 'Lightmap Static' property.");

			public GUIContent TerrainLightmapInfoBox = EditorGUIUtility.TextContent("To enable generation of lightmaps for this Mesh Renderer, please enable the 'Lightmap Static' property.");

			public GUIContent ResolutionTooHighWarning = EditorGUIUtility.TextContent("Precompute/indirect resolution for this terrain is probably too high. Use a lower realtime/indirect resolution setting in the Lighting window or assign LightmapParameters that use a lower resolution setting. Otherwise it may take a very long time to bake and memory consumption during and after the bake may be very high.");

			public GUIContent ResolutionTooLowWarning = EditorGUIUtility.TextContent("Precompute/indirect resolution for this terrain is probably too low. If the Clustering stage takes a long time, try using a higher realtime/indirect resolution setting in the Lighting window or assign LightmapParameters that use a higher resolution setting.");

			public GUIContent GINotEnabledInfo = EditorGUIUtility.TextContent("Lightmapping settings are currently disabled. Enable Baked Global Illumination or Realtime Global Illumination to display these settings.");
		}

		private static LightingSettingsInspector.Styles s_Styles;

		private ZoomableArea m_ZoomablePreview;

		private GUIContent m_SelectedObjectPreviewTexture;

		private int m_PreviousSelection;

		private bool m_ShowSettings = false;

		private bool m_ShowChartingSettings = true;

		private bool m_ShowLightmapSettings = true;

		private bool m_ShowBakedLM = false;

		private bool m_ShowRealtimeLM = false;

		private AnimBool m_ShowClampedSize = new AnimBool();

		private SerializedObject m_SerializedObject;

		private SerializedObject m_GameObjectsSerializedObject;

		private SerializedProperty m_StaticEditorFlags;

		private SerializedProperty m_ImportantGI;

		private SerializedProperty m_LightmapParameters;

		private SerializedProperty m_LightmapIndex;

		private SerializedProperty m_LightmapTilingOffsetX;

		private SerializedProperty m_LightmapTilingOffsetY;

		private SerializedProperty m_LightmapTilingOffsetZ;

		private SerializedProperty m_LightmapTilingOffsetW;

		private SerializedProperty m_PreserveUVs;

		private SerializedProperty m_AutoUVMaxDistance;

		private SerializedProperty m_IgnoreNormalsForChartDetection;

		private SerializedProperty m_AutoUVMaxAngle;

		private SerializedProperty m_MinimumChartSize;

		private SerializedProperty m_LightmapScale;

		private SerializedProperty m_CastShadows;

		private SerializedProperty m_ReceiveShadows;

		private SerializedProperty m_MotionVectors;

		private Renderer[] m_Renderers;

		private Terrain[] m_Terrains;

		public bool showSettings
		{
			get
			{
				return this.m_ShowSettings;
			}
			set
			{
				this.m_ShowSettings = value;
			}
		}

		public bool showChartingSettings
		{
			get
			{
				return this.m_ShowChartingSettings;
			}
			set
			{
				this.m_ShowChartingSettings = value;
			}
		}

		public bool showLightmapSettings
		{
			get
			{
				return this.m_ShowLightmapSettings;
			}
			set
			{
				this.m_ShowLightmapSettings = value;
			}
		}

		private bool isPrefabAsset
		{
			get
			{
				bool result;
				if (this.m_SerializedObject == null || this.m_SerializedObject.targetObject == null)
				{
					result = false;
				}
				else
				{
					PrefabType prefabType = PrefabUtility.GetPrefabType(this.m_SerializedObject.targetObject);
					result = (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab);
				}
				return result;
			}
		}

		public LightingSettingsInspector(SerializedObject serializedObject)
		{
			this.m_SerializedObject = serializedObject;
			this.m_GameObjectsSerializedObject = new SerializedObject((from t in serializedObject.targetObjects
			select ((Component)t).gameObject).ToArray<GameObject>());
			this.m_ImportantGI = this.m_SerializedObject.FindProperty("m_ImportantGI");
			this.m_LightmapParameters = this.m_SerializedObject.FindProperty("m_LightmapParameters");
			this.m_LightmapIndex = this.m_SerializedObject.FindProperty("m_LightmapIndex");
			this.m_LightmapTilingOffsetX = this.m_SerializedObject.FindProperty("m_LightmapTilingOffset.x");
			this.m_LightmapTilingOffsetY = this.m_SerializedObject.FindProperty("m_LightmapTilingOffset.y");
			this.m_LightmapTilingOffsetZ = this.m_SerializedObject.FindProperty("m_LightmapTilingOffset.z");
			this.m_LightmapTilingOffsetW = this.m_SerializedObject.FindProperty("m_LightmapTilingOffset.w");
			this.m_PreserveUVs = this.m_SerializedObject.FindProperty("m_PreserveUVs");
			this.m_AutoUVMaxDistance = this.m_SerializedObject.FindProperty("m_AutoUVMaxDistance");
			this.m_IgnoreNormalsForChartDetection = this.m_SerializedObject.FindProperty("m_IgnoreNormalsForChartDetection");
			this.m_AutoUVMaxAngle = this.m_SerializedObject.FindProperty("m_AutoUVMaxAngle");
			this.m_MinimumChartSize = this.m_SerializedObject.FindProperty("m_MinimumChartSize");
			this.m_LightmapScale = this.m_SerializedObject.FindProperty("m_ScaleInLightmap");
			this.m_CastShadows = serializedObject.FindProperty("m_CastShadows");
			this.m_ReceiveShadows = serializedObject.FindProperty("m_ReceiveShadows");
			this.m_MotionVectors = serializedObject.FindProperty("m_MotionVectors");
			this.m_Renderers = this.m_SerializedObject.targetObjects.OfType<Renderer>().ToArray<Renderer>();
			this.m_Terrains = this.m_SerializedObject.targetObjects.OfType<Terrain>().ToArray<Terrain>();
			this.m_StaticEditorFlags = this.m_GameObjectsSerializedObject.FindProperty("m_StaticEditorFlags");
		}

		public bool Begin()
		{
			if (LightingSettingsInspector.s_Styles == null)
			{
				LightingSettingsInspector.s_Styles = new LightingSettingsInspector.Styles();
			}
			this.m_ShowSettings = EditorGUILayout.Foldout(this.m_ShowSettings, LightingSettingsInspector.s_Styles.Lighting);
			bool result;
			if (!this.m_ShowSettings)
			{
				result = false;
			}
			else
			{
				EditorGUI.indentLevel++;
				result = true;
			}
			return result;
		}

		public void End()
		{
			if (this.m_ShowSettings)
			{
				EditorGUI.indentLevel--;
			}
		}

		public void RenderMeshSettings(bool showLightmapSettings)
		{
			if (LightingSettingsInspector.s_Styles == null)
			{
				LightingSettingsInspector.s_Styles = new LightingSettingsInspector.Styles();
			}
			if (this.m_SerializedObject != null && this.m_GameObjectsSerializedObject != null && this.m_GameObjectsSerializedObject.targetObjects.Length != 0)
			{
				this.m_GameObjectsSerializedObject.Update();
				EditorGUILayout.PropertyField(this.m_CastShadows, LightingSettingsInspector.s_Styles.CastShadows, true, new GUILayoutOption[0]);
				bool disabled = SceneView.IsUsingDeferredRenderingPath();
				using (new EditorGUI.DisabledScope(disabled))
				{
					EditorGUILayout.PropertyField(this.m_ReceiveShadows, LightingSettingsInspector.s_Styles.ReceiveShadows, true, new GUILayoutOption[0]);
				}
				EditorGUILayout.PropertyField(this.m_MotionVectors, LightingSettingsInspector.s_Styles.MotionVectors, true, new GUILayoutOption[0]);
				if (showLightmapSettings)
				{
					this.LightmapStaticSettings();
					if (!LightModeUtil.Get().IsAnyGIEnabled() && !this.isPrefabAsset)
					{
						EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.GINotEnabledInfo.text, MessageType.Info);
					}
					else
					{
						bool flag = (this.m_StaticEditorFlags.intValue & 1) != 0;
						if (flag)
						{
							this.m_ShowChartingSettings = EditorGUILayout.Foldout(this.m_ShowChartingSettings, LightingSettingsInspector.s_Styles.UVCharting);
							if (this.m_ShowChartingSettings)
							{
								this.RendererUVSettings();
							}
							this.m_ShowLightmapSettings = EditorGUILayout.Foldout(this.m_ShowLightmapSettings, LightingSettingsInspector.s_Styles.LightmapSettings);
							if (this.m_ShowLightmapSettings)
							{
								EditorGUI.indentLevel++;
								float num = LightmapVisualization.GetLightmapLODLevelScale(this.m_Renderers[0]);
								for (int i = 1; i < this.m_Renderers.Length; i++)
								{
									if (!Mathf.Approximately(num, LightmapVisualization.GetLightmapLODLevelScale(this.m_Renderers[i])))
									{
										num = 1f;
									}
								}
								float lightmapScale = this.LightmapScaleGUI(num) * LightmapVisualization.GetLightmapLODLevelScale(this.m_Renderers[0]);
								float cachedMeshSurfaceArea = InternalMeshUtil.GetCachedMeshSurfaceArea((MeshRenderer)this.m_Renderers[0]);
								this.ShowClampedSizeInLightmapGUI(lightmapScale, cachedMeshSurfaceArea);
								EditorGUILayout.PropertyField(this.m_ImportantGI, LightingSettingsInspector.s_Styles.ImportantGI, new GUILayoutOption[0]);
								LightingSettingsInspector.LightmapParametersGUI(this.m_LightmapParameters, LightingSettingsInspector.s_Styles.LightmapParameters);
								this.m_ShowBakedLM = EditorGUILayout.Foldout(this.m_ShowBakedLM, LightingSettingsInspector.s_Styles.Atlas);
								if (this.m_ShowBakedLM)
								{
									this.ShowAtlasGUI(this.m_Renderers[0].GetInstanceID());
								}
								this.m_ShowRealtimeLM = EditorGUILayout.Foldout(this.m_ShowRealtimeLM, LightingSettingsInspector.s_Styles.RealtimeLM);
								if (this.m_ShowRealtimeLM)
								{
									this.ShowRealtimeLMGUI(this.m_Renderers[0]);
								}
								EditorGUI.indentLevel--;
							}
							if (LightmapEditorSettings.HasZeroAreaMesh(this.m_Renderers[0]))
							{
								EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.ZeroAreaPackingMesh.text, MessageType.Warning);
							}
							if (LightmapEditorSettings.HasClampedResolution(this.m_Renderers[0]))
							{
								EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.ClampedPackingResolution.text, MessageType.Warning);
							}
							if (!LightingSettingsInspector.HasNormals(this.m_Renderers[0]))
							{
								EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.NoNormalsNoLightmapping.text, MessageType.Warning);
							}
							this.m_SerializedObject.ApplyModifiedProperties();
						}
						else
						{
							EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.LightmapInfoBox.text, MessageType.Info);
						}
					}
				}
			}
		}

		public void RenderTerrainSettings()
		{
			if (LightingSettingsInspector.s_Styles == null)
			{
				LightingSettingsInspector.s_Styles = new LightingSettingsInspector.Styles();
			}
			if (this.m_SerializedObject != null && this.m_GameObjectsSerializedObject != null && this.m_GameObjectsSerializedObject.targetObjects.Length != 0)
			{
				this.m_GameObjectsSerializedObject.Update();
				this.LightmapStaticSettings();
				if (!LightModeUtil.Get().IsAnyGIEnabled() && !this.isPrefabAsset)
				{
					EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.GINotEnabledInfo.text, MessageType.Info);
				}
				else
				{
					bool flag = (this.m_StaticEditorFlags.intValue & 1) != 0;
					if (flag)
					{
						this.m_ShowLightmapSettings = EditorGUILayout.Foldout(this.m_ShowLightmapSettings, LightingSettingsInspector.s_Styles.LightmapSettings);
						if (this.m_ShowLightmapSettings)
						{
							EditorGUI.indentLevel++;
							using (new EditorGUI.DisabledScope(!flag))
							{
								if (GUI.enabled)
								{
									this.ShowTerrainChunks(this.m_Terrains);
								}
								float lightmapScale = this.LightmapScaleGUI(1f);
								TerrainData terrainData = this.m_Terrains[0].terrainData;
								float cachedSurfaceArea = (!(terrainData != null)) ? 0f : (terrainData.size.x * terrainData.size.z);
								this.ShowClampedSizeInLightmapGUI(lightmapScale, cachedSurfaceArea);
								LightingSettingsInspector.LightmapParametersGUI(this.m_LightmapParameters, LightingSettingsInspector.s_Styles.LightmapParameters);
								if (GUI.enabled && this.m_Terrains.Length == 1 && this.m_Terrains[0].terrainData != null)
								{
									this.ShowBakePerformanceWarning(this.m_Terrains[0]);
								}
								this.m_ShowBakedLM = EditorGUILayout.Foldout(this.m_ShowBakedLM, LightingSettingsInspector.s_Styles.Atlas);
								if (this.m_ShowBakedLM)
								{
									this.ShowAtlasGUI(this.m_Terrains[0].GetInstanceID());
								}
								this.m_ShowRealtimeLM = EditorGUILayout.Foldout(this.m_ShowRealtimeLM, LightingSettingsInspector.s_Styles.RealtimeLM);
								if (this.m_ShowRealtimeLM)
								{
									this.ShowRealtimeLMGUI(this.m_Terrains[0]);
								}
								this.m_SerializedObject.ApplyModifiedProperties();
							}
							EditorGUI.indentLevel--;
						}
						GUILayout.Space(10f);
					}
					else
					{
						EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.TerrainLightmapInfoBox.text, MessageType.Info);
					}
				}
			}
		}

		private void LightmapStaticSettings()
		{
			bool flag = (this.m_StaticEditorFlags.intValue & 1) != 0;
			EditorGUI.BeginChangeCheck();
			flag = EditorGUILayout.Toggle(LightingSettingsInspector.s_Styles.LightmapStatic, flag, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				SceneModeUtility.SetStaticFlags(this.m_GameObjectsSerializedObject.targetObjects, 1, flag);
				this.m_GameObjectsSerializedObject.Update();
			}
		}

		private void RendererUVSettings()
		{
			EditorGUI.indentLevel++;
			bool flag = !this.m_PreserveUVs.boolValue;
			bool flag2 = EditorGUILayout.Toggle(LightingSettingsInspector.s_Styles.OptimizeRealtimeUVs, flag, new GUILayoutOption[0]);
			if (flag2 != flag)
			{
				this.m_PreserveUVs.boolValue = !flag2;
			}
			EditorGUI.indentLevel++;
			bool boolValue = this.m_PreserveUVs.boolValue;
			using (new EditorGUI.DisabledScope(boolValue))
			{
				EditorGUILayout.PropertyField(this.m_AutoUVMaxDistance, LightingSettingsInspector.s_Styles.AutoUVMaxDistance, new GUILayoutOption[0]);
				if (this.m_AutoUVMaxDistance.floatValue < 0f)
				{
					this.m_AutoUVMaxDistance.floatValue = 0f;
				}
				EditorGUILayout.Slider(this.m_AutoUVMaxAngle, 0f, 180f, LightingSettingsInspector.s_Styles.AutoUVMaxAngle, new GUILayoutOption[0]);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.PropertyField(this.m_IgnoreNormalsForChartDetection, LightingSettingsInspector.s_Styles.IgnoreNormalsForChartDetection, new GUILayoutOption[0]);
			EditorGUILayout.IntPopup(this.m_MinimumChartSize, LightingSettingsInspector.s_Styles.MinimumChartSizeStrings, LightingSettingsInspector.s_Styles.MinimumChartSizeValues, LightingSettingsInspector.s_Styles.MinimumChartSize, new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
		}

		private void ShowClampedSizeInLightmapGUI(float lightmapScale, float cachedSurfaceArea)
		{
			float num = Mathf.Sqrt(cachedSurfaceArea) * LightmapEditorSettings.bakeResolution * lightmapScale;
			float num2 = (float)Math.Min(LightmapEditorSettings.maxAtlasWidth, LightmapEditorSettings.maxAtlasHeight);
			this.m_ShowClampedSize.target = (num > num2);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowClampedSize.faded))
			{
				EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.ClampedSize.text, MessageType.Info);
			}
			EditorGUILayout.EndFadeGroup();
		}

		private float LightmapScaleGUI(float lodScale)
		{
			float num = lodScale * this.m_LightmapScale.floatValue;
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, LightingSettingsInspector.s_Styles.ScaleInLightmap, this.m_LightmapScale);
			EditorGUI.BeginChangeCheck();
			num = EditorGUI.FloatField(controlRect, LightingSettingsInspector.s_Styles.ScaleInLightmap, num);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_LightmapScale.floatValue = Mathf.Max(num / Mathf.Max(lodScale, 1.401298E-45f), 0f);
			}
			EditorGUI.EndProperty();
			return num;
		}

		private void ShowAtlasGUI(int instanceID)
		{
			EditorGUI.indentLevel++;
			Hash128 hash;
			LightmapEditorSettings.GetPVRInstanceHash(instanceID, out hash);
			EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.PVRInstanceHash, new GUIContent(hash.ToString()), new GUILayoutOption[0]);
			Hash128 hash2;
			LightmapEditorSettings.GetPVRAtlasHash(instanceID, out hash2);
			EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.PVRAtlasHash, new GUIContent(hash2.ToString()), new GUILayoutOption[0]);
			int num;
			LightmapEditorSettings.GetPVRAtlasInstanceOffset(instanceID, out num);
			EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.PVRAtlasInstanceOffset, new GUIContent(num.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.AtlasIndex, new GUIContent(this.m_LightmapIndex.intValue.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.AtlasTilingX, new GUIContent(this.m_LightmapTilingOffsetX.floatValue.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.AtlasTilingY, new GUIContent(this.m_LightmapTilingOffsetY.floatValue.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.AtlasOffsetX, new GUIContent(this.m_LightmapTilingOffsetZ.floatValue.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.AtlasOffsetY, new GUIContent(this.m_LightmapTilingOffsetW.floatValue.ToString()), new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
		}

		private void ShowRealtimeLMGUI(Terrain terrain)
		{
			EditorGUI.indentLevel++;
			int num;
			int num2;
			int num3;
			int num4;
			if (LightmapEditorSettings.GetTerrainSystemResolution(terrain, out num, out num2, out num3, out num4))
			{
				string text = num.ToString() + "x" + num2.ToString();
				if (num3 > 1 || num4 > 1)
				{
					text += string.Format(" ({0}x{1} chunks)", num3, num4);
				}
				EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.RealtimeLMResolution, new GUIContent(text), new GUILayoutOption[0]);
			}
			EditorGUI.indentLevel--;
		}

		private void ShowRealtimeLMGUI(Renderer renderer)
		{
			EditorGUI.indentLevel++;
			Hash128 hash;
			if (LightmapEditorSettings.GetInstanceHash(renderer, out hash))
			{
				EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.RealtimeLMInstanceHash, new GUIContent(hash.ToString()), new GUILayoutOption[0]);
			}
			Hash128 hash2;
			if (LightmapEditorSettings.GetGeometryHash(renderer, out hash2))
			{
				EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.RealtimeLMGeometryHash, new GUIContent(hash2.ToString()), new GUILayoutOption[0]);
			}
			int num;
			int num2;
			if (LightmapEditorSettings.GetInstanceResolution(renderer, out num, out num2))
			{
				EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.RealtimeLMInstanceResolution, new GUIContent(num.ToString() + "x" + num2.ToString()), new GUILayoutOption[0]);
			}
			Hash128 hash3;
			if (LightmapEditorSettings.GetInputSystemHash(renderer, out hash3))
			{
				EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.RealtimeLMInputSystemHash, new GUIContent(hash3.ToString()), new GUILayoutOption[0]);
			}
			int num3;
			int num4;
			if (LightmapEditorSettings.GetSystemResolution(renderer, out num3, out num4))
			{
				EditorGUILayout.LabelField(LightingSettingsInspector.s_Styles.RealtimeLMResolution, new GUIContent(num3.ToString() + "x" + num4.ToString()), new GUILayoutOption[0]);
			}
			EditorGUI.indentLevel--;
		}

		private static bool HasNormals(Renderer renderer)
		{
			Mesh mesh = null;
			if (renderer is MeshRenderer)
			{
				MeshFilter component = renderer.GetComponent<MeshFilter>();
				if (component != null)
				{
					mesh = component.sharedMesh;
				}
			}
			else if (renderer is SkinnedMeshRenderer)
			{
				mesh = (renderer as SkinnedMeshRenderer).sharedMesh;
			}
			return mesh != null && InternalMeshUtil.HasNormals(mesh);
		}

		private static bool isBuiltIn(SerializedProperty prop)
		{
			bool result;
			if (prop.objectReferenceValue != null)
			{
				LightmapParameters lightmapParameters = prop.objectReferenceValue as LightmapParameters;
				result = (lightmapParameters.hideFlags == HideFlags.NotEditable);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool LightmapParametersGUI(SerializedProperty prop, GUIContent content)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUIInternal.AssetPopup<LightmapParameters>(prop, content, "giparams", "Scene Default Parameters");
			string text = "Edit...";
			if (LightingSettingsInspector.isBuiltIn(prop))
			{
				text = "View";
			}
			bool result = false;
			if (prop.objectReferenceValue == null)
			{
				SerializedObject serializedObject = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
				SerializedProperty serializedProperty = serializedObject.FindProperty("m_LightmapEditorSettings.m_LightmapParameters");
				using (new EditorGUI.DisabledScope(serializedProperty == null))
				{
					if (LightingSettingsInspector.isBuiltIn(serializedProperty))
					{
						text = "View";
					}
					else
					{
						text = "Edit...";
					}
					if (GUILayout.Button(text, EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						Selection.activeObject = serializedProperty.objectReferenceValue;
						result = true;
					}
				}
			}
			else if (GUILayout.Button(text, EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				Selection.activeObject = prop.objectReferenceValue;
				result = true;
			}
			EditorGUILayout.EndHorizontal();
			return result;
		}

		private void ShowTerrainChunks(Terrain[] terrains)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < terrains.Length; i++)
			{
				Terrain terrain = terrains[i];
				int num3 = 0;
				int num4 = 0;
				Lightmapping.GetTerrainGIChunks(terrain, ref num3, ref num4);
				if (num == 0 && num2 == 0)
				{
					num = num3;
					num2 = num4;
				}
				else if (num != num3 || num2 != num4)
				{
					num2 = (num = 0);
					break;
				}
			}
			if (num * num2 > 1)
			{
				EditorGUILayout.HelpBox(string.Format("Terrain is chunked up into {0} instances for baking.", num * num2), MessageType.None);
			}
		}

		private void ShowBakePerformanceWarning(Terrain terrain)
		{
			float x = terrain.terrainData.size.x;
			float z = terrain.terrainData.size.z;
			LightmapParameters lightmapParameters = ((LightmapParameters)this.m_LightmapParameters.objectReferenceValue) ?? new LightmapParameters();
			float num = x * lightmapParameters.resolution * LightmapEditorSettings.realtimeResolution;
			float num2 = z * lightmapParameters.resolution * LightmapEditorSettings.realtimeResolution;
			if (num > 512f || num2 > 512f)
			{
				EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.ResolutionTooHighWarning.text, MessageType.Warning);
			}
			float num3 = num * lightmapParameters.clusterResolution;
			float num4 = num2 * lightmapParameters.clusterResolution;
			float num5 = (float)terrain.terrainData.heightmapResolution / num3;
			float num6 = (float)terrain.terrainData.heightmapResolution / num4;
			if (num5 > 51.2f || num6 > 51.2f)
			{
				EditorGUILayout.HelpBox(LightingSettingsInspector.s_Styles.ResolutionTooLowWarning.text, MessageType.Warning);
			}
		}
	}
}
