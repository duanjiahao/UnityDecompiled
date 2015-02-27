using System;
using UnityEngine;
namespace UnityEditor
{
	[Serializable]
	internal class TransformRotationGUI
	{
		private GUIContent rotationContent = new GUIContent("Rotation", "The local rotation of this Game Object relative to the parent.");
		private Vector3 m_EulerAngles;
		private Quaternion m_OldQuaternion = new Quaternion(1234f, 1000f, 4321f, -1000f);
		private SerializedProperty m_Rotation;
		private UnityEngine.Object[] targets;
		private static int s_FoldoutHash = "Foldout".GetHashCode();
		public void OnEnable(SerializedProperty m_Rotation, GUIContent label)
		{
			this.m_Rotation = m_Rotation;
			this.targets = m_Rotation.serializedObject.targetObjects;
			this.rotationContent = label;
		}
		public void RotationField()
		{
			this.RotationField(false);
		}
		public void RotationField(bool disabled)
		{
			Transform transform = this.targets[0] as Transform;
			Quaternion localRotation = transform.localRotation;
			if (this.m_OldQuaternion.x != localRotation.x || this.m_OldQuaternion.y != localRotation.y || this.m_OldQuaternion.z != localRotation.z || this.m_OldQuaternion.w != localRotation.w)
			{
				this.m_EulerAngles = transform.localEulerAngles;
				this.m_OldQuaternion = localRotation;
			}
			bool flag = false;
			UnityEngine.Object[] array = this.targets;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform2 = (Transform)array[i];
				flag |= (transform2.localEulerAngles != this.m_EulerAngles);
			}
			Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * (float)((!EditorGUIUtility.wideMode) ? 2 : 1), new GUILayoutOption[0]);
			GUIContent label = EditorGUI.BeginProperty(rect, this.rotationContent, this.m_Rotation);
			EditorGUI.showMixedValue = flag;
			EditorGUI.BeginChangeCheck();
			int controlID = GUIUtility.GetControlID(TransformRotationGUI.s_FoldoutHash, EditorGUIUtility.native, rect);
			rect = EditorGUI.MultiFieldPrefixLabel(rect, controlID, label, 3);
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginDisabledGroup(disabled);
			this.m_EulerAngles = EditorGUI.Vector3Field(rect, GUIContent.none, this.m_EulerAngles);
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObjects(this.targets, "Inspector");
				UnityEngine.Object[] array2 = this.targets;
				for (int j = 0; j < array2.Length; j++)
				{
					Transform transform3 = (Transform)array2[j];
					transform3.localEulerAngles = this.m_EulerAngles;
					if (transform3.parent != null)
					{
						transform3.SendTransformChangedScale();
					}
				}
				this.m_Rotation.serializedObject.SetIsDifferentCacheDirty();
				this.m_OldQuaternion = localRotation;
			}
			EditorGUI.showMixedValue = false;
			EditorGUI.EndProperty();
		}
	}
}
