using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public struct AnimatorTransitionInfo
	{
		private int m_FullPath;

		private int m_UserName;

		private int m_Name;

		private float m_NormalizedTime;

		private bool m_AnyState;

		private int m_TransitionType;

		public int fullPathHash
		{
			get
			{
				return this.m_FullPath;
			}
		}

		public int nameHash
		{
			get
			{
				return this.m_Name;
			}
		}

		public int userNameHash
		{
			get
			{
				return this.m_UserName;
			}
		}

		public float normalizedTime
		{
			get
			{
				return this.m_NormalizedTime;
			}
		}

		public bool anyState
		{
			get
			{
				return this.m_AnyState;
			}
		}

		internal bool entry
		{
			get
			{
				return (this.m_TransitionType & 2) != 0;
			}
		}

		internal bool exit
		{
			get
			{
				return (this.m_TransitionType & 4) != 0;
			}
		}

		public bool IsName(string name)
		{
			return Animator.StringToHash(name) == this.m_Name || Animator.StringToHash(name) == this.m_FullPath;
		}

		public bool IsUserName(string name)
		{
			return Animator.StringToHash(name) == this.m_UserName;
		}
	}
}
