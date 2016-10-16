using System;
using UnityEngine;

namespace UnityEditor
{
	internal class WindowFocusState : ScriptableObject
	{
		private static WindowFocusState m_Instance;

		internal string m_LastWindowTypeInSameDock = string.Empty;

		internal bool m_WasMaximizedBeforePlay;

		internal bool m_CurrentlyInPlayMode;

		internal static WindowFocusState instance
		{
			get
			{
				if (WindowFocusState.m_Instance == null)
				{
					WindowFocusState.m_Instance = (UnityEngine.Object.FindObjectOfType(typeof(WindowFocusState)) as WindowFocusState);
				}
				if (WindowFocusState.m_Instance == null)
				{
					WindowFocusState.m_Instance = ScriptableObject.CreateInstance<WindowFocusState>();
				}
				return WindowFocusState.m_Instance;
			}
		}

		private void OnEnable()
		{
			base.hideFlags = HideFlags.HideAndDontSave;
			WindowFocusState.m_Instance = this;
		}
	}
}
