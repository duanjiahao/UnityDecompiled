using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class KeyboardTextEditor : TextEditor
	{
		internal bool m_Changed;

		private bool m_Dragged;

		private bool m_DragToPosition = true;

		private bool m_PostPoneMove;

		private bool m_SelectAllOnMouseUp = true;

		private string m_PreDrawCursorText;

		public KeyboardTextEditor(TextField textField) : base(textField)
		{
		}

		public override EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			base.SyncTextEditor();
			this.m_Changed = false;
			EventPropagation result = EventPropagation.Continue;
			EventType type = evt.type;
			switch (type)
			{
			case EventType.MouseDown:
				result = this.DoMouseDown(evt);
				goto IL_94;
			case EventType.MouseUp:
				result = this.DoMouseUp(evt);
				goto IL_94;
			case EventType.MouseMove:
				IL_31:
				if (type == EventType.ValidateCommand)
				{
					result = this.DoValidateCommand(evt);
					goto IL_94;
				}
				if (type != EventType.ExecuteCommand)
				{
					goto IL_94;
				}
				result = this.DoExecuteCommand(evt);
				goto IL_94;
			case EventType.MouseDrag:
				result = this.DoMouseDrag(evt);
				goto IL_94;
			case EventType.KeyDown:
				result = this.DoKeyDown(evt);
				goto IL_94;
			}
			goto IL_31;
			IL_94:
			if (this.m_Changed)
			{
				if (base.maxLength >= 0 && base.text != null && base.text.Length > base.maxLength)
				{
					base.text = base.text.Substring(0, base.maxLength);
				}
				base.textField.text = base.text;
				result = EventPropagation.Stop;
			}
			base.UpdateScrollOffset();
			return result;
		}

		private EventPropagation DoMouseDown(Event evt)
		{
			this.TakeCapture();
			EventPropagation result;
			if (!this.m_HasFocus)
			{
				this.m_HasFocus = true;
				base.MoveCursorToPosition_Internal(evt.mousePosition, evt.shift);
				result = EventPropagation.Stop;
			}
			else
			{
				if (evt.clickCount == 2 && base.doubleClickSelectsWord)
				{
					base.SelectCurrentWord();
					base.DblClickSnap(UnityEngine.TextEditor.DblClickSnapping.WORDS);
					base.MouseDragSelectsWholeWords(true);
					this.m_DragToPosition = false;
				}
				else if (evt.clickCount == 3 && base.tripleClickSelectsLine)
				{
					base.SelectCurrentParagraph();
					base.MouseDragSelectsWholeWords(true);
					base.DblClickSnap(UnityEngine.TextEditor.DblClickSnapping.PARAGRAPHS);
					this.m_DragToPosition = false;
				}
				else
				{
					base.MoveCursorToPosition_Internal(evt.mousePosition, evt.shift);
					this.m_SelectAllOnMouseUp = false;
				}
				result = EventPropagation.Stop;
			}
			return result;
		}

		private EventPropagation DoMouseUp(Event evt)
		{
			EventPropagation result;
			if (!this.HasCapture())
			{
				result = EventPropagation.Continue;
			}
			else
			{
				if (this.m_Dragged && this.m_DragToPosition)
				{
					base.MoveSelectionToAltCursor();
				}
				else if (this.m_PostPoneMove)
				{
					base.MoveCursorToPosition_Internal(evt.mousePosition, evt.shift);
				}
				else if (this.m_SelectAllOnMouseUp)
				{
					this.m_SelectAllOnMouseUp = false;
				}
				base.MouseDragSelectsWholeWords(false);
				this.ReleaseCapture();
				this.m_DragToPosition = true;
				this.m_Dragged = false;
				this.m_PostPoneMove = false;
				result = EventPropagation.Stop;
			}
			return result;
		}

		private EventPropagation DoMouseDrag(Event evt)
		{
			EventPropagation result;
			if (!this.HasCapture())
			{
				result = EventPropagation.Continue;
			}
			else
			{
				if (!evt.shift && base.hasSelection && this.m_DragToPosition)
				{
					base.MoveAltCursorToPosition(Event.current.mousePosition);
				}
				else
				{
					if (evt.shift)
					{
						base.MoveCursorToPosition_Internal(evt.mousePosition, evt.shift);
					}
					else
					{
						base.SelectToPosition(evt.mousePosition);
					}
					this.m_DragToPosition = false;
					this.m_SelectAllOnMouseUp = !base.hasSelection;
				}
				this.m_Dragged = true;
				result = EventPropagation.Stop;
			}
			return result;
		}

		private EventPropagation DoKeyDown(Event evt)
		{
			EventPropagation result;
			if (!base.textField.hasFocus)
			{
				result = EventPropagation.Continue;
			}
			else if (base.HandleKeyEvent(evt))
			{
				this.m_Changed = true;
				base.textField.text = base.text;
				result = EventPropagation.Stop;
			}
			else if (evt.keyCode == KeyCode.Tab || evt.character == '\t')
			{
				result = EventPropagation.Continue;
			}
			else
			{
				char character = evt.character;
				if (character == '\n' && !this.multiline && !evt.alt)
				{
					result = EventPropagation.Continue;
				}
				else if ((base.textField.font != null && base.textField.font.HasCharacter(character)) || character == '\n')
				{
					base.Insert(character);
					this.m_Changed = true;
					result = EventPropagation.Continue;
				}
				else if (character == '\0')
				{
					if (!string.IsNullOrEmpty(Input.compositionString))
					{
						base.ReplaceSelection("");
						this.m_Changed = true;
					}
					result = EventPropagation.Stop;
				}
				else
				{
					result = EventPropagation.Continue;
				}
			}
			return result;
		}

		private EventPropagation DoValidateCommand(Event evt)
		{
			EventPropagation result;
			if (!base.textField.hasFocus)
			{
				result = EventPropagation.Continue;
			}
			else
			{
				string commandName = evt.commandName;
				if (commandName != null)
				{
					if (!(commandName == "Cut") && !(commandName == "Copy"))
					{
						if (!(commandName == "Paste"))
						{
							if (!(commandName == "SelectAll") && !(commandName == "Delete"))
							{
								if (!(commandName == "UndoRedoPerformed"))
								{
								}
							}
						}
						else if (!base.CanPaste())
						{
							result = EventPropagation.Continue;
							return result;
						}
					}
					else if (!base.hasSelection)
					{
						result = EventPropagation.Continue;
						return result;
					}
				}
				result = EventPropagation.Stop;
			}
			return result;
		}

		private EventPropagation DoExecuteCommand(Event evt)
		{
			bool flag = false;
			string text = base.text;
			EventPropagation result;
			if (!base.textField.hasFocus)
			{
				result = EventPropagation.Continue;
			}
			else
			{
				string commandName = evt.commandName;
				if (commandName != null)
				{
					if (commandName == "OnLostFocus")
					{
						result = EventPropagation.Stop;
						return result;
					}
					if (!(commandName == "Cut"))
					{
						if (commandName == "Copy")
						{
							base.Copy();
							result = EventPropagation.Stop;
							return result;
						}
						if (!(commandName == "Paste"))
						{
							if (commandName == "SelectAll")
							{
								base.SelectAll();
								result = EventPropagation.Stop;
								return result;
							}
							if (commandName == "Delete")
							{
								if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
								{
									base.Delete();
								}
								else
								{
									base.Cut();
								}
								flag = true;
							}
						}
						else
						{
							base.Paste();
							flag = true;
						}
					}
					else
					{
						base.Cut();
						flag = true;
					}
				}
				if (flag)
				{
					if (text != base.text)
					{
						this.m_Changed = true;
					}
					result = EventPropagation.Stop;
				}
				else
				{
					result = EventPropagation.Continue;
				}
			}
			return result;
		}

		public void PreDrawCursor(string newText)
		{
			base.SyncTextEditor();
			this.m_PreDrawCursorText = base.text;
			int num = base.cursorIndex;
			if (!string.IsNullOrEmpty(Input.compositionString))
			{
				base.text = newText.Substring(0, base.cursorIndex) + Input.compositionString + newText.Substring(base.selectIndex);
				num += Input.compositionString.Length;
			}
			else
			{
				base.text = newText;
			}
			if (base.maxLength >= 0 && base.text != null && base.text.Length > base.maxLength)
			{
				base.text = base.text.Substring(0, base.maxLength);
				num = Math.Min(num, base.maxLength - 1);
			}
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(this.localPosition, new GUIContent(base.text), num);
		}

		public void PostDrawCursor()
		{
			base.text = this.m_PreDrawCursorText;
		}
	}
}
