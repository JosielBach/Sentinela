namespace WebApi.Tests.Resources;

public class UserIdentityManager
{
    private readonly Sentinela.Domain.Entities.User _user;
    private readonly string _password;

    public UserIdentityManager(Sentinela.Domain.Entities.User user, string password)
    {
        _user = user;
        _password = password;
    }

    public Guid GetGuId() => _user.Id;
    public string GetEmail() => _user.Email;
    public string GetName() => _user.Name;
    public string GetPassword() => _password;
}
