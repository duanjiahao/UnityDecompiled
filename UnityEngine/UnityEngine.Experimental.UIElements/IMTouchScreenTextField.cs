using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMTouchScreenTextField : IMTextField
	{
		private static int s_HotTextField = -1;

		private string m_SecureText;

		private char m_MaskChar;

		public string secureText
		{
			get
			{
				return this.m_SecureText;
			}
			set
			{
				string text = value ?? string.Empty;
				if (text != this.m_SecureText)
				{
					this.m_SecureText = text;
				}
			}
		}

		public char maskChar
		{
			get
			{
				return this.m_MaskChar;
			}
			set
			{
				if (this.m_MaskChar != value)
				{
					this.m_MaskChar = value;
				}
			}
		}

		public IMTouchScreenTextField()
		{
			this.secureText = string.Empty;
			this.maskChar = '\0';
		}

		public override bool OnGUI(Event evt)
		{
			base.SyncTextEditor();
			bool flag = false;
			EventType type = evt.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.Repaint)
				{
					this.DoRepaint(new StylePainter(evt.mousePosition));
				}
			}
			else
			{
				flag = this.DoMouseDown(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
			}
			if (flag)
			{
				evt.Use();
			}
			base.editor.UpdateScrollOffsetIfNeeded(evt);
			return flag;
		}

		protected override int DoGenerateControlID()
		{
			return GUIUtility.GetControlID("TouchScreenTextField".GetHashCode(), base.focusType, base.position);
		}

		internal override void DoRepaint(IStylePainter args)
		{
			if (base.editor.keyboardOnScreen != null)
			{
				base.text = base.editor.keyboardOnScreen.text;
				if (base.maxLength >= 0 && base.text != null && base.text.Length > base.maxLength)
				{
					base.text = base.text.Substring(0, base.maxLength);
				}
				if (base.editor.keyboardOnScreen.done)
				{
					base.editor.keyboardOnScreen = null;
					GUI.changed = true;
				}
			}
			string text = base.text;
			if (!string.IsNullOrEmpty(this.secureText))
			{
				base.text = GUI.PasswordFieldGetStrToShow(text, this.maskChar);
			}
			base.style.Draw(base.position, GUIContent.Temp(base.text), base.id, false);
			base.text = text;
		}

		protected override bool DoMouseDown(MouseEventArgs args)
		{
			bool result;
			if (base.position.Contains(args.mousePosition))
			{
				GUIUtility.hotControl = base.id;
				if (IMTouchScreenTextField.s_HotTextField != -1 && IMTouchScreenTextField.s_HotTextField != base.id)
				{
					UnityEngine.TextEditor textEditor = (UnityEngine.TextEditor)GUIUtility.GetStateObject(typeof(UnityEngine.TextEditor), IMTouchScreenTextField.s_HotTextField);
					textEditor.keyboardOnScreen = null;
				}
				IMTouchScreenTextField.s_HotTextField = base.id;
				if (GUIUtility.keyboardControl != base.id)
				{
					GUIUtility.keyboardControl = base.id;
				}
				base.editor.keyboardOnScreen = TouchScreenKeyboard.Open(string.IsNullOrEmpty(this.secureText) ? base.text : this.secureText, TouchScreenKeyboardType.Default, true, base.multiline, !string.IsNullOrEmpty(this.secureText));
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
