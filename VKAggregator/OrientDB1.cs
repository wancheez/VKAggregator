using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orient;
using Orient.Client;
using OrientDB_Net.binary.Innov8tive.API;

namespace VKAggregator
{
    class OrientDB
    {
        ConnectionOptions opts = new ConnectionOptions();
        // OPEN DATABASE
        public ODatabase openDatabase()
        {
            opts.HostName = "localhost";
            opts.UserName = "root";
            opts.Password = "admin";
            opts.Port = 2424;
            opts.DatabaseName = "VK";
            opts.DatabaseType = ODatabaseType.Graph;

            // CONSOLE LOG
            Console.WriteLine("Opening Database: {0}", "VK");

            // OPEN DATABASE
            ODatabase database = new ODatabase(opts);

            // RETURN ODATABASE INSTANCE
            return database;
        }

        const string databaseAlias = "VK";
        public OrientDB()
        {
            /* OClient.CreateDatabasePool               
                 (
                  "127.0.0.1",
                  2424,
                  "VK",
                  ODatabaseType.Graph,
                  "root",
                  "admin",
                  10,
                  "VKAlias"
                  );
                  */
            openDatabase();
        }

        /// <summary>
        /// Выполнить кастомный запрос
        /// </summary>
        /// <param name="query">Запрос OrientDB</param>
        /// <param name="databaseAlias">Название базы</param>
        /// <returns></returns>
        public List<ODocument> PerformQuery(String query, String databaseAlias)
        {
            using (ODatabase database = new ODatabase(opts))
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
            using (ODatabase database = new ODatabase(opts))
            {
                foreach (VK.VKMan User in Users)
                {
                    //TODO Т.к. мы вставляем через UPSERT, то не получаем ORID. Поэтому надо наверно через select по уникальному vkid его получать.

                    Console.WriteLine("User has been added");
                    var strUpsert = String.Format("UPDATE user set first_name='{0}', last_name='{1}', sex='{2}',bday='{3}',country='{4}'," +
        "city='{5}',interests='{6}',music='{7}',movies='{8}',games='{9}',about='{10}', vkid='{11}'" +
        " UPSERT WHERE vkid='{11}'",
        User.first_name, User.last_name, User.sex, DateTimeAdapter(User.bday), User.country, User.city, User.interests,
        User.music, User.movies, User.games, "", User.id);
                    string queryString = String.Format("UPDATE user set first_name='{0}', last_name='{1}', sex='{2}',bday='{3}',country='{4}'," +
        "city='{5}',interests='{6}',music='{7}',movies='{8}',games='{9}',about='{10}', vkid='{11}'" +
        " UPSERT WHERE vkid='{11}'",
        User.first_name, User.last_name, User.sex, DateTimeAdapter(User.bday), User.country, User.city, User.interests,
        User.music, User.movies, User.games, "", User.id);

                    string queryStringSelectORID = string.Format("SELECT FROM user WHERE vkid={0}", User.id);
                    /*
                    OCommandResult commandResult = database.Command(String.Format("INSERT INTO user (first_name,last_name,sex,bday,country," +
                        "city,interests,music,movies,games,about, vkid)" +
                        "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}', '{11}')",
                        User.first_name, User.last_name, User.sex, DateTimeAdapter(User.bday), User.country, User.city, User.interests,
                        User.music, User.movies, User.games, "", User.id)
                        );
                        */
                    OCommandResult commandResult = database.Command(queryString);
                    //OCommandResult commandSelectORIDResult = database.Command(queryStringSelectORID);
                    //Переделывал работу с ORIENT CLIENT
                    //User.ORID = commandSelectORIDResult.ToDocument().GetField<ODocument>("Content").GetField<ORID>("@ORID").ToString();
                    List<ODocument> result = database.Query(queryStringSelectORID);
                    foreach (ODocument document in result)
                    {
                        User.ORID = document.ORID.ToString();
                        
                    }
                    
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
            using (ODatabase database = new ODatabase(opts))
            {
                foreach (VK.VKMan User in Users)
                {
                    /*
                    byte[] bytes = Encoding.C.GetBytes();
                    Encoding win1251 = Encoding.GetEncoding(1251);
                    string text = win1251.GetString(bytes);
                    */
                    /*
                    string queryString = String.Format("INSERT INTO user (first_name,last_name,sex,bday,country," +
                            "city,interests,music,movies,games,about)" +
                            " values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                            User.first_name, User.last_name, User.sex, DateTimeAdapter(User.bday), User.country, User.city, User.interests,
                            User.music, User.movies, User.games, "");
                            */

                    string queryString = String.Format("UPDATE user set first_name='{0}', last_name='{1}', sex='{2}',bday='{3}',country='{4}'," +
                            "city='{5}',interests='{6}',music='{7}',movies='{8}',games='{9}',about='{10}', vkid='{11}'" +
                            " UPSERT WHERE vkid='{11}'",
                            User.first_name, User.last_name, User.sex, DateTimeAdapter(User.bday), User.country, User.city, User.interests,
                            User.music, User.movies, User.games, "", User.id);

                    string queryStringSelectORID = string.Format("SELECT FROM user WHERE vkid={0}", User.id);
                    
                    try {
                        /* OCommandResult commandResult = database.Command(queryString);
                         OCommandResult commandSelectORIDResult = database.Command(queryStringSelectORID);
                         User.ORID = commandSelectORIDResult.ToDocument().GetField<ODocument>("Content").GetField<ORID>("@ORID").ToString();
                         database.Command(string.Format("CREATE EDGE FROM {0} TO {1} SET weight={2}", rootUser.ORID, User.ORID, calculateWeigth()));
                         */
                        List<ODocument> result = database.Query(queryStringSelectORID);
                        foreach (ODocument document in result)
                        {
                            User.ORID = document.ORID.ToString();

                        }
                    } catch(OException ex)
                    {
                        
                        Console.WriteLine(String.Format("There are problems with {0} {1}", User.first_name, User.last_name));
                    }
                                     
                }
                return true;
            }
        }

        public void writeRecursiveUsers(VK.VKMan rootUser, VK.VKObjects vkInstance,int currDepth, int depth)
        {
            
                using (ODatabase database = new ODatabase(opts))
                {
                    var Users = vkInstance.getFriendsListFromXML(rootUser.id);
                    Statistic.Statistic.LogVertex(depth,Users.Count);
                    foreach (VK.VKMan User in Users)
                    {
                        string queryString = String.Format("UPDATE user set first_name='{0}', last_name='{1}', sex='{2}',bday='{3}',country='{4}'," +
                                "city='{5}',interests='{6}',music='{7}',movies='{8}',games='{9}',about='{10}', vkid='{11}'" +
                                " UPSERT WHERE vkid='{11}'",
                                User.first_name, User.last_name, User.sex, DateTimeAdapter(User.bday), User.country, User.city, User.interests,
                                User.music, User.movies, User.games, "", User.id);

                        string queryStringSelectORID = string.Format("SELECT FROM user WHERE vkid={0}", User.id);

                        try
                        {
                        OCommandResult commandResult = database.Command(queryString);
                        OCommandResult commandSelectORIDResult = database.Command(queryStringSelectORID);
                        User.ORID = commandSelectORIDResult.ToDocument().GetField<ODocument>("Content").GetField<ORID>("@ORID").ToString();
                        
                        List<ODocument> result = database.Query(queryStringSelectORID);
                        foreach (ODocument document in result)
                        {
                            User.ORID = document.ORID.ToString();

                        }
                        //Логируем
                        var comupsert = string.Format("CREATE EDGE Friend FROM {0} TO {1} SET weight={2}", rootUser.ORID, User.ORID, User.rootEdgeWeigth);
                        database.Command(string.Format("CREATE EDGE Friend FROM {0} TO {1} SET weight={2}", rootUser.ORID, User.ORID, User.rootEdgeWeigth));
                        //Statistic.Statistic.LogEdge(depth,);//Логгируем
                        if (currDepth < depth)
                        {
                            currDepth++;
                            writeRecursiveUsers(User, vkInstance,currDepth, depth);
                            currDepth--;
                        }
                       }
                        catch (OException ex)
                        {
                        Statistic.Statistic.logError(ex.ToString());
                            Console.WriteLine(String.Format("There are problems with {0} {1}", User.first_name, User.last_name));
                        }

                    }
                    
                }
            
        }

        public void truncateDatabase()
        {
            
            using (ODatabase database = new ODatabase(opts))
            {
                string queryTruncateUser = string.Format("truncate class User unsafe");
                string queryTruncateFriend = string.Format("truncate class Friend unsafe");
                OCommandResult commandResult = database.Command(queryTruncateUser);
                commandResult = database.Command(queryTruncateFriend);

            }
        }

        /// <summary>
        /// Обработка даты
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string DateTimeAdapter(DateTime dateTime)
        {
            if (DateTime.Compare(dateTime, DateTime.MinValue) > 0)
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

        private string stringAdapter(string str)
        {
            return str.Replace("\n", "");
        }

        private string Win1251ToUTF8(string source)
        {
            // Create two different encodings.
            
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            Encoding utf8 = Encoding.UTF8;
            // Convert the string into a byte array.
            byte[] win1251Bytes = win1251.GetBytes(source);

            // Perform the conversion from one encoding to the other.
            byte[] utf8Bytes = Encoding.Convert(win1251, utf8, win1251Bytes);

            // Convert the new byte[] into a char[] and then into a string.
            char[] utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
            utf8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);
            string utf8String = new string(utf8Chars);


            return utf8String;

        }
    }
}
