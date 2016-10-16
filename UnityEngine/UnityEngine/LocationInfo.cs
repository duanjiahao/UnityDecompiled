using System;

namespace UnityEngine
{
	public struct LocationInfo
	{
		private double m_Timestamp;

		private float m_Latitude;

		private float m_Longitude;

		private float m_Altitude;

		private float m_HorizontalAccuracy;

		private float m_VerticalAccuracy;

		public float latitude
		{
			get
			{
				return this.m_Latitude;
			}
		}

		public float longitude
		{
			get
			{
				return this.m_Longitude;
			}
		}

		public float altitude
		{
			get
			{
				return this.m_Altitude;
			}
		}

		public float horizontalAccuracy
		{
			get
			{
				return this.m_HorizontalAccuracy;
			}
		}

		public float verticalAccuracy
		{
			get
			{
				return this.m_VerticalAccuracy;
			}
		}

		public double timestamp
		{
			get
			{
				return this.m_Timestamp;
			}
		}
	}
}
