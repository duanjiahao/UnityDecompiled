using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal class MaterialEnumDrawer : MaterialPropertyDrawer
	{
		private readonly GUIContent[] names;

		private readonly int[] values;

		public MaterialEnumDrawer(string enumName)
		{
			Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => AssemblyHelper.GetTypesFromAssembly(x)).ToArray<Type>();
			try
			{
				Type enumType = source.FirstOrDefault((Type x) => x.IsSubclassOf(typeof(Enum)) && (x.Name == enumName || x.FullName == enumName));
				string[] array = Enum.GetNames(enumType);
				this.names = new GUIContent[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.names[i] = new GUIContent(array[i]);
				}
				Array array2 = Enum.GetValues(enumType);
				this.values = new int[array2.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					this.values[j] = (int)array2.GetValue(j);
				}
			}
			catch (Exception)
			{
				Debug.LogWarningFormat("Failed to create MaterialEnum, enum {0} not found", new object[]
				{
					enumName
				});
				throw;
			}
		}

		public MaterialEnumDrawer(string n1, float v1) : this(new string[]
		{
			n1
		}, new float[]
		{
			v1
		})
		{
		}

		public MaterialEnumDrawer(string n1, float v1, string n2, float v2) : this(new string[]
		{
			n1,
			n2
		}, new float[]
		{
			v1,
			v2
		})
		{
		}

		public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3) : this(new string[]
		{
			n1,
			n2,
			n3
		}, new float[]
		{
			v1,
			v2,
			v3
		})
		{
		}

		public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4) : this(new string[]
		{
			n1,
			n2,
			n3,
			n4
		}, new float[]
		{
			v1,
			v2,
			v3,
			v4
		})
		{
		}

		public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5) : this(new string[]
		{
			n1,
			n2,
			n3,
			n4,
			n5
		}, new float[]
		{
			v1,
			v2,
			v3,
			v4,
			v5
		})
		{
		}

		public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6) : this(new string[]
		{
			n1,
			n2,
			n3,
			n4,
			n5,
			n6
		}, new float[]
		{
			v1,
			v2,
			v3,
			v4,
			v5,
			v6
		})
		{
		}

		public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6, string n7, float v7) : this(new string[]
		{
			n1,
			n2,
			n3,
			n4,
			n5,
			n6,
			n7
		}, new float[]
		{
			v1,
			v2,
			v3,
			v4,
			v5,
			v6,
			v7
		})
		{
		}

		public MaterialEnumDrawer(string[] enumNames, float[] vals)
		{
			this.names = new GUIContent[enumNames.Length];
			for (int i = 0; i < enumNames.Length; i++)
			{
				this.names[i] = new GUIContent(enumNames[i]);
			}
			this.values = new int[vals.Length];
			for (int j = 0; j < vals.Length; j++)
			{
				this.values[j] = (int)vals[j];
			}
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			if (prop.type != MaterialProperty.PropType.Float && prop.type != MaterialProperty.PropType.Range)
			{
				return 40f;
			}
			return base.GetPropertyHeight(prop, label, editor);
		}

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			if (prop.type != MaterialProperty.PropType.Float && prop.type != MaterialProperty.PropType.Range)
			{
				GUIContent label2 = EditorGUIUtility.TempContent("Enum used on a non-float property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
				EditorGUI.LabelField(position, label2, EditorStyles.helpBox);
				return;
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMixedValue;
			int num = (int)prop.floatValue;
			num = EditorGUI.IntPopup(position, label, num, this.names, this.values);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				prop.floatValue = (float)num;
			}
		}
	}
}
