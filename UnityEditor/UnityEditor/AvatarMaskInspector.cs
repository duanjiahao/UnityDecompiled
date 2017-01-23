using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEditor
{
	[CustomEditor(typeof(AvatarMask))]
	internal class AvatarMaskInspector : Editor
	{
		private static class Styles
		{
			public static GUIContent MaskDefinition = EditorGUIUtility.TextContent("Definition|Choose between Create From This Model, Copy From Other Avatar. The first one create a Mask for this file and the second one use a Mask from another file to import animation.");

			public static GUIContent[] MaskDefinitionOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Create From This Model|Create a Mask based on the model from this file. For Humanoid rig all the human transform are always imported and converted to muscle curve, thus they cannot be unchecked."),
				EditorGUIUtility.TextContent("Copy From Other Mask|Copy a Mask from another file to import animation clip."),
				EditorGUIUtility.TextContent("None | Import Everything")
			};

			public static GUIContent BodyMask = EditorGUIUtility.TextContent("Humanoid|Define which body part are active. Also define which animation curves will be imported for an Animation Clip.");

			public static GUIContent TransformMask = EditorGUIUtility.TextContent("Transform|Define which transform are active. Also define which animation curves will be imported for an Animation Clip.");
		}

		private struct NodeInfo
		{
			public bool m_Expanded;

			public bool m_Show;

			public bool m_Enabled;

			public int m_ParentIndex;

			public List<int> m_ChildIndices;

			public int m_Depth;

			public SerializedProperty m_Path;

			public SerializedProperty m_Weight;

			public string m_Name;
		}

		private bool m_ShowBodyMask = true;

		private bool m_BodyMaskFoldout = false;

		private bool m_CanImport = true;

		private SerializedProperty m_BodyMask = null;

		private SerializedProperty m_TransformMask = null;

		private SerializedProperty m_AnimationType = null;

		private AnimationClipInfoProperties m_ClipInfo = null;

		private string[] m_TransformPaths = null;

		private AvatarMaskInspector.NodeInfo[] m_NodeInfos;

		private Avatar m_RefAvatar;

		private ModelImporter m_RefImporter;

		private bool m_TransformMaskFoldout = false;

		private string[] m_HumanTransform = null;

		public bool canImport
		{
			get
			{
				return this.m_CanImport;
			}
			set
			{
				this.m_CanImport = value;
			}
		}

		public AnimationClipInfoProperties clipInfo
		{
			get
			{
				return this.m_ClipInfo;
			}
			set
			{
				this.m_ClipInfo = value;
				if (this.m_ClipInfo != null)
				{
					this.m_ClipInfo.MaskFromClip(base.target as AvatarMask);
					SerializedObject serializedObject = this.m_ClipInfo.maskTypeProperty.serializedObject;
					this.m_AnimationType = serializedObject.FindProperty("m_AnimationType");
					ModelImporter modelImporter = serializedObject.targetObject as ModelImporter;
					this.m_TransformPaths = modelImporter.transformPaths;
				}
				else
				{
					this.m_TransformPaths = null;
					this.m_AnimationType = null;
				}
				this.InitializeSerializedProperties();
			}
		}

		private ModelImporterAnimationType animationType
		{
			get
			{
				ModelImporterAnimationType result;
				if (this.m_AnimationType != null)
				{
					result = (ModelImporterAnimationType)this.m_AnimationType.intValue;
				}
				else
				{
					result = ModelImporterAnimationType.None;
				}
				return result;
			}
		}

		public bool showBody
		{
			get
			{
				return this.m_ShowBodyMask;
			}
			set
			{
				this.m_ShowBodyMask = value;
			}
		}

		public string[] humanTransforms
		{
			get
			{
				if (this.animationType == ModelImporterAnimationType.Human && this.clipInfo != null)
				{
					if (this.m_HumanTransform == null)
					{
						SerializedObject serializedObject = this.clipInfo.maskTypeProperty.serializedObject;
						ModelImporter modelImporter = serializedObject.targetObject as ModelImporter;
						this.m_HumanTransform = AvatarMaskUtility.GetAvatarHumanTransform(serializedObject, modelImporter.transformPaths);
					}
				}
				else
				{
					this.m_HumanTransform = null;
				}
				return this.m_HumanTransform;
			}
		}

		private void InitializeSerializedProperties()
		{
			if (this.clipInfo != null)
			{
				this.m_BodyMask = this.clipInfo.bodyMaskProperty;
				this.m_TransformMask = this.clipInfo.transformMaskProperty;
			}
			else
			{
				this.m_BodyMask = base.serializedObject.FindProperty("m_Mask");
				this.m_TransformMask = base.serializedObject.FindProperty("m_Elements");
			}
			this.FillNodeInfos();
		}

		private void OnEnable()
		{
			this.InitializeSerializedProperties();
		}

		private ClipAnimationMaskType IndexToMaskType(int index)
		{
			ClipAnimationMaskType result;
			if (index != 2)
			{
				result = (ClipAnimationMaskType)index;
			}
			else
			{
				result = ClipAnimationMaskType.None;
			}
			return result;
		}

		private int MaskTypeToIndex(ClipAnimationMaskType maskType)
		{
			int result;
			if (maskType != ClipAnimationMaskType.None)
			{
				result = (int)maskType;
			}
			else
			{
				result = 2;
			}
			return result;
		}

		public override void OnInspectorGUI()
		{
			Profiler.BeginSample("AvatarMaskInspector.OnInspectorGUI()");
			if (this.clipInfo == null)
			{
				base.serializedObject.Update();
			}
			bool flag = false;
			if (this.clipInfo != null)
			{
				EditorGUI.BeginChangeCheck();
				int num = this.MaskTypeToIndex(this.clipInfo.maskType);
				EditorGUI.showMixedValue = this.clipInfo.maskTypeProperty.hasMultipleDifferentValues;
				num = EditorGUILayout.Popup(AvatarMaskInspector.Styles.MaskDefinition, num, AvatarMaskInspector.Styles.MaskDefinitionOpt, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					this.clipInfo.maskType = this.IndexToMaskType(num);
					this.UpdateMask(this.clipInfo.maskType);
				}
				flag = (this.clipInfo.maskType == ClipAnimationMaskType.CopyFromOther);
			}
			if (flag)
			{
				this.CopyFromOtherGUI();
			}
			bool enabled = GUI.enabled;
			GUI.enabled = !flag;
			EditorGUI.BeginChangeCheck();
			this.OnBodyInspectorGUI();
			this.OnTransformInspectorGUI();
			if (this.clipInfo != null && EditorGUI.EndChangeCheck())
			{
				AvatarMask mask = base.target as AvatarMask;
				this.clipInfo.MaskFromClip(mask);
			}
			GUI.enabled = enabled;
			if (this.clipInfo == null)
			{
				base.serializedObject.ApplyModifiedProperties();
			}
			Profiler.EndSample();
		}

		protected void CopyFromOtherGUI()
		{
			if (this.clipInfo != null)
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.clipInfo.maskSourceProperty, GUIContent.Temp("Source"), new GUILayoutOption[0]);
				AvatarMask x = this.clipInfo.maskSourceProperty.objectReferenceValue as AvatarMask;
				if (EditorGUI.EndChangeCheck() && x != null)
				{
					this.UpdateMask(this.clipInfo.maskType);
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		public bool IsMaskEmpty()
		{
			return this.m_NodeInfos.Length == 0;
		}

		public bool IsMaskUpToDate()
		{
			bool result;
			if (this.clipInfo == null)
			{
				result = false;
			}
			else if (this.m_NodeInfos.Length != this.m_TransformPaths.Length)
			{
				result = false;
			}
			else
			{
				if (this.m_TransformMask.arraySize > 0)
				{
					SerializedProperty arrayElementAtIndex = this.m_TransformMask.GetArrayElementAtIndex(0);
					for (int i = 1; i < this.m_NodeInfos.Length; i++)
					{
						string path = this.m_NodeInfos[i].m_Path.stringValue;
						int num = ArrayUtility.FindIndex<string>(this.m_TransformPaths, (string s) => s == path);
						if (num == -1)
						{
							result = false;
							return result;
						}
						arrayElementAtIndex.Next(false);
					}
				}
				result = true;
			}
			return result;
		}

		private void UpdateMask(ClipAnimationMaskType maskType)
		{
			if (this.clipInfo != null)
			{
				if (maskType == ClipAnimationMaskType.CreateFromThisModel)
				{
					SerializedObject serializedObject = this.clipInfo.maskTypeProperty.serializedObject;
					ModelImporter modelImporter = serializedObject.targetObject as ModelImporter;
					AvatarMaskUtility.UpdateTransformMask(this.m_TransformMask, modelImporter.transformPaths, this.humanTransforms);
					this.FillNodeInfos();
				}
				else if (maskType == ClipAnimationMaskType.CopyFromOther)
				{
					AvatarMask avatarMask = this.clipInfo.maskSourceProperty.objectReferenceValue as AvatarMask;
					if (avatarMask != null)
					{
						AvatarMask avatarMask2 = base.target as AvatarMask;
						avatarMask2.Copy(avatarMask);
						if (this.humanTransforms != null)
						{
							AvatarMaskUtility.SetActiveHumanTransforms(avatarMask2, this.humanTransforms);
						}
						this.clipInfo.MaskToClip(avatarMask2);
						this.FillNodeInfos();
					}
				}
				else if (maskType == ClipAnimationMaskType.None)
				{
					AvatarMask mask = new AvatarMask();
					ModelImporter.UpdateTransformMask(mask, this.clipInfo.transformMaskProperty);
				}
				AvatarMask mask2 = base.target as AvatarMask;
				this.clipInfo.MaskFromClip(mask2);
			}
		}

		public void OnBodyInspectorGUI()
		{
			if (this.m_ShowBodyMask)
			{
				bool changed = GUI.changed;
				this.m_BodyMaskFoldout = EditorGUILayout.Foldout(this.m_BodyMaskFoldout, AvatarMaskInspector.Styles.BodyMask, true);
				GUI.changed = changed;
				if (this.m_BodyMaskFoldout)
				{
					BodyMaskEditor.Show(this.m_BodyMask, 13);
				}
			}
		}

		public void OnTransformInspectorGUI()
		{
			float xmin = 0f;
			float ymin = 0f;
			float num = 0f;
			float ymax = 0f;
			bool changed = GUI.changed;
			this.m_TransformMaskFoldout = EditorGUILayout.Foldout(this.m_TransformMaskFoldout, AvatarMaskInspector.Styles.TransformMask, true);
			GUI.changed = changed;
			if (this.m_TransformMaskFoldout)
			{
				if (this.canImport)
				{
					this.ImportAvatarReference();
				}
				if (this.m_NodeInfos == null || this.m_TransformMask.arraySize != this.m_NodeInfos.Length)
				{
					this.FillNodeInfos();
				}
				if (this.IsMaskEmpty())
				{
					GUILayout.BeginVertical(new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
					string text;
					if (this.animationType == ModelImporterAnimationType.Generic)
					{
						text = "No transform mask defined, everything will be imported";
					}
					else if (this.animationType == ModelImporterAnimationType.Human)
					{
						text = "No transform mask defined, only human curves will be imported";
					}
					else
					{
						text = "No transform mask defined";
					}
					GUILayout.Label(text, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
					GUILayout.EndHorizontal();
					if (!this.canImport && this.clipInfo.maskType == ClipAnimationMaskType.CreateFromThisModel)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.FlexibleSpace();
						string text2 = "Create Mask";
						if (GUILayout.Button(text2, new GUILayoutOption[0]))
						{
							this.UpdateMask(this.clipInfo.maskType);
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndVertical();
				}
				else
				{
					this.ComputeShownElements();
					GUILayout.Space(1f);
					int indentLevel = EditorGUI.indentLevel;
					int arraySize = this.m_TransformMask.arraySize;
					for (int i = 1; i < arraySize; i++)
					{
						if (this.m_NodeInfos[i].m_Show)
						{
							GUILayout.BeginHorizontal(new GUILayoutOption[0]);
							EditorGUI.indentLevel = this.m_NodeInfos[i].m_Depth + 1;
							EditorGUI.BeginChangeCheck();
							Rect rect = GUILayoutUtility.GetRect(15f, 15f, new GUILayoutOption[]
							{
								GUILayout.ExpandWidth(false)
							});
							GUILayoutUtility.GetRect(10f, 15f, new GUILayoutOption[]
							{
								GUILayout.ExpandWidth(false)
							});
							rect.x += 15f;
							bool enabled = GUI.enabled;
							GUI.enabled = this.m_NodeInfos[i].m_Enabled;
							bool flag = Event.current.button == 1;
							bool flag2 = this.m_NodeInfos[i].m_Weight.floatValue > 0f;
							flag2 = GUI.Toggle(rect, flag2, "");
							GUI.enabled = enabled;
							if (EditorGUI.EndChangeCheck())
							{
								this.m_NodeInfos[i].m_Weight.floatValue = ((!flag2) ? 0f : 1f);
								if (!flag)
								{
									this.CheckChildren(i, flag2);
								}
							}
							if (this.m_NodeInfos[i].m_ChildIndices.Count > 0)
							{
								this.m_NodeInfos[i].m_Expanded = EditorGUILayout.Foldout(this.m_NodeInfos[i].m_Expanded, this.m_NodeInfos[i].m_Name, true);
							}
							else
							{
								EditorGUILayout.LabelField(this.m_NodeInfos[i].m_Name, new GUILayoutOption[0]);
							}
							if (i == 1)
							{
								ymin = rect.yMin;
								xmin = rect.xMin;
							}
							else if (i == arraySize - 1)
							{
								ymax = rect.yMax;
							}
							num = Mathf.Max(num, GUILayoutUtility.GetLastRect().xMax);
							GUILayout.EndHorizontal();
						}
					}
					EditorGUI.indentLevel = indentLevel;
				}
			}
			Rect rect2 = Rect.MinMaxRect(xmin, ymin, num, ymax);
			if (Event.current != null && Event.current.type == EventType.MouseUp && Event.current.button == 1 && rect2.Contains(Event.current.mousePosition))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Select all"), false, new GenericMenu.MenuFunction(this.SelectAll));
				genericMenu.AddItem(new GUIContent("Deselect all"), false, new GenericMenu.MenuFunction(this.DeselectAll));
				genericMenu.ShowAsContext();
				Event.current.Use();
			}
		}

		private void SetAllTransformActive(bool active)
		{
			for (int i = 0; i < this.m_NodeInfos.Length; i++)
			{
				if (this.m_NodeInfos[i].m_Enabled)
				{
					this.m_NodeInfos[i].m_Weight.floatValue = ((!active) ? 0f : 1f);
				}
			}
		}

		private void SelectAll()
		{
			this.SetAllTransformActive(true);
		}

		private void DeselectAll()
		{
			this.SetAllTransformActive(false);
		}

		private void ImportAvatarReference()
		{
			EditorGUI.BeginChangeCheck();
			this.m_RefAvatar = (EditorGUILayout.ObjectField("Use skeleton from", this.m_RefAvatar, typeof(Avatar), true, new GUILayoutOption[0]) as Avatar);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_RefImporter = (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.m_RefAvatar)) as ModelImporter);
			}
			if (this.m_RefImporter != null && GUILayout.Button("Import skeleton", new GUILayoutOption[0]))
			{
				AvatarMaskUtility.UpdateTransformMask(this.m_TransformMask, this.m_RefImporter.transformPaths, null);
			}
		}

		private void FillNodeInfos()
		{
			this.m_NodeInfos = new AvatarMaskInspector.NodeInfo[this.m_TransformMask.arraySize];
			if (this.m_TransformMask.arraySize != 0)
			{
				string[] array = new string[this.m_TransformMask.arraySize];
				SerializedProperty arrayElementAtIndex = this.m_TransformMask.GetArrayElementAtIndex(0);
				arrayElementAtIndex.Next(false);
				for (int i = 1; i < this.m_NodeInfos.Length; i++)
				{
					this.m_NodeInfos[i].m_Path = arrayElementAtIndex.FindPropertyRelative("m_Path");
					this.m_NodeInfos[i].m_Weight = arrayElementAtIndex.FindPropertyRelative("m_Weight");
					array[i] = this.m_NodeInfos[i].m_Path.stringValue;
					string fullPath = array[i];
					if (this.humanTransforms != null)
					{
						this.m_NodeInfos[i].m_Enabled = (ArrayUtility.FindIndex<string>(this.humanTransforms, (string s) => fullPath == s) == -1);
					}
					else
					{
						this.m_NodeInfos[i].m_Enabled = true;
					}
					this.m_NodeInfos[i].m_Expanded = true;
					this.m_NodeInfos[i].m_ParentIndex = -1;
					this.m_NodeInfos[i].m_ChildIndices = new List<int>();
					AvatarMaskInspector.NodeInfo[] arg_181_0_cp_0 = this.m_NodeInfos;
					int arg_181_0_cp_1 = i;
					int arg_181_1;
					if (i == 0)
					{
						arg_181_1 = 0;
					}
					else
					{
						arg_181_1 = fullPath.Count((char f) => f == '/');
					}
					arg_181_0_cp_0[arg_181_0_cp_1].m_Depth = arg_181_1;
					string text = "";
					int num = fullPath.LastIndexOf('/');
					if (num > 0)
					{
						text = fullPath.Substring(0, num);
					}
					num = ((num != -1) ? (num + 1) : 0);
					this.m_NodeInfos[i].m_Name = fullPath.Substring(num);
					for (int j = 1; j < i; j++)
					{
						string a = array[j];
						if (text != "" && a == text)
						{
							this.m_NodeInfos[i].m_ParentIndex = j;
							this.m_NodeInfos[j].m_ChildIndices.Add(i);
						}
					}
					arrayElementAtIndex.Next(false);
				}
			}
		}

		private void ComputeShownElements()
		{
			for (int i = 0; i < this.m_NodeInfos.Length; i++)
			{
				if (this.m_NodeInfos[i].m_ParentIndex == -1)
				{
					this.ComputeShownElements(i, true);
				}
			}
		}

		private void ComputeShownElements(int currentIndex, bool show)
		{
			this.m_NodeInfos[currentIndex].m_Show = show;
			bool show2 = show && this.m_NodeInfos[currentIndex].m_Expanded;
			foreach (int current in this.m_NodeInfos[currentIndex].m_ChildIndices)
			{
				this.ComputeShownElements(current, show2);
			}
		}

		private void CheckChildren(int index, bool value)
		{
			foreach (int current in this.m_NodeInfos[index].m_ChildIndices)
			{
				if (this.m_NodeInfos[current].m_Enabled)
				{
					this.m_NodeInfos[current].m_Weight.floatValue = ((!value) ? 0f : 1f);
				}
				this.CheckChildren(current, value);
			}
		}
	}
}
