using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(MonoImporter))]
	internal class MonoScriptImporterInspector : AssetImporterInspector
	{
		private const int m_RowHeight = 16;
		private static GUIContent s_HelpIcon;
		private static GUIContent s_TitleSettingsIcon;
		private SerializedObject m_TargetObject;
		private SerializedProperty m_Icon;
		internal override void OnHeaderControlsGUI()
		{
			TextAsset textAsset = base.assetEditor.target as TextAsset;
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Open...", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				AssetDatabase.OpenAsset(textAsset);
				GUIUtility.ExitGUI();
			}
			if (textAsset as MonoScript && GUILayout.Button("Execution Order...", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				EditorApplication.ExecuteMenuItem("Edit/Project Settings/Script Execution Order");
				GUIUtility.ExitGUI();
			}
		}
		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			if (this.m_Icon == null)
			{
				this.m_TargetObject = new SerializedObject(base.assetEditor.targets);
				this.m_Icon = this.m_TargetObject.FindProperty("m_Icon");
			}
			EditorGUI.ObjectIconDropDown(iconRect, base.assetEditor.targets, true, null, this.m_Icon);
		}
		[MenuItem("CONTEXT/MonoImporter/Reset")]
		private static void ResetDefaultReferences(MenuCommand command)
		{
			MonoImporter monoImporter = command.context as MonoImporter;
			monoImporter.SetDefaultReferences(new string[0], new UnityEngine.Object[0]);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(monoImporter));
		}
		private void ShowFieldInfo(Type type, MonoImporter importer, List<string> names, List<UnityEngine.Object> objects, ref bool didModify)
		{
			if (type == null || !type.IsSubclassOf(typeof(MonoBehaviour)))
			{
				return;
			}
			this.ShowFieldInfo(type.BaseType, importer, names, objects, ref didModify);
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array = fields;
			int i = 0;
			while (i < array.Length)
			{
				FieldInfo fieldInfo = array[i];
				if (fieldInfo.IsPublic)
				{
					goto IL_77;
				}
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(SerializeField), true);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					goto IL_77;
				}
				IL_FC:
				i++;
				continue;
				IL_77:
				if (!fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) && fieldInfo.FieldType != typeof(UnityEngine.Object))
				{
					goto IL_FC;
				}
				UnityEngine.Object defaultReference = importer.GetDefaultReference(fieldInfo.Name);
				UnityEngine.Object @object = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(fieldInfo.Name), defaultReference, fieldInfo.FieldType, false, new GUILayoutOption[0]);
				names.Add(fieldInfo.Name);
				objects.Add(@object);
				if (defaultReference != @object)
				{
					didModify = true;
					goto IL_FC;
				}
				goto IL_FC;
			}
		}
		public override void OnInspectorGUI()
		{
			Vector2 iconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
			MonoImporter monoImporter = this.target as MonoImporter;
			MonoScript script = monoImporter.GetScript();
			if (script)
			{
				List<string> list = new List<string>();
				List<UnityEngine.Object> list2 = new List<UnityEngine.Object>();
				bool flag = false;
				Type @class = script.GetClass();
				this.ShowFieldInfo(@class, monoImporter, list, list2, ref flag);
				if (flag)
				{
					monoImporter.SetDefaultReferences(list.ToArray(), list2.ToArray());
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(monoImporter));
				}
			}
			EditorGUIUtility.SetIconSize(iconSize);
		}
	}
}
