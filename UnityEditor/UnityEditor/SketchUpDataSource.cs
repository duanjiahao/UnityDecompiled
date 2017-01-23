using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	internal class SketchUpDataSource : TreeViewDataSource
	{
		private SketchUpNodeInfo[] m_Nodes;

		private const int k_ProgressUpdateStep = 50;

		public SketchUpDataSource(TreeViewController treeView, SketchUpNodeInfo[] nodes) : base(treeView)
		{
			this.m_Nodes = nodes;
			this.FetchData();
		}

		public int[] FetchEnableNodes()
		{
			List<int> list = new List<int>();
			this.InternalFetchEnableNodes(this.m_RootItem as SketchUpNode, list);
			return list.ToArray();
		}

		private void InternalFetchEnableNodes(SketchUpNode node, List<int> enable)
		{
			if (node.Enabled)
			{
				enable.Add(node.Info.nodeIndex);
			}
			foreach (TreeViewItem current in node.children)
			{
				this.InternalFetchEnableNodes(current as SketchUpNode, enable);
			}
		}

		public override void FetchData()
		{
			this.m_RootItem = new SketchUpNode(this.m_Nodes[0].nodeIndex, 0, null, this.m_Nodes[0].name, this.m_Nodes[0]);
			List<SketchUpNode> list = new List<SketchUpNode>();
			list.Add(this.m_RootItem as SketchUpNode);
			this.SetExpanded(this.m_RootItem, true);
			int nodeIndex = this.m_Nodes[0].nodeIndex;
			for (int i = 1; i < this.m_Nodes.Length; i++)
			{
				SketchUpNodeInfo info = this.m_Nodes[i];
				if (info.parent >= 0 && info.parent <= list.Count)
				{
					if (info.parent >= i)
					{
						Debug.LogError("Parent node index is greater than child node");
					}
					else if (nodeIndex >= info.nodeIndex)
					{
						Debug.LogError("Node array is not sorted by nodeIndex");
					}
					else
					{
						SketchUpNode sketchUpNode = list[info.parent];
						SketchUpNode sketchUpNode2 = new SketchUpNode(info.nodeIndex, sketchUpNode.depth + 1, sketchUpNode, info.name, info);
						sketchUpNode.children.Add(sketchUpNode2);
						this.SetExpanded(sketchUpNode2, sketchUpNode2.Info.enabled);
						list.Add(sketchUpNode2);
						if (i % 50 == 0)
						{
							EditorUtility.DisplayProgressBar("SketchUp Import", "Building Node Selection", (float)i / (float)this.m_Nodes.Length);
						}
					}
				}
			}
			EditorUtility.ClearProgressBar();
			this.m_NeedRefreshRows = true;
		}

		public override bool CanBeParent(TreeViewItem item)
		{
			return item.hasChildren;
		}

		public override bool IsRenamingItemAllowed(TreeViewItem item)
		{
			return false;
		}
	}
}
