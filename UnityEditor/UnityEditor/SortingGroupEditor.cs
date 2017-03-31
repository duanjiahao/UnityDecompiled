using System;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SortingGroup))]
	internal class SortingGroupEditor : Editor
	{
		private SerializedProperty m_SortingOrder;

		private SerializedProperty m_SortingLayerID;

		public virtual void OnEnable()
		{
			this.m_SortingOrder = base.serializedObject.FindProperty("m_SortingOrder");
			this.m_SortingLayerID = base.serializedObject.FindProperty("m_SortingLayerID");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			SortingLayerEditorUtility.RenderSortingLayerFields(this.m_SortingOrder, this.m_SortingLayerID);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
