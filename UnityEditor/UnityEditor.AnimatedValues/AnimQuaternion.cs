using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.AnimatedValues
{
	[Serializable]
	public class AnimQuaternion : BaseAnimValue<Quaternion>
	{
		[SerializeField]
		private Quaternion m_Value;

		public AnimQuaternion(Quaternion value) : base(value)
		{
		}

		public AnimQuaternion(Quaternion value, UnityAction callback) : base(value, callback)
		{
		}

		protected override Quaternion GetValue()
		{
			this.m_Value = Quaternion.Slerp(base.start, base.target, base.lerpPosition);
			return this.m_Value;
		}
	}
}
