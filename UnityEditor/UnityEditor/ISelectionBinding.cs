using System;
using UnityEngine;

namespace UnityEditor
{
	internal interface ISelectionBinding
	{
		GameObject rootGameObject
		{
			get;
		}

		AnimationClip animationClip
		{
			get;
		}

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
