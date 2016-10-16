using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ModelImporterModelEditor : AssetImporterInspector
	{
		private class Styles
		{
			public GUIContent Meshes = EditorGUIUtility.TextContent("Meshes|These options control how geometry is imported.");

			public GUIContent ScaleFactor = EditorGUIUtility.TextContent("Scale Factor|How much to scale the models compared to what is in the source file.");

			public GUIContent UseFileUnits = EditorGUIUtility.TextContent("Use File Units|Detect file units and import as 1FileUnit=1UnityUnit, otherwise it will import as 1cm=1UnityUnit. See ModelImporter.useFileUnits for more details.");

			public GUIContent FileScaleFactor = EditorGUIUtility.TextContent("File Scale|Model scale defined in the source file. If available.");

			public GUIContent ImportBlendShapes = EditorGUIUtility.TextContent("Import BlendShapes|Should Unity import BlendShapes.");

			public GUIContent GenerateColliders = EditorGUIUtility.TextContent("Generate Colliders|Should Unity generate mesh colliders for all meshes.");

			public GUIContent SwapUVChannels = EditorGUIUtility.TextContent("Swap UVs|Swaps the 2 UV channels in meshes. Use if your diffuse texture uses UVs from the lightmap.");

			public GUIContent GenerateSecondaryUV = EditorGUIUtility.TextContent("Generate Lightmap UVs|Generate lightmap UVs into UV2.");

			public GUIContent GenerateSecondaryUVAdvanced = EditorGUIUtility.TextContent("Advanced");

			public GUIContent secondaryUVAngleDistortion = EditorGUIUtility.TextContent("Angle Error|Measured in percents. Angle error measures deviation of UV angles from geometry angles. Area error measures deviation of UV triangles area from geometry triangles if they were uniformly scaled.");

			public GUIContent secondaryUVAreaDistortion = EditorGUIUtility.TextContent("Area Error");

			public GUIContent secondaryUVHardAngle = EditorGUIUtility.TextContent("Hard Angle|Angle between neighbor triangles that will generate seam.");

			public GUIContent secondaryUVPackMargin = EditorGUIUtility.TextContent("Pack Margin|Measured in pixels, assuming mesh will cover an entire 1024x1024 lightmap.");

			public GUIContent secondaryUVDefaults = EditorGUIUtility.TextContent("Set Defaults");

			public GUIContent TangentSpace = EditorGUIUtility.TextContent("Normals & Tangents");

			public GUIContent TangentSpaceNormalLabel = EditorGUIUtility.TextContent("Normals");

			public GUIContent TangentSpaceTangentLabel = EditorGUIUtility.TextContent("Tangents");

			public GUIContent TangentSpaceOptionImport = EditorGUIUtility.TextContent("Import");

			public GUIContent TangentSpaceOptionCalculateLegacy = EditorGUIUtility.TextContent("Calculate Legacy");

			public GUIContent TangentSpaceOptionCalculateLegacySplit = EditorGUIUtility.TextContent("Calculate Legacy - Split Tangents");

			public GUIContent TangentSpaceOptionCalculate = EditorGUIUtility.TextContent("Calculate Tangent Space");

			public GUIContent TangentSpaceOptionNone = EditorGUIUtility.TextContent("None");

			public GUIContent TangentSpaceOptionNoneNoNormals = EditorGUIUtility.TextContent("None - (Normals required)");

			public GUIContent NormalOptionImport = EditorGUIUtility.TextContent("Import");

			public GUIContent NormalOptionCalculate = EditorGUIUtility.TextContent("Calculate");

			public GUIContent NormalOptionNone = EditorGUIUtility.TextContent("None");

			public GUIContent[] TangentSpaceModeOptLabelsAll;

			public GUIContent[] TangentSpaceModeOptLabelsCalculate;

			public GUIContent[] TangentSpaceModeOptLabelsNone;

			public GUIContent[] NormalModeLabelsAll;

			public ModelImporterTangents[] TangentSpaceModeOptEnumsAll;

			public ModelImporterTangents[] TangentSpaceModeOptEnumsCalculate;

			public ModelImporterTangents[] TangentSpaceModeOptEnumsNone;

			public GUIContent SmoothingAngle = EditorGUIUtility.TextContent("Smoothing Angle|Normal Smoothing Angle");

			public GUIContent MeshCompressionLabel = new GUIContent("Mesh Compression", "Higher compression ratio means lower mesh precision. If enabled, the mesh bounds and a lower bit depth per component are used to compress the mesh data.");

			public GUIContent[] MeshCompressionOpt = new GUIContent[]
			{
				new GUIContent("Off"),
				new GUIContent("Low"),
				new GUIContent("Medium"),
				new GUIContent("High")
			};

			public GUIContent OptimizeMeshForGPU = EditorGUIUtility.TextContent("Optimize Mesh|The vertices and indices will be reordered for better GPU performance.");

			public GUIContent KeepQuads = EditorGUIUtility.TextContent("Keep Quads|If model contains quad faces, they are kept for DX11 tessellation.");

			public GUIContent IsReadable = EditorGUIUtility.TextContent("Read/Write Enabled|Allow vertices and indices to be accessed from script.");

			public GUIContent Materials = EditorGUIUtility.TextContent("Materials");

			public GUIContent ImportMaterials = EditorGUIUtility.TextContent("Import Materials");

			public GUIContent MaterialName = EditorGUIUtility.TextContent("Material Naming");

			public GUIContent MaterialNameTex = EditorGUIUtility.TextContent("By Base Texture Name");

			public GUIContent MaterialNameMat = EditorGUIUtility.TextContent("From Model's Material");

			public GUIContent[] MaterialNameOptMain = new GUIContent[]
			{
				EditorGUIUtility.TextContent("By Base Texture Name"),
				EditorGUIUtility.TextContent("From Model's Material"),
				EditorGUIUtility.TextContent("Model Name + Model's Material")
			};

			public GUIContent[] MaterialNameOptAll = new GUIContent[]
			{
				EditorGUIUtility.TextContent("By Base Texture Name"),
				EditorGUIUtility.TextContent("From Model's Material"),
				EditorGUIUtility.TextContent("Model Name + Model's Material"),
				EditorGUIUtility.TextContent("Texture Name or Model Name + Model's Material (Obsolete)")
			};

			public GUIContent MaterialSearch = EditorGUIUtility.TextContent("Material Search");

			public GUIContent[] MaterialSearchOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Local Materials Folder"),
				EditorGUIUtility.TextContent("Recursive-Up"),
				EditorGUIUtility.TextContent("Project-Wide")
			};

			public GUIContent MaterialHelpStart = EditorGUIUtility.TextContent("For each imported material, Unity first looks for an existing material named %MAT%.");

			public GUIContent MaterialHelpEnd = EditorGUIUtility.TextContent("If it doesn't exist, a new one is created in the local Materials folder.");

			public GUIContent MaterialHelpDefault = EditorGUIUtility.TextContent("No new materials are generated. Unity's Default-Diffuse material is used instead.");

			public GUIContent[] MaterialNameHelp = new GUIContent[]
			{
				EditorGUIUtility.TextContent("[BaseTextureName]"),
				EditorGUIUtility.TextContent("[MaterialName]"),
				EditorGUIUtility.TextContent("[ModelFileName]-[MaterialName]"),
				EditorGUIUtility.TextContent("[BaseTextureName] or [ModelFileName]-[MaterialName] if no base texture can be found")
			};

			public GUIContent[] MaterialSearchHelp = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Unity will look for it in the local Materials folder."),
				EditorGUIUtility.TextContent("Unity will do a recursive-up search for it in all Materials folders up to the Assets folder."),
				EditorGUIUtility.TextContent("Unity will search for it anywhere inside the Assets folder.")
			};

			public Styles()
			{
				this.NormalModeLabelsAll = new GUIContent[]
				{
					this.NormalOptionImport,
					this.NormalOptionCalculate,
					this.NormalOptionNone
				};
				this.TangentSpaceModeOptLabelsAll = new GUIContent[]
				{
					this.TangentSpaceOptionImport,
					this.TangentSpaceOptionCalculate,
					this.TangentSpaceOptionCalculateLegacy,
					this.TangentSpaceOptionCalculateLegacySplit,
					this.TangentSpaceOptionNone
				};
				this.TangentSpaceModeOptLabelsCalculate = new GUIContent[]
				{
					this.TangentSpaceOptionCalculate,
					this.TangentSpaceOptionCalculateLegacy,
					this.TangentSpaceOptionCalculateLegacySplit,
					this.TangentSpaceOptionNone
				};
				this.TangentSpaceModeOptLabelsNone = new GUIContent[]
				{
					this.TangentSpaceOptionNoneNoNormals
				};
				this.TangentSpaceModeOptEnumsAll = new ModelImporterTangents[]
				{
					ModelImporterTangents.Import,
					ModelImporterTangents.CalculateMikk,
					ModelImporterTangents.CalculateLegacy,
					ModelImporterTangents.CalculateLegacyWithSplitTangents,
					ModelImporterTangents.None
				};
				this.TangentSpaceModeOptEnumsCalculate = new ModelImporterTangents[]
				{
					ModelImporterTangents.CalculateMikk,
					ModelImporterTangents.CalculateLegacy,
					ModelImporterTangents.CalculateLegacyWithSplitTangents,
					ModelImporterTangents.None
				};
				this.TangentSpaceModeOptEnumsNone = new ModelImporterTangents[]
				{
					ModelImporterTangents.None
				};
			}
		}

		private bool m_SecondaryUVAdvancedOptions;

		private bool m_ShowAllMaterialNameOptions = true;

		private SerializedProperty m_ImportMaterials;

		private SerializedProperty m_MaterialName;

		private SerializedProperty m_MaterialSearch;

		private SerializedProperty m_GlobalScale;

		private SerializedProperty m_FileScale;

		private SerializedProperty m_MeshCompression;

		private SerializedProperty m_ImportBlendShapes;

		private SerializedProperty m_AddColliders;

		private SerializedProperty m_SwapUVChannels;

		private SerializedProperty m_GenerateSecondaryUV;

		private SerializedProperty m_SecondaryUVAngleDistortion;

		private SerializedProperty m_SecondaryUVAreaDistortion;

		private SerializedProperty m_SecondaryUVHardAngle;

		private SerializedProperty m_SecondaryUVPackMargin;

		private SerializedProperty m_NormalSmoothAngle;

		private SerializedProperty m_SplitTangentsAcrossSeams;

		private SerializedProperty m_NormalImportMode;

		private SerializedProperty m_TangentImportMode;

		private SerializedProperty m_OptimizeMeshForGPU;

		private SerializedProperty m_IsReadable;

		private SerializedProperty m_KeepQuads;

		private static ModelImporterModelEditor.Styles styles;

		private void UpdateShowAllMaterialNameOptions()
		{
			this.m_MaterialName = base.serializedObject.FindProperty("m_MaterialName");
			this.m_ShowAllMaterialNameOptions = (this.m_MaterialName.intValue == 3);
		}

		internal virtual void OnEnable()
		{
			this.m_ImportMaterials = base.serializedObject.FindProperty("m_ImportMaterials");
			this.m_MaterialName = base.serializedObject.FindProperty("m_MaterialName");
			this.m_MaterialSearch = base.serializedObject.FindProperty("m_MaterialSearch");
			this.m_GlobalScale = base.serializedObject.FindProperty("m_GlobalScale");
			this.m_FileScale = base.serializedObject.FindProperty("m_FileScale");
			this.m_MeshCompression = base.serializedObject.FindProperty("m_MeshCompression");
			this.m_ImportBlendShapes = base.serializedObject.FindProperty("m_ImportBlendShapes");
			this.m_AddColliders = base.serializedObject.FindProperty("m_AddColliders");
			this.m_SwapUVChannels = base.serializedObject.FindProperty("swapUVChannels");
			this.m_GenerateSecondaryUV = base.serializedObject.FindProperty("generateSecondaryUV");
			this.m_SecondaryUVAngleDistortion = base.serializedObject.FindProperty("secondaryUVAngleDistortion");
			this.m_SecondaryUVAreaDistortion = base.serializedObject.FindProperty("secondaryUVAreaDistortion");
			this.m_SecondaryUVHardAngle = base.serializedObject.FindProperty("secondaryUVHardAngle");
			this.m_SecondaryUVPackMargin = base.serializedObject.FindProperty("secondaryUVPackMargin");
			this.m_NormalSmoothAngle = base.serializedObject.FindProperty("normalSmoothAngle");
			this.m_NormalImportMode = base.serializedObject.FindProperty("normalImportMode");
			this.m_TangentImportMode = base.serializedObject.FindProperty("tangentImportMode");
			this.m_OptimizeMeshForGPU = base.serializedObject.FindProperty("optimizeMeshForGPU");
			this.m_IsReadable = base.serializedObject.FindProperty("m_IsReadable");
			this.m_KeepQuads = base.serializedObject.FindProperty("keepQuads");
			this.UpdateShowAllMaterialNameOptions();
		}

		internal override void ResetValues()
		{
			base.ResetValues();
			this.UpdateShowAllMaterialNameOptions();
		}

		internal override void Apply()
		{
			this.ScaleAvatar();
			base.Apply();
			this.UpdateShowAllMaterialNameOptions();
		}

		public override void OnInspectorGUI()
		{
			if (ModelImporterModelEditor.styles == null)
			{
				ModelImporterModelEditor.styles = new ModelImporterModelEditor.Styles();
			}
			this.MeshesGUI();
			this.NormalsAndTangentsGUI();
			this.MaterialsGUI();
			base.ApplyRevertGUI();
		}

		private void MeshesGUI()
		{
			GUILayout.Label(ModelImporterModelEditor.styles.Meshes, EditorStyles.boldLabel, new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(base.targets.Length > 1))
			{
				EditorGUILayout.PropertyField(this.m_GlobalScale, ModelImporterModelEditor.styles.ScaleFactor, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_FileScale, ModelImporterModelEditor.styles.FileScaleFactor, new GUILayoutOption[0]);
			EditorGUILayout.Popup(this.m_MeshCompression, ModelImporterModelEditor.styles.MeshCompressionOpt, ModelImporterModelEditor.styles.MeshCompressionLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_IsReadable, ModelImporterModelEditor.styles.IsReadable, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_OptimizeMeshForGPU, ModelImporterModelEditor.styles.OptimizeMeshForGPU, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ImportBlendShapes, ModelImporterModelEditor.styles.ImportBlendShapes, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AddColliders, ModelImporterModelEditor.styles.GenerateColliders, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_KeepQuads, ModelImporterModelEditor.styles.KeepQuads, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_SwapUVChannels, ModelImporterModelEditor.styles.SwapUVChannels, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_GenerateSecondaryUV, ModelImporterModelEditor.styles.GenerateSecondaryUV, new GUILayoutOption[0]);
			if (this.m_GenerateSecondaryUV.boolValue)
			{
				EditorGUI.indentLevel++;
				this.m_SecondaryUVAdvancedOptions = EditorGUILayout.Foldout(this.m_SecondaryUVAdvancedOptions, ModelImporterModelEditor.styles.GenerateSecondaryUVAdvanced, EditorStyles.foldout);
				if (this.m_SecondaryUVAdvancedOptions)
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.Slider(this.m_SecondaryUVHardAngle, 0f, 180f, ModelImporterModelEditor.styles.secondaryUVHardAngle, new GUILayoutOption[0]);
					EditorGUILayout.Slider(this.m_SecondaryUVPackMargin, 1f, 64f, ModelImporterModelEditor.styles.secondaryUVPackMargin, new GUILayoutOption[0]);
					EditorGUILayout.Slider(this.m_SecondaryUVAngleDistortion, 1f, 75f, ModelImporterModelEditor.styles.secondaryUVAngleDistortion, new GUILayoutOption[0]);
					EditorGUILayout.Slider(this.m_SecondaryUVAreaDistortion, 1f, 75f, ModelImporterModelEditor.styles.secondaryUVAreaDistortion, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						this.m_SecondaryUVHardAngle.floatValue = Mathf.Round(this.m_SecondaryUVHardAngle.floatValue);
						this.m_SecondaryUVPackMargin.floatValue = Mathf.Round(this.m_SecondaryUVPackMargin.floatValue);
						this.m_SecondaryUVAngleDistortion.floatValue = Mathf.Round(this.m_SecondaryUVAngleDistortion.floatValue);
						this.m_SecondaryUVAreaDistortion.floatValue = Mathf.Round(this.m_SecondaryUVAreaDistortion.floatValue);
					}
				}
				EditorGUI.indentLevel--;
			}
		}

		private void MaterialsGUI()
		{
			GUILayout.Label(ModelImporterModelEditor.styles.Materials, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ImportMaterials, ModelImporterModelEditor.styles.ImportMaterials, new GUILayoutOption[0]);
			string text;
			if (this.m_ImportMaterials.boolValue)
			{
				EditorGUILayout.Popup(this.m_MaterialName, (!this.m_ShowAllMaterialNameOptions) ? ModelImporterModelEditor.styles.MaterialNameOptMain : ModelImporterModelEditor.styles.MaterialNameOptAll, ModelImporterModelEditor.styles.MaterialName, new GUILayoutOption[0]);
				EditorGUILayout.Popup(this.m_MaterialSearch, ModelImporterModelEditor.styles.MaterialSearchOpt, ModelImporterModelEditor.styles.MaterialSearch, new GUILayoutOption[0]);
				text = string.Concat(new string[]
				{
					ModelImporterModelEditor.styles.MaterialHelpStart.text.Replace("%MAT%", ModelImporterModelEditor.styles.MaterialNameHelp[this.m_MaterialName.intValue].text),
					"\n",
					ModelImporterModelEditor.styles.MaterialSearchHelp[this.m_MaterialSearch.intValue].text,
					"\n",
					ModelImporterModelEditor.styles.MaterialHelpEnd.text
				});
			}
			else
			{
				text = ModelImporterModelEditor.styles.MaterialHelpDefault.text;
			}
			GUILayout.Label(new GUIContent(text), EditorStyles.helpBox, new GUILayoutOption[0]);
		}

		private void NormalsAndTangentsGUI()
		{
			GUILayout.Label(ModelImporterModelEditor.styles.TangentSpace, EditorStyles.boldLabel, new GUILayoutOption[0]);
			bool flag = true;
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				ModelImporter modelImporter = (ModelImporter)targets[i];
				if (!modelImporter.isTangentImportSupported)
				{
					flag = false;
				}
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Popup(this.m_NormalImportMode, ModelImporterModelEditor.styles.NormalModeLabelsAll, ModelImporterModelEditor.styles.TangentSpaceNormalLabel, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				if (this.m_NormalImportMode.intValue == 2)
				{
					this.m_TangentImportMode.intValue = 2;
				}
				else if (this.m_NormalImportMode.intValue == 0 && flag)
				{
					this.m_TangentImportMode.intValue = 0;
				}
				else
				{
					this.m_TangentImportMode.intValue = 3;
				}
			}
			using (new EditorGUI.DisabledScope(this.m_NormalImportMode.intValue != 1))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider(this.m_NormalSmoothAngle, 0f, 180f, ModelImporterModelEditor.styles.SmoothingAngle, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_NormalSmoothAngle.floatValue = Mathf.Round(this.m_NormalSmoothAngle.floatValue);
				}
			}
			GUIContent[] displayedOptions = ModelImporterModelEditor.styles.TangentSpaceModeOptLabelsAll;
			ModelImporterTangents[] array = ModelImporterModelEditor.styles.TangentSpaceModeOptEnumsAll;
			if (this.m_NormalImportMode.intValue == 1 || !flag)
			{
				displayedOptions = ModelImporterModelEditor.styles.TangentSpaceModeOptLabelsCalculate;
				array = ModelImporterModelEditor.styles.TangentSpaceModeOptEnumsCalculate;
			}
			else if (this.m_NormalImportMode.intValue == 2)
			{
				displayedOptions = ModelImporterModelEditor.styles.TangentSpaceModeOptLabelsNone;
				array = ModelImporterModelEditor.styles.TangentSpaceModeOptEnumsNone;
			}
			using (new EditorGUI.DisabledScope(this.m_NormalImportMode.intValue == 2))
			{
				int num = Array.IndexOf<ModelImporterTangents>(array, (ModelImporterTangents)this.m_TangentImportMode.intValue);
				EditorGUI.BeginChangeCheck();
				num = EditorGUILayout.Popup(ModelImporterModelEditor.styles.TangentSpaceTangentLabel, num, displayedOptions, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_TangentImportMode.intValue = (int)array[num];
				}
			}
		}

		private void ScaleAvatar()
		{
			if (this.m_GlobalScale.hasMultipleDifferentValues)
			{
				return;
			}
			if (base.targets.Length != 1)
			{
				return;
			}
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				object obj = targets[i];
				float globalScale = (obj as ModelImporter).globalScale;
				float floatValue = this.m_GlobalScale.floatValue;
				if (globalScale != floatValue && floatValue != 0f && globalScale != 0f)
				{
					float d = floatValue / globalScale;
					SerializedProperty serializedProperty = base.serializedObject.FindProperty(AvatarSetupTool.sSkeleton);
					for (int j = 0; j < serializedProperty.arraySize; j++)
					{
						SerializedProperty arrayElementAtIndex = serializedProperty.GetArrayElementAtIndex(j);
						arrayElementAtIndex.FindPropertyRelative(AvatarSetupTool.sPosition).vector3Value = arrayElementAtIndex.FindPropertyRelative(AvatarSetupTool.sPosition).vector3Value * d;
					}
				}
			}
		}
	}
}
