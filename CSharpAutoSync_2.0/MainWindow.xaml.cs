using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSharpAutoSync
{
  /// <summary>
  /// MainWindow.xaml 的互動邏輯
  /// </summary>
  public partial class MainWindow : Window
  {

    #region 預載程序

    NotifyIcon notifyIcon = new NotifyIcon();

    public MainWindow()
    {
      InitializeComponent();
      icon();
    }

    private void icon()
    {
      notifyIcon.Text = "檔案同步工具";
      notifyIcon.Icon = new System.Drawing.Icon(@"img\sync.ico");
      notifyIcon.Visible = true;
      notifyIcon.MouseClick += OnNotifyIconClick;
      //notifyIcon.BalloonTipText = "啟動提示訊息";
      //notifyIcon.ShowBalloonTip(100);
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
      if (WindowState == WindowState.Minimized)
      {
        this.Hide();
      }
    }

    private void OnNotifyIconClick(object sender, EventArgs e)
    {
      this.Show();
      this.WindowState = WindowState.Normal;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      if (System.Windows.MessageBox.Show("確認是否關閉？", "確認", MessageBoxButton.YesNo) == MessageBoxResult.No)
      {
        e.Cancel = true;
      }
    }

    #endregion

    private void Button_SourcePath_Click(object sender, RoutedEventArgs e)
    {
      FolderBrowserDialog Path = new FolderBrowserDialog();
      Path.ShowDialog();
      TextBox_SourcePath.Text = Path.SelectedPath;
    }

    private void Button_TargetPath_Click(object sender, RoutedEventArgs e)
    {
      FolderBrowserDialog Path = new FolderBrowserDialog();
      Path.ShowDialog();
      TextBox_TargetPath.Text = Path.SelectedPath;
    }
  }
}
