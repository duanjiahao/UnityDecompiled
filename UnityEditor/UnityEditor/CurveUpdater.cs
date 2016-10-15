using System;
using System.Collections.Generic;

namespace UnityEditor
{
	internal interface CurveUpdater
	{
		void UpdateCurves(List<int> curveIds, string undoText);

		void UpdateCurves(List<ChangedCurve> curve, string undoText);
	}
}
