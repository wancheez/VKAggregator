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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Orient;
using Orient.Client;
using VKAggregator.VK;
using System.Diagnostics;
using System.Threading;

namespace VKAggregator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Объявляем список делегатов, чтоб туда добавлять функции фильтрации
        public delegate Boolean condDelegate(VK.VKMan vkman, string condition);

        public class conditionArr
        {
            public condDelegate delegate_ ;
            public string condition_;

            public conditionArr(condDelegate delegate_, string condition_)
            {
                this.delegate_ = delegate_;
                this.condition_ = condition_;
            }
        }

        public List<conditionArr> listConditions = new List<conditionArr>();

        public MainWindow()
        {
            InitializeComponent();
            OAuth authVK = new OAuth();
            Statistic.Statistic.CountEdgeChanged += new EventHandler(updateStatsView);
        }

        public void startCollecting(int rootUserID, int depth, int max)
        {

            TimeLabel.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                State.Text = "Сборка Начата";
            }));
            string elapsedTime;
            (new System.Threading.Thread(delegate () {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                
                
                Statistic.Statistic.newLayer(depth);
                Statistic.Statistic.logBegin();

                VKAPI vkAPI = new VKAPI(OAuth.appId, OAuth.token);
                VKObjects MenObjects = new VKObjects(OAuth.appId, OAuth.token);

                OrientDB orientDBAccessPoint = new OrientDB();
                orientDBAccessPoint.truncateDatabase(); //Очищаем базу
                //TODO написать создание edge и рекурсивную функцию с задаваемой глубиной для ввода друзей

                var rootUser = MenObjects.getManListFromXML(rootUserID);
                orientDBAccessPoint.WriteVKUsers(rootUser);
                Statistic.Statistic.LogVertex(0, rootUser.Count);
                Statistic.Statistic.LogEdge(0, rootUser.Count);
                //orientDBAccessPoint.WriteVKUserFriends(rootUser[0], MenObjects.getFriendsListFromXML(22920004));
                //Первичная входная точка для рекурсии
                orientDBAccessPoint.writeRecursiveUsers(rootUser.First(), MenObjects, 0, depth, listConditions);
               
                TimeSpan ts = stopWatch.Elapsed;

                elapsedTime = ts.ToString("mm\\:ss\\.ffff");
                Dispatcher.BeginInvoke(new ThreadStart(delegate { TimeLabel.Content = elapsedTime; }));

                Statistic.Statistic.LogEnd();

                TimeLabel.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    State.Text = "Сборка завершена";
                }));

            })).Start();
            
        }

        private void buttonDownload_Click(object sender, RoutedEventArgs e)
        {
            int depth;
            int rootUser;
            int max;
            if (
                int.TryParse(textBlockDepth.Text, out depth) &&
                int.TryParse(textBoxRootUser.Text, out rootUser) &&
                int.TryParse(textBoxVertexMax.Text, out max)
                )
            {
                Statistic.Statistic.logBegin();
                startCollecting(rootUser, depth,max);
                
            }
            
            //Тестирование делегатов

            //foreach (condDelegate func in listConditions)
            //    func(null, null);



        }

        private void updateStatsView(object sender, EventArgs e)
        {
            TimeLabel.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                TimeLabel.Content = Statistic.Statistic.GetTimeElapsed().ToString();
                labelVertexCount.Content = Statistic.Statistic.GetEdgesCount(2);
                labelVertexCount.Content = Statistic.Statistic.GetVertexCount(2);
            }));
            
        }

        private void button_addCondition(object sender, RoutedEventArgs e)
        {
            WindowCondition windowCondition = new WindowCondition();
            windowCondition.Owner = this;
            windowCondition.Show();
        }
    }
}
