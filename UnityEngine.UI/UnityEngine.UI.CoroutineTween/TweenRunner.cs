using System;
using System.Collections;
using System.Diagnostics;

namespace UnityEngine.UI.CoroutineTween
{
	internal class TweenRunner<T> where T : struct, ITweenValue
	{
		protected MonoBehaviour m_CoroutineContainer;

		protected IEnumerator m_Tween;

		[DebuggerHidden]
		private static IEnumerator Start(T tweenInfo)
		{
			TweenRunner<T>.<Start>c__Iterator0 <Start>c__Iterator = new TweenRunner<T>.<Start>c__Iterator0();
			<Start>c__Iterator.tweenInfo = tweenInfo;
			return <Start>c__Iterator;
		}

		public void Init(MonoBehaviour coroutineContainer)
		{
			this.m_CoroutineContainer = coroutineContainer;
		}

		public void StartTween(T info)
		{
			if (this.m_CoroutineContainer == null)
			{
				UnityEngine.Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
			}
			else
			{
				this.StopTween();
				if (!this.m_CoroutineContainer.gameObject.activeInHierarchy)
				{
					info.TweenValue(1f);
				}
				else
				{
					this.m_Tween = TweenRunner<T>.Start(info);
					this.m_CoroutineContainer.StartCoroutine(this.m_Tween);
				}
			}
		}

		public void StopTween()
		{
			if (this.m_Tween != null)
			{
				this.m_CoroutineContainer.StopCoroutine(this.m_Tween);
				this.m_Tween = null;
			}
		}
	}
}
