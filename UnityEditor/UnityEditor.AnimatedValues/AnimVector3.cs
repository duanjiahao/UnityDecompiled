using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.AnimatedValues
{
	[Serializable]
	public class AnimVector3 : BaseAnimValue<Vector3>
	{
		[SerializeField]
		private Vector3 m_Value;

		public AnimVector3() : base(Vector3.zero)
		{
		}

		public AnimVector3(Vector3 value) : base(value)
		{
		}

		public AnimVector3(Vector3 value, UnityAction callback) : base(value, callback)
		{
		}

		protected override Vector3 GetValue()
		{
			this.m_Value = Vector3.Lerp(base.start, base.target, base.lerpPosition);
			return this.m_Value;
		}
	}
}
