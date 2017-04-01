using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class TreeViewStateWithAssetUtility : TreeViewState
	{
		[SerializeField]
		private CreateAssetUtility m_CreateAssetUtility = new CreateAssetUtility();

		internal CreateAssetUtility createAssetUtility
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

		internal override void OnAwake()
		{
			base.OnAwake();
			this.m_CreateAssetUtility.Clear();
		}
	}
}
