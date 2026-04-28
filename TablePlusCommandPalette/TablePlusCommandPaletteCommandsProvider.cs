// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TablePlusCommandPalette.Pages;
using TablePlusCommandPalette.Services;

namespace TablePlusCommandPalette;

public partial class TablePlusCommandPaletteCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public TablePlusCommandPaletteCommandsProvider()
    {
        DisplayName = "TablePlus";
        Icon = IconHelpers.FromRelativePath("Assets\\Square44x44Logo.scale-200.png");

        var service = new TablePlusConnectionService();

        _commands = [
            new CommandItem(new TablePlusCommandPalettePage(service)) { Title = DisplayName },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }
}
