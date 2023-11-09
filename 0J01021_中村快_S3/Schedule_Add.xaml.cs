using Microsoft.Toolkit.Uwp.Notifications;
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
using Windows.UI.Notifications;

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

        private List<string> item = new List<string>();
        public List<string> Data { set { item = value; } }

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

        // 追加・変更ボタン
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
            DateTime date = new DateTime(date1.Year, date1.Month, date1.Day, int.Parse(hourComboBox.Text), int.Parse(minutesComboBox.Text), 0);
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
                if (alarmComboBox.SelectedIndex == 1) date = date.AddMinutes(-5);
                else if (alarmComboBox.SelectedIndex == 2) date = date.AddMinutes(-10);
                else if (alarmComboBox.SelectedIndex == 3) date = date.AddMinutes(-15);
                else if (alarmComboBox.SelectedIndex == 4) date = date.AddMinutes(-30);
                else if (alarmComboBox.SelectedIndex == 5) date = date.AddHours(-1);
                else if (alarmComboBox.SelectedIndex == 6) date = date.AddHours(-3);
                else if (alarmComboBox.SelectedIndex == 7) date = date.AddHours(-12);
                else if (alarmComboBox.SelectedIndex == 8) date = date.AddDays(-1);
                else if (alarmComboBox.SelectedIndex == 9) date = date.AddDays(-7);

                data.Add(date.ToString("G"));
            }

            // 完了済みかどうか
            data.Add("0");
            // 変更の場合
            if (change)
            {
                if (sql.Data_Update(data, item[0])) // データ更新が出来た場合
                {
                    MessageBox.Show("データの変更が完了しました");
                }
                else
                {
                    MessageBox.Show("データの変更に失敗しました");
                }

                // アラームの設定
                // 元のセットされていたアラームを削除
                // スケジュールされたトーストを取得する
                var notifier = ToastNotificationManagerCompat.CreateToastNotifier();
                var scheduledToasts = notifier.GetScheduledToastNotifications();
                // Group, Tag で対象トーストを取得する
                var toRemove = scheduledToasts.FirstOrDefault(i => i.Tag == data[0] && i.Group == "1");
                if (toRemove != null)
                {
                    // スケジュールから削除する
                    notifier.RemoveFromSchedule(toRemove);
                }
            }
            else // 追加の場合
            {
                if (sql.Data_Insert(data)) // データ挿入が出来た場合
                {
                    MessageBox.Show("データの登録が完了しました");
                }
                else // 出来なかった場合
                {
                    MessageBox.Show("データの登録に失敗しました");
                }
            }
            // アラームの設定
            if (data[7] != "")
            {
                // URLがついていたらリンクに飛ぶことができる
                if (data[4] != "")
                {
                    new ToastContentBuilder()
                        .AddText(DateTime.Parse(data[1]).ToString("F"))
                        .AddText(doText.Text)
                        .AddButton(new ToastButton()
                            .SetContent("リンク")
                            .AddArgument("url", data[4])
                            .SetBackgroundActivation())
                        .Schedule(date, toast =>
                        {
                            toast.Group = "1";
                            toast.Tag = data[0];
                        });
                }
                else
                {
                    new ToastContentBuilder()
                    .AddArgument(doText.Text)
                    .AddText(DateTime.Parse(data[1]).ToString("F"))
                    .AddText(doText.Text)
                    .Schedule(date, toast =>
                    {
                        toast.Group = "1";
                        toast.Tag = data[0];
                    });
                }
                new ToastContentBuilder()
                    .AddText(DateTime.Parse(data[1]).ToString("F"))
                    .AddText(doText.Text)
                    .AddButton(new ToastButton()
                        .SetContent("リンク")
                        .AddArgument("url", data[4])
                        .SetBackgroundActivation()).Show();
            }
        }

        // 削除ボタン
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("このデータを削除しますが本当によろしいですか", "Information", MessageBoxButton.YesNo) == MessageBoxResult.No) return; 
            string s = "DELETE FROM dbo.todo WHERE Id = @id";

            sql.Delete(s, item[0], "@id");
            // セットされていたアラームを削除
            ToastNotificationManager.History.Remove(item[1], "1");
            mainWindow.Detail_Close();
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
                categoryComboBox_ItemAdd();
                // 変更画面の時
                if (change)
                {
                    addButton.Content = "変更";
                    deleteButton.Visibility = Visibility.Visible;
                    doText.Text = item[1];
                    DateTime date = DateTime.Parse(item[2]);
                    dateText.SelectedDate = date;
                    hourComboBox.Text = date.ToString("HH");
                    minutesComboBox.Text = date.ToString("mm");
                    categoryComboBox.Text = item[3];
                    locationText.Text = item[4];
                    urlText.Text = item[5];
                    explanationText.Text = item[6];
                    setalarmComboBox();
                }
                else // データ追加の時
                {
                    addButton.Content = "追加";
                    deleteButton.Visibility = Visibility.Hidden;
                    doText.Text = "";
                    dateText.SelectedDate = DateTime.Now;
                    hourComboBox.SelectedIndex = 0;
                    minutesComboBox.SelectedIndex = 0;
                    locationText.Text = "";
                    urlText.Text = "";
                    explanationText.Text = "";
                    alarmComboBox.SelectedIndex = 0;
                }
            }
        }

        // alarmcomboBoxにセットするテキスト
        private void setalarmComboBox()
        {
            if (item[7] == "")
            {
                alarmComboBox.SelectedIndex = 0;
                return;
            }
            DateTime date = DateTime.Parse(item[2]);
            DateTime alarm = DateTime.Parse(item[7]);
            // 差
            TimeSpan span = date - alarm;
            // 5分
            if (span.Minutes == 5) alarmComboBox.SelectedIndex = 1;
            else if (span.Minutes == 10) alarmComboBox.SelectedIndex = 2;
            else if (span.Minutes == 15) alarmComboBox.SelectedIndex = 3;
            else if (span.Minutes == 30) alarmComboBox.SelectedIndex = 4;
            else if (span.Hours == 1) alarmComboBox.SelectedIndex = 5;
            else if (span.Hours == 3) alarmComboBox.SelectedIndex = 6;
            else if (span.Hours == 12) alarmComboBox.SelectedIndex = 7;
            else if (span.Days == 1) alarmComboBox.SelectedIndex = 8;
            else if (span.Days == 7) alarmComboBox.SelectedIndex = 9;
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
            categoryComboBox.SelectedIndex = 0;
        }
    }
}
