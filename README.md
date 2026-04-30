# TablePlus Command Palette

A PowerToys Command Palette extension that surfaces your TablePlus database
connections so you can launch them directly from CmdPal.

## Features

- Browse all TablePlus connections grouped by **connection group**
- Visual environment tags (Local / Staging / Production)
- Driver tag (PostgreSQL, MySQL, SQLite, …)
- Open a connection directly in TablePlus by selecting it
- Reads connections from the local TablePlus data files — no extra config

## How it works

This extension reads `Connections.plist` and `ConnectionGroups.plist` from
TablePlus' local data directory and renders each connection as a Command
Palette item that launches `tableplus://?id=<connection-id>` when invoked.

## Requirements

- **Windows 10 2004+** or **Windows 11**
- **TablePlus** installed
- Microsoft PowerToys with **Command Palette** support

## Installation

### Option 1: Microsoft Store (recommended)

[![Get from Microsoft Store](https://img.shields.io/badge/Microsoft%20Store-Get-blue?logo=microsoft-store)](https://apps.microsoft.com/detail/9P4L93228V63)

Open the listing: <https://apps.microsoft.com/detail/9P4L93228V63>

### Option 2: WinGet (Microsoft Store source)

```powershell
winget install --source msstore 9P4L93228V63
```

### Build from source

```powershell
dotnet restore
dotnet build TablePlusCommandPalette.sln -c Release -p:Platform=x64
```

See [Development](#development) below for the full build / sideload flow.

## Usage

After installation, open **PowerToys Command Palette** and look for the
**TablePlus** provider. Type to search across your connections, then press
<kbd>Enter</kbd> to open the selected connection in TablePlus.

## Development

### Build from source

```powershell
dotnet restore
dotnet build TablePlusCommandPalette.sln
```

### Publish a self-contained build

Example for x64:

```powershell
dotnet publish .\TablePlusCommandPalette\TablePlusCommandPalette.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  /p:WindowsPackageType=None
```

## Packaging

Release MSIX packages are built via `build-msix.ps1`:

```powershell
.\TablePlusCommandPalette\build-msix.ps1 -Version 1.0.0 -Platforms @('x64','arm64') -Bundle
```

Notes:

- Builds MSIX packages for **x64** and **arm64** by default
- Requires the Windows 10/11 SDK installed locally

## Project structure

```text
TablePlusCommandPalette/
├─ Commands/      # Command Palette invokable commands
├─ Models/        # TablePlus connection / group models
├─ Pages/         # Command Palette list pages
├─ Services/      # plist parsing and connection lookup
├─ Assets/        # App and extension icons
└─ build-msix.ps1 # Release MSIX build script
```

## Troubleshooting

### The extension shows "No TablePlus connections found"

- Make sure TablePlus is installed and you have at least one saved connection.
- The extension reads from
  `%LocalAppData%\com.tinyapp.TablePlus\data\Connections.plist`. If TablePlus
  stores its data elsewhere on your machine, this extension will not find it.

### Selecting a connection does nothing

The extension launches `tableplus://?id=<id>`. Confirm the URI handler works:

```powershell
start "tableplus://?id=<some-id>"
```

If TablePlus does not open, reinstall TablePlus so the URI handler is
registered.

## License

This project is licensed under the [MIT License](LICENSE).

## Disclaimer

This project is an independent extension for Microsoft PowerToys and is not
affiliated with or endorsed by TablePlus or Microsoft.
