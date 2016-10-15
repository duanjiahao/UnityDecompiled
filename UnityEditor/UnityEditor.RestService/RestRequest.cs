using System;
using System.IO;
using System.Net;
using System.Text;

namespace UnityEditor.RestService
{
	internal class RestRequest
	{
		public static bool Send(string endpoint, string payload, int timeout)
		{
			if (ScriptEditorSettings.ServerURL == null)
			{
				return false;
			}
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
				bool result = false;
				return result;
			}
			try
			{
				webRequest.BeginGetResponse(new AsyncCallback(RestRequest.GetResponseCallback), webRequest);
			}
			catch (Exception an_exception2)
			{
				Logger.Log(an_exception2);
				bool result = false;
				return result;
			}
			return true;
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
