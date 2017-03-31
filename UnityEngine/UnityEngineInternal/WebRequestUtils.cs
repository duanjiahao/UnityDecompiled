using System;
using System.Text.RegularExpressions;
using UnityEngine.Scripting;

namespace UnityEngineInternal
{
	internal static class WebRequestUtils
	{
		private static Regex domainRegex = new Regex("^\\s*\\w+(?:\\.\\w+)+\\s*$");

		[RequiredByNativeCode]
		internal static string RedirectTo(string baseUri, string redirectUri)
		{
			Uri uri;
			if (redirectUri[0] == '/')
			{
				uri = new Uri(redirectUri, UriKind.Relative);
			}
			else
			{
				uri = new Uri(redirectUri, UriKind.RelativeOrAbsolute);
			}
			string result;
			if (uri.IsAbsoluteUri)
			{
				result = redirectUri;
			}
			else
			{
				Uri baseUri2 = new Uri(baseUri, UriKind.Absolute);
				Uri uri2 = new Uri(baseUri2, uri);
				result = uri2.AbsoluteUri;
			}
			return result;
		}

		internal static string MakeInitialUrl(string targetUrl, string localUrl)
		{
			Uri uri = new Uri(localUrl);
			if (targetUrl.StartsWith("//"))
			{
				targetUrl = uri.Scheme + ":" + targetUrl;
			}
			if (targetUrl.StartsWith("/"))
			{
				targetUrl = uri.Scheme + "://" + uri.Host + targetUrl;
			}
			if (WebRequestUtils.domainRegex.IsMatch(targetUrl))
			{
				targetUrl = uri.Scheme + "://" + targetUrl;
			}
			Uri uri2 = null;
			try
			{
				uri2 = new Uri(targetUrl);
			}
			catch (FormatException ex)
			{
				try
				{
					uri2 = new Uri(uri, targetUrl);
				}
				catch (FormatException)
				{
					throw ex;
				}
			}
			return (!targetUrl.Contains("%")) ? uri2.AbsoluteUri : uri2.OriginalString;
		}
	}
}
