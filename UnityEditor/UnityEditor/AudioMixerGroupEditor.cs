using System;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(AudioMixerGroupController))]
	internal class AudioMixerGroupEditor : Editor
	{
		private AudioMixerEffectView m_EffectView = null;

		private readonly TickTimerHelper m_Ticker = new TickTimerHelper(0.05);

		public static readonly string kPrefKeyForShowCpuUsage = "AudioMixerShowCPU";

		private void OnEnable()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
		}

		private void OnDisable()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
		}

		public void Update()
		{
			if (EditorApplication.isPlaying && this.m_Ticker.DoTick())
			{
				base.Repaint();
			}
		}

		public override void OnInspectorGUI()
		{
			AudioMixerDrawUtils.InitStyles();
			if (this.m_EffectView == null)
			{
				this.m_EffectView = new AudioMixerEffectView();
			}
			AudioMixerGroupController group = base.target as AudioMixerGroupController;
			this.m_EffectView.OnGUI(group);
		}

		public override bool UseDefaultMargins()
		{
			return false;
		}

		internal override void DrawHeaderHelpAndSettingsGUI(Rect r)
		{
			if (this.m_EffectView != null)
			{
				AudioMixerGroupController audioMixerGroupController = base.target as AudioMixerGroupController;
				base.DrawHeaderHelpAndSettingsGUI(r);
				Rect position = new Rect(r.x + 44f, r.yMax - 20f, r.width - 50f, 15f);
				GUI.Label(position, GUIContent.Temp(audioMixerGroupController.controller.name), EditorStyles.miniLabel);
			}
		}

		[MenuItem("CONTEXT/AudioMixerGroupController/Copy all effect settings to all snapshots")]
		private static void CopyAllEffectToSnapshots(MenuCommand command)
		{
			AudioMixerGroupController audioMixerGroupController = command.context as AudioMixerGroupController;
			AudioMixerController controller = audioMixerGroupController.controller;
			if (!(controller == null))
			{
				Undo.RecordObject(controller, "Copy all effect settings to all snapshots");
				controller.CopyAllSettingsToAllSnapshots(audioMixerGroupController, controller.TargetSnapshot);
			}
		}

		[MenuItem("CONTEXT/AudioMixerGroupController/Toggle CPU usage display (only available on first editor instance)")]
		private static void ShowCPUUsage(MenuCommand command)
		{
			bool @bool = EditorPrefs.GetBool(AudioMixerGroupEditor.kPrefKeyForShowCpuUsage, false);
			EditorPrefs.SetBool(AudioMixerGroupEditor.kPrefKeyForShowCpuUsage, !@bool);
		}
	}
}
