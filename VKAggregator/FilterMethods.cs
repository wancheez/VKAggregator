using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAggregator
{
    public class FilterMethods
    {
        public static Boolean FilterSex(VK.VKMan vkman, string condition)
        {
            Console.WriteLine("Делегат 1 выполнен");
            if (vkman.sex.Contains(condition))
            {
                return true;
            }
            else return false;
            
        }

        public static Boolean FilterCity(VK.VKMan vkman, string condition)
        {
            Console.WriteLine("Делегат 1 выполнен");
            if (vkman.city.Contains(condition))
            {
                return true;
            }
            else return false;
        }

        public static Boolean FilterInterests(VK.VKMan vkman, string condition)
        {
            Console.WriteLine("Делегат 1 выполнен");
            if (vkman.interests.Contains(condition))
            {
                return true;
            }
            else return false;
        }

        public static Boolean FilterGroups(VK.VKMan vkman, string condition)
        {
            Console.WriteLine("Делегат 1 выполнен");
            if (vkman.city.Contains(condition))
            {
                return true;
            }
            else return false;
        }
    }
}
