using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CustomEditor(typeof(Terrain))]
	internal class TerrainInspector : Editor
	{
		private class Styles
		{
			public GUIStyle gridList = "GridList";

			public GUIStyle gridListText = "GridListText";

			public GUIStyle label = "RightLabel";

			public GUIStyle largeSquare = "Button";

			public GUIStyle command = "Command";

			public Texture settingsIcon = EditorGUIUtility.IconContent("SettingsIcon").image;

			public GUIContent[] toolIcons = new GUIContent[]
			{
				EditorGUIUtility.IconContent("TerrainInspector.TerrainToolRaise", "|Raise and lower the terrain height."),
				EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSetHeight", "|Set the terrain height."),
				EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSmoothHeight", "|Smooth the terrain height."),
				EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat", "|Paint the terrain texture."),
				EditorGUIUtility.IconContent("TerrainInspector.TerrainToolTrees", "|Place trees"),
				EditorGUIUtility.IconContent("TerrainInspector.TerrainToolPlants", "|Place plants, stones and other small foilage"),
				EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSettings", "|Settings for the terrain")
			};

			public GUIContent[] toolNames = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Raise / Lower Terrain|Click to raise. Hold down shift to lower."),
				EditorGUIUtility.TextContent("Paint Height|Hold shift to sample target height."),
				EditorGUIUtility.TextContent("Smooth Height"),
				EditorGUIUtility.TextContent("Paint Texture|Select a texture below, then click to paint"),
				EditorGUIUtility.TextContent("Place Trees|Hold down shift to erase trees.\nHold down ctrl to erase the selected tree type."),
				EditorGUIUtility.TextContent("Paint Details|Hold down shift to erase.\nHold down ctrl to erase the selected detail type."),
				EditorGUIUtility.TextContent("Terrain Settings")
			};

			public GUIContent brushSize = EditorGUIUtility.TextContent("Brush Size|Size of the brush used to paint");

			public GUIContent opacity = EditorGUIUtility.TextContent("Opacity|Strength of the applied effect");

			public GUIContent settings = EditorGUIUtility.TextContent("Settings");

			public GUIContent brushes = EditorGUIUtility.TextContent("Brushes");

			public GUIContent mismatchedTerrainData = EditorGUIUtility.TextContentWithIcon("The TerrainData used by the TerrainCollider component is different from this terrain. Would you like to assign the same TerrainData to the TerrainCollider component?", "console.warnicon");

			public GUIContent assign = EditorGUIUtility.TextContent("Assign");

			public GUIContent textures = EditorGUIUtility.TextContent("Textures");

			public GUIContent editTextures = EditorGUIUtility.TextContent("Edit Textures...");

			public GUIContent trees = EditorGUIUtility.TextContent("Trees");

			public GUIContent noTrees = EditorGUIUtility.TextContent("No Trees defined|Use edit button below to add new tree types.");

			public GUIContent editTrees = EditorGUIUtility.TextContent("Edit Trees...|Add/remove tree types.");

			public GUIContent treeDensity = EditorGUIUtility.TextContent("Tree Density|How dense trees are you painting");

			public GUIContent treeHeight = EditorGUIUtility.TextContent("Tree Height|Height of the planted trees");

			public GUIContent treeHeightRandomLabel = EditorGUIUtility.TextContent("Random?|Enable random variation in tree height (variation)");

			public GUIContent treeHeightRandomToggle = EditorGUIUtility.TextContent("|Enable random variation in tree height (variation)");

			public GUIContent lockWidth = EditorGUIUtility.TextContent("Lock Width to Height|Let the tree width be the same with height");

			public GUIContent treeWidth = EditorGUIUtility.TextContent("Tree Width|Width of the planted trees");

			public GUIContent treeWidthRandomLabel = EditorGUIUtility.TextContent("Random?|Enable random variation in tree width (variation)");

			public GUIContent treeWidthRandomToggle = EditorGUIUtility.TextContent("|Enable random variation in tree width (variation)");

			public GUIContent treeColorVar = EditorGUIUtility.TextContent("Color Variation|Amount of random shading applied to trees");

			public GUIContent treeRotation = EditorGUIUtility.TextContent("Random Tree Rotation|Enable?");

			public GUIContent massPlaceTrees = EditorGUIUtility.TextContent("Mass Place Trees");

			public GUIContent details = EditorGUIUtility.TextContent("Details");

			public GUIContent editDetails = EditorGUIUtility.TextContent("Edit Details...|Add/remove detail meshes");

			public GUIContent detailTargetStrength = EditorGUIUtility.TextContent("Target Strength|Target amount");

			public GUIContent heightmap = EditorGUIUtility.TextContent("Heightmap");

			public GUIContent importRaw = EditorGUIUtility.TextContent("Import Raw...");

			public GUIContent exportRaw = EditorGUIUtility.TextContent("Export Raw...");

			public GUIContent flatten = EditorGUIUtility.TextContent("Flatten");

			public GUIContent overrideSmoothness = EditorGUIUtility.TextContent("Override Smoothness|If checked, the smoothness value specified below will be used for all splat layers, otherwise smoothness of each individual splat layer will be controlled by the alpha channel of the splat texture.");

			public GUIContent bakeLightProbesForTrees = EditorGUIUtility.TextContent("Bake Light Probes For Trees|If the option is enabled, Unity will create internal light probes at the position of each tree (these probes are internal and will not affect other renderers in the scene) and apply them to tree renderers for lighting. Otherwise trees are still affected by LightProbeGroups. The option is only effective for trees that have LightProbe enabled on their prototype prefab.");

			public GUIContent resolution = EditorGUIUtility.TextContent("Resolution");

			public GUIContent refresh = EditorGUIUtility.TextContent("Refresh");
		}

		private const float kHeightmapBrushScale = 0.01f;

		private const float kMinBrushStrength = 0.00167849252f;

		private static TerrainInspector.Styles styles;

		internal static PrefKey[] s_ToolKeys = new PrefKey[]
		{
			new PrefKey("Terrain/Raise Height", "f1"),
			new PrefKey("Terrain/Set Height", "f2"),
			new PrefKey("Terrain/Smooth Height", "f3"),
			new PrefKey("Terrain/Texture Paint", "f4"),
			new PrefKey("Terrain/Tree Brush", "f5"),
			new PrefKey("Terrain/Detail Brush", "f6")
		};

		internal static PrefKey s_PrevBrush = new PrefKey("Terrain/Previous Brush", ",");

		internal static PrefKey s_NextBrush = new PrefKey("Terrain/Next Brush", ".");

		internal static PrefKey s_PrevTexture = new PrefKey("Terrain/Previous Detail", "#,");

		internal static PrefKey s_NextTexture = new PrefKey("Terrain/Next Detail", "#.");

		private Terrain m_Terrain;

		private TerrainCollider m_TerrainCollider;

		private Texture2D[] m_SplatIcons;

		private GUIContent[] m_TreeContents;

		private GUIContent[] m_DetailContents;

		private SavedFloat m_TargetHeight = new SavedFloat("TerrainBrushTargetHeight", 0.2f);

		private SavedFloat m_Strength = new SavedFloat("TerrainBrushStrength", 0.5f);

		private SavedInt m_Size = new SavedInt("TerrainBrushSize", 25);

		private SavedFloat m_SplatAlpha = new SavedFloat("TerrainBrushSplatAlpha", 1f);

		private SavedFloat m_DetailOpacity = new SavedFloat("TerrainDetailOpacity", 1f);

		private SavedFloat m_DetailStrength = new SavedFloat("TerrainDetailStrength", 0.8f);

		private Brush m_CachedBrush;

		private static Texture2D[] s_BrushTextures = null;

		private int m_SelectedBrush;

		private int m_SelectedSplat;

		private int m_SelectedDetail;

		private static int s_TerrainEditorHash = "TerrainEditor".GetHashCode();

		private List<ReflectionProbeBlendInfo> m_BlendInfoList = new List<ReflectionProbeBlendInfo>();

		private AnimBool m_ShowBuiltinSpecularSettings = new AnimBool();

		private AnimBool m_ShowCustomMaterialSettings = new AnimBool();

		private AnimBool m_ShowReflectionProbesGUI = new AnimBool();

		private bool m_LODTreePrototypePresent;

		private SavedInt m_SelectedTool = new SavedInt("TerrainSelectedTool", 0);

		private TerrainTool selectedTool
		{
			get
			{
				if (Tools.current == Tool.None)
				{
					return (TerrainTool)this.m_SelectedTool.value;
				}
				return TerrainTool.None;
			}
			set
			{
				if (value != TerrainTool.None)
				{
					Tools.current = Tool.None;
				}
				this.m_SelectedTool.value = (int)value;
			}
		}

		private static float PercentSlider(GUIContent content, float valueInPercent, float minVal, float maxVal)
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			float num = EditorGUILayout.Slider(content, Mathf.Round(valueInPercent * 100f), minVal * 100f, maxVal * 100f, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				return num / 100f;
			}
			GUI.changed = changed;
			return valueInPercent;
		}

		private void CheckKeys()
		{
			for (int i = 0; i < TerrainInspector.s_ToolKeys.Length; i++)
			{
				if (TerrainInspector.s_ToolKeys[i].activated)
				{
					this.selectedTool = (TerrainTool)i;
					base.Repaint();
					Event.current.Use();
				}
			}
			if (TerrainInspector.s_PrevBrush.activated)
			{
				this.m_SelectedBrush--;
				if (this.m_SelectedBrush < 0)
				{
					this.m_SelectedBrush = TerrainInspector.s_BrushTextures.Length - 1;
				}
				base.Repaint();
				Event.current.Use();
			}
			if (TerrainInspector.s_NextBrush.activated)
			{
				this.m_SelectedBrush++;
				if (this.m_SelectedBrush >= TerrainInspector.s_BrushTextures.Length)
				{
					this.m_SelectedBrush = 0;
				}
				base.Repaint();
				Event.current.Use();
			}
			int num = 0;
			if (TerrainInspector.s_NextTexture.activated)
			{
				num = 1;
			}
			if (TerrainInspector.s_PrevTexture.activated)
			{
				num = -1;
			}
			if (num != 0)
			{
				switch (this.selectedTool)
				{
				case TerrainTool.PaintTexture:
					this.m_SelectedSplat = (int)Mathf.Repeat((float)(this.m_SelectedSplat + num), (float)this.m_Terrain.terrainData.splatPrototypes.Length);
					Event.current.Use();
					base.Repaint();
					break;
				case TerrainTool.PlaceTree:
					if (TreePainter.selectedTree >= 0)
					{
						TreePainter.selectedTree = (int)Mathf.Repeat((float)(TreePainter.selectedTree + num), (float)this.m_TreeContents.Length);
					}
					else if (num == -1 && this.m_TreeContents.Length > 0)
					{
						TreePainter.selectedTree = this.m_TreeContents.Length - 1;
					}
					else if (num == 1 && this.m_TreeContents.Length > 0)
					{
						TreePainter.selectedTree = 0;
					}
					Event.current.Use();
					base.Repaint();
					break;
				case TerrainTool.PaintDetail:
					this.m_SelectedDetail = (int)Mathf.Repeat((float)(this.m_SelectedDetail + num), (float)this.m_Terrain.terrainData.detailPrototypes.Length);
					Event.current.Use();
					base.Repaint();
					break;
				}
			}
		}

		private void LoadBrushIcons()
		{
			ArrayList arrayList = new ArrayList();
			int num = 1;
			Texture texture;
			do
			{
				texture = (Texture2D)EditorGUIUtility.Load(string.Concat(new object[]
				{
					EditorResourcesUtility.brushesPath,
					"builtin_brush_",
					num,
					".png"
				}));
				if (texture)
				{
					arrayList.Add(texture);
				}
				num++;
			}
			while (texture);
			num = 0;
			do
			{
				texture = EditorGUIUtility.FindTexture("brush_" + num + ".png");
				if (texture)
				{
					arrayList.Add(texture);
				}
				num++;
			}
			while (texture);
			TerrainInspector.s_BrushTextures = (arrayList.ToArray(typeof(Texture2D)) as Texture2D[]);
		}

		private void Initialize()
		{
			this.m_Terrain = (this.target as Terrain);
			if (TerrainInspector.s_BrushTextures == null)
			{
				this.LoadBrushIcons();
			}
		}

		public void OnEnable()
		{
			this.m_ShowBuiltinSpecularSettings.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowCustomMaterialSettings.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowReflectionProbesGUI.valueChanged.AddListener(new UnityAction(base.Repaint));
			Terrain terrain = this.target as Terrain;
			if (terrain != null)
			{
				this.m_ShowBuiltinSpecularSettings.value = (terrain.materialType == Terrain.MaterialType.BuiltInLegacySpecular);
				this.m_ShowCustomMaterialSettings.value = (terrain.materialType == Terrain.MaterialType.Custom);
				this.m_ShowReflectionProbesGUI.value = (terrain.materialType == Terrain.MaterialType.BuiltInStandard || terrain.materialType == Terrain.MaterialType.Custom);
			}
		}

		public void OnDisable()
		{
			this.m_ShowReflectionProbesGUI.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowCustomMaterialSettings.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowBuiltinSpecularSettings.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			if (this.m_CachedBrush != null)
			{
				this.m_CachedBrush.Dispose();
			}
		}

		private static string IntString(float p)
		{
			return ((int)p).ToString();
		}

		public void MenuButton(GUIContent title, string menuName, int userData)
		{
			GUIContent content = new GUIContent(title.text, TerrainInspector.styles.settingsIcon, title.tooltip);
			Rect rect = GUILayoutUtility.GetRect(content, TerrainInspector.styles.largeSquare);
			if (GUI.Button(rect, content, TerrainInspector.styles.largeSquare))
			{
				MenuCommand command = new MenuCommand(this.m_Terrain, userData);
				EditorUtility.DisplayPopupMenu(new Rect(rect.x, rect.y, 0f, 0f), menuName, command);
			}
		}

		public static int AspectSelectionGrid(int selected, Texture[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
		{
			GUILayout.BeginVertical("box", new GUILayoutOption[]
			{
				GUILayout.MinHeight(10f)
			});
			int result = 0;
			doubleClick = false;
			if (textures.Length != 0)
			{
				float num = (EditorGUIUtility.currentViewWidth - 20f) / (float)approxSize;
				int num2 = (int)Mathf.Ceil((float)textures.Length / num);
				Rect aspectRect = GUILayoutUtility.GetAspectRect(num / (float)num2);
				Event current = Event.current;
				if (current.type == EventType.MouseDown && current.clickCount == 2 && aspectRect.Contains(current.mousePosition))
				{
					doubleClick = true;
					current.Use();
				}
				result = GUI.SelectionGrid(aspectRect, selected, textures, Mathf.RoundToInt(EditorGUIUtility.currentViewWidth - 20f) / approxSize, style);
			}
			else
			{
				GUILayout.Label(emptyString, new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			return result;
		}

		private static Rect GetBrushAspectRect(int elementCount, int approxSize, int extraLineHeight, out int xCount)
		{
			xCount = (int)Mathf.Ceil((EditorGUIUtility.currentViewWidth - 20f) / (float)approxSize);
			int num = elementCount / xCount;
			if (elementCount % xCount != 0)
			{
				num++;
			}
			Rect aspectRect = GUILayoutUtility.GetAspectRect((float)xCount / (float)num);
			Rect rect = GUILayoutUtility.GetRect(10f, (float)(extraLineHeight * num));
			aspectRect.height += rect.height;
			return aspectRect;
		}

		public static int AspectSelectionGridImageAndText(int selected, GUIContent[] textures, int approxSize, GUIStyle style, string emptyString, out bool doubleClick)
		{
			EditorGUILayout.BeginVertical(GUIContent.none, EditorStyles.helpBox, new GUILayoutOption[]
			{
				GUILayout.MinHeight(10f)
			});
			int result = 0;
			doubleClick = false;
			if (textures.Length != 0)
			{
				int xCount = 0;
				Rect brushAspectRect = TerrainInspector.GetBrushAspectRect(textures.Length, approxSize, 12, out xCount);
				Event current = Event.current;
				if (current.type == EventType.MouseDown && current.clickCount == 2 && brushAspectRect.Contains(current.mousePosition))
				{
					doubleClick = true;
					current.Use();
				}
				result = GUI.SelectionGrid(brushAspectRect, selected, textures, xCount, style);
			}
			else
			{
				GUILayout.Label(emptyString, new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			return result;
		}

		private void LoadSplatIcons()
		{
			SplatPrototype[] splatPrototypes = this.m_Terrain.terrainData.splatPrototypes;
			this.m_SplatIcons = new Texture2D[splatPrototypes.Length];
			for (int i = 0; i < this.m_SplatIcons.Length; i++)
			{
				this.m_SplatIcons[i] = (AssetPreview.GetAssetPreview(splatPrototypes[i].texture) ?? splatPrototypes[i].texture);
			}
		}

		private void LoadTreeIcons()
		{
			TreePrototype[] treePrototypes = this.m_Terrain.terrainData.treePrototypes;
			this.m_TreeContents = new GUIContent[treePrototypes.Length];
			for (int i = 0; i < this.m_TreeContents.Length; i++)
			{
				this.m_TreeContents[i] = new GUIContent();
				Texture assetPreview = AssetPreview.GetAssetPreview(treePrototypes[i].prefab);
				if (assetPreview != null)
				{
					this.m_TreeContents[i].image = assetPreview;
				}
				if (treePrototypes[i].prefab != null)
				{
					this.m_TreeContents[i].text = treePrototypes[i].prefab.name;
				}
				else
				{
					this.m_TreeContents[i].text = "Missing";
				}
			}
		}

		private void LoadDetailIcons()
		{
			DetailPrototype[] detailPrototypes = this.m_Terrain.terrainData.detailPrototypes;
			this.m_DetailContents = new GUIContent[detailPrototypes.Length];
			for (int i = 0; i < this.m_DetailContents.Length; i++)
			{
				this.m_DetailContents[i] = new GUIContent();
				if (detailPrototypes[i].usePrototypeMesh)
				{
					Texture assetPreview = AssetPreview.GetAssetPreview(detailPrototypes[i].prototype);
					if (assetPreview != null)
					{
						this.m_DetailContents[i].image = assetPreview;
					}
					if (detailPrototypes[i].prototype != null)
					{
						this.m_DetailContents[i].text = detailPrototypes[i].prototype.name;
					}
					else
					{
						this.m_DetailContents[i].text = "Missing";
					}
				}
				else
				{
					Texture prototypeTexture = detailPrototypes[i].prototypeTexture;
					if (prototypeTexture != null)
					{
						this.m_DetailContents[i].image = prototypeTexture;
					}
					if (prototypeTexture != null)
					{
						this.m_DetailContents[i].text = prototypeTexture.name;
					}
					else
					{
						this.m_DetailContents[i].text = "Missing";
					}
				}
			}
		}

		public void ShowTrees()
		{
			this.LoadTreeIcons();
			GUI.changed = false;
			this.ShowUpgradeTreePrototypeScaleUI();
			GUILayout.Label(TerrainInspector.styles.trees, EditorStyles.boldLabel, new GUILayoutOption[0]);
			bool flag;
			TreePainter.selectedTree = TerrainInspector.AspectSelectionGridImageAndText(TreePainter.selectedTree, this.m_TreeContents, 64, TerrainInspector.styles.gridListText, "No trees defined", out flag);
			if (TreePainter.selectedTree >= this.m_TreeContents.Length)
			{
				TreePainter.selectedTree = -1;
			}
			if (flag)
			{
				TerrainTreeContextMenus.EditTree(new MenuCommand(this.m_Terrain, TreePainter.selectedTree));
				GUIUtility.ExitGUI();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.ShowMassPlaceTrees();
			GUILayout.FlexibleSpace();
			this.MenuButton(TerrainInspector.styles.editTrees, "CONTEXT/TerrainEngineTrees", TreePainter.selectedTree);
			this.ShowRefreshPrototypes();
			GUILayout.EndHorizontal();
			if (TreePainter.selectedTree == -1)
			{
				return;
			}
			GUILayout.Label(TerrainInspector.styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			TreePainter.brushSize = EditorGUILayout.Slider(TerrainInspector.styles.brushSize, TreePainter.brushSize, 1f, 100f, new GUILayoutOption[0]);
			float num = (3.3f - TreePainter.spacing) / 3f;
			float num2 = TerrainInspector.PercentSlider(TerrainInspector.styles.treeDensity, num, 0.1f, 1f);
			if (num2 != num)
			{
				TreePainter.spacing = (1.1f - num2) * 3f;
			}
			GUILayout.Space(5f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(TerrainInspector.styles.treeHeight, new GUILayoutOption[]
			{
				GUILayout.Width(EditorGUIUtility.labelWidth - 6f)
			});
			GUILayout.Label(TerrainInspector.styles.treeHeightRandomLabel, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			TreePainter.allowHeightVar = GUILayout.Toggle(TreePainter.allowHeightVar, TerrainInspector.styles.treeHeightRandomToggle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (TreePainter.allowHeightVar)
			{
				EditorGUI.BeginChangeCheck();
				float num3 = TreePainter.treeHeight * (1f - TreePainter.treeHeightVariation);
				float num4 = TreePainter.treeHeight * (1f + TreePainter.treeHeightVariation);
				EditorGUILayout.MinMaxSlider(ref num3, ref num4, 0.01f, 2f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					TreePainter.treeHeight = (num3 + num4) * 0.5f;
					TreePainter.treeHeightVariation = (num4 - num3) / (num3 + num4);
				}
			}
			else
			{
				TreePainter.treeHeight = EditorGUILayout.Slider(TreePainter.treeHeight, 0.01f, 2f, new GUILayoutOption[0]);
				TreePainter.treeHeightVariation = 0f;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			TreePainter.lockWidthToHeight = EditorGUILayout.Toggle(TerrainInspector.styles.lockWidth, TreePainter.lockWidthToHeight, new GUILayoutOption[0]);
			if (TreePainter.lockWidthToHeight)
			{
				TreePainter.treeWidth = TreePainter.treeHeight;
				TreePainter.treeWidthVariation = TreePainter.treeHeightVariation;
				TreePainter.allowWidthVar = TreePainter.allowHeightVar;
			}
			GUILayout.Space(5f);
			using (new EditorGUI.DisabledScope(TreePainter.lockWidthToHeight))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(TerrainInspector.styles.treeWidth, new GUILayoutOption[]
				{
					GUILayout.Width(EditorGUIUtility.labelWidth - 6f)
				});
				GUILayout.Label(TerrainInspector.styles.treeWidthRandomLabel, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				TreePainter.allowWidthVar = GUILayout.Toggle(TreePainter.allowWidthVar, TerrainInspector.styles.treeWidthRandomToggle, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (TreePainter.allowWidthVar)
				{
					EditorGUI.BeginChangeCheck();
					float num5 = TreePainter.treeWidth * (1f - TreePainter.treeWidthVariation);
					float num6 = TreePainter.treeWidth * (1f + TreePainter.treeWidthVariation);
					EditorGUILayout.MinMaxSlider(ref num5, ref num6, 0.01f, 2f, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						TreePainter.treeWidth = (num5 + num6) * 0.5f;
						TreePainter.treeWidthVariation = (num6 - num5) / (num5 + num6);
					}
				}
				else
				{
					TreePainter.treeWidth = EditorGUILayout.Slider(TreePainter.treeWidth, 0.01f, 2f, new GUILayoutOption[0]);
					TreePainter.treeWidthVariation = 0f;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.Space(5f);
			if (TerrainEditorUtility.IsLODTreePrototype(this.m_Terrain.terrainData.treePrototypes[TreePainter.selectedTree].m_Prefab))
			{
				TreePainter.randomRotation = EditorGUILayout.Toggle(TerrainInspector.styles.treeRotation, TreePainter.randomRotation, new GUILayoutOption[0]);
			}
			else
			{
				TreePainter.treeColorAdjustment = EditorGUILayout.Slider(TerrainInspector.styles.treeColorVar, TreePainter.treeColorAdjustment, 0f, 1f, new GUILayoutOption[0]);
			}
		}

		public void ShowDetails()
		{
			this.LoadDetailIcons();
			this.ShowBrushes();
			GUI.changed = false;
			GUILayout.Label(TerrainInspector.styles.details, EditorStyles.boldLabel, new GUILayoutOption[0]);
			bool flag;
			this.m_SelectedDetail = TerrainInspector.AspectSelectionGridImageAndText(this.m_SelectedDetail, this.m_DetailContents, 64, TerrainInspector.styles.gridListText, "No Detail Objects defined", out flag);
			if (flag)
			{
				TerrainDetailContextMenus.EditDetail(new MenuCommand(this.m_Terrain, this.m_SelectedDetail));
				GUIUtility.ExitGUI();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			this.MenuButton(TerrainInspector.styles.editDetails, "CONTEXT/TerrainEngineDetails", this.m_SelectedDetail);
			this.ShowRefreshPrototypes();
			GUILayout.EndHorizontal();
			GUILayout.Label(TerrainInspector.styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.m_Size.value = Mathf.RoundToInt(EditorGUILayout.Slider(TerrainInspector.styles.brushSize, (float)this.m_Size, 1f, 100f, new GUILayoutOption[0]));
			this.m_DetailOpacity.value = EditorGUILayout.Slider(TerrainInspector.styles.opacity, this.m_DetailOpacity, 0f, 1f, new GUILayoutOption[0]);
			this.m_DetailStrength.value = EditorGUILayout.Slider(TerrainInspector.styles.detailTargetStrength, this.m_DetailStrength, 0f, 1f, new GUILayoutOption[0]);
			this.m_DetailStrength.value = Mathf.Round(this.m_DetailStrength * 16f) / 16f;
		}

		public void ShowSettings()
		{
			TerrainData terrainData = this.m_Terrain.terrainData;
			EditorGUI.BeginChangeCheck();
			GUILayout.Label("Base Terrain", EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.m_Terrain.drawHeightmap = EditorGUILayout.Toggle("Draw", this.m_Terrain.drawHeightmap, new GUILayoutOption[0]);
			this.m_Terrain.heightmapPixelError = EditorGUILayout.Slider("Pixel Error", this.m_Terrain.heightmapPixelError, 1f, 200f, new GUILayoutOption[0]);
			this.m_Terrain.basemapDistance = EditorGUILayout.Slider("Base Map Dist.", this.m_Terrain.basemapDistance, 0f, 2000f, new GUILayoutOption[0]);
			this.m_Terrain.castShadows = EditorGUILayout.Toggle("Cast Shadows", this.m_Terrain.castShadows, new GUILayoutOption[0]);
			this.m_Terrain.materialType = (Terrain.MaterialType)EditorGUILayout.EnumPopup("Material", this.m_Terrain.materialType, new GUILayoutOption[0]);
			if (this.m_Terrain.materialType != Terrain.MaterialType.Custom)
			{
				this.m_Terrain.materialTemplate = null;
			}
			this.m_ShowBuiltinSpecularSettings.target = (this.m_Terrain.materialType == Terrain.MaterialType.BuiltInLegacySpecular);
			this.m_ShowCustomMaterialSettings.target = (this.m_Terrain.materialType == Terrain.MaterialType.Custom);
			this.m_ShowReflectionProbesGUI.target = (this.m_Terrain.materialType == Terrain.MaterialType.BuiltInStandard || this.m_Terrain.materialType == Terrain.MaterialType.Custom);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowBuiltinSpecularSettings.faded))
			{
				EditorGUI.indentLevel++;
				this.m_Terrain.legacySpecular = EditorGUILayout.ColorField("Specular Color", this.m_Terrain.legacySpecular, new GUILayoutOption[0]);
				this.m_Terrain.legacyShininess = EditorGUILayout.Slider("Shininess", this.m_Terrain.legacyShininess, 0.03f, 1f, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowCustomMaterialSettings.faded))
			{
				EditorGUI.indentLevel++;
				this.m_Terrain.materialTemplate = (EditorGUILayout.ObjectField("Custom Material", this.m_Terrain.materialTemplate, typeof(Material), false, new GUILayoutOption[0]) as Material);
				if (this.m_Terrain.materialTemplate != null)
				{
					Shader shader = this.m_Terrain.materialTemplate.shader;
					if (ShaderUtil.HasTangentChannel(shader))
					{
						GUIContent gUIContent = EditorGUIUtility.TextContent("Can't use materials with shaders which need tangent geometry on terrain, use shaders in Nature/Terrain instead.");
						EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, false);
					}
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowReflectionProbesGUI.faded))
			{
				this.m_Terrain.reflectionProbeUsage = (ReflectionProbeUsage)EditorGUILayout.EnumPopup("Reflection Probes", this.m_Terrain.reflectionProbeUsage, new GUILayoutOption[0]);
				if (this.m_Terrain.reflectionProbeUsage != ReflectionProbeUsage.Off)
				{
					EditorGUI.indentLevel++;
					this.m_Terrain.GetClosestReflectionProbes(this.m_BlendInfoList);
					RendererEditorBase.Probes.ShowClosestReflectionProbes(this.m_BlendInfoList);
					EditorGUI.indentLevel--;
				}
			}
			EditorGUILayout.EndFadeGroup();
			terrainData.thickness = EditorGUILayout.FloatField("Thickness", terrainData.thickness, new GUILayoutOption[0]);
			GUILayout.Label("Tree & Detail Objects", EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.m_Terrain.drawTreesAndFoliage = EditorGUILayout.Toggle("Draw", this.m_Terrain.drawTreesAndFoliage, new GUILayoutOption[0]);
			this.m_Terrain.bakeLightProbesForTrees = EditorGUILayout.Toggle(TerrainInspector.styles.bakeLightProbesForTrees, this.m_Terrain.bakeLightProbesForTrees, new GUILayoutOption[0]);
			this.m_Terrain.detailObjectDistance = EditorGUILayout.Slider("Detail Distance", this.m_Terrain.detailObjectDistance, 0f, 250f, new GUILayoutOption[0]);
			this.m_Terrain.collectDetailPatches = EditorGUILayout.Toggle("Collect Detail Patches", this.m_Terrain.collectDetailPatches, new GUILayoutOption[0]);
			this.m_Terrain.detailObjectDensity = EditorGUILayout.Slider("Detail Density", this.m_Terrain.detailObjectDensity, 0f, 1f, new GUILayoutOption[0]);
			this.m_Terrain.treeDistance = EditorGUILayout.Slider("Tree Distance", this.m_Terrain.treeDistance, 0f, 2000f, new GUILayoutOption[0]);
			this.m_Terrain.treeBillboardDistance = EditorGUILayout.Slider("Billboard Start", this.m_Terrain.treeBillboardDistance, 5f, 2000f, new GUILayoutOption[0]);
			this.m_Terrain.treeCrossFadeLength = EditorGUILayout.Slider("Fade Length", this.m_Terrain.treeCrossFadeLength, 0f, 200f, new GUILayoutOption[0]);
			this.m_Terrain.treeMaximumFullLODCount = EditorGUILayout.IntSlider("Max Mesh Trees", this.m_Terrain.treeMaximumFullLODCount, 0, 10000, new GUILayoutOption[0]);
			if (Event.current.type == EventType.Layout)
			{
				this.m_LODTreePrototypePresent = false;
				for (int i = 0; i < terrainData.treePrototypes.Length; i++)
				{
					if (TerrainEditorUtility.IsLODTreePrototype(terrainData.treePrototypes[i].prefab))
					{
						this.m_LODTreePrototypePresent = true;
						break;
					}
				}
			}
			if (this.m_LODTreePrototypePresent)
			{
				EditorGUILayout.HelpBox("Tree Distance, Billboard Start, Fade Length and Max Mesh Trees have no effect on SpeedTree trees. Please use the LOD Group component on the tree prefab to control LOD settings.", MessageType.Info);
			}
			if (EditorGUI.EndChangeCheck())
			{
				EditorApplication.SetSceneRepaintDirty();
				EditorUtility.SetDirty(this.m_Terrain);
				if (!EditorApplication.isPlaying)
				{
					EditorSceneManager.MarkSceneDirty(this.m_Terrain.gameObject.scene);
				}
			}
			EditorGUI.BeginChangeCheck();
			GUILayout.Label("Wind Settings for Grass", EditorStyles.boldLabel, new GUILayoutOption[0]);
			float wavingGrassStrength = EditorGUILayout.Slider("Speed", terrainData.wavingGrassStrength, 0f, 1f, new GUILayoutOption[0]);
			float wavingGrassSpeed = EditorGUILayout.Slider("Size", terrainData.wavingGrassSpeed, 0f, 1f, new GUILayoutOption[0]);
			float wavingGrassAmount = EditorGUILayout.Slider("Bending", terrainData.wavingGrassAmount, 0f, 1f, new GUILayoutOption[0]);
			Color wavingGrassTint = EditorGUILayout.ColorField("Grass Tint", terrainData.wavingGrassTint, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				terrainData.wavingGrassStrength = wavingGrassStrength;
				terrainData.wavingGrassSpeed = wavingGrassSpeed;
				terrainData.wavingGrassAmount = wavingGrassAmount;
				terrainData.wavingGrassTint = wavingGrassTint;
				if (!EditorUtility.IsPersistent(terrainData) && !EditorApplication.isPlaying)
				{
					EditorSceneManager.MarkSceneDirty(this.m_Terrain.gameObject.scene);
				}
			}
			this.ShowResolution();
			this.ShowHeightmaps();
		}

		public void ShowRaiseHeight()
		{
			this.ShowBrushes();
			GUILayout.Label(TerrainInspector.styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.ShowBrushSettings();
		}

		public void ShowSmoothHeight()
		{
			this.ShowBrushes();
			GUILayout.Label(TerrainInspector.styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.ShowBrushSettings();
		}

		public void ShowTextures()
		{
			this.LoadSplatIcons();
			this.ShowBrushes();
			GUILayout.Label(TerrainInspector.styles.textures, EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUI.changed = false;
			bool flag;
			this.m_SelectedSplat = TerrainInspector.AspectSelectionGrid(this.m_SelectedSplat, this.m_SplatIcons, 64, TerrainInspector.styles.gridList, "No terrain textures defined.", out flag);
			if (flag)
			{
				TerrainSplatContextMenus.EditSplat(new MenuCommand(this.m_Terrain, this.m_SelectedSplat));
				GUIUtility.ExitGUI();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			this.MenuButton(TerrainInspector.styles.editTextures, "CONTEXT/TerrainEngineSplats", this.m_SelectedSplat);
			GUILayout.EndHorizontal();
			GUILayout.Label(TerrainInspector.styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.ShowBrushSettings();
			this.m_SplatAlpha.value = EditorGUILayout.Slider("Target Strength", this.m_SplatAlpha, 0f, 1f, new GUILayoutOption[0]);
		}

		public void ShowBrushes()
		{
			GUILayout.Label(TerrainInspector.styles.brushes, EditorStyles.boldLabel, new GUILayoutOption[0]);
			bool flag;
			this.m_SelectedBrush = TerrainInspector.AspectSelectionGrid(this.m_SelectedBrush, TerrainInspector.s_BrushTextures, 32, TerrainInspector.styles.gridList, "No brushes defined.", out flag);
		}

		public void ShowHeightmaps()
		{
			GUILayout.Label(TerrainInspector.styles.heightmap, EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(TerrainInspector.styles.importRaw, new GUILayoutOption[0]))
			{
				TerrainMenus.ImportRaw();
			}
			if (GUILayout.Button(TerrainInspector.styles.exportRaw, new GUILayoutOption[0]))
			{
				TerrainMenus.ExportHeightmapRaw();
			}
			GUILayout.EndHorizontal();
		}

		public void ShowResolution()
		{
			GUILayout.Label("Resolution", EditorStyles.boldLabel, new GUILayoutOption[0]);
			float num = this.m_Terrain.terrainData.size.x;
			float num2 = this.m_Terrain.terrainData.size.y;
			float num3 = this.m_Terrain.terrainData.size.z;
			int num4 = this.m_Terrain.terrainData.heightmapResolution;
			int num5 = this.m_Terrain.terrainData.detailResolution;
			int num6 = this.m_Terrain.terrainData.detailResolutionPerPatch;
			int num7 = this.m_Terrain.terrainData.alphamapResolution;
			int num8 = this.m_Terrain.terrainData.baseMapResolution;
			EditorGUI.BeginChangeCheck();
			num = EditorGUILayout.DelayedFloatField(EditorGUIUtility.TempContent("Terrain Width"), num, new GUILayoutOption[0]);
			if (num <= 0f)
			{
				num = 1f;
			}
			if (num > 100000f)
			{
				num = 100000f;
			}
			num3 = EditorGUILayout.DelayedFloatField(EditorGUIUtility.TempContent("Terrain Length"), num3, new GUILayoutOption[0]);
			if (num3 <= 0f)
			{
				num3 = 1f;
			}
			if (num3 > 100000f)
			{
				num3 = 100000f;
			}
			num2 = EditorGUILayout.DelayedFloatField(EditorGUIUtility.TempContent("Terrain Height"), num2, new GUILayoutOption[0]);
			if (num2 <= 0f)
			{
				num2 = 1f;
			}
			if (num2 > 10000f)
			{
				num2 = 10000f;
			}
			num4 = EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Heightmap Resolution"), num4, new GUILayoutOption[0]);
			num4 = Mathf.Clamp(num4, 33, 4097);
			num4 = this.m_Terrain.terrainData.GetAdjustedSize(num4);
			num5 = EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Detail Resolution"), num5, new GUILayoutOption[0]);
			num5 = Mathf.Clamp(num5, 0, 4048);
			num6 = EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Detail Resolution Per Patch"), num6, new GUILayoutOption[0]);
			num6 = Mathf.Clamp(num6, 8, 128);
			num7 = EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Control Texture Resolution"), num7, new GUILayoutOption[0]);
			num7 = Mathf.Clamp(Mathf.ClosestPowerOfTwo(num7), 16, 2048);
			num8 = EditorGUILayout.DelayedIntField(EditorGUIUtility.TempContent("Base Texture Resolution"), num8, new GUILayoutOption[0]);
			num8 = Mathf.Clamp(Mathf.ClosestPowerOfTwo(num8), 16, 2048);
			if (EditorGUI.EndChangeCheck())
			{
				ArrayList arrayList = new ArrayList();
				arrayList.Add(this.m_Terrain.terrainData);
				arrayList.AddRange(this.m_Terrain.terrainData.alphamapTextures);
				Undo.RegisterCompleteObjectUndo(arrayList.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[], "Set Resolution");
				if (this.m_Terrain.terrainData.heightmapResolution != num4)
				{
					this.m_Terrain.terrainData.heightmapResolution = num4;
				}
				this.m_Terrain.terrainData.size = new Vector3(num, num2, num3);
				if (this.m_Terrain.terrainData.detailResolution != num5 || num6 != this.m_Terrain.terrainData.detailResolutionPerPatch)
				{
					this.ResizeDetailResolution(this.m_Terrain.terrainData, num5, num6);
				}
				if (this.m_Terrain.terrainData.alphamapResolution != num7)
				{
					this.m_Terrain.terrainData.alphamapResolution = num7;
				}
				if (this.m_Terrain.terrainData.baseMapResolution != num8)
				{
					this.m_Terrain.terrainData.baseMapResolution = num8;
				}
				this.m_Terrain.Flush();
			}
			EditorGUILayout.HelpBox("Please note that modifying the resolution of the heightmap, detail map and control texture will clear their contents, respectively.", MessageType.Warning);
		}

		private void ResizeDetailResolution(TerrainData terrainData, int resolution, int resolutionPerPatch)
		{
			if (resolution == terrainData.detailResolution)
			{
				List<int[,]> list = new List<int[,]>();
				for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
				{
					list.Add(terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i));
				}
				terrainData.SetDetailResolution(resolution, resolutionPerPatch);
				for (int j = 0; j < list.Count; j++)
				{
					terrainData.SetDetailLayer(0, 0, j, list[j]);
				}
			}
			else
			{
				terrainData.SetDetailResolution(resolution, resolutionPerPatch);
			}
		}

		public void ShowUpgradeTreePrototypeScaleUI()
		{
			if (this.m_Terrain.terrainData != null && this.m_Terrain.terrainData.NeedUpgradeScaledTreePrototypes())
			{
				GUIContent content = EditorGUIUtility.TempContent("Some of your prototypes have scaling values on the prefab. Since Unity 5.2 these scalings will be applied to terrain tree instances. Do you want to upgrade to this behaviour?", EditorGUIUtility.GetHelpIcon(MessageType.Warning));
				GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.Label(content, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				GUILayout.Space(3f);
				if (GUILayout.Button("Upgrade", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					this.m_Terrain.terrainData.UpgradeScaledTreePrototype();
					TerrainMenus.RefreshPrototypes();
				}
				GUILayout.Space(3f);
				GUILayout.EndVertical();
			}
		}

		public void ShowRefreshPrototypes()
		{
			if (GUILayout.Button(TerrainInspector.styles.refresh, new GUILayoutOption[0]))
			{
				TerrainMenus.RefreshPrototypes();
			}
		}

		public void ShowMassPlaceTrees()
		{
			using (new EditorGUI.DisabledScope(TreePainter.selectedTree == -1))
			{
				if (GUILayout.Button(TerrainInspector.styles.massPlaceTrees, new GUILayoutOption[0]))
				{
					TerrainMenus.MassPlaceTrees();
				}
			}
		}

		public void ShowBrushSettings()
		{
			this.m_Size.value = Mathf.RoundToInt(EditorGUILayout.Slider(TerrainInspector.styles.brushSize, (float)this.m_Size, 1f, 100f, new GUILayoutOption[0]));
			this.m_Strength.value = TerrainInspector.PercentSlider(TerrainInspector.styles.opacity, this.m_Strength, 0.00167849252f, 1f);
		}

		public void ShowSetHeight()
		{
			this.ShowBrushes();
			GUILayout.Label(TerrainInspector.styles.settings, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.ShowBrushSettings();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.changed = false;
			float num = this.m_TargetHeight * this.m_Terrain.terrainData.size.y;
			num = EditorGUILayout.Slider("Height", num, 0f, this.m_Terrain.terrainData.size.y, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				this.m_TargetHeight.value = num / this.m_Terrain.terrainData.size.y;
			}
			if (GUILayout.Button(TerrainInspector.styles.flatten, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				Undo.RegisterCompleteObjectUndo(this.m_Terrain.terrainData, "Flatten Heightmap");
				HeightmapFilters.Flatten(this.m_Terrain.terrainData, this.m_TargetHeight.value);
			}
			GUILayout.EndHorizontal();
		}

		private void OnInspectorUpdate()
		{
			if (AssetPreview.HasAnyNewPreviewTexturesAvailable())
			{
				base.Repaint();
			}
		}

		public override void OnInspectorGUI()
		{
			this.Initialize();
			if (TerrainInspector.styles == null)
			{
				TerrainInspector.styles = new TerrainInspector.Styles();
			}
			if (!this.m_Terrain.terrainData)
			{
				GUI.enabled = false;
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Toolbar(-1, TerrainInspector.styles.toolIcons, TerrainInspector.styles.command, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUI.enabled = true;
				GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.Label("Terrain Asset Missing", new GUILayoutOption[0]);
				this.m_Terrain.terrainData = (EditorGUILayout.ObjectField("Assign:", this.m_Terrain.terrainData, typeof(TerrainData), false, new GUILayoutOption[0]) as TerrainData);
				GUILayout.EndVertical();
				return;
			}
			if (Event.current.type == EventType.Layout)
			{
				this.m_TerrainCollider = this.m_Terrain.gameObject.GetComponent<TerrainCollider>();
			}
			if (this.m_TerrainCollider && this.m_TerrainCollider.terrainData != this.m_Terrain.terrainData)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.Label(TerrainInspector.styles.mismatchedTerrainData, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				GUILayout.Space(3f);
				if (GUILayout.Button(TerrainInspector.styles.assign, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					Undo.RecordObject(this.m_TerrainCollider, "Assign TerrainData");
					this.m_TerrainCollider.terrainData = this.m_Terrain.terrainData;
				}
				GUILayout.Space(3f);
				GUILayout.EndVertical();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.changed = false;
			int selectedTool = (int)this.selectedTool;
			this.selectedTool = (TerrainTool)GUILayout.Toolbar(selectedTool, TerrainInspector.styles.toolIcons, TerrainInspector.styles.command, new GUILayoutOption[0]);
			if (this.selectedTool != (TerrainTool)selectedTool && Toolbar.get != null)
			{
				Toolbar.get.Repaint();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			this.CheckKeys();
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			if (selectedTool >= 0 && selectedTool < TerrainInspector.styles.toolIcons.Length)
			{
				GUILayout.Label(TerrainInspector.styles.toolNames[selectedTool].text, new GUILayoutOption[0]);
				GUILayout.Label(TerrainInspector.styles.toolNames[selectedTool].tooltip, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			}
			else
			{
				GUILayout.Label("No tool selected", new GUILayoutOption[0]);
				GUILayout.Label("Please select a tool", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			switch (selectedTool)
			{
			case 0:
				this.ShowRaiseHeight();
				break;
			case 1:
				this.ShowSetHeight();
				break;
			case 2:
				this.ShowSmoothHeight();
				break;
			case 3:
				this.ShowTextures();
				break;
			case 4:
				this.ShowTrees();
				break;
			case 5:
				this.ShowDetails();
				break;
			case 6:
				this.ShowSettings();
				break;
			}
			GUILayout.Space(5f);
		}

		private Brush GetActiveBrush(int size)
		{
			if (this.m_CachedBrush == null)
			{
				this.m_CachedBrush = new Brush();
			}
			this.m_CachedBrush.Load(TerrainInspector.s_BrushTextures[this.m_SelectedBrush], size);
			return this.m_CachedBrush;
		}

		public bool Raycast(out Vector2 uv, out Vector3 pos)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit raycastHit;
			if (this.m_Terrain.GetComponent<Collider>().Raycast(ray, out raycastHit, float.PositiveInfinity))
			{
				uv = raycastHit.textureCoord;
				pos = raycastHit.point;
				return true;
			}
			uv = Vector2.zero;
			pos = Vector3.zero;
			return false;
		}

		public bool HasFrameBounds()
		{
			return this.m_Terrain != null;
		}

		public Bounds OnGetFrameBounds()
		{
			Vector2 vector;
			Vector3 center;
			if (Camera.current && this.m_Terrain.terrainData && this.Raycast(out vector, out center))
			{
				if (SceneView.lastActiveSceneView != null)
				{
					SceneView.lastActiveSceneView.viewIsLockedToObject = false;
				}
				Bounds result = default(Bounds);
				float num = (this.selectedTool != TerrainTool.PlaceTree) ? ((float)this.m_Size) : TreePainter.brushSize;
				Vector3 size;
				size.x = num / (float)this.m_Terrain.terrainData.heightmapWidth * this.m_Terrain.terrainData.size.x;
				size.z = num / (float)this.m_Terrain.terrainData.heightmapHeight * this.m_Terrain.terrainData.size.z;
				size.y = (size.x + size.z) * 0.5f;
				result.center = center;
				result.size = size;
				if (this.selectedTool == TerrainTool.PaintDetail && this.m_Terrain.terrainData.detailWidth != 0)
				{
					size.x = num / (float)this.m_Terrain.terrainData.detailWidth * this.m_Terrain.terrainData.size.x * 0.7f;
					size.z = num / (float)this.m_Terrain.terrainData.detailHeight * this.m_Terrain.terrainData.size.z * 0.7f;
					size.y = 0f;
					result.size = size;
				}
				return result;
			}
			Vector3 position = this.m_Terrain.transform.position;
			if (this.m_Terrain.terrainData == null)
			{
				return new Bounds(position, Vector3.zero);
			}
			Vector3 size2 = this.m_Terrain.terrainData.size;
			float[,] heights = this.m_Terrain.terrainData.GetHeights(0, 0, this.m_Terrain.terrainData.heightmapWidth, this.m_Terrain.terrainData.heightmapHeight);
			float num2 = -3.40282347E+38f;
			for (int i = 0; i < this.m_Terrain.terrainData.heightmapHeight; i++)
			{
				for (int j = 0; j < this.m_Terrain.terrainData.heightmapWidth; j++)
				{
					num2 = Mathf.Max(num2, heights[j, i]);
				}
			}
			size2.y = num2 * size2.y;
			return new Bounds(position + size2 * 0.5f, size2);
		}

		private bool IsModificationToolActive()
		{
			if (!this.m_Terrain)
			{
				return false;
			}
			TerrainTool selectedTool = this.selectedTool;
			return selectedTool != TerrainTool.TerrainSettings && selectedTool >= TerrainTool.PaintHeight && selectedTool < TerrainTool.TerrainToolCount;
		}

		private bool IsBrushPreviewVisible()
		{
			Vector2 vector;
			Vector3 vector2;
			return this.IsModificationToolActive() && this.Raycast(out vector, out vector2);
		}

		private void DisableProjector()
		{
			if (this.m_CachedBrush != null)
			{
				this.m_CachedBrush.GetPreviewProjector().enabled = false;
			}
		}

		private void UpdatePreviewBrush()
		{
			if (!this.IsModificationToolActive() || this.m_Terrain.terrainData == null)
			{
				this.DisableProjector();
				return;
			}
			Projector previewProjector = this.GetActiveBrush(this.m_Size).GetPreviewProjector();
			float num = 1f;
			float num2 = this.m_Terrain.terrainData.size.x / this.m_Terrain.terrainData.size.z;
			Transform transform = previewProjector.transform;
			bool flag = true;
			Vector2 vector;
			Vector3 vector2;
			if (this.Raycast(out vector, out vector2))
			{
				if (this.selectedTool == TerrainTool.PlaceTree)
				{
					previewProjector.material.mainTexture = (Texture2D)EditorGUIUtility.Load(EditorResourcesUtility.brushesPath + "builtin_brush_4.png");
					num = TreePainter.brushSize / 0.8f;
					num2 = 1f;
				}
				else if (this.selectedTool == TerrainTool.PaintHeight || this.selectedTool == TerrainTool.SetHeight || this.selectedTool == TerrainTool.SmoothHeight)
				{
					if (this.m_Size % 2 == 0)
					{
						float num3 = 0.5f;
						vector.x = (Mathf.Floor(vector.x * (float)(this.m_Terrain.terrainData.heightmapWidth - 1)) + num3) / (float)(this.m_Terrain.terrainData.heightmapWidth - 1);
						vector.y = (Mathf.Floor(vector.y * (float)(this.m_Terrain.terrainData.heightmapHeight - 1)) + num3) / (float)(this.m_Terrain.terrainData.heightmapHeight - 1);
					}
					else
					{
						vector.x = Mathf.Round(vector.x * (float)(this.m_Terrain.terrainData.heightmapWidth - 1)) / (float)(this.m_Terrain.terrainData.heightmapWidth - 1);
						vector.y = Mathf.Round(vector.y * (float)(this.m_Terrain.terrainData.heightmapHeight - 1)) / (float)(this.m_Terrain.terrainData.heightmapHeight - 1);
					}
					vector2.x = vector.x * this.m_Terrain.terrainData.size.x;
					vector2.z = vector.y * this.m_Terrain.terrainData.size.z;
					vector2 += this.m_Terrain.transform.position;
					num = (float)this.m_Size * 0.5f / (float)this.m_Terrain.terrainData.heightmapWidth * this.m_Terrain.terrainData.size.x;
				}
				else if (this.selectedTool == TerrainTool.PaintTexture || this.selectedTool == TerrainTool.PaintDetail)
				{
					float num4 = (this.m_Size % 2 != 0) ? 0.5f : 0f;
					int num5;
					int num6;
					if (this.selectedTool == TerrainTool.PaintTexture)
					{
						num5 = this.m_Terrain.terrainData.alphamapWidth;
						num6 = this.m_Terrain.terrainData.alphamapHeight;
					}
					else
					{
						num5 = this.m_Terrain.terrainData.detailWidth;
						num6 = this.m_Terrain.terrainData.detailHeight;
					}
					if (num5 == 0 || num6 == 0)
					{
						flag = false;
					}
					vector.x = (Mathf.Floor(vector.x * (float)num5) + num4) / (float)num5;
					vector.y = (Mathf.Floor(vector.y * (float)num6) + num4) / (float)num6;
					vector2.x = vector.x * this.m_Terrain.terrainData.size.x;
					vector2.z = vector.y * this.m_Terrain.terrainData.size.z;
					vector2 += this.m_Terrain.transform.position;
					num = (float)this.m_Size * 0.5f / (float)num5 * this.m_Terrain.terrainData.size.x;
					num2 = (float)num5 / (float)num6;
				}
			}
			else
			{
				flag = false;
			}
			previewProjector.enabled = flag;
			if (flag)
			{
				vector2.y = this.m_Terrain.SampleHeight(vector2);
				transform.position = vector2 + new Vector3(0f, 50f, 0f);
			}
			previewProjector.orthographicSize = num / num2;
			previewProjector.aspectRatio = num2;
		}

		public void OnSceneGUI()
		{
			this.Initialize();
			if (!this.m_Terrain.terrainData)
			{
				return;
			}
			Event current = Event.current;
			this.CheckKeys();
			int controlID = GUIUtility.GetControlID(TerrainInspector.s_TerrainEditorHash, FocusType.Passive);
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
			case EventType.MouseDrag:
			{
				if (GUIUtility.hotControl != 0 && GUIUtility.hotControl != controlID)
				{
					return;
				}
				if (current.GetTypeForControl(controlID) == EventType.MouseDrag && GUIUtility.hotControl != controlID)
				{
					return;
				}
				if (Event.current.alt)
				{
					return;
				}
				if (current.button != 0)
				{
					return;
				}
				if (!this.IsModificationToolActive())
				{
					return;
				}
				if (HandleUtility.nearestControl != controlID)
				{
					return;
				}
				if (current.type == EventType.MouseDown)
				{
					GUIUtility.hotControl = controlID;
				}
				Vector2 vector;
				Vector3 vector2;
				if (this.Raycast(out vector, out vector2))
				{
					if (this.selectedTool == TerrainTool.SetHeight && Event.current.shift)
					{
						this.m_TargetHeight.value = this.m_Terrain.terrainData.GetInterpolatedHeight(vector.x, vector.y) / this.m_Terrain.terrainData.size.y;
						InspectorWindow.RepaintAllInspectors();
					}
					else if (this.selectedTool == TerrainTool.PlaceTree)
					{
						if (current.type == EventType.MouseDown)
						{
							Undo.RegisterCompleteObjectUndo(this.m_Terrain.terrainData, "Place Tree");
						}
						if (!Event.current.shift && !Event.current.control)
						{
							TreePainter.PlaceTrees(this.m_Terrain, vector.x, vector.y);
						}
						else
						{
							TreePainter.RemoveTrees(this.m_Terrain, vector.x, vector.y, Event.current.control);
						}
					}
					else if (this.selectedTool == TerrainTool.PaintTexture)
					{
						if (current.type == EventType.MouseDown)
						{
							List<UnityEngine.Object> list = new List<UnityEngine.Object>();
							list.Add(this.m_Terrain.terrainData);
							list.AddRange(this.m_Terrain.terrainData.alphamapTextures);
							Undo.RegisterCompleteObjectUndo(list.ToArray(), "Detail Edit");
						}
						SplatPainter splatPainter = new SplatPainter();
						splatPainter.size = this.m_Size;
						splatPainter.strength = this.m_Strength;
						splatPainter.terrainData = this.m_Terrain.terrainData;
						splatPainter.brush = this.GetActiveBrush(splatPainter.size);
						splatPainter.target = this.m_SplatAlpha;
						splatPainter.tool = this.selectedTool;
						this.m_Terrain.editorRenderFlags = TerrainRenderFlags.heightmap;
						splatPainter.Paint(vector.x, vector.y, this.m_SelectedSplat);
						this.m_Terrain.terrainData.SetBasemapDirty(false);
					}
					else if (this.selectedTool == TerrainTool.PaintDetail)
					{
						if (current.type == EventType.MouseDown)
						{
							Undo.RegisterCompleteObjectUndo(this.m_Terrain.terrainData, "Detail Edit");
						}
						DetailPainter detailPainter = new DetailPainter();
						detailPainter.size = this.m_Size;
						detailPainter.targetStrength = this.m_DetailStrength * 16f;
						detailPainter.opacity = this.m_DetailOpacity;
						if (Event.current.shift || Event.current.control)
						{
							detailPainter.targetStrength *= -1f;
						}
						detailPainter.clearSelectedOnly = Event.current.control;
						detailPainter.terrainData = this.m_Terrain.terrainData;
						detailPainter.brush = this.GetActiveBrush(detailPainter.size);
						detailPainter.tool = this.selectedTool;
						detailPainter.randomizeDetails = true;
						detailPainter.Paint(vector.x, vector.y, this.m_SelectedDetail);
					}
					else
					{
						if (current.type == EventType.MouseDown)
						{
							Undo.RegisterCompleteObjectUndo(this.m_Terrain.terrainData, "Heightmap Edit");
						}
						HeightmapPainter heightmapPainter = new HeightmapPainter();
						heightmapPainter.size = this.m_Size;
						heightmapPainter.strength = this.m_Strength * 0.01f;
						if (this.selectedTool == TerrainTool.SmoothHeight)
						{
							heightmapPainter.strength = this.m_Strength;
						}
						heightmapPainter.terrainData = this.m_Terrain.terrainData;
						heightmapPainter.brush = this.GetActiveBrush(this.m_Size);
						heightmapPainter.targetHeight = this.m_TargetHeight;
						heightmapPainter.tool = this.selectedTool;
						this.m_Terrain.editorRenderFlags = TerrainRenderFlags.heightmap;
						if (this.selectedTool == TerrainTool.PaintHeight && Event.current.shift)
						{
							heightmapPainter.strength = -heightmapPainter.strength;
						}
						heightmapPainter.PaintHeight(vector.x, vector.y);
					}
					current.Use();
				}
				break;
			}
			case EventType.MouseUp:
				if (GUIUtility.hotControl != controlID)
				{
					return;
				}
				GUIUtility.hotControl = 0;
				if (!this.IsModificationToolActive())
				{
					return;
				}
				if (this.selectedTool == TerrainTool.PaintTexture)
				{
					this.m_Terrain.terrainData.SetBasemapDirty(true);
				}
				this.m_Terrain.editorRenderFlags = TerrainRenderFlags.all;
				this.m_Terrain.ApplyDelayedHeightmapModification();
				current.Use();
				break;
			case EventType.MouseMove:
				if (this.IsBrushPreviewVisible())
				{
					HandleUtility.Repaint();
				}
				break;
			case EventType.Repaint:
				this.DisableProjector();
				break;
			case EventType.Layout:
				if (!this.IsModificationToolActive())
				{
					return;
				}
				HandleUtility.AddDefaultControl(controlID);
				break;
			}
		}

		public void OnPreSceneGUI()
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.UpdatePreviewBrush();
			}
		}
	}
}
