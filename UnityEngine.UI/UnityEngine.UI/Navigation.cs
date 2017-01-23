using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[Serializable]
	public struct Navigation : IEquatable<Navigation>
	{
		[Flags]
		public enum Mode
		{
			None = 0,
			Horizontal = 1,
			Vertical = 2,
			Automatic = 3,
			Explicit = 4
		}

		[FormerlySerializedAs("mode"), SerializeField]
		private Navigation.Mode m_Mode;

		[FormerlySerializedAs("selectOnUp"), SerializeField]
		private Selectable m_SelectOnUp;

		[FormerlySerializedAs("selectOnDown"), SerializeField]
		private Selectable m_SelectOnDown;

		[FormerlySerializedAs("selectOnLeft"), SerializeField]
		private Selectable m_SelectOnLeft;

		[FormerlySerializedAs("selectOnRight"), SerializeField]
		private Selectable m_SelectOnRight;

		public Navigation.Mode mode
		{
			get
			{
				return this.m_Mode;
			}
			set
			{
				this.m_Mode = value;
			}
		}

		public Selectable selectOnUp
		{
			get
			{
				return this.m_SelectOnUp;
			}
			set
			{
				this.m_SelectOnUp = value;
			}
		}

		public Selectable selectOnDown
		{
			get
			{
				return this.m_SelectOnDown;
			}
			set
			{
				this.m_SelectOnDown = value;
			}
		}

		public Selectable selectOnLeft
		{
			get
			{
				return this.m_SelectOnLeft;
			}
			set
			{
				this.m_SelectOnLeft = value;
			}
		}

		public Selectable selectOnRight
		{
			get
			{
				return this.m_SelectOnRight;
			}
			set
			{
				this.m_SelectOnRight = value;
			}
		}

		public static Navigation defaultNavigation
		{
			get
			{
				return new Navigation
				{
					m_Mode = Navigation.Mode.Automatic
				};
			}
		}

		public bool Equals(Navigation other)
		{
			return this.mode == other.mode && this.selectOnUp == other.selectOnUp && this.selectOnDown == other.selectOnDown && this.selectOnLeft == other.selectOnLeft && this.selectOnRight == other.selectOnRight;
		}
	}
}
