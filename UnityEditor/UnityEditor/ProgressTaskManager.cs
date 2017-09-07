using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class ProgressTaskManager
	{
		private ProgressHandler m_Handler;

		private List<Action> m_Tasks = new List<Action>();

		private int m_ProgressUpdatesForCurrentTask;

		private int m_StartedTasks;

		public ProgressTaskManager(ProgressHandler handler)
		{
			this.m_Handler = handler;
		}

		public void AddTask(Action task)
		{
			this.m_Tasks.Add(task);
		}

		public void Run()
		{
			foreach (Action current in this.m_Tasks)
			{
				this.m_StartedTasks++;
				this.m_ProgressUpdatesForCurrentTask = 0;
				current();
			}
		}

		public void UpdateProgress(string message)
		{
			if (this.m_Handler != null)
			{
				float num = 1f - Mathf.Pow(0.85f, (float)this.m_ProgressUpdatesForCurrentTask);
				int num2 = this.m_Tasks.Count;
				if (num2 <= this.m_StartedTasks)
				{
					num2 = this.m_StartedTasks;
				}
				float num3 = 1f / (float)num2;
				float progress = (float)(this.m_StartedTasks - 1) * num3 + num * num3;
				this.m_Handler.OnProgress(message, progress);
			}
			this.m_ProgressUpdatesForCurrentTask++;
		}

		public ProgressHandler SpawnProgressHandlerFromCurrentTask()
		{
			ProgressHandler result;
			if (this.m_Handler != null)
			{
				int count = this.m_Tasks.Count;
				float num = 1f / (float)count;
				float localRangeMin = (float)(this.m_StartedTasks - 1) * num;
				float localRangeMax = (float)this.m_StartedTasks * num;
				result = this.m_Handler.SpawnFromLocalSubRange(localRangeMin, localRangeMax);
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
