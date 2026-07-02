namespace Portfolio.Domain.Admins;

public class Admin
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    protected Admin() { }

    public Admin(string username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        Id = Guid.NewGuid();
        Username = username;
        PasswordHash = passwordHash;
    }
}
