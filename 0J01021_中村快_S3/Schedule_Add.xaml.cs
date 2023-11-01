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
    /// Schedule_Add.xaml の相互作用ロジック
    /// </summary>
    public partial class Schedule_Add : UserControl
    {
        private MainWindow mainWindow;
        private SQL sql = new SQL();
        // 変更画面か否か
        private bool change = false;
        public bool Change {set { change = value; } }

        public Schedule_Add(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 日付をその日に設定する
            dateText.SelectedDate = DateTime.Now;
            // 時間入力のコンボボックスアイテムを格納
            for(int i = 1; i < 24;  i++)
            {
                hourComboBox.Items.Add(i.ToString("00"));
            }
            for(int i = 5;i < 60; i+=5)
            {
                minutesComboBox.Items.Add(i.ToString("00"));
            }
        }

        // 追加ボタン
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();

            // 内容テキスト
            if (doText.Text == "")
            {
                warningText1.Visibility = Visibility.Visible;
                doText.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
                return;
            }
            else
            {
                data.Add(doText.Text);
            }

            // 日時
            DateTime date1 = (DateTime)dateText.SelectedDate;
            DateTime date = new DateTime(date1.Year, date1.Month, date1.Day, int.Parse(hourComboBox.Text), int.Parse(minutesComboBox.Text),0);
            // YYYY/MM/dd HH:mm:ss
            data.Add(date.ToString("G"));

            // カテゴリ
            if (categoryComboBox.Text == "")
            {
                warningText2.Visibility = Visibility.Visible;
                categoryComboBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
                return;
            }
            else
            {
                data.Add(categoryComboBox.Text);
            }

            // 場所
            if (locationText.Text == "") data.Add("");
            else data.Add(locationText.Text);

            // URL
            if (urlText.Text == "") data.Add("");
            else data.Add(urlText.Text);

            // 説明
            if (explanationText.Text == "") data.Add("");
            else data.Add(explanationText.Text);

            // アラーム
            // 設定しない
            if (alarmComboBox.SelectedIndex == 0) data.Add("");
            else
            {
                if (alarmComboBox.SelectedIndex == 1) date.AddMinutes(-5);
                else if (alarmComboBox.SelectedIndex == 2) date.AddMinutes(-10);
                else if (alarmComboBox.SelectedIndex == 3) date.AddMinutes(-15);
                else if (alarmComboBox.SelectedIndex == 4) date.AddMinutes(-30);
                else if (alarmComboBox.SelectedIndex == 5) date.AddHours(-1);
                else if (alarmComboBox.SelectedIndex == 6) date.AddHours(-3);
                else if (alarmComboBox.SelectedIndex == 7) date.AddHours(-12);
                else if (alarmComboBox.SelectedIndex == 8) date.AddDays(-1);
                else if (alarmComboBox.SelectedIndex == 9) date.AddDays(-7);

                data.Add(date.ToString("G"));
            }

            // 完了済みかどうか
            data.Add("0");

            if (sql.Data_Insert(data)) // データ挿入が出来た場合
            {
                MessageBox.Show("データの登録が完了しました");
            }
            else // 出来なかった場合
            {
                MessageBox.Show("データの登録に失敗しました");
            }
        }

        // 内容テキストでフォーカスが離れたときに内容が入力されていなかったらメッセージを表示し枠線の色を変える
        private void doText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (doText.Text == "")
            {
                warningText1.Visibility = Visibility.Visible;
                doText.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            }
            else
            {
                warningText1.Visibility = Visibility.Hidden;
                doText.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAB, 0xAD, 0xB3));
            }
        }

        // カテゴリテキストでフォーカスが離れたときに内容が入力されていなかったらメッセージを表示し枠線の色を変える
        private void categoryComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(categoryComboBox.Text == "")
            {
                warningText2.Visibility = Visibility.Visible;
                categoryComboBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF,0xFF,0x00,0x00));
            }
            else
            {
                warningText2.Visibility = Visibility.Hidden;
                categoryComboBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAB, 0xAD, 0xB3));
            }
        }

        // 画面が表示されたとき中身を初期化
        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                if (change)
                {

                }
                else
                {
                    doText.Text = "";
                    dateText.SelectedDate = DateTime.Now;
                    hourComboBox.SelectedIndex = 0;
                    minutesComboBox.SelectedIndex = 0;
                    categoryComboBox_ItemAdd();
                    locationText.Text = "";
                    urlText.Text = "";
                    explanationText.Text = "";
                }
            }
        }

        private void categoryComboBox_ItemAdd()
        {
            // カテゴリを取得
            List<List<string>> category = new List<List<string>>();
            int[] p = new int[1] { 0 };
            string s = "SELECT DISTINCT Category FROM dbo.todo";
            category = sql.Data_SelectAll(s, p);

            categoryComboBox.Items.Clear();
            if(category.Count > 0)
            {
                foreach (List<string> item in category)
                {
                    categoryComboBox.Items.Add(item[0]);
                }
            }            
        }
    }
}
