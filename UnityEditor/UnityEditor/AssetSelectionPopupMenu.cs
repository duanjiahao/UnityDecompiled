using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetSelectionPopupMenu
	{
		[CompilerGenerated]
		private static GenericMenu.MenuFunction2 <>f__mg$cache0;

		public static void Show(Rect buttonRect, string[] classNames, int initialSelectedInstanceID)
		{
			GenericMenu genericMenu = new GenericMenu();
			List<UnityEngine.Object> list = AssetSelectionPopupMenu.FindAssetsOfType(classNames);
			if (list.Any<UnityEngine.Object>())
			{
				list.Sort((UnityEngine.Object result1, UnityEngine.Object result2) => EditorUtility.NaturalCompare(result1.name, result2.name));
				foreach (UnityEngine.Object current in list)
				{
					GUIContent gUIContent = new GUIContent(current.name);
					bool flag = current.GetInstanceID() == initialSelectedInstanceID;
					GenericMenu arg_8E_0 = genericMenu;
					GUIContent arg_8E_1 = gUIContent;
					bool arg_8E_2 = flag;
					if (AssetSelectionPopupMenu.<>f__mg$cache0 == null)
					{
						AssetSelectionPopupMenu.<>f__mg$cache0 = new GenericMenu.MenuFunction2(AssetSelectionPopupMenu.SelectCallback);
					}
					arg_8E_0.AddItem(arg_8E_1, arg_8E_2, AssetSelectionPopupMenu.<>f__mg$cache0, current);
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
