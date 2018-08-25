using ProjectNameTemplate.Common.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProjectNameTemplate.Common.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        ///  获取枚举的描述
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string Description(this Enum e)
        {
            string objName = e.ToString();
            Type t = e.GetType();
            FieldInfo fi = t.GetField(objName);
            if (fi == null)
                return string.Empty;
            DescriptionAttribute[] arrDesc = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return arrDesc[0].Description;
        }

        /// <summary>
        /// 获取DisplayAttribute上指定的Name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDisplay(this Enum value)
        {
            var info = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)info.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
            return attribute == null ? value.ToString() : attribute.Name;
        }

        /// <summary>
        ///  获取DescriptionAttribute上指定的Description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum value)
        {
            return EnumHelper.GetEnumDescription(value);
        }

        /// <summary>
        ///  转化为枚举类型，转化不成功为默认值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int value, T defVal = default(T)) where T : struct
        {
            return EnumHelper.ConvertToEnum<T>(value.ToString(), defVal);
        }


        public static T ToEnum<T>(this string value, T defVal = default(T)) where T : struct
        {
            return EnumHelper.ConvertToEnum<T>(value.ToString(), defVal);
        }

        /// <summary>
        /// 获取DisplayAttribute上指定的Name
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal">defVal</param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum value, string defVal = "")
        {
            var attribute = EnumHelper.GetEnumDisplayAttributs(value).FirstOrDefault();
            return attribute == null ? defVal : attribute.GetName();
        }

        /// <summary>
        /// 获取DisplayAttribute上指定的ShortName
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal">defVal</param>
        /// <returns></returns>
        public static string GetDisplayShortName(this Enum value, string defVal = "")
        {
            var attribute = EnumHelper.GetEnumDisplayAttributs(value).FirstOrDefault();
            return attribute == null ? defVal : attribute.GetShortName();
        }

        /// <summary>
        /// 获取DisplayAttribute上指定的Desc
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal">defVal</param>
        /// <returns></returns>
        public static string GetDisplayDesc(this Enum value, string defVal = "")
        {
            var attribute = EnumHelper.GetEnumDisplayAttributs(value).FirstOrDefault();
            return attribute == null ? defVal : attribute.GetDescription();
        }

        /// <summary>
        /// 将枚举转成枚举数组
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static List<TEnum> ToList<TEnum>(this Enum arg) where TEnum : struct
        {
            var obj1 = arg.ToString();
            var obj2 = obj1.Split(',').ToList();
            return obj2.Select(t => EnumHelper.ConvertToEnum<TEnum>(t)).ToList();
        }

        /// <summary>
        /// 将枚举转成int数组
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static List<int> ToListNumber<TEnum>(this Enum arg) where TEnum : struct
        {
            var obj1 = arg.ToString();
            var obj2 = obj1.Split(',').ToList();
            return obj2.Select(t => Convert.ToInt32(EnumHelper.ConvertToEnum<TEnum>(t))).ToList();
        }
    }
}
