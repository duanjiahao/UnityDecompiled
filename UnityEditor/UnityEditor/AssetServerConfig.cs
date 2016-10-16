using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetServerConfig
	{
		private Dictionary<string, string> fileContents;

		private string fileName;

		private static Regex sKeyTag = new Regex("<key>([^<]+)</key>");

		private static Regex sValueTag = new Regex("<string>([^<]+)</string>");

		public string connectionSettings
		{
			get
			{
				return this.fileContents["Maint Connection Settings"];
			}
			set
			{
				this.fileContents["Maint Connection Settings"] = value;
			}
		}

		public string server
		{
			get
			{
				return this.fileContents["Maint Server"];
			}
			set
			{
				this.fileContents["Maint Server"] = value;
			}
		}

		public int portNumber
		{
			get
			{
				return int.Parse(this.fileContents["Maint port number"]);
			}
			set
			{
				this.fileContents["Maint port number"] = value.ToString();
			}
		}

		public float timeout
		{
			get
			{
				return float.Parse(this.fileContents["Maint Timeout"]);
			}
			set
			{
				this.fileContents["Maint Timeout"] = value.ToString();
			}
		}

		public string userName
		{
			get
			{
				return this.fileContents["Maint UserName"];
			}
			set
			{
				this.fileContents["Maint UserName"] = value;
			}
		}

		public string dbName
		{
			get
			{
				return this.fileContents["Maint database name"];
			}
			set
			{
				this.fileContents["Maint database name"] = value;
			}
		}

		public string projectName
		{
			get
			{
				return this.fileContents["Maint project name"];
			}
			set
			{
				this.fileContents["Maint project name"] = value;
			}
		}

		public string settingsType
		{
			get
			{
				return this.fileContents["Maint settings type"];
			}
			set
			{
				this.fileContents["Maint settings type"] = value;
			}
		}

		public AssetServerConfig()
		{
			this.fileContents = new Dictionary<string, string>();
			this.fileName = Application.dataPath + "/../Library/ServerPreferences.plist";
			try
			{
				using (StreamReader streamReader = new StreamReader(this.fileName))
				{
					string key = ".unkown";
					string input;
					while ((input = streamReader.ReadLine()) != null)
					{
						Match match = AssetServerConfig.sKeyTag.Match(input);
						if (match.Success)
						{
							key = match.Groups[1].Value;
						}
						match = AssetServerConfig.sValueTag.Match(input);
						if (match.Success)
						{
							this.fileContents[key] = match.Groups[1].Value;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Could not read asset server configuration: " + ex.Message);
			}
		}
	}
}
