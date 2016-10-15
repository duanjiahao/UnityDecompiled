using System;
using UnityEngine;

namespace UnityEditor.RestService
{
	internal class Logger
	{
		public static void Log(Exception an_exception)
		{
			Debug.Log(an_exception.ToString());
		}

		public static void Log(string a_message)
		{
			Debug.Log(a_message);
		}
	}
}
