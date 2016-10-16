using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UnityEditor
{
	internal abstract class AssetStoreResultBase<Derived> where Derived : class
	{
		public delegate void Callback(Derived res);

		private AssetStoreResultBase<Derived>.Callback callback;

		public string error;

		public string warnings;

		public AssetStoreResultBase(AssetStoreResultBase<Derived>.Callback cb)
		{
			this.callback = cb;
			this.warnings = string.Empty;
		}

		public void Parse(AssetStoreResponse response)
		{
			if (response.job.IsSuccess())
			{
				if (response.job.responseCode >= 300)
				{
					this.error = string.Format("HTTP status code {0}", response.job.responseCode);
				}
				else if (response.dict.ContainsKey("error"))
				{
					this.error = response.dict["error"].AsString(true);
				}
				else
				{
					this.Parse(response.dict);
				}
			}
			else
			{
				string str = (response.job != null) ? ((response.job.url != null) ? response.job.url : "null") : "nulljob";
				this.error = "Error receiving response from server on url '" + str + "': " + (response.job.text ?? "n/a");
			}
			this.callback(this as Derived);
		}

		protected abstract void Parse(Dictionary<string, JSONValue> dict);
	}
}
