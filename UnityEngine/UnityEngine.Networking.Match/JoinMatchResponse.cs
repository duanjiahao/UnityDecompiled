using System;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	internal class JoinMatchResponse : BasicResponse
	{
		public string address
		{
			get;
			set;
		}

		public int port
		{
			get;
			set;
		}

		public int domain
		{
			get;
			set;
		}

		public NetworkID networkId
		{
			get;
			set;
		}

		public string accessTokenString
		{
			get;
			set;
		}

		public NodeID nodeId
		{
			get;
			set;
		}

		public bool usingRelay
		{
			get;
			set;
		}

		public override string ToString()
		{
			return UnityString.Format("[{0}]-address:{1},port:{2},networkId:0x{3},accessTokenString.IsEmpty:{4},nodeId:0x{5},usingRelay:{6}", new object[]
			{
				base.ToString(),
				this.address,
				this.port,
				this.networkId.ToString("X"),
				string.IsNullOrEmpty(this.accessTokenString),
				this.nodeId.ToString("X"),
				this.usingRelay
			});
		}

		public override void Parse(object obj)
		{
			base.Parse(obj);
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary != null)
			{
				this.address = base.ParseJSONString("address", obj, dictionary);
				this.port = base.ParseJSONInt32("port", obj, dictionary);
				this.networkId = (NetworkID)base.ParseJSONUInt64("networkId", obj, dictionary);
				this.accessTokenString = base.ParseJSONString("accessTokenString", obj, dictionary);
				this.nodeId = (NodeID)base.ParseJSONUInt16("nodeId", obj, dictionary);
				this.usingRelay = base.ParseJSONBool("usingRelay", obj, dictionary);
				return;
			}
			throw new FormatException("While parsing JSON response, found obj is not of type IDictionary<string,object>:" + obj.ToString());
		}
	}
}
