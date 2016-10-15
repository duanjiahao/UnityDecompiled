using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class ObjectListAreaState
	{
		public List<int> m_SelectedInstanceIDs = new List<int>();

		public int m_LastClickedInstanceID;

		public bool m_HadKeyboardFocusLastEvent;

		public List<int> m_ExpandedInstanceIDs = new List<int>();

		public RenameOverlay m_RenameOverlay = new RenameOverlay();

		public CreateAssetUtility m_CreateAssetUtility = new CreateAssetUtility();

		public int m_NewAssetIndexInList = -1;

		public Vector2 m_ScrollPosition;

		public int m_GridSize = 64;

		public void OnAwake()
		{
			this.m_NewAssetIndexInList = -1;
			this.m_RenameOverlay.Clear();
			this.m_CreateAssetUtility = new CreateAssetUtility();
		}
	}
}
