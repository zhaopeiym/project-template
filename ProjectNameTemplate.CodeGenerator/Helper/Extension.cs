using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ProjectNameTemplate.CodeGenerator.Helper
{
    public static class Extension
    {
        /// <summary>
        /// 转C#类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCSharpType(this string str)
        {
            switch (str?.Trim().ToLower())
            {
                case "varchar":
                    return "string";
                case "datetime":
                    return "DateTime";
                case "bit":
                    return "bool";
                default:
                    return str;
            }
        }

        /// <summary>
        /// 转大驼峰
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBigHump(this string str, string suffix = "")
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            Match match = Regex.Match(str, @"_(\w*)*");
            while (match.Success)
            {
                var item = match.Value;
                while (item.IndexOf('_') >= 0)
                {
                    string newUpper = item.Substring(item.IndexOf('_'), 2);
                    item = item.Replace(newUpper, newUpper.Trim('_').ToUpper());
                    str = str.Replace(newUpper, newUpper.Trim('_').ToUpper());
                }
                match = match.NextMatch();
            }
            return str[0].ToString().ToUpper() + str.Remove(0, 1) + suffix;
        }

        /// <summary>
        /// 转小驼峰
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSmallHump(this string str, string suffix = "")
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            Match match = Regex.Match(str, @"_(\w*)*");
            while (match.Success)
            {
                var item = match.Value;
                while (item.IndexOf('_') >= 0)
                {
                    string newUpper = item.Substring(item.IndexOf('_'), 2);
                    item = item.Replace(newUpper, newUpper.Trim('_').ToUpper());
                    str = str.Replace(newUpper, newUpper.Trim('_').ToUpper());
                }
                match = match.NextMatch();
            }
            return str[0].ToString().ToLower() + str.Remove(0, 1) + suffix;
        }

        public static string ReplaceAnnotation(this string str)
        {
            return str.Replace("////", "").Replace("/*", "").Replace("*/", "");
        }

        public static void WriteAllText(this string strContent, string filepath)
        {
            Console.WriteLine($"生成文件{filepath}");
            File.WriteAllText(filepath, strContent);
        }
    }
}
