using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.TreeViewExamples
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

		private BackendData.Foo m_Root;

		public bool m_RecursiveFindParentsBelow = true;

		private int m_MaxItems = 10000;

		private const int k_MinChildren = 3;

		private const int k_MaxChildren = 15;

		private const float k_ProbOfLastDescendent = 0.5f;

		private const int k_MaxDepth = 12;

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
			for (int i = 0; i < 10; i++)
			{
				this.AddChildrenRecursive(this.m_Root, UnityEngine.Random.Range(3, 15), true);
			}
		}

		public BackendData.Foo Find(int id)
		{
			return this.FindRecursive(id, this.m_Root);
		}

		public BackendData.Foo FindRecursive(int id, BackendData.Foo parent)
		{
			BackendData.Foo result;
			if (!parent.hasChildren)
			{
				result = null;
			}
			else
			{
				foreach (BackendData.Foo current in parent.children)
				{
					if (current.id == id)
					{
						result = current;
						return result;
					}
					BackendData.Foo foo = this.FindRecursive(id, current);
					if (foo != null)
					{
						result = foo;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		public HashSet<int> GetParentsBelow(int id)
		{
			BackendData.Foo foo = BackendData.FindItemRecursive(this.root, id);
			HashSet<int> result;
			if (foo != null)
			{
				if (this.m_RecursiveFindParentsBelow)
				{
					result = this.GetParentsBelowRecursive(foo);
				}
				else
				{
					result = this.GetParentsBelowStackBased(foo);
				}
			}
			else
			{
				result = new HashSet<int>();
			}
			return result;
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
			if (item.hasChildren)
			{
				parentIDs.Add(item.id);
				foreach (BackendData.Foo current in item.children)
				{
					BackendData.GetParentsBelowRecursive(current, parentIDs);
				}
			}
		}

		public void ReparentSelection(BackendData.Foo parentItem, int insertionIndex, List<BackendData.Foo> draggedItems)
		{
			if (parentItem != null)
			{
				if (insertionIndex > 0)
				{
					insertionIndex -= parentItem.children.GetRange(0, insertionIndex).Count(new Func<BackendData.Foo, bool>(draggedItems.Contains));
				}
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
				if (insertionIndex == -1)
				{
					insertionIndex = 0;
				}
				list.InsertRange(insertionIndex, draggedItems);
				parentItem.children = list;
			}
		}

		private void AddChildrenRecursive(BackendData.Foo foo, int numChildren, bool force)
		{
			if (this.IDCounter <= this.m_MaxItems)
			{
				if (foo.depth < 12)
				{
					if (force || UnityEngine.Random.value >= 0.5f)
					{
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
						if (this.IDCounter <= this.m_MaxItems)
						{
							foreach (BackendData.Foo current in foo.children)
							{
								this.AddChildrenRecursive(current, UnityEngine.Random.Range(3, 15), false);
							}
						}
					}
				}
			}
		}

		public static BackendData.Foo FindItemRecursive(BackendData.Foo item, int id)
		{
			BackendData.Foo result;
			if (item == null)
			{
				result = null;
			}
			else if (item.id == id)
			{
				result = item;
			}
			else if (item.children == null)
			{
				result = null;
			}
			else
			{
				foreach (BackendData.Foo current in item.children)
				{
					BackendData.Foo foo = BackendData.FindItemRecursive(current, id);
					if (foo != null)
					{
						result = foo;
						return result;
					}
				}
				result = null;
			}
			return result;
		}
	}
}
