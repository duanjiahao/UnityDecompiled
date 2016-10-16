using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class ExposeTransformEditor
	{
		private ReorderableList m_ExtraExposedTransformList;

		private string[] m_TransformPaths;

		private SerializedProperty m_ExtraExposedTransformPaths;

		public void OnEnable(string[] transformPaths, SerializedObject serializedObject)
		{
			this.m_TransformPaths = transformPaths;
			this.m_ExtraExposedTransformPaths = serializedObject.FindProperty("m_ExtraExposedTransformPaths");
			if (this.m_ExtraExposedTransformList == null)
			{
				this.m_ExtraExposedTransformList = new ReorderableList(serializedObject, this.m_ExtraExposedTransformPaths, false, true, true, true);
				this.m_ExtraExposedTransformList.onAddDropdownCallback = new ReorderableList.AddDropdownCallbackDelegate(this.AddTransformPathInList);
				this.m_ExtraExposedTransformList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveTransformPathInList);
				this.m_ExtraExposedTransformList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawTransformPathElement);
				this.m_ExtraExposedTransformList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawTransformPathListHeader);
				this.m_ExtraExposedTransformList.elementHeight = 16f;
			}
		}

		public void OnGUI()
		{
			this.m_ExtraExposedTransformList.DoLayoutList();
		}

		private void TransformPathSelected(object userData, string[] options, int selected)
		{
			string b = options[selected];
			for (int i = 0; i < this.m_ExtraExposedTransformPaths.arraySize; i++)
			{
				string stringValue = this.m_ExtraExposedTransformPaths.GetArrayElementAtIndex(i).stringValue;
				if (stringValue == b)
				{
					return;
				}
			}
			this.m_ExtraExposedTransformPaths.InsertArrayElementAtIndex(this.m_ExtraExposedTransformPaths.arraySize);
			this.m_ExtraExposedTransformPaths.GetArrayElementAtIndex(this.m_ExtraExposedTransformPaths.arraySize - 1).stringValue = options[selected];
		}

		private void AddTransformPathInList(Rect rect, ReorderableList list)
		{
			EditorUtility.DisplayCustomMenu(rect, this.m_TransformPaths, null, new EditorUtility.SelectMenuItemFunction(this.TransformPathSelected), null);
		}

		private void RemoveTransformPathInList(ReorderableList list)
		{
			this.m_ExtraExposedTransformPaths.DeleteArrayElementAtIndex(list.index);
		}

		private void DrawTransformPathElement(Rect rect, int index, bool selected, bool focused)
		{
			string stringValue = this.m_ExtraExposedTransformPaths.GetArrayElementAtIndex(index).stringValue;
			GUI.Label(rect, stringValue.Substring(stringValue.LastIndexOf("/") + 1), EditorStyles.label);
		}

		private void DrawTransformPathListHeader(Rect rect)
		{
			GUI.Label(rect, "Extra Transforms to Expose", EditorStyles.label);
		}
	}
}
