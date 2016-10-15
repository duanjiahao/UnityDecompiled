using System;

namespace UnityEngine.Networking
{
	public interface IMultipartFormSection
	{
		string sectionName
		{
			get;
		}

		byte[] sectionData
		{
			get;
		}

		string fileName
		{
			get;
		}

		string contentType
		{
			get;
		}
	}
}
