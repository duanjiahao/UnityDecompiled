using System;
using UnityEngine;

namespace UnityEditor.Build
{
	internal class BuildPlatform
	{
		public string name;

		public GUIContent title;

		public Texture2D smallIcon;

		public BuildTargetGroup targetGroup;

		public bool forceShowTarget;

		public string tooltip;

		public BuildTarget defaultTarget
		{
			get
			{
				BuildTargetGroup buildTargetGroup = this.targetGroup;
				switch (buildTargetGroup)
				{
				case BuildTargetGroup.WebGL:
				{
					BuildTarget result = BuildTarget.WebGL;
					return result;
				}
				case BuildTargetGroup.WSA:
				{
					BuildTarget result = BuildTarget.WSAPlayer;
					return result;
				}
				case BuildTargetGroup.WP8:
				case BuildTargetGroup.BlackBerry:
				case BuildTargetGroup.PSM:
					IL_4D:
					switch (buildTargetGroup)
					{
					case BuildTargetGroup.Standalone:
					{
						BuildTarget result = BuildTarget.StandaloneWindows;
						return result;
					}
					case BuildTargetGroup.WebPlayer:
					case (BuildTargetGroup)3:
					{
						IL_65:
						BuildTarget result;
						if (buildTargetGroup != BuildTargetGroup.Android)
						{
							result = BuildTarget.iPhone;
							return result;
						}
						result = BuildTarget.Android;
						return result;
					}
					case BuildTargetGroup.iPhone:
					{
						BuildTarget result = BuildTarget.iOS;
						return result;
					}
					}
					goto IL_65;
				case BuildTargetGroup.Tizen:
				{
					BuildTarget result = BuildTarget.Tizen;
					return result;
				}
				case BuildTargetGroup.PSP2:
				{
					BuildTarget result = BuildTarget.PSP2;
					return result;
				}
				case BuildTargetGroup.PS4:
				{
					BuildTarget result = BuildTarget.PS4;
					return result;
				}
				case BuildTargetGroup.XboxOne:
				{
					BuildTarget result = BuildTarget.XboxOne;
					return result;
				}
				case BuildTargetGroup.SamsungTV:
				{
					BuildTarget result = BuildTarget.SamsungTV;
					return result;
				}
				case BuildTargetGroup.N3DS:
				{
					BuildTarget result = BuildTarget.N3DS;
					return result;
				}
				case BuildTargetGroup.WiiU:
				{
					BuildTarget result = BuildTarget.WiiU;
					return result;
				}
				case BuildTargetGroup.tvOS:
				{
					BuildTarget result = BuildTarget.tvOS;
					return result;
				}
				case BuildTargetGroup.Facebook:
				{
					BuildTarget result = BuildTarget.StandaloneWindows64;
					return result;
				}
				case BuildTargetGroup.Switch:
				{
					BuildTarget result = BuildTarget.Switch;
					return result;
				}
				}
				goto IL_4D;
			}
		}

		public BuildPlatform(string locTitle, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget) : this(locTitle, "", iconId, targetGroup, forceShowTarget)
		{
		}

		public BuildPlatform(string locTitle, string tooltip, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget)
		{
			this.targetGroup = targetGroup;
			this.name = ((targetGroup == BuildTargetGroup.Unknown) ? "" : BuildPipeline.GetBuildTargetGroupName(this.defaultTarget));
			this.title = EditorGUIUtility.TextContentWithIcon(locTitle, iconId);
			this.smallIcon = (EditorGUIUtility.IconContent(iconId + ".Small").image as Texture2D);
			this.tooltip = tooltip;
			this.forceShowTarget = forceShowTarget;
		}
	}
}
