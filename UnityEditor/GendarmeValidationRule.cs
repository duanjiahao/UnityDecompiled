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
		ValidationResult validationResult = default(ValidationResult);
		ValidationResult validationResult2 = validationResult;
		validationResult2.Success = true;
		validationResult2.Rule = this;
		validationResult2.CompilerMessages = null;
		validationResult = validationResult2;
		try
		{
			validationResult.Success = GendarmeValidationRule.StartManagedProgram(this._gendarmeExePath, arguments, new GendarmeOutputParser(), ref validationResult.CompilerMessages);
		}
		catch (Exception ex)
		{
			validationResult.Success = false;
			validationResult.CompilerMessages = new CompilerMessage[]
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
		return validationResult;
	}

	protected abstract GendarmeOptions ConfigureGendarme(IEnumerable<string> userAssemblies);

	protected string BuildGendarmeCommandLineArguments(IEnumerable<string> userAssemblies)
	{
		GendarmeOptions gendarmeOptions = this.ConfigureGendarme(userAssemblies);
		if (gendarmeOptions.UserAssemblies == null || gendarmeOptions.UserAssemblies.Length == 0)
		{
			return null;
		}
		List<string> list = new List<string>
		{
			"--config " + gendarmeOptions.ConfigFilePath,
			"--set " + gendarmeOptions.RuleSet
		};
		list.AddRange(gendarmeOptions.UserAssemblies);
		return list.Aggregate((string agg, string i) => agg + " " + i);
	}

	private static bool StartManagedProgram(string exe, string arguments, CompilerOutputParserBase parser, ref IEnumerable<CompilerMessage> compilerMessages)
	{
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
				return true;
			}
			compilerMessages = parser.Parse(managedProgram.GetErrorOutput(), managedProgram.GetStandardOutput(), true);
		}
		return false;
	}

	private static ManagedProgram ManagedProgramFor(string exe, string arguments)
	{
		return new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", exe, arguments, null);
	}
}
