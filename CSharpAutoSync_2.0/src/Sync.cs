using CSharpAutoSync;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      try
      {
        //來源路徑
        DirectoryInfo SourcePath = new DirectoryInfo(Main.Dispatcher.Invoke(new outputElement(getSourcePath)).ToString());

        //目標路徑
        DirectoryInfo TargetPath = new DirectoryInfo(Main.Dispatcher.Invoke(new outputElement(getTargetPath)).ToString());

        //指定副檔名
        string Extension = Main.Dispatcher.Invoke(new outputElement(getExtension)).ToString();

        //來源路徑如果有問題
        if (!SourcePath.Exists)
        {
          Main.Dispatcher.Invoke(new outputDelegate(InLog), "來源路徑異常。");
          Common.CatchMessage("來源路徑異常。");
          return;
        }

        //DirectoryInfo[] SourceDirs = SourcePath.GetDirectories();

        //檢查路徑目錄是否有創建
        if (!Directory.Exists(TargetPath.FullName)) Directory.CreateDirectory(TargetPath.FullName);

        //參數載入
        int countFile = 0;
        FileInfo[] SourceFiles = SourcePath.GetFiles();
        FileInfo[] TargetFiles = TargetPath.GetFiles();
        List<FileInfo> TargetFileList = new List<FileInfo>();
        List<string> ExtensionList = new List<string>();
        ExtensionList.AddRange(Extension.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

        foreach (FileInfo TargetInfo in TargetFiles) TargetFileList.Add(TargetInfo);
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

          //InputLog("已成功同步 " + countFile + " 個檔案。");

          //遞迴
          //if (copySubDirs)
          //{
          //  foreach (DirectoryInfo subdir in dirs)
          //  {
          //    string temppath = Path.Combine(Main.Dispatcher.Invoke(new outputElement(getTargetPath)).ToString()), subdir.Name);
          //    DirectoryCopy(subdir.FullName, temppath, Main.TextBox_Extension.Text, copySubDirs);
          //  }
          //}
        }
      }
      catch (Exception ex)
      {
        //ButtonSyncStatus();
        //MessageBox.Show(ex.Message, "警告");
      }
    }

    public delegate void outputDelegate(string msg);
    public void InLog(string content)
    {
      logger.Info(content);
      Main.TextBox_Log.Text += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + content + Environment.NewLine;
      Main.TextBox_Log.ScrollToEnd();
    }

    public delegate string outputElement();
    public string getSourcePath()
    {
      string SourcePath = "";
      SourcePath = Main.TextBox_SourcePath.Text;
      return SourcePath;
    }
    public string getTargetPath()
    {
      string TargetPath = "";
      TargetPath = Main.TextBox_TargetPath.Text;
      return TargetPath;
    }
    public string getExtension()
    {
      string Extension = "";
      Extension = Main.TextBox_Extension.Text;
      return Extension;
    }


  }
}
