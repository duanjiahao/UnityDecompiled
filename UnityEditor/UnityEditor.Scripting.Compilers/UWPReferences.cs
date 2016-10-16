using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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

			public UWPExtension(string manifest, string referencesFolder)
			{
				XDocument xDocument = XDocument.Load(manifest);
				XElement xElement = xDocument.Element("FileList");
				if (xElement.Attribute("TargetPlatform").Value != "UAP")
				{
					throw new Exception(string.Format("Invalid extension manifest at \"{0}\".", manifest));
				}
				this.Name = xElement.Attribute("DisplayName").Value;
				XElement containedApiContractsElement = xElement.Element("ContainedApiContracts");
				this.References = UWPReferences.GetReferences(referencesFolder, containedApiContractsElement);
			}
		}

		public static string[] GetReferences()
		{
			string text;
			Version version;
			UWPReferences.GetWindowsKit10(out text, out version);
			string text2 = version.ToString();
			if (version.Minor == -1)
			{
				text2 += ".0";
			}
			if (version.Build == -1)
			{
				text2 += ".0";
			}
			if (version.Revision == -1)
			{
				text2 += ".0";
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			string item = Path.Combine(text, "UnionMetadata\\Facade\\Windows.winmd");
			hashSet.Add(item);
			string[] platform = UWPReferences.GetPlatform(text, text2);
			for (int i = 0; i < platform.Length; i++)
			{
				string item2 = platform[i];
				hashSet.Add(item2);
			}
			UWPReferences.UWPExtension[] extensions = UWPReferences.GetExtensions(text, text2);
			for (int j = 0; j < extensions.Length; j++)
			{
				UWPReferences.UWPExtension uWPExtension = extensions[j];
				string[] references = uWPExtension.References;
				for (int k = 0; k < references.Length; k++)
				{
					string item3 = references[k];
					hashSet.Add(item3);
				}
			}
			return hashSet.ToArray<string>();
		}

		private static string[] GetPlatform(string folder, string version)
		{
			string referencesFolder = Path.Combine(folder, "References");
			string text = FileUtil.CombinePaths(new string[]
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
			return UWPReferences.GetReferences(referencesFolder, containedApiContractsElement);
		}

		private static UWPReferences.UWPExtension[] GetExtensions(string folder, string version)
		{
			string path = Path.Combine(folder, "Extension SDKs");
			string referencesFolder = Path.Combine(folder, "References");
			List<UWPReferences.UWPExtension> list = new List<UWPReferences.UWPExtension>();
			string[] directories = Directory.GetDirectories(path);
			for (int i = 0; i < directories.Length; i++)
			{
				string text = directories[i];
				string text2 = FileUtil.CombinePaths(new string[]
				{
					text,
					version,
					"SDKManifest.xml"
				});
				if (File.Exists(text2))
				{
					try
					{
						UWPReferences.UWPExtension item = new UWPReferences.UWPExtension(text2, referencesFolder);
						list.Add(item);
					}
					catch
					{
					}
				}
			}
			return list.ToArray();
		}

		private static string[] GetReferences(string referencesFolder, XElement containedApiContractsElement)
		{
			List<string> list = new List<string>();
			foreach (XElement current in containedApiContractsElement.Elements("ApiContract"))
			{
				string value = current.Attribute("name").Value;
				string value2 = current.Attribute("version").Value;
				string text = FileUtil.CombinePaths(new string[]
				{
					referencesFolder,
					value,
					value2,
					value + ".winmd"
				});
				if (File.Exists(text))
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		private static void GetWindowsKit10(out string folder, out Version version)
		{
			string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
			folder = Path.Combine(environmentVariable, "Windows Kits\\10\\");
			version = new Version(10, 0, 10240);
			try
			{
				folder = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v10.0", "InstallationFolder", folder);
				string registryStringValue = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v10.0", "ProductVersion", version.ToString());
				version = new Version(registryStringValue);
			}
			catch
			{
			}
		}
	}
}
