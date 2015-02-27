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
		}
		private static void ForceUpdateCanvases()
		{
			Canvas.ForceUpdateCanvases();
		}
	}
}
