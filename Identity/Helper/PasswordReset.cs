using System.Net.Mail;

namespace Identity.Helper
{
    public static class PasswordReset
    {
        public static void PasswordResetSendEmail(string link)
        {
            MailMessage mail = new MailMessage();

            SmtpClient smtpClient = new SmtpClient("mail.teknohub.net");
            mail.From = new MailAddress("mtolkunbek@it.salyk.kg");
            mail.To.Add("melis99mrx@gmail.com");
            mail.Subject = $"www.usman.kg: Сыр созду жанылоо";
            mail.Body = "<h2>Сыр созду жанылоо учун томонку шилтемеге басыныныз</h2><hr/>";
            mail.Body += $"<a href='{link}'>Сыр соз жанылоо</a>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("mtolkunbek@it.salyk.kg", "4rfvBGT%");
            smtpClient.Send(mail);
        }
    }
}