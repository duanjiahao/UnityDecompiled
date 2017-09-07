using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal static class AvatarSetupTool
	{
		[Serializable]
		internal class BoneWrapper
		{
			private static string sHumanName = "m_HumanName";

			private static string sBoneName = "m_BoneName";

			private string m_HumanBoneName;

			public string error = string.Empty;

			public Transform bone;

			public BoneState state;

			public const int kIconSize = 19;

			private static Color kBoneValid = new Color(0f, 0.75f, 0f, 1f);

			private static Color kBoneInvalid = new Color(1f, 0.3f, 0.25f, 1f);

			private static Color kBoneInactive = Color.gray;

			private static Color kBoneSelected = new Color(0.4f, 0.7f, 1f, 1f);

			private static Color kBoneDrop = new Color(0.1f, 0.7f, 1f, 1f);

			public string humanBoneName
			{
				get
				{
					return this.m_HumanBoneName;
				}
			}

			public string messageName
			{
				get
				{
					return ObjectNames.NicifyVariableName(this.m_HumanBoneName) + " Transform '" + ((!this.bone) ? "None" : this.bone.name) + "'";
				}
			}

			public BoneWrapper(string humanBoneName, SerializedObject serializedObject, Dictionary<Transform, bool> bones)
			{
				this.m_HumanBoneName = humanBoneName;
				this.Reset(serializedObject, bones);
			}

			public void Reset(SerializedObject serializedObject, Dictionary<Transform, bool> bones)
			{
				this.bone = null;
				SerializedProperty serializedProperty = this.GetSerializedProperty(serializedObject, false);
				if (serializedProperty != null)
				{
					string boneName = serializedProperty.FindPropertyRelative(AvatarSetupTool.BoneWrapper.sBoneName).stringValue;
					this.bone = bones.Keys.FirstOrDefault((Transform b) => b != null && b.name == boneName);
				}
				this.state = BoneState.Valid;
			}

			public void Serialize(SerializedObject serializedObject)
			{
				if (this.bone == null)
				{
					this.DeleteSerializedProperty(serializedObject);
				}
				else
				{
					SerializedProperty serializedProperty = this.GetSerializedProperty(serializedObject, true);
					if (serializedProperty != null)
					{
						serializedProperty.FindPropertyRelative(AvatarSetupTool.BoneWrapper.sBoneName).stringValue = this.bone.name;
					}
				}
			}

			protected void DeleteSerializedProperty(SerializedObject serializedObject)
			{
				SerializedProperty serializedProperty = serializedObject.FindProperty(AvatarSetupTool.sHuman);
				if (serializedProperty != null && serializedProperty.isArray)
				{
					for (int i = 0; i < serializedProperty.arraySize; i++)
					{
						SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative(AvatarSetupTool.BoneWrapper.sHumanName);
						if (serializedProperty2.stringValue == this.humanBoneName)
						{
							serializedProperty.DeleteArrayElementAtIndex(i);
							break;
						}
					}
				}
			}

			public SerializedProperty GetSerializedProperty(SerializedObject serializedObject, bool createIfMissing)
			{
				SerializedProperty serializedProperty = serializedObject.FindProperty(AvatarSetupTool.sHuman);
				SerializedProperty result;
				if (serializedProperty == null || !serializedProperty.isArray)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < serializedProperty.arraySize; i++)
					{
						SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative(AvatarSetupTool.BoneWrapper.sHumanName);
						if (serializedProperty2.stringValue == this.humanBoneName)
						{
							result = serializedProperty.GetArrayElementAtIndex(i);
							return result;
						}
					}
					if (createIfMissing)
					{
						serializedProperty.arraySize++;
						SerializedProperty arrayElementAtIndex = serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1);
						if (arrayElementAtIndex != null)
						{
							arrayElementAtIndex.FindPropertyRelative(AvatarSetupTool.BoneWrapper.sHumanName).stringValue = this.humanBoneName;
							result = arrayElementAtIndex;
							return result;
						}
					}
					result = null;
				}
				return result;
			}

			public void BoneDotGUI(Rect rect, Rect selectRect, int boneIndex, bool doClickSelect, bool doDragDrop, bool doDeleteKey, SerializedObject serializedObject, AvatarMappingEditor editor)
			{
				int controlID = GUIUtility.GetControlID(FocusType.Passive, rect);
				int controlID2 = GUIUtility.GetControlID(FocusType.Keyboard, selectRect);
				if (doClickSelect)
				{
					this.HandleClickSelection(controlID2, selectRect, boneIndex);
				}
				if (doDeleteKey)
				{
					this.HandleDeleteSelection(controlID2, serializedObject, editor);
				}
				if (doDragDrop)
				{
					this.HandleDragDrop(rect, boneIndex, controlID, serializedObject, editor);
				}
				Color color = GUI.color;
				if (AvatarMappingEditor.s_SelectedBoneIndex == boneIndex)
				{
					GUI.color = AvatarSetupTool.BoneWrapper.kBoneSelected;
					GUI.DrawTexture(rect, AvatarMappingEditor.styles.dotSelection.image);
				}
				if (DragAndDrop.activeControlID == controlID)
				{
					GUI.color = AvatarSetupTool.BoneWrapper.kBoneDrop;
				}
				else if (this.state == BoneState.Valid)
				{
					GUI.color = AvatarSetupTool.BoneWrapper.kBoneValid;
				}
				else if (this.state == BoneState.None)
				{
					GUI.color = AvatarSetupTool.BoneWrapper.kBoneInactive;
				}
				else
				{
					GUI.color = AvatarSetupTool.BoneWrapper.kBoneInvalid;
				}
				Texture image;
				if (HumanTrait.RequiredBone(boneIndex))
				{
					image = AvatarMappingEditor.styles.dotFrame.image;
				}
				else
				{
					image = AvatarMappingEditor.styles.dotFrameDotted.image;
				}
				GUI.DrawTexture(rect, image);
				if (this.bone != null || DragAndDrop.activeControlID == controlID)
				{
					GUI.DrawTexture(rect, AvatarMappingEditor.styles.dotFill.image);
				}
				GUI.color = color;
			}

			public void HandleClickSelection(int keyboardID, Rect selectRect, int boneIndex)
			{
				Event current = Event.current;
				if (current.type == EventType.MouseDown && selectRect.Contains(current.mousePosition))
				{
					AvatarMappingEditor.s_SelectedBoneIndex = boneIndex;
					AvatarMappingEditor.s_DirtySelection = true;
					AvatarMappingEditor.s_KeyboardControl = keyboardID;
					Selection.activeTransform = this.bone;
					if (this.bone != null)
					{
						EditorGUIUtility.PingObject(this.bone);
					}
					current.Use();
				}
			}

			public void HandleDeleteSelection(int keyboardID, SerializedObject serializedObject, AvatarMappingEditor editor)
			{
				Event current = Event.current;
				if (current.type == EventType.KeyDown)
				{
					if (GUIUtility.keyboardControl == keyboardID)
					{
						if (current.keyCode == KeyCode.Backspace || current.keyCode == KeyCode.Delete)
						{
							Undo.RegisterCompleteObjectUndo(editor, "Avatar mapping modified");
							this.bone = null;
							this.state = BoneState.None;
							this.Serialize(serializedObject);
							Selection.activeTransform = null;
							GUI.changed = true;
							current.Use();
						}
					}
				}
			}

			private void HandleDragDrop(Rect dropRect, int boneIndex, int id, SerializedObject serializedObject, AvatarMappingEditor editor)
			{
				EventType type = Event.current.type;
				if (type != EventType.DragExited)
				{
					if (type == EventType.DragUpdated || type == EventType.DragPerform)
					{
						if (dropRect.Contains(Event.current.mousePosition) && GUI.enabled)
						{
							UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
							UnityEngine.Object @object = (objectReferences.Length != 1) ? null : objectReferences[0];
							if (@object != null)
							{
								if ((!(@object is Transform) && !(@object is GameObject)) || EditorUtility.IsPersistent(@object))
								{
									@object = null;
								}
							}
							if (@object != null)
							{
								DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
								if (type == EventType.DragPerform)
								{
									Undo.RegisterCompleteObjectUndo(editor, "Avatar mapping modified");
									if (@object is GameObject)
									{
										this.bone = (@object as GameObject).transform;
									}
									else
									{
										this.bone = (@object as Transform);
									}
									this.Serialize(serializedObject);
									GUI.changed = true;
									DragAndDrop.AcceptDrag();
									DragAndDrop.activeControlID = 0;
								}
								else
								{
									DragAndDrop.activeControlID = id;
								}
								Event.current.Use();
							}
						}
					}
				}
				else if (GUI.enabled)
				{
					HandleUtility.Repaint();
				}
			}
		}

		private class BonePoseData
		{
			public Vector3 direction = Vector3.zero;

			public bool compareInGlobalSpace = false;

			public float maxAngle;

			public int[] childIndices = null;

			public Vector3 planeNormal = Vector3.zero;

			public BonePoseData(Vector3 dir, bool globalSpace, float maxAngleDiff)
			{
				this.direction = ((!(dir == Vector3.zero)) ? dir.normalized : dir);
				this.compareInGlobalSpace = globalSpace;
				this.maxAngle = maxAngleDiff;
			}

			public BonePoseData(Vector3 dir, bool globalSpace, float maxAngleDiff, int[] children) : this(dir, globalSpace, maxAngleDiff)
			{
				this.childIndices = children;
			}

			public BonePoseData(Vector3 dir, bool globalSpace, float maxAngleDiff, Vector3 planeNormal, int[] children) : this(dir, globalSpace, maxAngleDiff, children)
			{
				this.planeNormal = planeNormal;
			}
		}

		private class SkinTransformHierarchySorter : IComparer<SkinnedMeshRenderer>
		{
			public int Compare(SkinnedMeshRenderer skinA, SkinnedMeshRenderer skinB)
			{
				Transform transform = skinA.transform;
				Transform transform2 = skinB.transform;
				int result;
				while (!(transform == null))
				{
					if (transform2 == null)
					{
						result = 1;
						return result;
					}
					transform = transform.parent;
					transform2 = transform2.parent;
				}
				if (transform2 == null)
				{
					result = 0;
					return result;
				}
				result = -1;
				return result;
			}
		}

		private static string sHuman = "m_HumanDescription.m_Human";

		private static string sHasTranslationDoF = "m_HumanDescription.m_HasTranslationDoF";

		internal static string sSkeleton = "m_HumanDescription.m_Skeleton";

		internal static string sName = "m_Name";

		internal static string sParentName = "m_ParentName";

		internal static string sPosition = "m_Position";

		internal static string sRotation = "m_Rotation";

		internal static string sScale = "m_Scale";

		private static AvatarSetupTool.BonePoseData[] sBonePoses = new AvatarSetupTool.BonePoseData[]
		{
			new AvatarSetupTool.BonePoseData(Vector3.up, true, 15f),
			new AvatarSetupTool.BonePoseData(new Vector3(-0.05f, -1f, 0f), true, 15f),
			new AvatarSetupTool.BonePoseData(new Vector3(0.05f, -1f, 0f), true, 15f),
			new AvatarSetupTool.BonePoseData(new Vector3(-0.05f, -1f, -0.15f), true, 20f),
			new AvatarSetupTool.BonePoseData(new Vector3(0.05f, -1f, -0.15f), true, 20f),
			new AvatarSetupTool.BonePoseData(new Vector3(-0.05f, 0f, 1f), true, 20f, Vector3.up, null),
			new AvatarSetupTool.BonePoseData(new Vector3(0.05f, 0f, 1f), true, 20f, Vector3.up, null),
			new AvatarSetupTool.BonePoseData(Vector3.up, true, 30f, new int[]
			{
				8,
				54,
				9,
				10
			}),
			new AvatarSetupTool.BonePoseData(Vector3.up, true, 30f, new int[]
			{
				54,
				9,
				10
			}),
			new AvatarSetupTool.BonePoseData(Vector3.up, true, 30f),
			null,
			new AvatarSetupTool.BonePoseData(-Vector3.right, true, 20f),
			new AvatarSetupTool.BonePoseData(Vector3.right, true, 20f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, true, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, true, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, true, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, true, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 10f, Vector3.forward, new int[]
			{
				30
			}),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 10f, Vector3.forward, new int[]
			{
				45
			}),
			null,
			null,
			null,
			null,
			null,
			new AvatarSetupTool.BonePoseData(new Vector3(-1f, 0f, 1f), false, 10f),
			new AvatarSetupTool.BonePoseData(new Vector3(-1f, 0f, 1f), false, 5f),
			new AvatarSetupTool.BonePoseData(new Vector3(-1f, 0f, 1f), false, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 10f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 10f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 10f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 10f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(-Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(new Vector3(1f, 0f, 1f), false, 10f),
			new AvatarSetupTool.BonePoseData(new Vector3(1f, 0f, 1f), false, 5f),
			new AvatarSetupTool.BonePoseData(new Vector3(1f, 0f, 1f), false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 10f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 10f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 10f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 10f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.right, false, 5f),
			new AvatarSetupTool.BonePoseData(Vector3.up, true, 30f, new int[]
			{
				9,
				10
			})
		};

		public static Dictionary<Transform, bool> GetModelBones(Transform root, bool includeAll, AvatarSetupTool.BoneWrapper[] humanBones)
		{
			Dictionary<Transform, bool> result;
			if (root == null)
			{
				result = null;
			}
			else
			{
				Dictionary<Transform, bool> dictionary = new Dictionary<Transform, bool>();
				List<Transform> list = new List<Transform>();
				if (!includeAll)
				{
					SkinnedMeshRenderer[] componentsInChildren = root.GetComponentsInChildren<SkinnedMeshRenderer>();
					SkinnedMeshRenderer[] array = componentsInChildren;
					for (int i = 0; i < array.Length; i++)
					{
						SkinnedMeshRenderer skinnedMeshRenderer = array[i];
						Transform[] bones = skinnedMeshRenderer.bones;
						bool[] array2 = new bool[bones.Length];
						BoneWeight[] boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
						BoneWeight[] array3 = boneWeights;
						for (int j = 0; j < array3.Length; j++)
						{
							BoneWeight boneWeight = array3[j];
							if (boneWeight.weight0 != 0f)
							{
								array2[boneWeight.boneIndex0] = true;
							}
							if (boneWeight.weight1 != 0f)
							{
								array2[boneWeight.boneIndex1] = true;
							}
							if (boneWeight.weight2 != 0f)
							{
								array2[boneWeight.boneIndex2] = true;
							}
							if (boneWeight.weight3 != 0f)
							{
								array2[boneWeight.boneIndex3] = true;
							}
						}
						for (int k = 0; k < bones.Length; k++)
						{
							if (array2[k] && !list.Contains(bones[k]))
							{
								list.Add(bones[k]);
							}
						}
					}
					AvatarSetupTool.DetermineIsActualBone(root, dictionary, list, false, humanBones);
				}
				if (dictionary.Count < HumanTrait.RequiredBoneCount)
				{
					dictionary.Clear();
					list.Clear();
					AvatarSetupTool.DetermineIsActualBone(root, dictionary, list, true, humanBones);
				}
				result = dictionary;
			}
			return result;
		}

		private static bool DetermineIsActualBone(Transform tr, Dictionary<Transform, bool> bones, List<Transform> skinnedBones, bool includeAll, AvatarSetupTool.BoneWrapper[] humanBones)
		{
			bool flag = includeAll;
			bool flag2 = false;
			bool flag3 = false;
			int num = 0;
			IEnumerator enumerator = tr.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform tr2 = (Transform)enumerator.Current;
					if (AvatarSetupTool.DetermineIsActualBone(tr2, bones, skinnedBones, includeAll, humanBones))
					{
						num++;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			if (num > 0)
			{
				flag2 = true;
			}
			if (num > 1)
			{
				flag = true;
			}
			if (!flag && skinnedBones.Contains(tr))
			{
				flag = true;
			}
			if (!flag)
			{
				Component[] components = tr.GetComponents<Component>();
				if (components.Length > 1)
				{
					Component[] array = components;
					for (int i = 0; i < array.Length; i++)
					{
						Component component = array[i];
						if (component is Renderer && !(component is SkinnedMeshRenderer))
						{
							Bounds bounds = (component as Renderer).bounds;
							bounds.extents = bounds.size;
							if (tr.childCount == 0 && tr.parent && bounds.Contains(tr.parent.position))
							{
								if (tr.parent.GetComponent<Renderer>() != null)
								{
									flag = true;
								}
								else
								{
									flag3 = true;
								}
							}
							else if (bounds.Contains(tr.position))
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!flag && humanBones != null)
			{
				for (int j = 0; j < humanBones.Length; j++)
				{
					AvatarSetupTool.BoneWrapper boneWrapper = humanBones[j];
					if (tr == boneWrapper.bone)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				bones[tr] = true;
			}
			else if (flag2)
			{
				if (!bones.ContainsKey(tr))
				{
					bones[tr] = false;
				}
			}
			else if (flag3)
			{
				bones[tr.parent] = true;
			}
			return bones.ContainsKey(tr);
		}

		public static int GetFirstHumanBoneAncestor(AvatarSetupTool.BoneWrapper[] bones, int boneIndex)
		{
			boneIndex = HumanTrait.GetParentBone(boneIndex);
			while (boneIndex > 0 && bones[boneIndex].bone == null)
			{
				boneIndex = HumanTrait.GetParentBone(boneIndex);
			}
			return boneIndex;
		}

		public static int GetHumanBoneChild(AvatarSetupTool.BoneWrapper[] bones, int boneIndex)
		{
			int result;
			for (int i = 0; i < HumanTrait.BoneCount; i++)
			{
				if (HumanTrait.GetParentBone(i) == boneIndex)
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		public static AvatarSetupTool.BoneWrapper[] GetHumanBones(SerializedObject serializedObject, Dictionary<Transform, bool> actualBones)
		{
			string[] boneName = HumanTrait.BoneName;
			AvatarSetupTool.BoneWrapper[] array = new AvatarSetupTool.BoneWrapper[boneName.Length];
			for (int i = 0; i < boneName.Length; i++)
			{
				array[i] = new AvatarSetupTool.BoneWrapper(boneName[i], serializedObject, actualBones);
			}
			return array;
		}

		public static void ClearAll(SerializedObject serializedObject)
		{
			AvatarSetupTool.ClearHumanBoneArray(serializedObject);
			AvatarSetupTool.ClearSkeletonBoneArray(serializedObject);
		}

		public static void ClearHumanBoneArray(SerializedObject serializedObject)
		{
			SerializedProperty serializedProperty = serializedObject.FindProperty(AvatarSetupTool.sHuman);
			if (serializedProperty != null && serializedProperty.isArray)
			{
				serializedProperty.ClearArray();
			}
		}

		public static void ClearSkeletonBoneArray(SerializedObject serializedObject)
		{
			SerializedProperty serializedProperty = serializedObject.FindProperty(AvatarSetupTool.sSkeleton);
			if (serializedProperty != null && serializedProperty.isArray)
			{
				serializedProperty.ClearArray();
			}
		}

		public static void AutoSetupOnInstance(GameObject modelPrefab, SerializedObject modelImporterSerializedObject)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(modelPrefab);
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			AvatarSetupTool.AutoSetup(modelPrefab, gameObject, modelImporterSerializedObject);
			UnityEngine.Object.DestroyImmediate(gameObject);
		}

		public static bool IsPoseValidOnInstance(GameObject modelPrefab, SerializedObject modelImporterSerializedObject)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(modelPrefab);
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			Dictionary<Transform, bool> modelBones = AvatarSetupTool.GetModelBones(gameObject.transform, false, null);
			AvatarSetupTool.BoneWrapper[] humanBones = AvatarSetupTool.GetHumanBones(modelImporterSerializedObject, modelBones);
			AvatarSetupTool.TransferDescriptionToPose(modelImporterSerializedObject, gameObject.transform);
			bool result = AvatarSetupTool.IsPoseValid(humanBones);
			UnityEngine.Object.DestroyImmediate(gameObject);
			return result;
		}

		public static void AutoSetup(GameObject modelPrefab, GameObject modelInstance, SerializedObject modelImporterSerializedObject)
		{
			SerializedProperty serializedProperty = modelImporterSerializedObject.FindProperty(AvatarSetupTool.sHuman);
			SerializedProperty serializedProperty2 = modelImporterSerializedObject.FindProperty(AvatarSetupTool.sHasTranslationDoF);
			if (serializedProperty != null && serializedProperty.isArray)
			{
				AvatarSetupTool.ClearHumanBoneArray(modelImporterSerializedObject);
				bool flag = AvatarBipedMapper.IsBiped(modelInstance.transform, null);
				AvatarSetupTool.SampleBindPose(modelInstance);
				Dictionary<Transform, bool> modelBones = AvatarSetupTool.GetModelBones(modelInstance.transform, false, null);
				Dictionary<int, Transform> dictionary;
				if (flag)
				{
					dictionary = AvatarBipedMapper.MapBones(modelInstance.transform);
				}
				else
				{
					dictionary = AvatarAutoMapper.MapBones(modelInstance.transform, modelBones);
				}
				AvatarSetupTool.BoneWrapper[] humanBones = AvatarSetupTool.GetHumanBones(modelImporterSerializedObject, modelBones);
				for (int i = 0; i < humanBones.Length; i++)
				{
					AvatarSetupTool.BoneWrapper boneWrapper = humanBones[i];
					if (dictionary.ContainsKey(i))
					{
						boneWrapper.bone = dictionary[i];
					}
					else
					{
						boneWrapper.bone = null;
					}
					boneWrapper.Serialize(modelImporterSerializedObject);
				}
				if (!flag)
				{
					float poseError = AvatarSetupTool.GetPoseError(humanBones);
					AvatarSetupTool.CopyPose(modelInstance, modelPrefab);
					float poseError2 = AvatarSetupTool.GetPoseError(humanBones);
					if (poseError < poseError2)
					{
						AvatarSetupTool.SampleBindPose(modelInstance);
					}
					AvatarSetupTool.MakePoseValid(humanBones);
				}
				else
				{
					AvatarBipedMapper.BipedPose(modelInstance, humanBones);
					serializedProperty2.boolValue = true;
				}
				AvatarSetupTool.TransferPoseToDescription(modelImporterSerializedObject, modelInstance.transform);
			}
		}

		public static bool TestAndValidateAutoSetup(GameObject modelAsset)
		{
			bool result;
			if (modelAsset == null)
			{
				Debug.LogError("GameObject is null");
				result = false;
			}
			else if (PrefabUtility.GetPrefabType(modelAsset) != PrefabType.ModelPrefab)
			{
				Debug.LogError(modelAsset.name + ": GameObject is not a ModelPrefab", modelAsset);
				result = false;
			}
			else if (modelAsset.transform.parent != null)
			{
				Debug.LogError(modelAsset.name + ": GameObject is not the root", modelAsset);
				result = false;
			}
			else
			{
				string assetPath = AssetDatabase.GetAssetPath(modelAsset);
				ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
				if (modelImporter == null)
				{
					Debug.LogError(modelAsset.name + ": Could not load ModelImporter (path:" + assetPath + ")", modelAsset);
					result = false;
				}
				else
				{
					SerializedObject serializedObject = new SerializedObject(modelImporter);
					SerializedProperty serializedProperty = serializedObject.FindProperty("m_AnimationType");
					if (serializedProperty == null)
					{
						Debug.LogError(modelAsset.name + ": Could not find property m_AnimationType on ModelImporter", modelAsset);
						result = false;
					}
					else
					{
						serializedProperty.intValue = 2;
						AvatarSetupTool.ClearAll(serializedObject);
						serializedObject.ApplyModifiedProperties();
						AssetDatabase.ImportAsset(assetPath);
						serializedProperty.intValue = 3;
						AvatarSetupTool.AutoSetupOnInstance(modelAsset, serializedObject);
						serializedObject.ApplyModifiedProperties();
						AssetDatabase.ImportAsset(assetPath);
						Avatar avatar = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Avatar)) as Avatar;
						if (avatar == null)
						{
							Debug.LogError(modelAsset.name + ": Did not find Avatar after reimport with CreateAvatar enabled", modelAsset);
							result = false;
						}
						else if (!avatar.isHuman)
						{
							Debug.LogError(modelAsset.name + ": Avatar is not valid after reimport", modelAsset);
							result = false;
						}
						else if (!AvatarSetupTool.IsPoseValidOnInstance(modelAsset, serializedObject))
						{
							Debug.LogError(modelAsset.name + ": Avatar has invalid pose after reimport", modelAsset);
							result = false;
						}
						else
						{
							string str = assetPath.Substring(0, assetPath.Length - Path.GetExtension(assetPath).Length);
							string text = str + ".ht";
							HumanTemplate humanTemplate = AssetDatabase.LoadMainAssetAtPath(text) as HumanTemplate;
							if (humanTemplate == null)
							{
								Debug.LogWarning(modelAsset.name + ": Didn't find template at path " + text);
							}
							else
							{
								List<string> list = null;
								string path = str + ".ignore";
								if (File.Exists(path))
								{
									list = new List<string>(File.ReadAllLines(path));
								}
								GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(modelAsset);
								gameObject.hideFlags = HideFlags.HideAndDontSave;
								Dictionary<Transform, bool> modelBones = AvatarSetupTool.GetModelBones(gameObject.transform, false, null);
								AvatarSetupTool.BoneWrapper[] humanBones = AvatarSetupTool.GetHumanBones(serializedObject, modelBones);
								bool flag = false;
								for (int i = 0; i < humanBones.Length; i++)
								{
									if (list == null || !list.Contains(humanBones[i].humanBoneName))
									{
										string text2 = humanTemplate.Find(humanBones[i].humanBoneName);
										string text3 = (!(humanBones[i].bone == null)) ? humanBones[i].bone.name : "";
										if (!AvatarMappingEditor.MatchName(text3, text2))
										{
											flag = true;
											Debug.LogError(string.Concat(new string[]
											{
												modelAsset.name,
												": Avatar has bone ",
												humanBones[i].humanBoneName,
												" mapped to \"",
												text3,
												"\" but expected \"",
												text2,
												"\""
											}), modelAsset);
										}
									}
								}
								UnityEngine.Object.DestroyImmediate(gameObject);
								if (flag)
								{
									result = false;
									return result;
								}
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		public static void DebugTransformTree(Transform tr, Dictionary<Transform, bool> bones, int level)
		{
			string str = "  ";
			if (bones.ContainsKey(tr))
			{
				if (bones[tr])
				{
					str = "* ";
				}
				else
				{
					str = ". ";
				}
			}
			Debug.Log("                                             ".Substring(0, level * 2) + str + tr.name + "\n\n");
			IEnumerator enumerator = tr.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform tr2 = (Transform)enumerator.Current;
					AvatarSetupTool.DebugTransformTree(tr2, bones, level + 1);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public static SerializedProperty FindSkeletonBone(SerializedObject serializedObject, Transform t, bool createMissing, bool isRoot)
		{
			SerializedProperty serializedProperty = serializedObject.FindProperty(AvatarSetupTool.sSkeleton);
			SerializedProperty result;
			if (serializedProperty == null || !serializedProperty.isArray)
			{
				result = null;
			}
			else
			{
				result = AvatarSetupTool.FindSkeletonBone(serializedProperty, t, createMissing, isRoot);
			}
			return result;
		}

		public static SerializedProperty FindSkeletonBone(SerializedProperty skeletonBoneArray, Transform t, bool createMissing, bool isRoot)
		{
			SerializedProperty result;
			if (isRoot && skeletonBoneArray.arraySize > 0)
			{
				SerializedProperty arrayElementAtIndex = skeletonBoneArray.GetArrayElementAtIndex(0);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative(AvatarSetupTool.sName);
				if (serializedProperty.stringValue == t.name)
				{
					result = arrayElementAtIndex;
					return result;
				}
			}
			else
			{
				for (int i = 1; i < skeletonBoneArray.arraySize; i++)
				{
					SerializedProperty arrayElementAtIndex2 = skeletonBoneArray.GetArrayElementAtIndex(i);
					SerializedProperty serializedProperty2 = arrayElementAtIndex2.FindPropertyRelative(AvatarSetupTool.sName);
					if (serializedProperty2.stringValue == t.name)
					{
						result = arrayElementAtIndex2;
						return result;
					}
				}
			}
			if (createMissing)
			{
				skeletonBoneArray.arraySize++;
				SerializedProperty arrayElementAtIndex3 = skeletonBoneArray.GetArrayElementAtIndex(skeletonBoneArray.arraySize - 1);
				if (arrayElementAtIndex3 != null)
				{
					arrayElementAtIndex3.FindPropertyRelative(AvatarSetupTool.sName).stringValue = t.name;
					arrayElementAtIndex3.FindPropertyRelative(AvatarSetupTool.sParentName).stringValue = ((!isRoot) ? t.parent.name : "");
					arrayElementAtIndex3.FindPropertyRelative(AvatarSetupTool.sPosition).vector3Value = t.localPosition;
					arrayElementAtIndex3.FindPropertyRelative(AvatarSetupTool.sRotation).quaternionValue = t.localRotation;
					arrayElementAtIndex3.FindPropertyRelative(AvatarSetupTool.sScale).vector3Value = t.localScale;
					result = arrayElementAtIndex3;
					return result;
				}
			}
			result = null;
			return result;
		}

		public static void CopyPose(GameObject go, GameObject source)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(source);
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			AnimatorUtility.DeoptimizeTransformHierarchy(gameObject);
			AvatarSetupTool.CopyPose(go.transform, gameObject.transform);
			UnityEngine.Object.DestroyImmediate(gameObject);
		}

		private static void CopyPose(Transform t, Transform source)
		{
			t.localPosition = source.localPosition;
			t.localRotation = source.localRotation;
			t.localScale = source.localScale;
			IEnumerator enumerator = t.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					Transform transform2 = source.Find(transform.name);
					if (transform2)
					{
						AvatarSetupTool.CopyPose(transform, transform2);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public static void GetBindPoseBonePositionRotation(Matrix4x4 skinMatrix, Matrix4x4 boneMatrix, Transform bone, out Vector3 position, out Quaternion rotation)
		{
			Matrix4x4 matrix4x = skinMatrix * boneMatrix.inverse;
			Vector3 lhs = new Vector3(matrix4x.m00, matrix4x.m10, matrix4x.m20);
			Vector3 vector = new Vector3(matrix4x.m01, matrix4x.m11, matrix4x.m21);
			Vector3 vector2 = new Vector3(matrix4x.m02, matrix4x.m12, matrix4x.m22);
			Vector3 a = new Vector3(matrix4x.m03, matrix4x.m13, matrix4x.m23);
			float magnitude = vector2.magnitude;
			float num = Mathf.Abs(bone.lossyScale.z);
			position = a * (num / magnitude);
			if (Vector3.Dot(Vector3.Cross(lhs, vector), vector2) >= 0f)
			{
				rotation = Quaternion.LookRotation(vector2, vector);
			}
			else
			{
				rotation = Quaternion.LookRotation(-vector2, -vector);
			}
		}

		public static void SampleBindPose(GameObject go)
		{
			List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>(go.GetComponentsInChildren<SkinnedMeshRenderer>());
			list.Sort(new AvatarSetupTool.SkinTransformHierarchySorter());
			foreach (SkinnedMeshRenderer current in list)
			{
				Matrix4x4 localToWorldMatrix = current.transform.localToWorldMatrix;
				List<Transform> list2 = new List<Transform>(current.bones);
				Vector3[] array = new Vector3[list2.Count];
				for (int i = 0; i < list2.Count; i++)
				{
					array[i] = list2[i].localPosition;
				}
				Dictionary<Transform, Transform> dictionary = new Dictionary<Transform, Transform>();
				foreach (Transform current2 in list2)
				{
					dictionary[current2] = current2.parent;
					current2.parent = null;
				}
				for (int j = 0; j < list2.Count; j++)
				{
					Vector3 position;
					Quaternion rotation;
					AvatarSetupTool.GetBindPoseBonePositionRotation(localToWorldMatrix, current.sharedMesh.bindposes[j], list2[j], out position, out rotation);
					list2[j].position = position;
					list2[j].rotation = rotation;
				}
				foreach (Transform current3 in list2)
				{
					current3.parent = dictionary[current3];
				}
				for (int k = 0; k < list2.Count; k++)
				{
					list2[k].localPosition = array[k];
				}
			}
		}

		public static void ShowBindPose(SkinnedMeshRenderer skin)
		{
			Matrix4x4 localToWorldMatrix = skin.transform.localToWorldMatrix;
			for (int i = 0; i < skin.bones.Length; i++)
			{
				Vector3 vector;
				Quaternion rotation;
				AvatarSetupTool.GetBindPoseBonePositionRotation(localToWorldMatrix, skin.sharedMesh.bindposes[i], skin.bones[i], out vector, out rotation);
				float handleSize = HandleUtility.GetHandleSize(vector);
				Handles.color = Handles.xAxisColor;
				Handles.DrawLine(vector, vector + rotation * Vector3.right * 0.3f * handleSize);
				Handles.color = Handles.yAxisColor;
				Handles.DrawLine(vector, vector + rotation * Vector3.up * 0.3f * handleSize);
				Handles.color = Handles.zAxisColor;
				Handles.DrawLine(vector, vector + rotation * Vector3.forward * 0.3f * handleSize);
			}
		}

		public static void TransferPoseToDescription(SerializedObject serializedObject, Transform root)
		{
			SkeletonBone[] skeletonBones = new SkeletonBone[0];
			if (root)
			{
				AvatarSetupTool.TransferPoseToDescription(serializedObject, root, true, ref skeletonBones);
			}
			SerializedProperty serializedProperty = serializedObject.FindProperty(AvatarSetupTool.sSkeleton);
			ModelImporter.UpdateSkeletonPose(skeletonBones, serializedProperty);
		}

		private static void TransferPoseToDescription(SerializedObject serializedObject, Transform transform, bool isRoot, ref SkeletonBone[] skeletonBones)
		{
			ArrayUtility.Add<SkeletonBone>(ref skeletonBones, new SkeletonBone
			{
				name = transform.name,
				parentName = ((!isRoot) ? transform.parent.name : ""),
				position = transform.localPosition,
				rotation = transform.localRotation,
				scale = transform.localScale
			});
			IEnumerator enumerator = transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform2 = (Transform)enumerator.Current;
					AvatarSetupTool.TransferPoseToDescription(serializedObject, transform2, false, ref skeletonBones);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public static void TransferDescriptionToPose(SerializedObject serializedObject, Transform root)
		{
			if (root != null)
			{
				AvatarSetupTool.TransferDescriptionToPose(serializedObject, root, true);
			}
		}

		private static void TransferDescriptionToPose(SerializedObject serializedObject, Transform transform, bool isRoot)
		{
			SerializedProperty serializedProperty = AvatarSetupTool.FindSkeletonBone(serializedObject, transform, false, isRoot);
			if (serializedProperty != null)
			{
				SerializedProperty serializedProperty2 = serializedProperty.FindPropertyRelative(AvatarSetupTool.sPosition);
				SerializedProperty serializedProperty3 = serializedProperty.FindPropertyRelative(AvatarSetupTool.sRotation);
				SerializedProperty serializedProperty4 = serializedProperty.FindPropertyRelative(AvatarSetupTool.sScale);
				transform.localPosition = serializedProperty2.vector3Value;
				transform.localRotation = serializedProperty3.quaternionValue;
				transform.localScale = serializedProperty4.vector3Value;
			}
			IEnumerator enumerator = transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform2 = (Transform)enumerator.Current;
					AvatarSetupTool.TransferDescriptionToPose(serializedObject, transform2, false);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public static bool IsPoseValid(AvatarSetupTool.BoneWrapper[] bones)
		{
			return AvatarSetupTool.GetPoseError(bones) == 0f;
		}

		public static float GetPoseError(AvatarSetupTool.BoneWrapper[] bones)
		{
			Quaternion avatarOrientation = AvatarSetupTool.AvatarComputeOrientation(bones);
			float num = 0f;
			for (int i = 0; i < AvatarSetupTool.sBonePoses.Length; i++)
			{
				num += AvatarSetupTool.GetBoneAlignmentError(bones, avatarOrientation, i);
			}
			return num + AvatarSetupTool.GetCharacterPositionError(bones);
		}

		public static void MakePoseValid(AvatarSetupTool.BoneWrapper[] bones)
		{
			Quaternion avatarOrientation = AvatarSetupTool.AvatarComputeOrientation(bones);
			for (int i = 0; i < AvatarSetupTool.sBonePoses.Length; i++)
			{
				AvatarSetupTool.MakeBoneAlignmentValid(bones, avatarOrientation, i);
				if (i == 0)
				{
					avatarOrientation = AvatarSetupTool.AvatarComputeOrientation(bones);
				}
			}
			AvatarSetupTool.MakeCharacterPositionValid(bones);
		}

		public static float GetBoneAlignmentError(AvatarSetupTool.BoneWrapper[] bones, Quaternion avatarOrientation, int boneIndex)
		{
			float result;
			if (boneIndex < 0 || boneIndex >= AvatarSetupTool.sBonePoses.Length)
			{
				result = 0f;
			}
			else
			{
				AvatarSetupTool.BoneWrapper boneWrapper = bones[boneIndex];
				AvatarSetupTool.BonePoseData bonePoseData = AvatarSetupTool.sBonePoses[boneIndex];
				if (boneWrapper.bone == null || bonePoseData == null)
				{
					result = 0f;
				}
				else if (boneIndex == 0)
				{
					float num = Vector3.Angle(avatarOrientation * Vector3.right, Vector3.right);
					float num2 = Vector3.Angle(avatarOrientation * Vector3.up, Vector3.up);
					float num3 = Vector3.Angle(avatarOrientation * Vector3.forward, Vector3.forward);
					result = Mathf.Max(0f, Mathf.Max(new float[]
					{
						num,
						num2,
						num3
					}) - bonePoseData.maxAngle);
				}
				else
				{
					Vector3 vector = AvatarSetupTool.GetBoneAlignmentDirection(bones, avatarOrientation, boneIndex);
					if (vector == Vector3.zero)
					{
						result = 0f;
					}
					else
					{
						Quaternion rotationSpace = AvatarSetupTool.GetRotationSpace(bones, avatarOrientation, boneIndex);
						Vector3 to = rotationSpace * bonePoseData.direction;
						if (bonePoseData.planeNormal != Vector3.zero)
						{
							vector = Vector3.ProjectOnPlane(vector, rotationSpace * bonePoseData.planeNormal);
						}
						result = Mathf.Max(0f, Vector3.Angle(vector, to) - bonePoseData.maxAngle);
					}
				}
			}
			return result;
		}

		public static void MakeBoneAlignmentValid(AvatarSetupTool.BoneWrapper[] bones, Quaternion avatarOrientation, int boneIndex)
		{
			if (boneIndex >= 0 && boneIndex < AvatarSetupTool.sBonePoses.Length && boneIndex < bones.Length)
			{
				AvatarSetupTool.BoneWrapper boneWrapper = bones[boneIndex];
				AvatarSetupTool.BonePoseData bonePoseData = AvatarSetupTool.sBonePoses[boneIndex];
				if (!(boneWrapper.bone == null) && bonePoseData != null)
				{
					if (boneIndex == 0)
					{
						float num = Vector3.Angle(avatarOrientation * Vector3.right, Vector3.right);
						float num2 = Vector3.Angle(avatarOrientation * Vector3.up, Vector3.up);
						float num3 = Vector3.Angle(avatarOrientation * Vector3.forward, Vector3.forward);
						if (num > bonePoseData.maxAngle || num2 > bonePoseData.maxAngle || num3 > bonePoseData.maxAngle)
						{
							boneWrapper.bone.rotation = Quaternion.Inverse(avatarOrientation) * boneWrapper.bone.rotation;
						}
					}
					else
					{
						Vector3 vector = AvatarSetupTool.GetBoneAlignmentDirection(bones, avatarOrientation, boneIndex);
						if (!(vector == Vector3.zero))
						{
							Quaternion rotationSpace = AvatarSetupTool.GetRotationSpace(bones, avatarOrientation, boneIndex);
							Vector3 vector2 = rotationSpace * bonePoseData.direction;
							if (bonePoseData.planeNormal != Vector3.zero)
							{
								vector = Vector3.ProjectOnPlane(vector, rotationSpace * bonePoseData.planeNormal);
							}
							float num4 = Vector3.Angle(vector, vector2);
							if (num4 > bonePoseData.maxAngle * 0.99f)
							{
								Quaternion quaternion = Quaternion.FromToRotation(vector, vector2);
								Transform transform = null;
								Quaternion rotation = Quaternion.identity;
								if (boneIndex == 1 || boneIndex == 3)
								{
									transform = bones[5].bone;
								}
								if (boneIndex == 2 || boneIndex == 4)
								{
									transform = bones[6].bone;
								}
								if (transform != null)
								{
									rotation = transform.rotation;
								}
								float t = Mathf.Clamp01(1.05f - bonePoseData.maxAngle / num4);
								quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, t);
								boneWrapper.bone.rotation = quaternion * boneWrapper.bone.rotation;
								if (transform != null)
								{
									transform.rotation = rotation;
								}
							}
						}
					}
				}
			}
		}

		private static Quaternion GetRotationSpace(AvatarSetupTool.BoneWrapper[] bones, Quaternion avatarOrientation, int boneIndex)
		{
			Quaternion lhs = Quaternion.identity;
			AvatarSetupTool.BonePoseData bonePoseData = AvatarSetupTool.sBonePoses[boneIndex];
			if (!bonePoseData.compareInGlobalSpace)
			{
				int parentBone = HumanTrait.GetParentBone(boneIndex);
				if (parentBone > 0)
				{
					AvatarSetupTool.BonePoseData bonePoseData2 = AvatarSetupTool.sBonePoses[parentBone];
					if (bones[parentBone].bone != null && bonePoseData2 != null)
					{
						Vector3 boneAlignmentDirection = AvatarSetupTool.GetBoneAlignmentDirection(bones, avatarOrientation, parentBone);
						if (boneAlignmentDirection != Vector3.zero)
						{
							Vector3 fromDirection = avatarOrientation * bonePoseData2.direction;
							lhs = Quaternion.FromToRotation(fromDirection, boneAlignmentDirection);
						}
					}
				}
			}
			return lhs * avatarOrientation;
		}

		private static Vector3 GetBoneAlignmentDirection(AvatarSetupTool.BoneWrapper[] bones, Quaternion avatarOrientation, int boneIndex)
		{
			Vector3 result;
			if (AvatarSetupTool.sBonePoses[boneIndex] == null)
			{
				result = Vector3.zero;
			}
			else
			{
				AvatarSetupTool.BoneWrapper boneWrapper = bones[boneIndex];
				AvatarSetupTool.BonePoseData bonePoseData = AvatarSetupTool.sBonePoses[boneIndex];
				int num = -1;
				if (bonePoseData.childIndices != null)
				{
					int[] childIndices = bonePoseData.childIndices;
					for (int i = 0; i < childIndices.Length; i++)
					{
						int num2 = childIndices[i];
						if (bones[num2].bone != null)
						{
							num = num2;
							break;
						}
					}
				}
				else
				{
					num = AvatarSetupTool.GetHumanBoneChild(bones, boneIndex);
				}
				Vector3 vector;
				if (num >= 0 && bones[num] != null && bones[num].bone != null)
				{
					AvatarSetupTool.BoneWrapper boneWrapper2 = bones[num];
					vector = boneWrapper2.bone.position - boneWrapper.bone.position;
				}
				else
				{
					if (boneWrapper.bone.childCount != 1)
					{
						result = Vector3.zero;
						return result;
					}
					vector = Vector3.zero;
					IEnumerator enumerator = boneWrapper.bone.GetEnumerator();
					try
					{
						if (enumerator.MoveNext())
						{
							Transform transform = (Transform)enumerator.Current;
							vector = transform.position - boneWrapper.bone.position;
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
				result = vector.normalized;
			}
			return result;
		}

		public static Quaternion AvatarComputeOrientation(AvatarSetupTool.BoneWrapper[] bones)
		{
			Transform bone = bones[1].bone;
			Transform bone2 = bones[2].bone;
			Transform bone3 = bones[13].bone;
			Transform bone4 = bones[14].bone;
			Quaternion result;
			if (bone != null && bone2 != null && bone3 != null && bone4 != null)
			{
				result = AvatarSetupTool.AvatarComputeOrientation(bone.position, bone2.position, bone3.position, bone4.position);
			}
			else
			{
				result = Quaternion.identity;
			}
			return result;
		}

		public static Quaternion AvatarComputeOrientation(Vector3 leftUpLeg, Vector3 rightUpLeg, Vector3 leftArm, Vector3 rightArm)
		{
			Vector3 a = Vector3.Normalize(rightUpLeg - leftUpLeg);
			Vector3 b = Vector3.Normalize(rightArm - leftArm);
			Vector3 lhs = Vector3.Normalize(a + b);
			bool flag = Mathf.Abs(lhs.x * lhs.y) < 0.05f && Mathf.Abs(lhs.y * lhs.z) < 0.05f && Mathf.Abs(lhs.z * lhs.x) < 0.05f;
			Vector3 b2 = (leftUpLeg + rightUpLeg) * 0.5f;
			Vector3 a2 = (leftArm + rightArm) * 0.5f;
			Vector3 vector = Vector3.Normalize(a2 - b2);
			if (flag)
			{
				int index = 0;
				for (int i = 1; i < 3; i++)
				{
					if (Mathf.Abs(vector[i]) > Mathf.Abs(vector[index]))
					{
						index = i;
					}
				}
				float value = Mathf.Sign(vector[index]);
				vector = Vector3.zero;
				vector[index] = value;
			}
			Vector3 vector2 = Vector3.Cross(lhs, vector);
			Quaternion result;
			if (vector2 == Vector3.zero || vector == Vector3.zero)
			{
				result = Quaternion.identity;
			}
			else
			{
				result = Quaternion.LookRotation(vector2, vector);
			}
			return result;
		}

		private static float GetCharacterPositionError(AvatarSetupTool.BoneWrapper[] bones)
		{
			float result;
			AvatarSetupTool.GetCharacterPositionAdjustVector(bones, out result);
			return result;
		}

		internal static void MakeCharacterPositionValid(AvatarSetupTool.BoneWrapper[] bones)
		{
			float num;
			Vector3 characterPositionAdjustVector = AvatarSetupTool.GetCharacterPositionAdjustVector(bones, out num);
			if (characterPositionAdjustVector != Vector3.zero)
			{
				bones[0].bone.position += characterPositionAdjustVector;
			}
		}

		private static Vector3 GetCharacterPositionAdjustVector(AvatarSetupTool.BoneWrapper[] bones, out float error)
		{
			error = 0f;
			Transform bone = bones[1].bone;
			Transform bone2 = bones[2].bone;
			Vector3 result;
			if (bone == null || bone2 == null)
			{
				result = Vector3.zero;
			}
			else
			{
				Vector3 vector = (bone.position + bone2.position) * 0.5f;
				bool flag = true;
				Transform bone3 = bones[19].bone;
				Transform bone4 = bones[20].bone;
				if (bone3 == null || bone4 == null)
				{
					flag = false;
					bone3 = bones[5].bone;
					bone4 = bones[6].bone;
				}
				if (bone3 == null || bone4 == null)
				{
					result = Vector3.zero;
				}
				else
				{
					Vector3 vector2 = (bone3.position + bone4.position) * 0.5f;
					float num = vector.y - vector2.y;
					if (num <= 0f)
					{
						result = Vector3.zero;
					}
					else
					{
						Vector3 zero = Vector3.zero;
						if (vector2.y < 0f || vector2.y > num * ((!flag) ? 0.3f : 0.1f))
						{
							float num2 = vector.y - num * ((!flag) ? 1.13f : 1.03f);
							zero.y = -num2;
						}
						if (Mathf.Abs(vector.x) > 0.01f * num)
						{
							zero.x = -vector.x;
						}
						if (Mathf.Abs(vector.z) > 0.2f * num)
						{
							zero.z = -vector.z;
						}
						error = zero.magnitude * 100f / num;
						result = zero;
					}
				}
			}
			return result;
		}
	}
}
