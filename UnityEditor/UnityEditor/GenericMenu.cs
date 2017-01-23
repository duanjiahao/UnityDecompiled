using System;
using System.Collections;
using UnityEngine;

namespace UnityEditor
{
	public sealed class GenericMenu
	{
		public delegate void MenuFunction();

		public delegate void MenuFunction2(object userData);

		private sealed class MenuItem
		{
			public GUIContent content;

			public bool separator;

			public bool on;

			public GenericMenu.MenuFunction func;

			public GenericMenu.MenuFunction2 func2;

			public object userData;

			public MenuItem(GUIContent _content, bool _separator, bool _on, GenericMenu.MenuFunction _func)
			{
				this.content = _content;
				this.separator = _separator;
				this.on = _on;
				this.func = _func;
			}

			public MenuItem(GUIContent _content, bool _separator, bool _on, GenericMenu.MenuFunction2 _func, object _userData)
			{
				this.content = _content;
				this.separator = _separator;
				this.on = _on;
				this.func2 = _func;
				this.userData = _userData;
			}
		}

		private ArrayList menuItems = new ArrayList();

		public void AddItem(GUIContent content, bool on, GenericMenu.MenuFunction func)
		{
			this.menuItems.Add(new GenericMenu.MenuItem(content, false, on, func));
		}

		public void AddItem(GUIContent content, bool on, GenericMenu.MenuFunction2 func, object userData)
		{
			this.menuItems.Add(new GenericMenu.MenuItem(content, false, on, func, userData));
		}

		public void AddDisabledItem(GUIContent content)
		{
			this.menuItems.Add(new GenericMenu.MenuItem(content, false, false, null));
		}

		public void AddSeparator(string path)
		{
			this.menuItems.Add(new GenericMenu.MenuItem(new GUIContent(path), true, false, null));
		}

		public int GetItemCount()
		{
			return this.menuItems.Count;
		}

		public void ShowAsContext()
		{
			if (Event.current != null)
			{
				this.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
			}
		}

		public void DropDown(Rect position)
		{
			string[] array = new string[this.menuItems.Count];
			bool[] array2 = new bool[this.menuItems.Count];
			ArrayList arrayList = new ArrayList();
			bool[] array3 = new bool[this.menuItems.Count];
			for (int i = 0; i < this.menuItems.Count; i++)
			{
				GenericMenu.MenuItem menuItem = (GenericMenu.MenuItem)this.menuItems[i];
				array[i] = menuItem.content.text;
				array2[i] = (menuItem.func != null || menuItem.func2 != null);
				array3[i] = menuItem.separator;
				if (menuItem.on)
				{
					arrayList.Add(i);
				}
			}
			EditorUtility.DisplayCustomMenuWithSeparators(position, array, array2, array3, (int[])arrayList.ToArray(typeof(int)), new EditorUtility.SelectMenuItemFunction(this.CatchMenu), null, true);
		}

		internal void Popup(Rect position, int selectedIndex)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				this.DropDown(position);
			}
			else
			{
				this.DropDown(position);
			}
		}

		private void CatchMenu(object userData, string[] options, int selected)
		{
			GenericMenu.MenuItem menuItem = (GenericMenu.MenuItem)this.menuItems[selected];
			if (menuItem.func2 != null)
			{
				menuItem.func2(menuItem.userData);
			}
			else if (menuItem.func != null)
			{
				menuItem.func();
			}
		}
	}
}
