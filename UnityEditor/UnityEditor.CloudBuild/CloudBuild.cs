using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor.Utils;
using UnityEditor.Web;

namespace UnityEditor.CloudBuild
{
	[InitializeOnLoad]
	internal class CloudBuild
	{
		static CloudBuild()
		{
			JSProxyMgr.GetInstance().AddGlobalObject("unity/cloudbuild", new CloudBuild());
		}

		public Dictionary<string, Dictionary<string, string>> GetScmCandidates()
		{
			Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
			Dictionary<string, string> dictionary2 = this.DetectGit();
			if (dictionary2 != null)
			{
				dictionary.Add("git", dictionary2);
			}
			Dictionary<string, string> dictionary3 = this.DetectMercurial();
			if (dictionary3 != null)
			{
				dictionary.Add("mercurial", dictionary3);
			}
			Dictionary<string, string> dictionary4 = this.DetectSubversion();
			if (dictionary4 != null)
			{
				dictionary.Add("subversion", dictionary4);
			}
			Dictionary<string, string> dictionary5 = this.DetectPerforce();
			if (dictionary5 != null)
			{
				dictionary.Add("perforce", dictionary5);
			}
			return dictionary;
		}

		private Dictionary<string, string> DetectGit()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = this.RunCommand("git", "config --get remote.origin.url");
			Dictionary<string, string> result;
			if (string.IsNullOrEmpty(value))
			{
				result = null;
			}
			else
			{
				dictionary.Add("url", value);
				dictionary.Add("branch", this.RunCommand("git", "rev-parse --abbrev-ref HEAD"));
				dictionary.Add("root", this.RemoveProjectDirectory(this.RunCommand("git", "rev-parse --show-toplevel")));
				result = dictionary;
			}
			return result;
		}

		private Dictionary<string, string> DetectMercurial()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = this.RunCommand("hg", "paths default");
			Dictionary<string, string> result;
			if (string.IsNullOrEmpty(value))
			{
				result = null;
			}
			else
			{
				dictionary.Add("url", value);
				dictionary.Add("branch", this.RunCommand("hg", "branch"));
				dictionary.Add("root", this.RemoveProjectDirectory(this.RunCommand("hg", "root")));
				result = dictionary;
			}
			return result;
		}

		private Dictionary<string, string> DetectSubversion()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text = this.RunCommand("svn", "info");
			Dictionary<string, string> result;
			if (text == null)
			{
				result = null;
			}
			else
			{
				string[] array = text.Split(Environment.NewLine.ToCharArray());
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					string[] array3 = text2.Split(new char[]
					{
						':'
					}, 2);
					if (array3.Length == 2)
					{
						if (array3[0].Equals("Repository Root"))
						{
							dictionary.Add("url", array3[1].Trim());
						}
						if (array3[0].Equals("URL"))
						{
							dictionary.Add("branch", array3[1].Trim());
						}
						if (array3[0].Equals("Working Copy Root Path"))
						{
							dictionary.Add("root", this.RemoveProjectDirectory(array3[1].Trim()));
						}
					}
				}
				if (!dictionary.ContainsKey("url"))
				{
					result = null;
				}
				else
				{
					result = dictionary;
				}
			}
			return result;
		}

		private Dictionary<string, string> DetectPerforce()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string environmentVariable = Environment.GetEnvironmentVariable("P4PORT");
			Dictionary<string, string> result;
			if (string.IsNullOrEmpty(environmentVariable))
			{
				result = null;
			}
			else
			{
				dictionary.Add("url", environmentVariable);
				string environmentVariable2 = Environment.GetEnvironmentVariable("P4CLIENT");
				if (!string.IsNullOrEmpty(environmentVariable2))
				{
					dictionary.Add("workspace", environmentVariable2);
				}
				result = dictionary;
			}
			return result;
		}

		private string RunCommand(string command, string arguments)
		{
			string result;
			try
			{
				Program program = new Program(new ProcessStartInfo(command)
				{
					Arguments = arguments
				});
				program.Start();
				program.WaitForExit();
				if (program.ExitCode < 0)
				{
					result = null;
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					string[] standardOutput = program.GetStandardOutput();
					for (int i = 0; i < standardOutput.Length; i++)
					{
						string value = standardOutput[i];
						stringBuilder.AppendLine(value);
					}
					result = stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
				}
			}
			catch (Win32Exception)
			{
				result = null;
			}
			return result;
		}

		private string RemoveProjectDirectory(string workingDirectory)
		{
			string text = Directory.GetCurrentDirectory();
			if (text.StartsWith(workingDirectory.Replace('/', '\\')))
			{
				workingDirectory = workingDirectory.Replace('/', '\\');
			}
			text = text.Replace(workingDirectory, "");
			return text.Trim(new char[]
			{
				Path.DirectorySeparatorChar
			});
		}
	}
}
