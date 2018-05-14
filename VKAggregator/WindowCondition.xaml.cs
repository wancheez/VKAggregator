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

namespace VKAggregator
{
    /// <summary>
    /// Логика взаимодействия для WindowCondition.xaml
    /// </summary>
    public partial class WindowCondition : Window
    {
        public WindowCondition()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var own = (MainWindow)this.Owner;
            own.listConditions.Add(FilterMethods.FilterCity);
            this.Close();

        }

    }
}
