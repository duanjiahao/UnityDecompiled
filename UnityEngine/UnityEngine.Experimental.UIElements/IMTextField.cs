using System;

namespace UnityEngine.Experimental.UIElements
{
	internal abstract class IMTextField : IMElement
	{
		public UnityEngine.TextEditor editor
		{
			get;
			protected set;
		}

		public int maxLength
		{
			get;
			set;
		}

		public bool multiline
		{
			get;
			set;
		}

		protected override int DoGenerateControlID()
		{
			return GUIUtility.GetControlID("IMTextField".GetHashCode(), base.focusType, base.position);
		}

		protected void SyncTextEditor()
		{
			if (this.maxLength >= 0 && base.text != null && base.text.Length > this.maxLength)
			{
				base.text = base.text.Substring(0, this.maxLength);
			}
			this.editor = (UnityEngine.TextEditor)GUIUtility.GetStateObject(typeof(UnityEngine.TextEditor), base.id);
			this.editor.text = base.text;
			this.editor.SaveBackup();
			this.editor.position = base.position;
			this.editor.style = base.style;
			this.editor.multiline = this.multiline;
			this.editor.controlID = base.id;
			this.editor.DetectFocusChange();
		}
	}
}
