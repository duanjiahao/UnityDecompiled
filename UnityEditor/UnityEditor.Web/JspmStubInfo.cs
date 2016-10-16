using System;

namespace UnityEditor.Web
{
	internal class JspmStubInfo
	{
		public JspmPropertyInfo[] properties;

		public JspmMethodInfo[] methods;

		public string[] events;

		public JspmStubInfo(JspmPropertyInfo[] properties, JspmMethodInfo[] methods, string[] events)
		{
			this.methods = methods;
			this.properties = properties;
			this.events = events;
		}
	}
}
