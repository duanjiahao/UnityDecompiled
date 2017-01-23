using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CustomEditor(typeof(ShaderVariantCollection))]
	internal class ShaderVariantCollectionInspector : Editor
	{
		private class Styles
		{
			public static readonly GUIContent iconAdd = EditorGUIUtility.IconContent("Toolbar Plus", "|Add variant");

			public static readonly GUIContent iconRemove = EditorGUIUtility.IconContent("Toolbar Minus", "|Remove entry");

			public static readonly GUIStyle invisibleButton = "InvisibleButton";
		}

		private SerializedProperty m_Shaders;

		public virtual void OnEnable()
		{
			this.m_Shaders = base.serializedObject.FindProperty("m_Shaders");
		}

		private static Rect GetAddRemoveButtonRect(Rect r)
		{
			Vector2 vector = ShaderVariantCollectionInspector.Styles.invisibleButton.CalcSize(ShaderVariantCollectionInspector.Styles.iconRemove);
			return new Rect(r.xMax - vector.x, r.y + (float)((int)(r.height / 2f - vector.y / 2f)), vector.x, vector.y);
		}

		private void DisplayAddVariantsWindow(Shader shader, ShaderVariantCollection collection)
		{
			AddShaderVariantWindow.PopupData popupData = new AddShaderVariantWindow.PopupData();
			popupData.shader = shader;
			popupData.collection = collection;
			string[] array;
			ShaderUtil.GetShaderVariantEntries(shader, collection, out popupData.types, out array);
			if (array.Length == 0)
			{
				EditorApplication.Beep();
			}
			else
			{
				popupData.keywords = new string[array.Length][];
				for (int i = 0; i < array.Length; i++)
				{
					popupData.keywords[i] = array[i].Split(new char[]
					{
						' '
					});
				}
				AddShaderVariantWindow.ShowAddVariantWindow(popupData);
				GUIUtility.ExitGUI();
			}
		}

		private void DrawShaderEntry(int shaderIndex)
		{
			SerializedProperty arrayElementAtIndex = this.m_Shaders.GetArrayElementAtIndex(shaderIndex);
			Shader shader = (Shader)arrayElementAtIndex.FindPropertyRelative("first").objectReferenceValue;
			SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("second.variants");
			using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
			{
				Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
				Rect addRemoveButtonRect = ShaderVariantCollectionInspector.GetAddRemoveButtonRect(rect);
				rect.xMax = addRemoveButtonRect.x;
				GUI.Label(rect, shader.name, EditorStyles.boldLabel);
				if (GUI.Button(addRemoveButtonRect, ShaderVariantCollectionInspector.Styles.iconRemove, ShaderVariantCollectionInspector.Styles.invisibleButton))
				{
					this.m_Shaders.DeleteArrayElementAtIndex(shaderIndex);
					return;
				}
			}
			for (int i = 0; i < serializedProperty.arraySize; i++)
			{
				SerializedProperty arrayElementAtIndex2 = serializedProperty.GetArrayElementAtIndex(i);
				string text = arrayElementAtIndex2.FindPropertyRelative("keywords").stringValue;
				if (string.IsNullOrEmpty(text))
				{
					text = "<no keywords>";
				}
				PassType intValue = (PassType)arrayElementAtIndex2.FindPropertyRelative("passType").intValue;
				Rect rect2 = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
				Rect addRemoveButtonRect2 = ShaderVariantCollectionInspector.GetAddRemoveButtonRect(rect2);
				rect2.xMax = addRemoveButtonRect2.x;
				GUI.Label(rect2, intValue + " " + text, EditorStyles.miniLabel);
				if (GUI.Button(addRemoveButtonRect2, ShaderVariantCollectionInspector.Styles.iconRemove, ShaderVariantCollectionInspector.Styles.invisibleButton))
				{
					serializedProperty.DeleteArrayElementAtIndex(i);
				}
			}
			Rect rect3 = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
			Rect addRemoveButtonRect3 = ShaderVariantCollectionInspector.GetAddRemoveButtonRect(rect3);
			if (GUI.Button(addRemoveButtonRect3, ShaderVariantCollectionInspector.Styles.iconAdd, ShaderVariantCollectionInspector.Styles.invisibleButton))
			{
				this.DisplayAddVariantsWindow(shader, base.target as ShaderVariantCollection);
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			for (int i = 0; i < this.m_Shaders.arraySize; i++)
			{
				this.DrawShaderEntry(i);
			}
			if (GUILayout.Button("Add shader", new GUILayoutOption[0]))
			{
				ObjectSelector.get.Show(null, typeof(Shader), null, false);
				ObjectSelector.get.objectSelectorID = "ShaderVariantSelector".GetHashCode();
				GUIUtility.ExitGUI();
			}
			if (Event.current.type == EventType.ExecuteCommand)
			{
				if (Event.current.commandName == "ObjectSelectorClosed" && ObjectSelector.get.objectSelectorID == "ShaderVariantSelector".GetHashCode())
				{
					Shader shader = ObjectSelector.GetCurrentObject() as Shader;
					if (shader != null)
					{
						ShaderUtil.AddNewShaderToCollection(shader, base.target as ShaderVariantCollection);
					}
					Event.current.Use();
					GUIUtility.ExitGUI();
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
