using System;
using System.Collections.Generic;

namespace TablePlusCommandPalette.Models;

public enum TablePlusErrorKind
{
    None,
    DataDirectoryMissing,
    NoConnectionsFound,
    ParseFailed,
    Unknown,
}

public sealed class TablePlusQueryResult
{
    public IReadOnlyList<(TablePlusConnection Connection, TablePlusConnectionGroup Group)> Items { get; init; }
        = Array.Empty<(TablePlusConnection, TablePlusConnectionGroup)>();

    public TablePlusErrorKind ErrorKind { get; init; }
    public string ErrorTitle { get; init; } = string.Empty;
    public string ErrorDescription { get; init; } = string.Empty;

    public bool HasError => ErrorKind != TablePlusErrorKind.None;
}
