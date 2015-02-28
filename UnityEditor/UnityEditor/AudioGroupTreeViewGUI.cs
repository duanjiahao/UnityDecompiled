using System;
using UnityEditor.Audio;
using UnityEngine;
namespace UnityEditor
{
	internal class AudioGroupTreeViewGUI : TreeViewGUI
	{
		private readonly float column1Width = 20f;
		private readonly Texture2D k_VisibleON = EditorGUIUtility.FindTexture("VisibilityOn");
		public Action<AudioMixerTreeViewNode, bool> NodeWasToggled;
		public AudioMixerController m_Controller;
		public AudioGroupTreeViewGUI(TreeView treeView) : base(treeView)
		{
			this.k_BaseIndent = this.column1Width;
			this.k_IconWidth = 0f;
			this.k_TopRowMargin = (this.k_BottomRowMargin = 2f);
		}
		private void OpenGroupContextMenu(AudioMixerTreeViewNode audioNode, bool visible)
		{
			GenericMenu genericMenu = new GenericMenu();
			if (this.NodeWasToggled != null)
			{
				genericMenu.AddItem(new GUIContent((!visible) ? "Show Group" : "Hide group"), false, delegate
				{
					this.NodeWasToggled(audioNode, !visible);
				});
			}
			genericMenu.AddSeparator(string.Empty);
			AudioMixerColorCodes.AddColorItemsToGenericMenu(genericMenu, audioNode.group);
			genericMenu.ShowAsContext();
		}
		public override Rect OnRowGUI(TreeViewItem node, int row, float rowWidth, bool selected, bool focused)
		{
			Event current = Event.current;
			Rect rect = new Rect(0f, (float)row * this.k_LineHeight + this.k_TopRowMargin, rowWidth, this.k_LineHeight);
			this.DoNodeGUI(rect, node, selected, focused, false);
			if (this.m_Controller == null)
			{
				return rect;
			}
			AudioMixerTreeViewNode audioMixerTreeViewNode = node as AudioMixerTreeViewNode;
			if (audioMixerTreeViewNode != null)
			{
				bool flag = this.m_Controller.CurrentViewContainsGroup(audioMixerTreeViewNode.group.groupID);
				float num = 3f;
				Rect position = new Rect(rect.x + num, rect.y, 16f, 16f);
				Rect rect2 = new Rect(position.x + 1f, position.y + 1f, position.width - 2f, position.height - 2f);
				int userColorIndex = audioMixerTreeViewNode.group.userColorIndex;
				if (userColorIndex > 0)
				{
					EditorGUI.DrawRect(new Rect(rect.x, rect2.y, 2f, rect2.height), AudioMixerColorCodes.GetColor(userColorIndex));
				}
				EditorGUI.DrawRect(rect2, new Color(0.5f, 0.5f, 0.5f, 0.2f));
				if (flag)
				{
					GUI.DrawTexture(position, this.k_VisibleON);
				}
				Rect rect3 = new Rect(2f, rect.y, rect.height, rect.height);
				if (current.type == EventType.MouseDown && current.button == 0 && rect3.Contains(current.mousePosition) && this.NodeWasToggled != null)
				{
					this.NodeWasToggled(audioMixerTreeViewNode, !flag);
				}
				if (current.type == EventType.ContextClick && position.Contains(current.mousePosition))
				{
					this.OpenGroupContextMenu(audioMixerTreeViewNode, flag);
					current.Use();
				}
			}
			return rect;
		}
		protected override Texture GetIconForNode(TreeViewItem node)
		{
			if (node != null && node.icon != null)
			{
				return node.icon;
			}
			return null;
		}
		protected override void SyncFakeItem()
		{
		}
		protected override void RenameEnded()
		{
			bool userAcceptedRename = base.GetRenameOverlay().userAcceptedRename;
			if (userAcceptedRename)
			{
				string name = (!string.IsNullOrEmpty(base.GetRenameOverlay().name)) ? base.GetRenameOverlay().name : base.GetRenameOverlay().originalName;
				int userData = base.GetRenameOverlay().userData;
				AudioMixerTreeViewNode audioMixerTreeViewNode = this.m_TreeView.FindNode(userData) as AudioMixerTreeViewNode;
				if (audioMixerTreeViewNode != null)
				{
					ObjectNames.SetNameSmartWithInstanceID(userData, name);
					AudioMixerEffectController[] effects = audioMixerTreeViewNode.group.effects;
					for (int i = 0; i < effects.Length; i++)
					{
						AudioMixerEffectController audioMixerEffectController = effects[i];
						audioMixerEffectController.ClearCachedDisplayName();
					}
					this.m_TreeView.ReloadData();
					if (this.m_Controller != null)
					{
						this.m_Controller.OnSubAssetChanged();
					}
				}
			}
		}
	}
}
