using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAggregator.VK
{
    public class VKGroup
    {
        //Список полей, нужных объекту
        public const string fieldNeededList = "";

        public int id { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public int is_closed { get; set; }
        public string type { get; set; }
        public int is_admin { get; set; }
        public int is_member { get; set; }

        public VKGroup(int id, string name, string screen_name, int is_closed, string type, int is_admin, int is_member)
        {
            this.id = id;
            this.name = name;
            this.screen_name = screen_name;
            this.is_closed = is_closed;
            this.type = type;
            this.is_admin = is_admin;
            this.is_member = is_member;
        }
    }

   public class response
    {
        public int count;
        public VKGroup[] items;
    }
    public class error
    {
        public int error_code;
        public string error_msg;
    }


    class Data
    {
        public response response;
        public error error;
    }



}
