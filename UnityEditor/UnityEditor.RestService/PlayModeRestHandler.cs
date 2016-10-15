using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UnityEditor.RestService
{
	internal class PlayModeRestHandler : Handler
	{
		protected override JSONValue HandlePost(Request request, JSONValue payload)
		{
			string text = payload.Get("action").AsString();
			string s = this.CurrentState();
			string text2 = text;
			if (text2 != null)
			{
				if (PlayModeRestHandler.<>f__switch$mapA == null)
				{
					PlayModeRestHandler.<>f__switch$mapA = new Dictionary<string, int>(3)
					{
						{
							"play",
							0
						},
						{
							"pause",
							1
						},
						{
							"stop",
							2
						}
					};
				}
				int num;
				if (PlayModeRestHandler.<>f__switch$mapA.TryGetValue(text2, out num))
				{
					switch (num)
					{
					case 0:
						EditorApplication.isPlaying = true;
						EditorApplication.isPaused = false;
						break;
					case 1:
						EditorApplication.isPaused = true;
						break;
					case 2:
						EditorApplication.isPlaying = false;
						break;
					default:
						goto IL_B8;
					}
					JSONValue result = default(JSONValue);
					result["oldstate"] = s;
					result["newstate"] = this.CurrentState();
					return result;
				}
			}
			IL_B8:
			throw new RestRequestException
			{
				HttpStatusCode = HttpStatusCode.BadRequest,
				RestErrorString = "Invalid action: " + text
			};
		}

		protected override JSONValue HandleGet(Request request, JSONValue payload)
		{
			JSONValue result = default(JSONValue);
			result["state"] = this.CurrentState();
			return result;
		}

		internal static void Register()
		{
			Router.RegisterHandler("/unity/playmode", new PlayModeRestHandler());
		}

		internal string CurrentState()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return "stopped";
			}
			return (!EditorApplication.isPaused) ? "playing" : "paused";
		}
	}
}
