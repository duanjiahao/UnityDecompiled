using System;

namespace UnityEditor
{
	internal struct CompareInfo
	{
		public int left;

		public int right;

		public int convert_binary;

		public int autodetect_binary;

		public CompareInfo(int ver1, int ver2, int binary, int abinary)
		{
			this.left = ver1;
			this.right = ver2;
			this.convert_binary = binary;
			this.autodetect_binary = abinary;
		}
	}
}
