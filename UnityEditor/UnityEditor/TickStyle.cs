using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class TickStyle
	{
		public EditorGUIUtility.SkinnedColor tickColor = new EditorGUIUtility.SkinnedColor(new Color(0f, 0f, 0f, 0.2f), new Color(0.45f, 0.45f, 0.45f, 0.2f));

		public EditorGUIUtility.SkinnedColor labelColor = new EditorGUIUtility.SkinnedColor(new Color(0f, 0f, 0f, 0.32f), new Color(0.8f, 0.8f, 0.8f, 0.32f));

		public int distMin = 10;

		public int distFull = 80;

		public int distLabel = 50;

		public bool stubs;

		public bool centerLabel;

		public string unit = string.Empty;
	}
}
