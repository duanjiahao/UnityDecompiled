using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Audio;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityEditor
{
	internal class AudioMixersDataSource : TreeViewDataSource
	{
		private Func<List<AudioMixerController>> m_GetAllControllersCallback;

		public AudioMixersDataSource(TreeViewController treeView, Func<List<AudioMixerController>> getAllControllersCallback) : base(treeView)
		{
			base.showRootItem = false;
			this.m_GetAllControllersCallback = getAllControllersCallback;
		}

		public override void FetchData()
		{
			int depth = -1;
			bool flag = this.m_TreeView.state.expandedIDs.Count == 0;
			this.m_RootItem = new TreeViewItem(1010101010, depth, null, "InvisibleRoot");
			this.SetExpanded(this.m_RootItem.id, true);
			List<AudioMixerController> list = this.m_GetAllControllersCallback();
			this.m_NeedRefreshRows = true;
			if (list.Count > 0)
			{
				List<AudioMixerItem> list2 = (from mixer in list
				select new AudioMixerItem(mixer.GetInstanceID(), 0, this.m_RootItem, mixer.name, mixer, AudioMixersDataSource.GetInfoText(mixer))).ToList<AudioMixerItem>();
				foreach (AudioMixerItem current in list2)
				{
					this.SetChildParentOfMixerItem(current, list2);
				}
				this.SetItemDepthRecursive(this.m_RootItem, -1);
				this.SortRecursive(this.m_RootItem);
				if (flag)
				{
					this.m_TreeView.data.SetExpandedWithChildren(this.m_RootItem, true);
				}
			}
		}

		private static string GetInfoText(AudioMixerController controller)
		{
			string result;
			if (controller.outputAudioMixerGroup != null)
			{
				result = string.Format("({0} of {1})", controller.outputAudioMixerGroup.name, controller.outputAudioMixerGroup.audioMixer.name);
			}
			else
			{
				result = "(Audio Listener)";
			}
			return result;
		}

		private void SetChildParentOfMixerItem(AudioMixerItem item, List<AudioMixerItem> items)
		{
			if (item.mixer.outputAudioMixerGroup != null)
			{
				AudioMixer audioMixer = item.mixer.outputAudioMixerGroup.audioMixer;
				AudioMixerItem audioMixerItem = TreeViewUtility.FindItemInList<AudioMixerItem>(audioMixer.GetInstanceID(), items) as AudioMixerItem;
				if (audioMixerItem != null)
				{
					audioMixerItem.AddChild(item);
				}
			}
			else
			{
				this.m_RootItem.AddChild(item);
			}
		}

		private void SetItemDepthRecursive(TreeViewItem item, int depth)
		{
			item.depth = depth;
			if (item.hasChildren)
			{
				foreach (TreeViewItem current in item.children)
				{
					this.SetItemDepthRecursive(current, depth + 1);
				}
			}
		}

		private void SortRecursive(TreeViewItem item)
		{
			if (item.hasChildren)
			{
				item.children.Sort(new TreeViewItemAlphaNumericSort());
				foreach (TreeViewItem current in item.children)
				{
					this.SortRecursive(current);
				}
			}
		}

		public override bool IsRenamingItemAllowed(TreeViewItem item)
		{
			return true;
		}

		public int GetInsertAfterItemIDForNewItem(string newName, TreeViewItem parentItem)
		{
			int num = parentItem.id;
			int result;
			if (!parentItem.hasChildren)
			{
				result = num;
			}
			else
			{
				for (int i = 0; i < parentItem.children.Count; i++)
				{
					int id = parentItem.children[i].id;
					string assetPath = AssetDatabase.GetAssetPath(id);
					if (EditorUtility.NaturalCompare(Path.GetFileNameWithoutExtension(assetPath), newName) > 0)
					{
						break;
					}
					num = id;
				}
				result = num;
			}
			return result;
		}

		public override void InsertFakeItem(int id, int parentID, string name, Texture2D icon)
		{
			TreeViewItem treeViewItem = this.FindItem(id);
			if (treeViewItem != null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Cannot insert fake Item because id is not unique ",
					id,
					" Item already there: ",
					treeViewItem.displayName
				}));
			}
			else if (this.FindItem(parentID) != null)
			{
				this.SetExpanded(parentID, true);
				IList<TreeViewItem> rows = this.GetRows();
				int indexOfID = TreeViewController.GetIndexOfID(rows, parentID);
				TreeViewItem treeViewItem2;
				if (indexOfID >= 0)
				{
					treeViewItem2 = rows[indexOfID];
				}
				else
				{
					treeViewItem2 = this.m_RootItem;
				}
				int num = treeViewItem2.depth + 1;
				this.m_FakeItem = new TreeViewItem(id, num, treeViewItem2, name);
				this.m_FakeItem.icon = icon;
				int insertAfterItemIDForNewItem = this.GetInsertAfterItemIDForNewItem(name, treeViewItem2);
				int num2 = TreeViewController.GetIndexOfID(rows, insertAfterItemIDForNewItem);
				if (num2 >= 0)
				{
					while (++num2 < rows.Count)
					{
						if (rows[num2].depth <= num)
						{
							break;
						}
					}
					if (num2 < rows.Count)
					{
						rows.Insert(num2, this.m_FakeItem);
					}
					else
					{
						rows.Add(this.m_FakeItem);
					}
				}
				else if (rows.Count > 0)
				{
					rows.Insert(0, this.m_FakeItem);
				}
				else
				{
					rows.Add(this.m_FakeItem);
				}
				this.m_NeedRefreshRows = false;
				this.m_TreeView.Frame(this.m_FakeItem.id, true, false);
				this.m_TreeView.Repaint();
			}
			else
			{
				Debug.LogError("No parent Item found with ID: " + parentID);
			}
		}
	}
}
