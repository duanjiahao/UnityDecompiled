using System;
using UnityEditor.Audio;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerExposedParametersPopup : PopupWindowContent
	{
		private static GUIContent m_ButtonContent = new GUIContent("", "Audio Mixer parameters can be exposed to scripting. Select an Audio Mixer Group, right click one of its properties in the Inspector and select 'Expose ..'.");

		private static int m_LastNumExposedParams = -1;

		private readonly AudioMixerExposedParameterView m_ExposedParametersView;

		private AudioMixerExposedParametersPopup(AudioMixerController controller)
		{
			this.m_ExposedParametersView = new AudioMixerExposedParameterView(new ReorderableListWithRenameAndScrollView.State());
			this.m_ExposedParametersView.OnMixerControllerChanged(controller);
		}

		internal static void Popup(AudioMixerController controller, GUIStyle style, params GUILayoutOption[] options)
		{
			GUIContent buttonContent = AudioMixerExposedParametersPopup.GetButtonContent(controller);
			Rect rect = GUILayoutUtility.GetRect(buttonContent, style, options);
			if (EditorGUI.ButtonMouseDown(rect, buttonContent, FocusType.Passive, style))
			{
				PopupWindow.Show(rect, new AudioMixerExposedParametersPopup(controller));
			}
		}

		private static GUIContent GetButtonContent(AudioMixerController controller)
		{
			if (controller.numExposedParameters != AudioMixerExposedParametersPopup.m_LastNumExposedParams)
			{
				AudioMixerExposedParametersPopup.m_ButtonContent.text = string.Format("Exposed Parameters ({0})", controller.numExposedParameters);
				AudioMixerExposedParametersPopup.m_LastNumExposedParams = controller.numExposedParameters;
			}
			return AudioMixerExposedParametersPopup.m_ButtonContent;
		}

		public override void OnGUI(Rect rect)
		{
			this.m_ExposedParametersView.OnEvent();
			this.m_ExposedParametersView.OnGUI(rect);
		}

		public override Vector2 GetWindowSize()
		{
			Vector2 result = this.m_ExposedParametersView.CalcSize();
			result.x = Math.Max(result.x, 125f);
			result.y = Math.Max(result.y, 23f);
			return result;
		}
	}
}
