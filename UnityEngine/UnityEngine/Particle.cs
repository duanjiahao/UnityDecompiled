using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[Obsolete("This is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false), UsedByNativeCode]
	public struct Particle
	{
		private Vector3 m_Position;

		private Vector3 m_Velocity;

		private float m_Size;

		private float m_Rotation;

		private float m_AngularVelocity;

		private float m_Energy;

		private float m_StartEnergy;

		private Color m_Color;

		public Vector3 position
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

		public Vector3 velocity
		{
			get
			{
				return this.m_Velocity;
			}
			set
			{
				this.m_Velocity = value;
			}
		}

		public float energy
		{
			get
			{
				return this.m_Energy;
			}
			set
			{
				this.m_Energy = value;
			}
		}

		public float startEnergy
		{
			get
			{
				return this.m_StartEnergy;
			}
			set
			{
				this.m_StartEnergy = value;
			}
		}

		public float size
		{
			get
			{
				return this.m_Size;
			}
			set
			{
				this.m_Size = value;
			}
		}

		public float rotation
		{
			get
			{
				return this.m_Rotation;
			}
			set
			{
				this.m_Rotation = value;
			}
		}

		public float angularVelocity
		{
			get
			{
				return this.m_AngularVelocity;
			}
			set
			{
				this.m_AngularVelocity = value;
			}
		}

		public Color color
		{
			get
			{
				return this.m_Color;
			}
			set
			{
				this.m_Color = value;
			}
		}
	}
}
