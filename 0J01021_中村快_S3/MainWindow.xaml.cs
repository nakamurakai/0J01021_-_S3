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
        private Schedule_Add schedule_add;
        // ボタンを押しても処理を行わない(0:予定、1:追加)
        private int button_enable = 0;

        // 変更画面か否か
        private bool change = false;
        public bool Change { get { return change; } set { change = value; } }

        public MainWindow()
        {
            InitializeComponent();
            schedule = new Schedule(this);
            schedule_add = new Schedule_Add(this);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // パネルに画面を貼り付ける
            formPanel.Children.Add(schedule);
            formPanel.Children.Add(schedule_add);

            // 初めは予定画面を表示するから追加画面は非表示にしておく
            schedule_add.Visibility = Visibility.Hidden;
            // 既に表示しているので、予定ボタンを押しても反応しないようにする
            button_enable = 0;
            scheduleButton.Background = new SolidColorBrush(Color.FromArgb(0x88, 0xF5, 0xB1, 0x99));
        }

        // 予定・戻るボタン
        private void scheduleButton_Click(object sender, RoutedEventArgs e)
        {
            // 戻るボタンのとき
            if (change)
            {
                Detail_Close();
                change = false;
                return;
            }
            if (button_enable == 0) return;
            button_enable = 0;
            schedule.Visibility = Visibility.Visible;
            schedule_add.Visibility = Visibility.Hidden;
            scheduleButton.Background = new SolidColorBrush(Color.FromArgb(0x88, 0xF5, 0xB1, 0x99));
            addButton.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF8, 0x9C, 0x7B));
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (button_enable == 1) return;
            button_enable = 1;

            schedule.Visibility = Visibility.Hidden;
            schedule_add.Visibility = Visibility.Visible;
            scheduleButton.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF8, 0x9C, 0x7B));
            addButton.Background = new SolidColorBrush(Color.FromArgb(0x88, 0xF5, 0xB1, 0x99));
        }

        public void Detail_Open(List<string> data)
        {
            change = true;
            // 詳細画面を開く
            schedule_add.Change = true;
            schedule_add.Data = data;
            // 予定ボタンを戻るボタンにする
            scheduleButton.Content = "戻る";
            scheduleButton.Width = 125;
            scheduleButton.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF8, 0x9C, 0x7B));
            button_enable = 1;
            addButton.Visibility = Visibility.Hidden;
            // 画面切り替え
            schedule.Visibility = Visibility.Hidden;
            schedule_add.Visibility = Visibility.Visible;
        }

        public void Detail_Close()
        {
            schedule_add.Change = false;
            // 戻るボタンを予定ボタンにする
            scheduleButton.Content = "予定";
            scheduleButton.Width = 225;
            scheduleButton.Background = new SolidColorBrush(Color.FromArgb(0x88, 0xF5, 0xB1, 0x99));
            button_enable = 0;
            addButton.Visibility = Visibility.Visible;
            // 画面切り替え
            schedule.Visibility = Visibility.Visible;
            schedule_add.Visibility = Visibility.Hidden;
        }
    }
}
