using System;
using UnityEngine;
namespace UnityEditor
{
	internal class GameObjectTreeViewItem : TreeViewItem
	{
		private int m_ColorCode;
		private UnityEngine.Object m_ObjectPPTR;
		private bool m_ShouldDisplay;
		public override string displayName
		{
			get
			{
				if (string.IsNullOrEmpty(base.displayName))
				{
					if (this.m_ObjectPPTR != null)
					{
						this.displayName = this.objectPPTR.name;
					}
					else
					{
						this.displayName = "deleted gameobject";
					}
				}
				return base.displayName;
			}
			set
			{
				base.displayName = value;
			}
		}
		public virtual int colorCode
		{
			get
			{
				return this.m_ColorCode;
			}
			set
			{
				this.m_ColorCode = value;
			}
		}
		public virtual UnityEngine.Object objectPPTR
		{
			get
			{
				return this.m_ObjectPPTR;
			}
			set
			{
				this.m_ObjectPPTR = value;
			}
		}
		public virtual bool shouldDisplay
		{
			get
			{
				return this.m_ShouldDisplay;
			}
			set
			{
				this.m_ShouldDisplay = value;
			}
		}
		public GameObjectTreeViewItem(int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
		{
		}
	}
}
