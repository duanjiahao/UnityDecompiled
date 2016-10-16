using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public class TextEditor
	{
		public enum DblClickSnapping : byte
		{
			WORDS,
			PARAGRAPHS
		}

		private enum CharacterType
		{
			LetterLike,
			Symbol,
			Symbol2,
			WhiteSpace
		}

		private enum TextEditOp
		{
			MoveLeft,
			MoveRight,
			MoveUp,
			MoveDown,
			MoveLineStart,
			MoveLineEnd,
			MoveTextStart,
			MoveTextEnd,
			MovePageUp,
			MovePageDown,
			MoveGraphicalLineStart,
			MoveGraphicalLineEnd,
			MoveWordLeft,
			MoveWordRight,
			MoveParagraphForward,
			MoveParagraphBackward,
			MoveToStartOfNextWord,
			MoveToEndOfPreviousWord,
			SelectLeft,
			SelectRight,
			SelectUp,
			SelectDown,
			SelectTextStart,
			SelectTextEnd,
			SelectPageUp,
			SelectPageDown,
			ExpandSelectGraphicalLineStart,
			ExpandSelectGraphicalLineEnd,
			SelectGraphicalLineStart,
			SelectGraphicalLineEnd,
			SelectWordLeft,
			SelectWordRight,
			SelectToEndOfPreviousWord,
			SelectToStartOfNextWord,
			SelectParagraphBackward,
			SelectParagraphForward,
			Delete,
			Backspace,
			DeleteWordBack,
			DeleteWordForward,
			DeleteLineBack,
			Cut,
			Copy,
			Paste,
			SelectAll,
			SelectNone,
			ScrollStart,
			ScrollEnd,
			ScrollPageUp,
			ScrollPageDown
		}

		public TouchScreenKeyboard keyboardOnScreen;

		public int controlID;

		public GUIStyle style = GUIStyle.none;

		public bool multiline;

		public bool hasHorizontalCursorPos;

		public bool isPasswordField;

		internal bool m_HasFocus;

		public Vector2 scrollOffset = Vector2.zero;

		private GUIContent m_Content = new GUIContent();

		private Rect m_Position;

		private int m_CursorIndex;

		private int m_SelectIndex;

		private bool m_RevealCursor;

		public Vector2 graphicalCursorPos;

		public Vector2 graphicalSelectCursorPos;

		private bool m_MouseDragSelectsWholeWords;

		private int m_DblClickInitPos;

		private TextEditor.DblClickSnapping m_DblClickSnap;

		private bool m_bJustSelected;

		private int m_iAltCursorPos = -1;

		private string oldText;

		private int oldPos;

		private int oldSelectPos;

		private static Dictionary<Event, TextEditor.TextEditOp> s_Keyactions;

		[Obsolete("Please use 'text' instead of 'content'", false)]
		public GUIContent content
		{
			get
			{
				return this.m_Content;
			}
			set
			{
				this.m_Content = value;
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
				this.ClampTextIndex(ref this.m_CursorIndex);
				this.ClampTextIndex(ref this.m_SelectIndex);
			}
		}

		public Rect position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				if (this.m_Position == value)
				{
					return;
				}
				this.m_Position = value;
				this.UpdateScrollOffset();
			}
		}

		public int cursorIndex
		{
			get
			{
				return this.m_CursorIndex;
			}
			set
			{
				int cursorIndex = this.m_CursorIndex;
				this.m_CursorIndex = value;
				this.ClampTextIndex(ref this.m_CursorIndex);
				if (this.m_CursorIndex != cursorIndex)
				{
					this.m_RevealCursor = true;
				}
			}
		}

		public int selectIndex
		{
			get
			{
				return this.m_SelectIndex;
			}
			set
			{
				this.m_SelectIndex = value;
				this.ClampTextIndex(ref this.m_SelectIndex);
			}
		}

		public bool hasSelection
		{
			get
			{
				return this.cursorIndex != this.selectIndex;
			}
		}

		public string SelectedText
		{
			get
			{
				if (this.cursorIndex == this.selectIndex)
				{
					return string.Empty;
				}
				if (this.cursorIndex < this.selectIndex)
				{
					return this.text.Substring(this.cursorIndex, this.selectIndex - this.cursorIndex);
				}
				return this.text.Substring(this.selectIndex, this.cursorIndex - this.selectIndex);
			}
		}

		[RequiredByNativeCode]
		public TextEditor()
		{
		}

		private void ClearCursorPos()
		{
			this.hasHorizontalCursorPos = false;
			this.m_iAltCursorPos = -1;
		}

		public void OnFocus()
		{
			if (this.multiline)
			{
				int num = 0;
				this.selectIndex = num;
				this.cursorIndex = num;
			}
			else
			{
				this.SelectAll();
			}
			this.m_HasFocus = true;
		}

		public void OnLostFocus()
		{
			this.m_HasFocus = false;
			this.scrollOffset = Vector2.zero;
		}

		private void GrabGraphicalCursorPos()
		{
			if (!this.hasHorizontalCursorPos)
			{
				this.graphicalCursorPos = this.style.GetCursorPixelPosition(this.position, this.m_Content, this.cursorIndex);
				this.graphicalSelectCursorPos = this.style.GetCursorPixelPosition(this.position, this.m_Content, this.selectIndex);
				this.hasHorizontalCursorPos = false;
			}
		}

		public bool HandleKeyEvent(Event e)
		{
			this.InitKeyActions();
			EventModifiers modifiers = e.modifiers;
			e.modifiers &= ~EventModifiers.CapsLock;
			if (TextEditor.s_Keyactions.ContainsKey(e))
			{
				TextEditor.TextEditOp operation = TextEditor.s_Keyactions[e];
				this.PerformOperation(operation);
				e.modifiers = modifiers;
				return true;
			}
			e.modifiers = modifiers;
			return false;
		}

		public bool DeleteLineBack()
		{
			if (this.hasSelection)
			{
				this.DeleteSelection();
				return true;
			}
			int num = this.cursorIndex;
			int num2 = num;
			while (num2-- != 0)
			{
				if (this.text[num2] == '\n')
				{
					num = num2 + 1;
					break;
				}
			}
			if (num2 == -1)
			{
				num = 0;
			}
			if (this.cursorIndex != num)
			{
				this.m_Content.text = this.text.Remove(num, this.cursorIndex - num);
				int num3 = num;
				this.cursorIndex = num3;
				this.selectIndex = num3;
				return true;
			}
			return false;
		}

		public bool DeleteWordBack()
		{
			if (this.hasSelection)
			{
				this.DeleteSelection();
				return true;
			}
			int num = this.FindEndOfPreviousWord(this.cursorIndex);
			if (this.cursorIndex != num)
			{
				this.m_Content.text = this.text.Remove(num, this.cursorIndex - num);
				int num2 = num;
				this.cursorIndex = num2;
				this.selectIndex = num2;
				return true;
			}
			return false;
		}

		public bool DeleteWordForward()
		{
			if (this.hasSelection)
			{
				this.DeleteSelection();
				return true;
			}
			int num = this.FindStartOfNextWord(this.cursorIndex);
			if (this.cursorIndex < this.text.Length)
			{
				this.m_Content.text = this.text.Remove(this.cursorIndex, num - this.cursorIndex);
				return true;
			}
			return false;
		}

		public bool Delete()
		{
			if (this.hasSelection)
			{
				this.DeleteSelection();
				return true;
			}
			if (this.cursorIndex < this.text.Length)
			{
				this.m_Content.text = this.text.Remove(this.cursorIndex, 1);
				return true;
			}
			return false;
		}

		public bool CanPaste()
		{
			return GUIUtility.systemCopyBuffer.Length != 0;
		}

		public bool Backspace()
		{
			if (this.hasSelection)
			{
				this.DeleteSelection();
				return true;
			}
			if (this.cursorIndex > 0)
			{
				this.m_Content.text = this.text.Remove(this.cursorIndex - 1, 1);
				int num = this.cursorIndex - 1;
				this.cursorIndex = num;
				this.selectIndex = num;
				this.ClearCursorPos();
				return true;
			}
			return false;
		}

		public void SelectAll()
		{
			this.cursorIndex = 0;
			this.selectIndex = this.text.Length;
			this.ClearCursorPos();
		}

		public void SelectNone()
		{
			this.selectIndex = this.cursorIndex;
			this.ClearCursorPos();
		}

		public bool DeleteSelection()
		{
			if (this.cursorIndex == this.selectIndex)
			{
				return false;
			}
			if (this.cursorIndex < this.selectIndex)
			{
				this.m_Content.text = this.text.Substring(0, this.cursorIndex) + this.text.Substring(this.selectIndex, this.text.Length - this.selectIndex);
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.m_Content.text = this.text.Substring(0, this.selectIndex) + this.text.Substring(this.cursorIndex, this.text.Length - this.cursorIndex);
				this.cursorIndex = this.selectIndex;
			}
			this.ClearCursorPos();
			return true;
		}

		public void ReplaceSelection(string replace)
		{
			this.DeleteSelection();
			this.m_Content.text = this.text.Insert(this.cursorIndex, replace);
			this.selectIndex = (this.cursorIndex += replace.Length);
			this.ClearCursorPos();
		}

		public void Insert(char c)
		{
			this.ReplaceSelection(c.ToString());
		}

		public void MoveSelectionToAltCursor()
		{
			if (this.m_iAltCursorPos == -1)
			{
				return;
			}
			int iAltCursorPos = this.m_iAltCursorPos;
			string selectedText = this.SelectedText;
			this.m_Content.text = this.text.Insert(iAltCursorPos, selectedText);
			if (iAltCursorPos < this.cursorIndex)
			{
				this.cursorIndex += selectedText.Length;
				this.selectIndex += selectedText.Length;
			}
			this.DeleteSelection();
			int num = iAltCursorPos;
			this.cursorIndex = num;
			this.selectIndex = num;
			this.ClearCursorPos();
		}

		public void MoveRight()
		{
			this.ClearCursorPos();
			if (this.selectIndex == this.cursorIndex)
			{
				this.cursorIndex++;
				this.DetectFocusChange();
				this.selectIndex = this.cursorIndex;
			}
			else if (this.selectIndex > this.cursorIndex)
			{
				this.cursorIndex = this.selectIndex;
			}
			else
			{
				this.selectIndex = this.cursorIndex;
			}
		}

		public void MoveLeft()
		{
			if (this.selectIndex == this.cursorIndex)
			{
				this.cursorIndex--;
				this.selectIndex = this.cursorIndex;
			}
			else if (this.selectIndex > this.cursorIndex)
			{
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.cursorIndex = this.selectIndex;
			}
			this.ClearCursorPos();
		}

		public void MoveUp()
		{
			if (this.selectIndex < this.cursorIndex)
			{
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.cursorIndex = this.selectIndex;
			}
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y - 1f;
			int cursorStringIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, this.graphicalCursorPos);
			this.selectIndex = cursorStringIndex;
			this.cursorIndex = cursorStringIndex;
			if (this.cursorIndex <= 0)
			{
				this.ClearCursorPos();
			}
		}

		public void MoveDown()
		{
			if (this.selectIndex > this.cursorIndex)
			{
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.cursorIndex = this.selectIndex;
			}
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y + (this.style.lineHeight + 5f);
			int cursorStringIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, this.graphicalCursorPos);
			this.selectIndex = cursorStringIndex;
			this.cursorIndex = cursorStringIndex;
			if (this.cursorIndex == this.text.Length)
			{
				this.ClearCursorPos();
			}
		}

		public void MoveLineStart()
		{
			int num = (this.selectIndex >= this.cursorIndex) ? this.cursorIndex : this.selectIndex;
			int num2 = num;
			int num3;
			while (num2-- != 0)
			{
				if (this.text[num2] == '\n')
				{
					num3 = num2 + 1;
					this.cursorIndex = num3;
					this.selectIndex = num3;
					return;
				}
			}
			num3 = 0;
			this.cursorIndex = num3;
			this.selectIndex = num3;
		}

		public void MoveLineEnd()
		{
			int num = (this.selectIndex <= this.cursorIndex) ? this.cursorIndex : this.selectIndex;
			int i = num;
			int length = this.text.Length;
			int num2;
			while (i < length)
			{
				if (this.text[i] == '\n')
				{
					num2 = i;
					this.cursorIndex = num2;
					this.selectIndex = num2;
					return;
				}
				i++;
			}
			num2 = length;
			this.cursorIndex = num2;
			this.selectIndex = num2;
		}

		public void MoveGraphicalLineStart()
		{
			int graphicalLineStart = this.GetGraphicalLineStart((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			this.selectIndex = graphicalLineStart;
			this.cursorIndex = graphicalLineStart;
		}

		public void MoveGraphicalLineEnd()
		{
			int graphicalLineEnd = this.GetGraphicalLineEnd((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			this.selectIndex = graphicalLineEnd;
			this.cursorIndex = graphicalLineEnd;
		}

		public void MoveTextStart()
		{
			int num = 0;
			this.cursorIndex = num;
			this.selectIndex = num;
		}

		public void MoveTextEnd()
		{
			int length = this.text.Length;
			this.cursorIndex = length;
			this.selectIndex = length;
		}

		private int IndexOfEndOfLine(int startIndex)
		{
			int num = this.text.IndexOf('\n', startIndex);
			return (num == -1) ? this.text.Length : num;
		}

		public void MoveParagraphForward()
		{
			this.cursorIndex = ((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			if (this.cursorIndex < this.text.Length)
			{
				int num = this.IndexOfEndOfLine(this.cursorIndex + 1);
				this.cursorIndex = num;
				this.selectIndex = num;
			}
		}

		public void MoveParagraphBackward()
		{
			this.cursorIndex = ((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			if (this.cursorIndex > 1)
			{
				int num = this.text.LastIndexOf('\n', this.cursorIndex - 2) + 1;
				this.cursorIndex = num;
				this.selectIndex = num;
			}
			else
			{
				int num = 0;
				this.cursorIndex = num;
				this.selectIndex = num;
			}
		}

		public void MoveCursorToPosition(Vector2 cursorPosition)
		{
			this.selectIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
			if (!Event.current.shift)
			{
				this.cursorIndex = this.selectIndex;
			}
			this.DetectFocusChange();
		}

		public void MoveAltCursorToPosition(Vector2 cursorPosition)
		{
			int cursorStringIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
			this.m_iAltCursorPos = Mathf.Min(this.text.Length, cursorStringIndex);
			this.DetectFocusChange();
		}

		public bool IsOverSelection(Vector2 cursorPosition)
		{
			int cursorStringIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
			return cursorStringIndex < Mathf.Max(this.cursorIndex, this.selectIndex) && cursorStringIndex > Mathf.Min(this.cursorIndex, this.selectIndex);
		}

		public void SelectToPosition(Vector2 cursorPosition)
		{
			if (!this.m_MouseDragSelectsWholeWords)
			{
				this.cursorIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
			}
			else
			{
				int num = this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPosition + this.scrollOffset);
				if (this.m_DblClickSnap == TextEditor.DblClickSnapping.WORDS)
				{
					if (num < this.m_DblClickInitPos)
					{
						this.cursorIndex = this.FindEndOfClassification(num, -1);
						this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPos, 1);
					}
					else
					{
						if (num >= this.text.Length)
						{
							num = this.text.Length - 1;
						}
						this.cursorIndex = this.FindEndOfClassification(num, 1);
						this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPos - 1, -1);
					}
				}
				else if (num < this.m_DblClickInitPos)
				{
					if (num > 0)
					{
						this.cursorIndex = this.text.LastIndexOf('\n', Mathf.Max(0, num - 2)) + 1;
					}
					else
					{
						this.cursorIndex = 0;
					}
					this.selectIndex = this.text.LastIndexOf('\n', this.m_DblClickInitPos);
				}
				else
				{
					if (num < this.text.Length)
					{
						this.cursorIndex = this.IndexOfEndOfLine(num);
					}
					else
					{
						this.cursorIndex = this.text.Length;
					}
					this.selectIndex = this.text.LastIndexOf('\n', Mathf.Max(0, this.m_DblClickInitPos - 2)) + 1;
				}
			}
		}

		public void SelectLeft()
		{
			if (this.m_bJustSelected && this.cursorIndex > this.selectIndex)
			{
				int cursorIndex = this.cursorIndex;
				this.cursorIndex = this.selectIndex;
				this.selectIndex = cursorIndex;
			}
			this.m_bJustSelected = false;
			this.cursorIndex--;
		}

		public void SelectRight()
		{
			if (this.m_bJustSelected && this.cursorIndex < this.selectIndex)
			{
				int cursorIndex = this.cursorIndex;
				this.cursorIndex = this.selectIndex;
				this.selectIndex = cursorIndex;
			}
			this.m_bJustSelected = false;
			this.cursorIndex++;
		}

		public void SelectUp()
		{
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y - 1f;
			this.cursorIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, this.graphicalCursorPos);
		}

		public void SelectDown()
		{
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y + (this.style.lineHeight + 5f);
			this.cursorIndex = this.style.GetCursorStringIndex(this.position, this.m_Content, this.graphicalCursorPos);
		}

		public void SelectTextEnd()
		{
			this.cursorIndex = this.text.Length;
		}

		public void SelectTextStart()
		{
			this.cursorIndex = 0;
		}

		public void MouseDragSelectsWholeWords(bool on)
		{
			this.m_MouseDragSelectsWholeWords = on;
			this.m_DblClickInitPos = this.cursorIndex;
		}

		public void DblClickSnap(TextEditor.DblClickSnapping snapping)
		{
			this.m_DblClickSnap = snapping;
		}

		private int GetGraphicalLineStart(int p)
		{
			Vector2 cursorPixelPosition = this.style.GetCursorPixelPosition(this.position, this.m_Content, p);
			cursorPixelPosition.x = 0f;
			return this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPixelPosition);
		}

		private int GetGraphicalLineEnd(int p)
		{
			Vector2 cursorPixelPosition = this.style.GetCursorPixelPosition(this.position, this.m_Content, p);
			cursorPixelPosition.x += 5000f;
			return this.style.GetCursorStringIndex(this.position, this.m_Content, cursorPixelPosition);
		}

		private int FindNextSeperator(int startPos)
		{
			int length = this.text.Length;
			while (startPos < length && !TextEditor.isLetterLikeChar(this.text[startPos]))
			{
				startPos++;
			}
			while (startPos < length && TextEditor.isLetterLikeChar(this.text[startPos]))
			{
				startPos++;
			}
			return startPos;
		}

		private static bool isLetterLikeChar(char c)
		{
			return char.IsLetterOrDigit(c) || c == '\'';
		}

		private int FindPrevSeperator(int startPos)
		{
			startPos--;
			while (startPos > 0 && !TextEditor.isLetterLikeChar(this.text[startPos]))
			{
				startPos--;
			}
			while (startPos >= 0 && TextEditor.isLetterLikeChar(this.text[startPos]))
			{
				startPos--;
			}
			return startPos + 1;
		}

		public void MoveWordRight()
		{
			this.cursorIndex = ((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			int num = this.FindNextSeperator(this.cursorIndex);
			this.selectIndex = num;
			this.cursorIndex = num;
			this.ClearCursorPos();
		}

		public void MoveToStartOfNextWord()
		{
			this.ClearCursorPos();
			if (this.cursorIndex != this.selectIndex)
			{
				this.MoveRight();
				return;
			}
			int num = this.FindStartOfNextWord(this.cursorIndex);
			this.selectIndex = num;
			this.cursorIndex = num;
		}

		public void MoveToEndOfPreviousWord()
		{
			this.ClearCursorPos();
			if (this.cursorIndex != this.selectIndex)
			{
				this.MoveLeft();
				return;
			}
			int num = this.FindEndOfPreviousWord(this.cursorIndex);
			this.selectIndex = num;
			this.cursorIndex = num;
		}

		public void SelectToStartOfNextWord()
		{
			this.ClearCursorPos();
			this.cursorIndex = this.FindStartOfNextWord(this.cursorIndex);
		}

		public void SelectToEndOfPreviousWord()
		{
			this.ClearCursorPos();
			this.cursorIndex = this.FindEndOfPreviousWord(this.cursorIndex);
		}

		private TextEditor.CharacterType ClassifyChar(char c)
		{
			if (char.IsWhiteSpace(c))
			{
				return TextEditor.CharacterType.WhiteSpace;
			}
			if (char.IsLetterOrDigit(c) || c == '\'')
			{
				return TextEditor.CharacterType.LetterLike;
			}
			return TextEditor.CharacterType.Symbol;
		}

		public int FindStartOfNextWord(int p)
		{
			int length = this.text.Length;
			if (p == length)
			{
				return p;
			}
			char c = this.text[p];
			TextEditor.CharacterType characterType = this.ClassifyChar(c);
			if (characterType != TextEditor.CharacterType.WhiteSpace)
			{
				p++;
				while (p < length && this.ClassifyChar(this.text[p]) == characterType)
				{
					p++;
				}
			}
			else if (c == '\t' || c == '\n')
			{
				return p + 1;
			}
			if (p == length)
			{
				return p;
			}
			c = this.text[p];
			if (c == ' ')
			{
				while (p < length && char.IsWhiteSpace(this.text[p]))
				{
					p++;
				}
			}
			else if (c == '\t' || c == '\n')
			{
				return p;
			}
			return p;
		}

		private int FindEndOfPreviousWord(int p)
		{
			if (p == 0)
			{
				return p;
			}
			p--;
			while (p > 0 && this.text[p] == ' ')
			{
				p--;
			}
			TextEditor.CharacterType characterType = this.ClassifyChar(this.text[p]);
			if (characterType != TextEditor.CharacterType.WhiteSpace)
			{
				while (p > 0 && this.ClassifyChar(this.text[p - 1]) == characterType)
				{
					p--;
				}
			}
			return p;
		}

		public void MoveWordLeft()
		{
			this.cursorIndex = ((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			this.cursorIndex = this.FindPrevSeperator(this.cursorIndex);
			this.selectIndex = this.cursorIndex;
		}

		public void SelectWordRight()
		{
			this.ClearCursorPos();
			int selectIndex = this.selectIndex;
			if (this.cursorIndex < this.selectIndex)
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordRight();
				this.selectIndex = selectIndex;
				this.cursorIndex = ((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
				return;
			}
			this.selectIndex = this.cursorIndex;
			this.MoveWordRight();
			this.selectIndex = selectIndex;
		}

		public void SelectWordLeft()
		{
			this.ClearCursorPos();
			int selectIndex = this.selectIndex;
			if (this.cursorIndex > this.selectIndex)
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordLeft();
				this.selectIndex = selectIndex;
				this.cursorIndex = ((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
				return;
			}
			this.selectIndex = this.cursorIndex;
			this.MoveWordLeft();
			this.selectIndex = selectIndex;
		}

		public void ExpandSelectGraphicalLineStart()
		{
			this.ClearCursorPos();
			if (this.cursorIndex < this.selectIndex)
			{
				this.cursorIndex = this.GetGraphicalLineStart(this.cursorIndex);
			}
			else
			{
				int cursorIndex = this.cursorIndex;
				this.cursorIndex = this.GetGraphicalLineStart(this.selectIndex);
				this.selectIndex = cursorIndex;
			}
		}

		public void ExpandSelectGraphicalLineEnd()
		{
			this.ClearCursorPos();
			if (this.cursorIndex > this.selectIndex)
			{
				this.cursorIndex = this.GetGraphicalLineEnd(this.cursorIndex);
			}
			else
			{
				int cursorIndex = this.cursorIndex;
				this.cursorIndex = this.GetGraphicalLineEnd(this.selectIndex);
				this.selectIndex = cursorIndex;
			}
		}

		public void SelectGraphicalLineStart()
		{
			this.ClearCursorPos();
			this.cursorIndex = this.GetGraphicalLineStart(this.cursorIndex);
		}

		public void SelectGraphicalLineEnd()
		{
			this.ClearCursorPos();
			this.cursorIndex = this.GetGraphicalLineEnd(this.cursorIndex);
		}

		public void SelectParagraphForward()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex < this.selectIndex;
			if (this.cursorIndex < this.text.Length)
			{
				this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex + 1);
				if (flag && this.cursorIndex > this.selectIndex)
				{
					this.cursorIndex = this.selectIndex;
				}
			}
		}

		public void SelectParagraphBackward()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex > this.selectIndex;
			if (this.cursorIndex > 1)
			{
				this.cursorIndex = this.text.LastIndexOf('\n', this.cursorIndex - 2) + 1;
				if (flag && this.cursorIndex < this.selectIndex)
				{
					this.cursorIndex = this.selectIndex;
				}
			}
			else
			{
				int num = 0;
				this.cursorIndex = num;
				this.selectIndex = num;
			}
		}

		public void SelectCurrentWord()
		{
			this.ClearCursorPos();
			int length = this.text.Length;
			this.selectIndex = this.cursorIndex;
			if (length == 0)
			{
				return;
			}
			if (this.cursorIndex >= length)
			{
				this.cursorIndex = length - 1;
			}
			if (this.selectIndex >= length)
			{
				this.selectIndex--;
			}
			if (this.cursorIndex < this.selectIndex)
			{
				this.cursorIndex = this.FindEndOfClassification(this.cursorIndex, -1);
				this.selectIndex = this.FindEndOfClassification(this.selectIndex, 1);
			}
			else
			{
				this.cursorIndex = this.FindEndOfClassification(this.cursorIndex, 1);
				this.selectIndex = this.FindEndOfClassification(this.selectIndex, -1);
			}
			this.m_bJustSelected = true;
		}

		private int FindEndOfClassification(int p, int dir)
		{
			int length = this.text.Length;
			if (p >= length || p < 0)
			{
				return p;
			}
			TextEditor.CharacterType characterType = this.ClassifyChar(this.text[p]);
			while (true)
			{
				p += dir;
				if (p < 0)
				{
					break;
				}
				if (p >= length)
				{
					return length;
				}
				if (this.ClassifyChar(this.text[p]) != characterType)
				{
					goto Block_4;
				}
			}
			return 0;
			Block_4:
			if (dir == 1)
			{
				return p;
			}
			return p + 1;
		}

		public void SelectCurrentParagraph()
		{
			this.ClearCursorPos();
			int length = this.text.Length;
			if (this.cursorIndex < length)
			{
				this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex) + 1;
			}
			if (this.selectIndex != 0)
			{
				this.selectIndex = this.text.LastIndexOf('\n', this.selectIndex - 1) + 1;
			}
		}

		public void UpdateScrollOffsetIfNeeded()
		{
			if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout)
			{
				this.UpdateScrollOffset();
			}
		}

		private void UpdateScrollOffset()
		{
			int cursorIndex = this.cursorIndex;
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.m_Content, cursorIndex);
			Rect rect = this.style.padding.Remove(this.position);
			Vector2 vector = new Vector2(this.style.CalcSize(this.m_Content).x, this.style.CalcHeight(this.m_Content, this.position.width));
			if (vector.x < this.position.width)
			{
				this.scrollOffset.x = 0f;
			}
			else if (this.m_RevealCursor)
			{
				if (this.graphicalCursorPos.x + 1f > this.scrollOffset.x + rect.width)
				{
					this.scrollOffset.x = this.graphicalCursorPos.x - rect.width;
				}
				if (this.graphicalCursorPos.x < this.scrollOffset.x + (float)this.style.padding.left)
				{
					this.scrollOffset.x = this.graphicalCursorPos.x - (float)this.style.padding.left;
				}
			}
			if (vector.y < rect.height)
			{
				this.scrollOffset.y = 0f;
			}
			else if (this.m_RevealCursor)
			{
				if (this.graphicalCursorPos.y + this.style.lineHeight > this.scrollOffset.y + rect.height + (float)this.style.padding.top)
				{
					this.scrollOffset.y = this.graphicalCursorPos.y - rect.height - (float)this.style.padding.top + this.style.lineHeight;
				}
				if (this.graphicalCursorPos.y < this.scrollOffset.y + (float)this.style.padding.top)
				{
					this.scrollOffset.y = this.graphicalCursorPos.y - (float)this.style.padding.top;
				}
			}
			if (this.scrollOffset.y > 0f && vector.y - this.scrollOffset.y < rect.height)
			{
				this.scrollOffset.y = vector.y - rect.height - (float)this.style.padding.top - (float)this.style.padding.bottom;
			}
			this.scrollOffset.y = ((this.scrollOffset.y >= 0f) ? this.scrollOffset.y : 0f);
			this.m_RevealCursor = false;
		}

		public void DrawCursor(string newText)
		{
			string text = this.text;
			int num = this.cursorIndex;
			if (Input.compositionString.Length > 0)
			{
				this.m_Content.text = newText.Substring(0, this.cursorIndex) + Input.compositionString + newText.Substring(this.selectIndex);
				num += Input.compositionString.Length;
			}
			else
			{
				this.m_Content.text = newText;
			}
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.m_Content, num);
			Vector2 contentOffset = this.style.contentOffset;
			this.style.contentOffset -= this.scrollOffset;
			this.style.Internal_clipOffset = this.scrollOffset;
			Input.compositionCursorPos = this.graphicalCursorPos + new Vector2(this.position.x, this.position.y + this.style.lineHeight) - this.scrollOffset;
			if (Input.compositionString.Length > 0)
			{
				this.style.DrawWithTextSelection(this.position, this.m_Content, this.controlID, this.cursorIndex, this.cursorIndex + Input.compositionString.Length, true);
			}
			else
			{
				this.style.DrawWithTextSelection(this.position, this.m_Content, this.controlID, this.cursorIndex, this.selectIndex);
			}
			if (this.m_iAltCursorPos != -1)
			{
				this.style.DrawCursor(this.position, this.m_Content, this.controlID, this.m_iAltCursorPos);
			}
			this.style.contentOffset = contentOffset;
			this.style.Internal_clipOffset = Vector2.zero;
			this.m_Content.text = text;
		}

		private bool PerformOperation(TextEditor.TextEditOp operation)
		{
			this.m_RevealCursor = true;
			switch (operation)
			{
			case TextEditor.TextEditOp.MoveLeft:
				this.MoveLeft();
				return false;
			case TextEditor.TextEditOp.MoveRight:
				this.MoveRight();
				return false;
			case TextEditor.TextEditOp.MoveUp:
				this.MoveUp();
				return false;
			case TextEditor.TextEditOp.MoveDown:
				this.MoveDown();
				return false;
			case TextEditor.TextEditOp.MoveLineStart:
				this.MoveLineStart();
				return false;
			case TextEditor.TextEditOp.MoveLineEnd:
				this.MoveLineEnd();
				return false;
			case TextEditor.TextEditOp.MoveTextStart:
				this.MoveTextStart();
				return false;
			case TextEditor.TextEditOp.MoveTextEnd:
				this.MoveTextEnd();
				return false;
			case TextEditor.TextEditOp.MoveGraphicalLineStart:
				this.MoveGraphicalLineStart();
				return false;
			case TextEditor.TextEditOp.MoveGraphicalLineEnd:
				this.MoveGraphicalLineEnd();
				return false;
			case TextEditor.TextEditOp.MoveWordLeft:
				this.MoveWordLeft();
				return false;
			case TextEditor.TextEditOp.MoveWordRight:
				this.MoveWordRight();
				return false;
			case TextEditor.TextEditOp.MoveParagraphForward:
				this.MoveParagraphForward();
				return false;
			case TextEditor.TextEditOp.MoveParagraphBackward:
				this.MoveParagraphBackward();
				return false;
			case TextEditor.TextEditOp.MoveToStartOfNextWord:
				this.MoveToStartOfNextWord();
				return false;
			case TextEditor.TextEditOp.MoveToEndOfPreviousWord:
				this.MoveToEndOfPreviousWord();
				return false;
			case TextEditor.TextEditOp.SelectLeft:
				this.SelectLeft();
				return false;
			case TextEditor.TextEditOp.SelectRight:
				this.SelectRight();
				return false;
			case TextEditor.TextEditOp.SelectUp:
				this.SelectUp();
				return false;
			case TextEditor.TextEditOp.SelectDown:
				this.SelectDown();
				return false;
			case TextEditor.TextEditOp.SelectTextStart:
				this.SelectTextStart();
				return false;
			case TextEditor.TextEditOp.SelectTextEnd:
				this.SelectTextEnd();
				return false;
			case TextEditor.TextEditOp.ExpandSelectGraphicalLineStart:
				this.ExpandSelectGraphicalLineStart();
				return false;
			case TextEditor.TextEditOp.ExpandSelectGraphicalLineEnd:
				this.ExpandSelectGraphicalLineEnd();
				return false;
			case TextEditor.TextEditOp.SelectGraphicalLineStart:
				this.SelectGraphicalLineStart();
				return false;
			case TextEditor.TextEditOp.SelectGraphicalLineEnd:
				this.SelectGraphicalLineEnd();
				return false;
			case TextEditor.TextEditOp.SelectWordLeft:
				this.SelectWordLeft();
				return false;
			case TextEditor.TextEditOp.SelectWordRight:
				this.SelectWordRight();
				return false;
			case TextEditor.TextEditOp.SelectToEndOfPreviousWord:
				this.SelectToEndOfPreviousWord();
				return false;
			case TextEditor.TextEditOp.SelectToStartOfNextWord:
				this.SelectToStartOfNextWord();
				return false;
			case TextEditor.TextEditOp.SelectParagraphBackward:
				this.SelectParagraphBackward();
				return false;
			case TextEditor.TextEditOp.SelectParagraphForward:
				this.SelectParagraphForward();
				return false;
			case TextEditor.TextEditOp.Delete:
				return this.Delete();
			case TextEditor.TextEditOp.Backspace:
				return this.Backspace();
			case TextEditor.TextEditOp.DeleteWordBack:
				return this.DeleteWordBack();
			case TextEditor.TextEditOp.DeleteWordForward:
				return this.DeleteWordForward();
			case TextEditor.TextEditOp.DeleteLineBack:
				return this.DeleteLineBack();
			case TextEditor.TextEditOp.Cut:
				return this.Cut();
			case TextEditor.TextEditOp.Copy:
				this.Copy();
				return false;
			case TextEditor.TextEditOp.Paste:
				return this.Paste();
			case TextEditor.TextEditOp.SelectAll:
				this.SelectAll();
				return false;
			case TextEditor.TextEditOp.SelectNone:
				this.SelectNone();
				return false;
			}
			Debug.Log("Unimplemented: " + operation);
			return false;
		}

		public void SaveBackup()
		{
			this.oldText = this.text;
			this.oldPos = this.cursorIndex;
			this.oldSelectPos = this.selectIndex;
		}

		public void Undo()
		{
			this.m_Content.text = this.oldText;
			this.cursorIndex = this.oldPos;
			this.selectIndex = this.oldSelectPos;
		}

		public bool Cut()
		{
			if (this.isPasswordField)
			{
				return false;
			}
			this.Copy();
			return this.DeleteSelection();
		}

		public void Copy()
		{
			if (this.selectIndex == this.cursorIndex)
			{
				return;
			}
			if (this.isPasswordField)
			{
				return;
			}
			string systemCopyBuffer;
			if (this.cursorIndex < this.selectIndex)
			{
				systemCopyBuffer = this.text.Substring(this.cursorIndex, this.selectIndex - this.cursorIndex);
			}
			else
			{
				systemCopyBuffer = this.text.Substring(this.selectIndex, this.cursorIndex - this.selectIndex);
			}
			GUIUtility.systemCopyBuffer = systemCopyBuffer;
		}

		private static string ReplaceNewlinesWithSpaces(string value)
		{
			value = value.Replace("\r\n", " ");
			value = value.Replace('\n', ' ');
			value = value.Replace('\r', ' ');
			return value;
		}

		public bool Paste()
		{
			string text = GUIUtility.systemCopyBuffer;
			if (text != string.Empty)
			{
				if (!this.multiline)
				{
					text = TextEditor.ReplaceNewlinesWithSpaces(text);
				}
				this.ReplaceSelection(text);
				return true;
			}
			return false;
		}

		private static void MapKey(string key, TextEditor.TextEditOp action)
		{
			TextEditor.s_Keyactions[Event.KeyboardEvent(key)] = action;
		}

		private void InitKeyActions()
		{
			if (TextEditor.s_Keyactions != null)
			{
				return;
			}
			TextEditor.s_Keyactions = new Dictionary<Event, TextEditor.TextEditOp>();
			TextEditor.MapKey("left", TextEditor.TextEditOp.MoveLeft);
			TextEditor.MapKey("right", TextEditor.TextEditOp.MoveRight);
			TextEditor.MapKey("up", TextEditor.TextEditOp.MoveUp);
			TextEditor.MapKey("down", TextEditor.TextEditOp.MoveDown);
			TextEditor.MapKey("#left", TextEditor.TextEditOp.SelectLeft);
			TextEditor.MapKey("#right", TextEditor.TextEditOp.SelectRight);
			TextEditor.MapKey("#up", TextEditor.TextEditOp.SelectUp);
			TextEditor.MapKey("#down", TextEditor.TextEditOp.SelectDown);
			TextEditor.MapKey("delete", TextEditor.TextEditOp.Delete);
			TextEditor.MapKey("backspace", TextEditor.TextEditOp.Backspace);
			TextEditor.MapKey("#backspace", TextEditor.TextEditOp.Backspace);
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.OSXEditor || (Application.platform == RuntimePlatform.WebGLPlayer && SystemInfo.operatingSystem.StartsWith("Mac")))
			{
				TextEditor.MapKey("^left", TextEditor.TextEditOp.MoveGraphicalLineStart);
				TextEditor.MapKey("^right", TextEditor.TextEditOp.MoveGraphicalLineEnd);
				TextEditor.MapKey("&left", TextEditor.TextEditOp.MoveWordLeft);
				TextEditor.MapKey("&right", TextEditor.TextEditOp.MoveWordRight);
				TextEditor.MapKey("&up", TextEditor.TextEditOp.MoveParagraphBackward);
				TextEditor.MapKey("&down", TextEditor.TextEditOp.MoveParagraphForward);
				TextEditor.MapKey("%left", TextEditor.TextEditOp.MoveGraphicalLineStart);
				TextEditor.MapKey("%right", TextEditor.TextEditOp.MoveGraphicalLineEnd);
				TextEditor.MapKey("%up", TextEditor.TextEditOp.MoveTextStart);
				TextEditor.MapKey("%down", TextEditor.TextEditOp.MoveTextEnd);
				TextEditor.MapKey("#home", TextEditor.TextEditOp.SelectTextStart);
				TextEditor.MapKey("#end", TextEditor.TextEditOp.SelectTextEnd);
				TextEditor.MapKey("#^left", TextEditor.TextEditOp.ExpandSelectGraphicalLineStart);
				TextEditor.MapKey("#^right", TextEditor.TextEditOp.ExpandSelectGraphicalLineEnd);
				TextEditor.MapKey("#^up", TextEditor.TextEditOp.SelectParagraphBackward);
				TextEditor.MapKey("#^down", TextEditor.TextEditOp.SelectParagraphForward);
				TextEditor.MapKey("#&left", TextEditor.TextEditOp.SelectWordLeft);
				TextEditor.MapKey("#&right", TextEditor.TextEditOp.SelectWordRight);
				TextEditor.MapKey("#&up", TextEditor.TextEditOp.SelectParagraphBackward);
				TextEditor.MapKey("#&down", TextEditor.TextEditOp.SelectParagraphForward);
				TextEditor.MapKey("#%left", TextEditor.TextEditOp.ExpandSelectGraphicalLineStart);
				TextEditor.MapKey("#%right", TextEditor.TextEditOp.ExpandSelectGraphicalLineEnd);
				TextEditor.MapKey("#%up", TextEditor.TextEditOp.SelectTextStart);
				TextEditor.MapKey("#%down", TextEditor.TextEditOp.SelectTextEnd);
				TextEditor.MapKey("%a", TextEditor.TextEditOp.SelectAll);
				TextEditor.MapKey("%x", TextEditor.TextEditOp.Cut);
				TextEditor.MapKey("%c", TextEditor.TextEditOp.Copy);
				TextEditor.MapKey("%v", TextEditor.TextEditOp.Paste);
				TextEditor.MapKey("^d", TextEditor.TextEditOp.Delete);
				TextEditor.MapKey("^h", TextEditor.TextEditOp.Backspace);
				TextEditor.MapKey("^b", TextEditor.TextEditOp.MoveLeft);
				TextEditor.MapKey("^f", TextEditor.TextEditOp.MoveRight);
				TextEditor.MapKey("^a", TextEditor.TextEditOp.MoveLineStart);
				TextEditor.MapKey("^e", TextEditor.TextEditOp.MoveLineEnd);
				TextEditor.MapKey("&delete", TextEditor.TextEditOp.DeleteWordForward);
				TextEditor.MapKey("&backspace", TextEditor.TextEditOp.DeleteWordBack);
				TextEditor.MapKey("%backspace", TextEditor.TextEditOp.DeleteLineBack);
			}
			else
			{
				TextEditor.MapKey("home", TextEditor.TextEditOp.MoveGraphicalLineStart);
				TextEditor.MapKey("end", TextEditor.TextEditOp.MoveGraphicalLineEnd);
				TextEditor.MapKey("%left", TextEditor.TextEditOp.MoveWordLeft);
				TextEditor.MapKey("%right", TextEditor.TextEditOp.MoveWordRight);
				TextEditor.MapKey("%up", TextEditor.TextEditOp.MoveParagraphBackward);
				TextEditor.MapKey("%down", TextEditor.TextEditOp.MoveParagraphForward);
				TextEditor.MapKey("^left", TextEditor.TextEditOp.MoveToEndOfPreviousWord);
				TextEditor.MapKey("^right", TextEditor.TextEditOp.MoveToStartOfNextWord);
				TextEditor.MapKey("^up", TextEditor.TextEditOp.MoveParagraphBackward);
				TextEditor.MapKey("^down", TextEditor.TextEditOp.MoveParagraphForward);
				TextEditor.MapKey("#^left", TextEditor.TextEditOp.SelectToEndOfPreviousWord);
				TextEditor.MapKey("#^right", TextEditor.TextEditOp.SelectToStartOfNextWord);
				TextEditor.MapKey("#^up", TextEditor.TextEditOp.SelectParagraphBackward);
				TextEditor.MapKey("#^down", TextEditor.TextEditOp.SelectParagraphForward);
				TextEditor.MapKey("#home", TextEditor.TextEditOp.SelectGraphicalLineStart);
				TextEditor.MapKey("#end", TextEditor.TextEditOp.SelectGraphicalLineEnd);
				TextEditor.MapKey("^delete", TextEditor.TextEditOp.DeleteWordForward);
				TextEditor.MapKey("^backspace", TextEditor.TextEditOp.DeleteWordBack);
				TextEditor.MapKey("%backspace", TextEditor.TextEditOp.DeleteLineBack);
				TextEditor.MapKey("^a", TextEditor.TextEditOp.SelectAll);
				TextEditor.MapKey("^x", TextEditor.TextEditOp.Cut);
				TextEditor.MapKey("^c", TextEditor.TextEditOp.Copy);
				TextEditor.MapKey("^v", TextEditor.TextEditOp.Paste);
				TextEditor.MapKey("#delete", TextEditor.TextEditOp.Cut);
				TextEditor.MapKey("^insert", TextEditor.TextEditOp.Copy);
				TextEditor.MapKey("#insert", TextEditor.TextEditOp.Paste);
			}
		}

		public void DetectFocusChange()
		{
			if (this.m_HasFocus && this.controlID != GUIUtility.keyboardControl)
			{
				this.OnLostFocus();
			}
			if (!this.m_HasFocus && this.controlID == GUIUtility.keyboardControl)
			{
				this.OnFocus();
			}
		}

		private void ClampTextIndex(ref int index)
		{
			index = Mathf.Clamp(index, 0, this.text.Length);
		}
	}
}
