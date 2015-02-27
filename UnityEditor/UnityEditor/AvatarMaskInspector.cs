using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(AvatarMask))]
	internal class AvatarMaskInspector : Editor
	{
		private class Styles
		{
			public GUIContent MaskDefinition = EditorGUIUtility.TextContent("AvatarMaskEditor.MaskDefinition");
			public GUIContent[] MaskDefinitionOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("AvatarMaskEditor.CreateFromThisModel"),
				EditorGUIUtility.TextContent("AvatarMaskEditor.CopyFromOther")
			};
			public GUIContent BodyMask = EditorGUIUtility.TextContent("AvatarMaskEditor.BodyMask");
			public GUIContent TransformMask = EditorGUIUtility.TextContent("AvatarMaskEditor.TransformMask");
		}
		private struct NodeInfo
		{
			public bool m_Expanded;
			public bool m_Show;
			public bool m_Enabled;
			public int m_ParentIndex;
			public List<int> m_ChildIndices;
			public int m_Depth;
		}
		private static AvatarMaskInspector.Styles styles = new AvatarMaskInspector.Styles();
		protected bool[] m_BodyPartToggle;
		private bool m_ShowBodyMask = true;
		private bool m_BodyMaskFoldout;
		private bool m_CanImport = true;
		private SerializedProperty m_AnimationType;
		private AnimationClipInfoProperties m_ClipInfo;
		private AvatarMaskInspector.NodeInfo[] m_NodeInfos;
		private Avatar m_RefAvatar;
		private ModelImporter m_RefImporter;
		private bool m_TransformMaskFoldout;
		private string[] m_HumanTransform;
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
					this.m_ClipInfo.MaskFromClip(this.target as AvatarMask);
					SerializedObject serializedObject = this.m_ClipInfo.maskTypeProperty.serializedObject;
					this.m_AnimationType = serializedObject.FindProperty("m_AnimationType");
				}
				else
				{
					this.m_AnimationType = null;
				}
			}
		}
		private ModelImporterAnimationType animationType
		{
			get
			{
				if (this.m_AnimationType != null)
				{
					return (ModelImporterAnimationType)this.m_AnimationType.intValue;
				}
				return ModelImporterAnimationType.None;
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
		private void OnEnable()
		{
			AvatarMask avatarMask = this.target as AvatarMask;
			this.m_BodyPartToggle = new bool[avatarMask.humanoidBodyPartCount];
		}
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			bool flag = false;
			if (this.clipInfo != null)
			{
				EditorGUI.BeginChangeCheck();
				int num = (int)this.clipInfo.maskType;
				EditorGUI.showMixedValue = this.clipInfo.maskTypeProperty.hasMultipleDifferentValues;
				num = EditorGUILayout.Popup(AvatarMaskInspector.styles.MaskDefinition, num, AvatarMaskInspector.styles.MaskDefinitionOpt, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					this.clipInfo.maskType = (ClipAnimationMaskType)num;
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
			this.OnBodyInspectorGUI();
			this.OnTransformInspectorGUI();
			GUI.enabled = enabled;
			if (EditorGUI.EndChangeCheck() && this.clipInfo != null)
			{
				this.clipInfo.MaskToClip(this.target as AvatarMask);
			}
		}
		protected void CopyFromOtherGUI()
		{
			if (this.clipInfo == null)
			{
				return;
			}
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
		private void UpdateMask(ClipAnimationMaskType maskType)
		{
			if (this.clipInfo == null)
			{
				return;
			}
			if (maskType == ClipAnimationMaskType.CreateFromThisModel)
			{
				SerializedObject serializedObject = this.clipInfo.maskTypeProperty.serializedObject;
				ModelImporter modelImporter = serializedObject.targetObject as ModelImporter;
				AvatarMask mask = this.target as AvatarMask;
				AvatarMaskUtility.UpdateTransformMask(mask, modelImporter.transformPaths, this.humanTransforms);
				this.FillNodeInfos(mask);
			}
			else
			{
				if (maskType == ClipAnimationMaskType.CopyFromOther)
				{
					AvatarMask avatarMask = this.clipInfo.maskSourceProperty.objectReferenceValue as AvatarMask;
					if (avatarMask != null)
					{
						AvatarMask avatarMask2 = this.target as AvatarMask;
						avatarMask2.Copy(avatarMask);
						this.FillNodeInfos(avatarMask2);
					}
				}
			}
		}
		public void OnBodyInspectorGUI()
		{
			if (this.m_ShowBodyMask)
			{
				bool changed = GUI.changed;
				this.m_BodyMaskFoldout = EditorGUILayout.Foldout(this.m_BodyMaskFoldout, AvatarMaskInspector.styles.BodyMask);
				GUI.changed = changed;
				if (this.m_BodyMaskFoldout)
				{
					AvatarMask avatarMask = this.target as AvatarMask;
					for (int i = 0; i < avatarMask.humanoidBodyPartCount; i++)
					{
						this.m_BodyPartToggle[i] = avatarMask.GetHumanoidBodyPartActive(i);
					}
					this.m_BodyPartToggle = BodyMaskEditor.Show(this.m_BodyPartToggle, avatarMask.humanoidBodyPartCount);
					bool flag = false;
					for (int j = 0; j < avatarMask.humanoidBodyPartCount; j++)
					{
						flag |= (avatarMask.GetHumanoidBodyPartActive(j) != this.m_BodyPartToggle[j]);
					}
					if (flag)
					{
						Undo.RegisterCompleteObjectUndo(avatarMask, "Body Mask Edit");
						for (int k = 0; k < avatarMask.humanoidBodyPartCount; k++)
						{
							avatarMask.SetHumanoidBodyPartActive(k, this.m_BodyPartToggle[k]);
						}
						EditorUtility.SetDirty(avatarMask);
					}
				}
			}
		}
		public void OnTransformInspectorGUI()
		{
			AvatarMask avatarMask = this.target as AvatarMask;
			float left = 0f;
			float top = 0f;
			float num = 0f;
			float bottom = 0f;
			bool changed = GUI.changed;
			this.m_TransformMaskFoldout = EditorGUILayout.Foldout(this.m_TransformMaskFoldout, AvatarMaskInspector.styles.TransformMask);
			GUI.changed = changed;
			if (this.m_TransformMaskFoldout)
			{
				if (this.canImport)
				{
					this.ImportAvatarReference();
				}
				if (this.m_NodeInfos == null || avatarMask.transformCount != this.m_NodeInfos.Length)
				{
					this.FillNodeInfos(avatarMask);
				}
				this.ComputeShownElements();
				GUILayout.Space(1f);
				int indentLevel = EditorGUI.indentLevel;
				int transformCount = avatarMask.transformCount;
				for (int i = 1; i < transformCount; i++)
				{
					if (this.m_NodeInfos[i].m_Show)
					{
						string transformPath = avatarMask.GetTransformPath(i);
						string[] array = transformPath.Split(new char[]
						{
							'/'
						});
						string text = array[array.Length - 1];
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
						avatarMask.SetTransformActive(i, GUI.Toggle(rect, avatarMask.GetTransformActive(i), string.Empty));
						GUI.enabled = enabled;
						if (EditorGUI.EndChangeCheck() && !flag)
						{
							this.CheckChildren(avatarMask, i, avatarMask.GetTransformActive(i));
						}
						if (this.m_NodeInfos[i].m_ChildIndices.Count > 0)
						{
							this.m_NodeInfos[i].m_Expanded = EditorGUILayout.Foldout(this.m_NodeInfos[i].m_Expanded, text);
						}
						else
						{
							EditorGUILayout.LabelField(text, new GUILayoutOption[0]);
						}
						if (i == 1)
						{
							top = rect.yMin;
							left = rect.xMin;
						}
						else
						{
							if (i == transformCount - 1)
							{
								bottom = rect.yMax;
							}
						}
						num = Mathf.Max(num, GUILayoutUtility.GetLastRect().xMax);
						GUILayout.EndHorizontal();
					}
				}
				EditorGUI.indentLevel = indentLevel;
			}
			Rect rect2 = Rect.MinMaxRect(left, top, num, bottom);
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
					(this.target as AvatarMask).SetTransformActive(i, active);
				}
			}
			if (this.clipInfo != null)
			{
				this.clipInfo.MaskToClip(this.target as AvatarMask);
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
				AvatarMaskUtility.UpdateTransformMask(this.target as AvatarMask, this.m_RefImporter.transformPaths, null);
			}
		}
		private void FillNodeInfos(AvatarMask mask)
		{
			this.m_NodeInfos = new AvatarMaskInspector.NodeInfo[mask.transformCount];
			for (int i = 1; i < this.m_NodeInfos.Length; i++)
			{
				string fullPath = mask.GetTransformPath(i);
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
				AvatarMaskInspector.NodeInfo[] arg_F6_0_cp_0 = this.m_NodeInfos;
				int arg_F6_0_cp_1 = i;
				int arg_F6_1;
				if (i == 0)
				{
					arg_F6_1 = 0;
				}
				else
				{
					arg_F6_1 = fullPath.Count((char f) => f == '/');
				}
				arg_F6_0_cp_0[arg_F6_0_cp_1].m_Depth = arg_F6_1;
				string text = string.Empty;
				int num = fullPath.LastIndexOf('/');
				if (num > 0)
				{
					text = fullPath.Substring(0, num);
				}
				int transformCount = mask.transformCount;
				for (int j = 0; j < transformCount; j++)
				{
					string transformPath = mask.GetTransformPath(j);
					if (text != string.Empty && transformPath == text)
					{
						this.m_NodeInfos[i].m_ParentIndex = j;
					}
					if (transformPath.StartsWith(fullPath))
					{
						if (transformPath.Count((char f) => f == '/') == this.m_NodeInfos[i].m_Depth + 1)
						{
							this.m_NodeInfos[i].m_ChildIndices.Add(j);
						}
					}
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
		private void CheckChildren(AvatarMask mask, int index, bool value)
		{
			foreach (int current in this.m_NodeInfos[index].m_ChildIndices)
			{
				if (this.m_NodeInfos[current].m_Enabled)
				{
					mask.SetTransformActive(current, value);
				}
				this.CheckChildren(mask, current, value);
			}
		}
	}
}
