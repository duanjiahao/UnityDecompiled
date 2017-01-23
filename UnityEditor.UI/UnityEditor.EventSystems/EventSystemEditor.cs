using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEditor.EventSystems
{
	[CustomEditor(typeof(EventSystem), true)]
	public class EventSystemEditor : Editor
	{
		private GUIStyle m_PreviewLabelStyle;

		protected GUIStyle previewLabelStyle
		{
			get
			{
				if (this.m_PreviewLabelStyle == null)
				{
					this.m_PreviewLabelStyle = new GUIStyle("PreOverlayLabel")
					{
						richText = true,
						alignment = TextAnchor.UpperLeft,
						fontStyle = FontStyle.Normal
					};
				}
				return this.m_PreviewLabelStyle;
			}
		}

		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();
			EventSystem eventSystem = base.target as EventSystem;
			if (!(eventSystem == null))
			{
				if (!(eventSystem.GetComponent<BaseInputModule>() != null))
				{
					if (GUILayout.Button("Add Default Input Modules", new GUILayoutOption[0]))
					{
						Undo.AddComponent<StandaloneInputModule>(eventSystem.gameObject);
					}
				}
			}
		}

		public override bool HasPreviewGUI()
		{
			return Application.isPlaying;
		}

		public override bool RequiresConstantRepaint()
		{
			return Application.isPlaying;
		}

		public override void OnPreviewGUI(Rect rect, GUIStyle background)
		{
			EventSystem eventSystem = base.target as EventSystem;
			if (!(eventSystem == null))
			{
				GUI.Label(rect, eventSystem.ToString(), this.previewLabelStyle);
			}
		}
	}
}
