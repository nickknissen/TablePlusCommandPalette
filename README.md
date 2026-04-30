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

## Releasing

A new release is cut by triggering the `Build Store Bundle` GitHub Actions
workflow. It builds x64 + ARM64 MSIX, combines them into a single
`.msixbundle`, tags the commit, and creates a GitHub Release with all
three files attached.

```powershell
gh workflow run store-bundle.yml --repo nickknissen/TablePlusCommandPalette `
  -f version=1.0.1 `
  -f release_notes="One-line summary of what changed in this release."
```

When the run finishes:

1. The release appears at
   `https://github.com/nickknissen/TablePlusCommandPalette/releases/tag/<version>`.
2. Download `TablePlusCommandPalette_<version>.0_Bundle.msixbundle` from
   the release page (or `gh release download <version> --pattern *.msixbundle`).
3. In [Partner Center](https://partner.microsoft.com/dashboard/home), create a
   new submission for the app, upload the `.msixbundle` under **Packages**,
   fill in any updated listing copy, and submit.
4. Microsoft re-signs the package during ingestion. Once it certifies and
   publishes (typically 24–72h), the new version is live in the Store.

The MSIX files are unsigned at build time — the Store re-signs during
ingestion, so no code-signing cert is required.

### Local build for testing

If you want to build the bundle locally without going through the workflow:

```powershell
dotnet build .\TablePlusCommandPalette\TablePlusCommandPalette.csproj `
  -c Release -p:GenerateAppxPackageOnBuild=true `
  -p:Platform=x64 -p:RuntimeIdentifier=win-x64 -p:SelfContained=true `
  -p:AppxPackageDir=AppPackages\x64\
```

(Repeat with `Platform=ARM64` and `RuntimeIdentifier=win-arm64`, then bundle
with `makeappx bundle /f bundle_mapping.txt /p ...`.)

## Project structure

```text
TablePlusCommandPalette/
├─ Commands/      # Command Palette invokable commands
├─ Models/        # TablePlus connection / group models
├─ Pages/         # Command Palette list pages
├─ Services/      # plist parsing and connection lookup
└─ Assets/        # App and extension icons
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
