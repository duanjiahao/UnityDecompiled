using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityScript.Scripting;

namespace UnityEditor.Macros
{
	public static class MacroEvaluator
	{
		private class EditorEvaluationDomainProvider : SimpleEvaluationDomainProvider
		{
			private static readonly string[] DefaultImports = new string[]
			{
				"UnityEditor",
				"UnityEngine"
			};

			public EditorEvaluationDomainProvider() : base(MacroEvaluator.EditorEvaluationDomainProvider.DefaultImports)
			{
			}

			public override Assembly[] GetAssemblyReferences()
			{
				Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
				IEnumerable<Assembly> second = from a in loadedAssemblies.SelectMany((Assembly a) => a.GetReferencedAssemblies())
				select MacroEvaluator.EditorEvaluationDomainProvider.TryToLoad(a) into a
				where a != null
				select a;
				return loadedAssemblies.Concat(second).ToArray<Assembly>();
			}

			private static Assembly TryToLoad(AssemblyName a)
			{
				Assembly result;
				try
				{
					result = Assembly.Load(a);
				}
				catch (Exception)
				{
					result = null;
				}
				return result;
			}
		}

		private static readonly EvaluationContext EditorEvaluationContext = new EvaluationContext(new MacroEvaluator.EditorEvaluationDomainProvider());

		public static string Eval(string macro)
		{
			if (macro.StartsWith("ExecuteMethod: "))
			{
				return MacroEvaluator.ExecuteMethodThroughReflection(macro);
			}
			object obj = Evaluator.Eval(MacroEvaluator.EditorEvaluationContext, macro);
			return (obj != null) ? obj.ToString() : "Null";
		}

		private static string ExecuteMethodThroughReflection(string macro)
		{
			Regex regex = new Regex("ExecuteMethod: (?<type>.*)\\.(?<method>.*)");
			Match match = regex.Match(macro);
			string typename = match.Groups["type"].ToString();
			string text = match.Groups["method"].ToString();
			Type type = (from a in EditorAssemblies.loadedAssemblies
			select a.GetType(typename, false) into t
			where t != null
			select t).First<Type>();
			MethodInfo method = type.GetMethod(text, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				throw new ArgumentException(string.Format("cannot find method {0} in type {1}", text, typename));
			}
			if (method.GetParameters().Length > 0)
			{
				throw new ArgumentException("You can only invoke static methods with no arguments");
			}
			object obj = method.Invoke(null, new object[0]);
			return (obj != null) ? obj.ToString() : "Null";
		}
	}
}
