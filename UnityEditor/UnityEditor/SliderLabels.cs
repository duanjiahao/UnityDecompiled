using System;
using UnityEngine;

namespace UnityEditor
{
	internal struct SliderLabels
	{
		public GUIContent leftLabel;

		public GUIContent rightLabel;

		public void SetLabels(GUIContent leftLabel, GUIContent rightLabel)
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.leftLabel = leftLabel;
				this.rightLabel = rightLabel;
			}
		}

		public bool HasLabels()
		{
			return Event.current.type == EventType.Repaint && this.leftLabel != null && this.rightLabel != null;
		}
	}
}
