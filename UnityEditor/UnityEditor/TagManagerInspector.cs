using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(TagManager))]
	internal class TagManagerInspector : Editor
	{
		internal enum InitialExpansionState
		{
			None,
			Tags,
			Layers,
			SortingLayers
		}

		private class EnterNamePopup : PopupWindowContent
		{
			public delegate void EnterDelegate(string str);

			private readonly TagManagerInspector.EnterNamePopup.EnterDelegate EnterCB;

			private string m_NewTagName = "New tag";

			private bool m_NeedsFocus = true;

			public EnterNamePopup(SerializedProperty tags, TagManagerInspector.EnterNamePopup.EnterDelegate cb)
			{
				this.EnterCB = cb;
				List<string> list = new List<string>();
				for (int i = 0; i < tags.arraySize; i++)
				{
					string stringValue = tags.GetArrayElementAtIndex(i).stringValue;
					if (!string.IsNullOrEmpty(stringValue))
					{
						list.Add(stringValue);
					}
				}
				this.m_NewTagName = ObjectNames.GetUniqueName(list.ToArray(), this.m_NewTagName);
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(400f, 48f);
			}

			public override void OnGUI(Rect windowRect)
			{
				GUILayout.Space(5f);
				Event current = Event.current;
				bool flag = current.type == EventType.KeyDown && (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter);
				GUI.SetNextControlName("TagName");
				this.m_NewTagName = EditorGUILayout.TextField("New Tag Name", this.m_NewTagName, new GUILayoutOption[0]);
				if (this.m_NeedsFocus)
				{
					this.m_NeedsFocus = false;
					EditorGUI.FocusTextInControl("TagName");
				}
				GUI.enabled = (this.m_NewTagName.Length != 0);
				if (GUILayout.Button("Save", new GUILayoutOption[0]) || flag)
				{
					this.EnterCB(this.m_NewTagName);
					base.editorWindow.Close();
				}
			}
		}

		protected SerializedProperty m_Tags;

		protected SerializedProperty m_SortingLayers;

		protected SerializedProperty m_Layers;

		private ReorderableList m_TagsList;

		private ReorderableList m_SortLayersList;

		private ReorderableList m_LayersList;

		protected bool m_IsEditable = false;

		private bool m_HaveRemovedTag = false;

		private static TagManagerInspector.InitialExpansionState s_InitialExpansionState = TagManagerInspector.InitialExpansionState.None;

		public TagManager tagManager
		{
			get
			{
				return base.target as TagManager;
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
			this.CheckForRemovedTags();
			if (this.m_TagsList == null)
			{
				this.m_TagsList = new ReorderableList(base.serializedObject, this.m_Tags, false, false, true, true);
				this.m_TagsList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.NewElement);
				this.m_TagsList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveFromTagsList);
				this.m_TagsList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawTagListElement);
				this.m_TagsList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
				this.m_TagsList.headerHeight = 3f;
			}
			this.m_SortingLayers = base.serializedObject.FindProperty("m_SortingLayers");
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
			this.m_Layers = base.serializedObject.FindProperty("layers");
			if (this.m_LayersList == null)
			{
				this.m_LayersList = new ReorderableList(base.serializedObject, this.m_Layers, false, false, false, false);
				this.m_LayersList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawLayerListElement);
				this.m_LayersList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
				this.m_LayersList.headerHeight = 3f;
			}
			if (TagManagerInspector.s_InitialExpansionState != TagManagerInspector.InitialExpansionState.None)
			{
				this.m_Tags.isExpanded = false;
				this.m_SortingLayers.isExpanded = false;
				this.m_Layers.isExpanded = false;
				TagManagerInspector.InitialExpansionState initialExpansionState = TagManagerInspector.s_InitialExpansionState;
				if (initialExpansionState != TagManagerInspector.InitialExpansionState.Tags)
				{
					if (initialExpansionState != TagManagerInspector.InitialExpansionState.Layers)
					{
						if (initialExpansionState == TagManagerInspector.InitialExpansionState.SortingLayers)
						{
							this.m_SortingLayers.isExpanded = true;
						}
					}
					else
					{
						this.m_Layers.isExpanded = true;
					}
				}
				else
				{
					this.m_Tags.isExpanded = true;
				}
				TagManagerInspector.s_InitialExpansionState = TagManagerInspector.InitialExpansionState.None;
			}
		}

		internal static void ShowWithInitialExpansion(TagManagerInspector.InitialExpansionState initialExpansionState)
		{
			TagManagerInspector.s_InitialExpansionState = initialExpansionState;
			Selection.activeObject = EditorApplication.tagManager;
		}

		private void CheckForRemovedTags()
		{
			for (int i = 0; i < this.m_Tags.arraySize; i++)
			{
				if (string.IsNullOrEmpty(this.m_Tags.GetArrayElementAtIndex(i).stringValue))
				{
					this.m_HaveRemovedTag = true;
				}
			}
		}

		private void NewElement(Rect buttonRect, ReorderableList list)
		{
			buttonRect.x -= 400f;
			buttonRect.y -= 13f;
			PopupWindow.Show(buttonRect, new TagManagerInspector.EnterNamePopup(this.m_Tags, delegate(string s)
			{
				InternalEditorUtility.AddTag(s);
			}), null, ShowMode.PopupMenuWithKeyboardFocus);
		}

		private void RemoveFromTagsList(ReorderableList list)
		{
			SerializedProperty arrayElementAtIndex = this.m_Tags.GetArrayElementAtIndex(list.index);
			if (!(arrayElementAtIndex.stringValue == ""))
			{
				GameObject gameObject = GameObject.FindWithTag(arrayElementAtIndex.stringValue);
				if (gameObject != null)
				{
					EditorUtility.DisplayDialog("Error", "Can't remove this tag because it is being used by " + gameObject.name, "OK");
				}
				else
				{
					InternalEditorUtility.RemoveTag(arrayElementAtIndex.stringValue);
					this.m_HaveRemovedTag = true;
				}
			}
		}

		private void DrawTagListElement(Rect rect, int index, bool selected, bool focused)
		{
			rect.height -= 2f;
			rect.xMin -= 20f;
			string text = this.m_Tags.GetArrayElementAtIndex(index).stringValue;
			if (string.IsNullOrEmpty(text))
			{
				text = "(Removed)";
			}
			EditorGUI.LabelField(rect, " Tag " + index, text);
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
			GUI.enabled = (this.m_IsEditable && this.CanEditSortLayerEntry(index));
			string sortingLayerName = InternalEditorUtility.GetSortingLayerName(index);
			string text = EditorGUI.TextField(rect, " Layer ", sortingLayerName);
			if (text != sortingLayerName)
			{
				base.serializedObject.ApplyModifiedProperties();
				InternalEditorUtility.SetSortingLayerName(index, text);
				base.serializedObject.Update();
			}
			GUI.enabled = enabled;
		}

		private void DrawLayerListElement(Rect rect, int index, bool selected, bool focused)
		{
			rect.height -= 2f;
			rect.xMin -= 20f;
			bool flag = index >= 8;
			bool enabled = GUI.enabled;
			GUI.enabled = (this.m_IsEditable && flag);
			string stringValue = this.m_Layers.GetArrayElementAtIndex(index).stringValue;
			string text;
			if (flag)
			{
				text = EditorGUI.TextField(rect, " User Layer " + index, stringValue);
			}
			else
			{
				text = EditorGUI.TextField(rect, " Builtin Layer " + index, stringValue);
			}
			if (text != stringValue)
			{
				this.m_Layers.GetArrayElementAtIndex(index).stringValue = text;
			}
			GUI.enabled = enabled;
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.m_IsEditable = AssetDatabase.IsOpenForEdit("ProjectSettings/TagManager.asset", StatusQueryOptions.UseCachedIfPossible);
			bool enabled = GUI.enabled;
			GUI.enabled = this.m_IsEditable;
			this.m_Tags.isExpanded = EditorGUILayout.Foldout(this.m_Tags.isExpanded, "Tags", true);
			if (this.m_Tags.isExpanded)
			{
				EditorGUI.indentLevel++;
				this.m_TagsList.DoLayoutList();
				EditorGUI.indentLevel--;
				if (this.m_HaveRemovedTag)
				{
					EditorGUILayout.HelpBox("There are removed tags. They will be removed from this list the next time the project is loaded.", MessageType.Info, true);
				}
			}
			this.m_SortingLayers.isExpanded = EditorGUILayout.Foldout(this.m_SortingLayers.isExpanded, "Sorting Layers", true);
			if (this.m_SortingLayers.isExpanded)
			{
				EditorGUI.indentLevel++;
				this.m_SortLayersList.DoLayoutList();
				EditorGUI.indentLevel--;
			}
			this.m_Layers.isExpanded = EditorGUILayout.Foldout(this.m_Layers.isExpanded, "Layers", true);
			if (this.m_Layers.isExpanded)
			{
				EditorGUI.indentLevel++;
				this.m_LayersList.DoLayoutList();
				EditorGUI.indentLevel--;
			}
			GUI.enabled = enabled;
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
