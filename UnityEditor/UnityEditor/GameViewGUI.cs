using System;
using System.Text;
using UnityEngine;
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
		public static void GameViewStatsGUI()
		{
			GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
			GUI.color = new Color(1f, 1f, 1f, 0.75f);
			float num = 300f;
			float num2 = 200f;
			int num3 = Network.connections.Length;
			if (num3 != 0)
			{
				num2 += 220f;
			}
			GUILayout.BeginArea(new Rect((float)Screen.width - num - 10f, 27f, num, num2), "Statistics", GUI.skin.window);
			GUILayout.Label("Graphics:", GameViewGUI.sectionHeaderStyle, new GUILayoutOption[0]);
			GameViewGUI.UpdateFrameTime();
			string text = string.Format("{0:F1} FPS ({1:F1}ms)", 1f / Mathf.Max(GameViewGUI.m_MaxFrameTime, 1E-05f), GameViewGUI.m_MaxFrameTime * 1000f);
			GUI.Label(new Rect(170f, 19f, 120f, 20f), text);
			int usedTextureMemorySize = UnityStats.usedTextureMemorySize;
			int renderTextureBytes = UnityStats.renderTextureBytes;
			int screenBytes = UnityStats.screenBytes;
			int vboTotalBytes = UnityStats.vboTotalBytes;
			int bytes = screenBytes + renderTextureBytes;
			int bytes2 = screenBytes + Mathf.Max(usedTextureMemorySize, renderTextureBytes) + vboTotalBytes;
			StringBuilder stringBuilder = new StringBuilder(400);
			stringBuilder.Append(string.Format("  Main Thread: {0:F1}ms  Renderer: {1:F1}ms\n", GameViewGUI.m_ClientFrameTime * 1000f, GameViewGUI.m_RenderFrameTime * 1000f));
			stringBuilder.Append(string.Format("  Draw Calls: {0} \tSaved by batching: {1} \n", UnityStats.drawCalls, UnityStats.batchedDrawCalls - UnityStats.batches));
			stringBuilder.Append(string.Format("  Tris: {0} \tVerts: {1} \n", GameViewGUI.FormatNumber(UnityStats.triangles), GameViewGUI.FormatNumber(UnityStats.vertices)));
			stringBuilder.Append(string.Format("  Used Textures: {0} - {1}\n", UnityStats.usedTextureCount, EditorUtility.FormatBytes(usedTextureMemorySize)));
			stringBuilder.Append(string.Format("  Render Textures: {0} - {1} \tswitches: {2}\n", UnityStats.renderTextureCount, EditorUtility.FormatBytes(renderTextureBytes), UnityStats.renderTextureChanges));
			stringBuilder.Append(string.Format("  Screen: {0} - {1}\n", UnityStats.screenRes, EditorUtility.FormatBytes(screenBytes)));
			stringBuilder.Append(string.Format("  VRAM usage: {0} to {1} (of {2})\n", EditorUtility.FormatBytes(bytes), EditorUtility.FormatBytes(bytes2), EditorUtility.FormatBytes(SystemInfo.graphicsMemorySize * 1024 * 1024)));
			stringBuilder.Append(string.Format("  VBO Total: {0} - {1}\n", UnityStats.vboTotal, EditorUtility.FormatBytes(vboTotalBytes)));
			stringBuilder.Append(string.Format("  Shadow Casters: {0} \n", UnityStats.shadowCasters));
			stringBuilder.Append(string.Format("  Visible Skinned Meshes: {0} \t Animations: {1}", UnityStats.visibleSkinnedMeshes, UnityStats.visibleAnimations));
			GUILayout.Label(stringBuilder.ToString(), new GUILayoutOption[0]);
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
