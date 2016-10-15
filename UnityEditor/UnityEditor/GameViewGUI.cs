using System;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal class GameViewGUI
	{
		private static int m_FrameCounter;

		private static float m_ClientTimeAccumulator;

		private static float m_RenderTimeAccumulator;

		private static float m_MaxTimeAccumulator;

		private static float m_ClientFrameTime;

		private static float m_RenderFrameTime;

		private static float m_MaxFrameTime;

		private static GUIStyle s_SectionHeaderStyle;

		private static GUIStyle s_LabelStyle;

		private static GUIStyle sectionHeaderStyle
		{
			get
			{
				if (GameViewGUI.s_SectionHeaderStyle == null)
				{
					GameViewGUI.s_SectionHeaderStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).GetStyle("BoldLabel");
				}
				return GameViewGUI.s_SectionHeaderStyle;
			}
		}

		private static GUIStyle labelStyle
		{
			get
			{
				if (GameViewGUI.s_LabelStyle == null)
				{
					GameViewGUI.s_LabelStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).label);
					GameViewGUI.s_LabelStyle.richText = true;
				}
				return GameViewGUI.s_LabelStyle;
			}
		}

		private static string FormatNumber(int num)
		{
			if (num < 1000)
			{
				return num.ToString();
			}
			if (num < 1000000)
			{
				return ((double)num * 0.001).ToString("f1") + "k";
			}
			return ((double)num * 1E-06).ToString("f1") + "M";
		}

		private static void UpdateFrameTime()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			float frameTime = UnityStats.frameTime;
			float renderTime = UnityStats.renderTime;
			GameViewGUI.m_ClientTimeAccumulator += frameTime;
			GameViewGUI.m_RenderTimeAccumulator += renderTime;
			GameViewGUI.m_MaxTimeAccumulator += Mathf.Max(frameTime, renderTime);
			GameViewGUI.m_FrameCounter++;
			bool flag = GameViewGUI.m_ClientFrameTime == 0f && GameViewGUI.m_RenderFrameTime == 0f;
			bool flag2 = GameViewGUI.m_FrameCounter > 30 || GameViewGUI.m_ClientTimeAccumulator > 0.3f || GameViewGUI.m_RenderTimeAccumulator > 0.3f;
			if (flag || flag2)
			{
				GameViewGUI.m_ClientFrameTime = GameViewGUI.m_ClientTimeAccumulator / (float)GameViewGUI.m_FrameCounter;
				GameViewGUI.m_RenderFrameTime = GameViewGUI.m_RenderTimeAccumulator / (float)GameViewGUI.m_FrameCounter;
				GameViewGUI.m_MaxFrameTime = GameViewGUI.m_MaxTimeAccumulator / (float)GameViewGUI.m_FrameCounter;
			}
			if (flag2)
			{
				GameViewGUI.m_ClientTimeAccumulator = 0f;
				GameViewGUI.m_RenderTimeAccumulator = 0f;
				GameViewGUI.m_MaxTimeAccumulator = 0f;
				GameViewGUI.m_FrameCounter = 0;
			}
		}

		private static string FormatDb(float vol)
		{
			if (vol == 0f)
			{
				return "-∞ dB";
			}
			return string.Format("{0:F1} dB", 20f * Mathf.Log10(vol));
		}

		[RequiredByNativeCode]
		public static void GameViewStatsGUI()
		{
			GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
			GUI.color = new Color(1f, 1f, 1f, 0.75f);
			float num = 300f;
			float num2 = 204f;
			int num3 = Network.connections.Length;
			if (num3 != 0)
			{
				num2 += 220f;
			}
			GUILayout.BeginArea(new Rect(GUIView.current.position.width - num - 10f, 27f, num, num2), "Statistics", GUI.skin.window);
			GUILayout.Label("Audio:", GameViewGUI.sectionHeaderStyle, new GUILayoutOption[0]);
			StringBuilder stringBuilder = new StringBuilder(400);
			float audioLevel = UnityStats.audioLevel;
			stringBuilder.Append("  Level: " + GameViewGUI.FormatDb(audioLevel) + ((!EditorUtility.audioMasterMute) ? "\n" : " (MUTED)\n"));
			stringBuilder.Append(string.Format("  Clipping: {0:F1}%", 100f * UnityStats.audioClippingAmount));
			GUILayout.Label(stringBuilder.ToString(), new GUILayoutOption[0]);
			GUI.Label(new Rect(170f, 40f, 120f, 20f), string.Format("DSP load: {0:F1}%", 100f * UnityStats.audioDSPLoad));
			GUI.Label(new Rect(170f, 53f, 120f, 20f), string.Format("Stream load: {0:F1}%", 100f * UnityStats.audioStreamLoad));
			GUILayout.Label("Graphics:", GameViewGUI.sectionHeaderStyle, new GUILayoutOption[0]);
			GameViewGUI.UpdateFrameTime();
			string text = string.Format("{0:F1} FPS ({1:F1}ms)", 1f / Mathf.Max(GameViewGUI.m_MaxFrameTime, 1E-05f), GameViewGUI.m_MaxFrameTime * 1000f);
			GUI.Label(new Rect(170f, 75f, 120f, 20f), text);
			int screenBytes = UnityStats.screenBytes;
			int num4 = UnityStats.dynamicBatchedDrawCalls - UnityStats.dynamicBatches;
			int num5 = UnityStats.staticBatchedDrawCalls - UnityStats.staticBatches;
			int num6 = UnityStats.instancedBatchedDrawCalls - UnityStats.instancedBatches;
			StringBuilder stringBuilder2 = new StringBuilder(400);
			if (GameViewGUI.m_ClientFrameTime > GameViewGUI.m_RenderFrameTime)
			{
				stringBuilder2.Append(string.Format("  CPU: main <b>{0:F1}</b>ms  render thread {1:F1}ms\n", GameViewGUI.m_ClientFrameTime * 1000f, GameViewGUI.m_RenderFrameTime * 1000f));
			}
			else
			{
				stringBuilder2.Append(string.Format("  CPU: main {0:F1}ms  render thread <b>{1:F1}</b>ms\n", GameViewGUI.m_ClientFrameTime * 1000f, GameViewGUI.m_RenderFrameTime * 1000f));
			}
			stringBuilder2.Append(string.Format("  Batches: <b>{0}</b> \tSaved by batching: {1}\n", UnityStats.batches, num4 + num5 + num6));
			stringBuilder2.Append(string.Format("  Tris: {0} \tVerts: {1} \n", GameViewGUI.FormatNumber(UnityStats.triangles), GameViewGUI.FormatNumber(UnityStats.vertices)));
			stringBuilder2.Append(string.Format("  Screen: {0} - {1}\n", UnityStats.screenRes, EditorUtility.FormatBytes(screenBytes)));
			stringBuilder2.Append(string.Format("  SetPass calls: {0} \tShadow casters: {1} \n", UnityStats.setPassCalls, UnityStats.shadowCasters));
			stringBuilder2.Append(string.Format("  Visible skinned meshes: {0}  Animations: {1}", UnityStats.visibleSkinnedMeshes, UnityStats.visibleAnimations));
			GUILayout.Label(stringBuilder2.ToString(), GameViewGUI.labelStyle, new GUILayoutOption[0]);
			if (num3 != 0)
			{
				GUILayout.Label("Network:", GameViewGUI.sectionHeaderStyle, new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				for (int i = 0; i < num3; i++)
				{
					GUILayout.Label(UnityStats.GetNetworkStats(i), new GUILayoutOption[0]);
				}
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.Label("Network: (no players connected)", GameViewGUI.sectionHeaderStyle, new GUILayoutOption[0]);
			}
			GUILayout.EndArea();
		}
	}
}
