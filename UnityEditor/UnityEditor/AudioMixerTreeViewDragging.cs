using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerTreeViewDragging : TreeViewDragging
	{
		private class DragData
		{
			public List<AudioMixerItem> m_DraggedItems;

			public DragData(List<AudioMixerItem> draggedItems)
			{
				this.m_DraggedItems = draggedItems;
			}
		}

		private const string k_AudioMixerDraggingID = "AudioMixerDragging";

		private Action<List<AudioMixerController>, AudioMixerController> m_MixersDroppedOnMixerCallback;

		public AudioMixerTreeViewDragging(TreeView treeView, Action<List<AudioMixerController>, AudioMixerController> mixerDroppedOnMixerCallback) : base(treeView)
		{
			this.m_MixersDroppedOnMixerCallback = mixerDroppedOnMixerCallback;
		}

		public override void StartDrag(TreeViewItem draggedNode, List<int> draggedNodes)
		{
			if (EditorApplication.isPlaying)
			{
				return;
			}
			List<AudioMixerItem> audioMixerItemsFromIDs = this.GetAudioMixerItemsFromIDs(draggedNodes);
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.SetGenericData("AudioMixerDragging", new AudioMixerTreeViewDragging.DragData(audioMixerItemsFromIDs));
			DragAndDrop.objectReferences = new UnityEngine.Object[0];
			string title = draggedNodes.Count + " AudioMixer" + ((draggedNodes.Count <= 1) ? string.Empty : "s");
			DragAndDrop.StartDrag(title);
		}

		public override bool DragElement(TreeViewItem targetItem, Rect targetItemRect, bool firstItem)
		{
			AudioMixerTreeViewDragging.DragData dragData = DragAndDrop.GetGenericData("AudioMixerDragging") as AudioMixerTreeViewDragging.DragData;
			if (dragData == null)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.None;
				return false;
			}
			bool flag = targetItem == null;
			if (flag && this.m_TreeView.GetTotalRect().Contains(Event.current.mousePosition))
			{
				if (this.m_DropData != null)
				{
					this.m_DropData.dropTargetControlID = 0;
					this.m_DropData.rowMarkerControlID = 0;
				}
				if (Event.current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					if (this.m_MixersDroppedOnMixerCallback != null)
					{
						this.m_MixersDroppedOnMixerCallback(this.GetAudioMixersFromItems(dragData.m_DraggedItems), null);
					}
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Move;
				Event.current.Use();
				return false;
			}
			return base.DragElement(targetItem, targetItemRect, firstItem);
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentNode, TreeViewItem targetNode, bool perform, TreeViewDragging.DropPosition dragPos)
		{
			AudioMixerTreeViewDragging.DragData dragData = DragAndDrop.GetGenericData("AudioMixerDragging") as AudioMixerTreeViewDragging.DragData;
			if (dragData == null)
			{
				return DragAndDropVisualMode.None;
			}
			List<AudioMixerItem> draggedItems = dragData.m_DraggedItems;
			AudioMixerItem audioMixerItem = parentNode as AudioMixerItem;
			if (audioMixerItem != null && dragData != null)
			{
				List<AudioMixerGroupController> groupsToBeMoved = (from i in draggedItems
				select i.mixer.masterGroup).ToList<AudioMixerGroupController>();
				List<AudioMixerGroupController> allAudioGroupsSlow = audioMixerItem.mixer.GetAllAudioGroupsSlow();
				bool flag = AudioMixerController.WillModificationOfTopologyCauseFeedback(allAudioGroupsSlow, groupsToBeMoved, audioMixerItem.mixer.masterGroup, null);
				bool flag2 = this.ValidDrag(parentNode, draggedItems) && !flag;
				if (perform && flag2 && this.m_MixersDroppedOnMixerCallback != null)
				{
					this.m_MixersDroppedOnMixerCallback(this.GetAudioMixersFromItems(draggedItems), audioMixerItem.mixer);
				}
				return (!flag2) ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Move;
			}
			return DragAndDropVisualMode.None;
		}

		private bool ValidDrag(TreeViewItem parent, List<AudioMixerItem> draggedItems)
		{
			List<int> list = (from n in draggedItems
			select n.id).ToList<int>();
			for (TreeViewItem treeViewItem = parent; treeViewItem != null; treeViewItem = treeViewItem.parent)
			{
				if (list.Contains(treeViewItem.id))
				{
					return false;
				}
			}
			return true;
		}

		private List<AudioMixerItem> GetAudioMixerItemsFromIDs(List<int> draggedMixers)
		{
			List<TreeViewItem> source = TreeViewUtility.FindItemsInList(draggedMixers, this.m_TreeView.data.GetRows());
			return source.OfType<AudioMixerItem>().ToList<AudioMixerItem>();
		}

		private List<AudioMixerController> GetAudioMixersFromItems(List<AudioMixerItem> draggedItems)
		{
			return (from i in draggedItems
			select i.mixer).ToList<AudioMixerController>();
		}
	}
}
