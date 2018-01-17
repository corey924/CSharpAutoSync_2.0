using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAutoSync_2._0.src
{
  class Common
  {
    private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

    public void CatchMessage(string content)
    {
      try
      {
        //MailMessage(寄信者, 收信者)
        //System.Net.Mail.MailMessage errorMessage = new System.Net.Mail.MailMessage("corey_924@yahoo.com.tw", "corey_924@yahoo.com.tw");

        ////信件支援HTML
        //errorMessage.IsBodyHtml = true;
        ////E-mail編碼
        //errorMessage.BodyEncoding = System.Text.Encoding.UTF8;
        ////E-mail主旨
        //errorMessage.Subject = "【志登科技自動列印程序】錯誤訊息通知";
        ////優先權
        //errorMessage.Priority = System.Net.Mail.MailPriority.Normal;
        ////設定SMTP
        //System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("192.168.1.229", 25);
        ////E-mail內容
        //errorMessage.Body += "發生時間：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "<br/>";
        //errorMessage.Body += "錯誤內容：" + content + "<br/>";
        ////寄出信件
        //smtpClient.Send(errorMessage);

        logger.Error(content);
      }
      catch (Exception ex)
      {
        logger.Error(ex.Message);
      }
    }
  }
}
