using System;
using UnityEngine;

namespace UnityEditor
{
	[InitializeOnLoad]
	internal class DrivenRectTransformUndo
	{
		static DrivenRectTransformUndo()
		{
			Undo.willFlushUndoRecord = (Undo.WillFlushUndoRecord)Delegate.Combine(Undo.willFlushUndoRecord, new Undo.WillFlushUndoRecord(DrivenRectTransformUndo.ForceUpdateCanvases));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(DrivenRectTransformUndo.ForceUpdateCanvases));
		}

		private static void ForceUpdateCanvases()
		{
			Canvas.ForceUpdateCanvases();
		}
	}
}
