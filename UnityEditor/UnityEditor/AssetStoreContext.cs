using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor.Web;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[InitializeOnLoad]
	internal sealed class AssetStoreContext
	{
		public class DownloadInfo
		{
			public string url;

			public string key;

			public string id;
		}

		public class LabelAndId
		{
			public string label;

			public string id;

			public void Initialize(JSONValue json)
			{
				if (json.ContainsKey("label"))
				{
					this.label = json["label"].AsString();
				}
				if (json.ContainsKey("id"))
				{
					this.id = json["id"].AsString();
				}
			}

			public override string ToString()
			{
				return string.Format("{{label={0}, id={1}}}", this.label, this.id);
			}
		}

		public class Link
		{
			public string type;

			public string id;

			public void Initialize(JSONValue json)
			{
				if (json.ContainsKey("type"))
				{
					this.type = json["type"].AsString();
				}
				if (json.ContainsKey("id"))
				{
					this.id = json["id"].AsString();
				}
			}

			public override string ToString()
			{
				return string.Format("{{type={0}, id={1}}}", this.type, this.id);
			}
		}

		public class Package
		{
			public string title;

			public string id;

			public string version;

			public string version_id;

			public string local_icon;

			public string local_path;

			public string pubdate;

			public string description;

			public AssetStoreContext.LabelAndId publisher;

			public AssetStoreContext.LabelAndId category;

			public AssetStoreContext.Link link;

			public void Initialize(JSONValue json)
			{
				if (json.ContainsKey("title"))
				{
					this.title = json["title"].AsString();
				}
				if (json.ContainsKey("id"))
				{
					this.id = json["id"].AsString();
				}
				if (json.ContainsKey("version"))
				{
					this.version = json["version"].AsString();
				}
				if (json.ContainsKey("version_id"))
				{
					this.version_id = json["version_id"].AsString();
				}
				if (json.ContainsKey("local_icon"))
				{
					this.local_icon = json["local_icon"].AsString();
				}
				if (json.ContainsKey("local_path"))
				{
					this.local_path = json["local_path"].AsString();
				}
				if (json.ContainsKey("pubdate"))
				{
					this.pubdate = json["pubdate"].AsString();
				}
				if (json.ContainsKey("description"))
				{
					this.description = json["description"].AsString();
				}
				if (json.ContainsKey("publisher"))
				{
					this.publisher = new AssetStoreContext.LabelAndId();
					this.publisher.Initialize(json["publisher"]);
				}
				if (json.ContainsKey("category"))
				{
					this.category = new AssetStoreContext.LabelAndId();
					this.category.Initialize(json["category"]);
				}
				if (json.ContainsKey("link"))
				{
					this.link = new AssetStoreContext.Link();
					this.link.Initialize(json["link"]);
				}
			}

			public override string ToString()
			{
				return string.Format("{{title={0}, id={1}, publisher={2}, category={3}, pubdate={8}, version={4}, version_id={5}, description={9}, link={10}, local_icon={6}, local_path={7}}}", new object[]
				{
					this.title,
					this.id,
					this.publisher,
					this.category,
					this.version,
					this.version_id,
					this.local_icon,
					this.local_path,
					this.pubdate,
					this.description,
					this.link
				});
			}
		}

		public class PackageList
		{
			public AssetStoreContext.Package[] results;
		}

		private static Regex s_StandardPackageRegExp;

		private static Regex s_GeneratedIDRegExp;

		private static Regex s_InvalidPathCharsRegExp;

		internal bool docked;

		internal string initialOpenURL;

		private static AssetStoreContext s_Instance;

		static AssetStoreContext()
		{
			AssetStoreContext.s_StandardPackageRegExp = new Regex("/Standard Packages/(Character\\ Controller|Glass\\ Refraction\\ \\(Pro\\ Only\\)|Image\\ Effects\\ \\(Pro\\ Only\\)|Light\\ Cookies|Light\\ Flares|Particles|Physic\\ Materials|Projectors|Scripts|Standard\\ Assets\\ \\(Mobile\\)|Skyboxes|Terrain\\ Assets|Toon\\ Shading|Tree\\ Creator|Water\\ \\(Basic\\)|Water\\ \\(Pro\\ Only\\))\\.unitypackage$", RegexOptions.IgnoreCase);
			AssetStoreContext.s_GeneratedIDRegExp = new Regex("^\\{(.*)\\}$");
			AssetStoreContext.s_InvalidPathCharsRegExp = new Regex("[^a-zA-Z0-9() _-]");
			AssetStoreContext.GetInstance();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SessionSetString(string key, string value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string SessionGetString(string key);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SessionRemoveString(string key);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SessionHasString(string key);

		public static AssetStoreContext GetInstance()
		{
			if (AssetStoreContext.s_Instance == null)
			{
				AssetStoreContext.s_Instance = new AssetStoreContext();
				JSProxyMgr.GetInstance().AddGlobalObject("AssetStoreContext", AssetStoreContext.s_Instance);
			}
			return AssetStoreContext.s_Instance;
		}

		public string GetInitialOpenURL()
		{
			string result;
			if (this.initialOpenURL != null)
			{
				string text = this.initialOpenURL;
				this.initialOpenURL = null;
				result = text;
			}
			else
			{
				result = "";
			}
			return result;
		}

		public string GetAuthToken()
		{
			return InternalEditorUtility.GetAuthToken();
		}

		public int[] GetLicenseFlags()
		{
			return InternalEditorUtility.GetLicenseFlags();
		}

		public string GetString(string key)
		{
			return EditorPrefs.GetString(key);
		}

		public int GetInt(string key)
		{
			return EditorPrefs.GetInt(key);
		}

		public float GetFloat(string key)
		{
			return EditorPrefs.GetFloat(key);
		}

		public void SetString(string key, string value)
		{
			EditorPrefs.SetString(key, value);
		}

		public void SetInt(string key, int value)
		{
			EditorPrefs.SetInt(key, value);
		}

		public void SetFloat(string key, float value)
		{
			EditorPrefs.SetFloat(key, value);
		}

		public bool HasKey(string key)
		{
			return EditorPrefs.HasKey(key);
		}

		public void DeleteKey(string key)
		{
			EditorPrefs.DeleteKey(key);
		}

		public int GetSkinIndex()
		{
			return EditorGUIUtility.skinIndex;
		}

		public bool GetDockedStatus()
		{
			return this.docked;
		}

		public bool OpenPackage(string id)
		{
			return this.OpenPackage(id, "default");
		}

		public bool OpenPackage(string id, string action)
		{
			return AssetStoreContext.OpenPackageInternal(id);
		}

		public static bool OpenPackageInternal(string id)
		{
			Match match = AssetStoreContext.s_GeneratedIDRegExp.Match(id);
			bool result;
			if (match.Success && File.Exists(match.Groups[1].Value))
			{
				AssetDatabase.ImportPackage(match.Groups[1].Value, true);
				result = true;
			}
			else
			{
				PackageInfo[] packageList = PackageInfo.GetPackageList();
				for (int i = 0; i < packageList.Length; i++)
				{
					PackageInfo packageInfo = packageList[i];
					if (packageInfo.jsonInfo != "")
					{
						JSONValue jSONValue = JSONParser.SimpleParse(packageInfo.jsonInfo);
						string text = (!jSONValue.Get("id").IsNull()) ? jSONValue["id"].AsString(true) : null;
						if (text != null && text == id && File.Exists(packageInfo.packagePath))
						{
							AssetDatabase.ImportPackage(packageInfo.packagePath, true);
							result = true;
							return result;
						}
					}
				}
				Debug.LogError("Unknown package ID " + id);
				result = false;
			}
			return result;
		}

		public void OpenBrowser(string url)
		{
			Application.OpenURL(url);
		}

		public void Download(AssetStoreContext.Package package, AssetStoreContext.DownloadInfo downloadInfo)
		{
			AssetStoreContext.Download(downloadInfo.id, downloadInfo.url, downloadInfo.key, package.title, package.publisher.label, package.category.label, null);
		}

		public static void Download(string package_id, string url, string key, string package_name, string publisher_name, string category_name, AssetStoreUtils.DownloadDoneCallback doneCallback)
		{
			string[] destination = AssetStoreContext.PackageStorePath(publisher_name, category_name, package_name, package_id, url);
			JSONValue jSONValue = JSONParser.SimpleParse(AssetStoreUtils.CheckDownload(package_id, url, destination, key));
			if (jSONValue.Get("in_progress").AsBool(true))
			{
				Debug.Log("Will not download " + package_name + ". Download is already in progress.");
			}
			else
			{
				string a = jSONValue.Get("download.url").AsString(true);
				string a2 = jSONValue.Get("download.key").AsString(true);
				bool resumeOK = a == url && a2 == key;
				JSONValue value = default(JSONValue);
				value["url"] = url;
				value["key"] = key;
				JSONValue jSONValue2 = default(JSONValue);
				jSONValue2["download"] = value;
				AssetStoreUtils.Download(package_id, url, destination, key, jSONValue2.ToString(), resumeOK, doneCallback);
			}
		}

		public static string[] PackageStorePath(string publisher_name, string category_name, string package_name, string package_id, string url)
		{
			string[] array = new string[]
			{
				publisher_name,
				category_name,
				package_name
			};
			for (int i = 0; i < 3; i++)
			{
				array[i] = AssetStoreContext.s_InvalidPathCharsRegExp.Replace(array[i], "");
			}
			if (array[2] == "")
			{
				array[2] = AssetStoreContext.s_InvalidPathCharsRegExp.Replace(package_id, "");
			}
			if (array[2] == "")
			{
				array[2] = AssetStoreContext.s_InvalidPathCharsRegExp.Replace(url, "");
			}
			return array;
		}

		public AssetStoreContext.PackageList GetPackageList()
		{
			Dictionary<string, AssetStoreContext.Package> dictionary = new Dictionary<string, AssetStoreContext.Package>();
			PackageInfo[] packageList = PackageInfo.GetPackageList();
			PackageInfo[] array = packageList;
			int i = 0;
			while (i < array.Length)
			{
				PackageInfo packageInfo = array[i];
				AssetStoreContext.Package package = new AssetStoreContext.Package();
				if (packageInfo.jsonInfo == "")
				{
					package.title = Path.GetFileNameWithoutExtension(packageInfo.packagePath);
					package.id = packageInfo.packagePath;
					if (this.IsBuiltinStandardAsset(packageInfo.packagePath))
					{
						package.publisher = new AssetStoreContext.LabelAndId
						{
							label = "Unity Technologies",
							id = "1"
						};
						package.category = new AssetStoreContext.LabelAndId
						{
							label = "Prefab Packages",
							id = "4"
						};
						package.version = "3.5.0.0";
					}
					goto IL_14F;
				}
				JSONValue json = JSONParser.SimpleParse(packageInfo.jsonInfo);
				if (!json.IsNull())
				{
					package.Initialize(json);
					if (package.id == null)
					{
						JSONValue jSONValue = json.Get("link.id");
						if (!jSONValue.IsNull())
						{
							package.id = jSONValue.AsString();
						}
						else
						{
							package.id = packageInfo.packagePath;
						}
					}
					goto IL_14F;
				}
				IL_211:
				i++;
				continue;
				IL_14F:
				package.local_icon = packageInfo.iconURL;
				package.local_path = packageInfo.packagePath;
				if (!dictionary.ContainsKey(package.id) || dictionary[package.id].version_id == null || dictionary[package.id].version_id == "-1" || (package.version_id != null && package.version_id != "-1" && int.Parse(dictionary[package.id].version_id) <= int.Parse(package.version_id)))
				{
					dictionary[package.id] = package;
				}
				goto IL_211;
			}
			AssetStoreContext.Package[] results = dictionary.Values.ToArray<AssetStoreContext.Package>();
			return new AssetStoreContext.PackageList
			{
				results = results
			};
		}

		private bool IsBuiltinStandardAsset(string path)
		{
			return AssetStoreContext.s_StandardPackageRegExp.IsMatch(path);
		}
	}
}
