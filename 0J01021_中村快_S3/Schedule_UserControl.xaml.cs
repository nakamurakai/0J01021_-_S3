// 制作実習II 第3期プログラム
// author 0J01021 中村快

using System;
using System.Collections.Generic;
using System.Text;
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
    /// Schedule_UserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class Schedule_UserControl : UserControl
    {
        private MainWindow mainWindow;
        private SQL sql = new SQL();

        private List<List<string>> items = new List<List<string>>();
        private List<List<string>> category = new List<List<string>>();
        private int[] para = new int[9]
        {
            1,0,2,0,0,0,0,2,1
        };
        // チェックボタンリスト
        private List<Button> checkButtons = new List<Button>();
        // コンテンツリスト
        private List<Border> borders = new List<Border>();

        // ページのアイテム数
        private int item_cnt;

        // 処理が終わるまで他の処理を行わない
        private bool do_work = false;

        // 日付表示の時の現在表示している日付
        private DateTime setdate = DateTime.Now;
        // カテゴリ表示の時の現在表示しているカテゴリの番地
        private int setcategory = 0;
        // 列
        private int row;
        // SQL
        private string comp_sql = "";

        public Schedule_UserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (do_work) return;
            else do_work = true;

            // 全てを選択した状態にする
            comboBox1.SelectedIndex = 0;
            // 一覧を表示する
            allRadioButton.IsChecked = true;

            Create_All();

            do_work = false;
        }

        // 一覧表示
        private void allRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (do_work) return;
            else do_work = true;

            Create_All();

            do_work = false;
        }

        // 日付表示
        private void dateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (do_work) return;
            else do_work = true;

            // 一覧で未完了または完了済みの状態で日付表示に切り替えたときにSQL文をWHEREからANDに置き換える
            comp_sql = comp_sql.Replace("WHERE", "AND");
            Create_Date();

            do_work = false;
        }

        // カテゴリ表示
        private void categoryRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (do_work) return;
            else do_work = true;

            // カテゴリを取得
            category = new List<List<string>>();
            int[] p = new int[1] { 0 };
            string s = "SELECT DISTINCT Category FROM dbo.todo";
            category = sql.Data_SelectAll(s, p);

            // 一覧で未完了または完了済みの状態でカテゴリ表示に切り替えたときにSQL文をWHEREからANDに置き換える
            comp_sql = comp_sql.Replace("WHERE", "AND");
            Create_Category();

            do_work = false;
        }

        // チェックボタンのクリックイベント
        private void Checkbtn_Click(object sender, EventArgs e)
        {
            if (do_work) return;
            else do_work = true;

            string text = (string)((Button)sender).Content;
            string comp = "0";

            if (text != "✓")
            {
                ((Button)sender).Content = "✓";
                comp = "1";
            }
            else
            {
                ((Button)sender).Content = "";
                comp = "0";
            }

            // SQLにcompの内容を反映させる(0:未完了、1:完了)
            foreach (var item in items)
            {
                if (((Button)sender).Name.ToString().Substring(8) == item[0])
                {
                    string oid = item[0];
                    item[8] = comp;
                    List<string> data = new List<string>();
                    for (int i = 1; i < item.Count; i++)
                    {
                        data.Add(item[i].ToString());
                    }
                    if (sql.Data_Update(data, oid) == false)
                    {
                        MessageBox.Show("エラーが発生しました");
                    }
                }
            }

            do_work = false;
        }

        private void beforeButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)dateRadioButton.IsChecked)
            {
                setdate = setdate.AddDays(-1);
                Create_Date();
            }
            else if ((bool)categoryRadioButton.IsChecked)
            {
                if ((string)beforeButton.Content != "")
                {
                    setcategory--;
                    Create_Category();
                }
            }
        }

        private void afterButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)dateRadioButton.IsChecked)
            {
                setdate = setdate.AddDays(1);
                Create_Date();
            }
            else if ((bool)categoryRadioButton.IsChecked)
            {
                if ((string)afterButton.Content != "")
                {
                    setcategory++;
                    Create_Category();
                }
            }
        }

        // setdateをdatePicker1で選んだ日付にする
        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            setdate = (DateTime)datePicker1.SelectedDate;
            Create_Date();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // すべての場合
            if (comboBox1.SelectedIndex == 0)
            {
                comp_sql = "";
                if ((bool)allRadioButton.IsChecked)
                {
                    Create_All();
                }
                else if ((bool)dateRadioButton.IsChecked)
                {
                    Create_Date();
                }
                else if ((bool)categoryRadioButton.IsChecked)
                {
                    Create_Category();
                }
            }
            // 未完了の場合
            else if (comboBox1.SelectedIndex == 1)
            {
                comp_sql = "Comp = 0";
                if ((bool)allRadioButton.IsChecked)
                {
                    comp_sql = "WHERE " + comp_sql;
                    Create_All();
                }
                else if ((bool)dateRadioButton.IsChecked)
                {
                    comp_sql = "AND " + comp_sql;
                    Create_Date();
                }
                else if ((bool)categoryRadioButton.IsChecked)
                {
                    comp_sql = "AND " + comp_sql;
                    Create_Category();
                }
            }
            // 完了済みの場合
            else if (comboBox1.SelectedIndex == 2)
            {
                comp_sql = "Comp = 1";
                if ((bool)allRadioButton.IsChecked)
                {
                    comp_sql = "WHERE " + comp_sql;
                    Create_All();
                }
                else if ((bool)dateRadioButton.IsChecked)
                {
                    comp_sql = "AND " + comp_sql;
                    Create_Date();
                }
                else if ((bool)categoryRadioButton.IsChecked)
                {
                    comp_sql = "AND " + comp_sql;
                    Create_Category();
                }
            }
        }

        // 画面のコンテンツ作成
        private void Create_Content()
        {
            // 既存のコンテンツを削除
            foreach (var button in checkButtons)
            {
                contents.Children.Remove(button);
            }
            foreach (var border in borders)
            {
                contents.Children.Remove(border);
            }

            // 作成行の数
            row = 11;
            if (item_cnt > row)
            {
                row = item_cnt;
            }

            for (int i = 0; i < row; i++)
            {
                checkbtn(i);
                datelabel(i);
                dolabel(i);
                detailbtn(i);
            }
        }

        // 詳細ボタン作成
        private void detailbtn(int n)
        {
            // 枠線の設定
            Border border = new Border();
            border.Name = "ditailBorder";
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xC3, 0xC3));
            border.BorderThickness = new Thickness(1);
            border.Height = 30;
            border.Width = 50;
            border.Margin = new Thickness(383, (30 * n + 30), 0, 0);

            // リンクテキスト
            Hyperlink hyperlink = new Hyperlink();
            TextBlock textBlock = new TextBlock();
            TextBlock text = new TextBlock();

            if (n < item_cnt)
            {
                textBlock.FontFamily = new FontFamily("UD Digi Kyokasho NK-R");
                textBlock.FontSize = 16;
                textBlock.Text = "詳細";
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;

                text.TextAlignment = TextAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;

                hyperlink.Name = "detailHyperlink" + items[n][0];
                hyperlink.Click += DetailHyperlink_Click;
                hyperlink.Inlines.Add(textBlock);
                text.Inlines.Add(hyperlink);
            }

            border.Child = text;
            contents.Children.Add(border);
            borders.Add(border);
        }

        // 詳細画面に画面を遷移する
        private void DetailHyperlink_Click(object sender, RoutedEventArgs e)
        {
            // 表示するデータ
            string serch1 = ((Hyperlink)sender).Name.Substring(15);
            string s = "SELECT * FROM dbo.todo WHERE Id = @serch1";
            List<List<string>> datas = sql.Data_Select(s, para, serch1, "");
            mainWindow.Detail_Open(datas[0]);
        }

        // やることの設定したリンクに飛ぶことができるようにする処理
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // 設定したリンク先のURLに飛ぶ
            var startInfo = new System.Diagnostics.ProcessStartInfo(e.Uri.ToString());
            startInfo.UseShellExecute = true;
            System.Diagnostics.Process.Start(startInfo);
        }

        // やること作成
        private void dolabel(int n)
        {
            // 枠線の設定
            Border border = new Border();
            border.Name = "doBorder";
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xC3, 0xC3));
            border.BorderThickness = new Thickness(1);
            border.Height = 30;
            border.Width = 263;
            border.Margin = new Thickness(120, (30 * n + 30), 0, 0);
            // テキストの設定
            Hyperlink hyperlink = new Hyperlink();
            TextBlock textBlock = new TextBlock();
            TextBlock text = new TextBlock();

            if (n < item_cnt)
            {
                textBlock.Name = "doTextBlock";
                textBlock.FontFamily = new FontFamily("UD Digi Kyokasho NK-R");
                textBlock.FontSize = 12;
                textBlock.Text = items[n][1];
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.ToolTip = "内容: " + items[n][1] + "\nカテゴリ: " + items[n][3] + "\n場所: " + items[n][4] + "\n説明: " + items[n][6] + "\nアラーム: " + items[n][7];
                text.TextAlignment = TextAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;

                // URLを設定していたらそのURLに飛べるようにする
                if (items[n][5] != "")
                {
                    hyperlink.NavigateUri = new Uri(items[n][5]);
                    hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                    hyperlink.Inlines.Add(textBlock);
                    text.Inlines.Add(hyperlink);
                }
                else
                {
                    text.Inlines.Add(textBlock);
                }
            }

            border.Child = text;
            contents.Children.Add(border);
            borders.Add(border);
        }

        // 時間作成
        private void datelabel(int n)
        {
            // 枠線の設定
            Border border = new Border();
            border.Name = "dateBorder";
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xC3, 0xC3));
            border.BorderThickness = new Thickness(1);
            border.Height = 30;
            border.Width = 70;
            border.Margin = new Thickness(50, (30 * n + 30), 0, 0);

            // テキストの設定
            TextBlock textBlock = new TextBlock();

            if (n < item_cnt)
            {
                textBlock.Name = "dateTextBlock";
                textBlock.FontFamily = new FontFamily("UD Digi Kyokasho NK-R");

                DateTime date = DateTime.Parse(items[n][2]);
                if ((bool)allRadioButton.IsChecked || (bool)categoryRadioButton.IsChecked)
                {
                    textBlock.Text = date.ToString("yyyy/MM/dd");
                    textBlock.ToolTip = date.ToString("HH:mm");
                    textBlock.FontSize = 10;
                }
                else if ((bool)dateRadioButton.IsChecked)
                {
                    textBlock.Text = date.ToString("HH:mm");
                    textBlock.FontSize = 12;
                }

                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
            }

            border.Child = textBlock;
            contents.Children.Add(border);
            borders.Add(border);
        }

        // チェックボタン作成
        private void checkbtn(int n)
        {
            Button button = new Button();
            // 名前
            button.Name = "checkbtn";
            // 横幅
            button.Width = 50;
            // 縦幅
            button.Height = 30;
            // 位置
            button.Margin = new Thickness(0, (n * 30 + 30), 0, 0);
            // 背景色
            button.Background = Brushes.White;
            // 枠線の色
            button.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xC3, 0xC3));
            // 枠線の太さ
            button.BorderThickness = new Thickness(1, 1, 1, 1);

            if (n < item_cnt)
            {

                // 名前
                button.Name += items[n][0].ToString();
                // チェックマーク
                if (items[n][8] == "0")
                {
                    button.Content = "";
                }
                else
                {
                    button.Content = "✓";
                }
                // 文字のフォント
                button.FontFamily = new FontFamily("UD Digi Kyokasho NK-R");
                // フォントサイズ
                button.FontSize = 16;
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Click += Checkbtn_Click;
            }

            contents.Children.Add(button);
            checkButtons.Add(button);
        }

        private void Create_All()
        {
            // datePikerの設定
            datePicker1.Visibility = Visibility.Hidden;

            // nowButtonの設定
            nowButton.Margin = new Thickness(0, 40, 0, 0);
            nowButton.Content = "一覧";
            nowButton.Width = 450;

            // SQLのデータをすべて取得
            string s = "SELECT * FROM dbo.todo " + comp_sql + " ORDER BY Date";
            items = sql.Data_SelectAll(s, para);
            // ページのアイテム数
            item_cnt = items.Count;

            // 予定の項目のコンテンツ作成
            Create_Content();
        }

        private void Create_Date()
        {
            // datePikerの設定
            datePicker1.Visibility = Visibility.Visible;
            datePicker1.SelectedDate = setdate;
            // nowButtonの設定
            nowButton.Margin = new Thickness(120, 40, 0, 0);
            nowButton.Width = 210;

            beforeButton.Content = setdate.AddDays(-1).ToString("yyyy/MM/dd");
            afterButton.Content = setdate.AddDays(1).ToString("yyyy/MM/dd");

            // 日付が今日であるデータを取得
            string s = "SELECT * FROM dbo.todo WHERE Date BETWEEN @serch1  AND @serch2 " + comp_sql + " ORDER BY Date";
            items = sql.Data_Select(s, para, setdate.ToString("yyyy/MM/dd") + " 00:00:00", setdate.ToString("yyyy/MM/dd") + " 23:59:59");
            // ページのアイテム数
            item_cnt = items.Count;

            // コンテンツを更新
            Create_Content();
        }

        private void Create_Category()
        {
            // datePikerの設定
            datePicker1.Visibility = Visibility.Hidden;

            nowButton.Margin = new Thickness(120, 40, 0, 0);
            nowButton.Width = 210;
            if (setcategory >= 1)
            {
                beforeButton.Content = category[setcategory - 1][0];
            }
            else
            {
                beforeButton.Content = "";
            }

            if (category.Count > 1 && setcategory + 1 < category.Count)
            {
                afterButton.Content = category[setcategory + 1][0];
            }
            else
            {
                afterButton.Content = "";
            }

            // 選択しているカテゴリのデータを取得
            string s = "SELECT * FROM dbo.todo WHERE Category LIKE CONCAT('%',@serch1,'%') " + comp_sql + " ORDER BY Date";
            items = sql.Data_Select(s, para, category[setcategory][0], "");
            // ページのアイテム数
            item_cnt = items.Count;

            // コンテンツを更新
            Create_Content();
        }

        private void UserControl_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (do_work) return;
            else do_work = true;

            if (this.IsVisible)
            {
                // カテゴリを取得する
                category = new List<List<string>>();
                int[] p = new int[1] { 0 };
                string s = "SELECT DISTINCT Category FROM dbo.todo";
                category = sql.Data_SelectAll(s, p);
                // カテゴリが何もなかった場合カテゴリ表示をできなくする
                if (category.Count > 0) categoryRadioButton.IsEnabled = true;
                else categoryRadioButton.IsEnabled = false;

                // 内容の更新
                if ((bool)allRadioButton.IsChecked)
                {
                    Create_All();
                }
                else if ((bool)dateRadioButton.IsChecked)
                {
                    // 一覧で未完了または完了済みの状態で日付表示に切り替えたときにSQL文をWHEREからANDに置き換える
                    comp_sql = comp_sql.Replace("WHERE", "AND");
                    Create_Date();
                }
                else if ((bool)categoryRadioButton.IsChecked)
                {
                    // 一覧で未完了または完了済みの状態でカテゴリ表示に切り替えたときにSQL文をWHEREからANDに置き換える
                    comp_sql = comp_sql.Replace("WHERE", "AND");
                    Create_Category();
                }

            }
            do_work = false;
        }
    }
}
