using System;
using System.IO;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(CurvePresetLibrary))]
	internal class CurvePresetLibraryEditor : Editor
	{
		private GenericPresetLibraryInspector<CurvePresetLibrary> m_GenericPresetLibraryInspector;

		private CurveLibraryType m_CurveLibraryType;

		public void OnEnable()
		{
			string assetPath = AssetDatabase.GetAssetPath(this.target.GetInstanceID());
			this.m_CurveLibraryType = this.GetCurveLibraryTypeFromExtension(Path.GetExtension(assetPath).TrimStart(new char[]
			{
				'.'
			}));
			this.m_GenericPresetLibraryInspector = new GenericPresetLibraryInspector<CurvePresetLibrary>(this.target, this.GetHeader(), new Action<string>(this.OnEditButtonClicked));
			this.m_GenericPresetLibraryInspector.presetSize = new Vector2(72f, 20f);
			this.m_GenericPresetLibraryInspector.lineSpacing = 5f;
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
			string basePrefText = CurvePresetsContentsForPopupWindow.GetBasePrefText(this.m_CurveLibraryType);
			this.m_GenericPresetLibraryInspector.itemViewMode = PresetLibraryEditorState.GetItemViewMode(basePrefText);
			if (this.m_GenericPresetLibraryInspector != null)
			{
				this.m_GenericPresetLibraryInspector.OnInspectorGUI();
			}
		}

		private void OnEditButtonClicked(string libraryPath)
		{
			Rect curveRanges = this.GetCurveRanges();
			CurveEditorSettings curveEditorSettings = new CurveEditorSettings();
			if (curveRanges.width > 0f && curveRanges.height > 0f && curveRanges.width != float.PositiveInfinity && curveRanges.height != float.PositiveInfinity)
			{
				curveEditorSettings.hRangeMin = curveRanges.xMin;
				curveEditorSettings.hRangeMax = curveRanges.xMax;
				curveEditorSettings.vRangeMin = curveRanges.yMin;
				curveEditorSettings.vRangeMax = curveRanges.yMax;
			}
			CurveEditorWindow.curve = new AnimationCurve();
			CurveEditorWindow.color = new Color(0f, 0.8f, 0f);
			CurveEditorWindow.instance.Show(GUIView.current, curveEditorSettings);
			CurveEditorWindow.instance.currentPresetLibrary = libraryPath;
		}

		private string GetHeader()
		{
			CurveLibraryType curveLibraryType = this.m_CurveLibraryType;
			if (curveLibraryType == CurveLibraryType.Unbounded)
			{
				return "Curve Preset Library";
			}
			if (curveLibraryType != CurveLibraryType.NormalizedZeroToOne)
			{
				return "Curve Preset Library ?";
			}
			return "Curve Preset Library (Normalized 0 - 1)";
		}

		private Rect GetCurveRanges()
		{
			CurveLibraryType curveLibraryType = this.m_CurveLibraryType;
			if (curveLibraryType == CurveLibraryType.Unbounded)
			{
				return default(Rect);
			}
			if (curveLibraryType != CurveLibraryType.NormalizedZeroToOne)
			{
				return default(Rect);
			}
			return new Rect(0f, 0f, 1f, 1f);
		}

		private CurveLibraryType GetCurveLibraryTypeFromExtension(string extension)
		{
			string curveLibraryExtension = PresetLibraryLocations.GetCurveLibraryExtension(true);
			string curveLibraryExtension2 = PresetLibraryLocations.GetCurveLibraryExtension(false);
			if (extension.Equals(curveLibraryExtension, StringComparison.OrdinalIgnoreCase))
			{
				return CurveLibraryType.NormalizedZeroToOne;
			}
			if (extension.Equals(curveLibraryExtension2, StringComparison.OrdinalIgnoreCase))
			{
				return CurveLibraryType.Unbounded;
			}
			Debug.LogError("Extension not recognized!");
			return CurveLibraryType.NormalizedZeroToOne;
		}
	}
}
