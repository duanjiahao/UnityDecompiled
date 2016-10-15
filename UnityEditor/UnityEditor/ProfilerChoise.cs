using System;

namespace UnityEditor
{
	internal struct ProfilerChoise
	{
		public string Name;

		public bool Enabled;

		public Func<bool> IsSelected;

		public Action ConnectTo;
	}
}
