using System;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class VerticalLayout : IDisposable
	{
		private static readonly VerticalLayout instance = new VerticalLayout();

		private VerticalLayout()
		{
		}

		public static IDisposable DoLayout()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			return VerticalLayout.instance;
		}

		void IDisposable.Dispose()
		{
			GUILayout.EndVertical();
		}
	}
}
