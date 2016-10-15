using System;

namespace UnityEditor.Web
{
	internal class JspmSuccess : JspmResult
	{
		public object result;

		public string type;

		public JspmSuccess(long messageID, object result, string type) : base(messageID, 0)
		{
			this.result = result;
			this.type = type;
		}
	}
}
