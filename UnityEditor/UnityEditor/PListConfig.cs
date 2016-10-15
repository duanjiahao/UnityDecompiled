using System;
using System.IO;
using System.Text.RegularExpressions;

namespace UnityEditor
{
	internal class PListConfig
	{
		private string fileName;

		private string xml;

		public string this[string paramName]
		{
			get
			{
				Match match = PListConfig.GetRegex(paramName).Match(this.xml);
				return (!match.Success) ? string.Empty : match.Groups["Value"].Value;
			}
			set
			{
				Match match = PListConfig.GetRegex(paramName).Match(this.xml);
				if (match.Success)
				{
					this.xml = PListConfig.GetRegex(paramName).Replace(this.xml, "${Part1}" + value + "</string>");
				}
				else
				{
					this.WriteNewValue(paramName, value);
				}
			}
		}

		public PListConfig(string fileName)
		{
			if (File.Exists(fileName))
			{
				StreamReader streamReader = new StreamReader(fileName);
				this.xml = streamReader.ReadToEnd();
				streamReader.Close();
			}
			else
			{
				this.Clear();
			}
			this.fileName = fileName;
		}

		private static Regex GetRegex(string paramName)
		{
			return new Regex("(?<Part1><key>" + paramName + "</key>\\s*<string>)(?<Value>.*)</string>");
		}

		public void Save()
		{
			StreamWriter streamWriter = new StreamWriter(this.fileName);
			streamWriter.Write(this.xml);
			streamWriter.Close();
		}

		private void WriteNewValue(string key, string val)
		{
			Regex regex = new Regex("</dict>");
			this.xml = regex.Replace(this.xml, string.Concat(new string[]
			{
				"\t<key>",
				key,
				"</key>\n\t<string>",
				val,
				"</string>\n</dict>"
			}));
		}

		public void Clear()
		{
			this.xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n<plist version=\"1.0\">\n<dict>\n</dict>\n</plist>\n";
		}
	}
}
