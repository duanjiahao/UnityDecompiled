using System;
namespace UnityEngine.Serialization
{
	internal class SerializationHelper
	{
		private static bool alreadyWarned;
		public static void LogSerializationDepthWarning(string namespaze, string name)
		{
			if (SerializationHelper.alreadyWarned)
			{
				return;
			}
			SerializationHelper.alreadyWarned = true;
			string str = (namespaze.Length <= 0) ? name : (namespaze + "." + name);
			Debug.LogWarning("Serialization of " + str + " has too many depth levels. This means you have fallen of a performance cliff. Check the Serialization section of the docs for more details about this problem and how to fix it.");
		}
	}
}
