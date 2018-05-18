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
            if (vkman.sex.Contains(condition))
            {
                return true;
            }
            else return false;
            
        }

        public static Boolean FilterCity(VK.VKMan vkman, string condition)
        {
            if (vkman.city.Contains(condition))
            {
                return true;
            }
            else return false;
        }

        public static Boolean FilterInterests(VK.VKMan vkman, string condition)
        {
            if (vkman.interests.Contains(condition))
            {
                return true;
            }
            else return false;
        }

        public static Boolean FilterGroups(VK.VKMan vkman, string condition)
        {
            if (vkman.city.Contains(condition))
            {
                return true;
            }
            else return false;
        }

        public static Boolean FilterUni(VK.VKMan vkman, string condition)
        {
            if (vkman.university_name.Contains(condition))
            {
                return true;
            }
            else return false;
        }
    }
}
