using System.Net.Mail;

namespace Identity.Helper
{
    public static class EmailConfirmation
    {
        public static void EmailСonfirmSend(string link, string email)
        {
            MailMessage mail = new MailMessage();

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;

            mail.From = new MailAddress("mistermrx99@gmail.com");
            mail.To.Add(email);
            mail.Subject = $"www.usman.kg: Э-почтаны тастыктоо";
            mail.Body = "<h2>Э-почтаны тастыктоо учун томонку шилтемеге басыныныз</h2><hr/>";
            mail.Body += $"<a href='{link}'>Э-почтаны тастыктоо</a>";
            mail.IsBodyHtml = true;

            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("mistermrx99@gmail.com", "akmaral_love");
            smtpClient.Send(mail);
        }
    }
}
