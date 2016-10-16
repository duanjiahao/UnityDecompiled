using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor
{
	internal class MaterialPropertyHandler
	{
		private MaterialPropertyDrawer m_PropertyDrawer;

		private List<MaterialPropertyDrawer> m_DecoratorDrawers;

		private static Dictionary<string, MaterialPropertyHandler> s_PropertyHandlers = new Dictionary<string, MaterialPropertyHandler>();

		public MaterialPropertyDrawer propertyDrawer
		{
			get
			{
				return this.m_PropertyDrawer;
			}
		}

		public bool IsEmpty()
		{
			return this.m_PropertyDrawer == null && (this.m_DecoratorDrawers == null || this.m_DecoratorDrawers.Count == 0);
		}

		public void OnGUI(ref Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			float num = position.height;
			position.height = 0f;
			if (this.m_DecoratorDrawers != null)
			{
				foreach (MaterialPropertyDrawer current in this.m_DecoratorDrawers)
				{
					position.height = current.GetPropertyHeight(prop, label.text, editor);
					float labelWidth = EditorGUIUtility.labelWidth;
					float fieldWidth = EditorGUIUtility.fieldWidth;
					current.OnGUI(position, prop, label, editor);
					EditorGUIUtility.labelWidth = labelWidth;
					EditorGUIUtility.fieldWidth = fieldWidth;
					position.y += position.height;
					num -= position.height;
				}
			}
			position.height = num;
			if (this.m_PropertyDrawer != null)
			{
				float labelWidth = EditorGUIUtility.labelWidth;
				float fieldWidth = EditorGUIUtility.fieldWidth;
				this.m_PropertyDrawer.OnGUI(position, prop, label, editor);
				EditorGUIUtility.labelWidth = labelWidth;
				EditorGUIUtility.fieldWidth = fieldWidth;
			}
		}

		public float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			float num = 0f;
			if (this.m_DecoratorDrawers != null)
			{
				foreach (MaterialPropertyDrawer current in this.m_DecoratorDrawers)
				{
					num += current.GetPropertyHeight(prop, label, editor);
				}
			}
			if (this.m_PropertyDrawer != null)
			{
				num += this.m_PropertyDrawer.GetPropertyHeight(prop, label, editor);
			}
			return num;
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
			foreach (string current in MaterialPropertyHandler.s_PropertyHandlers.Keys)
			{
				if (current.StartsWith(value))
				{
					list.Add(current);
				}
			}
			foreach (string current2 in list)
			{
				MaterialPropertyHandler.s_PropertyHandlers.Remove(current2);
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

		private static MaterialPropertyDrawer GetShaderPropertyDrawer(string attrib, out bool isDecorator)
		{
			isDecorator = false;
			string text = attrib;
			string text2 = string.Empty;
			Match match = Regex.Match(attrib, "(\\w+)\\s*\\((.*)\\)");
			if (match.Success)
			{
				text = match.Groups[1].Value;
				text2 = match.Groups[2].Value.Trim();
			}
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(MaterialPropertyDrawer)))
			{
				if (!(current.Name == text) && !(current.Name == text + "Drawer") && !(current.Name == "Material" + text + "Drawer") && !(current.Name == text + "Decorator"))
				{
					if (!(current.Name == "Material" + text + "Decorator"))
					{
						continue;
					}
				}
				try
				{
					isDecorator = current.Name.EndsWith("Decorator");
					MaterialPropertyDrawer result = MaterialPropertyHandler.CreatePropertyDrawer(current, text2);
					return result;
				}
				catch (Exception)
				{
					Debug.LogWarningFormat("Failed to create material drawer {0} with arguments '{1}'", new object[]
					{
						text,
						text2
					});
					MaterialPropertyDrawer result = null;
					return result;
				}
			}
			return null;
		}

		private static MaterialPropertyHandler GetShaderPropertyHandler(Shader shader, string name)
		{
			string[] shaderPropertyAttributes = ShaderUtil.GetShaderPropertyAttributes(shader, name);
			if (shaderPropertyAttributes == null || shaderPropertyAttributes.Length == 0)
			{
				return null;
			}
			MaterialPropertyHandler materialPropertyHandler = new MaterialPropertyHandler();
			string[] array = shaderPropertyAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				string attrib = array[i];
				bool flag;
				MaterialPropertyDrawer shaderPropertyDrawer = MaterialPropertyHandler.GetShaderPropertyDrawer(attrib, out flag);
				if (shaderPropertyDrawer != null)
				{
					if (flag)
					{
						if (materialPropertyHandler.m_DecoratorDrawers == null)
						{
							materialPropertyHandler.m_DecoratorDrawers = new List<MaterialPropertyDrawer>();
						}
						materialPropertyHandler.m_DecoratorDrawers.Add(shaderPropertyDrawer);
					}
					else
					{
						if (materialPropertyHandler.m_PropertyDrawer != null)
						{
							Debug.LogWarning(string.Format("Shader property {0} already has a property drawer", name), shader);
						}
						materialPropertyHandler.m_PropertyDrawer = shaderPropertyDrawer;
					}
				}
			}
			return materialPropertyHandler;
		}

		internal static MaterialPropertyHandler GetHandler(Shader shader, string name)
		{
			if (shader == null)
			{
				return null;
			}
			string propertyString = MaterialPropertyHandler.GetPropertyString(shader, name);
			MaterialPropertyHandler materialPropertyHandler;
			if (MaterialPropertyHandler.s_PropertyHandlers.TryGetValue(propertyString, out materialPropertyHandler))
			{
				return materialPropertyHandler;
			}
			materialPropertyHandler = MaterialPropertyHandler.GetShaderPropertyHandler(shader, name);
			if (materialPropertyHandler != null && materialPropertyHandler.IsEmpty())
			{
				materialPropertyHandler = null;
			}
			MaterialPropertyHandler.s_PropertyHandlers[propertyString] = materialPropertyHandler;
			return materialPropertyHandler;
		}
	}
}
