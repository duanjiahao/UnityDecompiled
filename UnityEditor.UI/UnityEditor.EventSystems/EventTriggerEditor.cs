using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEditor.EventSystems
{
	[CustomEditor(typeof(EventTrigger), true)]
	public class EventTriggerEditor : Editor
	{
		private SerializedProperty m_DelegatesProperty;

		private GUIContent m_IconToolbarMinus;

		private GUIContent m_EventIDName;

		private GUIContent[] m_EventTypes;

		private GUIContent m_AddButonContent;

		protected virtual void OnEnable()
		{
			this.m_DelegatesProperty = base.serializedObject.FindProperty("m_Delegates");
			this.m_AddButonContent = new GUIContent("Add New Event Type");
			this.m_EventIDName = new GUIContent("");
			this.m_IconToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"));
			this.m_IconToolbarMinus.tooltip = "Remove all events in this list.";
			string[] names = Enum.GetNames(typeof(EventTriggerType));
			this.m_EventTypes = new GUIContent[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				this.m_EventTypes[i] = new GUIContent(names[i]);
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			int num = -1;
			EditorGUILayout.Space();
			Vector2 vector = GUIStyle.none.CalcSize(this.m_IconToolbarMinus);
			for (int i = 0; i < this.m_DelegatesProperty.arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex = this.m_DelegatesProperty.GetArrayElementAtIndex(i);
				SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("eventID");
				SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("callback");
				this.m_EventIDName.text = serializedProperty.enumDisplayNames[serializedProperty.enumValueIndex];
				EditorGUILayout.PropertyField(property, this.m_EventIDName, new GUILayoutOption[0]);
				Rect lastRect = GUILayoutUtility.GetLastRect();
				Rect position = new Rect(lastRect.xMax - vector.x - 8f, lastRect.y + 1f, vector.x, vector.y);
				if (GUI.Button(position, this.m_IconToolbarMinus, GUIStyle.none))
				{
					num = i;
				}
				EditorGUILayout.Space();
			}
			if (num > -1)
			{
				this.RemoveEntry(num);
			}
			Rect rect = GUILayoutUtility.GetRect(this.m_AddButonContent, GUI.skin.button);
			rect.x += (rect.width - 200f) / 2f;
			rect.width = 200f;
			if (GUI.Button(rect, this.m_AddButonContent))
			{
				this.ShowAddTriggermenu();
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		private void RemoveEntry(int toBeRemovedEntry)
		{
			this.m_DelegatesProperty.DeleteArrayElementAtIndex(toBeRemovedEntry);
		}

		private void ShowAddTriggermenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			for (int i = 0; i < this.m_EventTypes.Length; i++)
			{
				bool flag = true;
				for (int j = 0; j < this.m_DelegatesProperty.arraySize; j++)
				{
					SerializedProperty arrayElementAtIndex = this.m_DelegatesProperty.GetArrayElementAtIndex(j);
					SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("eventID");
					if (serializedProperty.enumValueIndex == i)
					{
						flag = false;
					}
				}
				if (flag)
				{
					genericMenu.AddItem(this.m_EventTypes[i], false, new GenericMenu.MenuFunction2(this.OnAddNewSelected), i);
				}
				else
				{
					genericMenu.AddDisabledItem(this.m_EventTypes[i]);
				}
			}
			genericMenu.ShowAsContext();
			Event.current.Use();
		}

		private void OnAddNewSelected(object index)
		{
			int enumValueIndex = (int)index;
			this.m_DelegatesProperty.arraySize++;
			SerializedProperty arrayElementAtIndex = this.m_DelegatesProperty.GetArrayElementAtIndex(this.m_DelegatesProperty.arraySize - 1);
			SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("eventID");
			serializedProperty.enumValueIndex = enumValueIndex;
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
