using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Gestor.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value) {
            return GetEnumDisplayName(value.GetType(), value);
        }
        public static string GetDescription(this Enum value) {
            return GetEnumDescription(value.GetType(), value);
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, int value, string placeholder = "")
        where TEnum : struct, IComparable, IFormattable, IConvertible {
            var destino_list = enumObj.ToDictionary();
            if (!string.IsNullOrEmpty( placeholder))
                destino_list.Add(-1, placeholder);

            destino_list = destino_list.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            var select_list = new SelectList(destino_list, "Key", "Value", value > 0 ? value : -1);

            return select_list;
        }

        public static MultiSelectList ToMultiSelectList<TEnum>(this TEnum enumObj, int[] values, string placeholder = "")
        where TEnum : struct, IComparable, IFormattable, IConvertible {
            var destino_list = enumObj.ToDictionary();
            if (!string.IsNullOrEmpty( placeholder))
                destino_list.Add(-1, placeholder);

            destino_list = destino_list.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            var select_list = new MultiSelectList(destino_list, "Key", "Value", values != null ? values : new int[] { -1 });
            
            if (!string.IsNullOrEmpty( placeholder))
                select_list.First().Disabled = true;

            return select_list;
        }

        public static Dictionary<int, string> ToDictionary<TEnum>(this TEnum enumObj)
        where TEnum : struct, IComparable, IFormattable, IConvertible {
            var values = from Enum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = Convert.ToInt32(e), e.GetAttribute<DisplayAttribute>().Name };
            return values.ToDictionary(key => key.Id, value => value.Name);
        }

        public static IEnumerable<string> GetEnumDisplayNames(Type type) {
            if (!type.IsEnum) throw new ArgumentException(String.Format("Type '{0}' is not Enum", type));

            foreach (var item in Enum.GetNames(type)) {
                var member = type.GetMember(item).First();
                var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attributes.Length == 0) {
                    yield return member.Name;
                } else {
                    yield return ((DisplayAttribute)attributes[0]).GetName();
                }
            }
        }


        public static string GetEnumDisplayName(Type type, object value) {
            if (value == null) {
                return "";
            }
            if (!type.IsEnum) throw new ArgumentException(String.Format("Type '{0}' is not Enum", type));

            var memberName = Enum.GetName(type, value);
            if (string.IsNullOrEmpty(memberName)) {
                return "";
            }

            var members = type.GetMember(memberName);
            if (members.Length == 0) throw new ArgumentException(String.Format("Member '{0}' not found in type '{1}'", value, type.Name));

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length == 0) {
                return memberName;
            }

            var attribute = (DisplayAttribute)attributes[0];
            return attribute.GetName();
        }

        public static string GetEnumDescription(Type type, object value) {
            if (value == null) {
                return "";
            }
            if (!type.IsEnum) throw new ArgumentException(String.Format("Type '{0}' is not Enum", type));

            var memberName = Enum.GetName(type, value);
            if (string.IsNullOrEmpty(memberName)) {
                return "";
            }

            var members = type.GetMember(memberName);
            if (members.Length == 0) throw new ArgumentException(String.Format("Member '{0}' not found in type '{1}'", value, type.Name));

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length == 0) {
                return memberName;
            }

            var attribute = (DescriptionAttribute)attributes[0];
            return attribute.Description;
        }


    }
}
