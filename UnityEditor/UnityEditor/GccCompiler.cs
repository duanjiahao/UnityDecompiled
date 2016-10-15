using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor
{
	internal class GccCompiler : NativeCompiler
	{
		private readonly ICompilerSettings m_Settings;

		protected override string objectFileExtension
		{
			get
			{
				return "o";
			}
		}

		public GccCompiler(ICompilerSettings settings)
		{
			this.m_Settings = settings;
		}

		private void Compile(string file, string includePaths)
		{
			string arguments = string.Format(" -c {0} -O0 -Wno-unused-value -Wno-invalid-offsetof -fvisibility=hidden -fno-rtti {1} {2} -o {3}", new object[]
			{
				this.m_Settings.MachineSpecification,
				includePaths,
				file,
				base.ObjectFileFor(file)
			});
			base.Execute(arguments, this.m_Settings.CompilerPath);
		}

		public override void CompileDynamicLibrary(string outFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths)
		{
			string[] array = sources.ToArray<string>();
			string includeDirs = includePaths.Aggregate(string.Empty, (string current, string sourceDir) => current + "-I" + sourceDir + " ");
			string empty = string.Empty;
			string text = NativeCompiler.Aggregate(libraryPaths.Union(this.m_Settings.LibPaths), "-L", " ");
			NativeCompiler.ParallelFor<string>(array, delegate(string file)
			{
				this.Compile(file, includeDirs);
			});
			string arg_F7_1 = this.m_Settings.LinkerPath;
			string[] expr_8E = new string[4];
			expr_8E[0] = string.Format("-shared {0} -o {1}", this.m_Settings.MachineSpecification, outFile);
			expr_8E[1] = array.Where(new Func<string, bool>(NativeCompiler.IsSourceFile)).Select(new Func<string, string>(base.ObjectFileFor)).Aggregate((string buff, string s) => buff + " " + s);
			expr_8E[2] = text;
			expr_8E[3] = empty;
			base.ExecuteCommand(arg_F7_1, expr_8E);
		}
	}
}
