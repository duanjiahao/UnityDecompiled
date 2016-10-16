using System;

namespace UnityEditor.Web
{
	internal class JspmError : JspmResult
	{
		public string errorClass;

		public string message;

		public JspmError(long messageID, int status, string errorClass, string message) : base(messageID, status)
		{
			this.errorClass = errorClass;
			this.message = message;
		}
	}
}
