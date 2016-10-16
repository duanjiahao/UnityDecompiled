using System;
using UnityEditor;

internal class PatchImportSettingRecycleID
{
	public static void Patch(SerializedObject serializedObject, int classID, string oldName, string newName)
	{
		PatchImportSettingRecycleID.PatchMultiple(serializedObject, classID, new string[]
		{
			oldName
		}, new string[]
		{
			newName
		});
	}

	public static void PatchMultiple(SerializedObject serializedObject, int classID, string[] oldNames, string[] newNames)
	{
		int num = oldNames.Length;
		SerializedProperty serializedProperty = serializedObject.FindProperty("m_FileIDToRecycleName");
		foreach (SerializedProperty serializedProperty2 in serializedProperty)
		{
			SerializedProperty serializedProperty3 = serializedProperty2.FindPropertyRelative("first");
			if (AssetImporter.LocalFileIDToClassID(serializedProperty3.longValue) == classID)
			{
				SerializedProperty serializedProperty4 = serializedProperty2.FindPropertyRelative("second");
				int num2 = Array.IndexOf<string>(oldNames, serializedProperty4.stringValue);
				if (num2 >= 0)
				{
					serializedProperty4.stringValue = newNames[num2];
					if (--num == 0)
					{
						break;
					}
				}
			}
		}
	}
}
