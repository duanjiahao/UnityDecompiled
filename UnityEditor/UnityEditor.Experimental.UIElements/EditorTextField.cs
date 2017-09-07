using System;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.Experimental.UIElements
{
	internal class EditorTextField : TextField
	{
		protected EditorTextField(int maxLength, bool multiline, bool isPasswordField, char maskChar) : base(maxLength, multiline, isPasswordField, maskChar)
		{
			ContextualMenu contextualMenu = new ContextualMenu();
			contextualMenu.AddAction("Cut", new GenericMenu.MenuFunction(this.Cut), new ContextualMenu.ActionStatusCallback(this.CutCopyActionStatus));
			contextualMenu.AddAction("Copy", new GenericMenu.MenuFunction(this.Copy), new ContextualMenu.ActionStatusCallback(this.CutCopyActionStatus));
			contextualMenu.AddAction("Paste", new GenericMenu.MenuFunction(this.Paste), new ContextualMenu.ActionStatusCallback(this.PasteActionStatus));
			base.AddManipulator(contextualMenu);
		}

		private ContextualMenu.ActionStatus CutCopyActionStatus()
		{
			return (!base.editor.hasSelection || base.isPasswordField) ? ContextualMenu.ActionStatus.Disabled : ContextualMenu.ActionStatus.Enabled;
		}

		private ContextualMenu.ActionStatus PasteActionStatus()
		{
			return (!base.editor.CanPaste()) ? ContextualMenu.ActionStatus.Off : ContextualMenu.ActionStatus.Enabled;
		}

		private void Cut()
		{
			base.editor.Cut();
		}

		private void Copy()
		{
			base.editor.Copy();
		}

		private void Paste()
		{
			base.editor.Paste();
		}
	}
}
