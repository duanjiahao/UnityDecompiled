using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEditor
{
	internal class SerializedPropertyTable
	{
		private static class Styles
		{
			public static readonly GUIStyle DragHandle = "RL DragHandle";
		}

		internal delegate SerializedPropertyTreeView.Column[] HeaderDelegate(out string[] propNames);

		private SerializedPropertyDataStore.GatherDelegate m_GatherDelegate;

		private SerializedPropertyTable.HeaderDelegate m_HeaderDelegate;

		private bool m_Initialized;

		private TreeViewState m_TreeViewState;

		private MultiColumnHeaderState m_MultiColumnHeaderState;

		private SerializedPropertyTreeView m_TreeView;

		private SerializedPropertyDataStore m_DataStore;

		private float m_ColumnHeaderHeight;

		private float m_TableHeight = 200f;

		private string m_SerializationUID;

		private static readonly string s_TableHeight = "_TableHeight";

		private bool m_DragHandleEnabled = false;

		private readonly float m_FilterHeight = 20f;

		private readonly float m_DragHeight = 20f;

		private readonly float m_DragWidth = 32f;

		public bool dragHandleEnabled
		{
			get
			{
				return this.m_DragHandleEnabled;
			}
			set
			{
				this.m_DragHandleEnabled = value;
			}
		}

		public SerializedPropertyTable(string serializationUID, SerializedPropertyDataStore.GatherDelegate gatherDelegate, SerializedPropertyTable.HeaderDelegate headerDelegate)
		{
			this.m_SerializationUID = serializationUID;
			this.m_GatherDelegate = gatherDelegate;
			this.m_HeaderDelegate = headerDelegate;
			this.OnEnable();
		}

		private void InitIfNeeded()
		{
			if (!this.m_Initialized)
			{
				if (this.m_TreeViewState == null)
				{
					this.m_TreeViewState = new TreeViewState();
				}
				if (this.m_MultiColumnHeaderState == null)
				{
					string[] propNames;
					this.m_MultiColumnHeaderState = new MultiColumnHeaderState(this.m_HeaderDelegate(out propNames));
					this.m_DataStore = new SerializedPropertyDataStore(propNames, this.m_GatherDelegate);
				}
				MultiColumnHeader multiColumnHeader = new MultiColumnHeader(this.m_MultiColumnHeaderState);
				this.m_ColumnHeaderHeight = multiColumnHeader.height;
				this.m_TreeView = new SerializedPropertyTreeView(this.m_TreeViewState, multiColumnHeader, this.m_DataStore);
				this.m_TreeView.DeserializeState(this.m_SerializationUID);
				this.m_TreeView.Reload();
				this.m_Initialized = true;
			}
		}

		private float GetMinHeight()
		{
			float singleLineHeight = EditorGUIUtility.singleLineHeight;
			float num = this.m_FilterHeight + this.m_ColumnHeaderHeight + singleLineHeight + this.m_DragHeight;
			return num + singleLineHeight * 3f;
		}

		public void OnInspectorUpdate()
		{
			if (this.m_DataStore != null && this.m_DataStore.Repopulate() && this.m_TreeView != null)
			{
				this.m_TreeView.FullReload();
			}
			else if (this.m_TreeView != null && this.m_TreeView.Update())
			{
				this.m_TreeView.Repaint();
			}
		}

		public void OnHierarchyChange()
		{
			if (this.m_DataStore != null && this.m_DataStore.Repopulate() && this.m_TreeView != null)
			{
				this.m_TreeView.FullReload();
			}
		}

		public void OnSelectionChange()
		{
			this.OnSelectionChange(Selection.instanceIDs);
		}

		public void OnSelectionChange(int[] instanceIDs)
		{
			if (this.m_TreeView != null)
			{
				this.m_TreeView.SetSelection(instanceIDs);
			}
		}

		public void OnGUI()
		{
			Profiler.BeginSample("SerializedPropertyTable.OnGUI");
			this.InitIfNeeded();
			Rect rect;
			if (this.dragHandleEnabled)
			{
				rect = GUILayoutUtility.GetRect(0f, 10000f, this.m_TableHeight, this.m_TableHeight);
			}
			else
			{
				rect = GUILayoutUtility.GetRect(0f, 3.40282347E+38f, 0f, 3.40282347E+38f);
			}
			if (Event.current.type != EventType.Layout)
			{
				float width = rect.width;
				float num = rect.height - this.m_FilterHeight - ((!this.dragHandleEnabled) ? 0f : this.m_DragHeight);
				float height = rect.height;
				rect.height = this.m_FilterHeight;
				Rect r = rect;
				rect.height = num;
				rect.y += this.m_FilterHeight;
				Rect rect2 = rect;
				Profiler.BeginSample("TreeView.OnGUI");
				this.m_TreeView.OnGUI(rect2);
				Profiler.EndSample();
				if (this.dragHandleEnabled)
				{
					rect.y += num + 1f;
					rect.height = 1f;
					Rect position = rect;
					rect.height = 10f;
					rect.y += 10f;
					rect.x += (rect.width - this.m_DragWidth) * 0.5f;
					rect.width = this.m_DragWidth;
					this.m_TableHeight = EditorGUI.HeightResizer(rect, this.m_TableHeight, this.GetMinHeight(), 3.40282347E+38f);
					if (this.m_MultiColumnHeaderState.widthOfAllVisibleColumns <= width)
					{
						Rect texCoords = new Rect(0f, 1f, 1f, 1f - 1f / (float)EditorStyles.inspectorTitlebar.normal.background.height);
						GUI.DrawTextureWithTexCoords(position, EditorStyles.inspectorTitlebar.normal.background, texCoords);
					}
					if (Event.current.type == EventType.Repaint)
					{
						SerializedPropertyTable.Styles.DragHandle.Draw(rect, false, false, false, false);
					}
				}
				this.m_TreeView.OnFilterGUI(r);
				if (this.m_TreeView.IsFilteredDirty())
				{
					this.m_TreeView.Reload();
				}
				Profiler.EndSample();
			}
		}

		public void OnEnable()
		{
			this.m_TableHeight = SessionState.GetFloat(this.m_SerializationUID + SerializedPropertyTable.s_TableHeight, 200f);
		}

		public void OnDisable()
		{
			if (this.m_TreeView != null)
			{
				this.m_TreeView.SerializeState(this.m_SerializationUID);
			}
			SessionState.SetFloat(this.m_SerializationUID + SerializedPropertyTable.s_TableHeight, this.m_TableHeight);
		}
	}
}
