namespace TablePlusCommandPalette.Models;

public sealed class TablePlusConnection
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Driver { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public bool IsOverSSH { get; set; }
    public bool IsSocket { get; set; }

    public string DatabaseUrl => $"tableplus://?id={Id}";
}
