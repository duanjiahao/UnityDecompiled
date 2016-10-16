using System;
using UnityEngine;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class ClipboardAccess
	{
		private ClipboardAccess()
		{
		}

		static ClipboardAccess()
		{
			JSProxyMgr.GetInstance().AddGlobalObject("unity/ClipboardAccess", new ClipboardAccess());
		}

		public void CopyToClipboard(string value)
		{
			TextEditor textEditor = new TextEditor();
			textEditor.text = value;
			textEditor.SelectAll();
			textEditor.Copy();
		}

		public string PasteFromClipboard()
		{
			TextEditor textEditor = new TextEditor();
			textEditor.Paste();
			return textEditor.text;
		}
	}
}
