namespace TablePlusCommandPalette.Models;

public sealed class TablePlusConnectionGroup
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public static readonly TablePlusConnectionGroup Ungrouped = new()
    {
        Id = "__ungrouped__",
        Name = "Ungrouped",
    };
}
