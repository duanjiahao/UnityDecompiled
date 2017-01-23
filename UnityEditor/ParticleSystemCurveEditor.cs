using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

internal class ParticleSystemCurveEditor
{
	internal class Styles
	{
		public GUIStyle curveEditorBackground = "CurveEditorBackground";

		public GUIStyle curveSwatch = "PopupCurveEditorSwatch";

		public GUIStyle curveSwatchArea = "PopupCurveSwatchBackground";

		public GUIStyle minus = "OL Minus";

		public GUIStyle plus = "OL Plus";

		public GUIStyle yAxisHeader = new GUIStyle(ParticleSystemStyles.Get().label);

		public GUIContent optimizeCurveText = new GUIContent("", "Click to optimize curve. Optimized curves are defined by having at most 3 keys, with a key at both ends, and do not support loop or ping pong wrapping.");

		public GUIContent removeCurveText = new GUIContent("", "Remove selected curve(s)");

		public GUIContent curveLibraryPopup = new GUIContent("", "Open curve library");

		public GUIContent presetTooltip = new GUIContent();
	}

	public class CurveData
	{
		public SerializedProperty m_Max;

		public SerializedProperty m_Min;

		public bool m_SignedRange;

		public Color m_Color;

		public string m_UniqueName;

		public GUIContent m_DisplayName;

		public CurveWrapper.GetAxisScalarsCallback m_GetAxisScalarsCallback;

		public CurveWrapper.SetAxisScalarsCallback m_SetAxisScalarsCallback;

		public int m_MaxId;

		public int m_MinId;

		public bool m_Visible;

		private static int s_IdCounter;

		public CurveData(string name, GUIContent displayName, SerializedProperty min, SerializedProperty max, Color color, bool signedRange, CurveWrapper.GetAxisScalarsCallback getAxisScalars, CurveWrapper.SetAxisScalarsCallback setAxisScalars, bool visible)
		{
			this.m_UniqueName = name;
			this.m_DisplayName = displayName;
			this.m_SignedRange = signedRange;
			this.m_Min = min;
			this.m_Max = max;
			if (this.m_Min != null)
			{
				this.m_MinId = ++ParticleSystemCurveEditor.CurveData.s_IdCounter;
			}
			if (this.m_Max != null)
			{
				this.m_MaxId = ++ParticleSystemCurveEditor.CurveData.s_IdCounter;
			}
			this.m_Color = color;
			this.m_GetAxisScalarsCallback = getAxisScalars;
			this.m_SetAxisScalarsCallback = setAxisScalars;
			this.m_Visible = visible;
			if (this.m_Max == null || this.m_MaxId == 0)
			{
				Debug.LogError("Max curve should always be valid! (Min curve can be null)");
			}
		}

		public bool IsRegion()
		{
			return this.m_Min != null;
		}
	}

	private List<ParticleSystemCurveEditor.CurveData> m_AddedCurves;

	private CurveEditor m_CurveEditor;

	private static CurveEditorSettings m_CurveEditorSettings = new CurveEditorSettings();

	private Color[] m_Colors;

	private List<Color> m_AvailableColors;

	private DoubleCurvePresetsContentsForPopupWindow m_DoubleCurvePresets;

	public const float k_PresetsHeight = 30f;

	internal static ParticleSystemCurveEditor.Styles s_Styles;

	private int m_LastTopMostCurveID = -1;

	public void OnDisable()
	{
		this.m_CurveEditor.OnDisable();
		Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
	}

	public void OnDestroy()
	{
		this.m_DoubleCurvePresets.GetPresetLibraryEditor().UnloadUsedLibraries();
	}

	public void Refresh()
	{
		this.ContentChanged();
		AnimationCurvePreviewCache.ClearCache();
	}

	public void Init()
	{
		if (this.m_AddedCurves == null)
		{
			this.m_AddedCurves = new List<ParticleSystemCurveEditor.CurveData>();
			this.m_Colors = new Color[]
			{
				new Color(1f, 0.619607866f, 0.129411772f),
				new Color(0.8745098f, 0.211764708f, 0.5803922f),
				new Color(0f, 0.6862745f, 1f),
				new Color(1f, 0.921568632f, 0f),
				new Color(0.196078435f, 1f, 0.266666681f),
				new Color(0.980392158f, 0f, 0f)
			};
			this.m_AvailableColors = new List<Color>(this.m_Colors);
			ParticleSystemCurveEditor.m_CurveEditorSettings.useFocusColors = true;
			ParticleSystemCurveEditor.m_CurveEditorSettings.showAxisLabels = false;
			ParticleSystemCurveEditor.m_CurveEditorSettings.hRangeMin = 0f;
			ParticleSystemCurveEditor.m_CurveEditorSettings.vRangeMin = 0f;
			ParticleSystemCurveEditor.m_CurveEditorSettings.vRangeMax = 1f;
			ParticleSystemCurveEditor.m_CurveEditorSettings.hRangeMax = 1f;
			ParticleSystemCurveEditor.m_CurveEditorSettings.vSlider = false;
			ParticleSystemCurveEditor.m_CurveEditorSettings.hSlider = false;
			ParticleSystemCurveEditor.m_CurveEditorSettings.showWrapperPopups = true;
			ParticleSystemCurveEditor.m_CurveEditorSettings.rectangleToolFlags = CurveEditorSettings.RectangleToolFlags.MiniRectangleTool;
			ParticleSystemCurveEditor.m_CurveEditorSettings.hTickLabelOffset = 5f;
			ParticleSystemCurveEditor.m_CurveEditorSettings.allowDraggingCurvesAndRegions = true;
			ParticleSystemCurveEditor.m_CurveEditorSettings.allowDeleteLastKeyInCurve = false;
			TickStyle tickStyle = new TickStyle();
			tickStyle.tickColor.color = new Color(0f, 0f, 0f, 0.2f);
			tickStyle.distLabel = 30;
			tickStyle.stubs = false;
			tickStyle.centerLabel = true;
			ParticleSystemCurveEditor.m_CurveEditorSettings.hTickStyle = tickStyle;
			TickStyle tickStyle2 = new TickStyle();
			tickStyle2.tickColor.color = new Color(0f, 0f, 0f, 0.2f);
			tickStyle2.distLabel = 20;
			tickStyle2.stubs = false;
			tickStyle2.centerLabel = true;
			ParticleSystemCurveEditor.m_CurveEditorSettings.vTickStyle = tickStyle2;
			this.m_CurveEditor = new CurveEditor(new Rect(0f, 0f, 1000f, 100f), this.CreateCurveWrapperArray(), false);
			this.m_CurveEditor.settings = ParticleSystemCurveEditor.m_CurveEditorSettings;
			this.m_CurveEditor.leftmargin = 40f;
			ZoomableArea arg_2A4_0 = this.m_CurveEditor;
			float num = 25f;
			this.m_CurveEditor.bottommargin = num;
			num = num;
			this.m_CurveEditor.topmargin = num;
			arg_2A4_0.rightmargin = num;
			this.m_CurveEditor.SetShownHRangeInsideMargins(ParticleSystemCurveEditor.m_CurveEditorSettings.hRangeMin, ParticleSystemCurveEditor.m_CurveEditorSettings.hRangeMax);
			this.m_CurveEditor.SetShownVRangeInsideMargins(ParticleSystemCurveEditor.m_CurveEditorSettings.vRangeMin, ParticleSystemCurveEditor.m_CurveEditorSettings.hRangeMax);
			this.m_CurveEditor.ignoreScrollWheelUntilClicked = false;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}
	}

	private void UndoRedoPerformed()
	{
		this.ContentChanged();
	}

	private void UpdateRangeBasedOnShownCurves()
	{
		bool flag = false;
		for (int i = 0; i < this.m_AddedCurves.Count; i++)
		{
			flag |= this.m_AddedCurves[i].m_SignedRange;
		}
		float num = (!flag) ? 0f : -1f;
		if (num != ParticleSystemCurveEditor.m_CurveEditorSettings.vRangeMin)
		{
			ParticleSystemCurveEditor.m_CurveEditorSettings.vRangeMin = num;
			this.m_CurveEditor.settings = ParticleSystemCurveEditor.m_CurveEditorSettings;
			this.m_CurveEditor.SetShownVRangeInsideMargins(ParticleSystemCurveEditor.m_CurveEditorSettings.vRangeMin, ParticleSystemCurveEditor.m_CurveEditorSettings.hRangeMax);
		}
	}

	public bool IsAdded(SerializedProperty min, SerializedProperty max)
	{
		return this.FindIndex(min, max) != -1;
	}

	public bool IsAdded(SerializedProperty max)
	{
		return this.FindIndex(null, max) != -1;
	}

	public void AddCurve(ParticleSystemCurveEditor.CurveData curveData)
	{
		this.Add(curveData);
	}

	public void RemoveCurve(SerializedProperty max)
	{
		this.RemoveCurve(null, max);
	}

	public void RemoveCurve(SerializedProperty min, SerializedProperty max)
	{
		if (this.Remove(this.FindIndex(min, max)))
		{
			this.ContentChanged();
			this.UpdateRangeBasedOnShownCurves();
		}
	}

	public Color GetCurveColor(SerializedProperty max)
	{
		int num = this.FindIndex(max);
		Color result;
		if (num >= 0 && num < this.m_AddedCurves.Count)
		{
			result = this.m_AddedCurves[num].m_Color;
		}
		else
		{
			result = new Color(0.8f, 0.8f, 0.8f, 0.7f);
		}
		return result;
	}

	public void AddCurveDataIfNeeded(string curveName, ParticleSystemCurveEditor.CurveData curveData)
	{
		Vector3 vector = SessionState.GetVector3(curveName, Vector3.zero);
		if (vector != Vector3.zero)
		{
			Color color = new Color(vector.x, vector.y, vector.z);
			curveData.m_Color = color;
			this.AddCurve(curveData);
			for (int i = 0; i < this.m_AvailableColors.Count; i++)
			{
				if (ParticleSystemCurveEditor.SameColor(this.m_AvailableColors[i], color))
				{
					this.m_AvailableColors.RemoveAt(i);
					break;
				}
			}
		}
	}

	public void SetVisible(SerializedProperty curveProp, bool visible)
	{
		int num = this.FindIndex(curveProp);
		if (num >= 0)
		{
			this.m_AddedCurves[num].m_Visible = visible;
		}
	}

	private static bool SameColor(Color c1, Color c2)
	{
		return Mathf.Abs(c1.r - c2.r) < 0.01f && Mathf.Abs(c1.g - c2.g) < 0.01f && Mathf.Abs(c1.b - c2.b) < 0.01f;
	}

	private int FindIndex(SerializedProperty prop)
	{
		return this.FindIndex(null, prop);
	}

	private int FindIndex(SerializedProperty min, SerializedProperty max)
	{
		int result;
		if (max == null)
		{
			result = -1;
		}
		else
		{
			if (min == null)
			{
				for (int i = 0; i < this.m_AddedCurves.Count; i++)
				{
					if (this.m_AddedCurves[i].m_Max == max)
					{
						result = i;
						return result;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.m_AddedCurves.Count; j++)
				{
					if (this.m_AddedCurves[j].m_Max == max && this.m_AddedCurves[j].m_Min == min)
					{
						result = j;
						return result;
					}
				}
			}
			result = -1;
		}
		return result;
	}

	private void Add(ParticleSystemCurveEditor.CurveData cd)
	{
		this.m_CurveEditor.SelectNone();
		this.m_AddedCurves.Add(cd);
		this.ContentChanged();
		SessionState.SetVector3(cd.m_UniqueName, new Vector3(cd.m_Color.r, cd.m_Color.g, cd.m_Color.b));
		this.UpdateRangeBasedOnShownCurves();
	}

	private bool Remove(int index)
	{
		bool result;
		if (index >= 0 && index < this.m_AddedCurves.Count)
		{
			Color color = this.m_AddedCurves[index].m_Color;
			this.m_AvailableColors.Add(color);
			string uniqueName = this.m_AddedCurves[index].m_UniqueName;
			SessionState.EraseVector3(uniqueName);
			this.m_AddedCurves.RemoveAt(index);
			if (this.m_AddedCurves.Count == 0)
			{
				this.m_AvailableColors = new List<Color>(this.m_Colors);
			}
			result = true;
		}
		else
		{
			Debug.Log("Invalid index in ParticleSystemCurveEditor::Remove");
			result = false;
		}
		return result;
	}

	private void RemoveTopMost()
	{
		int num;
		if (this.m_CurveEditor.GetTopMostCurveID(out num))
		{
			for (int i = 0; i < this.m_AddedCurves.Count; i++)
			{
				ParticleSystemCurveEditor.CurveData curveData = this.m_AddedCurves[i];
				if (curveData.m_MaxId == num || curveData.m_MinId == num)
				{
					this.Remove(i);
					this.ContentChanged();
					this.UpdateRangeBasedOnShownCurves();
					break;
				}
			}
		}
	}

	private void RemoveSelected()
	{
		bool flag = false;
		List<CurveSelection> selectedCurves = this.m_CurveEditor.selectedCurves;
		for (int i = 0; i < selectedCurves.Count; i++)
		{
			int curveID = selectedCurves[i].curveID;
			for (int j = 0; j < this.m_AddedCurves.Count; j++)
			{
				ParticleSystemCurveEditor.CurveData curveData = this.m_AddedCurves[j];
				if (curveData.m_MaxId == curveID || curveData.m_MinId == curveID)
				{
					flag |= this.Remove(j);
					break;
				}
			}
		}
		if (flag)
		{
			this.ContentChanged();
			this.UpdateRangeBasedOnShownCurves();
		}
		this.m_CurveEditor.SelectNone();
	}

	private void RemoveAll()
	{
		bool flag = false;
		while (this.m_AddedCurves.Count > 0)
		{
			flag |= this.Remove(0);
		}
		if (flag)
		{
			this.ContentChanged();
			this.UpdateRangeBasedOnShownCurves();
		}
	}

	public Color GetAvailableColor()
	{
		if (this.m_AvailableColors.Count == 0)
		{
			this.m_AvailableColors = new List<Color>(this.m_Colors);
		}
		int index = this.m_AvailableColors.Count - 1;
		Color result = this.m_AvailableColors[index];
		this.m_AvailableColors.RemoveAt(index);
		return result;
	}

	public void OnGUI(Rect rect)
	{
		this.Init();
		if (ParticleSystemCurveEditor.s_Styles == null)
		{
			ParticleSystemCurveEditor.s_Styles = new ParticleSystemCurveEditor.Styles();
		}
		Rect rect2 = new Rect(rect.x, rect.y, rect.width, rect.height - 30f);
		Rect rect3 = new Rect(rect.x, rect.y + rect2.height, rect.width, 30f);
		GUI.Label(rect2, GUIContent.none, ParticleSystemCurveEditor.s_Styles.curveEditorBackground);
		if (Event.current.type == EventType.Repaint)
		{
			this.m_CurveEditor.rect = rect2;
		}
		CurveWrapper[] animationCurves = this.m_CurveEditor.animationCurves;
		for (int i = 0; i < animationCurves.Length; i++)
		{
			CurveWrapper curveWrapper = animationCurves[i];
			if (curveWrapper.getAxisUiScalarsCallback != null && curveWrapper.setAxisUiScalarsCallback != null)
			{
				Vector2 newAxisScalars = curveWrapper.getAxisUiScalarsCallback();
				if (newAxisScalars.y > 1000000f)
				{
					newAxisScalars.y = 1000000f;
					curveWrapper.setAxisUiScalarsCallback(newAxisScalars);
				}
			}
		}
		this.m_CurveEditor.OnGUI();
		this.DoLabelForTopMostCurve(new Rect(rect.x + 4f, rect.y, rect.width, 20f));
		this.DoRemoveSelectedButton(new Rect(rect2.x, rect2.y, rect2.width, 24f));
		this.DoOptimizeCurveButton(rect3);
		rect3.x += 30f;
		rect3.width -= 60f;
		this.PresetCurveButtons(rect3, rect);
		this.SaveChangedCurves();
	}

	private void DoLabelForTopMostCurve(Rect rect)
	{
		if (this.m_CurveEditor.IsDraggingCurveOrRegion() || this.m_CurveEditor.selectedCurves.Count <= 1)
		{
			int num;
			if (this.m_CurveEditor.GetTopMostCurveID(out num))
			{
				for (int i = 0; i < this.m_AddedCurves.Count; i++)
				{
					if (this.m_AddedCurves[i].m_MaxId == num || this.m_AddedCurves[i].m_MinId == num)
					{
						ParticleSystemCurveEditor.s_Styles.yAxisHeader.normal.textColor = this.m_AddedCurves[i].m_Color;
						GUI.Label(rect, this.m_AddedCurves[i].m_DisplayName, ParticleSystemCurveEditor.s_Styles.yAxisHeader);
						break;
					}
				}
			}
		}
	}

	private void SetConstantCurve(CurveWrapper cw, float constantValue)
	{
		Keyframe[] array = new Keyframe[1];
		array[0].time = 0f;
		array[0].value = constantValue;
		cw.curve.keys = array;
		cw.changed = true;
	}

	private void SetCurve(CurveWrapper cw, AnimationCurve curve)
	{
		Keyframe[] array = new Keyframe[curve.keys.Length];
		Array.Copy(curve.keys, array, array.Length);
		cw.curve.keys = array;
		cw.changed = true;
	}

	private void SetTopMostCurve(DoubleCurve doubleCurve)
	{
		int num;
		if (this.m_CurveEditor.GetTopMostCurveID(out num))
		{
			for (int i = 0; i < this.m_AddedCurves.Count; i++)
			{
				ParticleSystemCurveEditor.CurveData curveData = this.m_AddedCurves[i];
				if (curveData.m_MaxId == num || curveData.m_MinId == num)
				{
					if (doubleCurve.signedRange == curveData.m_SignedRange)
					{
						if (curveData.m_MaxId > 0)
						{
							this.SetCurve(this.m_CurveEditor.GetCurveFromID(curveData.m_MaxId), doubleCurve.maxCurve);
						}
						if (curveData.m_MinId > 0)
						{
							this.SetCurve(this.m_CurveEditor.GetCurveFromID(curveData.m_MinId), doubleCurve.minCurve);
						}
					}
					else
					{
						Debug.LogWarning("Cannot assign a curves with different signed range");
					}
				}
			}
		}
	}

	private DoubleCurve CreateDoubleCurveFromTopMostCurve()
	{
		int num;
		DoubleCurve result;
		if (this.m_CurveEditor.GetTopMostCurveID(out num))
		{
			for (int i = 0; i < this.m_AddedCurves.Count; i++)
			{
				ParticleSystemCurveEditor.CurveData curveData = this.m_AddedCurves[i];
				if (curveData.m_MaxId == num || curveData.m_MinId == num)
				{
					AnimationCurve maxCurve = null;
					AnimationCurve minCurve = null;
					if (curveData.m_Min != null)
					{
						minCurve = curveData.m_Min.animationCurveValue;
					}
					if (curveData.m_Max != null)
					{
						maxCurve = curveData.m_Max.animationCurveValue;
					}
					result = new DoubleCurve(minCurve, maxCurve, curveData.m_SignedRange);
					return result;
				}
			}
		}
		result = null;
		return result;
	}

	private void PresetDropDown(Rect rect)
	{
		if (EditorGUI.ButtonMouseDown(rect, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Passive, EditorStyles.inspectorTitlebarText))
		{
			DoubleCurve doubleCurve = this.CreateDoubleCurveFromTopMostCurve();
			if (doubleCurve != null)
			{
				this.InitDoubleCurvePresets();
				if (this.m_DoubleCurvePresets != null)
				{
					this.m_DoubleCurvePresets.doubleCurveToSave = this.CreateDoubleCurveFromTopMostCurve();
					PopupWindow.Show(rect, this.m_DoubleCurvePresets);
				}
			}
		}
	}

	private void InitDoubleCurvePresets()
	{
		int num;
		if (this.m_CurveEditor.GetTopMostCurveID(out num))
		{
			if (this.m_DoubleCurvePresets == null || this.m_LastTopMostCurveID != num)
			{
				this.m_LastTopMostCurveID = num;
				Action<DoubleCurve> presetSelectedCallback = delegate(DoubleCurve presetDoubleCurve)
				{
					this.SetTopMostCurve(presetDoubleCurve);
					InternalEditorUtility.RepaintAllViews();
				};
				DoubleCurve doubleCurveToSave = this.CreateDoubleCurveFromTopMostCurve();
				this.m_DoubleCurvePresets = new DoubleCurvePresetsContentsForPopupWindow(doubleCurveToSave, presetSelectedCallback);
				this.m_DoubleCurvePresets.InitIfNeeded();
			}
		}
	}

	private void PresetCurveButtons(Rect position, Rect curveEditorRect)
	{
		if (this.m_CurveEditor.animationCurves.Length != 0)
		{
			this.InitDoubleCurvePresets();
			if (this.m_DoubleCurvePresets != null)
			{
				DoubleCurvePresetLibrary currentLib = this.m_DoubleCurvePresets.GetPresetLibraryEditor().GetCurrentLib();
				int a = (!(currentLib != null)) ? 0 : currentLib.Count();
				int num = Mathf.Min(a, 9);
				float num2 = 30f;
				float num3 = 15f;
				float num4 = 10f;
				float num5 = (float)num * num2 + (float)(num - 1) * num4;
				float num6 = (position.width - num5) * 0.5f;
				float num7 = (position.height - num3) * 0.5f;
				float num8 = 3f;
				if (num6 > 0f)
				{
					num8 = num6;
				}
				this.PresetDropDown(new Rect(num8 - 20f + position.x, num7 + position.y, 16f, 16f));
				GUI.BeginGroup(position);
				Color color;
				Color.white.a = color.a * 0.6f;
				for (int i = 0; i < num; i++)
				{
					if (i > 0)
					{
						num8 += num4;
					}
					Rect rect = new Rect(num8, num7, num2, num3);
					ParticleSystemCurveEditor.s_Styles.presetTooltip.tooltip = currentLib.GetName(i);
					if (GUI.Button(rect, ParticleSystemCurveEditor.s_Styles.presetTooltip, GUIStyle.none))
					{
						DoubleCurve doubleCurve = currentLib.GetPreset(i) as DoubleCurve;
						if (doubleCurve != null)
						{
							this.SetTopMostCurve(doubleCurve);
							this.m_CurveEditor.ClearSelection();
						}
					}
					if (Event.current.type == EventType.Repaint)
					{
						currentLib.Draw(rect, i);
					}
					num8 += num2;
				}
				GUI.EndGroup();
			}
		}
	}

	private void DoOptimizeCurveButton(Rect rect)
	{
		if (!this.m_CurveEditor.IsDraggingCurveOrRegion())
		{
			Rect position = new Rect(rect.xMax - 10f - 14f, rect.y + (rect.height - 14f) * 0.5f, 14f, 14f);
			int num = 0;
			List<CurveSelection> selectedCurves = this.m_CurveEditor.selectedCurves;
			int curveID;
			if (selectedCurves.Count > 0)
			{
				for (int i = 0; i < selectedCurves.Count; i++)
				{
					CurveWrapper curveWrapperFromSelection = this.m_CurveEditor.GetCurveWrapperFromSelection(selectedCurves[i]);
					num += ((!AnimationUtility.IsValidPolynomialCurve(curveWrapperFromSelection.curve)) ? 0 : 1);
				}
				if (selectedCurves.Count != num)
				{
					if (GUI.Button(position, ParticleSystemCurveEditor.s_Styles.optimizeCurveText, ParticleSystemCurveEditor.s_Styles.plus))
					{
						for (int j = 0; j < selectedCurves.Count; j++)
						{
							CurveWrapper curveWrapperFromSelection2 = this.m_CurveEditor.GetCurveWrapperFromSelection(selectedCurves[j]);
							if (!AnimationUtility.IsValidPolynomialCurve(curveWrapperFromSelection2.curve))
							{
								curveWrapperFromSelection2.curve.preWrapMode = WrapMode.Once;
								curveWrapperFromSelection2.curve.postWrapMode = WrapMode.Once;
								curveWrapperFromSelection2.renderer.SetWrap(WrapMode.Once, WrapMode.Once);
								AnimationUtility.ConstrainToPolynomialCurve(curveWrapperFromSelection2.curve);
								curveWrapperFromSelection2.changed = true;
							}
						}
						this.m_CurveEditor.SelectNone();
					}
				}
			}
			else if (this.m_CurveEditor.GetTopMostCurveID(out curveID))
			{
				CurveWrapper curveWrapperFromID = this.m_CurveEditor.GetCurveWrapperFromID(curveID);
				if (!AnimationUtility.IsValidPolynomialCurve(curveWrapperFromID.curve))
				{
					if (GUI.Button(position, ParticleSystemCurveEditor.s_Styles.optimizeCurveText, ParticleSystemCurveEditor.s_Styles.plus))
					{
						curveWrapperFromID.curve.preWrapMode = WrapMode.Once;
						curveWrapperFromID.curve.postWrapMode = WrapMode.Once;
						curveWrapperFromID.renderer.SetWrap(WrapMode.Once, WrapMode.Once);
						AnimationUtility.ConstrainToPolynomialCurve(curveWrapperFromID.curve);
						curveWrapperFromID.changed = true;
					}
				}
			}
		}
	}

	private void DoRemoveSelectedButton(Rect rect)
	{
		if (this.m_CurveEditor.animationCurves.Length != 0)
		{
			float num = 14f;
			Rect position = new Rect(rect.x + rect.width - num - 10f, rect.y + (rect.height - num) * 0.5f, num, num);
			if (GUI.Button(position, ParticleSystemCurveEditor.s_Styles.removeCurveText, ParticleSystemCurveEditor.s_Styles.minus))
			{
				if (this.m_CurveEditor.selectedCurves.Count > 0)
				{
					this.RemoveSelected();
				}
				else
				{
					this.RemoveTopMost();
				}
			}
		}
	}

	private void SaveCurve(SerializedProperty prop, CurveWrapper cw)
	{
		if (cw.curve.keys.Length == 1)
		{
			cw.renderer.SetCustomRange(0f, 1f);
			cw.wrapColorMultiplier = Color.clear;
		}
		else
		{
			cw.renderer.SetCustomRange(0f, 0f);
			cw.wrapColorMultiplier = cw.color;
		}
		prop.animationCurveValue = cw.curve;
		cw.changed = false;
	}

	private void SaveChangedCurves()
	{
		CurveWrapper[] animationCurves = this.m_CurveEditor.animationCurves;
		bool flag = false;
		for (int i = 0; i < animationCurves.Length; i++)
		{
			CurveWrapper curveWrapper = animationCurves[i];
			if (curveWrapper.changed)
			{
				for (int j = 0; j < this.m_AddedCurves.Count; j++)
				{
					if (this.m_AddedCurves[j].m_MaxId == curveWrapper.id)
					{
						this.SaveCurve(this.m_AddedCurves[j].m_Max, curveWrapper);
						break;
					}
					if (this.m_AddedCurves[j].IsRegion() && this.m_AddedCurves[j].m_MinId == curveWrapper.id)
					{
						this.SaveCurve(this.m_AddedCurves[j].m_Min, curveWrapper);
						break;
					}
				}
				flag = true;
			}
		}
		if (flag)
		{
			AnimationCurvePreviewCache.ClearCache();
			HandleUtility.Repaint();
		}
	}

	private CurveWrapper CreateCurveWrapper(SerializedProperty curve, int id, int regionId, Color color, bool signedRange, CurveWrapper.GetAxisScalarsCallback getAxisScalarsCallback, CurveWrapper.SetAxisScalarsCallback setAxisScalarsCallback)
	{
		CurveWrapper curveWrapper = new CurveWrapper();
		curveWrapper.id = id;
		curveWrapper.regionId = regionId;
		curveWrapper.color = color;
		curveWrapper.renderer = new NormalCurveRenderer(curve.animationCurveValue);
		curveWrapper.renderer.SetWrap(curve.animationCurveValue.preWrapMode, curve.animationCurveValue.postWrapMode);
		if (curveWrapper.curve.keys.Length == 1)
		{
			curveWrapper.renderer.SetCustomRange(0f, 1f);
			curveWrapper.wrapColorMultiplier = Color.clear;
		}
		else
		{
			curveWrapper.renderer.SetCustomRange(0f, 0f);
			curveWrapper.wrapColorMultiplier = color;
		}
		curveWrapper.vRangeMin = ((!signedRange) ? 0f : -1f);
		curveWrapper.getAxisUiScalarsCallback = getAxisScalarsCallback;
		curveWrapper.setAxisUiScalarsCallback = setAxisScalarsCallback;
		return curveWrapper;
	}

	private CurveWrapper[] CreateCurveWrapperArray()
	{
		List<CurveWrapper> list = new List<CurveWrapper>();
		int num = 0;
		for (int i = 0; i < this.m_AddedCurves.Count; i++)
		{
			ParticleSystemCurveEditor.CurveData curveData = this.m_AddedCurves[i];
			if (curveData.m_Visible)
			{
				int regionId = -1;
				if (curveData.IsRegion())
				{
					num = (regionId = num + 1);
				}
				if (curveData.m_Max != null)
				{
					list.Add(this.CreateCurveWrapper(curveData.m_Max, curveData.m_MaxId, regionId, curveData.m_Color, curveData.m_SignedRange, curveData.m_GetAxisScalarsCallback, curveData.m_SetAxisScalarsCallback));
				}
				if (curveData.m_Min != null)
				{
					list.Add(this.CreateCurveWrapper(curveData.m_Min, curveData.m_MinId, regionId, curveData.m_Color, curveData.m_SignedRange, curveData.m_GetAxisScalarsCallback, curveData.m_SetAxisScalarsCallback));
				}
			}
		}
		return list.ToArray();
	}

	private void ContentChanged()
	{
		this.m_CurveEditor.animationCurves = this.CreateCurveWrapperArray();
		ParticleSystemCurveEditor.m_CurveEditorSettings.showAxisLabels = (this.m_CurveEditor.animationCurves.Length > 0);
	}
}
