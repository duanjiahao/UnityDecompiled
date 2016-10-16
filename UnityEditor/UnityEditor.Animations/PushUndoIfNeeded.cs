using System;
using UnityEngine;

namespace UnityEditor.Animations
{
	internal struct PushUndoIfNeeded
	{
		private class PushUndoIfNeededImpl
		{
			public bool m_PushUndo;

			public PushUndoIfNeededImpl(bool pushUndo)
			{
				this.m_PushUndo = pushUndo;
			}

			public void DoUndo(UnityEngine.Object target, string undoOperation)
			{
				if (this.m_PushUndo)
				{
					Undo.RegisterCompleteObjectUndo(target, undoOperation);
				}
			}
		}

		private PushUndoIfNeeded.PushUndoIfNeededImpl m_Impl;

		public bool pushUndo
		{
			get
			{
				return this.impl.m_PushUndo;
			}
			set
			{
				this.impl.m_PushUndo = value;
			}
		}

		private PushUndoIfNeeded.PushUndoIfNeededImpl impl
		{
			get
			{
				if (this.m_Impl == null)
				{
					this.m_Impl = new PushUndoIfNeeded.PushUndoIfNeededImpl(true);
				}
				return this.m_Impl;
			}
		}

		public PushUndoIfNeeded(bool pushUndo)
		{
			this.m_Impl = new PushUndoIfNeeded.PushUndoIfNeededImpl(pushUndo);
		}

		public void DoUndo(UnityEngine.Object target, string undoOperation)
		{
			this.impl.DoUndo(target, undoOperation);
		}
	}
}
