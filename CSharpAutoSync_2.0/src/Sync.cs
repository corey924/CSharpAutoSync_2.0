using CSharpAutoSync;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace CSharpAutoSync_2._0.src
{
  class Sync
  {
    MainWindow Main;
    Common Common = new Common();

    private static Logger logger = LogManager.GetCurrentClassLogger();

    public Sync(MainWindow MainWindow)
    {
      Main = MainWindow;
    }

    internal void SyncFile(BackgroundWorker Worker, DoWorkEventArgs e)
    {
      //同步前延遲時間
      Thread.Sleep(3000);
      if (!Main.BackgroundWorker_Sync.IsBusy) return;

      try
      {
        //來源路徑
        DirectoryInfo SourcePath = new DirectoryInfo(Main.Dispatcher.Invoke(new outputElement(GetSourcePath)).ToString());
        //目標路徑
        DirectoryInfo TargetPath = new DirectoryInfo(Main.Dispatcher.Invoke(new outputElement(GetTarGetPath)).ToString());
        //指定副檔名
        string Extension = Main.Dispatcher.Invoke(new outputElement(GetExtension)).ToString();
        //延遲時間
        int Delay = Convert.ToInt32(Main.Dispatcher.Invoke(new outputElement(GetDelay)));
        //訊息內容
        string addLog = null;

        //來源路徑如果不存在
       if (!SourcePath.Exists)
        {
          addLog = "來源路徑異常。";
          Main.Dispatcher.Invoke(new outputDelegate(InLog), addLog);
          Common.CatchMessage(addLog);
          return;
        }

        //檢查路徑目錄是否有創建
        if (!Directory.Exists(TargetPath.FullName)) Directory.CreateDirectory(TargetPath.FullName);

        //參數載入
        int countFile = 0;
        FileInfo[] SourceFiles = SourcePath.GetFiles();
        FileInfo[] TargetFiles = TargetPath.GetFiles();
        List<FileInfo> TargetFileList = new List<FileInfo>();

        //指定副檔名切割
        List<string> ExtensionList = new List<string>();
        ExtensionList.AddRange(Extension.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

        foreach (FileInfo TarGetInfo in TargetFiles) TargetFileList.Add(TarGetInfo);
        foreach (FileInfo SourceInfo in SourceFiles)
        {
          //副檔名篩選
          if (ExtensionList.Contains(SourceInfo.Extension.ToLower()))
          {
            if (TargetFileList.Any(o => o.Name == SourceInfo.Name && o.LastWriteTime == SourceInfo.LastWriteTime)) continue;
            string temppath = Path.Combine(TargetPath.FullName, SourceInfo.Name);
            SourceInfo.CopyTo(temppath, true);
            countFile++;
          }

          double progress = (double)countFile / SourceFiles.Count();
          Worker.ReportProgress(Convert.ToInt32(progress * 100));
          Thread.Sleep(1);
        }

        addLog = "已成功同步 " + countFile + " 個檔案。";
        Main.Dispatcher.Invoke(new outputDelegate(InLog), addLog);

        //使用者設定延遲時間
        Thread.Sleep(Delay * 60000);
      }
      catch (Exception ex)
      {
        Main.Dispatcher.Invoke(new outputDelegate(InLog), ex.Message);
        Common.CatchMessage(ex.Message);
      }

      e.Cancel = true;

    }


    public delegate void outputDelegate(string msg);
    public void InLog(string content)
    {
      logger.Info(content);
      Main.TextBox_Log.Text += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + content + Environment.NewLine;
      Main.TextBox_Log.ScrollToEnd();
    }

    public delegate string outputElement();
    public string GetSourcePath()
    {
      string SourcePath = "";
      SourcePath = Main.TextBox_SourcePath.Text;
      return SourcePath;
    }
    public string GetTarGetPath()
    {
      string TarGetPath = "";
      TarGetPath = Main.TextBox_TargetPath.Text;
      return TarGetPath;
    }
    public string GetExtension()
    {
      string Extension = "";
      Extension = Main.TextBox_Extension.Text;
      return Extension;
    }
    public string GetDelay()
    {
      string Delay = "0";
      Delay = Main.ComboBox_Delay.SelectedItem.ToString();
      return Delay;
    }

  }
}
