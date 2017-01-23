using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityEditor
{
	internal class LookDevEnvironmentWindow
	{
		internal class EnvSettingsWindow : PopupWindowContent
		{
			public class Styles
			{
				public readonly GUIStyle sMenuItem = "MenuItem";

				public readonly GUIStyle sSeparator = "sv_iconselector_sep";

				public readonly GUIContent sEnvironment = EditorGUIUtility.TextContent("Environment");

				public readonly GUIContent sAngleOffset = EditorGUIUtility.TextContent("Angle offset|Rotate the environment");

				public readonly GUIContent sResetEnv = EditorGUIUtility.TextContent("Reset Environment|Reset environment settings");

				public readonly GUIContent sShadows = EditorGUIUtility.TextContent("Shadows");

				public readonly GUIContent sShadowIntensity = EditorGUIUtility.TextContent("Shadow brightness|Shadow brightness");

				public readonly GUIContent sShadowColor = EditorGUIUtility.TextContent("Color|Shadow color");

				public readonly GUIContent sBrightest = EditorGUIUtility.TextContent("Set position to brightest point|Set the shadow direction to the brightest (higher value) point of the latLong map");

				public readonly GUIContent sResetShadow = EditorGUIUtility.TextContent("Reset Shadows|Reset shadow properties");
			}

			private static LookDevEnvironmentWindow.EnvSettingsWindow.Styles s_Styles = null;

			private CubemapInfo m_CubemapInfo;

			private LookDevView m_LookDevView;

			private float kShadowSettingWidth = 200f;

			private float kShadowSettingHeight = 157f;

			public static LookDevEnvironmentWindow.EnvSettingsWindow.Styles styles
			{
				get
				{
					if (LookDevEnvironmentWindow.EnvSettingsWindow.s_Styles == null)
					{
						LookDevEnvironmentWindow.EnvSettingsWindow.s_Styles = new LookDevEnvironmentWindow.EnvSettingsWindow.Styles();
					}
					return LookDevEnvironmentWindow.EnvSettingsWindow.s_Styles;
				}
			}

			public EnvSettingsWindow(LookDevView lookDevView, CubemapInfo infos)
			{
				this.m_LookDevView = lookDevView;
				this.m_CubemapInfo = infos;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(this.kShadowSettingWidth, this.kShadowSettingHeight);
			}

			private void DrawSeparator()
			{
				GUILayout.Space(3f);
				GUILayout.Label(GUIContent.none, LookDevEnvironmentWindow.EnvSettingsWindow.styles.sSeparator, new GUILayoutOption[0]);
			}

			public override void OnGUI(Rect rect)
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Label(LookDevEnvironmentWindow.EnvSettingsWindow.styles.sEnvironment, EditorStyles.miniLabel, new GUILayoutOption[0]);
				EditorGUI.BeginChangeCheck();
				float angleOffset = EditorGUILayout.Slider(LookDevEnvironmentWindow.EnvSettingsWindow.styles.sAngleOffset, this.m_CubemapInfo.angleOffset % 360f, -360f, 360f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed environment settings");
					this.m_CubemapInfo.angleOffset = angleOffset;
					this.m_LookDevView.envLibrary.dirty = true;
					this.m_LookDevView.Repaint();
				}
				if (GUILayout.Button(LookDevEnvironmentWindow.EnvSettingsWindow.styles.sResetEnv, EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed environment settings");
					this.m_CubemapInfo.ResetEnvInfos();
					this.m_LookDevView.envLibrary.dirty = true;
					this.m_LookDevView.Repaint();
				}
				using (new EditorGUI.DisabledScope(!this.m_LookDevView.config.enableShadowCubemap))
				{
					this.DrawSeparator();
					GUILayout.Label(LookDevEnvironmentWindow.EnvSettingsWindow.styles.sShadows, EditorStyles.miniLabel, new GUILayoutOption[0]);
					EditorGUI.BeginChangeCheck();
					float shadowIntensity = EditorGUILayout.Slider(LookDevEnvironmentWindow.EnvSettingsWindow.styles.sShadowIntensity, this.m_CubemapInfo.shadowInfo.shadowIntensity, 0f, 5f, new GUILayoutOption[0]);
					Color shadowColor = EditorGUILayout.ColorField(LookDevEnvironmentWindow.EnvSettingsWindow.styles.sShadowColor, this.m_CubemapInfo.shadowInfo.shadowColor, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed shadow settings");
						this.m_CubemapInfo.shadowInfo.shadowIntensity = shadowIntensity;
						this.m_CubemapInfo.shadowInfo.shadowColor = shadowColor;
						this.m_LookDevView.envLibrary.dirty = true;
						this.m_LookDevView.Repaint();
					}
					if (GUILayout.Button(LookDevEnvironmentWindow.EnvSettingsWindow.styles.sBrightest, EditorStyles.toolbarButton, new GUILayoutOption[0]))
					{
						Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed shadow settings");
						LookDevResources.UpdateShadowInfoWithBrightestSpot(this.m_CubemapInfo);
						this.m_LookDevView.envLibrary.dirty = true;
						this.m_LookDevView.Repaint();
					}
					if (GUILayout.Button(LookDevEnvironmentWindow.EnvSettingsWindow.styles.sResetShadow, EditorStyles.toolbarButton, new GUILayoutOption[0]))
					{
						Undo.RecordObject(this.m_LookDevView.envLibrary, "Changed shadow settings");
						this.m_CubemapInfo.SetCubemapShadowInfo(this.m_CubemapInfo);
						this.m_LookDevView.envLibrary.dirty = true;
						this.m_LookDevView.Repaint();
					}
				}
				GUILayout.EndVertical();
			}
		}

		public class Styles
		{
			public readonly GUIContent sTitle = EditorGUIUtility.TextContent("HDRI View|Manage your list of HDRI environments.");

			public readonly GUIContent sCloseIcon = new GUIContent(EditorGUIUtility.IconContent("LookDevClose"));

			public readonly GUIStyle sSeparatorStyle = "sv_iconselector_sep";

			public readonly GUIStyle sLabelStyleFirstContext = new GUIStyle(EditorStyles.miniLabel);

			public readonly GUIStyle sLabelStyleSecondContext = new GUIStyle(EditorStyles.miniLabel);

			public readonly GUIStyle sLabelStyleBothContext = new GUIStyle(EditorStyles.miniLabel);

			public readonly Texture sLightTexture = EditorGUIUtility.FindTexture("LookDevLight");

			public readonly Texture sLatlongFrameTexture = EditorGUIUtility.FindTexture("LookDevShadowFrame");

			public readonly GUIContent sEnvControlIcon = new GUIContent(EditorGUIUtility.IconContent("LookDevPaneOption"));

			public readonly GUIContent sDragAndDropHDRIText = EditorGUIUtility.TextContent("Drag and drop HDR panorama here.");

			public Styles()
			{
				this.sLabelStyleFirstContext.normal.textColor = LookDevView.m_FirstViewGizmoColor;
				this.sLabelStyleSecondContext.normal.textColor = LookDevView.m_SecondViewGizmoColor;
				this.sLabelStyleBothContext.normal.textColor = ((!EditorGUIUtility.isProSkin) ? Color.black : Color.white);
				this.sEnvControlIcon.tooltip = "Environment parameters";
			}
		}

		private static LookDevEnvironmentWindow.Styles s_Styles = new LookDevEnvironmentWindow.Styles();

		private LookDevView m_LookDevView;

		private Vector2 m_ScrollPosition = new Vector2(0f, 0f);

		private Rect m_PositionInLookDev;

		private Cubemap m_SelectedCubemap = null;

		private CubemapInfo m_SelectedCubemapInfo = null;

		private CubemapInfo m_SelectedShadowCubemapOwnerInfo = null;

		private int m_SelectedLightIconIndex = -1;

		private ShadowInfo m_SelectedShadowInfo = null;

		private bool m_RenderOverlayThumbnailOnce = false;

		private int m_SelectedCubeMapOffsetIndex = -1;

		private int m_HoveringCubeMapIndex = -1;

		private float m_SelectedCubeMapOffsetValue = 0f;

		private Vector2 m_SelectedPositionOffset = new Vector2(0f, 0f);

		private Rect m_GUIRect;

		private Rect m_displayRect;

		private bool m_DragBeingPerformed = false;

		public const float m_latLongHeight = 125f;

		public const float m_HDRIHeaderHeight = 18f;

		public const float m_HDRIHeight = 146f;

		public const float m_HDRIWidth = 250f;

		private const float kButtonWidth = 16f;

		private const float kButtonHeight = 16f;

		public static LookDevEnvironmentWindow.Styles styles
		{
			get
			{
				if (LookDevEnvironmentWindow.s_Styles == null)
				{
					LookDevEnvironmentWindow.s_Styles = new LookDevEnvironmentWindow.Styles();
				}
				return LookDevEnvironmentWindow.s_Styles;
			}
		}

		public LookDevEnvironmentWindow(LookDevView lookDevView)
		{
			this.m_LookDevView = lookDevView;
		}

		public void SetRects(Rect positionInLookDev, Rect GUIRect, Rect displayRect)
		{
			this.m_PositionInLookDev = positionInLookDev;
			this.m_GUIRect = GUIRect;
			this.m_displayRect = displayRect;
		}

		public Cubemap GetCurrentSelection()
		{
			return this.m_SelectedCubemap;
		}

		public Vector2 GetSelectedPositionOffset()
		{
			return this.m_SelectedPositionOffset;
		}

		public void CancelSelection()
		{
			this.m_SelectedCubemap = null;
			this.m_SelectedCubemapInfo = null;
			this.m_SelectedShadowCubemapOwnerInfo = null;
			this.m_SelectedLightIconIndex = -1;
			this.m_SelectedShadowInfo = null;
			this.m_HoveringCubeMapIndex = -1;
			this.m_DragBeingPerformed = false;
		}

		private float ComputeAngleOffsetFromMouseCoord(Vector2 mousePosition)
		{
			return mousePosition.x / 250f * 360f;
		}

		private Vector2 LatLongToPosition(float latitude, float longitude)
		{
			latitude = (latitude + 90f) % 180f - 90f;
			if (latitude < -90f)
			{
				latitude = -90f;
			}
			if (latitude > 89f)
			{
				latitude = 89f;
			}
			longitude %= 360f;
			if ((double)longitude < 0.0)
			{
				longitude = 360f + longitude;
			}
			Vector2 result = new Vector2(longitude * 0.0174532924f / 6.28318548f * 2f - 1f, latitude * 0.0174532924f / 1.57079637f);
			return result;
		}

		public static Vector2 PositionToLatLong(Vector2 position)
		{
			Vector2 result = default(Vector2);
			result.x = position.y * 3.14159274f * 0.5f * 57.29578f;
			result.y = (position.x * 0.5f + 0.5f) * 2f * 3.14159274f * 57.29578f;
			if (result.x < -90f)
			{
				result.x = -90f;
			}
			if (result.x > 89f)
			{
				result.x = 89f;
			}
			return result;
		}

		private Rect GetInsertionRect(int envIndex)
		{
			Rect gUIRect = this.m_GUIRect;
			gUIRect.height = 21f;
			gUIRect.y = 146f * (float)envIndex;
			return gUIRect;
		}

		private int IsPositionInInsertionArea(Vector2 pos)
		{
			int result;
			for (int i = 0; i <= this.m_LookDevView.envLibrary.hdriCount; i++)
			{
				if (this.GetInsertionRect(i).Contains(pos))
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		private Rect GetThumbnailRect(int envIndex)
		{
			Rect gUIRect = this.m_GUIRect;
			gUIRect.height = 125f;
			gUIRect.y = (float)envIndex * 146f + 18f;
			return gUIRect;
		}

		private int IsPositionInThumbnailArea(Vector2 pos)
		{
			int result;
			for (int i = 0; i < this.m_LookDevView.envLibrary.hdriCount; i++)
			{
				if (this.GetThumbnailRect(i).Contains(pos))
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		private void RenderOverlayThumbnailIfNeeded()
		{
			if (this.m_RenderOverlayThumbnailOnce && Event.current.type == EventType.Repaint && this.m_SelectedCubemapInfo != null)
			{
				this.m_SelectedCubemap = this.m_SelectedCubemapInfo.cubemap;
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = LookDevResources.m_SelectionTexture;
				LookDevResources.m_LookDevCubeToLatlong.SetTexture("_MainTex", this.m_SelectedCubemap);
				LookDevResources.m_LookDevCubeToLatlong.SetVector("_WindowParams", new Vector4(this.m_displayRect.height, -1000f, 2f, 1f));
				LookDevResources.m_LookDevCubeToLatlong.SetVector("_CubeToLatLongParams", new Vector4(0.0174532924f * this.m_SelectedCubemapInfo.angleOffset, 0.5f, 1f, 0f));
				LookDevResources.m_LookDevCubeToLatlong.SetPass(0);
				GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
				LookDevView.DrawFullScreenQuad(new Rect(0f, 0f, 250f, 125f));
				GL.sRGBWrite = false;
				RenderTexture.active = active;
				this.m_RenderOverlayThumbnailOnce = false;
			}
		}

		private void DrawLatLongThumbnail(CubemapInfo infos, float angleOffset, float intensity, float alpha, Rect textureRect)
		{
			GUIStyle gUIStyle = "dockarea";
			LookDevResources.m_LookDevCubeToLatlong.SetVector("_WindowParams", new Vector4(this.m_displayRect.height, this.m_PositionInLookDev.y + 17f, (float)gUIStyle.margin.top, EditorGUIUtility.pixelsPerPoint));
			LookDevResources.m_LookDevCubeToLatlong.SetVector("_CubeToLatLongParams", new Vector4(0.0174532924f * angleOffset, alpha, intensity, 0f));
			GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			Graphics.DrawTexture(textureRect, infos.cubemap, LookDevResources.m_LookDevCubeToLatlong, 1);
			GL.sRGBWrite = false;
		}

		private void DrawSelectionFeedback(Rect textureRect, Color selectionColor1, Color selectionColor2)
		{
			float x = 0.5f;
			float x2 = textureRect.width - 0.5f;
			float y = textureRect.y + 0.5f;
			float y2 = textureRect.y + textureRect.height - 1f;
			float x3 = textureRect.width * 0.5f;
			Vector3[] lineSegments = new Vector3[]
			{
				new Vector3(x3, y, 0f),
				new Vector3(x, y, 0f),
				new Vector3(x, y, 0f),
				new Vector3(x, y2, 0f),
				new Vector3(x, y2, 0f),
				new Vector3(x3, y2, 0f)
			};
			Vector3[] lineSegments2 = new Vector3[]
			{
				new Vector3(x3, y, 0f),
				new Vector3(x2, y, 0f),
				new Vector3(x2, y, 0f),
				new Vector3(x2, y2, 0f),
				new Vector3(x2, y2, 0f),
				new Vector3(x3, y2, 0f)
			};
			Handles.color = selectionColor1;
			Handles.DrawLines(lineSegments);
			Handles.color = selectionColor2;
			Handles.DrawLines(lineSegments2);
		}

		public void ResetShadowCubemap()
		{
			if (this.m_SelectedShadowCubemapOwnerInfo != null)
			{
				this.m_SelectedShadowCubemapOwnerInfo.SetCubemapShadowInfo(this.m_SelectedShadowCubemapOwnerInfo);
			}
		}

		private void HandleMouseInput()
		{
			List<CubemapInfo> hdriList = this.m_LookDevView.envLibrary.hdriList;
			Vector2 vector = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y + this.m_ScrollPosition.y);
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(this.m_LookDevView.hotControl);
			switch (typeForControl)
			{
			case EventType.MouseUp:
				if (this.m_SelectedCubemap != null)
				{
					Rect gUIRect = this.m_GUIRect;
					gUIRect.yMax += 16f;
					if (gUIRect.Contains(Event.current.mousePosition))
					{
						int num = this.IsPositionInInsertionArea(vector);
						if (num != -1)
						{
							this.ResetShadowCubemap();
							this.m_LookDevView.envLibrary.InsertHDRI(this.m_SelectedCubemap, num);
						}
						else
						{
							int num2 = this.IsPositionInThumbnailArea(vector);
							if (num2 != -1 && this.m_LookDevView.config.enableShadowCubemap)
							{
								Undo.RecordObject(this.m_LookDevView.envLibrary, "Update shadow cubemap");
								CubemapInfo cubemapInfo = this.m_LookDevView.envLibrary.hdriList[num2];
								if (cubemapInfo != this.m_SelectedCubemapInfo)
								{
									cubemapInfo.SetCubemapShadowInfo(this.m_SelectedCubemapInfo);
								}
								this.m_LookDevView.envLibrary.dirty = true;
							}
						}
						this.CancelSelection();
					}
				}
				this.m_LookDevView.Repaint();
				if (this.m_SelectedCubeMapOffsetIndex != -1)
				{
					if (Mathf.Abs(hdriList[this.m_SelectedCubeMapOffsetIndex].angleOffset) <= 10f)
					{
						Undo.RecordObject(this.m_LookDevView.envLibrary, "");
						hdriList[this.m_SelectedCubeMapOffsetIndex].angleOffset = 0f;
						this.m_LookDevView.envLibrary.dirty = true;
					}
				}
				this.m_SelectedCubemapInfo = null;
				this.m_SelectedShadowCubemapOwnerInfo = null;
				this.m_SelectedLightIconIndex = -1;
				this.m_SelectedShadowInfo = null;
				this.m_SelectedCubeMapOffsetIndex = -1;
				this.m_HoveringCubeMapIndex = -1;
				this.m_SelectedCubeMapOffsetValue = 0f;
				GUIUtility.hotControl = 0;
				return;
			case EventType.MouseMove:
			case EventType.KeyUp:
			case EventType.ScrollWheel:
			case EventType.Layout:
				IL_95:
				if (typeForControl != EventType.DragExited)
				{
					return;
				}
				return;
			case EventType.MouseDrag:
				if (this.m_SelectedCubeMapOffsetIndex != -1)
				{
					Undo.RecordObject(this.m_LookDevView.envLibrary, "");
					CubemapInfo cubemapInfo2 = hdriList[this.m_SelectedCubeMapOffsetIndex];
					cubemapInfo2.angleOffset = this.ComputeAngleOffsetFromMouseCoord(vector) + this.m_SelectedCubeMapOffsetValue;
					this.m_LookDevView.envLibrary.dirty = true;
					Event.current.Use();
				}
				if (this.m_SelectedCubemapInfo != null)
				{
					if (this.IsPositionInInsertionArea(vector) == -1)
					{
						this.m_HoveringCubeMapIndex = this.IsPositionInThumbnailArea(vector);
					}
					else
					{
						this.m_HoveringCubeMapIndex = -1;
					}
				}
				this.m_LookDevView.Repaint();
				return;
			case EventType.KeyDown:
				if (Event.current.keyCode == KeyCode.Escape)
				{
					this.CancelSelection();
					this.m_LookDevView.Repaint();
				}
				return;
			case EventType.Repaint:
				if (this.m_SelectedCubeMapOffsetIndex != -1)
				{
					EditorGUIUtility.AddCursorRect(this.m_displayRect, MouseCursor.SlideArrow);
				}
				return;
			case EventType.DragUpdated:
			{
				bool flag = false;
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				for (int i = 0; i < objectReferences.Length; i++)
				{
					UnityEngine.Object @object = objectReferences[i];
					Cubemap exists = @object as Cubemap;
					if (exists)
					{
						flag = true;
					}
				}
				DragAndDrop.visualMode = ((!flag) ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Link);
				if (flag)
				{
					this.m_DragBeingPerformed = true;
				}
				current.Use();
				return;
			}
			case EventType.DragPerform:
			{
				int insertionIndex = this.IsPositionInInsertionArea(vector);
				UnityEngine.Object[] objectReferences2 = DragAndDrop.objectReferences;
				for (int j = 0; j < objectReferences2.Length; j++)
				{
					UnityEngine.Object object2 = objectReferences2[j];
					Cubemap cubemap = object2 as Cubemap;
					if (cubemap)
					{
						this.m_LookDevView.envLibrary.InsertHDRI(cubemap, insertionIndex);
					}
				}
				DragAndDrop.AcceptDrag();
				this.m_DragBeingPerformed = false;
				current.Use();
				return;
			}
			}
			goto IL_95;
		}

		private void GetFrameAndShadowTextureRect(Rect textureRect, out Rect frameTextureRect, out Rect shadowTextureRect)
		{
			frameTextureRect = textureRect;
			frameTextureRect.x += textureRect.width - (float)LookDevEnvironmentWindow.styles.sLatlongFrameTexture.width;
			frameTextureRect.y += textureRect.height - (float)LookDevEnvironmentWindow.styles.sLatlongFrameTexture.height * 1.05f;
			frameTextureRect.width = (float)LookDevEnvironmentWindow.styles.sLatlongFrameTexture.width;
			frameTextureRect.height = (float)LookDevEnvironmentWindow.styles.sLatlongFrameTexture.height;
			shadowTextureRect = frameTextureRect;
			shadowTextureRect.x += 6f;
			shadowTextureRect.y += 4f;
			shadowTextureRect.width = 105f;
			shadowTextureRect.height = 52f;
		}

		public void OnGUI(int windowID)
		{
			if (!(this.m_LookDevView == null))
			{
				List<CubemapInfo> hdriList = this.m_LookDevView.envLibrary.hdriList;
				bool flag = 146f * (float)hdriList.Count > this.m_PositionInLookDev.height;
				if (flag)
				{
					this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
				}
				else
				{
					this.m_ScrollPosition = new Vector2(0f, 0f);
				}
				if (hdriList.Count == 1)
				{
					Color color = GUI.color;
					GUI.color = Color.gray;
					Vector2 vector = GUI.skin.label.CalcSize(LookDevEnvironmentWindow.styles.sDragAndDropHDRIText);
					Rect position = new Rect(this.m_PositionInLookDev.width * 0.5f - vector.x * 0.5f, this.m_PositionInLookDev.height * 0.5f - vector.y * 0.5f, vector.x, vector.y);
					GUI.Label(position, LookDevEnvironmentWindow.styles.sDragAndDropHDRIText);
					GUI.color = color;
				}
				for (int i = 0; i < hdriList.Count; i++)
				{
					CubemapInfo cubemapInfo = hdriList[i];
					ShadowInfo shadowInfo = cubemapInfo.shadowInfo;
					int intProperty = this.m_LookDevView.config.GetIntProperty(LookDevProperty.HDRI, LookDevEditionContext.Left);
					int num = this.m_LookDevView.config.GetIntProperty(LookDevProperty.HDRI, LookDevEditionContext.Right);
					if (this.m_LookDevView.config.lookDevMode == LookDevMode.Single1 || this.m_LookDevView.config.lookDevMode == LookDevMode.Single2)
					{
						num = -1;
					}
					bool flag2 = i == intProperty || i == num;
					Color selectionColor = Color.black;
					Color selectionColor2 = Color.black;
					GUIStyle style = EditorStyles.miniLabel;
					if (flag2)
					{
						if (i == intProperty)
						{
							selectionColor = LookDevView.m_FirstViewGizmoColor;
							selectionColor2 = LookDevView.m_FirstViewGizmoColor;
							style = LookDevEnvironmentWindow.styles.sLabelStyleFirstContext;
						}
						else if (i == num)
						{
							selectionColor = LookDevView.m_SecondViewGizmoColor;
							selectionColor2 = LookDevView.m_SecondViewGizmoColor;
							style = LookDevEnvironmentWindow.styles.sLabelStyleSecondContext;
						}
						if (intProperty == num)
						{
							selectionColor = LookDevView.m_FirstViewGizmoColor;
							selectionColor2 = LookDevView.m_SecondViewGizmoColor;
							style = LookDevEnvironmentWindow.styles.sLabelStyleBothContext;
						}
					}
					GUILayout.BeginVertical(new GUILayoutOption[]
					{
						GUILayout.Width(250f)
					});
					int num2 = hdriList.FindIndex((CubemapInfo x) => x == this.m_SelectedCubemapInfo);
					if ((this.m_SelectedCubemap != null || this.m_DragBeingPerformed) && this.GetInsertionRect(i).Contains(Event.current.mousePosition) && ((num2 - i != 0 && num2 - i != -1) || num2 == -1))
					{
						GUILayout.Label(GUIContent.none, LookDevEnvironmentWindow.styles.sSeparatorStyle, new GUILayoutOption[0]);
						GUILayoutUtility.GetRect(250f, 16f);
					}
					GUILayout.Label(GUIContent.none, LookDevEnvironmentWindow.styles.sSeparatorStyle, new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(new GUILayoutOption[]
					{
						GUILayout.Width(250f),
						GUILayout.Height(18f)
					});
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(i.ToString());
					stringBuilder.Append(" - ");
					stringBuilder.Append(cubemapInfo.cubemap.name);
					GUILayout.Label(stringBuilder.ToString(), style, new GUILayoutOption[]
					{
						GUILayout.Height(18f),
						GUILayout.MaxWidth(175f)
					});
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(LookDevEnvironmentWindow.styles.sEnvControlIcon, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]))
					{
						Rect last = GUILayoutUtility.topLevel.GetLast();
						PopupWindow.Show(last, new LookDevEnvironmentWindow.EnvSettingsWindow(this.m_LookDevView, cubemapInfo));
						GUIUtility.ExitGUI();
					}
					using (new EditorGUI.DisabledScope(cubemapInfo.cubemap == LookDevResources.m_DefaultHDRI))
					{
						if (GUILayout.Button(LookDevEnvironmentWindow.styles.sCloseIcon, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]))
						{
							this.m_LookDevView.envLibrary.RemoveHDRI(cubemapInfo.cubemap);
						}
					}
					GUILayout.EndHorizontal();
					Rect lastRect = GUILayoutUtility.GetLastRect();
					if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition))
					{
						Event.current.Use();
					}
					Rect rect = GUILayoutUtility.GetRect(250f, 125f);
					rect.width = 253f;
					float num3 = 24f;
					float num4 = num3 * 0.5f;
					float latitude = shadowInfo.latitude;
					float longitude = shadowInfo.longitude;
					Vector2 vector2 = this.LatLongToPosition(latitude, longitude + cubemapInfo.angleOffset) * 0.5f + new Vector2(0.5f, 0.5f);
					Rect position2 = rect;
					position2.x = position2.x + vector2.x * rect.width - num4;
					position2.y = position2.y + (1f - vector2.y) * rect.height - num4;
					position2.width = num3;
					position2.height = num3;
					Rect position3 = rect;
					position3.x = position3.x + vector2.x * rect.width - num4 * 0.5f;
					position3.y = position3.y + (1f - vector2.y) * rect.height - num4 * 0.5f;
					position3.width = num3 * 0.5f;
					position3.height = num3 * 0.5f;
					Rect rect2;
					Rect textureRect;
					this.GetFrameAndShadowTextureRect(rect, out rect2, out textureRect);
					if (this.m_LookDevView.config.enableShadowCubemap)
					{
						EditorGUIUtility.AddCursorRect(position3, MouseCursor.Pan);
					}
					if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
					{
						if (!Event.current.control && Event.current.button == 0 && this.m_SelectedCubeMapOffsetIndex == -1)
						{
							if (this.m_LookDevView.config.enableShadowCubemap && position3.Contains(Event.current.mousePosition))
							{
								this.m_SelectedLightIconIndex = i;
								this.m_SelectedShadowInfo = shadowInfo;
								Undo.RecordObject(this.m_LookDevView.envLibrary, "Light Icon selection");
								this.m_SelectedShadowInfo.latitude = this.m_SelectedShadowInfo.latitude + 0.0001f;
								this.m_SelectedShadowInfo.longitude = this.m_SelectedShadowInfo.longitude + 0.0001f;
							}
							if (this.m_SelectedShadowInfo == null)
							{
								Rect rect3 = rect2;
								rect3.x += 100f;
								rect3.y += 4f;
								rect3.width = 11f;
								rect3.height = 11f;
								if (this.m_LookDevView.config.enableShadowCubemap && rect3.Contains(Event.current.mousePosition))
								{
									Undo.RecordObject(this.m_LookDevView.envLibrary, "Update shadow cubemap");
									hdriList[i].SetCubemapShadowInfo(hdriList[i]);
									this.m_LookDevView.envLibrary.dirty = true;
								}
								else
								{
									if (this.m_LookDevView.config.enableShadowCubemap && textureRect.Contains(Event.current.mousePosition))
									{
										this.m_SelectedShadowCubemapOwnerInfo = hdriList[i];
										this.m_SelectedCubemapInfo = this.m_SelectedShadowCubemapOwnerInfo.cubemapShadowInfo;
									}
									else
									{
										this.m_SelectedCubemapInfo = hdriList[i];
									}
									this.m_SelectedPositionOffset = Event.current.mousePosition - new Vector2(rect.x, rect.y);
									this.m_RenderOverlayThumbnailOnce = true;
								}
							}
						}
						else if (Event.current.control && Event.current.button == 0 && this.m_SelectedCubemapInfo == null && this.m_SelectedShadowInfo == null)
						{
							this.m_SelectedCubeMapOffsetIndex = i;
							this.m_SelectedCubeMapOffsetValue = cubemapInfo.angleOffset - this.ComputeAngleOffsetFromMouseCoord(Event.current.mousePosition);
						}
						GUIUtility.hotControl = this.m_LookDevView.hotControl;
						Event.current.Use();
					}
					if (Event.current.GetTypeForControl(this.m_LookDevView.hotControl) == EventType.MouseDrag)
					{
						if (this.m_SelectedShadowInfo == shadowInfo && this.m_SelectedLightIconIndex == i)
						{
							Vector2 mousePosition = Event.current.mousePosition;
							mousePosition.x = (mousePosition.x - rect.x) / rect.width * 2f - 1f;
							mousePosition.y = (1f - (mousePosition.y - rect.y) / rect.height) * 2f - 1f;
							Vector2 vector3 = LookDevEnvironmentWindow.PositionToLatLong(mousePosition);
							this.m_SelectedShadowInfo.latitude = vector3.x;
							this.m_SelectedShadowInfo.longitude = vector3.y - cubemapInfo.angleOffset;
							this.m_LookDevView.envLibrary.dirty = true;
						}
					}
					if (Event.current.type == EventType.Repaint)
					{
						this.DrawLatLongThumbnail(cubemapInfo, cubemapInfo.angleOffset, 1f, 1f, rect);
						if (this.m_LookDevView.config.enableShadowCubemap)
						{
							if (cubemapInfo.cubemapShadowInfo != cubemapInfo || (this.m_HoveringCubeMapIndex == i && this.m_SelectedCubemapInfo != cubemapInfo))
							{
								CubemapInfo infos = cubemapInfo.cubemapShadowInfo;
								if (this.m_HoveringCubeMapIndex == i && this.m_SelectedCubemapInfo != cubemapInfo)
								{
									infos = this.m_SelectedCubemapInfo;
								}
								float alpha = 1f;
								if (this.m_SelectedShadowInfo == shadowInfo)
								{
									alpha = 0.1f;
								}
								else if (this.m_HoveringCubeMapIndex == i && this.m_SelectedCubemapInfo != cubemapInfo && cubemapInfo.cubemapShadowInfo != this.m_SelectedCubemapInfo)
								{
									alpha = 0.5f;
								}
								this.DrawLatLongThumbnail(infos, cubemapInfo.angleOffset, 0.3f, alpha, textureRect);
								GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
								GUI.DrawTexture(rect2, LookDevEnvironmentWindow.styles.sLatlongFrameTexture);
								GL.sRGBWrite = false;
							}
							GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
							GUI.DrawTexture(position2, LookDevEnvironmentWindow.styles.sLightTexture);
							GL.sRGBWrite = false;
						}
						if (flag2)
						{
							this.DrawSelectionFeedback(rect, selectionColor, selectionColor2);
						}
					}
					GUILayout.EndVertical();
				}
				GUILayout.BeginVertical(new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				});
				if ((this.m_SelectedCubemap != null || this.m_DragBeingPerformed) && this.GetInsertionRect(hdriList.Count).Contains(Event.current.mousePosition))
				{
					GUILayout.Label(GUIContent.none, LookDevEnvironmentWindow.styles.sSeparatorStyle, new GUILayoutOption[0]);
					GUILayoutUtility.GetRect(250f, 16f);
					GUILayout.Label(GUIContent.none, LookDevEnvironmentWindow.styles.sSeparatorStyle, new GUILayoutOption[0]);
				}
				GUILayout.EndVertical();
				if (flag)
				{
					EditorGUILayout.EndScrollView();
				}
				this.HandleMouseInput();
				this.RenderOverlayThumbnailIfNeeded();
				if (Event.current.type == EventType.Repaint)
				{
					if (this.m_SelectedCubemap != null)
					{
						this.m_LookDevView.Repaint();
					}
				}
			}
		}
	}
}
