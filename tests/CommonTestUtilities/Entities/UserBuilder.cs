using Bogus;
using CommonTestUtilities.Security;
using Sentinela.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class UserBuilder
{
    public static (User user, string password) Build()
    {
        var (password, hashedPassword) = GenerateRandomPassword();

        var user = new Faker<User>()
            .RuleFor(u => u.Name, f => f.Person.FirstName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, _ => hashedPassword);

        return (user, password);
    }

    private static (string password, string hashedPassword) GenerateRandomPassword()
    {
        var passwordEncripter = new IPasswordHasherBuilder().Build();

        var password = new Faker().Internet.Password();

        return (password, passwordEncripter.HashPassword(password));
    }
}
