using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Animation))]
	internal class AnimationEditor : Editor
	{
		private static int s_BoxHash = "AnimationBoundsEditorHash".GetHashCode();
		private BoxEditor m_BoxEditor = new BoxEditor(false, AnimationEditor.s_BoxHash);
		private int m_PrePreviewAnimationArraySize = -1;
		private SerializedProperty m_UserAABB;
		public void OnEnable()
		{
			this.m_UserAABB = base.serializedObject.FindProperty("m_UserAABB");
			this.m_PrePreviewAnimationArraySize = -1;
			this.m_BoxEditor.OnEnable();
		}
		public void OnDisable()
		{
			this.m_BoxEditor.OnDisable();
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
			Animation animation = (Animation)this.target;
			if (animation && animation.cullingType == AnimationCullingType.BasedOnUserBounds)
			{
				EditorGUILayout.PropertyField(this.m_UserAABB, new GUIContent("Bounds"), new GUILayoutOption[0]);
				if (GUI.changed)
				{
					EditorUtility.SetDirty(this.target);
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}
		internal override void OnAssetStoreInspectorGUI()
		{
			this.OnInspectorGUI();
		}
		public void OnSceneGUI()
		{
			Animation animation = (Animation)this.target;
			if (animation && (animation.cullingType == AnimationCullingType.BasedOnClipBounds || animation.cullingType == AnimationCullingType.BasedOnUserBounds))
			{
				this.m_BoxEditor.SetAlwaysDisplayHandles(animation.cullingType == AnimationCullingType.BasedOnUserBounds);
				Bounds localBounds = animation.localBounds;
				Vector3 center = localBounds.center;
				Vector3 size = localBounds.size;
				if (this.m_BoxEditor.OnSceneGUI(animation.transform, Handles.s_BoundingBoxHandleColor, ref center, ref size))
				{
					Undo.RecordObject(animation, "Modified Animation bounds");
					animation.localBounds = new Bounds(center, size);
				}
			}
		}
	}
}
