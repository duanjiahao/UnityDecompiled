using System;
using UnityEngine;

namespace UnityEditor.U2D.Interface
{
	internal class UndoSystem : IUndoSystem
	{
		public void RegisterUndoCallback(Undo.UndoRedoCallback undoCallback)
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, undoCallback);
		}

		public void UnregisterUndoCallback(Undo.UndoRedoCallback undoCallback)
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, undoCallback);
		}

		public void RegisterCompleteObjectUndo(IUndoableObject obj, string undoText)
		{
			ScriptableObject scriptableObject = this.CheckUndoObjectType(obj);
			if (scriptableObject != null)
			{
				Undo.RegisterCompleteObjectUndo(scriptableObject, undoText);
			}
		}

		private ScriptableObject CheckUndoObjectType(IUndoableObject obj)
		{
			ScriptableObject scriptableObject = obj as ScriptableObject;
			if (scriptableObject == null)
			{
				Debug.LogError("Register Undo object is not a ScriptableObject");
			}
			return scriptableObject;
		}

		public void ClearUndo(IUndoableObject obj)
		{
			ScriptableObject scriptableObject = this.CheckUndoObjectType(obj);
			if (scriptableObject != null)
			{
				Undo.ClearUndo(scriptableObject);
			}
		}
	}
}
