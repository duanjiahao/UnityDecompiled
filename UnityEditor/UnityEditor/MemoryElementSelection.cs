using System;

namespace UnityEditor
{
	[Serializable]
	internal class MemoryElementSelection
	{
		private MemoryElement m_Selected = null;

		public MemoryElement Selected
		{
			get
			{
				return this.m_Selected;
			}
		}

		public void SetSelection(MemoryElement node)
		{
			this.m_Selected = node;
			for (MemoryElement parent = node.parent; parent != null; parent = parent.parent)
			{
				parent.expanded = true;
			}
		}

		public void ClearSelection()
		{
			this.m_Selected = null;
		}

		public bool isSelected(MemoryElement node)
		{
			return this.m_Selected == node;
		}

		public void MoveUp()
		{
			if (this.m_Selected != null)
			{
				if (this.m_Selected.parent != null)
				{
					MemoryElement prevNode = this.m_Selected.GetPrevNode();
					if (prevNode.parent != null)
					{
						this.SetSelection(prevNode);
					}
					else
					{
						this.SetSelection(prevNode.FirstChild());
					}
				}
			}
		}

		public void MoveDown()
		{
			if (this.m_Selected != null)
			{
				if (this.m_Selected.parent != null)
				{
					MemoryElement nextNode = this.m_Selected.GetNextNode();
					if (nextNode != null)
					{
						this.SetSelection(nextNode);
					}
				}
			}
		}

		public void MoveFirst()
		{
			if (this.m_Selected != null)
			{
				if (this.m_Selected.parent != null)
				{
					this.SetSelection(this.m_Selected.GetRoot().FirstChild());
				}
			}
		}

		public void MoveLast()
		{
			if (this.m_Selected != null)
			{
				if (this.m_Selected.parent != null)
				{
					this.SetSelection(this.m_Selected.GetRoot().LastChild());
				}
			}
		}

		public void MoveParent()
		{
			if (this.m_Selected != null)
			{
				if (this.m_Selected.parent != null)
				{
					if (this.m_Selected.parent.parent != null)
					{
						this.SetSelection(this.m_Selected.parent);
					}
				}
			}
		}
	}
}
