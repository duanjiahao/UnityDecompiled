using System;

namespace UnityEditor.RestService
{
	internal class RestRequestException : Exception
	{
		public string RestErrorString
		{
			get;
			set;
		}

		public HttpStatusCode HttpStatusCode
		{
			get;
			set;
		}

		public string RestErrorDescription
		{
			get;
			set;
		}

		public RestRequestException()
		{
		}

		public RestRequestException(HttpStatusCode httpStatusCode, string restErrorString) : this(httpStatusCode, restErrorString, null)
		{
		}

		public RestRequestException(HttpStatusCode httpStatusCode, string restErrorString, string restErrorDescription)
		{
			this.HttpStatusCode = httpStatusCode;
			this.RestErrorString = restErrorString;
			this.RestErrorDescription = restErrorDescription;
		}
	}
}
