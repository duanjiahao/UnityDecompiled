using System;

namespace UnityEngine
{
	public struct Touch
	{
		private int m_FingerId;

		private Vector2 m_Position;

		private Vector2 m_RawPosition;

		private Vector2 m_PositionDelta;

		private float m_TimeDelta;

		private int m_TapCount;

		private TouchPhase m_Phase;

		private TouchType m_Type;

		private float m_Pressure;

		private float m_maximumPossiblePressure;

		private float m_Radius;

		private float m_RadiusVariance;

		private float m_AltitudeAngle;

		private float m_AzimuthAngle;

		public int fingerId
		{
			get
			{
				return this.m_FingerId;
			}
			set
			{
				this.m_FingerId = value;
			}
		}

		public Vector2 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		public Vector2 rawPosition
		{
			get
			{
				return this.m_RawPosition;
			}
			set
			{
				this.m_RawPosition = value;
			}
		}

		public Vector2 deltaPosition
		{
			get
			{
				return this.m_PositionDelta;
			}
			set
			{
				this.m_PositionDelta = value;
			}
		}

		public float deltaTime
		{
			get
			{
				return this.m_TimeDelta;
			}
			set
			{
				this.m_TimeDelta = value;
			}
		}

		public int tapCount
		{
			get
			{
				return this.m_TapCount;
			}
			set
			{
				this.m_TapCount = value;
			}
		}

		public TouchPhase phase
		{
			get
			{
				return this.m_Phase;
			}
			set
			{
				this.m_Phase = value;
			}
		}

		public float pressure
		{
			get
			{
				return this.m_Pressure;
			}
			set
			{
				this.m_Pressure = value;
			}
		}

		public float maximumPossiblePressure
		{
			get
			{
				return this.m_maximumPossiblePressure;
			}
			set
			{
				this.m_maximumPossiblePressure = value;
			}
		}

		public TouchType type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		public float altitudeAngle
		{
			get
			{
				return this.m_AltitudeAngle;
			}
			set
			{
				this.m_AltitudeAngle = value;
			}
		}

		public float azimuthAngle
		{
			get
			{
				return this.m_AzimuthAngle;
			}
			set
			{
				this.m_AzimuthAngle = value;
			}
		}

		public float radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = value;
			}
		}

		public float radiusVariance
		{
			get
			{
				return this.m_RadiusVariance;
			}
			set
			{
				this.m_RadiusVariance = value;
			}
		}
	}
}
