using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace UnityEditor.RestService
{
	internal class RestRequest
	{
		[CompilerGenerated]
		private static AsyncCallback <>f__mg$cache0;

		public static bool Send(string endpoint, string payload, int timeout)
		{
			bool result;
			if (ScriptEditorSettings.ServerURL == null)
			{
				result = false;
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(payload);
				WebRequest webRequest = WebRequest.Create(ScriptEditorSettings.ServerURL + endpoint);
				webRequest.Timeout = timeout;
				webRequest.Method = "POST";
				webRequest.ContentType = "application/json";
				webRequest.ContentLength = (long)bytes.Length;
				try
				{
					Stream requestStream = webRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
				}
				catch (Exception an_exception)
				{
					Logger.Log(an_exception);
					result = false;
					return result;
				}
				try
				{
					WebRequest arg_A6_0 = webRequest;
					if (RestRequest.<>f__mg$cache0 == null)
					{
						RestRequest.<>f__mg$cache0 = new AsyncCallback(RestRequest.GetResponseCallback);
					}
					arg_A6_0.BeginGetResponse(RestRequest.<>f__mg$cache0, webRequest);
				}
				catch (Exception an_exception2)
				{
					Logger.Log(an_exception2);
					result = false;
					return result;
				}
				result = true;
			}
			return result;
		}

		private static void GetResponseCallback(IAsyncResult asynchronousResult)
		{
			WebRequest webRequest = (WebRequest)asynchronousResult.AsyncState;
			WebResponse webResponse = webRequest.EndGetResponse(asynchronousResult);
			try
			{
				Stream responseStream = webResponse.GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream);
				streamReader.ReadToEnd();
				streamReader.Close();
				responseStream.Close();
			}
			finally
			{
				webResponse.Close();
			}
		}
	}
}
