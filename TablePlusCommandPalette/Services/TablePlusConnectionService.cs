using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Claunia.PropertyList;
using TablePlusCommandPalette.Models;

namespace TablePlusCommandPalette.Services;

public sealed class TablePlusConnectionService
{
    private readonly string _connectionsPath;
    private readonly string _groupsPath;

#pragma warning disable CS0649, CS0169 // Unused under DEMO_MODE.
    private readonly object _cacheLock = new();
    private TablePlusQueryResult? _cachedResult;
    private DateTime _connectionsLastWrite = DateTime.MinValue;
    private DateTime _groupsLastWrite = DateTime.MinValue;
#pragma warning restore CS0649, CS0169

    public TablePlusConnectionService()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dataDir = Path.Combine(localAppData, "com.tinyapp.TablePlus", "data");
        _connectionsPath = Path.Combine(dataDir, "Connections.plist");
        _groupsPath = Path.Combine(dataDir, "ConnectionGroups.plist");

        // Warm up the cache so the first GetItems() doesn't block on disk I/O.
        Task.Run(() =>
        {
            try { _ = GetItems(); } catch { }
        });
    }

#pragma warning disable CA1822 // GetItems is static under DEMO_MODE — keep instance signature for the live path.
    public TablePlusQueryResult GetItems()
#pragma warning restore CA1822
    {
#if DEMO_MODE
        return DemoTablePlusData.Result();
#else
        var connectionsTimestamp = GetLastWriteTimeUtcSafe(_connectionsPath);
        var groupsTimestamp = GetLastWriteTimeUtcSafe(_groupsPath);

        lock (_cacheLock)
        {
            if (_cachedResult is not null
                && connectionsTimestamp == _connectionsLastWrite
                && groupsTimestamp == _groupsLastWrite)
            {
                return _cachedResult;
            }
        }

        var result = LoadFromDisk();

        lock (_cacheLock)
        {
            _cachedResult = result;
            _connectionsLastWrite = connectionsTimestamp;
            _groupsLastWrite = groupsTimestamp;
            return _cachedResult;
        }
#endif
    }

    private TablePlusQueryResult LoadFromDisk()
    {
        if (!File.Exists(_connectionsPath))
        {
            return new TablePlusQueryResult
            {
                ErrorKind = TablePlusErrorKind.DataDirectoryMissing,
                ErrorTitle = "TablePlus connections file not found",
                ErrorDescription = $"Expected: {_connectionsPath}",
            };
        }

        List<TablePlusConnection> connections;
        List<TablePlusConnectionGroup> groups;
        try
        {
            connections = LoadConnections(_connectionsPath);
            groups = File.Exists(_groupsPath)
                ? LoadGroups(_groupsPath)
                : new List<TablePlusConnectionGroup>();
        }
        catch (Exception ex)
        {
            return new TablePlusQueryResult
            {
                ErrorKind = TablePlusErrorKind.ParseFailed,
                ErrorTitle = "Couldn't read TablePlus connections",
                ErrorDescription = ex.Message,
            };
        }

        if (connections.Count == 0)
        {
            return new TablePlusQueryResult
            {
                ErrorKind = TablePlusErrorKind.NoConnectionsFound,
                ErrorTitle = "No TablePlus connections found",
                ErrorDescription = "Add a connection in TablePlus to see it here.",
            };
        }

        var groupMap = groups
            .Where(g => !string.IsNullOrEmpty(g.Id))
            .GroupBy(g => g.Id)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);

        var items = connections
            .Select(c => (c, ResolveGroup(c, groupMap)))
            .ToArray();

        return new TablePlusQueryResult { Items = items };
    }

    private static TablePlusConnectionGroup ResolveGroup(
        TablePlusConnection connection,
        Dictionary<string, TablePlusConnectionGroup> groupMap)
    {
        if (!string.IsNullOrEmpty(connection.GroupId)
            && groupMap.TryGetValue(connection.GroupId, out var group))
        {
            return group;
        }

        return TablePlusConnectionGroup.Ungrouped;
    }

    private static List<TablePlusConnection> LoadConnections(string path)
    {
        var result = new List<TablePlusConnection>();
        if (PropertyListParser.Parse(path) is not NSArray array)
        {
            return result;
        }

        foreach (var item in array)
        {
            if (item is NSDictionary dict)
            {
                var connection = ParseConnection(dict);
                if (connection is not null)
                {
                    result.Add(connection);
                }
            }
        }

        return result;
    }

    private static List<TablePlusConnectionGroup> LoadGroups(string path)
    {
        var result = new List<TablePlusConnectionGroup>();
        if (PropertyListParser.Parse(path) is not NSArray array)
        {
            return result;
        }

        foreach (var item in array)
        {
            if (item is NSDictionary dict)
            {
                result.Add(new TablePlusConnectionGroup
                {
                    Id = GetString(dict, "ID"),
                    Name = GetString(dict, "Name"),
                });
            }
        }

        return result;
    }

    private static TablePlusConnection? ParseConnection(NSDictionary dict)
    {
        var connection = new TablePlusConnection
        {
            Id = GetString(dict, "ID"),
            Name = GetString(dict, "ConnectionName"),
            Driver = GetString(dict, "Driver"),
            // Note: TablePlus persists this misspelled as "Enviroment".
            Environment = GetString(dict, "Enviroment"),
            GroupId = GetString(dict, "GroupID"),
            Host = GetString(dict, "DatabaseHost"),
            Database = GetString(dict, "DatabaseName"),
            IsOverSSH = GetBool(dict, "isOverSSH"),
            IsSocket = GetBool(dict, "isSocket"),
        };

        return string.IsNullOrEmpty(connection.Id) ? null : connection;
    }

    private static string GetString(NSDictionary dict, string key)
        => dict.ContainsKey(key) ? dict[key]?.ToString() ?? string.Empty : string.Empty;

    private static bool GetBool(NSDictionary dict, string key)
        => dict.ContainsKey(key) && dict[key] is NSNumber n && n.ToBool();

    private static DateTime GetLastWriteTimeUtcSafe(string path)
    {
        try
        {
            return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}
