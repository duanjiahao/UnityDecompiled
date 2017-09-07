using System;

namespace UnityEngine.Experimental.UIElements
{
	internal interface IOnGUIHandler : IRecyclable
	{
		int id
		{
			get;
		}

		bool OnGUI(Event evt);

		void GenerateControlID();
	}
}
