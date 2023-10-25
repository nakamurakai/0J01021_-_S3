using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Navigation;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

namespace _0J01021_中村快_S3
{
    /// <summary>
    /// Schedule.xaml の相互作用ロジック
    /// </summary>
    public partial class Schedule : UserControl
    {
        private MainWindow mainWindow;
        private SQL sql = new SQL();

        List<List<string>> items = new List<List<string>>();

        // チェックボタンリスト
        private List<Button> checkButtons = new List<Button>();

        // ページのアイテム数
        private int item_cnt;

        private int row;
        public Schedule(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            allRadioButton.IsChecked = true;
            // 予定の項目のコンテンツ作成
            // SQLのデータをすべて取得
            string s = "SELECT * FROM dbo.todo ORDER BY Date";
            items = sql.Data_Select(s);

            // ページのアイテム数
            item_cnt = items.Count;
            // 作成行の数
            row = 11;
            if(item_cnt > row)
            {
                row = item_cnt;
            }

            for(int i = 0; i < row; i++)
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
            border.Name = "doBorder" + n;
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
                textBlock.Name = "hyperlink" + n;
                textBlock.FontFamily = new FontFamily("UD Digi Kyokasho NK-R");
                textBlock.FontSize = 16;
                textBlock.Text = "詳細";
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
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

        }

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
            border.Name = "doBorder" + n;
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
                textBlock.Name = "doTextBlock" + n;
                textBlock.FontFamily = new FontFamily("UD Digi Kyokasho NK-R");
                textBlock.FontSize = 12;
                textBlock.Text = items[n][1];
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;

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
        }

        // 時間作成
        private void datelabel(int n)
        {
            // 枠線の設定
            Border border = new Border();
            border.Name = "dateBorder" + n;
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xC3, 0xC3));
            border.BorderThickness = new Thickness(1);
            border.Height = 30;
            border.Width = 70;
            border.Margin = new Thickness(50,(30 * n + 30),0,0);

            // テキストの設定
            TextBlock textBlock = new TextBlock();

            if (n < item_cnt)
            {
                textBlock.Name = "dateTextBlock" + n;
                textBlock.FontFamily = new FontFamily("UD Digi Kyokasho NK-R");
                textBlock.FontSize = 12;
                string str = items[n][2].ToString();
                DateTime date = new DateTime(int.Parse(str.Substring(0,4)), int.Parse(str.Substring(4, 2)), int.Parse(str.Substring(6,2)), int.Parse(str.Substring(8,2)), int.Parse(str.Substring(10,2)),int.Parse("00"));
                if ((bool)allRadioButton.IsChecked || (bool)categoryRadioButton.IsChecked)
                {
                    textBlock.Text = date.Year + "/" + date.Month + "/" + date.Day;
                }else if ((bool)dateRadioButton.IsChecked)
                {
                    textBlock.Text = date.Hour + ":" + date.Minute;
                }
                
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
            }
            
            border.Child = textBlock;
            contents.Children.Add(border);
        }

        // チェックボタン作成
        private void checkbtn(int n)
        {
            Button button = new Button();
            // 名前
            button.Name = "checkbtn" + n;
            // 横幅
            button.Width = 50;
            // 縦幅
            button.Height = 30;
            // 位置
            button.Margin = new Thickness(0,(n * 30 + 30),0,0);
            // 背景色
            button.Background = Brushes.White;
            // 枠線の色
            button.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xC3, 0xC3));
            // 枠線の太さ
            button.BorderThickness = new Thickness(1, 1, 1, 1);

            if(n < item_cnt)
            {
                // 文字のフォント
                button.FontFamily = new FontFamily("UD Digi Kyokasho NK-R");
                // フォントサイズ
                button.FontSize = 16;
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Click += (sender, e) => Checkbtn_Click(sender, e);
            }
            
            contents.Children.Add(button);
            checkButtons.Add(button);
        }

        // チェックボタンのクリックイベント
        private void Checkbtn_Click(object sender, EventArgs e)
        {
            string text = (string)((Button)sender).Content;
            if (text != "✓") ((Button)sender).Content = "✓";
            else ((Button)sender).Content = "";
        }

        // 一覧表示
        private void allRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            nowButton.Margin = new Thickness(0, 40, 0, 0);
            nowButton.Content = "一覧";
            nowButton.Width = 450;
        }

        // 日付表示
        private void dateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTime.Now;
            nowButton.Margin = new Thickness(120, 40, 0, 0);
            nowButton.Content = date.ToString("yyyy/MM/dd");
            nowButton.Width = 210;
            beforeButton.Content = date.AddDays(-1).ToString("yyyy/MM/dd");
            afterButton.Content = date.AddDays(1).ToString("yyyy/MM/dd");
        }

        // カテゴリ表示
        private void categoryRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            List<List<string>> category = new List<List<string>>();
            string s = "SELECT DISTINCT Category FROM dbo.todo";
            category = sql.Data_Select(s);
            nowButton.Margin = new Thickness(120, 40, 0, 0);
            nowButton.Content = category[1][0];
            nowButton.Width = 210;
            beforeButton.Content = category[0][0];
        }
    }
}
