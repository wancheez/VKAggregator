using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orient;
using Orient.Client;

namespace VKAggregator
{
    class OrientDB
    {
        const string databaseAlias = "VK";
        public OrientDB()
        {
            OClient.CreateDatabasePool(
                 "localhost",
                 2424,
                 "VK",
                 ODatabaseType.Graph,
                 "root",
                 "admin",
                 100,
                 databaseAlias
             );
        }


        /// <summary>
        /// Выполнить кастомный запрос
        /// </summary>
        /// <param name="query">Запрос OrientDB</param>
        /// <param name="databaseAlias">Название базы</param>
        /// <returns></returns>
        public List<ODocument> PerformQuery(String query, String databaseAlias)
        {
            using (ODatabase database = new ODatabase(databaseAlias))
            {
                List<ODocument> result = database.Query("query");
                /* foreach (ODocument document in result)
                {
                    textBlock.Text += "\n" +

                        document.ORID.ToString() + ' ' +
                        document.OClassName + ' ' +
                        document.GetField<string>("name");


                }
                */
                return result;
            }
        }

        /// <summary>
        /// Записать в базу список пользователей (без связей)
        /// </summary>
        /// <param name="Users"></param>
        /// <returns></returns>
        public bool WriteVKUsers(List<VK.VKMan> Users)
        {
            using (ODatabase database = new ODatabase(databaseAlias))
            {
                foreach (VK.VKMan User in Users)
                {
                    Console.WriteLine("User has been added");
                    OCommandResult commandResult = database.Command(String.Format("INSERT INTO user (first_name,last_name,sex,bday,country," +
                        "city,interests,music,movies,games,about)" +
                        "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                        User.first_name, User.last_name, User.sex, DateTimeAdapter(User.bday), User.country, User.city, User.interests,
                        User.music, User.movies, User.games, User.about)
                        );
                    User.ORID = commandResult.ToDocument().ORID.ToString();
                }
                return true;
            }
        }

        /// <summary>
        /// Записать в базу друзей пользователя
        /// </summary>
        /// <param name="rootUser">Главнвый пользователь</param>
        /// <param name="Users">Его друзья</param>
        /// <returns></returns>
        public bool WriteVKUserFriends(VK.VKMan rootUser, List<VK.VKMan> Users)
        {
            using (ODatabase database = new ODatabase(databaseAlias))
            {
                foreach (VK.VKMan User in Users)
                {
                    Console.WriteLine("User has been added");
                    OCommandResult commandResult = database.Command(String.Format("INSERT INTO user (first_name,last_name,sex,bday,country," +
                        "city,interests,music,movies,games,about)" +
                        "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                        User.first_name, User.last_name, User.sex, "1900-10-10 00:00:00", User.country, User.city, User.interests,
                        User.music, User.movies, User.games, User.about)
                        );
                    var test = commandResult.ToDocument()["ORID"];
                    User.ORID = commandResult.ToDocument().ORID.ToString();
                    database.Command(string.Format("CREATE EDGE FROM {0} TO {1} SET weight={2}", rootUser.ORID, User.ORID, calculateWeigth()));
                    
                    var i = 0;
                }
                return true;
            }
        }

        /// <summary>
        /// Обработка даты
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string DateTimeAdapter(DateTime dateTime)
        {
            if (DateTime.Compare(dateTime, DateTime.MinValue) >= 0)
            {
                return (dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            return "1900-10-10 00:00:00";
        }

        /// <summary>
        /// Нетривиальная функция для вычисления веса связи
        /// </summary>
        /// <returns></returns>
        private double calculateWeigth()
        {
            return 0;
        }

    }
}
