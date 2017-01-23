using System;
using System.Collections.Generic;
using System.IO;

namespace UnityEditor.Scripting.Compilers
{
	internal sealed class NuGetPackageResolver
	{
		public string PackagesDirectory
		{
			get;
			set;
		}

		public string ProjectLockFile
		{
			get;
			set;
		}

		public string TargetMoniker
		{
			get;
			set;
		}

		public string[] ResolvedReferences
		{
			get;
			private set;
		}

		public NuGetPackageResolver()
		{
			this.TargetMoniker = "UAP,Version=v10.0";
		}

		private string ConvertToWindowsPath(string path)
		{
			return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		}

		public string[] Resolve()
		{
			string json = File.ReadAllText(this.ProjectLockFile);
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["targets"];
			Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary2[this.TargetMoniker];
			List<string> list = new List<string>();
			string path = this.ConvertToWindowsPath(this.GetPackagesPath());
			foreach (KeyValuePair<string, object> current in dictionary3)
			{
				Dictionary<string, object> dictionary4 = (Dictionary<string, object>)current.Value;
				object obj;
				if (dictionary4.TryGetValue("compile", out obj))
				{
					Dictionary<string, object> dictionary5 = (Dictionary<string, object>)obj;
					string[] array = current.Key.Split(new char[]
					{
						'/'
					});
					string path2 = array[0];
					string path3 = array[1];
					string text = Path.Combine(Path.Combine(path, path2), path3);
					if (!Directory.Exists(text))
					{
						throw new Exception(string.Format("Package directory not found: \"{0}\".", text));
					}
					foreach (string current2 in dictionary5.Keys)
					{
						if (!string.Equals(Path.GetFileName(current2), "_._", StringComparison.InvariantCultureIgnoreCase))
						{
							string text2 = Path.Combine(text, this.ConvertToWindowsPath(current2));
							if (!File.Exists(text2))
							{
								throw new Exception(string.Format("Reference not found: \"{0}\".", text2));
							}
							list.Add(text2);
						}
					}
					if (dictionary4.ContainsKey("frameworkAssemblies"))
					{
						throw new NotImplementedException("Support for \"frameworkAssemblies\" property has not been implemented yet.");
					}
				}
			}
			this.ResolvedReferences = list.ToArray();
			return this.ResolvedReferences;
		}

		private string GetPackagesPath()
		{
			string text = this.PackagesDirectory;
			string result;
			if (!string.IsNullOrEmpty(text))
			{
				result = text;
			}
			else
			{
				text = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
				if (!string.IsNullOrEmpty(text))
				{
					result = text;
				}
				else
				{
					string environmentVariable = Environment.GetEnvironmentVariable("USERPROFILE");
					result = Path.Combine(Path.Combine(environmentVariable, ".nuget"), "packages");
				}
			}
			return result;
		}
	}
}
