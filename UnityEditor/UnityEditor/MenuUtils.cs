using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	internal class MenuUtils
	{
		private class MenuCallbackObject
		{
			public string menuItemPath;
			public UnityEngine.Object[] temporaryContext;
		}
		public static void MenuCallback(object callbackObject)
		{
			MenuUtils.MenuCallbackObject menuCallbackObject = callbackObject as MenuUtils.MenuCallbackObject;
			if (menuCallbackObject.temporaryContext != null)
			{
				EditorApplication.ExecuteMenuItemWithTemporaryContext(menuCallbackObject.menuItemPath, menuCallbackObject.temporaryContext);
			}
			else
			{
				EditorApplication.ExecuteMenuItem(menuCallbackObject.menuItemPath);
			}
		}
		public static void ExtractSubMenuWithPath(string path, GenericMenu menu, string replacementPath, UnityEngine.Object[] temporaryContext)
		{
			HashSet<string> hashSet = new HashSet<string>(Unsupported.GetSubmenus(path));
			string[] submenusIncludingSeparators = Unsupported.GetSubmenusIncludingSeparators(path);
			for (int i = 0; i < submenusIncludingSeparators.Length; i++)
			{
				string text = submenusIncludingSeparators[i];
				string replacementMenuString = replacementPath + text.Substring(path.Length);
				if (hashSet.Contains(text))
				{
					MenuUtils.ExtractMenuItemWithPath(text, menu, replacementMenuString, temporaryContext);
				}
			}
		}
		public static void ExtractMenuItemWithPath(string menuString, GenericMenu menu, string replacementMenuString, UnityEngine.Object[] temporaryContext)
		{
			MenuUtils.MenuCallbackObject menuCallbackObject = new MenuUtils.MenuCallbackObject();
			menuCallbackObject.menuItemPath = menuString;
			menuCallbackObject.temporaryContext = temporaryContext;
			menu.AddItem(new GUIContent(replacementMenuString), false, new GenericMenu.MenuFunction2(MenuUtils.MenuCallback), menuCallbackObject);
		}
	}
}
