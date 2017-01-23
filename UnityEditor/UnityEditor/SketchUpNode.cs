using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace UnityEditor
{
	internal class SketchUpNode : TreeViewItem
	{
		public SketchUpNodeInfo Info;

		public bool Enabled
		{
			get
			{
				return this.Info.enabled;
			}
			set
			{
				if (this.Info.enabled != value)
				{
					if (value)
					{
						this.ToggleParent(value);
					}
					this.ToggleChildren(value);
					this.Info.enabled = value;
				}
			}
		}

		public SketchUpNode(int id, int depth, TreeViewItem parent, string displayName, SketchUpNodeInfo info) : base(id, depth, parent, displayName)
		{
			this.Info = info;
			this.children = new List<TreeViewItem>();
		}

		private void ToggleParent(bool toggle)
		{
			SketchUpNode sketchUpNode = this.parent as SketchUpNode;
			if (sketchUpNode != null)
			{
				sketchUpNode.ToggleParent(toggle);
				sketchUpNode.Info.enabled = toggle;
			}
		}

		private void ToggleChildren(bool toggle)
		{
			foreach (TreeViewItem current in this.children)
			{
				SketchUpNode sketchUpNode = current as SketchUpNode;
				sketchUpNode.Info.enabled = toggle;
				sketchUpNode.ToggleChildren(toggle);
			}
		}
	}
}
