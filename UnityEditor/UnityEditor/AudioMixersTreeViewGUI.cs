using System;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixersTreeViewGUI : TreeViewGUI
	{
		public AudioMixersTreeViewGUI(TreeViewController treeView) : base(treeView)
		{
			this.k_IconWidth = 0f;
			this.k_TopRowMargin = (this.k_BottomRowMargin = 2f);
		}

		protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (!isPinging)
				{
					float contentIndent = this.GetContentIndent(item);
					rect.x += contentIndent;
					rect.width -= contentIndent;
				}
				AudioMixerItem audioMixerItem = item as AudioMixerItem;
				if (audioMixerItem != null)
				{
					GUIStyle gUIStyle = (!useBoldFont) ? TreeViewGUI.s_Styles.lineStyle : TreeViewGUI.s_Styles.lineBoldStyle;
					gUIStyle.padding.left = (int)(this.k_IconWidth + base.iconTotalPadding + this.k_SpaceBetweenIconAndText);
					gUIStyle.Draw(rect, label, false, false, selected, focused);
					audioMixerItem.UpdateSuspendedString(false);
					if (audioMixerItem.labelWidth <= 0f)
					{
						audioMixerItem.labelWidth = gUIStyle.CalcSize(GUIContent.Temp(label)).x;
					}
					Rect position = rect;
					position.x += audioMixerItem.labelWidth + 8f;
					using (new EditorGUI.DisabledScope(true))
					{
						gUIStyle.Draw(position, audioMixerItem.infoText, false, false, false, false);
					}
					if (base.iconOverlayGUI != null)
					{
						Rect arg = rect;
						arg.width = this.k_IconWidth + base.iconTotalPadding;
						base.iconOverlayGUI(item, arg);
					}
				}
			}
		}

		protected override Texture GetIconForItem(TreeViewItem node)
		{
			return null;
		}

		protected CreateAssetUtility GetCreateAssetUtility()
		{
			return this.m_TreeView.state.createAssetUtility;
		}

		protected override void RenameEnded()
		{
			string name = (!string.IsNullOrEmpty(base.GetRenameOverlay().name)) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
			int userData = base.GetRenameOverlay().userData;
			bool flag = this.GetCreateAssetUtility().IsCreatingNewAsset();
			bool userAcceptedRename = base.GetRenameOverlay().userAcceptedRename;
			if (userAcceptedRename)
			{
				if (flag)
				{
					this.GetCreateAssetUtility().EndNewAssetCreation(name);
					this.m_TreeView.ReloadData();
				}
				else
				{
					ObjectNames.SetNameSmartWithInstanceID(userData, name);
				}
			}
		}

		protected override void ClearRenameAndNewItemState()
		{
			this.GetCreateAssetUtility().Clear();
			base.ClearRenameAndNewItemState();
		}

		private AudioMixerItem GetSelectedItem()
		{
			return this.m_TreeView.FindItem(this.m_TreeView.GetSelection().FirstOrDefault<int>()) as AudioMixerItem;
		}

		protected override void SyncFakeItem()
		{
			if (!this.m_TreeView.data.HasFakeItem() && this.GetCreateAssetUtility().IsCreatingNewAsset())
			{
				int id = this.m_TreeView.data.root.id;
				AudioMixerItem selectedItem = this.GetSelectedItem();
				if (selectedItem != null)
				{
					id = selectedItem.parent.id;
				}
				this.m_TreeView.data.InsertFakeItem(this.GetCreateAssetUtility().instanceID, id, this.GetCreateAssetUtility().originalName, this.GetCreateAssetUtility().icon);
			}
			if (this.m_TreeView.data.HasFakeItem() && !this.GetCreateAssetUtility().IsCreatingNewAsset())
			{
				this.m_TreeView.data.RemoveFakeItem();
			}
		}

		public void BeginCreateNewMixer()
		{
			this.ClearRenameAndNewItemState();
			string newAssetResourceFile = string.Empty;
			AudioMixerItem selectedItem = this.GetSelectedItem();
			if (selectedItem != null && selectedItem.mixer.outputAudioMixerGroup != null)
			{
				newAssetResourceFile = selectedItem.mixer.outputAudioMixerGroup.GetInstanceID().ToString();
			}
			int num = 0;
			if (this.GetCreateAssetUtility().BeginNewAssetCreation(num, ScriptableObject.CreateInstance<DoCreateAudioMixer>(), "NewAudioMixer.mixer", null, newAssetResourceFile))
			{
				this.SyncFakeItem();
				if (!base.GetRenameOverlay().BeginRename(this.GetCreateAssetUtility().originalName, num, 0f))
				{
					Debug.LogError("Rename not started (when creating new asset)");
				}
			}
		}
	}
}
