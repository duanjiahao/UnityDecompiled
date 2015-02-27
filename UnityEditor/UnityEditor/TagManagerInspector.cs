using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(TagManager))]
	internal class TagManagerInspector : Editor
	{
		protected SerializedProperty m_Tags;
		protected SerializedProperty m_SortingLayers;
		protected SerializedProperty[] m_Layers;
		protected bool m_LayersExpanded = true;
		private ReorderableList m_SortLayersList;
		public TagManager tagManager
		{
			get
			{
				return this.target as TagManager;
			}
		}
		internal override string targetTitle
		{
			get
			{
				return "Tags & Layers";
			}
		}
		public virtual void OnEnable()
		{
			this.m_Tags = base.serializedObject.FindProperty("tags");
			this.m_SortingLayers = base.serializedObject.FindProperty("m_SortingLayers");
			this.m_Layers = new SerializedProperty[32];
			for (int i = 0; i < 32; i++)
			{
				string propertyPath = ((i < 8) ? "Builtin Layer " : "User Layer ") + i;
				this.m_Layers[i] = base.serializedObject.FindProperty(propertyPath);
			}
			if (this.m_SortLayersList == null)
			{
				this.m_SortLayersList = new ReorderableList(base.serializedObject, this.m_SortingLayers, true, false, true, true);
				this.m_SortLayersList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.ReorderSortLayerList);
				this.m_SortLayersList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddToSortLayerList);
				this.m_SortLayersList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveFromSortLayerList);
				this.m_SortLayersList.onCanRemoveCallback = new ReorderableList.CanRemoveCallbackDelegate(this.CanRemoveSortLayerEntry);
				this.m_SortLayersList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawSortLayerListElement);
				this.m_SortLayersList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
				this.m_SortLayersList.headerHeight = 3f;
			}
			this.m_LayersExpanded = false;
			this.m_SortingLayers.isExpanded = false;
			this.m_Tags.isExpanded = false;
			string defaultExpandedFoldout = this.tagManager.m_DefaultExpandedFoldout;
			switch (defaultExpandedFoldout)
			{
			case "Tags":
				this.m_Tags.isExpanded = true;
				return;
			case "SortingLayers":
				this.m_SortingLayers.isExpanded = true;
				return;
			case "Layers":
				this.m_LayersExpanded = true;
				return;
			}
			this.m_LayersExpanded = true;
		}
		private void AddToSortLayerList(ReorderableList list)
		{
			base.serializedObject.ApplyModifiedProperties();
			InternalEditorUtility.AddSortingLayer();
			base.serializedObject.Update();
			list.index = list.serializedProperty.arraySize - 1;
		}
		public void ReorderSortLayerList(ReorderableList list)
		{
			InternalEditorUtility.UpdateSortingLayersOrder();
		}
		private void RemoveFromSortLayerList(ReorderableList list)
		{
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
			base.serializedObject.ApplyModifiedProperties();
			base.serializedObject.Update();
			InternalEditorUtility.UpdateSortingLayersOrder();
		}
		private bool CanEditSortLayerEntry(int index)
		{
			return index >= 0 && index < InternalEditorUtility.GetSortingLayerCount() && !InternalEditorUtility.IsSortingLayerDefault(index);
		}
		private bool CanRemoveSortLayerEntry(ReorderableList list)
		{
			return this.CanEditSortLayerEntry(list.index);
		}
		private void DrawSortLayerListElement(Rect rect, int index, bool selected, bool focused)
		{
			rect.height -= 2f;
			rect.xMin -= 20f;
			bool enabled = GUI.enabled;
			GUI.enabled = this.CanEditSortLayerEntry(index);
			string sortingLayerName = InternalEditorUtility.GetSortingLayerName(index);
			int sortingLayerUserID = InternalEditorUtility.GetSortingLayerUserID(index);
			string text = EditorGUI.TextField(rect, " Layer " + sortingLayerUserID, sortingLayerName);
			if (text != sortingLayerName)
			{
				base.serializedObject.ApplyModifiedProperties();
				InternalEditorUtility.SetSortingLayerName(index, text);
				base.serializedObject.Update();
			}
			GUI.enabled = enabled;
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Tags, true, new GUILayoutOption[0]);
			this.m_SortingLayers.isExpanded = EditorGUILayout.Foldout(this.m_SortingLayers.isExpanded, "Sorting Layers");
			if (this.m_SortingLayers.isExpanded)
			{
				EditorGUI.indentLevel++;
				this.m_SortLayersList.DoLayoutList();
				EditorGUI.indentLevel--;
			}
			this.m_LayersExpanded = EditorGUILayout.Foldout(this.m_LayersExpanded, "Layers", EditorStyles.foldout);
			if (this.m_LayersExpanded)
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < this.m_Layers.Length; i++)
				{
					EditorGUILayout.PropertyField(this.m_Layers[i], new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
