using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;

namespace _0J01021_中村快_S3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Activated(object sender, EventArgs e)
        {
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                // 通知から引数を取得する
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                // 通知からユーザー入力 (テキスト ボックス、メニュー選択) を取得します。
                ValueSet userInput = toastArgs.UserInput;

                // UI操作を実行する場合はUIスレッドにディスパッチする必要があります
                Application.Current.Dispatcher.Invoke(delegate
                {
                    if (args.TryGetValue("url", out string s))
                    {
                        // URLのリンクにアクセスする
                        var startInfo = new System.Diagnostics.ProcessStartInfo(s);
                        startInfo.UseShellExecute = true;
                        System.Diagnostics.Process.Start(startInfo);
                    }
                });
            };
        }
    }
}
