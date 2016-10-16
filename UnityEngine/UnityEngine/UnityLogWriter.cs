using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace UnityEngine
{
	internal sealed class UnityLogWriter : TextWriter
	{
		public override Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WriteStringToUnityLog(string s);

		public static void Init()
		{
			Console.SetOut(new UnityLogWriter());
		}

		public override void Write(char value)
		{
			UnityLogWriter.WriteStringToUnityLog(value.ToString());
		}

		public override void Write(string s)
		{
			UnityLogWriter.WriteStringToUnityLog(s);
		}
	}
}
