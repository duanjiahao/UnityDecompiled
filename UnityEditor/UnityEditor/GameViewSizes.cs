using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[FilePath("GameViewSizes.asset", FilePathAttribute.Location.PreferencesFolder)]
	internal class GameViewSizes : ScriptableSingleton<GameViewSizes>
	{
		[SerializeField]
		private GameViewSizeGroup m_Standalone = new GameViewSizeGroup();

		[SerializeField]
		private GameViewSizeGroup m_iOS = new GameViewSizeGroup();

		[SerializeField]
		private GameViewSizeGroup m_Android = new GameViewSizeGroup();

		[SerializeField]
		private GameViewSizeGroup m_WiiU = new GameViewSizeGroup();

		[SerializeField]
		private GameViewSizeGroup m_Tizen = new GameViewSizeGroup();

		[SerializeField]
		private GameViewSizeGroup m_N3DS = new GameViewSizeGroup();

		[NonSerialized]
		private GameViewSize m_Remote = null;

		[NonSerialized]
		private Vector2 m_LastStandaloneScreenSize = new Vector2(-1f, -1f);

		[NonSerialized]
		private Vector2 m_LastRemoteScreenSize = new Vector2(-1f, -1f);

		[NonSerialized]
		private int m_ChangeID = 0;

		[NonSerialized]
		private static GameViewSizeGroupType s_GameViewSizeGroupType;

		public GameViewSizeGroupType currentGroupType
		{
			get
			{
				return GameViewSizes.s_GameViewSizeGroupType;
			}
		}

		public GameViewSizeGroup currentGroup
		{
			get
			{
				return this.GetGroup(GameViewSizes.s_GameViewSizeGroupType);
			}
		}

		private void OnEnable()
		{
			GameViewSizes.RefreshGameViewSizeGroupType();
			EditorUserBuildSettings.activeBuildTargetChanged = (Action)Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, new Action(delegate
			{
				GameViewSizes.RefreshGameViewSizeGroupType();
			}));
		}

		public GameViewSizeGroup GetGroup(GameViewSizeGroupType gameViewSizeGroupType)
		{
			this.InitBuiltinGroups();
			GameViewSizeGroup result;
			switch (gameViewSizeGroupType)
			{
			case GameViewSizeGroupType.Standalone:
			case GameViewSizeGroupType.WebPlayer:
			case GameViewSizeGroupType.PS3:
			case GameViewSizeGroupType.WP8:
				result = this.m_Standalone;
				break;
			case GameViewSizeGroupType.iOS:
				result = this.m_iOS;
				break;
			case GameViewSizeGroupType.Android:
				result = this.m_Android;
				break;
			case GameViewSizeGroupType.WiiU:
				result = this.m_WiiU;
				break;
			case GameViewSizeGroupType.Tizen:
				result = this.m_Tizen;
				break;
			case GameViewSizeGroupType.N3DS:
				result = this.m_N3DS;
				break;
			default:
				Debug.LogError("Unhandled group enum! " + gameViewSizeGroupType);
				result = this.m_Standalone;
				break;
			}
			return result;
		}

		public void SaveToHDD()
		{
			bool saveAsText = true;
			this.Save(saveAsText);
		}

		public bool IsDefaultStandaloneScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index)
		{
			return gameViewSizeGroupType == GameViewSizeGroupType.Standalone && this.GetDefaultStandaloneIndex() == index;
		}

		public bool IsRemoteScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index)
		{
			return this.GetGroup(gameViewSizeGroupType).IndexOf(this.m_Remote) == index;
		}

		public int GetDefaultStandaloneIndex()
		{
			return this.m_Standalone.GetBuiltinCount() - 1;
		}

		public void RefreshStandaloneAndRemoteDefaultSizes()
		{
			if (InternalEditorUtility.defaultScreenWidth != this.m_LastStandaloneScreenSize.x || InternalEditorUtility.defaultScreenHeight != this.m_LastStandaloneScreenSize.y)
			{
				this.m_LastStandaloneScreenSize = new Vector2(InternalEditorUtility.defaultScreenWidth, InternalEditorUtility.defaultScreenHeight);
				this.RefreshStandaloneDefaultScreenSize((int)this.m_LastStandaloneScreenSize.x, (int)this.m_LastStandaloneScreenSize.y);
			}
			if (InternalEditorUtility.remoteScreenWidth != this.m_LastRemoteScreenSize.x || InternalEditorUtility.remoteScreenHeight != this.m_LastRemoteScreenSize.y)
			{
				this.m_LastRemoteScreenSize = new Vector2(InternalEditorUtility.remoteScreenWidth, InternalEditorUtility.remoteScreenHeight);
				this.RefreshRemoteScreenSize((int)this.m_LastRemoteScreenSize.x, (int)this.m_LastRemoteScreenSize.y);
			}
		}

		public void RefreshStandaloneDefaultScreenSize(int width, int height)
		{
			GameViewSize gameViewSize = this.m_Standalone.GetGameViewSize(this.GetDefaultStandaloneIndex());
			gameViewSize.height = height;
			gameViewSize.width = width;
			this.Changed();
		}

		public void RefreshRemoteScreenSize(int width, int height)
		{
			this.m_Remote.width = width;
			this.m_Remote.height = height;
			if (width > 0 && height > 0)
			{
				this.m_Remote.baseText = "Remote";
			}
			else
			{
				this.m_Remote.baseText = "Remote (Not Connected)";
			}
			this.Changed();
		}

		public void Changed()
		{
			this.m_ChangeID++;
		}

		public int GetChangeID()
		{
			return this.m_ChangeID;
		}

		private void InitBuiltinGroups()
		{
			bool flag = this.m_Standalone.GetBuiltinCount() > 0;
			if (!flag)
			{
				this.m_Remote = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Remote (Not Connected)");
				GameViewSize gameViewSize = new GameViewSize(GameViewSizeType.AspectRatio, 0, 0, "Free Aspect");
				GameViewSize gameViewSize2 = new GameViewSize(GameViewSizeType.AspectRatio, 5, 4, "");
				GameViewSize gameViewSize3 = new GameViewSize(GameViewSizeType.AspectRatio, 4, 3, "");
				GameViewSize gameViewSize4 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "");
				GameViewSize gameViewSize5 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 10, "");
				GameViewSize gameViewSize6 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 9, "");
				GameViewSize gameViewSize7 = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Standalone");
				GameViewSize gameViewSize8 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 480, "iPhone Tall");
				GameViewSize gameViewSize9 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 320, "iPhone Wide");
				GameViewSize gameViewSize10 = new GameViewSize(GameViewSizeType.FixedResolution, 640, 960, "iPhone 4 Tall");
				GameViewSize gameViewSize11 = new GameViewSize(GameViewSizeType.FixedResolution, 960, 640, "iPhone 4 Wide");
				GameViewSize gameViewSize12 = new GameViewSize(GameViewSizeType.FixedResolution, 768, 1024, "iPad Tall");
				GameViewSize gameViewSize13 = new GameViewSize(GameViewSizeType.FixedResolution, 1024, 768, "iPad Wide");
				GameViewSize gameViewSize14 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 16, "iPhone 5 Tall");
				GameViewSize gameViewSize15 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 9, "iPhone 5 Wide");
				GameViewSize gameViewSize16 = new GameViewSize(GameViewSizeType.AspectRatio, 2, 3, "iPhone Tall");
				GameViewSize gameViewSize17 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "iPhone Wide");
				GameViewSize gameViewSize18 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 4, "iPad Tall");
				GameViewSize gameViewSize19 = new GameViewSize(GameViewSizeType.AspectRatio, 4, 3, "iPad Wide");
				GameViewSize gameViewSize20 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 480, "HVGA Portrait");
				GameViewSize gameViewSize21 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 320, "HVGA Landscape");
				GameViewSize gameViewSize22 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 800, "WVGA Portrait");
				GameViewSize gameViewSize23 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 480, "WVGA Landscape");
				GameViewSize gameViewSize24 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 854, "FWVGA Portrait");
				GameViewSize gameViewSize25 = new GameViewSize(GameViewSizeType.FixedResolution, 854, 480, "FWVGA Landscape");
				GameViewSize gameViewSize26 = new GameViewSize(GameViewSizeType.FixedResolution, 600, 1024, "WSVGA Portrait");
				GameViewSize gameViewSize27 = new GameViewSize(GameViewSizeType.FixedResolution, 1024, 600, "WSVGA Landscape");
				GameViewSize gameViewSize28 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 1280, "WXGA Portrait");
				GameViewSize gameViewSize29 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 800, "WXGA Landscape");
				GameViewSize gameViewSize30 = new GameViewSize(GameViewSizeType.AspectRatio, 2, 3, "3:2 Portrait");
				GameViewSize gameViewSize31 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "3:2 Landscape");
				GameViewSize gameViewSize32 = new GameViewSize(GameViewSizeType.AspectRatio, 10, 16, "16:10 Portrait");
				GameViewSize gameViewSize33 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 10, "16:10 Landscape");
				GameViewSize gameViewSize34 = new GameViewSize(GameViewSizeType.FixedResolution, 1920, 1080, "1080p (16:9)");
				GameViewSize gameViewSize35 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 720, "720p (16:9)");
				GameViewSize gameViewSize36 = new GameViewSize(GameViewSizeType.FixedResolution, 854, 480, "GamePad 480p (16:9)");
				GameViewSize gameViewSize37 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 720, "16:9 Landscape");
				GameViewSize gameViewSize38 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 1280, "9:16 Portrait");
				GameViewSize gameViewSize39 = new GameViewSize(GameViewSizeType.FixedResolution, 400, 240, "Top Screen");
				GameViewSize gameViewSize40 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 240, "Bottom Screen");
				this.m_Standalone.AddBuiltinSizes(new GameViewSize[]
				{
					gameViewSize,
					gameViewSize2,
					gameViewSize3,
					gameViewSize4,
					gameViewSize5,
					gameViewSize6,
					gameViewSize7
				});
				this.m_WiiU.AddBuiltinSizes(new GameViewSize[]
				{
					gameViewSize,
					gameViewSize3,
					gameViewSize6,
					gameViewSize34,
					gameViewSize35,
					gameViewSize36
				});
				this.m_iOS.AddBuiltinSizes(new GameViewSize[]
				{
					gameViewSize,
					gameViewSize8,
					gameViewSize9,
					gameViewSize10,
					gameViewSize11,
					gameViewSize12,
					gameViewSize13,
					gameViewSize14,
					gameViewSize15,
					gameViewSize16,
					gameViewSize17,
					gameViewSize18,
					gameViewSize19
				});
				this.m_Android.AddBuiltinSizes(new GameViewSize[]
				{
					gameViewSize,
					this.m_Remote,
					gameViewSize20,
					gameViewSize21,
					gameViewSize22,
					gameViewSize23,
					gameViewSize24,
					gameViewSize25,
					gameViewSize26,
					gameViewSize27,
					gameViewSize28,
					gameViewSize29,
					gameViewSize30,
					gameViewSize31,
					gameViewSize32,
					gameViewSize33
				});
				this.m_Tizen.AddBuiltinSizes(new GameViewSize[]
				{
					gameViewSize,
					gameViewSize37,
					gameViewSize38
				});
				this.m_N3DS.AddBuiltinSizes(new GameViewSize[]
				{
					gameViewSize,
					gameViewSize39,
					gameViewSize40
				});
			}
		}

		internal static bool DefaultLowResolutionSettingForSizeGroupType(GameViewSizeGroupType sizeGroupType)
		{
			bool result;
			switch (sizeGroupType)
			{
			case GameViewSizeGroupType.Standalone:
			case GameViewSizeGroupType.WiiU:
			case GameViewSizeGroupType.N3DS:
				result = true;
				return result;
			case GameViewSizeGroupType.iOS:
			case GameViewSizeGroupType.Android:
			case GameViewSizeGroupType.Tizen:
				result = false;
				return result;
			}
			result = false;
			return result;
		}

		private static void RefreshDerivedGameViewSize(GameViewSizeGroupType groupType, int gameViewSizeIndex, GameViewSize gameViewSize)
		{
			if (ScriptableSingleton<GameViewSizes>.instance.IsDefaultStandaloneScreenSize(groupType, gameViewSizeIndex))
			{
				gameViewSize.width = (int)InternalEditorUtility.defaultScreenWidth;
				gameViewSize.height = (int)InternalEditorUtility.defaultScreenHeight;
			}
			else if (ScriptableSingleton<GameViewSizes>.instance.IsRemoteScreenSize(groupType, gameViewSizeIndex))
			{
				if (InternalEditorUtility.remoteScreenWidth <= 0f || InternalEditorUtility.remoteScreenHeight <= 0f)
				{
					gameViewSize.sizeType = GameViewSizeType.AspectRatio;
					int num = 0;
					gameViewSize.height = num;
					gameViewSize.width = num;
				}
				else
				{
					gameViewSize.sizeType = GameViewSizeType.FixedResolution;
					gameViewSize.width = (int)InternalEditorUtility.remoteScreenWidth;
					gameViewSize.height = (int)InternalEditorUtility.remoteScreenHeight;
				}
			}
		}

		public static Rect GetConstrainedRect(Rect startRect, GameViewSizeGroupType groupType, int gameViewSizeIndex, out bool fitsInsideRect)
		{
			fitsInsideRect = true;
			Rect rect = startRect;
			GameViewSize gameViewSize = ScriptableSingleton<GameViewSizes>.instance.GetGroup(groupType).GetGameViewSize(gameViewSizeIndex);
			GameViewSizes.RefreshDerivedGameViewSize(groupType, gameViewSizeIndex, gameViewSize);
			Rect result;
			if (gameViewSize.isFreeAspectRatio)
			{
				result = startRect;
			}
			else
			{
				float num = 0f;
				GameViewSizeType sizeType = gameViewSize.sizeType;
				bool flag;
				if (sizeType != GameViewSizeType.AspectRatio)
				{
					if (sizeType != GameViewSizeType.FixedResolution)
					{
						throw new ArgumentException("Unrecognized size type");
					}
					if ((float)gameViewSize.height > startRect.height || (float)gameViewSize.width > startRect.width)
					{
						num = gameViewSize.aspectRatio;
						flag = true;
						fitsInsideRect = false;
					}
					else
					{
						rect.height = (float)gameViewSize.height;
						rect.width = (float)gameViewSize.width;
						flag = false;
					}
				}
				else
				{
					num = gameViewSize.aspectRatio;
					flag = true;
				}
				if (flag)
				{
					rect.height = ((rect.width / num <= startRect.height) ? (rect.width / num) : startRect.height);
					rect.width = rect.height * num;
				}
				rect.height = Mathf.Clamp(rect.height, 0f, startRect.height);
				rect.width = Mathf.Clamp(rect.width, 0f, startRect.width);
				rect.y = startRect.height * 0.5f - rect.height * 0.5f + startRect.y;
				rect.x = startRect.width * 0.5f - rect.width * 0.5f + startRect.x;
				rect.width = Mathf.Floor(rect.width + 0.5f);
				rect.height = Mathf.Floor(rect.height + 0.5f);
				rect.x = Mathf.Floor(rect.x + 0.5f);
				rect.y = Mathf.Floor(rect.y + 0.5f);
				result = rect;
			}
			return result;
		}

		public static Vector2 GetRenderTargetSize(Rect startRect, GameViewSizeGroupType groupType, int gameViewSizeIndex, out bool clamped)
		{
			GameViewSize gameViewSize = ScriptableSingleton<GameViewSizes>.instance.GetGroup(groupType).GetGameViewSize(gameViewSizeIndex);
			GameViewSizes.RefreshDerivedGameViewSize(groupType, gameViewSizeIndex, gameViewSize);
			clamped = false;
			Vector2 vector;
			if (gameViewSize.isFreeAspectRatio)
			{
				vector = startRect.size;
			}
			else
			{
				GameViewSizeType sizeType = gameViewSize.sizeType;
				if (sizeType != GameViewSizeType.AspectRatio)
				{
					if (sizeType != GameViewSizeType.FixedResolution)
					{
						throw new ArgumentException("Unrecognized size type");
					}
					vector = new Vector2((float)gameViewSize.width, (float)gameViewSize.height);
				}
				else if (startRect.height == 0f || gameViewSize.aspectRatio == 0f)
				{
					vector = Vector2.zero;
				}
				else
				{
					float num = startRect.width / startRect.height;
					if (num < gameViewSize.aspectRatio)
					{
						vector = new Vector2(startRect.width, startRect.width / gameViewSize.aspectRatio);
					}
					else
					{
						vector = new Vector2(startRect.height * gameViewSize.aspectRatio, startRect.height);
					}
				}
			}
			float num2 = (float)SystemInfo.graphicsMemorySize * 0.2f / 12f * 1024f * 1024f;
			float num3 = vector.x * vector.y;
			if (num3 > num2)
			{
				vector *= num2 / num3;
				clamped = true;
			}
			float b = 8192f;
			float num4 = Mathf.Min((float)SystemInfo.maxRenderTextureSize, b);
			if (vector.x > num4 || vector.y > num4)
			{
				if (vector.x > vector.y)
				{
					vector *= num4 / vector.x;
				}
				else
				{
					vector *= num4 / vector.y;
				}
				clamped = true;
			}
			return vector;
		}

		private static void RefreshGameViewSizeGroupType()
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			GameViewSizes.s_GameViewSizeGroupType = GameViewSizes.BuildTargetGroupToGameViewSizeGroup(buildTargetGroup);
		}

		public static GameViewSizeGroupType BuildTargetGroupToGameViewSizeGroup(BuildTargetGroup buildTargetGroup)
		{
			switch (buildTargetGroup)
			{
			case BuildTargetGroup.Standalone:
			{
				GameViewSizeGroupType result = GameViewSizeGroupType.Standalone;
				return result;
			}
			case BuildTargetGroup.WebPlayer:
			case (BuildTargetGroup)3:
			{
				IL_19:
				GameViewSizeGroupType result;
				if (buildTargetGroup == BuildTargetGroup.N3DS)
				{
					result = GameViewSizeGroupType.N3DS;
					return result;
				}
				if (buildTargetGroup == BuildTargetGroup.WiiU)
				{
					result = GameViewSizeGroupType.WiiU;
					return result;
				}
				if (buildTargetGroup == BuildTargetGroup.Android)
				{
					result = GameViewSizeGroupType.Android;
					return result;
				}
				if (buildTargetGroup != BuildTargetGroup.Tizen)
				{
					result = GameViewSizeGroupType.Standalone;
					return result;
				}
				result = GameViewSizeGroupType.Tizen;
				return result;
			}
			case BuildTargetGroup.iPhone:
			{
				GameViewSizeGroupType result = GameViewSizeGroupType.iOS;
				return result;
			}
			}
			goto IL_19;
		}
	}
}
