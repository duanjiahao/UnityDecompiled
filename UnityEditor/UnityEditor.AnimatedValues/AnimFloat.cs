using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.AnimatedValues
{
	[Serializable]
	public class AnimFloat : BaseAnimValue<float>
	{
		[SerializeField]
		private float m_Value;

		public AnimFloat(float value) : base(value)
		{
		}

		public AnimFloat(float value, UnityAction callback) : base(value, callback)
		{
		}

		protected override float GetValue()
		{
			this.m_Value = Mathf.Lerp(base.start, base.target, base.lerpPosition);
			return this.m_Value;
		}
	}
}
