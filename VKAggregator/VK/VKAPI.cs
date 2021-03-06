﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;
using System.Net;

namespace VKAggregator.VK
{
    public class VKAPI
    {
        //Идентификатор приложения Вконтаке
        protected int appId;
        protected string accessToken;


        /// <summary>
        /// Экземпляр класса VK API
        /// </summary>
        /// <param name="applicationID">ID приложения vk.com</param>
        /// <param name="userAccessToken">Token, полученные при OAuth авторизации</param>
        public VKAPI(int applicationID, string userAccessToken)
        {
            appId = applicationID;
            accessToken = userAccessToken;
        }

        public int getAppId()
        {
            return appId;
        }


        private XmlDocument ExecuteCommand(string name, NameValueCollection qs)
        {
            XmlDocument result = new XmlDocument();
            try
            {
                Console.WriteLine(String.Format("https://api.vk.com/method/{0}.xml?access_token={1}&{2}&v=5.74", name, accessToken, String.Join("&", from item in qs.AllKeys select item + "=" + qs[item])));
                result.Load(String.Format("https://api.vk.com/method/{0}.xml?access_token={1}&{2}&v=5.74", name, accessToken, String.Join("&", from item in qs.AllKeys select item + "=" + qs[item])));
            }
            catch (System.Net.WebException)
            {
                /*
                    if (MessageBox.Show("Нет подключения к интернету", "Ошибка", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
                    {
                        ExecuteCommand(name, qs);
                    }
                    else
                    {
                       Application.Exit();
                    }
                 */

            }
            return result;
        }



        private XmlDocument ExecuteWOTokenCommand(string name, NameValueCollection qs)
        {
            XmlDocument result = new XmlDocument();
            result.Load(String.Format("https://api.vk.com/method/{0}.xml?{1}", name, String.Join("&", from item in qs.AllKeys select item + "=" + qs[item])));

            return result;
        }

        /// <summary>
        /// Возвращает расширенную информацию о пользователе
        /// </summary>
        /// <param name="uid">перечисленные через запятую идентификаторы пользователей или их короткие имена (screen_name). По умолчанию — идентификатор текущего пользователя. </param>
        /// <param name="fields">Доступные значения: sex, bdate, city, country, photo_50, photo_100, photo_200_orig, photo_200, photo_400_orig, photo_max, photo_max_orig, photo_id, online, online_mobile, domain, has_mobile, contacts, connections, site, education, universities, schools, can_post, can_see_all_posts, can_see_audio, can_write_private_message, status, last_seen, common_count, relation, relatives, counters, screen_name, maiden_name, timezone, occupation,activities, interests, music, movies, tv, books, games, about, quotes </param>
        /// <returns></returns>
        public XmlDocument getProfile(int uid = 1, string fields = "career")
        {
            NameValueCollection qs = new NameValueCollection();
            if (uid != 1)
                qs["user_ids"] = uid.ToString();
            if (fields != "")
                qs["fields"] = fields;
            return ExecuteCommand("users.get", qs);
        }

        /// <summary>
        /// Возвращает расширенную информацию о множестве пользователей
        /// </summary>
        /// <param name="fields">Нужные поля</param>
        /// <param name="uid">Массив или перечисление id пользователей</param>
        /// <returns></returns>
        public XmlDocument getProfiles(string fields = "", params int[] uids)
        {
            NameValueCollection qs = new NameValueCollection();

            foreach (int value in uids)
            {
                qs["user_ids"] += value.ToString() + ',';
            }

            //Удаляем последнюю лишнюю запятую
            qs["user_ids"] = qs["user_ids"].Substring(0, qs["user_ids"].Length - 1);

            if (fields != "")
                qs["fields"] = fields;
            return ExecuteCommand("users.get", qs);
        }



        /// <summary>
        /// Получить последние личные сообщения
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="needOutMessages">Нужны исходящие сообщения</param>
        /// <param name="count">Количество сообщений</param>
        /// <returns></returns>
        public XmlDocument getLastMessages(int uid, bool needOutMessages, int count)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["uid"] = uid.ToString();
            if (needOutMessages)
                qs["out"] = "1";
            else qs["out"] = "0";
            qs["offset"] = "";
            qs["count"] = count.ToString();
            qs["time_offset"] = "";
            qs["filters"] = "";
            qs["preview_length"] = "";
            qs["last_message_id"] = "";
            return ExecuteCommand("messages.get", qs);
        }


        /// <summary>
        /// Получить аудиозаписи
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="count">Количество</param>
        /// <param name="offset">Смещение относительно начала</param>
        /// <returns></returns>
        public XmlDocument getAudio(int uid, int count, int offset)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["uid"] = uid.ToString();
            qs["offset"] = offset.ToString();//Смещение относительно начала списка аудиозаписей
            qs["count"] = count.ToString(); //количество аудиозаписей, информацию о которых необходимо вернуть. Максимальное значение — 6000. 
            qs["need_user"] = "0"; //1 — возвращать информацию о пользователях, загрузивших аудиозапись. 
            return ExecuteCommand("audio.get", qs);
        }

        /// <summary>
        /// Возвращает список аудиозаписей в соответствии с заданным критерием поиска.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="count">количество аудиозаписей, информацию о которых необходимо вернуть.</param>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножетсва аудиозаписей. По умолчанию — 0. </param>
        /// <param name="query">текст поискового запроса, например, The Beatles.</param>
        /// <param name="autoComplete">Если этот параметр равен 1, возможные ошибки в поисковом запросе будут исправлены.</param>
        /// <param name="sort">Вид сортировки. 2 — по популярности, 1 — по длительности аудиозаписи, 0 — по дате добавления.</param>
        /// <returns></returns>
        public XmlDocument getAudioGlobal(int uid, int count, int offset, string query, bool autoComplete = true, int sort = 2)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["uid"] = uid.ToString();
            qs["offset"] = offset.ToString();//Смещение относительно начала списка аудиозаписей
            qs["count"] = count.ToString(); //количество аудиозаписей, информацию о которых необходимо вернуть. Максимальное значение — 6000. 
            qs["q"] = query;
            if (sort > -1 && sort < 3)
                qs["sort"] = sort.ToString();
            if (autoComplete)
                qs["autoComplete"] = 1.ToString();
            else
                qs["autoComplete"] = 1.ToString();
            return ExecuteCommand("audio.search", qs);
        }

        /// <summary>
        /// Получить посты, на которые были поставлены отметки "Мне нравится"
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public XmlDocument getFavePosts(int uid, int offset, int count)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["uid"] = uid.ToString();
            qs["offset"] = offset.ToString();
            qs["count"] = count.ToString();

            return ExecuteCommand("fave.getPosts", qs);
        }

        /// <summary>
        /// Возвращает список идентификаторов друзей пользователя или расширенную информацию о друзьях пользователя (при использовании параметра fields).
        /// </summary>
        /// <param name="uid">id владельца</param>
        /// <param name="count">Количество выдаваемых записей</param>
        /// <param name="offset">Смещение относительно начала</param>
        /// <param name="fields">список дополнительных полей, которые необходимо вернуть.Доступные значения
        /// : nickname, domain, sex, bdate, city, country, timezone, photo_50, photo_100, photo_200_orig, has_mobile,
        ///  contacts, education, online, relation, last_seen, status, can_write_private_message, can_see_all_posts, can_post, universities список строк, разделенных через запятую </param>
        /// <param name="order">Порядок, в котором нужно вернуть список друзей: name - сортировать по имени,hints - сортировать по рейтингу, random - случайно</param>
        /// <returns></returns>
        public XmlDocument getFriends(int uid, int count, int offset, string order = "hints", string fields = "")
        {
            NameValueCollection qs = new NameValueCollection();
            qs["user_id"] = uid.ToString();
            qs["offset"] = offset.ToString();
            qs["count"] = count.ToString();
            if (fields != "")
            {
                qs["order"] = order;
                qs["fields"] = fields;
            }
            if(ExecuteCommand("friends.get", qs).Attributes is null)
            {
                var result = ExecuteCommand("friends.get", qs) ;
                Console.Write("null");
            }
            // Проверка на возврат ошибки

            return ExecuteCommand("friends.get", qs);
        }
        /// <summary>
        /// Возвращает результаты поиска по статусам. Новости возвращаются в порядке от более новых к более старым.
        /// </summary>        
        /// <param name="count">Количество выдаваемых записей</param>
        /// <param name="offset">Смещение относительно начала</param>
        /// <param name="query">поисковой запрос, например, "New Year"</param>
        /// <param name="extended">указывается true, если необходимо получить информацию о пользователе или группе, разместившей запись. По умолчанию false</param>
        /// <param name="excludeWords">исключить из выборки слова. Слова, которые нужно исключить, перечисляются через запятую</param>
        /// <param name="hasAttachments">список прикреплений у постов через запятую photo,audio,video</param>
        /// <param name="likes">количество отметок "мне нравится"</param>
        /// <param name="latitude">географическая широта точки, в радиусе от которой необходимо производить поиск, заданная в градусах (от -90 до 90).</param>
        /// <param name="longitude">географическая долгота точки, в радиусе от которой необходимо производить поиск, заданная в градусах (от -180 до 180). </param>
        /// <param name="fields">список дополнительных полей профилей, которые необходимо вернуть. См. подробное описание. Доступные значения: sex, bdate, city, country, photo_50, photo_100, photo_200_orig, photo_200, photo_400_orig, photo_max, photo_max_orig, online, online_mobile, domain, has_mobile, contacts, connections, site, education, universities, schools, can_post, can_see_all_posts, can_see_audio, can_write_private_message, status, last_seen, common_count, relation, relatives, screen_name, maiden_name, timezone, occupation,activities, interests, music, movies, tv, books, games, about, quotes</param>
        /// <returns>Возвращает XML-документ</returns>
        public XmlDocument searchPost(int count, int offset, string query = "", bool extended = false, string hasAttachments = "",
            int likes = 0, string excludeWords = "", string fields = "", double latitude = 0, double longitude = 0)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["offset"] = offset.ToString();
            qs["count"] = count.ToString();

            qs["q"] = query;

            if (likes > 0)
            {
                qs["q"] += " likes:" + likes.ToString();
            }
            excludeWords.Replace(" ", "");
            string[] excludeWordsArray = excludeWords.Split(',');
            foreach (string word in excludeWordsArray)
            {
                qs["q"] += " -" + word;
            }
            if (hasAttachments != "")
            {
                qs["q"] += " has:";
                hasAttachments.Replace(" ", "");
                string[] hasAttachmentsArray = hasAttachments.Split(',');
                foreach (string word in hasAttachmentsArray)
                {
                    qs["q"] += word + " ";
                }
            }
            if (extended)
            {
                qs["extended"] = 1.ToString();
            }

            if (fields != "")
            {
                qs["fields"] = fields;
            }
            if (longitude != 0)
            {
                qs["longitude"] = longitude.ToString();
            }
            if (latitude != 0)
            {
                qs["latitude"] = latitude.ToString();
            }

            return ExecuteCommand("newsfeed.search", qs);
        }

        /// <summary>
        /// Публикует новую запись на своей или чужой стене.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="owner_id">идентификатор пользователя или сообщества, на стене которого должна быть опубликована запись.</param>
        /// <param name="message">текст сообщения (является обязательным, если не задан параметр attachments) </param>
        /// <returns></returns>
        public XmlDocument wallPost(int uid, int owner_id, string message)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["uid"] = uid.ToString();
            qs["owner_id"] = owner_id.ToString();
            qs["message"] = message;

            return ExecuteCommand("wall.post", qs);
        }

        public XmlDocument getWallUploadServer(string group_id)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["group_id"] = group_id;


            return ExecuteCommand("photos.getWallUploadServer", qs);
        }

        /// <summary>
        /// Возвращает список аудиозаписей из раздела "Популярное"
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="count">количество возвращаемых аудиозаписей</param>
        /// <param name="offset">смещение, необходимое для выборки определенного подмножества аудиозаписей</param>
        /// <param name="only_eng">false – возвращать только зарубежные аудиозаписи. true – возвращать все аудиозаписи</param>
        /// <returns></returns>
        public XmlDocument audioGetPopular(int uid, int count, int offset, bool only_eng = false)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["uid"] = uid.ToString();
            qs["offset"] = offset.ToString();//Смещение относительно начала списка аудиозаписей
            qs["count"] = count.ToString(); //количество аудиозаписей, информацию о которых необходимо вернуть. Максимальное значение — 6000. 
            qs["only_eng"] = only_eng ? "1" : "0";

            return ExecuteCommand("audio.getPopular", qs);
        }

        /// <summary>
        /// Возвращает список рекомендуемых аудиозаписей на основе списка воспроизведения заданного пользователя или на основе одной выбранной аудиозаписи
        /// </summary>
        /// <param name="uid">идентификатор пользователя для получения списка рекомендаций на основе его набора аудиозаписей</param>
        /// <param name="count">количество возвращаемых аудиозаписей</param>
        /// <param name="offset">смещение относительно первой найденной аудиозаписи для выборки определенного подмножества</param>
        /// <param name="shuffle">включен случайный порядок</param>
        /// <returns></returns>
        public XmlDocument audiogetRecommendations(int uid, int count, int offset, bool shuffle = false)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["user_id"] = uid.ToString();
            qs["offset"] = offset.ToString();//Смещение относительно начала списка аудиозаписей
            qs["count"] = count.ToString(); //количество аудиозаписей, информацию о которых необходимо вернуть. Максимальное значение — 6000. 
            qs["shuffle"] = shuffle ? "1" : "0";

            return ExecuteCommand("audio.getRecommendations", qs);
        }

        public XmlDocument getCityById(int cityId)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["city_ids"] = cityId.ToString();
            return ExecuteWOTokenCommand("database.getCitiesById", qs);
        }

        /// <summary>
        /// Возвращает список идентификаторов общих друзей между парой пользователей.
        /// </summary>
        /// <param name="source_uid">идентификатор пользователя, чьи друзья пересекаются с друзьями пользователя
        ///  с идентификатором target_uid. Если параметр не задан, то считается, что source_uid равен идентификатору текущего пользователя</param>
        /// <param name="target_uid">идентификатор пользователя, с которым необходимо искать общих друзей</param>
        /// <param name="target_uids">список идентификаторов пользователей, с которыми необходимо искать общих друзей</param>
        /// <returns></returns>
        public XmlDocument firendsGetMutual(int source_uid, int? target_uid, string target_uids)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["source_uid"] = source_uid.ToString();
            if (target_uid != null)
            {
                qs["target_uid"] = target_uids;//Смещение относительно начала списка аудиозаписей
            }
            else
            {
                qs["target_uids"] = target_uids.ToString(); //количество аудиозаписей, информацию о которых необходимо вернуть. Максимальное значение — 6000. 
            }

            return ExecuteCommand("friends.getMutual", qs);
        }


        /// <summary>
        /// Возвращает список сообществ указанного пользователя. JSON!!!!!
        /// </summary>
        /// <param name="user_id">идентификатор пользователя, информацию о сообществах которого требуется получить. </param>
        /// <param name="fields">список дополнительных полей, которые необходимо вернуть. Возможные значения: city, country, place, description, wiki_page, members_count, counters, start_date, finish_date, can_post, can_see_all_posts, activity, status, contacts, links, fixed_post, verified, site, can_create_topic</param>
        /// <param name="count">количество сообществ, информацию о которых нужно вернуть</param>
        /// <returns></returns>
        public List<VKGroup> getUserGroups(int user_id, string fields, int count=1000)
        {
            NameValueCollection qs = new NameValueCollection();
            qs["user_id"] = user_id.ToString();
            if (fields.ToString().Equals(""))
            {
                qs["fields"] = "city";//Смещение относительно начала списка аудиозаписей
            }
            else
            {
                qs["fields"] = fields.ToString(); //количество аудиозаписей, информацию о которых необходимо вернуть. Максимальное значение — 6000. 
            }

            qs["count"] = count.ToString();
            qs["extended"] = "1";
            //Используем JSON и сразу возвращаем объект группы

            //return ExecuteCommand("groups.get", qs);
            //return (List<VKGroup>) ExecuteCommandJson <List<VKGroup>>("groups.get", qs);

            var url = String.Format("https://api.vk.com/method/{0}?access_token={1}&{2}&v=5.74", "groups.get", accessToken, String.Join("&", from item in qs.AllKeys select item + "=" + qs[item]));
            var test1 = new WebClient().DownloadString(url);
            ;
            

            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
               System.Threading.Thread.Sleep(400);

                var result = JsonConvert.DeserializeObject<Data>(webClient.DownloadString(url));
                if (result.response is null) {
                    Statistic.Statistic.logError("Error getting groups for id:" + qs["user_id"] + " err_code:" + result.error.error_code + " err_msg: " + result.error.error_msg);
                    return new List<VKGroup>();
                } else
                    return result.response.items.OfType<VKGroup>().ToList();
            }
            
        }

        public Object ExecuteCommandJson<T>(string method, NameValueCollection qs)
        {
            var url = String.Format("https://api.vk.com/method/{0}?access_token={1}&{2}&v=5.74", method, accessToken, String.Join("&", from item in qs.AllKeys select item + "=" + qs[item]));

            string var1 = new WebClient().DownloadString(url);

            var parsed = JsonConvert.DeserializeObject<T>(new WebClient().DownloadString(url));
            return parsed;
        }
    }


}

