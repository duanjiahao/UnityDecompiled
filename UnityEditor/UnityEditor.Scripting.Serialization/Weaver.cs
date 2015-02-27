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
		private static ManagedProgram ManagedProgramFor(string exe, string arguments)
		{
			return new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", exe, arguments);
		}
	}
}
