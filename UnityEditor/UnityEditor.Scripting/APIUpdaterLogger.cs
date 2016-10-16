using System;
using UnityEngine;

namespace UnityEditor.Scripting
{
	internal class APIUpdaterLogger
	{
		public static void WriteToFile(string msg, params object[] args)
		{
			Console.WriteLine("[Script API Updater] {0}", string.Format(msg, args));
		}

		public static void WriteErrorToConsole(string msg, params object[] args)
		{
			Debug.LogErrorFormat(msg, args);
		}

		public static void WriteInfoToConsole(string line)
		{
			Debug.Log(line);
		}
	}
}
