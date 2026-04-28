using System;
using System.Diagnostics;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TablePlusCommandPalette.Models;

namespace TablePlusCommandPalette.Commands;

internal sealed partial class OpenTablePlusConnectionCommand : InvokableCommand
{
    private readonly TablePlusConnection _connection;

    public OpenTablePlusConnectionCommand(TablePlusConnection connection)
    {
        _connection = connection;
        Icon = IconHelpers.FromRelativePath("Assets\\Square44x44Logo.scale-200.png");
    }

    public override string Name => $"Open {_connection.Name}";

    public override CommandResult Invoke()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _connection.DatabaseUrl,
                UseShellExecute = true,
                CreateNoWindow = true,
            });

            return CommandResult.Dismiss();
        }
        catch (Exception ex)
        {
            return CommandResult.ShowToast(new ToastArgs
            {
                Message = $"Couldn't open TablePlus: {ex.Message}",
                Result = CommandResult.KeepOpen(),
            });
        }
    }
}
