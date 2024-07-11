using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System;

public static class EnumExpansions
{
	public static string GetDescription(this Enum en)
	{
		MemberInfo[] member = en.GetType().GetMember(en.ToString());
		if (member == null || member.Length == 0)
		{
			return en.ToString();
		}
		object[] customAttributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
		if (customAttributes != null && customAttributes.Any())
		{
			return ((DescriptionAttribute)customAttributes.First()).Description;
		}
		return en.ToString();
	}
}
