using System;
using System.Collections.Generic;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioGroupDataSource : TreeViewDataSource
	{
		public AudioMixerController m_Controller;

		public AudioGroupDataSource(TreeView treeView, AudioMixerController controller) : base(treeView)
		{
			this.m_Controller = controller;
		}

		private void AddNodesRecursively(AudioMixerGroupController group, TreeViewItem parent, int depth)
		{
			List<TreeViewItem> list = new List<TreeViewItem>();
			for (int i = 0; i < group.children.Length; i++)
			{
				int uniqueNodeID = AudioGroupDataSource.GetUniqueNodeID(group.children[i]);
				AudioMixerTreeViewNode audioMixerTreeViewNode = new AudioMixerTreeViewNode(uniqueNodeID, depth, parent, group.children[i].name, group.children[i]);
				audioMixerTreeViewNode.parent = parent;
				list.Add(audioMixerTreeViewNode);
				this.AddNodesRecursively(group.children[i], audioMixerTreeViewNode, depth + 1);
			}
			parent.children = list;
		}

		public static int GetUniqueNodeID(AudioMixerGroupController group)
		{
			return group.GetInstanceID();
		}

		public override void FetchData()
		{
			if (this.m_Controller == null)
			{
				this.m_RootItem = null;
				return;
			}
			if (this.m_Controller.masterGroup == null)
			{
				Debug.LogError("The Master group is missing !!!");
				this.m_RootItem = null;
				return;
			}
			int uniqueNodeID = AudioGroupDataSource.GetUniqueNodeID(this.m_Controller.masterGroup);
			this.m_RootItem = new AudioMixerTreeViewNode(uniqueNodeID, 0, null, this.m_Controller.masterGroup.name, this.m_Controller.masterGroup);
			this.AddNodesRecursively(this.m_Controller.masterGroup, this.m_RootItem, 1);
			this.m_NeedRefreshVisibleFolders = true;
		}

		public override bool IsRenamingItemAllowed(TreeViewItem node)
		{
			AudioMixerTreeViewNode audioMixerTreeViewNode = node as AudioMixerTreeViewNode;
			return !(audioMixerTreeViewNode.group == this.m_Controller.masterGroup);
		}
	}
}
