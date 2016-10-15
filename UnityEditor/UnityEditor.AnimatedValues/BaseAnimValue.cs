using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.AnimatedValues
{
	public abstract class BaseAnimValue<T>
	{
		private T m_Start;

		[SerializeField]
		private T m_Target;

		private double m_LastTime;

		private double m_LerpPosition = 1.0;

		public float speed = 2f;

		[NonSerialized]
		public UnityEvent valueChanged;

		private bool m_Animating;

		public bool isAnimating
		{
			get
			{
				return this.m_Animating;
			}
		}

		protected float lerpPosition
		{
			get
			{
				double num = 1.0 - this.m_LerpPosition;
				double num2 = 1.0 - num * num * num * num;
				return (float)num2;
			}
		}

		protected T start
		{
			get
			{
				return this.m_Start;
			}
		}

		public T target
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				if (!this.m_Target.Equals(value))
				{
					this.BeginAnimating(value, this.value);
				}
			}
		}

		public T value
		{
			get
			{
				return this.GetValue();
			}
			set
			{
				this.StopAnim(value);
			}
		}

		protected BaseAnimValue(T value)
		{
			this.m_Start = value;
			this.m_Target = value;
			this.valueChanged = new UnityEvent();
		}

		protected BaseAnimValue(T value, UnityAction callback)
		{
			this.m_Start = value;
			this.m_Target = value;
			this.valueChanged = new UnityEvent();
			this.valueChanged.AddListener(callback);
		}

		private static T2 Clamp<T2>(T2 val, T2 min, T2 max) where T2 : IComparable<T2>
		{
			if (val.CompareTo(min) < 0)
			{
				return min;
			}
			if (val.CompareTo(max) > 0)
			{
				return max;
			}
			return val;
		}

		protected void BeginAnimating(T newTarget, T newStart)
		{
			this.m_Start = newStart;
			this.m_Target = newTarget;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			this.m_Animating = true;
			this.m_LastTime = EditorApplication.timeSinceStartup;
			this.m_LerpPosition = 0.0;
		}

		private void Update()
		{
			if (!this.m_Animating)
			{
				return;
			}
			this.UpdateLerpPosition();
			if (this.valueChanged != null)
			{
				this.valueChanged.Invoke();
			}
			if (this.lerpPosition >= 1f)
			{
				this.m_Animating = false;
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			}
		}

		private void UpdateLerpPosition()
		{
			double timeSinceStartup = EditorApplication.timeSinceStartup;
			double num = timeSinceStartup - this.m_LastTime;
			this.m_LerpPosition = BaseAnimValue<T>.Clamp<double>(this.m_LerpPosition + num * (double)this.speed, 0.0, 1.0);
			this.m_LastTime = timeSinceStartup;
		}

		protected void StopAnim(T newValue)
		{
			bool flag = false;
			if ((!newValue.Equals(this.GetValue()) || this.m_LerpPosition < 1.0) && this.valueChanged != null)
			{
				flag = true;
			}
			this.m_Target = newValue;
			this.m_Start = newValue;
			this.m_LerpPosition = 1.0;
			this.m_Animating = false;
			if (flag)
			{
				this.valueChanged.Invoke();
			}
		}

		protected abstract T GetValue();
	}
}
