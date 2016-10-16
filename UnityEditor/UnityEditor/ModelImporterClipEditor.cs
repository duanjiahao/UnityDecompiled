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
			public GUIContent ImportAnimations = EditorGUIUtility.TextContent("Import Animation|Controls if animations are imported.");

			public GUIStyle numberStyle = new GUIStyle(EditorStyles.label);

			public GUIContent AnimWrapModeLabel = EditorGUIUtility.TextContent("Wrap Mode|The default Wrap Mode for the animation in the mesh being imported.");

			public GUIContent[] AnimWrapModeOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Default|The animation plays as specified in the animation splitting options below."),
				EditorGUIUtility.TextContent("Once|The animation plays through to the end once and then stops."),
				EditorGUIUtility.TextContent("Loop|The animation plays through and then restarts when the end is reached."),
				EditorGUIUtility.TextContent("PingPong|The animation plays through and then plays in reverse from the end to the start, and so on."),
				EditorGUIUtility.TextContent("ClampForever|The animation plays through but the last frame is repeated indefinitely. This is not the same as Once mode because playback does not technically stop at the last frame (which is useful when blending animations).")
			};

			public GUIContent BakeIK = EditorGUIUtility.TextContent("Bake Animations|Enable this when using IK or simulation in your animation package. Unity will convert to forward kinematics on import. This option is available only for Maya, 3dsMax and Cinema4D files.");

			public GUIContent ResampleCurves = EditorGUIUtility.TextContent("Resample Curves | Curves will be resampled on every frame. Use this if you're having issues with the interpolation between keys in your original animation. Disable this to keep curves as close as possible to how they were originally authored.");

			public GUIContent AnimCompressionLabel = EditorGUIUtility.TextContent("Anim. Compression|The type of compression that will be applied to this mesh's animation(s).");

			public GUIContent[] AnimCompressionOptLegacy = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Off|Disables animation compression. This means that Unity doesn't reduce keyframe count on import, which leads to the highest precision animations, but slower performance and bigger file and runtime memory size. It is generally not advisable to use this option - if you need higher precision animation, you should enable keyframe reduction and lower allowed Animation Compression Error values instead."),
				EditorGUIUtility.TextContent("Keyframe Reduction|Reduces keyframes on import. If selected, the Animation Compression Errors options are displayed."),
				EditorGUIUtility.TextContent("Keyframe Reduction and Compression|Reduces keyframes on import and compresses keyframes when storing animations in files. This affects only file size - the runtime memory size is the same as Keyframe Reduction. If selected, the Animation Compression Errors options are displayed.")
			};

			public GUIContent[] AnimCompressionOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Off|Disables animation compression. This means that Unity doesn't reduce keyframe count on import, which leads to the highest precision animations, but slower performance and bigger file and runtime memory size. It is generally not advisable to use this option - if you need higher precision animation, you should enable keyframe reduction and lower allowed Animation Compression Error values instead."),
				EditorGUIUtility.TextContent("Keyframe Reduction|Reduces keyframes on import. If selected, the Animation Compression Errors options are displayed."),
				EditorGUIUtility.TextContent("Optimal|Reduces keyframes on import and choose between different curve representations to reduce memory usage at runtime. This affects the runtime memory size and how curves are evaluated.")
			};

			public GUIContent AnimRotationErrorLabel = EditorGUIUtility.TextContent("Rotation Error|Defines how much rotation curves should be reduced. The smaller value you use - the higher precision you get.");

			public GUIContent AnimPositionErrorLabel = EditorGUIUtility.TextContent("Position Error|Defines how much position curves should be reduced. The smaller value you use - the higher precision you get.");

			public GUIContent AnimScaleErrorLabel = EditorGUIUtility.TextContent("Scale Error|Defines how much scale curves should be reduced. The smaller value you use - the higher precision you get.");

			public GUIContent AnimationCompressionHelp = EditorGUIUtility.TextContent("Rotation error is defined as maximum angle deviation allowed in degrees, for others it is defined as maximum distance/delta deviation allowed in percents");

			public GUIContent clipMultiEditInfo = new GUIContent("Multi-object editing of clips not supported.");

			public GUIContent updateMuscleDefinitionFromSource = EditorGUIUtility.TextContent("Update|Update the copy of the muscle definition from the source.");

			public GUIContent MotionSetting = EditorGUIUtility.TextContent("Motion|Advanced setting for root motion and blending pivot");

			public GUIContent MotionNode = EditorGUIUtility.TextContent("Root Motion Node|Define a transform node that will be used to create root motion curves");

			public GUIContent ImportMessages = EditorGUIUtility.TextContent("Import Messages");

			public GUIContent GenerateRetargetingWarnings = EditorGUIUtility.TextContent("Generate Retargeting Quality Report");

			public GUIContent Mask = EditorGUIUtility.TextContent("Mask|Configure the mask for this clip to remove unnecessary curves.");

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

		private SerializedProperty m_ResampleCurves;

		private SerializedProperty m_AnimationCompression;

		private SerializedProperty m_AnimationRotationError;

		private SerializedProperty m_AnimationPositionError;

		private SerializedProperty m_AnimationScaleError;

		private SerializedProperty m_AnimationWrapMode;

		private SerializedProperty m_LegacyGenerateAnimations;

		private SerializedProperty m_MotionNodeName;

		private SerializedProperty m_PivotNodeName;

		private SerializedProperty m_AnimationImportErrors;

		private SerializedProperty m_AnimationImportWarnings;

		private SerializedProperty m_AnimationRetargetingWarnings;

		private SerializedProperty m_AnimationDoRetargetingWarnings;

		private GUIContent[] m_MotionNodeList;

		private static bool motionNodeFoldout;

		private static bool importMessageFoldout;

		private ReorderableList m_ClipList;

		private static ModelImporterClipEditor.Styles styles;

		private AvatarMask m_Mask;

		private AvatarMaskInspector m_MaskInspector;

		private static bool m_MaskFoldout;

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

		public int pivotNodeIndex
		{
			get;
			set;
		}

		private string[] referenceTransformPaths
		{
			get
			{
				return this.singleImporter.transformPaths;
			}
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
			this.m_ResampleCurves = base.serializedObject.FindProperty("m_ResampleCurves");
			this.m_AnimationCompression = base.serializedObject.FindProperty("m_AnimationCompression");
			this.m_AnimationRotationError = base.serializedObject.FindProperty("m_AnimationRotationError");
			this.m_AnimationPositionError = base.serializedObject.FindProperty("m_AnimationPositionError");
			this.m_AnimationScaleError = base.serializedObject.FindProperty("m_AnimationScaleError");
			this.m_AnimationWrapMode = base.serializedObject.FindProperty("m_AnimationWrapMode");
			this.m_AnimationImportErrors = base.serializedObject.FindProperty("m_AnimationImportErrors");
			this.m_AnimationImportWarnings = base.serializedObject.FindProperty("m_AnimationImportWarnings");
			this.m_AnimationRetargetingWarnings = base.serializedObject.FindProperty("m_AnimationRetargetingWarnings");
			this.m_AnimationDoRetargetingWarnings = base.serializedObject.FindProperty("m_AnimationDoRetargetingWarnings");
			if (base.serializedObject.isEditingMultipleObjects)
			{
				return;
			}
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
			if (this.m_AnimationClipEditor == null || this.m_MaskInspector == null)
			{
				return;
			}
			AnimationClipInfoProperties animationClipInfoAtIndex = this.GetAnimationClipInfoAtIndex(this.selectedClipIndex);
			this.m_MaskInspector.clipInfo = animationClipInfoAtIndex;
			this.m_AnimationClipEditor.ShowRange(animationClipInfoAtIndex);
			this.m_AnimationClipEditor.mask = this.m_Mask;
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
			this.DestroyEditorsAndData();
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
			string stringValue = this.m_AnimationImportErrors.stringValue;
			string stringValue2 = this.m_AnimationImportWarnings.stringValue;
			string stringValue3 = this.m_AnimationRetargetingWarnings.stringValue;
			if (stringValue.Length > 0)
			{
				EditorGUILayout.HelpBox("Error(s) found while importing animation this file. Open \"Import Messages\" foldout below for more details", MessageType.Error);
			}
			else if (stringValue2.Length > 0)
			{
				EditorGUILayout.HelpBox("Warning(s) found while importing this animation file. Open \"Import Messages\" foldout below for more details", MessageType.Warning);
			}
			this.AnimationSettings();
			if (base.serializedObject.isEditingMultipleObjects)
			{
				return;
			}
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
			this.RootMotionNodeSettings();
			ModelImporterClipEditor.importMessageFoldout = EditorGUILayout.Foldout(ModelImporterClipEditor.importMessageFoldout, ModelImporterClipEditor.styles.ImportMessages);
			if (ModelImporterClipEditor.importMessageFoldout)
			{
				if (stringValue.Length > 0)
				{
					EditorGUILayout.HelpBox(stringValue, MessageType.Error);
				}
				if (stringValue2.Length > 0)
				{
					EditorGUILayout.HelpBox(stringValue2, MessageType.Warning);
				}
				if (this.animationType == ModelImporterAnimationType.Human)
				{
					EditorGUILayout.PropertyField(this.m_AnimationDoRetargetingWarnings, ModelImporterClipEditor.styles.GenerateRetargetingWarnings, new GUILayoutOption[0]);
					if (this.m_AnimationDoRetargetingWarnings.boolValue)
					{
						if (stringValue3.Length > 0)
						{
							EditorGUILayout.HelpBox(stringValue3, MessageType.Info);
						}
					}
					else
					{
						EditorGUILayout.HelpBox("Retargeting Quality compares retargeted with original animation. It reports average and maximum position/orientation difference for body parts. It may slow down import time of this file.", MessageType.Info);
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			if (ModelImporterClipEditor.styles == null)
			{
				ModelImporterClipEditor.styles = new ModelImporterClipEditor.Styles();
			}
			EditorGUILayout.PropertyField(this.m_ImportAnimation, ModelImporterClipEditor.styles.ImportAnimations, new GUILayoutOption[0]);
			if (this.m_ImportAnimation.boolValue && !this.m_ImportAnimation.hasMultipleDifferentValues)
			{
				bool flag = base.targets.Length == 1 && this.singleImporter.importedTakeInfos.Length == 0;
				if (this.IsDeprecatedMultiAnimationRootImport())
				{
					EditorGUILayout.HelpBox("Animation data was imported using a deprecated Generation option in the Rig tab. Please switch to a non-deprecated import mode in the Rig tab to be able to edit the animation import settings.", MessageType.Info);
				}
				else if (flag)
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
				else if (this.m_AnimationType.hasMultipleDifferentValues)
				{
					EditorGUILayout.HelpBox("The rigs of the selected models have different animation types.", MessageType.Info);
				}
				else if (this.animationType == ModelImporterAnimationType.None)
				{
					EditorGUILayout.HelpBox("The rigs is not setup to handle animation. Edit the settings in the Rig tab.", MessageType.Info);
				}
				else if (this.m_ImportAnimation.boolValue && !this.m_ImportAnimation.hasMultipleDifferentValues)
				{
					this.AnimationClipGUI();
				}
			}
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
			using (new EditorGUI.DisabledScope(!flag))
			{
				EditorGUILayout.PropertyField(this.m_BakeSimulation, ModelImporterClipEditor.styles.BakeIK, new GUILayoutOption[0]);
			}
			if (this.animationType == ModelImporterAnimationType.Generic)
			{
				EditorGUILayout.PropertyField(this.m_ResampleCurves, ModelImporterClipEditor.styles.ResampleCurves, new GUILayoutOption[0]);
			}
			else
			{
				this.m_ResampleCurves.boolValue = true;
			}
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

		private void DestroyEditorsAndData()
		{
			if (this.m_AnimationClipEditor != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_AnimationClipEditor);
				this.m_AnimationClipEditor = null;
			}
			if (this.m_MaskInspector)
			{
				UnityEngine.Object.DestroyImmediate(this.m_MaskInspector);
				this.m_MaskInspector = null;
			}
			if (this.m_Mask)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Mask);
				this.m_Mask = null;
			}
		}

		private void SelectClip(int selected)
		{
			if (EditorGUI.s_DelayedTextEditor != null && Event.current != null)
			{
				EditorGUI.s_DelayedTextEditor.EndGUI(Event.current.type);
			}
			this.DestroyEditorsAndData();
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
				this.InitMask(animationClipInfoAtIndex);
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
			int num = 0;
			if (0 < this.selectedClipIndex && this.selectedClipIndex < this.m_ClipAnimations.arraySize)
			{
				AnimationClipInfoProperties animationClipInfoAtIndex = this.GetAnimationClipInfoAtIndex(this.selectedClipIndex);
				for (int i = 0; i < this.singleImporter.importedTakeInfos.Length; i++)
				{
					if (this.singleImporter.importedTakeInfos[i].name == animationClipInfoAtIndex.takeName)
					{
						num = i;
						break;
					}
				}
			}
			this.AddClip(this.singleImporter.importedTakeInfos[num]);
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
				this.AvatarMaskSettings(this.GetSelectedClipInfo());
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
			this.SetBodyMaskDefaultValues(animationClipInfoAtIndex);
			this.SetTransformMaskFromReference(animationClipInfoAtIndex);
			animationClipInfoAtIndex.ClearEvents();
			animationClipInfoAtIndex.ClearCurves();
		}

		private void AvatarMaskSettings(AnimationClipInfoProperties clipInfo)
		{
			if (clipInfo != null && this.m_AnimationClipEditor != null)
			{
				this.InitMask(clipInfo);
				int indentLevel = EditorGUI.indentLevel;
				bool changed = GUI.changed;
				ModelImporterClipEditor.m_MaskFoldout = EditorGUILayout.Foldout(ModelImporterClipEditor.m_MaskFoldout, ModelImporterClipEditor.styles.Mask);
				GUI.changed = changed;
				if (clipInfo.maskType == ClipAnimationMaskType.CreateFromThisModel && !this.m_MaskInspector.IsMaskUpToDate())
				{
					GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
					GUILayout.Label("Mask does not match hierarchy. Animation might not import correctly", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					GUILayout.BeginVertical(new GUILayoutOption[0]);
					GUILayout.Space(5f);
					if (GUILayout.Button("Fix Mask", new GUILayoutOption[0]))
					{
						this.SetTransformMaskFromReference(clipInfo);
					}
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				else if (clipInfo.maskType == ClipAnimationMaskType.CopyFromOther && clipInfo.MaskNeedsUpdating())
				{
					GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
					GUILayout.Label("Source Mask has changed since last import. It must be Updated", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					GUILayout.BeginVertical(new GUILayoutOption[0]);
					GUILayout.Space(5f);
					if (GUILayout.Button("Update Mask", new GUILayoutOption[0]))
					{
						clipInfo.MaskToClip(clipInfo.maskSource);
					}
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				if (ModelImporterClipEditor.m_MaskFoldout)
				{
					EditorGUI.indentLevel++;
					this.m_MaskInspector.OnInspectorGUI();
				}
				EditorGUI.indentLevel = indentLevel;
			}
		}

		private void InitMask(AnimationClipInfoProperties clipInfo)
		{
			if (this.m_Mask == null)
			{
				AnimationClip animationClip = this.m_AnimationClipEditor.target as AnimationClip;
				this.m_Mask = new AvatarMask();
				this.m_MaskInspector = (AvatarMaskInspector)Editor.CreateEditor(this.m_Mask);
				this.m_MaskInspector.canImport = false;
				this.m_MaskInspector.showBody = animationClip.isHumanMotion;
				this.m_MaskInspector.clipInfo = clipInfo;
			}
		}

		private void SetTransformMaskFromReference(AnimationClipInfoProperties clipInfo)
		{
			string[] referenceTransformPaths = this.referenceTransformPaths;
			string[] humanTransforms = (this.animationType != ModelImporterAnimationType.Human) ? null : AvatarMaskUtility.GetAvatarHumanTransform(base.serializedObject, referenceTransformPaths);
			AvatarMaskUtility.UpdateTransformMask(clipInfo.transformMaskProperty, referenceTransformPaths, humanTransforms);
		}

		private void SetBodyMaskDefaultValues(AnimationClipInfoProperties clipInfo)
		{
			SerializedProperty bodyMaskProperty = clipInfo.bodyMaskProperty;
			bodyMaskProperty.ClearArray();
			for (int i = 0; i < 13; i++)
			{
				bodyMaskProperty.InsertArrayElementAtIndex(i);
				bodyMaskProperty.GetArrayElementAtIndex(i).intValue = 1;
			}
		}
	}
}
