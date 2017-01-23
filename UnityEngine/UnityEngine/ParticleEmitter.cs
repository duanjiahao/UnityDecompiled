using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	[Obsolete("This component is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false)]
	public class ParticleEmitter : Component
	{
		public extern bool emit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float minSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float minEnergy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxEnergy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float minEmission
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxEmission
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float emitterVelocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 worldVelocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_worldVelocity(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_worldVelocity(ref value);
			}
		}

		public Vector3 localVelocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_localVelocity(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_localVelocity(ref value);
			}
		}

		public Vector3 rndVelocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_rndVelocity(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_rndVelocity(ref value);
			}
		}

		public extern bool useWorldSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool rndRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float angularVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float rndAngularVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Particle[] particles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int particleCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal ParticleEmitter()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_worldVelocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_worldVelocity(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_localVelocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_localVelocity(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rndVelocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rndVelocity(ref Vector3 value);

		public void ClearParticles()
		{
			ParticleEmitter.INTERNAL_CALL_ClearParticles(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearParticles(ParticleEmitter self);

		public void Emit()
		{
			this.Emit2((int)Random.Range(this.minEmission, this.maxEmission));
		}

		public void Emit(int count)
		{
			this.Emit2(count);
		}

		public void Emit(Vector3 pos, Vector3 velocity, float size, float energy, Color color)
		{
			InternalEmitParticleArguments internalEmitParticleArguments = default(InternalEmitParticleArguments);
			internalEmitParticleArguments.pos = pos;
			internalEmitParticleArguments.velocity = velocity;
			internalEmitParticleArguments.size = size;
			internalEmitParticleArguments.energy = energy;
			internalEmitParticleArguments.color = color;
			internalEmitParticleArguments.rotation = 0f;
			internalEmitParticleArguments.angularVelocity = 0f;
			this.Emit3(ref internalEmitParticleArguments);
		}

		public void Emit(Vector3 pos, Vector3 velocity, float size, float energy, Color color, float rotation, float angularVelocity)
		{
			InternalEmitParticleArguments internalEmitParticleArguments = default(InternalEmitParticleArguments);
			internalEmitParticleArguments.pos = pos;
			internalEmitParticleArguments.velocity = velocity;
			internalEmitParticleArguments.size = size;
			internalEmitParticleArguments.energy = energy;
			internalEmitParticleArguments.color = color;
			internalEmitParticleArguments.rotation = rotation;
			internalEmitParticleArguments.angularVelocity = angularVelocity;
			this.Emit3(ref internalEmitParticleArguments);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Emit2(int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Emit3(ref InternalEmitParticleArguments args);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Simulate(float deltaTime);
	}
}
