using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.Utils;
using UnityEditorInternal;

namespace UnityEditor.Scripting.Compilers
{
	internal static class UWPReferences
	{
		private sealed class UWPExtension
		{
			public string Name
			{
				get;
				private set;
			}

			public string[] References
			{
				get;
				private set;
			}

			public UWPExtension(string manifest, string windowsKitsFolder, string sdkVersion)
			{
				XDocument xDocument = XDocument.Load(manifest);
				XElement xElement = xDocument.Element("FileList");
				if (xElement.Attribute("TargetPlatform").Value != "UAP")
				{
					throw new Exception(string.Format("Invalid extension manifest at \"{0}\".", manifest));
				}
				this.Name = xElement.Attribute("DisplayName").Value;
				XElement containedApiContractsElement = xElement.Element("ContainedApiContracts");
				this.References = UWPReferences.GetReferences(windowsKitsFolder, sdkVersion, containedApiContractsElement);
			}
		}

		public static string[] GetReferences(Version sdkVersion)
		{
			string windowsKit = UWPReferences.GetWindowsKit10();
			string[] result;
			if (string.IsNullOrEmpty(windowsKit))
			{
				result = new string[0];
			}
			else
			{
				string text = UWPReferences.SdkVersionToString(sdkVersion);
				HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
				string text2 = UWPReferences.CombinePaths(new string[]
				{
					windowsKit,
					"UnionMetadata",
					text,
					"Facade",
					"Windows.winmd"
				});
				if (!File.Exists(text2))
				{
					text2 = UWPReferences.CombinePaths(new string[]
					{
						windowsKit,
						"UnionMetadata",
						"Facade",
						"Windows.winmd"
					});
				}
				hashSet.Add(text2);
				string[] platform = UWPReferences.GetPlatform(windowsKit, text);
				for (int i = 0; i < platform.Length; i++)
				{
					string item = platform[i];
					hashSet.Add(item);
				}
				UWPReferences.UWPExtension[] extensions = UWPReferences.GetExtensions(windowsKit, text);
				for (int j = 0; j < extensions.Length; j++)
				{
					UWPReferences.UWPExtension uWPExtension = extensions[j];
					string[] references = uWPExtension.References;
					for (int k = 0; k < references.Length; k++)
					{
						string item2 = references[k];
						hashSet.Add(item2);
					}
				}
				result = hashSet.ToArray<string>();
			}
			return result;
		}

		public static IEnumerable<UWPExtensionSDK> GetExtensionSDKs(Version sdkVersion)
		{
			string windowsKit = UWPReferences.GetWindowsKit10();
			IEnumerable<UWPExtensionSDK> result;
			if (string.IsNullOrEmpty(windowsKit))
			{
				result = new UWPExtensionSDK[0];
			}
			else
			{
				result = UWPReferences.GetExtensionSDKs(windowsKit, UWPReferences.SdkVersionToString(sdkVersion));
			}
			return result;
		}

		private static string SdkVersionToString(Version version)
		{
			string text = version.ToString();
			if (version.Minor == -1)
			{
				text += ".0";
			}
			if (version.Build == -1)
			{
				text += ".0";
			}
			if (version.Revision == -1)
			{
				text += ".0";
			}
			return text;
		}

		public static IEnumerable<Version> GetInstalledSDKVersions()
		{
			string windowsKit = UWPReferences.GetWindowsKit10();
			IEnumerable<Version> result;
			if (string.IsNullOrEmpty(windowsKit))
			{
				result = new Version[0];
			}
			else
			{
				string[] files = Directory.GetFiles(UWPReferences.CombinePaths(new string[]
				{
					windowsKit,
					"Platforms",
					"UAP"
				}), "*", SearchOption.AllDirectories);
				IEnumerable<string> enumerable = from f in files
				where string.Equals("Platform.xml", Path.GetFileName(f), StringComparison.OrdinalIgnoreCase)
				select f;
				List<Version> list = new List<Version>();
				foreach (string current in enumerable)
				{
					XDocument xDocument;
					try
					{
						xDocument = XDocument.Load(current);
					}
					catch
					{
						continue;
					}
					foreach (XNode current2 in xDocument.Nodes())
					{
						XElement xElement = current2 as XElement;
						if (xElement != null)
						{
							Version item;
							if (UWPReferences.FindVersionInNode(xElement, out item))
							{
								list.Add(item);
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		private static bool FindVersionInNode(XElement node, out Version version)
		{
			bool result;
			for (XAttribute xAttribute = node.FirstAttribute; xAttribute != null; xAttribute = xAttribute.NextAttribute)
			{
				if (string.Equals(xAttribute.Name.LocalName, "version", StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						version = new Version(xAttribute.Value);
						result = true;
						return result;
					}
					catch
					{
					}
				}
			}
			version = null;
			result = false;
			return result;
		}

		private static string[] GetPlatform(string folder, string version)
		{
			string text = UWPReferences.CombinePaths(new string[]
			{
				folder,
				"Platforms\\UAP",
				version,
				"Platform.xml"
			});
			XDocument xDocument = XDocument.Load(text);
			XElement xElement = xDocument.Element("ApplicationPlatform");
			if (xElement.Attribute("name").Value != "UAP")
			{
				throw new Exception(string.Format("Invalid platform manifest at \"{0}\".", text));
			}
			XElement containedApiContractsElement = xElement.Element("ContainedApiContracts");
			return UWPReferences.GetReferences(folder, version, containedApiContractsElement);
		}

		private static string CombinePaths(params string[] paths)
		{
			return Paths.Combine(paths);
		}

		private static IEnumerable<UWPExtensionSDK> GetExtensionSDKs(string sdkFolder, string sdkVersion)
		{
			List<UWPExtensionSDK> list = new List<UWPExtensionSDK>();
			string path = Path.Combine(sdkFolder, "Extension SDKs");
			IEnumerable<UWPExtensionSDK> result;
			if (!Directory.Exists(path))
			{
				result = new UWPExtensionSDK[0];
			}
			else
			{
				string[] directories = Directory.GetDirectories(path);
				for (int i = 0; i < directories.Length; i++)
				{
					string text = directories[i];
					string text2 = UWPReferences.CombinePaths(new string[]
					{
						text,
						sdkVersion,
						"SDKManifest.xml"
					});
					string fileName = Path.GetFileName(text);
					if (File.Exists(text2))
					{
						list.Add(new UWPExtensionSDK(fileName, sdkVersion, text2));
					}
					else if (fileName == "XboxLive")
					{
						text2 = UWPReferences.CombinePaths(new string[]
						{
							text,
							"1.0",
							"SDKManifest.xml"
						});
						if (File.Exists(text2))
						{
							list.Add(new UWPExtensionSDK(fileName, "1.0", text2));
						}
					}
				}
				result = list;
			}
			return result;
		}

		private static UWPReferences.UWPExtension[] GetExtensions(string windowsKitsFolder, string version)
		{
			List<UWPReferences.UWPExtension> list = new List<UWPReferences.UWPExtension>();
			foreach (UWPExtensionSDK current in UWPReferences.GetExtensionSDKs(windowsKitsFolder, version))
			{
				try
				{
					UWPReferences.UWPExtension item = new UWPReferences.UWPExtension(current.ManifestPath, windowsKitsFolder, version);
					list.Add(item);
				}
				catch
				{
				}
			}
			return list.ToArray();
		}

		private static string[] GetReferences(string windowsKitsFolder, string sdkVersion, XElement containedApiContractsElement)
		{
			List<string> list = new List<string>();
			foreach (XElement current in containedApiContractsElement.Elements("ApiContract"))
			{
				string value = current.Attribute("name").Value;
				string value2 = current.Attribute("version").Value;
				string text = UWPReferences.CombinePaths(new string[]
				{
					windowsKitsFolder,
					"References",
					sdkVersion,
					value,
					value2,
					value + ".winmd"
				});
				if (!File.Exists(text))
				{
					text = UWPReferences.CombinePaths(new string[]
					{
						windowsKitsFolder,
						"References",
						value,
						value2,
						value + ".winmd"
					});
					if (!File.Exists(text))
					{
						continue;
					}
				}
				list.Add(text);
			}
			return list.ToArray();
		}

		private static string GetWindowsKit10()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
			string text = Path.Combine(environmentVariable, "Windows Kits\\10\\");
			try
			{
				text = RegistryUtil.GetRegistryStringValue("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v10.0", "InstallationFolder", text, RegistryView._32);
			}
			catch
			{
			}
			string result;
			if (!Directory.Exists(text))
			{
				result = string.Empty;
			}
			else
			{
				result = text;
			}
			return result;
		}
	}
}
