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
			if (ScriptUpdatingManager.WaitForVCSServerConnection(true))
			{
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
		}

		private static void RunUpdatingProgram(string executable, string arguments)
		{
			string executable2 = EditorApplication.applicationContentsPath + "/Tools/ScriptUpdater/" + executable;
			ManagedProgram managedProgram = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), null, executable2, arguments, false, null);
			managedProgram.LogProcessStartInfo();
			managedProgram.Start();
			managedProgram.WaitForExit();
			Console.WriteLine(string.Join(Environment.NewLine, managedProgram.GetStandardOutput()));
			APIUpdaterHelper.HandleUpdaterReturnValue(managedProgram);
		}

		private static void HandleUpdaterReturnValue(ManagedProgram program)
		{
			if (program.ExitCode == 0)
			{
				Console.WriteLine(string.Join(Environment.NewLine, program.GetErrorOutput()));
				APIUpdaterHelper.UpdateFilesInVCIfNeeded();
			}
			else
			{
				ScriptUpdatingManager.ReportExpectedUpdateFailure();
				if (program.ExitCode > 0)
				{
					APIUpdaterHelper.ReportAPIUpdaterFailure(program.GetErrorOutput());
				}
				else
				{
					APIUpdaterHelper.ReportAPIUpdaterCrash(program.GetErrorOutput());
				}
			}
		}

		private static void ReportAPIUpdaterCrash(IEnumerable<string> errorOutput)
		{
			string arg_3F_0 = "Failed to run script updater.{0}Please, report a bug to Unity with these details{0}{1}";
			object[] expr_0C = new object[2];
			expr_0C[0] = Environment.NewLine;
			expr_0C[1] = errorOutput.Aggregate("", (string acc, string curr) => acc + Environment.NewLine + "\t" + curr);
			Debug.LogErrorFormat(arg_3F_0, expr_0C);
		}

		private static void ReportAPIUpdaterFailure(IEnumerable<string> errorOutput)
		{
			string msg = string.Format("APIUpdater encountered some issues and was not able to finish.{0}{1}", Environment.NewLine, errorOutput.Aggregate("", (string acc, string curr) => acc + Environment.NewLine + "\t" + curr));
			ScriptUpdatingManager.ReportGroupedAPIUpdaterFailure(msg);
		}

		private static void UpdateFilesInVCIfNeeded()
		{
			if (Provider.enabled)
			{
				string[] files = Directory.GetFiles("Temp/ScriptUpdater/", "*.*", SearchOption.AllDirectories);
				AssetList assetList = new AssetList();
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					assetList.Add(Provider.GetAssetByPath(text.Replace("Temp/ScriptUpdater/", "")));
				}
				Task task = Provider.Checkout(assetList, CheckoutMode.Exact);
				task.Wait();
				IEnumerable<Asset> source = from a in task.assetList
				where (a.state & Asset.States.ReadOnly) == Asset.States.ReadOnly
				select a;
				if (!task.success || source.Any<Asset>())
				{
					string arg_10A_0 = "[API Updater] Files cannot be updated (failed to check out): {0}";
					object[] expr_C1 = new object[1];
					expr_C1[0] = (from a in source
					select string.Concat(new object[]
					{
						a.fullName,
						" (",
						a.state,
						")"
					})).Aggregate((string acc, string curr) => acc + Environment.NewLine + "\t" + curr);
					Debug.LogErrorFormat(arg_10A_0, expr_C1);
					ScriptUpdatingManager.ReportExpectedUpdateFailure();
				}
				else
				{
					FileUtil.CopyDirectoryRecursive("Temp/ScriptUpdater/", ".", true);
					FileUtil.DeleteFileOrDirectory("Temp/ScriptUpdater/");
				}
			}
		}
	}
}
