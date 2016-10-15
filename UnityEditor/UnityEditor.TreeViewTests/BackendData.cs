using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TreeViewTests
{
	internal class BackendData
	{
		public class Foo
		{
			public string name
			{
				get;
				set;
			}

			public int id
			{
				get;
				set;
			}

			public int depth
			{
				get;
				set;
			}

			public BackendData.Foo parent
			{
				get;
				set;
			}

			public List<BackendData.Foo> children
			{
				get;
				set;
			}

			public bool hasChildren
			{
				get
				{
					return this.children != null && this.children.Count > 0;
				}
			}

			public Foo(string name, int depth, int id)
			{
				this.name = name;
				this.depth = depth;
				this.id = id;
			}
		}

		private const int k_MinChildren = 3;

		private const int k_MaxChildren = 15;

		private const float k_ProbOfLastDescendent = 0.5f;

		private const int k_MaxDepth = 12;

		private BackendData.Foo m_Root;

		public bool m_RecursiveFindParentsBelow = true;

		private int m_MaxItems = 10000;

		public BackendData.Foo root
		{
			get
			{
				return this.m_Root;
			}
		}

		public int IDCounter
		{
			get;
			private set;
		}

		public void GenerateData(int maxNumItems)
		{
			this.m_MaxItems = maxNumItems;
			this.IDCounter = 1;
			this.m_Root = new BackendData.Foo("Root", 0, 0);
			while (this.IDCounter < this.m_MaxItems)
			{
				this.AddChildrenRecursive(this.m_Root, UnityEngine.Random.Range(3, 15), true);
			}
		}

		public HashSet<int> GetParentsBelow(int id)
		{
			BackendData.Foo foo = BackendData.FindItemRecursive(this.root, id);
			if (foo == null)
			{
				return new HashSet<int>();
			}
			if (this.m_RecursiveFindParentsBelow)
			{
				return this.GetParentsBelowRecursive(foo);
			}
			return this.GetParentsBelowStackBased(foo);
		}

		private HashSet<int> GetParentsBelowStackBased(BackendData.Foo searchFromThis)
		{
			Stack<BackendData.Foo> stack = new Stack<BackendData.Foo>();
			stack.Push(searchFromThis);
			HashSet<int> hashSet = new HashSet<int>();
			while (stack.Count > 0)
			{
				BackendData.Foo foo = stack.Pop();
				if (foo.hasChildren)
				{
					hashSet.Add(foo.id);
					foreach (BackendData.Foo current in foo.children)
					{
						stack.Push(current);
					}
				}
			}
			return hashSet;
		}

		private HashSet<int> GetParentsBelowRecursive(BackendData.Foo searchFromThis)
		{
			HashSet<int> hashSet = new HashSet<int>();
			BackendData.GetParentsBelowRecursive(searchFromThis, hashSet);
			return hashSet;
		}

		private static void GetParentsBelowRecursive(BackendData.Foo item, HashSet<int> parentIDs)
		{
			if (!item.hasChildren)
			{
				return;
			}
			parentIDs.Add(item.id);
			foreach (BackendData.Foo current in item.children)
			{
				BackendData.GetParentsBelowRecursive(current, parentIDs);
			}
		}

		public void ReparentSelection(BackendData.Foo parentItem, BackendData.Foo insertAfterItem, List<BackendData.Foo> draggedItems)
		{
			foreach (BackendData.Foo current in draggedItems)
			{
				current.parent.children.Remove(current);
				current.parent = parentItem;
			}
			if (!parentItem.hasChildren)
			{
				parentItem.children = new List<BackendData.Foo>();
			}
			List<BackendData.Foo> list = new List<BackendData.Foo>(parentItem.children);
			int index = 0;
			if (parentItem == insertAfterItem)
			{
				index = 0;
			}
			else
			{
				int num = parentItem.children.IndexOf(insertAfterItem);
				if (num >= 0)
				{
					index = num + 1;
				}
				else
				{
					Debug.LogError("Did not find insertAfterItem, should be a child of parentItem!!");
				}
			}
			list.InsertRange(index, draggedItems);
			parentItem.children = list;
		}

		private void AddChildrenRecursive(BackendData.Foo foo, int numChildren, bool force)
		{
			if (this.IDCounter > this.m_MaxItems)
			{
				return;
			}
			if (foo.depth >= 12)
			{
				return;
			}
			if (!force && UnityEngine.Random.value < 0.5f)
			{
				return;
			}
			if (foo.children == null)
			{
				foo.children = new List<BackendData.Foo>(numChildren);
			}
			for (int i = 0; i < numChildren; i++)
			{
				BackendData.Foo foo2 = new BackendData.Foo("Tud" + this.IDCounter, foo.depth + 1, ++this.IDCounter);
				foo2.parent = foo;
				foo.children.Add(foo2);
			}
			if (this.IDCounter > this.m_MaxItems)
			{
				return;
			}
			foreach (BackendData.Foo current in foo.children)
			{
				this.AddChildrenRecursive(current, UnityEngine.Random.Range(3, 15), false);
			}
		}

		public static BackendData.Foo FindItemRecursive(BackendData.Foo item, int id)
		{
			if (item == null)
			{
				return null;
			}
			if (item.id == id)
			{
				return item;
			}
			if (item.children == null)
			{
				return null;
			}
			foreach (BackendData.Foo current in item.children)
			{
				BackendData.Foo foo = BackendData.FindItemRecursive(current, id);
				if (foo != null)
				{
					return foo;
				}
			}
			return null;
		}
	}
}
