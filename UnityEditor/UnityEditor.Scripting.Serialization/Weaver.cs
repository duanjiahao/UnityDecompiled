using System;
using System.IO;
using System.Linq;
using UnityEditor.Utils;
namespace UnityEditor.Scripting.Serialization
{
	internal static class Weaver
	{
		public static bool ShouldWeave(string name)
		{
			return !name.Contains("Boo.") && !name.Contains("Mono.") && !name.Contains("System") && name.EndsWith(".dll");
		}
		public static void WeaveAssembliesInFolder(string folder, string playerPackage)
		{
			string unityEngine = Path.Combine(folder, "UnityEngine.dll");
			foreach (string current in 
				from f in Directory.GetFiles(folder)
				where Weaver.ShouldWeave(Path.GetFileName(f))
				select f)
			{
				Weaver.WeaveInto(current, current, unityEngine, playerPackage);
			}
		}
		public static bool WeaveUnetFromEditor(string assemblyPath, string destPath, string unityEngine, string unityUNet)
		{
			Console.WriteLine("WeaveUnetFromEditor " + assemblyPath);
			return Weaver.WeaveUNetInto(assemblyPath, assemblyPath, unityEngine, unityUNet);
		}
		public static void WeaveInto(string assemblyPath, string destPath, string unityEngine, string playerPackage)
		{
			string arguments = Weaver.CommandLineArgsFor(assemblyPath, destPath, unityEngine);
			using (ManagedProgram managedProgram = Weaver.SerializationWeaverProgramWith(arguments, playerPackage))
			{
				try
				{
					managedProgram.Start();
				}
				catch
				{
					managedProgram.LogProcessStartInfo();
					throw new Exception("Could not start SerializationWeaver.exe");
				}
				managedProgram.WaitForExit();
				if (managedProgram.ExitCode != 0)
				{
					throw new Exception("Failed running SerializationWeaver. output was: " + managedProgram.GetAllOutput());
				}
			}
		}
		public static bool WeaveUNetInto(string assemblyPath, string destPath, string unityEngine, string unityUNet)
		{
			string text = Weaver.CommandLineArgsFor(assemblyPath, destPath, unityEngine);
			text = text + " -unity-unet=" + unityUNet;
			string frameWorksFolder = MonoInstallationFinder.GetFrameWorksFolder();
			Console.Write(string.Concat(new string[]
			{
				"WeaveUNetInto ",
				assemblyPath,
				" using ",
				frameWorksFolder,
				"/UNetWeaver/UNetWeaver.exe ",
				text
			}));
			bool result;
			using (ManagedProgram managedProgram = Weaver.UNetWeaverProgramWith(text, frameWorksFolder))
			{
				try
				{
					managedProgram.Start();
				}
				catch
				{
					managedProgram.LogProcessStartInfo();
					throw new Exception("Could not start UNetWeaver.exe");
				}
				managedProgram.WaitForExit();
				Console.Write(managedProgram.GetAllOutput());
				if (managedProgram.ExitCode == 0)
				{
					string allOutput = managedProgram.GetAllOutput();
					string text2 = "COMPILE_WARNING ";
					string[] array = allOutput.Split(new char[]
					{
						'\n'
					});
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text3 = array2[i];
						if (text3.Length >= text2.Length)
						{
							if (text3.Substring(0, text2.Length) == text2)
							{
								string msg = text3.Substring(text2.Length);
								EditorApplication.ReportUNetWeaver("file", msg, false);
							}
						}
					}
					result = true;
				}
				else
				{
					string allOutput2 = managedProgram.GetAllOutput();
					string text4 = "COMPILE_ERROR ";
					string text5 = "COMPILE_WARNING ";
					string[] array3 = allOutput2.Split(new char[]
					{
						'\n'
					});
					string[] array4 = array3;
					for (int j = 0; j < array4.Length; j++)
					{
						string text6 = array4[j];
						if (text6.Length >= text5.Length)
						{
							if (text6.Substring(0, text4.Length) == text4)
							{
								string msg2 = text6.Substring(text4.Length);
								EditorApplication.ReportUNetWeaver("file", msg2, true);
							}
							if (text6.Substring(0, text5.Length) == text5)
							{
								string msg3 = text6.Substring(text5.Length);
								EditorApplication.ReportUNetWeaver("file", msg3, false);
							}
						}
					}
					result = false;
				}
			}
			return result;
		}
		private static string CommandLineArgsFor(string assemblyPath, string destPath, string unityEngine)
		{
			return string.Concat(new string[]
			{
				assemblyPath,
				" ",
				Path.GetDirectoryName(destPath),
				" -unity-engine=\"",
				unityEngine,
				"\""
			});
		}
		private static ManagedProgram SerializationWeaverProgramWith(string arguments, string playerPackage)
		{
			return Weaver.ManagedProgramFor(playerPackage + "/SerializationWeaver/SerializationWeaver.exe", arguments);
		}
		private static ManagedProgram UNetWeaverProgramWith(string arguments, string playerPackage)
		{
			return Weaver.ManagedProgramFor(playerPackage + "/UNetWeaver/UNetWeaver.exe", arguments);
		}
		private static ManagedProgram ManagedProgramFor(string exe, string arguments)
		{
			return new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", exe, arguments);
		}
	}
}
