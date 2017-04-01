using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(ProceduralMaterial))]
	internal class ProceduralMaterialInspector : MaterialEditor
	{
		private class Styles
		{
			public GUIContent hslContent = new GUIContent("HSL Adjustment", "Hue_Shift, Saturation, Luminosity");

			public GUIContent randomSeedContent = new GUIContent("Random Seed", "$randomseed : the overall random aspect of the texture.");

			public GUIContent randomizeButtonContent = new GUIContent("Randomize");

			public GUIContent generateAllOutputsContent = new GUIContent("Generate all outputs", "Force the generation of all Substance outputs.");

			public GUIContent animatedContent = new GUIContent("Animation update rate", "Set the animation update rate in millisecond");

			public GUIContent defaultPlatform = EditorGUIUtility.TextContent("Default");

			public GUIContent targetWidth = new GUIContent("Target Width");

			public GUIContent targetHeight = new GUIContent("Target Height");

			public GUIContent textureFormat = EditorGUIUtility.TextContent("Format");

			public GUIContent loadBehavior = new GUIContent("Load Behavior");

			public GUIContent mipmapContent = new GUIContent("Generate Mip Maps");
		}

		[Serializable]
		protected class ProceduralPlatformSetting
		{
			private UnityEngine.Object[] targets;

			public string name;

			public bool m_Overridden;

			public int maxTextureWidth;

			public int maxTextureHeight;

			public int m_TextureFormat;

			public int m_LoadBehavior;

			public BuildTarget target;

			public Texture2D icon;

			public bool isDefault
			{
				get
				{
					return this.name == "";
				}
			}

			public int textureFormat
			{
				get
				{
					return this.m_TextureFormat;
				}
				set
				{
					this.m_TextureFormat = value;
				}
			}

			public bool overridden
			{
				get
				{
					return this.m_Overridden;
				}
			}

			public ProceduralPlatformSetting(UnityEngine.Object[] objects, string _name, BuildTarget _target, Texture2D _icon)
			{
				this.targets = objects;
				this.m_Overridden = false;
				this.target = _target;
				this.name = _name;
				this.icon = _icon;
				this.m_Overridden = false;
				if (this.name != "")
				{
					UnityEngine.Object[] array = this.targets;
					for (int i = 0; i < array.Length; i++)
					{
						ProceduralMaterial proceduralMaterial = (ProceduralMaterial)array[i];
						string assetPath = AssetDatabase.GetAssetPath(proceduralMaterial);
						SubstanceImporter substanceImporter = AssetImporter.GetAtPath(assetPath) as SubstanceImporter;
						if (substanceImporter != null && substanceImporter.GetPlatformTextureSettings(proceduralMaterial.name, this.name, out this.maxTextureWidth, out this.maxTextureHeight, out this.m_TextureFormat, out this.m_LoadBehavior))
						{
							this.m_Overridden = true;
							break;
						}
					}
				}
				if (!this.m_Overridden && this.targets.Length > 0)
				{
					string assetPath2 = AssetDatabase.GetAssetPath(this.targets[0]);
					SubstanceImporter substanceImporter2 = AssetImporter.GetAtPath(assetPath2) as SubstanceImporter;
					if (substanceImporter2 != null)
					{
						substanceImporter2.GetPlatformTextureSettings((this.targets[0] as ProceduralMaterial).name, "", out this.maxTextureWidth, out this.maxTextureHeight, out this.m_TextureFormat, out this.m_LoadBehavior);
					}
				}
			}

			public void SetOverride(ProceduralMaterialInspector.ProceduralPlatformSetting master)
			{
				this.m_Overridden = true;
			}

			public void ClearOverride(ProceduralMaterialInspector.ProceduralPlatformSetting master)
			{
				this.m_TextureFormat = master.textureFormat;
				this.maxTextureWidth = master.maxTextureWidth;
				this.maxTextureHeight = master.maxTextureHeight;
				this.m_LoadBehavior = master.m_LoadBehavior;
				this.m_Overridden = false;
			}

			public bool HasChanged()
			{
				ProceduralMaterialInspector.ProceduralPlatformSetting proceduralPlatformSetting = new ProceduralMaterialInspector.ProceduralPlatformSetting(this.targets, this.name, this.target, null);
				return proceduralPlatformSetting.m_Overridden != this.m_Overridden || proceduralPlatformSetting.maxTextureWidth != this.maxTextureWidth || proceduralPlatformSetting.maxTextureHeight != this.maxTextureHeight || proceduralPlatformSetting.textureFormat != this.textureFormat || proceduralPlatformSetting.m_LoadBehavior != this.m_LoadBehavior;
			}

			public void Apply()
			{
				UnityEngine.Object[] array = this.targets;
				for (int i = 0; i < array.Length; i++)
				{
					ProceduralMaterial proceduralMaterial = (ProceduralMaterial)array[i];
					string assetPath = AssetDatabase.GetAssetPath(proceduralMaterial);
					SubstanceImporter substanceImporter = AssetImporter.GetAtPath(assetPath) as SubstanceImporter;
					if (this.name != "")
					{
						if (this.m_Overridden)
						{
							substanceImporter.SetPlatformTextureSettings(proceduralMaterial, this.name, this.maxTextureWidth, this.maxTextureHeight, this.m_TextureFormat, this.m_LoadBehavior);
						}
						else
						{
							substanceImporter.ClearPlatformTextureSettings(proceduralMaterial.name, this.name);
						}
					}
					else
					{
						substanceImporter.SetPlatformTextureSettings(proceduralMaterial, this.name, this.maxTextureWidth, this.maxTextureHeight, this.m_TextureFormat, this.m_LoadBehavior);
					}
				}
			}
		}

		private static ProceduralMaterial m_Material = null;

		private static SubstanceImporter m_Importer = null;

		private static string[] kMaxTextureSizeStrings = new string[]
		{
			"32",
			"64",
			"128",
			"256",
			"512",
			"1024",
			"2048"
		};

		private static int[] kMaxTextureSizeValues = new int[]
		{
			32,
			64,
			128,
			256,
			512,
			1024,
			2048
		};

		private bool m_AllowTextureSizeModification = false;

		private bool m_ShowTexturesSection = false;

		private bool m_ShowHSLInputs = true;

		private ProceduralMaterialInspector.Styles m_Styles;

		private static string[] kMaxLoadBehaviorStrings = new string[]
		{
			"Do nothing",
			"Do nothing and cache",
			"Build on level load",
			"Build on level load and cache",
			"Bake and keep Substance",
			"Bake and discard Substance"
		};

		private static int[] kMaxLoadBehaviorValues = new int[]
		{
			0,
			5,
			1,
			4,
			2,
			3
		};

		private static string[] kTextureFormatStrings = new string[]
		{
			"Compressed",
			"Compressed - No Alpha",
			"RAW",
			"RAW - No Alpha"
		};

		private static int[] kTextureFormatValues = new int[]
		{
			0,
			2,
			1,
			3
		};

		private bool m_MightHaveModified = false;

		private static bool m_UndoWasPerformed = false;

		private static Dictionary<ProceduralMaterial, float> m_GeneratingSince = new Dictionary<ProceduralMaterial, float>();

		private bool m_ReimportOnDisable = true;

		private Vector2 m_ScrollPos = default(Vector2);

		protected List<ProceduralMaterialInspector.ProceduralPlatformSetting> m_PlatformSettings;

		public void DisableReimportOnDisable()
		{
			this.m_ReimportOnDisable = false;
		}

		public void ReimportSubstances()
		{
			string[] array = new string[base.targets.GetLength(0)];
			int num = 0;
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				ProceduralMaterial assetObject = (ProceduralMaterial)targets[i];
				array[num++] = AssetDatabase.GetAssetPath(assetObject);
			}
			for (int j = 0; j < num; j++)
			{
				SubstanceImporter substanceImporter = AssetImporter.GetAtPath(array[j]) as SubstanceImporter;
				if (substanceImporter && EditorUtility.IsDirty(substanceImporter.GetInstanceID()))
				{
					AssetDatabase.ImportAsset(array[j], ImportAssetOptions.ForceUncompressedImport);
				}
			}
		}

		public override void Awake()
		{
			base.Awake();
			this.m_ShowTexturesSection = EditorPrefs.GetBool("ProceduralShowTextures", false);
			this.m_ReimportOnDisable = true;
			if (ProceduralMaterialInspector.m_UndoWasPerformed)
			{
				ProceduralMaterialInspector.m_UndoWasPerformed = false;
				this.OnShaderChanged();
			}
			ProceduralMaterialInspector.m_UndoWasPerformed = false;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void ReimportSubstancesIfNeeded()
		{
			if (this.m_MightHaveModified && !ProceduralMaterialInspector.m_UndoWasPerformed)
			{
				if (!EditorApplication.isPlaying && !InternalEditorUtility.ignoreInspectorChanges)
				{
					this.ReimportSubstances();
				}
			}
		}

		public override void OnDisable()
		{
			ProceduralMaterial exists = base.target as ProceduralMaterial;
			if (exists && this.m_PlatformSettings != null && this.HasModified())
			{
				string message = "Unapplied import settings for '" + AssetDatabase.GetAssetPath(base.target) + "'";
				if (EditorUtility.DisplayDialog("Unapplied import settings", message, "Apply", "Revert"))
				{
					this.Apply();
					this.ReimportSubstances();
				}
				this.ResetValues();
			}
			if (this.m_ReimportOnDisable)
			{
				this.ReimportSubstancesIfNeeded();
			}
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			base.OnDisable();
		}

		public override void UndoRedoPerformed()
		{
			ProceduralMaterialInspector.m_UndoWasPerformed = true;
			if (ProceduralMaterialInspector.m_Material != null)
			{
				ProceduralMaterialInspector.m_Material.RebuildTextures();
			}
			base.UndoRedoPerformed();
		}

		protected override void OnShaderChanged()
		{
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				ProceduralMaterial proceduralMaterial = (ProceduralMaterial)targets[i];
				string assetPath = AssetDatabase.GetAssetPath(proceduralMaterial);
				SubstanceImporter substanceImporter = AssetImporter.GetAtPath(assetPath) as SubstanceImporter;
				if (substanceImporter != null && proceduralMaterial != null)
				{
					substanceImporter.OnShaderModified(proceduralMaterial);
				}
			}
		}

		internal void DisplayRestrictedInspector()
		{
			this.m_MightHaveModified = false;
			if (this.m_Styles == null)
			{
				this.m_Styles = new ProceduralMaterialInspector.Styles();
			}
			ProceduralMaterial proceduralMaterial = base.target as ProceduralMaterial;
			if (ProceduralMaterialInspector.m_Material != proceduralMaterial)
			{
				ProceduralMaterialInspector.m_Material = proceduralMaterial;
			}
			this.ProceduralProperties();
			GUILayout.Space(15f);
			this.GeneratedTextures();
		}

		internal override void OnAssetStoreInspectorGUI()
		{
			this.DisplayRestrictedInspector();
		}

		internal override bool IsEnabled()
		{
			return base.IsOpenForEdit();
		}

		internal override void OnHeaderTitleGUI(Rect titleRect, string header)
		{
			ProceduralMaterial proceduralMaterial = base.target as ProceduralMaterial;
			string assetPath = AssetDatabase.GetAssetPath(base.target);
			ProceduralMaterialInspector.m_Importer = (AssetImporter.GetAtPath(assetPath) as SubstanceImporter);
			if (!(ProceduralMaterialInspector.m_Importer == null))
			{
				string text = proceduralMaterial.name;
				text = EditorGUI.DelayedTextField(titleRect, text, EditorStyles.textField);
				if (text != proceduralMaterial.name)
				{
					if (ProceduralMaterialInspector.m_Importer.RenameMaterial(proceduralMaterial, text))
					{
						AssetDatabase.ImportAsset(ProceduralMaterialInspector.m_Importer.assetPath, ImportAssetOptions.ForceUncompressedImport);
						GUIUtility.ExitGUI();
					}
					else
					{
						text = proceduralMaterial.name;
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			using (new EditorGUI.DisabledScope(AnimationMode.InAnimationMode()))
			{
				this.m_MightHaveModified = true;
				if (this.m_Styles == null)
				{
					this.m_Styles = new ProceduralMaterialInspector.Styles();
				}
				ProceduralMaterial proceduralMaterial = base.target as ProceduralMaterial;
				string assetPath = AssetDatabase.GetAssetPath(base.target);
				ProceduralMaterialInspector.m_Importer = (AssetImporter.GetAtPath(assetPath) as SubstanceImporter);
				if (ProceduralMaterialInspector.m_Importer == null)
				{
					this.DisplayRestrictedInspector();
				}
				else
				{
					if (ProceduralMaterialInspector.m_Material != proceduralMaterial)
					{
						ProceduralMaterialInspector.m_Material = proceduralMaterial;
					}
					if (base.isVisible && !(proceduralMaterial.shader == null))
					{
						if (base.PropertiesGUI())
						{
							this.OnShaderChanged();
							base.PropertiesChanged();
						}
						GUILayout.Space(5f);
						this.ProceduralProperties();
						GUILayout.Space(15f);
						this.GeneratedTextures();
					}
				}
			}
		}

		private void ProceduralProperties()
		{
			GUILayout.Label("Procedural Properties", EditorStyles.boldLabel, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				ProceduralMaterial proceduralMaterial = (ProceduralMaterial)targets[i];
				if (proceduralMaterial.isProcessing)
				{
					base.Repaint();
					SceneView.RepaintAll();
					GameView.RepaintAll();
					break;
				}
			}
			if (base.targets.Length > 1)
			{
				GUILayout.Label("Procedural properties do not support multi-editing.", EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			}
			else
			{
				EditorGUIUtility.labelWidth = 0f;
				EditorGUIUtility.fieldWidth = 0f;
				if (ProceduralMaterialInspector.m_Importer != null)
				{
					if (!ProceduralMaterial.isSupported)
					{
						GUILayout.Label("Procedural Materials are not supported on " + EditorUserBuildSettings.activeBuildTarget + ". Textures will be baked.", EditorStyles.helpBox, new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(true)
						});
					}
					bool changed = GUI.changed;
					using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
					{
						EditorGUI.BeginChangeCheck();
						bool generated = EditorGUILayout.Toggle(this.m_Styles.generateAllOutputsContent, ProceduralMaterialInspector.m_Importer.GetGenerateAllOutputs(ProceduralMaterialInspector.m_Material), new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							ProceduralMaterialInspector.m_Importer.SetGenerateAllOutputs(ProceduralMaterialInspector.m_Material, generated);
						}
						EditorGUI.BeginChangeCheck();
						bool mode = EditorGUILayout.Toggle(this.m_Styles.mipmapContent, ProceduralMaterialInspector.m_Importer.GetGenerateMipMaps(ProceduralMaterialInspector.m_Material), new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							ProceduralMaterialInspector.m_Importer.SetGenerateMipMaps(ProceduralMaterialInspector.m_Material, mode);
						}
					}
					if (ProceduralMaterialInspector.m_Material.HasProceduralProperty("$time"))
					{
						EditorGUI.BeginChangeCheck();
						int animation_update_rate = EditorGUILayout.IntField(this.m_Styles.animatedContent, ProceduralMaterialInspector.m_Importer.GetAnimationUpdateRate(ProceduralMaterialInspector.m_Material), new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							ProceduralMaterialInspector.m_Importer.SetAnimationUpdateRate(ProceduralMaterialInspector.m_Material, animation_update_rate);
						}
					}
					GUI.changed = changed;
				}
				this.InputOptions(ProceduralMaterialInspector.m_Material);
			}
		}

		private void GeneratedTextures()
		{
			if (base.targets.Length <= 1)
			{
				ProceduralPropertyDescription[] proceduralPropertyDescriptions = ProceduralMaterialInspector.m_Material.GetProceduralPropertyDescriptions();
				ProceduralPropertyDescription[] array = proceduralPropertyDescriptions;
				for (int i = 0; i < array.Length; i++)
				{
					ProceduralPropertyDescription proceduralPropertyDescription = array[i];
					if (proceduralPropertyDescription.name == "$outputsize")
					{
						this.m_AllowTextureSizeModification = true;
						break;
					}
				}
				string text = "Generated Textures";
				if (ProceduralMaterialInspector.ShowIsGenerating(base.target as ProceduralMaterial))
				{
					text += " (Generating...)";
				}
				EditorGUI.BeginChangeCheck();
				this.m_ShowTexturesSection = EditorGUILayout.Foldout(this.m_ShowTexturesSection, text, true);
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetBool("ProceduralShowTextures", this.m_ShowTexturesSection);
				}
				if (this.m_ShowTexturesSection)
				{
					this.ShowProceduralTexturesGUI(ProceduralMaterialInspector.m_Material);
					this.ShowGeneratedTexturesGUI(ProceduralMaterialInspector.m_Material);
					if (ProceduralMaterialInspector.m_Importer != null)
					{
						if (this.HasProceduralTextureProperties(ProceduralMaterialInspector.m_Material))
						{
							this.OffsetScaleGUI(ProceduralMaterialInspector.m_Material);
						}
						GUILayout.Space(5f);
						using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
						{
							this.ShowTextureSizeGUI();
						}
					}
				}
			}
		}

		public static bool ShowIsGenerating(ProceduralMaterial mat)
		{
			if (!ProceduralMaterialInspector.m_GeneratingSince.ContainsKey(mat))
			{
				ProceduralMaterialInspector.m_GeneratingSince[mat] = 0f;
			}
			bool result;
			if (mat.isProcessing)
			{
				result = (Time.realtimeSinceStartup > ProceduralMaterialInspector.m_GeneratingSince[mat] + 0.4f);
			}
			else
			{
				ProceduralMaterialInspector.m_GeneratingSince[mat] = Time.realtimeSinceStartup;
				result = false;
			}
			return result;
		}

		public override string GetInfoString()
		{
			ProceduralMaterial proceduralMaterial = base.target as ProceduralMaterial;
			Texture[] generatedTextures = proceduralMaterial.GetGeneratedTextures();
			string result;
			if (generatedTextures.Length == 0)
			{
				result = string.Empty;
			}
			else
			{
				result = generatedTextures[0].width + "x" + generatedTextures[0].height;
			}
			return result;
		}

		public bool HasProceduralTextureProperties(Material material)
		{
			Shader shader = material.shader;
			int propertyCount = ShaderUtil.GetPropertyCount(shader);
			bool result;
			for (int i = 0; i < propertyCount; i++)
			{
				if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
				{
					string propertyName = ShaderUtil.GetPropertyName(shader, i);
					Texture texture = material.GetTexture(propertyName);
					if (SubstanceImporter.IsProceduralTextureSlot(material, texture, propertyName))
					{
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		protected void RecordForUndo(ProceduralMaterial material, SubstanceImporter importer, string message)
		{
			if (importer)
			{
				Undo.RecordObjects(new UnityEngine.Object[]
				{
					material,
					importer
				}, message);
			}
			else
			{
				Undo.RecordObject(material, message);
			}
		}

		protected void OffsetScaleGUI(ProceduralMaterial material)
		{
			if (!(ProceduralMaterialInspector.m_Importer == null) && base.targets.Length <= 1)
			{
				Vector2 materialScale = ProceduralMaterialInspector.m_Importer.GetMaterialScale(material);
				Vector2 materialOffset = ProceduralMaterialInspector.m_Importer.GetMaterialOffset(material);
				Vector4 scaleOffset = new Vector4(materialScale.x, materialScale.y, materialOffset.x, materialOffset.y);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(10f);
				Rect rect = GUILayoutUtility.GetRect(100f, 10000f, 32f, 32f);
				GUILayout.EndHorizontal();
				EditorGUI.BeginChangeCheck();
				scaleOffset = MaterialEditor.TextureScaleOffsetProperty(rect, scaleOffset);
				if (EditorGUI.EndChangeCheck())
				{
					this.RecordForUndo(material, ProceduralMaterialInspector.m_Importer, "Modify " + material.name + "'s Tiling/Offset");
					ProceduralMaterialInspector.m_Importer.SetMaterialScale(material, new Vector2(scaleOffset.x, scaleOffset.y));
					ProceduralMaterialInspector.m_Importer.SetMaterialOffset(material, new Vector2(scaleOffset.z, scaleOffset.w));
				}
			}
		}

		protected void InputOptions(ProceduralMaterial material)
		{
			EditorGUI.BeginChangeCheck();
			this.InputsGUI();
			if (EditorGUI.EndChangeCheck())
			{
				material.RebuildTextures();
			}
		}

		[MenuItem("CONTEXT/ProceduralMaterial/Reset", false, -100)]
		public static void ResetSubstance(MenuCommand command)
		{
			string assetPath = AssetDatabase.GetAssetPath(command.context);
			ProceduralMaterialInspector.m_Importer = (AssetImporter.GetAtPath(assetPath) as SubstanceImporter);
			ProceduralMaterialInspector.m_Importer.ResetMaterial(command.context as ProceduralMaterial);
		}

		private static void ExportBitmaps(ProceduralMaterial material, bool alphaRemap)
		{
			string text = EditorUtility.SaveFolderPanel("Set bitmap export path...", "", "");
			if (!(text == ""))
			{
				string assetPath = AssetDatabase.GetAssetPath(material);
				SubstanceImporter substanceImporter = AssetImporter.GetAtPath(assetPath) as SubstanceImporter;
				if (substanceImporter)
				{
					substanceImporter.ExportBitmaps(material, text, alphaRemap);
				}
			}
		}

		[MenuItem("CONTEXT/ProceduralMaterial/Export Bitmaps (remapped alpha channels)", false)]
		public static void ExportBitmapsAlphaRemap(MenuCommand command)
		{
			ProceduralMaterialInspector.ExportBitmaps(command.context as ProceduralMaterial, true);
		}

		[MenuItem("CONTEXT/ProceduralMaterial/Export Bitmaps (original alpha channels)", false)]
		public static void ExportBitmapsNoAlphaRemap(MenuCommand command)
		{
			ProceduralMaterialInspector.ExportBitmaps(command.context as ProceduralMaterial, false);
		}

		[MenuItem("CONTEXT/ProceduralMaterial/Export Preset", false)]
		public static void ExportPreset(MenuCommand command)
		{
			string text = EditorUtility.SaveFolderPanel("Set preset export path...", "", "");
			if (!(text == ""))
			{
				ProceduralMaterial proceduralMaterial = command.context as ProceduralMaterial;
				string assetPath = AssetDatabase.GetAssetPath(proceduralMaterial);
				SubstanceImporter substanceImporter = AssetImporter.GetAtPath(assetPath) as SubstanceImporter;
				if (substanceImporter)
				{
					substanceImporter.ExportPreset(proceduralMaterial, text);
				}
			}
		}

		protected void ShowProceduralTexturesGUI(ProceduralMaterial material)
		{
			if (base.targets.Length <= 1)
			{
				EditorGUILayout.Space();
				Shader shader = material.shader;
				if (!(shader == null))
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Space(4f);
					GUILayout.FlexibleSpace();
					float pixels = 10f;
					bool flag = true;
					for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
					{
						if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
						{
							string propertyName = ShaderUtil.GetPropertyName(shader, i);
							Texture texture = material.GetTexture(propertyName);
							if (SubstanceImporter.IsProceduralTextureSlot(material, texture, propertyName))
							{
								string propertyDescription = ShaderUtil.GetPropertyDescription(shader, i);
								TextureDimension texDim = ShaderUtil.GetTexDim(shader, i);
								Type textureTypeFromDimension = MaterialEditor.GetTextureTypeFromDimension(texDim);
								GUIStyle gUIStyle = "ObjectPickerResultsGridLabel";
								if (flag)
								{
									flag = false;
								}
								else
								{
									GUILayout.Space(pixels);
								}
								GUILayout.BeginVertical(new GUILayoutOption[]
								{
									GUILayout.Height(72f + gUIStyle.fixedHeight + gUIStyle.fixedHeight + 8f)
								});
								Rect rect = GUILayoutUtility.GetRect(72f, 72f);
								ProceduralMaterialInspector.DoObjectPingField(rect, rect, GUIUtility.GetControlID(12354, FocusType.Keyboard, rect), texture, textureTypeFromDimension);
								this.ShowAlphaSourceGUI(material, texture as ProceduralTexture, ref rect);
								rect.height = gUIStyle.fixedHeight;
								GUI.Label(rect, propertyDescription, gUIStyle);
								GUILayout.EndVertical();
								GUILayout.FlexibleSpace();
							}
						}
					}
					GUILayout.Space(4f);
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		protected void ShowGeneratedTexturesGUI(ProceduralMaterial material)
		{
			if (base.targets.Length <= 1)
			{
				if (!(ProceduralMaterialInspector.m_Importer != null) || ProceduralMaterialInspector.m_Importer.GetGenerateAllOutputs(ProceduralMaterialInspector.m_Material))
				{
					GUIStyle gUIStyle = "ObjectPickerResultsGridLabel";
					EditorGUILayout.Space();
					GUILayout.FlexibleSpace();
					this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[]
					{
						GUILayout.Height(64f + gUIStyle.fixedHeight + gUIStyle.fixedHeight + 16f)
					});
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					float pixels = 10f;
					Texture[] generatedTextures = material.GetGeneratedTextures();
					Texture[] array = generatedTextures;
					for (int i = 0; i < array.Length; i++)
					{
						Texture texture = array[i];
						ProceduralTexture proceduralTexture = texture as ProceduralTexture;
						if (proceduralTexture != null)
						{
							GUILayout.Space(pixels);
							GUILayout.BeginVertical(new GUILayoutOption[]
							{
								GUILayout.Height(64f + gUIStyle.fixedHeight + 8f)
							});
							Rect rect = GUILayoutUtility.GetRect(64f, 64f);
							ProceduralMaterialInspector.DoObjectPingField(rect, rect, GUIUtility.GetControlID(12354, FocusType.Keyboard, rect), proceduralTexture, typeof(Texture));
							this.ShowAlphaSourceGUI(material, proceduralTexture, ref rect);
							GUILayout.EndVertical();
							GUILayout.Space(pixels);
							GUILayout.FlexibleSpace();
						}
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndScrollView();
				}
			}
		}

		private void ShowAlphaSourceGUI(ProceduralMaterial material, ProceduralTexture tex, ref Rect rect)
		{
			GUIStyle gUIStyle = "ObjectPickerResultsGridLabel";
			float num = 10f;
			rect.y = rect.yMax + 2f;
			if (ProceduralMaterialInspector.m_Importer != null)
			{
				using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
				{
					if (tex.GetProceduralOutputType() != ProceduralOutputType.Normal && tex.hasAlpha)
					{
						rect.height = gUIStyle.fixedHeight;
						string[] displayedOptions = new string[]
						{
							"Source (A)",
							"Diffuse (A)",
							"Normal (A)",
							"Height (A)",
							"Emissive (A)",
							"Specular (A)",
							"Opacity (A)",
							"Smoothness (A)",
							"Amb. Occlusion (A)",
							"Detail Mask (A)",
							"Metallic (A)",
							"Roughness (A)"
						};
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUI.BeginChangeCheck();
						int alphaSource = EditorGUI.Popup(rect, (int)ProceduralMaterialInspector.m_Importer.GetTextureAlphaSource(material, tex.name), displayedOptions);
						if (EditorGUI.EndChangeCheck())
						{
							this.RecordForUndo(material, ProceduralMaterialInspector.m_Importer, "Modify " + material.name + "'s Alpha Modifier");
							ProceduralMaterialInspector.m_Importer.SetTextureAlphaSource(material, tex.name, (ProceduralOutputType)alphaSource);
						}
						rect.y = rect.yMax + 2f;
					}
				}
			}
			rect.width += num;
		}

		internal static void DoObjectPingField(Rect position, Rect dropRect, int id, UnityEngine.Object obj, Type objType)
		{
			Event current = Event.current;
			EventType eventType = current.type;
			if (!GUI.enabled && GUIClip.enabled && Event.current.rawType == EventType.MouseDown)
			{
				eventType = Event.current.rawType;
			}
			bool flag = EditorGUIUtility.HasObjectThumbnail(objType) && position.height > 16f;
			if (eventType != EventType.MouseDown)
			{
				if (eventType == EventType.Repaint)
				{
					GUIContent gUIContent = EditorGUIUtility.ObjectContent(obj, objType);
					if (flag)
					{
						GUIStyle objectFieldThumb = EditorStyles.objectFieldThumb;
						objectFieldThumb.Draw(position, GUIContent.none, id, DragAndDrop.activeControlID == id);
						if (obj != null)
						{
							EditorGUI.DrawPreviewTexture(objectFieldThumb.padding.Remove(position), gUIContent.image);
						}
						else
						{
							GUIStyle gUIStyle = objectFieldThumb.name + "Overlay";
							gUIStyle.Draw(position, gUIContent, id);
						}
					}
					else
					{
						GUIStyle objectField = EditorStyles.objectField;
						objectField.Draw(position, gUIContent, id, DragAndDrop.activeControlID == id);
					}
				}
			}
			else if (Event.current.button == 0)
			{
				if (position.Contains(Event.current.mousePosition))
				{
					UnityEngine.Object @object = obj;
					Component component = @object as Component;
					if (component)
					{
						@object = component.gameObject;
					}
					if (Event.current.clickCount == 1)
					{
						GUIUtility.keyboardControl = id;
						if (@object)
						{
							EditorGUIUtility.PingObject(@object);
						}
						current.Use();
					}
					else if (Event.current.clickCount == 2)
					{
						if (@object)
						{
							AssetDatabase.OpenAsset(@object);
							GUIUtility.ExitGUI();
						}
						current.Use();
					}
				}
			}
		}

		internal void ResetValues()
		{
			this.BuildTargetList();
			if (this.HasModified())
			{
				Debug.LogError("Impossible");
			}
		}

		internal void Apply()
		{
			foreach (ProceduralMaterialInspector.ProceduralPlatformSetting current in this.m_PlatformSettings)
			{
				current.Apply();
			}
		}

		internal bool HasModified()
		{
			bool result;
			foreach (ProceduralMaterialInspector.ProceduralPlatformSetting current in this.m_PlatformSettings)
			{
				if (current.HasChanged())
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public void BuildTargetList()
		{
			List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
			this.m_PlatformSettings = new List<ProceduralMaterialInspector.ProceduralPlatformSetting>();
			this.m_PlatformSettings.Add(new ProceduralMaterialInspector.ProceduralPlatformSetting(base.targets, "", BuildTarget.StandaloneWindows, null));
			foreach (BuildPlayerWindow.BuildPlatform current in validPlatforms)
			{
				this.m_PlatformSettings.Add(new ProceduralMaterialInspector.ProceduralPlatformSetting(base.targets, current.name, current.DefaultTarget, current.smallIcon));
			}
		}

		public void ShowTextureSizeGUI()
		{
			if (this.m_PlatformSettings == null)
			{
				this.BuildTargetList();
			}
			this.TextureSizeGUI();
		}

		protected void TextureSizeGUI()
		{
			BuildPlayerWindow.BuildPlatform[] platforms = BuildPlayerWindow.GetValidPlatforms().ToArray();
			int num = EditorGUILayout.BeginPlatformGrouping(platforms, this.m_Styles.defaultPlatform);
			ProceduralMaterialInspector.ProceduralPlatformSetting proceduralPlatformSetting = this.m_PlatformSettings[num + 1];
			ProceduralMaterialInspector.ProceduralPlatformSetting proceduralPlatformSetting2 = proceduralPlatformSetting;
			bool flag = true;
			if (proceduralPlatformSetting.name != "")
			{
				EditorGUI.BeginChangeCheck();
				flag = GUILayout.Toggle(proceduralPlatformSetting.overridden, "Override for " + proceduralPlatformSetting.name, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					if (flag)
					{
						proceduralPlatformSetting.SetOverride(this.m_PlatformSettings[0]);
					}
					else
					{
						proceduralPlatformSetting.ClearOverride(this.m_PlatformSettings[0]);
					}
				}
			}
			using (new EditorGUI.DisabledScope(!flag))
			{
				if (!this.m_AllowTextureSizeModification)
				{
					GUILayout.Label("This ProceduralMaterial was published with a fixed size.", EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				}
				using (new EditorGUI.DisabledScope(!this.m_AllowTextureSizeModification))
				{
					EditorGUI.BeginChangeCheck();
					proceduralPlatformSetting2.maxTextureWidth = EditorGUILayout.IntPopup(this.m_Styles.targetWidth.text, proceduralPlatformSetting2.maxTextureWidth, ProceduralMaterialInspector.kMaxTextureSizeStrings, ProceduralMaterialInspector.kMaxTextureSizeValues, new GUILayoutOption[0]);
					proceduralPlatformSetting2.maxTextureHeight = EditorGUILayout.IntPopup(this.m_Styles.targetHeight.text, proceduralPlatformSetting2.maxTextureHeight, ProceduralMaterialInspector.kMaxTextureSizeStrings, ProceduralMaterialInspector.kMaxTextureSizeValues, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck() && proceduralPlatformSetting2.isDefault)
					{
						foreach (ProceduralMaterialInspector.ProceduralPlatformSetting current in this.m_PlatformSettings)
						{
							if (!current.isDefault && !current.overridden)
							{
								current.maxTextureWidth = proceduralPlatformSetting2.maxTextureWidth;
								current.maxTextureHeight = proceduralPlatformSetting2.maxTextureHeight;
							}
						}
					}
				}
				EditorGUI.BeginChangeCheck();
				int num2 = proceduralPlatformSetting2.textureFormat;
				if (num2 < 0 || num2 >= ProceduralMaterialInspector.kTextureFormatStrings.Length)
				{
					Debug.LogError("Invalid TextureFormat");
				}
				num2 = EditorGUILayout.IntPopup(this.m_Styles.textureFormat.text, num2, ProceduralMaterialInspector.kTextureFormatStrings, ProceduralMaterialInspector.kTextureFormatValues, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					proceduralPlatformSetting2.textureFormat = num2;
					if (proceduralPlatformSetting2.isDefault)
					{
						foreach (ProceduralMaterialInspector.ProceduralPlatformSetting current2 in this.m_PlatformSettings)
						{
							if (!current2.isDefault && !current2.overridden)
							{
								current2.textureFormat = proceduralPlatformSetting2.textureFormat;
							}
						}
					}
				}
				EditorGUI.BeginChangeCheck();
				proceduralPlatformSetting2.m_LoadBehavior = EditorGUILayout.IntPopup(this.m_Styles.loadBehavior.text, proceduralPlatformSetting2.m_LoadBehavior, ProceduralMaterialInspector.kMaxLoadBehaviorStrings, ProceduralMaterialInspector.kMaxLoadBehaviorValues, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck() && proceduralPlatformSetting2.isDefault)
				{
					foreach (ProceduralMaterialInspector.ProceduralPlatformSetting current3 in this.m_PlatformSettings)
					{
						if (!current3.isDefault && !current3.overridden)
						{
							current3.m_LoadBehavior = proceduralPlatformSetting2.m_LoadBehavior;
						}
					}
				}
				GUILayout.Space(5f);
				using (new EditorGUI.DisabledScope(!this.HasModified()))
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Revert", new GUILayoutOption[0]))
					{
						this.ResetValues();
					}
					if (GUILayout.Button("Apply", new GUILayoutOption[0]))
					{
						this.Apply();
						this.ReimportSubstances();
						this.ResetValues();
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.Space(5f);
				EditorGUILayout.EndPlatformGrouping();
			}
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			base.OnPreviewGUI(r, background);
			if (ProceduralMaterialInspector.ShowIsGenerating(base.target as ProceduralMaterial) && r.width > 50f)
			{
				EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Generating...");
			}
		}

		public void InputsGUI()
		{
			List<string> list = new List<string>();
			Dictionary<string, List<ProceduralPropertyDescription>> dictionary = new Dictionary<string, List<ProceduralPropertyDescription>>();
			Dictionary<string, List<ProceduralPropertyDescription>> dictionary2 = new Dictionary<string, List<ProceduralPropertyDescription>>();
			ProceduralPropertyDescription[] proceduralPropertyDescriptions = ProceduralMaterialInspector.m_Material.GetProceduralPropertyDescriptions();
			ProceduralPropertyDescription proceduralPropertyDescription = null;
			ProceduralPropertyDescription proceduralPropertyDescription2 = null;
			ProceduralPropertyDescription proceduralPropertyDescription3 = null;
			ProceduralPropertyDescription[] array = proceduralPropertyDescriptions;
			for (int i = 0; i < array.Length; i++)
			{
				ProceduralPropertyDescription proceduralPropertyDescription4 = array[i];
				if (proceduralPropertyDescription4.name == "$randomseed")
				{
					this.InputSeedGUI(proceduralPropertyDescription4);
				}
				else if (proceduralPropertyDescription4.name.Length <= 0 || proceduralPropertyDescription4.name[0] != '$')
				{
					if (ProceduralMaterialInspector.m_Material.IsProceduralPropertyVisible(proceduralPropertyDescription4.name))
					{
						string group = proceduralPropertyDescription4.group;
						if (group != string.Empty && !list.Contains(group))
						{
							list.Add(group);
						}
						if (proceduralPropertyDescription4.name == "Hue_Shift" && proceduralPropertyDescription4.type == ProceduralPropertyType.Float && group == string.Empty)
						{
							proceduralPropertyDescription = proceduralPropertyDescription4;
						}
						if (proceduralPropertyDescription4.name == "Saturation" && proceduralPropertyDescription4.type == ProceduralPropertyType.Float && group == string.Empty)
						{
							proceduralPropertyDescription2 = proceduralPropertyDescription4;
						}
						if (proceduralPropertyDescription4.name == "Luminosity" && proceduralPropertyDescription4.type == ProceduralPropertyType.Float && group == string.Empty)
						{
							proceduralPropertyDescription3 = proceduralPropertyDescription4;
						}
						if (proceduralPropertyDescription4.type == ProceduralPropertyType.Texture)
						{
							if (!dictionary2.ContainsKey(group))
							{
								dictionary2.Add(group, new List<ProceduralPropertyDescription>());
							}
							dictionary2[group].Add(proceduralPropertyDescription4);
						}
						else
						{
							if (!dictionary.ContainsKey(group))
							{
								dictionary.Add(group, new List<ProceduralPropertyDescription>());
							}
							dictionary[group].Add(proceduralPropertyDescription4);
						}
					}
				}
			}
			bool flag = false;
			if (proceduralPropertyDescription != null && proceduralPropertyDescription2 != null && proceduralPropertyDescription3 != null)
			{
				flag = true;
			}
			List<ProceduralPropertyDescription> list2;
			if (dictionary.TryGetValue(string.Empty, out list2))
			{
				foreach (ProceduralPropertyDescription current in list2)
				{
					if (!flag || (current != proceduralPropertyDescription && current != proceduralPropertyDescription2 && current != proceduralPropertyDescription3))
					{
						this.InputGUI(current);
					}
				}
			}
			foreach (string current2 in list)
			{
				ProceduralMaterial proceduralMaterial = base.target as ProceduralMaterial;
				string name = proceduralMaterial.name;
				string key = name + current2;
				GUILayout.Space(5f);
				bool flag2 = EditorPrefs.GetBool(key, true);
				EditorGUI.BeginChangeCheck();
				flag2 = EditorGUILayout.Foldout(flag2, current2, true);
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetBool(key, flag2);
				}
				if (flag2)
				{
					EditorGUI.indentLevel++;
					List<ProceduralPropertyDescription> list3;
					if (dictionary.TryGetValue(current2, out list3))
					{
						foreach (ProceduralPropertyDescription current3 in list3)
						{
							this.InputGUI(current3);
						}
					}
					List<ProceduralPropertyDescription> list4;
					if (dictionary2.TryGetValue(current2, out list4))
					{
						GUILayout.Space(2f);
						foreach (ProceduralPropertyDescription current4 in list4)
						{
							this.InputGUI(current4);
						}
					}
					EditorGUI.indentLevel--;
				}
			}
			if (flag)
			{
				this.InputHSLGUI(proceduralPropertyDescription, proceduralPropertyDescription2, proceduralPropertyDescription3);
			}
			List<ProceduralPropertyDescription> list5;
			if (dictionary2.TryGetValue(string.Empty, out list5))
			{
				GUILayout.Space(5f);
				foreach (ProceduralPropertyDescription current5 in list5)
				{
					this.InputGUI(current5);
				}
			}
		}

		private void InputGUI(ProceduralPropertyDescription input)
		{
			ProceduralPropertyType type = input.type;
			GUIContent gUIContent = new GUIContent(input.label, input.name);
			switch (type)
			{
			case ProceduralPropertyType.Boolean:
			{
				EditorGUI.BeginChangeCheck();
				bool value = EditorGUILayout.Toggle(gUIContent, ProceduralMaterialInspector.m_Material.GetProceduralBoolean(input.name), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.RecordForUndo(ProceduralMaterialInspector.m_Material, ProceduralMaterialInspector.m_Importer, "Modified property " + input.name + " for material " + ProceduralMaterialInspector.m_Material.name);
					ProceduralMaterialInspector.m_Material.SetProceduralBoolean(input.name, value);
				}
				break;
			}
			case ProceduralPropertyType.Float:
			{
				EditorGUI.BeginChangeCheck();
				float value2;
				if (input.hasRange)
				{
					float minimum = input.minimum;
					float maximum = input.maximum;
					value2 = EditorGUILayout.Slider(gUIContent, ProceduralMaterialInspector.m_Material.GetProceduralFloat(input.name), minimum, maximum, new GUILayoutOption[0]);
				}
				else
				{
					value2 = EditorGUILayout.FloatField(gUIContent, ProceduralMaterialInspector.m_Material.GetProceduralFloat(input.name), new GUILayoutOption[0]);
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.RecordForUndo(ProceduralMaterialInspector.m_Material, ProceduralMaterialInspector.m_Importer, "Modified property " + input.name + " for material " + ProceduralMaterialInspector.m_Material.name);
					ProceduralMaterialInspector.m_Material.SetProceduralFloat(input.name, value2);
				}
				break;
			}
			case ProceduralPropertyType.Vector2:
			case ProceduralPropertyType.Vector3:
			case ProceduralPropertyType.Vector4:
			{
				int num = (type != ProceduralPropertyType.Vector2) ? ((type != ProceduralPropertyType.Vector3) ? 4 : 3) : 2;
				Vector4 vector = ProceduralMaterialInspector.m_Material.GetProceduralVector(input.name);
				EditorGUI.BeginChangeCheck();
				if (input.hasRange)
				{
					float minimum2 = input.minimum;
					float maximum2 = input.maximum;
					EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Space((float)(EditorGUI.indentLevel * 15));
					GUILayout.Label(gUIContent, new GUILayoutOption[0]);
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;
					for (int i = 0; i < num; i++)
					{
						vector[i] = EditorGUILayout.Slider(new GUIContent(input.componentLabels[i]), vector[i], minimum2, maximum2, new GUILayoutOption[0]);
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.EndVertical();
				}
				else if (num != 2)
				{
					if (num != 3)
					{
						if (num == 4)
						{
							vector = EditorGUILayout.Vector4Field(input.name, vector, new GUILayoutOption[0]);
						}
					}
					else
					{
						vector = EditorGUILayout.Vector3Field(input.name, vector, new GUILayoutOption[0]);
					}
				}
				else
				{
					vector = EditorGUILayout.Vector2Field(input.name, vector, new GUILayoutOption[0]);
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.RecordForUndo(ProceduralMaterialInspector.m_Material, ProceduralMaterialInspector.m_Importer, "Modified property " + input.name + " for material " + ProceduralMaterialInspector.m_Material.name);
					ProceduralMaterialInspector.m_Material.SetProceduralVector(input.name, vector);
				}
				break;
			}
			case ProceduralPropertyType.Color3:
			case ProceduralPropertyType.Color4:
			{
				EditorGUI.BeginChangeCheck();
				Color value3 = EditorGUILayout.ColorField(gUIContent, ProceduralMaterialInspector.m_Material.GetProceduralColor(input.name), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.RecordForUndo(ProceduralMaterialInspector.m_Material, ProceduralMaterialInspector.m_Importer, "Modified property " + input.name + " for material " + ProceduralMaterialInspector.m_Material.name);
					ProceduralMaterialInspector.m_Material.SetProceduralColor(input.name, value3);
				}
				break;
			}
			case ProceduralPropertyType.Enum:
			{
				GUIContent[] array = new GUIContent[input.enumOptions.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = new GUIContent(input.enumOptions[j]);
				}
				EditorGUI.BeginChangeCheck();
				int value4 = EditorGUILayout.Popup(gUIContent, ProceduralMaterialInspector.m_Material.GetProceduralEnum(input.name), array, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.RecordForUndo(ProceduralMaterialInspector.m_Material, ProceduralMaterialInspector.m_Importer, "Modified property " + input.name + " for material " + ProceduralMaterialInspector.m_Material.name);
					ProceduralMaterialInspector.m_Material.SetProceduralEnum(input.name, value4);
				}
				break;
			}
			case ProceduralPropertyType.Texture:
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space((float)(EditorGUI.indentLevel * 15));
				GUILayout.Label(gUIContent, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				Rect rect = GUILayoutUtility.GetRect(64f, 64f, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				EditorGUI.BeginChangeCheck();
				Texture2D value5 = EditorGUI.DoObjectField(rect, rect, GUIUtility.GetControlID(12354, FocusType.Keyboard, rect), ProceduralMaterialInspector.m_Material.GetProceduralTexture(input.name), typeof(Texture2D), null, null, false) as Texture2D;
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck())
				{
					this.RecordForUndo(ProceduralMaterialInspector.m_Material, ProceduralMaterialInspector.m_Importer, "Modified property " + input.name + " for material " + ProceduralMaterialInspector.m_Material.name);
					ProceduralMaterialInspector.m_Material.SetProceduralTexture(input.name, value5);
				}
				break;
			}
			}
		}

		private void InputHSLGUI(ProceduralPropertyDescription hInput, ProceduralPropertyDescription sInput, ProceduralPropertyDescription lInput)
		{
			GUILayout.Space(5f);
			this.m_ShowHSLInputs = EditorPrefs.GetBool("ProceduralShowHSL", true);
			EditorGUI.BeginChangeCheck();
			this.m_ShowHSLInputs = EditorGUILayout.Foldout(this.m_ShowHSLInputs, this.m_Styles.hslContent, true);
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool("ProceduralShowHSL", this.m_ShowHSLInputs);
			}
			if (this.m_ShowHSLInputs)
			{
				EditorGUI.indentLevel++;
				this.InputGUI(hInput);
				this.InputGUI(sInput);
				this.InputGUI(lInput);
				EditorGUI.indentLevel--;
			}
		}

		private void InputSeedGUI(ProceduralPropertyDescription input)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			float value = (float)this.RandomIntField(controlRect, this.m_Styles.randomSeedContent, (int)ProceduralMaterialInspector.m_Material.GetProceduralFloat(input.name), 0, 9999);
			if (EditorGUI.EndChangeCheck())
			{
				this.RecordForUndo(ProceduralMaterialInspector.m_Material, ProceduralMaterialInspector.m_Importer, "Modified random seed for material " + ProceduralMaterialInspector.m_Material.name);
				ProceduralMaterialInspector.m_Material.SetProceduralFloat(input.name, value);
			}
		}

		internal int RandomIntField(Rect position, GUIContent label, int val, int min, int max)
		{
			position = EditorGUI.PrefixLabel(position, 0, label);
			return this.RandomIntField(position, val, min, max);
		}

		internal int RandomIntField(Rect position, int val, int min, int max)
		{
			position.width = position.width - EditorGUIUtility.fieldWidth - 5f;
			if (GUI.Button(position, this.m_Styles.randomizeButtonContent, EditorStyles.miniButton))
			{
				val = UnityEngine.Random.Range(min, max + 1);
			}
			position.x += position.width + 5f;
			position.width = EditorGUIUtility.fieldWidth;
			val = Mathf.Clamp(EditorGUI.IntField(position, val), min, max);
			return val;
		}
	}
}
