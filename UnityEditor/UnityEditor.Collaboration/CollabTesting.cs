using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal class CollabTesting
	{
		private static IEnumerator<bool> _enumerator = null;

		private static Action _runAfter = null;

		public static Func<IEnumerable<bool>> Tick
		{
			set
			{
				CollabTesting._enumerator = value().GetEnumerator();
			}
		}

		public static Action AfterRun
		{
			set
			{
				CollabTesting._runAfter = value;
			}
		}

		public static bool IsRunning
		{
			get
			{
				return CollabTesting._enumerator != null;
			}
		}

		public static void OnCompleteJob()
		{
			CollabTesting.Execute();
		}

		public static void Execute()
		{
			if (CollabTesting._enumerator != null)
			{
				if (!Collab.instance.AnyJobRunning())
				{
					try
					{
						if (!CollabTesting._enumerator.MoveNext())
						{
							CollabTesting.End();
						}
					}
					catch (Exception)
					{
						Debug.LogError("Something Went wrong with the test framework itself");
						throw;
					}
				}
			}
		}

		public static void End()
		{
			if (CollabTesting._enumerator != null)
			{
				CollabTesting._runAfter();
				CollabTesting._enumerator = null;
			}
		}
	}
}
