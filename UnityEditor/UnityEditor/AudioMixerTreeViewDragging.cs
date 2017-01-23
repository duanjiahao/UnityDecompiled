using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEditor.IMGUI.Controls;
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

		public AudioMixerTreeViewDragging(TreeViewController treeView, Action<List<AudioMixerController>, AudioMixerController> mixerDroppedOnMixerCallback) : base(treeView)
		{
			this.m_MixersDroppedOnMixerCallback = mixerDroppedOnMixerCallback;
		}

		public override void StartDrag(TreeViewItem draggedNode, List<int> draggedNodes)
		{
			if (!EditorApplication.isPlaying)
			{
				List<AudioMixerItem> audioMixerItemsFromIDs = this.GetAudioMixerItemsFromIDs(draggedNodes);
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.SetGenericData("AudioMixerDragging", new AudioMixerTreeViewDragging.DragData(audioMixerItemsFromIDs));
				DragAndDrop.objectReferences = new UnityEngine.Object[0];
				string title = draggedNodes.Count + " AudioMixer" + ((draggedNodes.Count <= 1) ? "" : "s");
				DragAndDrop.StartDrag(title);
			}
		}

		public override bool DragElement(TreeViewItem targetItem, Rect targetItemRect, bool firstItem)
		{
			AudioMixerTreeViewDragging.DragData dragData = DragAndDrop.GetGenericData("AudioMixerDragging") as AudioMixerTreeViewDragging.DragData;
			bool result;
			if (dragData == null)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.None;
				result = false;
			}
			else
			{
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
					result = false;
				}
				else
				{
					result = base.DragElement(targetItem, targetItemRect, firstItem);
				}
			}
			return result;
		}

		public override DragAndDropVisualMode DoDrag(TreeViewItem parentNode, TreeViewItem targetNode, bool perform, TreeViewDragging.DropPosition dragPos)
		{
			AudioMixerTreeViewDragging.DragData dragData = DragAndDrop.GetGenericData("AudioMixerDragging") as AudioMixerTreeViewDragging.DragData;
			DragAndDropVisualMode result;
			if (dragData == null)
			{
				result = DragAndDropVisualMode.None;
			}
			else
			{
				List<AudioMixerItem> draggedItems = dragData.m_DraggedItems;
				AudioMixerItem audioMixerItem = parentNode as AudioMixerItem;
				if (audioMixerItem != null && dragData != null)
				{
					List<AudioMixerGroupController> groupsToBeMoved = (from i in draggedItems
					select i.mixer.masterGroup).ToList<AudioMixerGroupController>();
					List<AudioMixerGroupController> allAudioGroupsSlow = audioMixerItem.mixer.GetAllAudioGroupsSlow();
					bool flag = AudioMixerController.WillModificationOfTopologyCauseFeedback(allAudioGroupsSlow, groupsToBeMoved, audioMixerItem.mixer.masterGroup, null);
					bool flag2 = this.ValidDrag(parentNode, draggedItems) && !flag;
					if (perform && flag2)
					{
						if (this.m_MixersDroppedOnMixerCallback != null)
						{
							this.m_MixersDroppedOnMixerCallback(this.GetAudioMixersFromItems(draggedItems), audioMixerItem.mixer);
						}
					}
					result = ((!flag2) ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Move);
				}
				else
				{
					result = DragAndDropVisualMode.None;
				}
			}
			return result;
		}

		private bool ValidDrag(TreeViewItem parent, List<AudioMixerItem> draggedItems)
		{
			List<int> list = (from n in draggedItems
			select n.id).ToList<int>();
			bool result;
			for (TreeViewItem treeViewItem = parent; treeViewItem != null; treeViewItem = treeViewItem.parent)
			{
				if (list.Contains(treeViewItem.id))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
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
