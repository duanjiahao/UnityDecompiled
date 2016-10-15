using System;
using UnityEngine;

namespace UnityEditor
{
	internal class MaterialKeywordEnumDrawer : MaterialPropertyDrawer
	{
		private readonly GUIContent[] keywords;

		public MaterialKeywordEnumDrawer(string kw1) : this(new string[]
		{
			kw1
		})
		{
		}

		public MaterialKeywordEnumDrawer(string kw1, string kw2) : this(new string[]
		{
			kw1,
			kw2
		})
		{
		}

		public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3) : this(new string[]
		{
			kw1,
			kw2,
			kw3
		})
		{
		}

		public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4) : this(new string[]
		{
			kw1,
			kw2,
			kw3,
			kw4
		})
		{
		}

		public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5) : this(new string[]
		{
			kw1,
			kw2,
			kw3,
			kw4,
			kw5
		})
		{
		}

		public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6) : this(new string[]
		{
			kw1,
			kw2,
			kw3,
			kw4,
			kw5,
			kw6
		})
		{
		}

		public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7) : this(new string[]
		{
			kw1,
			kw2,
			kw3,
			kw4,
			kw5,
			kw6,
			kw7
		})
		{
		}

		public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8) : this(new string[]
		{
			kw1,
			kw2,
			kw3,
			kw4,
			kw5,
			kw6,
			kw7,
			kw8
		})
		{
		}

		public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8, string kw9) : this(new string[]
		{
			kw1,
			kw2,
			kw3,
			kw4,
			kw5,
			kw6,
			kw7,
			kw8,
			kw9
		})
		{
		}

		public MaterialKeywordEnumDrawer(params string[] keywords)
		{
			this.keywords = new GUIContent[keywords.Length];
			for (int i = 0; i < keywords.Length; i++)
			{
				this.keywords[i] = new GUIContent(keywords[i]);
			}
		}

		private static bool IsPropertyTypeSuitable(MaterialProperty prop)
		{
			return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range;
		}

		private void SetKeyword(MaterialProperty prop, int index)
		{
			for (int i = 0; i < this.keywords.Length; i++)
			{
				string keywordName = MaterialKeywordEnumDrawer.GetKeywordName(prop.name, this.keywords[i].text);
				UnityEngine.Object[] targets = prop.targets;
				for (int j = 0; j < targets.Length; j++)
				{
					Material material = (Material)targets[j];
					if (index == i)
					{
						material.EnableKeyword(keywordName);
					}
					else
					{
						material.DisableKeyword(keywordName);
					}
				}
			}
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			if (!MaterialKeywordEnumDrawer.IsPropertyTypeSuitable(prop))
			{
				return 40f;
			}
			return base.GetPropertyHeight(prop, label, editor);
		}

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			if (!MaterialKeywordEnumDrawer.IsPropertyTypeSuitable(prop))
			{
				GUIContent label2 = EditorGUIUtility.TempContent("KeywordEnum used on a non-float property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
				EditorGUI.LabelField(position, label2, EditorStyles.helpBox);
				return;
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			int num = (int)prop.floatValue;
			num = EditorGUI.Popup(position, label, num, this.keywords);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				prop.floatValue = (float)num;
				this.SetKeyword(prop, num);
			}
		}

		public override void Apply(MaterialProperty prop)
		{
			base.Apply(prop);
			if (!MaterialKeywordEnumDrawer.IsPropertyTypeSuitable(prop))
			{
				return;
			}
			if (prop.hasMixedValue)
			{
				return;
			}
			this.SetKeyword(prop, (int)prop.floatValue);
		}

		private static string GetKeywordName(string propName, string name)
		{
			string text = propName + "_" + name;
			return text.Replace(' ', '_').ToUpperInvariant();
		}
	}
}
