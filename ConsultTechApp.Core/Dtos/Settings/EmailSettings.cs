namespace ConsultTechApp.Core.Dtos.Settings;

public class EmailSettings
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public bool EnableSsl { get; set; }

    public string FromName { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;
}
