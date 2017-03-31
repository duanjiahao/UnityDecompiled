using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor.Collaboration;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class JSProxyMgr
	{
		protected delegate void TaskCallback();

		public delegate void ExecCallback(object result);

		public const double kProtocolVersion = 1.0;

		public const long kInvalidMessageID = -1L;

		public const int kErrNone = 0;

		public const int kErrInvalidMessageFormat = -1000;

		public const int kErrUnknownObject = -1001;

		public const int kErrUnknownMethod = -1002;

		public const int kErrInvocationFailed = -1003;

		public const int kErrUnsupportedProtocol = -1004;

		public const int kErrUnknownEvent = -1005;

		public const string kTypeInvoke = "INVOKE";

		public const string kTypeGetStubInfo = "GETSTUBINFO";

		public const string kTypeOnEvent = "ONEVENT";

		private Queue<JSProxyMgr.TaskCallback> m_TaskList = null;

		private Dictionary<string, object> m_GlobalObjects = null;

		private static JSProxyMgr s_Instance;

		private static readonly string[] s_IgnoredMethods;

		static JSProxyMgr()
		{
			JSProxyMgr.s_Instance = null;
			JSProxyMgr.s_IgnoredMethods = new string[]
			{
				"Equals",
				"GetHashCode",
				"GetType",
				"ToString"
			};
			WebView.OnDomainReload();
		}

		protected JSProxyMgr()
		{
			this.m_TaskList = new Queue<JSProxyMgr.TaskCallback>();
			this.m_GlobalObjects = new Dictionary<string, object>();
			this.AddGlobalObject("unity/collab", Collab.instance);
		}

		public static JSProxyMgr GetInstance()
		{
			if (JSProxyMgr.s_Instance == null)
			{
				JSProxyMgr.s_Instance = new JSProxyMgr();
			}
			return JSProxyMgr.s_Instance;
		}

		public static void DoTasks()
		{
			JSProxyMgr.GetInstance().ProcessTasks();
		}

		~JSProxyMgr()
		{
			this.m_GlobalObjects.Clear();
			this.m_GlobalObjects = null;
		}

		public void AddGlobalObject(string referenceName, object obj)
		{
			if (this.m_GlobalObjects == null)
			{
				this.m_GlobalObjects = new Dictionary<string, object>();
			}
			this.RemoveGlobalObject(referenceName);
			this.m_GlobalObjects.Add(referenceName, obj);
		}

		public void RemoveGlobalObject(string referenceName)
		{
			if (this.m_GlobalObjects != null)
			{
				if (this.m_GlobalObjects.ContainsKey(referenceName))
				{
					this.m_GlobalObjects.Remove(referenceName);
				}
			}
		}

		private void AddTask(JSProxyMgr.TaskCallback task)
		{
			if (this.m_TaskList == null)
			{
				this.m_TaskList = new Queue<JSProxyMgr.TaskCallback>();
			}
			this.m_TaskList.Enqueue(task);
		}

		private void ProcessTasks()
		{
			if (this.m_TaskList != null && this.m_TaskList.Count != 0)
			{
				int num = 10;
				while (this.m_TaskList.Count > 0 && num > 0)
				{
					JSProxyMgr.TaskCallback taskCallback = this.m_TaskList.Dequeue();
					taskCallback();
					num--;
				}
			}
		}

		public bool DoMessage(string jsonRequest, JSProxyMgr.ExecCallback callback, WebView webView)
		{
			long messageID = -1L;
			bool result;
			try
			{
				Dictionary<string, object> dictionary = Json.Deserialize(jsonRequest) as Dictionary<string, object>;
				if (dictionary == null || !dictionary.ContainsKey("messageID") || !dictionary.ContainsKey("version") || !dictionary.ContainsKey("type"))
				{
					callback(JSProxyMgr.FormatError(messageID, -1000, "errInvalidMessageFormat", jsonRequest));
					result = false;
					return result;
				}
				messageID = (long)dictionary["messageID"];
				double num = double.Parse((string)dictionary["version"], CultureInfo.InvariantCulture);
				string a = (string)dictionary["type"];
				if (num > 1.0)
				{
					callback(JSProxyMgr.FormatError(messageID, -1004, "errUnsupportedProtocol", "The protocol version <" + num + "> is not supported by this verison of the code"));
					result = false;
					return result;
				}
				if (a == "INVOKE")
				{
					result = this.DoInvokeMessage(messageID, callback, dictionary);
					return result;
				}
				if (a == "GETSTUBINFO")
				{
					result = this.DoGetStubInfoMessage(messageID, callback, dictionary);
					return result;
				}
				if (a == "ONEVENT")
				{
					result = this.DoOnEventMessage(messageID, callback, dictionary, webView);
					return result;
				}
			}
			catch (Exception ex)
			{
				callback(JSProxyMgr.FormatError(messageID, -1000, "errInvalidMessageFormat", ex.Message));
			}
			result = false;
			return result;
		}

		private bool DoGetStubInfoMessage(long messageID, JSProxyMgr.ExecCallback callback, Dictionary<string, object> jsonData)
		{
			bool result;
			if (!jsonData.ContainsKey("reference"))
			{
				callback(JSProxyMgr.FormatError(messageID, -1001, "errUnknownObject", "object reference missing"));
				result = false;
			}
			else
			{
				string text = (string)jsonData["reference"];
				object destinationObject = this.GetDestinationObject(text);
				if (destinationObject == null)
				{
					callback(JSProxyMgr.FormatError(messageID, -1001, "errUnknownObject", "cannot find object with reference <" + text + ">"));
					result = false;
				}
				else
				{
					List<MethodInfo> list = destinationObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).ToList<MethodInfo>();
					list.AddRange(destinationObject.GetType().GetMethods(BindingFlags.Static | BindingFlags.Public).ToList<MethodInfo>());
					ArrayList arrayList = new ArrayList();
					foreach (MethodInfo current in list)
					{
						if (Array.IndexOf<string>(JSProxyMgr.s_IgnoredMethods, current.Name) < 0)
						{
							if (!current.IsSpecialName || (!current.Name.StartsWith("set_") && !current.Name.StartsWith("get_")))
							{
								ParameterInfo[] parameters = current.GetParameters();
								ArrayList arrayList2 = new ArrayList();
								ParameterInfo[] array = parameters;
								for (int i = 0; i < array.Length; i++)
								{
									ParameterInfo parameterInfo = array[i];
									arrayList2.Add(parameterInfo.Name);
								}
								JspmMethodInfo value = new JspmMethodInfo(current.Name, (string[])arrayList2.ToArray(typeof(string)));
								arrayList.Add(value);
							}
						}
					}
					List<PropertyInfo> list2 = destinationObject.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).ToList<PropertyInfo>();
					ArrayList arrayList3 = new ArrayList();
					foreach (PropertyInfo current2 in list2)
					{
						arrayList3.Add(new JspmPropertyInfo(current2.Name, current2.GetValue(destinationObject, null)));
					}
					List<FieldInfo> list3 = destinationObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToList<FieldInfo>();
					foreach (FieldInfo current3 in list3)
					{
						arrayList3.Add(new JspmPropertyInfo(current3.Name, current3.GetValue(destinationObject)));
					}
					List<EventInfo> list4 = destinationObject.GetType().GetEvents(BindingFlags.Instance | BindingFlags.Public).ToList<EventInfo>();
					ArrayList arrayList4 = new ArrayList();
					foreach (EventInfo current4 in list4)
					{
						arrayList4.Add(current4.Name);
					}
					callback(new JspmStubInfoSuccess(messageID, text, (JspmPropertyInfo[])arrayList3.ToArray(typeof(JspmPropertyInfo)), (JspmMethodInfo[])arrayList.ToArray(typeof(JspmMethodInfo)), (string[])arrayList4.ToArray(typeof(string))));
					result = true;
				}
			}
			return result;
		}

		private bool DoOnEventMessage(long messageID, JSProxyMgr.ExecCallback callback, Dictionary<string, object> jsonData, WebView webView)
		{
			callback(JSProxyMgr.FormatError(messageID, -1002, "errUnknownMethod", "method DoOnEventMessage is deprecated"));
			return false;
		}

		private bool DoInvokeMessage(long messageID, JSProxyMgr.ExecCallback callback, Dictionary<string, object> jsonData)
		{
			bool result;
			if (!jsonData.ContainsKey("destination") || !jsonData.ContainsKey("method") || !jsonData.ContainsKey("params"))
			{
				callback(JSProxyMgr.FormatError(messageID, -1001, "errUnknownObject", "object reference, method name or parameters missing"));
				result = false;
			}
			else
			{
				string text = (string)jsonData["destination"];
				string text2 = (string)jsonData["method"];
				List<object> data = (List<object>)jsonData["params"];
				object destObject = this.GetDestinationObject(text);
				if (destObject == null)
				{
					callback(JSProxyMgr.FormatError(messageID, -1001, "errUnknownObject", "cannot find object with reference <" + text + ">"));
					result = false;
				}
				else
				{
					Type type = destObject.GetType();
					MethodInfo[] methods = type.GetMethods();
					MethodInfo foundMethod = null;
					object[] parameters = null;
					string text3 = "";
					MethodInfo[] array = methods;
					for (int i = 0; i < array.Length; i++)
					{
						MethodInfo methodInfo = array[i];
						if (!(methodInfo.Name != text2))
						{
							try
							{
								parameters = this.ParseParams(methodInfo, data);
								foundMethod = methodInfo;
								break;
							}
							catch (Exception ex)
							{
								text3 = ex.Message;
							}
						}
					}
					if (foundMethod == null)
					{
						callback(JSProxyMgr.FormatError(messageID, -1002, "errUnknownMethod", string.Concat(new string[]
						{
							"cannot find method <",
							text2,
							"> for object <",
							text,
							">, reason:",
							text3
						})));
						result = false;
					}
					else
					{
						this.AddTask(delegate
						{
							try
							{
								object result2 = foundMethod.Invoke(destObject, parameters);
								callback(JSProxyMgr.FormatSuccess(messageID, result2));
							}
							catch (TargetInvocationException ex2)
							{
								if (ex2.InnerException != null)
								{
									callback(JSProxyMgr.FormatError(messageID, -1003, ex2.InnerException.GetType().Name, ex2.InnerException.Message));
								}
								else
								{
									callback(JSProxyMgr.FormatError(messageID, -1003, ex2.GetType().Name, ex2.Message));
								}
							}
							catch (Exception ex3)
							{
								callback(JSProxyMgr.FormatError(messageID, -1003, ex3.GetType().Name, ex3.Message));
							}
						});
						result = true;
					}
				}
			}
			return result;
		}

		public static JspmError FormatError(long messageID, int status, string errorClass, string message)
		{
			return new JspmError(messageID, status, errorClass, message);
		}

		public static JspmSuccess FormatSuccess(long messageID, object result)
		{
			return new JspmSuccess(messageID, result, "INVOKE");
		}

		public object GetDestinationObject(string reference)
		{
			object result;
			this.m_GlobalObjects.TryGetValue(reference, out result);
			return result;
		}

		public object[] ParseParams(MethodInfo method, List<object> data)
		{
			ParameterInfo[] parameters = method.GetParameters();
			object[] result;
			if (parameters.Length != data.Count)
			{
				result = null;
			}
			else
			{
				List<object> list = new List<object>(data.Count);
				for (int i = 0; i < data.Count; i++)
				{
					object item = this.InternalParseParam(parameters[i].ParameterType, data[i]);
					list.Add(item);
				}
				result = list.ToArray();
			}
			return result;
		}

		private object InternalParseParam(Type type, object data)
		{
			object result;
			IList list;
			IDictionary dictionary;
			string text;
			if (data == null)
			{
				result = null;
			}
			else if ((list = (data as IList)) != null)
			{
				if (!type.IsArray)
				{
					throw new InvalidOperationException("Not an array " + type.FullName);
				}
				Type elementType = type.GetElementType();
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < list.Count; i++)
				{
					object value = this.InternalParseParam(elementType, list[i]);
					arrayList.Add(value);
				}
				result = arrayList.ToArray(elementType);
			}
			else if ((dictionary = (data as IDictionary)) != null)
			{
				if (!type.IsClass)
				{
					throw new InvalidOperationException("Not a class " + type.FullName);
				}
				ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, new Type[0], null);
				if (constructor == null)
				{
					throw new InvalidOperationException("Cannot find a default constructor for " + type.FullName);
				}
				object obj = constructor.Invoke(new object[0]);
				List<FieldInfo> list2 = type.GetFields(BindingFlags.Instance | BindingFlags.Public).ToList<FieldInfo>();
				foreach (FieldInfo current in list2)
				{
					try
					{
						object value2 = this.InternalParseParam(current.FieldType, dictionary[current.Name]);
						current.SetValue(obj, value2);
					}
					catch (KeyNotFoundException)
					{
					}
				}
				List<PropertyInfo> list3 = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList<PropertyInfo>();
				foreach (PropertyInfo current2 in list3)
				{
					try
					{
						object obj2 = this.InternalParseParam(current2.PropertyType, dictionary[current2.Name]);
						MethodInfo setMethod = current2.GetSetMethod();
						if (setMethod != null)
						{
							setMethod.Invoke(obj, new object[]
							{
								obj2
							});
						}
					}
					catch (KeyNotFoundException)
					{
					}
					catch (TargetInvocationException)
					{
					}
				}
				result = Convert.ChangeType(obj, type);
			}
			else if ((text = (data as string)) != null)
			{
				result = text;
			}
			else if (data is bool)
			{
				result = (bool)data;
			}
			else if (data is double)
			{
				result = (double)data;
			}
			else
			{
				if (!(data is int) && !(data is short) && !(data is int) && !(data is long) && !(data is long))
				{
					throw new InvalidOperationException("Cannot parse " + Json.Serialize(data));
				}
				result = Convert.ChangeType(data, type);
			}
			return result;
		}

		public string Stringify(object target)
		{
			return Json.Serialize(target);
		}

		protected object GetMemberValue(MemberInfo member, object target)
		{
			MemberTypes memberType = member.MemberType;
			object value;
			if (memberType != MemberTypes.Field)
			{
				if (memberType == MemberTypes.Property)
				{
					try
					{
						value = ((PropertyInfo)member).GetValue(target, null);
						return value;
					}
					catch (TargetParameterCountException innerException)
					{
						throw new ArgumentException("MemberInfo has index parameters", "member", innerException);
					}
				}
				throw new ArgumentException("MemberInfo is not of type FieldInfo or PropertyInfo", "member");
			}
			value = ((FieldInfo)member).GetValue(target);
			return value;
		}
	}
}
