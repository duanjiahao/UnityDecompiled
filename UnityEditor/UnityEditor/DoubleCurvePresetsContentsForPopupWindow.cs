using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class DoubleCurvePresetsContentsForPopupWindow : PopupWindowContent
	{
		private PresetLibraryEditor<DoubleCurvePresetLibrary> m_CurveLibraryEditor;

		private PresetLibraryEditorState m_CurveLibraryEditorState;

		private DoubleCurve m_DoubleCurve;

		private bool m_WantsToClose;

		private Action<DoubleCurve> m_PresetSelectedCallback;

		public DoubleCurve doubleCurveToSave
		{
			get
			{
				return this.m_DoubleCurve;
			}
			set
			{
				this.m_DoubleCurve = value;
			}
		}

		public DoubleCurvePresetsContentsForPopupWindow(DoubleCurve doubleCurveToSave, Action<DoubleCurve> presetSelectedCallback)
		{
			this.m_DoubleCurve = doubleCurveToSave;
			this.m_PresetSelectedCallback = presetSelectedCallback;
		}

		public override void OnClose()
		{
			this.m_CurveLibraryEditorState.TransferEditorPrefsState(false);
		}

		public PresetLibraryEditor<DoubleCurvePresetLibrary> GetPresetLibraryEditor()
		{
			return this.m_CurveLibraryEditor;
		}

		private bool IsSingleCurve(DoubleCurve doubleCurve)
		{
			return doubleCurve.minCurve == null || doubleCurve.minCurve.length == 0;
		}

		private string GetEditorPrefBaseName()
		{
			return PresetLibraryLocations.GetParticleCurveLibraryExtension(this.m_DoubleCurve.IsSingleCurve(), this.m_DoubleCurve.signedRange);
		}

		public void InitIfNeeded()
		{
			if (this.m_CurveLibraryEditorState == null)
			{
				this.m_CurveLibraryEditorState = new PresetLibraryEditorState(this.GetEditorPrefBaseName());
				this.m_CurveLibraryEditorState.TransferEditorPrefsState(true);
			}
			if (this.m_CurveLibraryEditor == null)
			{
				string particleCurveLibraryExtension = PresetLibraryLocations.GetParticleCurveLibraryExtension(this.m_DoubleCurve.IsSingleCurve(), this.m_DoubleCurve.signedRange);
				ScriptableObjectSaveLoadHelper<DoubleCurvePresetLibrary> helper = new ScriptableObjectSaveLoadHelper<DoubleCurvePresetLibrary>(particleCurveLibraryExtension, SaveType.Text);
				this.m_CurveLibraryEditor = new PresetLibraryEditor<DoubleCurvePresetLibrary>(helper, this.m_CurveLibraryEditorState, new Action<int, object>(this.ItemClickedCallback));
				PresetLibraryEditor<DoubleCurvePresetLibrary> expr_7B = this.m_CurveLibraryEditor;
				expr_7B.addDefaultPresets = (Action<PresetLibrary>)Delegate.Combine(expr_7B.addDefaultPresets, new Action<PresetLibrary>(this.AddDefaultPresetsToLibrary));
				this.m_CurveLibraryEditor.presetsWasReordered = new Action(this.PresetsWasReordered);
				this.m_CurveLibraryEditor.previewAspect = 4f;
				this.m_CurveLibraryEditor.minMaxPreviewHeight = new Vector2(24f, 24f);
				this.m_CurveLibraryEditor.showHeader = true;
			}
		}

		private void PresetsWasReordered()
		{
			InspectorWindow.RepaintAllInspectors();
		}

		public override void OnGUI(Rect rect)
		{
			this.InitIfNeeded();
			this.m_CurveLibraryEditor.OnGUI(rect, this.m_DoubleCurve);
			if (this.m_WantsToClose)
			{
				base.editorWindow.Close();
			}
		}

		private void ItemClickedCallback(int clickCount, object presetObject)
		{
			DoubleCurve doubleCurve = presetObject as DoubleCurve;
			if (doubleCurve == null)
			{
				Debug.LogError("Incorrect object passed " + presetObject);
			}
			this.m_PresetSelectedCallback(doubleCurve);
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(240f, 330f);
		}

		private void AddDefaultPresetsToLibrary(PresetLibrary presetLibrary)
		{
			DoubleCurvePresetLibrary doubleCurvePresetLibrary = presetLibrary as DoubleCurvePresetLibrary;
			if (doubleCurvePresetLibrary == null)
			{
				Debug.Log("Incorrect preset library, should be a DoubleCurvePresetLibrary but was a " + presetLibrary.GetType());
				return;
			}
			bool signedRange = this.m_DoubleCurve.signedRange;
			List<DoubleCurve> list = new List<DoubleCurve>();
			if (this.IsSingleCurve(this.m_DoubleCurve))
			{
				list = DoubleCurvePresetsContentsForPopupWindow.GetUnsignedSingleCurveDefaults(signedRange);
			}
			else if (signedRange)
			{
				list = DoubleCurvePresetsContentsForPopupWindow.GetSignedDoubleCurveDefaults();
			}
			else
			{
				list = DoubleCurvePresetsContentsForPopupWindow.GetUnsignedDoubleCurveDefaults();
			}
			foreach (DoubleCurve current in list)
			{
				doubleCurvePresetLibrary.Add(current, string.Empty);
			}
		}

		private static List<DoubleCurve> GetUnsignedSingleCurveDefaults(bool signedRange)
		{
			return new List<DoubleCurve>
			{
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetConstantKeys(1f)), signedRange),
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetLinearKeys()), signedRange),
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetLinearMirrorKeys()), signedRange),
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseInKeys()), signedRange),
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseInMirrorKeys()), signedRange),
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseOutKeys()), signedRange),
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseOutMirrorKeys()), signedRange),
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseInOutKeys()), signedRange),
				new DoubleCurve(null, new AnimationCurve(CurveEditorWindow.GetEaseInOutMirrorKeys()), signedRange)
			};
		}

		private static List<DoubleCurve> GetUnsignedDoubleCurveDefaults()
		{
			return new List<DoubleCurve>
			{
				new DoubleCurve(new AnimationCurve(CurveEditorWindow.GetConstantKeys(0f)), new AnimationCurve(CurveEditorWindow.GetConstantKeys(1f)), false)
			};
		}

		private static List<DoubleCurve> GetSignedDoubleCurveDefaults()
		{
			return new List<DoubleCurve>
			{
				new DoubleCurve(new AnimationCurve(CurveEditorWindow.GetConstantKeys(-1f)), new AnimationCurve(CurveEditorWindow.GetConstantKeys(1f)), true)
			};
		}
	}
}
