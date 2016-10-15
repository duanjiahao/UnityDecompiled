using System;

namespace UnityEditor
{
	internal interface IPrefType
	{
		string ToUniqueString();

		void FromUniqueString(string sstr);

		void Load();
	}
}
