using System;

namespace UnityEngine
{
	internal class AndroidJNISafe
	{
		public static void CheckException()
		{
			IntPtr intPtr = AndroidJNI.ExceptionOccurred();
			if (intPtr != IntPtr.Zero)
			{
				AndroidJNI.ExceptionClear();
				IntPtr intPtr2 = AndroidJNI.FindClass("java/lang/Throwable");
				IntPtr intPtr3 = AndroidJNI.FindClass("android/util/Log");
				try
				{
					IntPtr methodID = AndroidJNI.GetMethodID(intPtr2, "toString", "()Ljava/lang/String;");
					IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(intPtr3, "getStackTraceString", "(Ljava/lang/Throwable;)Ljava/lang/String;");
					string message = AndroidJNI.CallStringMethod(intPtr, methodID, new jvalue[0]);
					jvalue[] array = new jvalue[1];
					array[0].l = intPtr;
					string javaStackTrace = AndroidJNI.CallStaticStringMethod(intPtr3, staticMethodID, array);
					throw new AndroidJavaException(message, javaStackTrace);
				}
				finally
				{
					AndroidJNISafe.DeleteLocalRef(intPtr);
					AndroidJNISafe.DeleteLocalRef(intPtr2);
					AndroidJNISafe.DeleteLocalRef(intPtr3);
				}
			}
		}

		public static void DeleteGlobalRef(IntPtr globalref)
		{
			if (globalref != IntPtr.Zero)
			{
				AndroidJNI.DeleteGlobalRef(globalref);
			}
		}

		public static void DeleteLocalRef(IntPtr localref)
		{
			if (localref != IntPtr.Zero)
			{
				AndroidJNI.DeleteLocalRef(localref);
			}
		}

		public static IntPtr NewStringUTF(string bytes)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.NewStringUTF(bytes);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static string GetStringUTFChars(IntPtr str)
		{
			string stringUTFChars;
			try
			{
				stringUTFChars = AndroidJNI.GetStringUTFChars(str);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return stringUTFChars;
		}

		public static IntPtr GetObjectClass(IntPtr ptr)
		{
			IntPtr objectClass;
			try
			{
				objectClass = AndroidJNI.GetObjectClass(ptr);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return objectClass;
		}

		public static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig)
		{
			IntPtr staticMethodID;
			try
			{
				staticMethodID = AndroidJNI.GetStaticMethodID(clazz, name, sig);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticMethodID;
		}

		public static IntPtr GetMethodID(IntPtr obj, string name, string sig)
		{
			IntPtr methodID;
			try
			{
				methodID = AndroidJNI.GetMethodID(obj, name, sig);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return methodID;
		}

		public static IntPtr GetFieldID(IntPtr clazz, string name, string sig)
		{
			IntPtr fieldID;
			try
			{
				fieldID = AndroidJNI.GetFieldID(clazz, name, sig);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return fieldID;
		}

		public static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig)
		{
			IntPtr staticFieldID;
			try
			{
				staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, sig);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticFieldID;
		}

		public static IntPtr FromReflectedMethod(IntPtr refMethod)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.FromReflectedMethod(refMethod);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr FromReflectedField(IntPtr refField)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.FromReflectedField(refField);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr FindClass(string name)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.FindClass(name);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.NewObject(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val)
		{
			try
			{
				AndroidJNI.SetStaticObjectField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val)
		{
			try
			{
				AndroidJNI.SetStaticStringField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val)
		{
			try
			{
				AndroidJNI.SetStaticCharField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val)
		{
			try
			{
				AndroidJNI.SetStaticDoubleField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val)
		{
			try
			{
				AndroidJNI.SetStaticFloatField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val)
		{
			try
			{
				AndroidJNI.SetStaticLongField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val)
		{
			try
			{
				AndroidJNI.SetStaticShortField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticByteField(IntPtr clazz, IntPtr fieldID, byte val)
		{
			try
			{
				AndroidJNI.SetStaticByteField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val)
		{
			try
			{
				AndroidJNI.SetStaticBooleanField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val)
		{
			try
			{
				AndroidJNI.SetStaticIntField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID)
		{
			IntPtr staticObjectField;
			try
			{
				staticObjectField = AndroidJNI.GetStaticObjectField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticObjectField;
		}

		public static string GetStaticStringField(IntPtr clazz, IntPtr fieldID)
		{
			string staticStringField;
			try
			{
				staticStringField = AndroidJNI.GetStaticStringField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticStringField;
		}

		public static char GetStaticCharField(IntPtr clazz, IntPtr fieldID)
		{
			char staticCharField;
			try
			{
				staticCharField = AndroidJNI.GetStaticCharField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticCharField;
		}

		public static double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID)
		{
			double staticDoubleField;
			try
			{
				staticDoubleField = AndroidJNI.GetStaticDoubleField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticDoubleField;
		}

		public static float GetStaticFloatField(IntPtr clazz, IntPtr fieldID)
		{
			float staticFloatField;
			try
			{
				staticFloatField = AndroidJNI.GetStaticFloatField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticFloatField;
		}

		public static long GetStaticLongField(IntPtr clazz, IntPtr fieldID)
		{
			long staticLongField;
			try
			{
				staticLongField = AndroidJNI.GetStaticLongField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticLongField;
		}

		public static short GetStaticShortField(IntPtr clazz, IntPtr fieldID)
		{
			short staticShortField;
			try
			{
				staticShortField = AndroidJNI.GetStaticShortField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticShortField;
		}

		public static byte GetStaticByteField(IntPtr clazz, IntPtr fieldID)
		{
			byte staticByteField;
			try
			{
				staticByteField = AndroidJNI.GetStaticByteField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticByteField;
		}

		public static bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID)
		{
			bool staticBooleanField;
			try
			{
				staticBooleanField = AndroidJNI.GetStaticBooleanField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticBooleanField;
		}

		public static int GetStaticIntField(IntPtr clazz, IntPtr fieldID)
		{
			int staticIntField;
			try
			{
				staticIntField = AndroidJNI.GetStaticIntField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticIntField;
		}

		public static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			try
			{
				AndroidJNI.CallStaticVoidMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.CallStaticObjectMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			string result;
			try
			{
				result = AndroidJNI.CallStaticStringMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			char result;
			try
			{
				result = AndroidJNI.CallStaticCharMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			double result;
			try
			{
				result = AndroidJNI.CallStaticDoubleMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			float result;
			try
			{
				result = AndroidJNI.CallStaticFloatMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			long result;
			try
			{
				result = AndroidJNI.CallStaticLongMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			short result;
			try
			{
				result = AndroidJNI.CallStaticShortMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			byte result;
			try
			{
				result = AndroidJNI.CallStaticByteMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			bool result;
			try
			{
				result = AndroidJNI.CallStaticBooleanMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			int result;
			try
			{
				result = AndroidJNI.CallStaticIntMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val)
		{
			try
			{
				AndroidJNI.SetObjectField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStringField(IntPtr obj, IntPtr fieldID, string val)
		{
			try
			{
				AndroidJNI.SetStringField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetCharField(IntPtr obj, IntPtr fieldID, char val)
		{
			try
			{
				AndroidJNI.SetCharField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetDoubleField(IntPtr obj, IntPtr fieldID, double val)
		{
			try
			{
				AndroidJNI.SetDoubleField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetFloatField(IntPtr obj, IntPtr fieldID, float val)
		{
			try
			{
				AndroidJNI.SetFloatField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetLongField(IntPtr obj, IntPtr fieldID, long val)
		{
			try
			{
				AndroidJNI.SetLongField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetShortField(IntPtr obj, IntPtr fieldID, short val)
		{
			try
			{
				AndroidJNI.SetShortField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetByteField(IntPtr obj, IntPtr fieldID, byte val)
		{
			try
			{
				AndroidJNI.SetByteField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val)
		{
			try
			{
				AndroidJNI.SetBooleanField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetIntField(IntPtr obj, IntPtr fieldID, int val)
		{
			try
			{
				AndroidJNI.SetIntField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr GetObjectField(IntPtr obj, IntPtr fieldID)
		{
			IntPtr objectField;
			try
			{
				objectField = AndroidJNI.GetObjectField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return objectField;
		}

		public static string GetStringField(IntPtr obj, IntPtr fieldID)
		{
			string stringField;
			try
			{
				stringField = AndroidJNI.GetStringField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return stringField;
		}

		public static char GetCharField(IntPtr obj, IntPtr fieldID)
		{
			char charField;
			try
			{
				charField = AndroidJNI.GetCharField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return charField;
		}

		public static double GetDoubleField(IntPtr obj, IntPtr fieldID)
		{
			double doubleField;
			try
			{
				doubleField = AndroidJNI.GetDoubleField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return doubleField;
		}

		public static float GetFloatField(IntPtr obj, IntPtr fieldID)
		{
			float floatField;
			try
			{
				floatField = AndroidJNI.GetFloatField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return floatField;
		}

		public static long GetLongField(IntPtr obj, IntPtr fieldID)
		{
			long longField;
			try
			{
				longField = AndroidJNI.GetLongField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return longField;
		}

		public static short GetShortField(IntPtr obj, IntPtr fieldID)
		{
			short shortField;
			try
			{
				shortField = AndroidJNI.GetShortField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return shortField;
		}

		public static byte GetByteField(IntPtr obj, IntPtr fieldID)
		{
			byte byteField;
			try
			{
				byteField = AndroidJNI.GetByteField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return byteField;
		}

		public static bool GetBooleanField(IntPtr obj, IntPtr fieldID)
		{
			bool booleanField;
			try
			{
				booleanField = AndroidJNI.GetBooleanField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return booleanField;
		}

		public static int GetIntField(IntPtr obj, IntPtr fieldID)
		{
			int intField;
			try
			{
				intField = AndroidJNI.GetIntField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intField;
		}

		public static void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			try
			{
				AndroidJNI.CallVoidMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.CallObjectMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			string result;
			try
			{
				result = AndroidJNI.CallStringMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			char result;
			try
			{
				result = AndroidJNI.CallCharMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			double result;
			try
			{
				result = AndroidJNI.CallDoubleMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			float result;
			try
			{
				result = AndroidJNI.CallFloatMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			long result;
			try
			{
				result = AndroidJNI.CallLongMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			short result;
			try
			{
				result = AndroidJNI.CallShortMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			byte result;
			try
			{
				result = AndroidJNI.CallByteMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			bool result;
			try
			{
				result = AndroidJNI.CallBooleanMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			int result;
			try
			{
				result = AndroidJNI.CallIntMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr[] FromObjectArray(IntPtr array)
		{
			IntPtr[] result;
			try
			{
				result = AndroidJNI.FromObjectArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static char[] FromCharArray(IntPtr array)
		{
			char[] result;
			try
			{
				result = AndroidJNI.FromCharArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static double[] FromDoubleArray(IntPtr array)
		{
			double[] result;
			try
			{
				result = AndroidJNI.FromDoubleArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static float[] FromFloatArray(IntPtr array)
		{
			float[] result;
			try
			{
				result = AndroidJNI.FromFloatArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static long[] FromLongArray(IntPtr array)
		{
			long[] result;
			try
			{
				result = AndroidJNI.FromLongArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static short[] FromShortArray(IntPtr array)
		{
			short[] result;
			try
			{
				result = AndroidJNI.FromShortArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static byte[] FromByteArray(IntPtr array)
		{
			byte[] result;
			try
			{
				result = AndroidJNI.FromByteArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static bool[] FromBooleanArray(IntPtr array)
		{
			bool[] result;
			try
			{
				result = AndroidJNI.FromBooleanArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static int[] FromIntArray(IntPtr array)
		{
			int[] result;
			try
			{
				result = AndroidJNI.FromIntArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToObjectArray(IntPtr[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToObjectArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToObjectArray(IntPtr[] array, IntPtr type)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToObjectArray(array, type);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToCharArray(char[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToCharArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToDoubleArray(double[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToDoubleArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToFloatArray(float[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToFloatArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToLongArray(long[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToLongArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToShortArray(short[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToShortArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToByteArray(byte[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToByteArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToBooleanArray(bool[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToBooleanArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr ToIntArray(int[] array)
		{
			IntPtr result;
			try
			{
				result = AndroidJNI.ToIntArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return result;
		}

		public static IntPtr GetObjectArrayElement(IntPtr array, int index)
		{
			IntPtr objectArrayElement;
			try
			{
				objectArrayElement = AndroidJNI.GetObjectArrayElement(array, index);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return objectArrayElement;
		}

		public static int GetArrayLength(IntPtr array)
		{
			int arrayLength;
			try
			{
				arrayLength = AndroidJNI.GetArrayLength(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return arrayLength;
		}
	}
}
