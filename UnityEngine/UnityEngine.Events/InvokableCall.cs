using System;
using System.Reflection;
using System.Runtime.CompilerServices;
namespace UnityEngine.Events
{
	internal class InvokableCall : BaseInvokableCall
	{
		private event UnityAction Delegate
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Delegate = (UnityAction)System.Delegate.Combine(this.Delegate, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Delegate = (UnityAction)System.Delegate.Remove(this.Delegate, value);
			}
		}
		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (UnityAction)System.Delegate.Combine(this.Delegate, System.Delegate.CreateDelegate(typeof(UnityAction), target, theFunction) as UnityAction);
		}
		public InvokableCall(UnityAction action)
		{
			this.Delegate = (UnityAction)System.Delegate.Combine(this.Delegate, action);
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
			return this.Delegate.Target == targetObj && this.Delegate.Method == method;
		}
	}
	internal class InvokableCall<T1> : BaseInvokableCall
	{
		protected event UnityAction<T1> Delegate
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Delegate = (UnityAction<T1>)System.Delegate.Combine(this.Delegate, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Delegate = (UnityAction<T1>)System.Delegate.Remove(this.Delegate, value);
			}
		}
		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1>)System.Delegate.Combine(this.Delegate, System.Delegate.CreateDelegate(typeof(UnityAction<T1>), target, theFunction) as UnityAction<T1>);
		}
		public InvokableCall(UnityAction<T1> callback)
		{
			this.Delegate = (UnityAction<T1>)System.Delegate.Combine(this.Delegate, callback);
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
			return this.Delegate.Target == targetObj && this.Delegate.Method == method;
		}
	}
	internal class InvokableCall<T1, T2> : BaseInvokableCall
	{
		protected event UnityAction<T1, T2> Delegate
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Delegate = (UnityAction<T1, T2>)System.Delegate.Combine(this.Delegate, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Delegate = (UnityAction<T1, T2>)System.Delegate.Remove(this.Delegate, value);
			}
		}
		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (System.Delegate.CreateDelegate(typeof(UnityAction<T1, T2>), target, theFunction) as UnityAction<T1, T2>);
		}
		public InvokableCall(UnityAction<T1, T2> action)
		{
			this.Delegate = (UnityAction<T1, T2>)System.Delegate.Combine(this.Delegate, action);
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
			return this.Delegate.Target == targetObj && this.Delegate.Method == method;
		}
	}
	internal class InvokableCall<T1, T2, T3> : BaseInvokableCall
	{
		protected event UnityAction<T1, T2, T3> Delegate
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Delegate = (UnityAction<T1, T2, T3>)System.Delegate.Combine(this.Delegate, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Delegate = (UnityAction<T1, T2, T3>)System.Delegate.Remove(this.Delegate, value);
			}
		}
		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1, T2, T3>)System.Delegate.Combine(this.Delegate, System.Delegate.CreateDelegate(typeof(UnityAction<T1, T2, T3>), target, theFunction) as UnityAction<T1, T2, T3>);
		}
		public InvokableCall(UnityAction<T1, T2, T3> action)
		{
			this.Delegate = (UnityAction<T1, T2, T3>)System.Delegate.Combine(this.Delegate, action);
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
			return this.Delegate.Target == targetObj && this.Delegate.Method == method;
		}
	}
	internal class InvokableCall<T1, T2, T3, T4> : BaseInvokableCall
	{
		protected event UnityAction<T1, T2, T3, T4> Delegate
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Delegate = (UnityAction<T1, T2, T3, T4>)System.Delegate.Combine(this.Delegate, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Delegate = (UnityAction<T1, T2, T3, T4>)System.Delegate.Remove(this.Delegate, value);
			}
		}
		public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1, T2, T3, T4>)System.Delegate.Combine(this.Delegate, System.Delegate.CreateDelegate(typeof(UnityAction<T1, T2, T3, T4>), target, theFunction) as UnityAction<T1, T2, T3, T4>);
		}
		public InvokableCall(UnityAction<T1, T2, T3, T4> action)
		{
			this.Delegate = (UnityAction<T1, T2, T3, T4>)System.Delegate.Combine(this.Delegate, action);
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
			return this.Delegate.Target == targetObj && this.Delegate.Method == method;
		}
	}
}
