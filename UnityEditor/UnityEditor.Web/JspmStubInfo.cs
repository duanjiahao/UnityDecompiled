using System;

namespace UnityEditor.Web
{
	internal class JspmStubInfo
	{
		public JspmPropertyInfo[] properties = null;

		public JspmMethodInfo[] methods = null;

		public string[] events = null;

		public JspmStubInfo(JspmPropertyInfo[] properties, JspmMethodInfo[] methods, string[] events)
		{
			this.methods = methods;
			this.properties = properties;
			this.events = events;
		}
	}
}
