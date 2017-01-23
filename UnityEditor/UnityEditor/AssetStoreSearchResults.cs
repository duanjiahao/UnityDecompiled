using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UnityEditor
{
	internal class AssetStoreSearchResults : AssetStoreResultBase<AssetStoreSearchResults>
	{
		internal struct Group
		{
			public List<AssetStoreAsset> assets;

			public int totalFound;

			public string label;

			public string name;

			public int offset;

			public int limit;

			public static AssetStoreSearchResults.Group Create()
			{
				return new AssetStoreSearchResults.Group
				{
					assets = new List<AssetStoreAsset>(),
					label = "",
					name = "",
					offset = 0,
					limit = -1
				};
			}
		}

		internal List<AssetStoreSearchResults.Group> groups = new List<AssetStoreSearchResults.Group>();

		public AssetStoreSearchResults(AssetStoreResultBase<AssetStoreSearchResults>.Callback c) : base(c)
		{
		}

		protected override void Parse(Dictionary<string, JSONValue> dict)
		{
			foreach (JSONValue current in dict["groups"].AsList(true))
			{
				AssetStoreSearchResults.Group item = AssetStoreSearchResults.Group.Create();
				this.ParseList(current, ref item);
				this.groups.Add(item);
			}
			JSONValue jSONValue = dict["query"]["offsets"];
			List<JSONValue> list = dict["query"]["limits"].AsList(true);
			int num = 0;
			foreach (JSONValue current2 in jSONValue.AsList(true))
			{
				AssetStoreSearchResults.Group value = this.groups[num];
				value.offset = (int)current2.AsFloat(true);
				value.limit = (int)list[num].AsFloat(true);
				this.groups[num] = value;
				num++;
			}
		}

		private static string StripExtension(string path)
		{
			string result;
			if (path == null)
			{
				result = null;
			}
			else
			{
				int num = path.LastIndexOf(".");
				result = ((num >= 0) ? path.Substring(0, num) : path);
			}
			return result;
		}

		private void ParseList(JSONValue matches, ref AssetStoreSearchResults.Group group)
		{
			List<AssetStoreAsset> assets = group.assets;
			if (matches.ContainsKey("error"))
			{
				this.error = matches["error"].AsString(true);
			}
			if (matches.ContainsKey("warnings"))
			{
				this.warnings = matches["warnings"].AsString(true);
			}
			if (matches.ContainsKey("name"))
			{
				group.name = matches["name"].AsString(true);
			}
			if (matches.ContainsKey("label"))
			{
				group.label = matches["label"].AsString(true);
			}
			group.label = (group.label ?? group.name);
			if (matches.ContainsKey("total_found"))
			{
				group.totalFound = (int)matches["total_found"].AsFloat(true);
			}
			if (matches.ContainsKey("matches"))
			{
				foreach (JSONValue current in matches["matches"].AsList(true))
				{
					AssetStoreAsset assetStoreAsset = new AssetStoreAsset();
					if (current.ContainsKey("id") && current.ContainsKey("name") && current.ContainsKey("package_id"))
					{
						assetStoreAsset.id = (int)current["id"].AsFloat();
						assetStoreAsset.name = current["name"].AsString();
						assetStoreAsset.displayName = AssetStoreSearchResults.StripExtension(assetStoreAsset.name);
						assetStoreAsset.packageID = (int)current["package_id"].AsFloat();
						if (current.ContainsKey("static_preview_url"))
						{
							assetStoreAsset.staticPreviewURL = current["static_preview_url"].AsString();
						}
						if (current.ContainsKey("dynamic_preview_url"))
						{
							assetStoreAsset.dynamicPreviewURL = current["dynamic_preview_url"].AsString();
						}
						assetStoreAsset.className = ((!current.ContainsKey("class_name")) ? "" : current["class_name"].AsString());
						if (current.ContainsKey("price"))
						{
							assetStoreAsset.price = current["price"].AsString();
						}
						assets.Add(assetStoreAsset);
					}
				}
			}
		}
	}
}
