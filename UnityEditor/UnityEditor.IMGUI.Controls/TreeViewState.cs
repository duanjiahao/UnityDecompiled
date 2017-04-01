using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	[Serializable]
	public class TreeViewState
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
		private string m_SearchString;

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

		internal RenameOverlay renameOverlay
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

		internal virtual void OnAwake()
		{
			this.m_RenameOverlay.Clear();
		}
	}
}
