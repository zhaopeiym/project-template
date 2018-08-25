using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectNameTemplate.Common.Extensions
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// 将序列对象转换成列表对象
        /// </summary>
        /// <param name="list">序列对象</param>
        /// <returns></returns>
        public static List<int> Each2Int(this IEnumerable<string> list)
        {
            var lstVal = new List<int>();
            foreach (var item in list)
            {
                var value = -1;
                if (!int.TryParse(item, out value)) continue;
                lstVal.Add(value);
            }
            return lstVal;
        }

        public static List<T> WhereIfAdd<T>(this IEnumerable<T> list, bool condition, T item)
        {
            var myList = list?.ToList();
            if (condition) myList?.Add(item);
            return myList;
        }

        /// <summary>
        /// 枚举迭代序列对象，按照传入方法操作
        /// </summary>
        /// <typeparam name="T">序列对象类型</typeparam>
        /// <param name="list">序列对象</param>
        /// <param name="action">操作方法</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            var each = list as IList<T> ?? list.ToList();
            foreach (T t in each)
            {
                action.Invoke(t);
            }
            return each;
        }

        /// <summary>
        /// 确定序列是否包含任何元素。
        /// </summary>
        /// <typeparam name="T">序列对象类型</typeparam>
        /// <param name="t">序列对象</param>
        /// <returns>如果源序列包含任何元素，则为 true；否则为 false。</returns>
        public static bool IsAny<T>(this IEnumerable<T> t)
        {
            if (t == null) return false;
            return t.Any();
        }

        /// <summary>
        /// 将序列对象转换成字符串，多个时按分隔符分隔
        /// </summary>
        /// <typeparam name="T">序列对象的类型</typeparam>
        /// <param name="source">序列对象</param>
        /// <param name="separator">分隔符（默认,）</param>
        /// <returns></returns>
        public static string StringJoin<T>(this IEnumerable<T> source, string separator = ",")
        {
            if (!source.IsAny())
            {
                return "";
            }
            return string.Join(separator, source);
        }

        /// <summary>
        /// 串联集合的成员，其中在每个成员之间使用指定的分隔符。
        /// </summary>
        public static string JoinAsString<T>(this IEnumerable<T> source, string separator = ",")
        {
            if (typeof(T).IsEnum)
            {
                var enumSource = new List<int>();
                source.ForEach(f =>
                {
                    if (f != null)
                    {
                        var value = (int)Enum.Parse(typeof(T), f.ToString());
                        enumSource.Add(value);
                    }
                });
                return string.Join(separator, enumSource);
            };
            return string.Join(separator, source);
        }

        /// <summary>
        /// 串联集合的成员，其中在每个成员之间使用指定的分隔符。
        /// </summary>
        public static string JoinAsDbString<T>(this IEnumerable<T> source, string separator = ",")
        {
            if (typeof(T).IsEnum)
            {
                var enumSource = new List<int>();
                source.ForEach(f =>
                {
                    if (f != null)
                    {
                        var value = (int)Enum.Parse(typeof(T), f.ToString());
                        enumSource.Add(value);
                    }
                });
                return string.Join(separator, enumSource);
            };
            var type = typeof(T);
            if (type.Name == "Boolean")
            {
                var boolSource = new List<int>();
                source.ForEach(f =>
                {
                    if (f != null)
                    {
                        var value = Convert.ToInt32(Boolean.Parse(f.ToString()));

                        boolSource.Add(value);
                    }
                });
                return string.Join(separator, boolSource);
            }

            if (type.Name == "DateTime" || type.Name == "String")
            {
                var stringSource = new List<string>();
                source.ForEach(f =>
                {
                    if (f != null)
                    {
                        var value = $"'{f.ToString()}'";

                        stringSource.Add(value);
                    }
                });
                return string.Join(separator, stringSource);
            }
            return string.Join(separator, source);
        }

        /// <summary>
        /// 自定义排序，根据条件的先后将原List重新排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">需要排序的List</param>
        /// <param name="predicates">表达式条件</param>
        /// <returns></returns>
        public static List<T> CustomOrderBy<T>(this IEnumerable<T> list, params Func<T, bool>[] predicates)
        {
            List<T> items = new List<T>();
            foreach (var predicate in predicates)
            {
                var datas = list.Where(predicate).ToList();
                if (datas != null && datas.Any())
                {
                    var expect = items.Except(datas);
                    items.AddRange(expect);
                }
            }
            var diff = list.Except(items);
            items.AddRange(diff);
            return items;
        }
      
        /// <summary>
        /// 去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// 比较两个集合中的每个元素是否都一样
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <returns></returns>
        public static bool ItmesEquals<TSource>(this IEnumerable<TSource> source1, IEnumerable<TSource> source2)
        {
            return (source1.Count() == source2.Count() && !source1.Any(t => !source2.Contains(t)));
        }

        /// <summary>
        /// 判断集合中的元素是否都存在
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <returns></returns>
        public static bool ItemsContains<TSource>(this IEnumerable<TSource> source1, IEnumerable<TSource> source2)
        {
            foreach (var item in source2)
            {
                if (!source1.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
