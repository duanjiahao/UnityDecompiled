using System;

namespace UnityEditor.Web
{
	internal class JspmResult
	{
		public double version;

		public long messageID;

		public int status;

		public JspmResult()
		{
			this.version = 1.0;
			this.messageID = -1L;
			this.status = 0;
		}

		public JspmResult(long messageID, int status)
		{
			this.version = 1.0;
			this.messageID = messageID;
			this.status = status;
		}
	}
}
