namespace BeautySalon.Contracts.BindingModels;

public class MailSendInfoBM
{
    public required string MailAddress { get; set; }
    public required string Subject { get; set; }
    public required string Text { get; set; }
}