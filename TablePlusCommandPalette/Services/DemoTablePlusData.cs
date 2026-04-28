using TablePlusCommandPalette.Models;

namespace TablePlusCommandPalette.Services;

/// <summary>
/// Hard-coded fake connections used when the project is built with the
/// DEMO_MODE define. Used for Microsoft Store screenshots — never ships
/// to end users.
/// </summary>
internal static class DemoTablePlusData
{
    private static readonly TablePlusConnectionGroup Personal = new() { Id = "g-personal", Name = "Personal" };
    private static readonly TablePlusConnectionGroup Work = new() { Id = "g-work", Name = "Work" };
    private static readonly TablePlusConnectionGroup Sandbox = new() { Id = "g-sandbox", Name = "Sandbox" };

    public static TablePlusQueryResult Result() => new()
    {
        Items = new (TablePlusConnection, TablePlusConnectionGroup)[]
        {
            (Conn("c1",  "Blog (local)",       "PostgreSQL", "local",       "127.0.0.1"),                        Personal),
            (Conn("c2",  "Notes",              "SQLite",     "local",       "",            db: "notes.sqlite"),  Personal),
            (Conn("c3",  "Recipes Cache",      "Redis",      "local",       "127.0.0.1"),                        Personal),

            (Conn("c4",  "Acme API – staging", "PostgreSQL", "staging",     "db.staging.example"),               Work),
            (Conn("c5",  "Acme API – prod",    "PostgreSQL", "production",  "db.prod.example", overSsh: true),   Work),
            (Conn("c6",  "Acme Analytics",     "MySQL",      "production",  "analytics.example", overSsh: true), Work),
            (Conn("c7",  "Acme Sessions",      "Redis",      "production",  "redis.example"),                    Work),
            (Conn("c8",  "Acme Search",        "MariaDB",    "staging",     "search.staging.example"),           Work),

            (Conn("c9",  "Demo Postgres",      "PostgreSQL", "local",       "127.0.0.1"),                        Sandbox),
            (Conn("c10", "Demo Mysql Socket",  "MySQL",      "local",       "/tmp/mysql.sock", socket: true),    Sandbox),
            (Conn("c11", "Demo SQL Server",    "MSSQL",      "staging",     "mssql.staging.example"),            Sandbox),
        },
    };

    private static TablePlusConnection Conn(
        string id,
        string name,
        string driver,
        string environment,
        string host,
        string db = "",
        bool overSsh = false,
        bool socket = false)
        => new()
        {
            Id = id,
            Name = name,
            Driver = driver,
            Environment = environment,
            GroupId = string.Empty,
            Host = host,
            Database = db,
            IsOverSSH = overSsh,
            IsSocket = socket,
        };
}
