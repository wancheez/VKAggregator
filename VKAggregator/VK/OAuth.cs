using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VKAggregator.VK
{
    class OAuth
    {
        private int scope = (int)(VkontakteScopeList.friends);
        public static int appId = 4201412;
        public static string token = "";//Токен будет получен при авторизации 

        //Список разрешений (для удобства используем числовые значения)
        private enum VkontakteScopeList
        {
            /// <summary>
            /// Пользователь разрешил отправлять ему уведомления. 
            /// </summary>
            notify = 1,
            /// <summary>
            /// Доступ к друзьям.
            /// </summary>
            friends = 2,
            /// <summary>
            /// Доступ к фотографиям. 
            /// </summary>
            photos = 4,
            /// <summary>
            /// Доступ к аудиозаписям. 
            /// </summary>
            audio = 8,
            /// <summary>
            /// Доступ к видеозаписям. 
            /// </summary>
            video = 16,
            /// <summary>
            /// Доступ к предложениям (устаревшие методы). 
            /// </summary>
            offers = 32,
            /// <summary>
            /// Доступ к вопросам (устаревшие методы). 
            /// </summary>
            questions = 64,
            /// <summary>
            /// Доступ к wiki-страницам. 
            /// </summary>
            pages = 128,
            /// <summary>
            /// Добавление ссылки на приложение в меню слева.
            /// </summary>
            link = 256,
            /// <summary>
            /// Доступ заметкам пользователя. 
            /// </summary>
            notes = 2048,
            /// <summary>
            /// (для Standalone-приложений) Доступ к расширенным методам работы с сообщениями. 
            /// </summary>
            messages = 4096,
            /// <summary>
            /// Доступ к обычным и расширенным методам работы со стеной. 
            /// </summary>
            wall = 8192,
            /// <summary>
            /// Доступ к документам пользователя.
            /// </summary>
            docs = 131072
        }

        public OAuth()
        {
            string vkUri = String.Format("https://api.vk.com/oauth/authorize?client_id={0}&scope={1}&display=popup&response_type=token", appId, scope);
            WebAuthPage authPage = new WebAuthPage();
            authPage.InitializeComponent();
            authPage.WebBrowserAuth.Navigate(vkUri);
            authPage.ShowDialog();
            if (token != "")
            {
                token = token.Replace("access_token=", "");
            } else
            {
                //TODO: Обработать неудачную авторизацию
            }
        }
    }
}
    