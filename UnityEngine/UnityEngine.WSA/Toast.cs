using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.WSA
{
	public sealed class Toast
	{
		private int m_ToastId;

		public string arguments
		{
			get
			{
				return Toast.GetArguments(this.m_ToastId);
			}
			set
			{
				Toast.SetArguments(this.m_ToastId, value);
			}
		}

		public bool activated
		{
			get
			{
				return Toast.GetActivated(this.m_ToastId);
			}
		}

		public bool dismissed
		{
			get
			{
				return Toast.GetDismissed(this.m_ToastId, false);
			}
		}

		public bool dismissedByUser
		{
			get
			{
				return Toast.GetDismissed(this.m_ToastId, true);
			}
		}

		private Toast(int id)
		{
			this.m_ToastId = id;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetTemplate(ToastTemplate templ);

		public static Toast Create(string xml)
		{
			int num = Toast.CreateToastXml(xml);
			Toast result;
			if (num < 0)
			{
				result = null;
			}
			else
			{
				result = new Toast(num);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CreateToastXml(string xml);

		public static Toast Create(string image, string text)
		{
			int num = Toast.CreateToastImageAndText(image, text);
			Toast result;
			if (num < 0)
			{
				result = null;
			}
			else
			{
				result = new Toast(num);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CreateToastImageAndText(string image, string text);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetArguments(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetArguments(int id, string args);

		public void Show()
		{
			Toast.Show(this.m_ToastId);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Show(int id);

		public void Hide()
		{
			Toast.Hide(this.m_ToastId);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Hide(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetActivated(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetDismissed(int id, bool byUser);
	}
}
