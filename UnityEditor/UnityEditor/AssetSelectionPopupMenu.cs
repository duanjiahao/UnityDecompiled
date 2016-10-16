using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetSelectionPopupMenu
	{
		public static void Show(Rect buttonRect, string[] classNames, int initialSelectedInstanceID)
		{
			GenericMenu genericMenu = new GenericMenu();
			List<UnityEngine.Object> list = AssetSelectionPopupMenu.FindAssetsOfType(classNames);
			if (list.Any<UnityEngine.Object>())
			{
				list.Sort((UnityEngine.Object result1, UnityEngine.Object result2) => EditorUtility.NaturalCompare(result1.name, result2.name));
				foreach (UnityEngine.Object current in list)
				{
					GUIContent content = new GUIContent(current.name);
					bool on = current.GetInstanceID() == initialSelectedInstanceID;
					genericMenu.AddItem(content, on, new GenericMenu.MenuFunction2(AssetSelectionPopupMenu.SelectCallback), current);
				}
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent("No Audio Mixers found in this project"));
			}
			genericMenu.DropDown(buttonRect);
		}

		private static void SelectCallback(object userData)
		{
			UnityEngine.Object @object = userData as UnityEngine.Object;
			if (@object != null)
			{
				Selection.activeInstanceID = @object.GetInstanceID();
			}
		}

		private static List<UnityEngine.Object> FindAssetsOfType(string[] classNames)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			hierarchyProperty.SetSearchFilter(new SearchFilter
			{
				classNames = classNames
			});
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			while (hierarchyProperty.Next(null))
			{
				list.Add(hierarchyProperty.pptrValue);
			}
			return list;
		}
	}
}
