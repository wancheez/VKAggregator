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
                //TODO Надо выбирать по называнию узла, т.к. если там ниче нет, узел не передается
                //Пометка: вроде передается пустой тег, так что все норм
                VKMen.Add(new VKMan
                    (id: Convert.ToInt32(userNodes[0].InnerText), 
                    first_name: userNodes[1].InnerText,
                     last_name: userNodes[2].InnerText, 
                     sex: relateSex(userNodes[3].InnerText), 
                     bday: dateTemp,
                     country: userNodes[6]?.InnerText ?? "",
                     city: userNodes[5]?.InnerText ?? "",
                     interests: userNodes[7]?.InnerText ?? "",
                     music: userNodes[8]?.InnerText ?? "", 
                     movies: userNodes[9]?.InnerText ?? "",
                     games: userNodes[10]?.InnerText ?? "",
                     about: userNodes[11]?.InnerText ?? "",
                     rootEdgeWeigth: 0));
                     
            }
            return VKMen;
        }

        public List<VKMan> getFriendsListFromXML(int id)
        {
          
            XmlDocument XMLMen = getFriends(id,500,0,"hints", VKMan.fieldNeededList);
            List<VKMan> VKMen = new List<VKMan>();
            DateTime dateTemp;
            int temp_int;
            string UserIdList = "";
            //Перебираем users
            foreach (XmlNode userXML in XMLMen.DocumentElement.ChildNodes)
            {
                //Обработка ошибок
                if (userXML.Name != "error_code"&& userXML.Name !="error_msg" && userXML.Name != "request_params")
                {
                    XmlNodeList userNodes = userXML.ChildNodes;
                    //Обработка даты
                    try
                    {
                        dateTemp = Convert.ToDateTime(userNodes[4].InnerText);
                    }
                    catch (Exception ex)
                    {
                        dateTemp = DateTime.MinValue;
                    }

                    //TODO Надо выбирать по называнию узла, т.к. если там ниче нет, узел не передается
                    //Пометка: вроде передается пустой тег, так что все норм

                    //TODO: Падает на несозданном пользователе
                    VKMen.Add(new VKMan
                        (id: Convert.ToInt32(userNodes[0].InnerText),
                        first_name: userNodes[1].InnerText,
                         last_name: userNodes[2].InnerText,
                         sex: relateSex(userNodes[3].InnerText),
                         bday: dateTemp,
                         country: userNodes[6]?.InnerText ?? "",
                         //city: getCityById(userNodes[5].InnerText) ?? "",
                         city: "null",
                         interests: userNodes[7]?.InnerText ?? "",
                         music: userNodes[8]?.InnerText ?? "",
                         movies: userNodes[9]?.InnerText ?? "",
                         games: userNodes[10]?.InnerText ?? "",
                         about: userNodes[11]?.InnerText ?? "",
                         rootEdgeWeigth: 0
                         ));
                    UserIdList += VKMen.Last().id.ToString() + ',';
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
