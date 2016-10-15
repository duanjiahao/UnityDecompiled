using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;

namespace UnityEditorInternal
{
	internal class LinkXmlReader
	{
		private readonly List<string> _assembliesInALinkXmlFile = new List<string>();

		public LinkXmlReader()
		{
			foreach (string current in AssemblyStripper.GetUserBlacklistFiles())
			{
				XPathDocument xPathDocument = new XPathDocument(current);
				XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
				xPathNavigator.MoveToFirstChild();
				XPathNodeIterator xPathNodeIterator = xPathNavigator.SelectChildren("assembly", string.Empty);
				while (xPathNodeIterator.MoveNext())
				{
					this._assembliesInALinkXmlFile.Add(xPathNodeIterator.Current.GetAttribute("fullname", string.Empty));
				}
			}
		}

		public bool IsDLLUsed(string assemblyFileName)
		{
			return this._assembliesInALinkXmlFile.Contains(Path.GetFileNameWithoutExtension(assemblyFileName));
		}
	}
}
