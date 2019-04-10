using MimeKit;

namespace ProjectNameTemplate.Common.Helper
{
    public class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        public static void SendMail(SendMailModel model)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(model.MailInfo.MailFrom, model.MailInfo.MailFrom));
                foreach (var mailTo in model.MailInfo.MailTo.Replace("；", ";").Replace("，", ";").Replace(",", ";").Split(';'))
                {
                    message.To.Add(new MailboxAddress(mailTo, mailTo));
                }
                message.Subject = string.Format(model.Title);
                message.Body = new TextPart("html")
                {
                    Text = model.Content
                };
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(model.MailInfo.MailHost, 465, true);
                    client.Authenticate(model.MailInfo.MailFrom, model.MailInfo.MailPwd);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (System.Exception)
            {
            }
        }
    }

    public class SendMailModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public MailEntity MailInfo { get; set; } = new MailEntity();
    }

    public class MailEntity
    {
        /// <summary>
        /// 发件邮箱
        /// </summary>
        public string MailFrom { get; set; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string MailPwd { get; set; }
        /// <summary>
        /// 发件服务器
        /// </summary>
        public string MailHost { get; set; }
        /// <summary>
        /// 收件邮箱
        /// </summary>
        public string MailTo { get; set; }
    }
}
