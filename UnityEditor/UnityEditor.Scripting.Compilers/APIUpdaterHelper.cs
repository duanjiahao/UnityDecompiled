using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Utils;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor.Scripting.Compilers
{
	internal class APIUpdaterHelper
	{
		private const string tempOutputPath = "Temp/ScriptUpdater/";
		public static void UpdateScripts(string responseFile, string sourceExtension)
		{
			if (!ScriptUpdatingManager.WaitForVCSServerConnection(true))
			{
				return;
			}
			string text = (!Provider.enabled) ? "." : "Temp/ScriptUpdater/";
			APIUpdaterHelper.RunUpdatingProgram("ScriptUpdater.exe", string.Concat(new string[]
			{
				sourceExtension,
				" ",
				CommandLineFormatter.PrepareFileName(MonoInstallationFinder.GetFrameWorksFolder()),
				" ",
				text,
				" ",
				responseFile
			}));
		}
		private static void RunUpdatingProgram(string executable, string arguments)
		{
			string executable2 = EditorApplication.applicationContentsPath + "/Tools/ScriptUpdater/" + executable;
			ManagedProgram managedProgram = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.5", executable2, arguments);
			managedProgram.LogProcessStartInfo();
			managedProgram.Start();
			managedProgram.WaitForExit();
			Console.WriteLine(string.Join(Environment.NewLine, managedProgram.GetStandardOutput()));
			if (managedProgram.ExitCode == 0)
			{
				APIUpdaterHelper.UpdateFilesInVCIfNeeded();
			}
			else
			{
				APIUpdaterHelper.ReportAPIUpdaterFailure(managedProgram.GetErrorOutput());
			}
		}
		private static void ReportAPIUpdaterFailure(IEnumerable<string> errorOutput)
		{
			Console.WriteLine("Failed to run script updater.\n ");
			foreach (string current in errorOutput)
			{
				if (current.StartsWith("unity.console:"))
				{
					Debug.LogError(current.Substring("unity.console:".Length));
				}
				else
				{
					Console.WriteLine(current);
				}
			}
			ScriptUpdatingManager.ReportExpectedUpdateFailure();
		}
		private static void UpdateFilesInVCIfNeeded()
		{
			if (!Provider.enabled)
			{
				return;
			}
			string[] files = Directory.GetFiles("Temp/ScriptUpdater/", "*.*", SearchOption.AllDirectories);
			AssetList assetList = new AssetList();
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				assetList.Add(Provider.GetAssetByPath(text.Replace("Temp/ScriptUpdater/", string.Empty)));
			}
			Task task = Provider.Checkout(assetList, CheckoutMode.Both);
			task.Wait();
			IEnumerable<Asset> source = 
				from a in task.assetList
				where (a.state & Asset.States.ReadOnly) == Asset.States.ReadOnly
				select a;
			if (!task.success || source.Any<Asset>())
			{
				string arg_103_0 = "[API Updater] Files cannot be updated (failed to checkout): {0}";
				object[] expr_BA = new object[1];
				expr_BA[0] = (
					from a in source
					select string.Concat(new object[]
					{
						a.fullName,
						" (",
						a.state,
						")"
					})).Aggregate((string acc, string curr) => acc + Environment.NewLine + "\t" + curr);
				Debug.LogErrorFormat(arg_103_0, expr_BA);
				ScriptUpdatingManager.ReportExpectedUpdateFailure();
				return;
			}
			FileUtil.CopyDirectoryRecursive("Temp/ScriptUpdater/", ".", true);
			FileUtil.DeleteFileOrDirectory("Temp/ScriptUpdater/");
		}
	}
}
