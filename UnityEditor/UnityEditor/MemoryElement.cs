using System;
using System.Collections.Generic;

namespace UnityEditor
{
	[Serializable]
	internal class MemoryElement
	{
		public List<MemoryElement> children = null;

		public MemoryElement parent = null;

		public ObjectInfo memoryInfo;

		public long totalMemory;

		public int totalChildCount;

		public string name;

		public bool expanded;

		public string description;

		public MemoryElement()
		{
			this.children = new List<MemoryElement>();
		}

		public MemoryElement(string n)
		{
			this.expanded = false;
			this.name = n;
			this.children = new List<MemoryElement>();
			this.description = "";
		}

		public MemoryElement(ObjectInfo memInfo, bool finalize)
		{
			this.expanded = false;
			this.memoryInfo = memInfo;
			this.name = this.memoryInfo.name;
			this.totalMemory = ((memInfo == null) ? 0L : memInfo.memorySize);
			this.totalChildCount = 1;
			if (finalize)
			{
				this.children = new List<MemoryElement>();
			}
		}

		public MemoryElement(string n, List<MemoryElement> groups)
		{
			this.name = n;
			this.expanded = false;
			this.description = "";
			this.totalMemory = 0L;
			this.totalChildCount = 0;
			this.children = new List<MemoryElement>();
			foreach (MemoryElement current in groups)
			{
				this.AddChild(current);
			}
		}

		public void ExpandChildren()
		{
			if (this.children == null)
			{
				this.children = new List<MemoryElement>();
				for (int i = 0; i < this.ReferenceCount(); i++)
				{
					this.AddChild(new MemoryElement(this.memoryInfo.referencedBy[i], false));
				}
			}
		}

		public int AccumulatedChildCount()
		{
			return this.totalChildCount;
		}

		public int ChildCount()
		{
			int result;
			if (this.children != null)
			{
				result = this.children.Count;
			}
			else
			{
				result = this.ReferenceCount();
			}
			return result;
		}

		public int ReferenceCount()
		{
			return (this.memoryInfo == null || this.memoryInfo.referencedBy == null) ? 0 : this.memoryInfo.referencedBy.Count;
		}

		public void AddChild(MemoryElement node)
		{
			if (node == this)
			{
				throw new Exception("Should not AddChild to itself");
			}
			this.children.Add(node);
			node.parent = this;
			this.totalMemory += node.totalMemory;
			this.totalChildCount += node.totalChildCount;
		}

		public int GetChildIndexInList()
		{
			int result;
			for (int i = 0; i < this.parent.children.Count; i++)
			{
				if (this.parent.children[i] == this)
				{
					result = i;
					return result;
				}
			}
			result = this.parent.children.Count;
			return result;
		}

		public MemoryElement GetPrevNode()
		{
			int num = this.GetChildIndexInList() - 1;
			MemoryElement result;
			if (num >= 0)
			{
				MemoryElement memoryElement = this.parent.children[num];
				while (memoryElement.expanded)
				{
					memoryElement = memoryElement.children[memoryElement.children.Count - 1];
				}
				result = memoryElement;
			}
			else
			{
				result = this.parent;
			}
			return result;
		}

		public MemoryElement GetNextNode()
		{
			MemoryElement result;
			if (this.expanded && this.children.Count > 0)
			{
				result = this.children[0];
			}
			else
			{
				int num = this.GetChildIndexInList() + 1;
				if (num < this.parent.children.Count)
				{
					result = this.parent.children[num];
				}
				else
				{
					MemoryElement memoryElement = this.parent;
					while (memoryElement.parent != null)
					{
						int num2 = memoryElement.GetChildIndexInList() + 1;
						if (num2 < memoryElement.parent.children.Count)
						{
							result = memoryElement.parent.children[num2];
							return result;
						}
						memoryElement = memoryElement.parent;
					}
					result = null;
				}
			}
			return result;
		}

		public MemoryElement GetRoot()
		{
			MemoryElement result;
			if (this.parent != null)
			{
				result = this.parent.GetRoot();
			}
			else
			{
				result = this;
			}
			return result;
		}

		public MemoryElement FirstChild()
		{
			return this.children[0];
		}

		public MemoryElement LastChild()
		{
			MemoryElement result;
			if (!this.expanded)
			{
				result = this;
			}
			else
			{
				result = this.children[this.children.Count - 1].LastChild();
			}
			return result;
		}
	}
}
