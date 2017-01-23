using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(ColorPresetLibrary))]
	internal class ColorPresetLibraryEditor : Editor
	{
		private GenericPresetLibraryInspector<ColorPresetLibrary> m_GenericPresetLibraryInspector;

		public void OnEnable()
		{
			this.m_GenericPresetLibraryInspector = new GenericPresetLibraryInspector<ColorPresetLibrary>(base.target, "Color Preset Library", new Action<string>(this.OnEditButtonClicked));
			this.m_GenericPresetLibraryInspector.useOnePixelOverlappedGrid = true;
			this.m_GenericPresetLibraryInspector.maxShowNumPresets = 2000;
		}

		public void OnDestroy()
		{
			if (this.m_GenericPresetLibraryInspector != null)
			{
				this.m_GenericPresetLibraryInspector.OnDestroy();
			}
		}

		public override void OnInspectorGUI()
		{
			this.m_GenericPresetLibraryInspector.itemViewMode = PresetLibraryEditorState.GetItemViewMode(ColorPicker.presetsEditorPrefID);
			if (this.m_GenericPresetLibraryInspector != null)
			{
				this.m_GenericPresetLibraryInspector.OnInspectorGUI();
			}
		}

		private void OnEditButtonClicked(string libraryPath)
		{
			ColorPicker.Show(GUIView.current, Color.white);
			ColorPicker.get.currentPresetLibrary = libraryPath;
		}
	}
}
