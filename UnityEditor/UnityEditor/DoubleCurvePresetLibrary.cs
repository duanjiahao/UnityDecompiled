using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class DoubleCurvePresetLibrary : PresetLibrary
	{
		[Serializable]
		private class DoubleCurvePreset
		{
			[SerializeField]
			private string m_Name;

			[SerializeField]
			private DoubleCurve m_DoubleCurve;

			public DoubleCurve doubleCurve
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

			public string name
			{
				get
				{
					return this.m_Name;
				}
				set
				{
					this.m_Name = value;
				}
			}

			public DoubleCurvePreset(DoubleCurve doubleCurvePreset, string presetName)
			{
				this.doubleCurve = doubleCurvePreset;
				this.name = presetName;
			}
		}

		[SerializeField]
		private List<DoubleCurvePresetLibrary.DoubleCurvePreset> m_Presets = new List<DoubleCurvePresetLibrary.DoubleCurvePreset>();

		private readonly Rect kUnsignedRange = new Rect(0f, 0f, 1f, 1f);

		private readonly Rect kSignedRange = new Rect(0f, -1f, 1f, 2f);

		public override int Count()
		{
			return this.m_Presets.Count;
		}

		public override object GetPreset(int index)
		{
			return this.m_Presets[index].doubleCurve;
		}

		public override void Add(object presetObject, string presetName)
		{
			DoubleCurve doubleCurve = presetObject as DoubleCurve;
			if (doubleCurve == null)
			{
				Debug.LogError("Wrong type used in DoubleCurvePresetLibrary: Should be a DoubleCurve");
				return;
			}
			this.m_Presets.Add(new DoubleCurvePresetLibrary.DoubleCurvePreset(doubleCurve, presetName));
		}

		public override void Replace(int index, object newPresetObject)
		{
			DoubleCurve doubleCurve = newPresetObject as DoubleCurve;
			if (doubleCurve == null)
			{
				Debug.LogError("Wrong type used in DoubleCurvePresetLibrary");
				return;
			}
			this.m_Presets[index].doubleCurve = doubleCurve;
		}

		public override void Remove(int index)
		{
			this.m_Presets.RemoveAt(index);
		}

		public override void Move(int index, int destIndex, bool insertAfterDestIndex)
		{
			PresetLibraryHelpers.MoveListItem<DoubleCurvePresetLibrary.DoubleCurvePreset>(this.m_Presets, index, destIndex, insertAfterDestIndex);
		}

		public override void Draw(Rect rect, int index)
		{
			this.DrawInternal(rect, this.m_Presets[index].doubleCurve);
		}

		public override void Draw(Rect rect, object presetObject)
		{
			this.DrawInternal(rect, presetObject as DoubleCurve);
		}

		private void DrawInternal(Rect rect, DoubleCurve doubleCurve)
		{
			if (doubleCurve == null)
			{
				Debug.Log("DoubleCurve is null");
				return;
			}
			EditorGUIUtility.DrawRegionSwatch(rect, doubleCurve.maxCurve, doubleCurve.minCurve, new Color(0.8f, 0.8f, 0.8f, 1f), EditorGUI.kCurveBGColor, (!doubleCurve.signedRange) ? this.kUnsignedRange : this.kSignedRange);
		}

		public override string GetName(int index)
		{
			return this.m_Presets[index].name;
		}

		public override void SetName(int index, string presetName)
		{
			this.m_Presets[index].name = presetName;
		}
	}
}
