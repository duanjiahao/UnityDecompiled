using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UnityEditor
{
	internal class AssetStoreResponse
	{
		internal AsyncHTTPClient job;

		public Dictionary<string, JSONValue> dict;

		public bool ok;

		public bool failed
		{
			get
			{
				return !this.ok;
			}
		}

		public string message
		{
			get
			{
				string result;
				if (this.dict == null || !this.dict.ContainsKey("message"))
				{
					result = null;
				}
				else
				{
					result = this.dict["message"].AsString(true);
				}
				return result;
			}
		}

		private static string EncodeString(string str)
		{
			str = str.Replace("\"", "\\\"");
			str = str.Replace("\\", "\\\\");
			str = str.Replace("\b", "\\b");
			str = str.Replace("\f", "\\f");
			str = str.Replace("\n", "\\n");
			str = str.Replace("\r", "\\r");
			str = str.Replace("\t", "\\t");
			return str;
		}

		public override string ToString()
		{
			string text = "{";
			string text2 = "";
			foreach (KeyValuePair<string, JSONValue> current in this.dict)
			{
				string text3 = text;
				text = string.Concat(new object[]
				{
					text3,
					text2,
					'"',
					AssetStoreResponse.EncodeString(current.Key),
					"\" : ",
					current.Value.ToString()
				});
				text2 = ", ";
			}
			return text + "}";
		}
	}
}
