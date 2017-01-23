using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioGroupTreeViewDragging : AssetsTreeViewDragging
	{
		private AudioMixerGroupTreeView m_owner;

		public AudioGroupTreeViewDragging(TreeViewController treeView, AudioMixerGroupTreeView owner) : base(treeView)
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
			AudioMixerTreeViewNode audioMixerTreeViewNode = parentNode as AudioMixerTreeViewNode;
			List<AudioMixerGroupController> list = new List<UnityEngine.Object>(DragAndDrop.objectReferences).OfType<AudioMixerGroupController>().ToList<AudioMixerGroupController>();
			DragAndDropVisualMode result;
			if (audioMixerTreeViewNode != null && list.Count > 0)
			{
				List<int> list2 = (from i in list
				select i.GetInstanceID()).ToList<int>();
				bool flag = this.ValidDrag(parentNode, list2) && !AudioMixerController.WillModificationOfTopologyCauseFeedback(this.m_owner.Controller.GetAllAudioGroupsSlow(), list, audioMixerTreeViewNode.group, null);
				if (perform && flag)
				{
					AudioMixerGroupController group = audioMixerTreeViewNode.group;
					int insertionIndex = TreeViewDragging.GetInsertionIndex(parentNode, targetNode, dragPos);
					this.m_owner.Controller.ReparentSelection(group, insertionIndex, list);
					this.m_owner.ReloadTree();
					this.m_TreeView.SetSelection(list2.ToArray(), true);
				}
				result = ((!flag) ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Move);
			}
			else
			{
				result = DragAndDropVisualMode.None;
			}
			return result;
		}

		private bool ValidDrag(TreeViewItem parent, List<int> draggedInstanceIDs)
		{
			bool result;
			for (TreeViewItem treeViewItem = parent; treeViewItem != null; treeViewItem = treeViewItem.parent)
			{
				if (draggedInstanceIDs.Contains(treeViewItem.id))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
	}
}
