using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
namespace UnityEditor.Scripting.Compilers
{
	internal class CSharpLanguage : SupportedLanguage
	{
		private class VisitorData
		{
			public string TargetClassName;
			public Stack<string> CurrentNamespaces;
			public string DiscoveredNamespace;
			public VisitorData()
			{
				this.CurrentNamespaces = new Stack<string>();
			}
		}
		private class NamespaceVisitor : AbstractAstVisitor
		{
			public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
			{
				CSharpLanguage.VisitorData visitorData = (CSharpLanguage.VisitorData)data;
				visitorData.CurrentNamespaces.Push(namespaceDeclaration.Name);
				namespaceDeclaration.AcceptChildren(this, visitorData);
				visitorData.CurrentNamespaces.Pop();
				return null;
			}
			public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
			{
				CSharpLanguage.VisitorData visitorData = (CSharpLanguage.VisitorData)data;
				if (typeDeclaration.Name == visitorData.TargetClassName)
				{
					string text = string.Empty;
					foreach (string current in visitorData.CurrentNamespaces)
					{
						if (text == string.Empty)
						{
							text = current;
						}
						else
						{
							text = current + "." + text;
						}
					}
					visitorData.DiscoveredNamespace = text;
				}
				return null;
			}
		}
		public override string GetExtensionICanCompile()
		{
			return "cs";
		}
		public override string GetLanguageName()
		{
			return "CSharp";
		}
		internal static bool GetUseMicrosoftCSharpCompiler(BuildTarget targetPlatform, bool buildingForEditor, string assemblyName)
		{
			if (buildingForEditor || targetPlatform != BuildTarget.MetroPlayer)
			{
				return false;
			}
			assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
			PlayerSettings.WSACompilationOverrides compilationOverrides = PlayerSettings.WSA.compilationOverrides;
			if (compilationOverrides != PlayerSettings.WSACompilationOverrides.UseNetCore)
			{
				return compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCorePartially && string.Compare(assemblyName, "Assembly-CSharp", true) == 0;
			}
			return string.Compare(assemblyName, "Assembly-CSharp", true) == 0 || string.Compare(assemblyName, "Assembly-CSharp-firstPass", true) == 0;
		}
		public override ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform, bool runUpdater)
		{
			if (CSharpLanguage.GetUseMicrosoftCSharpCompiler(targetPlatform, buildingForEditor, island._output))
			{
				return new MicrosoftCSharpCompiler(island, runUpdater);
			}
			return new MonoCSharpCompiler(island, runUpdater);
		}
		public override string GetNamespace(string fileName)
		{
			using (IParser parser = ParserFactory.CreateParser(fileName))
			{
				parser.Parse();
				try
				{
					CSharpLanguage.NamespaceVisitor visitor = new CSharpLanguage.NamespaceVisitor();
					CSharpLanguage.VisitorData visitorData = new CSharpLanguage.VisitorData
					{
						TargetClassName = Path.GetFileNameWithoutExtension(fileName)
					};
					parser.CompilationUnit.AcceptVisitor(visitor, visitorData);
					return (!string.IsNullOrEmpty(visitorData.DiscoveredNamespace)) ? visitorData.DiscoveredNamespace : string.Empty;
				}
				catch
				{
				}
			}
			return string.Empty;
		}
	}
}
