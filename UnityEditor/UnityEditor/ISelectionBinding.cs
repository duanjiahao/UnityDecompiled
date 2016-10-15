using System;

namespace UnityEditor
{
	internal interface ISelectionBinding
	{
		bool clipIsEditable
		{
			get;
		}

		bool animationIsEditable
		{
			get;
		}

		float timeOffset
		{
			get;
		}

		int id
		{
			get;
		}
	}
}
