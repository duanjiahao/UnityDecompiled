using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class PresetLibraryEditorState
	{
		public enum ItemViewMode
		{
			Grid,
			List
		}

		[SerializeField]
		private PresetLibraryEditorState.ItemViewMode m_ItemViewMode;

		public float m_PreviewHeight = 32f;

		public Vector2 m_ScrollPosition;

		public string m_CurrrentLibrary = PresetLibraryLocations.defaultPresetLibraryPath;

		public int m_HoverIndex = -1;

		public RenameOverlay m_RenameOverlay = new RenameOverlay();

		public string m_Prefix;

		public PresetLibraryEditorState.ItemViewMode itemViewMode
		{
			get
			{
				return this.m_ItemViewMode;
			}
			set
			{
				if (this.m_ItemViewMode != value)
				{
					this.m_ItemViewMode = value;
					InspectorWindow.RepaintAllInspectors();
					EditorPrefs.SetInt(this.m_Prefix + "ViewMode", (int)this.m_ItemViewMode);
				}
			}
		}

		public PresetLibraryEditorState(string prefix)
		{
			this.m_Prefix = prefix;
		}

		public static PresetLibraryEditorState.ItemViewMode GetItemViewMode(string prefix)
		{
			return (PresetLibraryEditorState.ItemViewMode)EditorPrefs.GetInt(prefix + "ViewMode", 0);
		}

		public void TransferEditorPrefsState(bool load)
		{
			if (load)
			{
				this.m_ItemViewMode = (PresetLibraryEditorState.ItemViewMode)EditorPrefs.GetInt(this.m_Prefix + "ViewMode", (int)this.m_ItemViewMode);
				this.m_PreviewHeight = EditorPrefs.GetFloat(this.m_Prefix + "ItemHeight", this.m_PreviewHeight);
				this.m_ScrollPosition.y = EditorPrefs.GetFloat(this.m_Prefix + "Scroll", this.m_ScrollPosition.y);
				this.m_CurrrentLibrary = EditorPrefs.GetString(this.m_Prefix + "CurrentLib", this.m_CurrrentLibrary);
			}
			else
			{
				EditorPrefs.SetInt(this.m_Prefix + "ViewMode", (int)this.m_ItemViewMode);
				EditorPrefs.SetFloat(this.m_Prefix + "ItemHeight", this.m_PreviewHeight);
				EditorPrefs.SetFloat(this.m_Prefix + "Scroll", this.m_ScrollPosition.y);
				EditorPrefs.SetString(this.m_Prefix + "CurrentLib", this.m_CurrrentLibrary);
			}
		}
	}
}
