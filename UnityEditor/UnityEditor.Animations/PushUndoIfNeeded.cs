using System;
using UnityEngine;
namespace UnityEditor.Animations
{
	internal struct PushUndoIfNeeded
	{
		internal bool m_PushUndo;
		internal PushUndoIfNeeded(bool pushUndo)
		{
			this.m_PushUndo = pushUndo;
		}
		internal void DoUndo(UnityEngine.Object target, string undoOperation)
		{
			if (this.m_PushUndo)
			{
				Undo.RegisterCompleteObjectUndo(target, undoOperation);
			}
		}
	}
}
