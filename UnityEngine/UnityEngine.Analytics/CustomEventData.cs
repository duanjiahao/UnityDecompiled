using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[StructLayout(LayoutKind.Sequential)]
	internal class CustomEventData : IDisposable
	{
		[NonSerialized]
		internal IntPtr m_Ptr;

		private CustomEventData()
		{
		}

		public CustomEventData(string name)
		{
			this.InternalCreate(name);
		}

		~CustomEventData()
		{
			this.InternalDestroy();
		}

		public void Dispose()
		{
			this.InternalDestroy();
			GC.SuppressFinalize(this);
		}

		public bool Add(string key, string value)
		{
			return this.AddString(key, value);
		}

		public bool Add(string key, bool value)
		{
			return this.AddBool(key, value);
		}

		public bool Add(string key, char value)
		{
			return this.AddChar(key, value);
		}

		public bool Add(string key, byte value)
		{
			return this.AddByte(key, value);
		}

		public bool Add(string key, sbyte value)
		{
			return this.AddSByte(key, value);
		}

		public bool Add(string key, short value)
		{
			return this.AddInt16(key, value);
		}

		public bool Add(string key, ushort value)
		{
			return this.AddUInt16(key, value);
		}

		public bool Add(string key, int value)
		{
			return this.AddInt32(key, value);
		}

		public bool Add(string key, uint value)
		{
			return this.AddUInt32(key, value);
		}

		public bool Add(string key, long value)
		{
			return this.AddInt64(key, value);
		}

		public bool Add(string key, ulong value)
		{
			return this.AddUInt64(key, value);
		}

		public bool Add(string key, float value)
		{
			return this.AddDouble(key, (double)Convert.ToDecimal(value));
		}

		public bool Add(string key, double value)
		{
			return this.AddDouble(key, value);
		}

		public bool Add(string key, decimal value)
		{
			return this.AddDouble(key, (double)Convert.ToDecimal(value));
		}

		public bool Add(IDictionary<string, object> eventData)
		{
			foreach (KeyValuePair<string, object> current in eventData)
			{
				string key = current.Key;
				object value = current.Value;
				if (value == null)
				{
					this.Add(key, "null");
				}
				else
				{
					Type type = value.GetType();
					if (type == typeof(string))
					{
						this.Add(key, (string)value);
					}
					else if (type == typeof(char))
					{
						this.Add(key, (char)value);
					}
					else if (type == typeof(sbyte))
					{
						this.Add(key, (sbyte)value);
					}
					else if (type == typeof(byte))
					{
						this.Add(key, (byte)value);
					}
					else if (type == typeof(short))
					{
						this.Add(key, (short)value);
					}
					else if (type == typeof(ushort))
					{
						this.Add(key, (ushort)value);
					}
					else if (type == typeof(int))
					{
						this.Add(key, (int)value);
					}
					else if (type == typeof(uint))
					{
						this.Add(current.Key, (uint)value);
					}
					else if (type == typeof(long))
					{
						this.Add(key, (long)value);
					}
					else if (type == typeof(ulong))
					{
						this.Add(key, (ulong)value);
					}
					else if (type == typeof(bool))
					{
						this.Add(key, (bool)value);
					}
					else if (type == typeof(float))
					{
						this.Add(key, (float)value);
					}
					else if (type == typeof(double))
					{
						this.Add(key, (double)value);
					}
					else if (type == typeof(decimal))
					{
						this.Add(key, (decimal)value);
					}
					else
					{
						if (!type.IsValueType)
						{
							throw new ArgumentException(string.Format("Invalid type: {0} passed", type));
						}
						this.Add(key, value.ToString());
					}
				}
			}
			return true;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InternalCreate(string name);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalDestroy();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddString(string key, string value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddBool(string key, bool value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddChar(string key, char value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddByte(string key, byte value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddSByte(string key, sbyte value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddInt16(string key, short value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddUInt16(string key, ushort value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddInt32(string key, int value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddUInt32(string key, uint value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddInt64(string key, long value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddUInt64(string key, ulong value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddDouble(string key, double value);
	}
}
