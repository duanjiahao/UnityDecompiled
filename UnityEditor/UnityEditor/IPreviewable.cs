using System;
using UnityEngine;

namespace UnityEditor
{
	internal interface IPreviewable
	{
		UnityEngine.Object target
		{
			get;
		}

		void Initialize(UnityEngine.Object[] targets);

		bool MoveNextTarget();

		void ResetTarget();

		bool HasPreviewGUI();

		GUIContent GetPreviewTitle();

		void DrawPreview(Rect previewArea);

		void OnPreviewGUI(Rect r, GUIStyle background);

		void OnInteractivePreviewGUI(Rect r, GUIStyle background);

		void OnPreviewSettings();

		string GetInfoString();

		void ReloadPreviewInstances();
	}
}
