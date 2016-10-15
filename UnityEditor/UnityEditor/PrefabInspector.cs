using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PrefabInspector
	{
		public static void OnOverridenPrefabsInspector(GameObject gameObject)
		{
			GUI.enabled = true;
			UnityEngine.Object prefabObject = PrefabUtility.GetPrefabObject(gameObject);
			if (prefabObject == null)
			{
				return;
			}
			EditorGUIUtility.labelWidth = 200f;
			if (PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance)
			{
				PropertyModification[] propertyModifications = PrefabUtility.GetPropertyModifications(gameObject);
				if (propertyModifications != null && propertyModifications.Length != 0)
				{
					GUI.changed = false;
					for (int i = 0; i < propertyModifications.Length; i++)
					{
						propertyModifications[i].value = EditorGUILayout.TextField(propertyModifications[i].propertyPath, propertyModifications[i].value, new GUILayoutOption[0]);
					}
					if (GUI.changed)
					{
						PrefabUtility.SetPropertyModifications(gameObject, propertyModifications);
					}
				}
			}
			PrefabInspector.AddComponentGUI(prefabObject);
		}

		private static void AddComponentGUI(UnityEngine.Object prefab)
		{
			SerializedObject serializedObject = new SerializedObject(prefab);
			SerializedProperty serializedProperty = serializedObject.FindProperty("m_Modification");
			SerializedProperty endProperty = serializedProperty.GetEndProperty();
			do
			{
				bool enterChildren = EditorGUILayout.PropertyField(serializedProperty, new GUILayoutOption[0]);
				if (!serializedProperty.NextVisible(enterChildren))
				{
					break;
				}
			}
			while (!SerializedProperty.EqualContents(serializedProperty, endProperty));
			serializedObject.ApplyModifiedProperties();
		}
	}
}
