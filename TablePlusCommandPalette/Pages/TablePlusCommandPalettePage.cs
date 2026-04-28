using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TablePlusCommandPalette.Commands;
using TablePlusCommandPalette.Models;
using TablePlusCommandPalette.Services;

namespace TablePlusCommandPalette.Pages;

internal sealed partial class TablePlusCommandPalettePage : ListPage
{
    private readonly TablePlusConnectionService _service;

    public TablePlusCommandPalettePage(TablePlusConnectionService service)
    {
        _service = service;
        Icon = IconHelpers.FromRelativePath("Assets\\Square44x44Logo.scale-200.png");
        Title = "TablePlus";
        Name = "Open";
    }

    public override IListItem[] GetItems()
    {
        var result = _service.GetItems();
        if (result.HasError)
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = result.ErrorTitle,
                    Subtitle = result.ErrorDescription,
                }
            ];
        }

        return result.Items
            .Select(CreateItem)
            .OrderBy(x => x.Section)
            .ThenBy(x => x.Title)
            .ToArray();
    }

    private static ListItem CreateItem((TablePlusConnection Connection, TablePlusConnectionGroup Group) item)
    {
        var (connection, group) = item;
        var subtitle = BuildSubtitle(connection, group);

        return new ListItem(new OpenTablePlusConnectionCommand(connection))
        {
            Title = connection.Name,
            Subtitle = subtitle,
            Section = group.Name,
            Tags = [
                DriverTag(connection.Driver),
                EnvironmentTag(connection.Environment),
            ],
        };
    }

    private static string BuildSubtitle(TablePlusConnection connection, TablePlusConnectionGroup group)
    {
        var prefix = connection.IsOverSSH ? "SSH: "
            : connection.IsSocket ? "SOCKET: "
            : string.Empty;

        var location = string.IsNullOrEmpty(connection.Host) ? string.Empty : $"{prefix}{connection.Host}";

        if (string.Equals(connection.Driver, "SQLite", System.StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(connection.Database))
        {
            location = string.IsNullOrEmpty(location)
                ? connection.Database
                : $"{location} : {connection.Database}";
        }

        return $"🗂 {group.Name}  🔌 {location}".TrimEnd();
    }

    private static Tag DriverTag(string driver)
    {
        var label = string.IsNullOrEmpty(driver) ? "DB" : driver;
        return new Tag(label) { ToolTip = label };
    }

    private static Tag EnvironmentTag(string environment)
    {
        return environment.ToLowerInvariant() switch
        {
            "local" => new Tag("LOCAL")
            {
                Background = ColorHelpers.FromRgb(0, 128, 0),
                Foreground = ColorHelpers.FromRgb(255, 255, 255),
                ToolTip = "Local environment",
            },
            "staging" => new Tag("STAGING")
            {
                Background = ColorHelpers.FromRgb(255, 165, 0),
                Foreground = ColorHelpers.FromRgb(255, 255, 255),
                ToolTip = "Staging environment",
            },
            "production" => new Tag("PRODUCTION")
            {
                Background = ColorHelpers.FromArgb(255, 238, 94, 97),
                Foreground = ColorHelpers.FromRgb(255, 255, 255),
                ToolTip = "Production environment — use with caution!",
            },
            "" => new Tag("ENV")
            {
                Background = ColorHelpers.FromRgb(120, 120, 120),
                Foreground = ColorHelpers.FromRgb(255, 255, 255),
                ToolTip = "No environment set",
            },
            _ => new Tag(environment.ToUpperInvariant())
            {
                Background = ColorHelpers.FromArgb(255, 76, 161, 222),
                Foreground = ColorHelpers.FromRgb(255, 255, 255),
                ToolTip = $"{environment} environment",
            },
        };
    }
}
