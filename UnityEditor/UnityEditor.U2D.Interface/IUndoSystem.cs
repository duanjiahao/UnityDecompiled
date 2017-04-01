using System;

namespace UnityEditor.U2D.Interface
{
	internal interface IUndoSystem
	{
		void RegisterUndoCallback(Undo.UndoRedoCallback undoCallback);

		void UnregisterUndoCallback(Undo.UndoRedoCallback undoCallback);

		void RegisterCompleteObjectUndo(IUndoableObject obj, string undoText);

		void ClearUndo(IUndoableObject obj);
	}
}
