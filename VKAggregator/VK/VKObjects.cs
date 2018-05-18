using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace VKAggregator.VK
{
    class VKObjects : VKAPI
    {
        public VKObjects(int applicationID, string userAccessToken) :
            base(applicationID, userAccessToken)
        {
            this.appId = applicationID;
            this.accessToken = userAccessToken;
        }

        /// <summary>
        /// Возвращает список инфы о пользователях ВК по ID
        /// </summary>
        /// <param name="id">Id пользователей (массивом или перечислением)</param>
        /// <returns></returns>
        public List<VKMan> getManListFromXML(params int[] id)
        {
            XmlDocument XMLMen = getProfiles(VKMan.fieldNeededList, id);
            List<VKMan> VKMen = new List<VKMan>();
            DateTime dateTemp;
           
            //Перебираем users
            foreach(XmlNode userXML in XMLMen.DocumentElement.ChildNodes)
            {
                XmlNodeList userNodes = userXML.ChildNodes;
                //Обработка даты
                try
                {
                    dateTemp = Convert.ToDateTime(userNodes[4].InnerText);
                }
                catch (Exception ex)
                {
                    dateTemp = Convert.ToDateTime("01.01.0001");
                }
                //Группы перезгружают API
                //List<VKGroup> UserGroups = getUserGroups(Convert.ToInt32(userNodes[0].InnerText), "city");
                List<VKGroup> UserGroups = new List<VKGroup>();

                //TODO Надо выбирать по называнию узла, т.к. если там ниче нет, узел не передается
                //Пометка: вроде передается пустой тег, так что все норм
                var test1 = userXML.SelectSingleNode("university_name").InnerText;
                var tst = ("asdfalskdf'dsfa").Replace("'", "");
                VKMen.Add(new VKMan
                           (id: Convert.ToInt32(userXML.SelectSingleNode("id")?.InnerText),
                           first_name: (userXML.SelectSingleNode("first_name")?.InnerText).Replace("'",""),
                            last_name: (userXML.SelectSingleNode("last_name")?.InnerText).Replace("'", ""),
                            sex: relateSex(userXML.SelectSingleNode("sex")?.InnerText),
                            bday: dateTemp,
                            country: (userXML.SelectSingleNode("country")?.InnerText).Replace("'", "") ?? "",
                            //city: getCityById(userNodes[5].InnerText) ?? "",
                            city: "null",
                            interests: (userXML.SelectSingleNode("interests")?.InnerText).Replace("'", "") ?? "",
                            music: (userXML.SelectSingleNode("music")?.InnerText).Replace("'", "") ?? "",
                            movies: (userXML.SelectSingleNode("movies")?.InnerText).Replace("'", "") ?? "",
                            games: (userXML.SelectSingleNode("games")?.InnerText).Replace("'", "") ?? "",
                            university_name: (userXML.SelectSingleNode("university_name")?.InnerText).Replace("'", "") ?? "",
                            faculty_name: (userXML.SelectSingleNode("faculty_name")?.InnerText).Replace("'", "") ?? "",
                            about: (userXML.SelectSingleNode("about")?.InnerText).Replace("'", "") ?? "",
                            rootEdgeWeigth: 0,
                            userGroups: UserGroups
                            ));
            }
            return VKMen;
        }

        public List<VKMan> getFriendsListFromXML(int id, List<MainWindow.conditionArr> listConditions)
        {
          
            XmlDocument XMLMen = getFriends(id,500,0,"hints", VKMan.fieldNeededList);
            List<VKMan> VKMen = new List<VKMan>();
            DateTime dateTemp;
            int temp_int;
            string UserIdList = "";
            //Перебираем users
            foreach (XmlNode userXML in XMLMen.DocumentElement?.ChildNodes[1]?.ChildNodes)
            {
                //Обработка ошибок
                if (userXML.Name != "error_code"&& userXML.Name !="error_msg" && userXML.Name != "request_params" && userXML.InnerText!= "Too many requests per second")
                {
                    XmlNodeList userNodes = userXML.ChildNodes;
                    //Обработка даты

                    //dateTemp = Convert.ToDateTime(userNodes[4].InnerText);
                    var bdate = userXML.SelectSingleNode("bdate")?.LastChild.Value;
                    try
                    {
                        
                        dateTemp = (bdate is null) ? DateTime.MinValue : Convert.ToDateTime(bdate);
                    } catch
                    {
                        try
                        {
                            dateTemp = DateTime.ParseExact(bdate + ".2999", "dd.m.yyyy", System.Globalization.CultureInfo.InvariantCulture); //Convert.ToDateTime(bdate + ".2999");
                        }
                        catch { dateTemp = DateTime.MinValue; }
                    }
                        
                    
                  
                    //TODO Надо выбирать по называнию узла, т.к. если там ниче нет, узел не передается
                    //Пометка: вроде передается пустой тег, так что все норм
                    //List<VKGroup> UserGroups = getUserGroups(Convert.ToInt32(userNodes[0].InnerText), "city");
                    List<VKGroup> UserGroups = new List<VKGroup>();

                    //Отсеиваем удаленных
                    char[] charsToTrim = { '*', ' ', '\'' };

                    //if (userXML.SelectSingleNode("first_name")?.InnerText != "DELETED" && userXML.SelectSingleNode("first_name")?.InnerText != "BANNED")
                    if (userXML.SelectSingleNode("deactivated") is null)
                    {
                        VKMan vkman_tmp =  new VKMan
                           (id: Convert.ToInt32(userXML.SelectSingleNode("id")?.InnerText),
                           first_name: (userXML.SelectSingleNode("first_name")?.InnerText).Replace("'", ""),
                            last_name: (userXML.SelectSingleNode("last_name")?.InnerText).Replace("'", ""),
                            sex: relateSex(userXML.SelectSingleNode("sex")?.InnerText),
                            bday: dateTemp,
                            country: (userXML.SelectSingleNode("country")?.InnerText)?.Replace("'", "") ?? "",
                            //city: getCityById(userNodes[5].InnerText) ?? "",
                            city: "null",
                            interests: (userXML.SelectSingleNode("interests")?.InnerText)?.Replace("'", "") ?? "",
                            music: (userXML.SelectSingleNode("music")?.InnerText)?.Replace("'", "") ?? "",
                            movies: (userXML.SelectSingleNode("movies")?.InnerText)?.Replace("'", "") ?? "",
                            games: (userXML.SelectSingleNode("games")?.InnerText)?.Replace("'", "") ?? "",
                            university_name: (userXML.SelectSingleNode("university_name")?.InnerText)?.Replace("'", "") ?? "",
                            faculty_name: (userXML.SelectSingleNode("faculty_name")?.InnerText)?.Replace("'", "") ?? "",
                            about: (userXML.SelectSingleNode("about")?.InnerText)?.Replace("'", "") ?? "",
                            rootEdgeWeigth: 0,
                            userGroups: UserGroups
                            );

                        //Проверяем все фильтры
                        Boolean condResult = true;
                        foreach(MainWindow.conditionArr codition in listConditions)
                        {
                            condResult = codition.delegate_(vkman_tmp, codition.condition_);
                        }
                        if (condResult)
                        {
                            VKMen.Add(vkman_tmp);
                            UserIdList += VKMen.Last().id.ToString() + ',';
                        }
                    }
                } else
                {
                    Console.WriteLine(userXML.InnerText);
                }
            }
            //TODO: ошибка при большом (?) количестве друзей
            try {
                //Теперь задаем количество общих друзей. Для этого собрали ранее список id
                UserIdList = UserIdList.Substring(0, UserIdList.Length - 1);
                XmlDocument XMLMutualFriends = firendsGetMutual(id, null, UserIdList);
                foreach (XmlNode mutualFriends in XMLMutualFriends.DocumentElement.ChildNodes)
                {
                    XmlNodeList mutualNodes = mutualFriends.ChildNodes;
                    int.TryParse(mutualNodes[2].InnerText, out temp_int);
                    
                    VKMen.Find(x => x.id.ToString() == mutualNodes[0].InnerText).rootEdgeWeigth = temp_int;
                }
            } catch(Exception ex)
            {
                Console.WriteLine("Ошибка веса графа");
            }


            //Группы пользователей очень сильно грузят api
            /*foreach(VKMan vkmen in VKMen)
            {
                vkmen.userGroups = getUserGroups(Convert.ToInt32(vkmen.id), "city");
            }
            */
            return VKMen;
        }

        //TODO Сделать соответствие айдишники городов названиям
        private string getCityById(string id_str)
        {
            int id;
            XmlDocument vkApiAnswer = getCityById(1);
            var test = vkApiAnswer.DocumentElement.ChildNodes;
            if (int.TryParse(id_str, out id)&&id_str!="0")
            {                
                return getCityById(id).DocumentElement.ChildNodes[0].ChildNodes[1].InnerText;
            }
            else
            {
                return "Не задано";
            }
            
        }

        private string relateSex(string sexId)
        {
            try {
                switch (sexId)
                {
                    case "1": return "Женский";
                    case "2": return "Мужской";
                    default: return "Не указан";
                }
            }
            catch (NullReferenceException)
            {
                return "Не указан";
            }
        }

        /// <summary>
        /// Проверка на NULL, в случае NULL возвращает пусто
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string checkNullString(string str)
        {
            return String.IsNullOrEmpty(str) ? "" : str;
        }
    }
}
