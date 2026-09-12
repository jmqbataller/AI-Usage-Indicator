namespace UsageIndicator.Models;

public sealed class AccountProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "Account";
    public UsageSnapshot Usage { get; set; } = new();

    public override string ToString() => Name;
}
