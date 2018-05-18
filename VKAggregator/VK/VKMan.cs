using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAggregator.VK
{
    /// <summary>
    /// Класс представляет собой описание данных пользователя ВК
    /// </summary>
    public class VKMan
    {
        //Список полей, нужных объекту
        public const string fieldNeededList = "sex,bdate,country,city,interests,music," +
            "movies,games,about,education";

        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string sex { get; set; }
        public DateTime bday { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string relatives { get; set; }
        public string interests { get; set; }
        public string music { get; set; }
        public string university_name { get; set; }
        public string faculty_name { get; set; }
        public string movies { get; set; }
        public string games { get; set; }
        public string about { get; set; }
        public string ORID { get; set; }
        public int rootEdgeWeigth { get; set; }
        public List<VKGroup> userGroups { get; set; }

        public VKMan(int id, string first_name, string last_name, string sex, DateTime bday, string country, string city,
        string interests, string music, string movies, string games, string university_name, string faculty_name,
        string about, int rootEdgeWeigth, List<VKGroup> userGroups)
        {
            this.id = id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.sex = sex;
            this.bday = bday;
            this.country = country;
            this.city = city;
            //this.relatives = relatives;
            this.interests = interests;
            this.music = music;
            this.movies = movies;
            this.faculty_name = faculty_name;
            this.university_name = university_name;
            this.games = games;
            this.about = about;
            this.ORID = "";
            this.rootEdgeWeigth = 0;//Вес ребра от пользователя, чьим другом является
            this.userGroups = userGroups;
        }

    }

    //City or Country
    struct place
    {
        int id;
        string title;
    }
}
