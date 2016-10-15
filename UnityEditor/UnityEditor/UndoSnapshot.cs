using System;
using UnityEngine;

namespace UnityEditor
{
	[Obsolete("Use Undo.RecordObject before modifying the object instead")]
	public sealed class UndoSnapshot
	{
		public UndoSnapshot(UnityEngine.Object[] objectsToUndo)
		{
		}

		public void Restore()
		{
		}

		public void Dispose()
		{
		}
	}
}
