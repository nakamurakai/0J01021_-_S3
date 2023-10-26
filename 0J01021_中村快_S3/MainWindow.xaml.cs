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

namespace _0J01021_中村快_S3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Schedule schedule;
        private UserControl[] userControls;
        public MainWindow()
        {
            InitializeComponent();
            schedule = new Schedule(this);

            userControls = new UserControl[]{
                schedule
            };
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            formPanel.Children.Add(schedule);

        }

        private void scheduleButton_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = new Hyperlink();
        }
    }
}
