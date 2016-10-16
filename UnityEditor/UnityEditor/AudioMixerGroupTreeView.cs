using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerGroupTreeView
	{
		private class Styles
		{
			public GUIStyle optionsButton = "PaneOptions";

			public GUIContent header = new GUIContent("Groups", "An Audio Mixer Group is used by e.g Audio Sources to modify the audio output before it reaches the Audio Listener. An Audio Mixer Group will route its output to another Audio Mixer Group if it is made a child of that group. The Master Group will route its output to the Audio Listener if it doesn't route its output into another Mixer.");

			public GUIContent addText = new GUIContent("+", "Add child group");

			public Texture2D audioMixerGroupIcon = EditorGUIUtility.FindTexture("AudioMixerGroup Icon");
		}

		private AudioMixerController m_Controller;

		private AudioGroupDataSource m_AudioGroupTreeDataSource;

		private TreeViewState m_AudioGroupTreeState;

		private TreeView m_AudioGroupTree;

		private int m_TreeViewKeyboardControlID;

		private AudioGroupTreeViewGUI m_TreeViewGUI;

		private AudioMixerGroupController m_ScrollToItem;

		private static AudioMixerGroupTreeView.Styles s_Styles;

		public AudioMixerController Controller
		{
			get
			{
				return this.m_Controller;
			}
		}

		public AudioMixerGroupController ScrollToItem
		{
			get
			{
				return this.m_ScrollToItem;
			}
		}

		public AudioMixerGroupTreeView(AudioMixerWindow mixerWindow, TreeViewState treeState)
		{
			this.m_AudioGroupTreeState = treeState;
			this.m_AudioGroupTree = new TreeView(mixerWindow, this.m_AudioGroupTreeState);
			this.m_AudioGroupTree.deselectOnUnhandledMouseDown = false;
			TreeView expr_31 = this.m_AudioGroupTree;
			expr_31.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_31.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
			TreeView expr_58 = this.m_AudioGroupTree;
			expr_58.contextClickItemCallback = (Action<int>)Delegate.Combine(expr_58.contextClickItemCallback, new Action<int>(this.OnTreeViewContextClick));
			TreeView expr_7F = this.m_AudioGroupTree;
			expr_7F.expandedStateChanged = (Action)Delegate.Combine(expr_7F.expandedStateChanged, new Action(this.SaveExpandedState));
			this.m_TreeViewGUI = new AudioGroupTreeViewGUI(this.m_AudioGroupTree);
			AudioGroupTreeViewGUI expr_B7 = this.m_TreeViewGUI;
			expr_B7.NodeWasToggled = (Action<AudioMixerTreeViewNode, bool>)Delegate.Combine(expr_B7.NodeWasToggled, new Action<AudioMixerTreeViewNode, bool>(this.OnNodeToggled));
			this.m_AudioGroupTreeDataSource = new AudioGroupDataSource(this.m_AudioGroupTree, this.m_Controller);
			this.m_AudioGroupTree.Init(mixerWindow.position, this.m_AudioGroupTreeDataSource, this.m_TreeViewGUI, new AudioGroupTreeViewDragging(this.m_AudioGroupTree, this));
			this.m_AudioGroupTree.ReloadData();
		}

		public void UseScrollView(bool useScrollView)
		{
			this.m_AudioGroupTree.SetUseScrollView(useScrollView);
		}

		public void ReloadTreeData()
		{
			this.m_AudioGroupTree.ReloadData();
		}

		public void ReloadTree()
		{
			this.m_AudioGroupTree.ReloadData();
			if (this.m_Controller != null)
			{
				this.m_Controller.SanitizeGroupViews();
				this.m_Controller.OnSubAssetChanged();
			}
		}

		public void AddChildGroupPopupCallback(object obj)
		{
			AudioMixerGroupPopupContext audioMixerGroupPopupContext = (AudioMixerGroupPopupContext)obj;
			if (audioMixerGroupPopupContext.groups != null && audioMixerGroupPopupContext.groups.Length > 0)
			{
				this.AddAudioMixerGroup(audioMixerGroupPopupContext.groups[0]);
			}
		}

		public void AddSiblingGroupPopupCallback(object obj)
		{
			AudioMixerGroupPopupContext audioMixerGroupPopupContext = (AudioMixerGroupPopupContext)obj;
			if (audioMixerGroupPopupContext.groups != null && audioMixerGroupPopupContext.groups.Length > 0)
			{
				AudioMixerTreeViewNode audioMixerTreeViewNode = this.m_AudioGroupTree.FindItem(audioMixerGroupPopupContext.groups[0].GetInstanceID()) as AudioMixerTreeViewNode;
				if (audioMixerTreeViewNode != null)
				{
					AudioMixerTreeViewNode audioMixerTreeViewNode2 = audioMixerTreeViewNode.parent as AudioMixerTreeViewNode;
					this.AddAudioMixerGroup(audioMixerTreeViewNode2.group);
				}
			}
		}

		public void AddAudioMixerGroup(AudioMixerGroupController parent)
		{
			if (parent == null || this.m_Controller == null)
			{
				return;
			}
			Undo.RecordObjects(new UnityEngine.Object[]
			{
				this.m_Controller,
				parent
			}, "Add Child Group");
			AudioMixerGroupController audioMixerGroupController = this.m_Controller.CreateNewGroup("New Group", true);
			this.m_Controller.AddChildToParent(audioMixerGroupController, parent);
			this.m_Controller.AddGroupToCurrentView(audioMixerGroupController);
			Selection.objects = new AudioMixerGroupController[]
			{
				audioMixerGroupController
			};
			this.m_Controller.OnUnitySelectionChanged();
			this.m_AudioGroupTree.SetSelection(new int[]
			{
				audioMixerGroupController.GetInstanceID()
			}, true);
			this.ReloadTree();
			this.m_AudioGroupTree.BeginNameEditing(0f);
		}

		private static string PluralIfNeeded(int count)
		{
			return (count <= 1) ? string.Empty : "s";
		}

		public void DeleteGroups(List<AudioMixerGroupController> groups, bool recordUndo)
		{
			foreach (AudioMixerGroupController current in groups)
			{
				if (current.HasDependentMixers())
				{
					if (!EditorUtility.DisplayDialog("Referenced Group", "Deleted group is referenced by another AudioMixer, are you sure?", "Delete", "Cancel"))
					{
						return;
					}
					break;
				}
			}
			if (recordUndo)
			{
				Undo.RegisterCompleteObjectUndo(this.m_Controller, "Delete Group" + AudioMixerGroupTreeView.PluralIfNeeded(groups.Count));
			}
			this.m_Controller.DeleteGroups(groups.ToArray());
			this.ReloadTree();
		}

		public void DuplicateGroups(List<AudioMixerGroupController> groups, bool recordUndo)
		{
			if (recordUndo)
			{
				Undo.RecordObject(this.m_Controller, "Duplicate group" + AudioMixerGroupTreeView.PluralIfNeeded(groups.Count));
			}
			List<AudioMixerGroupController> list = this.m_Controller.DuplicateGroups(groups.ToArray());
			if (list.Count > 0)
			{
				this.ReloadTree();
				int[] array = (from audioMixerGroup in list
				select audioMixerGroup.GetInstanceID()).ToArray<int>();
				this.m_AudioGroupTree.SetSelection(array, false);
				this.m_AudioGroupTree.Frame(array[array.Length - 1], true, false);
			}
		}

		private void DeleteGroupsPopupCallback(object obj)
		{
			AudioMixerGroupTreeView audioMixerGroupTreeView = (AudioMixerGroupTreeView)obj;
			audioMixerGroupTreeView.DeleteGroups(this.GetGroupSelectionWithoutMasterGroup(), true);
		}

		private void DuplicateGroupPopupCallback(object obj)
		{
			AudioMixerGroupTreeView audioMixerGroupTreeView = (AudioMixerGroupTreeView)obj;
			audioMixerGroupTreeView.DuplicateGroups(this.GetGroupSelectionWithoutMasterGroup(), true);
		}

		private void RenameGroupCallback(object obj)
		{
			TreeViewItem treeViewItem = (TreeViewItem)obj;
			this.m_AudioGroupTree.SetSelection(new int[]
			{
				treeViewItem.id
			}, false);
			this.m_AudioGroupTree.BeginNameEditing(0f);
		}

		private List<AudioMixerGroupController> GetGroupSelectionWithoutMasterGroup()
		{
			List<AudioMixerGroupController> audioMixerGroupsFromNodeIDs = this.GetAudioMixerGroupsFromNodeIDs(this.m_AudioGroupTree.GetSelection());
			audioMixerGroupsFromNodeIDs.Remove(this.m_Controller.masterGroup);
			return audioMixerGroupsFromNodeIDs;
		}

		public void OnTreeViewContextClick(int index)
		{
			TreeViewItem treeViewItem = this.m_AudioGroupTree.FindItem(index);
			if (treeViewItem != null)
			{
				AudioMixerTreeViewNode audioMixerTreeViewNode = treeViewItem as AudioMixerTreeViewNode;
				if (audioMixerTreeViewNode != null && audioMixerTreeViewNode.group != null)
				{
					GenericMenu genericMenu = new GenericMenu();
					if (!EditorApplication.isPlaying)
					{
						genericMenu.AddItem(new GUIContent("Add child group"), false, new GenericMenu.MenuFunction2(this.AddChildGroupPopupCallback), new AudioMixerGroupPopupContext(this.m_Controller, audioMixerTreeViewNode.group));
						if (audioMixerTreeViewNode.group != this.m_Controller.masterGroup)
						{
							genericMenu.AddItem(new GUIContent("Add sibling group"), false, new GenericMenu.MenuFunction2(this.AddSiblingGroupPopupCallback), new AudioMixerGroupPopupContext(this.m_Controller, audioMixerTreeViewNode.group));
							genericMenu.AddSeparator(string.Empty);
							genericMenu.AddItem(new GUIContent("Rename"), false, new GenericMenu.MenuFunction2(this.RenameGroupCallback), treeViewItem);
							AudioMixerGroupController[] array = this.GetGroupSelectionWithoutMasterGroup().ToArray();
							genericMenu.AddItem(new GUIContent((array.Length <= 1) ? "Duplicate group (and children)" : "Duplicate groups (and children)"), false, new GenericMenu.MenuFunction2(this.DuplicateGroupPopupCallback), this);
							genericMenu.AddItem(new GUIContent((array.Length <= 1) ? "Remove group (and children)" : "Remove groups (and children)"), false, new GenericMenu.MenuFunction2(this.DeleteGroupsPopupCallback), this);
						}
					}
					else
					{
						genericMenu.AddDisabledItem(new GUIContent("Modifying group topology in play mode is not allowed"));
					}
					genericMenu.ShowAsContext();
				}
			}
		}

		private void OnNodeToggled(AudioMixerTreeViewNode node, bool nodeWasEnabled)
		{
			List<AudioMixerGroupController> list = this.GetAudioMixerGroupsFromNodeIDs(this.m_AudioGroupTree.GetSelection());
			if (!list.Contains(node.group))
			{
				list = new List<AudioMixerGroupController>
				{
					node.group
				};
			}
			List<GUID> list2 = new List<GUID>();
			List<AudioMixerGroupController> allAudioGroupsSlow = this.m_Controller.GetAllAudioGroupsSlow();
			foreach (AudioMixerGroupController current in allAudioGroupsSlow)
			{
				bool flag = this.m_Controller.CurrentViewContainsGroup(current.groupID);
				bool flag2 = list.Contains(current);
				bool flag3 = flag && !flag2;
				if (!flag && flag2)
				{
					flag3 = nodeWasEnabled;
				}
				if (flag3)
				{
					list2.Add(current.groupID);
				}
			}
			this.m_Controller.SetCurrentViewVisibility(list2.ToArray());
		}

		private List<AudioMixerGroupController> GetAudioMixerGroupsFromNodeIDs(int[] instanceIDs)
		{
			List<AudioMixerGroupController> list = new List<AudioMixerGroupController>();
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int id = instanceIDs[i];
				TreeViewItem treeViewItem = this.m_AudioGroupTree.FindItem(id);
				if (treeViewItem != null)
				{
					AudioMixerTreeViewNode audioMixerTreeViewNode = treeViewItem as AudioMixerTreeViewNode;
					if (audioMixerTreeViewNode != null)
					{
						list.Add(audioMixerTreeViewNode.group);
					}
				}
			}
			return list;
		}

		public void OnTreeSelectionChanged(int[] selection)
		{
			List<AudioMixerGroupController> audioMixerGroupsFromNodeIDs = this.GetAudioMixerGroupsFromNodeIDs(selection);
			Selection.objects = audioMixerGroupsFromNodeIDs.ToArray();
			this.m_Controller.OnUnitySelectionChanged();
			if (audioMixerGroupsFromNodeIDs.Count == 1)
			{
				this.m_ScrollToItem = audioMixerGroupsFromNodeIDs[0];
			}
			InspectorWindow.RepaintAllInspectors();
		}

		public void InitSelection(bool revealSelectionAndFrameLastSelected)
		{
			if (this.m_Controller == null)
			{
				return;
			}
			List<AudioMixerGroupController> cachedSelection = this.m_Controller.CachedSelection;
			this.m_AudioGroupTree.SetSelection((from x in cachedSelection
			select x.GetInstanceID()).ToArray<int>(), revealSelectionAndFrameLastSelected);
		}

		public float GetTotalHeight()
		{
			if (this.m_Controller == null)
			{
				return 0f;
			}
			return this.m_AudioGroupTree.gui.GetTotalSize().y + 22f;
		}

		public void OnGUI(Rect rect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			this.m_ScrollToItem = null;
			if (AudioMixerGroupTreeView.s_Styles == null)
			{
				AudioMixerGroupTreeView.s_Styles = new AudioMixerGroupTreeView.Styles();
			}
			this.m_AudioGroupTree.OnEvent();
			Rect r;
			Rect rect2;
			using (new EditorGUI.DisabledScope(this.m_Controller == null))
			{
				AudioMixerDrawUtils.DrawRegionBg(rect, out r, out rect2);
				AudioMixerDrawUtils.HeaderLabel(r, AudioMixerGroupTreeView.s_Styles.header, AudioMixerGroupTreeView.s_Styles.audioMixerGroupIcon);
			}
			if (this.m_Controller != null)
			{
				AudioMixerGroupController parent = (this.m_Controller.CachedSelection.Count != 1) ? this.m_Controller.masterGroup : this.m_Controller.CachedSelection[0];
				using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
				{
					if (GUI.Button(new Rect(r.xMax - 15f, r.y + 3f, 15f, 15f), AudioMixerGroupTreeView.s_Styles.addText, EditorStyles.label))
					{
						this.AddAudioMixerGroup(parent);
					}
				}
				this.m_AudioGroupTree.OnGUI(rect2, controlID);
				AudioMixerDrawUtils.DrawScrollDropShadow(rect2, this.m_AudioGroupTree.state.scrollPos.y, this.m_AudioGroupTree.gui.GetTotalSize().y);
				this.HandleKeyboardEvents(controlID);
				this.HandleCommandEvents(controlID);
			}
		}

		private void HandleCommandEvents(int treeViewKeyboardControlID)
		{
			if (GUIUtility.keyboardControl != treeViewKeyboardControlID)
			{
				return;
			}
			EventType type = Event.current.type;
			if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
			{
				bool flag = type == EventType.ExecuteCommand;
				if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
				{
					Event.current.Use();
					if (flag)
					{
						this.DeleteGroups(this.GetGroupSelectionWithoutMasterGroup(), true);
						GUIUtility.ExitGUI();
					}
				}
				else if (Event.current.commandName == "Duplicate")
				{
					Event.current.Use();
					if (flag)
					{
						this.DuplicateGroups(this.GetGroupSelectionWithoutMasterGroup(), true);
					}
				}
			}
		}

		private void HandleKeyboardEvents(int treeViewKeyboardControlID)
		{
			if (GUIUtility.keyboardControl != treeViewKeyboardControlID)
			{
				return;
			}
			Event current = Event.current;
			if (current.keyCode == KeyCode.Space && current.type == EventType.KeyDown)
			{
				int[] selection = this.m_AudioGroupTree.GetSelection();
				if (selection.Length > 0)
				{
					AudioMixerTreeViewNode audioMixerTreeViewNode = this.m_AudioGroupTree.FindItem(selection[0]) as AudioMixerTreeViewNode;
					bool flag = this.m_Controller.CurrentViewContainsGroup(audioMixerTreeViewNode.group.groupID);
					this.OnNodeToggled(audioMixerTreeViewNode, !flag);
					current.Use();
				}
			}
		}

		public void OnMixerControllerChanged(AudioMixerController controller)
		{
			if (this.m_Controller != controller)
			{
				this.m_TreeViewGUI.m_Controller = controller;
				this.m_Controller = controller;
				this.m_AudioGroupTreeDataSource.m_Controller = controller;
				if (controller != null)
				{
					this.ReloadTree();
					this.InitSelection(false);
					this.LoadExpandedState();
					this.m_AudioGroupTree.data.SetExpandedWithChildren(this.m_AudioGroupTree.data.root, true);
				}
			}
		}

		private static string GetUniqueAudioMixerName(AudioMixerController controller)
		{
			return "AudioMixer_" + controller.GetInstanceID();
		}

		private void SaveExpandedState()
		{
			SessionState.SetIntArray(AudioMixerGroupTreeView.GetUniqueAudioMixerName(this.m_Controller), this.m_AudioGroupTreeState.expandedIDs.ToArray());
		}

		private void LoadExpandedState()
		{
			int[] intArray = SessionState.GetIntArray(AudioMixerGroupTreeView.GetUniqueAudioMixerName(this.m_Controller), null);
			if (intArray != null)
			{
				this.m_AudioGroupTreeState.expandedIDs = new List<int>(intArray);
			}
			else
			{
				this.m_AudioGroupTree.state.expandedIDs = new List<int>();
				this.m_AudioGroupTree.data.SetExpandedWithChildren(this.m_AudioGroupTree.data.root, true);
			}
		}

		public void EndRenaming()
		{
			this.m_AudioGroupTree.EndNameEditing(true);
		}

		public void OnUndoRedoPerformed()
		{
			this.ReloadTree();
		}
	}
}
