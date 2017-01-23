using System;
using UnityEngine;

namespace UnityEditor
{
	internal class LookDevViewsWindow : PopupWindowContent
	{
		public class Styles
		{
			public readonly GUIStyle sMenuItem = "MenuItem";

			public readonly GUIStyle sHeaderStyle = EditorStyles.miniLabel;

			public readonly GUIStyle sToolBarButton = "toolbarbutton";

			public readonly GUIContent sTitle = EditorGUIUtility.TextContent("Views");

			public readonly GUIContent sExposure = EditorGUIUtility.TextContent("EV|Exposure value: control the brightness of the environment.");

			public readonly GUIContent sEnvironment = EditorGUIUtility.TextContent("Environment|Select an environment from the list of currently available environments");

			public readonly GUIContent sRotation = EditorGUIUtility.TextContent("Rotation|Change the rotation of the environment");

			public readonly GUIContent sZero = EditorGUIUtility.TextContent("0");

			public readonly GUIContent sLoD = EditorGUIUtility.TextContent("LoD|Choose displayed LoD");

			public readonly GUIContent sLoDAuto = EditorGUIUtility.TextContent("LoD (auto)|Choose displayed LoD");

			public readonly GUIContent sShadingMode = EditorGUIUtility.TextContent("Shading|Select shading mode");

			public readonly GUIContent[] sViewTitle = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Main View (1)"),
				EditorGUIUtility.TextContent("Second View (2)")
			};

			public readonly GUIStyle[] sViewTitleStyles = new GUIStyle[]
			{
				new GUIStyle(EditorStyles.miniLabel),
				new GUIStyle(EditorStyles.miniLabel)
			};

			public readonly string[] sShadingModeStrings = new string[]
			{
				"Shaded",
				"Shaded Wireframe",
				"Albedo",
				"Specular",
				"Smoothness",
				"Normal"
			};

			public readonly int[] sShadingModeValues = new int[]
			{
				-1,
				2,
				8,
				9,
				10,
				11
			};

			public readonly GUIContent sLinkActive = EditorGUIUtility.IconContent("LookDevMirrorViewsActive", "Link|Links the property between the different views");

			public readonly GUIContent sLinkInactive = EditorGUIUtility.IconContent("LookDevMirrorViewsInactive", "Link|Links the property between the different views");

			public Styles()
			{
				this.sViewTitleStyles[0].normal.textColor = LookDevView.m_FirstViewGizmoColor;
				this.sViewTitleStyles[1].normal.textColor = LookDevView.m_SecondViewGizmoColor;
			}
		}

		private static LookDevViewsWindow.Styles s_Styles = new LookDevViewsWindow.Styles();

		private static float kIconSize = 32f;

		private static float kLabelWidth = 120f;

		private static float kSliderWidth = 100f;

		private static float kSliderFieldWidth = 30f;

		private static float kSliderFieldPadding = 5f;

		private static float kLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		private float m_WindowHeight = 5f * LookDevViewsWindow.kLineHeight + EditorGUIUtility.standardVerticalSpacing;

		private float m_WindowWidth = LookDevViewsWindow.kLabelWidth + LookDevViewsWindow.kSliderWidth + LookDevViewsWindow.kSliderFieldWidth + LookDevViewsWindow.kSliderFieldPadding + 5f;

		private readonly LookDevView m_LookDevView;

		public static LookDevViewsWindow.Styles styles
		{
			get
			{
				return LookDevViewsWindow.s_Styles;
			}
		}

		public LookDevViewsWindow(LookDevView lookDevView)
		{
			this.m_LookDevView = lookDevView;
		}

		private GUIContent GetGUIContentLink(bool active)
		{
			return (!active) ? LookDevViewsWindow.styles.sLinkInactive : LookDevViewsWindow.styles.sLinkActive;
		}

		private bool NeedLoD()
		{
			return this.m_LookDevView.config.GetObjectLoDCount(LookDevEditionContext.Left) > 1 || this.m_LookDevView.config.GetObjectLoDCount(LookDevEditionContext.Right) > 1;
		}

		private float GetHeight()
		{
			float num = this.m_WindowHeight;
			if (this.NeedLoD())
			{
				num += LookDevViewsWindow.kLineHeight;
			}
			return num;
		}

		public override Vector2 GetWindowSize()
		{
			float x = this.m_WindowWidth + ((this.m_LookDevView.config.lookDevMode != LookDevMode.Single1 && this.m_LookDevView.config.lookDevMode != LookDevMode.Single2) ? (this.m_WindowWidth + LookDevViewsWindow.kIconSize) : 0f);
			return new Vector2(x, this.GetHeight());
		}

		public override void OnGUI(Rect rect)
		{
			if (!(this.m_LookDevView.config == null))
			{
				Rect drawPos = new Rect(0f, 0f, rect.width, this.GetHeight());
				this.DrawOneView(drawPos, (this.m_LookDevView.config.lookDevMode != LookDevMode.Single2) ? LookDevEditionContext.Left : LookDevEditionContext.Right);
				drawPos.x += this.m_WindowWidth;
				drawPos.x += LookDevViewsWindow.kIconSize;
				this.DrawOneView(drawPos, LookDevEditionContext.Right);
				if (Event.current.type == EventType.MouseMove)
				{
					Event.current.Use();
				}
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
				{
					base.editorWindow.Close();
					GUIUtility.ExitGUI();
				}
			}
		}

		private void DrawOneView(Rect drawPos, LookDevEditionContext context)
		{
			bool flag = (this.m_LookDevView.config.lookDevMode != LookDevMode.Single1 && context == LookDevEditionContext.Left) || (this.m_LookDevView.config.lookDevMode != LookDevMode.Single2 && context == LookDevEditionContext.Right);
			GUILayout.BeginArea(drawPos);
			GUILayout.Label(LookDevViewsWindow.styles.sViewTitle[(int)context], LookDevViewsWindow.styles.sViewTitleStyles[(int)context], new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(this.m_WindowWidth)
			});
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(LookDevViewsWindow.kLineHeight)
			});
			GUILayout.Label(LookDevViewsWindow.styles.sExposure, LookDevViewsWindow.styles.sMenuItem, new GUILayoutOption[]
			{
				GUILayout.Width(LookDevViewsWindow.kLabelWidth)
			});
			float num = this.m_LookDevView.config.GetFloatProperty(LookDevProperty.ExposureValue, context);
			EditorGUI.BeginChangeCheck();
			float num2 = Mathf.Round(this.m_LookDevView.config.exposureRange);
			num = Mathf.Clamp(GUILayout.HorizontalSlider(num, -num2, num2, new GUILayoutOption[]
			{
				GUILayout.Width(LookDevViewsWindow.kSliderWidth)
			}), -num2, num2);
			num = Mathf.Clamp(EditorGUILayout.FloatField((float)Math.Round((double)num, (num >= 0f) ? 2 : 1), new GUILayoutOption[]
			{
				GUILayout.Width(LookDevViewsWindow.kSliderFieldWidth)
			}), -num2, num2);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_LookDevView.config.UpdateFocus(context);
				this.m_LookDevView.config.UpdateFloatProperty(LookDevProperty.ExposureValue, num);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(LookDevViewsWindow.kLineHeight)
			});
			int hdriCount = this.m_LookDevView.envLibrary.hdriCount;
			using (new EditorGUI.DisabledScope(hdriCount <= 1))
			{
				GUILayout.Label(LookDevViewsWindow.styles.sEnvironment, LookDevViewsWindow.styles.sMenuItem, new GUILayoutOption[]
				{
					GUILayout.Width(LookDevViewsWindow.kLabelWidth)
				});
				if (hdriCount > 1)
				{
					int num3 = hdriCount - 1;
					int num4 = this.m_LookDevView.config.GetIntProperty(LookDevProperty.HDRI, context);
					EditorGUI.BeginChangeCheck();
					num4 = (int)GUILayout.HorizontalSlider((float)num4, 0f, (float)num3, new GUILayoutOption[]
					{
						GUILayout.Width(LookDevViewsWindow.kSliderWidth)
					});
					num4 = Mathf.Clamp(EditorGUILayout.IntField(num4, new GUILayoutOption[]
					{
						GUILayout.Width(LookDevViewsWindow.kSliderFieldWidth)
					}), 0, num3);
					if (EditorGUI.EndChangeCheck())
					{
						this.m_LookDevView.config.UpdateFocus(context);
						this.m_LookDevView.config.UpdateIntProperty(LookDevProperty.HDRI, num4);
					}
				}
				else
				{
					GUILayout.HorizontalSlider(0f, 0f, 0f, new GUILayoutOption[]
					{
						GUILayout.Width(LookDevViewsWindow.kSliderWidth)
					});
					GUILayout.Label(LookDevViewsWindow.styles.sZero, LookDevViewsWindow.styles.sMenuItem, new GUILayoutOption[0]);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(LookDevViewsWindow.kLineHeight)
			});
			GUILayout.Label(LookDevViewsWindow.styles.sShadingMode, LookDevViewsWindow.styles.sMenuItem, new GUILayoutOption[]
			{
				GUILayout.Width(LookDevViewsWindow.kLabelWidth)
			});
			int num5 = this.m_LookDevView.config.GetIntProperty(LookDevProperty.ShadingMode, context);
			EditorGUI.BeginChangeCheck();
			num5 = EditorGUILayout.IntPopup("", num5, LookDevViewsWindow.styles.sShadingModeStrings, LookDevViewsWindow.styles.sShadingModeValues, new GUILayoutOption[]
			{
				GUILayout.Width(LookDevViewsWindow.kSliderFieldWidth + LookDevViewsWindow.kSliderWidth + 4f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				this.m_LookDevView.config.UpdateFocus(context);
				this.m_LookDevView.config.UpdateIntProperty(LookDevProperty.ShadingMode, num5);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(LookDevViewsWindow.kLineHeight)
			});
			GUILayout.Label(LookDevViewsWindow.styles.sRotation, LookDevViewsWindow.styles.sMenuItem, new GUILayoutOption[]
			{
				GUILayout.Width(LookDevViewsWindow.kLabelWidth)
			});
			float num6 = this.m_LookDevView.config.GetFloatProperty(LookDevProperty.EnvRotation, context);
			EditorGUI.BeginChangeCheck();
			num6 = GUILayout.HorizontalSlider(num6, 0f, 720f, new GUILayoutOption[]
			{
				GUILayout.Width(LookDevViewsWindow.kSliderWidth)
			});
			num6 = Mathf.Clamp(EditorGUILayout.FloatField((float)Math.Round((double)num6, 0), new GUILayoutOption[]
			{
				GUILayout.Width(LookDevViewsWindow.kSliderFieldWidth)
			}), 0f, 720f);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_LookDevView.config.UpdateFocus(context);
				this.m_LookDevView.config.UpdateFloatProperty(LookDevProperty.EnvRotation, num6);
			}
			GUILayout.EndHorizontal();
			if (this.NeedLoD())
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Height(LookDevViewsWindow.kLineHeight)
				});
				if (this.m_LookDevView.config.GetObjectLoDCount(context) > 1)
				{
					int num7 = this.m_LookDevView.config.GetIntProperty(LookDevProperty.LoDIndex, context);
					GUILayout.Label((num7 != -1) ? LookDevViewsWindow.styles.sLoD : LookDevViewsWindow.styles.sLoDAuto, LookDevViewsWindow.styles.sMenuItem, new GUILayoutOption[]
					{
						GUILayout.Width(LookDevViewsWindow.kLabelWidth)
					});
					EditorGUI.BeginChangeCheck();
					int num8 = this.m_LookDevView.config.GetObjectLoDCount(context) - 1;
					if (this.m_LookDevView.config.lookDevMode != LookDevMode.Single1 && this.m_LookDevView.config.lookDevMode != LookDevMode.Single2 && this.m_LookDevView.config.IsPropertyLinked(LookDevProperty.LoDIndex))
					{
						num8 = Math.Min(this.m_LookDevView.config.GetObjectLoDCount(LookDevEditionContext.Left), this.m_LookDevView.config.GetObjectLoDCount(LookDevEditionContext.Right)) - 1;
					}
					num7 = Mathf.Clamp(num7, -1, num8);
					num7 = (int)GUILayout.HorizontalSlider((float)num7, -1f, (float)num8, new GUILayoutOption[]
					{
						GUILayout.Width(LookDevViewsWindow.kSliderWidth)
					});
					num7 = EditorGUILayout.IntField(num7, new GUILayoutOption[]
					{
						GUILayout.Width(LookDevViewsWindow.kSliderFieldWidth)
					});
					if (EditorGUI.EndChangeCheck())
					{
						this.m_LookDevView.config.UpdateFocus(context);
						this.m_LookDevView.config.UpdateIntProperty(LookDevProperty.LoDIndex, num7);
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			if (flag)
			{
				GUILayout.BeginVertical(new GUILayoutOption[]
				{
					GUILayout.Width(LookDevViewsWindow.kIconSize)
				});
				LookDevProperty[] array = new LookDevProperty[]
				{
					LookDevProperty.ExposureValue,
					LookDevProperty.HDRI,
					LookDevProperty.ShadingMode,
					LookDevProperty.EnvRotation,
					LookDevProperty.LoDIndex
				};
				int num9 = 4 + ((!this.NeedLoD()) ? 0 : 1);
				for (int i = 0; i < num9; i++)
				{
					EditorGUI.BeginChangeCheck();
					bool flag2 = this.m_LookDevView.config.IsPropertyLinked(array[i]);
					bool value = GUILayout.Toggle(flag2, this.GetGUIContentLink(flag2), LookDevViewsWindow.styles.sToolBarButton, new GUILayoutOption[]
					{
						GUILayout.Height(LookDevViewsWindow.kLineHeight)
					});
					if (EditorGUI.EndChangeCheck())
					{
						this.m_LookDevView.config.UpdatePropertyLink(array[i], value);
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
}
