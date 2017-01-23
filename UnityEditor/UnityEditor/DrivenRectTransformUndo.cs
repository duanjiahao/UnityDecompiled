using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[InitializeOnLoad]
	internal class DrivenRectTransformUndo
	{
		[CompilerGenerated]
		private static Undo.WillFlushUndoRecord <>f__mg$cache0;

		[CompilerGenerated]
		private static Undo.UndoRedoCallback <>f__mg$cache1;

		static DrivenRectTransformUndo()
		{
			Delegate arg_23_0 = Undo.willFlushUndoRecord;
			if (DrivenRectTransformUndo.<>f__mg$cache0 == null)
			{
				DrivenRectTransformUndo.<>f__mg$cache0 = new Undo.WillFlushUndoRecord(DrivenRectTransformUndo.ForceUpdateCanvases);
			}
			Undo.willFlushUndoRecord = (Undo.WillFlushUndoRecord)Delegate.Combine(arg_23_0, DrivenRectTransformUndo.<>f__mg$cache0);
			Delegate arg_54_0 = Undo.undoRedoPerformed;
			if (DrivenRectTransformUndo.<>f__mg$cache1 == null)
			{
				DrivenRectTransformUndo.<>f__mg$cache1 = new Undo.UndoRedoCallback(DrivenRectTransformUndo.ForceUpdateCanvases);
			}
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(arg_54_0, DrivenRectTransformUndo.<>f__mg$cache1);
		}

		private static void ForceUpdateCanvases()
		{
			Canvas.ForceUpdateCanvases();
		}
	}
}
