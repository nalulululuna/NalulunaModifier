using System;
using System.Reflection;
using UnityEngine;

namespace NalulunaModifier
{
	public static class ReflectionUtil
	{
		public static void SetPrivateField(this object obj, string fieldName, object value)
		{
			Type t = obj.GetType();
			FieldInfo fi = null;
			while (t != null)
			{
				fi = t.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				if (fi != null) break;
				t = t.BaseType;
			}
			fi.SetValue(obj, value);
		}

		public static T GetPrivateField<T>(this object obj, string fieldName)
		{
			Logger.log.Debug($"GetPrivateField1-{fieldName}");
			Type t = obj.GetType();
			FieldInfo fi = null;
			while (t != null)
			{
				fi = t.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				if (fi != null) break;
				t = t.BaseType;
			}
			var value = fi.GetValue(obj);
			return (T)value;
		}

		public static void SetPrivateProperty(this object obj, string propertyName, object value)
		{
			Type t = obj.GetType();
			PropertyInfo pi = null;
			while (t != null)
			{
				pi = t.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				if (pi != null) break;
				t = t.BaseType;
			}
			pi.SetValue(obj, value, null);
		}

		public static void SetNonPublicProperty(this object obj, string propertyName, object value)
		{
			Type t = obj.GetType();
			PropertyInfo pi = null;
			while (t != null)
			{
				pi = t.GetProperty(propertyName);
				if (pi != null) break;
				t = t.BaseType;
			}
			pi = pi.DeclaringType.GetProperty(propertyName);
			pi.GetSetMethod(true).Invoke(obj, new object[] { value });
		}

		public static T GetPrivateProperty<T>(this object obj, string propertyName)
		{
			Type t = obj.GetType();
			PropertyInfo pi = null;
			while (t != null)
			{
				pi = t.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				if (pi != null) break;
				t = t.BaseType;
			}
			var value = pi.GetValue(obj);
			return (T)value;
		}

		public static void InvokePrivateMethod(this object obj, string methodName, object[] methodParams)
		{
			Type t = obj.GetType();
			MethodInfo mi = null;
			while (t != null)
			{
				mi = t.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				if (mi != null) break;
				t = t.BaseType;
			}
			mi.Invoke(obj, methodParams);
		}

		public static Component CopyComponent(Component original, Type originalType, Type overridingType,
			GameObject destination)
		{
			var copy = destination.AddComponent(overridingType);
			var fields = originalType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
			                                    BindingFlags.GetField);
			foreach (var field in fields)
			{
				var value = field.GetValue(original);
				field.SetValue(copy, value);
			}

			return copy;
		}
	}
}
