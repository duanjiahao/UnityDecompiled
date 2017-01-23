using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Audio;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerGroupViewList
	{
		private class Styles
		{
			public GUIContent header = new GUIContent("Views", "A view is the saved visiblity state of the current Mixer Groups. Use views to setup often used combinations of Mixer Groups.");

			public GUIContent addButton = new GUIContent("+");

			public Texture2D viewsIcon = EditorGUIUtility.FindTexture("AudioMixerView Icon");
		}

		internal class ViewsContexttMenu
		{
			private class data
			{
				public int viewIndex;

				public AudioMixerGroupViewList list;
			}

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache0;

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache1;

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache2;

			public static void Show(Rect buttonRect, int viewIndex, AudioMixerGroupViewList list)
			{
				GenericMenu genericMenu = new GenericMenu();
				AudioMixerGroupViewList.ViewsContexttMenu.data userData = new AudioMixerGroupViewList.ViewsContexttMenu.data
				{
					viewIndex = viewIndex,
					list = list
				};
				GenericMenu arg_47_0 = genericMenu;
				GUIContent arg_47_1 = new GUIContent("Rename");
				bool arg_47_2 = false;
				if (AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache0 == null)
				{
					AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache0 = new GenericMenu.MenuFunction2(AudioMixerGroupViewList.ViewsContexttMenu.Rename);
				}
				arg_47_0.AddItem(arg_47_1, arg_47_2, AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache0, userData);
				GenericMenu arg_76_0 = genericMenu;
				GUIContent arg_76_1 = new GUIContent("Duplicate");
				bool arg_76_2 = false;
				if (AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache1 == null)
				{
					AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache1 = new GenericMenu.MenuFunction2(AudioMixerGroupViewList.ViewsContexttMenu.Duplicate);
				}
				arg_76_0.AddItem(arg_76_1, arg_76_2, AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache1, userData);
				GenericMenu arg_A5_0 = genericMenu;
				GUIContent arg_A5_1 = new GUIContent("Delete");
				bool arg_A5_2 = false;
				if (AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache2 == null)
				{
					AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache2 = new GenericMenu.MenuFunction2(AudioMixerGroupViewList.ViewsContexttMenu.Delete);
				}
				arg_A5_0.AddItem(arg_A5_1, arg_A5_2, AudioMixerGroupViewList.ViewsContexttMenu.<>f__mg$cache2, userData);
				genericMenu.DropDown(buttonRect);
			}

			private static void Rename(object userData)
			{
				AudioMixerGroupViewList.ViewsContexttMenu.data data = userData as AudioMixerGroupViewList.ViewsContexttMenu.data;
				data.list.Rename(data.viewIndex);
			}

			private static void Duplicate(object userData)
			{
				AudioMixerGroupViewList.ViewsContexttMenu.data data = userData as AudioMixerGroupViewList.ViewsContexttMenu.data;
				data.list.m_Controller.currentViewIndex = data.viewIndex;
				data.list.DuplicateCurrentView();
			}

			private static void Delete(object userData)
			{
				AudioMixerGroupViewList.ViewsContexttMenu.data data = userData as AudioMixerGroupViewList.ViewsContexttMenu.data;
				data.list.Delete(data.viewIndex);
			}
		}

		private ReorderableListWithRenameAndScrollView m_ReorderableListWithRenameAndScrollView;

		private AudioMixerController m_Controller;

		private List<MixerGroupView> m_Views;

		private readonly ReorderableListWithRenameAndScrollView.State m_State;

		private static AudioMixerGroupViewList.Styles s_Styles;

		public AudioMixerGroupViewList(ReorderableListWithRenameAndScrollView.State state)
		{
			this.m_State = state;
		}

		public void OnMixerControllerChanged(AudioMixerController controller)
		{
			this.m_Controller = controller;
			this.RecreateListControl();
		}

		public void OnUndoRedoPerformed()
		{
			this.RecreateListControl();
		}

		public void OnEvent()
		{
			if (!(this.m_Controller == null))
			{
				this.m_ReorderableListWithRenameAndScrollView.OnEvent();
			}
		}

		public void RecreateListControl()
		{
			if (!(this.m_Controller == null))
			{
				this.m_Views = new List<MixerGroupView>(this.m_Controller.views);
				if (this.m_Views.Count == 0)
				{
					MixerGroupView item = default(MixerGroupView);
					item.guids = (from gr in this.m_Controller.GetAllAudioGroupsSlow()
					select gr.groupID).ToArray<GUID>();
					item.name = "View";
					this.m_Views.Add(item);
					this.SaveToBackend();
				}
				ReorderableList reorderableList = new ReorderableList(this.m_Views, typeof(MixerGroupView), true, false, false, false);
				ReorderableList expr_B9 = reorderableList;
				expr_B9.onReorderCallback = (ReorderableList.ReorderCallbackDelegate)Delegate.Combine(expr_B9.onReorderCallback, new ReorderableList.ReorderCallbackDelegate(this.EndDragChild));
				reorderableList.elementHeight = 16f;
				reorderableList.headerHeight = 0f;
				reorderableList.footerHeight = 0f;
				reorderableList.showDefaultBackground = false;
				reorderableList.index = this.m_Controller.currentViewIndex;
				if (this.m_Controller.currentViewIndex >= reorderableList.count)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"State mismatch, currentViewIndex: ",
						this.m_Controller.currentViewIndex,
						", num items: ",
						reorderableList.count
					}));
				}
				this.m_ReorderableListWithRenameAndScrollView = new ReorderableListWithRenameAndScrollView(reorderableList, this.m_State);
				ReorderableListWithRenameAndScrollView expr_182 = this.m_ReorderableListWithRenameAndScrollView;
				expr_182.onSelectionChanged = (Action<int>)Delegate.Combine(expr_182.onSelectionChanged, new Action<int>(this.SelectionChanged));
				ReorderableListWithRenameAndScrollView expr_1A9 = this.m_ReorderableListWithRenameAndScrollView;
				expr_1A9.onNameChangedAtIndex = (Action<int, string>)Delegate.Combine(expr_1A9.onNameChangedAtIndex, new Action<int, string>(this.NameChanged));
				ReorderableListWithRenameAndScrollView expr_1D0 = this.m_ReorderableListWithRenameAndScrollView;
				expr_1D0.onDeleteItemAtIndex = (Action<int>)Delegate.Combine(expr_1D0.onDeleteItemAtIndex, new Action<int>(this.Delete));
				ReorderableListWithRenameAndScrollView expr_1F7 = this.m_ReorderableListWithRenameAndScrollView;
				expr_1F7.onGetNameAtIndex = (Func<int, string>)Delegate.Combine(expr_1F7.onGetNameAtIndex, new Func<int, string>(this.GetNameOfElement));
				ReorderableListWithRenameAndScrollView expr_21E = this.m_ReorderableListWithRenameAndScrollView;
				expr_21E.onCustomDrawElement = (ReorderableList.ElementCallbackDelegate)Delegate.Combine(expr_21E.onCustomDrawElement, new ReorderableList.ElementCallbackDelegate(this.CustomDrawElement));
			}
		}

		public float GetTotalHeight()
		{
			float result;
			if (this.m_Controller == null)
			{
				result = 0f;
			}
			else
			{
				result = this.m_ReorderableListWithRenameAndScrollView.list.GetHeight() + 22f;
			}
			return result;
		}

		public void OnGUI(Rect rect)
		{
			if (AudioMixerGroupViewList.s_Styles == null)
			{
				AudioMixerGroupViewList.s_Styles = new AudioMixerGroupViewList.Styles();
			}
			Rect r;
			Rect rect2;
			using (new EditorGUI.DisabledScope(this.m_Controller == null))
			{
				AudioMixerDrawUtils.DrawRegionBg(rect, out r, out rect2);
				AudioMixerDrawUtils.HeaderLabel(r, AudioMixerGroupViewList.s_Styles.header, AudioMixerGroupViewList.s_Styles.viewsIcon);
			}
			if (this.m_Controller != null)
			{
				if (this.m_ReorderableListWithRenameAndScrollView.list.index != this.m_Controller.currentViewIndex)
				{
					this.m_ReorderableListWithRenameAndScrollView.list.index = this.m_Controller.currentViewIndex;
					this.m_ReorderableListWithRenameAndScrollView.FrameItem(this.m_Controller.currentViewIndex);
				}
				this.m_ReorderableListWithRenameAndScrollView.OnGUI(rect2);
				if (GUI.Button(new Rect(r.xMax - 15f, r.y + 3f, 15f, 15f), AudioMixerGroupViewList.s_Styles.addButton, EditorStyles.label))
				{
					this.Add();
				}
			}
		}

		public void CustomDrawElement(Rect r, int index, bool isActive, bool isFocused)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseUp && current.button == 1 && r.Contains(current.mousePosition))
			{
				AudioMixerGroupViewList.ViewsContexttMenu.Show(r, index, this);
				current.Use();
			}
			bool isSelected = index == this.m_ReorderableListWithRenameAndScrollView.list.index && !this.m_ReorderableListWithRenameAndScrollView.IsRenamingIndex(index);
			this.m_ReorderableListWithRenameAndScrollView.DrawElementText(r, index, isActive, isSelected, isFocused);
		}

		private void SaveToBackend()
		{
			this.m_Controller.views = this.m_Views.ToArray();
		}

		private void LoadFromBackend()
		{
			this.m_Views.Clear();
			this.m_Views.AddRange(this.m_Controller.views);
		}

		private string GetNameOfElement(int index)
		{
			return this.m_Views[index].name;
		}

		private void Add()
		{
			this.m_Controller.CloneViewFromCurrent();
			this.LoadFromBackend();
			int num = this.m_Views.Count - 1;
			this.m_Controller.currentViewIndex = num;
			this.m_ReorderableListWithRenameAndScrollView.BeginRename(num, 0f);
		}

		private void Delete(int index)
		{
			if (this.m_Views.Count <= 1)
			{
				Debug.Log("Deleting all views is not allowed");
			}
			else
			{
				this.m_Controller.DeleteView(index);
				this.LoadFromBackend();
			}
		}

		public void NameChanged(int index, string newName)
		{
			this.LoadFromBackend();
			MixerGroupView value = this.m_Views[index];
			value.name = newName;
			this.m_Views[index] = value;
			this.SaveToBackend();
		}

		public void SelectionChanged(int selectedIndex)
		{
			this.LoadFromBackend();
			this.m_Controller.SetView(selectedIndex);
		}

		public void EndDragChild(ReorderableList list)
		{
			this.m_Views = (this.m_ReorderableListWithRenameAndScrollView.list.list as List<MixerGroupView>);
			this.SaveToBackend();
		}

		private void Rename(int index)
		{
			this.m_ReorderableListWithRenameAndScrollView.BeginRename(index, 0f);
		}

		private void DuplicateCurrentView()
		{
			this.m_Controller.CloneViewFromCurrent();
			this.LoadFromBackend();
		}
	}
}
