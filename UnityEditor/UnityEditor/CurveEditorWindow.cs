using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class CurveEditorWindow : EditorWindow
	{
		internal class Styles
		{
			public GUIStyle curveEditorBackground = "PopupCurveEditorBackground";

			public GUIStyle miniToolbarPopup = "MiniToolbarPopup";

			public GUIStyle miniToolbarButton = "MiniToolbarButtonLeft";

			public GUIStyle curveSwatch = "PopupCurveEditorSwatch";

			public GUIStyle curveSwatchArea = "PopupCurveSwatchBackground";

			public GUIStyle curveWrapPopup = "PopupCurveDropdown";
		}

		private const int kPresetsHeight = 46;

		private static GUIContent s_WrapModeMenuIcon;

		private static CurveEditorWindow s_SharedCurveEditor;

		private CurveEditor m_CurveEditor;

		private AnimationCurve m_Curve;

		private Color m_Color;

		private CurvePresetsContentsForPopupWindow m_CurvePresets;

		private GUIContent m_GUIContent = new GUIContent();

		[SerializeField]
		private GUIView delegateView;

		internal static CurveEditorWindow.Styles ms_Styles;

		public static CurveEditorWindow instance
		{
			get
			{
				if (!CurveEditorWindow.s_SharedCurveEditor)
				{
					CurveEditorWindow.s_SharedCurveEditor = ScriptableObject.CreateInstance<CurveEditorWindow>();
				}
				return CurveEditorWindow.s_SharedCurveEditor;
			}
		}

		public string currentPresetLibrary
		{
			get
			{
				this.InitCurvePresets();
				return this.m_CurvePresets.currentPresetLibrary;
			}
			set
			{
				this.InitCurvePresets();
				this.m_CurvePresets.currentPresetLibrary = value;
			}
		}

		public static AnimationCurve curve
		{
			get
			{
				return CurveEditorWindow.instance.m_Curve;
			}
			set
			{
				if (value == null)
				{
					CurveEditorWindow.instance.m_Curve = null;
				}
				else
				{
					CurveEditorWindow.instance.m_Curve = value;
					CurveEditorWindow.instance.RefreshShownCurves();
				}
			}
		}

		public static Color color
		{
			get
			{
				return CurveEditorWindow.instance.m_Color;
			}
			set
			{
				CurveEditorWindow.instance.m_Color = value;
				CurveEditorWindow.instance.RefreshShownCurves();
			}
		}

		public static bool visible
		{
			get
			{
				return CurveEditorWindow.s_SharedCurveEditor != null;
			}
		}

		private CurveLibraryType curveLibraryType
		{
			get
			{
				if (this.m_CurveEditor.settings.hasUnboundedRanges)
				{
					return CurveLibraryType.Unbounded;
				}
				return CurveLibraryType.NormalizedZeroToOne;
			}
		}

		static CurveEditorWindow()
		{
			CurveEditorWindow.s_WrapModeMenuIcon = EditorGUIUtility.IconContent("AnimationWrapModeMenu");
		}

		private void OnEnable()
		{
			CurveEditorWindow.s_SharedCurveEditor = this;
			this.Init(null);
		}

		private void Init(CurveEditorSettings settings)
		{
			this.m_CurveEditor = new CurveEditor(this.GetCurveEditorRect(), this.GetCurveWrapperArray(), true);
			this.m_CurveEditor.curvesUpdated = new CurveEditor.CallbackFunction(this.UpdateCurve);
			this.m_CurveEditor.scaleWithWindow = true;
			this.m_CurveEditor.margin = 40f;
			if (settings != null)
			{
				this.m_CurveEditor.settings = settings;
			}
			this.m_CurveEditor.settings.hTickLabelOffset = 10f;
			bool horizontally = true;
			bool vertically = true;
			if (this.m_CurveEditor.settings.hRangeMin != float.NegativeInfinity && this.m_CurveEditor.settings.hRangeMax != float.PositiveInfinity)
			{
				this.m_CurveEditor.SetShownHRangeInsideMargins(this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.hRangeMax);
				horizontally = false;
			}
			if (this.m_CurveEditor.settings.vRangeMin != float.NegativeInfinity && this.m_CurveEditor.settings.vRangeMax != float.PositiveInfinity)
			{
				this.m_CurveEditor.SetShownVRangeInsideMargins(this.m_CurveEditor.settings.vRangeMin, this.m_CurveEditor.settings.vRangeMax);
				vertically = false;
			}
			this.m_CurveEditor.FrameSelected(horizontally, vertically);
		}

		private bool GetNormalizationRect(out Rect normalizationRect)
		{
			normalizationRect = default(Rect);
			if (this.m_CurveEditor.settings.hasUnboundedRanges)
			{
				return false;
			}
			normalizationRect = new Rect(this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.vRangeMin, this.m_CurveEditor.settings.hRangeMax - this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.vRangeMax - this.m_CurveEditor.settings.vRangeMin);
			return true;
		}

		private static Keyframe[] CopyAndScaleCurveKeys(Keyframe[] orgKeys, Rect rect, bool realToNormalized)
		{
			if (rect.width == 0f || rect.height == 0f || float.IsInfinity(rect.width) || float.IsInfinity(rect.height))
			{
				Debug.LogError("CopyAndScaleCurve: Invalid scale: " + rect);
				return orgKeys;
			}
			Keyframe[] array = new Keyframe[orgKeys.Length];
			if (realToNormalized)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i].time = (orgKeys[i].time - rect.xMin) / rect.width;
					array[i].value = (orgKeys[i].value - rect.yMin) / rect.height;
				}
			}
			else
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[j].time = orgKeys[j].time * rect.width + rect.xMin;
					array[j].value = orgKeys[j].value * rect.height + rect.yMin;
				}
			}
			return array;
		}

		private void InitCurvePresets()
		{
			if (this.m_CurvePresets == null)
			{
				Action<AnimationCurve> presetSelectedCallback = delegate(AnimationCurve presetCurve)
				{
					this.ValidateCurveLibraryTypeAndScale();
					Rect rect;
					if (this.GetNormalizationRect(out rect))
					{
						bool realToNormalized = false;
						this.m_Curve.keys = CurveEditorWindow.CopyAndScaleCurveKeys(presetCurve.keys, rect, realToNormalized);
					}
					else
					{
						this.m_Curve.keys = presetCurve.keys;
					}
					this.m_Curve.postWrapMode = presetCurve.postWrapMode;
					this.m_Curve.preWrapMode = presetCurve.preWrapMode;
					this.m_CurveEditor.SelectNone();
					this.RefreshShownCurves();
					this.SendEvent("CurveChanged", true);
				};
				AnimationCurve animCurve = null;
				this.m_CurvePresets = new CurvePresetsContentsForPopupWindow(animCurve, this.curveLibraryType, presetSelectedCallback);
				this.m_CurvePresets.InitIfNeeded();
			}
		}

		private void OnDestroy()
		{
			this.m_CurvePresets.GetPresetLibraryEditor().UnloadUsedLibraries();
		}

		private void OnDisable()
		{
			this.m_CurveEditor.OnDisable();
			if (CurveEditorWindow.s_SharedCurveEditor == this)
			{
				CurveEditorWindow.s_SharedCurveEditor = null;
			}
			else if (!this.Equals(CurveEditorWindow.s_SharedCurveEditor))
			{
				throw new ApplicationException("s_SharedCurveEditor does not equal this");
			}
		}

		private void RefreshShownCurves()
		{
			this.m_CurveEditor.animationCurves = this.GetCurveWrapperArray();
		}

		public void Show(GUIView viewToUpdate, CurveEditorSettings settings)
		{
			this.delegateView = viewToUpdate;
			this.Init(settings);
			base.ShowAuxWindow();
			base.titleContent = new GUIContent("Curve");
			base.minSize = new Vector2(240f, 286f);
			base.maxSize = new Vector2(10000f, 10000f);
		}

		private CurveWrapper[] GetCurveWrapperArray()
		{
			if (this.m_Curve == null)
			{
				return new CurveWrapper[0];
			}
			CurveWrapper curveWrapper = new CurveWrapper();
			curveWrapper.id = "Curve".GetHashCode();
			curveWrapper.groupId = -1;
			curveWrapper.color = this.m_Color;
			curveWrapper.hidden = false;
			curveWrapper.readOnly = false;
			curveWrapper.renderer = new NormalCurveRenderer(this.m_Curve);
			curveWrapper.renderer.SetWrap(this.m_Curve.preWrapMode, this.m_Curve.postWrapMode);
			return new CurveWrapper[]
			{
				curveWrapper
			};
		}

		private Rect GetCurveEditorRect()
		{
			return new Rect(0f, 0f, base.position.width, base.position.height - 46f);
		}

		internal static Keyframe[] GetLinearKeys()
		{
			Keyframe[] array = new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			};
			for (int i = 0; i < 2; i++)
			{
				CurveUtility.SetKeyBroken(ref array[i], false);
				CurveUtility.SetKeyTangentMode(ref array[i], 0, TangentMode.Smooth);
				CurveUtility.SetKeyTangentMode(ref array[i], 1, TangentMode.Smooth);
			}
			return array;
		}

		internal static Keyframe[] GetLinearMirrorKeys()
		{
			Keyframe[] array = new Keyframe[]
			{
				new Keyframe(0f, 1f, -1f, -1f),
				new Keyframe(1f, 0f, -1f, -1f)
			};
			for (int i = 0; i < 2; i++)
			{
				CurveUtility.SetKeyBroken(ref array[i], false);
				CurveUtility.SetKeyTangentMode(ref array[i], 0, TangentMode.Smooth);
				CurveUtility.SetKeyTangentMode(ref array[i], 1, TangentMode.Smooth);
			}
			return array;
		}

		internal static Keyframe[] GetEaseInKeys()
		{
			Keyframe[] result = new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f),
				new Keyframe(1f, 1f, 2f, 2f)
			};
			CurveEditorWindow.SetSmoothEditable(ref result);
			return result;
		}

		internal static Keyframe[] GetEaseInMirrorKeys()
		{
			Keyframe[] result = new Keyframe[]
			{
				new Keyframe(0f, 1f, -2f, -2f),
				new Keyframe(1f, 0f, 0f, 0f)
			};
			CurveEditorWindow.SetSmoothEditable(ref result);
			return result;
		}

		internal static Keyframe[] GetEaseOutKeys()
		{
			Keyframe[] result = new Keyframe[]
			{
				new Keyframe(0f, 0f, 2f, 2f),
				new Keyframe(1f, 1f, 0f, 0f)
			};
			CurveEditorWindow.SetSmoothEditable(ref result);
			return result;
		}

		internal static Keyframe[] GetEaseOutMirrorKeys()
		{
			Keyframe[] result = new Keyframe[]
			{
				new Keyframe(0f, 1f, 0f, 0f),
				new Keyframe(1f, 0f, -2f, -2f)
			};
			CurveEditorWindow.SetSmoothEditable(ref result);
			return result;
		}

		internal static Keyframe[] GetEaseInOutKeys()
		{
			Keyframe[] result = new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f),
				new Keyframe(1f, 1f, 0f, 0f)
			};
			CurveEditorWindow.SetSmoothEditable(ref result);
			return result;
		}

		internal static Keyframe[] GetEaseInOutMirrorKeys()
		{
			Keyframe[] result = new Keyframe[]
			{
				new Keyframe(0f, 1f, 0f, 0f),
				new Keyframe(1f, 0f, 0f, 0f)
			};
			CurveEditorWindow.SetSmoothEditable(ref result);
			return result;
		}

		internal static Keyframe[] GetConstantKeys(float value)
		{
			Keyframe[] result = new Keyframe[]
			{
				new Keyframe(0f, value, 0f, 0f),
				new Keyframe(1f, value, 0f, 0f)
			};
			CurveEditorWindow.SetSmoothEditable(ref result);
			return result;
		}

		internal static void SetSmoothEditable(ref Keyframe[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				CurveUtility.SetKeyBroken(ref keys[i], false);
				CurveUtility.SetKeyTangentMode(ref keys[i], 0, TangentMode.Editable);
				CurveUtility.SetKeyTangentMode(ref keys[i], 1, TangentMode.Editable);
			}
		}

		private void OnGUI()
		{
			bool flag = Event.current.type == EventType.MouseUp;
			if (this.delegateView == null)
			{
				this.m_Curve = null;
			}
			if (CurveEditorWindow.ms_Styles == null)
			{
				CurveEditorWindow.ms_Styles = new CurveEditorWindow.Styles();
			}
			this.m_CurveEditor.rect = this.GetCurveEditorRect();
			this.m_CurveEditor.hRangeLocked = Event.current.shift;
			this.m_CurveEditor.vRangeLocked = EditorGUI.actionKey;
			GUI.changed = false;
			GUI.Label(this.m_CurveEditor.drawRect, GUIContent.none, CurveEditorWindow.ms_Styles.curveEditorBackground);
			this.m_CurveEditor.BeginViewGUI();
			this.m_CurveEditor.GridGUI();
			this.m_CurveEditor.CurveGUI();
			this.DoWrapperPopups();
			this.m_CurveEditor.EndViewGUI();
			GUI.Box(new Rect(0f, base.position.height - 46f, base.position.width, 46f), string.Empty, CurveEditorWindow.ms_Styles.curveSwatchArea);
			Color color = this.m_Color;
			color.a *= 0.6f;
			float num = base.position.height - 46f + 10.5f;
			this.InitCurvePresets();
			CurvePresetLibrary currentLib = this.m_CurvePresets.GetPresetLibraryEditor().GetCurrentLib();
			if (currentLib != null)
			{
				for (int i = 0; i < currentLib.Count(); i++)
				{
					Rect rect = new Rect(45f + 45f * (float)i, num, 40f, 25f);
					this.m_GUIContent.tooltip = currentLib.GetName(i);
					if (GUI.Button(rect, this.m_GUIContent, CurveEditorWindow.ms_Styles.curveSwatch))
					{
						AnimationCurve animationCurve = currentLib.GetPreset(i) as AnimationCurve;
						this.m_Curve.keys = animationCurve.keys;
						this.m_Curve.postWrapMode = animationCurve.postWrapMode;
						this.m_Curve.preWrapMode = animationCurve.preWrapMode;
						this.m_CurveEditor.SelectNone();
						this.SendEvent("CurveChanged", true);
					}
					if (Event.current.type == EventType.Repaint)
					{
						currentLib.Draw(rect, i);
					}
					if (rect.xMax > base.position.width - 90f)
					{
						break;
					}
				}
			}
			Rect rect2 = new Rect(25f, num + 5f, 20f, 20f);
			this.PresetDropDown(rect2);
			if (Event.current.type == EventType.Used && flag)
			{
				this.DoUpdateCurve(false);
				this.SendEvent("CurveChangeCompleted", true);
			}
			else if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
			{
				this.DoUpdateCurve(true);
			}
		}

		private void PresetDropDown(Rect rect)
		{
			if (EditorGUI.ButtonMouseDown(rect, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Native, EditorStyles.inspectorTitlebarText) && this.m_Curve != null)
			{
				if (this.m_CurvePresets == null)
				{
					Debug.LogError("Curve presets error");
					return;
				}
				this.ValidateCurveLibraryTypeAndScale();
				AnimationCurve animationCurve = new AnimationCurve();
				Rect rect2;
				if (this.GetNormalizationRect(out rect2))
				{
					bool realToNormalized = true;
					animationCurve.keys = CurveEditorWindow.CopyAndScaleCurveKeys(this.m_Curve.keys, rect2, realToNormalized);
				}
				else
				{
					animationCurve = new AnimationCurve(this.m_Curve.keys);
				}
				animationCurve.postWrapMode = this.m_Curve.postWrapMode;
				animationCurve.preWrapMode = this.m_Curve.preWrapMode;
				this.m_CurvePresets.curveToSaveAsPreset = animationCurve;
				PopupWindow.Show(rect, this.m_CurvePresets);
			}
		}

		private void ValidateCurveLibraryTypeAndScale()
		{
			Rect rect;
			if (this.GetNormalizationRect(out rect))
			{
				if (this.curveLibraryType != CurveLibraryType.NormalizedZeroToOne)
				{
					Debug.LogError("When having a normalize rect we should be using curve library type: NormalizedZeroToOne (normalizationRect: " + rect + ")");
				}
			}
			else if (this.curveLibraryType != CurveLibraryType.Unbounded)
			{
				Debug.LogError("When NOT having a normalize rect we should be using library type: Unbounded");
			}
		}

		public void UpdateCurve()
		{
			this.DoUpdateCurve(false);
		}

		private void DoUpdateCurve(bool exitGUI)
		{
			if (this.m_CurveEditor.animationCurves.Length > 0 && this.m_CurveEditor.animationCurves[0] != null && this.m_CurveEditor.animationCurves[0].changed)
			{
				this.m_CurveEditor.animationCurves[0].changed = false;
				this.SendEvent("CurveChanged", exitGUI);
			}
		}

		private void DoWrapperPopups()
		{
			if (this.m_Curve != null && this.m_Curve.length >= 2 && this.m_Curve.preWrapMode != WrapMode.Default)
			{
				Color contentColor = GUI.contentColor;
				GUI.contentColor = Color.white;
				Keyframe key = this.m_Curve.keys[0];
				WrapMode wrapMode = (this.m_Curve == null) ? WrapMode.Default : this.m_Curve.preWrapMode;
				wrapMode = this.WrapModeIconPopup(key, wrapMode, -1.5f);
				if (this.m_Curve != null && wrapMode != this.m_Curve.preWrapMode)
				{
					this.m_Curve.preWrapMode = wrapMode;
					this.RefreshShownCurves();
					this.SendEvent("CurveChanged", true);
				}
				Keyframe key2 = this.m_Curve.keys[this.m_Curve.length - 1];
				WrapMode wrapMode2 = (this.m_Curve == null) ? WrapMode.Default : this.m_Curve.postWrapMode;
				wrapMode2 = this.WrapModeIconPopup(key2, wrapMode2, 0.5f);
				if (this.m_Curve != null && wrapMode2 != this.m_Curve.postWrapMode)
				{
					this.m_Curve.postWrapMode = wrapMode2;
					this.RefreshShownCurves();
					this.SendEvent("CurveChanged", true);
				}
				GUI.contentColor = contentColor;
			}
		}

		private WrapMode WrapModeIconPopup(Keyframe key, WrapMode oldWrap, float hOffset)
		{
			float num = (float)CurveEditorWindow.s_WrapModeMenuIcon.image.width;
			Vector3 lhs = new Vector3(key.time, key.value);
			lhs = this.m_CurveEditor.DrawingToViewTransformPoint(lhs);
			Rect position = new Rect(lhs.x + num * hOffset, lhs.y, num, num);
			Enum[] array = Enum.GetValues(typeof(WrapModeFixedCurve)).Cast<Enum>().ToArray<Enum>();
			string[] texts = (from x in Enum.GetNames(typeof(WrapModeFixedCurve))
			select ObjectNames.NicifyVariableName(x)).ToArray<string>();
			int selected = Array.IndexOf<Enum>(array, (WrapModeFixedCurve)oldWrap);
			int controlID = GUIUtility.GetControlID("WrapModeIconPopup".GetHashCode(), EditorGUIUtility.native, position);
			int selectedValueForControl = EditorGUI.PopupCallbackInfo.GetSelectedValueForControl(controlID, selected);
			GUIContent[] options = EditorGUIUtility.TempContent(texts);
			Event current = Event.current;
			EventType type = current.type;
			switch (type)
			{
			case EventType.KeyDown:
				if (current.MainActionKeyForControl(controlID))
				{
					if (Application.platform == RuntimePlatform.OSXEditor)
					{
						position.y = position.y - (float)(selectedValueForControl * 16) - 19f;
					}
					EditorGUI.PopupCallbackInfo.instance = new EditorGUI.PopupCallbackInfo(controlID);
					EditorUtility.DisplayCustomMenu(position, options, selectedValueForControl, new EditorUtility.SelectMenuItemFunction(EditorGUI.PopupCallbackInfo.instance.SetEnumValueDelegate), null);
					current.Use();
				}
				goto IL_20E;
			case EventType.KeyUp:
			case EventType.ScrollWheel:
				IL_106:
				if (type != EventType.MouseDown)
				{
					goto IL_20E;
				}
				if (current.button == 0 && position.Contains(current.mousePosition))
				{
					if (Application.platform == RuntimePlatform.OSXEditor)
					{
						position.y = position.y - (float)(selectedValueForControl * 16) - 19f;
					}
					EditorGUI.PopupCallbackInfo.instance = new EditorGUI.PopupCallbackInfo(controlID);
					EditorUtility.DisplayCustomMenu(position, options, selectedValueForControl, new EditorUtility.SelectMenuItemFunction(EditorGUI.PopupCallbackInfo.instance.SetEnumValueDelegate), null);
					GUIUtility.keyboardControl = controlID;
					current.Use();
				}
				goto IL_20E;
			case EventType.Repaint:
				GUIStyle.none.Draw(position, CurveEditorWindow.s_WrapModeMenuIcon, controlID, false);
				goto IL_20E;
			}
			goto IL_106;
			IL_20E:
			return (WrapMode)array[selectedValueForControl];
		}

		private void SendEvent(string eventName, bool exitGUI)
		{
			if (this.delegateView)
			{
				Event e = EditorGUIUtility.CommandEvent(eventName);
				base.Repaint();
				this.delegateView.SendEvent(e);
				if (exitGUI)
				{
					GUIUtility.ExitGUI();
				}
			}
			GUI.changed = true;
		}
	}
}
