using System;
using System.Reflection;

namespace UnityEngineInternal
{
	public delegate MethodInfo GetMethodDelegate(Type classType, string methodName, bool searchBaseTypes, bool instanceMethod, Type[] methodParamTypes);
}
