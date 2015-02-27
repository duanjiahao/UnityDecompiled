using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
internal class FlashPostProcessSettings
{
	internal const string UnityFlashConstantNamespace = "UNITY_FLASH";
	public FlashBuildSubtarget FlashBuildSubtarget;
	public bool IsDevelopment;
	public bool StripPhysics;
	public int Width;
	public int Height;
	internal string GetUnityNativeSwcForSubTarget(bool stripPhysics)
	{
		string str;
		switch (this.FlashBuildSubtarget)
		{
		case FlashBuildSubtarget.Flash11dot2:
		case FlashBuildSubtarget.Flash11dot3:
		case FlashBuildSubtarget.Flash11dot4:
		case FlashBuildSubtarget.Flash11dot5:
		case FlashBuildSubtarget.Flash11dot6:
		case FlashBuildSubtarget.Flash11dot7:
		case FlashBuildSubtarget.Flash11dot8:
			str = "UnityNative11dot2";
			break;
		default:
			str = "UnityNative11dot2";
			break;
		}
		if (this.StripPhysics)
		{
			str += "_nophysx";
		}
		return str + ".swc";
	}
	internal int GetSwfVersionForPlayerVersion()
	{
		switch (this.FlashBuildSubtarget)
		{
		case FlashBuildSubtarget.Flash11dot2:
			return 15;
		case FlashBuildSubtarget.Flash11dot3:
			return 16;
		case FlashBuildSubtarget.Flash11dot4:
			return 17;
		case FlashBuildSubtarget.Flash11dot5:
			return 18;
		case FlashBuildSubtarget.Flash11dot6:
			return 19;
		case FlashBuildSubtarget.Flash11dot7:
			return 20;
		case FlashBuildSubtarget.Flash11dot8:
			return 21;
		default:
			return 15;
		}
	}
	internal string GetTargetPlayerForSubtarget()
	{
		switch (this.FlashBuildSubtarget)
		{
		case FlashBuildSubtarget.Flash11dot2:
			return "11.2.0";
		case FlashBuildSubtarget.Flash11dot3:
			return "11.3.0";
		case FlashBuildSubtarget.Flash11dot4:
			return "11.4.0";
		case FlashBuildSubtarget.Flash11dot5:
			return "11.5.0";
		case FlashBuildSubtarget.Flash11dot6:
			return "11.6.0";
		case FlashBuildSubtarget.Flash11dot7:
			return "11.7.0";
		case FlashBuildSubtarget.Flash11dot8:
			return "11.8.0";
		default:
			return "11.2.0";
		}
	}
	internal string MxmlcCompileTimeConstants()
	{
		return FlashPostProcessSettings.CompileTimeConstantsFor(this).Aggregate(string.Empty, (string agg, KeyValuePair<string, object> value) => agg + string.Format("-define+={0}::{1},{2} ", "UNITY_FLASH", value.Key, FlashPostProcessSettings.EncodeMxmlcConstant(value.Value)));
	}
	private static Dictionary<string, object> CompileTimeConstantsFor(FlashPostProcessSettings settings)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>
		{

			{
				"TargetFlashPlayerVersion",
				settings.GetTargetPlayerForSubtarget()
			},

			{
				"TargetSwfVersion",
				settings.GetSwfVersionForPlayerVersion().ToString()
			}
		};
		FlashPostProcessSettings.AddConfigDefineForFlashPlayerFeatureLevel(settings.FlashBuildSubtarget >= FlashBuildSubtarget.Flash11dot2, "11dot2", dictionary);
		FlashPostProcessSettings.AddConfigDefineForFlashPlayerFeatureLevel(settings.FlashBuildSubtarget >= FlashBuildSubtarget.Flash11dot3, "11dot3", dictionary);
		FlashPostProcessSettings.AddConfigDefineForFlashPlayerFeatureLevel(settings.FlashBuildSubtarget >= FlashBuildSubtarget.Flash11dot4, "11dot4", dictionary);
		FlashPostProcessSettings.AddConfigDefineForFlashPlayerFeatureLevel(settings.FlashBuildSubtarget >= FlashBuildSubtarget.Flash11dot5, "11dot5", dictionary);
		FlashPostProcessSettings.AddConfigDefineForFlashPlayerFeatureLevel(settings.FlashBuildSubtarget >= FlashBuildSubtarget.Flash11dot6, "11dot6", dictionary);
		FlashPostProcessSettings.AddConfigDefineForFlashPlayerFeatureLevel(settings.FlashBuildSubtarget >= FlashBuildSubtarget.Flash11dot7, "11dot7", dictionary);
		FlashPostProcessSettings.AddConfigDefineForFlashPlayerFeatureLevel(settings.FlashBuildSubtarget >= FlashBuildSubtarget.Flash11dot8, "11dot8", dictionary);
		return dictionary;
	}
	private static void AddConfigDefineForFlashPlayerFeatureLevel(bool enabled, string level, IDictionary<string, object> dict)
	{
		dict.Add(string.Format("PLAYERFEATURE_LEVEL_{0}", level), enabled);
	}
	private static string EncodeMxmlcConstant(object value)
	{
		if (value is string)
		{
			return string.Format("\"'{0}'\"", value);
		}
		return value.ToString().ToLower();
	}
	public static FlashPostProcessSettings CreateWithBuildArguments(BuildOptions options, int width, int height)
	{
		return new FlashPostProcessSettings
		{
			Width = Math.Min(Math.Max(width, 1), 4096),
			Height = Math.Min(Math.Max(height, 1), 4096),
			StripPhysics = PlayerSettings.stripPhysics,
			IsDevelopment = (options & BuildOptions.Development) != BuildOptions.None,
			FlashBuildSubtarget = EditorUserBuildSettings.flashBuildSubtarget
		};
	}
	public static FlashPostProcessSettings CreateFrom(PostProcessFlashPlayerOptions options)
	{
		return FlashPostProcessSettings.CreateWithBuildArguments(options.Options, options.Width, options.Height);
	}
}
