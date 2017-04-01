using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Animation))]
	internal class AnimationEditor : Editor
	{
		private int m_PrePreviewAnimationArraySize = -1;

		public void OnEnable()
		{
			this.m_PrePreviewAnimationArraySize = -1;
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			SerializedProperty serializedProperty = base.serializedObject.FindProperty("m_Animation");
			EditorGUILayout.PropertyField(serializedProperty, true, new GUILayoutOption[0]);
			int objectReferenceInstanceIDValue = serializedProperty.objectReferenceInstanceIDValue;
			SerializedProperty serializedProperty2 = base.serializedObject.FindProperty("m_Animations");
			int arraySize = serializedProperty2.arraySize;
			if (ObjectSelector.isVisible && this.m_PrePreviewAnimationArraySize == -1)
			{
				this.m_PrePreviewAnimationArraySize = arraySize;
			}
			if (this.m_PrePreviewAnimationArraySize != -1)
			{
				int num = (arraySize <= 0) ? -1 : serializedProperty2.GetArrayElementAtIndex(arraySize - 1).objectReferenceInstanceIDValue;
				if (num != objectReferenceInstanceIDValue)
				{
					serializedProperty2.arraySize = this.m_PrePreviewAnimationArraySize;
				}
				if (!ObjectSelector.isVisible)
				{
					this.m_PrePreviewAnimationArraySize = -1;
				}
			}
			Editor.DrawPropertiesExcluding(base.serializedObject, new string[]
			{
				"m_Animation",
				"m_UserAABB"
			});
			base.serializedObject.ApplyModifiedProperties();
		}

		internal override void OnAssetStoreInspectorGUI()
		{
			this.OnInspectorGUI();
		}
	}
}
