using System;
using System.Collections.Generic;
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
		public int pos;
		public int selectPos;
		public int controlID;
		public GUIContent content = new GUIContent();
		public GUIStyle style = GUIStyle.none;
		public Rect position;
		public bool multiline;
		public bool hasHorizontalCursorPos;
		public bool isPasswordField;
		internal bool m_HasFocus;
		public Vector2 scrollOffset = Vector2.zero;
		private bool m_TextHeightPotentiallyChanged;
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
		public bool hasSelection
		{
			get
			{
				return this.pos != this.selectPos;
			}
		}
		public string SelectedText
		{
			get
			{
				int length = this.content.text.Length;
				if (this.pos > length)
				{
					this.pos = length;
				}
				if (this.selectPos > length)
				{
					this.selectPos = length;
				}
				if (this.pos == this.selectPos)
				{
					return string.Empty;
				}
				if (this.pos < this.selectPos)
				{
					return this.content.text.Substring(this.pos, this.selectPos - this.pos);
				}
				return this.content.text.Substring(this.selectPos, this.pos - this.selectPos);
			}
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
				this.pos = (this.selectPos = 0);
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
				this.graphicalCursorPos = this.style.GetCursorPixelPosition(this.position, this.content, this.pos);
				this.graphicalSelectCursorPos = this.style.GetCursorPixelPosition(this.position, this.content, this.selectPos);
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
				this.UpdateScrollOffset();
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
			int num = this.pos;
			int num2 = num;
			while (num2-- != 0)
			{
				if (this.content.text[num2] == '\n')
				{
					num = num2 + 1;
					break;
				}
			}
			if (num2 == -1)
			{
				num = 0;
			}
			if (this.pos != num)
			{
				this.content.text = this.content.text.Remove(num, this.pos - num);
				this.selectPos = (this.pos = num);
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
			int num = this.FindEndOfPreviousWord(this.pos);
			if (this.pos != num)
			{
				this.content.text = this.content.text.Remove(num, this.pos - num);
				this.selectPos = (this.pos = num);
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
			int num = this.FindStartOfNextWord(this.pos);
			if (this.pos < this.content.text.Length)
			{
				this.content.text = this.content.text.Remove(this.pos, num - this.pos);
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
			if (this.pos < this.content.text.Length)
			{
				this.content.text = this.content.text.Remove(this.pos, 1);
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
			if (this.pos > 0)
			{
				this.content.text = this.content.text.Remove(this.pos - 1, 1);
				this.selectPos = --this.pos;
				this.ClearCursorPos();
				return true;
			}
			return false;
		}
		public void SelectAll()
		{
			this.pos = 0;
			this.selectPos = this.content.text.Length;
			this.ClearCursorPos();
		}
		public void SelectNone()
		{
			this.selectPos = this.pos;
			this.ClearCursorPos();
		}
		public bool DeleteSelection()
		{
			int length = this.content.text.Length;
			if (this.pos > length)
			{
				this.pos = length;
			}
			if (this.selectPos > length)
			{
				this.selectPos = length;
			}
			if (this.pos == this.selectPos)
			{
				return false;
			}
			if (this.pos < this.selectPos)
			{
				this.content.text = this.content.text.Substring(0, this.pos) + this.content.text.Substring(this.selectPos, this.content.text.Length - this.selectPos);
				this.selectPos = this.pos;
			}
			else
			{
				this.content.text = this.content.text.Substring(0, this.selectPos) + this.content.text.Substring(this.pos, this.content.text.Length - this.pos);
				this.pos = this.selectPos;
			}
			this.ClearCursorPos();
			return true;
		}
		public void ReplaceSelection(string replace)
		{
			this.DeleteSelection();
			this.content.text = this.content.text.Insert(this.pos, replace);
			this.selectPos = (this.pos += replace.Length);
			this.ClearCursorPos();
			this.UpdateScrollOffset();
			this.m_TextHeightPotentiallyChanged = true;
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
			this.content.text = this.content.text.Insert(iAltCursorPos, selectedText);
			if (iAltCursorPos < this.pos)
			{
				this.pos += selectedText.Length;
				this.selectPos += selectedText.Length;
			}
			this.DeleteSelection();
			this.selectPos = (this.pos = iAltCursorPos);
			this.ClearCursorPos();
			this.UpdateScrollOffset();
		}
		public void MoveRight()
		{
			this.ClearCursorPos();
			if (this.selectPos == this.pos)
			{
				this.pos++;
				this.ClampPos();
				this.selectPos = this.pos;
			}
			else
			{
				if (this.selectPos > this.pos)
				{
					this.pos = this.selectPos;
				}
				else
				{
					this.selectPos = this.pos;
				}
			}
			this.UpdateScrollOffset();
		}
		public void MoveLeft()
		{
			if (this.selectPos == this.pos)
			{
				this.pos--;
				if (this.pos < 0)
				{
					this.pos = 0;
				}
				this.selectPos = this.pos;
			}
			else
			{
				if (this.selectPos > this.pos)
				{
					this.selectPos = this.pos;
				}
				else
				{
					this.pos = this.selectPos;
				}
			}
			this.ClearCursorPos();
			this.UpdateScrollOffset();
		}
		public void MoveUp()
		{
			if (this.selectPos < this.pos)
			{
				this.selectPos = this.pos;
			}
			else
			{
				this.pos = this.selectPos;
			}
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y - 1f;
			this.pos = (this.selectPos = this.style.GetCursorStringIndex(this.position, this.content, this.graphicalCursorPos));
			if (this.pos <= 0)
			{
				this.ClearCursorPos();
			}
			this.UpdateScrollOffset();
		}
		public void MoveDown()
		{
			if (this.selectPos > this.pos)
			{
				this.selectPos = this.pos;
			}
			else
			{
				this.pos = this.selectPos;
			}
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y + (this.style.lineHeight + 5f);
			this.pos = (this.selectPos = this.style.GetCursorStringIndex(this.position, this.content, this.graphicalCursorPos));
			if (this.pos == this.content.text.Length)
			{
				this.ClearCursorPos();
			}
			this.UpdateScrollOffset();
		}
		public void MoveLineStart()
		{
			int num = (this.selectPos >= this.pos) ? this.pos : this.selectPos;
			int num2 = num;
			while (num2-- != 0)
			{
				if (this.content.text[num2] == '\n')
				{
					this.selectPos = (this.pos = num2 + 1);
					return;
				}
			}
			this.selectPos = (this.pos = 0);
			this.UpdateScrollOffset();
		}
		public void MoveLineEnd()
		{
			int num = (this.selectPos <= this.pos) ? this.pos : this.selectPos;
			int i = num;
			int length = this.content.text.Length;
			while (i < length)
			{
				if (this.content.text[i] == '\n')
				{
					this.selectPos = (this.pos = i);
					return;
				}
				i++;
			}
			this.selectPos = (this.pos = length);
			this.UpdateScrollOffset();
		}
		public void MoveGraphicalLineStart()
		{
			this.pos = (this.selectPos = this.GetGraphicalLineStart((this.pos >= this.selectPos) ? this.selectPos : this.pos));
			this.UpdateScrollOffset();
		}
		public void MoveGraphicalLineEnd()
		{
			this.pos = (this.selectPos = this.GetGraphicalLineEnd((this.pos <= this.selectPos) ? this.selectPos : this.pos));
			this.UpdateScrollOffset();
		}
		public void MoveTextStart()
		{
			this.selectPos = (this.pos = 0);
			this.UpdateScrollOffset();
		}
		public void MoveTextEnd()
		{
			this.selectPos = (this.pos = this.content.text.Length);
			this.UpdateScrollOffset();
		}
		public void MoveParagraphForward()
		{
			this.pos = ((this.pos <= this.selectPos) ? this.selectPos : this.pos);
			if (this.pos < this.content.text.Length)
			{
				this.selectPos = (this.pos = this.content.text.IndexOf('\n', this.pos + 1));
				if (this.pos == -1)
				{
					this.selectPos = (this.pos = this.content.text.Length);
				}
			}
			this.UpdateScrollOffset();
		}
		public void MoveParagraphBackward()
		{
			this.pos = ((this.pos >= this.selectPos) ? this.selectPos : this.pos);
			if (this.pos > 1)
			{
				this.selectPos = (this.pos = this.content.text.LastIndexOf('\n', this.pos - 2) + 1);
			}
			else
			{
				this.selectPos = (this.pos = 0);
			}
			this.UpdateScrollOffset();
		}
		public void MoveCursorToPosition(Vector2 cursorPosition)
		{
			this.selectPos = this.style.GetCursorStringIndex(this.position, this.content, cursorPosition + this.scrollOffset);
			if (!Event.current.shift)
			{
				this.pos = this.selectPos;
			}
			this.ClampPos();
			this.UpdateScrollOffset();
		}
		public void MoveAltCursorToPosition(Vector2 cursorPosition)
		{
			this.m_iAltCursorPos = this.style.GetCursorStringIndex(this.position, this.content, cursorPosition + this.scrollOffset);
			this.ClampPos();
			this.UpdateScrollOffset();
		}
		public bool IsOverSelection(Vector2 cursorPosition)
		{
			int cursorStringIndex = this.style.GetCursorStringIndex(this.position, this.content, cursorPosition + this.scrollOffset);
			return cursorStringIndex < Mathf.Max(this.pos, this.selectPos) && cursorStringIndex > Mathf.Min(this.pos, this.selectPos);
		}
		public void SelectToPosition(Vector2 cursorPosition)
		{
			if (!this.m_MouseDragSelectsWholeWords)
			{
				this.pos = this.style.GetCursorStringIndex(this.position, this.content, cursorPosition + this.scrollOffset);
			}
			else
			{
				int num = this.style.GetCursorStringIndex(this.position, this.content, cursorPosition + this.scrollOffset);
				if (this.m_DblClickSnap == TextEditor.DblClickSnapping.WORDS)
				{
					if (num < this.m_DblClickInitPos)
					{
						this.pos = this.FindEndOfClassification(num, -1);
						this.selectPos = this.FindEndOfClassification(this.m_DblClickInitPos, 1);
					}
					else
					{
						if (num >= this.content.text.Length)
						{
							num = this.content.text.Length - 1;
						}
						this.pos = this.FindEndOfClassification(num, 1);
						this.selectPos = this.FindEndOfClassification(this.m_DblClickInitPos - 1, -1);
					}
				}
				else
				{
					if (num < this.m_DblClickInitPos)
					{
						if (num > 0)
						{
							this.pos = this.content.text.LastIndexOf('\n', num - 2) + 1;
						}
						else
						{
							this.pos = 0;
						}
						this.selectPos = this.content.text.LastIndexOf('\n', this.m_DblClickInitPos);
					}
					else
					{
						if (num < this.content.text.Length)
						{
							this.pos = this.content.text.IndexOf('\n', num + 1) + 1;
							if (this.pos <= 0)
							{
								this.pos = this.content.text.Length;
							}
						}
						else
						{
							this.pos = this.content.text.Length;
						}
						this.selectPos = this.content.text.LastIndexOf('\n', this.m_DblClickInitPos - 2) + 1;
					}
				}
			}
			this.UpdateScrollOffset();
		}
		public void SelectLeft()
		{
			if (this.m_bJustSelected && this.pos > this.selectPos)
			{
				int num = this.pos;
				this.pos = this.selectPos;
				this.selectPos = num;
			}
			this.m_bJustSelected = false;
			this.pos--;
			if (this.pos < 0)
			{
				this.pos = 0;
			}
			this.UpdateScrollOffset();
		}
		public void SelectRight()
		{
			if (this.m_bJustSelected && this.pos < this.selectPos)
			{
				int num = this.pos;
				this.pos = this.selectPos;
				this.selectPos = num;
			}
			this.m_bJustSelected = false;
			this.pos++;
			int length = this.content.text.Length;
			if (this.pos > length)
			{
				this.pos = length;
			}
			this.UpdateScrollOffset();
		}
		public void SelectUp()
		{
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y - 1f;
			this.pos = this.style.GetCursorStringIndex(this.position, this.content, this.graphicalCursorPos);
			this.UpdateScrollOffset();
		}
		public void SelectDown()
		{
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y + (this.style.lineHeight + 5f);
			this.pos = this.style.GetCursorStringIndex(this.position, this.content, this.graphicalCursorPos);
			this.UpdateScrollOffset();
		}
		public void SelectTextEnd()
		{
			this.pos = this.content.text.Length;
			this.UpdateScrollOffset();
		}
		public void SelectTextStart()
		{
			this.pos = 0;
			this.UpdateScrollOffset();
		}
		public void MouseDragSelectsWholeWords(bool on)
		{
			this.m_MouseDragSelectsWholeWords = on;
			this.m_DblClickInitPos = this.pos;
		}
		public void DblClickSnap(TextEditor.DblClickSnapping snapping)
		{
			this.m_DblClickSnap = snapping;
		}
		private int GetGraphicalLineStart(int p)
		{
			Vector2 cursorPixelPosition = this.style.GetCursorPixelPosition(this.position, this.content, p);
			cursorPixelPosition.x = 0f;
			return this.style.GetCursorStringIndex(this.position, this.content, cursorPixelPosition);
		}
		private int GetGraphicalLineEnd(int p)
		{
			Vector2 cursorPixelPosition = this.style.GetCursorPixelPosition(this.position, this.content, p);
			cursorPixelPosition.x += 5000f;
			return this.style.GetCursorStringIndex(this.position, this.content, cursorPixelPosition);
		}
		private int FindNextSeperator(int startPos)
		{
			int length = this.content.text.Length;
			while (startPos < length && !TextEditor.isLetterLikeChar(this.content.text[startPos]))
			{
				startPos++;
			}
			while (startPos < length && TextEditor.isLetterLikeChar(this.content.text[startPos]))
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
			while (startPos > 0 && !TextEditor.isLetterLikeChar(this.content.text[startPos]))
			{
				startPos--;
			}
			while (startPos >= 0 && TextEditor.isLetterLikeChar(this.content.text[startPos]))
			{
				startPos--;
			}
			return startPos + 1;
		}
		public void MoveWordRight()
		{
			this.pos = ((this.pos <= this.selectPos) ? this.selectPos : this.pos);
			this.pos = (this.selectPos = this.FindNextSeperator(this.pos));
			this.ClearCursorPos();
			this.UpdateScrollOffset();
		}
		public void MoveToStartOfNextWord()
		{
			this.ClearCursorPos();
			if (this.pos != this.selectPos)
			{
				this.MoveRight();
				return;
			}
			this.pos = (this.selectPos = this.FindStartOfNextWord(this.pos));
			this.UpdateScrollOffset();
		}
		public void MoveToEndOfPreviousWord()
		{
			this.ClearCursorPos();
			if (this.pos != this.selectPos)
			{
				this.MoveLeft();
				return;
			}
			this.pos = (this.selectPos = this.FindEndOfPreviousWord(this.pos));
			this.UpdateScrollOffset();
		}
		public void SelectToStartOfNextWord()
		{
			this.ClearCursorPos();
			this.pos = this.FindStartOfNextWord(this.pos);
			this.UpdateScrollOffset();
		}
		public void SelectToEndOfPreviousWord()
		{
			this.ClearCursorPos();
			this.pos = this.FindEndOfPreviousWord(this.pos);
			this.UpdateScrollOffset();
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
			int length = this.content.text.Length;
			if (p == length)
			{
				return p;
			}
			char c = this.content.text[p];
			TextEditor.CharacterType characterType = this.ClassifyChar(c);
			if (characterType != TextEditor.CharacterType.WhiteSpace)
			{
				p++;
				while (p < length && this.ClassifyChar(this.content.text[p]) == characterType)
				{
					p++;
				}
			}
			else
			{
				if (c == '\t' || c == '\n')
				{
					return p + 1;
				}
			}
			if (p == length)
			{
				return p;
			}
			c = this.content.text[p];
			if (c == ' ')
			{
				while (p < length && char.IsWhiteSpace(this.content.text[p]))
				{
					p++;
				}
			}
			else
			{
				if (c == '\t' || c == '\n')
				{
					return p;
				}
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
			while (p > 0 && this.content.text[p] == ' ')
			{
				p--;
			}
			TextEditor.CharacterType characterType = this.ClassifyChar(this.content.text[p]);
			if (characterType != TextEditor.CharacterType.WhiteSpace)
			{
				while (p > 0 && this.ClassifyChar(this.content.text[p - 1]) == characterType)
				{
					p--;
				}
			}
			return p;
		}
		public void MoveWordLeft()
		{
			this.pos = ((this.pos >= this.selectPos) ? this.selectPos : this.pos);
			this.pos = this.FindPrevSeperator(this.pos);
			this.selectPos = this.pos;
			this.UpdateScrollOffset();
		}
		public void SelectWordRight()
		{
			this.ClearCursorPos();
			int num = this.selectPos;
			if (this.pos < this.selectPos)
			{
				this.selectPos = this.pos;
				this.MoveWordRight();
				this.selectPos = num;
				this.pos = ((this.pos >= this.selectPos) ? this.selectPos : this.pos);
				return;
			}
			this.selectPos = this.pos;
			this.MoveWordRight();
			this.selectPos = num;
			this.UpdateScrollOffset();
		}
		public void SelectWordLeft()
		{
			this.ClearCursorPos();
			int num = this.selectPos;
			if (this.pos > this.selectPos)
			{
				this.selectPos = this.pos;
				this.MoveWordLeft();
				this.selectPos = num;
				this.pos = ((this.pos <= this.selectPos) ? this.selectPos : this.pos);
				return;
			}
			this.selectPos = this.pos;
			this.MoveWordLeft();
			this.selectPos = num;
			this.UpdateScrollOffset();
		}
		public void ExpandSelectGraphicalLineStart()
		{
			this.ClearCursorPos();
			if (this.pos < this.selectPos)
			{
				this.pos = this.GetGraphicalLineStart(this.pos);
			}
			else
			{
				int num = this.pos;
				this.pos = this.GetGraphicalLineStart(this.selectPos);
				this.selectPos = num;
			}
			this.UpdateScrollOffset();
		}
		public void ExpandSelectGraphicalLineEnd()
		{
			this.ClearCursorPos();
			if (this.pos > this.selectPos)
			{
				this.pos = this.GetGraphicalLineEnd(this.pos);
			}
			else
			{
				int num = this.pos;
				this.pos = this.GetGraphicalLineEnd(this.selectPos);
				this.selectPos = num;
			}
			this.UpdateScrollOffset();
		}
		public void SelectGraphicalLineStart()
		{
			this.ClearCursorPos();
			this.pos = this.GetGraphicalLineStart(this.pos);
			this.UpdateScrollOffset();
		}
		public void SelectGraphicalLineEnd()
		{
			this.ClearCursorPos();
			this.pos = this.GetGraphicalLineEnd(this.pos);
			this.UpdateScrollOffset();
		}
		public void SelectParagraphForward()
		{
			this.ClearCursorPos();
			bool flag = this.pos < this.selectPos;
			if (this.pos < this.content.text.Length)
			{
				this.pos = this.content.text.IndexOf('\n', this.pos + 1);
				if (this.pos == -1)
				{
					this.pos = this.content.text.Length;
				}
				if (flag && this.pos > this.selectPos)
				{
					this.pos = this.selectPos;
				}
			}
			this.UpdateScrollOffset();
		}
		public void SelectParagraphBackward()
		{
			this.ClearCursorPos();
			bool flag = this.pos > this.selectPos;
			if (this.pos > 1)
			{
				this.pos = this.content.text.LastIndexOf('\n', this.pos - 2) + 1;
				if (flag && this.pos < this.selectPos)
				{
					this.pos = this.selectPos;
				}
			}
			else
			{
				this.selectPos = (this.pos = 0);
			}
			this.UpdateScrollOffset();
		}
		public void SelectCurrentWord()
		{
			this.ClearCursorPos();
			int length = this.content.text.Length;
			this.selectPos = this.pos;
			if (length == 0)
			{
				return;
			}
			if (this.pos >= length)
			{
				this.pos = length - 1;
			}
			if (this.selectPos >= length)
			{
				this.selectPos--;
			}
			if (this.pos < this.selectPos)
			{
				this.pos = this.FindEndOfClassification(this.pos, -1);
				this.selectPos = this.FindEndOfClassification(this.selectPos, 1);
			}
			else
			{
				this.pos = this.FindEndOfClassification(this.pos, 1);
				this.selectPos = this.FindEndOfClassification(this.selectPos, -1);
			}
			this.m_bJustSelected = true;
			this.UpdateScrollOffset();
		}
		private int FindEndOfClassification(int p, int dir)
		{
			int length = this.content.text.Length;
			if (p >= length || p < 0)
			{
				return p;
			}
			TextEditor.CharacterType characterType = this.ClassifyChar(this.content.text[p]);
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
				if (this.ClassifyChar(this.content.text[p]) != characterType)
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
			int length = this.content.text.Length;
			if (this.pos < length)
			{
				this.pos = this.content.text.IndexOf('\n', this.pos);
				if (this.pos == -1)
				{
					this.pos = this.content.text.Length;
				}
				else
				{
					this.pos++;
				}
			}
			if (this.selectPos != 0)
			{
				this.selectPos = this.content.text.LastIndexOf('\n', this.selectPos - 1) + 1;
			}
			this.UpdateScrollOffset();
		}
		public void UpdateScrollOffsetIfNeeded()
		{
			if (this.m_TextHeightPotentiallyChanged)
			{
				this.UpdateScrollOffset();
				this.m_TextHeightPotentiallyChanged = false;
			}
		}
		private void UpdateScrollOffset()
		{
			int cursorStringIndex = this.pos;
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.content, cursorStringIndex);
			Rect rect = this.style.padding.Remove(this.position);
			Vector2 vector = new Vector2(this.style.CalcSize(this.content).x, this.style.CalcHeight(this.content, this.position.width));
			if (vector.x < this.position.width)
			{
				this.scrollOffset.x = 0f;
			}
			else
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
			else
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
		}
		public void DrawCursor(string text)
		{
			string text2 = this.content.text;
			int num = this.pos;
			if (Input.compositionString.Length > 0)
			{
				this.content.text = text.Substring(0, this.pos) + Input.compositionString + text.Substring(this.selectPos);
				num += Input.compositionString.Length;
			}
			else
			{
				this.content.text = text;
			}
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.content, num);
			this.UpdateScrollOffset();
			Vector2 contentOffset = this.style.contentOffset;
			this.style.contentOffset -= this.scrollOffset;
			this.style.Internal_clipOffset = this.scrollOffset;
			Input.compositionCursorPos = this.graphicalCursorPos + new Vector2(this.position.x, this.position.y + this.style.lineHeight) - this.scrollOffset;
			if (Input.compositionString.Length > 0)
			{
				this.style.DrawWithTextSelection(this.position, this.content, this.controlID, this.pos, this.pos + Input.compositionString.Length, true);
			}
			else
			{
				this.style.DrawWithTextSelection(this.position, this.content, this.controlID, this.pos, this.selectPos);
			}
			if (this.m_iAltCursorPos != -1)
			{
				this.style.DrawCursor(this.position, this.content, this.controlID, this.m_iAltCursorPos);
			}
			this.style.contentOffset = contentOffset;
			this.style.Internal_clipOffset = Vector2.zero;
			this.content.text = text2;
		}
		private bool PerformOperation(TextEditor.TextEditOp operation)
		{
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
			this.oldText = this.content.text;
			this.oldPos = this.pos;
			this.oldSelectPos = this.selectPos;
		}
		public void Undo()
		{
			this.content.text = this.oldText;
			this.pos = this.oldPos;
			this.selectPos = this.oldSelectPos;
			this.UpdateScrollOffset();
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
			if (this.selectPos == this.pos)
			{
				return;
			}
			if (this.isPasswordField)
			{
				return;
			}
			string systemCopyBuffer;
			if (this.pos < this.selectPos)
			{
				systemCopyBuffer = this.content.text.Substring(this.pos, this.selectPos - this.pos);
			}
			else
			{
				systemCopyBuffer = this.content.text.Substring(this.selectPos, this.pos - this.selectPos);
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
			if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.OSXEditor)
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
		public void ClampPos()
		{
			if (this.m_HasFocus && this.controlID != GUIUtility.keyboardControl)
			{
				this.OnLostFocus();
			}
			if (!this.m_HasFocus && this.controlID == GUIUtility.keyboardControl)
			{
				this.OnFocus();
			}
			if (this.pos < 0)
			{
				this.pos = 0;
			}
			else
			{
				if (this.pos > this.content.text.Length)
				{
					this.pos = this.content.text.Length;
				}
			}
			if (this.selectPos < 0)
			{
				this.selectPos = 0;
			}
			else
			{
				if (this.selectPos > this.content.text.Length)
				{
					this.selectPos = this.content.text.Length;
				}
			}
			if (this.m_iAltCursorPos > this.content.text.Length)
			{
				this.m_iAltCursorPos = this.content.text.Length;
			}
		}
	}
}
