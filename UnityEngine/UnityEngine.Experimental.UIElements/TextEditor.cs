using System;

namespace UnityEngine.Experimental.UIElements
{
	public class TextEditor : UnityEngine.TextEditor, IManipulator, IEventHandler
	{
		public int maxLength
		{
			get;
			set;
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

		protected TextField textField
		{
			get;
			set;
		}

		internal override Rect localPosition
		{
			get
			{
				return new Rect(0f, 0f, base.position.width, base.position.height);
			}
		}

		public VisualElement target
		{
			get;
			set;
		}

		public EventPhase phaseInterest
		{
			get;
			set;
		}

		public IPanel panel
		{
			get
			{
				IPanel result;
				if (this.target != null)
				{
					result = this.target.panel;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		protected TextEditor(TextField textField)
		{
			this.phaseInterest = EventPhase.BubbleUp;
			this.textField = textField;
			this.SyncTextEditor();
		}

		public virtual EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			return EventPropagation.Continue;
		}

		public virtual void OnLostCapture()
		{
		}

		public virtual void OnLostKeyboardFocus()
		{
		}

		protected void SyncTextEditor()
		{
			string text = this.textField.text;
			if (this.maxLength >= 0 && text != null && text.Length > this.maxLength)
			{
				text = text.Substring(0, this.maxLength);
			}
			base.text = text;
			base.SaveBackup();
			this.style = this.textField.style;
			base.position = this.textField.position;
			this.maxLength = this.textField.maxLength;
			this.multiline = this.textField.multiline;
			this.isPasswordField = this.textField.isPasswordField;
			this.maskChar = this.textField.maskChar;
			this.doubleClickSelectsWord = this.textField.doubleClickSelectsWord;
			this.tripleClickSelectsLine = this.textField.tripleClickSelectsLine;
			base.DetectFocusChange();
		}

		internal override void OnDetectFocusChange()
		{
			if (this.m_HasFocus && !this.textField.hasFocus)
			{
				base.OnFocus();
			}
			if (!this.m_HasFocus && this.textField.hasFocus)
			{
				base.OnLostFocus();
			}
		}
	}
}
