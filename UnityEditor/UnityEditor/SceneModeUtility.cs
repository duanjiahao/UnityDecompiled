using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	public static class SceneModeUtility
	{
		private class Styles
		{
			public GUIStyle typeButton = "SearchModeFilter";
		}

		private static Type s_FocusType = null;

		private static SceneHierarchyWindow s_HierarchyWindow = null;

		private static GUIContent s_NoneButtonContent = null;

		private static SceneModeUtility.Styles s_Styles;

		private static SceneModeUtility.Styles styles
		{
			get
			{
				if (SceneModeUtility.s_Styles == null)
				{
					SceneModeUtility.s_Styles = new SceneModeUtility.Styles();
				}
				return SceneModeUtility.s_Styles;
			}
		}

		public static T[] GetSelectedObjectsOfType<T>(out GameObject[] gameObjects, params Type[] types) where T : UnityEngine.Object
		{
			if (types.Length == 0)
			{
				types = new Type[]
				{
					typeof(T)
				};
			}
			List<GameObject> list = new List<GameObject>();
			List<T> list2 = new List<T>();
			Transform[] transforms = Selection.GetTransforms((SelectionMode)12);
			Transform[] array = transforms;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				Type[] array2 = types;
				for (int j = 0; j < array2.Length; j++)
				{
					Type type = array2[j];
					UnityEngine.Object component = transform.gameObject.GetComponent(type);
					if (component != null)
					{
						list.Add(transform.gameObject);
						list2.Add((T)((object)component));
						break;
					}
				}
			}
			gameObjects = list.ToArray();
			return list2.ToArray();
		}

		public static void SearchForType(Type type)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow));
			SceneHierarchyWindow sceneHierarchyWindow = (array.Length <= 0) ? null : (array[0] as SceneHierarchyWindow);
			if (sceneHierarchyWindow)
			{
				SceneModeUtility.s_HierarchyWindow = sceneHierarchyWindow;
				if (type == null || type == typeof(GameObject))
				{
					SceneModeUtility.s_FocusType = null;
					sceneHierarchyWindow.ClearSearchFilter();
				}
				else
				{
					SceneModeUtility.s_FocusType = type;
					if (sceneHierarchyWindow.searchMode == SearchableEditorWindow.SearchMode.Name)
					{
						sceneHierarchyWindow.searchMode = SearchableEditorWindow.SearchMode.All;
					}
					sceneHierarchyWindow.SetSearchFilter("t:" + type.Name, sceneHierarchyWindow.searchMode, false);
					sceneHierarchyWindow.hasSearchFilterFocus = true;
				}
			}
			else
			{
				SceneModeUtility.s_FocusType = null;
			}
		}

		public static Type SearchBar(params Type[] types)
		{
			if (SceneModeUtility.s_NoneButtonContent == null)
			{
				SceneModeUtility.s_NoneButtonContent = EditorGUIUtility.IconContent("sv_icon_none");
				SceneModeUtility.s_NoneButtonContent.text = "None";
			}
			if (SceneModeUtility.s_FocusType != null && (SceneModeUtility.s_HierarchyWindow == null || SceneModeUtility.s_HierarchyWindow.m_SearchFilter != "t:" + SceneModeUtility.s_FocusType.Name))
			{
				SceneModeUtility.s_FocusType = null;
			}
			GUILayout.Label("Scene Filter:", new GUILayoutOption[0]);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUIContent label = EditorGUIUtility.TempContent("All", AssetPreview.GetMiniTypeThumbnail(typeof(GameObject)));
			if (SceneModeUtility.TypeButton(label, SceneModeUtility.s_FocusType == null, SceneModeUtility.styles.typeButton))
			{
				SceneModeUtility.SearchForType(null);
			}
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				Texture2D i2;
				if (type == typeof(Renderer))
				{
					i2 = (EditorGUIUtility.IconContent("MeshRenderer Icon").image as Texture2D);
				}
				else if (type == typeof(Terrain))
				{
					i2 = (EditorGUIUtility.IconContent("Terrain Icon").image as Texture2D);
				}
				else
				{
					i2 = AssetPreview.GetMiniTypeThumbnail(type);
				}
				string t = ObjectNames.NicifyVariableName(type.Name) + "s";
				GUIContent label2 = EditorGUIUtility.TempContent(t, i2);
				if (SceneModeUtility.TypeButton(label2, type == SceneModeUtility.s_FocusType, SceneModeUtility.styles.typeButton))
				{
					SceneModeUtility.SearchForType(type);
				}
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			return SceneModeUtility.s_FocusType;
		}

		private static bool TypeButton(GUIContent label, bool selected, GUIStyle style)
		{
			EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
			bool flag = GUILayout.Toggle(selected, label, style, new GUILayoutOption[0]);
			EditorGUIUtility.SetIconSize(Vector2.zero);
			return flag && flag != selected;
		}

		public static bool StaticFlagField(string label, SerializedProperty property, int flag)
		{
			bool flag2 = (property.intValue & flag) != 0;
			bool flag3 = (property.hasMultipleDifferentValuesBitwise & flag) != 0;
			EditorGUI.showMixedValue = flag3;
			EditorGUI.BeginChangeCheck();
			bool flag4 = EditorGUILayout.Toggle(label, flag2, new GUILayoutOption[0]);
			bool result;
			if (EditorGUI.EndChangeCheck())
			{
				if (!SceneModeUtility.SetStaticFlags(property.serializedObject.targetObjects, flag, flag4))
				{
					result = (flag2 && !flag3);
				}
				else
				{
					result = flag4;
				}
			}
			else
			{
				EditorGUI.showMixedValue = false;
				result = (flag4 && !flag3);
			}
			return result;
		}

		public static bool SetStaticFlags(UnityEngine.Object[] targetObjects, int changedFlags, bool flagValue)
		{
			bool flag = changedFlags == -1;
			StaticEditorFlags staticEditorFlags = (!flag) ? ((StaticEditorFlags)Enum.Parse(typeof(StaticEditorFlags), changedFlags.ToString())) : ((StaticEditorFlags)0);
			GameObjectUtility.ShouldIncludeChildren shouldIncludeChildren = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(targetObjects.OfType<GameObject>(), "Change Static Flags", (!flag) ? string.Concat(new string[]
			{
				"Do you want to ",
				(!flagValue) ? "disable" : "enable",
				" the ",
				ObjectNames.NicifyVariableName(staticEditorFlags.ToString()),
				" flag for all the child objects as well?"
			}) : ("Do you want to " + ((!flagValue) ? "disable" : "enable") + " the static flags for all the child objects as well?"));
			bool result;
			if (shouldIncludeChildren == GameObjectUtility.ShouldIncludeChildren.Cancel)
			{
				GUIUtility.ExitGUI();
				result = false;
			}
			else
			{
				GameObject[] objects = SceneModeUtility.GetObjects(targetObjects, shouldIncludeChildren == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
				Undo.RecordObjects(objects, "Change Static Flags");
				GameObject[] array = objects;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject go = array[i];
					int num = (int)GameObjectUtility.GetStaticEditorFlags(go);
					num = ((!flagValue) ? (num & ~changedFlags) : (num | changedFlags));
					GameObjectUtility.SetStaticEditorFlags(go, (StaticEditorFlags)num);
				}
				result = true;
			}
			return result;
		}

		private static void GetObjectsRecurse(Transform root, List<GameObject> arr)
		{
			arr.Add(root.gameObject);
			IEnumerator enumerator = root.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform root2 = (Transform)enumerator.Current;
					SceneModeUtility.GetObjectsRecurse(root2, arr);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public static GameObject[] GetObjects(UnityEngine.Object[] gameObjects, bool includeChildren)
		{
			List<GameObject> list = new List<GameObject>();
			if (!includeChildren)
			{
				for (int i = 0; i < gameObjects.Length; i++)
				{
					GameObject item = (GameObject)gameObjects[i];
					list.Add(item);
				}
			}
			else
			{
				for (int j = 0; j < gameObjects.Length; j++)
				{
					GameObject gameObject = (GameObject)gameObjects[j];
					SceneModeUtility.GetObjectsRecurse(gameObject.transform, list);
				}
			}
			return list.ToArray();
		}
	}
}
