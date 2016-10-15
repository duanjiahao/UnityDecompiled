using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TimeControl
	{
		private class Styles
		{
			public GUIContent playIcon = EditorGUIUtility.IconContent("PlayButton");

			public GUIContent pauseIcon = EditorGUIUtility.IconContent("PauseButton");

			public GUIStyle playButton = "TimeScrubberButton";

			public GUIStyle timeScrubber = "TimeScrubber";
		}

		private const float kStepTime = 0.01f;

		private const float kScrubberHeight = 21f;

		private const float kPlayButtonWidth = 33f;

		public float currentTime = float.NegativeInfinity;

		private bool m_NextCurrentTimeSet;

		public float startTime;

		public float stopTime = 1f;

		public bool playSelection;

		public bool loop = true;

		public float playbackSpeed = 1f;

		private float m_DeltaTime;

		private bool m_DeltaTimeSet;

		private double m_LastFrameEditorTime;

		private bool m_Playing;

		private bool m_ResetOnPlay;

		private float m_MouseDrag;

		private bool m_WrapForwardDrag;

		private static TimeControl.Styles s_Styles;

		private static readonly int kScrubberIDHash = "ScrubberIDHash".GetHashCode();

		public float nextCurrentTime
		{
			set
			{
				this.deltaTime = value - this.currentTime;
				this.m_NextCurrentTimeSet = true;
			}
		}

		public float deltaTime
		{
			get
			{
				return this.m_DeltaTime;
			}
			set
			{
				this.m_DeltaTime = value;
				this.m_DeltaTimeSet = true;
			}
		}

		public float normalizedTime
		{
			get
			{
				return (this.stopTime != this.startTime) ? ((this.currentTime - this.startTime) / (this.stopTime - this.startTime)) : 0f;
			}
			set
			{
				this.currentTime = this.startTime * (1f - value) + this.stopTime * value;
			}
		}

		public bool playing
		{
			get
			{
				return this.m_Playing;
			}
			set
			{
				if (this.m_Playing != value)
				{
					if (value)
					{
						EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(InspectorWindow.RepaintAllInspectors));
						this.m_LastFrameEditorTime = EditorApplication.timeSinceStartup;
						if (this.m_ResetOnPlay)
						{
							this.nextCurrentTime = this.startTime;
							this.m_ResetOnPlay = false;
						}
					}
					else
					{
						EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(InspectorWindow.RepaintAllInspectors));
					}
				}
				this.m_Playing = value;
			}
		}

		public void DoTimeControl(Rect rect)
		{
			if (TimeControl.s_Styles == null)
			{
				TimeControl.s_Styles = new TimeControl.Styles();
			}
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(TimeControl.kScrubberIDHash, FocusType.Keyboard);
			Rect rect2 = rect;
			rect2.height = 21f;
			Rect rect3 = rect2;
			rect3.xMin += 33f;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (rect.Contains(current.mousePosition))
				{
					GUIUtility.keyboardControl = controlID;
				}
				if (rect3.Contains(current.mousePosition))
				{
					EditorGUIUtility.SetWantsMouseJumping(1);
					GUIUtility.hotControl = controlID;
					this.m_MouseDrag = current.mousePosition.x - rect3.xMin;
					this.nextCurrentTime = this.m_MouseDrag * (this.stopTime - this.startTime) / rect3.width + this.startTime;
					this.m_WrapForwardDrag = false;
					current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					EditorGUIUtility.SetWantsMouseJumping(0);
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					this.m_MouseDrag += current.delta.x * this.playbackSpeed;
					if (this.loop && ((this.m_MouseDrag < 0f && this.m_WrapForwardDrag) || this.m_MouseDrag > rect3.width))
					{
						if (this.m_MouseDrag > rect3.width)
						{
							this.currentTime -= this.stopTime - this.startTime;
						}
						else if (this.m_MouseDrag < 0f)
						{
							this.currentTime += this.stopTime - this.startTime;
						}
						this.m_WrapForwardDrag = true;
						this.m_MouseDrag = Mathf.Repeat(this.m_MouseDrag, rect3.width);
					}
					this.nextCurrentTime = Mathf.Clamp(this.m_MouseDrag, 0f, rect3.width) * (this.stopTime - this.startTime) / rect3.width + this.startTime;
					current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.keyboardControl == controlID)
				{
					if (current.keyCode == KeyCode.LeftArrow)
					{
						if (this.currentTime - this.startTime > 0.01f)
						{
							this.deltaTime = -0.01f;
						}
						current.Use();
					}
					if (current.keyCode == KeyCode.RightArrow)
					{
						if (this.stopTime - this.currentTime > 0.01f)
						{
							this.deltaTime = 0.01f;
						}
						current.Use();
					}
				}
				break;
			}
			GUI.Box(rect2, GUIContent.none, TimeControl.s_Styles.timeScrubber);
			this.playing = GUI.Toggle(rect2, this.playing, (!this.playing) ? TimeControl.s_Styles.playIcon : TimeControl.s_Styles.pauseIcon, TimeControl.s_Styles.playButton);
			float num = Mathf.Lerp(rect3.x, rect3.xMax, this.normalizedTime);
			if (GUIUtility.keyboardControl == controlID)
			{
				Handles.color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				Handles.color = new Color(1f, 0f, 0f, 0.5f);
			}
			Handles.DrawLine(new Vector2(num, rect3.yMin), new Vector2(num, rect3.yMax));
			Handles.DrawLine(new Vector2(num + 1f, rect3.yMin), new Vector2(num + 1f, rect3.yMax));
		}

		public void OnDisable()
		{
			this.playing = false;
		}

		public void Update()
		{
			if (!this.m_DeltaTimeSet)
			{
				if (this.playing)
				{
					double timeSinceStartup = EditorApplication.timeSinceStartup;
					this.deltaTime = (float)(timeSinceStartup - this.m_LastFrameEditorTime) * this.playbackSpeed;
					this.m_LastFrameEditorTime = timeSinceStartup;
				}
				else
				{
					this.deltaTime = 0f;
				}
			}
			this.currentTime += this.deltaTime;
			bool flag = this.loop && this.playing && !this.m_NextCurrentTimeSet;
			if (flag)
			{
				this.normalizedTime = Mathf.Repeat(this.normalizedTime, 1f);
			}
			else
			{
				if (this.normalizedTime > 1f)
				{
					this.playing = false;
					this.m_ResetOnPlay = true;
				}
				this.normalizedTime = Mathf.Clamp01(this.normalizedTime);
			}
			this.m_DeltaTimeSet = false;
			this.m_NextCurrentTimeSet = false;
		}
	}
}
