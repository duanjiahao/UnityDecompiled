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
		public override ScriptCompilerBase CreateCompiler(MonoIsland island, bool buildingForEditor, BuildTarget targetPlatform)
		{
			if (!buildingForEditor && targetPlatform.ToString().Contains("MetroPlayer") && (PlayerSettings.Metro.compilationOverrides == PlayerSettings.MetroCompilationOverrides.UseNetCore || (PlayerSettings.Metro.compilationOverrides == PlayerSettings.MetroCompilationOverrides.UseNetCorePartially && !island._output.Contains("Assembly-CSharp-firstpass.dll"))))
			{
				return new MicrosoftCSharpCompiler(island);
			}
			return new MonoCSharpCompiler(island);
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
