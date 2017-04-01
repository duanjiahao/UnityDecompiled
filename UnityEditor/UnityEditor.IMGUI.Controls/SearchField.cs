using System;
using System.Threading;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	public class SearchField
	{
		public delegate void SearchFieldCallback();

		private int m_ControlID;

		private bool m_WantsFocus;

		private bool m_AutoSetFocusOnFindCommand = true;

		private const float kMinWidth = 36f;

		private const float kMaxWidth = 1E+07f;

		private const float kMinToolbarWidth = 29f;

		private const float kMaxToolbarWidth = 200f;

		public event SearchField.SearchFieldCallback downOrUpArrowKeyPressed
		{
			add
			{
				SearchField.SearchFieldCallback searchFieldCallback = this.downOrUpArrowKeyPressed;
				SearchField.SearchFieldCallback searchFieldCallback2;
				do
				{
					searchFieldCallback2 = searchFieldCallback;
					searchFieldCallback = Interlocked.CompareExchange<SearchField.SearchFieldCallback>(ref this.downOrUpArrowKeyPressed, (SearchField.SearchFieldCallback)Delegate.Combine(searchFieldCallback2, value), searchFieldCallback);
				}
				while (searchFieldCallback != searchFieldCallback2);
			}
			remove
			{
				SearchField.SearchFieldCallback searchFieldCallback = this.downOrUpArrowKeyPressed;
				SearchField.SearchFieldCallback searchFieldCallback2;
				do
				{
					searchFieldCallback2 = searchFieldCallback;
					searchFieldCallback = Interlocked.CompareExchange<SearchField.SearchFieldCallback>(ref this.downOrUpArrowKeyPressed, (SearchField.SearchFieldCallback)Delegate.Remove(searchFieldCallback2, value), searchFieldCallback);
				}
				while (searchFieldCallback != searchFieldCallback2);
			}
		}

		public int searchFieldControlID
		{
			get
			{
				return this.m_ControlID;
			}
			set
			{
				this.m_ControlID = value;
			}
		}

		public bool autoSetFocusOnFindCommand
		{
			get
			{
				return this.m_AutoSetFocusOnFindCommand;
			}
			set
			{
				this.m_AutoSetFocusOnFindCommand = value;
			}
		}

		public SearchField()
		{
			this.m_ControlID = GUIUtility.GetPermanentControlID();
		}

		public void SetFocus()
		{
			this.m_WantsFocus = true;
		}

		public bool HasFocus()
		{
			return GUIUtility.keyboardControl == this.m_ControlID;
		}

		public string OnGUI(Rect rect, string text, GUIStyle style, GUIStyle cancelButtonStyle, GUIStyle emptyCancelButtonStyle)
		{
			this.CommandEventHandling();
			this.FocusAndKeyHandling();
			float fixedWidth = cancelButtonStyle.fixedWidth;
			Rect position = rect;
			position.width -= fixedWidth;
			text = EditorGUI.TextFieldInternal(this.m_ControlID, position, text, style);
			Rect position2 = rect;
			position2.x += rect.width - fixedWidth;
			position2.width = fixedWidth;
			if (GUI.Button(position2, GUIContent.none, (!(text != "")) ? emptyCancelButtonStyle : cancelButtonStyle) && text != "")
			{
				text = "";
				GUIUtility.keyboardControl = 0;
			}
			return text;
		}

		public string OnGUI(Rect rect, string text)
		{
			return this.OnGUI(rect, text, EditorStyles.searchField, EditorStyles.searchFieldCancelButton, EditorStyles.searchFieldCancelButtonEmpty);
		}

		public string OnGUI(string text, params GUILayoutOption[] options)
		{
			Rect rect = GUILayoutUtility.GetRect(36f, 1E+07f, 16f, 16f, EditorStyles.searchField, options);
			return this.OnGUI(rect, text);
		}

		public string OnToolbarGUI(Rect rect, string text)
		{
			return this.OnGUI(rect, text, EditorStyles.toolbarSearchField, EditorStyles.toolbarSearchFieldCancelButton, EditorStyles.toolbarSearchFieldCancelButtonEmpty);
		}

		public string OnToolbarGUI(string text, params GUILayoutOption[] options)
		{
			Rect rect = GUILayoutUtility.GetRect(29f, 200f, 16f, 16f, EditorStyles.toolbarSearchField, options);
			return this.OnToolbarGUI(rect, text);
		}

		private void FocusAndKeyHandling()
		{
			Event current = Event.current;
			if (this.m_WantsFocus && current.type == EventType.Repaint)
			{
				GUIUtility.keyboardControl = this.m_ControlID;
				EditorGUIUtility.editingTextField = true;
				this.m_WantsFocus = false;
			}
			if (current.type == EventType.KeyDown && (current.keyCode == KeyCode.DownArrow || current.keyCode == KeyCode.UpArrow) && GUIUtility.keyboardControl == this.m_ControlID && GUIUtility.hotControl == 0)
			{
				if (this.downOrUpArrowKeyPressed != null)
				{
					this.downOrUpArrowKeyPressed();
					current.Use();
				}
			}
		}

		private void CommandEventHandling()
		{
			Event current = Event.current;
			if (current.type == EventType.ExecuteCommand || current.type == EventType.ValidateCommand)
			{
				if (this.m_AutoSetFocusOnFindCommand && current.commandName == "Find")
				{
					if (current.type == EventType.ExecuteCommand)
					{
						this.SetFocus();
					}
					current.Use();
				}
			}
		}
	}
}
