using System;

namespace UnityEngine.Experimental.UIElements
{
	internal interface IRecyclable
	{
		bool isTrashed
		{
			get;
			set;
		}

		void OnTrash();

		void OnReuse();
	}
}
