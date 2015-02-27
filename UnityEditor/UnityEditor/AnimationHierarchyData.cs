using System;
using System.Collections;
using UnityEngine;
namespace UnityEditor
{
	internal class AnimationHierarchyData
	{
		public AnimationWindow animationWindow;
		public AnimationSelection animationSelection;
		public FoldoutObjectState[] states;
		public SerializedStringTable expandedFoldouts
		{
			get
			{
				return this.animationWindow.expandedFoldouts;
			}
			set
			{
				this.animationWindow.expandedFoldouts = value;
			}
		}
		public bool showAllProperties
		{
			get
			{
				return this.animationWindow.showAllProperties;
			}
		}
		public GameObject animated
		{
			get
			{
				return this.animationSelection.animatedObject;
			}
		}
		public AnimationClip clip
		{
			get
			{
				return this.animationSelection.clip;
			}
		}
		public Hashtable animatedCurves
		{
			get
			{
				return this.animationSelection.animatedCurves;
			}
		}
		public Hashtable leftoverCurves
		{
			get
			{
				return this.animationSelection.leftoverCurves;
			}
		}
		public Hashtable animatedPaths
		{
			get
			{
				return this.animationSelection.animatedPaths;
			}
		}
	}
}
