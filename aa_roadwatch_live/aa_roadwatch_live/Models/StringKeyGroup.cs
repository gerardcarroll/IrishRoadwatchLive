using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace aa_roadwatch_live
{
    public class StringKeyGroup<T> : List<T>
    {
        public delegate String GetKeyDelegate(T item);
        public String Key { get; private set; }
        public StringKeyGroup(String key)
        {
            Key = key;
        }
        public static List<StringKeyGroup<T>> CreateGroups(IEnumerable<T> items, CultureInfo ci, GetKeyDelegate getKey, bool sort)
        {
            var list = new List<StringKeyGroup<T>>();

            foreach (var item in items)
            {
                var itemKey = getKey(item);//.Substring(0, 1).ToLower();

                var itemGroup = list.FirstOrDefault(li => li.Key == itemKey);
                var itemGroupIndex = itemGroup != null ? list.IndexOf(itemGroup) : -1;

                if (itemGroupIndex == -1)
                {
                    list.Add(new StringKeyGroup<T>(itemKey));
                    itemGroupIndex = list.Count - 1;
                }
                if (itemGroupIndex >= 0 && itemGroupIndex < list.Count)
                {
                    list[itemGroupIndex].Add(item);
                }
            }

            if (sort)
            {
                //foreach (var group in list)
                //{
                //    group.ToList().Sort((c0, c1) => ci.CompareInfo.Compare(getKey(c0).ToShortDateString(), getKey(c1).ToShortDateString()));
                //}
            }

            return list;
        }
    }
}
