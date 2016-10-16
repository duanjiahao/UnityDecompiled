using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioGroupTreeViewDragging : AssetsTreeViewDragging
	{
		private AudioMixerGroupTreeView m_owner;

		public AudioGroupTreeViewDragging(TreeView treeView, AudioMixerGroupTreeView owner) : base(treeView)
		{
			this.m_owner = owner;
		}

		public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
		{
			if (!EditorApplication.isPlaying)
			{
				base.StartDrag(draggedItem, draggedItemIDs);
			}
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentNode, TreeViewItem targetNode, bool perform, TreeViewDragging.DropPosition dragPos)
		{
			AudioMixerTreeViewNode audioMixerTreeViewNode = targetNode as AudioMixerTreeViewNode;
			AudioMixerTreeViewNode audioMixerTreeViewNode2 = parentNode as AudioMixerTreeViewNode;
			List<AudioMixerGroupController> list = new List<UnityEngine.Object>(DragAndDrop.objectReferences).OfType<AudioMixerGroupController>().ToList<AudioMixerGroupController>();
			if (audioMixerTreeViewNode2 != null && list.Count > 0)
			{
				List<int> list2 = (from i in list
				select i.GetInstanceID()).ToList<int>();
				bool flag = this.ValidDrag(parentNode, list2) && !AudioMixerController.WillModificationOfTopologyCauseFeedback(this.m_owner.Controller.GetAllAudioGroupsSlow(), list, audioMixerTreeViewNode2.group, null);
				if (perform && flag)
				{
					AudioMixerGroupController group = audioMixerTreeViewNode2.group;
					this.m_owner.Controller.ReparentSelection(group, audioMixerTreeViewNode.group, list);
					this.m_owner.ReloadTree();
					this.m_TreeView.SetSelection(list2.ToArray(), true);
				}
				return (!flag) ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Move;
			}
			return DragAndDropVisualMode.None;
		}

		private bool ValidDrag(TreeViewItem parent, List<int> draggedInstanceIDs)
		{
			for (TreeViewItem treeViewItem = parent; treeViewItem != null; treeViewItem = treeViewItem.parent)
			{
				if (draggedInstanceIDs.Contains(treeViewItem.id))
				{
					return false;
				}
			}
			return true;
		}
	}
}
