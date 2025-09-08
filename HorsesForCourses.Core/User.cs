using System.Linq.Expressions;

namespace HorsesForCourses.Core;

public class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EmailAddress Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public AppUser() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private AppUser(string name, EmailAddress email, string passwordhash) { Name = name; Email = email; PasswordHash = passwordhash; }

    public static AppUser From(string name, string email, string pass, string confirmPass)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Must have a name");
        var EmailValidate = EmailAddress.From(email);
        if (pass != confirmPass) throw new ArgumentException("Password is not same as confirmation password");

        return new AppUser(name, EmailValidate, pass);
    }
}