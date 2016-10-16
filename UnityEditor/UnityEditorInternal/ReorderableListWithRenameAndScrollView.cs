using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class ReorderableListWithRenameAndScrollView
	{
		[Serializable]
		public class State
		{
			public Vector2 m_ScrollPos = new Vector2(0f, 0f);

			public RenameOverlay m_RenameOverlay = new RenameOverlay();
		}

		public class Styles
		{
			public GUIStyle reorderableListLabel = new GUIStyle("PR Label");

			public GUIStyle reorderableListLabelRightAligned;

			public Styles()
			{
				Texture2D background = this.reorderableListLabel.hover.background;
				this.reorderableListLabel.normal.background = background;
				this.reorderableListLabel.active.background = background;
				this.reorderableListLabel.focused.background = background;
				this.reorderableListLabel.onNormal.background = background;
				this.reorderableListLabel.onHover.background = background;
				this.reorderableListLabel.onActive.background = background;
				this.reorderableListLabel.onFocused.background = background;
				RectOffset arg_C2_0 = this.reorderableListLabel.padding;
				int num = 0;
				this.reorderableListLabel.padding.right = num;
				arg_C2_0.left = num;
				this.reorderableListLabel.alignment = TextAnchor.MiddleLeft;
				this.reorderableListLabelRightAligned = new GUIStyle(this.reorderableListLabel);
				this.reorderableListLabelRightAligned.alignment = TextAnchor.MiddleRight;
				this.reorderableListLabelRightAligned.clipping = TextClipping.Overflow;
			}
		}

		private ReorderableList m_ReorderableList;

		private ReorderableListWithRenameAndScrollView.State m_State;

		private int m_LastSelectedIndex = -1;

		private bool m_HadKeyFocusAtMouseDown;

		private int m_FrameIndex = -1;

		public GUIStyle listElementStyle;

		public GUIStyle renameOverlayStyle;

		public Func<int, string> onGetNameAtIndex;

		public Action<int, string> onNameChangedAtIndex;

		public Action<int> onSelectionChanged;

		public Action<int> onDeleteItemAtIndex;

		public ReorderableList.ElementCallbackDelegate onCustomDrawElement;

		private static ReorderableListWithRenameAndScrollView.Styles s_Styles;

		public ReorderableList list
		{
			get
			{
				return this.m_ReorderableList;
			}
		}

		public GUIStyle elementStyle
		{
			get
			{
				return this.listElementStyle ?? ReorderableListWithRenameAndScrollView.s_Styles.reorderableListLabel;
			}
		}

		public GUIStyle elementStyleRightAligned
		{
			get
			{
				return ReorderableListWithRenameAndScrollView.s_Styles.reorderableListLabelRightAligned;
			}
		}

		public ReorderableListWithRenameAndScrollView(ReorderableList list, ReorderableListWithRenameAndScrollView.State state)
		{
			this.m_State = state;
			this.m_ReorderableList = list;
			ReorderableList expr_28 = this.m_ReorderableList;
			expr_28.drawElementCallback = (ReorderableList.ElementCallbackDelegate)Delegate.Combine(expr_28.drawElementCallback, new ReorderableList.ElementCallbackDelegate(this.DrawElement));
			ReorderableList expr_4F = this.m_ReorderableList;
			expr_4F.onSelectCallback = (ReorderableList.SelectCallbackDelegate)Delegate.Combine(expr_4F.onSelectCallback, new ReorderableList.SelectCallbackDelegate(this.SelectCallback));
			ReorderableList expr_76 = this.m_ReorderableList;
			expr_76.onMouseUpCallback = (ReorderableList.SelectCallbackDelegate)Delegate.Combine(expr_76.onMouseUpCallback, new ReorderableList.SelectCallbackDelegate(this.MouseUpCallback));
			ReorderableList expr_9D = this.m_ReorderableList;
			expr_9D.onReorderCallback = (ReorderableList.ReorderCallbackDelegate)Delegate.Combine(expr_9D.onReorderCallback, new ReorderableList.ReorderCallbackDelegate(this.ReorderCallback));
		}

		private RenameOverlay GetRenameOverlay()
		{
			return this.m_State.m_RenameOverlay;
		}

		public void OnEvent()
		{
			this.GetRenameOverlay().OnEvent();
		}

		private void EnsureRowIsVisible(int index, float scrollGUIHeight)
		{
			if (index < 0)
			{
				return;
			}
			float num = this.m_ReorderableList.elementHeight * (float)index + 2f;
			float min = num - scrollGUIHeight + this.m_ReorderableList.elementHeight + 3f;
			this.m_State.m_ScrollPos.y = Mathf.Clamp(this.m_State.m_ScrollPos.y, min, num);
		}

		public void OnGUI(Rect rect)
		{
			if (ReorderableListWithRenameAndScrollView.s_Styles == null)
			{
				ReorderableListWithRenameAndScrollView.s_Styles = new ReorderableListWithRenameAndScrollView.Styles();
			}
			if (this.onGetNameAtIndex == null)
			{
				Debug.LogError("Ensure to set: 'onGetNameAtIndex'");
			}
			Event current = Event.current;
			if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
			{
				this.m_HadKeyFocusAtMouseDown = this.m_ReorderableList.HasKeyboardControl();
			}
			if (this.m_FrameIndex != -1)
			{
				this.EnsureRowIsVisible(this.m_FrameIndex, rect.height);
				this.m_FrameIndex = -1;
			}
			GUILayout.BeginArea(rect);
			this.m_State.m_ScrollPos = GUILayout.BeginScrollView(this.m_State.m_ScrollPos, new GUILayoutOption[0]);
			this.m_ReorderableList.DoLayoutList();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			AudioMixerDrawUtils.DrawScrollDropShadow(rect, this.m_State.m_ScrollPos.y, this.m_ReorderableList.GetHeight());
			this.KeyboardHandling();
			this.CommandHandling();
		}

		public bool IsRenamingIndex(int index)
		{
			return this.GetRenameOverlay().IsRenaming() && this.GetRenameOverlay().userData == index && !this.GetRenameOverlay().isWaitingForDelay;
		}

		public void DrawElement(Rect r, int index, bool isActive, bool isFocused)
		{
			if (this.IsRenamingIndex(index))
			{
				if (r.width >= 0f && r.height >= 0f)
				{
					r.x -= 2f;
					this.GetRenameOverlay().editFieldRect = r;
				}
				this.DoRenameOverlay();
			}
			else if (this.onCustomDrawElement != null)
			{
				this.onCustomDrawElement(r, index, isActive, isFocused);
			}
			else
			{
				this.DrawElementText(r, index, isActive, index == this.m_ReorderableList.index, isFocused);
			}
		}

		public void DrawElementText(Rect r, int index, bool isActive, bool isSelected, bool isFocused)
		{
			if (Event.current.type == EventType.Repaint && this.onGetNameAtIndex != null)
			{
				this.elementStyle.Draw(r, this.onGetNameAtIndex(index), false, false, isSelected, true);
			}
		}

		public virtual void DoRenameOverlay()
		{
			if (this.GetRenameOverlay().IsRenaming() && !this.GetRenameOverlay().OnGUI())
			{
				this.RenameEnded();
			}
		}

		public void BeginRename(int index, float delay)
		{
			this.GetRenameOverlay().BeginRename(this.onGetNameAtIndex(index), index, delay);
			this.m_ReorderableList.index = index;
			this.m_LastSelectedIndex = index;
			this.FrameItem(index);
		}

		private void RenameEnded()
		{
			if (this.GetRenameOverlay().userAcceptedRename && this.onNameChangedAtIndex != null)
			{
				string arg = (!string.IsNullOrEmpty(this.GetRenameOverlay().name)) ? this.GetRenameOverlay().name : this.GetRenameOverlay().originalName;
				int userData = this.GetRenameOverlay().userData;
				this.onNameChangedAtIndex(userData, arg);
			}
			if (this.GetRenameOverlay().HasKeyboardFocus())
			{
				this.m_ReorderableList.GrabKeyboardFocus();
			}
			this.GetRenameOverlay().Clear();
		}

		public void EndRename(bool acceptChanges)
		{
			if (this.GetRenameOverlay().IsRenaming())
			{
				this.GetRenameOverlay().EndRename(acceptChanges);
				this.RenameEnded();
			}
		}

		public void ReorderCallback(ReorderableList list)
		{
			this.m_LastSelectedIndex = list.index;
		}

		public void MouseUpCallback(ReorderableList list)
		{
			if (this.m_HadKeyFocusAtMouseDown && list.index == this.m_LastSelectedIndex)
			{
				this.BeginRename(list.index, 0.5f);
			}
			this.m_LastSelectedIndex = list.index;
		}

		public void SelectCallback(ReorderableList list)
		{
			this.FrameItem(list.index);
			if (this.onSelectionChanged != null)
			{
				this.onSelectionChanged(list.index);
			}
		}

		private void RemoveSelected()
		{
			if (this.m_ReorderableList.index < 0 || this.m_ReorderableList.index >= this.m_ReorderableList.count)
			{
				Debug.Log("Invalid index to remove " + this.m_ReorderableList.index);
				return;
			}
			if (this.onDeleteItemAtIndex != null)
			{
				this.onDeleteItemAtIndex(this.m_ReorderableList.index);
			}
		}

		public void FrameItem(int index)
		{
			this.m_FrameIndex = index;
		}

		private bool CanBeginRename()
		{
			return !this.GetRenameOverlay().IsRenaming() && this.m_ReorderableList.index >= 0;
		}

		private void CommandHandling()
		{
			Event current = Event.current;
			if (Event.current.type == EventType.ExecuteCommand)
			{
				string commandName = current.commandName;
				if (commandName != null)
				{
					if (ReorderableListWithRenameAndScrollView.<>f__switch$map15 == null)
					{
						ReorderableListWithRenameAndScrollView.<>f__switch$map15 = new Dictionary<string, int>(1)
						{
							{
								"OnLostFocus",
								0
							}
						};
					}
					int num;
					if (ReorderableListWithRenameAndScrollView.<>f__switch$map15.TryGetValue(commandName, out num))
					{
						if (num == 0)
						{
							this.EndRename(true);
							current.Use();
						}
					}
				}
			}
		}

		private void KeyboardHandling()
		{
			Event current = Event.current;
			if (current.type != EventType.KeyDown)
			{
				return;
			}
			if (this.m_ReorderableList.HasKeyboardControl())
			{
				KeyCode keyCode = Event.current.keyCode;
				switch (keyCode)
				{
				case KeyCode.Home:
					current.Use();
					this.m_ReorderableList.index = 0;
					this.FrameItem(this.m_ReorderableList.index);
					return;
				case KeyCode.End:
					current.Use();
					this.m_ReorderableList.index = this.m_ReorderableList.count - 1;
					this.FrameItem(this.m_ReorderableList.index);
					return;
				case KeyCode.PageUp:
				case KeyCode.PageDown:
				case KeyCode.F1:
					IL_52:
					if (keyCode != KeyCode.Return)
					{
						if (keyCode == KeyCode.Delete)
						{
							this.RemoveSelected();
							current.Use();
							return;
						}
						if (keyCode != KeyCode.KeypadEnter)
						{
							return;
						}
					}
					if (this.CanBeginRename() && Application.platform == RuntimePlatform.OSXEditor)
					{
						this.BeginRename(this.m_ReorderableList.index, 0f);
						current.Use();
					}
					return;
				case KeyCode.F2:
					if (this.CanBeginRename() && Application.platform == RuntimePlatform.WindowsEditor)
					{
						this.BeginRename(this.m_ReorderableList.index, 0f);
						current.Use();
					}
					return;
				}
				goto IL_52;
			}
		}
	}
}
