using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace OnlineShop.Services
{
    public class MailService
    {
        private string gmail_account = "gary60430";
        private string gmail_password = "ggg1992516";
        private string gmail_mail = "gary60430@gmail.com";

        #region 取得亂數驗證碼
        public string GetValidateCode()
        {
            string[] code = new string[] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "1","2","3","4","5","6","7","8","9",
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};

            string validateCode = string.Empty;

            Random rd = new Random();

            for (int i = 0; i < 10; i++)
            {
                validateCode += code[rd.Next(code.Count())];
            }

            return validateCode;
        }
        #endregion

        #region 設定驗證信範本
        public string GetRegisterMailBody(string pTempString, string pUserName, string pValidateUrl)
        {
            pTempString = pTempString.Replace("{{UserName}}", pUserName);
            pTempString = pTempString.Replace("{{ValidateUrl}}", pValidateUrl);

            return pTempString;
        }
        #endregion

        #region 寄信方法
        public void SendRegisterMail(string pMailBody, string pToEmail)
        {
            //smtp物件
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            smtpServer.EnableSsl = true;
            //信件內容物件
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(pToEmail);
            mail.Subject = "會員註冊確認信";
            mail.Body = pMailBody;
            mail.IsBodyHtml = true;

            smtpServer.Send(mail);

        }
        #endregion
    }
}