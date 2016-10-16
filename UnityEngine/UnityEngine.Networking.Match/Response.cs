using System;
using System.Collections.Generic;

namespace UnityEngine.Networking.Match
{
	internal abstract class Response : ResponseBase, IResponse
	{
		public bool success
		{
			get;
			private set;
		}

		public string extendedInfo
		{
			get;
			private set;
		}

		public void SetSuccess()
		{
			this.success = true;
			this.extendedInfo = string.Empty;
		}

		public void SetFailure(string info)
		{
			this.success = false;
			this.extendedInfo += info;
		}

		public override string ToString()
		{
			return UnityString.Format("[{0}]-success:{1}-extendedInfo:{2}", new object[]
			{
				base.ToString(),
				this.success,
				this.extendedInfo
			});
		}

		public override void Parse(object obj)
		{
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary != null)
			{
				this.success = base.ParseJSONBool("success", obj, dictionary);
				this.extendedInfo = base.ParseJSONString("extendedInfo", obj, dictionary);
				if (!this.success)
				{
					throw new FormatException("FAILURE Returned from server: " + this.extendedInfo);
				}
			}
		}
	}
}
