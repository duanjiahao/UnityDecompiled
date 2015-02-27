using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
namespace UnityEditor
{
	public abstract class MaterialPropertyDrawer
	{
		internal static Dictionary<string, MaterialPropertyDrawer> s_PropertyDrawers = new Dictionary<string, MaterialPropertyDrawer>();
		public virtual void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
		{
			EditorGUI.LabelField(position, new GUIContent(label), EditorGUIUtility.TempContent("No GUI Implemented"));
		}
		public virtual float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			return 16f;
		}
		public virtual void Apply(MaterialProperty prop)
		{
		}
		private static string GetPropertyString(Shader shader, string name)
		{
			if (shader == null)
			{
				return string.Empty;
			}
			return shader.GetInstanceID() + "_" + name;
		}
		internal static void InvalidatePropertyCache(Shader shader)
		{
			if (shader == null)
			{
				return;
			}
			string value = shader.GetInstanceID() + "_";
			List<string> list = new List<string>();
			foreach (string current in MaterialPropertyDrawer.s_PropertyDrawers.Keys)
			{
				if (current.StartsWith(value))
				{
					list.Add(current);
				}
			}
			foreach (string current2 in list)
			{
				MaterialPropertyDrawer.s_PropertyDrawers.Remove(current2);
			}
		}
		private static MaterialPropertyDrawer CreatePropertyDrawer(Type klass, string argsText)
		{
			if (string.IsNullOrEmpty(argsText))
			{
				return Activator.CreateInstance(klass) as MaterialPropertyDrawer;
			}
			string[] array = argsText.Split(new char[]
			{
				','
			});
			object[] array2 = new object[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				float num;
				if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out num))
				{
					array2[i] = num;
				}
				else
				{
					array2[i] = text;
				}
			}
			return Activator.CreateInstance(klass, array2) as MaterialPropertyDrawer;
		}
		private static MaterialPropertyDrawer GetShaderPropertyDrawer(Shader shader, string name)
		{
			string shaderPropertyAttribute = ShaderUtil.GetShaderPropertyAttribute(shader, name);
			if (string.IsNullOrEmpty(shaderPropertyAttribute))
			{
				return null;
			}
			string text = shaderPropertyAttribute;
			string text2 = string.Empty;
			Match match = Regex.Match(shaderPropertyAttribute, "(\\w+)\\s*\\((.*)\\)");
			if (match.Success)
			{
				text = match.Groups[1].Value;
				text2 = match.Groups[2].Value.Trim();
			}
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(MaterialPropertyDrawer)))
			{
				if (!(current.Name == text) && !(current.Name == text + "Drawer"))
				{
					if (!(current.Name == "Material" + text + "Drawer"))
					{
						continue;
					}
				}
				try
				{
					MaterialPropertyDrawer result = MaterialPropertyDrawer.CreatePropertyDrawer(current, text2);
					return result;
				}
				catch (Exception)
				{
					Debug.LogWarning(string.Format("Failed to create material drawer {0} with arguments '{1}'", text, text2));
					MaterialPropertyDrawer result = null;
					return result;
				}
			}
			return null;
		}
		internal static MaterialPropertyDrawer GetDrawer(Shader shader, string name)
		{
			if (shader == null)
			{
				return null;
			}
			string propertyString = MaterialPropertyDrawer.GetPropertyString(shader, name);
			MaterialPropertyDrawer shaderPropertyDrawer;
			if (MaterialPropertyDrawer.s_PropertyDrawers.TryGetValue(propertyString, out shaderPropertyDrawer))
			{
				return shaderPropertyDrawer;
			}
			shaderPropertyDrawer = MaterialPropertyDrawer.GetShaderPropertyDrawer(shader, name);
			MaterialPropertyDrawer.s_PropertyDrawers[propertyString] = shaderPropertyDrawer;
			return shaderPropertyDrawer;
		}
	}
}
