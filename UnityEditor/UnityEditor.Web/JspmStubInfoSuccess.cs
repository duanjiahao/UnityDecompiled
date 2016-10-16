using System;

namespace UnityEditor.Web
{
	internal class JspmStubInfoSuccess : JspmSuccess
	{
		public string reference;

		public JspmStubInfoSuccess(long messageID, string reference, JspmPropertyInfo[] properties, JspmMethodInfo[] methods, string[] events) : base(messageID, new JspmStubInfo(properties, methods, events), "GETSTUBINFO")
		{
			this.reference = reference;
		}
	}
}
