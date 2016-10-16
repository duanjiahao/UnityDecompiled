using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.Scripting.Compilers
{
	internal static class CommandLineFormatter
	{
		private static readonly Regex UnsafeCharsWindows = new Regex("[^A-Za-z0-9\\_\\-\\.\\:\\,\\/\\@\\\\]");

		private static readonly Regex UnescapeableChars = new Regex("[\\x00-\\x08\\x10-\\x1a\\x1c-\\x1f\\x7f\\xff]");

		private static readonly Regex Quotes = new Regex("\"");

		public static string EscapeCharsQuote(string input)
		{
			if (input.IndexOf('\'') == -1)
			{
				return "'" + input + "'";
			}
			if (input.IndexOf('"') == -1)
			{
				return "\"" + input + "\"";
			}
			return null;
		}

		public static string PrepareFileName(string input)
		{
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				return CommandLineFormatter.EscapeCharsQuote(input);
			}
			return CommandLineFormatter.EscapeCharsWindows(input);
		}

		public static string EscapeCharsWindows(string input)
		{
			if (input.Length == 0)
			{
				return "\"\"";
			}
			if (CommandLineFormatter.UnescapeableChars.IsMatch(input))
			{
				Debug.LogWarning("Cannot escape control characters in string");
				return "\"\"";
			}
			if (CommandLineFormatter.UnsafeCharsWindows.IsMatch(input))
			{
				return "\"" + CommandLineFormatter.Quotes.Replace(input, "\"\"") + "\"";
			}
			return input;
		}

		internal static string GenerateResponseFile(IEnumerable<string> arguments)
		{
			string uniqueTempPathInProject = FileUtil.GetUniqueTempPathInProject();
			using (StreamWriter streamWriter = new StreamWriter(uniqueTempPathInProject))
			{
				foreach (string current in from a in arguments
				where a != null
				select a)
				{
					streamWriter.WriteLine(current);
				}
			}
			return uniqueTempPathInProject;
		}
	}
}
