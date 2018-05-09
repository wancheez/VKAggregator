using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace VKAggregator
{
    /// <summary>
    /// Логика взаимодействия для WebAuthPage.xaml
    /// </summary>
    public partial class WebAuthPage : Window
    {
        public WebAuthPage()
        {
            InitializeComponent();
        }

        private void WebBrowserAuth_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void WebBrowserAuth_LoadCompleted1(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //TODO
            //VK.OAuth parentAuth = Owner;
        }

        /// <summary>
        /// Изменение размера окна авторизации для полного отображения содержимого (темная магия)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowserAuth_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try {
                WebBrowserAuth.Height = (int)(this.WebBrowserAuth.Document as dynamic).body.scrollHeight + 20;
                WebBrowserAuth.Width = (int)(((this.WebBrowserAuth.Document as dynamic).body.scrollWidth + 20)/2);
                this.Width = WebBrowserAuth.Width;
                this.Height = WebBrowserAuth.Height + 30;
            }
            catch
            {

            }
            if (findToken(e.Uri))
            {
                this.Close();
            }

        }

        private bool findToken(Uri uri)
        {
            Regex regexToken = new Regex("access_token=\\w*");
            if (regexToken.IsMatch(uri.ToString())){
                VK.OAuth.token = regexToken.Match(uri.ToString()).Value;
                return true;
            }
            return false;
        }
    }
}
