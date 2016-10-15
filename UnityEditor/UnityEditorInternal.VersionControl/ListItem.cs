using System;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace UnityEditorInternal.VersionControl
{
	public class ListItem
	{
		private ListItem parent;

		private ListItem firstChild;

		private ListItem lastChild;

		private ListItem prev;

		private ListItem next;

		private Texture icon;

		private string name;

		private int indent;

		private bool expanded;

		private bool exclusive;

		private bool dummy;

		private bool hidden;

		private bool accept;

		private object item;

		private string[] actions;

		private int identifier;

		public Texture Icon
		{
			get
			{
				Asset asset = this.item as Asset;
				if (this.icon == null && asset != null)
				{
					return AssetDatabase.GetCachedIcon(asset.path);
				}
				return this.icon;
			}
			set
			{
				this.icon = value;
			}
		}

		public int Identifier
		{
			get
			{
				if (this.identifier == -1)
				{
					this.identifier = Provider.GenerateID();
				}
				return this.identifier;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public int Indent
		{
			get
			{
				return this.indent;
			}
			set
			{
				this.SetIntent(this, value);
			}
		}

		public object Item
		{
			get
			{
				return this.item;
			}
			set
			{
				this.item = value;
			}
		}

		public Asset Asset
		{
			get
			{
				return this.item as Asset;
			}
			set
			{
				this.item = value;
			}
		}

		public ChangeSet Change
		{
			get
			{
				return this.item as ChangeSet;
			}
			set
			{
				this.item = value;
			}
		}

		public bool Expanded
		{
			get
			{
				return this.expanded;
			}
			set
			{
				this.expanded = value;
			}
		}

		public bool Exclusive
		{
			get
			{
				return this.exclusive;
			}
			set
			{
				this.exclusive = value;
			}
		}

		public bool Dummy
		{
			get
			{
				return this.dummy;
			}
			set
			{
				this.dummy = value;
			}
		}

		public bool Hidden
		{
			get
			{
				return this.hidden;
			}
			set
			{
				this.hidden = value;
			}
		}

		public bool HasChildren
		{
			get
			{
				return this.FirstChild != null;
			}
		}

		public bool HasActions
		{
			get
			{
				return this.actions != null && this.actions.Length != 0;
			}
		}

		public string[] Actions
		{
			get
			{
				return this.actions;
			}
			set
			{
				this.actions = value;
			}
		}

		public bool CanExpand
		{
			get
			{
				return this.item is ChangeSet || this.HasChildren;
			}
		}

		public bool CanAccept
		{
			get
			{
				return this.accept;
			}
			set
			{
				this.accept = value;
			}
		}

		public int OpenCount
		{
			get
			{
				if (!this.Expanded)
				{
					return 0;
				}
				int num = 0;
				for (ListItem listItem = this.firstChild; listItem != null; listItem = listItem.next)
				{
					if (!listItem.Hidden)
					{
						num++;
						num += listItem.OpenCount;
					}
				}
				return num;
			}
		}

		public int ChildCount
		{
			get
			{
				int num = 0;
				for (ListItem listItem = this.firstChild; listItem != null; listItem = listItem.next)
				{
					num++;
				}
				return num;
			}
		}

		public ListItem Parent
		{
			get
			{
				return this.parent;
			}
		}

		public ListItem FirstChild
		{
			get
			{
				return this.firstChild;
			}
		}

		public ListItem LastChild
		{
			get
			{
				return this.lastChild;
			}
		}

		public ListItem Prev
		{
			get
			{
				return this.prev;
			}
		}

		public ListItem Next
		{
			get
			{
				return this.next;
			}
		}

		public ListItem PrevOpen
		{
			get
			{
				for (ListItem listItem = this.prev; listItem != null; listItem = listItem.lastChild)
				{
					if (listItem.lastChild == null || !listItem.Expanded)
					{
						return listItem;
					}
				}
				if (this.parent != null && this.parent.parent != null)
				{
					return this.parent;
				}
				return null;
			}
		}

		public ListItem NextOpen
		{
			get
			{
				if (this.Expanded && this.firstChild != null)
				{
					return this.firstChild;
				}
				if (this.next != null)
				{
					return this.next;
				}
				for (ListItem listItem = this.parent; listItem != null; listItem = listItem.parent)
				{
					if (listItem.Next != null)
					{
						return listItem.Next;
					}
				}
				return null;
			}
		}

		public ListItem PrevOpenSkip
		{
			get
			{
				ListItem prevOpen = this.PrevOpen;
				while (prevOpen != null && (prevOpen.Dummy || prevOpen.Hidden))
				{
					prevOpen = prevOpen.PrevOpen;
				}
				return prevOpen;
			}
		}

		public ListItem NextOpenSkip
		{
			get
			{
				ListItem nextOpen = this.NextOpen;
				while (nextOpen != null && (nextOpen.Dummy || nextOpen.Hidden))
				{
					nextOpen = nextOpen.NextOpen;
				}
				return nextOpen;
			}
		}

		public ListItem PrevOpenVisible
		{
			get
			{
				ListItem prevOpen = this.PrevOpen;
				while (prevOpen != null && prevOpen.Hidden)
				{
					prevOpen = prevOpen.PrevOpen;
				}
				return prevOpen;
			}
		}

		public ListItem NextOpenVisible
		{
			get
			{
				ListItem nextOpen = this.NextOpen;
				while (nextOpen != null && nextOpen.Hidden)
				{
					nextOpen = nextOpen.NextOpen;
				}
				return nextOpen;
			}
		}

		public ListItem()
		{
			this.Clear();
			this.identifier = -1;
		}

		~ListItem()
		{
			this.Clear();
		}

		public bool HasPath()
		{
			Asset asset = this.item as Asset;
			return asset != null && asset.path != null;
		}

		public bool IsChildOf(ListItem listItem)
		{
			for (ListItem listItem2 = this.Parent; listItem2 != null; listItem2 = listItem2.Parent)
			{
				if (listItem2 == listItem)
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			this.parent = null;
			this.firstChild = null;
			this.lastChild = null;
			this.prev = null;
			this.next = null;
			this.icon = null;
			this.name = string.Empty;
			this.indent = 0;
			this.expanded = false;
			this.exclusive = false;
			this.dummy = false;
			this.accept = false;
			this.item = null;
		}

		public void Add(ListItem listItem)
		{
			listItem.parent = this;
			listItem.next = null;
			listItem.prev = this.lastChild;
			listItem.Indent = this.indent + 1;
			if (this.firstChild == null)
			{
				this.firstChild = listItem;
			}
			if (this.lastChild != null)
			{
				this.lastChild.next = listItem;
			}
			this.lastChild = listItem;
		}

		public bool Remove(ListItem listItem)
		{
			if (listItem == null)
			{
				return false;
			}
			if (listItem.parent != this)
			{
				return false;
			}
			if (listItem == this.firstChild)
			{
				this.firstChild = listItem.next;
			}
			if (listItem == this.lastChild)
			{
				this.lastChild = listItem.prev;
			}
			if (listItem.prev != null)
			{
				listItem.prev.next = listItem.next;
			}
			if (listItem.next != null)
			{
				listItem.next.prev = listItem.prev;
			}
			listItem.parent = null;
			listItem.prev = null;
			listItem.next = null;
			return true;
		}

		public void RemoveAll()
		{
			for (ListItem listItem = this.firstChild; listItem != null; listItem = listItem.next)
			{
				listItem.parent = null;
			}
			this.firstChild = null;
			this.lastChild = null;
		}

		public ListItem FindWithIdentifierRecurse(int inIdentifier)
		{
			if (this.Identifier == inIdentifier)
			{
				return this;
			}
			for (ListItem listItem = this.firstChild; listItem != null; listItem = listItem.next)
			{
				ListItem listItem2 = listItem.FindWithIdentifierRecurse(inIdentifier);
				if (listItem2 != null)
				{
					return listItem2;
				}
			}
			return null;
		}

		private void SetIntent(ListItem listItem, int indent)
		{
			listItem.indent = indent;
			for (ListItem listItem2 = listItem.FirstChild; listItem2 != null; listItem2 = listItem2.Next)
			{
				this.SetIntent(listItem2, indent + 1);
			}
		}
	}
}
