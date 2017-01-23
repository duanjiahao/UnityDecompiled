using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityEditor
{
	internal class AudioMixersTreeView
	{
		private class Styles
		{
			public GUIStyle optionsButton = "PaneOptions";

			public GUIContent header = new GUIContent("Mixers", "All mixers in the project are shown here. By default a mixer outputs to the AudioListener but mixers can also route their output to other mixers. Each mixer shows where it outputs (in parenthesis). To reroute a mixer simply drag the mixer upon another mixer and select a group from the popup.");

			public GUIContent addText = new GUIContent("+", "Add mixer asset. The asset will be saved in the same folder as the current selected mixer or if none is selected saved in the Assets folder.");

			public Texture2D audioMixerIcon = EditorGUIUtility.FindTexture("AudioMixerController Icon");
		}

		private TreeViewController m_TreeView;

		private int m_TreeViewKeyboardControlID;

		private const int kObjectSelectorID = 1212;

		private List<AudioMixerController> m_DraggedMixers;

		private static AudioMixersTreeView.Styles s_Styles;

		private const string kExpandedStateIdentifier = "AudioMixerWindowMixers";

		public AudioMixersTreeView(AudioMixerWindow mixerWindow, TreeViewState treeState, Func<List<AudioMixerController>> getAllControllersCallback)
		{
			this.m_TreeView = new TreeViewController(mixerWindow, treeState);
			this.m_TreeView.deselectOnUnhandledMouseDown = false;
			TreeViewController expr_26 = this.m_TreeView;
			expr_26.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_26.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
			TreeViewController expr_4D = this.m_TreeView;
			expr_4D.contextClickItemCallback = (Action<int>)Delegate.Combine(expr_4D.contextClickItemCallback, new Action<int>(this.OnTreeViewContextClick));
			AudioMixersTreeViewGUI gui = new AudioMixersTreeViewGUI(this.m_TreeView);
			AudioMixersDataSource data = new AudioMixersDataSource(this.m_TreeView, getAllControllersCallback);
			AudioMixerTreeViewDragging dragging = new AudioMixerTreeViewDragging(this.m_TreeView, new Action<List<AudioMixerController>, AudioMixerController>(this.OnMixersDroppedOnMixerCallback));
			this.m_TreeView.Init(mixerWindow.position, data, gui, dragging);
			this.m_TreeView.ReloadData();
		}

		public void ReloadTree()
		{
			this.m_TreeView.ReloadData();
			this.m_TreeView.Repaint();
		}

		public void OnMixerControllerChanged(AudioMixerController controller)
		{
			if (controller != null)
			{
				this.m_TreeView.SetSelection(new int[]
				{
					controller.GetInstanceID()
				}, true);
			}
		}

		public void DeleteAudioMixerCallback(object obj)
		{
			AudioMixerController audioMixerController = (AudioMixerController)obj;
			if (audioMixerController != null)
			{
				Selection.activeObject = audioMixerController;
				ProjectBrowser.DeleteSelectedAssets(true);
			}
		}

		public void OnTreeViewContextClick(int index)
		{
			AudioMixerItem audioMixerItem = (AudioMixerItem)this.m_TreeView.FindItem(index);
			if (audioMixerItem != null)
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Delete AudioMixer"), false, new GenericMenu.MenuFunction2(this.DeleteAudioMixerCallback), audioMixerItem.mixer);
				genericMenu.ShowAsContext();
			}
		}

		public void OnTreeSelectionChanged(int[] selection)
		{
			Selection.instanceIDs = selection;
		}

		public float GetTotalHeight()
		{
			return 22f + Mathf.Max(20f, this.m_TreeView.gui.GetTotalSize().y);
		}

		public void OnGUI(Rect rect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			if (AudioMixersTreeView.s_Styles == null)
			{
				AudioMixersTreeView.s_Styles = new AudioMixersTreeView.Styles();
			}
			this.m_TreeView.OnEvent();
			Rect r;
			Rect rect2;
			AudioMixerDrawUtils.DrawRegionBg(rect, out r, out rect2);
			AudioMixerDrawUtils.HeaderLabel(r, AudioMixersTreeView.s_Styles.header, AudioMixersTreeView.s_Styles.audioMixerIcon);
			if (GUI.Button(new Rect(r.xMax - 15f, r.y + 3f, 15f, 15f), AudioMixersTreeView.s_Styles.addText, EditorStyles.label))
			{
				AudioMixersTreeViewGUI audioMixersTreeViewGUI = this.m_TreeView.gui as AudioMixersTreeViewGUI;
				audioMixersTreeViewGUI.BeginCreateNewMixer();
			}
			this.m_TreeView.OnGUI(rect2, controlID);
			if (this.m_TreeView.data.rowCount == 0)
			{
				using (new EditorGUI.DisabledScope(true))
				{
					GUI.Label(new RectOffset(-20, 0, -2, 0).Add(rect2), "No mixers found");
				}
			}
			AudioMixerDrawUtils.DrawScrollDropShadow(rect2, this.m_TreeView.state.scrollPos.y, this.m_TreeView.gui.GetTotalSize().y);
			this.HandleCommandEvents(controlID);
			this.HandleObjectSelectorResult();
		}

		private void HandleCommandEvents(int treeViewKeyboardControlID)
		{
			if (GUIUtility.keyboardControl == treeViewKeyboardControlID)
			{
				EventType type = Event.current.type;
				if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
				{
					bool flag = type == EventType.ExecuteCommand;
					if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
					{
						Event.current.Use();
						if (flag)
						{
							ProjectBrowser.DeleteSelectedAssets(true);
						}
					}
					else if (Event.current.commandName == "Duplicate")
					{
						Event.current.Use();
						if (flag)
						{
							ProjectWindowUtil.DuplicateSelectedAssets();
						}
					}
				}
			}
		}

		public void EndRenaming()
		{
			this.m_TreeView.EndNameEditing(true);
		}

		public void OnUndoRedoPerformed()
		{
			this.ReloadTree();
		}

		private void OnMixersDroppedOnMixerCallback(List<AudioMixerController> draggedMixers, AudioMixerController droppedUponMixer)
		{
			int[] array = (from i in draggedMixers
			select i.GetInstanceID()).ToArray<int>();
			this.m_TreeView.SetSelection(array, true);
			Selection.instanceIDs = array;
			if (droppedUponMixer == null)
			{
				Undo.RecordObjects(draggedMixers.ToArray(), "Set output group for mixer" + ((draggedMixers.Count <= 1) ? "" : "s"));
				foreach (AudioMixerController current in draggedMixers)
				{
					current.outputAudioMixerGroup = null;
				}
				this.ReloadTree();
			}
			else
			{
				this.m_DraggedMixers = draggedMixers;
				UnityEngine.Object obj = (draggedMixers.Count != 1) ? null : draggedMixers[0].outputAudioMixerGroup;
				ObjectSelector.get.Show(obj, typeof(AudioMixerGroup), null, false, new List<int>
				{
					droppedUponMixer.GetInstanceID()
				});
				ObjectSelector.get.objectSelectorID = 1212;
				ObjectSelector.get.titleContent = new GUIContent("Select Output Audio Mixer Group");
				GUIUtility.ExitGUI();
			}
		}

		private void HandleObjectSelectorResult()
		{
			Event current = Event.current;
			if (current.type == EventType.ExecuteCommand)
			{
				string commandName = current.commandName;
				if (commandName == "ObjectSelectorUpdated" && ObjectSelector.get.objectSelectorID == 1212)
				{
					if (this.m_DraggedMixers == null || this.m_DraggedMixers.Count == 0)
					{
						Debug.LogError("Unexpected invalid mixer list used for dragging");
					}
					UnityEngine.Object currentObject = ObjectSelector.GetCurrentObject();
					AudioMixerGroup outputAudioMixerGroup = (!(currentObject != null)) ? null : (currentObject as AudioMixerGroup);
					Undo.RecordObjects(this.m_DraggedMixers.ToArray(), "Set output group for mixer" + ((this.m_DraggedMixers.Count <= 1) ? "" : "s"));
					foreach (AudioMixerController current2 in this.m_DraggedMixers)
					{
						if (current2 != null)
						{
							current2.outputAudioMixerGroup = outputAudioMixerGroup;
						}
						else
						{
							Debug.LogError("invalid mixer: is null");
						}
					}
					GUI.changed = true;
					current.Use();
					this.ReloadTree();
					int[] selectedIDs = (from i in this.m_DraggedMixers
					select i.GetInstanceID()).ToArray<int>();
					this.m_TreeView.SetSelection(selectedIDs, true);
				}
				if (commandName == "ObjectSelectorClosed")
				{
					this.m_DraggedMixers = null;
				}
			}
		}
	}
}
