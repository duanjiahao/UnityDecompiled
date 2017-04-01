using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(DoubleCurvePresetLibrary))]
	internal class DoubleCurvePresetLibraryEditor : Editor
	{
		private GenericPresetLibraryInspector<DoubleCurvePresetLibrary> m_GenericPresetLibraryInspector;

		public void OnEnable()
		{
			this.m_GenericPresetLibraryInspector = new GenericPresetLibraryInspector<DoubleCurvePresetLibrary>(base.target, this.GetHeader(), null);
			this.m_GenericPresetLibraryInspector.presetSize = new Vector2(72f, 20f);
			this.m_GenericPresetLibraryInspector.lineSpacing = 5f;
		}

		private string GetHeader()
		{
			return "Particle Curve Preset Library";
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
			if (this.m_GenericPresetLibraryInspector != null)
			{
				this.m_GenericPresetLibraryInspector.OnInspectorGUI();
			}
		}
	}
}
