using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class SnapGuideCollection
	{
		private List<SnapGuide> currentGuides;

		private Dictionary<float, List<SnapGuide>> guides = new Dictionary<float, List<SnapGuide>>();

		public void Clear()
		{
			this.guides.Clear();
		}

		public void AddGuide(SnapGuide guide)
		{
			List<SnapGuide> list;
			if (!this.guides.TryGetValue(guide.value, out list))
			{
				list = new List<SnapGuide>();
				this.guides.Add(guide.value, list);
			}
			list.Add(guide);
		}

		public float SnapToGuides(float value, float snapDistance)
		{
			if (this.guides.Count == 0)
			{
				return value;
			}
			KeyValuePair<float, List<SnapGuide>> keyValuePair = default(KeyValuePair<float, List<SnapGuide>>);
			float num = float.PositiveInfinity;
			foreach (KeyValuePair<float, List<SnapGuide>> current in this.guides)
			{
				float num2 = Mathf.Abs(value - current.Key);
				if (num2 < num)
				{
					keyValuePair = current;
					num = num2;
				}
			}
			if (num <= snapDistance)
			{
				value = keyValuePair.Key;
				this.currentGuides = keyValuePair.Value;
			}
			else
			{
				this.currentGuides = null;
			}
			return value;
		}

		public void OnGUI()
		{
			if (Event.current.type == EventType.MouseUp)
			{
				this.currentGuides = null;
			}
		}

		public void DrawGuides()
		{
			if (this.currentGuides != null)
			{
				foreach (SnapGuide current in this.currentGuides)
				{
					current.Draw();
				}
			}
		}
	}
}
