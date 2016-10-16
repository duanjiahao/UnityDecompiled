using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UnityEditor
{
	internal class SimpleProfiler
	{
		private static Stack<string> m_Names = new Stack<string>();

		private static Stack<float> m_StartTime = new Stack<float>();

		private static Dictionary<string, float> m_Timers = new Dictionary<string, float>();

		private static Dictionary<string, int> m_Calls = new Dictionary<string, int>();

		[Conditional("SIMPLE_PROFILER")]
		public static void Begin(string label)
		{
			SimpleProfiler.m_Names.Push(label);
			SimpleProfiler.m_StartTime.Push(Time.realtimeSinceStartup);
		}

		[Conditional("SIMPLE_PROFILER")]
		public static void End()
		{
			string text = SimpleProfiler.m_Names.Pop();
			float num = Time.realtimeSinceStartup - SimpleProfiler.m_StartTime.Pop();
			if (SimpleProfiler.m_Timers.ContainsKey(text))
			{
				Dictionary<string, float> timers;
				Dictionary<string, float> expr_31 = timers = SimpleProfiler.m_Timers;
				string key;
				string expr_34 = key = text;
				float num2 = timers[key];
				expr_31[expr_34] = num2 + num;
			}
			else
			{
				SimpleProfiler.m_Timers[text] = num;
			}
			if (SimpleProfiler.m_Calls.ContainsKey(text))
			{
				Dictionary<string, int> calls;
				Dictionary<string, int> expr_6E = calls = SimpleProfiler.m_Calls;
				string key;
				string expr_72 = key = text;
				int num3 = calls[key];
				expr_6E[expr_72] = num3 + 1;
			}
			else
			{
				SimpleProfiler.m_Calls[text] = 1;
			}
		}

		[Conditional("SIMPLE_PROFILER")]
		public static void PrintTimes()
		{
			string text = "Measured execution times:\n----------------------------\n";
			foreach (KeyValuePair<string, float> current in SimpleProfiler.m_Timers)
			{
				text += string.Format("{0,6:0.0} ms: {1} in {2} calls\n", current.Value * 1000f, current.Key, SimpleProfiler.m_Calls[current.Key]);
			}
			UnityEngine.Debug.Log(text);
			SimpleProfiler.m_Names.Clear();
			SimpleProfiler.m_StartTime.Clear();
			SimpleProfiler.m_Timers.Clear();
			SimpleProfiler.m_Calls.Clear();
		}
	}
}
