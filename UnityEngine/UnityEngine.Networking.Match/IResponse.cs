using System;

namespace UnityEngine.Networking.Match
{
	internal interface IResponse
	{
		void SetSuccess();

		void SetFailure(string info);
	}
}
