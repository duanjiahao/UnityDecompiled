using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class CurveEditorSelection : ScriptableObject
	{
		[SerializeField]
		private List<CurveSelection> m_SelectedCurves;

		public List<CurveSelection> selectedCurves
		{
			get
			{
				List<CurveSelection> arg_1C_0;
				if ((arg_1C_0 = this.m_SelectedCurves) == null)
				{
					arg_1C_0 = (this.m_SelectedCurves = new List<CurveSelection>());
				}
				return arg_1C_0;
			}
			set
			{
				this.m_SelectedCurves = value;
			}
		}
	}
}
