using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class ModelImporterClipEditor : AssetImporterInspector
	{
		private class Styles
		{
			public GUIContent ImportAnimations = EditorGUIUtility.TextContent("ModelImporterImportAnimations");
			public GUIStyle numberStyle = new GUIStyle(EditorStyles.label);
			public GUIContent AnimWrapModeLabel = EditorGUIUtility.TextContent("ModelImporterAnimWrapMode");
			public GUIContent[] AnimWrapModeOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ModelImporterAnimWrapModeDefault"),
				EditorGUIUtility.TextContent("ModelImporterAnimWrapModeOnce"),
				EditorGUIUtility.TextContent("ModelImporterAnimWrapModeLoop"),
				EditorGUIUtility.TextContent("ModelImporterAnimWrapModePingPong"),
				EditorGUIUtility.TextContent("ModelImporterAnimWrapModeClampForever")
			};
			public GUIContent BakeIK = EditorGUIUtility.TextContent("ModelImporterAnimBakeIK");
			public GUIContent AnimCompressionLabel = EditorGUIUtility.TextContent("ModelImporterAnimComprSetting");
			public GUIContent[] AnimCompressionOptLegacy = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ModelImporterAnimComprSettingOff"),
				EditorGUIUtility.TextContent("ModelImporterAnimComprSettingReduction"),
				EditorGUIUtility.TextContent("ModelImporterAnimComprSettingReductionAndCompression")
			};
			public GUIContent[] AnimCompressionOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("ModelImporterAnimComprSettingOff"),
				EditorGUIUtility.TextContent("ModelImporterAnimComprSettingReduction"),
				EditorGUIUtility.TextContent("ModelImporterAnimComprSettingOptimal")
			};
			public GUIContent AnimRotationErrorLabel = EditorGUIUtility.TextContent("ModelImporterAnimComprRotationError");
			public GUIContent AnimPositionErrorLabel = EditorGUIUtility.TextContent("ModelImporterAnimComprPositionError");
			public GUIContent AnimScaleErrorLabel = EditorGUIUtility.TextContent("ModelImporterAnimComprScaleError");
			public GUIContent AnimationCompressionHelp = EditorGUIUtility.TextContent("ModelImporterAnimComprHelp");
			public GUIContent clipMultiEditInfo = new GUIContent("Multi-object editing of clips not supported.");
			public GUIContent updateMuscleDefinitionFromSource = EditorGUIUtility.TextContent("ModelImporterRigUpdateMuscleDefinitionFromSource");
			public GUIContent MotionSetting = EditorGUIUtility.TextContent("ModelImporterMotionSetting");
			public GUIContent MotionNode = EditorGUIUtility.TextContent("ModelImporterMotionNode");
			public Styles()
			{
				this.numberStyle.alignment = TextAnchor.UpperRight;
			}
		}
		private const int kFrameColumnWidth = 45;
		private AnimationClipEditor m_AnimationClipEditor;
		public int m_SelectedClipIndexDoNotUseDirectly = -1;
		private SerializedObject m_DefaultClipsSerializedObject;
		private SerializedProperty m_AnimationType;
		private SerializedProperty m_ImportAnimation;
		private SerializedProperty m_ClipAnimations;
		private SerializedProperty m_BakeSimulation;
		private SerializedProperty m_AnimationCompression;
		private SerializedProperty m_AnimationRotationError;
		private SerializedProperty m_AnimationPositionError;
		private SerializedProperty m_AnimationScaleError;
		private SerializedProperty m_AnimationWrapMode;
		private SerializedProperty m_LegacyGenerateAnimations;
		private SerializedProperty m_MotionNodeName;
		private GUIContent[] m_MotionNodeList;
		private static bool motionNodeFoldout;
		private ReorderableList m_ClipList;
		private static ModelImporterClipEditor.Styles styles;
		private ModelImporter singleImporter
		{
			get
			{
				return base.targets[0] as ModelImporter;
			}
		}
		public int selectedClipIndex
		{
			get
			{
				return this.m_SelectedClipIndexDoNotUseDirectly;
			}
			set
			{
				this.m_SelectedClipIndexDoNotUseDirectly = value;
				if (this.m_ClipList != null)
				{
					this.m_ClipList.index = value;
				}
			}
		}
		public int motionNodeIndex
		{
			get;
			set;
		}
		private ModelImporterAnimationType animationType
		{
			get
			{
				return (ModelImporterAnimationType)this.m_AnimationType.intValue;
			}
			set
			{
				this.m_AnimationType.intValue = (int)value;
			}
		}
		private ModelImporterGenerateAnimations legacyGenerateAnimations
		{
			get
			{
				return (ModelImporterGenerateAnimations)this.m_LegacyGenerateAnimations.intValue;
			}
			set
			{
				this.m_LegacyGenerateAnimations.intValue = (int)value;
			}
		}
		public void OnEnable()
		{
			this.m_ClipAnimations = base.serializedObject.FindProperty("m_ClipAnimations");
			this.m_AnimationType = base.serializedObject.FindProperty("m_AnimationType");
			this.m_LegacyGenerateAnimations = base.serializedObject.FindProperty("m_LegacyGenerateAnimations");
			this.m_ImportAnimation = base.serializedObject.FindProperty("m_ImportAnimation");
			this.m_BakeSimulation = base.serializedObject.FindProperty("m_BakeSimulation");
			this.m_AnimationCompression = base.serializedObject.FindProperty("m_AnimationCompression");
			this.m_AnimationRotationError = base.serializedObject.FindProperty("m_AnimationRotationError");
			this.m_AnimationPositionError = base.serializedObject.FindProperty("m_AnimationPositionError");
			this.m_AnimationScaleError = base.serializedObject.FindProperty("m_AnimationScaleError");
			this.m_AnimationWrapMode = base.serializedObject.FindProperty("m_AnimationWrapMode");
			if (this.m_ClipAnimations.arraySize == 0)
			{
				this.SetupDefaultClips();
			}
			this.selectedClipIndex = EditorPrefs.GetInt("ModelImporterClipEditor.ActiveClipIndex", 0);
			this.ValidateClipSelectionIndex();
			EditorPrefs.SetInt("ModelImporterClipEditor.ActiveClipIndex", this.selectedClipIndex);
			if (this.m_AnimationClipEditor != null && this.selectedClipIndex >= 0)
			{
				this.SyncClipEditor();
			}
			if (this.m_ClipAnimations.arraySize != 0)
			{
				this.SelectClip(this.selectedClipIndex);
			}
			string[] transformPaths = this.singleImporter.transformPaths;
			this.m_MotionNodeList = new GUIContent[transformPaths.Length + 1];
			this.m_MotionNodeList[0] = new GUIContent("<None>");
			for (int i = 0; i < transformPaths.Length; i++)
			{
				if (i == 0)
				{
					this.m_MotionNodeList[1] = new GUIContent("<Root Transform>");
				}
				else
				{
					this.m_MotionNodeList[i + 1] = new GUIContent(transformPaths[i]);
				}
			}
			this.m_MotionNodeName = base.serializedObject.FindProperty("m_MotionNodeName");
			this.motionNodeIndex = ArrayUtility.FindIndex<GUIContent>(this.m_MotionNodeList, (GUIContent content) => content.text == this.m_MotionNodeName.stringValue);
			this.motionNodeIndex = ((this.motionNodeIndex >= 1) ? this.motionNodeIndex : 0);
		}
		private void SyncClipEditor()
		{
			if (this.m_AnimationClipEditor == null)
			{
				return;
			}
			this.m_AnimationClipEditor.ShowRange(this.GetAnimationClipInfoAtIndex(this.selectedClipIndex));
			this.m_AnimationClipEditor.referenceTransformPaths = this.singleImporter.transformPaths;
		}
		private void SetupDefaultClips()
		{
			this.m_DefaultClipsSerializedObject = new SerializedObject(this.target);
			this.m_ClipAnimations = this.m_DefaultClipsSerializedObject.FindProperty("m_ClipAnimations");
			this.m_AnimationType = this.m_DefaultClipsSerializedObject.FindProperty("m_AnimationType");
			this.m_ClipAnimations.arraySize = 0;
			TakeInfo[] importedTakeInfos = this.singleImporter.importedTakeInfos;
			for (int i = 0; i < importedTakeInfos.Length; i++)
			{
				TakeInfo takeInfo = importedTakeInfos[i];
				this.AddClip(takeInfo);
			}
		}
		private void PatchDefaultClipTakeNamesToSplitClipNames()
		{
			TakeInfo[] importedTakeInfos = this.singleImporter.importedTakeInfos;
			for (int i = 0; i < importedTakeInfos.Length; i++)
			{
				TakeInfo takeInfo = importedTakeInfos[i];
				PatchImportSettingRecycleID.Patch(base.serializedObject, 74, takeInfo.name, takeInfo.defaultClipName);
			}
		}
		private void TransferDefaultClipsToCustomClips()
		{
			if (this.m_DefaultClipsSerializedObject == null)
			{
				return;
			}
			if (base.serializedObject.FindProperty("m_ClipAnimations").arraySize != 0)
			{
				Debug.LogError("Transferring default clips failed, target already has clips");
			}
			base.serializedObject.CopyFromSerializedProperty(this.m_ClipAnimations);
			this.m_ClipAnimations = base.serializedObject.FindProperty("m_ClipAnimations");
			this.m_DefaultClipsSerializedObject = null;
			this.PatchDefaultClipTakeNamesToSplitClipNames();
			this.SyncClipEditor();
		}
		private void ValidateClipSelectionIndex()
		{
			if (this.selectedClipIndex > this.m_ClipAnimations.arraySize - 1)
			{
				this.selectedClipIndex = 0;
			}
		}
		public void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this.m_AnimationClipEditor);
		}
		internal override void ResetValues()
		{
			base.ResetValues();
			this.m_ClipAnimations = base.serializedObject.FindProperty("m_ClipAnimations");
			this.m_AnimationType = base.serializedObject.FindProperty("m_AnimationType");
			this.m_DefaultClipsSerializedObject = null;
			if (this.m_ClipAnimations.arraySize == 0)
			{
				this.SetupDefaultClips();
			}
			this.ValidateClipSelectionIndex();
			this.UpdateList();
			this.SelectClip(this.selectedClipIndex);
		}
		private void AnimationClipGUI()
		{
			this.AnimationSettings();
			Profiler.BeginSample("Clip inspector");
			EditorGUILayout.Space();
			if (base.targets.Length == 1)
			{
				this.AnimationSplitTable();
			}
			else
			{
				GUILayout.Label(ModelImporterClipEditor.styles.clipMultiEditInfo, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			Profiler.EndSample();
			if (InternalEditorUtility.HasProFeaturesEnabled())
			{
				this.RootMotionNodeSettings();
			}
		}
		public override void OnInspectorGUI()
		{
			if (ModelImporterClipEditor.styles == null)
			{
				ModelImporterClipEditor.styles = new ModelImporterClipEditor.Styles();
			}
			EditorGUI.BeginDisabledGroup(this.singleImporter.animationType == ModelImporterAnimationType.None);
			EditorGUILayout.PropertyField(this.m_ImportAnimation, ModelImporterClipEditor.styles.ImportAnimations, new GUILayoutOption[0]);
			if (this.m_ImportAnimation.boolValue && !this.m_ImportAnimation.hasMultipleDifferentValues)
			{
				bool flag = base.targets.Length == 1 && this.singleImporter.importedTakeInfos.Length == 0;
				if (this.IsDeprecatedMultiAnimationRootImport())
				{
					EditorGUILayout.HelpBox("Animation data was imported using a deprecated Generation option in the Rig tab. Please switch to a non-deprecated import mode in the Rig tab to be able to edit the animation import settings.", MessageType.Info);
				}
				else
				{
					if (flag)
					{
						if (base.serializedObject.hasModifiedProperties)
						{
							EditorGUILayout.HelpBox("The animations settings can be edited after clicking Apply.", MessageType.Info);
						}
						else
						{
							EditorGUILayout.HelpBox("No animation data available in this model.", MessageType.Info);
						}
					}
					else
					{
						if (this.m_AnimationType.hasMultipleDifferentValues)
						{
							EditorGUILayout.HelpBox("The rigs of the selected models have different animation types.", MessageType.Info);
						}
						else
						{
							if (this.animationType == ModelImporterAnimationType.None)
							{
								EditorGUILayout.HelpBox("The rigs is not setup to handle animation. Edit the settings in the Rig tab.", MessageType.Info);
							}
							else
							{
								if (this.m_ImportAnimation.boolValue && !this.m_ImportAnimation.hasMultipleDifferentValues)
								{
									this.AnimationClipGUI();
								}
							}
						}
					}
				}
			}
			EditorGUI.EndDisabledGroup();
			base.ApplyRevertGUI();
		}
		private void AnimationSettings()
		{
			EditorGUILayout.Space();
			bool flag = true;
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				ModelImporter modelImporter = (ModelImporter)targets[i];
				if (!modelImporter.isBakeIKSupported)
				{
					flag = false;
				}
			}
			EditorGUI.BeginDisabledGroup(!flag);
			EditorGUILayout.PropertyField(this.m_BakeSimulation, ModelImporterClipEditor.styles.BakeIK, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			if (this.animationType == ModelImporterAnimationType.Legacy)
			{
				EditorGUI.showMixedValue = this.m_AnimationWrapMode.hasMultipleDifferentValues;
				EditorGUILayout.Popup(this.m_AnimationWrapMode, ModelImporterClipEditor.styles.AnimWrapModeOpt, ModelImporterClipEditor.styles.AnimWrapModeLabel, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				int[] optionValues = new int[]
				{
					0,
					1,
					2
				};
				EditorGUILayout.IntPopup(this.m_AnimationCompression, ModelImporterClipEditor.styles.AnimCompressionOptLegacy, optionValues, ModelImporterClipEditor.styles.AnimCompressionLabel, new GUILayoutOption[0]);
			}
			else
			{
				int[] optionValues2 = new int[]
				{
					0,
					1,
					3
				};
				EditorGUILayout.IntPopup(this.m_AnimationCompression, ModelImporterClipEditor.styles.AnimCompressionOpt, optionValues2, ModelImporterClipEditor.styles.AnimCompressionLabel, new GUILayoutOption[0]);
			}
			if (this.m_AnimationCompression.intValue > 0)
			{
				EditorGUILayout.PropertyField(this.m_AnimationRotationError, ModelImporterClipEditor.styles.AnimRotationErrorLabel, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AnimationPositionError, ModelImporterClipEditor.styles.AnimPositionErrorLabel, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AnimationScaleError, ModelImporterClipEditor.styles.AnimScaleErrorLabel, new GUILayoutOption[0]);
				GUILayout.Label(ModelImporterClipEditor.styles.AnimationCompressionHelp, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
		}
		private void RootMotionNodeSettings()
		{
			if (this.animationType == ModelImporterAnimationType.Human || this.animationType == ModelImporterAnimationType.Generic)
			{
				ModelImporterClipEditor.motionNodeFoldout = EditorGUILayout.Foldout(ModelImporterClipEditor.motionNodeFoldout, ModelImporterClipEditor.styles.MotionSetting);
				if (ModelImporterClipEditor.motionNodeFoldout)
				{
					EditorGUI.BeginChangeCheck();
					this.motionNodeIndex = EditorGUILayout.Popup(ModelImporterClipEditor.styles.MotionNode, this.motionNodeIndex, this.m_MotionNodeList, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						if (this.motionNodeIndex > 0 && this.motionNodeIndex < this.m_MotionNodeList.Length)
						{
							this.m_MotionNodeName.stringValue = this.m_MotionNodeList[this.motionNodeIndex].text;
						}
						else
						{
							this.m_MotionNodeName.stringValue = string.Empty;
						}
					}
				}
			}
		}
		private void SelectClip(int selected)
		{
			if (EditorGUI.s_DelayedTextEditor != null && Event.current != null)
			{
				EditorGUI.s_DelayedTextEditor.EndGUI(Event.current.type);
			}
			UnityEngine.Object.DestroyImmediate(this.m_AnimationClipEditor);
			this.m_AnimationClipEditor = null;
			this.selectedClipIndex = selected;
			if (this.selectedClipIndex < 0 || this.selectedClipIndex >= this.m_ClipAnimations.arraySize)
			{
				this.selectedClipIndex = -1;
				return;
			}
			AnimationClipInfoProperties animationClipInfoAtIndex = this.GetAnimationClipInfoAtIndex(selected);
			AnimationClip previewAnimationClipForTake = this.singleImporter.GetPreviewAnimationClipForTake(animationClipInfoAtIndex.takeName);
			if (previewAnimationClipForTake != null)
			{
				this.m_AnimationClipEditor = (AnimationClipEditor)Editor.CreateEditor(previewAnimationClipForTake);
				this.SyncClipEditor();
			}
		}
		private void UpdateList()
		{
			if (this.m_ClipList == null)
			{
				return;
			}
			List<AnimationClipInfoProperties> list = new List<AnimationClipInfoProperties>();
			for (int i = 0; i < this.m_ClipAnimations.arraySize; i++)
			{
				list.Add(this.GetAnimationClipInfoAtIndex(i));
			}
			this.m_ClipList.list = list;
		}
		private void AddClipInList(ReorderableList list)
		{
			if (this.m_DefaultClipsSerializedObject != null)
			{
				this.TransferDefaultClipsToCustomClips();
			}
			this.AddClip(this.singleImporter.importedTakeInfos[0]);
			this.UpdateList();
			this.SelectClip(list.list.Count - 1);
		}
		private void RemoveClipInList(ReorderableList list)
		{
			this.TransferDefaultClipsToCustomClips();
			this.RemoveClip(list.index);
			this.UpdateList();
			this.SelectClip(Mathf.Min(list.index, list.count - 1));
		}
		private void SelectClipInList(ReorderableList list)
		{
			this.SelectClip(list.index);
		}
		private void DrawClipElement(Rect rect, int index, bool selected, bool focused)
		{
			AnimationClipInfoProperties animationClipInfoProperties = this.m_ClipList.list[index] as AnimationClipInfoProperties;
			rect.xMax -= 90f;
			GUI.Label(rect, animationClipInfoProperties.name, EditorStyles.label);
			rect.x = rect.xMax;
			rect.width = 45f;
			GUI.Label(rect, animationClipInfoProperties.firstFrame.ToString("0.0"), ModelImporterClipEditor.styles.numberStyle);
			rect.x = rect.xMax;
			GUI.Label(rect, animationClipInfoProperties.lastFrame.ToString("0.0"), ModelImporterClipEditor.styles.numberStyle);
		}
		private void DrawClipHeader(Rect rect)
		{
			rect.xMax -= 90f;
			GUI.Label(rect, "Clips", EditorStyles.label);
			rect.x = rect.xMax;
			rect.width = 45f;
			GUI.Label(rect, "Start", ModelImporterClipEditor.styles.numberStyle);
			rect.x = rect.xMax;
			GUI.Label(rect, "End", ModelImporterClipEditor.styles.numberStyle);
		}
		private void AnimationSplitTable()
		{
			if (this.m_ClipList == null)
			{
				this.m_ClipList = new ReorderableList(new List<AnimationClipInfoProperties>(), typeof(string), false, true, true, true);
				this.m_ClipList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddClipInList);
				this.m_ClipList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectClipInList);
				this.m_ClipList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveClipInList);
				this.m_ClipList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawClipElement);
				this.m_ClipList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawClipHeader);
				this.m_ClipList.elementHeight = 16f;
				this.UpdateList();
				this.m_ClipList.index = this.selectedClipIndex;
			}
			this.m_ClipList.DoLayoutList();
			EditorGUI.BeginChangeCheck();
			AnimationClipInfoProperties selectedClipInfo = this.GetSelectedClipInfo();
			if (selectedClipInfo == null)
			{
				return;
			}
			if (this.m_AnimationClipEditor != null && this.selectedClipIndex != -1)
			{
				GUILayout.Space(5f);
				AnimationClip animationClip = this.m_AnimationClipEditor.target as AnimationClip;
				if (!animationClip.legacy)
				{
					this.GetSelectedClipInfo().AssignToPreviewClip(animationClip);
				}
				TakeInfo[] importedTakeInfos = this.singleImporter.importedTakeInfos;
				string[] array = new string[importedTakeInfos.Length];
				for (int i = 0; i < importedTakeInfos.Length; i++)
				{
					array[i] = importedTakeInfos[i].name;
				}
				EditorGUI.BeginChangeCheck();
				string name = selectedClipInfo.name;
				int num = ArrayUtility.IndexOf<string>(array, selectedClipInfo.takeName);
				this.m_AnimationClipEditor.takeNames = array;
				this.m_AnimationClipEditor.takeIndex = ArrayUtility.IndexOf<string>(array, selectedClipInfo.takeName);
				this.m_AnimationClipEditor.DrawHeader();
				if (EditorGUI.EndChangeCheck())
				{
					if (selectedClipInfo.name != name)
					{
						this.TransferDefaultClipsToCustomClips();
						PatchImportSettingRecycleID.Patch(base.serializedObject, 74, name, selectedClipInfo.name);
					}
					int takeIndex = this.m_AnimationClipEditor.takeIndex;
					if (takeIndex != -1 && takeIndex != num)
					{
						selectedClipInfo.name = this.MakeUniqueClipName(array[takeIndex], -1);
						this.SetupTakeNameAndFrames(selectedClipInfo, importedTakeInfos[takeIndex]);
						GUIUtility.keyboardControl = 0;
						this.SelectClip(this.selectedClipIndex);
						animationClip = (this.m_AnimationClipEditor.target as AnimationClip);
					}
				}
				this.m_AnimationClipEditor.OnInspectorGUI();
				if (!animationClip.legacy)
				{
					this.GetSelectedClipInfo().ExtractFromPreviewClip(animationClip);
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.TransferDefaultClipsToCustomClips();
			}
		}
		public override bool HasPreviewGUI()
		{
			return this.m_AnimationClipEditor != null && this.m_AnimationClipEditor.HasPreviewGUI();
		}
		public override void OnPreviewSettings()
		{
			if (this.m_AnimationClipEditor != null)
			{
				this.m_AnimationClipEditor.OnPreviewSettings();
			}
		}
		private bool IsDeprecatedMultiAnimationRootImport()
		{
			return this.animationType == ModelImporterAnimationType.Legacy && (this.legacyGenerateAnimations == ModelImporterGenerateAnimations.InOriginalRoots || this.legacyGenerateAnimations == ModelImporterGenerateAnimations.InNodes);
		}
		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_AnimationClipEditor)
			{
				this.m_AnimationClipEditor.OnInteractivePreviewGUI(r, background);
			}
		}
		private AnimationClipInfoProperties GetAnimationClipInfoAtIndex(int index)
		{
			return new AnimationClipInfoProperties(this.m_ClipAnimations.GetArrayElementAtIndex(index));
		}
		private AnimationClipInfoProperties GetSelectedClipInfo()
		{
			if (this.selectedClipIndex >= 0 && this.selectedClipIndex < this.m_ClipAnimations.arraySize)
			{
				return this.GetAnimationClipInfoAtIndex(this.selectedClipIndex);
			}
			return null;
		}
		private string MakeUniqueClipName(string name, int row)
		{
			string text = name;
			int num = 0;
			int i;
			do
			{
				for (i = 0; i < this.m_ClipAnimations.arraySize; i++)
				{
					AnimationClipInfoProperties animationClipInfoAtIndex = this.GetAnimationClipInfoAtIndex(i);
					if (text == animationClipInfoAtIndex.name && row != i)
					{
						text = name + num.ToString();
						num++;
						break;
					}
				}
			}
			while (i != this.m_ClipAnimations.arraySize);
			return text;
		}
		private void RemoveClip(int index)
		{
			this.m_ClipAnimations.DeleteArrayElementAtIndex(index);
			if (this.m_ClipAnimations.arraySize == 0)
			{
				this.SetupDefaultClips();
				this.m_ImportAnimation.boolValue = false;
			}
		}
		private void SetupTakeNameAndFrames(AnimationClipInfoProperties info, TakeInfo takeInfo)
		{
			info.takeName = takeInfo.name;
			info.firstFrame = (float)((int)Mathf.Round(takeInfo.bakeStartTime * takeInfo.sampleRate));
			info.lastFrame = (float)((int)Mathf.Round(takeInfo.bakeStopTime * takeInfo.sampleRate));
		}
		private void AddClip(TakeInfo takeInfo)
		{
			this.m_ClipAnimations.InsertArrayElementAtIndex(this.m_ClipAnimations.arraySize);
			AnimationClipInfoProperties animationClipInfoAtIndex = this.GetAnimationClipInfoAtIndex(this.m_ClipAnimations.arraySize - 1);
			animationClipInfoAtIndex.name = this.MakeUniqueClipName(takeInfo.defaultClipName, -1);
			this.SetupTakeNameAndFrames(animationClipInfoAtIndex, takeInfo);
			animationClipInfoAtIndex.wrapMode = 0;
			animationClipInfoAtIndex.loop = false;
			animationClipInfoAtIndex.orientationOffsetY = 0f;
			animationClipInfoAtIndex.level = 0f;
			animationClipInfoAtIndex.cycleOffset = 0f;
			animationClipInfoAtIndex.loopTime = false;
			animationClipInfoAtIndex.loopBlend = false;
			animationClipInfoAtIndex.loopBlendOrientation = false;
			animationClipInfoAtIndex.loopBlendPositionY = false;
			animationClipInfoAtIndex.loopBlendPositionXZ = false;
			animationClipInfoAtIndex.keepOriginalOrientation = false;
			animationClipInfoAtIndex.keepOriginalPositionY = true;
			animationClipInfoAtIndex.keepOriginalPositionXZ = false;
			animationClipInfoAtIndex.heightFromFeet = false;
			animationClipInfoAtIndex.mirror = false;
			animationClipInfoAtIndex.maskType = ClipAnimationMaskType.CreateFromThisModel;
			AvatarMask avatarMask = new AvatarMask();
			string[] array = null;
			SerializedObject serializedObject = animationClipInfoAtIndex.maskTypeProperty.serializedObject;
			ModelImporter modelImporter = serializedObject.targetObject as ModelImporter;
			if (this.animationType == ModelImporterAnimationType.Human && !modelImporter.isAssetOlderOr42)
			{
				array = AvatarMaskUtility.GetAvatarHumanTransform(serializedObject, modelImporter.transformPaths);
				if (array == null)
				{
					return;
				}
			}
			AvatarMaskUtility.UpdateTransformMask(avatarMask, modelImporter.transformPaths, array);
			animationClipInfoAtIndex.MaskToClip(avatarMask);
			animationClipInfoAtIndex.ClearEvents();
			animationClipInfoAtIndex.ClearCurves();
			UnityEngine.Object.DestroyImmediate(avatarMask);
		}
	}
}
