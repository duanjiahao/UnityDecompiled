using System;
using UnityEngine;
namespace UnityEditor
{
	internal class ModelImporterModelEditor : AssetImporterInspector
	{
		private class Styles
		{
			public GUIContent Meshes = EditorGUIUtility.TextContent("ModelImporter.Meshes");
			public GUIContent ScaleFactor = EditorGUIUtility.TextContent("ModelImporter.ScaleFactor");
			public GUIContent UseFileUnits = EditorGUIUtility.TextContent("ModelImporter.UseFileUnits");
			public GUIContent ImportBlendShapes = EditorGUIUtility.TextContent("ModelImporter.ImportBlendShapes");
			public GUIContent GenerateColliders = EditorGUIUtility.TextContent("ModelImporter.GenerateColliders");
			public GUIContent SwapUVChannels = EditorGUIUtility.TextContent("ModelImporter.SwapUVChannels");
			public GUIContent GenerateSecondaryUV = EditorGUIUtility.TextContent("ModelImporterGenerateSecondaryUV");
			public GUIContent GenerateSecondaryUVAdvanced = EditorGUIUtility.TextContent("ModelImporterGenerateSecondaryUVAdvanced");
			public GUIContent secondaryUVAngleDistortion = EditorGUIUtility.TextContent("ModelImporterSecondaryUVAngleDistortion");
			public GUIContent secondaryUVAreaDistortion = EditorGUIUtility.TextContent("ModelImporterSecondaryUVAreaDistortion");
			public GUIContent secondaryUVHardAngle = EditorGUIUtility.TextContent("ModelImporterSecondaryUVHardAngle");
			public GUIContent secondaryUVPackMargin = EditorGUIUtility.TextContent("ModelImporterSecondaryUVPackMargin");
			public GUIContent secondaryUVDefaults = EditorGUIUtility.TextContent("ModelImporterSecondaryUVDefaults");
			public GUIContent TangentSpace = EditorGUIUtility.TextContent("ModelImporter.TangentSpace.Title");
			public GUIContent TangentSpaceNormalLabel = EditorGUIUtility.TextContent("ModelImporter.TangentSpace.Normals");
			public GUIContent TangentSpaceTangentLabel = EditorGUIUtility.TextContent("ModelImporter.TangentSpace.Tangents");
			public GUIContent TangentSpaceOptionImport = EditorGUIUtility.TextContent("ModelImporter.TangentSpace.Options.Import");
			public GUIContent TangentSpaceOptionCalculate = EditorGUIUtility.TextContent("ModelImporter.TangentSpace.Options.Calculate");
			public GUIContent TangentSpaceOptionNone = EditorGUIUtility.TextContent("ModelImporter.TangentSpace.Options.None");
			public GUIContent[] TangentSpaceModeOptLabelsAll;
			public GUIContent[] TangentSpaceModeOptLabelsCalculate;
			public GUIContent[] TangentSpaceModeOptLabelsNone;
			public ModelImporterTangentSpaceMode[] TangentSpaceModeOptEnumsAll;
			public ModelImporterTangentSpaceMode[] TangentSpaceModeOptEnumsCalculate;
			public ModelImporterTangentSpaceMode[] TangentSpaceModeOptEnumsNone;
			public GUIContent SmoothingAngle = EditorGUIUtility.TextContent("ModelImporter.TangentSpace.NormalSmoothingAngle");
			public GUIContent SplitTangents = EditorGUIUtility.TextContent("ModelImporter.TangentSpace.SplitTangents");
			public GUIContent MeshCompressionLabel = new GUIContent("Mesh Compression");
			public GUIContent[] MeshCompressionOpt = new GUIContent[]
			{
				new GUIContent("Off"),
				new GUIContent("Low"),
				new GUIContent("Medium"),
				new GUIContent("High")
			};
			public GUIContent OptimizeMeshForGPU = EditorGUIUtility.TextContent("ModelImporterOptimizeMesh");
			public GUIContent IsReadable = EditorGUIUtility.TextContent("ModelImporterIsReadable");
			public GUIContent Materials = EditorGUIUtility.TextContent("ModelImporterMaterials");
			public GUIContent ImportMaterials = EditorGUIUtility.TextContent("ModelImporterMatImportMaterials");
			public GUIContent MaterialName = EditorGUIUtility.TextContent("ModelImporterMatMaterialName");
			public GUIContent MaterialNameTex = EditorGUIUtility.TextContent("ModelImporterMatMaterialNameTex");
			public GUIContent MaterialNameMat = EditorGUIUtility.TextContent("ModelImporterMatMaterialNameMat");
			public GUIContent[] MaterialNameOptMain = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameTex"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameMat"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameModelMat")
			};
			public GUIContent[] MaterialNameOptAll = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameTex"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameMat"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameModelMat"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameTex_ModelMat")
			};
			public GUIContent MaterialSearch = EditorGUIUtility.TextContent("ModelImporterMatMaterialSearch");
			public GUIContent[] MaterialSearchOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ModelImporterMatMaterialSearchLocal"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialSearchRecursiveUp"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialSearchEverywhere")
			};
			public GUIContent MaterialHelpStart = EditorGUIUtility.TextContent("ModelImporterMatHelpStart");
			public GUIContent MaterialHelpEnd = EditorGUIUtility.TextContent("ModelImporterMatHelpEnd");
			public GUIContent MaterialHelpDefault = EditorGUIUtility.TextContent("ModelImporterMatDefaultHelp");
			public GUIContent[] MaterialNameHelp = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameTexHelp"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameMatHelp"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameModelMatHelp"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialNameTex_ModelMatHelp")
			};
			public GUIContent[] MaterialSearchHelp = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ModelImporterMatMaterialSearchLocalHelp"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialSearchRecursiveUpHelp"),
				EditorGUIUtility.TextContent("ModelImporterMatMaterialSearchEverywhereHelp")
			};
			public Styles()
			{
				this.TangentSpaceModeOptLabelsAll = new GUIContent[]
				{
					this.TangentSpaceOptionImport,
					this.TangentSpaceOptionCalculate,
					this.TangentSpaceOptionNone
				};
				this.TangentSpaceModeOptLabelsCalculate = new GUIContent[]
				{
					this.TangentSpaceOptionCalculate,
					this.TangentSpaceOptionNone
				};
				this.TangentSpaceModeOptLabelsNone = new GUIContent[]
				{
					this.TangentSpaceOptionNone
				};
				this.TangentSpaceModeOptEnumsAll = new ModelImporterTangentSpaceMode[]
				{
					ModelImporterTangentSpaceMode.Import,
					ModelImporterTangentSpaceMode.Calculate,
					ModelImporterTangentSpaceMode.None
				};
				this.TangentSpaceModeOptEnumsCalculate = new ModelImporterTangentSpaceMode[]
				{
					ModelImporterTangentSpaceMode.Calculate,
					ModelImporterTangentSpaceMode.None
				};
				this.TangentSpaceModeOptEnumsNone = new ModelImporterTangentSpaceMode[]
				{
					ModelImporterTangentSpaceMode.None
				};
			}
		}
		private bool m_SecondaryUVAdvancedOptions;
		private bool m_ShowAllMaterialNameOptions = true;
		private SerializedProperty m_ImportMaterials;
		private SerializedProperty m_MaterialName;
		private SerializedProperty m_MaterialSearch;
		private SerializedProperty m_GlobalScale;
		private SerializedProperty m_MeshCompression;
		private SerializedProperty m_ImportBlendShapes;
		private SerializedProperty m_AddColliders;
		private SerializedProperty m_SwapUVChannels;
		private SerializedProperty m_GenerateSecondaryUV;
		private SerializedProperty m_UseFileUnits;
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
		private static ModelImporterModelEditor.Styles styles;
		private void UpdateShowAllMaterialNameOptions()
		{
			this.m_MaterialName = base.serializedObject.FindProperty("m_MaterialName");
			this.m_ShowAllMaterialNameOptions = (this.m_MaterialName.intValue == 3);
		}
		private void OnEnable()
		{
			this.m_ImportMaterials = base.serializedObject.FindProperty("m_ImportMaterials");
			this.m_MaterialName = base.serializedObject.FindProperty("m_MaterialName");
			this.m_MaterialSearch = base.serializedObject.FindProperty("m_MaterialSearch");
			this.m_GlobalScale = base.serializedObject.FindProperty("m_GlobalScale");
			this.m_MeshCompression = base.serializedObject.FindProperty("m_MeshCompression");
			this.m_ImportBlendShapes = base.serializedObject.FindProperty("m_ImportBlendShapes");
			this.m_AddColliders = base.serializedObject.FindProperty("m_AddColliders");
			this.m_SwapUVChannels = base.serializedObject.FindProperty("swapUVChannels");
			this.m_GenerateSecondaryUV = base.serializedObject.FindProperty("generateSecondaryUV");
			this.m_UseFileUnits = base.serializedObject.FindProperty("m_UseFileUnits");
			this.m_SecondaryUVAngleDistortion = base.serializedObject.FindProperty("secondaryUVAngleDistortion");
			this.m_SecondaryUVAreaDistortion = base.serializedObject.FindProperty("secondaryUVAreaDistortion");
			this.m_SecondaryUVHardAngle = base.serializedObject.FindProperty("secondaryUVHardAngle");
			this.m_SecondaryUVPackMargin = base.serializedObject.FindProperty("secondaryUVPackMargin");
			this.m_NormalSmoothAngle = base.serializedObject.FindProperty("normalSmoothAngle");
			this.m_SplitTangentsAcrossSeams = base.serializedObject.FindProperty("splitTangentsAcrossUV");
			this.m_NormalImportMode = base.serializedObject.FindProperty("normalImportMode");
			this.m_TangentImportMode = base.serializedObject.FindProperty("tangentImportMode");
			this.m_OptimizeMeshForGPU = base.serializedObject.FindProperty("optimizeMeshForGPU");
			this.m_IsReadable = base.serializedObject.FindProperty("m_IsReadable");
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
			GUILayout.Label(ModelImporterModelEditor.styles.Meshes, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_GlobalScale, ModelImporterModelEditor.styles.ScaleFactor, new GUILayoutOption[0]);
			bool flag = true;
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				ModelImporter modelImporter = (ModelImporter)targets[i];
				if (!modelImporter.isUseFileUnitsSupported)
				{
					flag = false;
				}
			}
			if (flag)
			{
				EditorGUILayout.PropertyField(this.m_UseFileUnits, ModelImporterModelEditor.styles.UseFileUnits, new GUILayoutOption[0]);
			}
			EditorGUILayout.Popup(this.m_MeshCompression, ModelImporterModelEditor.styles.MeshCompressionOpt, ModelImporterModelEditor.styles.MeshCompressionLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_IsReadable, ModelImporterModelEditor.styles.IsReadable, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_OptimizeMeshForGPU, ModelImporterModelEditor.styles.OptimizeMeshForGPU, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ImportBlendShapes, ModelImporterModelEditor.styles.ImportBlendShapes, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AddColliders, ModelImporterModelEditor.styles.GenerateColliders, new GUILayoutOption[0]);
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
			GUILayout.Label(ModelImporterModelEditor.styles.TangentSpace, EditorStyles.boldLabel, new GUILayoutOption[0]);
			bool flag2 = true;
			UnityEngine.Object[] targets2 = base.targets;
			for (int j = 0; j < targets2.Length; j++)
			{
				ModelImporter modelImporter2 = (ModelImporter)targets2[j];
				if (!modelImporter2.isTangentImportSupported)
				{
					flag2 = false;
				}
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Popup(this.m_NormalImportMode, ModelImporterModelEditor.styles.TangentSpaceModeOptLabelsAll, ModelImporterModelEditor.styles.TangentSpaceNormalLabel, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_TangentImportMode.intValue = this.m_NormalImportMode.intValue;
				if (!flag2 && this.m_TangentImportMode.intValue == 0)
				{
					this.m_TangentImportMode.intValue = 1;
				}
			}
			GUIContent[] displayedOptions = ModelImporterModelEditor.styles.TangentSpaceModeOptLabelsAll;
			ModelImporterTangentSpaceMode[] array = ModelImporterModelEditor.styles.TangentSpaceModeOptEnumsAll;
			if (this.m_NormalImportMode.intValue == 1 || !flag2)
			{
				displayedOptions = ModelImporterModelEditor.styles.TangentSpaceModeOptLabelsCalculate;
				array = ModelImporterModelEditor.styles.TangentSpaceModeOptEnumsCalculate;
			}
			else
			{
				if (this.m_NormalImportMode.intValue == 2)
				{
					displayedOptions = ModelImporterModelEditor.styles.TangentSpaceModeOptLabelsNone;
					array = ModelImporterModelEditor.styles.TangentSpaceModeOptEnumsNone;
				}
			}
			EditorGUI.BeginDisabledGroup(this.m_NormalImportMode.intValue == 2);
			int num = Array.IndexOf<ModelImporterTangentSpaceMode>(array, (ModelImporterTangentSpaceMode)this.m_TangentImportMode.intValue);
			EditorGUI.BeginChangeCheck();
			num = EditorGUILayout.Popup(ModelImporterModelEditor.styles.TangentSpaceTangentLabel, num, displayedOptions, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_TangentImportMode.intValue = (int)array[num];
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(this.m_NormalImportMode.intValue != 1);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider(this.m_NormalSmoothAngle, 0f, 180f, ModelImporterModelEditor.styles.SmoothingAngle, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_NormalSmoothAngle.floatValue = Mathf.Round(this.m_NormalSmoothAngle.floatValue);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(this.m_TangentImportMode.intValue != 1);
			EditorGUILayout.PropertyField(this.m_SplitTangentsAcrossSeams, ModelImporterModelEditor.styles.SplitTangents, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
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
			base.ApplyRevertGUI();
		}
		private void ScaleAvatar()
		{
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
