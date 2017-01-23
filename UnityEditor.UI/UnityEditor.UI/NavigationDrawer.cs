using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CustomPropertyDrawer(typeof(Navigation), true)]
	public class NavigationDrawer : PropertyDrawer
	{
		private class Styles
		{
			public readonly GUIContent navigationContent;

			public Styles()
			{
				this.navigationContent = new GUIContent("Navigation");
			}
		}

		private static NavigationDrawer.Styles s_Styles = null;

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			if (NavigationDrawer.s_Styles == null)
			{
				NavigationDrawer.s_Styles = new NavigationDrawer.Styles();
			}
			Rect position = pos;
			position.height = EditorGUIUtility.singleLineHeight;
			SerializedProperty serializedProperty = prop.FindPropertyRelative("m_Mode");
			Navigation.Mode navigationMode = NavigationDrawer.GetNavigationMode(serializedProperty);
			EditorGUI.PropertyField(position, serializedProperty, NavigationDrawer.s_Styles.navigationContent);
			EditorGUI.indentLevel++;
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			if (navigationMode == Navigation.Mode.Explicit)
			{
				SerializedProperty property = prop.FindPropertyRelative("m_SelectOnUp");
				SerializedProperty property2 = prop.FindPropertyRelative("m_SelectOnDown");
				SerializedProperty property3 = prop.FindPropertyRelative("m_SelectOnLeft");
				SerializedProperty property4 = prop.FindPropertyRelative("m_SelectOnRight");
				EditorGUI.PropertyField(position, property);
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				EditorGUI.PropertyField(position, property2);
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				EditorGUI.PropertyField(position, property3);
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				EditorGUI.PropertyField(position, property4);
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			EditorGUI.indentLevel--;
		}

		private static Navigation.Mode GetNavigationMode(SerializedProperty navigation)
		{
			return (Navigation.Mode)navigation.enumValueIndex;
		}

		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			SerializedProperty serializedProperty = prop.FindPropertyRelative("m_Mode");
			float result;
			if (serializedProperty == null)
			{
				result = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				Navigation.Mode navigationMode = NavigationDrawer.GetNavigationMode(serializedProperty);
				if (navigationMode != Navigation.Mode.None)
				{
					if (navigationMode != Navigation.Mode.Explicit)
					{
						result = EditorGUIUtility.singleLineHeight + 1f * EditorGUIUtility.standardVerticalSpacing;
					}
					else
					{
						result = 5f * EditorGUIUtility.singleLineHeight + 5f * EditorGUIUtility.standardVerticalSpacing;
					}
				}
				else
				{
					result = EditorGUIUtility.singleLineHeight;
				}
			}
			return result;
		}
	}
}
