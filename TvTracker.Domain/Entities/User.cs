namespace TvTracker.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = ""; // guarda hash, não a password
    public List<Favorite> Favorites { get; set; } = new();
}
