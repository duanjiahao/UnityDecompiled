using System;
using System.Collections.Generic;
using UnityEditor.Audio;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerSnapshotListView
	{
		private class Styles
		{
			public GUIContent starIcon = new GUIContent(EditorGUIUtility.FindTexture("Favorite"), "Start snapshot");

			public GUIContent header = new GUIContent("Snapshots", "A snapshot is a set of values for all parameters in the mixer. When using the mixer you modify parameters in the selected snapshot. Blend between multiple snapshots at runtime.");

			public GUIContent addButton = new GUIContent("+");

			public Texture2D snapshotsIcon = EditorGUIUtility.FindTexture("AudioMixerSnapshot Icon");
		}

		internal class SnapshotMenu
		{
			private class data
			{
				public AudioMixerSnapshotController snapshot;

				public AudioMixerSnapshotListView list;
			}

			public static void Show(Rect buttonRect, AudioMixerSnapshotController snapshot, AudioMixerSnapshotListView list)
			{
				GenericMenu genericMenu = new GenericMenu();
				AudioMixerSnapshotListView.SnapshotMenu.data userData = new AudioMixerSnapshotListView.SnapshotMenu.data
				{
					snapshot = snapshot,
					list = list
				};
				genericMenu.AddItem(new GUIContent("Set as start Snapshot"), false, new GenericMenu.MenuFunction2(AudioMixerSnapshotListView.SnapshotMenu.SetAsStartupSnapshot), userData);
				genericMenu.AddSeparator(string.Empty);
				genericMenu.AddItem(new GUIContent("Rename"), false, new GenericMenu.MenuFunction2(AudioMixerSnapshotListView.SnapshotMenu.Rename), userData);
				genericMenu.AddItem(new GUIContent("Duplicate"), false, new GenericMenu.MenuFunction2(AudioMixerSnapshotListView.SnapshotMenu.Duplicate), userData);
				genericMenu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction2(AudioMixerSnapshotListView.SnapshotMenu.Delete), userData);
				genericMenu.DropDown(buttonRect);
			}

			private static void SetAsStartupSnapshot(object userData)
			{
				AudioMixerSnapshotListView.SnapshotMenu.data data = userData as AudioMixerSnapshotListView.SnapshotMenu.data;
				data.list.SetAsStartupSnapshot(data.snapshot);
			}

			private static void Rename(object userData)
			{
				AudioMixerSnapshotListView.SnapshotMenu.data data = userData as AudioMixerSnapshotListView.SnapshotMenu.data;
				data.list.Rename(data.snapshot);
			}

			private static void Duplicate(object userData)
			{
				AudioMixerSnapshotListView.SnapshotMenu.data data = userData as AudioMixerSnapshotListView.SnapshotMenu.data;
				data.list.DuplicateCurrentSnapshot();
			}

			private static void Delete(object userData)
			{
				AudioMixerSnapshotListView.SnapshotMenu.data data = userData as AudioMixerSnapshotListView.SnapshotMenu.data;
				data.list.DeleteSnapshot(data.snapshot);
			}
		}

		private ReorderableListWithRenameAndScrollView m_ReorderableListWithRenameAndScrollView;

		private AudioMixerController m_Controller;

		private List<AudioMixerSnapshotController> m_Snapshots;

		private ReorderableListWithRenameAndScrollView.State m_State;

		private static AudioMixerSnapshotListView.Styles s_Styles;

		public AudioMixerSnapshotListView(ReorderableListWithRenameAndScrollView.State state)
		{
			this.m_State = state;
		}

		public void OnMixerControllerChanged(AudioMixerController controller)
		{
			this.m_Controller = controller;
			this.RecreateListControl();
		}

		private int GetSnapshotIndex(AudioMixerSnapshotController snapshot)
		{
			for (int i = 0; i < this.m_Snapshots.Count; i++)
			{
				if (this.m_Snapshots[i] == snapshot)
				{
					return i;
				}
			}
			return 0;
		}

		private void RecreateListControl()
		{
			if (this.m_Controller == null)
			{
				return;
			}
			this.m_Snapshots = new List<AudioMixerSnapshotController>(this.m_Controller.snapshots);
			this.m_ReorderableListWithRenameAndScrollView = new ReorderableListWithRenameAndScrollView(new ReorderableList(this.m_Snapshots, typeof(AudioMixerSnapshotController), true, false, false, false)
			{
				onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild),
				elementHeight = 16f,
				headerHeight = 0f,
				footerHeight = 0f,
				showDefaultBackground = false,
				index = this.GetSnapshotIndex(this.m_Controller.TargetSnapshot)
			}, this.m_State);
			ReorderableListWithRenameAndScrollView expr_AB = this.m_ReorderableListWithRenameAndScrollView;
			expr_AB.onSelectionChanged = (Action<int>)Delegate.Combine(expr_AB.onSelectionChanged, new Action<int>(this.SelectionChanged));
			ReorderableListWithRenameAndScrollView expr_D2 = this.m_ReorderableListWithRenameAndScrollView;
			expr_D2.onNameChangedAtIndex = (Action<int, string>)Delegate.Combine(expr_D2.onNameChangedAtIndex, new Action<int, string>(this.NameChanged));
			ReorderableListWithRenameAndScrollView expr_F9 = this.m_ReorderableListWithRenameAndScrollView;
			expr_F9.onDeleteItemAtIndex = (Action<int>)Delegate.Combine(expr_F9.onDeleteItemAtIndex, new Action<int>(this.Delete));
			ReorderableListWithRenameAndScrollView expr_120 = this.m_ReorderableListWithRenameAndScrollView;
			expr_120.onGetNameAtIndex = (Func<int, string>)Delegate.Combine(expr_120.onGetNameAtIndex, new Func<int, string>(this.GetNameOfElement));
			ReorderableListWithRenameAndScrollView expr_147 = this.m_ReorderableListWithRenameAndScrollView;
			expr_147.onCustomDrawElement = (ReorderableList.ElementCallbackDelegate)Delegate.Combine(expr_147.onCustomDrawElement, new ReorderableList.ElementCallbackDelegate(this.CustomDrawElement));
		}

		private void SaveToBackend()
		{
			this.m_Controller.snapshots = this.m_Snapshots.ToArray();
			this.m_Controller.OnSubAssetChanged();
		}

		public void LoadFromBackend()
		{
			if (this.m_Controller == null)
			{
				return;
			}
			this.m_Snapshots.Clear();
			this.m_Snapshots.AddRange(this.m_Controller.snapshots);
		}

		public void OnEvent()
		{
			if (this.m_Controller == null)
			{
				return;
			}
			this.m_ReorderableListWithRenameAndScrollView.OnEvent();
		}

		public void CustomDrawElement(Rect r, int index, bool isActive, bool isFocused)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseUp && current.button == 1 && r.Contains(current.mousePosition))
			{
				AudioMixerSnapshotListView.SnapshotMenu.Show(r, this.m_Snapshots[index], this);
				current.Use();
			}
			bool isSelected = index == this.m_ReorderableListWithRenameAndScrollView.list.index && !this.m_ReorderableListWithRenameAndScrollView.IsRenamingIndex(index);
			r.width -= 19f;
			this.m_ReorderableListWithRenameAndScrollView.DrawElementText(r, index, isActive, isSelected, isFocused);
			if (this.m_Controller.startSnapshot == this.m_Snapshots[index])
			{
				r.x = r.xMax + 5f + 5f;
				r.y += (r.height - 14f) / 2f;
				float num = 14f;
				r.height = num;
				r.width = num;
				GUI.Label(r, AudioMixerSnapshotListView.s_Styles.starIcon, GUIStyle.none);
			}
		}

		public float GetTotalHeight()
		{
			if (this.m_Controller == null)
			{
				return 0f;
			}
			return this.m_ReorderableListWithRenameAndScrollView.list.GetHeight() + 22f;
		}

		public void OnGUI(Rect rect)
		{
			if (AudioMixerSnapshotListView.s_Styles == null)
			{
				AudioMixerSnapshotListView.s_Styles = new AudioMixerSnapshotListView.Styles();
			}
			Rect r;
			Rect rect2;
			using (new EditorGUI.DisabledScope(this.m_Controller == null))
			{
				AudioMixerDrawUtils.DrawRegionBg(rect, out r, out rect2);
				AudioMixerDrawUtils.HeaderLabel(r, AudioMixerSnapshotListView.s_Styles.header, AudioMixerSnapshotListView.s_Styles.snapshotsIcon);
			}
			if (this.m_Controller != null)
			{
				int snapshotIndex = this.GetSnapshotIndex(this.m_Controller.TargetSnapshot);
				if (snapshotIndex != this.m_ReorderableListWithRenameAndScrollView.list.index)
				{
					this.m_ReorderableListWithRenameAndScrollView.list.index = snapshotIndex;
					this.m_ReorderableListWithRenameAndScrollView.FrameItem(snapshotIndex);
				}
				this.m_ReorderableListWithRenameAndScrollView.OnGUI(rect2);
				if (GUI.Button(new Rect(r.xMax - 15f, r.y + 3f, 15f, 15f), AudioMixerSnapshotListView.s_Styles.addButton, EditorStyles.label))
				{
					this.Add();
				}
			}
		}

		public void SelectionChanged(int index)
		{
			if (index >= this.m_Snapshots.Count)
			{
				index = this.m_Snapshots.Count - 1;
			}
			this.m_Controller.TargetSnapshot = this.m_Snapshots[index];
			this.UpdateViews();
		}

		private string GetNameOfElement(int index)
		{
			return this.m_Snapshots[index].name;
		}

		public void NameChanged(int index, string newName)
		{
			this.m_Snapshots[index].name = newName;
			this.SaveToBackend();
		}

		private void DuplicateCurrentSnapshot()
		{
			Undo.RecordObject(this.m_Controller, "Duplicate current snapshot");
			this.m_Controller.CloneNewSnapshotFromTarget(true);
			this.LoadFromBackend();
			this.UpdateViews();
		}

		private void Add()
		{
			Undo.RecordObject(this.m_Controller, "Add new snapshot");
			this.m_Controller.CloneNewSnapshotFromTarget(true);
			this.LoadFromBackend();
			this.Rename(this.m_Controller.TargetSnapshot);
			this.UpdateViews();
		}

		private void DeleteSnapshot(AudioMixerSnapshotController snapshot)
		{
			AudioMixerSnapshotController[] snapshots = this.m_Controller.snapshots;
			if (snapshots.Length <= 1)
			{
				Debug.Log("You must have at least 1 snapshot in an AudioMixer.");
				return;
			}
			this.m_Controller.RemoveSnapshot(snapshot);
			this.LoadFromBackend();
			this.m_ReorderableListWithRenameAndScrollView.list.index = this.GetSnapshotIndex(this.m_Controller.TargetSnapshot);
			this.UpdateViews();
		}

		private void Delete(int index)
		{
			this.DeleteSnapshot(this.m_Snapshots[index]);
		}

		public void EndDragChild(ReorderableList list)
		{
			this.m_Snapshots = (this.m_ReorderableListWithRenameAndScrollView.list.list as List<AudioMixerSnapshotController>);
			this.SaveToBackend();
		}

		private void UpdateViews()
		{
			AudioMixerWindow audioMixerWindow = (AudioMixerWindow)WindowLayout.FindEditorWindowOfType(typeof(AudioMixerWindow));
			if (audioMixerWindow != null)
			{
				audioMixerWindow.Repaint();
			}
			InspectorWindow.RepaintAllInspectors();
		}

		private void SetAsStartupSnapshot(AudioMixerSnapshotController snapshot)
		{
			this.m_Controller.startSnapshot = snapshot;
		}

		private void Rename(AudioMixerSnapshotController snapshot)
		{
			this.m_ReorderableListWithRenameAndScrollView.BeginRename(this.GetSnapshotIndex(snapshot), 0f);
		}

		public void OnUndoRedoPerformed()
		{
			this.LoadFromBackend();
		}
	}
}
