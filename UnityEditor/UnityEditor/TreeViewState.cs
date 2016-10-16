using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class TreeViewState
	{
		public Vector2 scrollPos;

		[SerializeField]
		private List<int> m_SelectedIDs = new List<int>();

		[SerializeField]
		private int m_LastClickedID;

		[SerializeField]
		private List<int> m_ExpandedIDs = new List<int>();

		[SerializeField]
		private RenameOverlay m_RenameOverlay = new RenameOverlay();

		[SerializeField]
		private CreateAssetUtility m_CreateAssetUtility = new CreateAssetUtility();

		[SerializeField]
		private string m_SearchString;

		[SerializeField]
		private float[] m_ColumnWidths;

		public List<int> selectedIDs
		{
			get
			{
				return this.m_SelectedIDs;
			}
			set
			{
				this.m_SelectedIDs = value;
			}
		}

		public int lastClickedID
		{
			get
			{
				return this.m_LastClickedID;
			}
			set
			{
				this.m_LastClickedID = value;
			}
		}

		public List<int> expandedIDs
		{
			get
			{
				return this.m_ExpandedIDs;
			}
			set
			{
				this.m_ExpandedIDs = value;
			}
		}

		public RenameOverlay renameOverlay
		{
			get
			{
				return this.m_RenameOverlay;
			}
			set
			{
				this.m_RenameOverlay = value;
			}
		}

		public CreateAssetUtility createAssetUtility
		{
			get
			{
				return this.m_CreateAssetUtility;
			}
			set
			{
				this.m_CreateAssetUtility = value;
			}
		}

		public float[] columnWidths
		{
			get
			{
				return this.m_ColumnWidths;
			}
			set
			{
				this.m_ColumnWidths = value;
			}
		}

		public string searchString
		{
			get
			{
				return this.m_SearchString;
			}
			set
			{
				this.m_SearchString = value;
			}
		}

		public void OnAwake()
		{
			this.m_RenameOverlay.Clear();
			this.m_CreateAssetUtility = new CreateAssetUtility();
		}
	}
}
