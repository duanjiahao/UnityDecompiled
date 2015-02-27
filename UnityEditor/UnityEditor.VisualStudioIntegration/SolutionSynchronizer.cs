using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor.Scripting;
using UnityEditorInternal;
namespace UnityEditor.VisualStudioIntegration
{
	internal class SolutionSynchronizer
	{
		public static readonly ISolutionSynchronizationSettings DefaultSynchronizationSettings = new DefaultSolutionSynchronizationSettings();
		private static readonly string WindowsNewline = "\r\n";
		private static readonly Dictionary<string, ScriptingLanguage> SupportedExtensions = new Dictionary<string, ScriptingLanguage>
		{

			{
				".cs",
				ScriptingLanguage.CSharp
			},

			{
				".js",
				ScriptingLanguage.UnityScript
			},

			{
				".boo",
				ScriptingLanguage.Boo
			},

			{
				".xml",
				ScriptingLanguage.None
			},

			{
				".shader",
				ScriptingLanguage.None
			},

			{
				".compute",
				ScriptingLanguage.None
			},

			{
				".cginc",
				ScriptingLanguage.None
			},

			{
				".glslinc",
				ScriptingLanguage.None
			},

			{
				".txt",
				ScriptingLanguage.None
			},

			{
				".fnt",
				ScriptingLanguage.None
			},

			{
				".cd",
				ScriptingLanguage.None
			},

			{
				"cs",
				ScriptingLanguage.CSharp
			},

			{
				"js",
				ScriptingLanguage.UnityScript
			},

			{
				"boo",
				ScriptingLanguage.Boo
			},

			{
				"xml",
				ScriptingLanguage.None
			},

			{
				"shader",
				ScriptingLanguage.None
			},

			{
				"compute",
				ScriptingLanguage.None
			},

			{
				"cginc",
				ScriptingLanguage.None
			},

			{
				"glslinc",
				ScriptingLanguage.None
			},

			{
				"txt",
				ScriptingLanguage.None
			},

			{
				"fnt",
				ScriptingLanguage.None
			},

			{
				"cd",
				ScriptingLanguage.None
			}
		};
		private static readonly Dictionary<ScriptingLanguage, string> ProjectExtensions = new Dictionary<ScriptingLanguage, string>
		{

			{
				ScriptingLanguage.Boo,
				".booproj"
			},

			{
				ScriptingLanguage.CSharp,
				".csproj"
			},

			{
				ScriptingLanguage.UnityScript,
				".unityproj"
			},

			{
				ScriptingLanguage.None,
				".csproj"
			}
		};
		private static readonly Regex _MonoDevelopPropertyHeader = new Regex("^\\s*GlobalSection\\(MonoDevelopProperties.*\\)");
		public static readonly string MSBuildNamespaceUri = "http://schemas.microsoft.com/developer/msbuild/2003";
		private readonly string _projectDirectory;
		private readonly ISolutionSynchronizationSettings _settings;
		private readonly string _projectName;
		private readonly string vsstub = "-vs";
		private Dictionary<string, string> _pathCache;
		private static readonly string DefaultMonoDevelopSolutionProperties = string.Join("\r\n", new string[]
		{
			"GlobalSection(MonoDevelopProperties) = preSolution",
			"\t\tStartupItem = Assembly-CSharp.csproj",
			"\t\tPolicies = $0",
			"\t\t$0.TextStylePolicy = $1",
			"\t\t$1.inheritsSet = null",
			"\t\t$1.scope = text/x-csharp",
			"\t\t$0.CSharpFormattingPolicy = $2",
			"\t\t$2.inheritsSet = Mono",
			"\t\t$2.inheritsScope = text/x-csharp",
			"\t\t$2.scope = text/x-csharp",
			"\t\t$0.TextStylePolicy = $3",
			"\t\t$3.FileWidth = 120",
			"\t\t$3.TabWidth = 4",
			"\t\t$3.IndentWidth = 4",
			"\t\t$3.EolMarker = Unix",
			"\t\t$3.inheritsSet = Mono",
			"\t\t$3.inheritsScope = text/plain",
			"\t\t$3.scope = text/plain",
			"\tEndGlobalSection"
		});
		public static readonly Regex scriptReferenceExpression = new Regex("^Library.ScriptAssemblies.(?<project>Assembly-(?<language>[^-]+)(?<editor>-Editor)?(?<firstpass>-firstpass)?).dll$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public SolutionSynchronizer(string projectDirectory, ISolutionSynchronizationSettings settings)
		{
			this._projectDirectory = projectDirectory;
			this._settings = settings;
			this._projectName = Path.GetFileName(this._projectDirectory);
			this._pathCache = new Dictionary<string, string>();
		}
		public SolutionSynchronizer(string projectDirectory) : this(projectDirectory, SolutionSynchronizer.DefaultSynchronizationSettings)
		{
		}
		public bool ShouldFileBePartOfSolution(string file)
		{
			string extension = Path.GetExtension(file);
			return extension == ".dll" || SolutionSynchronizer.SupportedExtensions.ContainsKey(extension);
		}
		public bool ProjectExists(MonoIsland island)
		{
			return File.Exists(this.ProjectFile(island, false));
		}
		public bool SolutionExists()
		{
			return File.Exists(this.SolutionFile(false));
		}
		private static void DumpIsland(MonoIsland island)
		{
			Console.WriteLine("{0} ({1})", island._output, island._classlib_profile);
			Console.WriteLine("Files: ");
			Console.WriteLine(string.Join("\n", island._files));
			Console.WriteLine("References: ");
			Console.WriteLine(string.Join("\n", island._references));
			Console.WriteLine(string.Empty);
		}
		public bool SyncIfNeeded(IEnumerable<string> affectedFiles)
		{
			if (this.SolutionExists() && affectedFiles.Any(new Func<string, bool>(this.ShouldFileBePartOfSolution)))
			{
				this.Sync();
				return true;
			}
			return false;
		}
		public void Sync()
		{
			IEnumerable<MonoIsland> enumerable = 
				from i in InternalEditorUtility.GetMonoIslands()
				where 0 < i._files.Length
				select i;
			string otherAssetsProjectPart = this.GenerateAllAssetProjectPart();
			this.SyncSolution(enumerable);
			foreach (MonoIsland current in enumerable)
			{
				this.SyncProject(current, otherAssetsProjectPart);
			}
		}
		private string GenerateAllAssetProjectPart()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
			for (int i = 0; i < allAssetPaths.Length; i++)
			{
				string text = allAssetPaths[i];
				string extension = Path.GetExtension(text);
				if (SolutionSynchronizer.SupportedExtensions.ContainsKey(extension) && SolutionSynchronizer.SupportedExtensions[extension] == ScriptingLanguage.None)
				{
					stringBuilder.AppendFormat("     <None Include=\"{0}\" />{1}", this.EscapedRelativePathFor(text), SolutionSynchronizer.WindowsNewline);
				}
			}
			return stringBuilder.ToString();
		}
		private void SyncProject(MonoIsland island, string otherAssetsProjectPart)
		{
			SolutionSynchronizer.SyncFileIfNotChanged(this.ProjectFile(island, false), this.ProjectText(island, false, otherAssetsProjectPart));
			SolutionSynchronizer.SyncFileIfNotChanged(this.ProjectFile(island, true), this.ProjectText(island, true, otherAssetsProjectPart));
		}
		private static void SyncFileIfNotChanged(string filename, string newContents)
		{
			if (File.Exists(filename) && newContents == File.ReadAllText(filename))
			{
				return;
			}
			File.WriteAllText(filename, newContents);
		}
		private string ProjectText(MonoIsland island, bool forVisualStudio, string allAssetsProject)
		{
			StringBuilder stringBuilder = new StringBuilder(this.ProjectHeader(island));
			List<string> list = new List<string>();
			List<Match> list2 = new List<Match>();
			string[] files = island._files;
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i];
				string b = Path.GetExtension(text).ToLower();
				string text2 = (!Path.IsPathRooted(text)) ? Path.Combine(this._projectDirectory, text) : text;
				if (".dll" != b)
				{
					string arg = "Compile";
					stringBuilder.AppendFormat("     <{0} Include=\"{1}\" />{2}", arg, this.EscapedRelativePathFor(text2), SolutionSynchronizer.WindowsNewline);
				}
				else
				{
					list.Add(text2);
				}
			}
			stringBuilder.Append(allAssetsProject);
			foreach (string current in list.Union(island._references))
			{
				if (!current.EndsWith("/UnityEditor.dll") && !current.EndsWith("/UnityEngine.dll") && !current.EndsWith("\\UnityEditor.dll") && !current.EndsWith("\\UnityEngine.dll"))
				{
					Match match;
					if ((match = SolutionSynchronizer.scriptReferenceExpression.Match(current)) != null && match.Success && (!forVisualStudio || (int)Enum.Parse(typeof(ScriptingLanguage), match.Groups["language"].Value, true) == 2))
					{
						list2.Add(match);
					}
					else
					{
						string text3 = (!Path.IsPathRooted(current)) ? Path.Combine(this._projectDirectory, current) : current;
						if (AssemblyHelper.IsManagedAssembly(text3))
						{
							if (!AssemblyHelper.IsInternalAssembly(text3))
							{
								text3 = text3.Replace("\\", "/");
								text3 = text3.Replace("\\\\", "/");
								stringBuilder.AppendFormat(" <Reference Include=\"{0}\">{1}", Path.GetFileNameWithoutExtension(text3), SolutionSynchronizer.WindowsNewline);
								stringBuilder.AppendFormat(" <HintPath>{0}</HintPath>{1}", text3, SolutionSynchronizer.WindowsNewline);
								stringBuilder.AppendFormat(" </Reference>{0}", SolutionSynchronizer.WindowsNewline);
							}
						}
					}
				}
			}
			if (0 < list2.Count)
			{
				stringBuilder.AppendLine("  </ItemGroup>");
				stringBuilder.AppendLine("  <ItemGroup>");
				foreach (Match current2 in list2)
				{
					string text4 = current2.Groups["project"].Value;
					if (forVisualStudio)
					{
						text4 += this.vsstub;
					}
					stringBuilder.AppendFormat("    <ProjectReference Include=\"{0}{1}\">{2}", text4, SolutionSynchronizer.GetProjectExtension((ScriptingLanguage)((int)Enum.Parse(typeof(ScriptingLanguage), current2.Groups["language"].Value, true))), SolutionSynchronizer.WindowsNewline);
					stringBuilder.AppendFormat("      <Project>{{{0}}}</Project>", this.ProjectGuid(Path.Combine("Temp", current2.Groups["project"].Value + ".dll")), SolutionSynchronizer.WindowsNewline);
					stringBuilder.AppendFormat("      <Name>{0}</Name>", text4, SolutionSynchronizer.WindowsNewline);
					stringBuilder.AppendLine("    </ProjectReference>");
				}
			}
			stringBuilder.Append(this.ProjectFooter(island));
			return stringBuilder.ToString();
		}
		private string GetProperDirectoryCapitalization(DirectoryInfo dirInfo)
		{
			if (dirInfo.FullName == this._projectDirectory)
			{
				return dirInfo.FullName;
			}
			if (this._pathCache.ContainsKey(dirInfo.FullName))
			{
				return this._pathCache[dirInfo.FullName];
			}
			DirectoryInfo parent = dirInfo.Parent;
			if (parent == null)
			{
				return dirInfo.FullName.ToUpperInvariant();
			}
			string text = Path.Combine(this.GetProperDirectoryCapitalization(parent), parent.GetDirectories().First((DirectoryInfo dir) => dir.Name.Equals(dirInfo.Name, StringComparison.OrdinalIgnoreCase)).Name);
			this._pathCache[dirInfo.FullName] = text;
			return text;
		}
		public string ProjectFile(MonoIsland island, bool forVisualStudio)
		{
			ScriptingLanguage key = SolutionSynchronizer.SupportedExtensions[island.GetExtensionOfSourceFiles()];
			string arg = (!forVisualStudio) ? string.Empty : this.vsstub;
			return Path.Combine(this._projectDirectory, string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(island._output), arg, SolutionSynchronizer.ProjectExtensions[key]));
		}
		internal string SolutionFile(bool onlyCSharp)
		{
			string arg = (!onlyCSharp) ? string.Empty : "-csharp";
			return Path.Combine(this._projectDirectory, string.Format("{0}{1}.sln", this._projectName, arg));
		}
		private string AssetsFolder()
		{
			return Path.Combine(this._projectDirectory, "Assets");
		}
		private string ProjectHeader(MonoIsland island)
		{
			string text = "4.0";
			string text2 = "10.0.20506";
			ScriptingLanguage language = SolutionSynchronizer.SupportedExtensions[island.GetExtensionOfSourceFiles()];
			if (this._settings.VisualStudioVersion == 9)
			{
				text = "3.5";
				text2 = "9.0.21022";
			}
			return string.Format(this._settings.GetProjectHeaderTemplate(language), new object[]
			{
				text,
				text2,
				this.ProjectGuid(island._output),
				this._settings.EngineAssemblyPath,
				this._settings.EditorAssemblyPath,
				string.Join(";", this._settings.Defines.Union(island._defines).ToArray<string>()),
				SolutionSynchronizer.MSBuildNamespaceUri,
				Path.GetFileNameWithoutExtension(island._output)
			});
		}
		private void SyncSolution(IEnumerable<MonoIsland> islands)
		{
			SolutionSynchronizer.SyncFileIfNotChanged(this.SolutionFile(false), this.SolutionText(islands, false));
			SolutionSynchronizer.SyncFileIfNotChanged(this.SolutionFile(true), this.SolutionText(islands, true));
		}
		private string SolutionText(IEnumerable<MonoIsland> islands, bool onlyCSharp)
		{
			string text = "11.00";
			if (this._settings.VisualStudioVersion == 9)
			{
				text = "10.00";
			}
			IEnumerable<MonoIsland> enumerable = 
				from i in islands
				where !onlyCSharp || ScriptingLanguage.CSharp == SolutionSynchronizer.SupportedExtensions[i.GetExtensionOfSourceFiles()]
				select i;
			string projectEntries = this.GetProjectEntries(enumerable, onlyCSharp);
			string text2 = string.Join(SolutionSynchronizer.WindowsNewline, (
				from i in enumerable
				select this.GetProjectActiveConfigurations(this.ProjectGuid(i._output))).ToArray<string>());
			return string.Format(this._settings.SolutionTemplate, new object[]
			{
				text,
				projectEntries,
				text2,
				this.ReadExistingMonoDevelopSolutionProperties()
			});
		}
		private string GetProjectEntries(IEnumerable<MonoIsland> islands, bool forVisualStudio)
		{
			IEnumerable<string> source = 
				from i in islands
				select string.Format(SolutionSynchronizer.DefaultSynchronizationSettings.SolutionProjectEntryTemplate, new object[]
				{
					this.SolutionGuid(),
					this._projectName,
					Path.GetFileName(this.ProjectFile(i, forVisualStudio)),
					this.ProjectGuid(i._output)
				});
			return string.Join(SolutionSynchronizer.WindowsNewline, source.ToArray<string>());
		}
		private string GetProjectActiveConfigurations(string projectGuid)
		{
			return string.Format(SolutionSynchronizer.DefaultSynchronizationSettings.SolutionProjectConfigurationTemplate, projectGuid);
		}
		private string EscapedRelativePathFor(FileSystemInfo file)
		{
			return this.EscapedRelativePathFor(file.FullName);
		}
		private string EscapedRelativePathFor(string file)
		{
			string value = this._projectDirectory.Replace("/", "\\");
			file = file.Replace("/", "\\");
			return SecurityElement.Escape((!file.StartsWith(value)) ? file : file.Substring(this._projectDirectory.Length + 1));
		}
		private string ProjectGuid(string assembly)
		{
			return SolutionGuidGenerator.GuidForProject(this._projectName + Path.GetFileNameWithoutExtension(assembly));
		}
		private string SolutionGuid()
		{
			return SolutionGuidGenerator.GuidForSolution(this._projectName);
		}
		private string ProjectFooter(MonoIsland island)
		{
			ScriptingLanguage language = SolutionSynchronizer.SupportedExtensions[island.GetExtensionOfSourceFiles()];
			return string.Format(this._settings.GetProjectFooterTemplate(language), this.ReadExistingMonoDevelopProjectProperties(island));
		}
		private string ReadExistingMonoDevelopSolutionProperties()
		{
			if (!this.SolutionExists())
			{
				return SolutionSynchronizer.DefaultMonoDevelopSolutionProperties;
			}
			string[] array;
			try
			{
				array = File.ReadAllLines(this.SolutionFile(false));
			}
			catch (IOException)
			{
				string defaultMonoDevelopSolutionProperties = SolutionSynchronizer.DefaultMonoDevelopSolutionProperties;
				return defaultMonoDevelopSolutionProperties;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (SolutionSynchronizer._MonoDevelopPropertyHeader.IsMatch(text))
				{
					flag = true;
				}
				if (flag)
				{
					stringBuilder.AppendFormat("{0}{1}", text, SolutionSynchronizer.WindowsNewline);
					if (text.Contains("EndGlobalSection"))
					{
						flag = false;
					}
				}
			}
			if (0 < stringBuilder.Length)
			{
				return stringBuilder.ToString();
			}
			return SolutionSynchronizer.DefaultMonoDevelopSolutionProperties;
		}
		private string ReadExistingMonoDevelopProjectProperties(MonoIsland island)
		{
			if (!this.ProjectExists(island))
			{
				return string.Empty;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlNamespaceManager xmlNamespaceManager;
			try
			{
				xmlDocument.Load(this.ProjectFile(island, false));
				xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("msb", SolutionSynchronizer.MSBuildNamespaceUri);
			}
			catch (Exception ex)
			{
				if (ex is IOException || ex is XmlException)
				{
					string empty = string.Empty;
					return empty;
				}
				throw;
			}
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/msb:Project/msb:ProjectExtensions", xmlNamespaceManager);
			if (xmlNodeList.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XmlNode xmlNode in xmlNodeList)
			{
				stringBuilder.AppendLine(xmlNode.OuterXml);
			}
			return stringBuilder.ToString();
		}
		[Obsolete("Use AssemblyHelper.IsManagedAssembly")]
		public static bool IsManagedAssembly(string file)
		{
			return AssemblyHelper.IsManagedAssembly(file);
		}
		public static string GetProjectExtension(ScriptingLanguage language)
		{
			if (!SolutionSynchronizer.ProjectExtensions.ContainsKey(language))
			{
				throw new ArgumentException("Unsupported language", "language");
			}
			return SolutionSynchronizer.ProjectExtensions[language];
		}
	}
}
