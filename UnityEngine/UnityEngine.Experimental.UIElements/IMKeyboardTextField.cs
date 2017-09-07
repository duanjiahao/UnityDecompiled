using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMKeyboardTextField : IMTextField
	{
		private bool m_Changed;

		public override bool OnGUI(Event evt)
		{
			base.SyncTextEditor();
			this.m_Changed = false;
			bool flag = false;
			switch (evt.type)
			{
			case EventType.MouseDown:
				flag = this.DoMouseDown(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				break;
			case EventType.MouseUp:
				flag = this.DoMouseUp(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				break;
			case EventType.MouseDrag:
				flag = this.DoMouseDrag(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				break;
			case EventType.KeyDown:
				flag = this.DoKeyDown(new KeyboardEventArgs(evt.character, evt.keyCode, evt.modifiers));
				break;
			case EventType.Repaint:
				this.DoRepaint(new StylePainter(evt.mousePosition));
				break;
			}
			if (flag)
			{
				evt.Use();
			}
			if (GUIUtility.keyboardControl == base.id)
			{
				GUIUtility.textFieldInput = true;
			}
			if (this.m_Changed)
			{
				GUI.changed = true;
				base.text = base.editor.text;
				if (base.maxLength >= 0 && base.text.Length > base.maxLength)
				{
					base.text = base.text.Substring(0, base.maxLength);
				}
				evt.Use();
			}
			base.editor.UpdateScrollOffsetIfNeeded(evt);
			return flag;
		}

		internal override void DoRepaint(IStylePainter args)
		{
			if (GUIUtility.keyboardControl != base.id)
			{
				base.style.Draw(base.position, GUIContent.Temp(base.text), base.id, false);
			}
			else
			{
				base.editor.DrawCursor(base.text);
			}
		}

		protected override bool DoMouseDown(MouseEventArgs args)
		{
			bool result;
			if (base.position.Contains(args.mousePosition))
			{
				GUIUtility.hotControl = base.id;
				GUIUtility.keyboardControl = base.id;
				base.editor.m_HasFocus = true;
				base.editor.MoveCursorToPosition(args.mousePosition);
				if (args.clickCount == 2 && GUI.skin.settings.doubleClickSelectsWord)
				{
					base.editor.SelectCurrentWord();
					base.editor.DblClickSnap(UnityEngine.TextEditor.DblClickSnapping.WORDS);
					base.editor.MouseDragSelectsWholeWords(true);
				}
				else if (args.clickCount == 3 && GUI.skin.settings.tripleClickSelectsLine)
				{
					base.editor.SelectCurrentParagraph();
					base.editor.MouseDragSelectsWholeWords(true);
					base.editor.DblClickSnap(UnityEngine.TextEditor.DblClickSnapping.PARAGRAPHS);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected override bool DoMouseUp(MouseEventArgs args)
		{
			bool result;
			if (GUIUtility.hotControl == base.id)
			{
				base.editor.MouseDragSelectsWholeWords(false);
				GUIUtility.hotControl = 0;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected override bool DoMouseDrag(MouseEventArgs args)
		{
			bool result;
			if (GUIUtility.hotControl == base.id)
			{
				if (args.shift)
				{
					base.editor.MoveCursorToPosition(args.mousePosition);
				}
				else
				{
					base.editor.SelectToPosition(args.mousePosition);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected override bool DoKeyDown(KeyboardEventArgs args)
		{
			bool result;
			if (GUIUtility.keyboardControl != base.id)
			{
				result = false;
			}
			else if (base.editor.HandleKeyEvent(args.ToEvent()))
			{
				this.m_Changed = true;
				base.text = base.editor.text;
				result = true;
			}
			else if (args.keyCode == KeyCode.Tab || args.character == '\t')
			{
				result = false;
			}
			else
			{
				char character = args.character;
				if (character == '\n' && !base.multiline && !args.alt)
				{
					result = false;
				}
				else
				{
					Font font = base.style.font;
					if (font == null)
					{
						font = GUI.skin.font;
					}
					if (font.HasCharacter(character) || character == '\n')
					{
						base.editor.Insert(character);
						this.m_Changed = true;
						result = (character == '\n');
					}
					else if (character == '\0')
					{
						if (Input.compositionString.Length > 0)
						{
							base.editor.ReplaceSelection("");
							this.m_Changed = true;
						}
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}
	}
}
