using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class AnimationWindowKeySelection : ScriptableObject, ISerializationCallbackReceiver
	{
		private HashSet<int> m_SelectedKeyHashes;

		[SerializeField]
		private List<int> m_SelectedKeyHashesSerialized;

		public HashSet<int> selectedKeyHashes
		{
			get
			{
				HashSet<int> arg_1C_0;
				if ((arg_1C_0 = this.m_SelectedKeyHashes) == null)
				{
					arg_1C_0 = (this.m_SelectedKeyHashes = new HashSet<int>());
				}
				return arg_1C_0;
			}
			set
			{
				this.m_SelectedKeyHashes = value;
			}
		}

		public void SaveSelection(string undoLabel)
		{
			Undo.RegisterCompleteObjectUndo(this, undoLabel);
		}

		public void OnBeforeSerialize()
		{
			this.m_SelectedKeyHashesSerialized = this.m_SelectedKeyHashes.ToList<int>();
		}

		public void OnAfterDeserialize()
		{
			this.m_SelectedKeyHashes = new HashSet<int>(this.m_SelectedKeyHashesSerialized);
		}
	}
}
