using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	public class NetworkMatch : MonoBehaviour
	{
		public delegate void BasicResponseDelegate(bool success, string extendedInfo);

		public delegate void DataResponseDelegate<T>(bool success, string extendedInfo, T responseData);

		private delegate void InternalResponseDelegate<T, U>(T response, U userCallback);

		private Uri m_BaseUri = new Uri("https://mm.unet.unity3d.com");

		public Uri baseUri
		{
			get
			{
				return this.m_BaseUri;
			}
			set
			{
				this.m_BaseUri = value;
			}
		}

		[Obsolete("This function is not used any longer to interface with the matchmaker. Please set up your project by logging in through the editor connect dialog.", true)]
		public void SetProgramAppID(AppID programAppID)
		{
		}

		public Coroutine CreateMatch(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, string publicClientAddress, string privateClientAddress, int eloScoreForMatch, int requestDomain, NetworkMatch.DataResponseDelegate<MatchInfo> callback)
		{
			Coroutine result;
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				UnityEngine.Debug.LogError("Matchmaking is not supported on WebGL player.");
				result = null;
			}
			else
			{
				result = this.CreateMatch(new CreateMatchRequest
				{
					name = matchName,
					size = matchSize,
					advertise = matchAdvertise,
					password = matchPassword,
					publicAddress = publicClientAddress,
					privateAddress = privateClientAddress,
					eloScore = eloScoreForMatch,
					domain = requestDomain
				}, callback);
			}
			return result;
		}

		internal Coroutine CreateMatch(CreateMatchRequest req, NetworkMatch.DataResponseDelegate<MatchInfo> callback)
		{
			Coroutine result;
			if (callback == null)
			{
				UnityEngine.Debug.Log("callback supplied is null, aborting CreateMatch Request.");
				result = null;
			}
			else
			{
				Uri uri = new Uri(this.baseUri, "/json/reply/CreateMatchRequest");
				UnityEngine.Debug.Log("MatchMakingClient Create :" + uri);
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("version", Request.currentVersion);
				wWWForm.AddField("projectId", Application.cloudProjectId);
				wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
				wWWForm.AddField("accessTokenString", 0);
				wWWForm.AddField("domain", req.domain);
				wWWForm.AddField("name", req.name);
				wWWForm.AddField("size", req.size.ToString());
				wWWForm.AddField("advertise", req.advertise.ToString());
				wWWForm.AddField("password", req.password);
				wWWForm.AddField("publicAddress", req.publicAddress);
				wWWForm.AddField("privateAddress", req.privateAddress);
				wWWForm.AddField("eloScore", req.eloScore.ToString());
				wWWForm.headers["Accept"] = "application/json";
				WWW client = new WWW(uri.ToString(), wWWForm);
				result = base.StartCoroutine(this.ProcessMatchResponse<CreateMatchResponse, NetworkMatch.DataResponseDelegate<MatchInfo>>(client, new NetworkMatch.InternalResponseDelegate<CreateMatchResponse, NetworkMatch.DataResponseDelegate<MatchInfo>>(this.OnMatchCreate), callback));
			}
			return result;
		}

		internal virtual void OnMatchCreate(CreateMatchResponse response, NetworkMatch.DataResponseDelegate<MatchInfo> userCallback)
		{
			if (response.success)
			{
				Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
			}
			userCallback(response.success, response.extendedInfo, new MatchInfo(response));
		}

		public Coroutine JoinMatch(NetworkID netId, string matchPassword, string publicClientAddress, string privateClientAddress, int eloScoreForClient, int requestDomain, NetworkMatch.DataResponseDelegate<MatchInfo> callback)
		{
			return this.JoinMatch(new JoinMatchRequest
			{
				networkId = netId,
				password = matchPassword,
				publicAddress = publicClientAddress,
				privateAddress = privateClientAddress,
				eloScore = eloScoreForClient,
				domain = requestDomain
			}, callback);
		}

		internal Coroutine JoinMatch(JoinMatchRequest req, NetworkMatch.DataResponseDelegate<MatchInfo> callback)
		{
			Coroutine result;
			if (callback == null)
			{
				UnityEngine.Debug.Log("callback supplied is null, aborting JoinMatch Request.");
				result = null;
			}
			else
			{
				Uri uri = new Uri(this.baseUri, "/json/reply/JoinMatchRequest");
				UnityEngine.Debug.Log("MatchMakingClient Join :" + uri);
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("version", Request.currentVersion);
				wWWForm.AddField("projectId", Application.cloudProjectId);
				wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
				wWWForm.AddField("accessTokenString", 0);
				wWWForm.AddField("domain", req.domain);
				wWWForm.AddField("networkId", req.networkId.ToString());
				wWWForm.AddField("password", req.password);
				wWWForm.AddField("publicAddress", req.publicAddress);
				wWWForm.AddField("privateAddress", req.privateAddress);
				wWWForm.AddField("eloScore", req.eloScore.ToString());
				wWWForm.headers["Accept"] = "application/json";
				WWW client = new WWW(uri.ToString(), wWWForm);
				result = base.StartCoroutine(this.ProcessMatchResponse<JoinMatchResponse, NetworkMatch.DataResponseDelegate<MatchInfo>>(client, new NetworkMatch.InternalResponseDelegate<JoinMatchResponse, NetworkMatch.DataResponseDelegate<MatchInfo>>(this.OnMatchJoined), callback));
			}
			return result;
		}

		internal void OnMatchJoined(JoinMatchResponse response, NetworkMatch.DataResponseDelegate<MatchInfo> userCallback)
		{
			if (response.success)
			{
				Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
			}
			userCallback(response.success, response.extendedInfo, new MatchInfo(response));
		}

		public Coroutine DestroyMatch(NetworkID netId, int requestDomain, NetworkMatch.BasicResponseDelegate callback)
		{
			return this.DestroyMatch(new DestroyMatchRequest
			{
				networkId = netId,
				domain = requestDomain
			}, callback);
		}

		internal Coroutine DestroyMatch(DestroyMatchRequest req, NetworkMatch.BasicResponseDelegate callback)
		{
			Coroutine result;
			if (callback == null)
			{
				UnityEngine.Debug.Log("callback supplied is null, aborting DestroyMatch Request.");
				result = null;
			}
			else
			{
				Uri uri = new Uri(this.baseUri, "/json/reply/DestroyMatchRequest");
				UnityEngine.Debug.Log("MatchMakingClient Destroy :" + uri.ToString());
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("version", Request.currentVersion);
				wWWForm.AddField("projectId", Application.cloudProjectId);
				wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
				wWWForm.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
				wWWForm.AddField("domain", req.domain);
				wWWForm.AddField("networkId", req.networkId.ToString());
				wWWForm.headers["Accept"] = "application/json";
				WWW client = new WWW(uri.ToString(), wWWForm);
				result = base.StartCoroutine(this.ProcessMatchResponse<BasicResponse, NetworkMatch.BasicResponseDelegate>(client, new NetworkMatch.InternalResponseDelegate<BasicResponse, NetworkMatch.BasicResponseDelegate>(this.OnMatchDestroyed), callback));
			}
			return result;
		}

		internal void OnMatchDestroyed(BasicResponse response, NetworkMatch.BasicResponseDelegate userCallback)
		{
			userCallback(response.success, response.extendedInfo);
		}

		public Coroutine DropConnection(NetworkID netId, NodeID dropNodeId, int requestDomain, NetworkMatch.BasicResponseDelegate callback)
		{
			return this.DropConnection(new DropConnectionRequest
			{
				networkId = netId,
				nodeId = dropNodeId,
				domain = requestDomain
			}, callback);
		}

		internal Coroutine DropConnection(DropConnectionRequest req, NetworkMatch.BasicResponseDelegate callback)
		{
			Coroutine result;
			if (callback == null)
			{
				UnityEngine.Debug.Log("callback supplied is null, aborting DropConnection Request.");
				result = null;
			}
			else
			{
				Uri uri = new Uri(this.baseUri, "/json/reply/DropConnectionRequest");
				UnityEngine.Debug.Log("MatchMakingClient DropConnection :" + uri);
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("version", Request.currentVersion);
				wWWForm.AddField("projectId", Application.cloudProjectId);
				wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
				wWWForm.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
				wWWForm.AddField("domain", req.domain);
				wWWForm.AddField("networkId", req.networkId.ToString());
				wWWForm.AddField("nodeId", req.nodeId.ToString());
				wWWForm.headers["Accept"] = "application/json";
				WWW client = new WWW(uri.ToString(), wWWForm);
				result = base.StartCoroutine(this.ProcessMatchResponse<DropConnectionResponse, NetworkMatch.BasicResponseDelegate>(client, new NetworkMatch.InternalResponseDelegate<DropConnectionResponse, NetworkMatch.BasicResponseDelegate>(this.OnDropConnection), callback));
			}
			return result;
		}

		internal void OnDropConnection(DropConnectionResponse response, NetworkMatch.BasicResponseDelegate userCallback)
		{
			userCallback(response.success, response.extendedInfo);
		}

		public Coroutine ListMatches(int startPageNumber, int resultPageSize, string matchNameFilter, bool filterOutPrivateMatchesFromResults, int eloScoreTarget, int requestDomain, NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>> callback)
		{
			Coroutine result;
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				UnityEngine.Debug.LogError("Matchmaking is not supported on WebGL player.");
				result = null;
			}
			else
			{
				result = this.ListMatches(new ListMatchRequest
				{
					pageNum = startPageNumber,
					pageSize = resultPageSize,
					nameFilter = matchNameFilter,
					filterOutPrivateMatches = filterOutPrivateMatchesFromResults,
					eloScore = eloScoreTarget,
					domain = requestDomain
				}, callback);
			}
			return result;
		}

		internal Coroutine ListMatches(ListMatchRequest req, NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>> callback)
		{
			Coroutine result;
			if (callback == null)
			{
				UnityEngine.Debug.Log("callback supplied is null, aborting ListMatch Request.");
				result = null;
			}
			else
			{
				Uri uri = new Uri(this.baseUri, "/json/reply/ListMatchRequest");
				UnityEngine.Debug.Log("MatchMakingClient ListMatches :" + uri);
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("version", Request.currentVersion);
				wWWForm.AddField("projectId", Application.cloudProjectId);
				wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
				wWWForm.AddField("accessTokenString", 0);
				wWWForm.AddField("domain", req.domain);
				wWWForm.AddField("pageSize", req.pageSize);
				wWWForm.AddField("pageNum", req.pageNum);
				wWWForm.AddField("nameFilter", req.nameFilter);
				wWWForm.AddField("filterOutPrivateMatches", req.filterOutPrivateMatches.ToString());
				wWWForm.AddField("eloScore", req.eloScore.ToString());
				wWWForm.headers["Accept"] = "application/json";
				WWW client = new WWW(uri.ToString(), wWWForm);
				result = base.StartCoroutine(this.ProcessMatchResponse<ListMatchResponse, NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>>>(client, new NetworkMatch.InternalResponseDelegate<ListMatchResponse, NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>>>(this.OnMatchList), callback));
			}
			return result;
		}

		internal void OnMatchList(ListMatchResponse response, NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>> userCallback)
		{
			List<MatchInfoSnapshot> list = new List<MatchInfoSnapshot>();
			foreach (MatchDesc current in response.matches)
			{
				list.Add(new MatchInfoSnapshot(current));
			}
			userCallback(response.success, response.extendedInfo, list);
		}

		public Coroutine SetMatchAttributes(NetworkID networkId, bool isListed, int requestDomain, NetworkMatch.BasicResponseDelegate callback)
		{
			return this.SetMatchAttributes(new SetMatchAttributesRequest
			{
				networkId = networkId,
				isListed = isListed,
				domain = requestDomain
			}, callback);
		}

		internal Coroutine SetMatchAttributes(SetMatchAttributesRequest req, NetworkMatch.BasicResponseDelegate callback)
		{
			Coroutine result;
			if (callback == null)
			{
				UnityEngine.Debug.Log("callback supplied is null, aborting SetMatchAttributes Request.");
				result = null;
			}
			else
			{
				Uri uri = new Uri(this.baseUri, "/json/reply/SetMatchAttributesRequest");
				UnityEngine.Debug.Log("MatchMakingClient SetMatchAttributes :" + uri);
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("version", Request.currentVersion);
				wWWForm.AddField("projectId", Application.cloudProjectId);
				wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
				wWWForm.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
				wWWForm.AddField("domain", req.domain);
				wWWForm.AddField("networkId", req.networkId.ToString());
				wWWForm.AddField("isListed", req.isListed.ToString());
				wWWForm.headers["Accept"] = "application/json";
				WWW client = new WWW(uri.ToString(), wWWForm);
				result = base.StartCoroutine(this.ProcessMatchResponse<BasicResponse, NetworkMatch.BasicResponseDelegate>(client, new NetworkMatch.InternalResponseDelegate<BasicResponse, NetworkMatch.BasicResponseDelegate>(this.OnSetMatchAttributes), callback));
			}
			return result;
		}

		internal void OnSetMatchAttributes(BasicResponse response, NetworkMatch.BasicResponseDelegate userCallback)
		{
			userCallback(response.success, response.extendedInfo);
		}

		[DebuggerHidden]
		private IEnumerator ProcessMatchResponse<JSONRESPONSE, USERRESPONSEDELEGATETYPE>(WWW client, NetworkMatch.InternalResponseDelegate<JSONRESPONSE, USERRESPONSEDELEGATETYPE> internalCallback, USERRESPONSEDELEGATETYPE userCallback) where JSONRESPONSE : Response, new()
		{
			NetworkMatch.<ProcessMatchResponse>c__Iterator0<JSONRESPONSE, USERRESPONSEDELEGATETYPE> <ProcessMatchResponse>c__Iterator = new NetworkMatch.<ProcessMatchResponse>c__Iterator0<JSONRESPONSE, USERRESPONSEDELEGATETYPE>();
			<ProcessMatchResponse>c__Iterator.client = client;
			<ProcessMatchResponse>c__Iterator.internalCallback = internalCallback;
			<ProcessMatchResponse>c__Iterator.userCallback = userCallback;
			return <ProcessMatchResponse>c__Iterator;
		}
	}
}
