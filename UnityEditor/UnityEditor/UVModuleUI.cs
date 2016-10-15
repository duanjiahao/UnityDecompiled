using System;
using UnityEngine;

namespace UnityEditor
{
	internal class UVModuleUI : ModuleUI
	{
		private enum AnimationType
		{
			WholeSheet,
			SingleRow
		}

		private class Texts
		{
			public GUIContent frameOverTime = EditorGUIUtility.TextContent("Frame over Time|Controls the uv animation frame of each particle over its lifetime. On the horisontal axis you will find the lifetime. On the vertical axis you will find the sheet index.");

			public GUIContent startFrame = EditorGUIUtility.TextContent("Start Frame|Phase the animation, so it starts on a frame other than 0.");

			public GUIContent animation = EditorGUIUtility.TextContent("Animation|Specifies the animation type: Whole Sheet or Single Row. Whole Sheet will animate over the whole texture sheet from left to right, top to bottom. Single Row will animate a single row in the sheet from left to right.");

			public GUIContent tiles = EditorGUIUtility.TextContent("Tiles|Defines the tiling of the texture.");

			public GUIContent tilesX = EditorGUIUtility.TextContent("X");

			public GUIContent tilesY = EditorGUIUtility.TextContent("Y");

			public GUIContent row = EditorGUIUtility.TextContent("Row|The row in the sheet which will be played.");

			public GUIContent frame = EditorGUIUtility.TextContent("Frame|The frame in the sheet which will be used.");

			public GUIContent cycles = EditorGUIUtility.TextContent("Cycles|Specifies how many times the animation will loop during the lifetime of the particle.");

			public GUIContent uvChannelMask = EditorGUIUtility.TextContent("Enabled UV Channels|Specifies which UV channels will be animated.");

			public GUIContent randomRow = EditorGUIUtility.TextContent("Random Row|If enabled, the animated row will be chosen randomly.");

			public string[] types = new string[]
			{
				"Whole Sheet",
				"Single Row"
			};
		}

		private SerializedMinMaxCurve m_FrameOverTime;

		private SerializedMinMaxCurve m_StartFrame;

		private SerializedProperty m_TilesX;

		private SerializedProperty m_TilesY;

		private SerializedProperty m_AnimationType;

		private SerializedProperty m_Cycles;

		private SerializedProperty m_UVChannelMask;

		private SerializedProperty m_RandomRow;

		private SerializedProperty m_RowIndex;

		private static UVModuleUI.Texts s_Texts;

		public UVModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "UVModule", displayName)
		{
			this.m_ToolTip = "Particle UV animation. This allows you to specify a texture sheet (a texture with multiple tiles/sub frames) and animate or randomize over it per particle.";
		}

		protected override void Init()
		{
			if (this.m_TilesX != null)
			{
				return;
			}
			if (UVModuleUI.s_Texts == null)
			{
				UVModuleUI.s_Texts = new UVModuleUI.Texts();
			}
			this.m_FrameOverTime = new SerializedMinMaxCurve(this, UVModuleUI.s_Texts.frameOverTime, "frameOverTime");
			this.m_StartFrame = new SerializedMinMaxCurve(this, UVModuleUI.s_Texts.startFrame, "startFrame");
			this.m_StartFrame.m_AllowCurves = false;
			this.m_TilesX = base.GetProperty("tilesX");
			this.m_TilesY = base.GetProperty("tilesY");
			this.m_AnimationType = base.GetProperty("animationType");
			this.m_Cycles = base.GetProperty("cycles");
			this.m_RandomRow = base.GetProperty("randomRow");
			this.m_RowIndex = base.GetProperty("rowIndex");
			this.m_UVChannelMask = base.GetProperty("uvChannelMask");
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (UVModuleUI.s_Texts == null)
			{
				UVModuleUI.s_Texts = new UVModuleUI.Texts();
			}
			ModuleUI.GUIIntDraggableX2(UVModuleUI.s_Texts.tiles, UVModuleUI.s_Texts.tilesX, this.m_TilesX, UVModuleUI.s_Texts.tilesY, this.m_TilesY);
			int num = ModuleUI.GUIPopup(UVModuleUI.s_Texts.animation, this.m_AnimationType, UVModuleUI.s_Texts.types);
			if (num == 1)
			{
				ModuleUI.GUIToggle(UVModuleUI.s_Texts.randomRow, this.m_RandomRow);
				if (!this.m_RandomRow.boolValue)
				{
					ModuleUI.GUIInt(UVModuleUI.s_Texts.row, this.m_RowIndex);
				}
			}
			if (num == 1)
			{
				this.m_FrameOverTime.m_RemapValue = (float)this.m_TilesX.intValue;
				this.m_StartFrame.m_RemapValue = (float)this.m_TilesX.intValue;
			}
			if (num == 0)
			{
				this.m_FrameOverTime.m_RemapValue = (float)(this.m_TilesX.intValue * this.m_TilesY.intValue);
				this.m_StartFrame.m_RemapValue = (float)(this.m_TilesX.intValue * this.m_TilesY.intValue - 1);
			}
			ModuleUI.GUIMinMaxCurve(UVModuleUI.s_Texts.frameOverTime, this.m_FrameOverTime);
			ModuleUI.GUIMinMaxCurve(UVModuleUI.s_Texts.startFrame, this.m_StartFrame);
			ModuleUI.GUIFloat(UVModuleUI.s_Texts.cycles, this.m_Cycles);
			this.m_UVChannelMask.intValue = (int)((UVChannelFlags)ModuleUI.GUIEnumMask(UVModuleUI.s_Texts.uvChannelMask, (UVChannelFlags)this.m_UVChannelMask.intValue));
		}
	}
}
