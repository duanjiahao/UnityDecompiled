using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngineInternal;

namespace UnityEditor
{
	internal class LightingWindowObjectTab
	{
		private class Styles
		{
			public GUIContent PreserveUVs = EditorGUIUtility.TextContent("Preserve UVs|Preserve the incoming lightmap UVs when generating realtime GI UVs. The incoming UVs are packed but charts are not scaled or merged. This is necessary for correct edge stitching of axis aligned chart edges.");

			public GUIContent IgnoreNormalsForChartDetection = EditorGUIUtility.TextContent("Ignore Normals|Do not compare normals when detecting charts for realtime GI. This can be necessary when using hand authored UVs to avoid unnecessary chart splits.");

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

			public GUIContent MinimumChartSize = EditorGUIUtility.TextContent("Min Chart Size|Directionality is generated at half resolution so in order to stitch properly at least 4x4 texels are needed in a chart so that a gradient can be generated on all sides of the chart. If stitching is not needed set this value to 2 in order to save texels for better performance at runtime and a faster lighting build.");

			public GUIContent ImportantGI = EditorGUIUtility.TextContent("Important GI|Make all other objects dependent upon this object. Useful for objects that will be strongly emissive to make sure that other objects will be illuminated by it.");

			public GUIContent AutoUVMaxDistance = EditorGUIUtility.TextContent("Auto UV Max Distance|Enlighten automatically generates simplified UVs by merging UV charts. Charts will only be simplified if the worldspace distance between the charts is smaller than this value.");

			public GUIContent AutoUVMaxAngle = EditorGUIUtility.TextContent("Auto UV Max Angle|Enlighten automatically generates simplified UVs by merging UV charts. Charts will only be simplified if the angle between the charts is smaller than this value.");

			public GUIContent LightmapParameters = EditorGUIUtility.TextContent("Advanced Parameters|Lets you configure per instance lightmapping parameters. Objects will be automatically grouped by unique parameter sets.");

			public GUIContent AtlasTilingX = EditorGUIUtility.TextContent("Tiling X");

			public GUIContent AtlasTilingY = EditorGUIUtility.TextContent("Tiling Y");

			public GUIContent AtlasOffsetX = EditorGUIUtility.TextContent("Offset X");

			public GUIContent AtlasOffsetY = EditorGUIUtility.TextContent("Offset Y");

			public GUIContent ClampedSize = EditorGUIUtility.TextContent("Object's size in lightmap has reached the max atlas size.|If you need higher resolution for this object, divide it into smaller meshes or set higher max atlas size via the LightmapEditorSettings class.");

			public GUIContent ClampedPackingResolution = EditorGUIUtility.TextContent("Object's size in the realtime lightmap has reached the maximum size.|If you need higher resolution for this object, divide it into smaller meshes.");

			public GUIContent ZeroAreaPackingMesh = EditorGUIUtility.TextContent("Mesh used by the renderer has zero UV or surface area. Non zero area is required for lightmapping.");

			public GUIContent NoNormalsNoLightmapping = EditorGUIUtility.TextContent("Mesh used by the renderer doesn't have normals. Normals are needed for lightmapping.");

			public GUIContent Atlas = EditorGUIUtility.TextContent("Baked Lightmap");

			public GUIContent RealtimeLM = EditorGUIUtility.TextContent("Realtime Lightmap");

			public GUIContent ChunkSize = EditorGUIUtility.TextContent("Chunk Size");

			public GUIContent EmptySelection = EditorGUIUtility.TextContent("Select a Light, Mesh Renderer or a Terrain from the scene.");

			public GUIContent ScaleInLightmap = EditorGUIUtility.TextContent("Scale In Lightmap|Object's surface multiplied by this value determines it's size in the lightmap. 0 - don't lightmap this object.");

			public GUIContent TerrainLightmapSize = EditorGUIUtility.TextContent("Lightmap Size|Defines the size of the lightmap that will be used only by this terrain.");

			public GUIContent AtlasIndex = EditorGUIUtility.TextContent("Lightmap Index");

			public GUIContent RealtimeLMResolution = EditorGUIUtility.TextContent("System Resolution|The resolution in texels of the realtime lightmap that this renderer belongs to.");

			public GUIContent RealtimeLMInstanceResolution = EditorGUIUtility.TextContent("Instance Resolution|The resolution in texels of the realtime lightmap packed instance.");

			public GUIContent RealtimeLMInputSystemHash = EditorGUIUtility.TextContent("System Hash|The hash of the realtime system that the renderer belongs to.");

			public GUIContent RealtimeLMInstanceHash = EditorGUIUtility.TextContent("Instance Hash|The hash of the realtime GI instance.");

			public GUIContent RealtimeLMGeometryHash = EditorGUIUtility.TextContent("Geometry Hash|The hash of the realtime GI geometry that the renderer is using.");
		}

		private GITextureType[] kObjectPreviewTextureTypes = new GITextureType[]
		{
			GITextureType.Charting,
			GITextureType.Albedo,
			GITextureType.Emissive,
			GITextureType.Irradiance,
			GITextureType.Directionality,
			GITextureType.Baked,
			GITextureType.BakedDirectional
		};

		private static GUIContent[] kObjectPreviewTextureOptions = new GUIContent[]
		{
			EditorGUIUtility.TextContent("Charting"),
			EditorGUIUtility.TextContent("Albedo"),
			EditorGUIUtility.TextContent("Emissive"),
			EditorGUIUtility.TextContent("Realtime Intensity"),
			EditorGUIUtility.TextContent("Realtime Direction"),
			EditorGUIUtility.TextContent("Baked Intensity"),
			EditorGUIUtility.TextContent("Baked Direction")
		};

		private static LightingWindowObjectTab.Styles s_Styles;

		private ZoomableArea m_ZoomablePreview;

		private GUIContent m_SelectedObjectPreviewTexture;

		private int m_PreviousSelection;

		private bool m_ShowBakedLM = false;

		private bool m_ShowRealtimeLM = false;

		private bool m_HasSeparateIndirectUV = false;

		private AnimBool m_ShowClampedSize = new AnimBool();

		private Editor m_LightEditor;

		private Editor m_LightmapParametersEditor;

		public void OnEnable(EditorWindow window)
		{
			this.m_ShowClampedSize.value = false;
			this.m_ShowClampedSize.valueChanged.AddListener(new UnityAction(window.Repaint));
		}

		public void OnDisable()
		{
			UnityEngine.Object.DestroyImmediate(this.m_LightEditor);
			UnityEngine.Object.DestroyImmediate(this.m_LightmapParametersEditor);
		}

		private Editor GetLightEditor(Light[] lights)
		{
			Editor.CreateCachedEditor(lights, typeof(LightEditor), ref this.m_LightEditor);
			return this.m_LightEditor;
		}

		private Editor GetLightmapParametersEditor(UnityEngine.Object[] lights)
		{
			Editor.CreateCachedEditor(lights, typeof(LightmapParametersEditor), ref this.m_LightmapParametersEditor);
			return this.m_LightmapParametersEditor;
		}

		public void ObjectPreview(Rect r)
		{
			if (r.height > 0f)
			{
				if (LightingWindowObjectTab.s_Styles == null)
				{
					LightingWindowObjectTab.s_Styles = new LightingWindowObjectTab.Styles();
				}
				List<Texture2D> list = new List<Texture2D>();
				GITextureType[] array = this.kObjectPreviewTextureTypes;
				for (int i = 0; i < array.Length; i++)
				{
					GITextureType textureType = array[i];
					list.Add(LightmapVisualizationUtility.GetGITexture(textureType));
				}
				if (list.Count != 0)
				{
					if (this.m_ZoomablePreview == null)
					{
						this.m_ZoomablePreview = new ZoomableArea(true);
						this.m_ZoomablePreview.hRangeMin = 0f;
						this.m_ZoomablePreview.vRangeMin = 0f;
						this.m_ZoomablePreview.hRangeMax = 1f;
						this.m_ZoomablePreview.vRangeMax = 1f;
						this.m_ZoomablePreview.SetShownHRange(0f, 1f);
						this.m_ZoomablePreview.SetShownVRange(0f, 1f);
						this.m_ZoomablePreview.uniformScale = true;
						this.m_ZoomablePreview.scaleWithWindow = true;
					}
					GUI.Box(r, "", "PreBackground");
					Rect position = new Rect(r);
					position.y += 1f;
					position.height = 18f;
					GUI.Box(position, "", EditorStyles.toolbar);
					Rect rect = new Rect(r);
					rect.y += 1f;
					rect.height = 18f;
					rect.width = 120f;
					Rect rect2 = new Rect(r);
					rect2.yMin += rect.height;
					rect2.yMax -= 14f;
					rect2.width -= 11f;
					int num = Array.IndexOf<GUIContent>(LightingWindowObjectTab.kObjectPreviewTextureOptions, this.m_SelectedObjectPreviewTexture);
					if (num < 0)
					{
						num = 0;
					}
					num = EditorGUI.Popup(rect, num, LightingWindowObjectTab.kObjectPreviewTextureOptions, EditorStyles.toolbarPopup);
					if (num >= LightingWindowObjectTab.kObjectPreviewTextureOptions.Length)
					{
						num = 0;
					}
					this.m_SelectedObjectPreviewTexture = LightingWindowObjectTab.kObjectPreviewTextureOptions[num];
					LightmapType lightmapType = (this.kObjectPreviewTextureTypes[num] != GITextureType.Baked && this.kObjectPreviewTextureTypes[num] != GITextureType.BakedDirectional) ? LightmapType.DynamicLightmap : LightmapType.StaticLightmap;
					bool flag = (this.kObjectPreviewTextureTypes[num] == GITextureType.Baked || this.kObjectPreviewTextureTypes[num] == GITextureType.BakedDirectional) && LightmapSettings.lightmapsMode == LightmapsMode.SeparateDirectional;
					if (flag)
					{
						GUIContent gUIContent = GUIContent.Temp("Indirect");
						Rect position2 = rect;
						position2.x += rect.width;
						position2.width = EditorStyles.toolbarButton.CalcSize(gUIContent).x;
						this.m_HasSeparateIndirectUV = GUI.Toggle(position2, this.m_HasSeparateIndirectUV, gUIContent.text, EditorStyles.toolbarButton);
					}
					Event current = Event.current;
					EventType type = current.type;
					if (type != EventType.ValidateCommand && type != EventType.ExecuteCommand)
					{
						if (type == EventType.Repaint)
						{
							Texture2D texture2D = list[num];
							if (texture2D && Event.current.type == EventType.Repaint)
							{
								Rect rect3 = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
								rect3 = this.ResizeRectToFit(rect3, rect2);
								rect3 = this.CenterToRect(rect3, rect2);
								rect3 = this.ScaleRectByZoomableArea(rect3, this.m_ZoomablePreview);
								Rect position3 = new Rect(rect3);
								position3.x += 3f;
								position3.y += rect2.y + 20f;
								Rect drawableArea = new Rect(rect2);
								drawableArea.y += rect.height + 3f;
								float num2 = drawableArea.y - 14f;
								position3.y -= num2;
								drawableArea.y -= num2;
								FilterMode filterMode = texture2D.filterMode;
								texture2D.filterMode = FilterMode.Point;
								GITextureType textureType2 = this.kObjectPreviewTextureTypes[num];
								bool drawSpecularUV = flag && this.m_HasSeparateIndirectUV;
								LightmapVisualizationUtility.DrawTextureWithUVOverlay(texture2D, Selection.activeGameObject, drawableArea, position3, textureType2, drawSpecularUV);
								texture2D.filterMode = filterMode;
							}
						}
					}
					else if (Event.current.commandName == "FrameSelected")
					{
						Vector4 lightmapTilingOffset = LightmapVisualizationUtility.GetLightmapTilingOffset(lightmapType);
						Vector2 vector = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
						Vector2 lhs = vector + new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
						vector = Vector2.Max(vector, Vector2.zero);
						lhs = Vector2.Min(lhs, Vector2.one);
						float y = 1f - vector.y;
						vector.y = 1f - lhs.y;
						lhs.y = y;
						Rect shownArea = new Rect(vector.x, vector.y, lhs.x - vector.x, lhs.y - vector.y);
						shownArea.x -= Mathf.Clamp(shownArea.height - shownArea.width, 0f, 3.40282347E+38f) / 2f;
						shownArea.y -= Mathf.Clamp(shownArea.width - shownArea.height, 0f, 3.40282347E+38f) / 2f;
						float num3 = Mathf.Max(shownArea.width, shownArea.height);
						shownArea.height = num3;
						shownArea.width = num3;
						if (flag && this.m_HasSeparateIndirectUV)
						{
							shownArea.x += 0.5f;
						}
						this.m_ZoomablePreview.shownArea = shownArea;
						Event.current.Use();
					}
					if (this.m_PreviousSelection != Selection.activeInstanceID)
					{
						this.m_PreviousSelection = Selection.activeInstanceID;
						this.m_ZoomablePreview.SetShownHRange(0f, 1f);
						this.m_ZoomablePreview.SetShownVRange(0f, 1f);
					}
					Rect rect4 = new Rect(r);
					rect4.yMin += rect.height;
					this.m_ZoomablePreview.rect = rect4;
					this.m_ZoomablePreview.BeginViewGUI();
					this.m_ZoomablePreview.EndViewGUI();
					GUILayoutUtility.GetRect(r.width, r.height);
				}
			}
		}

		public bool EditLights()
		{
			GameObject[] array;
			Light[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Light>(out array, new Type[0]);
			bool result;
			if (array.Length == 0)
			{
				result = false;
			}
			else
			{
				EditorGUILayout.InspectorTitlebar(selectedObjectsOfType);
				this.GetLightEditor(selectedObjectsOfType).OnInspectorGUI();
				GUILayout.Space(10f);
				result = true;
			}
			return result;
		}

		public bool EditLightmapParameters()
		{
			UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(LightmapParameters), SelectionMode.Unfiltered);
			bool result;
			if (filtered.Length == 0)
			{
				result = false;
			}
			else
			{
				EditorGUILayout.MultiSelectionObjectTitleBar(filtered);
				this.GetLightmapParametersEditor(filtered).OnInspectorGUI();
				GUILayout.Space(10f);
				result = true;
			}
			return result;
		}

		public bool EditTerrains()
		{
			GameObject[] array;
			Terrain[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Terrain>(out array, new Type[0]);
			bool result;
			if (array.Length == 0)
			{
				result = false;
			}
			else
			{
				EditorGUILayout.InspectorTitlebar(selectedObjectsOfType);
				SerializedObject serializedObject = new SerializedObject(array);
				using (new EditorGUI.DisabledScope(!SceneModeUtility.StaticFlagField("Lightmap Static", serializedObject.FindProperty("m_StaticEditorFlags"), 1)))
				{
					if (GUI.enabled)
					{
						this.ShowTerrainChunks(selectedObjectsOfType);
					}
					SerializedObject serializedObject2 = new SerializedObject(selectedObjectsOfType.ToArray<Terrain>());
					float lightmapScale = this.LightmapScaleGUI(serializedObject2, 1f);
					TerrainData terrainData = selectedObjectsOfType[0].terrainData;
					float cachedSurfaceArea = (!(terrainData != null)) ? 0f : (terrainData.size.x * terrainData.size.z);
					this.ShowClampedSizeInLightmapGUI(lightmapScale, cachedSurfaceArea);
					LightingWindowObjectTab.LightmapParametersGUI(serializedObject2.FindProperty("m_LightmapParameters"), LightingWindowObjectTab.s_Styles.LightmapParameters, true);
					if (GUI.enabled && selectedObjectsOfType.Length == 1 && selectedObjectsOfType[0].terrainData != null)
					{
						this.ShowBakePerformanceWarning(serializedObject2, selectedObjectsOfType[0]);
					}
					this.m_ShowBakedLM = EditorGUILayout.Foldout(this.m_ShowBakedLM, LightingWindowObjectTab.s_Styles.Atlas);
					if (this.m_ShowBakedLM)
					{
						this.ShowAtlasGUI(serializedObject2);
					}
					this.m_ShowRealtimeLM = EditorGUILayout.Foldout(this.m_ShowRealtimeLM, LightingWindowObjectTab.s_Styles.RealtimeLM);
					if (this.m_ShowRealtimeLM)
					{
						this.ShowRealtimeLMGUI(selectedObjectsOfType[0]);
					}
					serializedObject.ApplyModifiedProperties();
					serializedObject2.ApplyModifiedProperties();
				}
				GUILayout.Space(10f);
				result = true;
			}
			return result;
		}

		public bool EditRenderers()
		{
			GameObject[] array;
			Renderer[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<Renderer>(out array, new Type[]
			{
				typeof(MeshRenderer),
				typeof(SkinnedMeshRenderer)
			});
			bool result;
			if (array.Length == 0)
			{
				result = false;
			}
			else
			{
				EditorGUILayout.InspectorTitlebar(selectedObjectsOfType);
				SerializedObject serializedObject = new SerializedObject(array);
				using (new EditorGUI.DisabledScope(!SceneModeUtility.StaticFlagField("Lightmap Static", serializedObject.FindProperty("m_StaticEditorFlags"), 1)))
				{
					SerializedObject serializedObject2 = new SerializedObject(selectedObjectsOfType);
					float num = LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[0]);
					for (int i = 1; i < selectedObjectsOfType.Length; i++)
					{
						if (!Mathf.Approximately(num, LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[i])))
						{
							num = 1f;
						}
					}
					float lightmapScale = this.LightmapScaleGUI(serializedObject2, num) * LightmapVisualization.GetLightmapLODLevelScale(selectedObjectsOfType[0]);
					float cachedSurfaceArea = (!(selectedObjectsOfType[0] is MeshRenderer)) ? InternalMeshUtil.GetCachedSkinnedMeshSurfaceArea(selectedObjectsOfType[0] as SkinnedMeshRenderer) : InternalMeshUtil.GetCachedMeshSurfaceArea(selectedObjectsOfType[0] as MeshRenderer);
					this.ShowClampedSizeInLightmapGUI(lightmapScale, cachedSurfaceArea);
					EditorGUILayout.PropertyField(serializedObject2.FindProperty("m_ImportantGI"), LightingWindowObjectTab.s_Styles.ImportantGI, new GUILayoutOption[0]);
					LightingWindowObjectTab.LightmapParametersGUI(serializedObject2.FindProperty("m_LightmapParameters"), LightingWindowObjectTab.s_Styles.LightmapParameters, true);
					GUILayout.Space(10f);
					this.RendererUVSettings(serializedObject2);
					GUILayout.Space(10f);
					this.m_ShowBakedLM = EditorGUILayout.Foldout(this.m_ShowBakedLM, LightingWindowObjectTab.s_Styles.Atlas, true);
					if (this.m_ShowBakedLM)
					{
						this.ShowAtlasGUI(serializedObject2);
					}
					this.m_ShowRealtimeLM = EditorGUILayout.Foldout(this.m_ShowRealtimeLM, LightingWindowObjectTab.s_Styles.RealtimeLM, true);
					if (this.m_ShowRealtimeLM)
					{
						this.ShowRealtimeLMGUI(serializedObject2, selectedObjectsOfType[0]);
					}
					if (LightmapEditorSettings.HasZeroAreaMesh(selectedObjectsOfType[0]))
					{
						EditorGUILayout.HelpBox(LightingWindowObjectTab.s_Styles.ZeroAreaPackingMesh.text, MessageType.Warning);
					}
					if (LightmapEditorSettings.HasClampedResolution(selectedObjectsOfType[0]))
					{
						EditorGUILayout.HelpBox(LightingWindowObjectTab.s_Styles.ClampedPackingResolution.text, MessageType.Warning);
					}
					if (!LightingWindowObjectTab.HasNormals(selectedObjectsOfType[0]))
					{
						EditorGUILayout.HelpBox(LightingWindowObjectTab.s_Styles.NoNormalsNoLightmapping.text, MessageType.Warning);
					}
					serializedObject.ApplyModifiedProperties();
					serializedObject2.ApplyModifiedProperties();
				}
				GUILayout.Space(10f);
				result = true;
			}
			return result;
		}

		public void ObjectSettings()
		{
			if (LightingWindowObjectTab.s_Styles == null)
			{
				LightingWindowObjectTab.s_Styles = new LightingWindowObjectTab.Styles();
			}
			SceneModeUtility.SearchBar(new Type[]
			{
				typeof(Light),
				typeof(Renderer),
				typeof(Terrain)
			});
			EditorGUILayout.Space();
			bool flag = false;
			flag |= this.EditRenderers();
			flag |= this.EditLightmapParameters();
			flag |= this.EditLights();
			if (!(flag | this.EditTerrains()))
			{
				GUILayout.Label(LightingWindowObjectTab.s_Styles.EmptySelection, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
		}

		private Rect ResizeRectToFit(Rect rect, Rect to)
		{
			float a = to.width / rect.width;
			float b = to.height / rect.height;
			float num = Mathf.Min(a, b);
			float width = (float)((int)Mathf.Round(rect.width * num));
			float height = (float)((int)Mathf.Round(rect.height * num));
			return new Rect(rect.x, rect.y, width, height);
		}

		private Rect CenterToRect(Rect rect, Rect to)
		{
			float num = Mathf.Clamp((float)((int)(to.width - rect.width)) / 2f, 0f, 2.14748365E+09f);
			float num2 = Mathf.Clamp((float)((int)(to.height - rect.height)) / 2f, 0f, 2.14748365E+09f);
			return new Rect(rect.x + num, rect.y + num2, rect.width, rect.height);
		}

		private Rect ScaleRectByZoomableArea(Rect rect, ZoomableArea zoomableArea)
		{
			float num = -(zoomableArea.shownArea.x / zoomableArea.shownArea.width) * rect.width;
			float num2 = (zoomableArea.shownArea.y - (1f - zoomableArea.shownArea.height)) / zoomableArea.shownArea.height * rect.height;
			float width = rect.width / zoomableArea.shownArea.width;
			float height = rect.height / zoomableArea.shownArea.height;
			return new Rect(rect.x + num, rect.y + num2, width, height);
		}

		private void RendererUVSettings(SerializedObject so)
		{
			SerializedProperty serializedProperty = so.FindProperty("m_PreserveUVs");
			EditorGUILayout.PropertyField(serializedProperty, LightingWindowObjectTab.s_Styles.PreserveUVs, new GUILayoutOption[0]);
			bool boolValue = serializedProperty.boolValue;
			using (new EditorGUI.DisabledScope(boolValue))
			{
				SerializedProperty serializedProperty2 = so.FindProperty("m_AutoUVMaxDistance");
				EditorGUILayout.PropertyField(serializedProperty2, LightingWindowObjectTab.s_Styles.AutoUVMaxDistance, new GUILayoutOption[0]);
				if (serializedProperty2.floatValue < 0f)
				{
					serializedProperty2.floatValue = 0f;
				}
				EditorGUILayout.Slider(so.FindProperty("m_AutoUVMaxAngle"), 0f, 180f, LightingWindowObjectTab.s_Styles.AutoUVMaxAngle, new GUILayoutOption[0]);
			}
			SerializedProperty property = so.FindProperty("m_IgnoreNormalsForChartDetection");
			EditorGUILayout.PropertyField(property, LightingWindowObjectTab.s_Styles.IgnoreNormalsForChartDetection, new GUILayoutOption[0]);
			SerializedProperty property2 = so.FindProperty("m_MinimumChartSize");
			EditorGUILayout.IntPopup(property2, LightingWindowObjectTab.s_Styles.MinimumChartSizeStrings, LightingWindowObjectTab.s_Styles.MinimumChartSizeValues, LightingWindowObjectTab.s_Styles.MinimumChartSize, new GUILayoutOption[0]);
		}

		private void ShowClampedSizeInLightmapGUI(float lightmapScale, float cachedSurfaceArea)
		{
			float num = Mathf.Sqrt(cachedSurfaceArea) * LightmapEditorSettings.bakeResolution * lightmapScale;
			float num2 = (float)Math.Min(LightmapEditorSettings.maxAtlasWidth, LightmapEditorSettings.maxAtlasHeight);
			this.m_ShowClampedSize.target = (num > num2);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowClampedSize.faded))
			{
				GUILayout.Label(LightingWindowObjectTab.s_Styles.ClampedSize, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
		}

		private float LightmapScaleGUI(SerializedObject so, float lodScale)
		{
			SerializedProperty serializedProperty = so.FindProperty("m_ScaleInLightmap");
			float num = lodScale * serializedProperty.floatValue;
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, LightingWindowObjectTab.s_Styles.ScaleInLightmap, serializedProperty);
			EditorGUI.BeginChangeCheck();
			num = EditorGUI.FloatField(controlRect, LightingWindowObjectTab.s_Styles.ScaleInLightmap, num);
			if (EditorGUI.EndChangeCheck())
			{
				serializedProperty.floatValue = Mathf.Max(num / lodScale, 0f);
			}
			EditorGUI.EndProperty();
			return num;
		}

		private void ShowAtlasGUI(SerializedObject so)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.AtlasIndex, new GUIContent(so.FindProperty("m_LightmapIndex").intValue.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.AtlasTilingX, new GUIContent(so.FindProperty("m_LightmapTilingOffset.x").floatValue.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.AtlasTilingY, new GUIContent(so.FindProperty("m_LightmapTilingOffset.y").floatValue.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.AtlasOffsetX, new GUIContent(so.FindProperty("m_LightmapTilingOffset.z").floatValue.ToString()), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.AtlasOffsetY, new GUIContent(so.FindProperty("m_LightmapTilingOffset.w").floatValue.ToString()), new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
		}

		private void ShowRealtimeLMGUI(SerializedObject so, Renderer renderer)
		{
			EditorGUI.indentLevel++;
			Hash128 hash;
			if (LightmapEditorSettings.GetInstanceHash(renderer, out hash))
			{
				EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.RealtimeLMInstanceHash, new GUIContent(hash.ToString()), new GUILayoutOption[0]);
			}
			Hash128 hash2;
			if (LightmapEditorSettings.GetGeometryHash(renderer, out hash2))
			{
				EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.RealtimeLMGeometryHash, new GUIContent(hash2.ToString()), new GUILayoutOption[0]);
			}
			int num;
			int num2;
			if (LightmapEditorSettings.GetInstanceResolution(renderer, out num, out num2))
			{
				EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.RealtimeLMInstanceResolution, new GUIContent(num.ToString() + "x" + num2.ToString()), new GUILayoutOption[0]);
			}
			Hash128 hash3;
			if (LightmapEditorSettings.GetInputSystemHash(renderer, out hash3))
			{
				EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.RealtimeLMInputSystemHash, new GUIContent(hash3.ToString()), new GUILayoutOption[0]);
			}
			int num3;
			int num4;
			if (LightmapEditorSettings.GetSystemResolution(renderer, out num3, out num4))
			{
				EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.RealtimeLMResolution, new GUIContent(num3.ToString() + "x" + num4.ToString()), new GUILayoutOption[0]);
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
			return InternalMeshUtil.HasNormals(mesh);
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
				GUILayout.Label(string.Format("Terrain is chunked up into {0} instances for baking.", num * num2), EditorStyles.helpBox, new GUILayoutOption[0]);
			}
		}

		private void ShowBakePerformanceWarning(SerializedObject so, Terrain terrain)
		{
			float x = terrain.terrainData.size.x;
			float z = terrain.terrainData.size.z;
			LightmapParameters lightmapParameters = ((LightmapParameters)so.FindProperty("m_LightmapParameters").objectReferenceValue) ?? new LightmapParameters();
			float num = x * lightmapParameters.resolution * LightmapEditorSettings.realtimeResolution;
			float num2 = z * lightmapParameters.resolution * LightmapEditorSettings.realtimeResolution;
			if (num > 512f || num2 > 512f)
			{
				EditorGUILayout.HelpBox("Baking resolution for this terrain probably is TOO HIGH. Try use a lower resolution parameter set otherwise it may take long or even infinite time to bake and memory consumption during baking may get greatly increased as well.", MessageType.Warning);
			}
			float num3 = num * lightmapParameters.clusterResolution;
			float num4 = num2 * lightmapParameters.clusterResolution;
			float num5 = (float)terrain.terrainData.heightmapResolution / num3;
			float num6 = (float)terrain.terrainData.heightmapResolution / num4;
			if (num5 > 51.2f || num6 > 51.2f)
			{
				EditorGUILayout.HelpBox("Baking resolution for this terrain probably is TOO LOW. If it takes long time in Clustering stage, try use a higher resolution parameter set.", MessageType.Warning);
			}
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
				EditorGUILayout.LabelField(LightingWindowObjectTab.s_Styles.RealtimeLMResolution, new GUIContent(text), new GUILayoutOption[0]);
			}
			EditorGUI.indentLevel--;
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
				result = true;
			}
			return result;
		}

		public static bool LightmapParametersGUI(SerializedProperty prop, GUIContent content, bool advancedParameters)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (advancedParameters)
			{
				EditorGUIInternal.AssetPopup<LightmapParameters>(prop, content, "giparams", "Default scene parameter");
			}
			else
			{
				EditorGUIInternal.AssetPopup<LightmapParameters>(prop, content, "giparams", "Default-Medium");
			}
			string text = "Edit...";
			if (LightingWindowObjectTab.isBuiltIn(prop))
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
	}
}
