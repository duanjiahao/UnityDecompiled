using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Utils;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class AssemblyStripper
	{
		private readonly string[] _assemblies;
		private readonly string[] _searchDirs;
		internal static string MonoLinkerPath
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform != RuntimePlatform.WindowsEditor)
				{
					return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/UnusedByteCodeStripper/UnusedBytecodeStripper.exe");
				}
				return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/UnusedBytecodeStripper.exe");
			}
		}
		internal static string MonoLinker2Path
		{
			get
			{
				return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/UnusedByteCodeStripper2/UnusedBytecodeStripper2.exe");
			}
		}
		internal static string BlacklistPath
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(AssemblyStripper.MonoLinkerPath), "native_link.xml");
			}
		}
		private AssemblyStripper(string[] assemblies, string[] searchDirs)
		{
			this._assemblies = assemblies;
			this._searchDirs = searchDirs;
		}
		internal static bool Strip(string[] assemblies, string[] searchDirs, string outputFolder, string workingDirectory, out string output, out string error, string monoLinkerPath, string descriptorsFolder, IEnumerable<string> additionalBlacklist)
		{
			return new AssemblyStripper(assemblies, searchDirs).Strip(outputFolder, workingDirectory, out output, out error, monoLinkerPath, descriptorsFolder, additionalBlacklist);
		}
		private bool Strip(string outputFolder, string workingDirectory, out string output, out string error, string monoLinkerPath, string descriptorsFolder, IEnumerable<string> additionalBlacklist)
		{
			this.BackupInputFolderIfNeeded(outputFolder);
			return this.StripAssembliesTo(outputFolder, workingDirectory, out output, out error, monoLinkerPath, descriptorsFolder, additionalBlacklist);
		}
		private void BackupInputFolderIfNeeded(string outputFolder)
		{
			string fullOutput = AssemblyStripper.FullPathOf(outputFolder);
			if ((
				from a in this._assemblies
				select AssemblyStripper.FullPathOf(Path.GetDirectoryName(a))).All((string p) => string.Compare(p, fullOutput) != 0))
			{
				return;
			}
			string text = Path.Combine(outputFolder, "not-stripped");
			FileUtil.CreateOrCleanDirectory(text);
			string[] files = Directory.GetFiles(outputFolder);
			for (int i = 0; i < files.Length; i++)
			{
				string text2 = files[i];
				File.Copy(text2, Path.Combine(text, Path.GetFileName(text2)));
			}
		}
		private static string FullPathOf(string dir)
		{
			return Path.GetFullPath(dir).TrimEnd(new char[]
			{
				'\\'
			});
		}
		private bool StripAssembliesTo(string outputFolder, string workingDirectory, out string output, out string error, string linkerPath, string descriptorsFolder, IEnumerable<string> additionalBlacklist)
		{
			if (!Directory.Exists(outputFolder))
			{
				Directory.CreateDirectory(outputFolder);
			}
			additionalBlacklist = (
				from s in additionalBlacklist
				select (!Path.IsPathRooted(s)) ? Path.Combine(workingDirectory, s) : s).Where(new Func<string, bool>(File.Exists));
			IEnumerable<string> enumerable = 
				from s in Directory.GetFiles("Assets", "link.xml", SearchOption.AllDirectories)
				select Path.Combine(Directory.GetCurrentDirectory(), s);
			foreach (string current in enumerable)
			{
				Console.WriteLine("UserBlackList: " + current);
			}
			additionalBlacklist = additionalBlacklist.Concat(enumerable);
			List<string> list = new List<string>
			{
				"-out \"" + outputFolder + "\"",
				"-l none",
				"-c link",
				"-x \"" + AssemblyStripper.BlacklistPath + "\"",
				"-f \"" + descriptorsFolder + "\""
			};
			list.AddRange(
				from path in additionalBlacklist
				select "-x \"" + path + "\"");
			list.AddRange(
				from d in this._searchDirs
				select "-d \"" + d + "\"");
			list.AddRange(
				from assembly in this._assemblies
				select "-a  \"" + Path.GetFullPath(assembly) + "\"");
			return AssemblyStripper.RunAssemblyLinker(list, out output, out error, linkerPath, workingDirectory);
		}
		private static bool RunAssemblyLinker(IEnumerable<string> args, out string @out, out string err, string linkerPath, string workingDirectory)
		{
			string text = args.Aggregate((string buff, string s) => buff + " " + s);
			Console.WriteLine("Invoking UnusedByteCodeStripper2 with arguments: " + text);
			Runner.RunManagedProgram(linkerPath, text, workingDirectory, null);
			@out = string.Empty;
			err = string.Empty;
			return true;
		}
	}
}
