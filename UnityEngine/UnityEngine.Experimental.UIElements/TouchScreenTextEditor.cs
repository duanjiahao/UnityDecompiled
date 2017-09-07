using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class TouchScreenTextEditor : TextEditor
	{
		private string m_SecureText;

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

		public TouchScreenTextEditor(TextField textField) : base(textField)
		{
			this.secureText = string.Empty;
		}

		public override EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			base.SyncTextEditor();
			EventPropagation result = EventPropagation.Continue;
			EventType type = evt.type;
			if (type == EventType.MouseDown)
			{
				result = this.DoMouseDown();
			}
			base.UpdateScrollOffset();
			return result;
		}

		private EventPropagation DoMouseDown()
		{
			base.textField.TakeCapture();
			this.keyboardOnScreen = TouchScreenKeyboard.Open(string.IsNullOrEmpty(this.secureText) ? base.textField.text : this.secureText, TouchScreenKeyboardType.Default, true, this.multiline, !string.IsNullOrEmpty(this.secureText));
			return EventPropagation.Stop;
		}
	}
}
