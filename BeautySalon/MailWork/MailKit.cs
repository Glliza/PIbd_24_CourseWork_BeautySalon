using BeautySalon.Contracts.BindingModels;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace BeautySalon.MailWork;

public class MailKit : AbstractMailWork
{
    public MailKit(ILogger<MailKit> logger) : base(logger) { }

    protected override async Task SendMailAsync(MailSendInfoBM info)
    {
        using var objMailMessage = new MailMessage();
        using var objSmtpClient = new SmtpClient(_smtpClientHost, _smtpClientPort);
        try
        {
            objMailMessage.From = new MailAddress(_mailLogin);
            objMailMessage.To.Add(new MailAddress(info.MailAddress));
            objMailMessage.Subject = info.Subject;
            objMailMessage.Body = info.Text;
            objMailMessage.SubjectEncoding = Encoding.UTF8;
            objMailMessage.BodyEncoding = Encoding.UTF8;
            Attachment attachment = new Attachment("C:\\Reports\\pdffile.txt", new ContentType(MediaTypeNames.Text.Plain)); // change report to TXT
            objMailMessage.Attachments.Add(attachment);
            objSmtpClient.UseDefaultCredentials = false;
            objSmtpClient.EnableSsl = true;
            objSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            objSmtpClient.Credentials = new NetworkCredential(_mailLogin, _mailPassword);
            await Task.Run(() => objSmtpClient.Send(objMailMessage));
        }
        catch (Exception)
        {
            throw;
        }
    }
}
