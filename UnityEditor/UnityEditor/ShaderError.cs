using System;

namespace UnityEditor
{
	internal struct ShaderError
	{
		public string message;

		public string messageDetails;

		public string platform;

		public string file;

		public int line;

		public int warning;
	}
}
