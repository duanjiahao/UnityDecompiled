using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Build;
using UnityEditor.U2D.Common;
using UnityEditor.U2D.Interface;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;

namespace UnityEditor.U2D
{
	[CanEditMultipleObjects, CustomEditor(typeof(SpriteAtlas))]
	internal class SpriteAtlasInspector : Editor
	{
		private class SpriteAtlasInspectorPlatformSettingView : TexturePlatformSettingsView
		{
			private bool m_ShowMaxSizeOption;

			public SpriteAtlasInspectorPlatformSettingView(bool showMaxSizeOption)
			{
				this.m_ShowMaxSizeOption = showMaxSizeOption;
			}

			public override int DrawMaxSize(int defaultValue, bool isMixedValue, out bool changed)
			{
				int result;
				if (this.m_ShowMaxSizeOption)
				{
					result = base.DrawMaxSize(defaultValue, isMixedValue, out changed);
				}
				else
				{
					changed = false;
					result = defaultValue;
				}
				return result;
			}
		}

		private class Styles
		{
			public readonly GUIStyle dropzoneStyle = new GUIStyle("BoldLabel");

			public readonly GUIStyle preDropDown = "preDropDown";

			public readonly GUIStyle previewButton = "preButton";

			public readonly GUIStyle previewSlider = "preSlider";

			public readonly GUIStyle previewSliderThumb = "preSliderThumb";

			public readonly GUIStyle previewLabel = new GUIStyle("preLabel");

			public readonly GUIContent textureSettingLabel = EditorGUIUtility.TextContent("Texture");

			public readonly GUIContent variantSettingLabel = EditorGUIUtility.TextContent("Variant");

			public readonly GUIContent packingParametersLabel = EditorGUIUtility.TextContent("Packing");

			public readonly GUIContent atlasTypeLabel = EditorGUIUtility.TextContent("Type");

			public readonly GUIContent defaultPlatformLabel = EditorGUIUtility.TextContent("Default");

			public readonly GUIContent masterAtlasLabel = EditorGUIUtility.TextContent("Master Atlas|Assigning another Sprite Atlas asset will make this atlas a variant of it.");

			public readonly GUIContent bindAsDefaultLabel = EditorGUIUtility.TextContent("Include in Build|Packed textures will be included in the build by default.");

			public readonly GUIContent enableRotationLabel = EditorGUIUtility.TextContent("Allow Rotation|Try rotating the sprite to fit better during packing.");

			public readonly GUIContent enableTightPackingLabel = EditorGUIUtility.TextContent("Tight Packing|Use the mesh outline to fit instead of the whole texture rect during packing.");

			public readonly GUIContent generateMipMapLabel = EditorGUIUtility.TextContent("Generate Mip Maps");

			public readonly GUIContent readWrite = EditorGUIUtility.TextContent("Read/Write Enabled|Enable to be able to access the raw pixel data from code.");

			public readonly GUIContent variantMultiplierLabel = EditorGUIUtility.TextContent("Scale|Down scale ratio.");

			public readonly GUIContent copyMasterButton = EditorGUIUtility.TextContent("Copy Master's Settings|Copy all master's settings into this variant.");

			public readonly GUIContent packButton = EditorGUIUtility.TextContent("Pack Preview|Pack this atlas.");

			public readonly GUIContent disabledPackLabel = EditorGUIUtility.TextContent("Sprite Atlas packing is disabled. Enable it in Edit > Project Settings > Editor.");

			public readonly GUIContent packableListLabel = EditorGUIUtility.TextContent("Objects for Packing|Only accept Folder, Sprite Sheet(Texture) and Sprite.");

			public readonly GUIContent smallZoom = EditorGUIUtility.IconContent("PreTextureMipMapLow");

			public readonly GUIContent largeZoom = EditorGUIUtility.IconContent("PreTextureMipMapHigh");

			public readonly GUIContent alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");

			public readonly GUIContent RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");

			public readonly int packableElementHash = "PackableElement".GetHashCode();

			public readonly int packableSelectorHash = "PackableSelector".GetHashCode();

			public readonly int[] atlasTypeValues = new int[]
			{
				0,
				1
			};

			public readonly GUIContent[] atlasTypeOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Master"),
				EditorGUIUtility.TextContent("Variant")
			};

			public Styles()
			{
				this.dropzoneStyle.alignment = TextAnchor.MiddleCenter;
				this.dropzoneStyle.border = new RectOffset(10, 10, 10, 10);
			}
		}

		private enum AtlasType
		{
			Undefined = -1,
			Master,
			Variant
		}

		private static SpriteAtlasInspector.Styles s_Styles;

		private readonly string kDefaultPlatformName = "default";

		private SerializedProperty m_MaxTextureSize;

		private SerializedProperty m_TextureCompression;

		private SerializedProperty m_UseCrunchedCompression;

		private SerializedProperty m_CompressionQuality;

		private SerializedProperty m_FilterMode;

		private SerializedProperty m_AnisoLevel;

		private SerializedProperty m_GenerateMipMaps;

		private SerializedProperty m_Readable;

		private SerializedProperty m_EnableTightPacking;

		private SerializedProperty m_EnableRotation;

		private SerializedProperty m_BindAsDefault;

		private SerializedProperty m_Packables;

		private SerializedProperty m_MasterAtlas;

		private SerializedProperty m_VariantMultiplier;

		private string m_Hash;

		private int m_PreviewPage = 0;

		private int m_TotalPages = 0;

		private int[] m_OptionValues = null;

		private string[] m_OptionDisplays = null;

		private Texture2D[] m_PreviewTextures = null;

		private bool m_PackableListExpanded = true;

		private ReorderableList m_PackableList;

		private float m_MipLevel = 0f;

		private bool m_ShowAlpha;

		private List<BuildPlatform> m_ValidPlatforms;

		private Dictionary<string, List<TextureImporterPlatformSettings>> m_TempPlatformSettings;

		private ITexturePlatformSettingsView m_TexturePlatformSettingsView;

		private ITexturePlatformSettingsFormatHelper m_TexturePlatformSettingTextureHelper;

		private ITexturePlatformSettingsController m_TexturePlatformSettingsController;

		[CompilerGenerated]
		private static EditorGUI.ObjectFieldValidator <>f__mg$cache0;

		private SpriteAtlas spriteAtlas
		{
			get
			{
				return base.target as SpriteAtlas;
			}
		}

		private static bool IsPackable(UnityEngine.Object o)
		{
			return o != null && (o.GetType() == typeof(Sprite) || o.GetType() == typeof(DefaultAsset) || o.GetType() == typeof(Texture2D));
		}

		private static UnityEngine.Object ValidateObjectForPackableFieldAssignment(UnityEngine.Object[] references, Type objType, SerializedProperty property, EditorGUI.ObjectFieldValidatorOptions options)
		{
			UnityEngine.Object result;
			if (references.Length > 0 && SpriteAtlasInspector.IsPackable(references[0]))
			{
				result = references[0];
			}
			else
			{
				result = null;
			}
			return result;
		}

		private bool AllTargetsAreVariant()
		{
			UnityEngine.Object[] targets = base.targets;
			bool result;
			for (int i = 0; i < targets.Length; i++)
			{
				SpriteAtlas spriteAtlas = (SpriteAtlas)targets[i];
				if (!spriteAtlas.isVariant)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}

		private bool AllTargetsAreMaster()
		{
			UnityEngine.Object[] targets = base.targets;
			bool result;
			for (int i = 0; i < targets.Length; i++)
			{
				SpriteAtlas spriteAtlas = (SpriteAtlas)targets[i];
				if (spriteAtlas.isVariant)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}

		private void OnEnable()
		{
			this.m_MaxTextureSize = base.serializedObject.FindProperty("m_EditorData.textureSettings.maxTextureSize");
			this.m_TextureCompression = base.serializedObject.FindProperty("m_EditorData.textureSettings.textureCompression");
			this.m_UseCrunchedCompression = base.serializedObject.FindProperty("m_EditorData.textureSettings.crunchedCompression");
			this.m_CompressionQuality = base.serializedObject.FindProperty("m_EditorData.textureSettings.compressionQuality");
			this.m_FilterMode = base.serializedObject.FindProperty("m_EditorData.textureSettings.filterMode");
			this.m_AnisoLevel = base.serializedObject.FindProperty("m_EditorData.textureSettings.anisoLevel");
			this.m_GenerateMipMaps = base.serializedObject.FindProperty("m_EditorData.textureSettings.generateMipMaps");
			this.m_Readable = base.serializedObject.FindProperty("m_EditorData.textureSettings.readable");
			this.m_EnableTightPacking = base.serializedObject.FindProperty("m_EditorData.packingParameters.enableTightPacking");
			this.m_EnableRotation = base.serializedObject.FindProperty("m_EditorData.packingParameters.enableRotation");
			this.m_Hash = base.serializedObject.FindProperty("m_EditorData.hashString").stringValue;
			this.m_MasterAtlas = base.serializedObject.FindProperty("m_MasterAtlas");
			this.m_BindAsDefault = base.serializedObject.FindProperty("m_EditorData.bindAsDefault");
			this.m_VariantMultiplier = base.serializedObject.FindProperty("m_EditorData.variantMultiplier");
			this.m_Packables = base.serializedObject.FindProperty("m_EditorData.packables");
			this.m_PackableList = new ReorderableList(base.serializedObject, this.m_Packables, true, true, true, true);
			this.m_PackableList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddPackable);
			this.m_PackableList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemovePackable);
			this.m_PackableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawPackableElement);
			this.m_PackableList.elementHeight = EditorGUIUtility.singleLineHeight;
			this.m_PackableList.headerHeight = 0f;
			this.SyncPlatformSettings();
			TextureImporterInspector.InitializeTextureFormatStrings();
			this.m_TexturePlatformSettingsView = new SpriteAtlasInspector.SpriteAtlasInspectorPlatformSettingView(this.AllTargetsAreMaster());
			this.m_TexturePlatformSettingTextureHelper = new TexturePlatformSettingsFormatHelper();
			this.m_TexturePlatformSettingsController = new TexturePlatformSettingsViewController();
		}

		private void SyncPlatformSettings()
		{
			this.m_TempPlatformSettings = new Dictionary<string, List<TextureImporterPlatformSettings>>();
			List<TextureImporterPlatformSettings> list = new List<TextureImporterPlatformSettings>();
			this.m_TempPlatformSettings.Add(this.kDefaultPlatformName, list);
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				UnityEngine.Object obj = targets[i];
				TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings();
				textureImporterPlatformSettings.name = this.kDefaultPlatformName;
				SerializedObject serializedObject = new SerializedObject(obj);
				textureImporterPlatformSettings.maxTextureSize = serializedObject.FindProperty("m_EditorData.textureSettings.maxTextureSize").intValue;
				textureImporterPlatformSettings.textureCompression = (TextureImporterCompression)serializedObject.FindProperty("m_EditorData.textureSettings.textureCompression").enumValueIndex;
				textureImporterPlatformSettings.crunchedCompression = serializedObject.FindProperty("m_EditorData.textureSettings.crunchedCompression").boolValue;
				textureImporterPlatformSettings.compressionQuality = serializedObject.FindProperty("m_EditorData.textureSettings.compressionQuality").intValue;
				list.Add(textureImporterPlatformSettings);
			}
			this.m_ValidPlatforms = BuildPlatforms.instance.GetValidPlatforms();
			foreach (BuildPlatform current in this.m_ValidPlatforms)
			{
				List<TextureImporterPlatformSettings> list2 = new List<TextureImporterPlatformSettings>();
				this.m_TempPlatformSettings.Add(current.name, list2);
				UnityEngine.Object[] targets2 = base.targets;
				for (int j = 0; j < targets2.Length; j++)
				{
					SpriteAtlas spriteAtlas = (SpriteAtlas)targets2[j];
					TextureImporterPlatformSettings textureImporterPlatformSettings2 = new TextureImporterPlatformSettings();
					textureImporterPlatformSettings2.name = current.name;
					spriteAtlas.CopyPlatformSettingsIfAvailable(current.name, textureImporterPlatformSettings2);
					list2.Add(textureImporterPlatformSettings2);
				}
			}
		}

		private void AddPackable(ReorderableList list)
		{
			ObjectSelector.get.Show(null, typeof(UnityEngine.Object), null, false);
			ObjectSelector.get.objectSelectorID = SpriteAtlasInspector.s_Styles.packableSelectorHash;
		}

		private void RemovePackable(ReorderableList list)
		{
			int index = list.index;
			if (index != -1)
			{
				this.spriteAtlas.RemoveAt(index);
			}
		}

		private void DrawPackableElement(Rect rect, int index, bool selected, bool focused)
		{
			SerializedProperty arrayElementAtIndex = this.m_Packables.GetArrayElementAtIndex(index);
			int controlID = GUIUtility.GetControlID(SpriteAtlasInspector.s_Styles.packableElementHash, FocusType.Passive);
			UnityEngine.Object objectReferenceValue = arrayElementAtIndex.objectReferenceValue;
			EditorGUI.BeginChangeCheck();
			int arg_58_2 = controlID;
			UnityEngine.Object arg_58_3 = objectReferenceValue;
			Type arg_58_4 = typeof(UnityEngine.Object);
			SerializedProperty arg_58_5 = null;
			if (SpriteAtlasInspector.<>f__mg$cache0 == null)
			{
				SpriteAtlasInspector.<>f__mg$cache0 = new EditorGUI.ObjectFieldValidator(SpriteAtlasInspector.ValidateObjectForPackableFieldAssignment);
			}
			UnityEngine.Object objectReferenceValue2 = EditorGUI.DoObjectField(rect, rect, arg_58_2, arg_58_3, arg_58_4, arg_58_5, SpriteAtlasInspector.<>f__mg$cache0, false);
			if (EditorGUI.EndChangeCheck())
			{
				if (objectReferenceValue != null)
				{
					this.spriteAtlas.Remove(new UnityEngine.Object[]
					{
						objectReferenceValue
					});
				}
				arrayElementAtIndex.objectReferenceValue = objectReferenceValue2;
			}
			if (GUIUtility.keyboardControl == controlID && !selected)
			{
				this.m_PackableList.index = index;
			}
		}

		public override void OnInspectorGUI()
		{
			SpriteAtlasInspector.s_Styles = (SpriteAtlasInspector.s_Styles ?? new SpriteAtlasInspector.Styles());
			base.serializedObject.Update();
			this.HandleCommonSettingUI();
			GUILayout.Space(5f);
			if (this.AllTargetsAreVariant())
			{
				this.HandleVariantSettingUI();
			}
			else if (this.AllTargetsAreMaster())
			{
				this.HandleMasterSettingUI();
			}
			GUILayout.Space(5f);
			this.HandleTextureSettingUI();
			GUILayout.Space(5f);
			if (base.targets.Length == 1 && this.AllTargetsAreMaster())
			{
				this.HandlePackableListUI();
			}
			bool flag = EditorSettings.spritePackerMode == SpritePackerMode.BuildTimeOnlyAtlas || EditorSettings.spritePackerMode == SpritePackerMode.AlwaysOnAtlas;
			if (flag)
			{
				if (GUILayout.Button(SpriteAtlasInspector.s_Styles.packButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					SpriteAtlas[] array = new SpriteAtlas[base.targets.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = (SpriteAtlas)base.targets[i];
					}
					SpriteAtlasUtility.PackAtlases(array, EditorUserBuildSettings.activeBuildTarget);
					this.SyncPlatformSettings();
				}
			}
			else
			{
				EditorGUILayout.HelpBox(SpriteAtlasInspector.s_Styles.disabledPackLabel.text, MessageType.Info);
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		private void HandleCommonSettingUI()
		{
			SpriteAtlasInspector.AtlasType atlasType = SpriteAtlasInspector.AtlasType.Undefined;
			if (this.AllTargetsAreMaster())
			{
				atlasType = SpriteAtlasInspector.AtlasType.Master;
			}
			else if (this.AllTargetsAreVariant())
			{
				atlasType = SpriteAtlasInspector.AtlasType.Variant;
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = (atlasType == SpriteAtlasInspector.AtlasType.Undefined);
			atlasType = (SpriteAtlasInspector.AtlasType)EditorGUILayout.IntPopup(SpriteAtlasInspector.s_Styles.atlasTypeLabel, (int)atlasType, SpriteAtlasInspector.s_Styles.atlasTypeOptions, SpriteAtlasInspector.s_Styles.atlasTypeValues, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				bool value = atlasType == SpriteAtlasInspector.AtlasType.Variant;
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					SpriteAtlas spriteAtlas = (SpriteAtlas)targets[i];
					spriteAtlas.SetIsVariant(value);
				}
				this.m_TexturePlatformSettingsView = new SpriteAtlasInspector.SpriteAtlasInspectorPlatformSettingView(this.AllTargetsAreMaster());
			}
			if (atlasType == SpriteAtlasInspector.AtlasType.Variant)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_MasterAtlas, SpriteAtlasInspector.s_Styles.masterAtlasLabel, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					base.serializedObject.ApplyModifiedProperties();
					UnityEngine.Object[] targets2 = base.targets;
					for (int j = 0; j < targets2.Length; j++)
					{
						SpriteAtlas spriteAtlas2 = (SpriteAtlas)targets2[j];
						spriteAtlas2.CopyMasterAtlasSettings();
						this.SyncPlatformSettings();
					}
				}
			}
			EditorGUILayout.PropertyField(this.m_BindAsDefault, SpriteAtlasInspector.s_Styles.bindAsDefaultLabel, new GUILayoutOption[0]);
		}

		private void HandleVariantSettingUI()
		{
			EditorGUILayout.LabelField(SpriteAtlasInspector.s_Styles.variantSettingLabel, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_VariantMultiplier, SpriteAtlasInspector.s_Styles.variantMultiplierLabel, new GUILayoutOption[0]);
		}

		private void HandleBoolToIntPropertyField(SerializedProperty prop, GUIContent content)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, content, prop);
			EditorGUI.BeginChangeCheck();
			bool boolValue = EditorGUI.Toggle(controlRect, content, prop.boolValue);
			if (EditorGUI.EndChangeCheck())
			{
				prop.boolValue = boolValue;
			}
			EditorGUI.EndProperty();
		}

		private void HandleMasterSettingUI()
		{
			EditorGUILayout.LabelField(SpriteAtlasInspector.s_Styles.packingParametersLabel, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.HandleBoolToIntPropertyField(this.m_EnableRotation, SpriteAtlasInspector.s_Styles.enableRotationLabel);
			this.HandleBoolToIntPropertyField(this.m_EnableTightPacking, SpriteAtlasInspector.s_Styles.enableTightPackingLabel);
			GUILayout.Space(5f);
		}

		private void HandleTextureSettingUI()
		{
			EditorGUILayout.LabelField(SpriteAtlasInspector.s_Styles.textureSettingLabel, EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.HandleBoolToIntPropertyField(this.m_Readable, SpriteAtlasInspector.s_Styles.readWrite);
			this.HandleBoolToIntPropertyField(this.m_GenerateMipMaps, SpriteAtlasInspector.s_Styles.generateMipMapLabel);
			EditorGUILayout.PropertyField(this.m_FilterMode, new GUILayoutOption[0]);
			bool flag = !this.m_FilterMode.hasMultipleDifferentValues && !this.m_GenerateMipMaps.hasMultipleDifferentValues && this.m_FilterMode.intValue != 0 && this.m_GenerateMipMaps.boolValue;
			if (flag)
			{
				EditorGUILayout.IntSlider(this.m_AnisoLevel, 0, 16, new GUILayoutOption[0]);
			}
			GUILayout.Space(5f);
			this.HandlePlatformSettingUI();
		}

		private void HandlePlatformSettingUI()
		{
			int num = EditorGUILayout.BeginPlatformGrouping(this.m_ValidPlatforms.ToArray(), SpriteAtlasInspector.s_Styles.defaultPlatformLabel);
			if (num == -1)
			{
				List<TextureImporterPlatformSettings> list = this.m_TempPlatformSettings[this.kDefaultPlatformName];
				List<TextureImporterPlatformSettings> list2 = new List<TextureImporterPlatformSettings>(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings();
					list[i].CopyTo(textureImporterPlatformSettings);
					list2.Add(textureImporterPlatformSettings);
				}
				if (this.m_TexturePlatformSettingsController.HandleDefaultSettings(list2, this.m_TexturePlatformSettingsView))
				{
					for (int j = 0; j < list2.Count; j++)
					{
						if (list[j].maxTextureSize != list2[j].maxTextureSize)
						{
							this.m_MaxTextureSize.intValue = list2[j].maxTextureSize;
						}
						if (list[j].textureCompression != list2[j].textureCompression)
						{
							this.m_TextureCompression.enumValueIndex = (int)list2[j].textureCompression;
						}
						if (list[j].crunchedCompression != list2[j].crunchedCompression)
						{
							this.m_UseCrunchedCompression.boolValue = list2[j].crunchedCompression;
						}
						if (list[j].compressionQuality != list2[j].compressionQuality)
						{
							this.m_CompressionQuality.intValue = list2[j].compressionQuality;
						}
						list2[j].CopyTo(list[j]);
					}
				}
			}
			else
			{
				BuildPlatform buildPlatform = this.m_ValidPlatforms[num];
				List<TextureImporterPlatformSettings> list3 = this.m_TempPlatformSettings[buildPlatform.name];
				for (int k = 0; k < list3.Count; k++)
				{
					TextureImporterPlatformSettings textureImporterPlatformSettings2 = list3[k];
					if (!textureImporterPlatformSettings2.overridden)
					{
						SpriteAtlas spriteAtlas = (SpriteAtlas)base.targets[k];
						textureImporterPlatformSettings2.format = spriteAtlas.FormatDetermineByAtlasSettings(buildPlatform.defaultTarget);
					}
				}
				this.m_TexturePlatformSettingsView.buildPlatformTitle = buildPlatform.title.text;
				if (this.m_TexturePlatformSettingsController.HandlePlatformSettings(buildPlatform.defaultTarget, list3, this.m_TexturePlatformSettingsView, this.m_TexturePlatformSettingTextureHelper))
				{
					for (int l = 0; l < list3.Count; l++)
					{
						SpriteAtlas spriteAtlas2 = (SpriteAtlas)base.targets[l];
						spriteAtlas2.SetPlatformSettings(list3[l]);
					}
				}
			}
			EditorGUILayout.EndPlatformGrouping();
		}

		private void HandlePackableListUI()
		{
			Event current = Event.current;
			bool flag = false;
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			int s_LastControlID = EditorGUIUtility.s_LastControlID;
			switch (current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (controlRect.Contains(current.mousePosition) && GUI.enabled)
				{
					bool flag2 = false;
					UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
					UnityEngine.Object[] array = objectReferences;
					for (int i = 0; i < array.Length; i++)
					{
						UnityEngine.Object @object = array[i];
						if (SpriteAtlasInspector.IsPackable(@object))
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
							if (current.type == EventType.DragPerform)
							{
								this.m_Packables.AppendFoldoutPPtrValue(@object);
								flag2 = true;
								DragAndDrop.activeControlID = 0;
							}
							else
							{
								DragAndDrop.activeControlID = s_LastControlID;
							}
						}
					}
					if (flag2)
					{
						GUI.changed = true;
						DragAndDrop.AcceptDrag();
						flag = true;
					}
				}
				break;
			case EventType.ValidateCommand:
				if (current.commandName == "ObjectSelectorClosed" && ObjectSelector.get.objectSelectorID == SpriteAtlasInspector.s_Styles.packableSelectorHash)
				{
					flag = true;
				}
				break;
			case EventType.ExecuteCommand:
				if (current.commandName == "ObjectSelectorClosed" && ObjectSelector.get.objectSelectorID == SpriteAtlasInspector.s_Styles.packableSelectorHash)
				{
					UnityEngine.Object currentObject = ObjectSelector.GetCurrentObject();
					if (SpriteAtlasInspector.IsPackable(currentObject))
					{
						this.m_Packables.AppendFoldoutPPtrValue(currentObject);
						this.m_PackableList.index = this.m_Packables.arraySize - 1;
					}
					flag = true;
				}
				break;
			case EventType.DragExited:
				if (GUI.enabled)
				{
					HandleUtility.Repaint();
				}
				break;
			}
			this.m_PackableListExpanded = EditorGUI.Foldout(controlRect, this.m_PackableListExpanded, SpriteAtlasInspector.s_Styles.packableListLabel, true);
			if (flag)
			{
				current.Use();
			}
			if (this.m_PackableListExpanded)
			{
				EditorGUI.indentLevel++;
				this.m_PackableList.DoLayoutList();
				EditorGUI.indentLevel--;
			}
		}

		private void CachePreviewTexture()
		{
			if (this.m_PreviewTextures == null || this.m_Hash != this.spriteAtlas.GetHashString())
			{
				this.m_PreviewTextures = this.spriteAtlas.GetPreviewTextures();
				this.m_Hash = this.spriteAtlas.GetHashString();
				if (this.m_PreviewTextures != null && this.m_PreviewTextures.Length > 0 && this.m_TotalPages != this.m_PreviewTextures.Length)
				{
					this.m_TotalPages = this.m_PreviewTextures.Length;
					this.m_OptionDisplays = new string[this.m_TotalPages];
					this.m_OptionValues = new int[this.m_TotalPages];
					for (int i = 0; i < this.m_TotalPages; i++)
					{
						this.m_OptionDisplays[i] = string.Format("# {0}", i + 1);
						this.m_OptionValues[i] = i;
					}
				}
			}
		}

		public override string GetInfoString()
		{
			string result;
			if (this.m_PreviewTextures != null && this.m_PreviewPage < this.m_PreviewTextures.Length)
			{
				Texture2D texture2D = this.m_PreviewTextures[this.m_PreviewPage];
				TextureFormat textureFormat = TextureUtil.GetTextureFormat(texture2D);
				result = string.Format("{0}x{1} {2}\n{3}", new object[]
				{
					texture2D.width,
					texture2D.height,
					TextureUtil.GetTextureFormatString(textureFormat),
					EditorUtility.FormatBytes(TextureUtil.GetStorageMemorySizeLong(texture2D))
				});
			}
			else
			{
				result = "";
			}
			return result;
		}

		public override bool HasPreviewGUI()
		{
			this.CachePreviewTexture();
			return this.m_PreviewTextures != null && this.m_PreviewTextures.Length > 0;
		}

		public override void OnPreviewSettings()
		{
			if (base.targets.Length == 1 && this.m_OptionDisplays != null && this.m_OptionValues != null && this.m_TotalPages > 1)
			{
				this.m_PreviewPage = EditorGUILayout.IntPopup(this.m_PreviewPage, this.m_OptionDisplays, this.m_OptionValues, SpriteAtlasInspector.s_Styles.preDropDown, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(50f)
				});
			}
			else
			{
				this.m_PreviewPage = 0;
			}
			if (this.m_PreviewTextures != null)
			{
				Texture2D texture2D = this.m_PreviewTextures[this.m_PreviewPage];
				if (TextureUtil.HasAlphaTextureFormat(texture2D.format))
				{
					this.m_ShowAlpha = GUILayout.Toggle(this.m_ShowAlpha, (!this.m_ShowAlpha) ? SpriteAtlasInspector.s_Styles.RGBIcon : SpriteAtlasInspector.s_Styles.alphaIcon, SpriteAtlasInspector.s_Styles.previewButton, new GUILayoutOption[0]);
				}
				int num = Mathf.Max(1, TextureUtil.GetMipmapCount(texture2D));
				if (num > 1)
				{
					GUILayout.Box(SpriteAtlasInspector.s_Styles.smallZoom, SpriteAtlasInspector.s_Styles.previewLabel, new GUILayoutOption[0]);
					this.m_MipLevel = Mathf.Round(GUILayout.HorizontalSlider(this.m_MipLevel, (float)(num - 1), 0f, SpriteAtlasInspector.s_Styles.previewSlider, SpriteAtlasInspector.s_Styles.previewSliderThumb, new GUILayoutOption[]
					{
						GUILayout.MaxWidth(64f)
					}));
					GUILayout.Box(SpriteAtlasInspector.s_Styles.largeZoom, SpriteAtlasInspector.s_Styles.previewLabel, new GUILayoutOption[0]);
				}
			}
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			this.CachePreviewTexture();
			if (this.m_PreviewTextures != null && this.m_PreviewPage < this.m_PreviewTextures.Length)
			{
				Texture2D texture2D = this.m_PreviewTextures[this.m_PreviewPage];
				float mipMapBias = texture2D.mipMapBias;
				float bias = this.m_MipLevel - (float)(Math.Log((double)((float)texture2D.width / r.width)) / Math.Log(2.0));
				TextureUtil.SetMipMapBiasNoDirty(texture2D, bias);
				if (this.m_ShowAlpha)
				{
					EditorGUI.DrawTextureAlpha(r, texture2D, ScaleMode.ScaleToFit);
				}
				else
				{
					EditorGUI.DrawTextureTransparent(r, texture2D, ScaleMode.ScaleToFit);
				}
				TextureUtil.SetMipMapBiasNoDirty(texture2D, mipMapBias);
			}
		}

		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			SpriteAtlas spriteAtlas = AssetDatabase.LoadMainAssetAtPath(assetPath) as SpriteAtlas;
			Texture2D result;
			if (spriteAtlas == null)
			{
				result = null;
			}
			else
			{
				Texture2D[] previewTextures = spriteAtlas.GetPreviewTextures();
				if (previewTextures == null || previewTextures.Length == 0)
				{
					result = null;
				}
				else
				{
					Texture2D texture2D = previewTextures[0];
					PreviewHelpers.AdjustWidthAndHeightForStaticPreview(texture2D.width, texture2D.height, ref width, ref height);
					result = SpriteUtility.CreateTemporaryDuplicate(texture2D, width, height);
				}
			}
			return result;
		}
	}
}
