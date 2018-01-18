using CSharpAutoSync_2._0.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    public BackgroundWorker BackgroundWorker_Sync;

    public MainWindow()
    {
      Sync = new Sync(this);
      InitializeComponent();
      icon();

      //載入延遲時間選項
      ComboBox_Delay.Items.Add(0);
      ComboBox_Delay.Items.Add(10);
      ComboBox_Delay.Items.Add(30);
      ComboBox_Delay.Items.Add(60);
      ComboBox_Delay.Items.Add(120);
      ComboBox_Delay.SelectedIndex = 0;
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

    #region 預載程序

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
        string Message = "本程序每 " + Convert.ToInt16(ComboBox_Delay.SelectedItem) + " 分鐘自動同步一次。";
        if (ComboBox_Delay.SelectedIndex <= 0) Message = "是否僅執行一次同步？";

        if (System.Windows.MessageBox.Show(Message, "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          Button_Sync.Content = "停止";

          Button_SourcePath.IsEnabled = false;
          Button_TargetPath.IsEnabled = false;
          TextBox_Extension.IsEnabled = false;
          ComboBox_Delay.IsEnabled = false;

          Sync.InLog("三秒後開始同步處理...");
          BackgroundWorker_Sync.RunWorkerAsync();
        }
      }
      else
      {
        Button_Sync.Content = "開始自動同步";

        Button_SourcePath.IsEnabled = true;
        Button_TargetPath.IsEnabled = true;
        TextBox_Extension.IsEnabled = true;
        ComboBox_Delay.IsEnabled = true;

        Sync.InLog("已停止自動同步。");
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
      if (!BackgroundWorker_Sync.CancellationPending && ComboBox_Delay.SelectedIndex != 0)
      {
        BackgroundWorker_Sync.RunWorkerAsync();
      }
      else
      {
        Button_Sync.Content = "開始自動同步";
        Button_SourcePath.IsEnabled = true;
        Button_TargetPath.IsEnabled = true;
        TextBox_Extension.IsEnabled = true;
        ComboBox_Delay.IsEnabled = true;
        Sync.InLog("已停止自動同步。");
      }

      if (e.Error != null)
      {
        Sync.InLog(e.Error.Message);
      }
    }

    #endregion
  }
}
