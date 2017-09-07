using System;

namespace UnityEngine
{
	internal class AndroidReflection
	{
		private const string RELECTION_HELPER_CLASS_NAME = "com/unity3d/player/ReflectionHelper";

		private static readonly GlobalJavaObjectRef s_ReflectionHelperClass = new GlobalJavaObjectRef(AndroidJNISafe.FindClass("com/unity3d/player/ReflectionHelper"));

		private static readonly IntPtr s_ReflectionHelperGetConstructorID = AndroidReflection.GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getConstructorID", "(Ljava/lang/Class;Ljava/lang/String;)Ljava/lang/reflect/Constructor;");

		private static readonly IntPtr s_ReflectionHelperGetMethodID = AndroidReflection.GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getMethodID", "(Ljava/lang/Class;Ljava/lang/String;Ljava/lang/String;Z)Ljava/lang/reflect/Method;");

		private static readonly IntPtr s_ReflectionHelperGetFieldID = AndroidReflection.GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getFieldID", "(Ljava/lang/Class;Ljava/lang/String;Ljava/lang/String;Z)Ljava/lang/reflect/Field;");

		private static readonly IntPtr s_ReflectionHelperNewProxyInstance = AndroidReflection.GetStaticMethodID("com/unity3d/player/ReflectionHelper", "newProxyInstance", "(ILjava/lang/Class;)Ljava/lang/Object;");

		public static bool IsPrimitive(Type t)
		{
			return t.IsPrimitive;
		}

		public static bool IsAssignableFrom(Type t, Type from)
		{
			return t.IsAssignableFrom(from);
		}

		private static IntPtr GetStaticMethodID(string clazz, string methodName, string signature)
		{
			IntPtr intPtr = AndroidJNISafe.FindClass(clazz);
			IntPtr staticMethodID;
			try
			{
				staticMethodID = AndroidJNISafe.GetStaticMethodID(intPtr, methodName, signature);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
			return staticMethodID;
		}

		public static IntPtr GetConstructorMember(IntPtr jclass, string signature)
		{
			jvalue[] array = new jvalue[2];
			IntPtr result;
			try
			{
				array[0].l = jclass;
				array[1].l = AndroidJNISafe.NewStringUTF(signature);
				result = AndroidJNISafe.CallStaticObjectMethod(AndroidReflection.s_ReflectionHelperClass, AndroidReflection.s_ReflectionHelperGetConstructorID, array);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(array[1].l);
			}
			return result;
		}

		public static IntPtr GetMethodMember(IntPtr jclass, string methodName, string signature, bool isStatic)
		{
			jvalue[] array = new jvalue[4];
			IntPtr result;
			try
			{
				array[0].l = jclass;
				array[1].l = AndroidJNISafe.NewStringUTF(methodName);
				array[2].l = AndroidJNISafe.NewStringUTF(signature);
				array[3].z = isStatic;
				result = AndroidJNISafe.CallStaticObjectMethod(AndroidReflection.s_ReflectionHelperClass, AndroidReflection.s_ReflectionHelperGetMethodID, array);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(array[1].l);
				AndroidJNISafe.DeleteLocalRef(array[2].l);
			}
			return result;
		}

		public static IntPtr GetFieldMember(IntPtr jclass, string fieldName, string signature, bool isStatic)
		{
			jvalue[] array = new jvalue[4];
			IntPtr result;
			try
			{
				array[0].l = jclass;
				array[1].l = AndroidJNISafe.NewStringUTF(fieldName);
				array[2].l = AndroidJNISafe.NewStringUTF(signature);
				array[3].z = isStatic;
				result = AndroidJNISafe.CallStaticObjectMethod(AndroidReflection.s_ReflectionHelperClass, AndroidReflection.s_ReflectionHelperGetFieldID, array);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(array[1].l);
				AndroidJNISafe.DeleteLocalRef(array[2].l);
			}
			return result;
		}

		public static IntPtr NewProxyInstance(int delegateHandle, IntPtr interfaze)
		{
			jvalue[] array = new jvalue[2];
			array[0].i = delegateHandle;
			array[1].l = interfaze;
			return AndroidJNISafe.CallStaticObjectMethod(AndroidReflection.s_ReflectionHelperClass, AndroidReflection.s_ReflectionHelperNewProxyInstance, array);
		}
	}
}
