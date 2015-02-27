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
			public GUIContent frameOverTime = new GUIContent("Frame over Time", "Controls the uv animation frame of each particle over its lifetime. On the horisontal axis you will find the lifetime. On the vertical axis you will find the sheet index.");
			public GUIContent animation = new GUIContent("Animation", "Specifies the animation type: Whole Sheet or Single Row. Whole Sheet will animate over the whole texture sheet from left to right, top to bottom. Single Row will animate a single row in the sheet from left to right.");
			public GUIContent tiles = new GUIContent("Tiles", "Defines the tiling of the texture.");
			public GUIContent tilesX = new GUIContent("X");
			public GUIContent tilesY = new GUIContent("Y");
			public GUIContent row = new GUIContent("Row", "The row in the sheet which will be played.");
			public GUIContent frame = new GUIContent("Frame", "The frame in the sheet which will be used.");
			public GUIContent cycles = new GUIContent("Cycles", "Specifies how many times the animation will loop during the lifetime of the particle.");
			public GUIContent randomRow = new GUIContent("Random Row", "If enabled, the animated row will be chosen randomly.");
			public string[] types = new string[]
			{
				"Whole Sheet",
				"Single Row"
			};
		}
		private SerializedMinMaxCurve m_FrameOverTime;
		private SerializedProperty m_TilesX;
		private SerializedProperty m_TilesY;
		private SerializedProperty m_AnimationType;
		private SerializedProperty m_Cycles;
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
			this.m_TilesX = base.GetProperty("tilesX");
			this.m_TilesY = base.GetProperty("tilesY");
			this.m_AnimationType = base.GetProperty("animationType");
			this.m_Cycles = base.GetProperty("cycles");
			this.m_RandomRow = base.GetProperty("randomRow");
			this.m_RowIndex = base.GetProperty("rowIndex");
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
			}
			if (num == 0)
			{
				this.m_FrameOverTime.m_RemapValue = (float)(this.m_TilesX.intValue * this.m_TilesY.intValue);
			}
			ModuleUI.GUIMinMaxCurve(UVModuleUI.s_Texts.frameOverTime, this.m_FrameOverTime);
			ModuleUI.GUIFloat(UVModuleUI.s_Texts.cycles, this.m_Cycles);
		}
	}
}
