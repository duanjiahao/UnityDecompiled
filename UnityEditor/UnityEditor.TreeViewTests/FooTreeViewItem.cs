using System;

namespace UnityEditor.TreeViewTests
{
	internal class FooTreeViewItem : TreeViewItem
	{
		public BackendData.Foo foo
		{
			get;
			private set;
		}

		public FooTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, BackendData.Foo foo) : base(id, depth, parent, displayName)
		{
			this.foo = foo;
		}
	}
}
