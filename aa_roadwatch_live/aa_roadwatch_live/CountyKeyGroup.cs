using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aa_roadwatch_live
{
    public class CountyKeyGroup<T> : List<T>
    {
        public delegate String GetKeyDelegate(T item);
        public String Key { get; private set; }
        public CountyKeyGroup(String key)
        {
            Key = key;
        }
        public static List<CountyKeyGroup<T>> CreateGroups(IEnumerable<T> items, CultureInfo ci, GetKeyDelegate getKey, bool sort)
        {
            var list = new List<CountyKeyGroup<T>>();

            foreach (var item in items)
            {
                var itemKey = getKey(item);//.Substring(0, 1).ToLower();

                var itemGroup = list.FirstOrDefault(li => li.Key == itemKey);
                var itemGroupIndex = itemGroup != null ? list.IndexOf(itemGroup) : -1;

                if (itemGroupIndex == -1)
                {
                    list.Add(new CountyKeyGroup<T>(itemKey));
                    itemGroupIndex = list.Count - 1;
                }
                if (itemGroupIndex >= 0 && itemGroupIndex < list.Count)
                {
                    list[itemGroupIndex].Add(item);
                }
            }

            if (sort)
            {
                foreach (var group in list)
                {
                    group.ToList().Sort((c0, c1) => ci.CompareInfo.Compare(getKey(c0), getKey(c1)));
                }
            }

            return list;
        }
    }
}
