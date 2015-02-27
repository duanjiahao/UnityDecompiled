using System;
using System.Text;
namespace UnityEngine
{
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
				else
				{
					if (obj.GetType().IsPrimitive)
					{
						if (obj is int)
						{
							array[num].i = (int)obj;
						}
						else
						{
							if (obj is bool)
							{
								array[num].z = (bool)obj;
							}
							else
							{
								if (obj is byte)
								{
									array[num].b = (byte)obj;
								}
								else
								{
									if (obj is short)
									{
										array[num].s = (short)obj;
									}
									else
									{
										if (obj is long)
										{
											array[num].j = (long)obj;
										}
										else
										{
											if (obj is float)
											{
												array[num].f = (float)obj;
											}
											else
											{
												if (obj is double)
												{
													array[num].d = (double)obj;
												}
												else
												{
													if (obj is char)
													{
														array[num].c = (char)obj;
													}
												}
											}
										}
									}
								}
							}
						}
					}
					else
					{
						if (obj is string)
						{
							array[num].l = AndroidJNISafe.NewStringUTF((string)obj);
						}
						else
						{
							if (obj is AndroidJavaClass)
							{
								array[num].l = ((AndroidJavaClass)obj).GetRawClass();
							}
							else
							{
								if (obj is AndroidJavaObject)
								{
									array[num].l = ((AndroidJavaObject)obj).GetRawObject();
								}
								else
								{
									if (obj is Array)
									{
										array[num].l = _AndroidJNIHelper.ConvertToJNIArray((Array)obj);
									}
									else
									{
										if (obj is AndroidJavaProxy)
										{
											array[num].l = AndroidJNIHelper.CreateJavaProxy((AndroidJavaProxy)obj);
										}
										else
										{
											if (!(obj is AndroidJavaRunnable))
											{
												throw new Exception("JNI; Unknown argument type '" + obj.GetType() + "'");
											}
											array[num].l = AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable)obj);
										}
									}
								}
							}
						}
					}
				}
				num++;
			}
			return array;
		}
		public static object UnboxArray(AndroidJavaObject obj)
		{
			if (obj == null)
			{
				return null;
			}
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
				else
				{
					if ("Z" == text)
					{
						array = new bool[num];
					}
					else
					{
						if ("B" == text)
						{
							array = new byte[num];
						}
						else
						{
							if ("S" == text)
							{
								array = new short[num];
							}
							else
							{
								if ("L" == text)
								{
									array = new long[num];
								}
								else
								{
									if ("F" == text)
									{
										array = new float[num];
									}
									else
									{
										if ("D" == text)
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
								}
							}
						}
					}
				}
			}
			else
			{
				if ("java.lang.String" == text)
				{
					array = new string[num];
				}
				else
				{
					if ("java.lang.Class" == text)
					{
						array = new AndroidJavaClass[num];
					}
					else
					{
						array = new AndroidJavaObject[num];
					}
				}
			}
			for (int i = 0; i < num; i++)
			{
				array.SetValue(_AndroidJNIHelper.Unbox(androidJavaClass.CallStatic<AndroidJavaObject>("get", new object[]
				{
					obj,
					i
				})), i);
			}
			return array;
		}
		public static object Unbox(AndroidJavaObject obj)
		{
			if (obj == null)
			{
				return null;
			}
			AndroidJavaObject androidJavaObject = obj.Call<AndroidJavaObject>("getClass", new object[0]);
			string b = androidJavaObject.Call<string>("getName", new object[0]);
			if ("java.lang.Integer" == b)
			{
				return obj.Call<int>("intValue", new object[0]);
			}
			if ("java.lang.Boolean" == b)
			{
				return obj.Call<bool>("booleanValue", new object[0]);
			}
			if ("java.lang.Byte" == b)
			{
				return obj.Call<byte>("byteValue", new object[0]);
			}
			if ("java.lang.Short" == b)
			{
				return obj.Call<short>("shortValue", new object[0]);
			}
			if ("java.lang.Long" == b)
			{
				return obj.Call<int>("longValue", new object[0]);
			}
			if ("java.lang.Float" == b)
			{
				return obj.Call<float>("floatValue", new object[0]);
			}
			if ("java.lang.Double" == b)
			{
				return obj.Call<double>("doubleValue", new object[0]);
			}
			if ("java.lang.Character" == b)
			{
				return obj.Call<char>("charValue", new object[0]);
			}
			if ("java.lang.String" == b)
			{
				return obj.Call<string>("toString", new object[0]);
			}
			if ("java.lang.Class" == b)
			{
				return new AndroidJavaClass(obj.GetRawObject());
			}
			if (androidJavaObject.Call<bool>("isArray", new object[0]))
			{
				return _AndroidJNIHelper.UnboxArray(obj);
			}
			return obj;
		}
		public static AndroidJavaObject Box(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj.GetType().IsPrimitive)
			{
				if (obj is int)
				{
					return new AndroidJavaObject("java.lang.Integer", new object[]
					{
						(int)obj
					});
				}
				if (obj is bool)
				{
					return new AndroidJavaObject("java.lang.Boolean", new object[]
					{
						(bool)obj
					});
				}
				if (obj is byte)
				{
					return new AndroidJavaObject("java.lang.Byte", new object[]
					{
						(byte)obj
					});
				}
				if (obj is short)
				{
					return new AndroidJavaObject("java.lang.Short", new object[]
					{
						(short)obj
					});
				}
				if (obj is long)
				{
					return new AndroidJavaObject("java.lang.Long", new object[]
					{
						(long)obj
					});
				}
				if (obj is float)
				{
					return new AndroidJavaObject("java.lang.Float", new object[]
					{
						(float)obj
					});
				}
				if (obj is double)
				{
					return new AndroidJavaObject("java.lang.Double", new object[]
					{
						(double)obj
					});
				}
				if (obj is char)
				{
					return new AndroidJavaObject("java.lang.Character", new object[]
					{
						(char)obj
					});
				}
				throw new Exception("JNI; Unknown argument type '" + obj.GetType() + "'");
			}
			else
			{
				if (obj is string)
				{
					return new AndroidJavaObject("java.lang.String", new object[]
					{
						(string)obj
					});
				}
				if (obj is AndroidJavaClass)
				{
					return new AndroidJavaObject(((AndroidJavaClass)obj).GetRawClass());
				}
				if (obj is AndroidJavaObject)
				{
					return (AndroidJavaObject)obj;
				}
				if (obj is Array)
				{
					return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(_AndroidJNIHelper.ConvertToJNIArray((Array)obj));
				}
				if (obj is AndroidJavaProxy)
				{
					return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(AndroidJNIHelper.CreateJavaProxy((AndroidJavaProxy)obj));
				}
				if (obj is AndroidJavaRunnable)
				{
					return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable)obj));
				}
				throw new Exception("JNI; Unknown argument type '" + obj.GetType() + "'");
			}
		}
		public static void DeleteJNIArgArray(object[] args, jvalue[] jniArgs)
		{
			int num = 0;
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				if (obj is string || obj is AndroidJavaRunnable || obj is AndroidJavaProxy || obj is Array)
				{
					AndroidJNISafe.DeleteLocalRef(jniArgs[num].l);
				}
				num++;
			}
		}
		public static IntPtr ConvertToJNIArray(Array array)
		{
			Type elementType = array.GetType().GetElementType();
			if (elementType.IsPrimitive)
			{
				if (elementType == typeof(int))
				{
					return AndroidJNISafe.ToIntArray((int[])array);
				}
				if (elementType == typeof(bool))
				{
					return AndroidJNISafe.ToBooleanArray((bool[])array);
				}
				if (elementType == typeof(byte))
				{
					return AndroidJNISafe.ToByteArray((byte[])array);
				}
				if (elementType == typeof(short))
				{
					return AndroidJNISafe.ToShortArray((short[])array);
				}
				if (elementType == typeof(long))
				{
					return AndroidJNISafe.ToLongArray((long[])array);
				}
				if (elementType == typeof(float))
				{
					return AndroidJNISafe.ToFloatArray((float[])array);
				}
				if (elementType == typeof(double))
				{
					return AndroidJNISafe.ToDoubleArray((double[])array);
				}
				if (elementType == typeof(char))
				{
					return AndroidJNISafe.ToCharArray((char[])array);
				}
				return IntPtr.Zero;
			}
			else
			{
				if (elementType == typeof(string))
				{
					string[] array2 = (string[])array;
					int length = array.GetLength(0);
					IntPtr[] array3 = new IntPtr[length];
					for (int i = 0; i < length; i++)
					{
						array3[i] = AndroidJNISafe.NewStringUTF(array2[i]);
					}
					IntPtr intPtr = AndroidJNISafe.FindClass("java/lang/String");
					IntPtr result = AndroidJNISafe.ToObjectArray(array3, intPtr);
					AndroidJNISafe.DeleteLocalRef(intPtr);
					return result;
				}
				if (elementType == typeof(AndroidJavaObject))
				{
					AndroidJavaObject[] array4 = (AndroidJavaObject[])array;
					int length2 = array.GetLength(0);
					IntPtr[] array5 = new IntPtr[length2];
					IntPtr intPtr2 = AndroidJNISafe.FindClass("java/lang/Object");
					IntPtr intPtr3 = IntPtr.Zero;
					for (int j = 0; j < length2; j++)
					{
						if (array4[j] != null)
						{
							array5[j] = array4[j].GetRawObject();
							IntPtr rawClass = array4[j].GetRawClass();
							if (intPtr3 != rawClass)
							{
								if (intPtr3 == IntPtr.Zero)
								{
									intPtr3 = rawClass;
								}
								else
								{
									intPtr3 = intPtr2;
								}
							}
						}
						else
						{
							array5[j] = IntPtr.Zero;
						}
					}
					IntPtr result2 = AndroidJNISafe.ToObjectArray(array5, intPtr3);
					AndroidJNISafe.DeleteLocalRef(intPtr2);
					return result2;
				}
				throw new Exception("JNI; Unknown array type '" + elementType + "'");
			}
		}
		public static ArrayType ConvertFromJNIArray<ArrayType>(IntPtr array)
		{
			Type elementType = typeof(ArrayType).GetElementType();
			if (elementType.IsPrimitive)
			{
				if (elementType == typeof(int))
				{
					return (ArrayType)((object)AndroidJNISafe.FromIntArray(array));
				}
				if (elementType == typeof(bool))
				{
					return (ArrayType)((object)AndroidJNISafe.FromBooleanArray(array));
				}
				if (elementType == typeof(byte))
				{
					return (ArrayType)((object)AndroidJNISafe.FromByteArray(array));
				}
				if (elementType == typeof(short))
				{
					return (ArrayType)((object)AndroidJNISafe.FromShortArray(array));
				}
				if (elementType == typeof(long))
				{
					return (ArrayType)((object)AndroidJNISafe.FromLongArray(array));
				}
				if (elementType == typeof(float))
				{
					return (ArrayType)((object)AndroidJNISafe.FromFloatArray(array));
				}
				if (elementType == typeof(double))
				{
					return (ArrayType)((object)AndroidJNISafe.FromDoubleArray(array));
				}
				if (elementType == typeof(char))
				{
					return (ArrayType)((object)AndroidJNISafe.FromCharArray(array));
				}
				return default(ArrayType);
			}
			else
			{
				if (elementType == typeof(string))
				{
					IntPtr[] array2 = AndroidJNISafe.FromObjectArray(array);
					int length = array2.GetLength(0);
					string[] array3 = new string[length];
					for (int i = 0; i < length; i++)
					{
						array3[i] = AndroidJNISafe.GetStringUTFChars(array2[i]);
					}
					return (ArrayType)((object)array3);
				}
				if (elementType == typeof(AndroidJavaObject))
				{
					IntPtr[] array4 = AndroidJNISafe.FromObjectArray(array);
					int length2 = array4.GetLength(0);
					AndroidJavaObject[] array5 = new AndroidJavaObject[length2];
					for (int j = 0; j < length2; j++)
					{
						array5[j] = new AndroidJavaObject(array4[j]);
					}
					return (ArrayType)((object)array5);
				}
				throw new Exception("JNI: Unknown generic array type '" + elementType + "'");
			}
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
				IntPtr intPtr2 = (!isStatic) ? AndroidJNISafe.GetMethodID(jclass, methodName, signature) : AndroidJNISafe.GetStaticMethodID(jclass, methodName, signature);
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
			if (obj == null)
			{
				return "Ljava/lang/Object;";
			}
			Type type = (!(obj is Type)) ? obj.GetType() : ((Type)obj);
			if (type.IsPrimitive)
			{
				if (type.Equals(typeof(int)))
				{
					return "I";
				}
				if (type.Equals(typeof(bool)))
				{
					return "Z";
				}
				if (type.Equals(typeof(byte)))
				{
					return "B";
				}
				if (type.Equals(typeof(short)))
				{
					return "S";
				}
				if (type.Equals(typeof(long)))
				{
					return "J";
				}
				if (type.Equals(typeof(float)))
				{
					return "F";
				}
				if (type.Equals(typeof(double)))
				{
					return "D";
				}
				if (type.Equals(typeof(char)))
				{
					return "C";
				}
				return string.Empty;
			}
			else
			{
				if (type.Equals(typeof(string)))
				{
					return "Ljava/lang/String;";
				}
				if (obj is AndroidJavaProxy)
				{
					AndroidJavaObject androidJavaObject = new AndroidJavaObject(((AndroidJavaProxy)obj).javaInterface.GetRawClass());
					return "L" + androidJavaObject.Call<string>("getName", new object[0]) + ";";
				}
				if (type.Equals(typeof(AndroidJavaRunnable)))
				{
					return "Ljava/lang/Runnable;";
				}
				if (type.Equals(typeof(AndroidJavaClass)))
				{
					return "Ljava/lang/Class;";
				}
				if (type.Equals(typeof(AndroidJavaObject)))
				{
					if (obj == type)
					{
						return "Ljava/lang/Object;";
					}
					AndroidJavaObject androidJavaObject2 = (AndroidJavaObject)obj;
					using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getClass", new object[0]))
					{
						return "L" + androidJavaObject3.Call<string>("getName", new object[0]) + ";";
					}
				}
				if (!typeof(Array).IsAssignableFrom(type))
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
				return stringBuilder.ToString();
			}
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
