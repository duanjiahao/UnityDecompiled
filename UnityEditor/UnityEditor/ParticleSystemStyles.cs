using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ParticleSystemStyles
	{
		private static ParticleSystemStyles s_ParticleSystemStyles;

		public GUIStyle label = ParticleSystemStyles.FindStyle("ShurikenLabel");

		public GUIStyle numberField = ParticleSystemStyles.FindStyle("ShurikenValue");

		public GUIStyle objectField = ParticleSystemStyles.FindStyle("ShurikenObjectField");

		public GUIStyle effectBgStyle = ParticleSystemStyles.FindStyle("ShurikenEffectBg");

		public GUIStyle emitterHeaderStyle = ParticleSystemStyles.FindStyle("ShurikenEmitterTitle");

		public GUIStyle moduleHeaderStyle = ParticleSystemStyles.FindStyle("ShurikenModuleTitle");

		public GUIStyle moduleBgStyle = ParticleSystemStyles.FindStyle("ShurikenModuleBg");

		public GUIStyle plus = ParticleSystemStyles.FindStyle("ShurikenPlus");

		public GUIStyle minus = ParticleSystemStyles.FindStyle("ShurikenMinus");

		public GUIStyle line = ParticleSystemStyles.FindStyle("ShurikenLine");

		public GUIStyle checkmark = ParticleSystemStyles.FindStyle("ShurikenCheckMark");

		public GUIStyle minMaxCurveStateDropDown = ParticleSystemStyles.FindStyle("ShurikenDropdown");

		public GUIStyle toggle = ParticleSystemStyles.FindStyle("ShurikenToggle");

		public GUIStyle popup = ParticleSystemStyles.FindStyle("ShurikenPopUp");

		public GUIStyle selectionMarker = ParticleSystemStyles.FindStyle("IN ThumbnailShadow");

		public GUIStyle toolbarButtonLeftAlignText = new GUIStyle(ParticleSystemStyles.FindStyle("ToolbarButton"));

		public GUIStyle modulePadding = new GUIStyle();

		public Texture2D warningIcon;

		private ParticleSystemStyles()
		{
			this.emitterHeaderStyle.clipping = TextClipping.Clip;
			this.emitterHeaderStyle.padding.right = 45;
			this.warningIcon = EditorGUIUtility.LoadIcon("console.infoicon.sml");
			this.toolbarButtonLeftAlignText.alignment = TextAnchor.MiddleLeft;
			this.modulePadding.padding = new RectOffset(3, 3, 4, 2);
		}

		public static ParticleSystemStyles Get()
		{
			if (ParticleSystemStyles.s_ParticleSystemStyles == null)
			{
				ParticleSystemStyles.s_ParticleSystemStyles = new ParticleSystemStyles();
			}
			return ParticleSystemStyles.s_ParticleSystemStyles;
		}

		private static GUIStyle FindStyle(string styleName)
		{
			return styleName;
		}
	}
}
