using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetPopupBackend
	{
		public static void AssetPopup<T>(SerializedProperty serializedProperty, GUIContent label, string fileExtension, string defaultFieldName) where T : UnityEngine.Object, new()
		{
			bool showMixedValue = EditorGUI.showMixedValue;
			EditorGUI.showMixedValue = serializedProperty.hasMultipleDifferentValues;
			string objectReferenceTypeString = serializedProperty.objectReferenceTypeString;
			GUIContent buttonContent;
			if (serializedProperty.hasMultipleDifferentValues)
			{
				buttonContent = EditorGUI.mixedValueContent;
			}
			else if (serializedProperty.objectReferenceValue != null)
			{
				buttonContent = GUIContent.Temp(serializedProperty.objectReferenceStringValue);
			}
			else
			{
				buttonContent = GUIContent.Temp(defaultFieldName);
			}
			Rect buttonRect;
			if (AudioMixerEffectGUI.PopupButton(label, buttonContent, EditorStyles.popup, out buttonRect, new GUILayoutOption[0]))
			{
				AssetPopupBackend.ShowAssetsPopupMenu<T>(buttonRect, objectReferenceTypeString, serializedProperty, fileExtension, defaultFieldName);
			}
			EditorGUI.showMixedValue = showMixedValue;
		}

		public static void AssetPopup<T>(SerializedProperty serializedProperty, GUIContent label, string fileExtension) where T : UnityEngine.Object, new()
		{
			AssetPopupBackend.AssetPopup<T>(serializedProperty, label, fileExtension, "Default");
		}

		private static void ShowAssetsPopupMenu<T>(Rect buttonRect, string typeName, SerializedProperty serializedProperty, string fileExtension, string defaultFieldName) where T : UnityEngine.Object, new()
		{
			GenericMenu genericMenu = new GenericMenu();
			int num = (!(serializedProperty.objectReferenceValue != null)) ? 0 : serializedProperty.objectReferenceValue.GetInstanceID();
			genericMenu.AddItem(new GUIContent(defaultFieldName), num == 0, new GenericMenu.MenuFunction2(AssetPopupBackend.AssetPopupMenuCallback), new object[]
			{
				0,
				serializedProperty
			});
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			SearchFilter searchFilter = new SearchFilter
			{
				classNames = new string[]
				{
					typeName
				}
			};
			hierarchyProperty.SetSearchFilter(searchFilter);
			hierarchyProperty.Reset();
			while (hierarchyProperty.Next(null))
			{
				genericMenu.AddItem(new GUIContent(hierarchyProperty.name), hierarchyProperty.instanceID == num, new GenericMenu.MenuFunction2(AssetPopupBackend.AssetPopupMenuCallback), new object[]
				{
					hierarchyProperty.instanceID,
					serializedProperty
				});
			}
			int num2 = BaseObjectTools.StringToClassID(typeName);
			if (num2 > 0)
			{
				BuiltinResource[] builtinResourceList = EditorGUIUtility.GetBuiltinResourceList(num2);
				BuiltinResource[] array = builtinResourceList;
				for (int i = 0; i < array.Length; i++)
				{
					BuiltinResource builtinResource = array[i];
					genericMenu.AddItem(new GUIContent(builtinResource.m_Name), builtinResource.m_InstanceID == num, new GenericMenu.MenuFunction2(AssetPopupBackend.AssetPopupMenuCallback), new object[]
					{
						builtinResource.m_InstanceID,
						serializedProperty
					});
				}
			}
			genericMenu.AddSeparator(string.Empty);
			genericMenu.AddItem(new GUIContent("Create New..."), false, delegate
			{
				T t = Activator.CreateInstance<T>();
				ProjectWindowUtil.CreateAsset(t, "New " + typeName + "." + fileExtension);
				serializedProperty.objectReferenceValue = t;
				serializedProperty.m_SerializedObject.ApplyModifiedProperties();
			});
			genericMenu.DropDown(buttonRect);
		}

		private static void ShowAssetsPopupMenu<T>(Rect buttonRect, string typeName, SerializedProperty serializedProperty, string fileExtension) where T : UnityEngine.Object, new()
		{
			AssetPopupBackend.ShowAssetsPopupMenu<T>(buttonRect, typeName, serializedProperty, fileExtension, "Default");
		}

		private static void AssetPopupMenuCallback(object userData)
		{
			object[] array = userData as object[];
			int instanceID = (int)array[0];
			SerializedProperty serializedProperty = (SerializedProperty)array[1];
			serializedProperty.objectReferenceValue = EditorUtility.InstanceIDToObject(instanceID);
			serializedProperty.m_SerializedObject.ApplyModifiedProperties();
		}
	}
}
