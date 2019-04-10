namespace ProjectNameTemplate.Constant
{
    /// <summary>
    /// 配置
    /// </summary>
    public class AppSetting
    {
        public static string RedisConnection => ConfigurationManager.GetConfig("RedisConnection");
        public static string LogMailFrom => ConfigurationManager.GetConfig("LogMail:MailFrom");

        public static string LogMailPwd => ConfigurationManager.GetConfig("LogMail:MailPwd");

        public static string LogMailHost => ConfigurationManager.GetConfig("LogMail:MailHost");
        public static string LogMailTo => ConfigurationManager.GetConfig("LogMail:MailTo");
        public static string LogMailLevel => ConfigurationManager.GetConfig("LogMail:LogLevel");
    }
}
