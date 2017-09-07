using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	internal class UVModuleUI : ModuleUI
	{
		private enum AnimationMode
		{
			Grid,
			Sprites
		}

		private enum AnimationType
		{
			WholeSheet,
			SingleRow
		}

		private class Texts
		{
			public GUIContent mode = EditorGUIUtility.TextContent("Mode|Animation frames can either be specified on a regular grid texture, or as a list of Sprites.");

			public GUIContent frameOverTime = EditorGUIUtility.TextContent("Frame over Time|Controls the uv animation frame of each particle over its lifetime. On the horisontal axis you will find the lifetime. On the vertical axis you will find the sheet index.");

			public GUIContent startFrame = EditorGUIUtility.TextContent("Start Frame|Phase the animation, so it starts on a frame other than 0.");

			public GUIContent tiles = EditorGUIUtility.TextContent("Tiles|Defines the tiling of the texture.");

			public GUIContent tilesX = EditorGUIUtility.TextContent("X");

			public GUIContent tilesY = EditorGUIUtility.TextContent("Y");

			public GUIContent animation = EditorGUIUtility.TextContent("Animation|Specifies the animation type: Whole Sheet or Single Row. Whole Sheet will animate over the whole texture sheet from left to right, top to bottom. Single Row will animate a single row in the sheet from left to right.");

			public GUIContent randomRow = EditorGUIUtility.TextContent("Random Row|If enabled, the animated row will be chosen randomly.");

			public GUIContent row = EditorGUIUtility.TextContent("Row|The row in the sheet which will be played.");

			public GUIContent sprites = EditorGUIUtility.TextContent("Sprites|The list of Sprites to be played.");

			public GUIContent frame = EditorGUIUtility.TextContent("Frame|The frame in the sheet which will be used.");

			public GUIContent cycles = EditorGUIUtility.TextContent("Cycles|Specifies how many times the animation will loop during the lifetime of the particle.");

			public GUIContent uvChannelMask = EditorGUIUtility.TextContent("Enabled UV Channels|Specifies which UV channels will be animated.");

			public GUIContent flipU = EditorGUIUtility.TextContent("Flip U|Cause some particle texture mapping to be flipped horizontally. (Set between 0 and 1, where a higher value causes more to flip)");

			public GUIContent flipV = EditorGUIUtility.TextContent("Flip V|Cause some particle texture mapping to be flipped vertically. (Set between 0 and 1, where a higher value causes more to flip)");

			public string[] modes = new string[]
			{
				"Grid",
				"Sprites"
			};

			public string[] types = new string[]
			{
				"Whole Sheet",
				"Single Row"
			};
		}

		private SerializedProperty m_Mode;

		private SerializedMinMaxCurve m_FrameOverTime;

		private SerializedMinMaxCurve m_StartFrame;

		private SerializedProperty m_TilesX;

		private SerializedProperty m_TilesY;

		private SerializedProperty m_AnimationType;

		private SerializedProperty m_RandomRow;

		private SerializedProperty m_RowIndex;

		private SerializedProperty m_Sprites;

		private SerializedProperty m_Cycles;

		private SerializedProperty m_UVChannelMask;

		private SerializedProperty m_FlipU;

		private SerializedProperty m_FlipV;

		private static UVModuleUI.Texts s_Texts;

		public UVModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "UVModule", displayName)
		{
			this.m_ToolTip = "Particle UV animation. This allows you to specify a texture sheet (a texture with multiple tiles/sub frames) and animate or randomize over it per particle.";
		}

		protected override void Init()
		{
			if (this.m_TilesX == null)
			{
				if (UVModuleUI.s_Texts == null)
				{
					UVModuleUI.s_Texts = new UVModuleUI.Texts();
				}
				this.m_Mode = base.GetProperty("mode");
				this.m_FrameOverTime = new SerializedMinMaxCurve(this, UVModuleUI.s_Texts.frameOverTime, "frameOverTime");
				this.m_StartFrame = new SerializedMinMaxCurve(this, UVModuleUI.s_Texts.startFrame, "startFrame");
				this.m_StartFrame.m_AllowCurves = false;
				this.m_TilesX = base.GetProperty("tilesX");
				this.m_TilesY = base.GetProperty("tilesY");
				this.m_AnimationType = base.GetProperty("animationType");
				this.m_RandomRow = base.GetProperty("randomRow");
				this.m_RowIndex = base.GetProperty("rowIndex");
				this.m_Sprites = base.GetProperty("sprites");
				this.m_Cycles = base.GetProperty("cycles");
				this.m_UVChannelMask = base.GetProperty("uvChannelMask");
				this.m_FlipU = base.GetProperty("flipU");
				this.m_FlipV = base.GetProperty("flipV");
			}
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (UVModuleUI.s_Texts == null)
			{
				UVModuleUI.s_Texts = new UVModuleUI.Texts();
			}
			int num = ModuleUI.GUIPopup(UVModuleUI.s_Texts.mode, this.m_Mode, UVModuleUI.s_Texts.modes, new GUILayoutOption[0]);
			if (!this.m_Mode.hasMultipleDifferentValues)
			{
				if (num == 0)
				{
					ModuleUI.GUIIntDraggableX2(UVModuleUI.s_Texts.tiles, UVModuleUI.s_Texts.tilesX, this.m_TilesX, UVModuleUI.s_Texts.tilesY, this.m_TilesY, new GUILayoutOption[0]);
					int num2 = ModuleUI.GUIPopup(UVModuleUI.s_Texts.animation, this.m_AnimationType, UVModuleUI.s_Texts.types, new GUILayoutOption[0]);
					if (num2 == 1)
					{
						ModuleUI.GUIToggle(UVModuleUI.s_Texts.randomRow, this.m_RandomRow, new GUILayoutOption[0]);
						if (!this.m_RandomRow.boolValue)
						{
							ModuleUI.GUIInt(UVModuleUI.s_Texts.row, this.m_RowIndex, new GUILayoutOption[0]);
						}
						this.m_FrameOverTime.m_RemapValue = (float)this.m_TilesX.intValue;
						this.m_StartFrame.m_RemapValue = (float)this.m_TilesX.intValue;
					}
					else
					{
						this.m_FrameOverTime.m_RemapValue = (float)(this.m_TilesX.intValue * this.m_TilesY.intValue);
						this.m_StartFrame.m_RemapValue = (float)(this.m_TilesX.intValue * this.m_TilesY.intValue);
					}
				}
				else
				{
					this.DoListOfSpritesGUI();
					this.ValidateSpriteList();
					this.m_FrameOverTime.m_RemapValue = (float)this.m_Sprites.arraySize;
					this.m_StartFrame.m_RemapValue = (float)this.m_Sprites.arraySize;
				}
				ModuleUI.GUIMinMaxCurve(UVModuleUI.s_Texts.frameOverTime, this.m_FrameOverTime, new GUILayoutOption[0]);
				ModuleUI.GUIMinMaxCurve(UVModuleUI.s_Texts.startFrame, this.m_StartFrame, new GUILayoutOption[0]);
			}
			ModuleUI.GUIFloat(UVModuleUI.s_Texts.cycles, this.m_Cycles, new GUILayoutOption[0]);
			bool disabled = false;
			ParticleSystem[] particleSystems = this.m_ParticleSystemUI.m_ParticleSystems;
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem particleSystem = particleSystems[i];
				ParticleSystemRenderer component = particleSystem.GetComponent<ParticleSystemRenderer>();
				if (component != null && component.renderMode == ParticleSystemRenderMode.Mesh)
				{
					disabled = true;
					break;
				}
			}
			using (new EditorGUI.DisabledScope(disabled))
			{
				ModuleUI.GUIFloat(UVModuleUI.s_Texts.flipU, this.m_FlipU, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(UVModuleUI.s_Texts.flipV, this.m_FlipV, new GUILayoutOption[0]);
			}
			this.m_UVChannelMask.intValue = (int)((UVChannelFlags)ModuleUI.GUIEnumMask(UVModuleUI.s_Texts.uvChannelMask, (UVChannelFlags)this.m_UVChannelMask.intValue, new GUILayoutOption[0]));
		}

		private void DoListOfSpritesGUI()
		{
			for (int i = 0; i < this.m_Sprites.arraySize; i++)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				SerializedProperty arrayElementAtIndex = this.m_Sprites.GetArrayElementAtIndex(i);
				SerializedProperty objectProp = arrayElementAtIndex.FindPropertyRelative("sprite");
				ModuleUI.GUIObject(new GUIContent(" "), objectProp, typeof(Sprite), new GUILayoutOption[0]);
				if (i == 0)
				{
					if (GUILayout.Button(GUIContent.none, new GUIStyle("OL Plus"), new GUILayoutOption[]
					{
						GUILayout.Width(16f)
					}))
					{
						this.m_Sprites.InsertArrayElementAtIndex(this.m_Sprites.arraySize);
						SerializedProperty arrayElementAtIndex2 = this.m_Sprites.GetArrayElementAtIndex(this.m_Sprites.arraySize - 1);
						SerializedProperty serializedProperty = arrayElementAtIndex2.FindPropertyRelative("sprite");
						serializedProperty.objectReferenceValue = null;
					}
				}
				else if (GUILayout.Button(GUIContent.none, new GUIStyle("OL Minus"), new GUILayoutOption[]
				{
					GUILayout.Width(16f)
				}))
				{
					this.m_Sprites.DeleteArrayElementAtIndex(i);
				}
				GUILayout.EndHorizontal();
			}
		}

		private void ValidateSpriteList()
		{
			if (this.m_Sprites.arraySize > 1)
			{
				Texture x = null;
				for (int i = 0; i < this.m_Sprites.arraySize; i++)
				{
					SerializedProperty arrayElementAtIndex = this.m_Sprites.GetArrayElementAtIndex(i);
					SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("sprite");
					Sprite sprite = serializedProperty.objectReferenceValue as Sprite;
					if (sprite != null)
					{
						if (x == null)
						{
							x = sprite.GetTextureForPlayMode();
						}
						else
						{
							if (x != sprite.GetTextureForPlayMode())
							{
								EditorGUILayout.HelpBox("All Sprites must share the same texture. Either pack all Sprites into one Texture by setting the Packing Tag, or use a Multiple Mode Sprite.", MessageType.Error, true);
								break;
							}
							if (sprite.border != Vector4.zero)
							{
								EditorGUILayout.HelpBox("Sprite borders are not supported. They will be ignored.", MessageType.Warning, true);
								break;
							}
						}
					}
				}
			}
		}
	}
}
