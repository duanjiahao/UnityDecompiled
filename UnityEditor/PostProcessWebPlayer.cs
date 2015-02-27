using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
internal class PostProcessWebPlayer
{
	private static Dictionary<BuildOptions, string> optionNames = new Dictionary<BuildOptions, string>
	{

		{
			BuildOptions.AllowDebugging,
			"enableDebugging"
		}
	};
	public static void PostProcess(BuildOptions options, string installPath, string downloadWebplayerUrl, int width, int height)
	{
		string str = FileUtil.UnityGetFileName(installPath);
		string text = "Temp/BuildingWebplayerTemplate";
		FileUtil.DeleteFileOrDirectory(text);
		if (PlayerSettings.webPlayerTemplate == null || !PlayerSettings.webPlayerTemplate.Contains(":"))
		{
			Debug.LogError("Invalid WebPlayer template selection! Select a template in player settings.");
			return;
		}
		string[] array = PlayerSettings.webPlayerTemplate.Split(new char[]
		{
			':'
		});
		string text2;
		if (array[0].Equals("PROJECT"))
		{
			text2 = Application.dataPath;
		}
		else
		{
			text2 = Path.Combine(EditorApplication.applicationContentsPath, "Resources");
		}
		text2 = Path.Combine(Path.Combine(text2, "WebPlayerTemplates"), array[1]);
		if (!Directory.Exists(text2))
		{
			Debug.LogError("Invalid WebPlayer template path! Select a template in player settings.");
			return;
		}
		string[] files = Directory.GetFiles(text2, "index.*");
		if (files.Length < 1)
		{
			Debug.LogError("Invalid WebPlayer template selection! Select a template in player settings.");
			return;
		}
		FileUtil.CopyDirectoryRecursive(text2, text);
		files = Directory.GetFiles(text, "index.*");
		string text3 = files[0];
		string extension = Path.GetExtension(text3);
		string text4 = Path.Combine(text, str + extension);
		FileUtil.MoveFileOrDirectory(text3, text4);
		string[] files2 = Directory.GetFiles(text, "thumbnail.*");
		if (files2.Length > 0)
		{
			FileUtil.DeleteFileOrDirectory(files2[0]);
		}
		bool flag = (options & BuildOptions.WebPlayerOfflineDeployment) != BuildOptions.None;
		string item = (!flag) ? (downloadWebplayerUrl + "/3.0/uo/UnityObject2.js") : "UnityObject2.js";
		string item2 = string.Format("<script type='text/javascript' src='{0}'></script>", (!flag) ? "https://ssl-webplayer.unity3d.com/download_webplayer-3.x/3.0/uo/jquery.min.js" : "jquery.min.js");
		List<string> list = new List<string>();
		list.Add("%UNITY_UNITYOBJECT_DEPENDENCIES%");
		list.Add(item2);
		list.Add("%UNITY_UNITYOBJECT_URL%");
		list.Add(item);
		list.Add("%UNITY_WIDTH%");
		list.Add(width.ToString());
		list.Add("%UNITY_HEIGHT%");
		list.Add(height.ToString());
		list.Add("%UNITY_PLAYER_PARAMS%");
		list.Add(PostProcessWebPlayer.GeneratePlayerParamsString(options));
		list.Add("%UNITY_WEB_NAME%");
		list.Add(PlayerSettings.productName);
		list.Add("%UNITY_WEB_PATH%");
		list.Add(str + ".unity3d");
		if (InternalEditorUtility.IsUnityBeta())
		{
			list.Add("%UNITY_BETA_WARNING%");
			list.Add("\r\n\t\t<p style=\"color: #c00; font-size: small; font-style: italic;\">Built with beta version of Unity. Will only work on your computer!</p>");
			list.Add("%UNITY_SET_BASE_DOWNLOAD_URL%");
			list.Add(",baseDownloadUrl: \"" + downloadWebplayerUrl + "/\"");
		}
		else
		{
			list.Add("%UNITY_BETA_WARNING%");
			list.Add(string.Empty);
			list.Add("%UNITY_SET_BASE_DOWNLOAD_URL%");
			list.Add(string.Empty);
		}
		string[] templateCustomKeys = PlayerSettings.templateCustomKeys;
		for (int i = 0; i < templateCustomKeys.Length; i++)
		{
			string text5 = templateCustomKeys[i];
			list.Add("%UNITY_CUSTOM_" + text5.ToUpper() + "%");
			list.Add(PlayerSettings.GetTemplateCustomValue(text5));
		}
		FileUtil.ReplaceText(text4, list.ToArray());
		if (flag)
		{
			string text6 = Path.Combine(text, "UnityObject2.js");
			FileUtil.DeleteFileOrDirectory(text6);
			FileUtil.UnityFileCopy(EditorApplication.applicationContentsPath + "/Resources/UnityObject2.js", text6);
			text6 = Path.Combine(text, "jquery.min.js");
			FileUtil.DeleteFileOrDirectory(text6);
			FileUtil.UnityFileCopy(EditorApplication.applicationContentsPath + "/Resources/jquery.min.js", text6);
		}
		FileUtil.CopyDirectoryRecursive(text, installPath, true);
		string text7 = Path.Combine(installPath, str + ".unity3d");
		FileUtil.DeleteFileOrDirectory(text7);
		FileUtil.MoveFileOrDirectory("Temp/unitystream.unity3d", text7);
		if (Directory.Exists("Assets/StreamingAssets"))
		{
			FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", Path.Combine(installPath, "StreamingAssets"), true);
		}
	}
	private static string GeneratePlayerParamsString(BuildOptions options)
	{
		return string.Format("{{ {0} }}", string.Join(",", (
			from pair in PostProcessWebPlayer.optionNames
			select string.Format("{0}:\"{1}\"", pair.Value, (pair.Key != (options & pair.Key)) ? 0 : 1)).ToArray<string>()));
	}
}
