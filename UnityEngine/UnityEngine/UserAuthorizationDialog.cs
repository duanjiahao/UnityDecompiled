using System;

namespace UnityEngine
{
	[AddComponentMenu("")]
	internal class UserAuthorizationDialog : MonoBehaviour
	{
		private Rect windowRect;

		private const int width = 385;

		private const int height = 155;

		private Texture warningIcon;

		private void Start()
		{
			this.warningIcon = (Resources.GetBuiltinResource(typeof(Texture2D), "WarningSign.psd") as Texture2D);
			if (Screen.width < 385 || Screen.height < 155)
			{
				Debug.LogError("Screen is to small to display authorization dialog. Authorization denied.");
				Application.ReplyToUserAuthorizationRequest(false);
			}
			this.windowRect = new Rect((float)(Screen.width / 2 - 192), (float)(Screen.height / 2 - 77), 385f, 155f);
		}

		private void OnGUI()
		{
			GUISkin skin = GUI.skin;
			GUISkin gUISkin = ScriptableObject.CreateInstance("GUISkin") as GUISkin;
			gUISkin.box.normal.background = (Texture2D)Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/box.png");
			gUISkin.box.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
			gUISkin.box.padding.left = 6;
			gUISkin.box.padding.right = 6;
			gUISkin.box.padding.top = 4;
			gUISkin.box.padding.bottom = 4;
			gUISkin.box.border.left = 6;
			gUISkin.box.border.right = 6;
			gUISkin.box.border.top = 6;
			gUISkin.box.border.bottom = 6;
			gUISkin.box.margin.left = 4;
			gUISkin.box.margin.right = 4;
			gUISkin.box.margin.top = 4;
			gUISkin.box.margin.bottom = 4;
			gUISkin.button.normal.background = (Texture2D)Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/button.png");
			gUISkin.button.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
			gUISkin.button.hover.background = (Texture2D)Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/button hover.png");
			gUISkin.button.hover.textColor = Color.white;
			gUISkin.button.active.background = (Texture2D)Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/button active.png");
			gUISkin.button.active.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
			gUISkin.button.border.left = 6;
			gUISkin.button.border.right = 6;
			gUISkin.button.border.top = 6;
			gUISkin.button.border.bottom = 6;
			gUISkin.button.padding.left = 8;
			gUISkin.button.padding.right = 8;
			gUISkin.button.padding.top = 4;
			gUISkin.button.padding.bottom = 4;
			gUISkin.button.margin.left = 4;
			gUISkin.button.margin.right = 4;
			gUISkin.button.margin.top = 4;
			gUISkin.button.margin.bottom = 4;
			gUISkin.label.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
			gUISkin.label.padding.left = 6;
			gUISkin.label.padding.right = 6;
			gUISkin.label.padding.top = 4;
			gUISkin.label.padding.bottom = 4;
			gUISkin.label.margin.left = 4;
			gUISkin.label.margin.right = 4;
			gUISkin.label.margin.top = 4;
			gUISkin.label.margin.bottom = 4;
			gUISkin.label.alignment = TextAnchor.UpperLeft;
			gUISkin.window.normal.background = (Texture2D)Resources.GetBuiltinResource(typeof(Texture2D), "GameSkin/window.png");
			gUISkin.window.normal.textColor = Color.white;
			gUISkin.window.border.left = 8;
			gUISkin.window.border.right = 8;
			gUISkin.window.border.top = 18;
			gUISkin.window.border.bottom = 8;
			gUISkin.window.padding.left = 8;
			gUISkin.window.padding.right = 8;
			gUISkin.window.padding.top = 20;
			gUISkin.window.padding.bottom = 5;
			gUISkin.window.alignment = TextAnchor.UpperCenter;
			gUISkin.window.contentOffset = new Vector2(0f, -18f);
			GUI.skin = gUISkin;
			this.windowRect = GUI.Window(0, this.windowRect, new GUI.WindowFunction(this.DoUserAuthorizationDialog), "Unity Web Player Authorization Request");
			GUI.skin = skin;
		}

		private void DoUserAuthorizationDialog(int windowID)
		{
			UserAuthorization userAuthorizationRequestMode = Application.GetUserAuthorizationRequestMode();
			GUILayout.FlexibleSpace();
			GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.7f);
			GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(this.warningIcon, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (userAuthorizationRequestMode == (UserAuthorization)3)
			{
				GUILayout.Label("The content on this site would like to use your\ncomputer's web camera and microphone.\nThese images and sounds may be recorded.", new GUILayoutOption[0]);
			}
			else if (userAuthorizationRequestMode == UserAuthorization.WebCam)
			{
				GUILayout.Label("The content on this site would like to use\nyour computer's web camera. The images\nfrom your web camera may be recorded.", new GUILayoutOption[0]);
			}
			else
			{
				if (userAuthorizationRequestMode != UserAuthorization.Microphone)
				{
					return;
				}
				GUILayout.Label("The content on this site would like to use\nyour computer's microphone. The sounds\nfrom your microphone may be recorded.", new GUILayoutOption[0]);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUI.backgroundColor = Color.white;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Deny", new GUILayoutOption[0]))
			{
				Application.ReplyToUserAuthorizationRequest(false);
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Always Allow for this Site", new GUILayoutOption[0]))
			{
				Application.ReplyToUserAuthorizationRequest(true, true);
			}
			GUILayout.Space(5f);
			if (GUILayout.Button("Allow", new GUILayoutOption[0]))
			{
				Application.ReplyToUserAuthorizationRequest(true);
			}
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		}
	}
}
