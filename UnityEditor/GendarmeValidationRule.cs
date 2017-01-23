using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;

internal abstract class GendarmeValidationRule : IValidationRule
{
	private readonly string _gendarmeExePath;

	protected GendarmeValidationRule(string gendarmeExePath)
	{
		this._gendarmeExePath = gendarmeExePath;
	}

	public ValidationResult Validate(IEnumerable<string> userAssemblies, params object[] options)
	{
		string arguments = this.BuildGendarmeCommandLineArguments(userAssemblies);
		ValidationResult result = new ValidationResult
		{
			Success = true,
			Rule = this,
			CompilerMessages = null
		};
		try
		{
			result.Success = GendarmeValidationRule.StartManagedProgram(this._gendarmeExePath, arguments, new GendarmeOutputParser(), ref result.CompilerMessages);
		}
		catch (Exception ex)
		{
			result.Success = false;
			result.CompilerMessages = new CompilerMessage[]
			{
				new CompilerMessage
				{
					file = "Exception",
					message = ex.Message,
					line = 0,
					column = 0,
					type = CompilerMessageType.Error
				}
			};
		}
		return result;
	}

	protected abstract GendarmeOptions ConfigureGendarme(IEnumerable<string> userAssemblies);

	protected string BuildGendarmeCommandLineArguments(IEnumerable<string> userAssemblies)
	{
		GendarmeOptions gendarmeOptions = this.ConfigureGendarme(userAssemblies);
		string result;
		if (gendarmeOptions.UserAssemblies == null || gendarmeOptions.UserAssemblies.Length == 0)
		{
			result = null;
		}
		else
		{
			List<string> list = new List<string>
			{
				"--config " + gendarmeOptions.ConfigFilePath,
				"--set " + gendarmeOptions.RuleSet
			};
			list.AddRange(gendarmeOptions.UserAssemblies);
			result = list.Aggregate((string agg, string i) => agg + " " + i);
		}
		return result;
	}

	private static bool StartManagedProgram(string exe, string arguments, CompilerOutputParserBase parser, ref IEnumerable<CompilerMessage> compilerMessages)
	{
		bool result;
		using (ManagedProgram managedProgram = GendarmeValidationRule.ManagedProgramFor(exe, arguments))
		{
			managedProgram.LogProcessStartInfo();
			try
			{
				managedProgram.Start();
			}
			catch
			{
				throw new Exception("Could not start " + exe);
			}
			managedProgram.WaitForExit();
			if (managedProgram.ExitCode == 0)
			{
				result = true;
				return result;
			}
			compilerMessages = parser.Parse(managedProgram.GetErrorOutput(), managedProgram.GetStandardOutput(), true);
		}
		result = false;
		return result;
	}

	private static ManagedProgram ManagedProgramFor(string exe, string arguments)
	{
		return new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), null, exe, arguments, false, null);
	}
}
