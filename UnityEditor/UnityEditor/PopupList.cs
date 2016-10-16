using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class PopupList : PopupWindowContent
	{
		public enum Gravity
		{
			Top,
			Bottom
		}

		public class ListElement
		{
			public GUIContent m_Content;

			private float m_FilterScore;

			private bool m_Selected;

			private bool m_WasSelected;

			private bool m_PartiallySelected;

			private bool m_Enabled;

			public float filterScore
			{
				get
				{
					return (!this.m_WasSelected) ? this.m_FilterScore : 3.40282347E+38f;
				}
				set
				{
					this.m_FilterScore = value;
					this.ResetScore();
				}
			}

			public bool selected
			{
				get
				{
					return this.m_Selected;
				}
				set
				{
					this.m_Selected = value;
					if (this.m_Selected)
					{
						this.m_WasSelected = true;
					}
				}
			}

			public bool enabled
			{
				get
				{
					return this.m_Enabled;
				}
				set
				{
					this.m_Enabled = value;
				}
			}

			public bool partiallySelected
			{
				get
				{
					return this.m_PartiallySelected;
				}
				set
				{
					this.m_PartiallySelected = value;
					if (this.m_PartiallySelected)
					{
						this.m_WasSelected = true;
					}
				}
			}

			public string text
			{
				get
				{
					return this.m_Content.text;
				}
				set
				{
					this.m_Content.text = value;
				}
			}

			public ListElement(string text, bool selected, float score)
			{
				this.m_Content = new GUIContent(text);
				if (!string.IsNullOrEmpty(this.m_Content.text))
				{
					char[] array = this.m_Content.text.ToCharArray();
					array[0] = char.ToUpper(array[0]);
					this.m_Content.text = new string(array);
				}
				this.m_Selected = selected;
				this.filterScore = score;
				this.m_PartiallySelected = false;
				this.m_Enabled = true;
			}

			public ListElement(string text, bool selected)
			{
				this.m_Content = new GUIContent(text);
				this.m_Selected = selected;
				this.filterScore = 0f;
				this.m_PartiallySelected = false;
				this.m_Enabled = true;
			}

			public ListElement(string text) : this(text, false)
			{
			}

			public void ResetScore()
			{
				this.m_WasSelected = (this.m_Selected || this.m_PartiallySelected);
			}
		}

		public class InputData
		{
			public List<PopupList.ListElement> m_ListElements;

			public bool m_CloseOnSelection;

			public bool m_AllowCustom;

			public bool m_EnableAutoCompletion = true;

			public bool m_SortAlphabetically;

			public PopupList.OnSelectCallback m_OnSelectCallback;

			public int m_MaxCount;

			public InputData()
			{
				this.m_ListElements = new List<PopupList.ListElement>();
			}

			public void DeselectAll()
			{
				foreach (PopupList.ListElement current in this.m_ListElements)
				{
					current.selected = false;
					current.partiallySelected = false;
				}
			}

			public void ResetScores()
			{
				foreach (PopupList.ListElement current in this.m_ListElements)
				{
					current.ResetScore();
				}
			}

			public virtual IEnumerable<PopupList.ListElement> BuildQuery(string prefix)
			{
				if (prefix == string.Empty)
				{
					return this.m_ListElements;
				}
				return from element in this.m_ListElements
				where element.m_Content.text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
				select element;
			}

			public IEnumerable<PopupList.ListElement> GetFilteredList(string prefix)
			{
				IEnumerable<PopupList.ListElement> enumerable = this.BuildQuery(prefix);
				if (this.m_MaxCount > 0)
				{
					enumerable = (from element in enumerable
					orderby element.filterScore descending
					select element).Take(this.m_MaxCount);
				}
				if (this.m_SortAlphabetically)
				{
					return from element in enumerable
					orderby element.text.ToLower()
					select element;
				}
				return enumerable;
			}

			public int GetFilteredCount(string prefix)
			{
				IEnumerable<PopupList.ListElement> source = this.BuildQuery(prefix);
				if (this.m_MaxCount > 0)
				{
					source = source.Take(this.m_MaxCount);
				}
				return source.Count<PopupList.ListElement>();
			}

			public PopupList.ListElement NewOrMatchingElement(string label)
			{
				foreach (PopupList.ListElement current in this.m_ListElements)
				{
					if (current.text.Equals(label, StringComparison.OrdinalIgnoreCase))
					{
						return current;
					}
				}
				PopupList.ListElement listElement = new PopupList.ListElement(label, false, -1f);
				this.m_ListElements.Add(listElement);
				return listElement;
			}
		}

		private class Styles
		{
			public GUIStyle menuItem = "MenuItem";

			public GUIStyle menuItemMixed = "MenuItemMixed";

			public GUIStyle label = "PR Label";

			public GUIStyle background = "grey_border";

			public GUIStyle customTextField;

			public GUIStyle customTextFieldCancelButton;

			public GUIStyle customTextFieldCancelButtonEmpty;

			public Styles()
			{
				this.customTextField = new GUIStyle(EditorStyles.toolbarSearchField);
				this.customTextFieldCancelButton = new GUIStyle(EditorStyles.toolbarSearchFieldCancelButton);
				this.customTextFieldCancelButtonEmpty = new GUIStyle(EditorStyles.toolbarSearchFieldCancelButtonEmpty);
			}
		}

		public delegate void OnSelectCallback(PopupList.ListElement element);

		private const float scrollBarWidth = 14f;

		private const float listElementHeight = 18f;

		private const float gizmoRightAlign = 23f;

		private const float iconRightAlign = 64f;

		private const float frameWidth = 1f;

		private const float k_LineHeight = 16f;

		private const float k_TextFieldHeight = 16f;

		private const float k_Margin = 10f;

		private static EditorGUI.RecycledTextEditor s_RecycledEditor = new EditorGUI.RecycledTextEditor();

		private static string s_TextFieldName = "ProjectBrowserPopupsTextField";

		private static int s_TextFieldHash = PopupList.s_TextFieldName.GetHashCode();

		private static PopupList.Styles s_Styles;

		private PopupList.InputData m_Data;

		private Vector2 m_ScrollPosition;

		private Vector2 m_ScreenPos;

		private PopupList.Gravity m_Gravity;

		private string m_EnteredTextCompletion = string.Empty;

		private string m_EnteredText = string.Empty;

		private int m_SelectedCompletionIndex;

		public PopupList(PopupList.InputData inputData) : this(inputData, null)
		{
		}

		public PopupList(PopupList.InputData inputData, string initialSelectionLabel)
		{
			this.m_Data = inputData;
			this.m_Data.ResetScores();
			this.SelectNoCompletion();
			this.m_Gravity = PopupList.Gravity.Top;
			if (initialSelectionLabel != null)
			{
				this.m_EnteredTextCompletion = initialSelectionLabel;
				this.UpdateCompletion();
			}
		}

		public override void OnClose()
		{
			if (this.m_Data != null)
			{
				this.m_Data.ResetScores();
			}
		}

		public virtual float GetWindowHeight()
		{
			int num = (this.m_Data.m_MaxCount != 0) ? this.m_Data.m_MaxCount : this.m_Data.GetFilteredCount(this.m_EnteredText);
			return (float)num * 16f + 20f + ((!this.m_Data.m_AllowCustom) ? 0f : 16f);
		}

		public virtual float GetWindowWidth()
		{
			return 150f;
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(this.GetWindowWidth(), this.GetWindowHeight());
		}

		public override void OnGUI(Rect windowRect)
		{
			Event current = Event.current;
			if (current.type == EventType.Layout)
			{
				return;
			}
			if (PopupList.s_Styles == null)
			{
				PopupList.s_Styles = new PopupList.Styles();
			}
			if (current.type == EventType.KeyDown && current.keyCode == KeyCode.Escape)
			{
				base.editorWindow.Close();
				GUIUtility.ExitGUI();
			}
			if (this.m_Gravity == PopupList.Gravity.Bottom)
			{
				this.DrawList(base.editorWindow, windowRect);
				this.DrawCustomTextField(base.editorWindow, windowRect);
			}
			else
			{
				this.DrawCustomTextField(base.editorWindow, windowRect);
				this.DrawList(base.editorWindow, windowRect);
			}
			if (current.type == EventType.Repaint)
			{
				PopupList.s_Styles.background.Draw(new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height), false, false, false, false);
			}
		}

		private void DrawCustomTextField(EditorWindow editorWindow, Rect windowRect)
		{
			if (!this.m_Data.m_AllowCustom)
			{
				return;
			}
			Event current = Event.current;
			bool flag = this.m_Data.m_EnableAutoCompletion;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			string text = this.CurrentDisplayedText();
			if (current.type == EventType.KeyDown)
			{
				KeyCode keyCode = current.keyCode;
				switch (keyCode)
				{
				case KeyCode.Backspace:
					goto IL_136;
				case KeyCode.Tab:
					goto IL_B5;
				case (KeyCode)10:
				case (KeyCode)11:
				case KeyCode.Clear:
					IL_67:
					if (keyCode == KeyCode.UpArrow)
					{
						this.ChangeSelectedCompletion(-1);
						flag3 = true;
						goto IL_17A;
					}
					if (keyCode == KeyCode.DownArrow)
					{
						this.ChangeSelectedCompletion(1);
						flag3 = true;
						goto IL_17A;
					}
					if (keyCode == KeyCode.None)
					{
						if (current.character == ' ' || current.character == ',')
						{
							flag3 = true;
						}
						goto IL_17A;
					}
					if (keyCode == KeyCode.Space)
					{
						goto IL_B5;
					}
					if (keyCode == KeyCode.Comma)
					{
						goto IL_B5;
					}
					if (keyCode != KeyCode.Delete)
					{
						goto IL_17A;
					}
					goto IL_136;
				case KeyCode.Return:
					goto IL_B5;
				}
				goto IL_67;
				IL_B5:
				if (text != string.Empty)
				{
					if (this.m_Data.m_OnSelectCallback != null)
					{
						this.m_Data.m_OnSelectCallback(this.m_Data.NewOrMatchingElement(text));
					}
					if (current.keyCode == KeyCode.Tab || current.keyCode == KeyCode.Comma)
					{
						flag4 = true;
					}
					if (this.m_Data.m_CloseOnSelection || current.keyCode == KeyCode.Return)
					{
						flag2 = true;
					}
				}
				flag3 = true;
				goto IL_17A;
				IL_136:
				flag = false;
			}
			IL_17A:
			bool flag5 = false;
			Rect rect = new Rect(windowRect.x + 5f, windowRect.y + ((this.m_Gravity != PopupList.Gravity.Top) ? (windowRect.height - 16f - 5f) : 5f), windowRect.width - 10f - 14f, 16f);
			GUI.SetNextControlName(PopupList.s_TextFieldName);
			EditorGUI.FocusTextInControl(PopupList.s_TextFieldName);
			int controlID = GUIUtility.GetControlID(PopupList.s_TextFieldHash, FocusType.Keyboard, rect);
			if (flag3)
			{
				current.Use();
			}
			if (GUIUtility.keyboardControl == 0)
			{
				GUIUtility.keyboardControl = controlID;
			}
			string text2 = EditorGUI.DoTextField(PopupList.s_RecycledEditor, controlID, rect, text, PopupList.s_Styles.customTextField, null, out flag5, false, false, false);
			Rect position = rect;
			position.x += rect.width;
			position.width = 14f;
			if ((GUI.Button(position, GUIContent.none, (!(text2 != string.Empty)) ? PopupList.s_Styles.customTextFieldCancelButtonEmpty : PopupList.s_Styles.customTextFieldCancelButton) && text2 != string.Empty) || flag4)
			{
				string empty = string.Empty;
				PopupList.s_RecycledEditor.text = empty;
				text2 = (EditorGUI.s_OriginalText = empty);
				PopupList.s_RecycledEditor.cursorIndex = 0;
				PopupList.s_RecycledEditor.selectIndex = 0;
				flag = false;
			}
			if (text != text2)
			{
				this.m_EnteredText = ((0 > PopupList.s_RecycledEditor.cursorIndex || PopupList.s_RecycledEditor.cursorIndex >= text2.Length) ? text2 : text2.Substring(0, PopupList.s_RecycledEditor.cursorIndex));
				if (flag)
				{
					this.UpdateCompletion();
				}
				else
				{
					this.SelectNoCompletion();
				}
			}
			if (flag2)
			{
				editorWindow.Close();
			}
		}

		private string CurrentDisplayedText()
		{
			return (!(this.m_EnteredTextCompletion != string.Empty)) ? this.m_EnteredText : this.m_EnteredTextCompletion;
		}

		private void UpdateCompletion()
		{
			if (!this.m_Data.m_EnableAutoCompletion)
			{
				return;
			}
			IEnumerable<string> source = from element in this.m_Data.GetFilteredList(this.m_EnteredText)
			select element.text;
			if (this.m_EnteredTextCompletion != string.Empty && this.m_EnteredTextCompletion.StartsWith(this.m_EnteredText, StringComparison.OrdinalIgnoreCase))
			{
				this.m_SelectedCompletionIndex = source.TakeWhile((string element) => element != this.m_EnteredTextCompletion).Count<string>();
			}
			else
			{
				if (this.m_SelectedCompletionIndex < 0)
				{
					this.m_SelectedCompletionIndex = 0;
				}
				else if (this.m_SelectedCompletionIndex >= source.Count<string>())
				{
					this.m_SelectedCompletionIndex = source.Count<string>() - 1;
				}
				this.m_EnteredTextCompletion = source.Skip(this.m_SelectedCompletionIndex).DefaultIfEmpty(string.Empty).FirstOrDefault<string>();
			}
			this.AdjustRecycledEditorSelectionToCompletion();
		}

		private void ChangeSelectedCompletion(int change)
		{
			int filteredCount = this.m_Data.GetFilteredCount(this.m_EnteredText);
			if (this.m_SelectedCompletionIndex == -1 && change < 0)
			{
				this.m_SelectedCompletionIndex = filteredCount;
			}
			int index = (filteredCount <= 0) ? 0 : ((this.m_SelectedCompletionIndex + change + filteredCount) % filteredCount);
			this.SelectCompletionWithIndex(index);
		}

		private void SelectCompletionWithIndex(int index)
		{
			this.m_SelectedCompletionIndex = index;
			this.m_EnteredTextCompletion = string.Empty;
			this.UpdateCompletion();
		}

		private void SelectNoCompletion()
		{
			this.m_SelectedCompletionIndex = -1;
			this.m_EnteredTextCompletion = string.Empty;
			this.AdjustRecycledEditorSelectionToCompletion();
		}

		private void AdjustRecycledEditorSelectionToCompletion()
		{
			if (this.m_EnteredTextCompletion != string.Empty)
			{
				PopupList.s_RecycledEditor.text = this.m_EnteredTextCompletion;
				EditorGUI.s_OriginalText = this.m_EnteredTextCompletion;
				PopupList.s_RecycledEditor.cursorIndex = this.m_EnteredText.Length;
				PopupList.s_RecycledEditor.selectIndex = this.m_EnteredTextCompletion.Length;
			}
		}

		private void DrawList(EditorWindow editorWindow, Rect windowRect)
		{
			Event current = Event.current;
			int num = -1;
			foreach (PopupList.ListElement current2 in this.m_Data.GetFilteredList(this.m_EnteredText))
			{
				num++;
				Rect position = new Rect(windowRect.x, windowRect.y + 10f + (float)num * 16f + ((this.m_Gravity != PopupList.Gravity.Top || !this.m_Data.m_AllowCustom) ? 0f : 16f), windowRect.width, 16f);
				EventType type = current.type;
				switch (type)
				{
				case EventType.MouseDown:
					if (Event.current.button == 0 && position.Contains(Event.current.mousePosition) && current2.enabled)
					{
						if (this.m_Data.m_OnSelectCallback != null)
						{
							this.m_Data.m_OnSelectCallback(current2);
						}
						current.Use();
						if (this.m_Data.m_CloseOnSelection)
						{
							editorWindow.Close();
						}
					}
					continue;
				case EventType.MouseUp:
				{
					IL_A5:
					if (type != EventType.Repaint)
					{
						continue;
					}
					GUIStyle gUIStyle = (!current2.partiallySelected) ? PopupList.s_Styles.menuItem : PopupList.s_Styles.menuItemMixed;
					bool flag = current2.selected || current2.partiallySelected;
					bool hasKeyboardFocus = false;
					bool isHover = num == this.m_SelectedCompletionIndex;
					bool isActive = flag;
					using (new EditorGUI.DisabledScope(!current2.enabled))
					{
						GUIContent content = current2.m_Content;
						gUIStyle.Draw(position, content, isHover, isActive, flag, hasKeyboardFocus);
					}
					continue;
				}
				case EventType.MouseMove:
					if (position.Contains(Event.current.mousePosition))
					{
						this.SelectCompletionWithIndex(num);
						current.Use();
					}
					continue;
				}
				goto IL_A5;
			}
		}
	}
}
