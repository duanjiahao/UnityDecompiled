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
		private GameViewSizeGroup m_WebPlayer = new GameViewSizeGroup();
		[SerializeField]
		private GameViewSizeGroup m_iOS = new GameViewSizeGroup();
		[SerializeField]
		private GameViewSizeGroup m_Android = new GameViewSizeGroup();
		[SerializeField]
		private GameViewSizeGroup m_XBox360 = new GameViewSizeGroup();
		[SerializeField]
		private GameViewSizeGroup m_PS3 = new GameViewSizeGroup();
		[SerializeField]
		private GameViewSizeGroup m_BB10 = new GameViewSizeGroup();
		[SerializeField]
		private GameViewSizeGroup m_Tizen = new GameViewSizeGroup();
		[SerializeField]
		private GameViewSizeGroup m_WP8 = new GameViewSizeGroup();
		[NonSerialized]
		private GameViewSize m_Remote;
		[NonSerialized]
		private Vector2 m_LastStandaloneScreenSize = new Vector2(-1f, -1f);
		[NonSerialized]
		private Vector2 m_LastWebPlayerScreenSize = new Vector2(-1f, -1f);
		[NonSerialized]
		private Vector2 m_LastRemoteScreenSize = new Vector2(-1f, -1f);
		[NonSerialized]
		private int m_ChangeID;
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
		static GameViewSizes()
		{
			GameViewSizes.RefreshGameViewSizeGroupType();
			EditorUserBuildSettings.activeBuildTargetChanged = (Action)Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, delegate
			{
				GameViewSizes.RefreshGameViewSizeGroupType();
			});
		}
		public GameViewSizeGroup GetGroup(GameViewSizeGroupType gameViewSizeGroupType)
		{
			this.InitBuiltinGroups();
			switch (gameViewSizeGroupType)
			{
			case GameViewSizeGroupType.Standalone:
				return this.m_Standalone;
			case GameViewSizeGroupType.WebPlayer:
				return this.m_WebPlayer;
			case GameViewSizeGroupType.iOS:
				return this.m_iOS;
			case GameViewSizeGroupType.Android:
				return this.m_Android;
			case GameViewSizeGroupType.PS3:
				return this.m_PS3;
			case GameViewSizeGroupType.Xbox360:
				return this.m_XBox360;
			case GameViewSizeGroupType.BB10:
				return this.m_BB10;
			case GameViewSizeGroupType.Tizen:
				return this.m_Tizen;
			case GameViewSizeGroupType.WP8:
				return this.m_WP8;
			default:
				Debug.LogError("Unhandled group enum! " + gameViewSizeGroupType);
				return this.m_Standalone;
			}
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
		public bool IsDefaultWebPlayerScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index)
		{
			return gameViewSizeGroupType == GameViewSizeGroupType.WebPlayer && this.GetDefaultWebPlayerIndex() == index;
		}
		public bool IsRemoteScreenSize(GameViewSizeGroupType gameViewSizeGroupType, int index)
		{
			return this.GetGroup(gameViewSizeGroupType).IndexOf(this.m_Remote) == index;
		}
		public int GetDefaultStandaloneIndex()
		{
			return this.m_Standalone.GetBuiltinCount() - 1;
		}
		public int GetDefaultWebPlayerIndex()
		{
			return this.m_WebPlayer.GetBuiltinCount() - 1;
		}
		public void RefreshStandaloneAndWebplayerDefaultSizes()
		{
			if (InternalEditorUtility.defaultScreenWidth != this.m_LastStandaloneScreenSize.x || InternalEditorUtility.defaultScreenHeight != this.m_LastStandaloneScreenSize.y)
			{
				this.m_LastStandaloneScreenSize = new Vector2(InternalEditorUtility.defaultScreenWidth, InternalEditorUtility.defaultScreenHeight);
				this.RefreshStandaloneDefaultScreenSize((int)this.m_LastStandaloneScreenSize.x, (int)this.m_LastStandaloneScreenSize.y);
			}
			if (InternalEditorUtility.defaultWebScreenWidth != this.m_LastWebPlayerScreenSize.x || InternalEditorUtility.defaultWebScreenHeight != this.m_LastWebPlayerScreenSize.y)
			{
				this.m_LastWebPlayerScreenSize = new Vector2(InternalEditorUtility.defaultWebScreenWidth, InternalEditorUtility.defaultWebScreenHeight);
				this.RefreshWebPlayerDefaultScreenSize((int)this.m_LastWebPlayerScreenSize.x, (int)this.m_LastWebPlayerScreenSize.y);
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
		public void RefreshWebPlayerDefaultScreenSize(int width, int height)
		{
			GameViewSize gameViewSize = this.m_WebPlayer.GetGameViewSize(this.GetDefaultWebPlayerIndex());
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
			if (flag)
			{
				return;
			}
			this.m_Remote = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Remote (Not Connected)");
			GameViewSize gameViewSize = new GameViewSize(GameViewSizeType.AspectRatio, 0, 0, "Free Aspect");
			GameViewSize gameViewSize2 = new GameViewSize(GameViewSizeType.AspectRatio, 5, 4, string.Empty);
			GameViewSize gameViewSize3 = new GameViewSize(GameViewSizeType.AspectRatio, 4, 3, string.Empty);
			GameViewSize gameViewSize4 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, string.Empty);
			GameViewSize gameViewSize5 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 10, string.Empty);
			GameViewSize gameViewSize6 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 9, string.Empty);
			GameViewSize gameViewSize7 = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Standalone");
			GameViewSize gameViewSize8 = new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "Web");
			GameViewSize gameViewSize9 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 480, "iPhone Tall");
			GameViewSize gameViewSize10 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 320, "iPhone Wide");
			GameViewSize gameViewSize11 = new GameViewSize(GameViewSizeType.FixedResolution, 640, 960, "iPhone 4 Tall");
			GameViewSize gameViewSize12 = new GameViewSize(GameViewSizeType.FixedResolution, 960, 640, "iPhone 4 Wide");
			GameViewSize gameViewSize13 = new GameViewSize(GameViewSizeType.FixedResolution, 768, 1024, "iPad Tall");
			GameViewSize gameViewSize14 = new GameViewSize(GameViewSizeType.FixedResolution, 1024, 768, "iPad Wide");
			GameViewSize gameViewSize15 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 16, "iPhone 5 Tall");
			GameViewSize gameViewSize16 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 9, "iPhone 5 Wide");
			GameViewSize gameViewSize17 = new GameViewSize(GameViewSizeType.AspectRatio, 2, 3, "iPhone Tall");
			GameViewSize gameViewSize18 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "iPhone Wide");
			GameViewSize gameViewSize19 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 4, "iPad Tall");
			GameViewSize gameViewSize20 = new GameViewSize(GameViewSizeType.AspectRatio, 4, 3, "iPad Wide");
			GameViewSize gameViewSize21 = new GameViewSize(GameViewSizeType.FixedResolution, 320, 480, "HVGA Portrait");
			GameViewSize gameViewSize22 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 320, "HVGA Landscape");
			GameViewSize gameViewSize23 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 800, "WVGA Portrait");
			GameViewSize gameViewSize24 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 480, "WVGA Landscape");
			GameViewSize gameViewSize25 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 854, "FWVGA Portrait");
			GameViewSize gameViewSize26 = new GameViewSize(GameViewSizeType.FixedResolution, 854, 480, "FWVGA Landscape");
			GameViewSize gameViewSize27 = new GameViewSize(GameViewSizeType.FixedResolution, 600, 1024, "WSVGA Portrait");
			GameViewSize gameViewSize28 = new GameViewSize(GameViewSizeType.FixedResolution, 1024, 600, "WSVGA Landscape");
			GameViewSize gameViewSize29 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 1280, "WXGA Portrait");
			GameViewSize gameViewSize30 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 800, "WXGA Landscape");
			GameViewSize gameViewSize31 = new GameViewSize(GameViewSizeType.AspectRatio, 2, 3, "3:2 Portrait");
			GameViewSize gameViewSize32 = new GameViewSize(GameViewSizeType.AspectRatio, 3, 2, "3:2 Landscape");
			GameViewSize gameViewSize33 = new GameViewSize(GameViewSizeType.AspectRatio, 10, 16, "16:10 Portrait");
			GameViewSize gameViewSize34 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 10, "16:10 Landscape");
			GameViewSize gameViewSize35 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 720, "720p (16:9)");
			GameViewSize gameViewSize36 = new GameViewSize(GameViewSizeType.FixedResolution, 1920, 1080, "1080p (16:9)");
			GameViewSize gameViewSize37 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 720, "720p (16:9)");
			GameViewSize gameViewSize38 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 576, "576p (4:3)");
			GameViewSize gameViewSize39 = new GameViewSize(GameViewSizeType.FixedResolution, 1024, 576, "576p (16:9)");
			GameViewSize gameViewSize40 = new GameViewSize(GameViewSizeType.FixedResolution, 640, 480, "480p (4:3)");
			GameViewSize gameViewSize41 = new GameViewSize(GameViewSizeType.FixedResolution, 853, 480, "480p (16:9)");
			GameViewSize gameViewSize42 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 1280, "Touch Phone Portrait");
			GameViewSize gameViewSize43 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 720, "Touch Phone Landscape");
			GameViewSize gameViewSize44 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 720, "Keyboard Phone");
			GameViewSize gameViewSize45 = new GameViewSize(GameViewSizeType.FixedResolution, 600, 1024, "Playbook Portrait");
			GameViewSize gameViewSize46 = new GameViewSize(GameViewSizeType.FixedResolution, 1024, 600, "Playbook Landscape");
			GameViewSize gameViewSize47 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 16, "9:16 Portrait");
			GameViewSize gameViewSize48 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 9, "16:9 Landscape");
			GameViewSize gameViewSize49 = new GameViewSize(GameViewSizeType.AspectRatio, 1, 1, "1:1");
			GameViewSize gameViewSize50 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 720, "16:9 Landscape");
			GameViewSize gameViewSize51 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 1280, "9:16 Portrait");
			GameViewSize gameViewSize52 = new GameViewSize(GameViewSizeType.FixedResolution, 480, 800, "WVGA Portrait");
			GameViewSize gameViewSize53 = new GameViewSize(GameViewSizeType.FixedResolution, 800, 480, "WVGA Landscape");
			GameViewSize gameViewSize54 = new GameViewSize(GameViewSizeType.FixedResolution, 768, 1280, "WXGA Portrait");
			GameViewSize gameViewSize55 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 768, "WXGA Landscape");
			GameViewSize gameViewSize56 = new GameViewSize(GameViewSizeType.FixedResolution, 720, 1280, "720p Portrait");
			GameViewSize gameViewSize57 = new GameViewSize(GameViewSizeType.FixedResolution, 1280, 720, "720p Landscape");
			GameViewSize gameViewSize58 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 15, "WVGA Portrait");
			GameViewSize gameViewSize59 = new GameViewSize(GameViewSizeType.AspectRatio, 15, 9, "WVGA Landscape");
			GameViewSize gameViewSize60 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 15, "WXGA Portrait");
			GameViewSize gameViewSize61 = new GameViewSize(GameViewSizeType.AspectRatio, 15, 9, "WXGA Landscape");
			GameViewSize gameViewSize62 = new GameViewSize(GameViewSizeType.AspectRatio, 9, 16, "720p Portrait");
			GameViewSize gameViewSize63 = new GameViewSize(GameViewSizeType.AspectRatio, 16, 9, "720p Landscape");
			this.m_WP8.AddBuiltinSizes(new GameViewSize[]
			{
				gameViewSize,
				gameViewSize52,
				gameViewSize58,
				gameViewSize53,
				gameViewSize59,
				gameViewSize54,
				gameViewSize60,
				gameViewSize55,
				gameViewSize61,
				gameViewSize56,
				gameViewSize62,
				gameViewSize57,
				gameViewSize63
			});
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
			this.m_WebPlayer.AddBuiltinSizes(new GameViewSize[]
			{
				gameViewSize,
				gameViewSize2,
				gameViewSize3,
				gameViewSize4,
				gameViewSize5,
				gameViewSize6,
				gameViewSize8
			});
			this.m_PS3.AddBuiltinSizes(new GameViewSize[]
			{
				gameViewSize,
				gameViewSize3,
				gameViewSize6,
				gameViewSize5,
				gameViewSize36,
				gameViewSize37,
				gameViewSize38,
				gameViewSize39,
				gameViewSize40,
				gameViewSize41
			});
			this.m_XBox360.AddBuiltinSizes(new GameViewSize[]
			{
				gameViewSize,
				gameViewSize3,
				gameViewSize6,
				gameViewSize5,
				gameViewSize35
			});
			this.m_iOS.AddBuiltinSizes(new GameViewSize[]
			{
				gameViewSize,
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
				gameViewSize19,
				gameViewSize20
			});
			this.m_Android.AddBuiltinSizes(new GameViewSize[]
			{
				gameViewSize,
				this.m_Remote,
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
				gameViewSize33,
				gameViewSize34
			});
			this.m_BB10.AddBuiltinSizes(new GameViewSize[]
			{
				gameViewSize,
				gameViewSize42,
				gameViewSize43,
				gameViewSize44,
				gameViewSize45,
				gameViewSize46,
				gameViewSize47,
				gameViewSize48,
				gameViewSize49
			});
			this.m_Tizen.AddBuiltinSizes(new GameViewSize[]
			{
				gameViewSize,
				gameViewSize50,
				gameViewSize51
			});
		}
		private static void RefreshDerivedGameViewSize(GameViewSizeGroupType groupType, int gameViewSizeIndex, GameViewSize gameViewSize)
		{
			if (ScriptableSingleton<GameViewSizes>.instance.IsDefaultStandaloneScreenSize(groupType, gameViewSizeIndex))
			{
				gameViewSize.width = (int)InternalEditorUtility.defaultScreenWidth;
				gameViewSize.height = (int)InternalEditorUtility.defaultScreenHeight;
			}
			else
			{
				if (ScriptableSingleton<GameViewSizes>.instance.IsDefaultWebPlayerScreenSize(groupType, gameViewSizeIndex))
				{
					gameViewSize.width = (int)InternalEditorUtility.defaultWebScreenWidth;
					gameViewSize.height = (int)InternalEditorUtility.defaultWebScreenHeight;
				}
				else
				{
					if (ScriptableSingleton<GameViewSizes>.instance.IsRemoteScreenSize(groupType, gameViewSizeIndex))
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
			}
		}
		public static Rect GetConstrainedRect(Rect startRect, GameViewSizeGroupType groupType, int gameViewSizeIndex, out bool fitsInsideRect)
		{
			fitsInsideRect = true;
			Rect result = startRect;
			GameViewSize gameViewSize = ScriptableSingleton<GameViewSizes>.instance.GetGroup(groupType).GetGameViewSize(gameViewSizeIndex);
			GameViewSizes.RefreshDerivedGameViewSize(groupType, gameViewSizeIndex, gameViewSize);
			if (gameViewSize.isFreeAspectRatio)
			{
				return startRect;
			}
			float num = 0f;
			GameViewSizeType sizeType = gameViewSize.sizeType;
			bool flag;
			if (sizeType != GameViewSizeType.AspectRatio)
			{
				if (sizeType != GameViewSizeType.FixedResolution)
				{
					Debug.LogError("Unhandled enum");
					return startRect;
				}
				if ((float)gameViewSize.height > startRect.height || (float)gameViewSize.width > startRect.width)
				{
					num = gameViewSize.aspectRatio;
					flag = true;
					fitsInsideRect = false;
				}
				else
				{
					result.height = (float)gameViewSize.height;
					result.width = (float)gameViewSize.width;
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
				result.height = ((result.width / num <= startRect.height) ? (result.width / num) : startRect.height);
				result.width = result.height * num;
			}
			result.height = Mathf.Clamp(result.height, 0f, startRect.height);
			result.width = Mathf.Clamp(result.width, 0f, startRect.width);
			result.y = startRect.height * 0.5f - result.height * 0.5f + startRect.y;
			result.x = startRect.width * 0.5f - result.width * 0.5f + startRect.x;
			result.width = Mathf.Floor(result.width + 0.5f);
			result.height = Mathf.Floor(result.height + 0.5f);
			result.x = Mathf.Floor(result.x + 0.5f);
			result.y = Mathf.Floor(result.y + 0.5f);
			return result;
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
				return GameViewSizeGroupType.Standalone;
			case BuildTargetGroup.WebPlayer:
			case BuildTargetGroup.FlashPlayer:
				return GameViewSizeGroupType.WebPlayer;
			case BuildTargetGroup.iPhone:
				return GameViewSizeGroupType.iOS;
			case BuildTargetGroup.PS3:
				return GameViewSizeGroupType.PS3;
			case BuildTargetGroup.XBOX360:
				return GameViewSizeGroupType.Xbox360;
			case BuildTargetGroup.Android:
				return GameViewSizeGroupType.Android;
			case BuildTargetGroup.WP8:
				return GameViewSizeGroupType.WP8;
			case BuildTargetGroup.BB10:
				return GameViewSizeGroupType.BB10;
			case BuildTargetGroup.Tizen:
				return GameViewSizeGroupType.Tizen;
			}
			return GameViewSizeGroupType.Standalone;
		}
	}
}
