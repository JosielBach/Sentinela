namespace Sentinela.Communication.Requests;

public class RequestChangePasswordJson
{
    public string CurrentPassword { set;  get; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
