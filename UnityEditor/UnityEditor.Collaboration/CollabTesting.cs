using System;
using System.Collections.Generic;

namespace UnityEditor.Collaboration
{
	internal class CollabTesting
	{
		private static readonly Queue<Action> m_Actions = new Queue<Action>();

		public static int ActionsCount
		{
			get
			{
				return CollabTesting.m_Actions.Count;
			}
		}

		public static void OnCompleteJob()
		{
			CollabTesting.Execute();
		}

		public static void AddAction(Action action)
		{
			CollabTesting.m_Actions.Enqueue(action);
		}

		public static void Execute()
		{
			if (CollabTesting.m_Actions.Count != 0)
			{
				Action action = CollabTesting.m_Actions.Dequeue();
				action();
			}
		}

		public static void DropAll()
		{
			CollabTesting.m_Actions.Clear();
		}
	}
}
