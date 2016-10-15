using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SpeedTreeImporter))]
	internal class SpeedTreeImporterInspector : AssetImporterInspector
	{
		private class Styles
		{
			public static GUIContent LODHeader = EditorGUIUtility.TextContent("LODs");

			public static GUIContent ResetLOD = EditorGUIUtility.TextContent("Reset LOD to...|Unify the LOD settings for all selected assets.");

			public static GUIContent SmoothLOD = EditorGUIUtility.TextContent("Smooth LOD|Toggles smooth LOD transitions.");

			public static GUIContent AnimateCrossFading = EditorGUIUtility.TextContent("Animate Cross-fading|Cross-fading is animated instead of being calculated by distance.");

			public static GUIContent CrossFadeWidth = EditorGUIUtility.TextContent("Crossfade Width|Proportion of the last 3D mesh LOD region width which is used for cross-fading to billboard tree.");

			public static GUIContent FadeOutWidth = EditorGUIUtility.TextContent("Fade Out Width|Proportion of the billboard LOD region width which is used for fading out the billboard.");

			public static GUIContent MeshesHeader = EditorGUIUtility.TextContent("Meshes");

			public static GUIContent ScaleFactor = EditorGUIUtility.TextContent("Scale Factor|How much to scale the tree model compared to what is in the .spm file.");

			public static GUIContent ScaleFactorHelp = EditorGUIUtility.TextContent("The default value of Scale Factor is 0.3048, the conversion ratio from feet to meters, as these are the most conventional measurements used in SpeedTree and Unity, respectively.");

			public static GUIContent MaterialsHeader = EditorGUIUtility.TextContent("Materials");

			public static GUIContent MainColor = EditorGUIUtility.TextContent("Main Color|The color modulating the diffuse lighting component.");

			public static GUIContent HueVariation = EditorGUIUtility.TextContent("Hue Color|Apply to LODs that have Hue Variation effect enabled.");

			public static GUIContent AlphaTestRef = EditorGUIUtility.TextContent("Alpha Cutoff|The alpha-test reference value.");

			public static GUIContent CastShadows = EditorGUIUtility.TextContent("Cast Shadows|The tree casts shadow.");

			public static GUIContent ReceiveShadows = EditorGUIUtility.TextContent("Receive Shadows|The tree receives shadow.");

			public static GUIContent UseLightProbes = EditorGUIUtility.TextContent("Use Light Probes|The tree uses light probe for lighting.");

			public static GUIContent UseReflectionProbes = EditorGUIUtility.TextContent("Use Reflection Probes|The tree uses reflection probe for rendering.");

			public static GUIContent EnableBump = EditorGUIUtility.TextContent("Normal Map|Enable normal mapping (aka Bump mapping).");

			public static GUIContent EnableHue = EditorGUIUtility.TextContent("Enable Hue Variation|Enable Hue variation color (color is adjusted between Main Color and Hue Color).");

			public static GUIContent WindQuality = EditorGUIUtility.TextContent("Wind Quality|Controls the wind quality.");

			public static GUIContent ApplyAndGenerate = EditorGUIUtility.TextContent("Apply & Generate Materials|Apply current importer settings and generate materials with new settings.");

			public static GUIContent Regenerate = EditorGUIUtility.TextContent("Regenerate Materials|Regenerate materials from the current importer settings.");
		}

		private const float kFeetToMetersRatio = 0.3048f;

		private SerializedProperty m_LODSettings;

		private SerializedProperty m_EnableSmoothLOD;

		private SerializedProperty m_AnimateCrossFading;

		private SerializedProperty m_BillboardTransitionCrossFadeWidth;

		private SerializedProperty m_FadeOutWidth;

		private SerializedProperty m_MainColor;

		private SerializedProperty m_HueVariation;

		private SerializedProperty m_AlphaTestRef;

		private SerializedProperty m_ScaleFactor;

		private int m_SelectedLODSlider = -1;

		private int m_SelectedLODRange;

		private readonly AnimBool m_ShowSmoothLODOptions = new AnimBool();

		private readonly AnimBool m_ShowCrossFadeWidthOptions = new AnimBool();

		private readonly int m_LODSliderId = "LODSliderIDHash".GetHashCode();

		private SpeedTreeImporter[] importers
		{
			get
			{
				return base.targets.Cast<SpeedTreeImporter>().ToArray<SpeedTreeImporter>();
			}
		}

		private bool upgradeMaterials
		{
			get
			{
				return this.importers.Any((SpeedTreeImporter i) => i.materialsShouldBeRegenerated);
			}
		}

		private void OnEnable()
		{
			this.m_LODSettings = base.serializedObject.FindProperty("m_LODSettings");
			this.m_EnableSmoothLOD = base.serializedObject.FindProperty("m_EnableSmoothLODTransition");
			this.m_AnimateCrossFading = base.serializedObject.FindProperty("m_AnimateCrossFading");
			this.m_BillboardTransitionCrossFadeWidth = base.serializedObject.FindProperty("m_BillboardTransitionCrossFadeWidth");
			this.m_FadeOutWidth = base.serializedObject.FindProperty("m_FadeOutWidth");
			this.m_MainColor = base.serializedObject.FindProperty("m_MainColor");
			this.m_HueVariation = base.serializedObject.FindProperty("m_HueVariation");
			this.m_AlphaTestRef = base.serializedObject.FindProperty("m_AlphaTestRef");
			this.m_ScaleFactor = base.serializedObject.FindProperty("m_ScaleFactor");
			this.m_ShowSmoothLODOptions.value = (this.m_EnableSmoothLOD.hasMultipleDifferentValues || this.m_EnableSmoothLOD.boolValue);
			this.m_ShowSmoothLODOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowCrossFadeWidthOptions.value = (this.m_AnimateCrossFading.hasMultipleDifferentValues || !this.m_AnimateCrossFading.boolValue);
			this.m_ShowCrossFadeWidthOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_ShowSmoothLODOptions.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowCrossFadeWidthOptions.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		protected override bool ApplyRevertGUIButtons()
		{
			bool result;
			using (new EditorGUI.DisabledScope(!this.HasModified()))
			{
				base.RevertButton();
				result = base.ApplyButton("Apply Prefab");
			}
			bool upgradeMaterials = this.upgradeMaterials;
			GUIContent content = (!this.HasModified() && !upgradeMaterials) ? SpeedTreeImporterInspector.Styles.Regenerate : SpeedTreeImporterInspector.Styles.ApplyAndGenerate;
			if (GUILayout.Button(content, new GUILayoutOption[0]))
			{
				bool flag = this.HasModified();
				if (flag)
				{
					this.Apply();
				}
				if (upgradeMaterials)
				{
					SpeedTreeImporter[] importers = this.importers;
					for (int i = 0; i < importers.Length; i++)
					{
						SpeedTreeImporter speedTreeImporter = importers[i];
						speedTreeImporter.SetMaterialVersionToCurrent();
					}
				}
				this.GenerateMaterials();
				if (flag || upgradeMaterials)
				{
					base.ApplyAndImport();
					result = true;
				}
			}
			return result;
		}

		private void GenerateMaterials()
		{
			string[] array = (from im in this.importers
			select im.materialFolderPath).ToArray<string>();
			string[] source = AssetDatabase.FindAssets("t:Material", array);
			string[] array2 = (from guid in source
			select AssetDatabase.GUIDToAssetPath(guid)).ToArray<string>();
			bool flag = true;
			if (array2.Length > 0)
			{
				flag = Provider.PromptAndCheckoutIfNeeded(array2, string.Format("Materials will be checked out in:\n{0}", string.Join("\n", array)));
			}
			if (flag)
			{
				SpeedTreeImporter[] importers = this.importers;
				for (int i = 0; i < importers.Length; i++)
				{
					SpeedTreeImporter speedTreeImporter = importers[i];
					speedTreeImporter.GenerateMaterials();
				}
			}
		}

		internal List<LODGroupGUI.LODInfo> GetLODInfoArray(Rect area)
		{
			int lodCount = this.m_LODSettings.arraySize;
			return LODGroupGUI.CreateLODInfos(lodCount, area, (int i) => (i != lodCount - 1 || !(this.target as SpeedTreeImporter).hasBillboard) ? string.Format("LOD {0}", i) : "Billboard", (int i) => this.m_LODSettings.GetArrayElementAtIndex(i).FindPropertyRelative("height").floatValue);
		}

		public bool HasSameLODConfig()
		{
			if (base.serializedObject.FindProperty("m_HasBillboard").hasMultipleDifferentValues)
			{
				return false;
			}
			if (this.m_LODSettings.FindPropertyRelative("Array.size").hasMultipleDifferentValues)
			{
				return false;
			}
			for (int i = 0; i < this.m_LODSettings.arraySize; i++)
			{
				if (this.m_LODSettings.GetArrayElementAtIndex(i).FindPropertyRelative("height").hasMultipleDifferentValues)
				{
					return false;
				}
			}
			return true;
		}

		public bool CanUnifyLODConfig()
		{
			return !base.serializedObject.FindProperty("m_HasBillboard").hasMultipleDifferentValues && !this.m_LODSettings.FindPropertyRelative("Array.size").hasMultipleDifferentValues;
		}

		public override void OnInspectorGUI()
		{
			this.ShowMeshGUI();
			this.ShowMaterialGUI();
			this.ShowLODGUI();
			EditorGUILayout.Space();
			if (this.upgradeMaterials)
			{
				EditorGUILayout.HelpBox(string.Format("SpeedTree materials need to be upgraded. Please back them up (if modified manually) then hit the \"{0}\" button below.", SpeedTreeImporterInspector.Styles.ApplyAndGenerate.text), MessageType.Warning);
			}
			base.ApplyRevertGUI();
		}

		private void ShowMeshGUI()
		{
			GUILayout.Label(SpeedTreeImporterInspector.Styles.MeshesHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ScaleFactor, SpeedTreeImporterInspector.Styles.ScaleFactor, new GUILayoutOption[0]);
			if (!this.m_ScaleFactor.hasMultipleDifferentValues && Mathf.Approximately(this.m_ScaleFactor.floatValue, 0.3048f))
			{
				EditorGUILayout.HelpBox(SpeedTreeImporterInspector.Styles.ScaleFactorHelp.text, MessageType.Info);
			}
		}

		private void ShowMaterialGUI()
		{
			GUILayout.Label(SpeedTreeImporterInspector.Styles.MaterialsHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MainColor, SpeedTreeImporterInspector.Styles.MainColor, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_HueVariation, SpeedTreeImporterInspector.Styles.HueVariation, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_AlphaTestRef, 0f, 1f, SpeedTreeImporterInspector.Styles.AlphaTestRef, new GUILayoutOption[0]);
		}

		private void ShowLODGUI()
		{
			this.m_ShowSmoothLODOptions.target = (this.m_EnableSmoothLOD.hasMultipleDifferentValues || this.m_EnableSmoothLOD.boolValue);
			this.m_ShowCrossFadeWidthOptions.target = (this.m_AnimateCrossFading.hasMultipleDifferentValues || !this.m_AnimateCrossFading.boolValue);
			GUILayout.Label(SpeedTreeImporterInspector.Styles.LODHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_EnableSmoothLOD, SpeedTreeImporterInspector.Styles.SmoothLOD, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowSmoothLODOptions.faded))
			{
				EditorGUILayout.PropertyField(this.m_AnimateCrossFading, SpeedTreeImporterInspector.Styles.AnimateCrossFading, new GUILayoutOption[0]);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowCrossFadeWidthOptions.faded))
				{
					EditorGUILayout.Slider(this.m_BillboardTransitionCrossFadeWidth, 0f, 1f, SpeedTreeImporterInspector.Styles.CrossFadeWidth, new GUILayoutOption[0]);
					EditorGUILayout.Slider(this.m_FadeOutWidth, 0f, 1f, SpeedTreeImporterInspector.Styles.FadeOutWidth, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
			if (this.HasSameLODConfig())
			{
				EditorGUILayout.Space();
				Rect rect = GUILayoutUtility.GetRect(0f, 30f, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				List<LODGroupGUI.LODInfo> lODInfoArray = this.GetLODInfoArray(rect);
				this.DrawLODLevelSlider(rect, lODInfoArray);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				if (this.m_SelectedLODRange != -1 && lODInfoArray.Count > 0)
				{
					EditorGUILayout.LabelField(lODInfoArray[this.m_SelectedLODRange].LODName + " Options", EditorStyles.boldLabel, new GUILayoutOption[0]);
					bool flag = this.m_SelectedLODRange == lODInfoArray.Count - 1 && this.importers[0].hasBillboard;
					EditorGUILayout.PropertyField(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("castShadows"), SpeedTreeImporterInspector.Styles.CastShadows, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("receiveShadows"), SpeedTreeImporterInspector.Styles.ReceiveShadows, new GUILayoutOption[0]);
					SerializedProperty serializedProperty = this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("useLightProbes");
					EditorGUILayout.PropertyField(serializedProperty, SpeedTreeImporterInspector.Styles.UseLightProbes, new GUILayoutOption[0]);
					if (!serializedProperty.hasMultipleDifferentValues && serializedProperty.boolValue && flag)
					{
						EditorGUILayout.HelpBox("Enabling Light Probe for billboards breaks batched rendering and may cause performance problem.", MessageType.Warning);
					}
					EditorGUILayout.PropertyField(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("enableBump"), SpeedTreeImporterInspector.Styles.EnableBump, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("enableHue"), SpeedTreeImporterInspector.Styles.EnableHue, new GUILayoutOption[0]);
					int num = this.importers.Min((SpeedTreeImporter im) => im.bestWindQuality);
					if (num > 0)
					{
						if (flag)
						{
							num = ((num < 1) ? 0 : 1);
						}
						EditorGUILayout.Popup(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("windQuality"), (from s in SpeedTreeImporter.windQualityNames.Take(num + 1)
						select new GUIContent(s)).ToArray<GUIContent>(), SpeedTreeImporterInspector.Styles.WindQuality, new GUILayoutOption[0]);
					}
				}
			}
			else
			{
				if (this.CanUnifyLODConfig())
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					Rect rect2 = GUILayoutUtility.GetRect(SpeedTreeImporterInspector.Styles.ResetLOD, EditorStyles.miniButton);
					if (GUI.Button(rect2, SpeedTreeImporterInspector.Styles.ResetLOD, EditorStyles.miniButton))
					{
						GenericMenu genericMenu = new GenericMenu();
						foreach (SpeedTreeImporter current in base.targets.Cast<SpeedTreeImporter>())
						{
							string text = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(current.assetPath), string.Join(" | ", (from height in current.LODHeights
							select string.Format("{0:0}%", height * 100f)).ToArray<string>()));
							genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.OnResetLODMenuClick), current);
						}
						genericMenu.DropDown(rect2);
					}
					EditorGUILayout.EndHorizontal();
				}
				Rect rect3 = GUILayoutUtility.GetRect(0f, 30f, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				if (Event.current.type == EventType.Repaint)
				{
					LODGroupGUI.DrawMixedValueLODSlider(rect3);
				}
			}
			EditorGUILayout.Space();
		}

		private void DrawLODLevelSlider(Rect sliderPosition, List<LODGroupGUI.LODInfo> lods)
		{
			int controlID = GUIUtility.GetControlID(this.m_LODSliderId, FocusType.Passive);
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
			{
				Rect rect = sliderPosition;
				rect.x -= 5f;
				rect.width += 10f;
				if (rect.Contains(current.mousePosition))
				{
					current.Use();
					GUIUtility.hotControl = controlID;
					IOrderedEnumerable<LODGroupGUI.LODInfo> collection = from lod in lods
					where lod.ScreenPercent > 0.5f
					select lod into x
					orderby x.LODLevel descending
					select x;
					IOrderedEnumerable<LODGroupGUI.LODInfo> collection2 = from lod in lods
					where lod.ScreenPercent <= 0.5f
					select lod into x
					orderby x.LODLevel
					select x;
					List<LODGroupGUI.LODInfo> list = new List<LODGroupGUI.LODInfo>();
					list.AddRange(collection);
					list.AddRange(collection2);
					foreach (LODGroupGUI.LODInfo current2 in list)
					{
						if (current2.m_ButtonPosition.Contains(current.mousePosition))
						{
							this.m_SelectedLODSlider = current2.LODLevel;
							this.m_SelectedLODRange = current2.LODLevel;
							break;
						}
						if (current2.m_RangePosition.Contains(current.mousePosition))
						{
							this.m_SelectedLODSlider = -1;
							this.m_SelectedLODRange = current2.LODLevel;
							break;
						}
					}
				}
				break;
			}
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID && this.m_SelectedLODSlider >= 0 && lods[this.m_SelectedLODSlider] != null)
				{
					current.Use();
					float num = Mathf.Clamp01(1f - (current.mousePosition.x - sliderPosition.x) / sliderPosition.width);
					LODGroupGUI.SetSelectedLODLevelPercentage(num - 0.001f, this.m_SelectedLODSlider, lods);
					this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODSlider).FindPropertyRelative("height").floatValue = lods[this.m_SelectedLODSlider].RawScreenPercent;
				}
				break;
			case EventType.Repaint:
				LODGroupGUI.DrawLODSlider(sliderPosition, lods, this.m_SelectedLODRange);
				break;
			}
		}

		private void OnResetLODMenuClick(object userData)
		{
			float[] lODHeights = (userData as SpeedTreeImporter).LODHeights;
			for (int i = 0; i < lODHeights.Length; i++)
			{
				this.m_LODSettings.GetArrayElementAtIndex(i).FindPropertyRelative("height").floatValue = lODHeights[i];
			}
		}
	}
}
