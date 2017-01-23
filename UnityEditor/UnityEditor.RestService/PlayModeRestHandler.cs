using System;
using UnityEditorInternal;

namespace UnityEditor.RestService
{
	internal class PlayModeRestHandler : Handler
	{
		protected override JSONValue HandlePost(Request request, JSONValue payload)
		{
			string text = payload.Get("action").AsString();
			string s = this.CurrentState();
			if (text != null)
			{
				if (!(text == "play"))
				{
					if (!(text == "pause"))
					{
						if (!(text == "stop"))
						{
							goto IL_7F;
						}
						EditorApplication.isPlaying = false;
					}
					else
					{
						EditorApplication.isPaused = true;
					}
				}
				else
				{
					EditorApplication.isPlaying = true;
					EditorApplication.isPaused = false;
				}
				JSONValue result = default(JSONValue);
				result["oldstate"] = s;
				result["newstate"] = this.CurrentState();
				return result;
			}
			IL_7F:
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
			string result;
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				result = "stopped";
			}
			else
			{
				result = ((!EditorApplication.isPaused) ? "playing" : "paused");
			}
			return result;
		}
	}
}
