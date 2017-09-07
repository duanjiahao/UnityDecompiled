using System;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	public class TextField : VisualElement
	{
		private bool m_Multiline;

		private bool m_IsPasswordField;

		internal const int kMaxLengthNone = -1;

		private GUIStyle m_DrawGUIStyle;

		public bool multiline
		{
			get
			{
				return this.m_Multiline;
			}
			set
			{
				this.m_Multiline = value;
				if (!value)
				{
					base.text = base.text.Replace("\n", "");
				}
			}
		}

		public bool isPasswordField
		{
			get
			{
				return this.m_IsPasswordField;
			}
			set
			{
				this.m_IsPasswordField = value;
				if (value)
				{
					this.multiline = false;
				}
			}
		}

		public char maskChar
		{
			get;
			set;
		}

		public bool doubleClickSelectsWord
		{
			get;
			set;
		}

		public bool tripleClickSelectsLine
		{
			get;
			set;
		}

		public int maxLength
		{
			get;
			set;
		}

		private bool touchScreenTextField
		{
			get
			{
				return TouchScreenKeyboard.isSupported;
			}
		}

		internal GUIStyle style
		{
			get
			{
				GUIStyle arg_1C_0;
				if ((arg_1C_0 = this.m_DrawGUIStyle) == null)
				{
					arg_1C_0 = (this.m_DrawGUIStyle = new GUIStyle());
				}
				return arg_1C_0;
			}
		}

		public bool hasFocus
		{
			get
			{
				return base.elementPanel != null && base.elementPanel.focusedElement == this;
			}
		}

		public TextEditor editor
		{
			get;
			protected set;
		}

		public TextField() : this(-1, false, false, '\0')
		{
		}

		public TextField(int maxLength, bool multiline, bool isPasswordField, char maskChar)
		{
			this.maxLength = maxLength;
			this.multiline = multiline;
			this.isPasswordField = isPasswordField;
			this.maskChar = maskChar;
			if (this.touchScreenTextField)
			{
				this.editor = new TouchScreenTextEditor(this);
			}
			else
			{
				this.doubleClickSelectsWord = true;
				this.tripleClickSelectsLine = true;
				this.editor = new KeyboardTextEditor(this);
			}
			base.AddManipulator(this.editor);
		}

		public override void OnStylesResolved(ICustomStyles styles)
		{
			base.OnStylesResolved(styles);
			this.m_Styles.WriteToGUIStyle(this.style);
		}

		internal override void DoRepaint(IStylePainter painter)
		{
			if (this.touchScreenTextField)
			{
				TouchScreenTextEditor touchScreenTextEditor = this.editor as TouchScreenTextEditor;
				if (touchScreenTextEditor != null && touchScreenTextEditor.keyboardOnScreen != null)
				{
					base.text = touchScreenTextEditor.keyboardOnScreen.text;
					if (this.editor.maxLength >= 0 && base.text != null && base.text.Length > this.editor.maxLength)
					{
						base.text = base.text.Substring(0, this.editor.maxLength);
					}
					if (touchScreenTextEditor.keyboardOnScreen.done)
					{
						touchScreenTextEditor.keyboardOnScreen = null;
						GUI.changed = true;
					}
				}
				string t = base.text;
				if (touchScreenTextEditor != null && !string.IsNullOrEmpty(touchScreenTextEditor.secureText))
				{
					t = "".PadRight(touchScreenTextEditor.secureText.Length, this.maskChar);
				}
				this.style.Draw(base.position, GUIContent.Temp(t), 0, false);
			}
			else if (this.isPasswordField)
			{
				string text = "".PadRight(base.text.Length, this.maskChar);
				if (!this.hasFocus)
				{
					this.style.Draw(base.position, GUIContent.Temp(text), 0, false);
				}
				else
				{
					this.DrawCursor(text);
				}
			}
			else if (!this.hasFocus)
			{
				this.style.Draw(base.position, GUIContent.Temp(base.text), 0, false);
			}
			else
			{
				this.DrawCursor(base.text);
			}
		}

		private void DrawCursor(string newText)
		{
			KeyboardTextEditor keyboardTextEditor = this.editor as KeyboardTextEditor;
			if (keyboardTextEditor != null)
			{
				keyboardTextEditor.PreDrawCursor(newText);
				int cursorIndex = keyboardTextEditor.cursorIndex;
				int selectIndex = keyboardTextEditor.selectIndex;
				Rect localPosition = keyboardTextEditor.localPosition;
				Vector2 scrollOffset = keyboardTextEditor.scrollOffset;
				Vector2 contentOffset = this.style.contentOffset;
				this.style.contentOffset -= scrollOffset;
				this.style.Internal_clipOffset = scrollOffset;
				Input.compositionCursorPos = keyboardTextEditor.graphicalCursorPos - scrollOffset + new Vector2(localPosition.x, localPosition.y + this.style.lineHeight);
				GUIContent content = new GUIContent(keyboardTextEditor.text);
				if (!string.IsNullOrEmpty(Input.compositionString))
				{
					this.style.DrawWithTextSelection(base.position, content, this.HasCapture(), this.hasFocus, cursorIndex, cursorIndex + Input.compositionString.Length, true);
				}
				else
				{
					this.style.DrawWithTextSelection(base.position, content, this.HasCapture(), this.hasFocus, cursorIndex, selectIndex, false);
				}
				if (keyboardTextEditor.altCursorPosition != -1)
				{
					this.style.DrawCursor(base.position, content, 0, keyboardTextEditor.altCursorPosition);
				}
				this.style.contentOffset = contentOffset;
				this.style.Internal_clipOffset = Vector2.zero;
				keyboardTextEditor.PostDrawCursor();
			}
		}
	}
}
