using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public class TreeViewItem : IComparable<TreeViewItem>
	{
		private int m_ID;

		private TreeViewItem m_Parent;

		private List<TreeViewItem> m_Children = null;

		private int m_Depth;

		private string m_DisplayName;

		private Texture2D m_Icon;

		public virtual int id
		{
			get
			{
				return this.m_ID;
			}
			set
			{
				this.m_ID = value;
			}
		}

		public virtual string displayName
		{
			get
			{
				return this.m_DisplayName;
			}
			set
			{
				this.m_DisplayName = value;
			}
		}

		public virtual int depth
		{
			get
			{
				return this.m_Depth;
			}
			set
			{
				this.m_Depth = value;
			}
		}

		public virtual bool hasChildren
		{
			get
			{
				return this.m_Children != null && this.m_Children.Count > 0;
			}
		}

		public virtual List<TreeViewItem> children
		{
			get
			{
				return this.m_Children;
			}
			set
			{
				this.m_Children = value;
			}
		}

		public virtual TreeViewItem parent
		{
			get
			{
				return this.m_Parent;
			}
			set
			{
				this.m_Parent = value;
			}
		}

		public virtual Texture2D icon
		{
			get
			{
				return this.m_Icon;
			}
			set
			{
				this.m_Icon = value;
			}
		}

		public TreeViewItem()
		{
		}

		public TreeViewItem(int id)
		{
			this.m_ID = id;
		}

		public TreeViewItem(int id, int depth)
		{
			this.m_ID = id;
			this.m_Depth = depth;
		}

		public TreeViewItem(int id, int depth, string displayName)
		{
			this.m_Depth = depth;
			this.m_ID = id;
			this.m_DisplayName = displayName;
		}

		internal TreeViewItem(int id, int depth, TreeViewItem parent, string displayName)
		{
			this.m_Depth = depth;
			this.m_Parent = parent;
			this.m_ID = id;
			this.m_DisplayName = displayName;
		}

		public void AddChild(TreeViewItem child)
		{
			if (this.m_Children == null)
			{
				this.m_Children = new List<TreeViewItem>();
			}
			this.m_Children.Add(child);
			if (child != null)
			{
				child.parent = this;
			}
		}

		public virtual int CompareTo(TreeViewItem other)
		{
			return this.displayName.CompareTo(other.displayName);
		}

		public override string ToString()
		{
			return string.Format("Item: '{0}' ({1}), has {2} children, depth {3}, parent id {4}", new object[]
			{
				this.displayName,
				this.id,
				(!this.hasChildren) ? 0 : this.children.Count,
				this.depth,
				(this.parent == null) ? -1 : this.parent.id
			});
		}
	}
}
