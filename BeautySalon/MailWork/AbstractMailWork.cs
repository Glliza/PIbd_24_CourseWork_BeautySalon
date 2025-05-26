using Microsoft.Extensions.Logging;
using BeautySalon.Contracts.BindingModels;
using System.Threading.Tasks;

namespace BeautySalon.MailWork
{
    public abstract class AbstractMailWork
    {
        protected readonly ILogger<AbstractMailWork> _logger;
        protected string _smtpClientHost = "your_smtp_host";  // Replace with your SMTP host
        protected int _smtpClientPort = 587; // Replace with your SMTP port
        protected string _mailLogin = "your_email@example.com"; // Replace with your email
        protected string _mailPassword = "your_password"; // Replace with your password

        public AbstractMailWork(ILogger<AbstractMailWork> logger)
        {
            _logger = logger;
        }

        public async Task MailSendAsync(MailSendInfoBM info)
        {
            try
            {
                _logger.LogInformation($"Attempting to send email to: {info.MailAddress}");
                await SendMailAsync(info);
                _logger.LogInformation($"Email sent successfully to: {info.MailAddress}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to: {info.MailAddress}");
                throw; // Re-throw the exception to be handled upstream
            }
        }

        protected abstract Task SendMailAsync(MailSendInfoBM info);
    }
}