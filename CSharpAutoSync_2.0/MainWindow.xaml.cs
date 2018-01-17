using CSharpAutoSync_2._0.src;
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
    Sync Sync;
    BackgroundWorker BackgroundWorker_Sync;

    public MainWindow()
    {
      Sync = new Sync(this);
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

    private void Button_Sync_Click(object sender, RoutedEventArgs e)
    {
      BackgroundWorker_Sync = new BackgroundWorker();
      BackgroundWorker_Sync.WorkerSupportsCancellation = true;
      BackgroundWorker_Sync.WorkerReportsProgress = true;
      BackgroundWorker_Sync.DoWork += new DoWorkEventHandler(DoWork_Sync);
      BackgroundWorker_Sync.ProgressChanged += new ProgressChangedEventHandler(DuringWork_Sync);
      BackgroundWorker_Sync.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AfterWork_Sync);

      if (Equals(Button_Sync.Content, "開始自動同步"))
      {
        Button_Sync.Content = "停止";

        Button_SourcePath.IsEnabled = false;
        Button_TargetPath.IsEnabled = false;
        TextBox_Extension.IsEnabled = false;
        ComboBox_Delay.IsEnabled = false;

        BackgroundWorker_Sync.RunWorkerAsync();

        //SAP_Sync.OrderLog("開始自動轉入訂單...");
      }
      else
      {
        Button_Sync.Content = "開始自動同步";

        Button_SourcePath.IsEnabled = true;
        Button_TargetPath.IsEnabled = true;
        TextBox_Extension.IsEnabled = true;
        ComboBox_Delay.IsEnabled = true;

        BackgroundWorker_Sync.CancelAsync();
      }
    }

    internal void DoWork_Sync(object sender, DoWorkEventArgs e)
    {
      Sync.SyncFile(sender as BackgroundWorker, e);
    }

    internal void DuringWork_Sync(object sender, ProgressChangedEventArgs e)
    {
      ProgressBar_Sync.Value = e.ProgressPercentage;
    }

    internal void AfterWork_Sync(object sender, RunWorkerCompletedEventArgs e)
    {
      if (!BackgroundWorker_Sync.CancellationPending)
      {
        BackgroundWorker_Sync.RunWorkerAsync();
      }
      else
      {
        //SAP_Sync.OrderLog("已停止自動轉入訂單。");
      }

      if (e.Error != null)
      {
        //SAP_Sync.OrderLog(e.Error.Message);
      }
    }


  }
}
