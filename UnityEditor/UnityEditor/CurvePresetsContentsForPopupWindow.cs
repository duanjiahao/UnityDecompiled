using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class CurvePresetsContentsForPopupWindow : PopupWindowContent
	{
		private PresetLibraryEditor<CurvePresetLibrary> m_CurveLibraryEditor;

		private PresetLibraryEditorState m_CurveLibraryEditorState;

		private AnimationCurve m_Curve;

		private CurveLibraryType m_CurveLibraryType;

		private bool m_WantsToClose = false;

		private Action<AnimationCurve> m_PresetSelectedCallback;

		public AnimationCurve curveToSaveAsPreset
		{
			get
			{
				return this.m_Curve;
			}
			set
			{
				this.m_Curve = value;
			}
		}

		public string currentPresetLibrary
		{
			get
			{
				this.InitIfNeeded();
				return this.m_CurveLibraryEditor.currentLibraryWithoutExtension;
			}
			set
			{
				this.InitIfNeeded();
				this.m_CurveLibraryEditor.currentLibraryWithoutExtension = value;
			}
		}

		public CurvePresetsContentsForPopupWindow(AnimationCurve animCurve, CurveLibraryType curveLibraryType, Action<AnimationCurve> presetSelectedCallback)
		{
			this.m_CurveLibraryType = curveLibraryType;
			this.m_Curve = animCurve;
			this.m_PresetSelectedCallback = presetSelectedCallback;
		}

		public static string GetBasePrefText(CurveLibraryType curveLibraryType)
		{
			return CurvePresetsContentsForPopupWindow.GetExtension(curveLibraryType);
		}

		private static string GetExtension(CurveLibraryType curveLibraryType)
		{
			string result;
			if (curveLibraryType != CurveLibraryType.NormalizedZeroToOne)
			{
				if (curveLibraryType != CurveLibraryType.Unbounded)
				{
					Debug.LogError("Enum not handled!");
					result = "curves";
				}
				else
				{
					result = PresetLibraryLocations.GetCurveLibraryExtension(false);
				}
			}
			else
			{
				result = PresetLibraryLocations.GetCurveLibraryExtension(true);
			}
			return result;
		}

		public override void OnClose()
		{
			this.m_CurveLibraryEditorState.TransferEditorPrefsState(false);
		}

		public PresetLibraryEditor<CurvePresetLibrary> GetPresetLibraryEditor()
		{
			return this.m_CurveLibraryEditor;
		}

		public void InitIfNeeded()
		{
			if (this.m_CurveLibraryEditorState == null)
			{
				this.m_CurveLibraryEditorState = new PresetLibraryEditorState(CurvePresetsContentsForPopupWindow.GetBasePrefText(this.m_CurveLibraryType));
				this.m_CurveLibraryEditorState.TransferEditorPrefsState(true);
			}
			if (this.m_CurveLibraryEditor == null)
			{
				ScriptableObjectSaveLoadHelper<CurvePresetLibrary> helper = new ScriptableObjectSaveLoadHelper<CurvePresetLibrary>(CurvePresetsContentsForPopupWindow.GetExtension(this.m_CurveLibraryType), SaveType.Text);
				this.m_CurveLibraryEditor = new PresetLibraryEditor<CurvePresetLibrary>(helper, this.m_CurveLibraryEditorState, new Action<int, object>(this.ItemClickedCallback));
				PresetLibraryEditor<CurvePresetLibrary> expr_72 = this.m_CurveLibraryEditor;
				expr_72.addDefaultPresets = (Action<PresetLibrary>)Delegate.Combine(expr_72.addDefaultPresets, new Action<PresetLibrary>(this.AddDefaultPresetsToLibrary));
				PresetLibraryEditor<CurvePresetLibrary> expr_99 = this.m_CurveLibraryEditor;
				expr_99.presetsWasReordered = (Action)Delegate.Combine(expr_99.presetsWasReordered, new Action(this.OnPresetsWasReordered));
				this.m_CurveLibraryEditor.previewAspect = 4f;
				this.m_CurveLibraryEditor.minMaxPreviewHeight = new Vector2(24f, 24f);
				this.m_CurveLibraryEditor.showHeader = true;
			}
		}

		private void OnPresetsWasReordered()
		{
			InternalEditorUtility.RepaintAllViews();
		}

		public override void OnGUI(Rect rect)
		{
			this.InitIfNeeded();
			this.m_CurveLibraryEditor.OnGUI(rect, this.m_Curve);
			if (this.m_WantsToClose)
			{
				base.editorWindow.Close();
			}
		}

		private void ItemClickedCallback(int clickCount, object presetObject)
		{
			AnimationCurve animationCurve = presetObject as AnimationCurve;
			if (animationCurve == null)
			{
				Debug.LogError("Incorrect object passed " + presetObject);
			}
			this.m_PresetSelectedCallback(animationCurve);
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(240f, 330f);
		}

		private void AddDefaultPresetsToLibrary(PresetLibrary presetLibrary)
		{
			CurvePresetLibrary curvePresetLibrary = presetLibrary as CurvePresetLibrary;
			if (curvePresetLibrary == null)
			{
				Debug.Log("Incorrect preset library, should be a CurvePresetLibrary but was a " + presetLibrary.GetType());
			}
			else
			{
				foreach (AnimationCurve current in new List<AnimationCurve>
				{
					new AnimationCurve(CurveEditorWindow.GetConstantKeys(1f)),
					new AnimationCurve(CurveEditorWindow.GetLinearKeys()),
					new AnimationCurve(CurveEditorWindow.GetEaseInKeys()),
					new AnimationCurve(CurveEditorWindow.GetEaseOutKeys()),
					new AnimationCurve(CurveEditorWindow.GetEaseInOutKeys())
				})
				{
					curvePresetLibrary.Add(current, "");
				}
			}
		}
	}
}
