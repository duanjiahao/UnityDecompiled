using System;
using System.Reflection;
using System.Threading;
using UnityEngineInternal;

namespace UnityEngine.Events
{
	internal class InvokableCall : BaseInvokableCall
	{
		private event UnityAction Delegate
		{
			add
			{
				UnityAction unityAction = this.Delegate;
				UnityAction unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction>(ref this.Delegate, (UnityAction)System.Delegate.Combine(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
			remove
			{
				UnityAction unityAction = this.Delegate;
				UnityAction unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction>(ref this.Delegate, (UnityAction)System.Delegate.Remove(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
		}

		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate += (UnityAction)theFunction.CreateDelegate(typeof(UnityAction), target);
		}

		public InvokableCall(UnityAction action)
		{
			this.Delegate += action;
		}

		public override void Invoke(object[] args)
		{
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate();
			}
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}
	}
	internal class InvokableCall<T1> : BaseInvokableCall
	{
		protected event UnityAction<T1> Delegate
		{
			add
			{
				UnityAction<T1> unityAction = this.Delegate;
				UnityAction<T1> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<T1>>(ref this.Delegate, (UnityAction<T1>)System.Delegate.Combine(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
			remove
			{
				UnityAction<T1> unityAction = this.Delegate;
				UnityAction<T1> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<T1>>(ref this.Delegate, (UnityAction<T1>)System.Delegate.Remove(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
		}

		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate += (UnityAction<T1>)theFunction.CreateDelegate(typeof(UnityAction<T1>), target);
		}

		public InvokableCall(UnityAction<T1> action)
		{
			this.Delegate += action;
		}

		public override void Invoke(object[] args)
		{
			if (args.Length != 1)
			{
				throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
			}
			BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate((T1)((object)args[0]));
			}
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}
	}
	internal class InvokableCall<T1, T2> : BaseInvokableCall
	{
		protected event UnityAction<T1, T2> Delegate
		{
			add
			{
				UnityAction<T1, T2> unityAction = this.Delegate;
				UnityAction<T1, T2> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<T1, T2>>(ref this.Delegate, (UnityAction<T1, T2>)System.Delegate.Combine(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
			remove
			{
				UnityAction<T1, T2> unityAction = this.Delegate;
				UnityAction<T1, T2> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<T1, T2>>(ref this.Delegate, (UnityAction<T1, T2>)System.Delegate.Remove(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
		}

		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1, T2>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2>), target);
		}

		public InvokableCall(UnityAction<T1, T2> action)
		{
			this.Delegate += action;
		}

		public override void Invoke(object[] args)
		{
			if (args.Length != 2)
			{
				throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
			}
			BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
			BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate((T1)((object)args[0]), (T2)((object)args[1]));
			}
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}
	}
	internal class InvokableCall<T1, T2, T3> : BaseInvokableCall
	{
		protected event UnityAction<T1, T2, T3> Delegate
		{
			add
			{
				UnityAction<T1, T2, T3> unityAction = this.Delegate;
				UnityAction<T1, T2, T3> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<T1, T2, T3>>(ref this.Delegate, (UnityAction<T1, T2, T3>)System.Delegate.Combine(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
			remove
			{
				UnityAction<T1, T2, T3> unityAction = this.Delegate;
				UnityAction<T1, T2, T3> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<T1, T2, T3>>(ref this.Delegate, (UnityAction<T1, T2, T3>)System.Delegate.Remove(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
		}

		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1, T2, T3>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3>), target);
		}

		public InvokableCall(UnityAction<T1, T2, T3> action)
		{
			this.Delegate += action;
		}

		public override void Invoke(object[] args)
		{
			if (args.Length != 3)
			{
				throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
			}
			BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
			BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
			BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate((T1)((object)args[0]), (T2)((object)args[1]), (T3)((object)args[2]));
			}
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}
	}
	internal class InvokableCall<T1, T2, T3, T4> : BaseInvokableCall
	{
		protected event UnityAction<T1, T2, T3, T4> Delegate
		{
			add
			{
				UnityAction<T1, T2, T3, T4> unityAction = this.Delegate;
				UnityAction<T1, T2, T3, T4> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<T1, T2, T3, T4>>(ref this.Delegate, (UnityAction<T1, T2, T3, T4>)System.Delegate.Combine(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
			remove
			{
				UnityAction<T1, T2, T3, T4> unityAction = this.Delegate;
				UnityAction<T1, T2, T3, T4> unityAction2;
				do
				{
					unityAction2 = unityAction;
					unityAction = Interlocked.CompareExchange<UnityAction<T1, T2, T3, T4>>(ref this.Delegate, (UnityAction<T1, T2, T3, T4>)System.Delegate.Remove(unityAction2, value), unityAction);
				}
				while (unityAction != unityAction2);
			}
		}

		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1, T2, T3, T4>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3, T4>), target);
		}

		public InvokableCall(UnityAction<T1, T2, T3, T4> action)
		{
			this.Delegate += action;
		}

		public override void Invoke(object[] args)
		{
			if (args.Length != 4)
			{
				throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
			}
			BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
			BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
			BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
			BaseInvokableCall.ThrowOnInvalidArg<T4>(args[3]);
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate((T1)((object)args[0]), (T2)((object)args[1]), (T3)((object)args[2]), (T4)((object)args[3]));
			}
		}

		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}
	}
}
