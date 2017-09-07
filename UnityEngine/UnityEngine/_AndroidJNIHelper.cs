using System;
using System.Text;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	internal sealed class _AndroidJNIHelper
	{
		public static IntPtr CreateJavaProxy(int delegateHandle, AndroidJavaProxy proxy)
		{
			return AndroidReflection.NewProxyInstance(delegateHandle, proxy.javaInterface.GetRawClass());
		}

		public static IntPtr CreateJavaRunnable(AndroidJavaRunnable jrunnable)
		{
			return AndroidJNIHelper.CreateJavaProxy(new AndroidJavaRunnableProxy(jrunnable));
		}

		public static IntPtr InvokeJavaProxyMethod(AndroidJavaProxy proxy, IntPtr jmethodName, IntPtr jargs)
		{
			int num = 0;
			if (jargs != IntPtr.Zero)
			{
				num = AndroidJNISafe.GetArrayLength(jargs);
			}
			AndroidJavaObject[] array = new AndroidJavaObject[num];
			for (int i = 0; i < num; i++)
			{
				IntPtr objectArrayElement = AndroidJNISafe.GetObjectArrayElement(jargs, i);
				array[i] = ((!(objectArrayElement != IntPtr.Zero)) ? null : new AndroidJavaObject(objectArrayElement));
			}
			IntPtr result;
			using (AndroidJavaObject androidJavaObject = proxy.Invoke(AndroidJNI.GetStringUTFChars(jmethodName), array))
			{
				if (androidJavaObject == null)
				{
					result = IntPtr.Zero;
				}
				else
				{
					result = AndroidJNI.NewLocalRef(androidJavaObject.GetRawObject());
				}
			}
			return result;
		}

		public static jvalue[] CreateJNIArgArray(object[] args)
		{
			jvalue[] array = new jvalue[args.GetLength(0)];
			int num = 0;
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				if (obj == null)
				{
					array[num].l = IntPtr.Zero;
				}
				else if (AndroidReflection.IsPrimitive(obj.GetType()))
				{
					if (obj is int)
					{
						array[num].i = (int)obj;
					}
					else if (obj is bool)
					{
						array[num].z = (bool)obj;
					}
					else if (obj is byte)
					{
						array[num].b = (byte)obj;
					}
					else if (obj is short)
					{
						array[num].s = (short)obj;
					}
					else if (obj is long)
					{
						array[num].j = (long)obj;
					}
					else if (obj is float)
					{
						array[num].f = (float)obj;
					}
					else if (obj is double)
					{
						array[num].d = (double)obj;
					}
					else if (obj is char)
					{
						array[num].c = (char)obj;
					}
				}
				else if (obj is string)
				{
					array[num].l = AndroidJNISafe.NewStringUTF((string)obj);
				}
				else if (obj is AndroidJavaClass)
				{
					array[num].l = ((AndroidJavaClass)obj).GetRawClass();
				}
				else if (obj is AndroidJavaObject)
				{
					array[num].l = ((AndroidJavaObject)obj).GetRawObject();
				}
				else if (obj is Array)
				{
					array[num].l = _AndroidJNIHelper.ConvertToJNIArray((Array)obj);
				}
				else if (obj is AndroidJavaProxy)
				{
					array[num].l = ((AndroidJavaProxy)obj).GetProxy().GetRawObject();
				}
				else
				{
					if (!(obj is AndroidJavaRunnable))
					{
						throw new Exception("JNI; Unknown argument type '" + obj.GetType() + "'");
					}
					array[num].l = AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable)obj);
				}
				num++;
			}
			return array;
		}

		public static object UnboxArray(AndroidJavaObject obj)
		{
			object result;
			if (obj == null)
			{
				result = null;
			}
			else
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("java/lang/reflect/Array");
				AndroidJavaObject androidJavaObject = obj.Call<AndroidJavaObject>("getClass", new object[0]);
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getComponentType", new object[0]);
				string text = androidJavaObject2.Call<string>("getName", new object[0]);
				int num = androidJavaClass.Call<int>("getLength", new object[]
				{
					obj
				});
				Array array;
				if (androidJavaObject2.Call<bool>("IsPrimitive", new object[0]))
				{
					if ("I" == text)
					{
						array = new int[num];
					}
					else if ("Z" == text)
					{
						array = new bool[num];
					}
					else if ("B" == text)
					{
						array = new byte[num];
					}
					else if ("S" == text)
					{
						array = new short[num];
					}
					else if ("J" == text)
					{
						array = new long[num];
					}
					else if ("F" == text)
					{
						array = new float[num];
					}
					else if ("D" == text)
					{
						array = new double[num];
					}
					else
					{
						if (!("C" == text))
						{
							throw new Exception("JNI; Unknown argument type '" + text + "'");
						}
						array = new char[num];
					}
				}
				else if ("java.lang.String" == text)
				{
					array = new string[num];
				}
				else if ("java.lang.Class" == text)
				{
					array = new AndroidJavaClass[num];
				}
				else
				{
					array = new AndroidJavaObject[num];
				}
				for (int i = 0; i < num; i++)
				{
					array.SetValue(_AndroidJNIHelper.Unbox(androidJavaClass.CallStatic<AndroidJavaObject>("get", new object[]
					{
						obj,
						i
					})), i);
				}
				result = array;
			}
			return result;
		}

		public static object Unbox(AndroidJavaObject obj)
		{
			object result;
			if (obj == null)
			{
				result = null;
			}
			else
			{
				AndroidJavaObject androidJavaObject = obj.Call<AndroidJavaObject>("getClass", new object[0]);
				string b = androidJavaObject.Call<string>("getName", new object[0]);
				if ("java.lang.Integer" == b)
				{
					result = obj.Call<int>("intValue", new object[0]);
				}
				else if ("java.lang.Boolean" == b)
				{
					result = obj.Call<bool>("booleanValue", new object[0]);
				}
				else if ("java.lang.Byte" == b)
				{
					result = obj.Call<byte>("byteValue", new object[0]);
				}
				else if ("java.lang.Short" == b)
				{
					result = obj.Call<short>("shortValue", new object[0]);
				}
				else if ("java.lang.Long" == b)
				{
					result = obj.Call<long>("longValue", new object[0]);
				}
				else if ("java.lang.Float" == b)
				{
					result = obj.Call<float>("floatValue", new object[0]);
				}
				else if ("java.lang.Double" == b)
				{
					result = obj.Call<double>("doubleValue", new object[0]);
				}
				else if ("java.lang.Character" == b)
				{
					result = obj.Call<char>("charValue", new object[0]);
				}
				else if ("java.lang.String" == b)
				{
					result = obj.Call<string>("toString", new object[0]);
				}
				else if ("java.lang.Class" == b)
				{
					result = new AndroidJavaClass(obj.GetRawObject());
				}
				else if (androidJavaObject.Call<bool>("isArray", new object[0]))
				{
					result = _AndroidJNIHelper.UnboxArray(obj);
				}
				else
				{
					result = obj;
				}
			}
			return result;
		}

		public static AndroidJavaObject Box(object obj)
		{
			AndroidJavaObject result;
			if (obj == null)
			{
				result = null;
			}
			else if (AndroidReflection.IsPrimitive(obj.GetType()))
			{
				if (obj is int)
				{
					result = new AndroidJavaObject("java.lang.Integer", new object[]
					{
						(int)obj
					});
				}
				else if (obj is bool)
				{
					result = new AndroidJavaObject("java.lang.Boolean", new object[]
					{
						(bool)obj
					});
				}
				else if (obj is byte)
				{
					result = new AndroidJavaObject("java.lang.Byte", new object[]
					{
						(byte)obj
					});
				}
				else if (obj is short)
				{
					result = new AndroidJavaObject("java.lang.Short", new object[]
					{
						(short)obj
					});
				}
				else if (obj is long)
				{
					result = new AndroidJavaObject("java.lang.Long", new object[]
					{
						(long)obj
					});
				}
				else if (obj is float)
				{
					result = new AndroidJavaObject("java.lang.Float", new object[]
					{
						(float)obj
					});
				}
				else if (obj is double)
				{
					result = new AndroidJavaObject("java.lang.Double", new object[]
					{
						(double)obj
					});
				}
				else
				{
					if (!(obj is char))
					{
						throw new Exception("JNI; Unknown argument type '" + obj.GetType() + "'");
					}
					result = new AndroidJavaObject("java.lang.Character", new object[]
					{
						(char)obj
					});
				}
			}
			else if (obj is string)
			{
				result = new AndroidJavaObject("java.lang.String", new object[]
				{
					(string)obj
				});
			}
			else if (obj is AndroidJavaClass)
			{
				result = new AndroidJavaObject(((AndroidJavaClass)obj).GetRawClass());
			}
			else if (obj is AndroidJavaObject)
			{
				result = (AndroidJavaObject)obj;
			}
			else if (obj is Array)
			{
				result = AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(_AndroidJNIHelper.ConvertToJNIArray((Array)obj));
			}
			else if (obj is AndroidJavaProxy)
			{
				result = ((AndroidJavaProxy)obj).GetProxy();
			}
			else
			{
				if (!(obj is AndroidJavaRunnable))
				{
					throw new Exception("JNI; Unknown argument type '" + obj.GetType() + "'");
				}
				result = AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable)obj));
			}
			return result;
		}

		public static void DeleteJNIArgArray(object[] args, jvalue[] jniArgs)
		{
			int num = 0;
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				if (obj is string || obj is AndroidJavaRunnable || obj is Array)
				{
					AndroidJNISafe.DeleteLocalRef(jniArgs[num].l);
				}
				num++;
			}
		}

		public static IntPtr ConvertToJNIArray(Array array)
		{
			Type elementType = array.GetType().GetElementType();
			IntPtr result;
			if (AndroidReflection.IsPrimitive(elementType))
			{
				if (elementType == typeof(int))
				{
					result = AndroidJNISafe.ToIntArray((int[])array);
				}
				else if (elementType == typeof(bool))
				{
					result = AndroidJNISafe.ToBooleanArray((bool[])array);
				}
				else if (elementType == typeof(byte))
				{
					result = AndroidJNISafe.ToByteArray((byte[])array);
				}
				else if (elementType == typeof(short))
				{
					result = AndroidJNISafe.ToShortArray((short[])array);
				}
				else if (elementType == typeof(long))
				{
					result = AndroidJNISafe.ToLongArray((long[])array);
				}
				else if (elementType == typeof(float))
				{
					result = AndroidJNISafe.ToFloatArray((float[])array);
				}
				else if (elementType == typeof(double))
				{
					result = AndroidJNISafe.ToDoubleArray((double[])array);
				}
				else if (elementType == typeof(char))
				{
					result = AndroidJNISafe.ToCharArray((char[])array);
				}
				else
				{
					result = IntPtr.Zero;
				}
			}
			else if (elementType == typeof(string))
			{
				string[] array2 = (string[])array;
				int length = array.GetLength(0);
				IntPtr intPtr = AndroidJNISafe.FindClass("java/lang/String");
				IntPtr intPtr2 = AndroidJNI.NewObjectArray(length, intPtr, IntPtr.Zero);
				for (int i = 0; i < length; i++)
				{
					IntPtr intPtr3 = AndroidJNISafe.NewStringUTF(array2[i]);
					AndroidJNI.SetObjectArrayElement(intPtr2, i, intPtr3);
					AndroidJNISafe.DeleteLocalRef(intPtr3);
				}
				AndroidJNISafe.DeleteLocalRef(intPtr);
				result = intPtr2;
			}
			else
			{
				if (elementType != typeof(AndroidJavaObject))
				{
					throw new Exception("JNI; Unknown array type '" + elementType + "'");
				}
				AndroidJavaObject[] array3 = (AndroidJavaObject[])array;
				int length2 = array.GetLength(0);
				IntPtr[] array4 = new IntPtr[length2];
				IntPtr intPtr4 = AndroidJNISafe.FindClass("java/lang/Object");
				IntPtr intPtr5 = IntPtr.Zero;
				for (int j = 0; j < length2; j++)
				{
					if (array3[j] != null)
					{
						array4[j] = array3[j].GetRawObject();
						IntPtr rawClass = array3[j].GetRawClass();
						if (intPtr5 != rawClass)
						{
							if (intPtr5 == IntPtr.Zero)
							{
								intPtr5 = rawClass;
							}
							else
							{
								intPtr5 = intPtr4;
							}
						}
					}
					else
					{
						array4[j] = IntPtr.Zero;
					}
				}
				IntPtr intPtr6 = AndroidJNISafe.ToObjectArray(array4, intPtr5);
				AndroidJNISafe.DeleteLocalRef(intPtr4);
				result = intPtr6;
			}
			return result;
		}

		public static ArrayType ConvertFromJNIArray<ArrayType>(IntPtr array)
		{
			Type elementType = typeof(ArrayType).GetElementType();
			ArrayType result;
			if (AndroidReflection.IsPrimitive(elementType))
			{
				if (elementType == typeof(int))
				{
					result = (ArrayType)((object)AndroidJNISafe.FromIntArray(array));
				}
				else if (elementType == typeof(bool))
				{
					result = (ArrayType)((object)AndroidJNISafe.FromBooleanArray(array));
				}
				else if (elementType == typeof(byte))
				{
					result = (ArrayType)((object)AndroidJNISafe.FromByteArray(array));
				}
				else if (elementType == typeof(short))
				{
					result = (ArrayType)((object)AndroidJNISafe.FromShortArray(array));
				}
				else if (elementType == typeof(long))
				{
					result = (ArrayType)((object)AndroidJNISafe.FromLongArray(array));
				}
				else if (elementType == typeof(float))
				{
					result = (ArrayType)((object)AndroidJNISafe.FromFloatArray(array));
				}
				else if (elementType == typeof(double))
				{
					result = (ArrayType)((object)AndroidJNISafe.FromDoubleArray(array));
				}
				else if (elementType == typeof(char))
				{
					result = (ArrayType)((object)AndroidJNISafe.FromCharArray(array));
				}
				else
				{
					result = default(ArrayType);
				}
			}
			else if (elementType == typeof(string))
			{
				int arrayLength = AndroidJNISafe.GetArrayLength(array);
				string[] array2 = new string[arrayLength];
				for (int i = 0; i < arrayLength; i++)
				{
					IntPtr objectArrayElement = AndroidJNI.GetObjectArrayElement(array, i);
					array2[i] = AndroidJNISafe.GetStringUTFChars(objectArrayElement);
					AndroidJNISafe.DeleteLocalRef(objectArrayElement);
				}
				result = (ArrayType)((object)array2);
			}
			else
			{
				if (elementType != typeof(AndroidJavaObject))
				{
					throw new Exception("JNI: Unknown generic array type '" + elementType + "'");
				}
				int arrayLength2 = AndroidJNISafe.GetArrayLength(array);
				AndroidJavaObject[] array3 = new AndroidJavaObject[arrayLength2];
				for (int j = 0; j < arrayLength2; j++)
				{
					IntPtr objectArrayElement2 = AndroidJNI.GetObjectArrayElement(array, j);
					array3[j] = new AndroidJavaObject(objectArrayElement2);
					AndroidJNISafe.DeleteLocalRef(objectArrayElement2);
				}
				result = (ArrayType)((object)array3);
			}
			return result;
		}

		public static IntPtr GetConstructorID(IntPtr jclass, object[] args)
		{
			return AndroidJNIHelper.GetConstructorID(jclass, _AndroidJNIHelper.GetSignature(args));
		}

		public static IntPtr GetMethodID(IntPtr jclass, string methodName, object[] args, bool isStatic)
		{
			return AndroidJNIHelper.GetMethodID(jclass, methodName, _AndroidJNIHelper.GetSignature(args), isStatic);
		}

		public static IntPtr GetMethodID<ReturnType>(IntPtr jclass, string methodName, object[] args, bool isStatic)
		{
			return AndroidJNIHelper.GetMethodID(jclass, methodName, _AndroidJNIHelper.GetSignature<ReturnType>(args), isStatic);
		}

		public static IntPtr GetFieldID<ReturnType>(IntPtr jclass, string fieldName, bool isStatic)
		{
			return AndroidJNIHelper.GetFieldID(jclass, fieldName, _AndroidJNIHelper.GetSignature(typeof(ReturnType)), isStatic);
		}

		public static IntPtr GetConstructorID(IntPtr jclass, string signature)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr result;
			try
			{
				intPtr = AndroidReflection.GetConstructorMember(jclass, signature);
				result = AndroidJNISafe.FromReflectedMethod(intPtr);
			}
			catch (Exception ex)
			{
				IntPtr methodID = AndroidJNISafe.GetMethodID(jclass, "<init>", signature);
				if (!(methodID != IntPtr.Zero))
				{
					throw ex;
				}
				result = methodID;
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
			return result;
		}

		public static IntPtr GetMethodID(IntPtr jclass, string methodName, string signature, bool isStatic)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr result;
			try
			{
				intPtr = AndroidReflection.GetMethodMember(jclass, methodName, signature, isStatic);
				result = AndroidJNISafe.FromReflectedMethod(intPtr);
			}
			catch (Exception ex)
			{
				IntPtr methodIDFallback = _AndroidJNIHelper.GetMethodIDFallback(jclass, methodName, signature, isStatic);
				if (!(methodIDFallback != IntPtr.Zero))
				{
					throw ex;
				}
				result = methodIDFallback;
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
			return result;
		}

		private static IntPtr GetMethodIDFallback(IntPtr jclass, string methodName, string signature, bool isStatic)
		{
			IntPtr result;
			try
			{
				result = ((!isStatic) ? AndroidJNISafe.GetMethodID(jclass, methodName, signature) : AndroidJNISafe.GetStaticMethodID(jclass, methodName, signature));
				return result;
			}
			catch (Exception)
			{
			}
			result = IntPtr.Zero;
			return result;
		}

		public static IntPtr GetFieldID(IntPtr jclass, string fieldName, string signature, bool isStatic)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr result;
			try
			{
				intPtr = AndroidReflection.GetFieldMember(jclass, fieldName, signature, isStatic);
				result = AndroidJNISafe.FromReflectedField(intPtr);
			}
			catch (Exception ex)
			{
				IntPtr intPtr2 = (!isStatic) ? AndroidJNISafe.GetFieldID(jclass, fieldName, signature) : AndroidJNISafe.GetStaticFieldID(jclass, fieldName, signature);
				if (!(intPtr2 != IntPtr.Zero))
				{
					throw ex;
				}
				result = intPtr2;
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
			return result;
		}

		public static string GetSignature(object obj)
		{
			string result;
			if (obj == null)
			{
				result = "Ljava/lang/Object;";
			}
			else
			{
				Type type = (!(obj is Type)) ? obj.GetType() : ((Type)obj);
				if (AndroidReflection.IsPrimitive(type))
				{
					if (type.Equals(typeof(int)))
					{
						result = "I";
					}
					else if (type.Equals(typeof(bool)))
					{
						result = "Z";
					}
					else if (type.Equals(typeof(byte)))
					{
						result = "B";
					}
					else if (type.Equals(typeof(short)))
					{
						result = "S";
					}
					else if (type.Equals(typeof(long)))
					{
						result = "J";
					}
					else if (type.Equals(typeof(float)))
					{
						result = "F";
					}
					else if (type.Equals(typeof(double)))
					{
						result = "D";
					}
					else if (type.Equals(typeof(char)))
					{
						result = "C";
					}
					else
					{
						result = "";
					}
				}
				else if (type.Equals(typeof(string)))
				{
					result = "Ljava/lang/String;";
				}
				else if (obj is AndroidJavaProxy)
				{
					AndroidJavaObject androidJavaObject = new AndroidJavaObject(((AndroidJavaProxy)obj).javaInterface.GetRawClass());
					result = "L" + androidJavaObject.Call<string>("getName", new object[0]) + ";";
				}
				else if (type.Equals(typeof(AndroidJavaRunnable)))
				{
					result = "Ljava/lang/Runnable;";
				}
				else if (type.Equals(typeof(AndroidJavaClass)))
				{
					result = "Ljava/lang/Class;";
				}
				else
				{
					if (type.Equals(typeof(AndroidJavaObject)))
					{
						if (obj == type)
						{
							result = "Ljava/lang/Object;";
							return result;
						}
						AndroidJavaObject androidJavaObject2 = (AndroidJavaObject)obj;
						using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getClass", new object[0]))
						{
							result = "L" + androidJavaObject3.Call<string>("getName", new object[0]) + ";";
							return result;
						}
					}
					if (!AndroidReflection.IsAssignableFrom(typeof(Array), type))
					{
						throw new Exception(string.Concat(new object[]
						{
							"JNI: Unknown signature for type '",
							type,
							"' (obj = ",
							obj,
							") ",
							(type != obj) ? "instance" : "equal"
						}));
					}
					if (type.GetArrayRank() != 1)
					{
						throw new Exception("JNI: System.Array in n dimensions is not allowed");
					}
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append('[');
					stringBuilder.Append(_AndroidJNIHelper.GetSignature(type.GetElementType()));
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		public static string GetSignature(object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				stringBuilder.Append(_AndroidJNIHelper.GetSignature(obj));
			}
			stringBuilder.Append(")V");
			return stringBuilder.ToString();
		}

		public static string GetSignature<ReturnType>(object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				stringBuilder.Append(_AndroidJNIHelper.GetSignature(obj));
			}
			stringBuilder.Append(')');
			stringBuilder.Append(_AndroidJNIHelper.GetSignature(typeof(ReturnType)));
			return stringBuilder.ToString();
		}
	}
}
