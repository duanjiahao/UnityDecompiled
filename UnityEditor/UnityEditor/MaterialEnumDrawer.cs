using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace UnityEditor
{
	internal class MaterialEnumDrawer : MaterialPropertyDrawer
	{
		private readonly string[] names;
		private readonly int[] values;
		public MaterialEnumDrawer(string enumName)
		{
			Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => AssemblyHelper.GetTypesFromAssembly(x)).ToArray<Type>();
			try
			{
				Type enumType = source.FirstOrDefault((Type x) => x.IsSubclassOf(typeof(Enum)) && (x.Name == enumName || x.FullName == enumName));
				this.names = Enum.GetNames(enumType);
				Array array = Enum.GetValues(enumType);
				this.values = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.values[i] = (int)array.GetValue(i);
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
		public MaterialEnumDrawer(string[] names, float[] vals)
		{
			this.names = names;
			this.values = new int[vals.Length];
			for (int i = 0; i < vals.Length; i++)
			{
				this.values[i] = (int)vals[i];
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
		public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
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
