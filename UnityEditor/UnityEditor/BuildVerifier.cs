using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using UnityEngine;

namespace UnityEditor
{
	internal class BuildVerifier
	{
		private Dictionary<string, HashSet<string>> m_UnsupportedAssemblies = null;

		private static BuildVerifier ms_Inst = null;

		protected BuildVerifier()
		{
			this.m_UnsupportedAssemblies = new Dictionary<string, HashSet<string>>();
			string text = Path.Combine(Path.Combine(EditorApplication.applicationContentsPath, "Resources"), "BuildVerification.xml");
			XPathDocument xPathDocument = new XPathDocument(text);
			XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
			xPathNavigator.MoveToFirstChild();
			XPathNodeIterator xPathNodeIterator = xPathNavigator.SelectChildren("assembly", "");
			while (xPathNodeIterator.MoveNext())
			{
				string attribute = xPathNodeIterator.Current.GetAttribute("name", "");
				if (string.IsNullOrEmpty(attribute))
				{
					throw new ApplicationException(string.Format("Failed to load {0}, <assembly> name attribute is empty", text));
				}
				string text2 = xPathNodeIterator.Current.GetAttribute("platform", "");
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "*";
				}
				if (!this.m_UnsupportedAssemblies.ContainsKey(text2))
				{
					this.m_UnsupportedAssemblies.Add(text2, new HashSet<string>());
				}
				this.m_UnsupportedAssemblies[text2].Add(attribute);
			}
		}

		protected void VerifyBuildInternal(BuildTarget target, string managedDllFolder)
		{
			string[] files = Directory.GetFiles(managedDllFolder);
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i];
				if (text.EndsWith(".dll"))
				{
					string fileName = Path.GetFileName(text);
					if (!this.VerifyAssembly(target, fileName))
					{
						Debug.LogWarningFormat("{0} assembly is referenced by user code, but is not supported on {1} platform. Various failures might follow.", new object[]
						{
							fileName,
							target.ToString()
						});
					}
				}
			}
		}

		protected bool VerifyAssembly(BuildTarget target, string assembly)
		{
			return (!this.m_UnsupportedAssemblies.ContainsKey("*") || !this.m_UnsupportedAssemblies["*"].Contains(assembly)) && (!this.m_UnsupportedAssemblies.ContainsKey(target.ToString()) || !this.m_UnsupportedAssemblies[target.ToString()].Contains(assembly));
		}

		public static void VerifyBuild(BuildTarget target, string managedDllFolder)
		{
			if (BuildVerifier.ms_Inst == null)
			{
				BuildVerifier.ms_Inst = new BuildVerifier();
			}
			BuildVerifier.ms_Inst.VerifyBuildInternal(target, managedDllFolder);
		}
	}
}
